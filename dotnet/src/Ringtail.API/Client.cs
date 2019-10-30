using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ringtail.API
{
    public class Client
    {
        public Configuration Configuration { get; private set; }
        private HttpClient HttpClient { get; set; }

        public Client(Configuration config)
        {
            this.Configuration = config;
            Initialize();
        }

        private void Initialize()
        {
            if (HttpClient != null)
                return;

            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Configuration.Token);
            HttpClient.DefaultRequestHeaders.Add("ApiKey", Configuration.ApiKey);
            HttpClient.BaseAddress = new Uri(Configuration.Uri);
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<HttpResponseMessage> Execute(string command, string operation = null, dynamic variables = null)
        {
            var query = new GraphQLQuery() { OperationName = operation, Query = command, Variables = variables };
            var content = JsonConvert.SerializeObject(query);

            return await HttpClient.PostAsync("", new StringContent(content, Encoding.ASCII, "application/json"));
        }

        public async Task<HttpResponseMessage> Post(string url, HttpContent content)
        {
            return await HttpClient.PostAsync(url, content);
        }

        private class GraphQLQuery
        {
            public string OperationName { get; set; }
            public string Query { get; set; }
            public dynamic Variables { get; set; }
        }
    }
}
