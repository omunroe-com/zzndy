VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "IDGiantRunSummaryA"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' Name:         IDGiantRunSummaryA.cls
' Function:     Run Summary Interface Definition
' Date:         22 Sep 2004 JWD
'---------------------------------------------------------
' Modifications:
' 3 Dec 2004 JWD
'  -> Add ThirdPartyRateOfReturn property. (C0846)
'  -> Add NOCRateOfReturn property. (C0846)
'---------------------------------------------------------
Option Explicit

' The properties provided for in this interface are
' (initially as of 22 Sep 2004) as follows:
'
' Following are read/write so CASHFLOW can update them
Public RunTitle As String                       ' (RF$(2))
'Public RunCurrency As String                    ' (sCur)
Public TotalEquivalentReserves As Single        ' (TTL1T) = 1
Public TotalOperatingExpenses As Single         ' (OPST) = 2
Public TotalExplorationCapital As Single        ' (Ex) = 3
Public TotalDevelopmentCapital As Single        ' (DV) = 4
Public TotalOtherCapital As Single              ' (OTH) = 5
Public AverageEquivalentPrice As Single         ' (PRCE) = 6
Public UnitAverageOperatingExpenses As Single   ' (OXB) = 7
Public UnitAverageExplorationCapital As Single  ' (EXB) = 8
Public UnitAverageDevelopmentCapital As Single  ' (DVB) = 9
Public UnitAverageOtherCapital As Single        ' (OTHB) = 10
Public ProductionLife As Single                 ' (LFITemp) = 11
Public ProjectLife As Single                    ' (ZN) = 12
'Public CompanyRateOfReturn As Single            ' (RR) = 13
'Public GovernmentRateOfReturn As Single         ' (RRB) = 14

' 3 Dec 2004 JWD (C0846) Add new R/W properties for new RateOfReturns
Public ThirdPartyRateOfReturn As Single         ' RRT = ??
Public NOCRateOfReturn As Single                ' RRN = ??


' Following read-only

Public Property Get CompanyRateOfReturn() As Single
End Property

Public Property Get GovernmentRateOfReturn() As Single
End Property

Public Property Get RunCurrency() As String
End Property

'
' Following are read-only Indicators of cash flows discounted at rate 1
'
Public Property Get CompanyIncomeDCF1() As Single
End Property

Public Property Get CompanyOperatingExpenseDCF1() As Single
End Property

Public Property Get CompanyCapitalExpenditureDCF1() As Single
End Property

Public Property Get CompanyRoyaltyTaxDCF1() As Single
End Property

Public Property Get CompanyNetCashFlowDCF1() As Single
End Property

Public Property Get CompanyPayoutDCF1() As Single
End Property

Public Property Get CompanyRiskReturnRatioDCF1() As Single
End Property

Public Property Get CompanyProfitabilityIndexDCF1() As Single
End Property

Public Property Get CompanyGovernmentTakeDCF1() As Single
End Property

'
' Following are read-only Indicators of cash flows discounted at rate 5
'
Public Property Get CompanyIncomeDCF5() As Single
End Property

Public Property Get CompanyOperatingExpenseDCF5() As Single
End Property

Public Property Get CompanyCapitalExpenditureDCF5() As Single
End Property

Public Property Get CompanyRoyaltyTaxDCF5() As Single
End Property

Public Property Get CompanyNetCashFlowDCF5() As Single
End Property

Public Property Get CompanyPayoutDCF5() As Single
End Property

Public Property Get CompanyRiskReturnRatioDCF5() As Single
End Property

Public Property Get CompanyProfitabilityIndexDCF5() As Single
End Property

Public Property Get CompanyGovernmentTakeDCF5() As Single
End Property


