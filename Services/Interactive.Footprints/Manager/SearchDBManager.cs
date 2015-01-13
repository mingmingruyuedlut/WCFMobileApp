using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive.Footprints.Manager
{
    [System.Web.Services.WebServiceBindingAttribute(Name = "SearchWebService", Namespace = "MRWebServices")]
    public class SearchDBManager : System.Web.Services.Protocols.SoapHttpClientProtocol
    {
        public SearchDBManager()
        {
            this.Url = ConfigurationManager.AppSettings["FootPrintsURL"];
        }

        [
            System.Web.Services.Protocols.SoapDocumentMethodAttribute(
                "MRWebServices#MRWebServices__search",
                RequestNamespace = "MRWebServices",
                ResponseNamespace = "MRWebServices",
                Use = System.Web.Services.Description.SoapBindingUse.Encoded,
                ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)
        ]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public string MRWebServices__search(string usr, string pw, string extraInfo, string query)
        {
            object[] results = this.Invoke("MRWebServices__search", new object[] { usr, pw, extraInfo, query });
            return ((string)(results[0]));
        }
    }
}
