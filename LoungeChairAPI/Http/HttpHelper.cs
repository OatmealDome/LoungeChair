using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LoungeChairAPI.Http
{
    public class HttpHelper
    {
        // Internals
        private readonly HttpClientHandler handler;
        private readonly HttpClient client;
        private CookieContainer _cookieContainer;

        // Public properties
        public CookieContainer cookieContainer
        {
            get
            {
                return _cookieContainer;
            }
            set
            {
                _cookieContainer = value;
                handler.CookieContainer = value;
            }
        }

        public HttpHelper()
        {
            handler = new HttpClientHandler();
            cookieContainer = new CookieContainer();
            client = new HttpClient(handler);

            //handler.UseProxy = true;
            //handler.Proxy = new WebProxy("127.0.0.1:8888");
            client.DefaultRequestHeaders.Add("User-Agent", "AccountsHelper/1.0.0 NASDKAPI");
            //client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 10_2_1 like Mac OS X) AppleWebKit/602.4.3 (KHTML, like Gecko) Version/10.0 Mobile/14D15 Safari/602.1");
        }

        public Task<HttpResponseMessage> GETRequest(string uri)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            return client.SendAsync(request);
        }

        public Task<HttpResponseMessage> POSTRequest(string uri, StringContent content)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = content;
            return client.SendAsync(request);
        }

        public Task<HttpResponseMessage> POSTRequest(string uri, FormUrlEncodedContent content)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = content;
            return client.SendAsync(request);
        }

        public Task<HttpResponseMessage> SendRequest(HttpRequestMessage request)
        {
            return client.SendAsync(request);
        }

        public static async Task<string> GetResponseContent(HttpResponseMessage response)
        {
            using (StreamReader reader = new StreamReader(await response.Content.ReadAsStreamAsync(), Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        public static async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<T>(await GetResponseContent(response));
        }

    }
}
