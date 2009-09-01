Attribute VB_Name = "UDTSAL"
'record type for SALScreen (single Amount Loan)
  TYPE SALType
    dat AS STRING * 5                   'month/year
    amt AS SINGLE                       'loan amount
    Del AS SINGLE                       'repayment delay
    mtd AS STRING * 3                   'repayment method(tot/pri)
    int AS SINGLE                       'interest %
    mth AS SINGLE                       'period in months
    acc AS STRING * 3                   'accrual (yes/no)
  END TYPE
