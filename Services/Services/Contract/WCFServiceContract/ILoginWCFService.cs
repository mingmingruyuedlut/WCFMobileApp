using Interactive.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ILoginWCFService" in both code and config file together.
    [ServiceContract]
    public interface ILoginWCFService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "v1/login", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Login IsLogin(Login login);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "v1/logout", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Login IsLogout();
    }
}
