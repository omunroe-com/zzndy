Attribute VB_Name = "UDTPDC"
'record for PDCScreen (Production Decline Curves)
   TYPE PDCType                         'DIMMED in common as PDC()
     cat AS STRING * 3                  'category
     unit AS STRING * 3                 'time unit (day, mon, yrs)
     mtd AS STRING * 3                  'decline method (exp, har, hyp)
     begprod AS SINGLE                  'beginning production rate
     RATE AS SINGLE                     'escalation/decline rate
     hypexp AS SINGLE                   'hyperbolic exponent
     endprod AS SINGLE                  'ending prod rate
     cumprod AS SINGLE                  'cumulative production rate
     time AS SINGLE                     'time in years
   END TYPE
