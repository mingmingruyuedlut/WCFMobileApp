using Interactive.DBManager.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Interactive.Services.Contract
{
    [DataContract]
    public class UserIncidentMapModel
    {
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public int IncidentId { get; set; }
    }

    public static class UserIncidentMapConvert
    {
        public static UserIncidentMapEntity ConvertCompanyModelToEntity(UserIncidentMapModel model)
        {
            UserIncidentMapEntity entity = new UserIncidentMapEntity();
            entity.UserId = model.UserId;
            entity.IncidentId = model.IncidentId;
            return entity;
        }
        public static UserIncidentMapModel ConvertCompanyEntityToModel(UserIncidentMapEntity entity)
        {
            UserIncidentMapModel model = new UserIncidentMapModel();
            entity.UserId = model.UserId;
            entity.IncidentId = model.IncidentId;
            return model;
        }
        public static UserIncidentMapEntity GetCompanyEntity(int userId, int incidentId)
        {
            UserIncidentMapEntity entity = new UserIncidentMapEntity();
            entity.UserId = userId;
            entity.IncidentId = incidentId;
            return entity;
        }
    }
}