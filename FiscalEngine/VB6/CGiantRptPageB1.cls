VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CGiantRptPageB1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' Name:         CGiantRptPageB1.cls
' Function:     Wrapper class for Giant Report Page Type
' Date:         27 Jan 2004 JWD
'---------------------------------------------------------
' This wraps the Economic Summary Page type
'---------------------------------------------------------
' Modifications:
' 22 Sep 2004 JWD
'  -> Add IDGiantRunSummaryA as implemented interface to
'     provide output of run summary (other indicators)
'     data. (C0839)
'  -> Add property AsIDGiantRunSummaryA() to return the
'     IDGiantRunSummaryA interface. (C0839)
'
' 3 Dec 2004 JWD
'  -> Change constant symbol value for number of indicator
'     profiles to be accomodated to allow for 3rd party
'     and NOC indicators. (C0846)
'  -> Add new 3rd party and NOC rate of return indicator
'     properties. Theses are not being stored as part of
'     present value profile table (unlike company and gov-
'     ernment rates of return) (C0846)
'  -> Add IDGiantRunSummaryA_ThirdPartyRateOfReturn() and
'     IDGiantRunSummaryA_NOCRateOfReturn() property
'     methods because of changes to IDGiantRunSummaryA
'     interface definition. (C0846)
'---------------------------------------------------------
Option Explicit

Implements IDGiantRptPageInd
Implements IDPersistObject

' 22 Sep 2004 JWD (C0839) Add interface
Implements IDGiantRunSummaryA

'
' IDPersistObject attributes
'
Private Const m_lClassID As Long = 2
Private Const m_sClassName As String = "CGiantRptPageB1"
Private m_lObjectID As Long

' 3 Dec 2004 JWD (C0846) Change count of indicators
Private Const mc_nIndicatorCount As Integer = 24        ' was: 16
Private Const mc_nDiscountRateCount As Integer = 6

Private m_oHeader As CGiantRptPageHdr1

Private ma_rRates() As Single
Private ma_sTitles() As String
Private ma_rValues() As Single

' 22 Sep 2004 JWD (C0839) Add IDGiantRunSummaryA attributes
Private m_sRunTitle As String
Private m_rTotalEquivalentReserves As Single        ' (TTL1T) = 1
'Private m_rTotalEquivalentVolumetricReserves As Single

Private m_rTotalOperatingExpenses As Single         ' (OPST) = 2
Private m_rTotalExplorationCapital As Single        ' (Ex) = 3
Private m_rTotalDevelopmentCapital As Single        ' (DV) = 4
Private m_rTotalOtherCapital As Single              ' (OTH) = 5
Private m_rAverageEquivalentPrice As Single         ' (PRCE) = 6
Private m_rUnitAverageOperatingExpenses As Single   ' (OXB) = 7
Private m_rUnitAverageExplorationCapital As Single  ' (EXB) = 8
Private m_rUnitAverageDevelopmentCapital As Single  ' (DVB) = 9
Private m_rUnitAverageOtherCapital As Single        ' (OTHB) = 10
Private m_rProductionLife As Single                 ' (LFITemp) = 11
Private m_rProjectLife As Single                    ' (ZN) = 12
' These properties are already allocated to storage
'Private m_sRunCurrency As String                    ' (sCur)
'Private m_rCompanyRateOfReturn As Single            ' (RR) = 13
'Private m_rGovernmentRateOfReturn As Single         ' (RRB) = 14

' 3 Dec 2004 JWD (C0846) Add new rate of return indicators storage
Private m_rThirdPartyRateOfReturn As Single
Private m_rNOCRateOfReturn As Single

