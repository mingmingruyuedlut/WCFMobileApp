using Interactive.Services.Contract;
using Services.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "OperateIncidentWCFService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select OperateIncidentWCFService.svc or OperateIncidentWCFService.svc.cs at the Solution Explorer and start debugging.
    public class OperateIncidentWCFService : IOperateIncidentWCFService
    {
        #region Field and Property
        private OperateIncidentService opService;
        internal OperateIncidentService _opService
        {
            get
            {
                if (this.opService == null)
                {
                    this.opService = new OperateIncidentService();
                }
                return this.opService;
            }
        }
        #endregion

        public RegisterUser RegistUser(RegisterUser user)
        {
            return _opService.RegistUser(user);
        }

        public List<UserModel> GetCompanyOtherUsersToShareIncident(string incidentId)
        {
            return _opService.GetCompanyOtherUsersToShareIncident(incidentId);
        }

        public List<UserModel> GetSharedUsers(string incidentId)
        {
            return _opService.GetSharedUsers(incidentId);
        }

        public WCFResponse ShareIncident(List<UserModel> userList, string incidentId)
        {
            return _opService.ShareIncident(userList, incidentId);
        }

        public Incident CreateIncident(Incident incident)
        {
            return _opService.CreateIncident(incident);
        }

        public List<Incident> GetIncidents()
        {
            return _opService.GetIncidents();
        }

        public List<Incident> GetOpenIncidents()
        {
            return _opService.GetOpenIncidents();
        }

        public List<Incident> GetHistoryIncidents()
        {
            return _opService.GetHistoryIncidents();
        }

        public int GetOpenIncidentsCount()
        {
            return _opService.GetOpenIncidentsCount();
        }

        public int GetHistoryIncidentsCount()
        {
            return _opService.GetHistoryIncidentsCount();
        }

        public Incident GetIncidentDetail(string fpIncidentId)
        {
            return _opService.GetIncidentDetail(fpIncidentId);            
        }
    }
}
