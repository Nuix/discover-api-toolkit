using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FASPClient
{
    public class AwsConfig
    {
        public string BucketName { get; set; }
        public RegionEndpoint BucketRegion { get; set; }
        public AWSCredentials Credentials { get; set; }
        public string Prefix { get; set; }
    }
    public class AwsClient
    {
        private AwsConfig _config;

        private static IAmazonS3 _s3Client;

        
        public AwsClient(AwsConfig config) {
            _config = config;
            _s3Client = new AmazonS3Client(_config.Credentials, _config.BucketRegion);
        }

        public async Task SendFile(string filePath)
        {
            var fileTransferUtility =
                    new TransferUtility(_s3Client);

            // Option 1. Upload a file. The file name is used as the object key name.
            await fileTransferUtility.UploadAsync(filePath, _config.BucketName, Path.Combine(_config.Prefix, Path.GetFileName(filePath)).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            //Console.WriteLine("Upload 1 completed");
        }

       
    }
}
