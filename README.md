# SimpleRestClient

## Just a simple rest client implementation using RestSharp and HttpClient

### Usage
```cs
//Factory is used here which caches and reuses the HttpClient used internally in RestSharp for the same baseUri
//This reduces latency by almost half in benchmarks and real life scenarios
IRestClient restClient = new RestSharpClient();
//Create a body request containing the payload, Url(construct the url too), and add any optional parameters if needed
//pass ct along to 
var result = await restClient.PostAsync(bodyRequest, ct);
```