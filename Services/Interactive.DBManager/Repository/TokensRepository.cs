using Interactive.Constant;
using Interactive.DBManager.Entity;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Interactive.DBManager.Repository
{
    public class TokensRepository
    {
        #region Check token exipired

        public DataSet CheckTokenExpired(string token)
        {
            string sqlCheck = "SELECT StartTime,TokenType FROM Tokens WHERE Value = @Value";
            SqlParameter[] checkParms = {				   
				new SqlParameter("@Value", SqlDbType.NVarChar,100)};
            checkParms[0].Value = token;
            DataSet ds = SqlHelper.ExcuteDataSet(CommandType.Text, sqlCheck, checkParms);
            return ds;
        }

        #endregion

        #region Delete expired token

        public void DeleteExpiredToken(string token)
        {
            string sqlDelete = "DELETE FROM Tokens WHERE Value = @Value";
            SqlParameter[] deleteParms = {				   
				new SqlParameter("@Value", SqlDbType.NVarChar,100)};
            deleteParms[0].Value = token;
            SqlHelper.ExcuteNonQuery(CommandType.Text, sqlDelete, deleteParms);
        }

        #endregion

        #region Check token exist

        public bool CheckTokenExist(string tokenValue)
        {
            string sqlCheck = "SELECT COUNT(*) FROM Tokens WHERE Value = @Value";
            SqlParameter[] parms = {				   
				new SqlParameter("@Value", SqlDbType.NVarChar,100)};
            parms[0].Value = tokenValue;
            int i = Convert.ToInt32(SqlHelper.ExcuteScalar(CommandType.Text, sqlCheck, parms));
            if (i > 0)
                return true;
            else
                return false;
        }

        #endregion

        #region Check login token exist

        public bool CheckLoginTokenExist(string mail)
        {
            string sqlCheck = "SELECT COUNT(*) FROM Tokens WHERE TokenType = 1 AND UserId = (SELECT Id FROM Users WHERE UMail = @UMail)";
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

        #region Check login token exist

        public bool CheckForgetPwdTokenExist(string mail)
        {
            string sqlCheck = "SELECT COUNT(*) FROM Tokens WHERE TokenType = 2 AND UserId = (SELECT Id FROM Users WHERE UMail = @UMail)";
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

        #region Get token from mail

        public string GetTokenFromMail(string mail, TokenType type)
        {
            string sqlGet = "SELECT Value FROM Tokens WHERE UserId = (SELECT Id FROM Users WHERE UMail = @UMail) AND TokenType = @TokenType";
            SqlParameter[] parms = {				   
				new SqlParameter("@UMail", SqlDbType.NVarChar,100),
                new SqlParameter("@TokenType", SqlDbType.Int)};
            parms[0].Value = mail;
            parms[1].Value = type;
            DataSet ds = SqlHelper.ExcuteDataSet(CommandType.Text, sqlGet, parms);
            return ds.Tables[0].Rows[0][0].ToString();
        }

        #endregion

        #region Add login token

        public void AddLoginToken(string mail, string token, TokenType tokenType)
        {
            UsersRepository _userPository = new UsersRepository();
            string strInsert = "INSERT INTO Tokens(UserId,Value,StartTime,TokenType) VALUES (@UserId,@Value,@StartTime,@TokenType)";
            SqlParameter[] parms = {				   
				new SqlParameter("@UserId", SqlDbType.Int),
				new SqlParameter("@Value", SqlDbType.NVarChar,200),
				new SqlParameter("@StartTime", SqlDbType.DateTime),
				new SqlParameter("@TokenType", SqlDbType.Int)};
            parms[0].Value = _userPository.GetUserIdFromEmail(mail);
            parms[1].Value = token;
            parms[2].Value = DateTime.Now;
            parms[3].Value = tokenType;
            SqlHelper.ExcuteNonQuery(CommandType.Text, strInsert, parms);
        }

        #endregion

        #region Return token string

        public string TokenString()
        {
            return Guid.NewGuid().ToString("N");
        }

        #endregion

        #region Update token when login
        public void UpdateToken(string tokenValue)
        {
            string strUpdate = "UPDATE Tokens SET StartTime = @StartTime WHERE Value = @Value";
            SqlParameter[] parms = {				   
				new SqlParameter("@StartTime", SqlDbType.DateTime),
				new SqlParameter("@Value", SqlDbType.NVarChar,100)};
            parms[0].Value = DateTime.Now;
            parms[1].Value = tokenValue;
            SqlHelper.ExcuteNonQuery(CommandType.Text, strUpdate, parms);
        }
        #endregion

        #region Get user email by token

        public string GetUserEmailByToken(string token)
        {
            string sqlGet = "SELECT UMail FROM Users WHERE Id = (SELECT UserId FROM Tokens WHERE Value = @Value)";
            SqlParameter[] parms = {				   
				new SqlParameter("@Value", SqlDbType.NVarChar, 100)};
            parms[0].Value = token;
            DataSet ds = SqlHelper.ExcuteDataSet(CommandType.Text, sqlGet, parms);
            return ds.Tables[0].Rows[0][0].ToString();
        }

        #endregion
    }
}
