using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Interactive.Services.Contract
{
    [DataContract]
    public class CheckToken
    {
        [DataMember]
        public bool IsTokenExist { get; set; }
        [DataMember]
        public bool IsTokenExpired { get; set; }
        [DataMember]
        public bool IsTokenValid { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Token { get; set; }
    }
}