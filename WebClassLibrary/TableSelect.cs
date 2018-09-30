using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace WebClassLibrary
{
    public class TableSelect
    {
        private int result = 0;

        private string errors = "";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tablename">table name</param>
        /// <param name="username">username</param>
        /// <param name="listedfield">"field1,field2"</param>
        /// <param name="conditoins">"field1=12.5 and field2=TO_DATE('12 May 2009','YYYY-MM-DD') and field3='abc'</param>
        /// <returns></returns>
        public DataTable DoSelect(string tablename, string username, string listedfield, string conditoins,string orderfields)
        {

            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["etcConnectionString"].ToString());

            ////string tempstr = " select  uninumber, recnumber,recdate,stat01,operationmodenote ";
            ////string sql = tempstr + " from  " + username + "." + tablename +
            ////                  " where " + datefield + "=TO_DATE('" + thetime + "','YYYY-MM-DD') " +
            ////                  "and " + varnumber + "=" + thenumber;

            string sql = "select ";
            if (conditoins == "")
            {
                sql = sql + listedfield + " from " + username + "." + tablename + " order by " + orderfields; 
            }
            else
            {
                sql = sql + listedfield + " from " + username + "." + tablename + " where " + conditoins + " order by "+ orderfields; 
            }
            SqlCommand cmd = new SqlCommand(sql, cn);

            DataTable dt = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();

            ad.SelectCommand = cmd;


            try
            {
                cn.Open();
                ad.Fill(dt);


            }
            catch (Exception xcp)
            {
                errors = xcp.Message;
                return null;
            }
            finally
            {
                cmd.Dispose();
                cn.Dispose();
                cn.Close();
            }
            return dt;

        }   



    }
}
