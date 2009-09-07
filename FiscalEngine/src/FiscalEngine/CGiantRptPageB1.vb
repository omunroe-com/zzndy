Option Strict Off
Option Explicit On
Friend Class CGiantRptPageB1
    Implements IDGiantRptPageInd
    Implements IDPersistObject
    Implements _IDGiantRunSummaryA
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


    ' 22 Sep 2004 JWD (C0839) Add interface

    '
    ' IDPersistObject attributes
    '
    Private Const m_lClassID As Integer = 2
    Private Const m_sClassName As String = "CGiantRptPageB1"
    Private m_lObjectID As Integer

    ' 3 Dec 2004 JWD (C0846) Change count of indicators
    Private Const mc_nIndicatorCount As Short = 24 ' was: 16
    Private Const mc_nDiscountRateCount As Short = 6

    Private m_oHeader As CGiantRptPageHdr1

    Private ma_rRates() As Single
    Private ma_sTitles() As String
    Private ma_rValues(,) As Single

    ' 22 Sep 2004 JWD (C0839) Add IDGiantRunSummaryA attributes
    Private m_sRunTitle As String
    Private m_rTotalEquivalentReserves As Single ' (TTL1T) = 1
    'Private m_rTotalEquivalentVolumetricReserves As Single

    Private m_rTotalOperatingExpenses As Single ' (OPST) = 2
    Private m_rTotalExplorationCapital As Single ' (Ex) = 3
    Private m_rTotalDevelopmentCapital As Single ' (DV) = 4
    Private m_rTotalOtherCapital As Single ' (OTH) = 5
    Private m_rAverageEquivalentPrice As Single ' (PRCE) = 6
    Private m_rUnitAverageOperatingExpenses As Single ' (OXB) = 7
    Private m_rUnitAverageExplorationCapital As Single ' (EXB) = 8
    Private m_rUnitAverageDevelopmentCapital As Single ' (DVB) = 9
    Private m_rUnitAverageOtherCapital As Single ' (OTHB) = 10
    Private m_rProductionLife As Single ' (LFITemp) = 11
    Private m_rProjectLife As Single ' (ZN) = 12
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
    Public Sub SetPageHeader(ByVal PageType As Short, ByVal startyear As Short, ByVal PageCount As Short, ByVal IndicatorCount As Short, ByVal DiscountRateCount As Short, ByVal PageTitle As String, ByVal ColumnWidth As Short, ByVal FinalWorkingInt As Single, ByVal FinalParticipation As Single, ByVal PageCurrency As String)

        With m_oHeader
            .PageType = PageType
            .year_Renamed = startyear
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

        ReDim ma_rRates(mc_nDiscountRateCount - 1)
        ReDim ma_sTitles(mc_nIndicatorCount - 1)
        ReDim ma_rValues(mc_nDiscountRateCount - 1, mc_nIndicatorCount - 1)

    End Sub

    '
    ' Set the Economic Summary discount rates
    '
    ' This is to replace the Write statement that writes
    ' this data on the report file.
    '
    'UPGRADE_WARNING: ParamArray DiscountRates was changed from ByRef to ByVal. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="93C6A0DC-8C99-429A-8696-35FC4DCEFCCC"'
    Public Sub SetDiscountRates(ByVal ParamArray DiscountRates() As Object)

        Dim i As Short
        Dim k As Short

        k = LBound(DiscountRates)

        For i = 0 To mc_nDiscountRateCount - 1
            'UPGRADE_WARNING: Couldn't resolve default property of object DiscountRates(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            ma_rRates(i) = DiscountRates(i + k)
        Next i

    End Sub

    '
    ' Set the values for the specified indicator (row)
    '
    ' This is to replace the Write statement that writes
    ' this data on the report file.
    '
    'UPGRADE_WARNING: ParamArray ProfileValues was changed from ByRef to ByVal. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="93C6A0DC-8C99-429A-8696-35FC4DCEFCCC"'
    Public Sub SetProfileValues(ByVal RowIndex As Short, ByVal ParamArray ProfileValues() As Object)

        Dim i As Short
        Dim k As Short

        k = LBound(ProfileValues)

        ' first element is indicator name
        'UPGRADE_WARNING: Couldn't resolve default property of object ProfileValues(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        ma_sTitles(RowIndex - 1) = ProfileValues(k)

        k = k + 1 ' point to value @ first discount rate
        For i = 0 To mc_nDiscountRateCount - 1
            'UPGRADE_WARNING: Couldn't resolve default property of object ProfileValues(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            ma_rValues(i, RowIndex - 1) = ProfileValues(i + k)
        Next i

    End Sub

    '
    ' Property to return the company rate of return for the run
    '
    ' Company rate of return is stored in first discount rate
    ' column for next to last indicator in the values array.
    '
    Public ReadOnly Property CompanyRateOfReturn() As Single
        Get
            CompanyRateOfReturn = ma_rValues(LBound(ma_rValues, 1), UBound(ma_rValues, 2) - 1)
        End Get
    End Property

    '
    ' Property to return the government rate of return for the run
    '
    ' Government rate of return is stored in first discount
    ' rate column for next to last indicator in the values
    ' array.
    '
    Public ReadOnly Property GovernmentRateOfReturn() As Single
        Get
            GovernmentRateOfReturn = ma_rValues(LBound(ma_rValues, 1), UBound(ma_rValues, 2))
        End Get
    End Property

    '
    ' 3 Dec 2004 JWD New (C0846)
    '
    ' National Oil Company rate of return for the run
    '
    Public ReadOnly Property NOCRateOfReturn() As Single
        Get
            NOCRateOfReturn = m_rNOCRateOfReturn
        End Get
    End Property

    '
    ' 3 Dec 2004 JWD New (C0846)
    '
    ' Third Party rate of return for the run
    '
    Public ReadOnly Property ThirdPartyRateOfReturn() As Single
        Get
            ThirdPartyRateOfReturn = m_rThirdPartyRateOfReturn
        End Get
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
    Public ReadOnly Property PresentValueTable() As Single()
        Get

            Dim i As Integer
            Dim j As Integer
            Dim a_rValues(,) As Single

            'UPGRADE_WARNING: Lower bound of array a_rValues was changed from 1,1 to 0,0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ReDim a_rValues(UBound(ma_rValues, 2) - 1, UBound(ma_rValues, 1) + 1)

            For j = 1 To UBound(a_rValues, 2)
                For i = 1 To UBound(a_rValues, 1)
                    a_rValues(i, j) = ma_rValues(j - 1, i - 1)
                Next i
            Next j

            PresentValueTable = VB6.CopyArray(a_rValues)

        End Get
    End Property


    '=========================================================
    '
    ' IDGiantRptPageInd Interface
    '



    Private Property IDGiantRptPageInd_PageHeader() As CGiantRptPageHdr1 Implements IDGiantRptPageInd.PageHeader
        Get
            IDGiantRptPageInd_PageHeader = m_oHeader
        End Get
        Set(ByVal Value As CGiantRptPageHdr1)
            m_oHeader = Value
        End Set
    End Property


    Private Property IDGiantRptPageInd_Headers() As String() Implements IDGiantRptPageInd.Headers
        Get
            IDGiantRptPageInd_Headers = VB6.CopyArray(ma_sTitles)
        End Get
        Set(ByVal Value() As String)
            ma_sTitles = VB6.CopyArray(Value)
        End Set
    End Property


    Private Property IDGiantRptPageInd_Rates() As Single() Implements IDGiantRptPageInd.Rates
        Get
            IDGiantRptPageInd_Rates = VB6.CopyArray(ma_rRates)
        End Get
        Set(ByVal Value() As Single)
            ma_rRates = VB6.CopyArray(Value)
        End Set
    End Property


    Private Property IDGiantRptPageInd_Values() As Single(,) Implements IDGiantRptPageInd.Values
        Get
            IDGiantRptPageInd_Values = VB6.CopyArray(ma_rValues)
        End Get
        Set(ByVal Value(,) As Single)
            ma_rValues = VB6.CopyArray(Value)
        End Set
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
    Public ReadOnly Property AsIDGiantRunSummaryA() As _IDGiantRunSummaryA
        Get
            AsIDGiantRunSummaryA = Me
        End Get
    End Property

    '
    ' Interface methods follow
    '

    Private Property IDGiantRunSummaryA_AverageEquivalentPrice() As Single Implements _IDGiantRunSummaryA.AverageEquivalentPrice
        Get
            IDGiantRunSummaryA_AverageEquivalentPrice = m_rAverageEquivalentPrice
        End Get
        Set(ByVal Value As Single)
            m_rAverageEquivalentPrice = Value
        End Set
    End Property

    Private ReadOnly Property IDGiantRunSummaryA_CompanyCapitalExpenditureDCF1() As Single Implements _IDGiantRunSummaryA.CompanyCapitalExpenditureDCF1
        Get
            IDGiantRunSummaryA_CompanyCapitalExpenditureDCF1 = ma_rValues(LBound(ma_rValues, 1), LBound(ma_rValues, 2) + 2)
        End Get
    End Property

    Private ReadOnly Property IDGiantRunSummaryA_CompanyCapitalExpenditureDCF5() As Single Implements _IDGiantRunSummaryA.CompanyCapitalExpenditureDCF5
        Get
            IDGiantRunSummaryA_CompanyCapitalExpenditureDCF5 = ma_rValues(LBound(ma_rValues, 1) + 4, LBound(ma_rValues, 2) + 2)
        End Get
    End Property

    Private ReadOnly Property IDGiantRunSummaryA_CompanyGovernmentTakeDCF1() As Single Implements _IDGiantRunSummaryA.CompanyGovernmentTakeDCF1
        Get
            IDGiantRunSummaryA_CompanyGovernmentTakeDCF1 = ma_rValues(LBound(ma_rValues, 1), LBound(ma_rValues, 2) + 8)
        End Get
    End Property

    Private ReadOnly Property IDGiantRunSummaryA_CompanyGovernmentTakeDCF5() As Single Implements _IDGiantRunSummaryA.CompanyGovernmentTakeDCF5
        Get
            IDGiantRunSummaryA_CompanyGovernmentTakeDCF5 = ma_rValues(LBound(ma_rValues, 1) + 4, LBound(ma_rValues, 2) + 8)
        End Get
    End Property

    Private ReadOnly Property IDGiantRunSummaryA_CompanyIncomeDCF1() As Single Implements _IDGiantRunSummaryA.CompanyIncomeDCF1
        Get
            IDGiantRunSummaryA_CompanyIncomeDCF1 = ma_rValues(LBound(ma_rValues, 1), LBound(ma_rValues, 2) + 0)
        End Get
    End Property

    Private ReadOnly Property IDGiantRunSummaryA_CompanyIncomeDCF5() As Single Implements _IDGiantRunSummaryA.CompanyIncomeDCF5
        Get
            IDGiantRunSummaryA_CompanyIncomeDCF5 = ma_rValues(LBound(ma_rValues, 1) + 4, LBound(ma_rValues, 2) + 0)
        End Get
    End Property

    Private ReadOnly Property IDGiantRunSummaryA_CompanyNetCashFlowDCF1() As Single Implements _IDGiantRunSummaryA.CompanyNetCashFlowDCF1
        Get
            IDGiantRunSummaryA_CompanyNetCashFlowDCF1 = ma_rValues(LBound(ma_rValues, 1), LBound(ma_rValues, 2) + 4)
        End Get
    End Property

    Private ReadOnly Property IDGiantRunSummaryA_CompanyNetCashFlowDCF5() As Single Implements _IDGiantRunSummaryA.CompanyNetCashFlowDCF5
        Get
            IDGiantRunSummaryA_CompanyNetCashFlowDCF5 = ma_rValues(LBound(ma_rValues, 1) + 4, LBound(ma_rValues, 2) + 4)
        End Get
    End Property

    Private ReadOnly Property IDGiantRunSummaryA_CompanyOperatingExpenseDCF1() As Single Implements _IDGiantRunSummaryA.CompanyOperatingExpenseDCF1
        Get
            IDGiantRunSummaryA_CompanyOperatingExpenseDCF1 = ma_rValues(LBound(ma_rValues, 1), LBound(ma_rValues, 2) + 1)
        End Get
    End Property

    Private ReadOnly Property IDGiantRunSummaryA_CompanyOperatingExpenseDCF5() As Single Implements _IDGiantRunSummaryA.CompanyOperatingExpenseDCF5
        Get
            IDGiantRunSummaryA_CompanyOperatingExpenseDCF5 = ma_rValues(LBound(ma_rValues, 1) + 4, LBound(ma_rValues, 2) + 1)
        End Get
    End Property

    Private ReadOnly Property IDGiantRunSummaryA_CompanyPayoutDCF1() As Single Implements _IDGiantRunSummaryA.CompanyPayoutDCF1
        Get
            IDGiantRunSummaryA_CompanyPayoutDCF1 = ma_rValues(LBound(ma_rValues, 1), LBound(ma_rValues, 2) + 5)
        End Get
    End Property

    Private ReadOnly Property IDGiantRunSummaryA_CompanyPayoutDCF5() As Single Implements _IDGiantRunSummaryA.CompanyPayoutDCF5
        Get
            IDGiantRunSummaryA_CompanyPayoutDCF5 = ma_rValues(LBound(ma_rValues, 1) + 4, LBound(ma_rValues, 2) + 5)
        End Get
    End Property

    Private ReadOnly Property IDGiantRunSummaryA_CompanyProfitabilityIndexDCF1() As Single Implements _IDGiantRunSummaryA.CompanyProfitabilityIndexDCF1
        Get
            IDGiantRunSummaryA_CompanyProfitabilityIndexDCF1 = ma_rValues(LBound(ma_rValues, 1), LBound(ma_rValues, 2) + 7)
        End Get
    End Property

    Private ReadOnly Property IDGiantRunSummaryA_CompanyProfitabilityIndexDCF5() As Single Implements _IDGiantRunSummaryA.CompanyProfitabilityIndexDCF5
        Get
            IDGiantRunSummaryA_CompanyProfitabilityIndexDCF5 = ma_rValues(LBound(ma_rValues, 1) + 4, LBound(ma_rValues, 2) + 7)
        End Get
    End Property

    Private ReadOnly Property IDGiantRunSummaryA_CompanyRateOfReturn() As Single Implements _IDGiantRunSummaryA.CompanyRateOfReturn
        Get
            IDGiantRunSummaryA_CompanyRateOfReturn = Me.CompanyRateOfReturn
        End Get
    End Property

    Private ReadOnly Property IDGiantRunSummaryA_CompanyRiskReturnRatioDCF1() As Single Implements _IDGiantRunSummaryA.CompanyRiskReturnRatioDCF1
        Get
            IDGiantRunSummaryA_CompanyRiskReturnRatioDCF1 = ma_rValues(LBound(ma_rValues, 1), LBound(ma_rValues, 2) + 6)
        End Get
    End Property

    Private ReadOnly Property IDGiantRunSummaryA_CompanyRiskReturnRatioDCF5() As Single Implements _IDGiantRunSummaryA.CompanyRiskReturnRatioDCF5
        Get
            IDGiantRunSummaryA_CompanyRiskReturnRatioDCF5 = ma_rValues(LBound(ma_rValues, 1) + 4, LBound(ma_rValues, 2) + 6)
        End Get
    End Property

    Private ReadOnly Property IDGiantRunSummaryA_CompanyRoyaltyTaxDCF1() As Single Implements _IDGiantRunSummaryA.CompanyRoyaltyTaxDCF1
        Get
            IDGiantRunSummaryA_CompanyRoyaltyTaxDCF1 = ma_rValues(LBound(ma_rValues, 1), LBound(ma_rValues, 2) + 3)
        End Get
    End Property

    Private ReadOnly Property IDGiantRunSummaryA_CompanyRoyaltyTaxDCF5() As Single Implements _IDGiantRunSummaryA.CompanyRoyaltyTaxDCF5
        Get
            IDGiantRunSummaryA_CompanyRoyaltyTaxDCF5 = ma_rValues(LBound(ma_rValues, 1) + 4, LBound(ma_rValues, 2) + 3)
        End Get
    End Property

    Private ReadOnly Property IDGiantRunSummaryA_GovernmentRateOfReturn() As Single Implements _IDGiantRunSummaryA.GovernmentRateOfReturn
        Get
            IDGiantRunSummaryA_GovernmentRateOfReturn = Me.GovernmentRateOfReturn
        End Get
    End Property


    Private Property IDGiantRunSummaryA_NOCRateOfReturn() As Single Implements _IDGiantRunSummaryA.NOCRateOfReturn
        Get
            IDGiantRunSummaryA_NOCRateOfReturn = m_rNOCRateOfReturn
        End Get
        Set(ByVal Value As Single)
            m_rNOCRateOfReturn = Value
        End Set
    End Property


    Private Property IDGiantRunSummaryA_ProductionLife() As Single Implements _IDGiantRunSummaryA.ProductionLife
        Get
            IDGiantRunSummaryA_ProductionLife = m_rProductionLife
        End Get
        Set(ByVal Value As Single)
            m_rProductionLife = Value
        End Set
    End Property


    Private Property IDGiantRunSummaryA_ProjectLife() As Single Implements _IDGiantRunSummaryA.ProjectLife
        Get
            IDGiantRunSummaryA_ProjectLife = m_rProjectLife
        End Get
        Set(ByVal Value As Single)
            m_rProjectLife = Value
        End Set
    End Property

    Private ReadOnly Property IDGiantRunSummaryA_RunCurrency() As String Implements _IDGiantRunSummaryA.RunCurrency
        Get
            IDGiantRunSummaryA_RunCurrency = m_oHeader.CurrencyCode
        End Get
    End Property


    Private Property IDGiantRunSummaryA_RunTitle() As String Implements _IDGiantRunSummaryA.RunTitle
        Get
            IDGiantRunSummaryA_RunTitle = m_sRunTitle
        End Get
        Set(ByVal Value As String)
            m_sRunTitle = Value
        End Set
    End Property


    Private Property IDGiantRunSummaryA_ThirdPartyRateOfReturn() As Single Implements _IDGiantRunSummaryA.ThirdPartyRateOfReturn
        Get
            IDGiantRunSummaryA_ThirdPartyRateOfReturn = m_rThirdPartyRateOfReturn
        End Get
        Set(ByVal Value As Single)
            m_rThirdPartyRateOfReturn = Value
        End Set
    End Property


    Private Property IDGiantRunSummaryA_TotalDevelopmentCapital() As Single Implements _IDGiantRunSummaryA.TotalDevelopmentCapital
        Get
            IDGiantRunSummaryA_TotalDevelopmentCapital = m_rTotalDevelopmentCapital
        End Get
        Set(ByVal Value As Single)
            m_rTotalDevelopmentCapital = Value
        End Set
    End Property


    Private Property IDGiantRunSummaryA_TotalEquivalentReserves() As Single Implements _IDGiantRunSummaryA.TotalEquivalentReserves
        Get
            IDGiantRunSummaryA_TotalEquivalentReserves = m_rTotalEquivalentReserves
        End Get
        Set(ByVal Value As Single)
            m_rTotalEquivalentReserves = Value
        End Set
    End Property



    'Private Property Let IDGiantRunSummaryA_TotalEquivalentVolumetricReserves(ByVal NewValue As Single)
    '    m_rTotalEquivalentVolumetricReserves = NewValue
    'End Property
    '
    'Private Property Get IDGiantRunSummaryA_TotalEquivalentVolumetricReserves() As Single
    '    IDGiantRunSummaryA_TotalEquivalentVolumetricReserves = m_rTotalEquivalentVolumetricReserves
    'End Property



    Private Property IDGiantRunSummaryA_TotalExplorationCapital() As Single Implements _IDGiantRunSummaryA.TotalExplorationCapital
        Get
            IDGiantRunSummaryA_TotalExplorationCapital = m_rTotalExplorationCapital
        End Get
        Set(ByVal Value As Single)
            m_rTotalExplorationCapital = Value
        End Set
    End Property


    Private Property IDGiantRunSummaryA_TotalOperatingExpenses() As Single Implements _IDGiantRunSummaryA.TotalOperatingExpenses
        Get
            IDGiantRunSummaryA_TotalOperatingExpenses = m_rTotalOperatingExpenses
        End Get
        Set(ByVal Value As Single)
            m_rTotalOperatingExpenses = Value
        End Set
    End Property


    Private Property IDGiantRunSummaryA_TotalOtherCapital() As Single Implements _IDGiantRunSummaryA.TotalOtherCapital
        Get
            IDGiantRunSummaryA_TotalOtherCapital = m_rTotalOtherCapital
        End Get
        Set(ByVal Value As Single)
            m_rTotalOtherCapital = Value
        End Set
    End Property


    Private Property IDGiantRunSummaryA_UnitAverageDevelopmentCapital() As Single Implements _IDGiantRunSummaryA.UnitAverageDevelopmentCapital
        Get
            IDGiantRunSummaryA_UnitAverageDevelopmentCapital = m_rUnitAverageDevelopmentCapital
        End Get
        Set(ByVal Value As Single)
            m_rUnitAverageDevelopmentCapital = Value
        End Set
    End Property


    Private Property IDGiantRunSummaryA_UnitAverageExplorationCapital() As Single Implements _IDGiantRunSummaryA.UnitAverageExplorationCapital
        Get
            IDGiantRunSummaryA_UnitAverageExplorationCapital = m_rUnitAverageExplorationCapital
        End Get
        Set(ByVal Value As Single)
            m_rUnitAverageExplorationCapital = Value
        End Set
    End Property


    Private Property IDGiantRunSummaryA_UnitAverageOperatingExpenses() As Single Implements _IDGiantRunSummaryA.UnitAverageOperatingExpenses
        Get
            IDGiantRunSummaryA_UnitAverageOperatingExpenses = m_rUnitAverageOperatingExpenses
        End Get
        Set(ByVal Value As Single)
            m_rUnitAverageOperatingExpenses = Value
        End Set
    End Property


    Private Property IDGiantRunSummaryA_UnitAverageOtherCapital() As Single Implements _IDGiantRunSummaryA.UnitAverageOtherCapital
        Get
            IDGiantRunSummaryA_UnitAverageOtherCapital = m_rUnitAverageOtherCapital
        End Get
        Set(ByVal Value As Single)
            m_rUnitAverageOtherCapital = Value
        End Set
    End Property

    '=========================================================
    '
    ' IDPersistObject Interface
    '

    '
    ' The first four are also attributes of IDObject
    ' since all IDPersistObjects are also IDObjects.
    '

    Private ReadOnly Property IDPersistObject_ClassIDNumber() As Integer Implements IDPersistObject.ClassIDNumber
        Get
            IDPersistObject_ClassIDNumber = m_lClassID
        End Get
    End Property

    Private ReadOnly Property IDPersistObject_ClassName() As String Implements IDPersistObject.ClassName
        Get
            IDPersistObject_ClassName = m_sClassName
        End Get
    End Property


    Private Property IDPersistObject_ObjectIDNumber() As Integer Implements IDPersistObject.ObjectIDNumber
        Get
            IDPersistObject_ObjectIDNumber = m_lObjectID
        End Get
        Set(ByVal Value As Integer)
            m_lObjectID = Value
        End Set
    End Property


    'UPGRADE_NOTE: Class_Initialize was upgraded to Class_Initialize_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    Private Sub Class_Initialize_Renamed()
        m_oHeader = New CGiantRptPageHdr1
    End Sub
    Public Sub New()
        MyBase.New()
        Class_Initialize_Renamed()
    End Sub

    '
    ' These methods are specific to IDPersistObjects
    '
    Private Function IDPersistObject_RegisterInTable(ByVal ObjectTable As IDPersistObjectTable) As Boolean Implements IDPersistObject.RegisterInTable
    End Function

    Private Function IDPersistObject_RegisterObjectConnections() As Boolean Implements IDPersistObject.RegisterObjectConnections
    End Function

    Private Function IDPersistObject_RestoreUsingFormat(ByVal TheStore As IDStore, ByVal TheMap As IDPersistClassMap) As Boolean Implements IDPersistObject.RestoreUsingFormat
    End Function

    Private Function IDPersistObject_StoreUsingFormat(ByVal TheStore As IDStore, ByVal TheMap As IDPersistClassMap) As Boolean Implements IDPersistObject.StoreUsingFormat

        'UPGRADE_WARNING: Couldn't resolve default property of object Me. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        TheMap.ClassFormat(Me).Store(TheStore, Me)

    End Function
End Class