using AcApi.Infrastructure.Exception;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AcApi.Infrastructure.Http
{
    public class HttpRequest : IHttpRequest
    {

        public IHttpClientFactory ClientFactory { get; set; }
        public virtual ILogger<HttpRequest> Logger { get; }

        public HttpRequest(IHttpClientFactory clientFactory, ILogger<HttpRequest> logger)
        {
            this.ClientFactory = clientFactory;
            this.Logger = logger;
        }

        public T Get<T>(string url, Token token, TimeSpan timeout)
        {
            var client = ClientFactory.CreateClient("HttpClientWithSSLUntrusted");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = timeout;

            Logger.LogInformation("Try execute GET requset to {}", url);
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(token.Type, token.Value);
                HttpResponseMessage response = client.SendAsync(requestMessage).Result;
                Logger.LogInformation("GET done {}, try get body", url, response.StatusCode);

                String body = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    Logger.LogInformation("Succes in POST {}, response is {}", url, body);
                    return JsonConvert.DeserializeObject<T>(body);
                }
                Logger.LogError("Fail on execute GET to {}, status code is {}", url, response.StatusCode, response.Content.ReadAsStringAsync().Result);
                return default(T);
            }
        }

        public String GetText(string url, Token token, TimeSpan timeout)
        {
            var client = ClientFactory.CreateClient("HttpClientWithSSLUntrusted");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = timeout;

            Logger.LogInformation($"Try execute GET requset to {url}");
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(token.Type, token.Value);
                HttpResponseMessage response = client.SendAsync(requestMessage).Result;
                Logger.LogInformation($"GET done {url}, try get body {response.StatusCode}");

                String body = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    Logger.LogInformation("Succes in POST {}, response is {}", url, body);
                    return body;
                }
                Logger.LogError("Fail on execute GET to {}, status code is {}", url, response.StatusCode, response.Content.ReadAsStringAsync().Result);
                return null;
            }
        }

        public T Post<T>(string url, object postBody, Token token, TimeSpan timeout)
        {
            var client = ClientFactory.CreateClient("HttpClientWithSSLUntrusted");
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            string bodyInJson = JsonConvert.SerializeObject(postBody, settings);
            Logger.LogInformation($"Try execute POST requset to {url} with {bodyInJson}");
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, url))
            {
                requestMessage.Content = new StringContent(bodyInJson, Encoding.UTF8, "application/json");
                client.Timeout = timeout;
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(token.Type, token.Value);
                HttpResponseMessage response = client.SendAsync(requestMessage).Result;
                Logger.LogInformation($"POST done {url}, try get body {response.StatusCode}");

                String body = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    Logger.LogInformation($"Succes in POST {url}, response is {body}");
                    return JsonConvert.DeserializeObject<T>(body);
                }
                throw new BusinessRuleExpcetion($"Falha ao executar post {url}  status is {response.StatusCode} and body is {body}");
            }
        }

        public T Get<T>(string url, Token token)
        {
            return this.Get<T>(url, token, TimeSpan.FromSeconds(5));
        }
    }
}
