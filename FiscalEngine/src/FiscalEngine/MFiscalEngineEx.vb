Option Strict Off
Option Explicit On
Module MFiscalEngineEx
	' Name:        MFiscalEngineEx.bas
	' Function:    Fiscal Engine Driver Module
	' Date:        26 May 2005 JWD
	'---------------------------------------------------------
	' Developed for Discoveries Databank project. Derived from
	' MFiscalEngine.bas (rev 015) as used in ASPEEngine.dll
	' (rev 018).
	'---------------------------------------------------------
	' 26 May 2005 JWD New
	'---------------------------------------------------------
	' Modifications:
	' 10 Sep 2003 JWD
	'  -> Changed FiscalCalculator(). (C0744)
	'
	' 17 Sep 2003 JWD
	'  -> Changed FiscalCalculator(). (C0746)
	'
	' 19 Sep 2003 JWD
	'  -> Changed FiscalCalculator(). (C0748)
	'
	' 23 Sep 2003 JWD
	'  -> Changed FiscalCalculator(). (C0749)
	'
	' 30 Sep 2003 JWD
	'  -> Changed FiscalCalculator(). (C0754)
	'  -> Changed zzzCheckInputForecastsLife(). (C0754)
	'  -> Changed zzzCheckInputForecastsLife(). (C0755)
	'
	' 7 Oct 2003 JWD
	'  -> Changed value assigned to pRateCode_RTO to base
	'     on public symbol gc_nRtPrm_RTO. (C0756)
	'  -> Changed FiscalCalculator(). (C0757)
	'  -> Changed zzzDetermineOutputArraySize(). (C0758)
	'  -> Changed FiscalCalculator(). (C0759)
	'
	' 23 Sep 2004 JWD
	'  -> Changed FiscalCalculator(). (C0839)
	'
	' 2 Aug 2005 JWD
	'  -> Changed FiscalCalculatorEx(). (C0887)
	'  -> Changed FiscalCalculatorEx(). (C0888)
	'
	' 21 Aug 2006 JWD
	'  -> Added IsAllocatedArrayStr(). (C0909)
	'  -> Changed FiscalCalculatorEX(). (C0909)
	'
	' 24 Aug 2006 JWD
	'  -> Changed FiscalCalculatorEX(). (C0911)
	'---------------------------------------------------------
	
	Public Const RunSwitches_RunType As Short = 1 ' type of run to do
	Public Const RunSwitches_RunTypeStd As Short = 1 ' type of run is standard project
	Public Const RunSwitches_RunTypeConsol As Short = 2 ' type of run is post-tax consol
	
	Public Const RunSwitches_InitAccum As Short = 2 ' whether or not to initialize AC, CC, L1
	Public Const RunSwitches_InitAccumNo As Short = 0 ' no, do not initialize
	Public Const RunSwitches_InitAccumYes As Short = 1 ' yes, initialize
	
	
	' The following are constant values referenced in this
	' module that are picked out from other places in the
	' system and that typically are numeric literals
	
	Const MY3_Use_AWIN As Short = -998 ' This is the old WIN constant code
	
	' 7 Oct 2003 JWD (C0756) Change to replace literal with symbol
	'Const pRateCode_RTO = 58         ' This is the CURRENT value that indicates
	Const pRateCode_RTO As Short = gc_nRtPrmRTO ' This is the CURRENT value that indicates
	' that RateCalc() should use the RTO method
	' It is subject to change if the list of codes
	' changes (as happened with the addition of
	' new volume streams)
	
	Const pRateCode_UserBase As Short = 100 ' This is the CURRENT value that is added to
	' the position of the user variable in fiscal
	' definition to identify those rate parameters
	' that are user variables rather than those
	' that are pre-defined.
	
	
	
	'
	' Modifications:
	' 10 Sep 2003 JWD
	'  -> Add procedure calls to assign abandonment items
	'     to Abandonment module from inputs. (C0744)
	'
	' 17 Sep 2003 JWD
	'  -> Add initialization of variable symbols for working
	'     interest, participation, signature and discovery
	'     bonuses. (C0746)
	'
	' 19 Sep 2003 JWD
	'  -> Add initialization of price inflations array
	'     (Inflate). (C0748)
	'
	' 23 Sep 2003 JWD
	'  -> Add initialization of variables BURS and REIM().
	'     (C0749)
	'
	' 30 Sep 2003 JWD
	'  -> Add LFX to zzzCheckInputForecastsLife() parameter
	'     list. (C0754)
	'  -> Add calculation of LGI, LFI. (C0754)
	'
	' 7 Oct 2003 JWD
	'  -> Change statements assigning BaseAmounts() to A() to
	'     convert to intermediate text representation of
	'     values. This is to permit exact matching of results
	'     with MAINEXEC and implements effects of conversion
	'     of data to text as when data is written/read from
	'     file. (C0757)
	'  -> Add statements performing country file excel link
	'     initialization. These had been omitted. (C0759)
	'
	' 23 Sep 2004 JWD
	'  -> Add RunSummary() array to parameter list. (C0839)
	'  -> Add assignment of run summary (other indicators)
	'     data to RunSummary(). (C0839)
	'
	' 2 Aug 2005 JWD
	'  -> Change to remove adjustment of LG for abandonment
	'     timing. This is done already in MAbandonment.bas:
	'     ApplyAbandonmentFundingProvisions(). (C0887)
	'  -> Change to use gc_nMAXLIFE as time dimension upper
	'     array bound instead of LG in allocating A() array.
	'     (C0888)
	'
	' 21 Aug 2006 JWD
	'  -> Add copy of country file sensitivities to the run
	'     file. (C0909)
	'
	' 24 Aug 2006 JWD
	'  -> Correct UBound() usage in loop copying country file
	'     sensitivities to run commands, add omitted dimension
	'     parameter. (C0911)
	'  -> Add condition of loop on proper dimension 1 (column)
	'     values. (C0911)
	'  -> Change lower bound of loop to 1 (from Lbound())
	'     array should have LBound of 1. LBound value of zero
	'     is to be accepted as an empty (no entries) array.
	'     (C0911)
	'
    Public Function FiscalCalculatorEx(ByRef BaseAmounts(,) As Double, ByRef BaseNames() As String, ByRef CapexAmounts(,) As Double, ByRef CapexWI(,) As Double, ByRef CapexRP(,) As Double, ByRef CapexNames() As String, ByRef CapexTangible() As Double, ByRef Inflation() As Double, ByRef DiscountRates() As Double, ByRef ProjectDates() As String, ByRef MiscellaneousItems() As Double, ByRef LoansData() As Double, ByRef BaseAmountsExtraIn() As Double, ByRef CapexPARIn() As Double, ByRef ModelVarsIn() As String, ByRef ModelVarsAmountsIn() As Double, ByRef ModelVarsToReturn() As String, ByVal CountryFileName As String, ByRef CountryFileSensitivities(,) As String, ByRef CalcSettings() As Short, ByRef OutputAmounts() As Double, ByRef OutputNames() As String, ByRef OutputInterests() As Double, ByRef EconomicIndicators(,) As Double, ByRef OtherIndicators() As Double, ByRef BaseAmountsExtraOut() As Double, ByRef CapexPAROut() As Double, ByRef ModelVarsOut() As String, ByRef ModelVarsAmountsOut() As Double, ByRef CalcIndicators() As Short, ByRef OutputText As _CReportText, ByRef RunSwitches() As Integer) As Boolean

        On Error GoTo FiscalCalculator_Error

        Const settings_discount_method As Short = 1
        Const settings_field_type As Short = 2
        Const settings_allocation_method As Short = 3
        Const settings_abandonment_timing As Short = 4

        Dim i As Short
        Dim j As Short

        Dim iiiX As Short
        Dim iiiY As Short
        Dim ReportLevelSaved As String
        Dim RunNameSaved As String

        Dim sTemp As String

        Dim mainexecCompatibilityMode As Boolean
        ' If in compatibility mode, loading of arrays is
        ' done through a text representation state to mimic
        ' the effect of writing and reading the data to and
        ' from text data files. This is for testing purposes.

