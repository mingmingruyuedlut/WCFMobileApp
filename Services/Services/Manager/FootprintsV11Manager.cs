using Interactive.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Interactive.Footprints;
using Interactive.Footprints.Model;
using System.Xml;
using Interactive.DBManager.Service;
using Services.Service;
using Interactive.Constant;

namespace Services.Manager
{
    public class FootprintsV11Manager
    {
        FPService fpService = new FPService();

        #region Field

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

        private log4net.ILog logInfo;
        internal log4net.ILog _logInfo
        {
            get
            {
                if (this.logInfo == null)
                {
                    this.logInfo = log4net.LogManager.GetLogger("InfoFile");
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
                    this.logError = log4net.LogManager.GetLogger("ErrorFile");
                }
                return this.logError;
            }
        }

        #endregion

        public List<Incident> GetIncidents(string fpIncidentId)
        {
            List<Incident> incidentList = new List<Incident>();
            try
            {
                string result = fpService.SearchIncident(fpIncidentId);
                incidentList = ConvertXmlToIncidentList(result);
                //Todo logging
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.FootprintsV11Manager_GetIncidents_Error, ex);
            }
            return incidentList;
        }

        public List<Incident> GetOpenIncidents(string fpIncidentId)
        {
            List<Incident> incidentList = new List<Incident>();
            try
            {
                string result = fpService.SearchOpenIncident(fpIncidentId);
                incidentList = ConvertXmlToIncidentList(result);
                //Todo logging
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.FootprintsV11Manager_GetOpenIncidents_Error, ex);
            }
            return incidentList;
        }

        public int GetOpenIncidentsCount(string fpIncidentId)
        {
            int incidentCount = 0;
            try
            {
                string result = fpService.SearchOpenIncidentCount(fpIncidentId);
                incidentCount = ConvertXmlToIncidentCount(result);
                //Todo logging
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.FootprintsV11Manager_GetOpenIncidentsCount_Error, ex);
            }
            return incidentCount;
        }

        public List<Incident> GetHistoryIncidents(string fpIncidentId)
        {
            List<Incident> incidentList = new List<Incident>();
            try
            {
                string result = fpService.SearchHistoryIncident(fpIncidentId);
                incidentList = ConvertXmlToIncidentList(result);
                //fake data
                if (incidentList.Count > 1)
                {
                    incidentList.Reverse();
                    incidentList.RemoveRange(1, incidentList.Count - 1);
                }
                if (incidentList.Count == 1)
                {
                    incidentList[0].Status = StatusType.Resolved.ToString();
                    incidentList[0].StatusHistory = GetIncidentStatusHistory(StatusType.Resolved);
                }
                //Todo logging
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.FootprintsV11Manager_GetHistoryIncidents_Error, ex);
            }
            return incidentList;
        }

        public int GetHistoryIncidentsCount(string fpIncidentId)
        {
            int incidentCount = 0;
            try
            {
                //fake data
                incidentCount = 1;
                //string result = fpService.SearchHistoryIncidentCount(fpIncidentId);
                //incidentCount = ConvertXmlToIncidentCount(result);
                //Todo logging
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.FootprintsV11Manager_GetHistoryIncidentsCount_Error, ex);
            }
            return incidentCount;
        }

        public IncidentModel CreateRegistrationIncident(RegisterUser user)
        {
            IssueArgs issue = ConvertRegisterUserToIssueArgs(user);
            issue.projectID = "1";

            IncidentModel incident = new IncidentModel();

            try
            {
                incident.FPIncidentId = fpService.CreateIssue(issue); // just get the incident id, need change when the db structure ready
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.FootprintsV11Manager_CreateRegistrationIncident_Error, ex);
            }
            return incident;
        }

        public Incident CreateIncident(Incident incident)
        {
            IssueArgs issue = ConvertIncidentToIssueArgs(incident);
            issue.projectID = "1"; // test environment project id must equals 1

            try
            {
                incident.IdInFP = fpService.CreateIssue(issue);
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.FootprintsV11Manager_CreateIncident_Error, ex);
            }
            return incident;
        }

        public CheckUser CheckUser(string email)
        {
            CheckUser user = new CheckUser();
            try
            {
                string result = fpService.CheckUser(email);
                user = ConvertXmlToCheckUser(result);
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.FootprintsV11Manager_CheckUser_Error, ex);
            }
            return user;
        }

        public UserModel GetUserDetail(string email)
        {
            UserModel user = new UserModel();
            try
            {
                string result = fpService.GetUserDetail(email);
                user = ConvertXmlToGetUserDetail(result);
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.FootprintsV11Manager_GetUserDetail_Error, ex);
            }
            return user;
        }

        #region Private Functions

        private IssueArgs ConvertIncidentToIssueArgs(Incident incident)
        {
            IssueArgs issue = new IssueArgs();
            issue.title = incident.Description;
            issue.priorityNumber = ((int)Enum.Parse(typeof(SeverityType), incident.Severity.Value)).ToString();
            issue.status = StatusType.Open.ToString();  //currrently only 'Open' works
            issue.description = incident.Description;
            issue.selectContact = incident.Technician.Id;
            return issue;
        }

        private IssueArgs ConvertRegisterUserToIssueArgs(RegisterUser user)
        {
            //fake data
            IssueArgs issue = new IssueArgs();
            issue.title = "Register Customer From Mobile App";
            issue.priorityNumber = ((int)Enum.Parse(typeof(SeverityType), "P0")).ToString();
            issue.status = "Open";  //currrently only 'Open' works
            issue.description = "Email:" + user.Email + ";  Name:" + user.Name + ";  Customer:" + user.Customer + ";  Phone Number:" + user.Phone;
            issue.selectContact = "1";
            return issue;
        }

        private Severity ConvertPriorityToSeverity(string priority)
        {
            Severity sev = new Severity();
            SeverityType sType = (SeverityType)int.Parse(priority);
            if (sType == SeverityType.P0)
            {
                sev.Value = "P0";
                sev.Label = "Not sure";
            }
            else if (sType == SeverityType.P1)
            {
                sev.Value = "P1";
                sev.Label = "P1 - Urgent";
            }
            else if (sType == SeverityType.P2)
            {
                sev.Value = "P2";
                sev.Label = "P2 - Normal";
            }
            else if (sType == SeverityType.P3)
            {
                sev.Value = "P3";
                sev.Label = "P3 - Minor";
            }
            else if (sType == SeverityType.P4)
            {
                sev.Value = "P4";
                sev.Label = "P4 - Low";
            }
            return sev;
        }

        private Incident ConvertXmlToIncident(string result)
        {
            Incident incident = new Incident();
            //fake data. replace it if we can get the real value
            incident.Service = "business continuity";
            incident.Technician = new Technician() { Id = "admin", Name = "Gennady Vaisman", Title = "SSE", Avatar = "Tech", Rating = 5 };
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(result);
                XmlNode mapNode = xmlDoc.SelectSingleNode("data/map");
                if (mapNode != null)
                {
                    string createDate = string.Empty;
                    string createTime = string.Empty;
                    foreach (XmlNode item in mapNode.ChildNodes)
                    {
                        if (item.Attributes["key"].Value.Equals("title"))
                        {

                        }
                        else if (item.Attributes["key"].Value.Equals("mr"))
                        {
                            incident.Id = item.InnerText;
                        }
                        else if (item.Attributes["key"].Value.Equals("priority"))
                        {
                            incident.Severity = ConvertPriorityToSeverity(item.InnerText);
                        }
                        else if (item.Attributes["key"].Value.Equals("status"))
                        {
                            incident.Status = ConvertFPStatusToUIStatus(item.InnerText).ToString();
                            incident.StatusHistory = GetIncidentStatusHistory(ConvertFPStatusToUIStatus(item.InnerText));
                        }
                        else if (item.Attributes["key"].Value.Equals("submitter"))
                        {

                        }
                        else if (item.Attributes["key"].Value.Equals("entrydate"))
                        {
                            //2014-11-04
                            createDate = item.InnerText;
                        }
                        else if (item.Attributes["key"].Value.Equals("entrytime"))
                        {
                            //06:20:24
                            createTime = item.InnerText;
                        }
                        else if (item.Attributes["key"].Value.Equals("assignees"))
                        {
                            //maybe it's an array
                            //admin
                        }
                        else if (item.Attributes["key"].Value.Equals("description"))
                        {
                            incident.Description = GetDescription(item.InnerText);
                        }
                    }
                    incident.TimeStamp = GetTimeStamp(createDate, createTime);
                }
            }
            catch (Exception ex)
            {
                _logError.Error("FootprintsV11Manager -- ConvertXmlToIncident function error: ", ex);
            }
            return incident;
        }

        private CheckUser ConvertXmlToCheckUser(string result)
        {
            CheckUser user = new CheckUser();
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(result);
                XmlNode mapNode = xmlDoc.SelectSingleNode("data/array/item/map");
                if (mapNode != null)
                {
                    string createDate = string.Empty;
                    string createTime = string.Empty;
                    foreach (XmlNode item in mapNode.ChildNodes)
                    {
                        if (item.Attributes["key"].Value.Equals("customer__bid"))
                        {
                            user.BillingId = item.InnerText;
                        }
                        else if (item.Attributes["key"].Value.Equals("customer"))
                        {
                            user.CompanyName = item.InnerText;
                        }
                        else if (item.Attributes["key"].Value.Equals("email"))
                        {
                            user.Email = item.InnerText;
                        }
                    }
                    user.IsExistInFP = true;
                }
                else
                {
                    user.IsExistInFP = false;
                }
            }
            catch (Exception ex)
            {
                _logError.Error("FootprintsV11Manager -- ConvertXmlToCheckUser function error: ", ex);
            }
            return user;
        }

        private UserModel ConvertXmlToGetUserDetail(string result)
        {
            UserModel user = new UserModel();
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(result);
                XmlNode mapNode = xmlDoc.SelectSingleNode("data/array/item/map");
                if (mapNode != null)
                {
                    string createDate = string.Empty;
                    string createTime = string.Empty;
                    foreach (XmlNode item in mapNode.ChildNodes)
                    {
                        switch (item.Attributes["key"].Value)
                        {
                            case "phone__bnumber__b1":
                                user.ContactNumber = item.InnerText;
                                break;
                            case "contact__bname":
                                user.Name = item.InnerText;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logError.Error("FootprintsV11Manager -- ConvertXmlToGetUserDetail function error: ", ex);
            }
            return user;
        }

        private List<Incident> ConvertXmlToIncidentList(string result)
        {
            List<Incident> incidentList = new List<Incident>();
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(result);

                XmlNodeList mapNodeList = xmlDoc.SelectNodes("data/array/item/map");
                if (mapNodeList.Count > 0)
                {
                    int j = 0;
                    foreach (XmlNode mapNode in mapNodeList)
                    {
                        Incident incidentItem = new Incident();

                        incidentItem = ConvertXmlNodeToIncident(mapNode);

                        #region Fake data

                        incidentItem.SerialNumber = "1234567890";
                        incidentItem.Device = "Router";
                        incidentItem.DeviceLocation = "Melbourne";

                        j++;
                        switch (j)
                        {
                            case 1:
                                incidentItem.ContactedSLA = ConvertFPSLAToUISLA("Yes");
                                incidentItem.OnsiteSLA = ConvertFPSLAToUISLA("Yes");
                                break;
                            case 2:
                                incidentItem.ContactedSLA = ConvertFPSLAToUISLA("No");
                                incidentItem.OnsiteSLA = ConvertFPSLAToUISLA("No");
                                break;
                            case 3:
                                incidentItem.ContactedSLA = ConvertFPSLAToUISLA("Yes");
                                incidentItem.OnsiteSLA = ConvertFPSLAToUISLA("No");
                                break;
                            case 4:
                                incidentItem.ContactedSLA = ConvertFPSLAToUISLA("No");
                                incidentItem.OnsiteSLA = ConvertFPSLAToUISLA("Yes");
                                break;
                            case 5:
                                incidentItem.ContactedSLA = ConvertFPSLAToUISLA("Yes");
                                incidentItem.OnsiteSLA = ConvertFPSLAToUISLA("");
                                break;
                            case 6:
                                incidentItem.ContactedSLA = ConvertFPSLAToUISLA("");
                                incidentItem.OnsiteSLA = ConvertFPSLAToUISLA("Yes");
                                break;
                        }

                        #endregion

                        incidentList.Add(incidentItem);
                    }
                }
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.FootprintsV11Manager_ConvertXmlToIncidentList_Error, ex);
            }
            return incidentList;
        }

        private Incident ConvertXmlNodeToIncident(XmlNode mapNode)
        {
            IncidentRepService _incidentService = new IncidentRepService();
            Incident incident = new Incident();

            incident.Service = "business continuity";
            incident.Technician = new Technician() { Id = "admin", Name = "Gennady Vaisman", Title = "SSE", Avatar = "Tech", Rating = 5 };

            foreach (XmlNode item in mapNode.ChildNodes)
            {
                switch (item.Attributes["key"].Value)
                {
                    case "mrtitle":
                        incident.MrTitle = item.InnerText;
                        break;
                    case "mrid":
                        incident.IdInFP = item.InnerText;
                        incident.Id = _incidentService.GetIncidentIdByFpId(incident.IdInFP);
                        incident.IsOwner = _incidentService.CheckIncidentOwner(incident.Id, _commonService.GetHttpHeaderToken());
                        incident.ShareCount = _incidentService.GetShareCount(incident.Id, _commonService.GetHttpHeaderToken());
                        break;
                    case "mrpriority":
                        incident.Severity = ConvertPriorityToSeverity(item.InnerText);
                        break;
                    case "mrstatus":
                        incident.Status = ConvertFPStatusToUIStatus(item.InnerText).ToString();
                        incident.StatusHistory = GetIncidentStatusHistory(ConvertFPStatusToUIStatus(item.InnerText));
                        break;
                    case "mrsubmitter":
                        break;
                    case "mrsubmitdate":
                        string time = item.InnerText.Substring(0, item.InnerText.IndexOf('.'));
                        string submitDate = time.Split(' ')[0];
                        string submitTime = time.Split(' ')[1];
                        incident.TimeStamp = GetTimeStamp(submitDate, submitTime);
                        break;
                    case "mrassignees":
                        break;
                    case "mrdescription":
                        break;
                    case "closure__bcode":
                        incident.ClosureCode = ConvertClosureCode(item.InnerText);
                        break;
                    case "resolution":
                        incident.Resolution = item.InnerText;
                        break;
                }
            }
            return incident;
        }

        private int ConvertXmlToIncidentCount(string result)
        {
            int incidentCount = 0;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(result);

                XmlNodeList mapNodeList = xmlDoc.SelectNodes("data/array/item/map");
                if (mapNodeList.Count > 0)
                {
                    XmlNode mapNode = mapNodeList[0];
                    if (mapNode.HasChildNodes)
                    {
                        string incidentCountStr = mapNode.FirstChild.InnerText;
                        if (!string.IsNullOrEmpty(incidentCountStr))
                        {
                            incidentCount = Int32.Parse(incidentCountStr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logError.Error(LogConstant.FootprintsV11Manager_ConvertXmlToIncidentCount_Error, ex);
            }
            return incidentCount;
        }

        private long GetTimeStamp(string date, string time)
        {
            long timeStamp = 0;
            string[] dateSplit = date.Split('-');
            string[] timeSplit = time.Split(':');
            DateTime dt = new DateTime(int.Parse(dateSplit[0]), int.Parse(dateSplit[1]), int.Parse(dateSplit[2]), int.Parse(timeSplit[0]), int.Parse(timeSplit[1]), int.Parse(timeSplit[2]));
            timeStamp = long.Parse((dt - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds.ToString());
            return timeStamp;
        }

        private long ConvertDateTimeToTimeStamp(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (long)(time - startTime).TotalMilliseconds;
        }

        private string GetDescription(string description)
        {
            return description.StartsWith("<!") ? description.Substring(description.IndexOf('>') + 1) : description;
        }

        private IncidentStatusHistory GetIncidentStatusHistory(StatusType status)
        {
            IncidentStatusHistory statusHistroy = new IncidentStatusHistory();
            if (status == StatusType.Open)
            {
                statusHistroy.Open = new IncidentStatus();
                statusHistroy.Open.TimeStamp = ConvertDateTimeToTimeStamp(DateTime.Now);
                statusHistroy.Open.IsActive = true;
            }
            else if (status == StatusType.Contact)
            {
                statusHistroy.Open = new IncidentStatus();
                statusHistroy.Open.TimeStamp = ConvertDateTimeToTimeStamp(DateTime.Now);
                statusHistroy.Open.IsActive = false;

                statusHistroy.Contacted = new IncidentStatus();
                statusHistroy.Contacted.TimeStamp = ConvertDateTimeToTimeStamp(DateTime.Now);
                statusHistroy.Contacted.IsActive = true;
            }
            else if (status == StatusType.Onsite || status == StatusType.Scheduled || status == StatusType.RectificationInProgress)
            {
                statusHistroy.Open = new IncidentStatus();
                statusHistroy.Open.TimeStamp = ConvertDateTimeToTimeStamp(DateTime.Now);
                statusHistroy.Open.IsActive = false;

                statusHistroy.Contacted = new IncidentStatus();
                statusHistroy.Contacted.TimeStamp = ConvertDateTimeToTimeStamp(DateTime.Now);
                statusHistroy.Contacted.IsActive = false;

                statusHistroy.Onsite = new IncidentStatus();
                statusHistroy.Onsite.TimeStamp = ConvertDateTimeToTimeStamp(DateTime.Now);
                statusHistroy.Onsite.IsActive = true;
            }
            else if (status == StatusType.Resolved)
            {
                statusHistroy.Open = new IncidentStatus();
                statusHistroy.Open.TimeStamp = ConvertDateTimeToTimeStamp(DateTime.Now);
                statusHistroy.Open.IsActive = false;

                statusHistroy.Contacted = new IncidentStatus();
                statusHistroy.Contacted.TimeStamp = ConvertDateTimeToTimeStamp(DateTime.Now);
                statusHistroy.Contacted.IsActive = false;

                statusHistroy.Onsite = new IncidentStatus();
                statusHistroy.Onsite.TimeStamp = ConvertDateTimeToTimeStamp(DateTime.Now);
                statusHistroy.Onsite.IsActive = false;

                statusHistroy.Resolved = new IncidentStatus();
                statusHistroy.Resolved.TimeStamp = ConvertDateTimeToTimeStamp(DateTime.Now);
                statusHistroy.Resolved.IsActive = true;
            }
            return statusHistroy;
        }

        private string ConvertClosureCode(string code)
        {
            string closureCode = "No Choice";
            if (code.Equals("Completed__bSuccessfully"))
            {
                closureCode = "Completed Successfully";
            }
            else if (code.Equals("Training__bRequired"))
            {
                closureCode = "Training Required";
            }
            else if (code.Equals("Documentation__bNeeds__bReview"))
            {
                closureCode = "Documentation Needs Review";
            }
            else if (code.Equals("No__bFault__bFound"))
            {
                closureCode = "No Fault Found";
            }
            else if (code.Equals("Monitoring__bRequired"))
            {
                closureCode = "Monitoring Required";
            }
            else if (code.Equals("Advice__bGiven"))
            {
                closureCode = "Advice Given";
            }
            else if (code.Equals("RFC__bNeeded"))
            {
                closureCode = "RFC Needed";
            }
            return closureCode;
        }

        private StatusType ConvertFPStatusToUIStatus(string status)
        {
            StatusType uiStatus = StatusType.Open;
            if (status.Equals("Open"))
            {
                uiStatus = StatusType.Open;
            }
            else if (status.Equals("Accepted"))
            {
                uiStatus = StatusType.Contact;
            }
            else if (status.Equals("Scheduled"))
            {
                uiStatus = StatusType.Scheduled;
            }
            else if (status.Equals("Work__bIn__bProgress"))
            {
                uiStatus = StatusType.RectificationInProgress;
            }
            else if (status.Equals("On__bHold"))
            {
                uiStatus = StatusType.Onsite;
            }
            else if (status.Equals("Closed"))
            {
                uiStatus = StatusType.Resolved;
            }
            return uiStatus;
        }

        private string ConvertFPSLAToUISLA(string sla)
        {
            string uiSLA = SLAType.Passed.ToString();
            if (sla.Equals("Yes"))
            {
                uiSLA = SLAType.Passed.ToString();
            }
            else if (sla.Equals("No"))
            {
                uiSLA = SLAType.Failed.ToString();
            }
            else if (string.IsNullOrEmpty(sla))
            {
                uiSLA = string.Empty;
            }
            return uiSLA;
        }

        #endregion

    }
}