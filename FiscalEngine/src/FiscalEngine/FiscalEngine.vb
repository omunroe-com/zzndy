Option Strict Off
Option Explicit On
<System.Runtime.InteropServices.ProgId("AMPEEngine_NET.FiscalEngine")> Public Class FiscalEngine
	Implements ASPEEngineExTypeLib.ASPEEngineEx
	Implements MainexecTypeLib.IMainexec
	' Name:         FiscalEngine.cls
	' Function:     AS$ET Fiscal Model Engine
	' Date:         10 Jan 2005 JWD
	'---------------------------------------------------------
	' This is the fiscal model engine definition for AS$ET.
	'---------------------------------------------------------
	' Modifications:
	' 22 Apr 2005 JWD
	'  -> Changed IMainexec_CalculateEconomics(). (C0874)
	'
	' 26 May 2005 JWD
	'  -> Added ASPEEngineEx interface implementation. (C0880)
	'---------------------------------------------------------
	 ' 26 May 2005 JWD (C0880)
	
	
    Private zzz_ReportText As CReportText
	
	Private zzz_error_text As String
	
	Private zzz_InitAccum As Integer
	
	'
	' Return description of latest error.
	'
	Public ReadOnly Property ErrorDescription() As Object
		Get
			
			'UPGRADE_WARNING: Couldn't resolve default property of object ErrorDescription. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			ErrorDescription = zzz_error_text
			
		End Get
	End Property
	
	'
	' Return a reference to the IMainexec interface
	'
	Public ReadOnly Property AsIMainexec() As MainexecTypeLib.IMainexec
		Get
			
			AsIMainexec = Me
			
		End Get
	End Property
	
	Private ReadOnly Property IMainexec_ErrorDescription() As String Implements MainexecTypeLib.IMainexec.ErrorDescription
		Get
			IMainexec_ErrorDescription = zzz_error_text
		End Get
	End Property
	
    ''
    '' 26 May 2005 JWD New (C0880)
    ''
    '' Return a reference to the ASPEEngineEx interface
    ''
	''Public Property Get AsASPEEngineEx _
    ''    () As ASPEEngineEx
    ''
    ''    Set AsASPEEngineEx = Me
    ''
    ''End Property



    'UPGRADE_NOTE: Class_Initialize was upgraded to Class_Initialize_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    Private Sub Class_Initialize_Renamed()

        On Error Resume Next

        zzz_ReportText = New CReportText

        If Err.Number <> 0 Then
            zzzSaveErrorDescription()
            Err.Clear()
        End If

        zzz_InitAccum = RunSwitches_InitAccumYes

    End Sub
    Public Sub New()
        MyBase.New()
        Class_Initialize_Renamed()
    End Sub

    '
    ' Perform procedure cleanup
    '
    Private Sub zzzCleanup()

        Dim sTxt As String

        ' Exhaust any pending Dir iteration. Failure to do so
        ' prevents removal of the temporary folder when done.
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        sTxt = Dir(FPStatus)
        Do While Len(sTxt) > 0
            'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
            sTxt = Dir()
        Loop

    End Sub


    Private Sub zzzSaveErrorDescription()

        Dim lTemp As Integer

        lTemp = Err.Number
        If lTemp < 0 Then
            lTemp = lTemp - vbObjectError
        End If

        zzz_error_text = "Error " & VB6.Format(lTemp, "#") & "; " & Err.Source & "; " & Err.Description & "."

    End Sub

    '
    ' 26 May 2005 JWD New (C0880)
    '
    '=========================================================
    '
    ' ASPEEngineEx Interface
    '
    '

    Private Function ASPEEngineEx_CalculateConsolidatedEconomics(ByVal DiscountDate As String, ByRef DiscountRates As System.Array, ByRef CalcSettings As System.Array, ByRef OutputAmounts As System.Array, ByRef OutputNames As System.Array, ByRef OutputInterests As System.Array, ByRef EconomicIndicators As System.Array, ByRef OtherIndicators As System.Array, ByRef CalcIndicators As System.Array, ByRef ConsolidatedDates As System.Array, ByRef ErrorDescription As String, ByVal DumpFileName As String) As Boolean Implements ASPEEngineExTypeLib.ASPEEngineEx.CalculateConsolidatedEconomics

        Dim bResult As Boolean

        Dim BaseAmounts(,) As Double
        Dim BaseNames() As String
        Dim CapexAmounts(,) As Double
        Dim CapexWI(,) As Double
        Dim CapexRP(,) As Double
        Dim CapexTangible() As Double
        Dim CapexNames() As String
        Dim Inflation() As Double
        Dim ProjectDates() As String
        Dim MiscellaneousItems() As Double
        Dim LoansData() As Double
        Dim BaseAmountsExtraIn() As Double
        Dim BaseAmountsExtraOut() As Double
        Dim ModelVarsIn() As String
        Dim ModelVarsOut() As String
        Dim ModelVarsAmountsIn() As Double
        Dim ModelVarsAmountsOut() As Double
        Dim ModelVarsToReturn() As String
        Dim CapexPARIn() As Double
        Dim CapexPAROut() As Double
        Dim CountryFileName As String
        Dim run_switches() As Integer

        Dim CountryFileSensitivities(,) As String

        zzz_error_text = vbNullString


        ''     15 Oct 2003 JWD (C0764) Add license check, exit on fail
        ''    If g_objLicense Is Nothing Then
        ''        zzz_error_text = "Unable to acquire license. Ensure that security device is properly installed."
        ''        CalculateEconomics = False
        ''        Exit Function
        ''    End If
        ''    ' End (C0764)

        On Error Resume Next

        ''    ' 19 Sep 2003 JWD (C0745) Add dump of input data for analysis
        ''    If Len(DumpFileName) > 0 Then
        ''        DumpInputData DumpFileName, BaseAmounts(), BaseNames(), CapexAmounts(), CapexWI(), CapexRP(), CapexNames(), CapexTangible(), AbandonmentInflation(), DiscountRates(), ProjectDates(), MiscellaneousItems(), Codes(), CountryFileName
        ''    End If

        'UPGRADE_WARNING: Lower bound of array ProjectDates was changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim ProjectDates(4)
        ProjectDates(4) = DiscountDate

        'UPGRADE_WARNING: Lower bound of array run_switches was changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim run_switches(2)
        run_switches(RunSwitches_RunType) = RunSwitches_RunTypeConsol
        run_switches(RunSwitches_InitAccum) = RunSwitches_InitAccumNo

        g_oReport = New CGiantReport1
        g_oReport.ReportText = zzz_ReportText
        g_oFileSystem = New CXFileSystemMem1

        bResult = FiscalCalculatorEx(BaseAmounts, BaseNames, CapexAmounts, CapexWI, CapexRP, CapexNames, CapexTangible, Inflation, DiscountRates, ProjectDates, MiscellaneousItems, LoansData, BaseAmountsExtraIn, CapexPARIn, ModelVarsIn, ModelVarsAmountsIn, ModelVarsToReturn, CountryFileName, CountryFileSensitivities, CalcSettings, OutputAmounts, OutputNames, OutputInterests, EconomicIndicators, OtherIndicators, BaseAmountsExtraOut, CapexPAROut, ModelVarsOut, ModelVarsAmountsOut, CalcIndicators, zzz_ReportText, run_switches)

        'UPGRADE_NOTE: Object g_oReport may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        g_oReport = Nothing

        ASPEEngineEx_CalculateConsolidatedEconomics = bResult

        If Err.Number <> 0 Then
            zzzSaveErrorDescription()
            ErrorDescription = zzz_error_text
        End If

    End Function


    Private Function ASPEEngineEx_CalculateEconomics(ByRef BaseAmounts As System.Array, ByRef BaseNames As System.Array, ByRef CapexAmounts As System.Array, ByRef CapexWI As System.Array, ByRef CapexRP As System.Array, ByRef CapexNames As System.Array, ByRef CapexTangible As System.Array, ByRef Inflation As System.Array, ByRef DiscountRates As System.Array, ByRef ProjectDates As System.Array, ByRef MiscellaneousItems As System.Array, ByRef LoansData As System.Array, ByRef BaseAmountsExtraIn As System.Array, ByRef CapexPARIn As System.Array, ByRef ModelVarsIn As System.Array, ByRef ModelVarsAmountsIn As System.Array, ByRef ModelVarsToReturn As System.Array, ByVal CountryFileName As String, ByRef CountryFileSensitivities As System.Array, ByRef CalcSettings As System.Array, ByVal DumpFileName As String, ByRef OutputAmounts As System.Array, ByRef OutputNames As System.Array, ByRef OutputInterests As System.Array, ByRef EconomicIndicators As System.Array, ByRef OtherIndicators As System.Array, ByRef BaseAmountsExtraOut As System.Array, ByRef CapexPAROut As System.Array, ByRef ModelVarsOut As System.Array, ByRef ModelVarsAmountsOut As System.Array, ByRef CalcIndicators As System.Array, ByRef ErrorDescription As String) As Boolean Implements ASPEEngineExTypeLib.ASPEEngineEx.CalculateEconomics

        Dim bResult As Boolean
        Dim run_switches() As Integer

        'UPGRADE_WARNING: Lower bound of array run_switches was changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim run_switches(2)
        run_switches(RunSwitches_RunType) = RunSwitches_RunTypeStd
        run_switches(RunSwitches_InitAccum) = zzz_InitAccum
        zzz_InitAccum = RunSwitches_InitAccumNo

        zzz_error_text = vbNullString


        ''    ' 15 Oct 2003 JWD (C0764) Add license check, exit on fail
        ''    If g_objLicense Is Nothing Then
        ''        zzz_error_text = "Unable to acquire license. Ensure that security device is properly installed."
        ''        CalculateEconomics = False
        ''        Exit Function
        ''    End If
        ''    ' End (C0764)

        On Error Resume Next

        ''    ' 19 Sep 2003 JWD (C0745) Add dump of input data for analysis
        ''    If Len(DumpFileName) > 0 Then
        ''        DumpInputData DumpFileName, BaseAmounts(), BaseNames(), CapexAmounts(), CapexWI(), CapexRP(), CapexNames(), CapexTangible(), AbandonmentInflation(), DiscountRates(), ProjectDates(), MiscellaneousItems(), Codes(), CountryFileName
        ''    End If

        g_oReport = New CGiantReport1
        g_oReport.ReportText = zzz_ReportText
        g_oFileSystem = New CXFileSystemMem1

        'bResult = FiscalCalculator(BaseAmounts(), BaseNames(), CapexAmounts(), CapexWI(), CapexRP(), CapexNames(), CapexTangible(), AbandonmentInflation(), DiscountRates(), ProjectDates(), MiscellaneousItems(), Codes(), CountryFileName, OutputAmounts(), OutputNames(), OutputInterests(), EconomicIndicators(), CompanyROR, GovernmentROR, EconomicLimitApplied, EconomicLife, zzz_ReportText, OtherIndicators())
        bResult = FiscalCalculatorEx(BaseAmounts, BaseNames, CapexAmounts, CapexWI, CapexRP, CapexNames, CapexTangible, Inflation, DiscountRates, ProjectDates, MiscellaneousItems, LoansData, BaseAmountsExtraIn, CapexPARIn, ModelVarsIn, ModelVarsAmountsIn, ModelVarsToReturn, CountryFileName, CountryFileSensitivities, CalcSettings, OutputAmounts, OutputNames, OutputInterests, EconomicIndicators, OtherIndicators, BaseAmountsExtraOut, CapexPAROut, ModelVarsOut, ModelVarsAmountsOut, CalcIndicators, zzz_ReportText, run_switches)

        'UPGRADE_NOTE: Object g_oReport may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        g_oReport = Nothing

        ASPEEngineEx_CalculateEconomics = bResult

        If Err.Number <> 0 Then
            zzzSaveErrorDescription()
            ErrorDescription = zzz_error_text
        End If

    End Function
	
	
	'=========================================================
	'
	' IMainexec Interface
	'
	
	'
	' Modifications:
	' 22 Apr 2005
	'  -> Add initialization of g_nFinanceEvents(). (C0874)
	'
	Private Function IMainexec_CalculateEconomics(ByVal CommandParameter As String) As Boolean Implements MainexecTypeLib.IMainexec.CalculateEconomics
		
		On Error Resume Next
		
		' Set flag to indicate IMainexec interface is how
		' engine object is being run. This controls certain
		' code execution in called procedures.
		g_bIsMainexecRun = True
		
		' Initialization
		' These are variables that are not otherwise
		' initialized between runs that depend on the
		' default initialization at MAINEXEC startup
		LG = 0
		LFX = 0
		LFI = 0
		LGI = 0
		
		RNU = 0
		FinalWin = 0
		FINALPARTIC = 0
		SIG = 0
		DIS = 0
		BURS = 0
		ReDim REIM(gc_nMAXLIFE)
		ReDim OPEX(gc_nMAXLIFE)
		UseGrossProductionAmounts = False
		g_bPTCons = False
		
		g_nFinanceEvents = 0 ' 22 Apr 2005 JWD (C0874)
		
		InitializeAbandonmentExpenditureData()
		
		g_oReport = New CGiantReport1
		g_oReport.ReportText = zzz_ReportText
		
		' Using disk files (mostly)
		g_oFileSystem = New CXFileSystemDisk1
		
		Mainexec(CommandParameter)
		
		If Err.Number <> 0 Then
			zzzSaveErrorDescription()
			Err.Clear()
		End If
		
		'UPGRADE_NOTE: Object g_oReport may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		g_oReport = Nothing
		
		IMainexec_CalculateEconomics = (Err.Number = 0)
		
		g_bIsMainexecRun = False
		
		zzzCleanup()
		
	End Function
End Class