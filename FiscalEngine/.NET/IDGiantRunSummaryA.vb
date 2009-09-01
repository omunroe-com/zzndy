Option Strict Off
Option Explicit On
Interface _IDGiantRunSummaryA
	 Property RunTitle As String
	 Property TotalEquivalentReserves As Single
	 Property TotalOperatingExpenses As Single
	 Property TotalExplorationCapital As Single
	 Property TotalDevelopmentCapital As Single
	 Property TotalOtherCapital As Single
	 Property AverageEquivalentPrice As Single
	 Property UnitAverageOperatingExpenses As Single
	 Property UnitAverageExplorationCapital As Single
	 Property UnitAverageDevelopmentCapital As Single
	 Property UnitAverageOtherCapital As Single
	 Property ProductionLife As Single
	 Property ProjectLife As Single
	 Property ThirdPartyRateOfReturn As Single
	 Property NOCRateOfReturn As Single
	ReadOnly Property CompanyRateOfReturn As Single
	ReadOnly Property GovernmentRateOfReturn As Single
	ReadOnly Property RunCurrency As String
	ReadOnly Property CompanyIncomeDCF1 As Single
	ReadOnly Property CompanyOperatingExpenseDCF1 As Single
	ReadOnly Property CompanyCapitalExpenditureDCF1 As Single
	ReadOnly Property CompanyRoyaltyTaxDCF1 As Single
	ReadOnly Property CompanyNetCashFlowDCF1 As Single
	ReadOnly Property CompanyPayoutDCF1 As Single
	ReadOnly Property CompanyRiskReturnRatioDCF1 As Single
	ReadOnly Property CompanyProfitabilityIndexDCF1 As Single
	ReadOnly Property CompanyGovernmentTakeDCF1 As Single
	ReadOnly Property CompanyIncomeDCF5 As Single
	ReadOnly Property CompanyOperatingExpenseDCF5 As Single
	ReadOnly Property CompanyCapitalExpenditureDCF5 As Single
	ReadOnly Property CompanyRoyaltyTaxDCF5 As Single
	ReadOnly Property CompanyNetCashFlowDCF5 As Single
	ReadOnly Property CompanyPayoutDCF5 As Single
	ReadOnly Property CompanyRiskReturnRatioDCF5 As Single
	ReadOnly Property CompanyProfitabilityIndexDCF5 As Single
	ReadOnly Property CompanyGovernmentTakeDCF5 As Single
