using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace WebClassLibrary
{
   public class TableFk
    {
        private int result = 0;
        private int currentFk;
        private string errors = "";
        public Int32 GetFk(string username,string tableName, string fkName)
        {


            //SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["cn"].ToString());
            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["etcConnectionString"].ToString());

            //string sql = "insert into shanghai.UnitStatus(StatusNumber,StatusName)"
            //           + " values(:UniNumber,:UniName)";

            //string sql = "select max(" + fkName + ") as MaxNumber from shanghai." + tableName;
            
            string sql = "select max(" + fkName + ") as MaxNumber from "+username+"." + tableName;

           SqlCommand cmd = new SqlCommand(sql, cn);

            cmd.CommandType = CommandType.Text;

            Int32 result = 1;

            try
            {
                cn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(rdr, LoadOption.Upsert);
                string temp1 = table.Rows[0]["MaxNumber"].ToString();
                if (temp1 == "")
                    temp1 = "0";
                
                result = Convert.ToInt16(temp1);
                currentFk = result;
                result++;

            }
            catch (Exception xcp)
            {
                errors = xcp.Message;
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
