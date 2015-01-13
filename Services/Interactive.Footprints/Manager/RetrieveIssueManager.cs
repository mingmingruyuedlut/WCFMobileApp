using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive.Footprints.Manager
{

    [System.Web.Services.WebServiceBindingAttribute(Name = "GetIssueDetailsWebService", Namespace = "MRWebServices")]
    public class RetrieveIssueManager : System.Web.Services.Protocols.SoapHttpClientProtocol
    {
        public RetrieveIssueManager()
        {
            this.Url = ConfigurationManager.AppSettings["FootPrintsURL"];
        }

        [
            System.Web.Services.Protocols.SoapDocumentMethodAttribute(
                "MRWebServices#MRWebServices__getIssueDetails",
                RequestNamespace = "MRWebServices",
                ResponseNamespace = "MRWebServices",
                Use = System.Web.Services.Description.SoapBindingUse.Encoded,
                ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)
        ]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public string MRWebServices__getIssueDetails(string usr, string pw, string extraInfo, int proj, int num)
        {
            object[] results = this.Invoke("MRWebServices__getIssueDetails", new object[] { usr, pw, extraInfo, proj, num });
            return ((string)(results[0]));
        }
    }
}
