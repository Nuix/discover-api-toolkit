using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace FASPClient
{
    public class AwsConfig
    {
        public string BucketName { get; set; }
        public RegionEndpoint BucketRegion { get; set; }
        public AWSCredentials Credentials { get; set; }
        public string Prefix { get; set; }
    }
    internal class Event
    {
        public FileMetaData[] metaData;
    }

    public class AwsClient
    {
        private AwsConfig _config;

        private static IAmazonS3 _s3Client;
        private AmazonLambdaClient _lambdaClient;

        public AwsClient(AwsConfig config) {
            _config = config;
            _s3Client = new AmazonS3Client(_config.Credentials, _config.BucketRegion);
            var creds = new BasicAWSCredentials(key, secret);
            _lambdaClient = new AmazonLambdaClient(creds, _config.BucketRegion);
        }

        public async Task SendFile(string key, FileMetaData fileMeta, string filePath)
        {
            var fileTransferUtility =
                    new TransferUtility(_s3Client);

            // Option 1. Upload a file. The file name is used as the object key name.
            await fileTransferUtility.UploadAsync(filePath, _config.BucketName, Path.Combine(_config.Prefix, key).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            
            var ev = new JavaScriptSerializer().Serialize(new Event() { metaData = new[]{ fileMeta } });
            var invokeRequest = new InvokeRequest()
            {
                FunctionName = "arn:aws:lambda:us-east-1:593404624303:function:dfredermetadata-functionMeta-ILO7EN88RTY6",
                InvocationType = InvocationType.Event,
                Payload = ev
            };
            var resp = _lambdaClient.InvokeAsync(invokeRequest);            
        }
        
        public async Task SendData(string key, Stream stream)
        {
            var fileTransferUtility =
                    new TransferUtility(_s3Client);

            // Option 1. Upload a file. The file name is used as the object key name.
            await fileTransferUtility.UploadAsync(stream, _config.BucketName, Path.Combine(_config.Prefix, key).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        }

       
    }
}
