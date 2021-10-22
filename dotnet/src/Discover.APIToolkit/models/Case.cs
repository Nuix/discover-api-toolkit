using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Discover.APIToolkit.models
{
    public class Case
    {
        [DataMember(Name = "name")]
        public string Name { get; private set; }
        [DataMember(Name = "token")]
        public string Token { get; private set; }
        [DataMember(Name = "uri")]
        public string Uri { get; private set; }
    }
}
