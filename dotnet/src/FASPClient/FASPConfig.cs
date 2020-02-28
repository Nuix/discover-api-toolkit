namespace FASPClient
{
    public class FASPConfig
    {
        public FASPConfig()
        {
        }

        public string HostName { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Path { get; set; }
        public string Token { get; set; }
        public string Keyfilepath { get; set; }
    }
}