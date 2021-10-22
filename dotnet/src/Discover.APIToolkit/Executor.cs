using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discover.APIToolkit
{
    public class Executor
    {
        private Configuration _configuration;
        private Client _client;
        public Executor()
        {
            Logger.Log("Getting configuration");
            _configuration = Configuration.Load();
            _client = new Client(_configuration);
        }

        public async Task<dynamic> RunAsync(string command, dynamic variables = null)
        {
            var response = await _client.Execute(command, variables: variables);
            var content = await response.Content.ReadAsStringAsync();

            // Just show the content
            Logger.Log(content);

            // Return a dynamic object
            return JsonConvert.DeserializeObject<dynamic>(content);
        }
    }
}
