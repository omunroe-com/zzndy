Attribute VB_Name = "UDTDPL"
        'record type for DPLScreen (Depletion)
  TYPE DPLType
    var AS STRING * 3                   'variable name
    rat1 AS SINGLE                      'rate 1
    prm1 AS STRING * 3                  'parameter 1
    code AS STRING * 3                  'code
    rat2 AS SINGLE                      'rate 2
    prm2 AS STRING * 3                  'parameter 2
    base AS SINGLE                      'recoup base %
    start AS SINGLE                     'recoup start years
    per AS SINGLE                       'recoup period years
  END TYPE
