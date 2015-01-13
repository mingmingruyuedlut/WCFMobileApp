using Interactive.Common;
using Interactive.Constant;
using Interactive.DBManager.Entity;
using Interactive.DBManager.Service;
using Interactive.Services.Contract;
using Services.Manager;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;

namespace Services.Service
{
    public class OperateIncidentService
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

        private UserIncidentMapRepService uiMapRepService;
        internal UserIncidentMapRepService _uiMapRepService
        {
            get
            {
                if (this.uiMapRepService == null)
                {
                    this.uiMapRepService = new UserIncidentMapRepService();
                }
                return this.uiMapRepService;
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

        #region OperateIncidentService

        public OperateIncidentService()
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

        #region Create incident

        public RegisterUser RegistUser(RegisterUser user)
        {
            try
            {
                IncidentModel incidentM = fpManager.CreateRegistrationIncident(user);
                incidentM.SubmitterEmail = user.Email;
                incidentM.Status = IncidentModelStatus.Open;
                incidentM.Type = IncidentType.Registration;

                IncidentEntity incidentE = IncidentModelConvert.ConvertIncidentModelToEntity(incidentM);
                _incidentRepService.CreateRegistrationIncident(incidentE);

                //int incidentId = _incidentRepService.GetRegistrationIncidentByEmail(user.Email).Id;

                #region Just Simulation

                CheckUser cUserInFP = fpManager.CheckUser(user.Email);
                if (!cUserInFP.IsExistInFP)
                {
                    cUserInFP.CompanyName = Defaults.CompanyName;
                    cUserInFP.BillingId = Defaults.BillingId;
                }
                _companyRepService.CreateCompany(CompanyConvert.GetCompanyEntity(user.Customer));
                CompanyModel companyM = CompanyConvert.ConvertCompanyEntityToModel(_companyRepService.GetCompanyByName(user.Customer));

                //if user doesn't exist, create user and token, then return token.
                string token = _tokenRepService.ProvideTokenForAddress(user.Email, companyM.Id, cUserInFP.BillingId);

                //after create user
                //int userId = _userRepService.GetUserIdByEmail(user.Email);
                //_uiMapRepService.AddUserIncidentMap(UserIncidentMapConvert.GetCompanyEntity(userId, incidentId));

                _commonService.SendConfirmationMail(user.Email, token);

                #endregion

                _commonService.SendRequestSubmitMail(user.Email);
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.OperateIncidentService_RegistUser_Error, ex);
            }

            return user;
        }

        #endregion

        #region Get incidents

        public List<Incident> GetIncidents()
        {
            List<Incident> incidentList = new List<Incident>();
            try
            {
                string fpIncidentId = "0";
                string httpToken = _commonService.GetHttpHeaderToken();
                DataSet ds = _incidentRepService.GetIncidentIdByToken(httpToken);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    fpIncidentId = "";
                    string flag = ",";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (i == ds.Tables[0].Rows.Count - 1)
                            flag = "";
                        fpIncidentId += ds.Tables[0].Rows[i][1].ToString() + flag;
                    }
                }
                incidentList = fpManager.GetIncidents(fpIncidentId);
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.OperateIncidentService_GetIncidents_Error, ex);
            }
            return incidentList;
        }

        public List<Incident> GetOpenIncidents()
        {
            List<Incident> incidentList = new List<Incident>();
            try
            {
                string fpIncidentId = "0";
                string httpToken = _commonService.GetHttpHeaderToken();
                DataSet ds = _incidentRepService.GetIncidentIdByToken(httpToken);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    fpIncidentId = "";
                    string flag = ",";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (i == ds.Tables[0].Rows.Count - 1)
                            flag = "";
                        fpIncidentId += ds.Tables[0].Rows[i][1].ToString() + flag;
                    }
                }
                incidentList = fpManager.GetOpenIncidents(fpIncidentId);
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.OperateIncidentService_GetOpenIncidents_Error, ex);
            }
            return incidentList;
        }

        public int GetOpenIncidentsCount()
        {
            int incidentCount = 0;
            try
            {
                string fpIncidentId = "0";
                string httpToken = _commonService.GetHttpHeaderToken();
                DataSet ds = _incidentRepService.GetIncidentIdByToken(httpToken);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    fpIncidentId = "";
                    string flag = ",";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (i == ds.Tables[0].Rows.Count - 1)
                            flag = "";
                        fpIncidentId += ds.Tables[0].Rows[i][1].ToString() + flag;
                    }
                }
                incidentCount = fpManager.GetOpenIncidentsCount(fpIncidentId);
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.OperateIncidentService_GetOpenIncidentsCount_Error, ex);
            }
            return incidentCount;
        }

        public List<Incident> GetHistoryIncidents()
        {
            List<Incident> incidentList = new List<Incident>();
            try
            {
                string fpIncidentId = "0";
                string httpToken = _commonService.GetHttpHeaderToken();
                DataSet ds = _incidentRepService.GetIncidentIdByToken(httpToken);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    fpIncidentId = "";
                    string flag = ",";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (i == ds.Tables[0].Rows.Count - 1)
                            flag = "";
                        fpIncidentId += ds.Tables[0].Rows[i][1].ToString() + flag;
                    }
                }
                incidentList = fpManager.GetHistoryIncidents(fpIncidentId);
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.OperateIncidentService_GetHistoryIncidents_Error, ex);
            }
            return incidentList;
        }

        public int GetHistoryIncidentsCount()
        {
            int incidentCount = 0;
            try
            {
                string fpIncidentId = "0";
                string httpToken = _commonService.GetHttpHeaderToken();
                DataSet ds = _incidentRepService.GetIncidentIdByToken(httpToken);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    fpIncidentId = "";
                    string flag = ",";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (i == ds.Tables[0].Rows.Count - 1)
                            flag = "";
                        fpIncidentId += ds.Tables[0].Rows[i][1].ToString() + flag;
                    }
                }
                incidentCount = fpManager.GetHistoryIncidentsCount(fpIncidentId);
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.OperateIncidentService_GetHistoryIncidentsCount_Error, ex);
            }
            return incidentCount;
        }

        #endregion

        public Incident CreateIncident(Incident incident)
        {
            Incident currentIncident = new Incident();
            try
            {
                string httpToken = _commonService.GetHttpHeaderToken();
                string userEmail = _tokenRepService.GetUserEmailByToken(httpToken);
                int userId = _userRepService.GetUserIdByEmail(userEmail);

                currentIncident = fpManager.CreateIncident(incident);

                IncidentModel incidentM = IncidentModelConvert.GetIncidentModel(null, currentIncident.IdInFP, userId.ToString(), "", IncidentModelStatus.Open, IncidentType.Normal);
                IncidentEntity incidentE = IncidentModelConvert.ConvertIncidentModelToEntity(incidentM);
                _incidentRepService.CreateIncident(incidentE);
                incidentE = _incidentRepService.GetIncidentByFPId(incidentE);

                UserIncidentMapEntity uiMap = new UserIncidentMapEntity();
                uiMap.UserId = userId;
                uiMap.IncidentId = incidentE.Id;
                _uiMapRepService.AddUserIncidentMap(uiMap);

                currentIncident.Id = incidentE.Id.ToString();
                currentIncident.IsOwner = true;
                currentIncident.Device = "Router";
                currentIncident.SerialNumber = "1234567890";
                currentIncident.DeviceLocation = "Melbourne";
                currentIncident.Description = "This is a new incident.";
                currentIncident.Status = StatusType.Open.ToString();
                currentIncident.TimeStamp = (long)(DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds;
                currentIncident.StatusHistory = new IncidentStatusHistory();
                currentIncident.StatusHistory.Open = new IncidentStatus();
                currentIncident.StatusHistory.Open.TimeStamp = (long)(DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds;
                currentIncident.StatusHistory.Open.IsActive = true;
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.OperateIncidentService_CreateIncident_Error, ex);
            }
            return currentIncident;
        }

        public List<UserModel> GetCompanyOtherUsersToShareIncident(string incidentId)
        {
            List<UserModel> userModelList = new List<UserModel>();
            List<UserEntity> sharedUserEntityList = new List<UserEntity>();
            try
            {
                string httpToken = _commonService.GetHttpHeaderToken();
                string userEmail = _tokenRepService.GetUserEmailByToken(httpToken);
                UserEntity userE = UserModelConvert.ConvertCompanyModelToEntity(UserModelConvert.GetUserModel(userEmail));
                userE = _userRepService.GetUserByEmail(userE);
                userModelList = UserModelConvert.ConvertCompanyEntityToModel(_userRepService.GetOtherUsersToShareIncident(userE, Int32.Parse(incidentId)));
                sharedUserEntityList = _userRepService.GetSharedUsersExceptOwner(userE, Int32.Parse(incidentId));
                foreach (UserEntity user in sharedUserEntityList)
                {
                    userModelList.Find(u => u.Id == user.Id.ToString()).IsShared = true;
                }
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.OperateIncidentService_GetCompanyOtherUsersToShareIncident_Error, ex);
            }
            return userModelList;
        }

        public List<UserModel> GetSharedUsers(string incidentId)
        {
            List<UserModel> userModelList = new List<UserModel>();
            try
            {
                string httpToken = _commonService.GetHttpHeaderToken();
                string userEmail = _tokenRepService.GetUserEmailByToken(httpToken);
                UserEntity userE = UserModelConvert.ConvertCompanyModelToEntity(UserModelConvert.GetUserModel(userEmail));
                userE = _userRepService.GetUserByEmail(userE);

                List<UserEntity> userEntityList = new List<UserEntity>();

                if (_userRepService.CheckUserIsIncidentOwner(userE, Int32.Parse(incidentId)))
                {
                    userEntityList = _userRepService.GetSharedUsersExceptOwner(userE, Int32.Parse(incidentId));
                }
                else
                {
                    userEntityList = _userRepService.GetSharedUsers(userE, Int32.Parse(incidentId));
                }

                userModelList = UserModelConvert.ConvertCompanyEntityToModel(userEntityList);
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.OperateIncidentService_GetSharedUsers_Error, ex);
            }
            return userModelList;
        }

        public WCFResponse ShareIncident(List<UserModel> userList, string incidentId)
        {
            WCFResponse response = new WCFResponse();
            try
            {
                List<UserEntity> sharedUserEntityList = UserModelConvert.ConvertCompanyModelToEntity(userList.FindAll(u => u.IsShared == true));
                List<UserEntity> noShareUserEntityList = UserModelConvert.ConvertCompanyModelToEntity(userList.FindAll(u => u.IsShared == false));
                
                List<UserIncidentMapEntity> sharedUIMapEntityList = new List<UserIncidentMapEntity>();
                foreach (UserEntity user in sharedUserEntityList)
                {
                    UserIncidentMapEntity uiMap = new UserIncidentMapEntity();
                    uiMap.UserId = user.Id;
                    uiMap.IncidentId = Int32.Parse(incidentId);
                    sharedUIMapEntityList.Add(uiMap);
                }

                List<UserIncidentMapEntity> noSharedUIMapEntityList = new List<UserIncidentMapEntity>();
                foreach (UserEntity user in noShareUserEntityList)
                {
                    UserIncidentMapEntity uiMap = new UserIncidentMapEntity();
                    uiMap.UserId = user.Id;
                    uiMap.IncidentId = Int32.Parse(incidentId);
                    noSharedUIMapEntityList.Add(uiMap);
                }

                _uiMapRepService.AddUserIncidentMap(sharedUIMapEntityList);
                _uiMapRepService.DeleteUserIncidentMap(noSharedUIMapEntityList);

                if (sharedUserEntityList.Count > 0)
                {
                    _incidentRepService.UpdateIncidentType(IncidentModelConvert.ConvertIncidentModelToEntity(IncidentModelConvert.GetIncidentModel(incidentId, null, null, null, IncidentModelStatus.Open, IncidentType.Share)));
                }
                else
                {
                    _incidentRepService.UpdateIncidentType(IncidentModelConvert.ConvertIncidentModelToEntity(IncidentModelConvert.GetIncidentModel(incidentId, null, null, null, IncidentModelStatus.Open, IncidentType.Normal)));
                }

                response.IsSuccess = true;
                response.Message = "Share incident successfully.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Type = WCFResponseType.DBError;
                response.Message = "Share incident failed.";
                _logError.Error(LogConstant.OperateIncidentService_ShareIncident_Error, ex);
            }
            return response;
        }

        public Incident GetIncidentDetail(string fpIncidentId)
        {
            // need code reflector
            Incident currentIncident = new Incident();
            try
            {
                currentIncident = fpManager.GetIncidents(fpIncidentId)[0];
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.OperateIncidentService_GetIncidentDetail_Error, ex);
            }
            return currentIncident;
        }

    }
}