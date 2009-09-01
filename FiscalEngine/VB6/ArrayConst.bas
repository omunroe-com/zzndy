Attribute VB_Name = "modArrayConst"
' Modifications:
' 23 May 2003 JWD
'  -> Add symbols for new adjustment categories AJ6-AJ0.
'     (C0700)
'  -> Change gc_nAMAXADJ and gc_nASIZE for additional
'     adjustment categories. (C0700)
'  -> Change symbols used with CA() array for additional
'     categories. (C0700)
'
' 5 Jan 2005 JWD
'  -> Add symbols for indexes of accumulator vectors
'     for 3rd party and NOC consolidated cash flow
'     calculation. (C0846)
'  -> Adjust value for gc_nCASIZE for new index values.
'     (C0846)
'
' 10 Feb 2005 JWD
'  -> Add symbol for value of 2nd dimension size of AC().
'     (C0856)
'  -> Add symbols for 2nd dimension index values of AC().
'     (C0856)
'  -> Add symbol for value of 2nd dimension size of CC().
'     (C0856)
'  -> Add symbols for 2nd dimension index values of CC().
'     (C0856)
'
' 11 May 2005 JWD
'  -> Add symbols for new adjustment categories A11-A20.
'     and adjust values of symbols following. (C0876)
'
' 16 May 2005 JWD
'  -> Add symbols for new operating expense categories
'     OX6-O20 and adjust values of symbols following.
'     (C0877)
'---------------------------------------------------------
Option Explicit

Public Const gc_nASIZE As Integer = 66                   ' was 51 (JWD-C0877) was 41 (JWD-C0876) was 36 (JWD-C0700)
Public Const gc_nAPRICEOFFSET As Integer = 14

Public Const gc_nAMINVOL As Integer = 1
Public Const gc_nAMAXVOL As Integer = 12

Public Const gc_nAMINPRC As Integer = 15
Public Const gc_nAMAXPRC As Integer = 26

Public Const gc_nAMINOPX As Integer = 27
Public Const gc_nAMAXOPX As Integer = gc_nAMINOPX + 19      ' was 31 (JWD-C0877)

Public Const gc_nAMINADJ As Integer = gc_nAMAXOPX + 1       ' was 32 (JWD-C0877)
Public Const gc_nAMAXADJ As Integer = gc_nAMINADJ + 19      ' was 41 (JWD-C0876) was 36 (JWD-C0700)

Public Const gc_nCASIZE As Integer = gc_nAMAXADJ + 44       ' was 95 (JWD-C0877) was 85 (JWD-C0876) was 70 (JWD C0846) ' was 65 (JWD-C0700)
Public Const gc_nCAREVENUEOFFSET As Integer = 14
Public Const gc_nCAGROSSOFFSETVOLUME As Integer = 71        ' was 56 (JWD-C0877) was 46 (JWD-C0876) was 41 (JWD-C0700)
Public Const gc_nCAGROSSOFFSETREVENUE As Integer = 69       ' was 54 (JWD-C0877) was 44 (JWD-C0876) was 39 (JWD-C0700)




' CA and A Array Constants

