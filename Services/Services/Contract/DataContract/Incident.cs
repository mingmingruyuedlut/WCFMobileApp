using Interactive.Constant;
using Interactive.DBManager.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Interactive.Services.Contract
{
    [DataContract]
    public class Incident
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string IdInFP { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public long TimeStamp { get; set; }
        [DataMember]
        public string Service { get; set; }
        [DataMember]
        public Severity Severity { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public IncidentStatusHistory StatusHistory { get; set; }
        [DataMember]
        public Technician Technician { get; set; }
        [DataMember]
        public bool IsShared { get; set; }
        [DataMember]
        public int ShareCount { get; set; }
        [DataMember]
        public bool IsOwner { get; set; }
        [DataMember]
        public string ClosureCode { get; set; }
        [DataMember]
        public string Resolution { get; set; }
        [DataMember]
        public string Device { get; set; }
        [DataMember]
        public string DeviceLocation { get; set; }
        [DataMember]
        public string SerialNumber { get; set; }
        [DataMember]
        public string ContactedSLA { get; set; }
        [DataMember]
        public string OnsiteSLA { get; set; }
        [DataMember]
        public string MrTitle { get; set; }
    }

    [DataContract]
    public class IncidentStatusHistory
    {
        [DataMember]
        public IncidentStatus Open { get; set; }
        [DataMember]
        public IncidentStatus Contacted { get; set; }
        [DataMember]
        public IncidentStatus Onsite { get; set; }
        [DataMember]
        public IncidentStatus Resolved { get; set; }
    }

    [DataContract]
    public class IncidentStatus
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public long TimeStamp { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
    }

    [DataContract]
    public enum StatusType
    {
        [EnumMember]
        Open,
        [EnumMember]
        CallBack,
        [EnumMember]
        CallOpen,
        [EnumMember]
        CallAllocated,
        [EnumMember]
        Contact,
        [EnumMember]
        Onsite,
        [EnumMember]
        Scheduled,
        [EnumMember]
        Validate,
        [EnumMember]
        RectificationInProgress,
        [EnumMember]
        Resolved,
        [EnumMember]
        Closed
    }

    [DataContract]
    public enum ServiceType
    {
        [EnumMember]
        CloudManaged,
        [EnumMember]
        DataCentre,
        [EnumMember]
        BusinessContinuity,
        [EnumMember]
        HardwareMaintenance
    }

    [DataContract]
    public enum SLAType
    {
        [EnumMember]
        Passed,
        [EnumMember]
        Failed
    }

    [DataContract]
    public class IncidentModel
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string FPIncidentId { get; set; }
        [DataMember]
        public string UserId { get; set; }
        [DataMember]
        public string SubmitterEmail { get; set; }
        [DataMember]
        public IncidentModelStatus Status { get; set; }
        [DataMember]
        public IncidentType Type { get; set; }
    }

    public static class IncidentModelConvert
    {
        public static IncidentEntity ConvertIncidentModelToEntity(IncidentModel model)
        {
            IncidentEntity entity = new IncidentEntity();
            entity.Id = !string.IsNullOrEmpty(model.Id) ? Int32.Parse(model.Id) : 0;
            entity.FPIncidentId = !string.IsNullOrEmpty(model.FPIncidentId) ? Int32.Parse(model.FPIncidentId) : 0;
            entity.UserId = !string.IsNullOrEmpty(model.UserId) ? Int32.Parse(model.UserId) : 0;
            entity.SubmitterEmail = model.SubmitterEmail;
            entity.Status = model.Status;
            entity.Type = model.Type;
            return entity;
        }

        public static IncidentModel ConvertIncidentEntityToModel(IncidentEntity entity)
        {
            IncidentModel model = new IncidentModel();
            model.Id = entity.Id.ToString();
            model.FPIncidentId = entity.FPIncidentId.ToString();
            model.UserId = entity.UserId.ToString();
            model.SubmitterEmail = entity.SubmitterEmail;
            model.Status = entity.Status;
            model.Type = entity.Type;
            return model;
        }

        public static IncidentModel GetIncidentModel(string id, string fpIncidentId, string userId, string email, IncidentModelStatus status, IncidentType type)
        {
            IncidentModel model = new IncidentModel();
            model.Id = id;
            model.FPIncidentId = fpIncidentId;
            model.UserId = userId;
            model.SubmitterEmail = email;
            model.Status = status;
            model.Type = type;
            return model;
        }
    }

}