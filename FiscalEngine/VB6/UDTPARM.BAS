Attribute VB_Name = "UDTParm"
'record for ANNScreen (Country Annual Forecasts)
   Type ParmType
     Cat As String * 3                  'category
     unit As String * 3                 'unit
     dat As String * 3                  'start date code
     mtd As Integer                     'method (1-0)
     'parm1 As String * 8                'parameter 1
     parm1 As String                    'parameter 1
     parm2 As Single                    'parameter 2
     parm3 As Single                    'parameter 3
     parm4 As Single                    'parameter 4
     parm5 As Single                    'parameter 5
     parm6 As Single                    'parameter 6
   End Type
