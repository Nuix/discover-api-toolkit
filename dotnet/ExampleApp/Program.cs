using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Discover.APIToolkit;
using Newtonsoft.Json;

namespace ExampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Run the queries in parallel
            var t1 = Queries.Cases();
            var t2 = Queries.Cases("Enron");
            Task.WaitAll(new Task[] {t1, t2});

            // Log some results
            var caseList = t1.Result;
            var enronCases = t2.Result;

            Logger.Log(string.Format("Found {0} cases", caseList.data.cases.Count));
            Logger.Log(string.Format("Found {0} Enron cases", enronCases.data.cases.Count));

            Console.Write("Press the <return> key to exit...");
            Console.ReadLine();
        }
    }
}