' Volume streams
Public Const gc_nAOIL As Integer = 1
Public Const gc_nAGAS As Integer = 2
Public Const gc_nAOV1 As Integer = 3
Public Const gc_nAOV2 As Integer = 4
Public Const gc_nAOV3 As Integer = 5
Public Const gc_nAOV4 As Integer = 6
Public Const gc_nAOV5 As Integer = 7
Public Const gc_nAOV6 As Integer = 8
Public Const gc_nAOV7 As Integer = 9
Public Const gc_nAOV8 As Integer = 10
Public Const gc_nAOV9 As Integer = 11
Public Const gc_nAOV0 As Integer = 12
'Reserves Additions and Working Interest
Public Const gc_nARES As Integer = 13
Public Const gc_nAWIN As Integer = 14
' Prices
Public Const gc_nAOPC As Integer = 15
Public Const gc_nAGPC As Integer = 16
Public Const gc_nAOP1 As Integer = 17
Public Const gc_nAOP2 As Integer = 18
Public Const gc_nAOP3 As Integer = 19
Public Const gc_nAOP4 As Integer = 20
Public Const gc_nAOP5 As Integer = 21
Public Const gc_nAOP6 As Integer = 22
Public Const gc_nAOP7 As Integer = 23
Public Const gc_nAOP8 As Integer = 24
Public Const gc_nAOP9 As Integer = 25
Public Const gc_nAOP0 As Integer = 26
' Opex
Public Const gc_nAOX1 As Integer = 27
Public Const gc_nAOX2 As Integer = 28
Public Const gc_nAOX3 As Integer = 29
Public Const gc_nAOX4 As Integer = 30
Public Const gc_nAOX5 As Integer = 31
' 13 May 2005 JWD (C0877) Add new opex categories
Public Const gc_nAOX6 As Integer = 32
Public Const gc_nAOX7 As Integer = 33
Public Const gc_nAOX8 As Integer = 34
Public Const gc_nAOX9 As Integer = 35
Public Const gc_nAOX0 As Integer = 36
Public Const gc_nAO11 As Integer = 37
Public Const gc_nAO12 As Integer = 38
Public Const gc_nAO13 As Integer = 39
Public Const gc_nAO14 As Integer = 40
Public Const gc_nAO15 As Integer = 41
Public Const gc_nAO16 As Integer = 42
Public Const gc_nAO17 As Integer = 43
Public Const gc_nAO18 As Integer = 44
Public Const gc_nAO19 As Integer = 45
Public Const gc_nAO20 As Integer = 46
' End (C0877)

' Adjustment
Public Const gc_nAAJ1 As Integer = 47
Public Const gc_nAAJ2 As Integer = 48
Public Const gc_nAAJ3 As Integer = 49
Public Const gc_nAAJ4 As Integer = 50
Public Const gc_nAAJ5 As Integer = 51
' 23 May 2003 JWD (C0700) New categories
Public Const gc_nAAJ6 As Integer = 52
Public Const gc_nAAJ7 As Integer = 53
Public Const gc_nAAJ8 As Integer = 54
Public Const gc_nAAJ9 As Integer = 55
Public Const gc_nAAJ0 As Integer = 56
' 11 May 2005 JWD (C0876) New categories
Public Const gc_nAA11 As Integer = 57
Public Const gc_nAA12 As Integer = 58
Public Const gc_nAA13 As Integer = 59
Public Const gc_nAA14 As Integer = 60
Public Const gc_nAA15 As Integer = 61
Public Const gc_nAA16 As Integer = 62
Public Const gc_nAA17 As Integer = 63
Public Const gc_nAA18 As Integer = 64
Public Const gc_nAA19 As Integer = 65
Public Const gc_nAA20 As Integer = 66

' CA Array only

Public Const gc_nAINF1 As Integer = 67                ' was 52 (JWD-C0877) was 42 (JWD-C0876) was 37 (JWD-C0700)
Public Const gc_nAINF2 As Integer = 68                ' was 43 (JWD-C0876) was 38 (JWD-C0700)
Public Const gc_nADFL As Integer = 69                 ' was 44 (JWD-C0876) was 39 (JWD-C0700)
Public Const gc_nAREPAY As Integer = 70               ' was 45 (JWD-C0876) was 40 (JWD-C0700)
Public Const gc_nAFIN As Integer = 71                 ' was 46 (JWD-C0876) was 41 (JWD-C0700)
'Gross Volumes
Public Const gc_nAGOIL As Integer = 72                ' was 47 (JWD-C0876) was 42, etc. (JWD-C0700)
Public Const gc_nAGGAS As Integer = 73
Public Const gc_nAGOV1 As Integer = 74
Public Const gc_nAGOV2 As Integer = 75
Public Const gc_nAGOV3 As Integer = 76
Public Const gc_nAGOV4 As Integer = 77
Public Const gc_nAGOV5 As Integer = 78
Public Const gc_nAGOV6 As Integer = 79
Public Const gc_nAGOV7 As Integer = 80
Public Const gc_nAGOV8 As Integer = 81
Public Const gc_nAGOV9 As Integer = 82
Public Const gc_nAGOV0 As Integer = 83
'Gross Revenues
Public Const gc_nAGOILREV As Integer = 84
Public Const gc_nAGGASREV As Integer = 85
Public Const gc_nAGOV1REV As Integer = 86
Public Const gc_nAGOV2REV As Integer = 87
Public Const gc_nAGOV3REV As Integer = 88
Public Const gc_nAGOV4REV As Integer = 89
Public Const gc_nAGOV5REV As Integer = 90
Public Const gc_nAGOV6REV As Integer = 91
Public Const gc_nAGOV7REV As Integer = 92
Public Const gc_nAGOV8REV As Integer = 93
Public Const gc_nAGOV9REV As Integer = 94
Public Const gc_nAGOV0REV As Integer = 95

