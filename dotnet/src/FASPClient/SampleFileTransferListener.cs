using global::Aspera.Transfer;
using System;
using System.Threading;

namespace FASPClient
{

    /// <summary>
    /// All listener classes must inherit 'FileTransferListener' class.
    /// </summary>
    public class SampleFileTransferListener : FileTransferListener
    {
        /// <summary>
        /// The session start wait handle
        /// </summary>
        protected internal EventWaitHandle sessionStartWaitHandle = new AutoResetEvent(false);

        /// <summary>
        /// The session stop wait handle
        /// </summary>
        protected internal EventWaitHandle sessionStopWaitHandle = new AutoResetEvent(false);

        /// <summary>
        /// This is the call back method called by the FaspManager on any event
        /// </summary>
        /// <param name="transferEvent">The event for callback.</param>
        /// <param name="sessionStats">The Session levels statistics for the Session that triggered this event</param>
        /// <param name="fileStats">Statistics of the last file transferred in this session</param>
        public void fileSessionEvent(TransferEvent transferEvent, SessionStats sessionStats, FileStats fileStats)
        {
            //displayProgress(transferEvent, sessionStats, fileStats);

            if (transferEvent == TransferEvent.SESSION_START)
            {
                //Console.WriteLine("Session Started. ID: {0}", sessionStats.SessionId);
                //Console.WriteLine("Tags: {0}", sessionStats.Tags.ToString(Newtonsoft.Json.Formatting.None));
                sessionStartWaitHandle.Set();
            }

            if (transferEvent == TransferEvent.SESSION_STOP)
            {
                //Console.WriteLine("\tSession Stopped");
                sessionStopWaitHandle.Set();
            }
            else if (transferEvent == TransferEvent.SESSION_ERROR || transferEvent == TransferEvent.FILE_ERROR || transferEvent == TransferEvent.FILE_SKIP)
            {
                Console.WriteLine(sessionStats.ErrorDescription);
				sessionStopWaitHandle.Set();
            }
        }

        /// <summary>
        /// Displays the progress.
        /// </summary>
        /// <param name="transferEvent">The transfer event.</param>
        /// <param name="sessionStats">The session stats.</param>
        /// <param name="fileStats">The file stats.</param>
        public void displayProgress(TransferEvent transferEvent, SessionStats sessionStats, FileStats fileStats)
        {
            Console.WriteLine("Job Name: " + sessionStats.Cookie);
            Console.WriteLine("XferId: " + sessionStats.XferId);
            Console.WriteLine("\tJob State: " + sessionStats.State);
            Console.WriteLine("\tTarget Rate: " + sessionStats.TargetRateKbps + "Kbps");

            if (sessionStats.ElapsedUSec > 0)
                Console.WriteLine("\tAvg Actual Rate: " + (sessionStats.TotalTransferredBytes * 8 / sessionStats.ElapsedUSec) / 1000 + "Kbps");
            Console.WriteLine("\tSession Downloaded: " + (sessionStats.TotalTransferredBytes / 1000) + "KB");
            if (fileStats != null)
            {
                Console.WriteLine("\tFileName: " + fileStats.name);
                Console.WriteLine("\tFile Downloaded: " + (fileStats.writtenBytes / 1000) + "KB");
            }

            if (transferEvent == TransferEvent.FILE_ERROR)
                Console.WriteLine("File failed: " + fileStats.errDescription);

            if (transferEvent == TransferEvent.SESSION_ERROR)
            {
                Console.WriteLine(sessionStats.ErrorDescription);
            }

            Console.Write("\n");
        }
    }
}