'
' Set all elements of the page header.
'
' This is to replace the Write statement that writes
' this data on the report file.
'
Public Sub SetPageHeader _
    ( _
    ByVal PageType As Integer, _
    ByVal startyear As Integer, _
    ByVal PageCount As Integer, _
    ByVal IndicatorCount As Integer, _
    ByVal DiscountRateCount As Integer, _
    ByVal PageTitle As String, _
    ByVal ColumnWidth As Integer, _
    ByVal FinalWorkingInt As Single, _
    ByVal FinalParticipation As Single, _
    ByVal PageCurrency As String _
    )

    Dim i As Integer
    
    With m_oHeader
        .PageType = PageType
        .year = startyear
        .PageCounter = PageCount
        .Rows = IndicatorCount
        .Columns = DiscountRateCount
        .PageTitle = PageTitle
        .ColumnWidth = ColumnWidth
        .CompanyWorkingInterest = FinalWorkingInt
        .GovernmentParticipation = FinalParticipation
        .CurrencyCode = PageCurrency
        
        'If .PageTitle = "GGG (GGG)" Then Stop
    End With
    
    ReDim ma_rRates(0 To mc_nDiscountRateCount - 1)
    ReDim ma_sTitles(0 To mc_nIndicatorCount - 1)
    ReDim ma_rValues(0 To mc_nDiscountRateCount - 1, 0 To mc_nIndicatorCount - 1)
    
End Sub

'
' Set the Economic Summary discount rates
'
' This is to replace the Write statement that writes
' this data on the report file.
'
Public Sub SetDiscountRates _
    ( _
    ParamArray DiscountRates() As Variant _
    )
    
    Dim i As Integer
    Dim k As Integer
    
    k = LBound(DiscountRates)
    
    For i = 0 To mc_nDiscountRateCount - 1
        ma_rRates(i) = DiscountRates(i + k)
    Next i
    
End Sub

'
' Set the values for the specified indicator (row)
'
' This is to replace the Write statement that writes
' this data on the report file.
'
Public Sub SetProfileValues _
    ( _
    ByVal RowIndex As Integer, _
    ParamArray ProfileValues() As Variant _
    )

    Dim i As Integer
    Dim k As Integer
    
    k = LBound(ProfileValues)
    
    ' first element is indicator name
    ma_sTitles(RowIndex - 1) = ProfileValues(k)
        
    k = k + 1   ' point to value @ first discount rate
    For i = 0 To mc_nDiscountRateCount - 1
        ma_rValues(i, RowIndex - 1) = ProfileValues(i + k)
    Next i
    
End Sub

'
' Property to return the company rate of return for the run
'
' Company rate of return is stored in first discount rate
' column for next to last indicator in the values array.
'
Public Property Get CompanyRateOfReturn() As Single
    CompanyRateOfReturn = ma_rValues(LBound(ma_rValues, 1), UBound(ma_rValues, 2) - 1)
End Property

'
' Property to return the government rate of return for the run
'
' Government rate of return is stored in first discount
' rate column for next to last indicator in the values
' array.
'
Public Property Get GovernmentRateOfReturn() As Single
    GovernmentRateOfReturn = ma_rValues(LBound(ma_rValues, 1), UBound(ma_rValues, 2))
End Property

'
' 3 Dec 2004 JWD New (C0846)
'
' National Oil Company rate of return for the run
'
Public Property Get NOCRateOfReturn() As Single
    NOCRateOfReturn = m_rNOCRateOfReturn
End Property

'
' 3 Dec 2004 JWD New (C0846)
'
' Third Party rate of return for the run
'
Public Property Get ThirdPartyRateOfReturn() As Single
    ThirdPartyRateOfReturn = m_rThirdPartyRateOfReturn
End Property

'
' Add Property to return PresentValueTable as array.
' Returned array is 2-d as follows:
'       PresentValueTable(1 To IndicatorsCount, 1 To DiscountRatesCount)
'
' Basically this just returns ma_rValues(), transposing
' the elements and rebasing the array to have 1 as the
' lower bound of each dimension. It also does not return
' the elements for the last two indicators in the array,
' these are actually the Company Rate of Return and
' Government Rate of Return values in the first discount
' rate. All elements for these two indicators are omitted
' from the returned array.
'
Public Property Get PresentValueTable() As Single()

    Dim i As Long
    Dim j As Long
    Dim a_rValues() As Single
    
    ReDim a_rValues(1 To UBound(ma_rValues, 2) - 1, 1 To UBound(ma_rValues, 1) + 1)
    
    For j = 1 To UBound(a_rValues, 2)
        For i = 1 To UBound(a_rValues, 1)
            a_rValues(i, j) = ma_rValues(j - 1, i - 1)
        Next i
    Next j
    
    PresentValueTable = a_rValues

