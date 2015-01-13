using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Interactive.Services.Contract
{
    [DataContract]
    public class Severity
    {
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public string Label { get; set; }
    }

    public enum SeverityType
    { 
        P0 = 1,
        P1 = 2,
        P2 = 3,
        P3 = 4,
        P4 = 5
    }
}