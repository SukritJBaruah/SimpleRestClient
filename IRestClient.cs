using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.RestClient
{
    public interface IRestClient
    {
        public Task<Response> GetAsync(Request Request, CancellationToken ct = default);
        public Task<Response> PostAsync(BodyRequest Request, CancellationToken ct = default);
        public Task<Response> PatchAsync(BodyRequest Request, CancellationToken ct = default);
        public Task<Response> PutAsync(BodyRequest Request, CancellationToken ct = default);
        public Task<Response> DeleteAsync(Request Request, CancellationToken ct = default);
    }
}