End Property


Private Sub Class_Initialize()
    Set m_oHeader = New CGiantRptPageHdr1
End Sub


'=========================================================
'
' IDGiantRptPageInd Interface
'


Private Property Get IDGiantRptPageInd_PageHeader _
    () As CGiantRptPageHdr1
    Set IDGiantRptPageInd_PageHeader = m_oHeader
End Property

Private Property Set IDGiantRptPageInd_PageHeader _
    ( _
    ByVal NewObject As CGiantRptPageHdr1 _
    )
    Set m_oHeader = NewObject
End Property

Private Property Get IDGiantRptPageInd_Headers _
    () As String()
    IDGiantRptPageInd_Headers = ma_sTitles
End Property

Private Property Let IDGiantRptPageInd_Headers _
    ( _
    ByRef NewValues() As String _
    )
    ma_sTitles = NewValues
End Property

Private Property Get IDGiantRptPageInd_Rates _
    () As Single()
    IDGiantRptPageInd_Rates = ma_rRates
End Property

Private Property Let IDGiantRptPageInd_Rates _
    ( _
    ByRef NewValues() As Single _
    )
    ma_rRates = NewValues
End Property

Private Property Get IDGiantRptPageInd_Values _
    () As Single()
    IDGiantRptPageInd_Values = ma_rValues
End Property

Private Property Let IDGiantRptPageInd_Values _
    ( _
    ByRef NewValues() As Single _
    )
    ma_rValues = NewValues
End Property


'=========================================================
'
' IDGiantRunSummaryA Interface
'
' 22 Sep 2004 JWD New (C0839)
'

'
' Return a reference to this object's implemented interface
'
Public Property Get AsIDGiantRunSummaryA() As IDGiantRunSummaryA
    Set AsIDGiantRunSummaryA = Me
End Property

'
' Interface methods follow
'
Private Property Let IDGiantRunSummaryA_AverageEquivalentPrice(ByVal NewValue As Single)
    m_rAverageEquivalentPrice = NewValue
End Property

Private Property Get IDGiantRunSummaryA_AverageEquivalentPrice() As Single
    IDGiantRunSummaryA_AverageEquivalentPrice = m_rAverageEquivalentPrice
End Property

Private Property Get IDGiantRunSummaryA_CompanyCapitalExpenditureDCF1() As Single
    IDGiantRunSummaryA_CompanyCapitalExpenditureDCF1 = ma_rValues(LBound(ma_rValues, 1), LBound(ma_rValues, 2) + 2)
End Property

Private Property Get IDGiantRunSummaryA_CompanyCapitalExpenditureDCF5() As Single
    IDGiantRunSummaryA_CompanyCapitalExpenditureDCF5 = ma_rValues(LBound(ma_rValues, 1) + 4, LBound(ma_rValues, 2) + 2)
End Property

Private Property Get IDGiantRunSummaryA_CompanyGovernmentTakeDCF1() As Single
    IDGiantRunSummaryA_CompanyGovernmentTakeDCF1 = ma_rValues(LBound(ma_rValues, 1), LBound(ma_rValues, 2) + 8)
End Property

Private Property Get IDGiantRunSummaryA_CompanyGovernmentTakeDCF5() As Single
    IDGiantRunSummaryA_CompanyGovernmentTakeDCF5 = ma_rValues(LBound(ma_rValues, 1) + 4, LBound(ma_rValues, 2) + 8)
End Property

