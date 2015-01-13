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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IOperateIncidentWCFService" in both code and config file together.
    [ServiceContract]
    public interface IOperateIncidentWCFService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "v1/registuser", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        RegisterUser RegistUser(RegisterUser user);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "v1/getincidents", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Incident> GetIncidents();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "v1/getopenincidents", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Incident> GetOpenIncidents();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "v1/gethistoryincidents", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Incident> GetHistoryIncidents();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "v1/getopenincidentscount", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        int GetOpenIncidentsCount();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "v1/gethistoryincidentscount", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        int GetHistoryIncidentsCount();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "v1/createincident", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Incident CreateIncident(Incident incident);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "v1/getuserstoshare/{incidentId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<UserModel> GetCompanyOtherUsersToShareIncident(string incidentId);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "v1/getsharedusers/{incidentId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<UserModel> GetSharedUsers(string incidentId);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "v1/shareincident/{incidentId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        WCFResponse ShareIncident(List<UserModel> userList, string incidentId);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "v1/incidentdetail/{fpIncidentId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Incident GetIncidentDetail(string fpIncidentId);
    }
}
