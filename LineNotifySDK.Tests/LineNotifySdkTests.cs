using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LineNotifySDK.Model;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using Xunit;

namespace LineNotifySDK.Tests
{
    public class LineNotifySdkTests
    {
        private readonly ILineNotifyServices _lineNotify;
        private readonly LineNotifyOptions _options;
        private readonly MockHttpMessageHandler _mockHttp;

        public LineNotifySdkTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Development"}.json", true)
                .Build();
            var services = new ServiceCollection();
            services.AddSingleton(configuration);
            services.AddLogging(logging =>
            {
                logging.AddConfiguration(configuration.GetSection("Logging"));
                logging.AddDebug();
            });
            if (configuration.GetValue("HttpClient:Mock", true))
            {
                _mockHttp = new MockHttpMessageHandler();
                services.AddSingleton(_mockHttp);
                var mockFactory = new Mock<IHttpClientFactory>();
                mockFactory.Setup(_ => _.CreateClient("notifyBotClient")).Returns(new HttpClient(_mockHttp){BaseAddress = new Uri("https://notify-bot.line.me") });
                mockFactory.Setup(_ => _.CreateClient("notifyApiClient")).Returns(new HttpClient(_mockHttp){BaseAddress = new Uri("https://notify-api.line.me") });
                services.AddSingleton(x => mockFactory.Object);
            }
            services.AddLineNotifyServices((_, options) =>
            {
                options.ClientId = configuration.GetValue("LineNotifyOptions:ClientId", "");
                options.ClientSecret = configuration.GetValue("LineNotifyOptions:ClientSecret", "");
                options.RedirectUri = configuration.GetValue("LineNotifyOptions:RedirectUri", "");
            });
            var serviceProvider = services.BuildServiceProvider();
            _lineNotify = serviceProvider.GetRequiredService<ILineNotifyServices>();
            _options = serviceProvider.GetRequiredService<IOptions<LineNotifyOptions>>().Value;
        }

        [Fact]
        public void GetAuthGetAuthorizeUrl()
        {
            // Arrange
            var uri = _lineNotify.GetAuthorizeUri();
            var query = QueryHelpers.ParseQuery(uri.Query);

            static string SnakeCase(string target)
            {
                return string.Concat(target.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower();
            }

            Assert.Equal(_options.ClientId, query[SnakeCase(nameof(_options.ClientId))]);
            Assert.Equal(_options.RedirectUri, query[SnakeCase(nameof(_options.RedirectUri))]);
        }

        [Fact]
        public async Task GetToken()
        {
            _mockHttp?.Expect(HttpMethod.Post, "https://notify-bot.line.me/oauth/token")
                .Respond(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"access_token\":\"token\"}") });
            Assert.Equal("token", await _lineNotify.GetTokenAsync("token").ConfigureAwait(false));
        }

        [Fact]
        public async Task SentMessage()
        {
            var str = JsonSerializer.Serialize(new LineMessageResponse { Status = 200, Message = "ok" });
            var result = new HttpResponseMessage { Content = new StringContent(str, Encoding.UTF8, "application/json") };
            _mockHttp?.Expect(HttpMethod.Post, "https://notify-api.line.me/api/notify")
                .Respond(_ => result);
            await _lineNotify.SentAsync("token", new LineNotifyMessage { Message = "test" }).ConfigureAwait(false);
        }
    }
}