FiscalCalculator_DummyUp:


FiscalCalculator_Data:
        ' Here's were the real stuff begins

        ' Initialization for a run
FiscalCalculator_Initialize:

        ReDim xRunSwitches(RunSwitchesCount)
        xRunSwitches(RunSwitch_LVL) = RunSwitch_LVL_Company
        xRunSwitches(RunSwitch_PAR) = RunSwitch_PAR_On
        xRunSwitches(RunSwitch_WIN) = RunSwitch_WIN_On
        xRunSwitches(RunSwitch_FIN) = RunSwitch_FIN_On
        xRunSwitches(RunSwitch_LMT) = RunSwitch_LMT_On
        xRunSwitches(RunSwitch_DCF) = RunSwitch_DCF_On

        If (RunSwitches(RunSwitches_RunType) = RunSwitches_RunTypeStd) And (RunSwitches(RunSwitches_InitAccum) = RunSwitches_InitAccumYes) Then
            ' Re-initialize the consolidation accumulator data arrays
            ReDim L1(14) ' Stores dates & durations for consolidations
            L1(6) = 10000
            L1(2) = 10000
            L1(7) = 10000
            L1(8) = 0
            L1(12) = 10000
            L1(14) = 10000

            ' consolidation arrays - filled out in cashflow
            ReDim AC(gc_nMAXLIFE, gc_nACSIZED2)
            ReDim CC(gc_nMAXCAPEX, gc_nCCSIZED2)
            CCT = 0

        End If

        ReDim gn(11)

        ' Run file command line (current)
        'UPGRADE_WARNING: Lower bound of array RF was changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim RF(RUNFILECOLS)

        ' Default compatibility mode is to match Mainexec
        mainexecCompatibilityMode = True

        FinalWin = 0
        FINALPARTIC = 0
        SIG = 0
        DIS = 0

        'UPGRADE_WARNING: Lower bound of array Inflate was changed from 1,1 to 0,0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim Inflate(gc_nMAXLIFE, 2)

        BURS = 0
        ReDim REIM(gc_nMAXLIFE)

        SearchCodeString(CPXCategoryCodesString, "BAL", 3, CPXCategoryCodeBAL)
        SearchCodeString(CPXCategoryCodesString, "BL2", 3, CPXCategoryCodeBL2)
        SearchCodeString(CPXCategoryCodesString, "BL3", 3, CPXCategoryCodeBL3)
        ' Initialize the value of the last balance category code (C0878)
        SearchCodeString(CPXCategoryCodesString, CPXCategoryCodeString_BLn, 3, CPXCategoryCodeBLn) ' (C0878)
        SearchCodeString(CPXCategoryCodesString, CPXCategoryCodeString_AbandonmentCashExpenditure, 3, CPXCategoryCode_AbandonmentCashExpenditure)
        SearchCodeString(CPXCategoryCodesString, CPXCategoryCodeString_AbandonmentAccrualEntry, 3, CPXCategoryCode_AbandonmentAccrualEntry)

        ' Branch to initialization for standard project run ...
        If RunSwitches(RunSwitches_RunType) = RunSwitches_RunTypeStd Then
            GoTo FiscalCalculator_InitializeStandard
        End If

        ' ... or fall through to initialization for post-tax consolidation run

FiscalCalculator_InitializeConsol:

        ' Post-tax consolidation run, using accumulated
        ' data in consolidation data (AC(), CC())

        RF(1) = "CONSOL"
        RF(2) = "" ' 2
        RF(3) = "" ' 3
        RF(4) = "" ' 4
        RF(5) = "ALL" ' 5
        For i = 6 To RUNFILECOLS
            RF(i) = ""
        Next i

        ' Plug the supplied discount date into the L1() array
        ' Validate and convert date entry, store in L1() (global)
        zzzConvertDateEntry(ProjectDates(4), L1(10), L1(11))

        DiscMthd = CalcSettings(settings_discount_method)

        ' Assign the discount rates to use
        gn(4) = DiscountRates(1)
        gn(5) = DiscountRates(2)
        gn(6) = DiscountRates(3)
        gn(7) = DiscountRates(4)
        gn(8) = DiscountRates(5)
        gn(9) = DiscountRates(6)

        GoTo FiscalCalculator_CashFlow

