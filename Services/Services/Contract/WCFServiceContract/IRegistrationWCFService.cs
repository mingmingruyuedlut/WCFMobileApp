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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRegistrationWCFService" in both code and config file together.
    [ServiceContract]
    public interface IRegistrationWCFService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "v1/createpassword", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        SetPassword CreatePassword(SetPassword password);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "v1/checktoken", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        CheckToken CheckToken();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "v1/checkuser", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        CheckUser CheckUser(CheckUser user);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "v1/forgetpassword", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        CheckUser ForgetPassword(CheckUser user);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "v1/updatepassword", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        SetPassword UpdatePassword(SetPassword password);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "v1/gettoken/{email}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string GetToken(string email);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "v1/getuserdetail", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        UserModel GetUserDetail();
    }
}
