using System;
using System.Configuration;

using Interactive.Footprints.Model;

namespace Interactive.Footprints.Manager
{
    [System.Web.Services.WebServiceBindingAttribute(Name = "CreateIssueWebService", Namespace = "MRWebServices")]
    public class CreateIssueManager : System.Web.Services.Protocols.SoapHttpClientProtocol
    {
        public CreateIssueManager()
        {
            this.Url = ConfigurationManager.AppSettings["FootPrintsURL"];

            // Comment this out if not using a proxy server.
            //System.Net.IWebProxy proxyObject = new System.Net.WebProxy("http://localhost:8888/", false);
            //this.Proxy = proxyObject;
        }
        [
            System.Web.Services.Protocols.SoapDocumentMethodAttribute(
                "MRWebServices#MRWebServices__createIssue",
                RequestNamespace = "MRWebServices",
                ResponseNamespace = "MRWebServices",
                Use = System.Web.Services.Description.SoapBindingUse.Encoded,
                ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped
            )
        ]

        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public string MRWebServices__createIssue(string usr, string pw, string extraInfo, IssueArgs args)
        {
            object[] results = this.Invoke("MRWebServices__createIssue", new object[] { usr, pw, extraInfo, args });
            return ((string)(results[0]));
        }
    }
}