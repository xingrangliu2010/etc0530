using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Text;
using System.Data;
using WebClassLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebClassLibrary
{
  public  class externalWSdata
    {
    public static int gettingWSdata(string exteranlWSname,string valueOfYesterday)
        {
            int result = 0;// 0:correct 1: errors

            //converting today to the required date format

            DateTime today = DateTime.Today;
            string stringTodayYear = Convert.ToString(today.Year);
            string stringTodayMonth = "";
            if (today.Month < 10)
            {
                stringTodayMonth = "0" + Convert.ToString(today.Month);
            }
            else
            {
                stringTodayMonth = Convert.ToString(today.Month);
            }
            string stringTodayDay = "";
            if (today.Day < 10)
            {
                stringTodayDay = "0" + Convert.ToString(today.Day);
            }
            {
                stringTodayDay = Convert.ToString(today.Day);
            }

            string stringToday = stringTodayYear + "-" + stringTodayMonth + "-" + stringTodayDay;

            DateTime yesterday = today.AddDays(-1);

            //using farmId to get external WS name to get WS ID
            //string NameAndId = "Cambrai_RMPW31";
            string NameAndId = exteranlWSname;
            int theStart = NameAndId.LastIndexOf('_');
            int theLength = NameAndId.Length;
            string wsId = NameAndId.Substring(theStart + 1, theLength - (theStart + 1));

            DateTime oneMonthAgo = today.AddMonths(-1);

            string stringAgoYear = Convert.ToString(oneMonthAgo.Year);
            string stringAgoMonth = "";
            string stringAgoDay = "";
            if (oneMonthAgo.Month < 10)
            {
                stringAgoMonth = "0" + Convert.ToString(oneMonthAgo.Month);

            }
            else
            {
                stringAgoMonth = Convert.ToString(oneMonthAgo.Month);
            }
            if (oneMonthAgo.Day < 10)
            {
                stringAgoDay = "0" + Convert.ToString(oneMonthAgo.Day);
            }
            else
            {
                stringAgoDay = Convert.ToString(oneMonthAgo.Day);
            }

            string stringOneMonthAgo = stringAgoYear + "-" + stringAgoMonth + "-" + stringAgoDay;

            //from date value to get the last day of a month
            //string yesterday = "2016-03-08"; //we don't need to get and store external ws data using the way as bom used.

            DateTime theYesterday = Convert.ToDateTime(yesterday);




            DateTime firstDayOfMonth = new DateTime(theYesterday.Year, theYesterday.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);


            int theYear = theYesterday.Year;
            int theMonth = theYesterday.Month;
            int startDay = 1;
            int endDay = lastDayOfMonth.Day;

            string stringMonth = "01";

            if (theMonth > 9)
            {
                stringMonth = Convert.ToString(theMonth);
            }
            else
            {
                stringMonth = "0" + Convert.ToString(theMonth);
            }

            string stringYear = Convert.ToString(theYear);

            string stringStartDay = stringYear + "-" + stringMonth + "-" + "01";
            string stringEndDay = stringYear + "-" + stringMonth + "-" + Convert.ToString(endDay);


            //for external ws data, we automatically put the latest one month data at the page started first.
            stringStartDay = stringOneMonthAgo;



            //////string filePath=@"http://static.social.ndtv.com/files/blanket-jk-count.json";
            //////WebProxy myProxy = new WebProxy();
            ////////var webRequest = WebRequest.Create(filePath);

            //////Uri newUri = new Uri("http://proxy.usq.edu.au:8080");//downloader.Proxy = new WebProxy("139.86.9.80", 8080);
            //////myProxy.Address = newUri;
            ////////webRequest.Proxy = myProxy;
            //////WebClient webClient=new WebClient();
            //////webClient.Proxy = myProxy;

            //////string json=webClient.DownloadString(filePath);

            ////////string json1 = new System.Net.WebClient().DownloadString(filePath);
            //////string count = json.Split(':')[1].Replace("}", "");
            ////////Update code here

            //string filePath = @"http://static.social.ndtv.com/files/blanket-jk-count.json";
            //string filePath = @"http://aws.naturalresources.sa.gov.au/api/data/?timestep=daily&station_ids=RMPW14&start_date=2015-11-01&end_date=2015-11-30&parameters=stamp,rain_total,et_asce_s";

            //path for stations modified on 20160422   
            //string filePath = @"http://aws.naturalresources.sa.gov.au/api/station/";

            //path for weather data modified on 20160426   
            //data each minute
            //http://aws.naturalresources.sa.gov.au/api/data/?timestep=minutes&station_ids=RMPW12&start_date=2015-01-01&end_date=2015-01-01
            //string filePath = @"http://aws.naturalresources.sa.gov.au/api/data/?timestep=minutes&station_ids=RMPW12&start_date=2015-01-01&end_date=2015-01-05";

            //data each data 
            //http://aws.naturalresources.sa.gov.au/api/data/?timestep=daily&station_ids=RMPW12&start_date=2015-01-01&end_date=2015-01-15
            //string filePath = @"http://aws.naturalresources.sa.gov.au/api/data/?timestep=daily&station_ids=RMPW12&start_date=2015-01-01&end_date=2015-01-15";


            //string filePath = @"http://aws.naturalresources.sa.gov.au/api/data/?timestep=daily&station_ids=" + wsId + "&start_date=" + stringStartDay + "&end_date=" + stringEndDay;
            string filePath = @"http://aws.naturalresources.sa.gov.au/api/data/?timestep=daily&station_ids=" + wsId + "&start_date=" + stringStartDay + "&end_date=" + stringToday;



            TableInsert tbinsert = new TableInsert();
            TableFk tb = new TableFk();    //To prepare siteId field

            string canBeReadstring = "";
            string strContent = "";

            WebProxy myProxy = new WebProxy();


            var webRequest = WebRequest.Create(filePath);

            Uri newUri = new Uri("http://proxy.usq.edu.au:8080");//downloader.Proxy = new WebProxy("139.86.9.80", 8080);
            myProxy.Address = newUri;
            webRequest.Proxy = myProxy;

            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                if (response != null)
                {
                    canBeReadstring = "Ok, it can be read.";

                    //var strContent = reader.ReadToEnd();
                    strContent = reader.ReadToEnd();

                    //stations[] theStations = JsonConvert.DeserializeObject<stations[]>(strContent);

                    //Json arry can not be converted to arry in C#
                    //weatherData weatherData = JsonConvert.DeserializeObject<weatherData>(strContent);
                    //var theWeatherData  = JsonConvert.DeserializeObject<weatherData>(strContent);

                    string[] theSeparator1 = { "#" };
                    string[] theSeparator2 = { "," };

                    string processedString1 = strContent.Replace("parameters\": [", "#");
                    string processedString2 = processedString1.Replace("]}, \"data\": [", "#");
                    string processedString3 = processedString2.Replace("]}", "#");

                    //processing header
                    string[] processedLines = processedString3.Split(theSeparator1, StringSplitOptions.None);

                    string[] headers = processedLines[1].Split(theSeparator2, StringSplitOptions.None);

                    //processing data
                    // removing "],"

                    string processedData = processedLines[2].Replace("],", "#");

                    string[] dataGroups = processedData.Split(theSeparator1, StringSplitOptions.None);

                    //removing "["
                    for (int i = 0; i < dataGroups.Length; i++)
                    {
                        dataGroups[i] = dataGroups[i].Replace("[", "");

                    }



                    string[,] dataMatrix = new string[dataGroups.Length, headers.Length];
                    //processing data in each dataGroup
                    for (int i = 0; i < dataGroups.Length; i++)
                    {
                        string[] rowData = dataGroups[i].Split(theSeparator2, StringSplitOptions.None);

                        for (int j = 0; j < rowData.Length; j++)
                        {
                            dataMatrix[i, j] = rowData[j];
                        }

                    }


                    int test = 0;

                    //to insert the new WS data to the database, for each recod to be insert, first check if the recoder has been added


                    TableSelect dataselect = new TableSelect();

                    string exteranlWSflag = "external";

                    DataTable theWSid = dataselect.DoSelect("WeatherStation", "dbo", "SiteID", "Name='" + NameAndId + "' and State = '" + exteranlWSflag + "'", " SiteId desc");
                    if ((theWSid != null) || (theWSid.Rows.Count > 0))
                    {

                        string theWeatherStionId = Convert.ToString(theWSid.Rows[0]["SiteID"]);
                        DataTable checkDt = dataselect.DoSelect("GDCRainfall", "dbo", "WeatherStation", "WeatherStation=" + theWeatherStionId + " and Date='" + stringToday + "'", " WeatherStation desc");

                        if ((checkDt == null) || (checkDt.Rows.Count == 0))
                        {

                            string theComment1 = "No record found, the latest one month data should be inserted.";

                            //string tempUser = checkDt.Rows[0]["username"].ToString();

                            int the0 = dataMatrix.GetLength(0);//29
                            int the1 = dataMatrix.GetLength(1);//55
                            string rainData = "0";
                            string etData = "0";
                            string theDate = "";
                            int operationResultStat = 1; // 1: failure; 0:successful
                            string tableName = "GDCRainfall";
                            string userName = "[gdc].[dbo]";
                            string tbfileds = "WeatherStation,Date,FAO56,Rainfall";

                            for (int i = 0; i < dataMatrix.GetLength(0); i++)
                            {
                                theDate = dataMatrix[i, 1];
                                theDate = theDate.Replace("\"", "").Trim();
                                rainData = dataMatrix[i, 25];
                                etData = dataMatrix[i, 51];

                                //check if the record has been added
                                DataTable checkEachRecod = dataselect.DoSelect("GDCRainfall", "dbo", "WeatherStation", "WeatherStation=" + theWeatherStionId + " and Date='" + theDate + "'", " WeatherStation desc");



                                if ((checkEachRecod == null) || (checkEachRecod.Rows.Count == 0))
                                {
                                    string theResult = " NO this record found in the database.";
                                    //if it has not been added to insert it
                                    string toBeInsertWSid = theWeatherStionId;
                                    string recordValues = theWeatherStionId + ",'" + theDate + "'," + etData + "," + rainData;
                                    operationResultStat = tbinsert.Insert(tableName, userName, tbfileds, recordValues);


                                }
                                else
                                {
                                    string theComment2 = " The record has been in database.";
                                }

                            }//for


                        }
                        else
                        {
                            string theComment2 = "Don't need to insert data.";

                        }// if the latest one month data is needed to be inserted.
                    }  //if the SiteId (the external WS) is in the database 
                    else
                    {
                        string theComment4 = "The exteral WS does not exist in the database.";
                    }

                    // Insert external weather station into database
                    //insert data
                    ////int operationResultStat = 1; // 1: failure; 0:successful
                    ////string tableName = "WeatherStation";
                    ////string userName = "[gdc].[dbo]";
                    ////string tbfileds = "SiteID,Name,Lat,Lon,StartYear,EndYear,State";

                    //////////for (int i = 0; i < theStations.Length; i++)
                    //////////{
                    //////////    if(theStations[i].status=="on")
                    //////////    {
                    //////////        Int32 theId = tb.GetFk("dbo", "WeatherStation", "SiteID");
                    //////////        string wsId = Convert.ToString(theId);
                    //////////        string wsName = theStations[i].name+"_"+theStations[i].aws_id;
                    //////////        float lat = theStations[i].lat;
                    //////////        float lon = theStations[i].lon;
                    //////////        string start = "Jan 2000";
                    //////////        string end = "Apr 2016";
                    //////////        string theState = "external";
                    //////////        string recordValues = wsId + ",'" + wsName.Trim() + "'," + lat + "," + lon + ",'" + start + "','" + end + "','" + theState.ToLower() + "'";
                    //////////        operationResultStat = tbinsert.Insert(tableName, userName, tbfileds, recordValues);
                    //////////        if (operationResultStat == 0)
                    //////////        {
                    //////////        //feedback inserting is successful
                    //////////        }
                    //////////        else
                    //////////        {
                    //////////            //error message 
                    //////////        }
                    //////////    }
                    //////////}

                }//if the external WS data file is found
                else
                {
                    if (response != null)
                    {
                        response.Close();
                    }
                    canBeReadstring = "It doesn't exist.";
                }
            }
            return result;

        }
    }
}
