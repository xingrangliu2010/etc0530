using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace WebClassLibrary
{
    public class etAndRainfallHourlyData
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
        public DataTable DoSelect(string startDay, string endDay)
        {


            string conditoins = "(a.ts = b.ts) and  (a.ts>='" + startDay + "' and a.ts<='" + endDay + "' )";

            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["etcConnectionString"].ToString());

            ////string tempstr = " select  uninumber, recnumber,recdate,stat01,operationmodenote ";
            ////string sql = tempstr + " from  " + username + "." + tablename +
            ////                  " where " + datefield + "=TO_DATE('" + thetime + "','YYYY-MM-DD') " +
            ////                  "and " + varnumber + "=" + thenumber;



            string sql = "select " +
       "a.[ts] as theDateTime" +
       ",cast(a.[ts] as date) as theDay" +
       ",a.NetTot_Avg as Net_Radiation" +
       ",isnull((select  a.NetTot_Avg where  a.NetTot_Avg > 0),0) as  Net_Radiation_Corrected" +
       ",a.SHF_mean_Avg as Soil_Heat_Flux" +
       ",a.LE_wpl as Latenet_Heat_Flux" +
       ",a.Hc as Sensible_heat_flux" +
       ",a.Hc/isnull((select a.LE_wpl where  a.LE_wpl <> 0),0.00000001) as BR" +
       ",(a.NetTot_Avg-a.SHF_mean_Avg)/(1+a.Hc/isnull((select a.LE_wpl where  a.LE_wpl <> 0),0.00000001)) as leadjust1" +
       ",(a.NetTot_Avg-a.SHF_mean_Avg)/(1+a.Hc/isnull((select a.LE_wpl where  a.LE_wpl <> 0),0.00000001))/(28.36*24) as leadjust2" +
       ",abs((a.NetTot_Avg-a.SHF_mean_Avg)/(1+Hc/isnull((select a.LE_wpl where  a.LE_wpl <> 0),0.00000001))/(28.36*24)) as leadjust3" +
       ",b.ts" +
       ",b.[rainfall]  " +
       "FROM  [ETC].[dbo].[etc] a, [ETC].[dbo].[rainfall] b  " +
       "where   " + conditoins;

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
