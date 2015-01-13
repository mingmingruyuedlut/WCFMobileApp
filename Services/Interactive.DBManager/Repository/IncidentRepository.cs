using Interactive.Constant;
using Interactive.DBManager.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive.DBManager.Repository
{
    public class IncidentRepository
    {
        public void AddIncident(IncidentEntity incident)
        {
            string strInsert = "INSERT INTO Incidents(FPIncidentId,UserId,SubmitterEmail,Status,Type) VALUES (@FPIncidentId,@UserId,@SubmitterEmail,@Status,@Type)";
            SqlParameter[] parms = {
				new SqlParameter("@FPIncidentId", SqlDbType.Int),
                new SqlParameter("@UserId", SqlDbType.Int),
                new SqlParameter("@SubmitterEmail", SqlDbType.NVarChar, 100),
                new SqlParameter("@Status", SqlDbType.Int),
                new SqlParameter("@Type", SqlDbType.Int)};
            parms[0].Value = incident.FPIncidentId;
            parms[1].Value = incident.UserId;
            parms[2].Value = incident.SubmitterEmail;
            parms[3].Value = incident.Status;
            parms[4].Value = incident.Type;
            SqlHelper.ExcuteNonQuery(CommandType.Text, strInsert, parms);
        }

        public void DeleteIncident(IncidentEntity incident)
        {
            string sqlDelete = "DELETE FROM Incidents WHERE Id = @Id";
            SqlParameter[] deleteParms = {				   
				new SqlParameter("@Id", SqlDbType.Int)};
            deleteParms[0].Value = incident.Id;
            SqlHelper.ExcuteNonQuery(CommandType.Text, sqlDelete, deleteParms);
        }

        public void UpdateRegistrationIncident(IncidentEntity incident)
        {
            string sqlUpdate = "UPDATE Incidents SET UserId = @UserId, SubmitterEmail = '', Status = @Status WHERE SubmitterEmail = @SubmitterEmail And Type = @Type";
            SqlParameter[] parms = {	
			    new SqlParameter("@UserId", SqlDbType.Int),
				new SqlParameter("@Status", SqlDbType.Int),
				new SqlParameter("@SubmitterEmail", SqlDbType.NVarChar,100),
                new SqlParameter("@Type", SqlDbType.Int)};
            parms[0].Value = incident.UserId;
            parms[1].Value = incident.Status;
            parms[2].Value = incident.SubmitterEmail;
            parms[3].Value = incident.Type;
            SqlHelper.ExcuteNonQuery(CommandType.Text, sqlUpdate, parms);
        }

        public void UpdateIncidentType(IncidentEntity incident)
        {
            string sqlUpdate = "UPDATE Incidents SET Type = @Type WHERE Id = @Id";
            SqlParameter[] parms = {	
			    new SqlParameter("@Id", SqlDbType.Int),
				new SqlParameter("@Type", SqlDbType.Int)};
            parms[0].Value = incident.Id;
            parms[1].Value = incident.Type;
            SqlHelper.ExcuteNonQuery(CommandType.Text, sqlUpdate, parms);
        }

        public IncidentEntity GetIncidentById(IncidentEntity incident)
        {
            string sqlGet = "SELECT Id, FPIncidentId, UserId, SubmitterEmail, Status, Type FROM Incidents WHERE Id = @Id";
            SqlParameter[] parms = {				   
				new SqlParameter("@Id", SqlDbType.Int)};
            parms[0].Value = incident.Id;
            DataSet ds = SqlHelper.ExcuteDataSet(CommandType.Text, sqlGet, parms);

            return ConvertDataSetToIncidentEntity(ds);
        }

        public IncidentEntity GetIncidentByFPId(IncidentEntity incident)
        {
            string sqlGet = "SELECT Id, FPIncidentId, UserId, SubmitterEmail, Status, Type FROM Incidents WHERE FPIncidentId = @FPIncidentId";
            SqlParameter[] parms = {				   
				new SqlParameter("@FPIncidentId", SqlDbType.Int)};
            parms[0].Value = incident.FPIncidentId;
            DataSet ds = SqlHelper.ExcuteDataSet(CommandType.Text, sqlGet, parms);

            return ConvertDataSetToIncidentEntity(ds);
        }

        public IncidentEntity GetRegistrationIncidentByEmail(string email)
        {
            string sqlGet = "SELECT Id, FPIncidentId, UserId, SubmitterEmail, Status, Type FROM Incidents WHERE SubmitterEmail = @SubmitterEmail And Type = @Type";
            SqlParameter[] parms = {				   
				new SqlParameter("@SubmitterEmail", SqlDbType.NVarChar,100),
                new SqlParameter("@Type", SqlDbType.Int)};
            parms[0].Value = email;
            parms[1].Value = IncidentType.Registration;
            DataSet ds = SqlHelper.ExcuteDataSet(CommandType.Text, sqlGet, parms);

            return ConvertDataSetToIncidentEntity(ds);
        }

        public bool CheckRegistrationIncidentByEmail(string email)
        {
            string sqlCheck = "SELECT COUNT(*) FROM Incidents WHERE SubmitterEmail = @SubmitterEmail And Type = @Type";
            SqlParameter[] parms = {				   
				new SqlParameter("@SubmitterEmail", SqlDbType.NVarChar,100),
                new SqlParameter("@Type", SqlDbType.Int)};
            parms[0].Value = email;
            parms[1].Value = IncidentType.Registration;
            int i = Convert.ToInt32(SqlHelper.ExcuteScalar(CommandType.Text, sqlCheck, parms));
            if (i > 0)
                return true;
            else
                return false;
        }

        public bool CheckOpenRegistrationIncidentByEmail(string email)
        {
            string sqlCheck = "SELECT COUNT(*) FROM Incidents WHERE SubmitterEmail = @SubmitterEmail And Status = @Status And Type = @Type";
            SqlParameter[] parms = {				   
				new SqlParameter("@SubmitterEmail", SqlDbType.NVarChar,100),
                new SqlParameter("@Status", SqlDbType.Int),
                new SqlParameter("@Type", SqlDbType.Int)};
            parms[0].Value = email;
            parms[1].Value = IncidentModelStatus.Open;
            parms[2].Value = IncidentType.Registration;
            int i = Convert.ToInt32(SqlHelper.ExcuteScalar(CommandType.Text, sqlCheck, parms));
            if (i > 0)
                return true;
            else
                return false;
        }

        public DataSet GetIncidentIdByToken(string token)
        {
            string sqlGet = " SELECT A.IncidentId,(SELECT FPIncidentId FROM Incidents B WHERE B.Id = A.IncidentId) AS FPId FROM UserIncidentMapping A";
            sqlGet += " WHERE UserId = (SELECT UserId FROM Tokens WHERE Value = @Value)";
            SqlParameter[] parms = {				   
				new SqlParameter("@Value", SqlDbType.NVarChar, 100)};
            parms[0].Value = token;
            return SqlHelper.ExcuteDataSet(CommandType.Text, sqlGet, parms);
        }

        private IncidentEntity ConvertDataSetToIncidentEntity(DataSet ds)
        {
            IncidentEntity incidentE = new IncidentEntity();
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                incidentE.Id = Int32.Parse(ds.Tables[0].Rows[0][0].ToString());
                incidentE.FPIncidentId = Int32.Parse(ds.Tables[0].Rows[0][1].ToString());
                incidentE.UserId = Int32.Parse(ds.Tables[0].Rows[0][2].ToString());
                incidentE.SubmitterEmail = ds.Tables[0].Rows[0][3].ToString();
                incidentE.Status = (IncidentModelStatus)Int32.Parse(ds.Tables[0].Rows[0][4].ToString());
                incidentE.Type = (IncidentType)Int32.Parse(ds.Tables[0].Rows[0][5].ToString());
            }
            return incidentE;
        }

        public string GetIncidentIdByFpId(string fpIncidentId)
        {
            string strGet = "SELECT Id FROM Incidents WHERE FPIncidentId = @FPIncidentId";
            SqlParameter[] parms = {				   
                new SqlParameter("@FPIncidentId", SqlDbType.Int)};
            parms[0].Value = fpIncidentId;
            DataSet ds = SqlHelper.ExcuteDataSet(CommandType.Text, strGet, parms);
            return ds.Tables[0].Rows[0][0].ToString();
        }

        public int GetIncidentType(string incidentId)
        {
            string strGet = "SELECT Type FROM Incidents WHERE Id = @Id";
            SqlParameter[] parms = {				   
                new SqlParameter("@Id", SqlDbType.Int)};
            parms[0].Value = incidentId;
            DataSet ds = SqlHelper.ExcuteDataSet(CommandType.Text, strGet, parms);
            return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
        }

        public int GetShareCount(string incidentId)
        {
            string strGet = "SELECT COUNT(*) FROM UserIncidentMapping WHERE IncidentId = @IncidentId";
            SqlParameter[] parms = {				   
                new SqlParameter("@IncidentId", SqlDbType.Int)};
            parms[0].Value = incidentId;
            int i = Convert.ToInt32(SqlHelper.ExcuteScalar(CommandType.Text, strGet, parms));
            return i;
        }

        public int GetShareCountOwner(string incidentId, string userId)
        {
            string strGet = "SELECT COUNT(*) FROM UserIncidentMapping WHERE IncidentId = @IncidentId AND UserId != @UserId";
            SqlParameter[] parms = {				        
                new SqlParameter("@IncidentId", SqlDbType.Int),
                new SqlParameter("@UserId", SqlDbType.Int)};
            parms[0].Value = incidentId;
            parms[1].Value = userId;

            int i = Convert.ToInt32(SqlHelper.ExcuteScalar(CommandType.Text, strGet, parms));
            return i;
        }

        public int GetOwnerByIncidentId(string incidentId)
        {
            string strGet = "SELECT UserId FROM Incidents WHERE Id = @Id";
            SqlParameter[] parms = {				        
                new SqlParameter("@Id", SqlDbType.Int)};
            parms[0].Value = incidentId;
            DataSet ds = SqlHelper.ExcuteDataSet(CommandType.Text, strGet, parms);
            return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
        }
    }
}
