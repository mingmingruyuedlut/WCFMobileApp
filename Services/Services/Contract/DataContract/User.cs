using Interactive.DBManager.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Interactive.Services.Contract
{
    [DataContract]
    public class CheckUser
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public bool IsExistInFP { get; set; }
        [DataMember]
        public bool IsExistInMDB { get; set; }
        [DataMember]
        public bool IsWaitingFPResponse { get; set; }

        [DataMember]
        public string CompanyName { get; set; }
        [DataMember]
        public string BillingId { get; set; }
    }

    [DataContract]
    public class RegisterUser
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public string Customer { get; set; }
    }

    [DataContract]
    public class UserModel
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string CompanyId { get; set; }
        [DataMember]
        public string BillingId { get; set; }
        [DataMember]
        public string ContactNumber { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool IsShared { get; set; }
    }

    public static class UserModelConvert
    {
        public static UserEntity ConvertCompanyModelToEntity(UserModel model)
        {
            UserEntity entity = new UserEntity();
            entity.Id = model.Id != null ? Int32.Parse(model.Id) : 0;
            entity.Email = model.Email;
            entity.CompanyId = model.Id != null ? Int32.Parse(model.CompanyId) : 0;
            entity.BillingId = model.BillingId;
            return entity;
        }
        public static UserModel ConvertCompanyEntityToModel(UserEntity entity)
        {
            UserModel model = new UserModel();
            model.Id = entity.Id.ToString();
            model.Email = entity.Email;
            model.CompanyId = entity.CompanyId.ToString();
            model.BillingId = entity.BillingId;
            return model;
        }
        public static List<UserEntity> ConvertCompanyModelToEntity(List<UserModel> modelList)
        {
            List<UserEntity> entityList = new List<UserEntity>();
            foreach (UserModel model in modelList)
            {
                entityList.Add(ConvertCompanyModelToEntity(model));
            }
            return entityList;
        }
        public static List<UserModel> ConvertCompanyEntityToModel(List<UserEntity> entityList)
        {
            List<UserModel> modelList = new List<UserModel>();
            foreach (UserEntity entity in entityList)
            {
                modelList.Add(ConvertCompanyEntityToModel(entity));
            }
            return modelList;
        }
        public static UserModel GetUserModel(string email)
        {
            UserModel model = new UserModel();
            model.Email = email;
            return model;
        }
    }
}