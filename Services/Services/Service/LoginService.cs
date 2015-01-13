using Interactive.DBManager.Service;
using Interactive.Services.Contract;
using Interactive.Constant;
using System;
using System.ServiceModel.Web;
using Services.Manager;
using Interactive.Common;
using System.Configuration;

namespace Services.Service
{
    public class LoginService
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

        private UsersRepService userRepService;
        internal UsersRepService _userRepService
        {
            get
            {
                if (this.userRepService == null)
                {
                    this.userRepService = new UsersRepService();
                }
                return this.userRepService;
            }
        }

        private CommonService commonService;
        internal CommonService _commonService
        {
            get
            {
                if (this.commonService == null)
                {
                    this.commonService = new CommonService();
                }
                return this.commonService;
            }
        }

        private FootprintsV11Manager fpManager = null;

        #endregion

        #region LoginService

        public LoginService()
        {
            ProviderSection config = (ProviderSection)ConfigurationManager.GetSection("ServiceProxy");
            Type _typeProxy = null;

            foreach (ProviderSettings p in config.Providers)
            {
                if (p.Name == config.DefaultProvider)
                {
                    _typeProxy = Type.GetType(p.Type);
                    break;
                }
            }

            fpManager = (FootprintsV11Manager)Activator.CreateInstance(_typeProxy);
        }

        #endregion

        #region Check login

        public Login IsLogin(Login login)
        {
            Login loginModel = new Login();
            try
            {
                _logInfo.InfoFormat(LogConstant.LoginService_IsLogin_Start, login.Email);

                var loginEntity = _userRepService.CheckLogin(login.Email, login.Password);
                loginModel.IsEmailExist = loginEntity.IsEmailExist;
                loginModel.IsPwdValid = loginEntity.IsPwdValid;
                loginModel.Token = loginEntity.Token;

                //get contact name and contact number from footprint
                UserModel userM = fpManager.GetUserDetail(login.Email);
                loginModel.ContactName = userM.Name;
                loginModel.ContactNumber = userM.ContactNumber;

                _logInfo.InfoFormat(LogConstant.LoginService_IsLogin_End, login.Email);
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.LoginService_IsLogin_Error, ex);
            }
            return loginModel;
        }

        #endregion

        #region Log out

        public Login IsLogout()
        {
            Login loginModel = new Login();
            try
            {
                string token = _commonService.GetHttpHeaderToken();
                string email = _tokenRepService.GetUserEmailByToken(token);

                _logInfo.InfoFormat(LogConstant.LoginService_IsLogout_Start, email);

                _tokenRepService.DeleteExpiredToken(token);
                loginModel.IsLogout = true;

                _logInfo.InfoFormat(LogConstant.LoginService_IsLogout_End, email);
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.LoginService_IsLogout_Error, ex);
            }
            return loginModel;
        }

        #endregion
    }
}