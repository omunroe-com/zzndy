Attribute VB_Name = "UDTPDT"
        'record type for PDTScreen (Prepaid/Deferred Tax)
  TYPE PDTType
    var AS STRING * 3                   'variable name
    pre AS SINGLE                       'prepaid %
    year AS SINGLE                      'number of prior years
    CUR AS STRING * 3                   'include current year
    def1 AS SINGLE                      'deferred %
    def2 AS SINGLE                      'deferred years
  END TYPE
