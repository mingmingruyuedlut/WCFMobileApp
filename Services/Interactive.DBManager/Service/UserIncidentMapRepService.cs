using Interactive.DBManager.Entity;
using Interactive.DBManager.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive.DBManager.Service
{
    public class UserIncidentMapRepService
    {
        #region Field and Property
        private UserIncidentMapRepository userIncidentMapRep;
        internal UserIncidentMapRepository UserIncidentMapRep
        {
            get
            {
                if (this.userIncidentMapRep == null)
                {
                    this.userIncidentMapRep = new UserIncidentMapRepository();
                }
                return this.userIncidentMapRep;
            }
        }
        #endregion

        public void AddUserIncidentMap(UserIncidentMapEntity uiMap)
        {
            if (!UserIncidentMapRep.CheckUserIncidentMap(uiMap))
            {
                UserIncidentMapRep.AddUserIncidentMap(uiMap);
            }
        }

        public void AddUserIncidentMap(List<UserIncidentMapEntity> uiMapList)
        {
            foreach (UserIncidentMapEntity uiMap in uiMapList)
            {
                AddUserIncidentMap(uiMap);
            }
        }

        public void DeleteUserIncidentMap(UserIncidentMapEntity uiMap)
        {
            if (UserIncidentMapRep.CheckUserIncidentMap(uiMap))
            {
                UserIncidentMapRep.DeleteUserIncidentMap(uiMap);
            }
        }

        public void DeleteUserIncidentMap(List<UserIncidentMapEntity> uiMapList)
        {
            foreach (UserIncidentMapEntity uiMap in uiMapList)
            {
                DeleteUserIncidentMap(uiMap);
            }
        }
    }
}
