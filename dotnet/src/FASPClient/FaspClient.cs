using Aspera.FaspStream;
using Aspera.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FASPClient
{
    public class FaspClient
    {
        private FASPConfig _config;

        public FaspClient(FASPConfig config)
        {
            _config = config;
            Aspera.Transfer.Environment.setFaspScpPath("FaspRedist\\ascp.exe");
        }
        public void SendFile(string source, string sourceRoot, string dest, string keyfilepath)
        {
            var manager = FaspManager.getInstance();

            var xferListener = new SampleFileTransferListener();
            string key;

            RemoteFileLocation remoteLocaton = new RemoteFileLocation(_config.HostName, _config.User, keyfilepath, null);
            remoteLocaton.addPath(dest);
            var host = remoteLocaton.getHost();
            LocalFileLocation localLocation = new LocalFileLocation();
            localLocation.addPath(source, "TempFolder\\test.text");

            XferParams xferParams = new XferParams()
            {
                cipher = Cipher.AES_128,
                createDirs = true,
                policy = Policy.FAIR,
                token = _config.Token,
                tcpPort = _config.Port,
                udpPort = _config.Port,
                targetRateKbps = 10000,
                applyLocalDocroot = true,
                //the bearer token enforces this client cannot access anything outside this folder
                //any attempts will result in the following error
                //Authorization refused: invalid token - transfer files don't match
                destinationRoot = _config.Path, 
                sourceRoot = sourceRoot,
                saveBeforeOverwriteEnabled = true,
            };

            JobOrder transferOrder = new JobOrder(localLocation, remoteLocaton, xferParams);

            // Here we specify the number of parallel transfer sessions to use.
            int nbSessions = 4;
            List<string> jobIds = manager.startTransfer(transferOrder, nbSessions, xferListener);

            Console.WriteLine("JobIds created: {0}", jobIds);
            Console.WriteLine("Waiting for all sessions to be started...");
            for (int i = 0; i < nbSessions; i++)
            {
                xferListener.sessionStartWaitHandle.WaitOne();
            }

            Console.WriteLine("Waiting for all sessions to be finished...");
            for (int i = 0; i < nbSessions; i++)
            {
                xferListener.sessionStopWaitHandle.WaitOne();
            }

            Console.WriteLine("All done! Press any key to exit...");
            Console.Read();
            FaspManager.destroy();
        }

        public void SendStream(string source, string sourceRoot, string dest, string keyfilepath)
        {
            var srcFilePath = Path.Combine(sourceRoot, source);
            if (!File.Exists(srcFilePath))
            {
                Console.WriteLine("The source file specified is not valid (" + srcFilePath + "). Please provide a valid file path");
                System.Environment.Exit(2);
            }
            try
            {
                var f = new FileInfo(srcFilePath);
                var srcFileStream = f.Open(FileMode.Open, FileAccess.Read);
                //FileStream srcFileStream = File.Open(srcFilePath, System.IO.FileMode.Open);
                BufferedStream srcBuffStream = new BufferedStream(srcFileStream);

                //var fsc = new FaspStreamClient();
                //fsc.FaspStreamPath = @".\faspstream.exe";
                //fsc.SSHUser = _config.User;
                //fsc.SSHKeyFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "asperaweb_id_dsa.openssh");
                //fsc.Port = _config.Port;
                //fsc.UdpPort = _config.Port;
                //fsc.TransferMode = Constants.TRANSFER_MODE_TYPE.SEND;

                ////fsc.Hostname = _config.HostName;
                //// Connect to server
                //fsc.connect(_config.HostName);

                var fos = new FaspOutputStreamSAE();
                fos.AscpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ascp.exe");
                fos.Username = _config.User;
                fos.Password = "PASSWORD";
                fos.Token = _config.Token;
                fos.SshKeyFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "asperaweb_id_dsa.openssh");
                fos.TcpPort = _config.Port;
                fos.UdpPort = _config.Port;
                fos.DestinationDirectory = _config.Path;
                fos.DestinationFilename = dest + DateTime.Now.ToFileTimeUtc();
                fos.connect(_config.HostName, f.Length);
                byte[] data = new byte[Constants.TWO_Mb];
                int count;
                while ((count = srcBuffStream.Read(data, 0, data.Length)) > 0)
                {
                    //Loop over the input data and write to the faspstream output stream to transfer
                    fos.write(data, 0, count);
                }
                srcBuffStream.Close();
                fos.close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
