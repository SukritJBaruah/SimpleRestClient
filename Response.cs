using System.Collections.Generic;

namespace Simple.RestClient
{
    public class Response
    {
        public int Code { get; internal set; }
        public IDictionary<string, string> Headers { get; internal set; }
        public string Payload { get; internal set; }
    }
}