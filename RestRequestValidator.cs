using System;
using System.Collections.Generic;
using System.Text;
using Simple.RestClient;

namespace Simple.RestClient
{
    internal class RestRequestValidator
    {
        public void ValidateRequest(Request request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Url == null)
                throw new ArgumentNullException(nameof(request.Url));

            if (string.IsNullOrEmpty(request.Url.BaseUrl))
                throw new ArgumentException(nameof(request.Url.BaseUrl));

            if (string.IsNullOrEmpty(request.Url.RelativeUrl))
                throw new ArgumentException(nameof(request.Url.RelativeUrl));

        }

        public void ValidateBodyRequest(BodyRequest request)
        {
            ValidateRequest(request);

            var bodyrequest = request;
            if (string.IsNullOrEmpty(bodyrequest.Payload))
            {
                throw new ArgumentException(nameof(bodyrequest.Payload));
            }
        }
    }
}