Private Property Get IDGiantRunSummaryA_CompanyIncomeDCF1() As Single
    IDGiantRunSummaryA_CompanyIncomeDCF1 = ma_rValues(LBound(ma_rValues, 1), LBound(ma_rValues, 2) + 0)
End Property

Private Property Get IDGiantRunSummaryA_CompanyIncomeDCF5() As Single
    IDGiantRunSummaryA_CompanyIncomeDCF5 = ma_rValues(LBound(ma_rValues, 1) + 4, LBound(ma_rValues, 2) + 0)
End Property

Private Property Get IDGiantRunSummaryA_CompanyNetCashFlowDCF1() As Single
    IDGiantRunSummaryA_CompanyNetCashFlowDCF1 = ma_rValues(LBound(ma_rValues, 1), LBound(ma_rValues, 2) + 4)
End Property

Private Property Get IDGiantRunSummaryA_CompanyNetCashFlowDCF5() As Single
    IDGiantRunSummaryA_CompanyNetCashFlowDCF5 = ma_rValues(LBound(ma_rValues, 1) + 4, LBound(ma_rValues, 2) + 4)
End Property

Private Property Get IDGiantRunSummaryA_CompanyOperatingExpenseDCF1() As Single
    IDGiantRunSummaryA_CompanyOperatingExpenseDCF1 = ma_rValues(LBound(ma_rValues, 1), LBound(ma_rValues, 2) + 1)
End Property

Private Property Get IDGiantRunSummaryA_CompanyOperatingExpenseDCF5() As Single
    IDGiantRunSummaryA_CompanyOperatingExpenseDCF5 = ma_rValues(LBound(ma_rValues, 1) + 4, LBound(ma_rValues, 2) + 1)
End Property

Private Property Get IDGiantRunSummaryA_CompanyPayoutDCF1() As Single
    IDGiantRunSummaryA_CompanyPayoutDCF1 = ma_rValues(LBound(ma_rValues, 1), LBound(ma_rValues, 2) + 5)
End Property

Private Property Get IDGiantRunSummaryA_CompanyPayoutDCF5() As Single
    IDGiantRunSummaryA_CompanyPayoutDCF5 = ma_rValues(LBound(ma_rValues, 1) + 4, LBound(ma_rValues, 2) + 5)
End Property

Private Property Get IDGiantRunSummaryA_CompanyProfitabilityIndexDCF1() As Single
    IDGiantRunSummaryA_CompanyProfitabilityIndexDCF1 = ma_rValues(LBound(ma_rValues, 1), LBound(ma_rValues, 2) + 7)
End Property

Private Property Get IDGiantRunSummaryA_CompanyProfitabilityIndexDCF5() As Single
    IDGiantRunSummaryA_CompanyProfitabilityIndexDCF5 = ma_rValues(LBound(ma_rValues, 1) + 4, LBound(ma_rValues, 2) + 7)
End Property

Private Property Get IDGiantRunSummaryA_CompanyRateOfReturn() As Single
    IDGiantRunSummaryA_CompanyRateOfReturn = Me.CompanyRateOfReturn
End Property

Private Property Get IDGiantRunSummaryA_CompanyRiskReturnRatioDCF1() As Single
    IDGiantRunSummaryA_CompanyRiskReturnRatioDCF1 = ma_rValues(LBound(ma_rValues, 1), LBound(ma_rValues, 2) + 6)
End Property

Private Property Get IDGiantRunSummaryA_CompanyRiskReturnRatioDCF5() As Single
    IDGiantRunSummaryA_CompanyRiskReturnRatioDCF5 = ma_rValues(LBound(ma_rValues, 1) + 4, LBound(ma_rValues, 2) + 6)
End Property

Private Property Get IDGiantRunSummaryA_CompanyRoyaltyTaxDCF1() As Single
    IDGiantRunSummaryA_CompanyRoyaltyTaxDCF1 = ma_rValues(LBound(ma_rValues, 1), LBound(ma_rValues, 2) + 3)
