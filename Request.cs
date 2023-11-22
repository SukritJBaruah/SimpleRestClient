using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Simple.RestClient
{
    public class Request
    {
        public Url Url { get; set; }
        public IDictionary<string, string> Headers { internal get; set; } = new Dictionary<string, string>();

        public Request AddHeader(string key, string value)
        {
            Headers.Add(key, value);
            return this;
        }
    }

    public class Url
    {
        public IDictionary<string, string> QueryParams { internal get; set; } = new Dictionary<string, string>();

        public string BaseUrl { internal get; set; }
        public string RelativeUrl { internal get; set; }

        public Url(string BaseUrl, params string[] PathParams)
        {
            this.BaseUrl = BaseUrl;
            
            StringBuilder sb = new StringBuilder();
            foreach (string path in PathParams)
                sb.Append("/" + path);
            RelativeUrl = sb.ToString();
        }

        public Url AddQueyParam(string key, string value)
        {
            QueryParams.Add(key, value);
            return this;
        }

    }

    public class BodyRequest :Request
    {
        public string Payload { internal get; set; }


        public BodyRequest SetPayloadFromJson(object data)
        {
            Payload = JsonConvert.SerializeObject(data);
            return this;
        }
    }

}
