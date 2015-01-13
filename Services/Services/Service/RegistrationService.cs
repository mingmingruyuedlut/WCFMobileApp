using Interactive.Common;
using Interactive.Constant;
using Interactive.DBManager.Service;
using Interactive.Services.Contract;
using Services.Manager;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Services.Service
{
    public class RegistrationService
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

        private IncidentRepService incidentRepService;
        internal IncidentRepService _incidentRepService
        {
            get
            {
                if (this.incidentRepService == null)
                {
                    this.incidentRepService = new IncidentRepService();
                }
                return this.incidentRepService;
            }
        }

        private CompanyRepService companyRepService;
        internal CompanyRepService _companyRepService
        {
            get
            {
                if (this.companyRepService == null)
                {
                    this.companyRepService = new CompanyRepService();
                }
                return this.companyRepService;
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

        #region RegistrationService

        public RegistrationService()
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

        #region Create password

        public SetPassword CreatePassword(SetPassword password)
        {
            try
            {
                string httpToken = _commonService.GetHttpHeaderToken();

                string userEmail = _tokenRepService.GetUserEmailByToken(httpToken);
                int userId = _userRepService.GetUserIdByEmail(userEmail);

                IncidentModel incidentM = IncidentModelConvert.GetIncidentModel(null, null, userId.ToString(), userEmail, IncidentModelStatus.Closed, IncidentType.Registration);
                _incidentRepService.UpdateRegistrationIncident(IncidentModelConvert.ConvertIncidentModelToEntity(incidentM));

                _userRepService.CreatePassword(httpToken, password.Password);
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.RegistrationService_CreatePassword_Error, ex);
            }
            return password;
        }

        #endregion

        #region Check mail token

        public CheckToken CheckToken()
        {
            CheckToken cCheckToken = new CheckToken();
            try
            {
                string tokenValue = _commonService.GetHttpHeaderToken();
                if (!string.IsNullOrEmpty(tokenValue))
                {
                    string email = string.Empty;
                    var validToken = _tokenRepService.CheckToken(tokenValue);
                    cCheckToken.IsTokenExist = validToken.IsTokenExist;
                    cCheckToken.IsTokenExpired = validToken.IsTokenExpired;
                    if (cCheckToken.IsTokenExist && !cCheckToken.IsTokenExpired)
                        email = _tokenRepService.GetUserEmailByToken(tokenValue);
                    cCheckToken.Email = email;
                }
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.RegistrationService_CheckToken_Error, ex);
            }
            //WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Unauthorized;
            return cCheckToken;
        }

        #endregion

        #region Check user

        public CheckUser CheckUser(CheckUser user)
        {
            CheckUser cUser = new CheckUser();

            try
            {
                if (_userRepService.CheckMailInDB(user.Email))
                {
                    if (_incidentRepService.CheckOpenRegistrationIncidentByEmail(user.Email))
                    {
                        cUser.IsWaitingFPResponse = true;
                    }
                    else
                    {
                        cUser.IsExistInFP = true;
                        if (_userRepService.CheckPasswordExist(user.Email))
                        {
                            cUser.IsExistInMDB = true;
                        }
                        else
                        {
                            CheckUser cUserInFP = fpManager.CheckUser(user.Email);
                            if (!cUserInFP.IsExistInFP)
                            {
                                cUserInFP.CompanyName = Defaults.CompanyName;
                                cUserInFP.BillingId = Defaults.BillingId;
                            }
                            _companyRepService.CreateCompany(CompanyConvert.GetCompanyEntity(cUserInFP.CompanyName));
                            CompanyModel companyM = CompanyConvert.ConvertCompanyEntityToModel(_companyRepService.GetCompanyByName(cUserInFP.CompanyName));

                            string token = _tokenRepService.ProvideTokenForAddress(user.Email, companyM.Id, cUserInFP.BillingId);
                            _commonService.SendConfirmationMail(user.Email, token);
                        }
                    }
                }
                else
                {
                    //check mail in FP
                    CheckUser cUserInFP = fpManager.CheckUser(user.Email);
                    if (_userRepService.CheckMailInFP(user.Email) || cUserInFP.IsExistInFP)
                    {
                        if (!cUserInFP.IsExistInFP)
                        {
                            cUserInFP.CompanyName = Defaults.CompanyName;
                            cUserInFP.BillingId = Defaults.BillingId;
                        }

                        _companyRepService.CreateCompany(CompanyConvert.GetCompanyEntity(cUserInFP.CompanyName));
                        CompanyModel companyM = CompanyConvert.ConvertCompanyEntityToModel(_companyRepService.GetCompanyByName(cUserInFP.CompanyName));


                        cUser.IsExistInFP = true;

                        string token = _tokenRepService.ProvideTokenForAddress(user.Email, companyM.Id, cUserInFP.BillingId);
                        _commonService.SendConfirmationMail(user.Email, token);
                    }
                    else
                    {
                        // go to submit details page.
                    }
                }
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.RegistrationService_CheckUser_Error, ex);
            }

            return cUser;
        }

        #endregion

        #region forget password

        public CheckUser ForgetPassword(CheckUser user)
        {
            CheckUser cUser = new CheckUser();
            try
            {
                var rtnCustomer = _userRepService.CheckForgetPasswordMail(user.Email);
                cUser.IsExistInMDB = rtnCustomer.IsCustomerExistInMDB;

                if (cUser.IsExistInMDB)
                {
                    string token = _tokenRepService.GetTokenWhenForgetPwd(user.Email);
                    _commonService.SendForgetPwdMail(user.Email, token);
                }
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.RegistrationService_ForgetPassword_Error, ex);
            }
            return cUser;
        }

        #endregion

        #region Update password

        public SetPassword UpdatePassword(SetPassword password)
        {
            SetPassword _setPwd = new SetPassword();
            try
            {
                _userRepService.UpdatePassword(_commonService.GetHttpHeaderToken(), password.Password);
                _setPwd.IsUpPwd = true;
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.RegistrationService_UpdatePassword_Error, ex);
            }
            return _setPwd;
        }

        #endregion

        public string GetToken(string email)
        {
            string token = string.Empty;
            try
            {
                CheckUser cUser = fpManager.CheckUser(email);
                _companyRepService.CreateCompany(CompanyConvert.GetCompanyEntity(cUser.CompanyName));
                CompanyModel companyM = CompanyConvert.ConvertCompanyEntityToModel(_companyRepService.GetCompanyByName(cUser.CompanyName));

                token = _tokenRepService.ProvideTokenForAddress(email, companyM.Id, cUser.BillingId);
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.RegistrationService_GetToken_Error, ex);
            }
            return token;
        }

        public UserModel GetUserDetail()
        {
            UserModel user = new UserModel();
            try
            {
                string token = _commonService.GetHttpHeaderToken();
                string email = _tokenRepService.GetUserEmailByToken(token);
                user = fpManager.GetUserDetail(email);
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.RegistrationService_GetUserDetail_Error, ex);
            }
            return user;
        }
    }
}