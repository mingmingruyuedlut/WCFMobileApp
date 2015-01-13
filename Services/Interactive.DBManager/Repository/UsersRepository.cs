using Interactive.Common;
using Interactive.Constant;
using Interactive.DBManager.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Interactive.DBManager.Repository
{
    public class UsersRepository
    {
        #region Create password

        public void CreatePassword(string token, string password)
        {
            string sqlInsert = "UPDATE Users SET Password = @Password WHERE Id = (SELECT UserId FROM Tokens WHERE Value = @Value)";
            SqlParameter[] parms = {				   
				new SqlParameter("@Value", SqlDbType.NVarChar,100),
				new SqlParameter("@Password", SqlDbType.NVarChar,200)};
            parms[0].Value = token;
            parms[1].Value = Password.CreateHash(password);
            SqlHelper.ExcuteNonQuery(CommandType.Text, sqlInsert, parms);
        }

        #endregion

        #region Update password

        public void UpdatePassword(string token, string password)
        {
            string sqlInsert = "UPDATE Users SET Password = @Password WHERE Id = (SELECT UserId FROM Tokens WHERE Value = @Value)";
            SqlParameter[] parms = {				   
				new SqlParameter("@Value", SqlDbType.NVarChar,100),
				new SqlParameter("@Password", SqlDbType.NVarChar,200)};
            parms[0].Value = token;
            parms[1].Value = Password.CreateHash(password);
            SqlHelper.ExcuteNonQuery(CommandType.Text, sqlInsert, parms);
        }

        #endregion

        #region Create new user and token

        public void CreateUserAndToken(string mail, int companyId, string billingId, string token, TokenType tokenType)
        {
            SqlParameter[] parms = {				   
				new SqlParameter("@UMail", SqlDbType.NVarChar,100),
                new SqlParameter("@CompanyId", SqlDbType.Int),
                new SqlParameter("@BillingId", SqlDbType.NVarChar,254),
				new SqlParameter("@TokenValue", SqlDbType.NVarChar,100),
				new SqlParameter("@TokenType", SqlDbType.Int)};
            parms[0].Value = mail;
            parms[1].Value = companyId;
            parms[2].Value = billingId;
            parms[3].Value = token;
            parms[4].Value = tokenType;
            SqlHelper.ExcuteNonQuery(CommandType.StoredProcedure, "AddUser", parms);
        }

        #endregion

        #region Check mail in DB

        public bool CheckMailInDB(string mail)
        {
            string sqlCheck = "SELECT COUNT(*) FROM Users WHERE UMail = @UMail";
            SqlParameter[] parms = {				   
				new SqlParameter("@UMail", SqlDbType.NVarChar,100)};
            parms[0].Value = mail;
            int i = Convert.ToInt32(SqlHelper.ExcuteScalar(CommandType.Text, sqlCheck, parms));
            if (i > 0)
                return true;
            else
                return false;
        }

        #endregion

        #region Check password exist

        public bool CheckPasswordExist(string mail)
        {
            string sqlCheck = "SELECT Password FROM Users WHERE UMail = @UMail";
            SqlParameter[] parms = {				   
				new SqlParameter("@UMail", SqlDbType.NVarChar,100)};
            parms[0].Value = mail;
            object i = SqlHelper.ExcuteScalar(CommandType.Text, sqlCheck, parms);
            if (i != DBNull.Value)
                return true;
            else
                return false;
        }

        #endregion

        #region Get corrent password

        public string GetCorrectPassword(string mail)
        {
            string sqlGetPwd = "SELECT Password FROM Users WHERE UMail = @UMail";
            SqlParameter[] pwdParms = {				   
				new SqlParameter("@UMail", SqlDbType.NVarChar,100)};
            pwdParms[0].Value = mail;
            DataSet ds = SqlHelper.ExcuteDataSet(CommandType.Text, sqlGetPwd, pwdParms);
            if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return "";
        }

        #endregion

        #region Get userid from email

        public int GetUserIdFromEmail(string mail)
        {
            string strGet = "SELECT Id FROM Users WHERE UMail = @UMail";
            SqlParameter[] parms = {				   
				new SqlParameter("@UMail", SqlDbType.NVarChar,100)};
            parms[0].Value = mail;
            DataSet ds = SqlHelper.ExcuteDataSet(CommandType.Text, strGet, parms);
            return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
        }

        #endregion

        #region Get userid by token

        public int GetUserIdByToken(string token)
        {
            string strGet = "SELECT UserId FROM Tokens WHERE Value = @Value";
            SqlParameter[] parms = {				   
				new SqlParameter("@Value", SqlDbType.NVarChar,100)};
            parms[0].Value = token;
            DataSet ds = SqlHelper.ExcuteDataSet(CommandType.Text, strGet, parms);
            return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
        }

        #endregion

        public void UpdateCompanyIdAndBillingId(int companyId, string billingId, string email)
        {
            string sqlUpdate = "UPDATE Users SET CompanyId = @CompanyId, BillingId = @BillingId WHERE UMail = @UMail";
            SqlParameter[] parms = {				   
				new SqlParameter("@CompanyId", SqlDbType.Int),
				new SqlParameter("@BillingId", SqlDbType.NVarChar,254)};
            parms[0].Value = companyId;
            parms[1].Value = billingId;
            SqlHelper.ExcuteNonQuery(CommandType.Text, sqlUpdate, parms);
        }

        public List<UserEntity> GetOtherUsersToShareIncident(UserEntity user, int incidentId)
        {
            string strGet = "SELECT Id, UMail, CompanyId, BillingId FROM Users WHERE CompanyId = @CompanyId AND Password IS NOT NULL AND Id != @Id";
            SqlParameter[] parms = {				   
				new SqlParameter("@CompanyId", SqlDbType.Int),
                new SqlParameter("@Id", SqlDbType.Int)};
            parms[0].Value = user.CompanyId;
            parms[1].Value = user.Id;
            DataSet ds = SqlHelper.ExcuteDataSet(CommandType.Text, strGet, parms);
            return ConvertDataSetToUserEntityList(ds);
        }

        public List<UserEntity> GetSharedUsers(UserEntity user, int incidentId)
        {
            string strGet = "SELECT Id, UMail, CompanyId, BillingId FROM Users WHERE CompanyId = @CompanyId AND Password IS NOT NULL AND Id IN (SELECT UserId FROM UserIncidentMapping WHERE IncidentId = @IncidentId)";
            SqlParameter[] parms = {				   
				new SqlParameter("@CompanyId", SqlDbType.Int),
                new SqlParameter("@IncidentId", SqlDbType.Int)};
            parms[0].Value = user.CompanyId;
            parms[1].Value = incidentId;
            DataSet ds = SqlHelper.ExcuteDataSet(CommandType.Text, strGet, parms);
            return ConvertDataSetToUserEntityList(ds);
        }

        public List<UserEntity> GetSharedUsersExceptOwner(UserEntity user, int incidentId)
        {
            string strGet = "SELECT Id, UMail, CompanyId, BillingId FROM Users WHERE CompanyId = @CompanyId AND Password IS NOT NULL AND Id IN (SELECT UserId FROM UserIncidentMapping WHERE IncidentId = @IncidentId) AND Id != @Id";
            SqlParameter[] parms = {				   
				new SqlParameter("@CompanyId", SqlDbType.Int),
                new SqlParameter("@IncidentId", SqlDbType.Int),
                new SqlParameter("@Id", SqlDbType.Int)};
            parms[0].Value = user.CompanyId;
            parms[1].Value = incidentId;
            parms[2].Value = user.Id;
            DataSet ds = SqlHelper.ExcuteDataSet(CommandType.Text, strGet, parms);
            return ConvertDataSetToUserEntityList(ds);
        }

        public bool CheckUserIsIncidentOwner(UserEntity user, int incidentId)
        {
            string sqlCheck = "SELECT COUNT(*) FROM Incidents WHERE Id = @Id AND UserId = @UserId";
            SqlParameter[] parms = {				   
				new SqlParameter("@Id", SqlDbType.Int),
                new SqlParameter("@UserId", SqlDbType.Int)};
            parms[0].Value = incidentId;
            parms[1].Value = user.Id;
            int i = Convert.ToInt32(SqlHelper.ExcuteScalar(CommandType.Text, sqlCheck, parms));
            if (i > 0)
                return true;
            else
                return false;
        }

        public UserEntity GetUserByEmail(UserEntity user)
        {
            string strGet = "SELECT Id, UMail, CompanyId, BillingId FROM Users WHERE UMail = @UMail";
            SqlParameter[] parms = {				   
				new SqlParameter("@UMail", SqlDbType.NVarChar, 100)};
            parms[0].Value = user.Email;
            DataSet ds = SqlHelper.ExcuteDataSet(CommandType.Text, strGet, parms);
            return ConvertDataSetToUserEntity(ds);
        }

        private UserEntity ConvertDataSetToUserEntity(DataSet ds)
        {
            UserEntity userE = new UserEntity();
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                userE.Id = Int32.Parse(ds.Tables[0].Rows[0][0].ToString());
                userE.Email = ds.Tables[0].Rows[0][1].ToString();
                userE.CompanyId = Int32.Parse(ds.Tables[0].Rows[0][2].ToString());
                userE.BillingId = ds.Tables[0].Rows[0][3].ToString();
            }
            return userE;
        }

        private List<UserEntity> ConvertDataSetToUserEntityList(DataSet ds)
        {
            List<UserEntity> userEntityList = new List<UserEntity>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    for (int j = 0; j < ds.Tables[i].Rows.Count; j++)
                    {
                        UserEntity userE = new UserEntity();
                        userE.Id = Int32.Parse(ds.Tables[i].Rows[j][0].ToString());
                        userE.Email = ds.Tables[i].Rows[j][1].ToString();
                        userE.CompanyId = Int32.Parse(ds.Tables[i].Rows[j][2].ToString());
                        userE.BillingId = ds.Tables[i].Rows[j][3].ToString();
                        userEntityList.Add(userE);
                    }
                }
            }
            return userEntityList;
        }
    }
}
