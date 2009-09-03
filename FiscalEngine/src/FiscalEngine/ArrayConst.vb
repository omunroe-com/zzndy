Option Strict Off
Option Explicit On
Module modArrayConst
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
	
	Public Const gc_nASIZE As Short = 66 ' was 51 (JWD-C0877) was 41 (JWD-C0876) was 36 (JWD-C0700)
	Public Const gc_nAPRICEOFFSET As Short = 14
	
	Public Const gc_nAMINVOL As Short = 1
	Public Const gc_nAMAXVOL As Short = 12
	
	Public Const gc_nAMINPRC As Short = 15
	Public Const gc_nAMAXPRC As Short = 26
	
	Public Const gc_nAMINOPX As Short = 27
	Public Const gc_nAMAXOPX As Short = gc_nAMINOPX + 19 ' was 31 (JWD-C0877)
	
	Public Const gc_nAMINADJ As Short = gc_nAMAXOPX + 1 ' was 32 (JWD-C0877)
	Public Const gc_nAMAXADJ As Short = gc_nAMINADJ + 19 ' was 41 (JWD-C0876) was 36 (JWD-C0700)
	
	Public Const gc_nCASIZE As Short = gc_nAMAXADJ + 44 ' was 95 (JWD-C0877) was 85 (JWD-C0876) was 70 (JWD C0846) ' was 65 (JWD-C0700)
	Public Const gc_nCAREVENUEOFFSET As Short = 14
	Public Const gc_nCAGROSSOFFSETVOLUME As Short = 71 ' was 56 (JWD-C0877) was 46 (JWD-C0876) was 41 (JWD-C0700)
	Public Const gc_nCAGROSSOFFSETREVENUE As Short = 69 ' was 54 (JWD-C0877) was 44 (JWD-C0876) was 39 (JWD-C0700)
	
	
	
	
	' CA and A Array Constants
	
	' Volume streams
	Public Const gc_nAOIL As Short = 1
	Public Const gc_nAGAS As Short = 2
	Public Const gc_nAOV1 As Short = 3
	Public Const gc_nAOV2 As Short = 4
	Public Const gc_nAOV3 As Short = 5
	Public Const gc_nAOV4 As Short = 6
	Public Const gc_nAOV5 As Short = 7
	Public Const gc_nAOV6 As Short = 8
	Public Const gc_nAOV7 As Short = 9
	Public Const gc_nAOV8 As Short = 10
	Public Const gc_nAOV9 As Short = 11
	Public Const gc_nAOV0 As Short = 12
	'Reserves Additions and Working Interest
	Public Const gc_nARES As Short = 13
	Public Const gc_nAWIN As Short = 14
	' Prices
	Public Const gc_nAOPC As Short = 15
	Public Const gc_nAGPC As Short = 16
	Public Const gc_nAOP1 As Short = 17
	Public Const gc_nAOP2 As Short = 18
	Public Const gc_nAOP3 As Short = 19
	Public Const gc_nAOP4 As Short = 20
	Public Const gc_nAOP5 As Short = 21
	Public Const gc_nAOP6 As Short = 22
	Public Const gc_nAOP7 As Short = 23
	Public Const gc_nAOP8 As Short = 24
	Public Const gc_nAOP9 As Short = 25
	Public Const gc_nAOP0 As Short = 26
	' Opex
	Public Const gc_nAOX1 As Short = 27
	Public Const gc_nAOX2 As Short = 28
	Public Const gc_nAOX3 As Short = 29
	Public Const gc_nAOX4 As Short = 30
	Public Const gc_nAOX5 As Short = 31
	' 13 May 2005 JWD (C0877) Add new opex categories
	Public Const gc_nAOX6 As Short = 32
	Public Const gc_nAOX7 As Short = 33
	Public Const gc_nAOX8 As Short = 34
	Public Const gc_nAOX9 As Short = 35
	Public Const gc_nAOX0 As Short = 36
	Public Const gc_nAO11 As Short = 37
	Public Const gc_nAO12 As Short = 38
	Public Const gc_nAO13 As Short = 39
	Public Const gc_nAO14 As Short = 40
	Public Const gc_nAO15 As Short = 41
	Public Const gc_nAO16 As Short = 42
	Public Const gc_nAO17 As Short = 43
	Public Const gc_nAO18 As Short = 44
	Public Const gc_nAO19 As Short = 45
	Public Const gc_nAO20 As Short = 46
	' End (C0877)
	
	' Adjustment
	Public Const gc_nAAJ1 As Short = 47
	Public Const gc_nAAJ2 As Short = 48
	Public Const gc_nAAJ3 As Short = 49
	Public Const gc_nAAJ4 As Short = 50
	Public Const gc_nAAJ5 As Short = 51
	' 23 May 2003 JWD (C0700) New categories
	Public Const gc_nAAJ6 As Short = 52
	Public Const gc_nAAJ7 As Short = 53
	Public Const gc_nAAJ8 As Short = 54
	Public Const gc_nAAJ9 As Short = 55
	Public Const gc_nAAJ0 As Short = 56
	' 11 May 2005 JWD (C0876) New categories
	Public Const gc_nAA11 As Short = 57
	Public Const gc_nAA12 As Short = 58
	Public Const gc_nAA13 As Short = 59
	Public Const gc_nAA14 As Short = 60
	Public Const gc_nAA15 As Short = 61
	Public Const gc_nAA16 As Short = 62
	Public Const gc_nAA17 As Short = 63
	Public Const gc_nAA18 As Short = 64
	Public Const gc_nAA19 As Short = 65
	Public Const gc_nAA20 As Short = 66
	
	' CA Array only
	
	Public Const gc_nAINF1 As Short = 67 ' was 52 (JWD-C0877) was 42 (JWD-C0876) was 37 (JWD-C0700)
	Public Const gc_nAINF2 As Short = 68 ' was 43 (JWD-C0876) was 38 (JWD-C0700)
	Public Const gc_nADFL As Short = 69 ' was 44 (JWD-C0876) was 39 (JWD-C0700)
	Public Const gc_nAREPAY As Short = 70 ' was 45 (JWD-C0876) was 40 (JWD-C0700)
	Public Const gc_nAFIN As Short = 71 ' was 46 (JWD-C0876) was 41 (JWD-C0700)
	'Gross Volumes
	Public Const gc_nAGOIL As Short = 72 ' was 47 (JWD-C0876) was 42, etc. (JWD-C0700)
	Public Const gc_nAGGAS As Short = 73
	Public Const gc_nAGOV1 As Short = 74
	Public Const gc_nAGOV2 As Short = 75
	Public Const gc_nAGOV3 As Short = 76
	Public Const gc_nAGOV4 As Short = 77
	Public Const gc_nAGOV5 As Short = 78
	Public Const gc_nAGOV6 As Short = 79
	Public Const gc_nAGOV7 As Short = 80
	Public Const gc_nAGOV8 As Short = 81
	Public Const gc_nAGOV9 As Short = 82
	Public Const gc_nAGOV0 As Short = 83
	'Gross Revenues
	Public Const gc_nAGOILREV As Short = 84
	Public Const gc_nAGGASREV As Short = 85
	Public Const gc_nAGOV1REV As Short = 86
	Public Const gc_nAGOV2REV As Short = 87
	Public Const gc_nAGOV3REV As Short = 88
	Public Const gc_nAGOV4REV As Short = 89
	Public Const gc_nAGOV5REV As Short = 90
	Public Const gc_nAGOV6REV As Short = 91
	Public Const gc_nAGOV7REV As Short = 92
	Public Const gc_nAGOV8REV As Short = 93
	Public Const gc_nAGOV9REV As Short = 94
	Public Const gc_nAGOV0REV As Short = 95
	
	' 5 Jan 2005 JWD (C0846) New indexes for accumulating for 3rd party and NOC
	Public Const gc_nCA_GRSREV As Short = 96 ' index to gross revenues
	Public Const gc_nCA_CMPREV As Short = 97 ' index to company revenues
	Public Const gc_nCA_3DPREV As Short = 98 ' index to 3rd party revenues
	Public Const gc_nCA_NOCREV As Short = 99 ' index to NOC revenues
	Public Const gc_nCA_GRSOPX As Short = 100 ' index to grossed operating expense
	Public Const gc_nCA_CMPOPX As Short = 101 ' index to company opex
	Public Const gc_nCA_3DPOPX As Short = 102 ' index to 3rd party opex
	Public Const gc_nCA_NOCOPX As Short = 103 ' index to NOC opex
	Public Const gc_nCA_GRSFIN As Short = 104 ' index to grossed-up financing
	Public Const gc_nCA_CMPFIN As Short = 105 ' index to company financing
	Public Const gc_nCA_3DPFIN As Short = 106 ' index to 3rd party financing
	Public Const gc_nCA_NOCFIN As Short = 107 ' index to NOC financing
	Public Const gc_nCA_GRSPMT As Short = 108 ' index to grossed-up NOC repayment
	Public Const gc_nCA_CMPPMT As Short = 109 ' index to company NOC repayment
	Public Const gc_nCA_CMPBUR As Short = 110 ' index to company's partner reimbursement
	' End (C0846)
	
	' 10 Feb 2005 JWD (C0856)
	' New symbols for existing AC() consolidation array index 2 positions
	Public Const gc_nAC_CMPPOS As Short = 1 ' index to company positive cash flows
	Public Const gc_nAC_CMPNEG As Short = 2 ' index to company negative cash flows
	Public Const gc_nAC_CMPOPX As Short = 3 ' index to company opex
	Public Const gc_nAC_CMPCPX As Short = 4 ' index to company capex
	Public Const gc_nAC_EQVPRD As Short = 5 ' index to gross equivalent production
	Public Const gc_nAC_EQVREV As Short = 6 ' index to gross revenues
	Public Const gc_nAC_CURDFL As Short = 7 ' index to deflator
	Public Const gc_nAC_GOVTCF As Short = 8 ' index to government cash flow
	Public Const gc_nAC_CMPNGT As Short = 9 ' index to company "t" cash flows
	Public Const gc_nAC_CMPNGA As Short = 10 ' index to company "a" cash flows
	Public Const gc_nAC_CMPNGU As Short = 11 ' index to company "u" cash flows
	
	' Symbols for new AC() array index 2 positions
	Public Const gc_nAC_CMPNCF As Short = 12 ' index to company net cash flow (for 3d party, NOC calcs)
	Public Const gc_nAC_3DPNCF As Short = 13 ' index to 3rd party net cash flow
	Public Const gc_nAC_NOCNCF As Short = 14 ' index to NOC net cash flow
	Public Const gc_nAC_GRSREV As Short = 15 ' index to gross sales revenues
	
	' 18 Jun 2008 JWD
	Public Const gc_nAC_TPS As Short = 16 ' index to gross-level positive cash flow
	Public Const gc_nAC_TNG As Short = 17 ' index to gross-level negative cash flow
	Public Const gc_nAC_OPS As Short = 18 ' index to operating-level positive cash flow
	Public Const gc_nAC_ONG As Short = 19 ' index to operating-level negative cash flow
	Public Const gc_nAC_GPS As Short = 20 ' index to group-level positive cash flow
	Public Const gc_nAC_GNG As Short = 21 ' index to group-level negative cash flow
	Public Const gc_nAC_CPS As Short = 22 ' index to company-level positive cash flow
	Public Const gc_nAC_CNG As Short = 23 ' index to company-level negative cash flow
	
	Public Const gc_nACSIZED2 As Short = 23 ' dimension 2 upper bound
	
	
	' New symbols for existing CC() consolidation array index 2 positions
	Public Const gc_nCC_CAT As Short = 1 ' capital expenditure category
	Public Const gc_nCC_XMO As Short = 2 ' month of capital expenditure
	Public Const gc_nCC_XYR As Short = 3 ' year of capital expenditure
	Public Const gc_nCC_AMT As Short = 4 ' company capital expenditure amount
	
	' Symbols for new CC() array index 2 positions
	Public Const gc_nCC_CMP As Short = 5 ' company net capital expenditure amount (for 3d party & noc calc)
	Public Const gc_nCC_3DP As Short = 6 ' 3rd party net capital expenditure amount
	Public Const gc_nCC_NOC As Short = 7 ' NOC net capital expenditure amount
	
	Public Const gc_nCC_TCX As Short = 8 ' index to gross-level capital expenditure
	Public Const gc_nCC_OCX As Short = 9 ' index to operating-level capital expenditure
	Public Const gc_nCC_GCX As Short = 10 ' index to group-level capital expenditure
	Public Const gc_nCC_CCX As Short = 11 ' index to company-level capital expenditure
	
	Public Const gc_nCCSIZED2 As Short = 11 ' dimension 2 upper bound
	' End (C0856)
	
	Public Function ATotalRevenues(ByVal nYr As Short) As Single
		ATotalRevenues = (A(nYr, gc_nAOIL) * A(nYr, gc_nAOPC)) + (A(nYr, gc_nAGAS) * A(nYr, gc_nAGPC)) + (A(nYr, gc_nAOV1) * A(nYr, gc_nAOP1)) + (A(nYr, gc_nAOV2) * A(nYr, gc_nAOP2)) + (A(nYr, gc_nAOV3) * A(nYr, gc_nAOP3)) + (A(nYr, gc_nAOV4) * A(nYr, gc_nAOP4)) + (A(nYr, gc_nAOV5) * A(nYr, gc_nAOP5)) + (A(nYr, gc_nAOV6) * A(nYr, gc_nAOP6)) + (A(nYr, gc_nAOV7) * A(nYr, gc_nAOP7)) + (A(nYr, gc_nAOV8) * A(nYr, gc_nAOP8)) + (A(nYr, gc_nAOV9) * A(nYr, gc_nAOP9)) + (A(nYr, gc_nAOV0) * A(nYr, gc_nAOP0))
		
	End Function
	
	Public Function TotalGrossRevenue(ByVal nYr As Short) As Single
		TotalGrossRevenue = GrossRevenue(nYr, gc_nAOIL) + GrossRevenue(nYr, gc_nAGAS) + GrossRevenue(nYr, gc_nAOV1) + GrossRevenue(nYr, gc_nAOV2) + GrossRevenue(nYr, gc_nAOV3) + GrossRevenue(nYr, gc_nAOV4) + GrossRevenue(nYr, gc_nAOV5) + GrossRevenue(nYr, gc_nAOV6) + GrossRevenue(nYr, gc_nAOV7) + GrossRevenue(nYr, gc_nAOV8) + GrossRevenue(nYr, gc_nAOV9) + GrossRevenue(nYr, gc_nAOV0)
		
	End Function
End Module