using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ringtail.API;

namespace ExampleApp
{
    class Program
    {
        private static Configuration Config { get; set; }
        private static Client Client { get; set; }

        static void Main(string[] args)
        {
            Log("Getting configuration");
            Config = Configuration.Load();
            Client = new Client(Config);

            // Run the queries in parallel    
            var t1 = GetCases();
            var t2 = GetCaseUsingVariables("Enron");
            Task.WaitAll(new Task[] {t1, t2});

            // Log some results
            var caseList = t1.Result;
            var enronCases = t2.Result;

            Log(string.Format("Found {0} cases", caseList.data.cases.Count));
            Log(string.Format("Found {0} Enron cases", enronCases.data.cases.Count));

            Console.Write("Press the <return> key to exit...");
            Console.ReadLine();
        }

        private static async Task<dynamic> GetCases()
        {
            Log("Requesting case data");
            var command = @"{ cases { id name } }";

            return await RunAsync(command);
        }

        private static async Task<dynamic> GetCaseUsingVariables(string caseFilter)
        {
            var command = @"query Enron($caseName: String) { cases(name: $caseName) { id name databaseName } }";
            var vars = new {caseName = caseFilter};

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
