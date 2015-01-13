using System;
using Interactive.Footprints.Model;
using Interactive.Footprints.Manager;
using System.Configuration;
using System.Xml;

namespace Interactive.Footprints
{
    public class FPService
    {
        public string CreateIssue(IssueArgs issueargs)
        {
            CreateIssueManager createIssuemanger = new CreateIssueManager();
            string username = ConfigurationManager.AppSettings["FPUserName"];
            string psw = ConfigurationManager.AppSettings["FPPassword"];
            string IssueId = createIssuemanger.MRWebServices__createIssue(username, psw, "", issueargs);

            return IssueId;
        }

        public string SearchIssue(int projectId)
        {
            SearchDBManager searchManager = new SearchDBManager();
            string username = ConfigurationManager.AppSettings["FPUserName"];
            string psw = ConfigurationManager.AppSettings["FPPassword"];
            string query = "select mrID, mrTITLE, mrPRIORITY, mrSTATUS, mrDESCRIPTION, mrSUBMITTER, mrSUBMITDATE, mrASSIGNEES, mrUPDATEDATE from MASTER" + projectId.ToString() + " WHERE mrSUBMITTER = 'admin'";
            string result = searchManager.MRWebServices__search(username, psw, "RETURN_MODE => 'xml'", query);
            return result;
        }

        public string SearchIncident(string fpIncidentId)
        {
            SearchDBManager searchManager = new SearchDBManager();
            string username = ConfigurationManager.AppSettings["FPUserName"];
            string psw = ConfigurationManager.AppSettings["FPPassword"];
            string query = "SELECT mrID, mrTITLE, mrPRIORITY, mrSTATUS, mrDESCRIPTION, mrSUBMITTER, mrSUBMITDATE, mrASSIGNEES, mrUPDATEDATE,Closure__bCode,Resolution FROM MASTER1 WHERE mrID IN (" + fpIncidentId + ")";
            string result = searchManager.MRWebServices__search(username, psw, "RETURN_MODE => 'xml'", query);
            return result;
        }

        public string SearchOpenIncident(string fpIncidentId)
        {
            SearchDBManager searchManager = new SearchDBManager();
            string username = ConfigurationManager.AppSettings["FPUserName"];
            string psw = ConfigurationManager.AppSettings["FPPassword"];
            //SELECT (all the status time)..... WHERE ...... AND mrSTATUS != 'Closed' OR (mrSTATUS = 'Closed' AND Time__bResolved <= 7 days)
            string query = "SELECT mrID, mrTITLE, mrPRIORITY, mrSTATUS, mrDESCRIPTION, mrSUBMITTER, mrSUBMITDATE, mrASSIGNEES, mrUPDATEDATE,Closure__bCode,Resolution FROM MASTER1 WHERE mrID IN (" + fpIncidentId + ")";
            string result = searchManager.MRWebServices__search(username, psw, "RETURN_MODE => 'xml'", query);
            return result;
        }

        public string SearchOpenIncidentCount(string fpIncidentId)
        {
            SearchDBManager searchManager = new SearchDBManager();
            string username = ConfigurationManager.AppSettings["FPUserName"];
            string psw = ConfigurationManager.AppSettings["FPPassword"];
            //SELECT (all the status time)..... WHERE ...... AND mrSTATUS != 'Closed' OR (mrSTATUS = 'Closed' AND Time__bResolved <= 7 days)
            string query = "SELECT Count(mrID) incidentcount FROM MASTER1 WHERE mrID IN (" + fpIncidentId + ")";
            string result = searchManager.MRWebServices__search(username, psw, "RETURN_MODE => 'xml'", query);
            return result;
        }

        public string SearchHistoryIncident(string fpIncidentId)
        {
            SearchDBManager searchManager = new SearchDBManager();
            string username = ConfigurationManager.AppSettings["FPUserName"];
            string psw = ConfigurationManager.AppSettings["FPPassword"];
            //SELECT (all the status time)..... WHERE ...... AND mrSTATUS = 'Closed' AND (7days < Time__bResolved <= 30 days)
            string query = "SELECT mrID, mrTITLE, mrPRIORITY, mrSTATUS, mrDESCRIPTION, mrSUBMITTER, mrSUBMITDATE, mrASSIGNEES, mrUPDATEDATE,Closure__bCode,Resolution FROM MASTER1 WHERE mrID IN (" + fpIncidentId + ")";
            string result = searchManager.MRWebServices__search(username, psw, "RETURN_MODE => 'xml'", query);
            return result;
        }

        public string SearchHistoryIncidentCount(string fpIncidentId)
        {
            SearchDBManager searchManager = new SearchDBManager();
            string username = ConfigurationManager.AppSettings["FPUserName"];
            string psw = ConfigurationManager.AppSettings["FPPassword"];
            //SELECT (all the status time)..... WHERE ...... AND mrSTATUS = 'Closed' AND (7days < Time__bResolved <= 30 days)
            string query = "SELECT Count(mrID) incidentcount FROM MASTER1 WHERE mrID IN (" + fpIncidentId + ")";
            string result = searchManager.MRWebServices__search(username, psw, "RETURN_MODE => 'xml'", query);
            return result;
        }

        public string GetIssueDetail(int projectId, int issueId)
        {
            RetrieveIssueManager retrieveManager = new RetrieveIssueManager();
            string username = ConfigurationManager.AppSettings["FPUserName"];
            string psw = ConfigurationManager.AppSettings["FPPassword"];
            string result = retrieveManager.MRWebServices__getIssueDetails(username, psw, "RETURN_MODE => 'xml'", projectId, issueId);
            return result;
        }

        public string SearchContact(int projectId, int customerId)
        {
            SearchDBManager searchManager = new SearchDBManager();
            string username = ConfigurationManager.AppSettings["UserName"];
            string psw = ConfigurationManager.AppSettings["FPPassword"];
            //string query = "select mrID, Last__bName, First_bName, Email_bAddress from MASTER" + projectId.ToString() + "_ABDATA";
            string query = "select abID, abSUBMITTER, abSUBMITDATE, abUPDATEDATE, abASSIGNEE, abSTATUS, Service__bLevel, User__bId, Last__bName, First__bName, Email__bAddress, Department, Phone, Site, Supervisor from ABMASTER" + projectId.ToString();           
            string result = searchManager.MRWebServices__search(username, psw, "RETURN_MODE => 'xml'", query);
            return result;
        }

        public string CheckUser(string email)
        {
            SearchDBManager searchManager = new SearchDBManager();
            string username = ConfigurationManager.AppSettings["FPUserName"];
            string psw = ConfigurationManager.AppSettings["FPPassword"];
            string query = "select abID, abSUBMITTER, abSUBMITDATE, abUPDATEDATE, abASSIGNEE, abSTATUS, Email, Customer, Customer__bID from ABMASTER5 Where Email='" + email + "'";
            string result = searchManager.MRWebServices__search(username, psw, "RETURN_MODE => 'xml'", query);
            return result;
        }

        public string GetUserDetail(string email)
        {
            SearchDBManager searchManager = new SearchDBManager();
            string username = ConfigurationManager.AppSettings["FPUserName"];
            string psw = ConfigurationManager.AppSettings["FPPassword"];
            string query = "select Phone__bNumber__b1,Contact__bName from ABMASTER5 Where Email='" + email + "'";
            string result = searchManager.MRWebServices__search(username, psw, "RETURN_MODE => 'xml'", query);
            return result;
        }

    }
}
