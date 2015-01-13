using Interactive.Constant;
using Interactive.DBManager.Entity;
using Interactive.DBManager.Repository;
using System;
using System.Configuration;
using System.Data;
using System.Net;
using System.ServiceModel.Web;

namespace Interactive.DBManager.Service
{
    public class TokensRepService
    {
        #region Field and Property
        private TokensRepository tokenRep;
        internal TokensRepository _tokenRep
        {
            get
            {
                if (this.tokenRep == null)
                {
                    this.tokenRep = new TokensRepository();
                }
                return this.tokenRep;
            }
        }

        private UsersRepository userRep;
        internal UsersRepository _userRep
        {
            get
            {
                if (this.userRep == null)
                {
                    this.userRep = new UsersRepository();
                }
                return this.userRep;
            }
        }
        #endregion



        #region Check mail token

        public TokenEntity CheckToken(string tokenValue)
        {
            TokenEntity _tokenEntity = new TokenEntity();

            _tokenEntity.IsTokenExist = true;
            _tokenEntity.IsTokenExpired = false;

            if (_tokenRep.CheckTokenExist(tokenValue))
            {
                if (CheckTokenExpired(tokenValue))
                {
                    _tokenRep.DeleteExpiredToken(tokenValue);
                    _tokenEntity.IsTokenExpired = true;
                }
            }
            else
            {
                _tokenEntity.IsTokenExist = false;
            }
            return _tokenEntity;
        }

        #endregion

        #region Provide token for mail address

        public string ProvideTokenForAddress(string mail, int companyId, string billingId)
        {
            string result = _tokenRep.TokenString();

            if (_userRep.CheckMailInDB(mail))
            {
                if (CheckTokenExpired(_tokenRep.GetTokenFromMail(mail, TokenType.Registration)))
                {
                    _tokenRep.DeleteExpiredToken(_tokenRep.GetTokenFromMail(mail, TokenType.Registration));
                    _userRep.CreateUserAndToken(mail, companyId, billingId, result, TokenType.Registration);
                }
                else
                {
                    result = _tokenRep.GetTokenFromMail(mail, TokenType.Registration);
                }
            }
            else
            {
                _userRep.CreateUserAndToken(mail, companyId, billingId, result, TokenType.Registration);
            }
            return result;
        }

        #endregion

        #region Get token when login

        public string GetTokenWhenLogin(string mail)
        {
            string result = _tokenRep.TokenString();
            if (_tokenRep.CheckLoginTokenExist(mail))
            {
                if (CheckTokenExpired(_tokenRep.GetTokenFromMail(mail, TokenType.Login)))
                {
                    _tokenRep.DeleteExpiredToken(_tokenRep.GetTokenFromMail(mail, TokenType.Login));
                    _tokenRep.AddLoginToken(mail, result, TokenType.Login);
                }
                else
                {
                    result = _tokenRep.GetTokenFromMail(mail, TokenType.Login);
                }
            }
            else
            {
                _tokenRep.AddLoginToken(mail, result, TokenType.Login);
            }

            return result;
        }

        #endregion

        #region Check token expired

        public bool CheckTokenExpired(string token)
        {
            bool result = false;
            DataSet ds = _tokenRep.CheckTokenExpired(token);
            DateTime expiredDate = DateTime.Now;

            if (string.Equals(ds.Tables[0].Rows[0][1].ToString(), ((int)TokenType.Registration).ToString()))
            {
                expiredDate = Convert.ToDateTime(ds.Tables[0].Rows[0][0]).AddHours(Convert.ToInt32(ConfigurationManager.AppSettings["TokenExpire"]));
            }
            else if (string.Equals(ds.Tables[0].Rows[0][1].ToString(), ((int)TokenType.Login).ToString()))
            {
                expiredDate = Convert.ToDateTime(ds.Tables[0].Rows[0][0]).AddHours(Convert.ToDouble(ConfigurationManager.AppSettings["TokenLoginExpire"]));
            }
            else if (string.Equals(ds.Tables[0].Rows[0][1].ToString(), ((int)TokenType.ResetPassword).ToString()))
            {
                expiredDate = Convert.ToDateTime(ds.Tables[0].Rows[0][0]).AddHours(Convert.ToInt32(ConfigurationManager.AppSettings["TokenForgetPwdExpire"]));
            }

            if (DateTime.Now > expiredDate)
            {
                result = true;
            }
            else
            {
                //if login token and not expired
                if (string.Equals(ds.Tables[0].Rows[0][1].ToString(), ConfigurationManager.AppSettings["LoginTokenType"]))
                {
                    _tokenRep.UpdateToken(token);
                }
            }
            return result;
        }

        #endregion

        #region Delete expired token

        public void DeleteExpiredToken(string token)
        {
            _tokenRep.DeleteExpiredToken(token);
        }

        #endregion

        #region Get token when forget password

        public string GetTokenWhenForgetPwd(string mail)
        {
            string result = _tokenRep.TokenString();

            if (_tokenRep.CheckForgetPwdTokenExist(mail))
            {
                if (CheckTokenExpired(_tokenRep.GetTokenFromMail(mail, TokenType.ResetPassword)))
                {
                    _tokenRep.DeleteExpiredToken(_tokenRep.GetTokenFromMail(mail, TokenType.ResetPassword));
                    _tokenRep.AddLoginToken(mail, result, TokenType.ResetPassword);
                }
                else
                {
                    result = _tokenRep.GetTokenFromMail(mail, TokenType.ResetPassword);
                }
            }
            else
            {
                _tokenRep.AddLoginToken(mail, result, TokenType.ResetPassword);
            }
            return result;
        }

        #endregion

        public string GetUserEmailByToken(string token)
        {
            return _tokenRep.GetUserEmailByToken(token);
        }
    }
}
