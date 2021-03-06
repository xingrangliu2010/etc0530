/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP 1000 [recordNum]
      ,[ts]
      ,[Hs]
      ,[SHF1_raw_Avg]
      ,[SHF2_raw_Avg]
      ,[SHF_mean_Avg]
      ,[amp_h_f_Tot]
      ,[amp_l_f_Tot]
      ,[chopper_f_Tot]
      ,[detector_f_Tot]
      ,[pll_f_Tot]
      ,[agc_Avg]
      ,[panel_temp_Avg]
      ,[batt_volt_Avg]
      ,[netRadiationCorrected]
      ,[calculatedBR]
      ,[leadjust1]
      ,[leadjust2]
      ,[leadjust3]
  FROM [ETC].[dbo].[etc]

  where ts>='2016-02-07 00:00:00' and ts<='2016-02-10 00:00:00'



  SELECT 
       [ts]
       ,[rainfall]
  FROM [ETC].[dbo].[rainfall]
  where ts>='2016-02-07 00:00:00' and ts<='2016-02-10 00:00:00'



  SELECT 
        a.[ts]
	   ,cast(a.[ts] as date) as theDay
	   ,a.NetTot_Avg
       ,isnull((select  a.NetTot_Avg where  a.NetTot_Avg > 0),0) as  NetTot_Avg1
	   ,a.SHF_mean_Avg
	   ,a.LE_wpl
	   ,a.Hc
	   ,a.Hc/isnull((select a.LE_wpl where  a.LE_wpl <> 0),0.00000001) as br
	   ,(a.NetTot_Avg-a.SHF_mean_Avg)/(1+a.Hc/isnull((select a.LE_wpl where  a.LE_wpl <> 0),0.00000001)) as leadjust1
	   ,(a.NetTot_Avg-a.SHF_mean_Avg)/(1+a.Hc/isnull((select a.LE_wpl where  a.LE_wpl <> 0),0.00000001))/(28.36*24) as leadjust2
	   ,abs((a.NetTot_Avg-a.SHF_mean_Avg)/(1+Hc/isnull((select a.LE_wpl where  a.LE_wpl <> 0),0.00000001))/(28.36*24)) as leadjust3
	   ,b.ts
      ,b.[rainfall]
  FROM  [ETC].[dbo].[etc] a, [ETC].[dbo].[rainfall] b
  where a.ts=b.ts and a.ts>='2016-02-07 00:00:00' and a.ts<='2016-02-10 00:00:00' 
