using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using LineNotifySDK.Model;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Refit;

namespace LineNotifySDK
{
    public class LineNotifyServices : ILineNotifyServices
    {
        private readonly IOptionsMonitor<LineNotifyOptions> _options;
        private readonly IValidator<LineNotifyMessage> _validator;
        private readonly ILineNotifyClient _notifyApiClient;
        private readonly HttpClient _notifyBotClient;

        public LineNotifyServices(IHttpClientFactory httpClientFactory, IOptionsMonitor<LineNotifyOptions> options, IValidator<LineNotifyMessage> validator)
        {
            _options = options;
            _validator = validator;
            _notifyBotClient = httpClientFactory.CreateClient("notifyBotClient");
            _notifyApiClient = RestService.For<ILineNotifyClient>(httpClientFactory.CreateClient("notifyApiClient"));
        }

        public Uri GetAuthorizeUri(string state = null) =>
            new Uri(QueryHelpers.AddQueryString("https://notify-bot.line.me/oauth/authorize",
                JsonSerializer.Deserialize<Dictionary<string, string>>(
                    JsonSerializer.Serialize(
                        new
                        {
                            response_type = _options.CurrentValue.ResponseType,
                            scope = _options.CurrentValue.Scope,
                            redirect_uri = _options.CurrentValue.RedirectUri,
                            client_id = _options.CurrentValue.ClientId,
                            state = !string.IsNullOrWhiteSpace(state) ? state : Guid.NewGuid().ToString("N")
                        }))));

        public async Task<string> GetTokenAsync(string code, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(code));
            }

            var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(
                JsonSerializer.Serialize(
                    new
                    {
                        grant_type = _options.CurrentValue.GrantType,
                        code,
                        redirect_uri = _options.CurrentValue.RedirectUri,
                        client_id = _options.CurrentValue.ClientId,
                        client_secret = _options.CurrentValue.ClientSecret
                    }));
            var response = await _notifyBotClient.PostAsync("/oauth/token",
                new FormUrlEncodedContent(dictionary), cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var message = JsonDocument.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false))
                    .RootElement
                    .EnumerateObject()
                    .FirstOrDefault(c => c.Name.Equals("message", StringComparison.OrdinalIgnoreCase)).Value.ToString();
                throw new LineNotifyException(message);
            }

            var content = JsonDocument.Parse(
                await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            return content.RootElement.GetProperty("access_token").GetString();
        }

        public async Task SentAsync(string token, LineNotifyMessage message, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(token));
            }

            await _validator.ValidateAndThrowAsync(message, cancellationToken).ConfigureAwait(false);
            var response = await _notifyApiClient.Sent(
                token,
                message.Message,
                message.ImageThumbnail,
                message.ImageFullSize,
                message.ImageFile,
                message.StickerPackageId,
                message.StickerId,
                message.NotificationDisabled,
                cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode && response.Error != null)
            {
                ThrowLineNotifyException(response.Error);
            }
        }

        public async Task<LineNotifyStatus> StatusAsync(string token, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(token));
            }

            var response = await _notifyApiClient.Status(token, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode && response.Error != null)
            {
                ThrowLineNotifyException(response.Error);
            }

            return response.Content;
        }

        public async Task RevokeAsync(string token, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(token));
            }

            var response = await _notifyApiClient.Revoke(token, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode && response.Error != null)
            {
                ThrowLineNotifyException(response.Error);
            }
        }

        private static void ThrowLineNotifyException(ApiException exception)
        {
            var message = string.IsNullOrWhiteSpace(exception.Content)
                ? string.Empty
                : JsonDocument.Parse(exception.Content)
                    .RootElement
                    .EnumerateObject()
                    .FirstOrDefault(c => c.Name.Equals("message", StringComparison.OrdinalIgnoreCase)).Value.ToString();
            throw new LineNotifyException(message, exception);
        }
    }
}