End Interface
Friend Class IDGiantRunSummaryA
	Implements _IDGiantRunSummaryA
	' Name:         IDGiantRunSummaryA.cls
	' Function:     Run Summary Interface Definition
	' Date:         22 Sep 2004 JWD
	'---------------------------------------------------------
	' Modifications:
	' 3 Dec 2004 JWD
	'  -> Add ThirdPartyRateOfReturn property. (C0846)
	'  -> Add NOCRateOfReturn property. (C0846)
	'---------------------------------------------------------
	
	' The properties provided for in this interface are
	' (initially as of 22 Sep 2004) as follows:
	'
	' Following are read/write so CASHFLOW can update them
	Dim RunTitle_MemberVariable As String
	Public Property RunTitle() As String Implements _IDGiantRunSummaryA.RunTitle
		Get
			RunTitle = RunTitle_MemberVariable
		End Get
		Set(ByVal Value As String)
			RunTitle_MemberVariable = Value
		End Set
	End Property ' (RF$(2))
	'Public RunCurrency As String                    ' (sCur)
	Dim TotalEquivalentReserves_MemberVariable As Single
	Public Property TotalEquivalentReserves() As Single Implements _IDGiantRunSummaryA.TotalEquivalentReserves
		Get
			TotalEquivalentReserves = TotalEquivalentReserves_MemberVariable
		End Get
		Set(ByVal Value As Single)
			TotalEquivalentReserves_MemberVariable = Value
		End Set
	End Property ' (TTL1T) = 1
	Dim TotalOperatingExpenses_MemberVariable As Single
	Public Property TotalOperatingExpenses() As Single Implements _IDGiantRunSummaryA.TotalOperatingExpenses
		Get
			TotalOperatingExpenses = TotalOperatingExpenses_MemberVariable
		End Get
		Set(ByVal Value As Single)
			TotalOperatingExpenses_MemberVariable = Value
		End Set
	End Property ' (OPST) = 2
	Dim TotalExplorationCapital_MemberVariable As Single
	Public Property TotalExplorationCapital() As Single Implements _IDGiantRunSummaryA.TotalExplorationCapital
		Get
			TotalExplorationCapital = TotalExplorationCapital_MemberVariable
		End Get
		Set(ByVal Value As Single)
			TotalExplorationCapital_MemberVariable = Value
		End Set
	End Property ' (Ex) = 3
	Dim TotalDevelopmentCapital_MemberVariable As Single
	Public Property TotalDevelopmentCapital() As Single Implements _IDGiantRunSummaryA.TotalDevelopmentCapital
		Get
			TotalDevelopmentCapital = TotalDevelopmentCapital_MemberVariable
		End Get
		Set(ByVal Value As Single)
			TotalDevelopmentCapital_MemberVariable = Value
		End Set
	End Property ' (DV) = 4
	Dim TotalOtherCapital_MemberVariable As Single
	Public Property TotalOtherCapital() As Single Implements _IDGiantRunSummaryA.TotalOtherCapital
		Get
			TotalOtherCapital = TotalOtherCapital_MemberVariable
		End Get
		Set(ByVal Value As Single)
			TotalOtherCapital_MemberVariable = Value
		End Set
	End Property ' (OTH) = 5
	Dim AverageEquivalentPrice_MemberVariable As Single
	Public Property AverageEquivalentPrice() As Single Implements _IDGiantRunSummaryA.AverageEquivalentPrice
		Get
			AverageEquivalentPrice = AverageEquivalentPrice_MemberVariable
		End Get
		Set(ByVal Value As Single)
			AverageEquivalentPrice_MemberVariable = Value
		End Set
	End Property ' (PRCE) = 6
	Dim UnitAverageOperatingExpenses_MemberVariable As Single
	Public Property UnitAverageOperatingExpenses() As Single Implements _IDGiantRunSummaryA.UnitAverageOperatingExpenses
		Get
			UnitAverageOperatingExpenses = UnitAverageOperatingExpenses_MemberVariable
		End Get
		Set(ByVal Value As Single)
			UnitAverageOperatingExpenses_MemberVariable = Value
		End Set
	End Property ' (OXB) = 7
	Dim UnitAverageExplorationCapital_MemberVariable As Single
	Public Property UnitAverageExplorationCapital() As Single Implements _IDGiantRunSummaryA.UnitAverageExplorationCapital
		Get
			UnitAverageExplorationCapital = UnitAverageExplorationCapital_MemberVariable
		End Get
		Set(ByVal Value As Single)
			UnitAverageExplorationCapital_MemberVariable = Value
		End Set
	End Property ' (EXB) = 8
	Dim UnitAverageDevelopmentCapital_MemberVariable As Single
	Public Property UnitAverageDevelopmentCapital() As Single Implements _IDGiantRunSummaryA.UnitAverageDevelopmentCapital
		Get
			UnitAverageDevelopmentCapital = UnitAverageDevelopmentCapital_MemberVariable
		End Get
		Set(ByVal Value As Single)
			UnitAverageDevelopmentCapital_MemberVariable = Value
		End Set
	End Property ' (DVB) = 9
	Dim UnitAverageOtherCapital_MemberVariable As Single
	Public Property UnitAverageOtherCapital() As Single Implements _IDGiantRunSummaryA.UnitAverageOtherCapital
		Get
			UnitAverageOtherCapital = UnitAverageOtherCapital_MemberVariable
		End Get
		Set(ByVal Value As Single)
			UnitAverageOtherCapital_MemberVariable = Value
		End Set
	End Property ' (OTHB) = 10
	Dim ProductionLife_MemberVariable As Single
	Public Property ProductionLife() As Single Implements _IDGiantRunSummaryA.ProductionLife
		Get
			ProductionLife = ProductionLife_MemberVariable
		End Get
		Set(ByVal Value As Single)
			ProductionLife_MemberVariable = Value
		End Set
	End Property ' (LFITemp) = 11
	Dim ProjectLife_MemberVariable As Single
	Public Property ProjectLife() As Single Implements _IDGiantRunSummaryA.ProjectLife
		Get
			ProjectLife = ProjectLife_MemberVariable
		End Get
		Set(ByVal Value As Single)
			ProjectLife_MemberVariable = Value
		End Set
	End Property ' (ZN) = 12
	'Public CompanyRateOfReturn As Single            ' (RR) = 13
	'Public GovernmentRateOfReturn As Single         ' (RRB) = 14
	
	' 3 Dec 2004 JWD (C0846) Add new R/W properties for new RateOfReturns
	Dim ThirdPartyRateOfReturn_MemberVariable As Single
	Public Property ThirdPartyRateOfReturn() As Single Implements _IDGiantRunSummaryA.ThirdPartyRateOfReturn
		Get
			ThirdPartyRateOfReturn = ThirdPartyRateOfReturn_MemberVariable
		End Get
		Set(ByVal Value As Single)
			ThirdPartyRateOfReturn_MemberVariable = Value
		End Set
	End Property ' RRT = ??
	Dim NOCRateOfReturn_MemberVariable As Single
	Public Property NOCRateOfReturn() As Single Implements _IDGiantRunSummaryA.NOCRateOfReturn
		Get
			NOCRateOfReturn = NOCRateOfReturn_MemberVariable
		End Get
		Set(ByVal Value As Single)
			NOCRateOfReturn_MemberVariable = Value
		End Set
	End Property ' RRN = ??
	
	
	' Following read-only
	
	Public ReadOnly Property CompanyRateOfReturn() As Single Implements _IDGiantRunSummaryA.CompanyRateOfReturn
		Get
		End Get
	End Property
	
	Public ReadOnly Property GovernmentRateOfReturn() As Single Implements _IDGiantRunSummaryA.GovernmentRateOfReturn
		Get
		End Get
	End Property
	
	Public ReadOnly Property RunCurrency() As String Implements _IDGiantRunSummaryA.RunCurrency
		Get
		End Get
	End Property
	
	'
	' Following are read-only Indicators of cash flows discounted at rate 1
	'
	Public ReadOnly Property CompanyIncomeDCF1() As Single Implements _IDGiantRunSummaryA.CompanyIncomeDCF1
		Get
		End Get
	End Property
	
	Public ReadOnly Property CompanyOperatingExpenseDCF1() As Single Implements _IDGiantRunSummaryA.CompanyOperatingExpenseDCF1
		Get
		End Get
	End Property
	
	Public ReadOnly Property CompanyCapitalExpenditureDCF1() As Single Implements _IDGiantRunSummaryA.CompanyCapitalExpenditureDCF1
		Get
		End Get
	End Property
	
	Public ReadOnly Property CompanyRoyaltyTaxDCF1() As Single Implements _IDGiantRunSummaryA.CompanyRoyaltyTaxDCF1
		Get
		End Get
	End Property
	
	Public ReadOnly Property CompanyNetCashFlowDCF1() As Single Implements _IDGiantRunSummaryA.CompanyNetCashFlowDCF1
		Get
		End Get
	End Property
	
	Public ReadOnly Property CompanyPayoutDCF1() As Single Implements _IDGiantRunSummaryA.CompanyPayoutDCF1
		Get
		End Get
	End Property
	
	Public ReadOnly Property CompanyRiskReturnRatioDCF1() As Single Implements _IDGiantRunSummaryA.CompanyRiskReturnRatioDCF1
		Get
		End Get
	End Property
	
	Public ReadOnly Property CompanyProfitabilityIndexDCF1() As Single Implements _IDGiantRunSummaryA.CompanyProfitabilityIndexDCF1
		Get
		End Get
	End Property
	
	Public ReadOnly Property CompanyGovernmentTakeDCF1() As Single Implements _IDGiantRunSummaryA.CompanyGovernmentTakeDCF1
		Get
		End Get
	End Property
	
	'
	' Following are read-only Indicators of cash flows discounted at rate 5
	'
	Public ReadOnly Property CompanyIncomeDCF5() As Single Implements _IDGiantRunSummaryA.CompanyIncomeDCF5
		Get
		End Get
	End Property
	
	Public ReadOnly Property CompanyOperatingExpenseDCF5() As Single Implements _IDGiantRunSummaryA.CompanyOperatingExpenseDCF5
		Get
		End Get
	End Property
	
	Public ReadOnly Property CompanyCapitalExpenditureDCF5() As Single Implements _IDGiantRunSummaryA.CompanyCapitalExpenditureDCF5
		Get
		End Get
	End Property
	
	Public ReadOnly Property CompanyRoyaltyTaxDCF5() As Single Implements _IDGiantRunSummaryA.CompanyRoyaltyTaxDCF5
		Get
		End Get
	End Property
	
	Public ReadOnly Property CompanyNetCashFlowDCF5() As Single Implements _IDGiantRunSummaryA.CompanyNetCashFlowDCF5
		Get
		End Get
	End Property
	
	Public ReadOnly Property CompanyPayoutDCF5() As Single Implements _IDGiantRunSummaryA.CompanyPayoutDCF5
		Get
		End Get
	End Property
	
	Public ReadOnly Property CompanyRiskReturnRatioDCF5() As Single Implements _IDGiantRunSummaryA.CompanyRiskReturnRatioDCF5
		Get
		End Get
	End Property
	
	Public ReadOnly Property CompanyProfitabilityIndexDCF5() As Single Implements _IDGiantRunSummaryA.CompanyProfitabilityIndexDCF5
		Get
		End Get
	End Property
	
	Public ReadOnly Property CompanyGovernmentTakeDCF5() As Single Implements _IDGiantRunSummaryA.CompanyGovernmentTakeDCF5
		Get
		End Get
	End Property
End Class