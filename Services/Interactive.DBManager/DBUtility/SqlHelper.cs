
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive.DBManager
{
    public class SqlHelper
    {
        public static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["FootprintsMobileAppConnection"].ToString();

        private static void PrepareCommand(SqlConnection conn, SqlCommand cmd, CommandType ct, string CommandString, SqlParameter[] param)
        {
            //if close,open 
            if (conn.State != ConnectionState.Open)
                conn.Open();

            //bind sqlcommand
            cmd.Connection = conn;
            cmd.CommandText = CommandString;
            cmd.CommandType = ct;

            //if have param
            if (param != null)
            {
                foreach (SqlParameter pa in param)
                {
                    cmd.Parameters.Add(pa);
                }
            }
        }
        public static object ExcuteScalar(CommandType ct, string CommandString, params SqlParameter[] param)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                PrepareCommand(conn, cmd, ct, CommandString, param);
                object o = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return o;
            }
        }

        public static int ExcuteNonQuery(CommandType ct, string CommandString, params SqlParameter[] param)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                PrepareCommand(conn, cmd, ct, CommandString, param);
                int i = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return i;
            }
        }

        public static SqlDataReader ExcuteReader(CommandType ct, string CommandString, params SqlParameter[] param)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(ConnectionString);
            try
            {
                PrepareCommand(conn, cmd, ct, CommandString, param);
                SqlDataReader sdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return sdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        public static DataSet ExcuteDataSet(string SqlString)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlDataAdapter sda = new SqlDataAdapter(SqlString, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                return ds;
            }
        }

        public static DataSet ExcuteDataSet(CommandType ct, string SqlString, SqlParameter[] param)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                PrepareCommand(conn, cmd, ct, SqlString, param);
                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = cmd;
                DataSet ds = new DataSet();
                sda.Fill(ds);
                return ds;
            }
        }
    }
}
