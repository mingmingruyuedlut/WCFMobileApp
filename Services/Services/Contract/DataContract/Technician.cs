using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Interactive.Services.Contract
{
    [DataContract]
    public class Technician
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public int Rating { get; set; }
        [DataMember]
        public string Avatar { get; set; }
    }
}