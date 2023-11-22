using Polly;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Polly.Contrib.WaitAndRetry;
using System.Linq;

namespace Simple.RestClient
{
    public class RestSharpClient : IRestClient
    {
        private readonly IAsyncPolicy<RestResponse> _retryPolicy;
        private readonly RestRequestValidator _requestValidator;
        private readonly int _requestTimeout;
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="RequestTimeout">In MilliSeconds</param>
        public RestSharpClient(int RequestTimeout = 5_000)
        {
            _requestTimeout = RequestTimeout;
            _retryPolicy = Policy
                .HandleResult<RestResponse>(response => !response.IsSuccessful || response.StatusCode >= System.Net.HttpStatusCode.InternalServerError)
                .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5));
            _requestValidator = new RestRequestValidator();
        }
        public async Task<Response> DeleteAsync(Request Request, CancellationToken ct = default)
        {
            _requestValidator.ValidateRequest(Request);
            RestRequest request = CreateRequest(Request, Method.Delete);
            return await Execute(Request.Url.BaseUrl, request, ct);
        }

        public async Task<Response> GetAsync(Request Request, CancellationToken ct = default)
        {
            _requestValidator.ValidateRequest(Request);
            RestRequest request = CreateRequest(Request, Method.Get);
            return await Execute(Request.Url.BaseUrl, request, ct);
        }

        public async Task<Response> PatchAsync(BodyRequest Request, CancellationToken ct = default)
        {
            _requestValidator.ValidateRequest(Request);
            RestRequest request = CreateBodyRequest(Request, Method.Patch);
            return await Execute(Request.Url.BaseUrl, request, ct);
        }

        public async Task<Response> PostAsync(BodyRequest Request, CancellationToken ct = default)
        {
            _requestValidator.ValidateRequest(Request);
            RestRequest request = CreateBodyRequest(Request, Method.Post);
            return await Execute(Request.Url.BaseUrl, request, ct);

        }

        public async Task<Response> PutAsync(BodyRequest Request, CancellationToken ct = default)
        {
            _requestValidator.ValidateRequest(Request);
            RestRequest request = CreateBodyRequest(Request, Method.Put);
            return await Execute(Request.Url.BaseUrl, request, ct);
        }

        private async Task<Response> Execute(string baseUrl, RestRequest request, CancellationToken ct)
        {
            var restOptions = new RestClientOptions()
            {
                BaseUrl = new Uri(baseUrl),
                MaxTimeout = _requestTimeout
            };
            using (var client = new RestSharp.RestClient(restOptions, useClientFactory: true))
            {
                var result = await _retryPolicy.ExecuteAsync(async () =>
                {

                    var response = await client.ExecuteAsync(request, ct);
                    return response;
                });

                return getResponse(result);
            }
        }

        private static RestRequest CreateRequest(Request Request, Method method)
        {
            var request = new RestRequest(Request.Url.RelativeUrl, method);
            foreach (KeyValuePair<string, string> param in Request.Url.QueryParams)
                request.AddQueryParameter(param.Key, param.Value);

            request.AddHeaders(Request.Headers);
            return request;
        }

        private static RestRequest CreateBodyRequest(BodyRequest Request, Method method)
        {
            var request = new RestRequest(Request.Url.RelativeUrl, method);
            foreach (KeyValuePair<string, string> param in Request.Url.QueryParams)
                request.AddQueryParameter(param.Key, param.Value);
            request.AddHeaders(Request.Headers);
            request.AddJsonBody(Request.Payload);
            return request;
        }

        private Response getResponse(RestResponse restresponse)
        {
            return new Response()
            {
                Code = (int)restresponse.StatusCode,
                Payload = restresponse.Content,
                Headers = restresponse.ContentHeaders?.ToDictionary(x => x.Name, x => (string)x.Value)
            };
        }

    }
}
