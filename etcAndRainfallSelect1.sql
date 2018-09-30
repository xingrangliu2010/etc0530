  SELECT 
        a.[ts] as theDateTime
	   ,cast(a.[ts] as date) as theDay
	   ,a.NetTot_Avg as Net_Radiation
       ,isnull((select  a.NetTot_Avg where  a.NetTot_Avg > 0),0) as  Net_Radiation_Corrected
	   ,a.SHF_mean_Avg as Soil_Heat_Flux 
	   ,a.LE_wpl as Latenet_Heat_Flux
	   ,a.Hc as Sesible_heat_flux
	   ,a.Hc/isnull((select a.LE_wpl where  a.LE_wpl <> 0),0.00000001) as BR
	   ,(a.NetTot_Avg-a.SHF_mean_Avg)/(1+a.Hc/isnull((select a.LE_wpl where  a.LE_wpl <> 0),0.00000001)) as leadjust1
	   ,(a.NetTot_Avg-a.SHF_mean_Avg)/(1+a.Hc/isnull((select a.LE_wpl where  a.LE_wpl <> 0),0.00000001))/(28.36*24) as leadjust2
	   ,abs((a.NetTot_Avg-a.SHF_mean_Avg)/(1+Hc/isnull((select a.LE_wpl where  a.LE_wpl <> 0),0.00000001))/(28.36*24)) as leadjust3
	   ,b.ts
      ,b.[rainfall]
  FROM  [ETC].[dbo].[etc] a, [ETC].[dbo].[rainfall] b
  where a.ts=b.ts and a.ts>='2016-02-07 00:00:00' and a.ts<='2016-02-10 00:00:00' 