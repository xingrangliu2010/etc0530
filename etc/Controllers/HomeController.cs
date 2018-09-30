using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using etc.Models;
using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using WebClassLibrary;
using System.Data;
using System.IO;
using System.Configuration;

namespace etc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(lineDataTimeSpan model)
        {
            string startDate = model.start;


            if (startDate == null)
            {
                startDate = Convert.ToString(DateTime.Now.AddMonths(-1));
            }

            DateTime theStartDate = Convert.ToDateTime(startDate);
            theStartDate = theStartDate.AddDays(-1);

            //get the day before the  startDate data for daily et calcualting
            startDate = Convert.ToString(theStartDate);
            string strYear5 = "";
            string strMonth5 = "";
            string strDay5 = "";
            strYear5 = Convert.ToString(theStartDate.Year);
            if (theStartDate.Month < 10)
            {
                strMonth5 = "0" + Convert.ToString(theStartDate.Month);
            }
            else
            {
                strMonth5 = Convert.ToString(theStartDate.Month);
            }
            if (theStartDate.Day < 10)
            {
                strDay5 = "0" + Convert.ToString(theStartDate.Day);
            }
            else
            {
                strDay5 = Convert.ToString(theStartDate.Day);
            }

            startDate = strYear5 + "-" + strMonth5 + "-" + strDay5;


            string endDate = model.end;

            if (endDate == null)
            {
                endDate = Convert.ToString(DateTime.Now);
            }

            DateTime theEndDate = Convert.ToDateTime(endDate);
            string strYear6 = "";
            string strMonth6 = "";
            string strDay6 = "";
            strYear6 = Convert.ToString(theEndDate.Year);
            if (theEndDate.Month < 10)
            {
                strMonth6 = "0" + Convert.ToString(theEndDate.Month);
            }
            else
            {
                strMonth6 = Convert.ToString(theEndDate.Month);
            }
            if (theEndDate.Day < 10)
            {
                strDay6 = "0" + Convert.ToString(theEndDate.Day);
            }
            else
            {
                strDay6 = Convert.ToString(theEndDate.Day);
            }

            endDate = strYear6 + "-" + strMonth6 + "-" + strDay6;



            TimeSpan timeSpan = theEndDate - theStartDate;

            int days = timeSpan.Days;
            days++;//including the day of both startDate and endDate

            string[] allDate = new string[days];
            double[] allData = new double[days];
            double[] allRain = new double[days];

            double[] originalAllData = new double[days];

            double[] allData2 = new double[days - 1];
            string[] allDate2 = new string[days - 1];
            double[] allRain2 = new double[days - 1];

            RainfallData rainfallData = new RainfallData();
            etData theEtData = new etData();





            for (int i = 0; i < days; i++)
            {


                DateTime tempDate2 = theStartDate.AddDays(i);
                string strYear2 = "";
                string strMonth2 = "";
                string strDay2 = "";
                strYear2 = Convert.ToString(tempDate2.Year);
                if (tempDate2.Month < 10)
                {
                    strMonth2 = "0" + Convert.ToString(tempDate2.Month);
                }
                else
                {
                    strMonth2 = Convert.ToString(tempDate2.Month);
                }
                if (tempDate2.Day < 10)
                {
                    strDay2 = "0" + Convert.ToString(tempDate2.Day);
                }
                else
                {
                    strDay2 = Convert.ToString(tempDate2.Day);
                }

                string standartDate2 = strYear2 + "-" + strMonth2 + "-" + strDay2;
                allDate[i] = standartDate2;

                allData[i] = 0;
                //DataTable rainfallDataTable = rainfallData.DoSelect("2016-03-02");

                //get each day's rain data
                double theDayRainFall = 0;
                DataTable rainfallDataTable = rainfallData.DoSelect(standartDate2);
                if ((rainfallDataTable != null) && (rainfallDataTable.Rows.Count > 0))
                {
                    string tempstr = Convert.ToString(rainfallDataTable.Rows[0]["totalRain"]);
                    if (double.TryParse(tempstr, out theDayRainFall))
                    {
                        theDayRainFall = Math.Round(theDayRainFall, 2);
                    }
                }
                allRain[i] = theDayRainFall;

                //get each day's et data

                double theDayEt = 0;
                DataTable etDataTable = theEtData.DoSelect(standartDate2);
                if ((etDataTable != null) && (etDataTable.Rows.Count > 0))
                {
                    string tempstr = Convert.ToString(etDataTable.Rows[0]["etDailyData"]);
                    if (double.TryParse(tempstr, out theDayEt))
                    {
                        theDayEt = Math.Round(theDayEt, 2);
                    }
                }
                allData[i] = theDayEt;


            }
            //calculatting  final et

            for (int i = 0; i < days; i++)
            {
                originalAllData[i] = allData[i];
            }

            for (int i = 0; i < (days - 1); i++)
            {
                allData[0] = 0;
                allData[i + 1] = allData[i] - originalAllData[i + 1] + allRain[i + 1];
                allData2[i] = Math.Round(allData[i + 1], 2);
                allRain2[i] = Math.Round(allRain[i + 1], 2);
                allDate2[i] = allDate[i + 1];
            }

            //Geting final et data of each day within the seleted period 

            ////////for (int i = 0; i < (days - 1); i++)
            ////////{
            ////////    allData[i + 1] = allData[i] - originalAllData[i + 1] + allRain[i + 1];
            ////////    allData2[i] = Math.Round(allData[i + 1], 2);
            ////////    allRain2[i] = Math.Round(allRain[i + 1], 2);
            ////////    allDate2[i] = allDate[i + 1];
            ////////}


            //////return the input data for test
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.test = "This is test__";

            //get the latest date D1 from  database and check if more sensor data is required to be stored in database

            int theResult = checkAndStoreSensorData();


            //get the time span on which ET data is required by the users



            //check if the hourly .dat files after D1 exist. If yes, put these data in to database one by one. If no, go to next step. 


            //seleting the raw hourly data into different datatables 


            //NetTot_Avg        
            //SHF_mean_Avg
            //LE_wpl
            //Hc

            TableSelect dataselect = new TableSelect();


            //////DataTable dt = dataselect.DoSelect("etc", "[ETC].[dbo]", "recordNum"
            //////    + ",ts"
            //////    + ",cast([ts] as date) as theDay"
            //////    + ",NetTot_Avg"
            //////    + ",isnull((select  NetTot_Avg where  NetTot_Avg > 0),0) as  NetTot_Avg1"
            //////    + ",SHF_mean_Avg"
            //////    + ",LE_wpl"
            //////    + ",Hc"
            //////    + ",Hc/isnull((select LE_wpl where  LE_wpl <> 0),0.00000001) as br"
            //////    + ",(LE_wpl-SHF_mean_Avg)/(isnull((select (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) where  (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) <> 0),0.00000001)) as leadjust1"
            //////    + ",((LE_wpl-SHF_mean_Avg)/(isnull((select (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) where  (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) <> 0),0.00000001)))/(28.36*24) as leadjust2"
            //////    + ",isnull((select (((LE_wpl-SHF_mean_Avg)/(isnull((select (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) where  (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) <> 0),0.00000001)))/(28.36*24))*-1 where  (((LE_wpl-SHF_mean_Avg)/(isnull((select (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) where  (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) <> 0),0.00000001)))/(28.36*24)) <0),((LE_wpl-SHF_mean_Avg)/(isnull((select (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) where  (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) <> 0),0.00000001)))/(28.36*24)) as leadjust3", "convert(date,ts)>='" + startDate + "' and convert(date,ts)<='" + endDate + "'", " recordNum");

            ////DataTable dt = dataselect.DoSelect("etc", "[ETC].[dbo]", "recordNum"
            ////    + ",ts"
            ////    + ",cast([ts] as date) as theDay"
            ////    + ",NetTot_Avg"
            ////    + ",isnull((select  NetTot_Avg where  NetTot_Avg > 0),0) as  NetTot_Avg1"
            ////    + ",SHF_mean_Avg"
            ////    + ",LE_wpl"
            ////    + ",Hc"
            ////    + ",Hc/isnull((select LE_wpl where  LE_wpl <> 0),0.00000001) as br"
            ////    + ",(LE_wpl-SHF_mean_Avg)/(isnull((select (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) where  (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) <> 0),0.00000001)) as leadjust1"
            ////    + ",((LE_wpl-SHF_mean_Avg)/(isnull((select (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) where  (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) <> 0),0.00000001)))/(28.36*24) as leadjust2"
            ////    + ",isnull((select (((LE_wpl-SHF_mean_Avg)/(isnull((select (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) where  (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) <> 0),0.00000001)))/(28.36*24))*-1 where  (((LE_wpl-SHF_mean_Avg)/(isnull((select (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) where  (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) <> 0),0.00000001)))/(28.36*24)) <0),((LE_wpl-SHF_mean_Avg)/(isnull((select (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) where  (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) <> 0),0.00000001)))/(28.36*24)) as leadjust3", "convert(date,ts)>='" + startDate + "' and convert(date,ts)<='" + endDate + "'", " recordNum");



            ////if (dt != null)
            ////{

            ////    if (dt.Rows.Count > 0)
            ////    {
            ////        ////string[] x = new string[dt.Rows.Count];
            ////        ////double[] y = new double[dt.Rows.Count];
            ////for (int i = 0; i < dt.Rows.Count; i++)
            ////{
            ////    x[i] = dt.Rows[i]["theDay"].ToString();
            ////    y[i] = Convert.ToDouble(dt.Rows[i]["leadjust3"]);

            ////}

            ////string tempDay = "";
            ////double sumHourlyData = 0;

            ////if (x.Length > 0)
            ////{
            ////    tempDay = x[0];
            ////}
            ////////for (int i = 0; i < dt.Rows.Count; i++)
            ////////{
            ////////    if (x[i] == tempDay)
            ////////    {

            ////////        sumHourlyData = sumHourlyData + y[i];
            ////////        tempDay = x[i];
            ////////    }
            ////////    else
            ////////    {

            ////////        DateTime tempDate1 = Convert.ToDateTime(tempDay);

            ////////        string strYear = "";
            ////////        string strMonth = "";
            ////////        string strDay = "";
            ////////        strYear = Convert.ToString(tempDate1.Year);
            ////////        if (tempDate1.Month < 10)
            ////////        {
            ////////            strMonth = "0" + Convert.ToString(tempDate1.Month);
            ////////        }
            ////////        else
            ////////        {
            ////////            strMonth = Convert.ToString(tempDate1.Month);
            ////////        }
            ////////        if (tempDate1.Day < 10)
            ////////        {
            ////////            strDay = "0" + Convert.ToString(tempDate1.Day);
            ////////        }
            ////////        else
            ////////        {
            ////////            strDay = Convert.ToString(tempDate1.Day);
            ////////        }

            ////////        string standartDate1 = strYear + "-" + strMonth + "-" + strDay;

            ////////        for (int j = 0; j < days; j++)
            ////////        {

            ////////            string standartDate2 = allDate[j];

            ////////            if (standartDate1 == standartDate2)
            ////////            {
            ////////                allData[j] = sumHourlyData;
            ////////                sumHourlyData = 0;
            ////////            }
            ////////        }


            ////////        tempDay = x[i];
            ////////        sumHourlyData = sumHourlyData + y[i];

            ////////    }

            ////////}

            // for the day of endDate data
            ////if (x.Length > 0)
            ////{
            ////    DateTime tempDate8 = Convert.ToDateTime(tempDay);

            ////    string strYear8 = "";
            ////    string strMonth8 = "";
            ////    string strDay8 = "";
            ////    strYear8 = Convert.ToString(tempDate8.Year);
            ////    if (tempDate8.Month < 10)
            ////    {
            ////        strMonth8 = "0" + Convert.ToString(tempDate8.Month);
            ////    }
            ////    else
            ////    {
            ////        strMonth8 = Convert.ToString(tempDate8.Month);
            ////    }
            ////    if (tempDate8.Day < 10)
            ////    {
            ////        strDay8 = "0" + Convert.ToString(tempDate8.Day);
            ////    }
            ////    else
            ////    {
            ////        strDay8 = Convert.ToString(tempDate8.Day);
            ////    }

            ////    string standartDate8 = strYear8 + "-" + strMonth8 + "-" + strDay8;
            ////    for (int j = 0; j < days; j++)
            ////    {

            ////        string standartDate2 = allDate[j];

            ////        if (standartDate8 == standartDate2)
            ////        {
            ////            allData[j] = sumHourlyData;
            ////            sumHourlyData = 0;
            ////        }
            ////    }
            ////}
            //ETecv (final et)

            //rainfall data is just for testing
            //////for (int i = 0; i < days; i++)
            //////{
            //////    originalAllData[i] = allData[i];
            //////    allRain[i] = GetRandomNumber(0, 10.34);
            //////}




            //////////Geting final et data of each day within the seleted period 

            ////////for (int i = 0; i < (days - 1); i++)
            ////////{
            ////////    allData[i + 1] = allData[i] - originalAllData[i + 1] + allRain[i + 1];
            ////////    allData2[i] = Math.Round(allData[i + 1], 2);
            ////////    allRain2[i] = Math.Round(allRain[i + 1], 2);
            ////////    allDate2[i] = allDate[i + 1];
            ////////}

            ////    }
            ////}


            //Highcharts
            //For rainfall data
            Series rainfall = new Series();
            object[] objRainfall = new object[days - 1];
            for (int i = 0; i < (days - 1); i++)
            {
                objRainfall[i] = allRain2[i];
            }

            rainfall.Data = new Data(objRainfall);

            //for et data

            Series et = new Series();
            object[] objEt = new Object[days - 1];
            for (int i = 0; i < (days - 1); i++)
            {
                objEt[i] = allData2[i];
            }
            et.Data = new Data(objEt);

            rainfall.Name = "Rainfall";
            rainfall.Type = ChartTypes.Column;

            et.Name = "ET";
            et.Type = ChartTypes.Line;

            Series[] theSeries = new Series[] { rainfall, et };

            Highcharts theChart = new Highcharts("ET")
            .SetTitle(new Title
            {
                Text = "ET and Rainfall Data Analysis"
            })
            .SetSubtitle(new Subtitle
            {
                Text = "ET and Rainfall Data"

            }
            )
            .SetXAxis(new XAxis
            {
                Categories = allDate2

            }

            ).SetSeries(theSeries);

            ViewBag.theChartModel = theChart;


            //get hourly data of et and rainfall within the same periond 

            //get hourly data of et and rainfall within the same periond 
            
            string dateTime1 = startDate+ " 00:00:00";
            string dateTime2 = endDate+ " 23:00:00";

            etAndRainfallHourlyData theEtRainfallHourlydata = new etAndRainfallHourlyData();

            DataTable dt = theEtRainfallHourlydata.DoSelect(dateTime1,dateTime2);

            ViewData["hourlyData"] = dt;



            //get the period data between start date and end date to display

            //string dateTime1 = theYear + "-" + theMonth + "-" + theDay + " 00:00:00";
            //string dateTime2 = theYear + "-" + theMonth + "-" + theDay + " 23:00:00";

            //dt = dataselect.DoSelect("rainfall", "dbo", "ts,rainfall", " ts>='" + dateTime1 + "' and ts<='" + dateTime2 + "'", "recordNum");



            //ViewData["oneDayData"] = dt;





            //calculating daily data and put them into 


            ////Create Series 1
            //var series1 = new Series();
            //series1.Name = "Area Series: ET ";
            //Point[] series1Points =
            //    {
            //        new Point() {X = 0.0, Y = 0.0},
            //        new Point() {X = 10.0, Y = 0.0},
            //        new Point() {X = 10.0, Y = 10.0},
            //        new Point() {X = 0.0, Y = 10.0}
            //    };
            //series1.Data = new Data(series1Points);

            ////Create Series 2
            //var series2 = new Series();
            //series2.Name = "Area Series: Rainfall";
            //Point[] series2Points =
            //    {
            //        new Point() {X = 5.0, Y = 5.0},
            //        new Point() {X = 15.0, Y =5.0},
            //        new Point() {X = 15.0, Y = 15.0},
            //        new Point() {X = 5.0, Y = 15.0}
            //    };
            //series2.Data = new Data(series2Points);

            ////Create List of Series and Add both series to the collection
            //var chartSeries = new List<Series>();
            //chartSeries.Add(series1);
            //chartSeries.Add(series2);

            ////Create chart Model
            //var chart1 = new Highcharts("Chart1");
            //chart1
            //    .InitChart(new Chart() { DefaultSeriesType = ChartTypes.Area })
            //    .SetTitle(new Title() { Text = "ET and Rainfall data" })
            //    .SetSeries(chartSeries.ToArray());

            ////pass Chart1Model using ViewBag
            //ViewBag.Chart1Model = chart1;

            return View();
        }
        public ActionResult InputRainfallData(rainfallData model)
        {
            string userInput = model.theDate + "|| " + Convert.ToString(model.theHour) + " : " + Convert.ToString(model.theData);
            int result = 0;
            DateTime inputDate = Convert.ToDateTime(model.theDate);
            string stringMinDate = "2010-01-01";
            DateTime minDate = Convert.ToDateTime(stringMinDate);

            DateTime tempDateTime = inputDate;
            string theHour = Convert.ToString(model.theHour);
            string inputRainfall = Convert.ToString(model.theData);

            string theYear = "";
            string theMonth = "";
            string theDay = "";

            theYear = Convert.ToString(tempDateTime.Year);

            if (tempDateTime.Month > 9)
            {
                theMonth = Convert.ToString(tempDateTime.Month);
            }
            else
            {
                theMonth = "0" + Convert.ToString(tempDateTime.Month);
            }
            if (tempDateTime.Day > 9)
            {
                theDay = Convert.ToString(tempDateTime.Day);
            }
            else
            {
                theDay = "0" + Convert.ToString(tempDateTime.Day);
            }

            string theFinalInputDateTime = theYear + "-" + theMonth + "-" + theDay + " " + theHour + ":00:00.000";




            //check if this hour data has been stored in the database. If no insert else update 

            //preparing to insert data
            TableFk tb = new TableFk();    //To prepare iMISID field
            long theId = 1;
            theId = tb.GetFk("dbo", "rainfall", "recordNum");
            string strtheId = Convert.ToString(theId);

            //check if this data has been inserted 
            TableSelect tbSelect = new TableSelect();
            DataTable dt = tbSelect.DoSelect("rainfall", "dbo", "recordNum,ts", " ts='" + theFinalInputDateTime + "'", "recordNum");

            if (minDate < inputDate)
            {


                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    //updating
                    int theresult = 0;
                    TableUpdate updatetable = new TableUpdate();
                    string tableName = "rainfall";
                    string userName = "dbo";
                    string fieldvalue = "rainfall=" + inputRainfall;

                    string conditions = "ts='" + theFinalInputDateTime + "'";

                    theresult = updatetable.Update(tableName, userName, fieldvalue, conditions);



                }
                else
                {
                    //inserting 


                    TableInsert tbinsert = new TableInsert();
                    int operationResultStat = 0;
                    string tableName = "rainfall";
                    string userName = "dbo";
                    string tbfileds = "recordNum,ts,rainfall";

                    string recordValues = strtheId + ",'" + theFinalInputDateTime + "'," + inputRainfall;

                    operationResultStat = tbinsert.Insert(tableName, userName, tbfileds, recordValues);
                    result = operationResultStat;

                }


                //get the 24 hour data of the date to display

                string dateTime1 = theYear + "-" + theMonth + "-" + theDay + " 00:00:00";
                string dateTime2 = theYear + "-" + theMonth + "-" + theDay + " 23:00:00";

                dt = tbSelect.DoSelect("rainfall", "dbo", "ts,rainfall", " ts>='" + dateTime1 + "' and ts<='" + dateTime2 + "'", "recordNum");



                ViewData["oneDayData"] = dt;
                //ViewBag.theDayData = dt;
            }
            ViewBag.fromControllerMessage = "The user input is " + userInput;
            return View();
        }
        public ActionResult About(HttpPostedFileBase file)
        {
            string uploadedFileName = "";

            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    string path = Path.Combine(Server.MapPath("~/uploadedFiles"),
                                                Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    uploadedFileName = Path.GetFileName(file.FileName);
                    ViewBag.Message = "File uploaded successfully";
                    int result = storeRainfallData(uploadedFileName);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Error:" + ex.Message.ToString();
                }
            }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }


            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Users can delete sensor data which has been stored and processed by server. The data which has not been stored by server is limited to delete.";

            return View();
        }
        public ActionResult theTimeSpan()
        {
            ETtimeSpan timeSpanModel = new ETtimeSpan();
            return View(timeSpanModel);
        }

        public ActionResult getTimeSpan(ETtimeSpan model)
        {
            string userInput = model.start + " ; " + model.end;
            string startDate = model.start;
            string endDate = model.end;
            //////Checking all the hourly data record to confirm if it has been stored in database one by one 
            //get time span to get file name of each hourly data file within the time span
            if (startDate == null)
            {
                startDate = Convert.ToString(DateTime.Now.AddMonths(-1)); //default to check if delete hourly data from one month ago to one week agao  
            }

            DateTime theStartDate = Convert.ToDateTime(startDate);
            theStartDate = theStartDate.AddDays(-1);

            //get the day before the  startDate data for daily et calcualting
            startDate = Convert.ToString(theStartDate);
            string strYear5 = "";
            string strMonth5 = "";
            string strDay5 = "";
            strYear5 = Convert.ToString(theStartDate.Year);
            if (theStartDate.Month < 10)
            {
                strMonth5 = "0" + Convert.ToString(theStartDate.Month);
            }
            else
            {
                strMonth5 = Convert.ToString(theStartDate.Month);
            }
            if (theStartDate.Day < 10)
            {
                strDay5 = "0" + Convert.ToString(theStartDate.Day);
            }
            else
            {
                strDay5 = Convert.ToString(theStartDate.Day);
            }

            startDate = strYear5 + "-" + strMonth5 + "-" + strDay5;


            

            if (endDate == null)
            {
                endDate = Convert.ToString(DateTime.Now.AddDays(-7));
                //default end date is one week ago

            }

            DateTime theEndDate = Convert.ToDateTime(endDate);
            string strYear6 = "";
            string strMonth6 = "";
            string strDay6 = "";
            strYear6 = Convert.ToString(theEndDate.Year);
            if (theEndDate.Month < 10)
            {
                strMonth6 = "0" + Convert.ToString(theEndDate.Month);
            }
            else
            {
                strMonth6 = Convert.ToString(theEndDate.Month);
            }
            if (theEndDate.Day < 10)
            {
                strDay6 = "0" + Convert.ToString(theEndDate.Day);
            }
            else
            {
                strDay6 = Convert.ToString(theEndDate.Day);
            }

            endDate = strYear6 + "-" + strMonth6 + "-" + strDay6;



            DateTime dateTime1 = Convert.ToDateTime(startDate+" "+"00:00:00");
            DateTime dateTime2 = Convert.ToDateTime(endDate+" "+"23:00:00");

            TimeSpan hoursSpan = dateTime2 - dateTime1;
            double passedHours = hoursSpan.TotalHours;
            double iPassedHours = Math.Round(passedHours);
            TimeSpan hourIntervel = new TimeSpan(0, 1, 0, 0); //one hour 
            DateTime tempDateTime = dateTime1.AddHours(-1);
            string theYear = "";
            string theMonth = "";
            string theDay = "";
            string theHour = "";
            string theFileName = "";
            if (passedHours >= 1)
            {
                for (int i = 1; i <= iPassedHours; i++)
                {
                    tempDateTime = tempDateTime + hourIntervel;
                    theYear = Convert.ToString(tempDateTime.Year);

                    if (tempDateTime.Month > 9)
                    {
                        theMonth = Convert.ToString(tempDateTime.Month);
                    }
                    else
                    {
                        theMonth = "0" + Convert.ToString(tempDateTime.Month);
                    }
                    if (tempDateTime.Day > 9)
                    {
                        theDay = Convert.ToString(tempDateTime.Day);
                    }
                    else
                    {
                        theDay = "0" + Convert.ToString(tempDateTime.Day);
                    }
                    if (tempDateTime.Hour > 9)
                    {

                        theHour = Convert.ToString(tempDateTime.Hour);
                    }
                    else
                    {
                        theHour = "0" + Convert.ToString(tempDateTime.Hour);
                    }
                    theYear = Convert.ToString(tempDateTime.Year);
                    theFileName = "Waverley_" + theYear + "-" + theMonth + "-" + theDay + "_" + theHour + "-00_FluxData" + ".dat";
                    // Check and call etDataFileDelete if the hourly data file has been stored in database
                    TableSelect tbSelect = new TableSelect();

                    DataTable dt = tbSelect.DoSelect("etc", "dbo", "recordNum,ts", " ts='" + theYear + "-" + theMonth + "-" + theDay + " " + theHour+":00:00'", "recordNum");
                    int deleteOperationResult = 1;
                    if (!((dt != null) && (dt.Rows.Count > 0)))
                    {
                        //// The hourly data has not been stored in database.

                    }
                    else
                    {

                        ////The hourly data has been stored in database.
                        // call etDataFileDelete()
                        deleteOperationResult = DeleteEtDataFile(theFileName);


                    }

                    if(deleteOperationResult==1)
                    {
                        ViewBag.deleteOperationResponse = "Deleting hourly data file is unsuccessful.";
                    }
                }

            }


            //////return Content(deleteResponse.ToString());
            //ViewBag.fromControllerMessage = "The user input is " + userInput;
            //ViewBag.test = "This is test__";
            return View();
        }

        public int checkAndStoreSensorData()
        {
            int result = 0;
            //getting now time
            DateTime nowTime = DateTime.Now;

            //getting latest time in database
            TableFk tb = new TableFk();    //To prepare iMISID field
            long theId = 1;
            theId = tb.GetFk("dbo", "etc", "recordNum");
            string strtheId = Convert.ToString(theId - 1);

            //get the latest time based on the main key

            string latedDatabaseTime = "";
            TableSelect tbSelect = new TableSelect();
            DataTable dt = tbSelect.DoSelect("etc", "dbo", "recordNum,ts", " recordNum=" + strtheId, "recordNum");
            if (dt.Rows.Count == 0)
            {
                ////Display("The database is empty. Transferring all data from the hosting server."); //this will never happer because some records should be imported in the initial stage.
            }
            else
            {
                //Get the latest time of database
                latedDatabaseTime = dt.Rows[0]["ts"].ToString();
            }

            DateTime latestDatabaseTime = Convert.ToDateTime(latedDatabaseTime);

            TimeSpan hoursSpan = nowTime - latestDatabaseTime;
            double passedHours = hoursSpan.TotalHours;
            double iPassedHours = Math.Round(passedHours);
            TimeSpan hourIntervel = new TimeSpan(0, 1, 0, 0); //one hour 
            DateTime tempDateTime = DateTime.Now;
            string theYear = "";
            string theMonth = "";
            string theDay = "";
            string theHour = "";
            string theFileName = "";
            if (passedHours >= 1)
            {
                tempDateTime = latestDatabaseTime;
                for (int i = 1; i <= iPassedHours; i++)
                {
                    tempDateTime = tempDateTime + hourIntervel;
                    theYear = Convert.ToString(tempDateTime.Year);

                    if (tempDateTime.Month > 9)
                    {
                        theMonth = Convert.ToString(tempDateTime.Month);
                    }
                    else
                    {
                        theMonth = "0" + Convert.ToString(tempDateTime.Month);
                    }
                    if (tempDateTime.Day > 9)
                    {
                        theDay = Convert.ToString(tempDateTime.Day);
                    }
                    else
                    {
                        theDay = "0" + Convert.ToString(tempDateTime.Day);
                    }
                    if (tempDateTime.Hour > 9)
                    {

                        theHour = Convert.ToString(tempDateTime.Hour);
                    }
                    else
                    {
                        theHour = "0" + Convert.ToString(tempDateTime.Hour);
                    }
                    theYear = Convert.ToString(tempDateTime.Year);
                    theFileName = "Waverley_" + theYear + "-" + theMonth + "-" + theDay + "_" + theHour + "-00_FluxData" + ".dat";
                    string path1 = ReadSetting("etDatafilePath");

                    //string filePath = @"e:\etcData";
                    string filePath = path1;
                    string dataLine = "";
                    string errors = "";
                    ////localhostETC.etcWebService etcService = new localhostETC.etcWebService();
                    try
                    {
                        dataLine = ReadTextData(filePath, theFileName);
                    }
                    catch (Exception ex)
                    {
                        errors = ex.ToString();
                    }
                    if (dataLine != "")// transferred the .dat file data into database 
                    {
                        string[] stringArray = dataLine.Split(',');
                        //processing stringArray[0]
                        string stringArray0 = stringArray[0];
                        stringArray0 = stringArray0.Substring(1, stringArray0.Length - 2);
                        stringArray[0] = stringArray0;
                        TableInsert tbinsert = new TableInsert();
                        int operationResultStat = 0;
                        string tableName = "etc";
                        string userName = "dbo";
                        string tbfileds = "recordNum,ts";
                        for (int j = 2; j < dt.Columns.Count; j++)
                        {
                            tbfileds = tbfileds + "," + dt.Columns[i];
                        }

                        tbfileds = tbfileds + ",[cov_h2o_Ux_Avg],[cov_h2o_Uy_Avg],[cov_h2o_Uz_Avg],[cov_h2o_Uz_Tot],[cov_co2_Ux_Avg],[cov_co2_Uy_Avg],[cov_co2_Uz_Avg],[cov_co2_Uz_Tot]" +
                            ",[wnd_dir_compass_Avg],[wnd_dir_csat3_Avg],[wnd_spd_Avg],[rslt_wnd_spd_Avg],[Hs],[Fc_wpl],[LE_wpl],[Hc],[tau],[u_star]" +
                            ",[Ts_mean],[stdev_Ts],[cov_Ts_Ux],[cov_Ts_Uy],[cov_Ts_Uz],[co2_mean],[stdev_co2],[cov_co2_Ux],[cov_co2_Uy],[cov_co2_Uz]" +
                            ",[h2o_Avg],[stdev_h2o],[cov_h2o_Ux],[cov_h2o_Uy],[cov_h2o_Uz],[Ux_Avg],[stdev_Ux],[cov_Ux_Uy],[cov_Ux_Uz],[Uy_Avg]" +
                            ",[stdev_Uy],[cov_Uy_Uz],[Uz_Avg],[stdev_Uz],[press_mean],[rho_a_mean],[wnd_dir_compass],[wnd_dir_csat3],[wnd_spd],[rslt_wnd_spd]" +
                            ",[std_wnd_dir],[Fc_irga],[LE_irga],[co2_wpl_LE],[co2_wpl_H],[h2o_wpl_LE],[h2o_wpl_H],[SHF1_raw_Avg],[SHF2_raw_Avg],[SHF_mean_Avg]" +
                            ",[SR01Up_Avg] ,[SR01Dn_Avg],[IR01Up_Avg],[IR01Dn_Avg],[NR01TC_Avg],[NR01TK_Avg],[NetRs_Avg],[NetRl_Avg],[Albedo_Avg],[UpTot_Avg]" +
                            ",[DnTot_Avg],[NetTot_Avg],[IR01UpCo_Avg],[IR01DnCo_Avg],[Canopy_Temp_Avg],[AirTC1_Avg],[RH1_Avg],[e_kPa1_Avg],[e_Sat1_Avg],[VPD1_Avg]" +
                            ",[AirTC2_Avg],[RH2_Avg],[e_kPa2_Avg],[e_Sat2_Avg],[VPD2_Avg],[n_Tot],[csat_warnings],[irga_warnings],[del_T_f_Tot],[sig_lck_f_Tot]" +
                            ",[amp_h_f_Tot],[amp_l_f_Tot],[chopper_f_Tot],[detector_f_Tot],[pll_f_Tot],[sync_f_Tot],[agc_Avg],[panel_temp_Avg],[batt_volt_Avg]";

                        string recordValues = Convert.ToString(stringArray[1]) + ",'" + Convert.ToString(stringArray[0]) + "'";
                        for (int j = 2; j < stringArray.Length; j++)
                        {

                            recordValues = recordValues + "," + Convert.ToString(stringArray[j]);
                        }


                        operationResultStat = tbinsert.Insert(tableName, userName, tbfileds, recordValues);
                        result = operationResultStat;


                    }
                    ////TextBox1.Text = dataLine;

                }

            }
            //Display("New ID number is created.");


            return result;
        }

        public string ReadTextData(string filePath, string datFile)
        {
            string dataLine = "";

            //string strExcelConn =
            //  @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source= c:\etcData\Waverley_2016-02-07_01-00_FluxData.dat;" +
            //  @"Extended Properties='Excel 12.0 Macro;HDR=Yes;'";

            int counter = 0;
            string[] dataLines = new string[5];
            string line;

            string theDatFile = filePath + @"\" + datFile;


            if (System.IO.File.Exists(theDatFile))
            {
                // Read the file and display it line by line.
                System.IO.StreamReader file =
                   new System.IO.StreamReader(theDatFile);
                while ((line = file.ReadLine()) != null)
                {

                    dataLines[counter] = line;
                    counter++;
                }

                file.Close();
            }
            else
            {
                dataLines[4] = "";
            }

            return dataLines[4];
        }
        public string[] ReadTextData1(string filePath, string datFile)
        {
            string dataLine = "";

            //string strExcelConn =
            //  @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source= c:\etcData\Waverley_2016-02-07_01-00_FluxData.dat;" +
            //  @"Extended Properties='Excel 12.0 Macro;HDR=Yes;'";

            int counter = 0;
            string[] dataLines = new string[5];
            string line;
            string[] processedLines = null; ;
            string theDatFile = filePath + @"\" + datFile;
            string allDataLine = "";
            string[] theSeparator1 = { "#" };
            if (System.IO.File.Exists(theDatFile))
            {
                // Read the file and display it line by line.
                System.IO.StreamReader file =
                   new System.IO.StreamReader(theDatFile);
                while ((line = file.ReadLine()) != null)
                {
                    counter++;
                    if (counter > 3)
                    {
                        allDataLine = allDataLine + '#' + line;
                    }


                }
                processedLines = allDataLine.Split(theSeparator1, StringSplitOptions.None);
                file.Close();

            }
            else
            {
                processedLines = null;
            }

            return processedLines;
        }

        public int storeRainfallData(string theFileName)
        {
            int result = 0;
            //string filePath = @"D:\etc\etc\uploadedFiles";
            string path1 = ReadSetting("rainFallDatafilePath");
            //string filePath = @"C:\2016codes\etc\etc\uploadedFiles";
            string filePath = path1;
            string[] dataLine = null;
            string errors = "";
            ////localhostETC.etcWebService etcService = new localhostETC.etcWebService();
            try
            {
                dataLine = ReadTextData1(filePath, theFileName);
                if (theFileName.Length > 0)
                {

                    int theOperationResultStat = DeleteRainfallDataFile(theFileName);

                    result = theOperationResultStat;
                }
            }
            catch (Exception ex)
            {
                errors = ex.ToString();
            }

            if (dataLine != null)// transferred the .dat file data into database 
            {

                //check and insert rainfall data
                string[] theSeparator2 = { "," };
                string theYear = "";
                string theMonth = "";
                string theDay = "";
                for (int i = 1; i < dataLine.Length; i++)
                {
                    string[] lineData = dataLine[i].Split(theSeparator2, StringSplitOptions.None);
                    string lineDate = lineData[0];
                    DateTime theLineDate = Convert.ToDateTime(lineDate);
                    DateTime tempDateTime = theLineDate;

                    theYear = Convert.ToString(tempDateTime.Year);

                    if (tempDateTime.Month > 9)
                    {
                        theMonth = Convert.ToString(tempDateTime.Month);
                    }
                    else
                    {
                        theMonth = "0" + Convert.ToString(tempDateTime.Month);
                    }
                    if (tempDateTime.Day > 9)
                    {
                        theDay = Convert.ToString(tempDateTime.Day);
                    }
                    else
                    {
                        theDay = "0" + Convert.ToString(tempDateTime.Day);
                    }

                    string theDateTime = theYear + "-" + theMonth + "-" + theDay + " " + lineData[1];

                    string totalRain = lineData[14];
                    // select the dateTime in the rainfall table, if not exite, insert it into the table
                    // select

                    //preparing to insert data
                    TableFk tb = new TableFk();    //To prepare iMISID field
                    long theId = 1;
                    theId = tb.GetFk("dbo", "rainfall", "recordNum");
                    string strtheId = Convert.ToString(theId);

                    //check if this data has been inserted 
                    TableSelect tbSelect = new TableSelect();
                    DataTable dt = tbSelect.DoSelect("rainfall", "dbo", "recordNum,ts", " ts='" + theDateTime + "'", "recordNum");


                    if ((dt != null) && (dt.Rows.Count > 0))
                    {
                        //nothing to do



                    }
                    else
                    {
                        //inserting 


                        TableInsert tbinsert = new TableInsert();
                        int operationResultStat = 0;
                        string tableName = "rainfall";
                        string userName = "dbo";
                        string tbfileds = "recordNum,ts,rainfall";

                        string recordValues = strtheId + ",'" + theDateTime + "'," + totalRain;

                        operationResultStat = tbinsert.Insert(tableName, userName, tbfileds, recordValues);
                        result = operationResultStat;

                    }




                } //for

                int test = 1;


            }

            return result;
        }
        public double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
        public int DeleteRainfallDataFile(string fileName)
        {
            int result = 1; //result=0 failed to delete; result=1 deleted.
            string path1 = ReadSetting("rainFallDatafilePath");
            //if (System.IO.File.Exists(@"C:\2016codes\etc\etc\uploadedFiles\" + fileName))
                if (System.IO.File.Exists(path1+@"\" + fileName))
                {
                // Use a try block to catch IOExceptions, to 
                // handle the case of the file already being 
                // opened by another process. 
                try
                {
                    //System.IO.File.Delete(@"C:\2016codes\etc\etc\uploadedFiles\" + fileName);
                    System.IO.File.Delete( path1+@"\" + fileName);
                    
                    result = 0;
                }
                catch (System.IO.IOException e)
                {
                    //Console.WriteLine(e.Message);
                    result = 1;
                }
            }

            return result;
        }

        public int DeleteEtDataFile(string fileName)
        {
            int result = 1; //result=0 failed to delete; result=1 deleted.
            string path1 = ReadSetting("etDatafilePath");
            //if (System.IO.File.Exists(@"C:\2016codes\etc\etc\uploadedFiles\" + fileName))
            if (System.IO.File.Exists(path1 + @"\" + fileName))
            {
                // Use a try block to catch IOExceptions, to 
                // handle the case of the file already being 
                // opened by another process. 
                try
                {
                    //System.IO.File.Delete(@"C:\2016codes\etc\etc\uploadedFiles\" + fileName);
                    System.IO.File.Delete(path1 + @"\" + fileName);

                    result = 0;
                }
                catch (System.IO.IOException e)
                {
                    //Console.WriteLine(e.Message);
                    result = 1;
                }
            }

            return result;
        }
        public string ReadSetting(string key)
        {
            string result = "";
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                result = appSettings[key] ?? "1";
                //Console.WriteLine(result);
            }
            catch (ConfigurationErrorsException)
            {
                //Console.WriteLine("Error reading app settings");
                result = "";
            }
            return result;

        }
    }
}