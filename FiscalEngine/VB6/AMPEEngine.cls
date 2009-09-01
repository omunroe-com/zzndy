VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "AMPEEngine"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
' Name:         AMPEEngine.cls
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
Option Explicit

Implements ASPEEngineEx     ' 26 May 2005 JWD (C0880)

Implements IMainexec

Private zzz_ReportText As CReportText

Private zzz_error_text As String

Private zzz_InitAccum As Long

'
' Return description of latest error.
'
Public Property Get ErrorDescription()

    ErrorDescription = zzz_error_text

End Property

'
' Return a reference to the IMainexec interface
'
Public Property Get AsIMainexec _
    () As IMainexec
    
    Set AsIMainexec = Me
    
End Property

'''
''' 26 May 2005 JWD New (C0880)
'''
''' Return a reference to the ASPEEngineEx interface
'''
''Public Property Get AsASPEEngineEx _
''    () As ASPEEngineEx
''
''    Set AsASPEEngineEx = Me
''
''End Property



Private Sub Class_Initialize()

    On Error Resume Next
   
    Set zzz_ReportText = New CReportText
   
    If Err.number <> 0 Then
        zzzSaveErrorDescription
        Err.Clear
    End If

    zzz_InitAccum = RunSwitches_InitAccumYes
    
End Sub

'
' Perform procedure cleanup
'
Private Sub zzzCleanup()
    
    Dim sTxt As String
    
    ' Exhaust any pending Dir iteration. Failure to do so
    ' prevents removal of the temporary folder when done.
    sTxt = Dir(FPStatus$)
    Do While Len(sTxt) > 0
        sTxt = Dir
    Loop

End Sub


Private Sub zzzSaveErrorDescription()

    Dim lTemp As Long
    
    lTemp = Err.number
    If lTemp < 0 Then
        lTemp = lTemp - vbObjectError
    End If
    
    zzz_error_text = "Error " & Format(lTemp, "#") & "; " & Err.Source & "; " & Err.Description & "."
   
End Sub

'
' 26 May 2005 JWD New (C0880)
'
'=========================================================
'
' ASPEEngineEx Interface
'
'

Private Function ASPEEngineEx_CalculateConsolidatedEconomics _
    ( _
    ByVal DiscountDate As String, _
    ByRef DiscountRates() As Double, _
    ByRef CalcSettings() As Integer, _
    ByRef OutputAmounts() As Double, _
    ByRef OutputNames() As String, _
    ByRef OutputInterests() As Double, _
    ByRef EconomicIndicators() As Double, _
    ByRef OtherIndicators() As Double, _
    ByRef CalcIndicators() As Integer, _
    ByRef ConsolidatedDates() As String, _
    ByRef ErrorDescription As String, _
    ByVal DumpFileName As String _
    ) As Boolean

    Dim bResult As Boolean

    Dim BaseAmounts() As Double
    Dim BaseNames() As String
    Dim CapexAmounts() As Double
    Dim CapexWI() As Double
    Dim CapexRP() As Double
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
    Dim run_switches() As Long
    
    Dim CountryFileSensitivities() As String
    
    zzz_error_text = vbNullString


'''    ' 15 Oct 2003 JWD (C0764) Add license check, exit on fail
'''    If g_objLicense Is Nothing Then
'''        zzz_error_text = "Unable to acquire license. Ensure that security device is properly installed."
'''        CalculateEconomics = False
'''        Exit Function
'''    End If
'''    ' End (C0764)

    On Error Resume Next

'''    ' 19 Sep 2003 JWD (C0745) Add dump of input data for analysis
'''    If Len(DumpFileName) > 0 Then
'''        DumpInputData DumpFileName, BaseAmounts(), BaseNames(), CapexAmounts(), CapexWI(), CapexRP(), CapexNames(), CapexTangible(), AbandonmentInflation(), DiscountRates(), ProjectDates(), MiscellaneousItems(), Codes(), CountryFileName
'''    End If

    ReDim ProjectDates(1 To 4)
    ProjectDates(4) = DiscountDate
    
    ReDim run_switches(1 To 2)
    run_switches(RunSwitches_RunType) = RunSwitches_RunTypeConsol
    run_switches(RunSwitches_InitAccum) = RunSwitches_InitAccumNo
    
    Set g_oReport = New CGiantReport1
    Set g_oReport.ReportText = zzz_ReportText
    Set g_oFileSystem = New CXFileSystemMem1

    bResult = FiscalCalculatorEx( _
                                BaseAmounts(), BaseNames(), CapexAmounts(), _
                                CapexWI(), CapexRP(), CapexNames(), _
                                CapexTangible(), Inflation(), DiscountRates(), _
                                ProjectDates(), MiscellaneousItems(), _
                                LoansData(), BaseAmountsExtraIn(), CapexPARIn(), _
                                ModelVarsIn(), ModelVarsAmountsIn(), ModelVarsToReturn(), _
                                CountryFileName, CountryFileSensitivities(), _
                                CalcSettings(), _
                                OutputAmounts(), OutputNames(), OutputInterests(), _
                                EconomicIndicators(), OtherIndicators(), BaseAmountsExtraOut(), _
                                CapexPAROut(), ModelVarsOut(), ModelVarsAmountsOut(), _
                                CalcIndicators(), zzz_ReportText, run_switches() _
                                )

    Set g_oReport = Nothing

    ASPEEngineEx_CalculateConsolidatedEconomics = bResult

    If Err.number <> 0 Then
        zzzSaveErrorDescription
        ErrorDescription = zzz_error_text
    End If

