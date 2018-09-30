using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace WebClassLibrary
{
   public class TableInsert
    {

        private int result = 0;

        private string errors = "";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tablename">table name</param>
        /// <param name="username">schema name</param>
        /// <param name="datefield">date field</param>
        /// <param name="thetime">the date to set data </param>
        /// <param name="varnumber">parameter ID</param>
        /// <param name="thenumber">parameter ID value</param>
        /// <param name="values">the different time value for the parameter that ID is thenumber</param>
        /// <returns></returns>
        public int Insert(string tablename, string username, string fields,string values)
        {

            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["GDCConnectionString"].ToString());

            //string tempstr = " set stat01=" + statevalue + ",operationmodenote='" + notevalue + "' ";
            //string sql = "update " + username + "." + tablename + tempstr +
            //                  " where " + datefield + "=TO_DATE('" + datevalue + "','YY-MM-DD') " +
            //                  " and " + unitfield + "=" + unitvalue;

            string sql = "insert into " + username + "." + tablename+"("+fields+")"+" values("+values+")";
            
            SqlCommand cmd = new  SqlCommand(sql, cn);

      



            try
            {
                cn.Open();
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