FiscalCalculator_InitializeStandard:  ' This is the target of a goto, coming from FiscalCalculator_Initialize

        ' Create a dummy run file for use
        Dim l_oRunFileOut As IEFSFileSeqOut ' the 'open for output' interface

        ' Standard run, using supplied forecasts data
        ' Output the run commands to the file
        l_oRunFileOut = g_oFileSystem.OpenForOutput(vbNullString)
        With l_oRunFileOut
            ' Add the country file command information
            .NextItem = "GETCTRY"
            .NextItem = GetPathPart(CountryFileName) & "\"

            sTemp = GetFilePart(CountryFileName)
            ' If rightmost 4 characters are ".cty", remove from the name
            If StrComp(Right(sTemp, 4), ".cty", CompareMethod.Text) = 0 Then
                sTemp = Left(sTemp, Len(sTemp) - 4)
            End If

            .NextItem = sTemp ' 3
            For i = 4 To RUNFILECOLS
                .NextItem = vbNullString
            Next i

            ' 21 Aug 2006 JWD (C0909) Add country file sensitivities to run command 'file'
            'UPGRADE_WARNING: Couldn't resolve default property of object IsAllocatedArrayStr(CountryFileSensitivities()). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            If IsAllocatedArrayStr(CountryFileSensitivities) Then
                ' 24 Aug 2006 JWD (C0911) Correct UBound usage, add omitted dimension parameter
                '                 Set lower bound to 1, so lower and upper of 0 is skipped
                '                 Condition all on proper dimensioning of 1st (column) dimension
                If (LBound(CountryFileSensitivities, 1) = 1) And (UBound(CountryFileSensitivities, 1) = 4) Then
                    For j = 1 To UBound(CountryFileSensitivities, 2)
                        .NextItem = "SNSCTRY"
                        .NextItem = vbNullString
                        .NextItem = vbNullString
                        .NextItem = CountryFileSensitivities(1, j) ' Form Code
                        .NextItem = CountryFileSensitivities(2, j) ' Form Row
                        .NextItem = CountryFileSensitivities(3, j) ' Form Column
                        .NextItem = CountryFileSensitivities(4, j) ' Value
                        For i = 8 To RUNFILECOLS
                            .NextItem = vbNullString
                        Next i
                    Next j
                End If
                ' was:
                'For j = LBound(CountryFileSensitivities, 2) To UBound(CountryFileSensitivities)
                '    .NextItem = "SNSCTRY"
                '    .NextItem = vbNullString
                '    .NextItem = vbNullString
                '    .NextItem = CountryFileSensitivities(1, j)  ' Form Code
                '    .NextItem = CountryFileSensitivities(2, j)  ' Form Row
                '    .NextItem = CountryFileSensitivities(3, j)  ' Form Column
                '    .NextItem = CountryFileSensitivities(4, j)  ' Value
                '    For i = 8 To RUNFILECOLS
                '        .NextItem = vbNullString
                '    Next i
                'Next j
                ' End (C0911)
            End If
            ' End (C0909)

            ' Add the run command information
            .NextItem = "RUN"
            .NextItem = "" ' 2
            .NextItem = "" ' 3
            .NextItem = "" ' 4
            .NextItem = "ALL" ' 5
            .NextItem = "" ' 6
            .NextItem = "100" ' 7
            For i = 8 To RUNFILECOLS
                .NextItem = ""
            Next i
            .CloseFile()
        End With

        ' Define a reference symbol for the closed file, so it can be re-opened
        Dim l_oRunFile As IEFSFileSeq
        l_oRunFile = l_oRunFileOut ' set the now-closed file reference


        '<<TBD>> How to set the UseGrossProductionAmounts: specified in Codes()?
        ' Need to ask Dennis about this, but after checking, I don't think
        ' this applies for single project economics at all and should be false.
        ' Certainly don't need to use the run file anymore for this...
        '      UseGrossProductionAmounts = True

        UseGrossProductionAmounts = False

        g_bPTCons = False ' This is not a pre-tax consolidation run

        '<<TBD>> which calcindicator is this?
        '''        EconomicLimitApplied = 0      ' 0=Economic Limit keyword not found in fiscal definition

        ' Add load of currency exchange rate data
        LoadCurrencyFileAMPE()

        InitializeAbandonmentExpenditureData()

        InitializeAbandonmentFundingProvisions()

        '<<TBD>> Proper size of economic indicators array??? 22 indicators?
        'UPGRADE_WARNING: Lower bound of array EconomicIndicators was changed from 1,1 to 0,0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim EconomicIndicators(6, 14)

        ' Initialize deflator factors
        ReDim DFL(gc_nMAXLIFE)
        zzzInitializeArray(DFL, 1)

        ' Don't currently support ring fence but need these to satisfy dcf code changes
        ' Set a global reference to the ring fence file, and initialize it
        g_oRingFenceFile = g_oFileSystem.OpenForOutput(TempDir & "RING.FNC").CloseFile

        ReDim gfa_RingFenceFiles(4)

        gfa_RingFenceFiles(gfa_RingFenceFile_OPS) = g_oFileSystem.OpenForOutput(TempDir & "RINGO.FNC").CloseFile
        gfa_RingFenceFiles(gfa_RingFenceFile_GRP) = g_oFileSystem.OpenForOutput(TempDir & "RINGG.FNC").CloseFile
        gfa_RingFenceFiles(gfa_RingFenceFile_CMP) = g_oRingFenceFile ' holds another reference, for when g_oRingFenceFile is assigned one of the others
        gfa_RingFenceFiles(gfa_RingFenceFile_DUM) = g_oFileSystem.OpenForOutput(TempDir & "DUMMY.FNC").CloseFile


FiscalCalculator_Input:

        ' CompatibilityMode indicates if the copy should go
        ' through an intermediate text representation state on
        ' assignment. This is for comparison with Mainexec.
        If mainexecCompatibilityMode Then
            ' Convert input arrays
            ' Convert base data
            ' Convert capex data
        End If


        DiscMthd = CalcSettings(settings_discount_method)
        PPR = CalcSettings(settings_field_type)

        ' Copy general parameters data
        gn(1) = MiscellaneousItems(1) ' water depth
        gn(2) = MiscellaneousItems(2) ' volumes equivalence
        gn(3) = 0 ' Unused
        gn(4) = DiscountRates(1)
        gn(5) = DiscountRates(2)
        gn(6) = DiscountRates(3)
        gn(7) = DiscountRates(4)
        gn(8) = DiscountRates(5)
        gn(9) = DiscountRates(6)

        ' Validate and convert date entries
        zzzConvertDateEntry(ProjectDates(1), mo, YR) ' project start
        zzzConvertDateEntry(ProjectDates(2), M2, Y2) ' discovery
        zzzConvertDateEntry(ProjectDates(3), M1, Y1) ' production start
        zzzConvertDateEntry(ProjectDates(4), M3, Y3) ' discount

        ' Convert the capex forecast amounts to single precision,
        ' performing any conversions for compatibility
        ' DO the other capex input arrays as well, then
        ' Store capital expenditure forecasts in MY3()
        ConvertCapexForecasts(zzzArray2DDoubleAsSingle(CapexAmounts, mainexecCompatibilityMode), zzzArray2DDoubleAsSingle(CapexWI, mainexecCompatibilityMode), zzzArray2DDoubleAsSingle(CapexRP, mainexecCompatibilityMode), zzzArray1DDoubleAsSingle(CapexTangible, mainexecCompatibilityMode), CalcSettings(settings_allocation_method), YR, my3, MY3T)

        ' Determine life of input forecasts and update project life variable
        ' 30 Sep 2003 JWD (C0754) Add LFX to parameter list
        zzzCheckInputForecastsLife(BaseAmounts, LG, LFX)

        '<<TBD>> Check for LG = 0? What to do if no volume forecasts

        ' 2 Aug 2005 JWD (C0887) Remove this, already being done
        '           elsewhere (ApplyAbandonmentFundingProvisions)
        'If CalcSettings(settings_abandonment_timing) = 2 Then
        '    ' Extend life for abandonment expenditure timing
        '    LG = LG + 1
        'End If
        ' End (C0887)

        ' 'Actual project life' When project start other than jan 1
        LGI = LG - ((mo - 1) / 12)
        ' 'Actual production life' when production start other than jan 1
        LFI = LFX - ((M1 - 1) / 12)

        ' Copy the base forecast amounts to the A() array
        ' 2 Aug 2005 JWD (C0888) Change A time dimension size
        'UPGRADE_WARNING: Lower bound of array A was changed from 1,1 to 0,0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim A(gc_nMAXLIFE, UBound(BaseAmounts, 1))
        ' was:
        'ReDim A(1 To LG, 1 To UBound(BaseAmounts, 1))
        ' End (C0888)
        If mainexecCompatibilityMode Then
            For i = 1 To LG
                For j = 1 To UBound(BaseAmounts, 1)
                    ' Add text representation state to assignment
                    ' to make conversions consistent with disk data
                    ' files for comparisons with external engine
                    A(i, j) = Val(Str(CSng(BaseAmounts(j, i))))
                Next j
            Next i
        Else
            For i = 1 To LG
                For j = 1 To UBound(BaseAmounts, 1)
                    A(i, j) = BaseAmounts(j, i)
                Next j
            Next i
        End If

        ' Assign the working interest forecast if all zeroes
        zzzCheckWIForecast(A)

        ' Make sure the working interest is assigned to capex
        zzzUpdateCapexWI(A, my3, MY3T)

        ' Abandonment parameters
        SetGrossAbandonmentExpenditure(MiscellaneousItems(3))
        SetAbandonmentExpenditurePeriodOffset((CalcSettings(settings_abandonment_timing) - 1))
        '<<TBD>> extract the abandonment inflation from Inflation()
        '''SetAbandonmentInflationForecast AbandonmentInflation()

        ' Open the "run file"
        ' Assign to the global symbol
        g_oRunFileIn = l_oRunFile.OpenForInput

        ' Load the first run file command line
        GetRunFileLine()

        ' Load the fiscal model (country file)
        CountryForecast()

        ' Copy non-blank forecast names to the output text object
        For i = 1 To UBound(BaseNames)
            If Len(BaseNames(i)) > 0 Then
                OutputText.ForecastTitle(Mid(BDACategoryCodesString, (i - 1) * 3 + 1, 3)) = BaseNames(i)
            End If
        Next i

        For i = 1 To UBound(CapexNames)
            If Len(CapexNames(i)) > 0 Then
                OutputText.ForecastTitle(Mid(CPXCategoryCodesString, (i - 1) * 3 + 1, 3)) = CapexNames(i)
            End If
        Next i

