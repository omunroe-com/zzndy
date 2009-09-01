Attribute VB_Name = "UDTEBL"
'record type for EBLScreen (Expenditure-Based Loan)
  TYPE EBLType
    cat AS STRING * 3                   'capital category
    endt AS STRING * 3                  'finance enddate
    fin AS SINGLE                       'financed portion
    Del AS SINGLE                       'delay
    mtd AS STRING * 3                   'repayment method
    int AS SINGLE                       'interest %
    mth AS SINGLE                       'period in months
    acc AS STRING * 3                   'accrual (yes/no)
  END TYPE
