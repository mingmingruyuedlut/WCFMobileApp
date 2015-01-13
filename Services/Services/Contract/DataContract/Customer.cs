using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Collections;
using Newtonsoft.Json;

namespace Interactive.Services.Contract
{
    [DataContract]
    public class Customer
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string ContactNumber { get; set; }
        [DataMember]
        public List<Incident> Incidents { get; set; }

    }

    [DataContract]
    public class SetPassword
    {
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public bool IsUpPwd { get; set; }
    }

    [DataContract]
    public class Login
    {
        [DataMember]
        public bool IsEmailExist { get; set; }

        [DataMember]
        public bool IsPwdValid { get; set; }

        [DataMember]
        public string Token { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public bool IsLogout { get; set; }

        [DataMember]
        public string ContactName { get; set; }

        [DataMember]
        public string ContactNumber { get; set; }
    }
}