Attribute VB_Name = "UDTPRC"
        'record type for PRCScreen (Price Definition)
  TYPE PRCType
    var AS STRING * 3                   'variable name
    prc1 AS STRING * 3                  'price 1
    code AS STRING * 3                  'code
    prc2 AS STRING * 3                  'price 2
    base AS SINGLE                      'base %
    inc AS SINGLE                       'increment
  END TYPE
