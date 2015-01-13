using Interactive.DBManager.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Interactive.Services.Contract
{
    [DataContract]
    public class CompanyModel
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
    }

    public static class CompanyConvert
    {
        public static CompanyEntity ConvertCompanyModelToEntity(CompanyModel model)
        {
            CompanyEntity entity = new CompanyEntity();
            entity.Id = model.Id;
            entity.Name = model.Name;
            return entity;
        }
        public static CompanyModel ConvertCompanyEntityToModel(CompanyEntity entity)
        {
            CompanyModel model = new CompanyModel();
            model.Id = entity.Id;
            model.Name = entity.Name;
            return model;
        }
        public static CompanyEntity GetCompanyEntity(int id, string name)
        {
            CompanyEntity entity = new CompanyEntity();
            entity.Id = id;
            entity.Name = !string.IsNullOrEmpty(name) ? name : string.Empty;
            return entity;
        }
        public static CompanyEntity GetCompanyEntity(string name)
        {
            CompanyEntity entity = new CompanyEntity();
            entity.Name = !string.IsNullOrEmpty(name) ? name : string.Empty;
            return entity;
        }
    }
}