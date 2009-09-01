Attribute VB_Name = "UDTGNL"
'record for GNLScreen (Data file General Parms)
   TYPE GNLType                         'DIMMED in common as GNL
     ttl(1 TO 4) AS STRING * 40         'titleline 1-4
     dt(1 TO 4) AS STRING * 5           'dates (proj, discov, prod, discnt)
     pcd(1 TO 2) AS STRING * 3          'price codes (oil and gas)
     wdepth AS SINGLE                   'water depth in meters
     og AS STRING * 3                   '1-OIL 2-GAS
     pval(1 TO 6) AS SINGLE             'discount factors 1-6
     dmtd AS STRING * 3                 'discount method
     eqvl AS SINGLE
   END TYPE
