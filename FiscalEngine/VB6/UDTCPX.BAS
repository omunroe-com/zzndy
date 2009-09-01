Attribute VB_Name = "UDTCPX"
'record for CPXScreen (Capital Expenditure
  Type CPXType
    cat As String * 3                   'category
    dat As String                       'month/year
    tan As Single                       'tangible percent
    amt As Single                       'gross amount
    WIN As Single                       'working interest percent
    remb As Single                      'reimburse percent
    desc As String * 24                 'description
  End Type