End Property

Private Property Get IDGiantRunSummaryA_CompanyRoyaltyTaxDCF5() As Single
    IDGiantRunSummaryA_CompanyRoyaltyTaxDCF5 = ma_rValues(LBound(ma_rValues, 1) + 4, LBound(ma_rValues, 2) + 3)
End Property

Private Property Get IDGiantRunSummaryA_GovernmentRateOfReturn() As Single
    IDGiantRunSummaryA_GovernmentRateOfReturn = Me.GovernmentRateOfReturn
End Property

Private Property Let IDGiantRunSummaryA_NOCRateOfReturn(ByVal NewValue As Single)
    m_rNOCRateOfReturn = NewValue
End Property

Private Property Get IDGiantRunSummaryA_NOCRateOfReturn() As Single
    IDGiantRunSummaryA_NOCRateOfReturn = m_rNOCRateOfReturn
End Property

Private Property Let IDGiantRunSummaryA_ProductionLife(ByVal NewValue As Single)
    m_rProductionLife = NewValue
End Property

Private Property Get IDGiantRunSummaryA_ProductionLife() As Single
    IDGiantRunSummaryA_ProductionLife = m_rProductionLife
End Property

Private Property Let IDGiantRunSummaryA_ProjectLife(ByVal NewValue As Single)
    m_rProjectLife = NewValue
End Property

Private Property Get IDGiantRunSummaryA_ProjectLife() As Single
    IDGiantRunSummaryA_ProjectLife = m_rProjectLife
End Property

Private Property Get IDGiantRunSummaryA_RunCurrency() As String
    IDGiantRunSummaryA_RunCurrency = m_oHeader.CurrencyCode
End Property

Private Property Let IDGiantRunSummaryA_RunTitle(ByVal NewValue As String)
    m_sRunTitle = NewValue
End Property

Private Property Get IDGiantRunSummaryA_RunTitle() As String
    IDGiantRunSummaryA_RunTitle = m_sRunTitle
End Property

Private Property Get IDGiantRunSummaryA_ThirdPartyRateOfReturn() As Single
    IDGiantRunSummaryA_ThirdPartyRateOfReturn = m_rThirdPartyRateOfReturn
End Property

Private Property Let IDGiantRunSummaryA_ThirdPartyRateOfReturn(ByVal NewValue As Single)
    m_rThirdPartyRateOfReturn = NewValue
End Property

Private Property Let IDGiantRunSummaryA_TotalDevelopmentCapital(ByVal NewValue As Single)
    m_rTotalDevelopmentCapital = NewValue
End Property

Private Property Get IDGiantRunSummaryA_TotalDevelopmentCapital() As Single
    IDGiantRunSummaryA_TotalDevelopmentCapital = m_rTotalDevelopmentCapital
End Property

Private Property Let IDGiantRunSummaryA_TotalEquivalentReserves(ByVal NewValue As Single)
    m_rTotalEquivalentReserves = NewValue
End Property

Private Property Get IDGiantRunSummaryA_TotalEquivalentReserves() As Single
    IDGiantRunSummaryA_TotalEquivalentReserves = m_rTotalEquivalentReserves
End Property



'Private Property Let IDGiantRunSummaryA_TotalEquivalentVolumetricReserves(ByVal NewValue As Single)
'    m_rTotalEquivalentVolumetricReserves = NewValue
'End Property
'
'Private Property Get IDGiantRunSummaryA_TotalEquivalentVolumetricReserves() As Single
'    IDGiantRunSummaryA_TotalEquivalentVolumetricReserves = m_rTotalEquivalentVolumetricReserves
'End Property


Private Property Let IDGiantRunSummaryA_TotalExplorationCapital(ByVal NewValue As Single)
    m_rTotalExplorationCapital = NewValue
End Property

Private Property Get IDGiantRunSummaryA_TotalExplorationCapital() As Single
    IDGiantRunSummaryA_TotalExplorationCapital = m_rTotalExplorationCapital
