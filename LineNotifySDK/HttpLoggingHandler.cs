using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;

namespace LineNotifySDK
{
    internal class HttpLoggingHandler : DelegatingHandler
    {
        private readonly string[] _types = { "html", "text", "xml", "json", "txt", "x-www-form-urlencoded" };
        private readonly ILog _logger;

        public HttpLoggingHandler()
        {
            if (InnerHandler is null)
            {
                InnerHandler = new HttpClientHandler();
            }
            _logger = LogManager.GetLogger(GetType());
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid().ToString();
            var msg = $"[{id} - Request]";

            _logger.Debug($"{msg} ==========Start==========");
            _logger.Debug($"{msg} {request.Method} {request.RequestUri.PathAndQuery} {request.RequestUri.Scheme}/{request.Version}");
            _logger.Debug($"{msg} Host: {request.RequestUri.Scheme}://{request.RequestUri.Host}");

            foreach (var header in request.Headers)
            {
                _logger.Debug($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
            }

            if (request.Content != null)
            {
                foreach (var header in request.Content.Headers)
                {
                    _logger.Debug($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
                }

                if (request.Content is StringContent
                    || IsTextBasedContentType(request.Headers)
                    || IsTextBasedContentType(request.Content.Headers))
                {
                    var result = await request.Content.ReadAsStringAsync().ConfigureAwait(false);

                    _logger.Debug($"{msg} Content:");
                    _logger.Debug($"{msg} {result}");
                }
            }
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            stopWatch.Stop();

            _logger.Debug($"{msg} Duration: {stopWatch.Elapsed.Milliseconds} ms");
            _logger.Debug($"{msg} ==========End==========");

            // Response
            msg = $"[{id} - Response]";
            _logger.Debug($"{msg} =========Start=========");
            _logger.Debug($"{msg} {request.RequestUri.Scheme.ToUpper()}/{response.Version} {(int)response.StatusCode} {response.ReasonPhrase}");

            foreach (var header in response.Headers)
            {
                _logger.Debug($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
            }

            if (response.Content != null)
            {
                foreach (var header in response.Content.Headers)
                {
                    _logger.Debug($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
                }

                if (response.Content is StringContent
                    || IsTextBasedContentType(response.Headers)
                    || IsTextBasedContentType(response.Content.Headers))
                {
                    var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    _logger.Debug($"{msg} Content:");
                    _logger.Debug($"{msg} {result}");
                }
            }
            _logger.Debug($"{msg} ==========End==========");
            return response;
        }

        private bool IsTextBasedContentType(HttpHeaders headers)
        {
            if (!headers.TryGetValues("Content-Type", out var values))
            {
                return false;
            }

            var header = string.Join(" ", values).ToLowerInvariant();
            return _types.Any(t => header.Contains(t));
        }
    }
}