using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Discover.APIToolkit.models
{
    public class QueryResult
    {
        [DataMember(Name = "data")]
        public string Data { get; private set; }
    }
}
