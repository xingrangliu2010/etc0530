
SELECT recordNum
       ,[ts]
	   ,day([ts]) as theDay
	   ,cast([ts] as date) as theDay
	   ,NetTot_Avg
       ,isnull((select  NetTot_Avg where  NetTot_Avg > 0),0) as  NetTot_Avg1
	   ,SHF_mean_Avg
	   ,LE_wpl
	   ,Hc
	   ,Hc/isnull((select LE_wpl where  LE_wpl <> 0),0.00000001) as br
	   ,(NetTot_Avg-SHF_mean_Avg)/(1+Hc/isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) as leadjust1
	   ,(NetTot_Avg-SHF_mean_Avg)/(1+Hc/isnull((select LE_wpl where  LE_wpl <> 0),0.00000001))/(28.36*24) as leadjust2
	   ,abs((NetTot_Avg-SHF_mean_Avg)/(1+Hc/isnull((select LE_wpl where  LE_wpl <> 0),0.00000001))/(28.36*24)) as leadjust3
  FROM [ETC].[dbo].[etc]

  select sum(Hc) FROM [ETC].[dbo].[etc]

  select isnull((select (((LE_wpl-SHF_mean_Avg)/(isnull((select (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) where  (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) <> 0),0.00000001)))/(28.36*24))*-1 where  (((LE_wpl-SHF_mean_Avg)/(isnull((select (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) where  (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) <> 0),0.00000001)))/(28.36*24)) <0),((LE_wpl-SHF_mean_Avg)/(isnull((select (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) where  (isnull((select LE_wpl where  LE_wpl <> 0),0.00000001)) <> 0),0.00000001)))/(28.36*24)) as dailyLeadjust3
  FROM [ETC].[dbo].[etc]  

 select sum(Leadjust3) as dailyData from
 (  select abs((NetTot_Avg-SHF_mean_Avg)/(1+Hc/isnull((select LE_wpl where  LE_wpl <> 0),0.00000001))/(28.36*24)) as leadjust3
  FROM [ETC].[dbo].[etc] 
	 ) t1
