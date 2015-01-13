using Interactive.Constant;
using Interactive.DBManager.Entity;
using Interactive.DBManager.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive.DBManager.Service
{
    public class IncidentRepService
    {
        #region Field and Property
        private IncidentRepository incidentRep;
        internal IncidentRepository IncidentRep
        {
            get
            {
                if (this.incidentRep == null)
                {
                    this.incidentRep = new IncidentRepository();
                }
                return this.incidentRep;
            }
        }
        #endregion

        public void CreateRegistrationIncident(IncidentEntity incident)
        {
            if (!IncidentRep.CheckRegistrationIncidentByEmail(incident.SubmitterEmail))
            {
                IncidentRep.AddIncident(incident);
            }
        }

        public void DeleteIncident(IncidentEntity incident)
        {
            IncidentRep.DeleteIncident(incident);
        }

        public bool CheckRegistrationIncidentByEmail(string email)
        {
            return IncidentRep.CheckRegistrationIncidentByEmail(email);
        }

        public bool CheckOpenRegistrationIncidentByEmail(string email)
        {
            return IncidentRep.CheckOpenRegistrationIncidentByEmail(email);
        }

        public IncidentEntity GetRegistrationIncidentByEmail(string email)
        {
            return IncidentRep.GetRegistrationIncidentByEmail(email);
        }

        public void UpdateRegistrationIncident(IncidentEntity incident)
        {
            IncidentRep.UpdateRegistrationIncident(incident);
        }

        public void UpdateIncidentType(IncidentEntity incident)
        {
            IncidentRep.UpdateIncidentType(incident);
        }

        public void CreateIncident(IncidentEntity incident)
        {
            IncidentRep.AddIncident(incident);
        }

        public IncidentEntity GetIncidentByFPId(IncidentEntity incident)
        {
            return IncidentRep.GetIncidentByFPId(incident);
        }

        public DataSet GetIncidentIdByToken(string token)
        {
            return IncidentRep.GetIncidentIdByToken(token);
        }

        public string GetIncidentIdByFpId(string fpIncidentId)
        {
            return IncidentRep.GetIncidentIdByFpId(fpIncidentId);
        }

        public bool CheckIncidentShared(string incidentId)
        {
            if ((IncidentType)IncidentRep.GetIncidentType(incidentId) == IncidentType.Share)
                return true;
            else
                return false;
        }

        public int GetShareCount(string incidentId, string token)
        {
            UsersRepService _userService = new UsersRepService();
            int owner = IncidentRep.GetOwnerByIncidentId(incidentId);
            int currentUser = _userService.GetUserIdByToken(token);

            if (owner == currentUser)
                return IncidentRep.GetShareCountOwner(incidentId, currentUser.ToString());
            else
                return IncidentRep.GetShareCount(incidentId);
        }

        public bool CheckIncidentOwner(string incidentId, string token)
        {
            UsersRepService _userService = new UsersRepService();
            int owner = IncidentRep.GetOwnerByIncidentId(incidentId);
            int currentUser = _userService.GetUserIdByToken(token);
            if (owner == currentUser)
                return true;
            else
                return false;
        }
    }
}
