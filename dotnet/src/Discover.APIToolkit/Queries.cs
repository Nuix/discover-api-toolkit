using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discover.APIToolkit
{
    public class Queries
    {
        public static async Task<dynamic> Cases()
        {
            var command = @"{ cases { id name } }";
            

            return await new Executor().RunAsync(command);
        }

        public static async Task<dynamic> Cases(string caseFilter)
        {
            var command = @"query Enron($caseName: String) { cases(name: $caseName) { id name databaseName } }";
            var vars = new { caseName = caseFilter };

            return await new Executor().RunAsync(command, vars);
        }

        public static async Task<dynamic> CaseWithInfo(string caseFilter)
        {
            var command = @"query Enron($caseName: String) { cases(name: $caseName) { users { userName } statistics { aggregateBaseDocsUsageSummary } } }"; 
            var vars = new { caseName = caseFilter };

            return await new Executor().RunAsync(command, vars);
        }
    }
}
