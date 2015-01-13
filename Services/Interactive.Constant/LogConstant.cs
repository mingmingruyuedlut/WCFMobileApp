using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive.Constant
{
    public static class LogConstant
    {
        //These two values need map to the web.config logger node name
        public static string LogInfoFile = "InfoFile";
        public static string LogErrorFile = "ErrorFile";

        public static string LoginService_IsLogin_Error = "LoginService -- IsLogin function error: ";
        public static string LoginService_IsLogin_Start = "LoginService -- IsLogin function start. And the login email is {0}";
        public static string LoginService_IsLogin_End = "LoginService -- IsLogin function end. And the login email is {0}";
        public static string LoginService_IsLogout_Error = "LoginService -- IsLogout function error: ";
        public static string LoginService_IsLogout_Start = "LoginService -- IsLogout function start. And the logout email is {0}";
        public static string LoginService_IsLogout_End = "LoginService -- IsLogout function end. And the logout email is {0}";


        public static string OperateIncidentService_RegistUser_Error = "OperateIncidentService -- RegistUser function error: ";
        public static string OperateIncidentService_GetIncidents_Error = "OperateIncidentService -- GetIncidents function error: ";
        public static string OperateIncidentService_GetOpenIncidents_Error = "OperateIncidentService -- GetOpenIncidents function error: ";
        public static string OperateIncidentService_GetOpenIncidentsCount_Error = "OperateIncidentService -- GetOpenIncidentsCount function error: ";
        public static string OperateIncidentService_GetHistoryIncidents_Error = "OperateIncidentService -- GetHistoryIncidents function error: ";
        public static string OperateIncidentService_GetHistoryIncidentsCount_Error = "OperateIncidentService -- GetHistoryIncidentsCount function error: ";
        public static string OperateIncidentService_CreateIncident_Error = "OperateIncidentService -- CreateIncident function error: ";
        public static string OperateIncidentService_GetCompanyOtherUsersToShareIncident_Error = "OperateIncidentService -- GetCompanyOtherUsersToShareIncident function error: ";
        public static string OperateIncidentService_GetSharedUsers_Error = "OperateIncidentService -- GetSharedUsers function error: ";
        public static string OperateIncidentService_ShareIncident_Error = "OperateIncidentService -- ShareIncident function error: ";
        public static string OperateIncidentService_GetIncidentDetail_Error = "OperateIncidentService -- GetIncidentDetail function error: ";

        public static string RegistrationService_CreatePassword_Error = "RegistrationService -- CreatePassword function error: ";
        public static string RegistrationService_CheckToken_Error = "RegistrationService -- CheckToken function error: ";
        public static string RegistrationService_CheckUser_Error = "RegistrationService -- CheckUser function error: ";
        public static string RegistrationService_ForgetPassword_Error = "RegistrationService -- ForgetPassword function error: ";
        public static string RegistrationService_UpdatePassword_Error = "RegistrationService -- UpdatePassword function error: ";
        public static string RegistrationService_GetToken_Error = "RegistrationService -- GetToken function error: ";
        public static string RegistrationService_GetUserDetail_Error = "RegistrationService -- GetUserDetail function error: ";

        public static string CommonService_SendConfirmationMail_Error = "CommonService -- SendConfirmationMail function error: ";
        public static string CommonService_SendRequestSubmitMail_Error = "CommonService -- SendRequestSubmitMail function error: ";
        public static string CommonService_SendForgetPwdMail_Error = "CommonService -- SendForgetPwdMail function error: ";
        public static string FootprintsV11Manager_GetIncidents_Error = "FootprintsV11Manager -- GetIncidents function error: ";
        public static string FootprintsV11Manager_GetOpenIncidents_Error = "FootprintsV11Manager -- GetOpenIncidents function error: ";
        public static string FootprintsV11Manager_GetOpenIncidentsCount_Error = "FootprintsV11Manager -- GetOpenIncidentsCount function error: ";
        public static string FootprintsV11Manager_GetHistoryIncidents_Error = "FootprintsV11Manager -- GetHistoryIncidents function error: ";
        public static string FootprintsV11Manager_GetHistoryIncidentsCount_Error = "FootprintsV11Manager -- GetHistoryIncidentsCount function error: ";
        public static string FootprintsV11Manager_CreateRegistrationIncident_Error = "FootprintsV11Manager -- CreateRegistrationIncident function error: ";
        public static string FootprintsV11Manager_CreateIncident_Error = "FootprintsV11Manager -- CreateIncident function error: ";
        public static string FootprintsV11Manager_CheckUser_Error = "FootprintsV11Manager -- CheckUser function error: ";
        public static string FootprintsV11Manager_GetUserDetail_Error = "FootprintsV11Manager -- GetUserDetail function error: ";
        public static string FootprintsV11Manager_ConvertXmlToIncidentList_Error = "FootprintsV11Manager -- ConvertXmlToIncidentList function error: ";
        public static string FootprintsV11Manager_ConvertXmlToIncidentCount_Error = "FootprintsV11Manager -- ConvertXmlToIncidentCount function error: ";
    }
}
