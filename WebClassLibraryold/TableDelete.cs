using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
namespace WebClassLibrary
{
   public class TableDelete
    {
        private int result = 0;
        private string errors = "";

        public int Delete(string tablename, string username, string conditions)
        {

            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["GDCConnectionString"].ToString());

            //string tempstr = " set stat01=" + statevalue + ",operationmodenote='" + notevalue + "' ";
            //string sql = "update " + username + "." + tablename + tempstr +
            //                  " where " + datefield + "=TO_DATE('" + datevalue + "','YY-MM-DD') " +
            //                  " and " + unitfield + "=" + unitvalue;

            string sql = "delete from " + username + "." + tablename;
            sql = sql +" where " + conditions;


            SqlCommand cmd = new SqlCommand(sql, cn);

            DataTable dt = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            ad.SelectCommand = cmd;


            try
            {
                cn.Open();
                ad.Fill(dt);
                cmd.ExecuteNonQuery();


            }
            catch (Exception xcp)
            {
                errors = xcp.Message;
                result = 1; //failure of updating
            }
            finally
            {
                cmd.Dispose();
                cn.Dispose();
                cn.Close();
            }
            return result;

        }


    
    }
}