' 5 Jan 2005 JWD (C0846) New indexes for accumulating for 3rd party and NOC
Public Const gc_nCA_GRSREV As Integer = 96      ' index to gross revenues
Public Const gc_nCA_CMPREV As Integer = 97      ' index to company revenues
Public Const gc_nCA_3DPREV As Integer = 98      ' index to 3rd party revenues
Public Const gc_nCA_NOCREV As Integer = 99      ' index to NOC revenues
Public Const gc_nCA_GRSOPX As Integer = 100     ' index to grossed operating expense
Public Const gc_nCA_CMPOPX As Integer = 101     ' index to company opex
Public Const gc_nCA_3DPOPX As Integer = 102     ' index to 3rd party opex
Public Const gc_nCA_NOCOPX As Integer = 103     ' index to NOC opex
Public Const gc_nCA_GRSFIN As Integer = 104     ' index to grossed-up financing
Public Const gc_nCA_CMPFIN As Integer = 105     ' index to company financing
Public Const gc_nCA_3DPFIN As Integer = 106     ' index to 3rd party financing
Public Const gc_nCA_NOCFIN As Integer = 107     ' index to NOC financing
Public Const gc_nCA_GRSPMT As Integer = 108     ' index to grossed-up NOC repayment
Public Const gc_nCA_CMPPMT As Integer = 109     ' index to company NOC repayment
Public Const gc_nCA_CMPBUR As Integer = 110     ' index to company's partner reimbursement
' End (C0846)

' 10 Feb 2005 JWD (C0856)
' New symbols for existing AC() consolidation array index 2 positions
Public Const gc_nAC_CMPPOS As Integer = 1       ' index to company positive cash flows
Public Const gc_nAC_CMPNEG As Integer = 2       ' index to company negative cash flows
Public Const gc_nAC_CMPOPX As Integer = 3       ' index to company opex
Public Const gc_nAC_CMPCPX As Integer = 4       ' index to company capex
Public Const gc_nAC_EQVPRD As Integer = 5       ' index to gross equivalent production
Public Const gc_nAC_EQVREV As Integer = 6       ' index to gross revenues
Public Const gc_nAC_CURDFL As Integer = 7       ' index to deflator
Public Const gc_nAC_GOVTCF As Integer = 8       ' index to government cash flow
Public Const gc_nAC_CMPNGT As Integer = 9       ' index to company "t" cash flows
Public Const gc_nAC_CMPNGA As Integer = 10      ' index to company "a" cash flows
Public Const gc_nAC_CMPNGU As Integer = 11      ' index to company "u" cash flows

' Symbols for new AC() array index 2 positions
Public Const gc_nAC_CMPNCF As Integer = 12      ' index to company net cash flow (for 3d party, NOC calcs)
Public Const gc_nAC_3DPNCF As Integer = 13      ' index to 3rd party net cash flow
Public Const gc_nAC_NOCNCF As Integer = 14      ' index to NOC net cash flow
Public Const gc_nAC_GRSREV As Integer = 15      ' index to gross sales revenues

