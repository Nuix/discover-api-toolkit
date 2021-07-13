using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Discover.APIToolkit
{
    [DataContract]
    public class Configuration
    {
        [DataMember(Name = "name")]
        public string Name { get; private set; }
        [DataMember(Name = "token")]
        public string Token { get; private set; }
        [DataMember(Name = "uri")]
        public string Uri { get; private set; }

        public static Configuration Load(string profileName = "default")
        {
            var path = ConfigFilePath();

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            var text = File.ReadAllText(path);

            // Allow for an array of configurations
            try
            {
                var configs = JsonConvert.DeserializeObject<List<Configuration>>(text);
                return configs.First(c => c.Name.Equals(profileName));
            }

            //...or a single config object (non-array)
            catch
            {
                var config = JsonConvert.DeserializeObject<Configuration>(text);
                if (config != null && config.Name.Equals(profileName))
                    return config;
            }
            return null;
        }

        public static string ConfigFilePath()
        {
            var home = Environment.GetEnvironmentVariable("USERPROFILE");
            return Path.Combine(home, @".ringtail\config");
        }
    }
}
