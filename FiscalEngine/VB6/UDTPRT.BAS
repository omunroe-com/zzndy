Attribute VB_Name = "UDTPRT"
        'record type for PRTScreen (Government Participation Terms)
  Type PRTType
    Cat As String * 3                   'category
    st1 As String * 3                   'participation start date
    RATE As Single                      'participation rate
    st2 As String * 3                   'repayment start date
    repay As Single                     'repayment %
    PRD As Single                       'period
    int As Single                       'interest %
    acc As String * 3                   'accrual
    dpcrRate As Single                  'depreciation/cost recovery share rate %
  End Type
