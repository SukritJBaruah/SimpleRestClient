using Polly;
using Polly.Contrib.WaitAndRetry;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Simple.RestClient
{
    internal class SystemHttpClient : IRestClient
    {
        private readonly RestRequestValidator _requestValidator;
        private readonly int _requestTimeout;
        private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy =
            Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(x => x.StatusCode >= System.Net.HttpStatusCode.InternalServerError)
                .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5));
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="RequestTimeout">In Seconds</param>
        public SystemHttpClient(int RequestTimeout = 5)
        {
            _requestTimeout = RequestTimeout;
            _requestValidator = new RestRequestValidator();
        }
        public async Task<Response> DeleteAsync(Request Request, CancellationToken ct = default)
        {
            _requestValidator.ValidateRequest(Request);

            using(var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(_requestTimeout) })
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var uri = buildUri(Request.Url);
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Delete, uri);
                
                foreach(var header in Request.Headers)
                    httpRequest.Headers.Add(header.Key, header.Value);

                using (var response = await _retryPolicy.ExecuteAsync(() => client.SendAsync(httpRequest, ct)))
                {
                    return await getResponse(response);
                }
            }
        }

        public async Task<Response> GetAsync(Request Request, CancellationToken ct = default)
        {
            _requestValidator.ValidateRequest(Request);

            using (var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(_requestTimeout) })
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var uri = buildUri(Request.Url);
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);

                foreach (var header in Request.Headers)
                    httpRequest.Headers.Add(header.Key, header.Value);

                using (var response = await _retryPolicy.ExecuteAsync(() => client.SendAsync(httpRequest, ct)))
                {
                    return await getResponse(response);
                }
            }
        }

        public async Task<Response> PatchAsync(BodyRequest Request, CancellationToken ct = default)
        {
            _requestValidator.ValidateRequest(Request);

            using (var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(_requestTimeout) })
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var uri = buildUri(Request.Url);
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Patch, uri);

                foreach (var header in Request.Headers)
                    httpRequest.Headers.Add(header.Key, header.Value);

                httpRequest.Content = new StringContent(Request.Payload, Encoding.UTF8, "application/json");

                using (var response = await _retryPolicy.ExecuteAsync(() => client.SendAsync(httpRequest, ct)))
                {
                    return await getResponse(response);
                }
            }
        }

        public async Task<Response> PostAsync(BodyRequest Request, CancellationToken ct = default)
        {
            _requestValidator.ValidateRequest(Request);

            using (var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(_requestTimeout) })
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var uri = buildUri(Request.Url);
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);

                foreach (var header in Request.Headers)
                    httpRequest.Headers.Add(header.Key, header.Value);

                httpRequest.Content = new StringContent(Request.Payload, Encoding.UTF8, "application/json");

                using (var response = await _retryPolicy.ExecuteAsync(() => client.SendAsync(httpRequest, ct)))
                {
                    return await getResponse(response);
                }
            }
        }

        public async Task<Response> PutAsync(BodyRequest Request, CancellationToken ct = default)
        {
            _requestValidator.ValidateRequest(Request);

            using (var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(_requestTimeout) })
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var uri = buildUri(Request.Url);
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Put, uri);

                foreach (var header in Request.Headers)
                    httpRequest.Headers.Add(header.Key, header.Value);

                httpRequest.Content = new StringContent(Request.Payload, Encoding.UTF8, "application/json");

                using (var response = await _retryPolicy.ExecuteAsync(() => client.SendAsync(httpRequest, ct)))
                {
                    return await getResponse(response);
                }
            }
        }

        private Uri buildUri(Url Url)
        {
            var uriBuilder = new UriBuilder(Url.BaseUrl + Url.RelativeUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            foreach (var param in Url.QueryParams)
            {
                query[param.Key] = param.Value;
            }
            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri;
        }

        private async Task<Response> getResponse(HttpResponseMessage httpResponseMessage)
        {
            return new Response()
            {
                Code = (int)httpResponseMessage.StatusCode,
                Payload = await httpResponseMessage.Content.ReadAsStringAsync(),
                Headers = httpResponseMessage.Headers.ToDictionary(x => x.Key, x => string.Join(";", x.Value))
            };
        }
    }
}