FiscalCalculator_Process:

        '''      '' See if it looks like any capex carries are in the project
        '''      'zzzCheckCapexForCarriedExpenditures A(), my3(), MY3T, b_have_capex_carry
        '''
        '''      '' See how big the model is for sizing output reports arrays
        '''      'zzzDetermineOutputArraySize i_estimated_size, b_have_capex_carry
        '''      '
        '''      'ReDim OutAmts(1 To i_estimated_size, 1 To LG)
        '''      'ReDim OutNms(1 To i_estimated_size, 1 To 2)
        '''      'ReDim OutInts(1 To i_estimated_size, 1 To 2)

        ' Compute total opex
        ReDim OPEX(gc_nMAXLIFE)
        For i = 1 To LG
            For j = gc_nAOX1 To gc_nAOX5
                OPEX(i) = OPEX(i) + A(i, j)
            Next j
        Next i

        '<<TBD>> Is this next necessary ?????? If nothing else don't reference RF$()
        ''** Currency conversion of input here
        '' Don't need to convert for single project purposes
        '' per Dennis Smith, 1 May 2003
        'If Len(RF$(3)) > 0 Then
        '   ConvertInputData RF$(3)
        'End If

        ApplyAbandonmentFundingProvisions()

        CalculateBonus()

        '<<TBD>> Same here, don't use RF$()
        'If Len(RF$(3)) > 0 Then sCur = RF$(3)

        Call GrossReport()

        ReDim gna_ACFX(LG, 26)
        'UPGRADE_WARNING: Lower bound of array xRunSwitches was changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim xRunSwitches(RunSwitchesCount)

        ' go ahead and do the normal calls through the
        ' generation of after-tax cash flow page while the
        ' data is absolutely right at this point.
        xRunSwitches(RunSwitch_LVL) = RunSwitch_LVL_Company
        xRunSwitches(RunSwitch_PAR) = RunSwitch_PAR_On
        xRunSwitches(RunSwitch_WIN) = RunSwitch_WIN_On
        xRunSwitches(RunSwitch_FIN) = RunSwitch_FIN_On
        xRunSwitches(RunSwitch_LMT) = RunSwitch_LMT_On
        xRunSwitches(RunSwitch_DCF) = RunSwitch_DCF_Off

        Call FiscalDef()
        Call ConsolidateLoan()
        Call CalculateRepayment()

        ' capture the company cash flow values now
        xxx_POSCF = gna_ACFX_CPS
        xxx_NEGCF = gna_ACFX_CNG

        Call Cashflow()


        ' Now go back and generate the data for the dcf page
        ' single data run (non pre-tax consol, g_bPTCons=false)
        For iiiX = 1 To LG
            gna_ACFX(iiiX, gna_ACFX_TPS) = ATotalRevenues(iiiX)
            gna_ACFX(iiiX, gna_ACFX_TNG) = OPEX(iiiX)
        Next iiiX

        For iiiX = 1 To my3tt
            If (my3(iiiX, 1) > 1 And my3(iiiX, 1) < CPXCategoryCodeBAL) Or my3(iiiX, 1) = CPXCategoryCode_AbandonmentCashExpenditure Then
                iiiY = my3(iiiX, 3) - YR + 1
                gna_ACFX(iiiY, gna_ACFX_TNG) = gna_ACFX(iiiY, gna_ACFX_TNG) + my3(iiiX, 5)
            End If
        Next iiiX

        xRunSwitches(RunSwitch_LVL) = RunSwitch_LVL_Operating
        xRunSwitches(RunSwitch_PAR) = RunSwitch_PAR_Off
        xRunSwitches(RunSwitch_WIN) = RunSwitch_WIN_Off
        xRunSwitches(RunSwitch_LMT) = RunSwitch_LMT_Off
        xRunSwitches(RunSwitch_DCF) = RunSwitch_DCF_Off

        ReportLevelSaved = RF(5)
        RF(5) = "   " ' set to no report output

        ' Change the ring fence file to use for this level
        ' Used from here through the next call of Cashflow()
        g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_OPS)

        Call FiscalDef()
        Call ConsolidateLoan()
        Call CalculateRepayment()

        xxx_POSCF = gna_ACFX_OPS
        xxx_NEGCF = gna_ACFX_ONG

        Call Cashflow()

        xRunSwitches(RunSwitch_LVL) = RunSwitch_LVL_Group
        xRunSwitches(RunSwitch_PAR) = RunSwitch_PAR_On

        ' Change the ring fence file to use for this level
        ' Used from here through the next call of Cashflow()
        g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_GRP)

        Call FiscalDef()
        Call ConsolidateLoan()
        Call CalculateRepayment()

        xxx_POSCF = gna_ACFX_GPS
        xxx_NEGCF = gna_ACFX_GNG

        Call Cashflow()

        xRunSwitches(RunSwitch_LVL) = RunSwitch_LVL_Company
        xRunSwitches(RunSwitch_WIN) = RunSwitch_WIN_On

        ' Change the ring fence file to use for this level
        ' Used from here through the next call of Cashflow()
        g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_CMP)

        ' Now a bit of a hack, keeps FiscalDef from writing to the ring fence file for this part of the run
        ' don't want to write to the file for company-level ring fence because done
        ' above already. But still have to have a reference to the company-level for reading
        RunNameSaved = RF(2) ' save the actual name
        RF(2) = RunNameSaved & "X" ' fixup to fool ring fence output code
        Call FiscalDef()
        RF(2) = RunNameSaved ' restore the actual name
        Call ConsolidateLoan()
        Call CalculateRepayment()

        xxx_POSCF = 0
        xxx_NEGCF = 0

        ' Change the ring fence file to use for this level
        ' Don't want the company level data written again, was done above
        g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_DUM)

        Call Cashflow()

        ' do the discounted cash flow page
        xRunSwitches(RunSwitch_DCF) = RunSwitch_DCF_Only
        RF(5) = ReportLevelSaved

        ' Change the ring fence file to use for this level
        ' Used from here through the next call of Cashflow()
        g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_DUM)


FiscalCalculator_CashFlow:  ' this is a target of a goto, coming from FiscalCalculator_InitializeConsol
        ' If this is a consolidation run, rejoining here...
        Cashflow()

        '<<TBD>> Is this the best place to do this especially with life extension due to abandonment
        '<<TBD>> Designation of CalcIndicator to receive this indicator? or put in Other indicators? It is in the run summary!
        '''EconomicLife = LG

