using Newtonsoft.Json;
using RestSharp;

namespace FakeStoreAPIUsingNUnitAndRestSharp.Clients
{
    public class ApiClients
    {
        public readonly RestClient _client;

        public ApiClients(string baseUrl)
        {
            _client = new RestClient(baseUrl);
        }


        public T Get<T>(string endpoint)
        {
            var request = new RestRequest(endpoint, Method.Get);
            var response = _client.Execute(request);
            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        public RestResponse GetRaw(string endpoint)
        {
            var request = new RestRequest(endpoint, Method.Get);
            var response = _client.Execute(request);
            return response;
        }

        public T Post<T>(string endpoint, object body)
        {
            var request = new RestRequest(endpoint, Method.Post);
            request.AddJsonBody(body);
            var response = _client.Execute(request);
            return JsonConvert.DeserializeObject<T>(response.Content);
        }


        public RestResponse PostRaw(string endpoint, object body)
        {
            var request = new RestRequest(endpoint, Method.Post);
            request.AddJsonBody(body);
            var response = _client.Execute(request);
            return response;
        }

        public T Put<T>(string endpoint, object body)
        {
            var request = new RestRequest(endpoint, Method.Put);
            request.AddJsonBody(body);
            var response = _client.Execute(request);
            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        public RestResponse Delete(string endpoint)
        {
            var request = new RestRequest(endpoint, Method.Delete);
            _client.Execute(request);
            return _client.Execute(request);
        }
    }
}