End Property

Private Property Let IDGiantRunSummaryA_TotalOperatingExpenses(ByVal NewValue As Single)
    m_rTotalOperatingExpenses = NewValue
End Property

Private Property Get IDGiantRunSummaryA_TotalOperatingExpenses() As Single
    IDGiantRunSummaryA_TotalOperatingExpenses = m_rTotalOperatingExpenses
End Property

Private Property Let IDGiantRunSummaryA_TotalOtherCapital(ByVal NewValue As Single)
    m_rTotalOtherCapital = NewValue
End Property

Private Property Get IDGiantRunSummaryA_TotalOtherCapital() As Single
    IDGiantRunSummaryA_TotalOtherCapital = m_rTotalOtherCapital
End Property

Private Property Let IDGiantRunSummaryA_UnitAverageDevelopmentCapital(ByVal NewValue As Single)
    m_rUnitAverageDevelopmentCapital = NewValue
End Property

Private Property Get IDGiantRunSummaryA_UnitAverageDevelopmentCapital() As Single
    IDGiantRunSummaryA_UnitAverageDevelopmentCapital = m_rUnitAverageDevelopmentCapital
End Property

Private Property Let IDGiantRunSummaryA_UnitAverageExplorationCapital(ByVal NewValue As Single)
    m_rUnitAverageExplorationCapital = NewValue
End Property

Private Property Get IDGiantRunSummaryA_UnitAverageExplorationCapital() As Single
    IDGiantRunSummaryA_UnitAverageExplorationCapital = m_rUnitAverageExplorationCapital
End Property

Private Property Let IDGiantRunSummaryA_UnitAverageOperatingExpenses(ByVal NewValue As Single)
    m_rUnitAverageOperatingExpenses = NewValue
End Property

Private Property Get IDGiantRunSummaryA_UnitAverageOperatingExpenses() As Single
    IDGiantRunSummaryA_UnitAverageOperatingExpenses = m_rUnitAverageOperatingExpenses
End Property

Private Property Let IDGiantRunSummaryA_UnitAverageOtherCapital(ByVal NewValue As Single)
    m_rUnitAverageOtherCapital = NewValue
End Property

Private Property Get IDGiantRunSummaryA_UnitAverageOtherCapital() As Single
    IDGiantRunSummaryA_UnitAverageOtherCapital = m_rUnitAverageOtherCapital
End Property

'=========================================================
'
' IDPersistObject Interface
'

'
' The first four are also attributes of IDObject
' since all IDPersistObjects are also IDObjects.
'

Private Property Get IDPersistObject_ClassIDNumber _
    () As Long
    IDPersistObject_ClassIDNumber = m_lClassID
End Property

Private Property Get IDPersistObject_ClassName _
    () As String
    IDPersistObject_ClassName = m_sClassName
End Property

Private Property Let IDPersistObject_ObjectIDNumber _
    ( _
    ByVal NewIDNumber As Long _
    )
    m_lObjectID = NewIDNumber
End Property

Private Property Get IDPersistObject_ObjectIDNumber _
    () As Long
    IDPersistObject_ObjectIDNumber = m_lObjectID
End Property

'
' These methods are specific to IDPersistObjects
'
Private Function IDPersistObject_RegisterInTable _
    ( _
    ByVal ObjectTable As IDPersistObjectTable _
    ) As Boolean
End Function

Private Function IDPersistObject_RegisterObjectConnections _
    () As Boolean
End Function

Private Function IDPersistObject_RestoreUsingFormat _
    ( _
    ByVal TheStore As IDStore, _
    ByVal TheMap As IDPersistClassMap _
    ) As Boolean
End Function

Private Function IDPersistObject_StoreUsingFormat _
    ( _
    ByVal TheStore As IDStore, _
    ByVal TheMap As IDPersistClassMap _
    ) As Boolean
    
    TheMap.ClassFormat(Me).Store TheStore, Me
    
End Function