FiscalCalculator_Output:

        Dim l_oMap As IDPersistClassMap
        Dim l_oAmountsOut As CDASPEAmountsSeq
        Dim l_oNamesOut As CDASPENamesSeq
        Dim l_oNamesMap As CDGiantRptPageMapSeqD

        l_oAmountsOut = New CDASPEAmountsSeq

        i = g_oReport.TimeSeriesProfileCount
        j = g_oReport.MaxProfileElementCount

        ' Get amounts array from report object
        l_oAmountsOut.Initialize(i, j)
        l_oMap = New CDGiantRptPageMapSeqB ' amounts array
        'UPGRADE_WARNING: Couldn't resolve default property of object l_oAmountsOut. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        g_oReport.WriteReport(l_oAmountsOut, l_oMap)
        OutputAmounts = VB6.CopyArray(l_oAmountsOut.AllValues)

        ' Get interests array from report object
        l_oAmountsOut.Initialize(i, 2)
        l_oMap = New CDGiantRptPageMapSeqC ' interests array
        'UPGRADE_WARNING: Couldn't resolve default property of object l_oAmountsOut. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        g_oReport.WriteReport(l_oAmountsOut, l_oMap)
        OutputInterests = VB6.CopyArray(l_oAmountsOut.AllValues)

        l_oNamesOut = New CDASPENamesSeq

        ' Get names array from report object
        l_oNamesOut.Initialize(i, 2)
        l_oNamesMap = New CDGiantRptPageMapSeqD ' names array
        ' Make Report Text object available to the names map
        ' so it can assign it to the report page formats
        With l_oNamesMap
            .ReportText = g_oReport
            .VariableTitles = g_oReport
        End With
        'UPGRADE_WARNING: Couldn't resolve default property of object l_oNamesMap. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        'UPGRADE_WARNING: Couldn't resolve default property of object l_oNamesOut. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        g_oReport.WriteReport(l_oNamesOut, l_oNamesMap)
        OutputNames = VB6.CopyArray(l_oNamesOut.AllNames)

        ' Get indicators data from report object
        With g_oReport
            EconomicIndicators = Array2DSingleAsDouble(.PresentValueTable)
            '''CompanyROR = CDbl(.CompanyRateOfReturn)
            '''GovernmentROR = CDbl(.GovernmentRateOfReturn)
        End With

        ' Return whether or not the economic limit keyword was applied
        '<<TBD>> CalcIndicators() element to receive this indicator
        '''        EconomicLimitApplied = IIf(zzzFiscalDefinitionHasLMTKeyword, 1, 0)

        ' Get other indicators from report object (run summary)
        Dim l_oSummary As CDArray1DDblA
        l_oSummary = New CDArray1DDblA ' written to array
        l_oSummary.Initialize(1, 16) ' allocate receiving array, need 16 items (was 14, but added 3d party, NOC)
        l_oMap = New CCMGiantRunSummarySeqB ' provides format to output the 16 items
        'UPGRADE_WARNING: Couldn't resolve default property of object l_oSummary. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        g_oReport.WriteReport(l_oSummary, l_oMap)
        OtherIndicators = VB6.CopyArray(l_oSummary.AllValues) ' assign the output array

FiscalCalculator_Termination:

        ' <<TBD>> Wrap up
        CleanUpExcel()

        FiscalCalculatorEx = True

FiscalCalculator_Exit:

        '<<TBD>> PutProgramStatus info
        '<<TBD>> WriteErrorLog Err.Description ' This is just a single line text file


        Exit Function

FiscalCalculator_Error:

        FiscalCalculatorEx = False

        If Err.Source = "AESFiscal" Then
            Err.Raise(Err.Number, ": MFiscalEngineEx")
        Else
            Err.Raise(Err.Number, Err.Source, Err.Description)
        End If

        Resume FiscalCalculator_Exit

    End Function
	
	'''
	''' Check the capital expenditures arrays for the presence
	''' of data indicating that the company or a partner is
	''' carried for some capital expenditures. These would
	''' cause inclusion of related report sections. The purpose
	''' for this is not to actually determine absolutely if the
	''' section would need to be included, or not, but to see
	''' if it is likely that it would.
	'''
	''' Whether or not a carry is in effect is if the working
	''' interest value for the specific expenditure is diffent
	''' from the working interest forecast value for the
	''' corresponding period in the base forecasts.
	'''
	''' Any discrepancy in working interests is enough to
	''' assume that there will be a carry, and should include
	''' the additional report sections.
	'''
	''Private Sub zzzCheckCapexForCarriedExpenditures(ByRef Forecasts() As Single, ByRef Capex() As Single, ByVal CapexCount As Integer, ByRef HaveCarriedCapex As Boolean)
	''
	''   Dim i As Integer
	''   Dim j As Integer
	''
	''   HaveCarriedCapex = False
	''
	''   For i = 1 To CapexCount
	''      j = Capex(i, 3) - YR + 1
	''      If Forecasts(j, gc_nAWIN) <> Capex(i, 6) Then
	''         HaveCarriedCapex = True
	''         Exit For
	''      End If
	''   Next i
	''
	''End Sub
	
	'
	' Modifications:
	' 30 Sep 2003 JWD
	'  -> Add formal parameter ProductionLife. (C0754)
	'  -> Change references to global symbol LG to refer to
	'     formal parameter symbol ProjectLife. (C0755)
	'
	' Go through the volumes looking for the last period of
	' non-zero forecast. This is the project life. This is
	' done in the event the input forecast is oversized and
	' has trailing zeroes.
	'
	' Also, after determination of overall project life,
	' set the production life.
	'
    Private Sub zzzCheckInputForecastsLife(ByRef InputForecasts(,) As Double, ByRef ProjectLife As Short, ByRef ProductionLife As Short)

        Dim i As Short
        Dim j As Short

        ProjectLife = 0

        ' Go backward, period by period, checking volume forecasts.
        ' Continue while volume forecasts are zero.
        For i = UBound(InputForecasts, 2) To LBound(InputForecasts, 2) Step -1
            For j = gc_nAMINVOL To gc_nAMAXVOL
                If InputForecasts(j, i) <> 0 Then
                    ProjectLife = i - LBound(InputForecasts, 2) + 1 ' adjust for lower bound value
                    Exit For
                End If
            Next j
            If ProjectLife > 0 Then ' done checking
                Exit For
            End If
        Next i

        ' 30 Sep 2003 JWD (C0754) Add computation of production life
        ' (periods of production)
        ProductionLife = ProjectLife
        For i = LBound(InputForecasts, 2) To UBound(InputForecasts, 2)
            For j = gc_nAMINVOL To gc_nAMAXVOL
                If InputForecasts(j, i) <> 0 Then
                    ProductionLife = ProjectLife - i + 1
                    Exit For
                End If
            Next j
            If ProductionLife > 0 Then
                Exit For
            End If
        Next i
        ' End (C0754)

    End Sub
	
	'
	' Examine the working interest forecast for entries
	' and update as 100% if no data is entered, i. e. all
	' entries of working interest forecast are zeroes.
	'
    Private Sub zzzCheckWIForecast(ByRef Forecasts(,) As Single)

        Dim i As Short
        Dim j As Short

        j = -1
        For i = 1 To LG
            If Forecasts(i, gc_nAWIN) <> 0 Then
                j = 0 'false
                Exit For
            End If
        Next i

        If j = -1 Then ' no data, all entries were zero
            For i = 1 To LG
                Forecasts(i, gc_nAWIN) = 100 ' update all as 100% WI
            Next i
        End If

    End Sub
	
	'
	' Validate the date entry and assign to
	' referenced variables if good.
	'
	Private Sub zzzConvertDateEntry(ByRef sDateEntry As String, ByRef rMonth As Single, ByRef rYear As Single)
		
		Dim sTmp As String
		Dim tDate As Date
		
		sTmp = Replace(sDateEntry, ".", "/")
		
		tDate = CDate(sTmp)
		
		' If we get here, it's a good date
		
		rMonth = Month(tDate)
		rYear = Year(tDate)
		
	End Sub
	
	'
	' Examine the capital expenditure forecast entries and
	' replace any WIN codes with the actual working interest
	' for the period from the working interest forecast.
	'
    Private Sub zzzUpdateCapexWI(ByRef Forecasts(,) As Single, ByRef Capex(,) As Single, ByVal CapexCount As Short)

        Dim rpx As Short ' period of capital expenditure
        ' relative to project start (ordinal)
        Dim i As Short

        For i = 1 To CapexCount
            If Capex(i, 6) = MY3_Use_AWIN Then
                rpx = Capex(i, 3) - YR + 1
                Capex(i, 6) = Forecasts(rpx, gc_nAWIN)
            End If
        Next i

    End Sub
	
	'
	' Initialize the array to some value
	'
	Private Sub zzzInitializeArray(ByRef TheArray() As Single, ByVal InitialValue As Single)
		
		Dim i As Short
		
		For i = LBound(TheArray) To UBound(TheArray)
			TheArray(i) = InitialValue
		Next i
		
	End Sub
	
	''''
	'''' Modifications:
	'''' 7 Oct 2003 JWD
	''''  -> Change to count the appearance of keywords as
	''''     variables when the keywords appear within an
	''''     iteration loop. This sizes the output arrays to
	''''     avoid 'subscript out of range' errors when such
	''''     improper country file constructions are processed.
	''''     This should be removed when such constructions are
	''''     considered invalid country files. (C0758)
	''''
	'''' Analyze the fiscal model to determine the approximate
	'''' size of the model. This is for sizing of the output
	'''' arrays.
	''''
	'''Private Sub zzzDetermineOutputArraySize( _
	''''   ByRef arraysize As Integer, _
	''''   ByRef HaveCarriedCapex As Boolean)
	'''
	'''
	'''   Dim i As Integer
	'''   Dim j As Integer
	'''   Dim p As Integer
	'''
	'''   Dim cvars As Integer       ' count of user variables
	'''   Dim cvcfe As Integer       ' count of cash flow variables
	'''   Dim cvdpr As Integer       ' count of depreciation deductions
	'''   Dim cvcrc As Integer       ' count of cost recovery incomes
	'''   Dim cvrto As Integer       ' count of variables using rate based calculations
	'''   Dim ckfin As Integer       ' count of FIN keywords
	'''   Dim ckpar As Integer       ' count of PAR keywords
	'''
	'''   Dim keywords As String
	'''   Dim keyword_dpr As String
	'''
	'''   Dim asize As Integer       ' count of rows needed in output array
	'''
	'''   ' 7 Oct 2003 JWD (C0758) Iteration loop flag
	'''   Dim bInIterLoop As Boolean
	'''
	'''   keywords = "CURPARFINLMTWINITBITE"
	'''   keyword_dpr = "DPR"
	'''
	'''   ckpar = 0
	'''   ckfin = 0
	'''   cvars = 0
	'''   cvcfe = 0
	'''   cvdpr = 0
	'''   cvcrc = 0
	'''   cvrto = 0
	'''
	'''   ' Having carried capex uses same report sections
	'''   ' as government participation
	'''   If HaveCarriedCapex Then
	'''      ckpar = ckpar + 1
	'''   End If
	'''
	'''   For i = 1 To TDT
	'''      SearchCodeString keywords, TD$(i, 1), 3, p
	'''      If p > 0 Then     ' keyword
	'''         ' 7 Oct 2003 JWD (C0758) Add test of iter loop flag
	'''         If Not bInIterLoop Then
	'''            If p = 2 Then        ' participation
	'''               ckpar = ckpar + 1
	'''            ElseIf p = 3 Then    ' financing
	'''               ckfin = ckfin + 1
	'''            ElseIf p = 6 Then    ' enter iteration loop
	'''               bInIterLoop = True
	'''            End If
	'''         Else
	'''            If p = 7 Then        ' exit iter loop
	'''                bInIterLoop = False
	'''            ElseIf p = 6 Then
	'''                ' do nothing
	'''            Else                 ' treat as normal variable
	'''                cvars = cvars + 1
	'''            End If
	'''         End If
	'''         ' ~~~~~~ was:
	'''         ' Note: Restore this code when above construction is disallowed
	'''         'If p = 2 Then        ' participation
	'''         '   ckpar = ckpar + 1
	'''         'ElseIf p = 3 Then    ' financing
	'''         '   ckfin = ckfin + 1
	'''         'End If
	'''         ' End (C0758)
	'''      Else              ' user defined variable
	'''         cvars = cvars + 1
	'''
	'''         ' Check for cash flow effect
	'''         If Len(Trim$(TD$(i, 4))) > 0 Then
	'''            cvcfe = cvcfe + 1
	'''         End If
	'''
	'''         ' Check for cost recovery variables
	'''         If StrComp(TD$(i, 5), keyword_dpr, vbTextCompare) = 0 Then
	'''            cvcrc = cvcrc + 1
	'''         End If
	'''         If StrComp(TD$(i, 6), keyword_dpr, vbTextCompare) = 0 Then
	'''            cvcrc = cvcrc + 1
	'''         End If
	'''
	'''         ' Check for depreciation deductions
	'''         For j = 8 To 12
	'''            If StrComp(TD$(i, j), keyword_dpr, vbTextCompare) = 0 Then
	'''               cvdpr = cvdpr + 1
	'''            End If
	'''         Next j
	'''
	'''         ' Check the variable rates table for IRR/RTO calculations
	'''         For j = 1 To RTT
	'''            If sRTV(j) = TD$(i, 1) Then   ' If the current variable
	'''               If RT(j, 4) >= pRateCode_RTO Then
	'''                  If RT(j, 4) < pRateCode_UserBase + 1 Then ' 101 and greater are user variables
	'''                     cvrto = cvrto + 1
	'''                     Exit For       ' Only need to count the first one
	'''                  End If
	'''               End If
	'''            End If
	'''         Next j
	'''      End If
	'''
	'''   Next i
	'''
	'''   ' Repay/reimbursement section only occurs once
	'''   ' even when both participation and capex carry
	'''   If ckpar > 1 Then
	'''      ckpar = 1
	'''   End If
	'''
	'''   ' Having gathered characteristics of the fiscal model
	'''   ' determine the size of the output reports array
	'''
	'''   asize = 78                    ' lines in GrossReports section
	'''
	'''   asize = asize + 15 * ckpar    ' 15 lines in group expenditures and repayment sections
	'''
	'''   asize = asize + 6 * ckfin     ' lines in loans section
	'''
	'''   asize = asize + 15 * cvars    ' 15 lines per variable sheet
	'''
	'''   asize = asize + 10 * cvdpr    ' 10 lines per depreciation work sheet
	'''
	'''   asize = asize + 13 * cvcrc    ' 13 lines per cost recovery work sheet
	'''
	'''   asize = asize + 14 * cvrto    ' max 14 lines per rate based (IRR/RTO) work sheet
	'''
	'''   asize = asize + 7 + cvcfe     ' after tax cash flow max 7 + number of cash flow effect variables
	'''
	'''   asize = asize + 9             ' lines in deflated cash flow section
	'''
	'''   arraysize = asize
	'''
	'''End Sub
	
	'
	' Examine the fiscal definition and return whether
	' or not it contains the LMT (economic limit) keyword.
	'
	Private Function zzzFiscalDefinitionHasLMTKeyword() As Object
		
		Dim i As Short
		Dim bResult As Boolean
		
		bResult = False
		For i = 1 To UBound(TD, 1)
			If StrComp(Left(TD(i, 1), 3), "LMT", CompareMethod.Text) = 0 Then
				bResult = True
				Exit For
			End If
		Next i
		
		'UPGRADE_WARNING: Couldn't resolve default property of object zzzFiscalDefinitionHasLMTKeyword. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		zzzFiscalDefinitionHasLMTKeyword = bResult
		
	End Function
	
	
	'
	' Return a copy of the input array (double precision) as
	' a single precision array. If compatibility mode is true
	' copy through a text representation state to simulate
	' the effect of passing the data through a text disk file
	' for comparing with Mainexec.
	'
	Private Function zzzArray1DDoubleAsSingle(ByRef InputArray() As Double, ByVal mainexecCompatibilityMode As Boolean) As Single()
		
		Dim sOutput() As Single
		Dim i As Integer
		
		' Copy the double precision input amounts to the single precision output array
		'UPGRADE_WARNING: Lower bound of array sOutput was changed from LBound(InputArray, 1) to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
		ReDim sOutput(UBound(InputArray, 1))
		
		If mainexecCompatibilityMode Then
			For i = LBound(InputArray, 1) To UBound(InputArray, 1)
				' Add text representation state to assignment
				' to make conversions consistent with disk data
				' files for comparisons with external engine
				sOutput(i) = Val(Str(CSng(InputArray(i))))
			Next i
		Else
			For i = LBound(InputArray, 1) To UBound(InputArray, 1)
				sOutput(i) = InputArray(i)
			Next i
		End If
		
		zzzArray1DDoubleAsSingle = VB6.CopyArray(sOutput)
		
	End Function
	
	
	'
	' Return a copy of the input array (double precision) as
	' a single precision array. If compatibility mode is true
	' copy through a text representation state to simulate
	' the effect of passing the data through a text disk file
	' for comparing with Mainexec.
	'
    Private Function zzzArray2DDoubleAsSingle(ByRef InputArray(,) As Double, ByVal mainexecCompatibilityMode As Boolean) As Single(,)

        Dim sOutput(,) As Single
        Dim i As Integer
        Dim j As Integer

        ' Copy the double precision input amounts to the single precision output array
        'UPGRADE_WARNING: Lower bound of array sOutput was changed from LBound(InputArray, 1),LBound(InputArray, 2) to 0,0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim sOutput(UBound(InputArray, 1), UBound(InputArray, 2))

        If mainexecCompatibilityMode Then
            For j = LBound(InputArray, 2) To UBound(InputArray, 2)
                For i = LBound(InputArray, 1) To UBound(InputArray, 1)
                    ' Add text representation state to assignment
                    ' to make conversions consistent with disk data
                    ' files for comparisons with external engine
                    sOutput(i, j) = Val(Str(CSng(InputArray(i, j))))
                Next i
            Next j
        Else
            For j = LBound(InputArray, 2) To UBound(InputArray, 2)
                For i = LBound(InputArray, 1) To UBound(InputArray, 1)
                    sOutput(i, j) = InputArray(i, j)
                Next i
            Next j
        End If

        zzzArray2DDoubleAsSingle = VB6.CopyArray(sOutput)

    End Function
	
	'
	' 21 Aug 2006 JWD New (C0909)
	'
	' Return true if the string array is dimensioned,
	' false otherwise.
	'
    Private Function IsAllocatedArrayStr(ByRef test(,) As String) As Object

        ' Determines whether or not the array is dimensioned
        ' by attempting to retrieve the lower bound of dimension 1.
        ' Will raise subscript out of range error if it is not
        ' dimensioned.

        Dim x As Short

        On Error Resume Next

        x = LBound(test, 1)

        If Err.Number = 0 Then
            'UPGRADE_WARNING: Couldn't resolve default property of object IsAllocatedArrayStr. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            IsAllocatedArrayStr = True
            Exit Function
        End If

        If Err.Number = 9 Then
            'UPGRADE_WARNING: Couldn't resolve default property of object IsAllocatedArrayStr. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            IsAllocatedArrayStr = False
            Exit Function
        End If

        On Error GoTo 0

        Err.Raise(Err.Number, Err.Source, Err.Description, Err.HelpFile, Err.HelpContext)

    End Function
End Module