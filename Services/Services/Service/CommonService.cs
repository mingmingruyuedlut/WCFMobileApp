using Interactive.Common;
using Interactive.Constant;
using Interactive.DBManager.Entity;
using Interactive.DBManager.Service;
using Interactive.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Web;

namespace Services.Service
{
    public class CommonService
    {

        #region Field and Property
        private log4net.ILog logInfo;
        internal log4net.ILog _logInfo
        {
            get
            {
                if (this.logInfo == null)
                {
                    this.logInfo = log4net.LogManager.GetLogger(LogConstant.LogInfoFile);
                }
                return this.logInfo;
            }
        }

        private log4net.ILog logError;
        internal log4net.ILog _logError
        {
            get
            {
                if (this.logError == null)
                {
                    this.logError = log4net.LogManager.GetLogger(LogConstant.LogErrorFile);
                }
                return this.logError;
            }
        }

        private TokensRepService tokenRepService;
        internal TokensRepService _tokenRepService
        {
            get
            {
                if (this.tokenRepService == null)
                {
                    this.tokenRepService = new TokensRepService();
                }
                return this.tokenRepService;
            }
        }

        #endregion


        #region Send mail when register

        public void SendConfirmationMail(string mail, string token)
        {
            try
            {
                Email _mail = new Email();
                List<string> listEmail = new List<string>();

                //add current user mail
                listEmail.Add(mail);

                //add test mail user
                foreach (string s in Email.TestEmailAddress())
                {
                    listEmail.Add(s);
                }

                string serverHost = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Length - System.Web.HttpContext.Current.Request.Url.AbsolutePath.Length);
                string link = serverHost + "/#/app/set-new-password?token=" + token + "";

                //add mail image
                Dictionary<string, string> lrDic = new Dictionary<string, string>();
                lrDic.Add("logoImage", "logo.png");
                lrDic.Add("logoFooterImage", "logo-footer.png");

                _mail.SendMail(listEmail, EmailHTML.ConfirmationEmailSubject, Email.EmailBodyTemplate(link, EmailHTML.ConfirmationEmailContent(mail)), lrDic);
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.CommonService_SendConfirmationMail_Error, ex);
            }
        }

        #endregion

        #region Send notice mail when create incident

        public void SendRequestSubmitMail(string mail)
        {
            try
            {
                Email _mail = new Email();
                List<string> listEmail = new List<string>();

                //add current user mail
                listEmail.Add(mail);

                //add test mail user
                foreach (string s in Email.TestEmailAddress())
                {
                    listEmail.Add(s);
                }

                //add mail image
                Dictionary<string, string> lrDic = new Dictionary<string, string>();
                lrDic.Add("logoImage", "logo.png");
                lrDic.Add("logoFooterImage", "logo-footer.png");
                _mail.SendMail(listEmail, EmailHTML.RequestSubmitEmailSubject, Email.EmailBodyTemplate(null, EmailHTML.RequestSubmitEmailContent), lrDic);
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.CommonService_SendRequestSubmitMail_Error, ex);
            }
        }

        #endregion

        #region Send mail when register

        public void SendForgetPwdMail(string mail, string token)
        {
            try
            {
                Email _mail = new Email();
                List<string> listEmail = new List<string>();

                //add current user mail
                listEmail.Add(mail);

                string serverHost = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Length - System.Web.HttpContext.Current.Request.Url.AbsolutePath.Length);
                string link = serverHost + "/#/app/reset-password?token=" + token;

                //add mail image
                Dictionary<string, string> lrDic = new Dictionary<string, string>();
                lrDic.Add("logoImage", "logo.png");
                lrDic.Add("logoFooterImage", "logo-footer.png");

                _mail.SendMail(listEmail, EmailHTML.ForgetPasswordSubject, Email.EmailBodyTemplate(link, EmailHTML.ForgetPasswordContent(mail)), lrDic);
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.CommonService_SendForgetPwdMail_Error, ex);
            }
        }

        #endregion

        #region Get token from http header

        public string GetHttpHeaderToken()
        {
            string value = string.Empty;
            WebHeaderCollection headerCollection = WebOperationContext.Current.IncomingRequest.Headers;
            value = headerCollection["Authorization"];
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Substring(Defaults.TokenPrefix.Length);
            }
            return value;
        }

        #endregion

    }
}