using FASPClient;
using Newtonsoft.Json;
using Ringtail.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadExample
{
    class Program
    {
        private static Configuration Config { get; set; }
        private static Client Client { get; set; }

        static async Task Main(string[] args)
        {
            Log("Getting configuration");
            Config = Configuration.Load();
            Client = new Client(Config);

            var caseResult = await GetCaseUsingVariables("Clean_Enron");
            
            Log(string.Format("Found {0} Clean_Enron cases", caseResult.data.cases.Count));

            Log(string.Format("First case id={0}", caseResult.data.cases[0].id));

            var tokenResult = await GetAsperaToken("Clean_Enron");
            dynamic tokenData = tokenResult.data.asperaToken;
            
            var config = new FASPConfig()
            {
                HostName = tokenData.hostname,
                Port = tokenData.port,
                User = tokenData.user,
                Path = tokenData.path,
                Token = tokenData.token
            };

            var faspClient = new FaspClient(config);
            var keyfile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "asperaweb_id_dsa.openssh");
            faspClient.SendFile("test.txt", AppDomain.CurrentDomain.BaseDirectory, "TestFolder", keyfile);

            //streaming does not work yet, the library returns a licensing error blocking further progress
            //we are waiting on IBM support for a resolution
            //faspClient.SendStream("test.txt", AppDomain.CurrentDomain.BaseDirectory, "TestFolder", keyfile);
            Console.Write("Press the <return> key to exit...");
            Console.ReadLine();
        }

        

        private static async Task<dynamic> GetCaseUsingVariables(string caseFilter)
        {
            var command = @"query Enron($caseName: String) { cases(name: $caseName) { id name databaseName } }";
            var vars = new { caseName = caseFilter };
            return await RunAsync(command, vars);
        }

        private static async Task<dynamic> GetAsperaToken(string caseName)
        {

            var command = @"query AsperaToken($caseName: String) { asperaToken(caseName: $caseName) { token, port, path, hostname, user } }";
            var vars = new { caseName = caseName };
            return await RunAsync(command, vars);
        }

        private static async Task<dynamic> RunAsync(string command, dynamic variables = null)
        {
            var response = await Client.Execute(command, variables: variables);
            var content = await response.Content.ReadAsStringAsync();

            // Just show the content
            Log(content);

            // Return a dynamic object
            return JsonConvert.DeserializeObject<dynamic>(content);
        }

        private static void Log(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
