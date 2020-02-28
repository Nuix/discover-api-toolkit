using Aspera.Transfer;
using System;
using System.Collections.Generic;
using System.IO;

namespace FASPClient
{
    public class FaspClient : IDisposable
    {
        private FASPConfig _config;
        private FaspManager _manager;
        private XferParams _xferParams;
        private RemoteFileLocation _remoteLocaton;

        public FaspClient(FASPConfig config) 
        {
            _config = config;
            Aspera.Transfer.Environment.setFaspScpPath("FaspRedist\\ascp.exe");
            _manager = FaspManager.getInstance();
            _xferParams = new XferParams()
            {
                cipher = Cipher.AES_128,
                createDirs = true,
                policy = Policy.FAIR,
                //the bearer token enforces this client cannot access anything outside the path in the response
                //any attempts will result in the following error
                //Authorization refused: invalid token - transfer files don't match
                token = _config.Token,
                tcpPort = _config.Port,
                udpPort = _config.Port,
                //targetRateKbps = 5000, //this can be adjusted but beware, yo
                //applyLocalDocroot = true,
                //root path at the server to drop files (this should match the path in the token response)
                destinationRoot = _config.Path,
                //root path of the transfer, all local sources should be relative to this
                //sourceRoot = sourceRoot,
                saveBeforeOverwriteEnabled = true//,
                //multiSessionThreshold = 4096
            };
            _remoteLocaton = new RemoteFileLocation(_config.HostName, _config.User, _config.Keyfilepath, null);

        }

        public void Dispose()
        {
            _manager = null;
            FaspManager.destroy();
        }

        public void SendFile(string source, string sourceRoot)
        {
            

            var xferListener = new SampleFileTransferListener();
            
            LocalFileLocation localLocation = new LocalFileLocation();
            //where to find the file to send
            //inSrc = relative to the sourceRoot 
            //inDest = where to drop the file on the server, relative to the destinationRoot
            //inDest can be used to move or rename files while sending
            var fileUri = new Uri(source);
            var rootUri = new Uri(sourceRoot);
            var relativePath = rootUri.MakeRelativeUri(fileUri).ToString();
            //localLocation.addPath(source, relativePath);
            localLocation.addPath(source);



            JobOrder transferOrder = new JobOrder(localLocation, _remoteLocaton, _xferParams);

            // Here we specify the number of parallel transfer sessions to use.
            int nbSessions = 4;
            List<string> jobIds = _manager.startTransfer(transferOrder, nbSessions, xferListener);

            //Console.WriteLine("JobIds created: {0}", jobIds);
            //Console.WriteLine("Waiting for all sessions to be started...");
            for (int i = 0; i < nbSessions; i++)
            {
                xferListener.sessionStartWaitHandle.WaitOne();
            }

            Console.WriteLine("Waiting for all sessions to be finished...");
            for (int i = 0; i < nbSessions; i++)
            {
                xferListener.sessionStopWaitHandle.WaitOne();
            }

            //Console.WriteLine("All done! Press any key to exit...");
            //Console.Read();

        }
    }
}
