using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace WebClassLibrary
{
    public class RainfallData
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
        //public DataTable DoSelect(string tablename, string username, string listedfield, string conditoins,string orderfields)
        public DataTable DoSelect(string theDay)
        {


            string conditoins = " ts>='" + theDay + " " + "00:00:00' and ts<='" + theDay + " " + "23:00:00' ";

            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["etcConnectionString"].ToString());

            ////string tempstr = " select  uninumber, recnumber,recdate,stat01,operationmodenote ";
            ////string sql = tempstr + " from  " + username + "." + tablename +
            ////                  " where " + datefield + "=TO_DATE('" + thetime + "','YYYY-MM-DD') " +
            ////                  "and " + varnumber + "=" + thenumber;



            string sql = "select sum(rainfall) as totalRain from dbo.rainfall where   " + conditoins;

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