End Function


Private Function ASPEEngineEx_CalculateEconomics _
    ( _
    ByRef BaseAmounts() As Double, ByRef BaseNames() As String, ByRef CapexAmounts() As Double, _
    ByRef CapexWI() As Double, ByRef CapexRP() As Double, ByRef CapexNames() As String, _
    ByRef CapexTangible() As Double, ByRef Inflation() As Double, ByRef DiscountRates() As Double, _
    ByRef ProjectDates() As String, ByRef MiscellaneousItems() As Double, ByRef LoansData() As Double, _
    ByRef BaseAmountsExtraIn() As Double, ByRef CapexPARIn() As Double, ByRef ModelVarsIn() As String, _
    ByRef ModelVarsAmountsIn() As Double, ByRef ModelVarsToReturn() As String, ByVal CountryFileName As String, _
    ByRef CountryFileSensitivities() As String, ByRef CalcSettings() As Integer, ByVal DumpFileName As String, _
    ByRef OutputAmounts() As Double, ByRef OutputNames() As String, ByRef OutputInterests() As Double, _
    ByRef EconomicIndicators() As Double, ByRef OtherIndicators() As Double, ByRef BaseAmountsExtraOut() As Double, _
    ByRef CapexPAROut() As Double, ByRef ModelVarsOut() As String, ByRef ModelVarsAmountsOut() As Double, _
    ByRef CalcIndicators() As Integer, ByRef ErrorDescription As String _
    ) As Boolean

    Dim bResult As Boolean
    Dim run_switches() As Long
    
    ReDim run_switches(1 To 2)
    run_switches(RunSwitches_RunType) = RunSwitches_RunTypeStd
    run_switches(RunSwitches_InitAccum) = zzz_InitAccum
    zzz_InitAccum = RunSwitches_InitAccumNo
    
    zzz_error_text = vbNullString


'''    ' 15 Oct 2003 JWD (C0764) Add license check, exit on fail
'''    If g_objLicense Is Nothing Then
'''        zzz_error_text = "Unable to acquire license. Ensure that security device is properly installed."
'''        CalculateEconomics = False
'''        Exit Function
'''    End If
'''    ' End (C0764)

    On Error Resume Next

'''    ' 19 Sep 2003 JWD (C0745) Add dump of input data for analysis
'''    If Len(DumpFileName) > 0 Then
'''        DumpInputData DumpFileName, BaseAmounts(), BaseNames(), CapexAmounts(), CapexWI(), CapexRP(), CapexNames(), CapexTangible(), AbandonmentInflation(), DiscountRates(), ProjectDates(), MiscellaneousItems(), Codes(), CountryFileName
'''    End If

    Set g_oReport = New CGiantReport1
    Set g_oReport.ReportText = zzz_ReportText
    Set g_oFileSystem = New CXFileSystemMem1

    'bResult = FiscalCalculator(BaseAmounts(), BaseNames(), CapexAmounts(), CapexWI(), CapexRP(), CapexNames(), CapexTangible(), AbandonmentInflation(), DiscountRates(), ProjectDates(), MiscellaneousItems(), Codes(), CountryFileName, OutputAmounts(), OutputNames(), OutputInterests(), EconomicIndicators(), CompanyROR, GovernmentROR, EconomicLimitApplied, EconomicLife, zzz_ReportText, OtherIndicators())
    bResult = FiscalCalculatorEx( _
                                BaseAmounts(), BaseNames(), CapexAmounts(), _
                                CapexWI(), CapexRP(), CapexNames(), _
                                CapexTangible(), Inflation(), DiscountRates(), _
                                ProjectDates(), MiscellaneousItems(), _
                                LoansData(), BaseAmountsExtraIn(), CapexPARIn(), _
                                ModelVarsIn(), ModelVarsAmountsIn(), ModelVarsToReturn(), _
                                CountryFileName, CountryFileSensitivities(), _
                                CalcSettings(), _
                                OutputAmounts(), OutputNames(), OutputInterests(), _
                                EconomicIndicators(), OtherIndicators(), BaseAmountsExtraOut(), _
                                CapexPAROut(), ModelVarsOut(), ModelVarsAmountsOut(), _
                                CalcIndicators(), zzz_ReportText, run_switches() _
                                )

    Set g_oReport = Nothing

    ASPEEngineEx_CalculateEconomics = bResult

    If Err.number <> 0 Then
        zzzSaveErrorDescription
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
Private Function IMainexec_CalculateEconomics _
    ( _
    ByVal CommandParameter As String _
    ) As Boolean
    
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
    ReDim REIM(0 To gc_nMAXLIFE)
    ReDim OPEX(0 To gc_nMAXLIFE)
    UseGrossProductionAmounts = False
    g_bPTCons = False
    
    g_nFinanceEvents = 0                        ' 22 Apr 2005 JWD (C0874)
    
    InitializeAbandonmentExpenditureData
    
    Set g_oReport = New CGiantReport1
    Set g_oReport.ReportText = zzz_ReportText
    
    ' Using disk files (mostly)
    Set g_oFileSystem = New CXFileSystemDisk1
    
    Mainexec CommandParameter
    
    If Err.number <> 0 Then
       zzzSaveErrorDescription
       Err.Clear
    End If
    
    Set g_oReport = Nothing

    IMainexec_CalculateEconomics = (Err.number = 0)

    g_bIsMainexecRun = False
    
    zzzCleanup
    
End Function

Private Property Get IMainexec_ErrorDescription _
    () As String
    IMainexec_ErrorDescription = zzz_error_text
End Property

