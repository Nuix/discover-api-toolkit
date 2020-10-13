using Amazon;
using Amazon.Runtime;
using FASPClient;
using Newtonsoft.Json;
using Ringtail.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace UploadExample
{
    class Program
    {
        private static Configuration Config { get; set; }
        private static Client Client { get; set; }

        static async Task Main(string[] args)
        {
            var folder = @"c:\temp";
            var filesToSend = Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories);
            var s3Time = SendViaAWS(filesToSend);
            Console.WriteLine("Aws time: " + s3Time);
            //var aspTime = await SendViaAspera(files);
            //var aspTime = await SendViaAspera(new List<string>() { folder });
            //Console.WriteLine("IBM time: " + aspTime);
            Console.Write("Press the <return> key to exit...");
            Console.ReadLine();
        }

        public static async Task<TimeSpan> SendViaAspera(IEnumerable<string> files)
        {
            //Log("Getting configuration");
            Config = Configuration.Load("edtworkinggroup");
            Client = new Client(Config);

            var caseResult = await GetCaseUsingVariables("Clean_Enron");

            //Log(string.Format("Found {0} Clean_Enron cases", caseResult.data.cases.Count));

            var caseId = (int)caseResult.data.cases[0].id.Value;
            //Log(string.Format("using Clean_Enron caseId: {0}", caseId));

            var tokenResult = await GetAsperaToken(caseId);
            dynamic tokenData = tokenResult.data.asperaToken;
            var keyfile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "asperaweb_id_dsa.openssh");

            var config = new FASPConfig()
            {
                HostName = tokenData.hostname,
                Port = tokenData.port,
                User = tokenData.user,
                Path = tokenData.path,
                Token = tokenData.token,
                Keyfilepath = keyfile
            };
            var watch = new Stopwatch();
            watch.Start();
            using (var faspClient = new FaspClient(config))
            {

                


                List<Task> tasks = new List<Task>();
                foreach (var file in files)
                {
                    tasks.Add(Task.Run(() => faspClient.SendFile(file, @"D:\engineToDiscover\RevisedEDRMv1_Complete\benjamin_rogers")));
                }
                Task.WaitAll(tasks.ToArray());
            }
            watch.Stop();
            return watch.Elapsed;
        }

        public static TimeSpan SendViaAWS(IEnumerable<string> files)
        {
            var watch = new Stopwatch();
            watch.Start();
            var prefix = bucketprefix;
            //var creds = new SessionAWSCredentials()
            var awsClient = new AwsClient(new AwsConfig()
            {
                BucketName = "test-bucket-nuix-xfer",
                BucketRegion = RegionEndpoint.USEast1,
                Credentials = new SessionAWSCredentials(
                    //accessKeyId
                    temporaryKeyId,
                    //secretAccessKey
                    temporarySecret,
                    //sessionToken
                    temporarySessionToken
                ), 
                //credentials are restricted to only be able to access objects with this prefix 
                Prefix = prefix
            });
            var tasks = new List<Task>();
            var metafiles = new List<FileMetaData>();
            foreach(var file in files) {
                var fileInfo = new FileInfo(file);
                var key = Guid.NewGuid().ToString();
                var fileMeta = new FileMetaData(prefix, key, fileInfo);
                //metafiles.Add(fileMeta);
                tasks.Add(awsClient.SendFile(key, fileMeta, file));
            }
            //tasks.Add(awsClient.SendData("._metadata.1", new MemoryStream(System.Text.Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(metafiles)))));
            Task.WaitAll(tasks.ToArray());
            watch.Stop();
            return watch.Elapsed;
        }



        private static async Task<dynamic> GetCaseUsingVariables(string caseFilter)
        {
            var command = @"query Enron($caseName: String) { cases(name: $caseName) { id name databaseName } }";
            var vars = new { caseName = caseFilter };
            return await RunAsync(command, vars);
        }

        private static async Task<dynamic> GetAsperaToken(int caseId)
        {

            var command = @"query AsperaToken($caseId: Int) { asperaToken(caseId: $caseId) { token, port, path, hostname, user } }";
            var vars = new { caseId = caseId };
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
            //Console.WriteLine(msg);
        }
    }
}
