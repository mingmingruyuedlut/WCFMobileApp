using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Interactive.Services.Contract
{
    [DataContract]
    public class WCFResponse
    {
        [DataMember]
        public WCFResponseType Type { get; set; }
        [DataMember]
        public bool IsSuccess { get; set; }
        [DataMember]
        public string Message { get; set; }
    }

    [DataContract]
    public enum WCFResponseType
    { 
        Success,
        Failed,
        DBError,
        FPError
    }
}