using Interactive.DBManager.Entity;
using Interactive.DBManager.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive.DBManager.Service
{
    public class CompanyRepService
    {
        #region Field and Property
        private CompanyRepository companyRep;
        internal CompanyRepository CompanyRep
        {
            get
            {
                if (this.companyRep == null)
                {
                    this.companyRep = new CompanyRepository();
                }
                return this.companyRep;
            }
        }
        #endregion

        public void CreateCompany(CompanyEntity company)
        {
            if (!CompanyRep.CheckCompanyByName(company.Name))
            {
                CompanyRep.AddCompany(company);
            }
        }

        public CompanyEntity GetCompanyByName(string name)
        {
            return CompanyRep.GetCompanyByName(name);
        }
    }
}
