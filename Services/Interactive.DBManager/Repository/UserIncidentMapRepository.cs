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
    public class UserIncidentMapRepository
    {
        public void AddUserIncidentMap(UserIncidentMapEntity userIncidentMap)
        {
            string strInsert = "INSERT INTO UserIncidentMapping(UserId,IncidentId) VALUES (@UserId,@IncidentId)";
            SqlParameter[] parms = {
				new SqlParameter("@UserId", SqlDbType.Int),
				new SqlParameter("@IncidentId", SqlDbType.Int)};
            parms[0].Value = userIncidentMap.UserId;
            parms[1].Value = userIncidentMap.IncidentId;
            SqlHelper.ExcuteNonQuery(CommandType.Text, strInsert, parms);
        }

        public void DeleteUserIncidentMap(UserIncidentMapEntity userIncidentMap)
        {
            string sqlDelete = "DELETE FROM UserIncidentMapping WHERE UserId = @UserId AND IncidentId = @IncidentId";
            SqlParameter[] deleteParms = {				   
				new SqlParameter("@UserId", SqlDbType.Int),
                new SqlParameter("@IncidentId", SqlDbType.Int)};
            deleteParms[0].Value = userIncidentMap.UserId;
            deleteParms[1].Value = userIncidentMap.IncidentId;
            SqlHelper.ExcuteNonQuery(CommandType.Text, sqlDelete, deleteParms);
        }

        public UserIncidentMapEntity UpdateUserIncidentMap()
        {
            return null;
        }

        public UserIncidentMapEntity SelectUserIncidentMap()
        {
            return null;
        }

        public bool CheckUserIncidentMap(UserIncidentMapEntity userIncidentMap)
        {
            string sqlCheck = "SELECT COUNT(*) FROM UserIncidentMapping WHERE UserId = @UserId And IncidentId = @IncidentId";
            SqlParameter[] parms = {				   
				new SqlParameter("@UserId", SqlDbType.Int),
                new SqlParameter("@IncidentId", SqlDbType.Int)};
            parms[0].Value = userIncidentMap.UserId;
            parms[1].Value = userIncidentMap.IncidentId;
            int i = Convert.ToInt32(SqlHelper.ExcuteScalar(CommandType.Text, sqlCheck, parms));
            if (i > 0)
                return true;
            else
                return false;
        }
    }
}