' 18 Jun 2008 JWD
Public Const gc_nAC_TPS As Integer = 16         ' index to gross-level positive cash flow
Public Const gc_nAC_TNG As Integer = 17         ' index to gross-level negative cash flow
Public Const gc_nAC_OPS As Integer = 18         ' index to operating-level positive cash flow
Public Const gc_nAC_ONG As Integer = 19         ' index to operating-level negative cash flow
Public Const gc_nAC_GPS As Integer = 20         ' index to group-level positive cash flow
Public Const gc_nAC_GNG As Integer = 21         ' index to group-level negative cash flow
Public Const gc_nAC_CPS As Integer = 22         ' index to company-level positive cash flow
Public Const gc_nAC_CNG As Integer = 23         ' index to company-level negative cash flow

Public Const gc_nACSIZED2 = 23                  ' dimension 2 upper bound


' New symbols for existing CC() consolidation array index 2 positions
Public Const gc_nCC_CAT As Integer = 1          ' capital expenditure category
Public Const gc_nCC_XMO As Integer = 2          ' month of capital expenditure
Public Const gc_nCC_XYR As Integer = 3          ' year of capital expenditure
Public Const gc_nCC_AMT As Integer = 4          ' company capital expenditure amount

' Symbols for new CC() array index 2 positions
Public Const gc_nCC_CMP As Integer = 5          ' company net capital expenditure amount (for 3d party & noc calc)
Public Const gc_nCC_3DP As Integer = 6          ' 3rd party net capital expenditure amount
Public Const gc_nCC_NOC As Integer = 7          ' NOC net capital expenditure amount

Public Const gc_nCC_TCX As Integer = 8          ' index to gross-level capital expenditure
Public Const gc_nCC_OCX As Integer = 9          ' index to operating-level capital expenditure
Public Const gc_nCC_GCX As Integer = 10          ' index to group-level capital expenditure
Public Const gc_nCC_CCX As Integer = 11          ' index to company-level capital expenditure

Public Const gc_nCCSIZED2 = 11                   ' dimension 2 upper bound
' End (C0856)

Public Function ATotalRevenues(ByVal nYr As Integer) As Single
    ATotalRevenues = (A(nYr, gc_nAOIL) * A(nYr, gc_nAOPC)) + (A(nYr, gc_nAGAS) * A(nYr, gc_nAGPC)) _
                      + (A(nYr, gc_nAOV1) * A(nYr, gc_nAOP1)) + (A(nYr, gc_nAOV2) * A(nYr, gc_nAOP2)) _
                      + (A(nYr, gc_nAOV3) * A(nYr, gc_nAOP3)) + (A(nYr, gc_nAOV4) * A(nYr, gc_nAOP4)) _
                      + (A(nYr, gc_nAOV5) * A(nYr, gc_nAOP5)) + (A(nYr, gc_nAOV6) * A(nYr, gc_nAOP6)) _
                      + (A(nYr, gc_nAOV7) * A(nYr, gc_nAOP7)) + (A(nYr, gc_nAOV8) * A(nYr, gc_nAOP8)) _
                      + (A(nYr, gc_nAOV9) * A(nYr, gc_nAOP9)) + (A(nYr, gc_nAOV0) * A(nYr, gc_nAOP0))

End Function

Public Function TotalGrossRevenue(ByVal nYr As Integer) As Single
    TotalGrossRevenue = GrossRevenue(nYr, gc_nAOIL) + GrossRevenue(nYr, gc_nAGAS) _
                      + GrossRevenue(nYr, gc_nAOV1) + GrossRevenue(nYr, gc_nAOV2) _
                      + GrossRevenue(nYr, gc_nAOV3) + GrossRevenue(nYr, gc_nAOV4) _
                      + GrossRevenue(nYr, gc_nAOV5) + GrossRevenue(nYr, gc_nAOV6) _
                      + GrossRevenue(nYr, gc_nAOV7) + GrossRevenue(nYr, gc_nAOV8) _
                      + GrossRevenue(nYr, gc_nAOV9) + GrossRevenue(nYr, gc_nAOV0)

End Function
