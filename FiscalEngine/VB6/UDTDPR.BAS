Attribute VB_Name = "UDTDPR"
        'record type for DPRScreen (Depreciation/Cost Recovery)
  TYPE DPRType
    var AS STRING * 3                   'variable
    Cat AS STRING * 3                   'category
    pre AS STRING * 3                   'pre/post
    tan AS STRING * 3                   'tangible/intangible
    DPR AS SINGLE                       '% depreciated
    mtd AS STRING * 3                   'method
    dbr AS SINGLE                       'db rate
    PRD AS SINGLE                       'period (years)
    all AS SINGLE                       'year 1 allowance
    crd AS SINGLE                       'credit %
    int AS SINGLE                       'interest %
    acc AS STRING * 3                   'accrual method
  END TYPE
