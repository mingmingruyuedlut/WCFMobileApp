using Interactive.DBManager.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive.DBManager.Repository
{
    public class CompanyRepository
    {
        public void AddCompany(CompanyEntity company)
        {
            string strInsert = "INSERT INTO Companies(Name) VALUES (@Name)";
            SqlParameter[] parms = {
                new SqlParameter("@Name", SqlDbType.NVarChar, 254)};
            parms[0].Value = company.Name;
            SqlHelper.ExcuteNonQuery(CommandType.Text, strInsert, parms);
        }

        public CompanyEntity DeleteCompany()
        {
            return null;
        }

        public CompanyEntity UpdateCompany()
        {
            return null;
        }

        public CompanyEntity SelectCompany()
        {
            return null;
        }

        public bool CheckCompanyByName(string name)
        {
            string sqlCheck = "SELECT COUNT(*) FROM Companies WHERE Name = @Name";
            SqlParameter[] parms = {				   
				new SqlParameter("@Name", SqlDbType.NVarChar,254)};
            parms[0].Value = name;
            int i = Convert.ToInt32(SqlHelper.ExcuteScalar(CommandType.Text, sqlCheck, parms));
            if (i > 0)
                return true;
            else
                return false;
        }

        public CompanyEntity GetCompanyByName(string name)
        {
            string sqlGet = "SELECT Id, Name FROM Companies WHERE Name = @Name";
            SqlParameter[] parms = {				   
				new SqlParameter("@Name", SqlDbType.NVarChar, 254)};
            parms[0].Value = name;
            DataSet ds = SqlHelper.ExcuteDataSet(CommandType.Text, sqlGet, parms);

            return ConvertDataSetToCompanyEntity(ds);
        }

        private CompanyEntity ConvertDataSetToCompanyEntity(DataSet ds)
        {
            CompanyEntity companyE = new CompanyEntity();
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                companyE.Id = Int32.Parse(ds.Tables[0].Rows[0][0].ToString());
                companyE.Name = ds.Tables[0].Rows[0][1].ToString();
            }
            return companyE;
        }
    }
}
