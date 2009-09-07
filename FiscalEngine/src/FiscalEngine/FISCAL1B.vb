Option Strict Off
Option Explicit On
Module FISCAL1B
	' Name:        FISCAL1B.BAS
	' Function:    Fiscal Definition Processing
	'---------------------------------------------------------
	' ********************************************************
	' *            COPYRIGHT - PETROCONSULTANTS, INC.        *
	' *               1986, 1991, 1995, 1996, 1997           *
	' *                  ALL RIGHTS RESERVED                 *
	' ********************************************************
	' *  This program file is proprietary information of     *
	' *  Petroconsultants, Incorporated.  Unauthorized use   *
	' *  for any purpose is prohibited.                      *
	' ********************************************************
	'---------------------------------------------------------
	' Modifications:
	' 3 Mar 1995 JWD
	'  -> Converted module level executable code to subroutine.
	'
	' 8 Feb 1996 JWD
	'  -> Removed main routine code to FISMAIN1.BAS. This
	'     module only contains FiscalDef().
	'  -> Replace explicit subroutine declarations with
	'     include files.
	'  -> Replaced include file CTYIN.BAS with CTYIN1.BG.
	'  -> Add explicit declaration of default storage class as
	'     Single.
	'
	' 19 Feb 1996 JWD
	'  -> Changed references to common array RATE() to
	'     PARTRATE().  RATE is reserved function name in VB.
	'
	' 10 June 1996 MKD
	'  -> Fiscal at approx. line 10053.  Removed adjustments
	'     to RT, PR and CLR. (Variable, Govt. and Ceiling
	'     Rates).  This applied to breakpoint dollar amounts
	'     based on the parameter selected.  Those should be
	'     entered  in the currency at that level in the
	'     country file.
	'
	' 25 July 1996 MKD
	'  -> Sub Fiscal at approx. lines 10600, 20400 and 23075.
	'     Added ability to reference forward variables even
	'     when not in a loop.
	'
	' 17 Jan 1997 DS
	'  -> Changed FiscalDef().  (SCO0015)
	'
	' 27 Mar 2001 JWD
	'  -> Changed FiscalDef(). (C0296)
	'
	' 24 Apr 2001 JWD
	'  -> Changed FiscalDef(). (C0301)
	'
	' 7 May 2001 JWD
	'  -> Changed FiscalDef(). (C0305)
	'
	' 21 Jun 2001 JWD
	'  -> Changed FiscalDef(). (C0339)
	'
	' 26 Jun 2001 JWD
	'  -> Changed FiscalDef(). (C0339)
	'
	' 2 Aug 2001 JWD
	'  -> Changed FiscalDef(). (C0363)
	'
	' 4 Aug 2001 JWD
	'  -> Add Option Explicit directive. (C0367)
	'  -> Changed FiscalDef(). (C0367)
	'
	' 21 Sep 2001 JWD
	'  -> Changed FiscalDef(). (C0451)
	'
	' 01 Oct 2001 GDP
	'  -> Changed FiscalDef(). Added code after label 10059 to
	'     call UK PRT calculation routines
	'
	' 05 Oct 2001 GDP
	'  -> Changed FiscalDef(). Changed code after label 10059 so
	'     reserved UK PRT variables are "PRP" and "PRI" royalty
	'     reserved variable remain "RYL" and "RYP"
	'
	' 1 Apr 2002 JWD
	'  -> Changed FiscalDef(). (C0528)
	'
	' 19 Nov 2002 JWD
	'  -> Changed FiscalDef(). (C0633)
	'  -> Added new procedure zzzGetCeilingAmounts(). (C0633)
	'
	' 5 Dec 2002 JWD
	'  -> Changed FiscalDef(). (C0640)
	'
	' 6 Dec 2002 JWD
	'  -> Changed FiscalDef(). (C0642)
	
	' 17 Dec 2002 GDP
	'  -> Changed FiscalDef(). Added code to calculate excel linked variables.
	'
	' 8 Jan 2003
	'  -> Changed FiscalDef(). Added code so variable report writes zeros for everything
	'     except NetAmount if the variable is linked to excel or a calc function.
	'
	' 20 Jan 2003
	'  -> Changed FiscalDef().
	'
	' 24 Jan 2003
	'  - Changed FiscalDef().
	'
	' 08 Apr 2003
	'  -> Changed FiscalDef().
	'
	' 28 Apr 2003
	'  -> Changed FiscalDef().
	'
	' 27 May 2003 JWD
	'  -> Changed FiscalDef(). (C0702)
	'
	' 29 May 2003 JWD
	'  -> Changed FiscalDef(). (C0703)
	'
	' 5 Jun 2003 JWD
	'  -> Changed FiscalDef(). (C0710)
	'
	' 12 Jan 2004 JWD
	'  -> Changed FiscalDef(). (C0772)
	'
	' 19 Jan 2004 JWD
	'  -> Changed FiscalDef(). (C0774)
	'
	' 21 Jan 2004 JWD
	'  -> Changed FiscalDef(). (C0774)
	'
	' 3 Feb 2004 JWD
	'  -> Changed FiscalDef(). (C0776)
	'
	' 9 Feb 2004 JWD
	'  -> Changed FiscalDef(). (C0779)
	'
	' 29 Dec 2004 JWD
	'  -> Changed FiscalDef(). (C0846)
	'
	' 22 Apr 2005 JWD
	'  -> Changed FiscalDef(). (C0873)
	'
	' 3 Jul 2008 JWD
	'  -> Changed FiscalDef(). (UGLY HACK!)
	'-----------------------------------------------------------------------
	'$DYNAMIC
	'UPGRADE_NOTE: DefSng A-Z statement was removed. Variables were explicitly declared as type Single. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="92AFD3E3-440D-4D49-A8BF-580D74A8C9F2"'
	
	Const FISCAL As String = "FISCAL"
	
	'$INCLUDE: 'ctyin1.bg'
	
	'$INCLUDE: 'currency.bi'
	'$INCLUDE: 'deprec.bi'
	'$INCLUDE: 'fiscal.bi'
	'$INCLUDE: 'grossrpt.bi'
	'$INCLUDE: 'loan.bi'
	'$INCLUDE: 'partic.bi'
	'$INCLUDE: 'pgm9900.bi'
	'$INCLUDE: 'util5.bi'
	
	'-----------------------------------------------------------------------
	
	'=========================================================
	Sub FiscalDef()
		Dim CRE As Object
        Dim ded() As Single
        Dim dumrev() As Single
		'---------------------------------------------------------
		' Modifications:
		' 8 Feb 1996 JWD
		'  -> Removed includes of SCRA1IN.BAS and SCRA1OUT.BAS.
		'     Data written on the scratch file SCRA1.SCR is now
		'     stored in common.
		'
		' 20 Feb 1996 JWD
		'  -> Change CUR$ to sCur, duplicate definition (CUR()).
		'  -> Change CPD$ to sCPD, duplicate definition (CPD()).
		'  -> Change DL$ to sDL, duplicate definition (DL()).
		'  -> Change DP$ to sDP, duplicate definition (DP()).
		'  -> Change PD$ to sPDV, duplicate definition (PD()).
		'  -> Change RT$ to sRTV, duplicate definition (RT()).
		'  -> Change TM$ to sTMV, duplicate definition (TM()).
		'
		' 17 Jan 1997 DS
		'  -> Add code for Venezuela Service Fee calculation at
		'     line 10058 to 10059.  (SCO0015)
		'
		' 27 Mar 2001 JWD
		'  -> Added block to handle OT3 code for CMB fiscal def
		'     variable. (C0296)
		'
		' 24 Apr 2001 JWD
		'  -> Added block to handle OT4 code for CMB fiscal def
		'     variable. (C0301)
		'
		' 7 May 2001 JWD
		'  -> Add code to handle numeric entries in DED1-5 when
		'     variable is CMB, assigning values to internal
		'     symbols. This implemented to provide a more flexible
		'     alternative to using hard values associated with the
		'     codes. (C0305)
		'
		' 21 Jun 2001 JWD
		'  -> Replace explicit references to MY3() category codes
		'     18, 19, & 20 (BAL, BL2, BL3) with global symbols for
		'     the same. Necessitated by addition of new capital
		'     category codes that changed the actual values of the
		'     BAL codes.(C0339)
		'
		' 26 Jun 2001 JWD
		'  -> Add call to procedure to adjust the timing of the
		'     abandonment expenditures due to the application of
		'     the economic limit. (C0341)
		'  -> Add Redim and initialization of working interest,
		'     group base and participation rates and currency that
		'     are done at the end of CalculateBonus to allow for
		'     addition of abandonment early funding payments to
		'     the capital expenditures array. (C0341)
		'
		' 2 Aug 2001 JWD
		'  -> Change procedure called on hitting economic limit to
		'     a generic 'event handling' abandonment provisions
		'     procedure. (C0363)
		'
		' 4 Aug 2001 JWD
		'  -> Add declarations necessary to satisfy Option
		'     Explicit directive. (C0367)
		'  -> Change the ReDim of the arrays to redimension to the
		'     maximum life to accomodate the 'last year + 1' entry
		'     of abandonment expenditure. (C0367)
		'  -> Added CPXCategoryCode_AbandonmentCashExpenditure as
		'     a 'non-BAL' item to conditional tests looking for
		'     same. (C0363)
		'
		' 21 Sep 2001 JWD
		'  -> Remove the assignment of "USA" to sCur. On entry to
		'     this procedure, sCur has already been set in Main().
		'     Assignment here steps on correct value. (C0451)
		'
		' 1 Apr 2002 JWD
		'  -> Remove loop searching variable titles table for
		'     matching code. This search is done to retrieve the
		'     long title associated with the variable code. Change
		'     in design (content/arrangement of report) assigns
		'     the variable code to the report page header as the
		'     "title." (C0528)
		'
		' 19 Nov 2002 JWD
		'  -> Add alternate block for calculating amount of loss
		'     carry forward that reduces current year positive
		'     tax that limits the amount with a ceiling. (C0633)
		'  -> Add column to output report that contains the
		'     ceiling amounts used by year. (C0633)
		'
		' 5 Dec 2002 JWD
		'  -> Add internal subroutine to check to see if a VEN,
		'     PRT, or calc column variable is ring fenced and set
		'     flag appropriately. (C0640)
		'  -> Change to check on entry to VEN, PRT, and calc
		'     variable calculation routines to see if the fiscal
		'     variable has been ring fenced. If so, branch to ring
		'     fence routine at 10220, otherwise continue with
		'     normal calculation. (C0640)
		'
		' 6 Dec 2002 JWD
		'  -> Add allocation and initialization of loss carry
		'     forward ceiling amounts to the variable report page
		'     fixup in the ring fence routine. (C0642)
		
		' 17 Dec 2002 GDP
		'  -> Added code to send values to the linked Excel spreadsheet
		'     and retreive calculated values back.
		'
		' 20 Jan 2003 GDP
		'  -> Changes for additional volume streams. Mostly involves changing
		'     A array references to use constants in modArrayConst
		'
		' 24 Feb 2003 GDP
		'  -> Added code to stop Production and Price columns appearing in reports
		'     if "PRC" is not present in Price column in Fiscal Definition
		'
		' 08 Apr 2003 GDP
		'  -> Commented out PRT code - nobody uses it.
		'
		' 28 Apr 2003 GDP
		'  -> Uncommented the PRT code - somebody wants to use it now!
		'  -> PRT code is now runs conditional on a registry setting.
		'
		' 27 May 2003 JWD
		'  -> Replace literal numbers representing rate parameters
		'     RTO & IRR with symbols. Symbols are defined in
		'     UTIL5A.BAS. (C0700)
		'
		' 29 May 2003 JWD
		'  -> Change cost recovery/depreciation test on the
		'     accumulation of credits to test at beginning of
		'     code block so that credits specified on the variable
		'     definition line can be applied in cost recovery
		'     cases. The old test did not permit any credits to be
		'     applied to cost recovery variables. (C0703)
		'
		' 5 Jun 2003 JWD
		'  -> Change to output the production and price rows on
		'     any variable that has ANY code entered in the price
		'     column. (C0710)
		'
		' 12 Jan 2004 JWD
		'  -> Add references to CGiantReport1 object to collect
		'     report data in object rather than output directly to
		'     file. For consolidation engine development testing
		'     purposes. (C0772)
		'
		' 19 Jan 2004 JWD
		'  -> Changed report page object type from CGiantRptPageA1
		'     to interface IGiantRptPageAssignStd. (C0774)
		'
		' 21 Jan 2004 JWD
		'  -> Change report page instantiation method name (C0774)
		'
		' 3 Feb 2004 JWD
		'  -> Remove explicit writes to report file. (C0776)
		'
		' 9 Feb 2004 JWD
		'  -> Replace call to TerminateExecution with re-raise of
		'     error to caller. (C0779)
		'
		' 29 Dec 2004 JWD
		'  -> Add set of g_nFiscalEvents bit map with financing
		'     and WIN and PAR events. (C0846)
		'
		' 22 Apr 2005 JWD
		'  -> Changed conditional expressions testing symbol
		'     g_nFiscalEvents to perform bit-wise And before the
		'     relational (>) operation. Was always executing the
		'     conditional code if any financing events, not the
		'     intended masking of events. (C0873)
		'
		' 3 Jul 2008 JWD
		'  -> An ugly hack to recalculate the cost recovery variables
		'     when applying PAR and WIN. Changed to jump out of an
		'     active For ... Next loop to re-calculate the pointed
		'     to variable, then when done, jump back into the loop
		'     at a point after the GOTO out. (Very bad code, but
		'     no time to do it better!).
		'
		'     WARNING, be alert to the changes described in
		'     the PAR and WIN handling code. (at 10037-10038 and
		'     10050-10051).
		'---------------------------------------------------------
		
		Dim bInApplyPAR As Boolean
		Dim bInApplyWIN As Boolean
		Dim ipxyz As Short
		Dim iX_Saved As Short
		Dim rptLvl_Saved As String
		
		Dim bDPCR As Short ' Cost Recovery switch: True=Cost Recov.
		
		Dim fRingFence As String
		
		Dim i As Short
		Dim iBGN As Short
		Dim iBGYE As Short
		Dim iCK As Short
		Dim iENDE As Short
		Dim iEndLoop As Short
		Dim iENYE As Short
		Dim iFirstLoop As Short
		Dim iPeriod As Short
		Dim iPPX As Short
		Dim iPX As Short
		Dim ipxy As Short
		Dim iQ As Short
		Dim ii As Short
		Dim iX As Short
		Dim iXJ As Short
		Dim iXY As Short
		Dim iXYZ As Short
		Dim ixz As Short
		Dim iXZA As Short
		Dim iZ As Short
		Dim j As Short
		Dim jk As Short
		Dim js As Short
		Dim jp As Short
		Dim k As Short
		Dim kk As Short
		Dim LCT As Short
		Dim sDumCur As String
		Dim sDumGrp As String
		Dim a_fCurFactor() As Single
		
		Dim ResLimit As Single
		
		'<<<<<< 4 Aug 2001 JWD (C0367)
		' Add declarations of variables
		Dim TP1 As Single
		Dim TP2 As Single
		Dim AMTPER As Single
		Dim VART As Single
		Dim PgCounter As Short
		Dim cmbresv As Single
		Dim LGMonth As Single
		Dim PDY As Single
		Dim ipp As Single
		Dim ipxymo As Single
		Dim mon As Single
		Dim ipz As Single
		Dim ipzmo As Single
		Dim LCFPool As Single
		Dim cmbaccrev As Single
		Dim cmbaccopex As Single
		Dim cmbacccap As Single
		Dim cmbaccrepay As Single
		Dim varnum As Single
		Dim cmbaccroy As Single
		Dim avgcmbrate As Single
		Dim prodstmo As Single
		Dim zeros As Short
		Dim pers As Single
		Dim elm As Single
		Dim ZD As Single
		Dim jj As Single
		Dim AMTS As Single
		Dim largestamount As Single
		Dim largestitem As Single
		Dim lastyear As Single
		Dim qtrlg As Single
		Dim DiscRate As Single
		Dim TotalBVen As Single
		Dim DUM As String
		Dim DUMYR As Single
		Dim DUMLIFE As Single
		Dim DUMx As Single
		Dim MatchPrc As Short
		Dim losscf As Single
		Dim itl As Single
		Dim param As Short
		Dim DefAmount As Single
		Dim IncDedBlank As Short
		Dim TMA As Single
		Dim num As Short
		Dim TEMP As Single
		Dim oblig As Single
		Dim SumTax As Single
		Dim avgtax As Single
		Dim prepaid As Single
		Dim balance As Single
		Dim balcur As Single
		Dim baldef As Single
		Dim defyear As Single
		Dim TaxTot As Single
		Dim DefTot As Single
		Dim TLL As String
		Dim y As Single
		' Add declarations of local arrays
		Dim ald() As Single
		Dim TAX() As Single
		Dim cdt() As Single
		Dim Lcf() As Single
		Dim tp() As Single
		Dim GTax() As Single
		Dim ColumnNm() As String
		Dim DUMC() As Single
		Dim DUMD() As Single
		Dim DUME() As Single
		'>>>>>> End (C0367)
		
		Dim bUseVariableVolumetrics As Boolean
		
		' 19 Nov 2002 JWD (C0633) Add next 2 to support loss carry forward ceilings
		Dim l_CeilingIsDefined As Boolean
		Dim l_CeilingAmounts() As Single
		' End (C0633)
		
		' 5 Dec 2002 JWD (C0640)
		Dim l_FiscalVariableIsRingFenced As Boolean
		' End (C0640)
		
		'-----------------------------------------------------------------------
		' << GDP 17 Dec 2002
		' Added for excel link
		Dim a_fRVNExcel() As Single
		Dim a_fVLMExcel() As Single
		Dim bRVN As Boolean
		Dim bVLM As Boolean
		Dim wksCurrent As Object
		' End GDP 17 Dec 2002 >>
		
		Program = FISCAL
		'Dim dStart As Double
		'dStart = Timer
		
		'<<<<<< 26 Jun 2001 JWD (C0341)
		'  APPLY DEFAULT PARTICIPATION RATES
		
		'<<<<<< 4 Aug 2001 JWD (C0367)
		ReDim GPBASE(my3tt)
		ReDim GPRATE(my3tt)
		ReDim PARTRATE(LG + 1)
		ReDim OPEXRATE(LG + 1)
		ReDim WIN(LG + 1)
		ReDim WINC(my3tt)
		'~~~~~~ was:
		'ReDim GPBASE(my3tt), GPRATE(my3tt), PARTRATE(LG), OPEXRATE(LG)
		'ReDim WIN(LG), WINC(my3tt)
		'>>>>>> End (C0367)
		ReDim GPDPCR(my3tt)
		
		For i = 1 To my3tt
			GPBASE(i) = 1
			GPRATE(i) = 1
			GPDPCR(i) = 1
		Next i
		
		'<<<<<< 4 Aug 2001 JWD (C0367)
		For i = 1 To LG + 1
			PARTRATE(i) = 0
			OPEXRATE(i) = 0
		Next i
		'~~~~~~ was:
		'For i = 1 To LG
		'   PARTRATE(i) = 0
		'   OPEXRATE(i) = 0
		'Next i
		'>>>>>> End (C0367)
		
		' SET DEFAULT CURRENCY TO USA DOLLARS
		
		'<<<<<< 4 Aug 2001 JWD (C0367)
		ReDim CUR(LG + 1)
		ReDim LCur(LG + 1)
		
		'<<<<<< 21 Sep 2001 JWD (C0451)
		'sCur = "USA"
		'>>>>>> End (C0451)
		
		For i = 1 To LG + 1
			CUR(i) = 1
			LCur(i) = 1
		Next i
		'~~~~~~ was:
		'ReDim CUR(LG), LCur(LG)
		'sCur = "USA"
		'For i = 1 To LG
		'   CUR(i) = 1
		'   LCur(i) = 1
		'Next i
		'>>>>>> End (C0367)
		'>>>>>> End (C0341)
		
15: ReDim Preserve REIM(my3tt)
		
		
		'<<<<<< 4 Aug 2001 JWD (C0367)
16: ReDim ald(gc_nMAXLIFE)
		ReDim TAX(gc_nMAXLIFE)
		ReDim cdt(gc_nMAXLIFE)
		ReDim Lcf(gc_nMAXLIFE)
		ReDim DPC(gc_nMAXLIFE)
		ReDim tp(2)
17: ReDim GTax(gc_nMAXLIFE)
		ReDim ColumnNm(15)
		ReDim DUMC(gc_nMAXLIFE)
		ReDim DUMD(gc_nMAXLIFE)
		ReDim DUME(gc_nMAXLIFE)
		ReDim PRINC(gc_nMAXLIFE)
		ReDim INTRST(gc_nMAXLIFE)
		ReDim FINANCE(gc_nMAXLIFE)
		ReDim TOTPMT(gc_nMAXLIFE)
		ReDim LOAN(gc_nMAXLIFE)
		ReDim FVAR(TDT)
		ReDim RVN(gc_nMAXLIFE, TDT)
		ReDim RLD(gc_nMAXLIFE)
		ReDim VLM(gc_nMAXLIFE, TDT)
		ReDim VOL(gc_nMAXLIFE)
		ReDim REV(gc_nMAXLIFE)
		ReDim DPC(gc_nMAXLIFE)
		ReDim PCE(gc_nMAXLIFE)
		ReDim FCRD(gc_nMAXLIFE)
		ReDim DDT(gc_nMAXLIFE, 6)
		'~~~~~~ was:
		'16    ReDim ald(LG), TAX(LG), cdt(LG), Lcf(LG), DPC(LG), tp(2)
		'17    ReDim GTax(LG), ColumnNm$(15), DUMC(LG), DUMD(LG), DUME(LG)
		'      ReDim PRINC(LG), INTRST(LG), FINANCE(LG), TOTPMT(LG), LOAN(LG)
		'      ReDim FVAR$(TDT), RVN(LG, TDT), RLD(LG)
		'      ReDim VLM(LG, TDT), VOL(LG), REV(LG)
		'      ReDim DPC(LG), PCE(LG), FCRD(LG), DDT(LG, 6)
		'>>>>>> End (C0367)
		
		'-----------------------------------------------------------------------
		' THIS PROGRAM READS THE VARIABLE DEFINITIONS AND PROCESSES THEM
		
		On Error GoTo 35000
		fRingFence = TempDir & "RING.FNC" 'declare file variable
		
		FINALPARTIC = 0
		FinalWin = 0
		
		
		'    Call GrossReport
		
		
		' 12 Mar 2004 JWD
		' Add next to input the values of fiscal variables
		' for this run that are ring-fenced. Input and
		' accumulate for future reference so file is only
		' processed once for each run, instead of once for
		' each ring-fenced variable.
		'
		' symbols for values of second subscript
		Const c_RF_RVN As Short = 1 ' RVN values
		Const c_RF_VLM As Short = 2 ' VLM values
		
		Dim iRFV As Short ' index of current variable
        Dim RF_Values(,,) As Single
		
        Dim l_oRingFenceFile As IEFSFileSeq

        l_oRingFenceFile = g_oRingFenceFile

        ' See if any variables for this run are ring-fenced
        iRFV = Len(Trim(RF(8))) \ 3
        If iRFV > 0 Then
            'UPGRADE_WARNING: Lower bound of array RF_Values was changed from 1,1,1 to 0,0,0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ReDim RF_Values(iRFV, 2, gc_nMAXLIFE)

            With l_oRingFenceFile.OpenForInput
                While Not .AtEnd
                    'UPGRADE_WARNING: Couldn't resolve default property of object l_oRingFenceFile.OpenForInput.NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    sDumGrp = .NextItem
                    'UPGRADE_WARNING: Couldn't resolve default property of object l_oRingFenceFile.OpenForInput.NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    DUM = .NextItem
                    'UPGRADE_WARNING: Couldn't resolve default property of object l_oRingFenceFile.OpenForInput.NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    DUMYR = .NextItem
                    'UPGRADE_WARNING: Couldn't resolve default property of object l_oRingFenceFile.OpenForInput.NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    DUMLIFE = .NextItem
                    'UPGRADE_WARNING: Couldn't resolve default property of object l_oRingFenceFile.OpenForInput.NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    sDumCur = .NextItem
                    ' See if this variable's values belong to this run
                    If StrComp(Trim(sDumGrp), Trim(RF(10)), CompareMethod.Text) = 0 Then
                        ' See if this variable is in the list
                        SearchCodeString(RF(8), DUM, 3, iRFV)
                        If iRFV > 0 Then
                            GetCurrencyConversionSpecific(sDumCur, sCur, a_fCurFactor, DUMYR, DUMLIFE)
                            ' Variable is in the list, accumulate the values
                            For ixz = 1 To DUMLIFE
                                'UPGRADE_WARNING: Couldn't resolve default property of object l_oRingFenceFile.OpenForInput.NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                DUMC(ixz) = .NextItem
                                'UPGRADE_WARNING: Couldn't resolve default property of object l_oRingFenceFile.OpenForInput.NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                DUMD(ixz) = .NextItem
                                RF_Values(iRFV, c_RF_RVN, DUMYR - YR + ixz) = RF_Values(iRFV, c_RF_RVN, DUMYR - YR + ixz) + (DUMC(ixz) * a_fCurFactor(ixz))
                                RF_Values(iRFV, c_RF_VLM, DUMYR - YR + ixz) = RF_Values(iRFV, c_RF_VLM, DUMYR - YR + ixz) + DUMD(ixz)
                            Next ixz
                        Else
                            For ixz = 1 To DUMLIFE
                                'UPGRADE_WARNING: Couldn't resolve default property of object l_oRingFenceFile.OpenForInput.NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                DUMx = .NextItem
                                'UPGRADE_WARNING: Couldn't resolve default property of object l_oRingFenceFile.OpenForInput.NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                DUMx = .NextItem
                            Next ixz
                        End If
                    Else
                        For ixz = 1 To DUMLIFE
                            'UPGRADE_WARNING: Couldn't resolve default property of object l_oRingFenceFile.OpenForInput.NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            DUMx = .NextItem
                            'UPGRADE_WARNING: Couldn't resolve default property of object l_oRingFenceFile.OpenForInput.NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            DUMx = .NextItem
                        Next ixz
                    End If
                End While
                .CloseFile()
            End With
        End If


        GoTo 10000 'Branch around the following subroutine...

        '-----------------------------------------------------------------------
        'THIS IS A GOSUB
        '--------------------------------------------------------------------
10000:  ' LOOP THRU TD$() AND PROCESS DATA

        ' These are used to support the recalculation of cost recovery
        ' variables on the application of PAR and WIN in the model
        bInApplyPAR = False
        bInApplyWIN = False

10005:  ReDim FVAR(TDT) : VART = 0 : WINT = 0 : PRTA = 0

        For j = 1 To LG
            WIN(j) = 1
        Next j

        For j = 1 To my3tt
            WINC(j) = 1
        Next j

10006:  PgCounter = 1 'for multiple variables, count the number of pages
        iX = 0

        'Giant 5.4  ----------   initialize iteration variables  START

        XYear = 0 'loop counter - year
        XFirst = 0 'line number of first variable after ITB (iteration begin)
        XLast = 0 'line number of last variable before ITE (iteration end)
        XIter = 0 'switch.  0 = we are NOT iterating, 1 = we are iterating.
        For ii = 1 To TDT
            If Left(TD(ii, 1), 3) = "ITB" Then
                XFirst = ii + 1
            ElseIf Left(TD(ii, 1), 3) = "ITE" Then
                XLast = ii - 1
            End If
        Next ii

        'Giant 5.4  ----------   initialize iteration variables  END


10010:

        '
        ' Warning! These next are really ugly, unstructured code...
        ' The kind of stuff you aren't supposed to do..
        ' But, without being given the time to do a
        ' "proper refactoring," this will have to suffice for now...

        ' These variables indicate whether or not the loop is in
        ' normal variable calculation mode (when false) or if the
        ' loop is in a special recalculation of a cost recovery
        ' variable (when true). When true, goes back to a loop over
        ' the variables prior to the PAR or WIN line in the model.
        '
        ' These GoTos re-enter an active For...Next loop
        ' ReEnterPAR is in the participation code between line numbers 10037-10038
        ' ReEnterWIN is in the working interest code between line numbers 10050-10051
        ' If either of these is true, then iX does NOT point to the correct
        ' line in the model, it points to a prior variable that defines a
        ' cost recovery variable. iX is restored to the proper value after
        ' the loop (into which these jump) has completed...

        Dim specialReEnterPAR As Boolean
        specialReEnterPAR = False

        Dim specialReEnterWIN As Boolean
        specialReEnterWIN = False

        If bInApplyPAR Then
            specialReEnterPAR = True
            GoTo ReEnterPAR
        End If

        If bInApplyWIN Then
            specialReEnterWIN = True
            GoTo ReEnterWIN
        End If


        'Giant 5.4  Start ----------
        'XIter is a switch. If 0 then we are not iterating.  If 1 then we are.
        If XIter = 1 Then
            GoTo 10015
        End If
        'Giant 5.4  End ------------

        iX = iX + 1
        If iX > TDT Then
            'Erase AMTLOAN, B, BONS, sCPD, CPD
            'Erase DL, sDL, dp, sDP, EXPLOAN, INTRST, LOAN, PD, sPDV
            'Erase PRINC, SEQ
            'If bDebugging Then
            '''Print "Calculating Economic Indicators"
            'End If
            '''PushStats "FISCAL", dStart
            Exit Sub
        End If


        'Giant 5.4  Start -------------------------------------------------
        'If we encounter an ITB variable, we set flags XFirst and XYear.
        '  We will continue processing normally until we encounter an ITE
        '  variable.  At that time, we will be executing the following code
        '  repeatedly calculating each variable in a double loop.
        'The outer loop is the year (1 to LG) and the inner loop is the
        '  variable line number (XFirst to XLast).  When we have completed
        '  the loops, iX is correct and we turn off XIter switch and continue
        '  processing the remaining lines normally.

        If Left(TD(iX, 1), 3) = "ITB" Then
            XFirst = iX + 1
            XYear = 1
            GoTo 10010
        End If

        If Left(TD(iX, 1), 3) <> "ITE" Then
            GoTo 10017 'continue processing normally until an ITE is found
        End If

        'you are here if the variable read was ITE.  This triggers the iteration loops
        XLast = iX - 1 'save line number of last iteration variable
        XIter = 1 'set flag to YES

10013:  XYear = XYear + 1 'increment year counter. First year is OK - start at year 2

        If XYear > LG Then 'we are finished the iterations
            XIter = 0 'turn off flag
            GoTo 10010 'restart normal flow
        End If
        iX = XFirst - 1 'initialize to first - 1 so that the next line works
10015:  iX = iX + 1

        'IF bDebugging THEN
        '  IF iX = 5 THEN
        '    OPEN "calc.log" FOR APPEND AS #17
        '      PRINT #17, "10015 incremented iX  iX = "; iX
        '    CLOSE #17
        '  END IF
        'END IF

        If iX > XLast Then
            GoTo 10013
        End If

        'from here to end of 5.4 code is basically the same as 10017 - almost 10036
        FVAR(iX) = TD(iX, 1)

        '      If bDebugging Then
        '''Print "Calculating "; FVAR$(iX)
        '      End If

        ReDim RLD(LG)
        ReDim GTax(LG)
        ReDim FCRD(LG)
        ReDim cdt(LG)
        ReDim VOL(LG)
        ReDim PCE(LG)
        ReDim REV(LG)
        ReDim DDT(LG, 6)
        ReDim TAX(LG)
        ReDim Lcf(LG)

        'IF bDebugging THEN
        '  IF iX = 5 THEN
        '    OPEN "calc.log" FOR APPEND AS #17
        '      PRINT #17, "10015+ redimmed RLD  RLD(5) = "; RLD(5)
        '    CLOSE #17
        '  END IF
        'END IF

        'CHECK ON FIELD TYPE
        If Left(TD(iX, 2), 3) = "OIL" And PPR = 2 Then
            GoTo 10015
        End If
        If Left(TD(iX, 2), 3) = "GAS" And PPR = 1 Then
            GoTo 10015
        End If

        'Continue normal code processing for the variable
        'IF bDebugging THEN
        '  IF iX = 5 THEN
        '    OPEN "calc.log" FOR APPEND AS #17
        '      PRINT #17, "10015++ goto 10227  This is it!"
        '    CLOSE #17
        '  END IF
        'END IF
        '4-20-95 start ----------------------------------------------------------------
        'If the variable used the calc columns, we have stepped on them
        '  with redims throughout the iteration loops.  This should
        '  re-define the vars values

        If TD(iX, 15) <> "" Then 'USE CALC COLUMNS INSTEAD OF STANDARD METHOD
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            x28000(iX) 'SET OTHER VARIABLES SO VARIABLE PAGE WILL LOOK OK

            'If bDebugging Then
            '  Open "calc.log" For Append As #17
            '  Print #17, "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
            '  Print #17, "10060 TD$(iX, 15) = "; TD$(iX, 15); "  back from gosub 20000 RVN(5,iX) = "; RVN(5, iX)
            '  Close #17
            'End If

            ReDim DDT(LG, 6) '0-5 is all we need
            ReDim Lcf(LG)
            For ixz = 1 To LG
                VOL(ixz) = VLM(ixz, iX)
                REV(ixz) = RVN(ixz, iX)
                If VOL(ixz) <> 0 Then
                    PCE(ixz) = REV(ixz) / VOL(ixz)
                ElseIf VOL(ixz) = 0 Then
                    PCE(ixz) = 0
                End If
                RLD(ixz) = REV(ixz)
                TAX(ixz) = 100
                GTax(ixz) = REV(ixz)
                'If bDebugging Then
                '  Open "calc.log" For Append As #17
                '  Print #17, "After 28000 iX = "; iX; "RLD("; iXZ; ") = "; RLD(iXZ); "  Tax = "; TAX(iXZ); "   GTax(iXZ) = "; GTax(iXZ)
                '  Close #17
                'End If
            Next ixz
            '10-19-95       GOSUB finishit
            GoTo 25000
        End If
        '10-19-95
        '      IF TD$(iX, 15) <> "" THEN  'USE CALC COLUMNS INSTEAD OF STANDARD METHOD
        '        IF bDebugging THEN
        '          OPEN "calc.log" FOR APPEND AS #17
        '          PRINT #17, " "
        '          PRINT #17, "**********************************************"
        '          PRINT #17, "10059 check for calc entries TD$("; iX; ",1) = "; TD$(iX, 1)
        '          CLOSE #17
        '        END IF
        '
        '        GOSUB 28000       'SET OTHER VARIABLES SO VARIABLE PAGE WILL LOOK OK
        '
        '        REDIM DDT(LG, 6)      '0-5 is all we need
        '        REDIM Lcf(LG)
        '        FOR iXZ = 1 TO LG
        '          VOL(iXZ) = VLM(iXZ, iX)
        '          REV(iXZ) = RVN(iXZ, iX)
        '          IF VOL(iXZ) <> 0 THEN
        '            PCE(iXZ) = REV(iXZ) / VOL(iXZ)
        '          ELSEIF VOL(iXZ) = 0 THEN
        '            PCE(iXZ) = 0
        '          END IF
        '          RLD(iXZ) = REV(iXZ)
        '          TAX(iXZ) = 100
        '          GTax(iXZ) = REV(iXZ)
        '        NEXT iXZ
        '        GOTO 25000
        '      END IF
        '10-19-95



        '4-20-95 end ------------------------------------------------------------

        GoTo 10227

        'Giant 5.4  End ---------------------------------------------------


10017:  '***
        '      Debug.Print "lg = "; LG; " ix = "; iX
        '      Debug.Print "TD$(ix,1) = "; TD$(iX, 1)
        '      Debug.Print "FVAR$(ix) = "; FVAR$(iX)
        FVAR(iX) = TD(iX, 1)
        '      Debug.Print "ix = "; iX; "  FVAR$(ix) = "; FVAR$(iX); " LG = "; LG
        'If bDebugging Then
        '''Print "Calculating "; FVAR$(iX)
        'IF iX = 5 THEN
        '   OPEN "calc.log" FOR APPEND AS #17
        '   PRINT #17, "10017 redimmed RLD  RLD(5) = "; RLD(5)
        '   CLOSE #17
        'END IF
        'End If

10018:  ReDim RLD(LG)
        ReDim GTax(LG)
        ReDim FCRD(LG)
        ReDim cdt(LG)
        ReDim VOL(LG)
        ReDim PCE(LG)
        ReDim REV(LG)
        ReDim DDT(LG, 6)
        ReDim TAX(LG)
        ReDim Lcf(LG)
        'IF bDebugging THEN
        '  IF iX = 5 THEN
        '    OPEN "calc.log" FOR APPEND AS #17
        '      PRINT #17, "10018 redimmed RLD"
        '    CLOSE #17
        '  END IF
        'END IF

10020:  'CHECK ON FIELD TYPE
10030:  If Left(TD(iX, 2), 3) = "OIL" And PPR = 2 Then
            GoTo 10010
        End If
10035:  If Left(TD(iX, 2), 3) = "GAS" And PPR = 1 Then
            GoTo 10010
        End If

        '=======================================================================
        '10036    'Check for Government Participation
        If Left(TD(iX, 1), 3) <> "PAR" Then
            GoTo 10038
        End If

10036:  ' Moved this label to bypass the above check when applying PAR at top and bottom (iX=0 or iX>TDT)

        ' If not applying participation for this run, loop back
        If xRunSwitches(RunSwitch_PAR) = RunSwitch_PAR_Off Then
            GoTo 10010
        End If

        If PRTA = 1 Then ' already been here (PAR applied at TOP?) loop back
            GoTo 10010
        End If

        'You are here ONLY if Variable is PAR
        PRTA = 1
        If Not g_bPTCons Then ' Don't call if this is a consolidation run
            ' 29 Dec 2004 JWD (C0846) Add set of finance event bit map
            ' 22 Apr 2005 JWD (C0873) Add change of order of operations in expression
            If (g_nFinanceEvents And gc_nFinanceEvents_FIN) > 0 Then
                g_nFinanceEvents = g_nFinanceEvents + gc_nFinanceEvents_PAR
            End If
            ' End (C0846)
            Call Participation()
        End If

10037:  ' Come here after returning from Partic
        ' Now apply net participation interest to all previously defined variables
        '
        If iX = 1 Then
            GoTo 10010
        End If

        For iPX = 1 To iX - 1
            For ipxy = 1 To LG
                RVN(ipxy, iPX) = RVN(ipxy, iPX) * (1 - PARTRATE(ipxy))

                '            If iPX = 4 Or iPX = 3 Then
                '                Debug.Print "RVN(ipxy, iPX) = RVN(ipxy, iPX) * (1 - PARTRATE(ipxy)) = " & iPX & " = " & RVN(ipxy, iPX)
                '            End If

                VLM(ipxy, iPX) = VLM(ipxy, iPX) * (1 - PARTRATE(ipxy))
            Next ipxy

        Next iPX
        For ipxy = 1 To LG
            FINANCE(ipxy) = FINANCE(ipxy) * (1 - PARTRATE(ipxy))
            PRINC(ipxy) = PRINC(ipxy) * (1 - PARTRATE(ipxy))
            INTRST(ipxy) = INTRST(ipxy) * (1 - PARTRATE(ipxy))
        Next ipxy

        ' This next bit of code is very risky maintenance-wise...
        ' Note the GOTO 10227, this will go and recalculate the
        ' the previous variable pointed to by iPX as if it were
        ' the next variable in the model to be calculated.
        '
        ' We save the current value of iX (it points to where we are
        ' in the model) and then set it to point to a different place
        ' (iX=iPX). Now it points at a cost recovery variable. Jump
        ' to the code that handles normal variable calculations. When
        ' it gets to the end of the normal code path (at 29999), it will
        ' return to the top of the normal model variable loop at 10010,
        ' where the bInApplyPAR variable will signal whether or not the
        ' calculation is normal mode (false) or a special recalc (true).
        ' When true, jump back to the "re-entry" location in the loop.
        '
        ' It works at this time but may not in the future...

        iX_Saved = iX ' save the current location in the model

ReEnterPAR:
        For ipxyz = 1 To iX_Saved - 1
            If specialReEnterPAR Then
                bInApplyPAR = False
                Continue For
            End If
            ' If cost recovery, go recalculate the variable
            If TD(ipxyz, 5) = "DPR" Or TD(ipxyz, 6) = "DPR" Then
                iX = ipxyz ' set the model location pointer to the cost recovery variable
                bInApplyPAR = True ' set the flag
                ' See if ring-fenced before jumping, don't recalculate if ring-fenced
                If Len(Trim(RF(8))) = 0 Then GoTo 10227
                ' If param 5 doesn't contain the current fiscal variable then not ring-fenced
                If InStr(1, Trim(RF(8)), Left(TD(iX, 1), 3), CompareMethod.Text) Mod 3 <> 1 Then GoTo 10227
            End If


            bInApplyPAR = False
        Next ipxyz

        iX = iX_Saved ' restore current location in the model


        GoTo 10010
        '=======================================================================
10038:  ' 4/1/97 for colombia only, check for CMB code
        ' this will modify PARTRATE() and OPEXRATE() based on an R-factor
        'Debug.Print "i made it to 10038"

        ' Added GDP 08/02/2000 so consolidation problem fixed in Colombia fixed
        If Left(TD(iX, 1), 3) <> "CMB" Then
            GoTo 10039
        Else
            If g_bPTCons Then
                GoTo 10039
            End If
        End If
        '====================================================== changes made 11/20/97
        ' 7 May 2001 JWD Add explicit declaration of these
        Dim FirstBrk As Single
        Dim SecBrk As Single
        Dim RDenom As Single
        Dim CapInterest As Single

        ' set up variables for various triggers    - updates made 8/27/99

        ResLimit = 0 'R-factor only used after this MMBBLS of reserves
        FirstBrk = 1 'First Breakpoint
        SecBrk = 2 'Second Breakpoint
        RDenom = 0 '0 = use R as denominator, .5 = use R- .5, 1 = use R - 1

        ' Following added 7 May 2001 to support determination
        ' of whether codes or numeric entries are being used.

        ' See if the codes are being used or numeric entries
        Dim cmb_test_entry As String
        Dim i_cmb_test As Short
        Dim cmb_have_numerics As Boolean
        Dim cmb_have_codes As Boolean

        cmb_have_numerics = False
        cmb_have_codes = False

        ' Go through the deduction entries...
        For i_cmb_test = 8 To 12
            cmb_test_entry = Trim(TD(iX, i_cmb_test))
            ' See if there is an entry
            If Len(cmb_test_entry) > 0 Then
                ' Determine if numeric or code and save
                ' the result of the determination.
                If IsNumeric(cmb_test_entry) Then
                    cmb_have_numerics = True
                Else
                    cmb_have_codes = True
                End If
            End If
        Next i_cmb_test

        ' Entries can be numeric or they may use the codes
        ' but they can't be mixed (can't use both)
        If cmb_have_codes And Not cmb_have_numerics Then
            ' End new code 7 May 2001 JWD
            ' Now the following original block is conditional...

            ' Do it the way it used to be done...
            If Left(TD(iX, 8), 3) = "OT1" Then
                If PPR = 1 Then
                    ResLimit = 30 '30 MMBBL limit if primary stream is oil
                Else
                    ResLimit = 210 ' 210 BCF limit if primary stream is gas
                End If
            ElseIf Left(TD(iX, 8), 3) = "OT2" Then
                If PPR = 1 Then
                    ResLimit = 60 '60 MMBBL limit if primary stream is oil
                Else
                    ResLimit = 420 '420 BCF limit if primary stream is gas
                End If
            ElseIf Left(TD(iX, 8), 3) = "OT3" Then
                ' This case added 27 Mar 2001 JWD
                If PPR = 1 Then
                    ResLimit = 60 ' 60 MMBL limit
                Else
                    ResLimit = 900 ' 900 BCF limit
                End If
            ElseIf Left(TD(iX, 8), 3) = "OT4" Then
                ' This case added 23 Apr 2001 JWD
                If PPR = 1 Then
                    ResLimit = 5 ' 5 MMBL limit
                Else
                    ResLimit = 210 ' 210 BCF limit
                End If
            End If

            If Left(TD(iX, 9), 3) = "OT1" Then
                FirstBrk = 1.5
                SecBrk = 2.5
                RDenom = 0.5
            ElseIf Left(TD(iX, 9), 3) = "OT2" Then
                FirstBrk = 2
                SecBrk = 3
                RDenom = 1
            End If

            CapInterest = 0 'Ecopetrol keeps original participating interest in capex for life
            If Left(TD(iX, 10), 3) = "OT1" Then
                CapInterest = 1 'Ecopetrol's interest in capex changes as new interest is calculated via R-factor
            End If

            ' More new code 7 May 2001 JWD
            ' Handle new numeric entries
        ElseIf cmb_have_numerics And Not cmb_have_codes Then
            ' Have numeric entries to assign to the internal storage
            ' Only assign actual numeric entries.
            ' If the entry is blank, use the defaults
            ' assigned above.
            If PPR = 1 Then
                If Len(Trim(TD(iX, 8))) > 0 Then
                    ResLimit = CSng(TD(iX, 8))
                End If
            Else
                If Len(Trim(TD(iX, 9))) > 0 Then
                    ResLimit = CSng(TD(iX, 9))
                End If
            End If

            If Len(Trim(TD(iX, 10))) > 0 Then
                FirstBrk = CSng(TD(iX, 10))
            End If

            If Len(Trim(TD(iX, 11))) > 0 Then
                SecBrk = CSng(TD(iX, 11))
            End If

            If Len(Trim(TD(iX, 12))) > 0 Then
                RDenom = CSng(TD(iX, 12))
            End If

            CapInterest = 0 'Ecopetrol keeps original participating interest in capex for life
            ' When we use numerics, this item gets moved to the credit 1 column
            If Left(TD(iX, 13), 3) = "OT1" Then
                CapInterest = 1 'Ecopetrol's interest in capex changes as new interest is calculated via R-factor
            End If

        End If
        ' End of more new code 7 May 2001 JWD
        ' Didn't touch anything past here (today).



        ' check for reserves greater than a designated reserve size.  modified 11/20/97
        ' If not that big, then no adjustment necessary

        cmbresv = 0
        For ipxy = 1 To LG
            ' GDP 20 Jan 2003
            ' Changed A references to use constants
            If PPR = 1 Then
                cmbresv = cmbresv + A(ipxy, gc_nAOIL) 'accumulate oil production
            Else
                cmbresv = cmbresv + A(ipxy, gc_nAGAS) 'accumlate gas production
            End If
        Next ipxy

        Dim oPg1 As IGiantRptPageAssignStd
        If cmbresv > ResLimit Then 'If Reserves exceed limit then proceed, else skip over everything

            Dim oldrate(LG) As Single ' set OldRate() = participation rate before R factor
            For ipxy = 1 To LG
                oldrate(ipxy) = 1 - PARTRATE(ipxy) 'oldrate() is contractors share
            Next ipxy


            LGMonth = LG * 12 'number of months in project
            Dim cmbmorev(LGMonth) As Single
            Dim cmbmoroy(LGMonth) As Single
            Dim cmbmoopex(LGMonth) As Single

            Dim oldmorate(LGMonth) As Single
            ' set old monthly contractor participation rate
            For ipxy = 1 To LGMonth
                oldmorate(ipxy) = 1 - PARTRATE(LG)
            Next ipxy


            Dim cmbrev(LG) As Single
            Dim cmbroy(LG) As Single
            Dim cmbopex(LG) As Single
            PDY = Y1 - YR + 1 'production start year

            For ipp = 1 To iX - 1

                If Left(TD(ipp, 1), 3) = "REV" Then
                    For ipxy = 1 To LG
                        cmbrev(ipxy) = RVN(ipxy, ipp) 'cmbrev() is annual contractors share of revenue
                        If ipxy = PDY Then 'in prod start year, allocate annual amounts over remaining months

                            For ipxymo = 1 To (M1 - 1)
                                mon = ((ipxy - 1) * 12) + ipxymo
                                cmbmorev(mon) = 0
                            Next ipxymo
                            For ipxymo = M1 To 12
                                mon = ((ipxy - 1) * 12) + ipxymo
                                cmbmorev(mon) = RVN(ipxy, ipp) / (13 - M1) 'cmbmorev() is monthly contractors share of rev
                            Next ipxymo

                        Else

                            For ipxymo = 1 To 12
                                mon = ((ipxy - 1) * 12) + ipxymo
                                cmbmorev(mon) = RVN(ipxy, ipp) / 12
                            Next ipxymo

                        End If

                    Next ipxy

                ElseIf Left(TD(ipp, 1), 3) = "ROY" Then
                    For ipxy = 1 To LG
                        cmbroy(ipxy) = RVN(ipxy, ipp) 'cmbroy() is annual contractors share of royalty
                        If ipxy = PDY Then 'in prod start year, allocate annual amounts over remaining months

                            For ipxymo = 1 To (M1 - 1)
                                mon = ((ipxy - 1) * 12) + ipxymo
                                cmbmoroy(mon) = 0
                            Next ipxymo
                            For ipxymo = M1 To 12
                                mon = ((ipxy - 1) * 12) + ipxymo
                                cmbmoroy(mon) = RVN(ipxy, ipp) / (13 - M1)
                            Next ipxymo

                        Else

                            For ipxymo = 1 To 12
                                mon = ((ipxy - 1) * 12) + ipxymo
                                cmbmoroy(mon) = RVN(ipxy, ipp) / 12
                            Next ipxymo

                        End If

                    Next ipxy

                ElseIf Left(TD(ipp, 1), 3) = "OCS" Then
                    For ipxy = 1 To LG
                        cmbopex(ipxy) = RVN(ipxy, ipp) 'cmbopex() is annual contractors share of opex
                        If ipxy = PDY Then 'in prod start year, allocate annual amounts over remaining months

                            For ipxymo = 1 To (M1 - 1)
                                mon = ((ipxy - 1) * 12) + ipxymo
                                cmbmoopex(mon) = 0
                            Next ipxymo
                            For ipxymo = M1 To 12
                                mon = ((ipxy - 1) * 12) + ipxymo
                                cmbmoopex(mon) = RVN(ipxy, ipp) / (13 - M1)
                            Next ipxymo

                        Else

                            For ipxymo = 1 To 12
                                mon = ((ipxy - 1) * 12) + ipxymo
                                cmbmoopex(mon) = RVN(ipxy, ipp) / 12
                            Next ipxymo

                        End If

                    Next ipxy

                End If

            Next ipp

            ' accumulate capex after standard participation into cmbcap()
            Dim cmbcap(LG) As Single
            Dim cmbmocap(LGMonth) As Single
            For iPX = 1 To my3tt
                '<<<<<< 4 Aug 2001 JWD (C0363)
                If my3(iPX, 1) < CPXCategoryCodeBAL Or my3(iPX, 1) = CPXCategoryCode_AbandonmentCashExpenditure Then ' do not include BAL
                    '~~~~~~ was:
                    ''<<<<<< 21 Jun 2001 JWD (C0339)
                    'If my3(iPX, 1) < CPXCategoryCodeBAL Then    ' do not include BAL
                    ''~~~~~~ was:
                    ''If my3(iPX, 1) < 18 Then    ' do not include BAL
                    ''>>>>>> End (C0339)
                    '>>>>>> End (C0363)
                    ipz = my3(iPX, 3) - YR + 1 'year index
                    cmbcap(ipz) = cmbcap(ipz) + (my3(iPX, 5) * GPRATE(iPX)) 'cmbcap() is annual contr. Capex
                    ipzmo = ((my3(iPX, 3) - YR) * 12) + my3(iPX, 2) 'month index
                    cmbmocap(ipzmo) = cmbmocap(ipzmo) + (my3(iPX, 5) * GPRATE(iPX)) 'monthly contr. capex
                End If
            Next iPX

            ' now calculate repayment of carried interest

            'calculate ceiling for repayment 4/16/97 - per Ecopetrol - subtract royalty from govt share of revenues and take 50%
            Dim cmbannclg(LG) As Single
            For ipxy = 1 To LG
                cmbannclg(ipxy) = ((cmbrev(ipxy) - cmbroy(ipxy)) / (1 - PARTRATE(ipxy))) * PARTRATE(ipxy) * 0.5
            Next ipxy

            'calculate amount of repayment due from each capex
            Dim cmbrepbase(my3tt) As Single
            For iPX = 1 To my3tt
                cmbrepbase(iPX) = (GPRATE(iPX) - GPBASE(iPX)) * my3(iPX, 5)
                'Debug.Print ipx; " GPRATE = "; GPRATE(ipx); "  GPBASE = "; GPBASE(ipx)
                'Debug.Print "my3 = "; my3(ipx, 5)
                'Debug.Print "cmbrepbase = "; cmbrepbase(ipx)
            Next iPX

            'group amounts of repayment by year
            Dim cmbannrep(LG) As Single
            For iPX = 1 To my3tt
                ipxy = my3(iPX, 3) - YR + 1
                cmbannrep(ipxy) = cmbannrep(ipxy) + cmbrepbase(iPX)
            Next iPX

            'now compare standard repayment schedule to ceilings
            Dim cmbrem(LG) As Single
            Dim cmbtaken(LG) As Single
            LCFPool = 0
            For ipxy = 1 To LG
                If cmbannrep(ipxy) <= cmbannclg(ipxy) Then 'first check if repayment is less than ceiling
                    cmbtaken(ipxy) = cmbannrep(ipxy) 'repayment less than ceiling
                    If LCFPool > 0 Then 'there is some unused repayments from prior years
                        cmbrem(ipxy) = cmbannclg(ipxy) - cmbtaken(ipxy)
                        If LCFPool > cmbrem(ipxy) Then
                            cmbtaken(ipxy) = cmbtaken(ipxy) + cmbrem(ipxy)
                            LCFPool = LCFPool - cmbrem(ipxy)
                        Else
                            cmbtaken(ipxy) = LCFPool
                            LCFPool = 0
                        End If
                    End If
                Else
                    cmbtaken(ipxy) = cmbannclg(ipxy)
                    LCFPool = LCFPool + cmbannrep(ipxy) - cmbannclg(ipxy)
                End If
            Next ipxy


            'assign cmbtaken() to cmbrepay()
            Dim cmbrepay(LG) As Single
            Dim cmbmorepay(LGMonth) As Single
            For ipxy = 1 To LG
                cmbrepay(ipxy) = cmbtaken(ipxy) 'annual repay
                If ipxy = PDY Then 'in prod start year, allocate annual amounts over remaining months

                    For ipxymo = 1 To (M1 - 1)
                        mon = ((ipxy - 1) * 12) + ipxymo
                        cmbmorepay(mon) = 0
                    Next ipxymo
                    For ipxymo = M1 To 12
                        mon = ((ipxy - 1) * 12) + ipxymo
                        cmbmorepay(mon) = cmbrepay(ipxy) / (13 - M1)
                    Next ipxymo

                Else

                    For ipxymo = 1 To 12
                        mon = ((ipxy - 1) * 12) + ipxymo
                        cmbmorepay(mon) = cmbrepay(ipxy) / 12
                    Next ipxymo

                End If

            Next ipxy


            ' calculate monthly primary production
            Dim cmbmoprod(LGMonth) As Single
            For ipxy = 1 To LG

                If ipxy = PDY Then 'in prod start year, allocate annual amounts over remaining months

                    For ipxymo = 1 To (M1 - 1)
                        mon = ((ipxy - 1) * 12) + ipxymo
                        cmbmoprod(mon) = 0
                    Next ipxymo
                    For ipxymo = M1 To 12
                        mon = ((ipxy - 1) * 12) + ipxymo
                        ' GDP 20 Jan 2003
                        ' Changed A array references to use constants
                        If PPR = 1 Then
                            cmbmoprod(mon) = A(ipxy, gc_nAOIL) / (13 - M1)
                        Else
                            cmbmoprod(mon) = A(ipxy, gc_nAGAS) / (13 - M1)
                        End If
                    Next ipxymo

                Else

                    For ipxymo = 1 To 12
                        mon = ((ipxy - 1) * 12) + ipxymo
                        If PPR = 1 Then
                            cmbmoprod(mon) = A(ipxy, gc_nAOIL) / 12
                        Else
                            cmbmoprod(mon) = A(ipxy, gc_nAGAS) / 12
                        End If

                    Next ipxymo

                End If

            Next ipxy



            'now calculate ratio on a monthly basis and modify next years rate if necessary
            cmbresv = 0 : cmbaccrev = 0 : cmbaccopex = 0 : cmbacccap = 0 : cmbaccrepay = 0

            If Left(RF(5), 3) = "ALL" And TD(varnum, 18) <> "NOP" And TD(varnum, 18) <> "VOP" Then

                ''''Write #5, 22, YR, PgCounter%, LGMonth, 14, "COLOMBIA PARTICIPATION WORKSHEET", 8, FinalWin, FINALPARTIC, sCur
                ''''Write #5, "CMBPER", "CMBPROD", "CMBREV", "CMBROY", "CMBCPX", "CMBOPX", "CMBRPY", "ADJREV", "ADJROY", "ADJCPX", "ADJOPX", "ADJRPY", "CMBRATIO", "CMBRATE"

                oPg1 = g_oReport.NewStandardRptPageSpecial(22)
                oPg1.SetPageHeader(22, YR, PgCounter, LGMonth, 14, "COLOMBIA PARTICIPATION WORKSHEET", 8, FinalWin, FINALPARTIC, sCur)
                oPg1.SetProfileHeaders("CMBPER", "CMBPROD", "CMBREV", "CMBROY", "CMBCPX", "CMBOPX", "CMBRPY", "ADJREV", "ADJROY", "ADJCPX", "ADJOPX", "ADJRPY", "CMBRATIO", "CMBRATE")
            End If

            'Copy monthly values into separate arrays because we will be adjusting the monthly values for participation rate changes

            Dim adjmoprod(LGMonth) As Single
            Dim adjmorev(LGMonth) As Single
            Dim adjmoroy(LGMonth) As Single
            Dim adjmoopex(LGMonth) As Single
            Dim adjmocap(LGMonth) As Single
            Dim adjmorepay(LGMonth) As Single
            Dim cmbrate(LGMonth) As Single
            Dim cmbratio(LGMonth) As Single

            For ipxy = 1 To LGMonth
                adjmoprod(ipxy) = cmbmoprod(ipxy)
                adjmorev(ipxy) = cmbmorev(ipxy)
                adjmoroy(ipxy) = cmbmoroy(ipxy)
                adjmoopex(ipxy) = cmbmoopex(ipxy)
                adjmocap(ipxy) = cmbmocap(ipxy)
                adjmorepay(ipxy) = cmbmorepay(ipxy)
                cmbrate(ipxy) = oldmorate(ipxy) * 100
            Next ipxy

            For ipxy = 1 To LGMonth
                cmbresv = cmbresv + adjmoprod(ipxy) 'cumulative production
                cmbaccrev = cmbaccrev + adjmorev(ipxy) 'cumulative revenue
                cmbaccroy = cmbaccroy + adjmoroy(ipxy) 'cumulative royalty
                cmbaccopex = cmbaccopex + adjmoopex(ipxy) 'cumulative opex
                cmbacccap = cmbacccap + adjmocap(ipxy) 'cumulative capex
                cmbaccrepay = cmbaccrepay + adjmorepay(ipxy) 'cumulative repay

                If (cmbacccap + cmbaccopex - cmbaccrepay) > 0 Then
                    cmbratio(ipxy) = (cmbaccrev - cmbaccroy) / (cmbacccap + cmbaccopex - cmbaccrepay)
                Else
                    cmbratio(ipxy) = 0
                End If

                If cmbresv > ResLimit Then 'partic rate does NOT change unless reserve limit has been reached

                    If cmbratio(ipxy) > FirstBrk Then

                        If cmbratio(ipxy) < SecBrk Then
                            'Cmbrate() is contractors share
                            If ipxy + 1 <= LGMonth Then
                                cmbrate(ipxy + 1) = (oldmorate(ipxy + 1) * 100) / (cmbratio(ipxy) - RDenom)
                            End If
                        Else
                            If ipxy + 1 <= LGMonth Then
                                cmbrate(ipxy + 1) = (oldmorate(ipxy + 1) / 2) * 100
                            End If
                        End If


                        If ipxy + 1 <= LGMonth Then
                            adjmorev(ipxy + 1) = (adjmorev(ipxy + 1) / oldmorate(ipxy + 1)) * (cmbrate(ipxy + 1) / 100)
                            adjmoroy(ipxy + 1) = (adjmoroy(ipxy + 1) / oldmorate(ipxy + 1)) * (cmbrate(ipxy + 1) / 100)
                            adjmoopex(ipxy + 1) = (adjmoopex(ipxy + 1) / oldmorate(ipxy + 1)) * (cmbrate(ipxy + 1) / 100)
                            If CapInterest = 1 Then 'adjust partic in capex if this switch is on
                                adjmocap(ipxy + 1) = (adjmocap(ipxy + 1) / oldmorate(ipxy + 1)) * (cmbrate(ipxy + 1) / 100)
                            End If
                        End If

                    End If

                End If

                'Write variables to output file
                If Left(RF(5), 3) = "ALL" And TD(varnum, 18) <> "NOP" And TD(varnum, 18) <> "VOP" Then
                    ''''Write #5, ipxy, cmbresv, cmbmorev(ipxy), cmbmoroy(ipxy), cmbmocap(ipxy), cmbmoopex(ipxy), cmbmorepay(ipxy), cmbaccrev, cmbaccroy, cmbacccap, cmbaccopex, cmbaccrepay, cmbratio(ipxy), cmbrate(ipxy)
                    oPg1.SetProfileValues(ipxy, ipxy, cmbresv, cmbmorev(ipxy), cmbmoroy(ipxy), cmbmocap(ipxy), cmbmoopex(ipxy), cmbmorepay(ipxy), cmbaccrev, cmbaccroy, cmbacccap, cmbaccopex, cmbaccrepay, cmbratio(ipxy), cmbrate(ipxy))
                End If

            Next ipxy

            ' Reset PARTRATE() and OPEXRATE() per year based on the monthly values

            For ipxy = 1 To LG

                If ipxy = PDY Then 'in prod start year, allocate rate over remaining months
                    avgcmbrate = 0
                    For ipxymo = M1 To 12
                        mon = ((ipxy - 1) * 12) + ipxymo
                        avgcmbrate = avgcmbrate + cmbrate(mon) / (13 - M1)
                    Next ipxymo

                Else
                    avgcmbrate = 0
                    For ipxymo = 1 To 12
                        mon = ((ipxy - 1) * 12) + ipxymo
                        avgcmbrate = avgcmbrate + (cmbrate(mon) / 12)
                    Next ipxymo

                End If

                PARTRATE(ipxy) = 1 - (avgcmbrate / 100) 'PARTRATE() is governments share ***
                OPEXRATE(ipxy) = 1 - (avgcmbrate / 100) 'OPEXRATE() is governments share***

            Next ipxy

            'Set GPRATE() depending on CapInterest
            'If CapInterest = 0 then Ecopetrols WI in capex is fixed at first participation rate
            'If CapInterest = 1 then Ecopetrols WI in capex varies with the participation rate

            If CapInterest = 1 Then
                'adjust GPRATE() for capex for new monthly participation rates

                prodstmo = ((Y1 - YR) * 12) + M1
                '            Debug.Print "y1 = "; Y1; "  yr = "; YR; "  m1 = "; M1; "  prodstmo = "; prodstmo

                For iPX = 1 To my3tt

                    '<<<<<< 4 Aug 2001 JWD (C0363)
                    If my3(iPX, 1) < CPXCategoryCodeBAL Or my3(iPX, 1) = CPXCategoryCode_AbandonmentCashExpenditure Then ' do not include BAL
                        '~~~~~~ was:
                        ''<<<<<< 21 Jun 2001 JWD (C0339)
                        'If my3(iPX, 1) < CPXCategoryCodeBAL Then    ' do not include BAL
                        ''~~~~~~ was:
                        ''If my3(iPX, 1) < 18 Then    ' do not include BAL
                        ''>>>>>> End (C0339)
                        '>>>>>> End (C0363)

                        mon = ((my3(iPX, 3) - YR) * 12) + my3(iPX, 2) 'month index
                        'Debug.Print "GPRATE(ipx) before = "; GPRATE(ipx)
                        If mon > prodstmo Then 'only adjust GPRATE() after production starts
                            GPRATE(iPX) = cmbrate(mon) / 100
                            'Debug.Print "ipx = "; ipx; "  my3(ipx,3) = "; my3(ipx, 2); "  my3(ipx,2) = "; my3(ipx, 3); "  yr = "; yr
                            'Debug.Print "mon = "; mon; "  cmbrate(mon) = "; cmbrate(mon); "  GPRATE(ipx) = "; GPRATE(ipx)
                        End If
                    End If
                Next iPX
            End If

            ' reset original variables, adjusting them for the new participating interest       ***
            For iPX = 1 To iX - 1

                If Left(TD(iPX, 1), 3) = "REV" Then
                    For ipxy = 1 To LG
                        RVN(ipxy, iPX) = (RVN(ipxy, iPX) / oldrate(ipxy)) * (1 - PARTRATE(ipxy))
                        VLM(ipxy, iPX) = (VLM(ipxy, iPX) / oldrate(ipxy)) * (1 - PARTRATE(ipxy))
                    Next ipxy

                ElseIf Left(TD(iPX, 1), 3) = "ROY" Then
                    For ipxy = 1 To LG
                        RVN(ipxy, iPX) = (RVN(ipxy, iPX) / oldrate(ipxy)) * (1 - PARTRATE(ipxy))
                        VLM(ipxy, iPX) = (VLM(ipxy, iPX) / oldrate(ipxy)) * (1 - PARTRATE(ipxy))
                    Next ipxy

                ElseIf Left(TD(iPX, 1), 3) = "OCS" Then
                    For ipxy = 1 To LG
                        RVN(ipxy, iPX) = (RVN(ipxy, iPX) / oldrate(ipxy)) * (1 - PARTRATE(ipxy))
                        VLM(ipxy, iPX) = (VLM(ipxy, iPX) / oldrate(ipxy)) * (1 - PARTRATE(ipxy))
                    Next ipxy

                End If

            Next iPX

        End If

        GoTo 10010


        '=======================================================================

10039:  ' CHECK FOR WORKING INTEREST SPECIFICATION
10040:  If Left(TD(iX, 1), 3) <> "WIN" Then
            GoTo 10051
        End If

10041:
        ' If no working interest for this run, loop back
        If xRunSwitches(RunSwitch_WIN) = RunSwitch_WIN_Off Then
            GoTo 10010
        End If

        If WINT = 1 Then ' already been here (WIN applied at TOP?) so loop back
            GoTo 10010
        End If

10042:  WINT = 1 'working interest is on from here on, all values are net of WIN

        'if user has entered anything in Base data for WIN, load WIN()
        '   else - leave WIN(1 to LG) set to 1
        zeros = -1
        For iZ = 1 To LG
            ' GDP 20 Jan 2003
            ' A(iZ, 6) to A(iZ, gc_nAWIN)
            If A(iZ, gc_nAWIN) <> 0 Then
                zeros = 0 'false
                Exit For
            End If
        Next iZ
        If Not zeros Then
            For iPX = 1 To LG
                WIN(iPX) = A(iPX, gc_nAWIN) / 100
            Next iPX
        End If
        For iPX = 1 To my3tt
            WINC(iPX) = my3(iPX, 6) / 100
        Next iPX
        '---------------------------------------------------------------
        '8-21-92 put final WIN % on reports
        FinalWin = WIN(LG)
        '--------------------------------------------------------------

10043:  ' APPLY WORKING INTEREST TO ALL PREVIOUSLY DEFINED VARIABLES
10044:  If iX <> 1 Then
10045:      For iPX = 1 To iX - 1
10046:          For ipxy = 1 To LG
10047:              RVN(ipxy, iPX) = RVN(ipxy, iPX) * WIN(ipxy)
                    If iPX = 4 Or iPX = 3 Then
                        Debug.Print("RVN(ipxy, iPX) = RVN(ipxy, iPX) * WIN(ipxy) = " & iPX & " = " & RVN(ipxy, iPX))
                    End If
10048:              VLM(ipxy, iPX) = VLM(ipxy, iPX) * WIN(ipxy)
10049:          Next ipxy
10050:      Next iPX
        End If

        ' 29 Dec 2004 JWD (C0846) Add set of finance event bit map
        ' 22 Apr 2005 JWD (C0873) Add change of order of operations in expression
        If (g_nFinanceEvents And gc_nFinanceEvents_FIN) > 0 Then
            g_nFinanceEvents = g_nFinanceEvents + gc_nFinanceEvents_WIN
        End If
        ' End (C0846)

        '      Debug.Print "TOTPMT(iPX) = TOTPMT(iPX) * WIN(iPX)"
        For iPX = 1 To LG
            TOTPMT(iPX) = TOTPMT(iPX) * WIN(iPX)
            '         Debug.Print TOTPMT(iPX) & " = " & TOTPMT(iPX) & " * " & WIN(iPX)

            FINANCE(iPX) = FINANCE(iPX) * WIN(iPX)
            PRINC(iPX) = PRINC(iPX) * WIN(iPX)
            INTRST(iPX) = INTRST(iPX) * WIN(iPX)
        Next iPX

        BURS = 0
        If my3tt <> 0 Then
            For iPX = 1 To my3tt
                pers = my3(iPX, 3) - YR + 1
                If pers <= LG Then 'make sure not alter proj end
                    REIM(iPX) = (WINC(iPX) - WIN(my3(iPX, 3) - YR + 1)) * (my3(iPX, 7) / 100)
                End If
                If REIM(iPX) <> 0 Then
                    BURS = 1
                End If
            Next iPX
        End If


        ' This next bit of code is very risky maintenance-wise...
        ' Note the GOTO 10227, this will go and recalculate the
        ' the previous variable pointed to by iPX as if it were
        ' the next variable in the model to be calculated.
        '
        ' We save the current value of iX (it points to where we are
        ' in the model) and the set it to point to a different place
        ' (iX=iPX). Now it points at a cost recovery variable. Jump
        ' to the code that handles normal variable calculations. When
        ' it gets to the end of the normal code path (at 29999), it will
        ' return to the top of the normal model variable loop at 10010,
        ' where the bInApplyWIN variable will signal whether or not the
        ' calculation is normal mode (false) or a special recalc (true).
        ' When true, jump back to the "re-entry" location in the loop.
        '
        ' It works at this time but may not in the future...

        iX_Saved = iX ' save the current location in the model
        rptLvl_Saved = RF(5) ' save the current report level setting
        RF(5) = "   " ' set to no report to suppress cost recovery and variable sheets

ReEnterWIN:

        For ipxyz = 1 To iX_Saved - 1
            If specialReEnterWIN Then
                bInApplyWIN = False
                Continue For
            End If

            ' If cost recovery, go recalculate the variable
            If TD(ipxyz, 5) = "DPR" Or TD(ipxyz, 6) = "DPR" Then
                iX = ipxyz ' set the model location pointer to the cost recovery variable
                bInApplyWIN = True ' set the flag
                ' See if ring-fenced before jumping, don't recalculate if ring-fenced
                If Len(Trim(RF(8))) = 0 Then GoTo 10227
                ' If param 5 doesn't contain the current fiscal variable then not ring-fenced
                If InStr(1, Trim(RF(8)), Left(TD(iX, 1), 3), CompareMethod.Text) Mod 3 <> 1 Then GoTo 10227
            End If


            bInApplyWIN = False

        Next ipxyz

        iX = iX_Saved ' restore current location in the model
        RF(5) = rptLvl_Saved ' restore the actual report level setting


        GoTo 10010

        '=======================================================================

10051:  'CHECK FOR CURRENCY CONVERSION
        If Left(TD(iX, 1), 3) <> "CUR" Then
            GoTo 10055
        End If
        'If bDebugging Then
        '  Open "curr.log" For Append As #16
        '    Print #16, "FISCAL 10051   found CUR"
        '  Close #16
        'End If

        sCur = Left(TD(iX, 3), 3)
        CURT = CURT + 1 'counts "CUR" lines - only do revaluation for first "CUR"
        'If bDebugging Then
        '  Open "curr.log" For Append As #16
        '    Print #16, "currency code = "; sCur
        '  Close #16
        'End If

        ConvertCurrency()

10053:  'APPLY CURRENCY CONVERSION TO ALL PREVIOUSLY DEFINED VARIABLES

        If iX <> 1 Then
            For iPX = 1 To iX - 1
                For ipxy = 1 To LG
                    RVN(ipxy, iPX) = RVN(ipxy, iPX) * CUR(ipxy)
                Next ipxy
            Next iPX
        End If

        'APPLY CURRENCY CONVERSION TO CERTAIN INPUT VARIABLES

        If my3tt <> 0 Then
            For iPX = 1 To my3tt
                my3(iPX, 5) = my3(iPX, 5) * CUR(my3(iPX, 3) - YR + 1)
            Next iPX
        End If

        For iPX = 1 To LG
            ' GDP 20 Jan 2003
            ' Use constants for loop bounds
            For ipxy = gc_nAMINPRC To gc_nASIZE
                A(iPX, ipxy) = A(iPX, ipxy) * CUR(iPX)
            Next ipxy
        Next iPX

        SIG = SIG * CUR(1)
        DIS = DIS * CUR(Y2 - YR + 1)
        For iPX = 1 To LG
            '     SEC(iPX, 2) = SEC(iPX, 2) * CUR(iPX)
            '     SEC(iPX, 4) = SEC(iPX, 4) * CUR(iPX)
            '     SEC(iPX, 6) = SEC(iPX, 6) * CUR(iPX)

            OPEX(iPX) = OPEX(iPX) * CUR(iPX)
            LOAN(iPX) = LOAN(iPX) * CUR(iPX)
            PRINC(iPX) = PRINC(iPX) * CUR(iPX)
            INTRST(iPX) = INTRST(iPX) * CUR(iPX)
            FINANCE(iPX) = FINANCE(iPX) * CUR(iPX)
            TOTPMT(iPX) = TOTPMT(iPX) * CUR(iPX)
            BONS(iPX) = BONS(iPX) * CUR(iPX)

            For iZ = 1 To 14
                If iZ <= 5 Or iZ >= 10 Then 'do 1-5 and 10-14
                    B(iPX, iZ) = B(iPX, iZ) * CUR(iPX)
                End If
            Next iZ
        Next iPX

        '6-10-96 MKD - removed the adjustments to RT, PR and CLR. (Variable, Govt. and Ceiling Rates)
        'This applied to breakpoint dollar amounts based on the parameter selected.  Those should be
        'entered  in the currency at that level in the country file.

        '      If RTT > 0 Then
        '         For iPX = 1 To RTT
        '            If (RT(iPX, 4) >= 6 And RT(iPX, 4) <= 25) Or (RT(iPX, 4) = 34) Or (RT(iPX, 4) >= 100) Then
        '               RT(iPX, 5) = RT(iPX, 5) * CUR(iPX)
        '            End If
        '         Next iPX
        '      End If

        '      If CGRT > 0 Then
        '         For iPX = 1 To CGRT
        '            If (CGR(iPX, 4) >= 6 And CGR(iPX, 4) <= 25) Or (CGR(iPX, 4) = 34) Or (CGR(iPX, 4) >= 100) Then
        '               CGR(iPX, 5) = CGR(iPX, 5) * CUR(iPX)
        '            End If
        '         Next iPX
        '      End If

        '      If PRT > 0 Then
        '         For iPX = 1 To PRT
        '            If (PR(iPX, 4) >= 6 And PR(iPX, 4) <= 25) Or (PR(iPX, 4) = 34) Or (PR(iPX, 4) >= 100) Then
        '               PR(iPX, 5) = PR(iPX, 5) * CUR(iPX)
        '            End If
        '         Next iPX
        '      End If

        '      If PCT > 0 Then
        '         For iPX = 1 To PCT
        '            PC(iPX, 5) = PC(iPX, 5) * CUR(iPX)
        '         Next iPX
        '     End If


        '6/10/96 MKD End of changed portion.

10054:

        If AMTLT > 0 Then
            For iPX = 1 To AMTLT
                AMTLOAN(iPX, 3) = AMTLOAN(iPX, 3) * CUR(iPX)
            Next iPX
        End If

        '--------------------------------------------------------------------
        '8-21-92 if we changed currency and then changed back to USA, CUR()
        '  now contains 1/last currency in each element (to convert the above
        '  values back to the original values. However, for future variables,
        '  CUR() should be = 1
        '   IF sCur = "USA" THEN
        '     FOR iQ = 1 TO UBOUND(CUR)
        '       LCUR(iQ) = CUR(iQ)
        '       CUR(iQ) = SAMCUR(iQ)
        '     NEXT iQ
        '   END IF
        '--------------------------------------------------------------------

        GoTo 10010

        '=======================================================================
10055:

        If Left(TD(iX, 1), 3) = "LMT" Then 'ECONOMIC LIMIT

            ' See if economic limit is suppressed...
            If xRunSwitches(RunSwitch_LMT) = RunSwitch_LMT_Off Then
                GoTo 10010
            End If

            elm = 0 'limit? 0 = FALSE
            ZD = Y1 - YR + 1 'prod start year
            '8-3-92 - change to set limit at year of maximum cumulative cashflow (excluding CAPEX)
            'put CUM flows in cumcf() then find the peak year. That is the limit
            'NOTE: iX = current Fiscal Def line number
            Dim cumcf(LG) As Single
            For jj = 1 To LG
                cumcf(jj) = 0
                For kk = 1 To iX
                    If TD(kk, 4) = "+" Then
                        cumcf(jj) = cumcf(jj) + RVN(jj, kk)
                    ElseIf TD(kk, 4) = "-" Then
                        cumcf(jj) = cumcf(jj) - RVN(jj, kk)
                    ElseIf TD(kk, 4) = "T" Then
                        cumcf(jj) = cumcf(jj) - RVN(jj, kk)
                    ElseIf TD(kk, 4) = "A" Then
                        cumcf(jj) = cumcf(jj) - RVN(jj, kk)
                    ElseIf TD(kk, 4) = "U" Then
                        cumcf(jj) = cumcf(jj) - RVN(jj, kk)
                    End If
                Next kk

                'totpmt = repay col on cashflow page
                AMTS = TOTPMT(jj) - (OPEX(jj) * (1 - OPEXRATE(jj)))
                AMTS = AMTS * WIN(jj)
                cumcf(jj) = cumcf(jj) + AMTS
            Next jj

            'CUMCF() now has annual cashflow amounts
            'now convert cumcf() into Cumulative amounts

            For jj = 2 To UBound(cumcf)
                cumcf(jj) = cumcf(jj - 1) + cumcf(jj)
            Next jj
            'now find maximum year
            largestamount = cumcf(1)
            largestitem = 1

            For jj = 2 To UBound(cumcf)
                If cumcf(jj) > largestamount Then
                    largestamount = cumcf(jj)
                    largestitem = jj
                End If
            Next jj

            'if the cum for EVERY year is negative, the project is
            '  NOT economic!!!
            If largestamount <= 0 Then
                largestitem = 0
            End If

            'largestitem now points to the project year with the maximum cum cashflow
            elm = 1 'TRUE
            LG = largestitem

            If LG < ZD Then 'econ limit MUST be after prod start
                LG = ZD 'ZD points at 1st year of production
            End If
            LGI = LG
            LFI = LGI - ((((M1 - 1) / 12) + Y1) - YR)
            '--------------------------------------------------
            '8-21-92 store final partic rate in FINALPARTIC for report display
            'Since we changed LG, update these two items in common
            FINALPARTIC = PARTRATE(LG)
            FinalWin = WIN(LG)
            '--------------------------------------------------

            'WE ARE FINISHED
            '8-3-92  old method     (versions 4.0 - 5.2)
            '        FOR jj = 1 TO LG
            '          CUML = 0
            '          IF jj >= ZD THEN
            '            IF ELM <> 1 THEN
            '              FOR kk = 1 TO iX
            '                IF TD$(kk, 4) = "+" THEN
            '                  CUML = CUML + RVN(jj, kk)
            '                ELSEIF TD$(kk, 4) = "-" THEN
            '                  CUML = CUML - RVN(jj, kk)
            '                END IF
            '              NEXT kk
            '                'totpmt = repay col on cashflow page
            '              AMTS = TOTPMT(jj) - (OPEX(jj) * (1 - OPEXRATE(jj)))
            '              AMTS = AMTS * WIN(jj)
            '              CUML = CUML + AMTS
            '              IF CUML < 0 THEN
            '                ELM = 1
            '                LG = jj - 1
            '                IF LG < ZD THEN 'econ limit MUST be after prod start
            '                  LG = ZD       'ZD points at 1st year of production
            '                END IF
            '                LGI = LG
            '                LFI = LGI - ((((M1 - 1) / 12) + Y1) - YR)
            '              END IF
            '            END IF
            '          END IF
            '10056   NEXT jj

            'Now examine MY3 to be sure that no capital is being
            '  spent after the end of the project.  MY3() is already
            '  sorted by date. Find the last rec that is valid and
            '  reset MY3T to point to that record.
16151:

            '<<<<<< 2 Aug 2001 JWD (C0363)
            ' Abandonment Provisions are affected by the economic
            ' limit event.
            TriggerAbandonmentProvisionsEconomicLimitEvent()
            '~~~~~~ was:
            ''<<<<<< 26 Jun 2001 JWD (C0341)
            'ChangeAbandonmentTimingForEconomicLimit
            'OrderCapitalExpendituresByDate
            ''>>>>>> End (C0341)
            '>>>>>> End (C0363)

            lastyear = YR + LG - 1
            For iQ = 1 To my3tt
                If my3(iQ, 3) > lastyear Then
                    my3tt = iQ - 1
                    Exit For
                End If
            Next iQ
            GoTo 10010
        End If

        '=======================================================================

10057:  ' Check for Financing

        If Left(TD(iX, 1), 3) = "FIN" Then

            ' See if financing is suppressed...
            If xRunSwitches(RunSwitch_FIN) = RunSwitch_FIN_Off Then
                GoTo 10010
            End If

            ' Added if 14/11/2000 - don't run financing if consolidating
            If Not g_bPTCons Then
                ' 29 Dec 2004 JWD (C0846) Record fact that financing is present
                g_nFinanceEvents = gc_nFinanceEvents_FIN
                ' End (C0846)
                Call Financing()
            End If
            GoTo 10010
        End If



        '=======================================================================
        '<<<<<<<<<<<<<<
        ' 1/16/97  This section checks for the presence of VEN in the Variable
        '          column of Fiscal Definition.  VEN is a reserved word that
        '          triggers the calculation of the quarterly service fee in
        '          Venezuela only.

        'check for VEN in column 1 of Fiscal Definition

        If Left(TD(iX, 1), 3) <> "VEN" Then GoTo 10059

        ' 5 Dec 2002 JWD (C0640)
        ' Check to see if this variable is ring fenced
        ' and branch to the ring fence routine if it is.
        'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        l_FiscalVariableIsRingFenced = FiscalDef_VariableIsRingFencedCheck(iX)
        If l_FiscalVariableIsRingFenced = True Then GoTo 10220
        ' End (C0640)

        'Calculate Venezuela Service Fee

        'call quarterly conversion subroutine, which returns NHV(), BaseOpex(), IncrOpex(), Capital(), FeeDeflator() & qtrlg
        Dim NHV(LG) As Single
        Dim BaseOpex(LG) As Single
        Dim IncrOpex(LG) As Single
        Dim Capital(LG) As Single
        Dim FeeDeflator(LG) As Single
        QtrConvert(A, TD, RVN, my3, TDT, my3tt, LG, mo, YR, gn, NHV, BaseOpex, IncrOpex, Capital, FeeDeflator, qtrlg)

        ' given quarterly values of all variables, the following calculates the service fee


        Dim BVen(qtrlg) As Single

        DiscRate = gn(9)
        'Debug.Print "before Service Fee, DiscRate = ", DiscRate
        ServiceFee(iX, qtrlg, mo, DiscRate, RF, NHV, BaseOpex, IncrOpex, Capital, FeeDeflator, BVen)

        'Debug.Print "after Service Fee, qtrlg = ", qtrlg

        For iZ = 1 To qtrlg
            '   Debug.Print iz, BVen(iz)
            TotalBVen = TotalBVen + BVen(iZ)
        Next iZ

        ' Convert quarterly array BVen() back to an annual array for continued standard processing
        ' this subroutine returns IncrFee()

        Dim IncrFee(LG) As Single
        'Debug.Print "before annconvert"
        'Debug.Print "lg = "; lg
        'Debug.Print "qtrlg = "; qtrlg

        AnnConvert(mo, qtrlg, LG, BVen, IncrFee)

        'Debug.Print "iz", "IncrFee(iz)"

        'For iz = 1 To lg
        '   Debug.Print iz, IncrFee(iz)
        '   TotalIncrFee = TotalIncrFee + IncrFee(iz)
        'Next iz

        '   Debug.Print "TotalBVen    = "; TotalBVen
        '   Debug.Print "TotalIncrFee = "; TotalIncrFee


        ' set RVN() & VOL() - continue processing variable normally

        For ixz = 1 To LG

            RVN(ixz, iX) = IncrFee(ixz)
            ' GDP 20 Jan 2003
            ' Use constant rather than numeric value for offset
            If A(ixz, PPR + gc_nAPRICEOFFSET) <> 0 Then
                VLM(ixz, iX) = RVN(ixz, iX) / A(ixz, PPR + gc_nAPRICEOFFSET)
            Else
                VLM(ixz, iX) = 0
            End If

        Next ixz

        'For iz = 1 To lg
        '   Debug.Print iz, RVN(iz, 10), VLM(iz, 10)
        'Next iz

        ' set these variables for variable print page

        ReDim DDT(LG, 6) '0-5 is all we need
        ReDim Lcf(LG)
        For ixz = 1 To LG
            VOL(ixz) = VLM(ixz, iX)
            REV(ixz) = RVN(ixz, iX)
            If VOL(ixz) <> 0 Then
                PCE(ixz) = REV(ixz) / VOL(ixz)
            ElseIf VOL(ixz) = 0 Then
                PCE(ixz) = 0
            End If
            RLD(ixz) = REV(ixz)
            TAX(ixz) = 100
            GTax(ixz) = REV(ixz)
        Next ixz


        GoTo 25000 'write out variables for variable page

        '>>>>>>>>>>>>  End 16 Jan 1997 Venezuela service fee calc
        '=======================================================================

10059:  'If something is entered in first Calc column, then do this
        ' GDP 28 Apr 2003 - Uncommented out PRT code and added in check for registry setting
        ' GDP 08 Apr 2003
        ' Commented out PRT code as nobody uses it.
        ' Added PRT Calcs - GDP / JA 01 Oct 2001
        ' Start of PRT Calcs - **************************
        If GetSetting(gc_sREGNAME, "Preferences", "CalcPRT", "false") = "true" Then
            Select Case Left(TD(iX, 1), 3)
                Case "RYL"
                    ' 5 Dec 2002 JWD (C0640)
                    ' Check to see if this variable is ring fenced
                    ' and branch to the ring fence routine if it is.
                    'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
                    l_FiscalVariableIsRingFenced = FiscalDef_VariableIsRingFencedCheck(iX)
                    If l_FiscalVariableIsRingFenced = True Then GoTo 10220
                    ' End (C0640)
                    CalcUKRoyLiability(iX)
                    GoTo 25000
                Case "RYP"
                    ' 5 Dec 2002 JWD (C0640)
                    ' Check to see if this variable is ring fenced
                    ' and branch to the ring fence routine if it is.
                    'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
                    l_FiscalVariableIsRingFenced = FiscalDef_VariableIsRingFencedCheck(iX)
                    If l_FiscalVariableIsRingFenced = True Then GoTo 10220
                    ' End (C0640)
                    CalcUKRoyaltyPaid(iX)
                    GoTo 25000
                Case "PRP"
                    ' 5 Dec 2002 JWD (C0640)
                    ' Check to see if this variable is ring fenced
                    ' and branch to the ring fence routine if it is.
                    'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
                    l_FiscalVariableIsRingFenced = FiscalDef_VariableIsRingFencedCheck(iX)
                    If l_FiscalVariableIsRingFenced = True Then GoTo 10220
                    ' End (C0640)
                    CalcPRTPaid(iX)
                    GoTo 25000
                Case "PRI"
                    ' 5 Dec 2002 JWD (C0640)
                    ' Check to see if this variable is ring fenced
                    ' and branch to the ring fence routine if it is.
                    'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
                    l_FiscalVariableIsRingFenced = FiscalDef_VariableIsRingFencedCheck(iX)
                    If l_FiscalVariableIsRingFenced = True Then GoTo 10220
                    ' End (C0640)
                    CalcPRTInterest(iX)
                    GenerateReportPage()
                    GoTo 25000
            End Select
        End If
        ' End of PRT Calcs - ****************************


        'IF bDebugging THEN
        '   IF iX = 5 THEN
        '      OPEN "calc.log" FOR APPEND AS #17
        '      PRINT #17, " "
        '      PRINT #17, "**********************************************"
        '      PRINT #17, "10059 check for calc entries TD$(5,1) = "; TD$(5, 1)
        '      CLOSE #17
        '   END IF
        'END IF

10060:  If TD(iX, 15) <> "" Then 'USE CALC COLUMNS INSTEAD OF STANDARD METHOD

            ' 5 Dec 2002 JWD (C0640)
            ' Check to see if this variable is ring fenced
            ' and branch to the ring fence routine if it is.
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            l_FiscalVariableIsRingFenced = FiscalDef_VariableIsRingFencedCheck(iX)
            If l_FiscalVariableIsRingFenced = True Then GoTo 10220
            ' End (C0640)

            ' If bDebugging Then
            '   Open "calc.log" For Append As #17
            '   Print #17, " "
            '   Print #17, "**********************************************"
            '   Print #17, "10059 check for calc entries TD$("; iX; ",1) = "; TD$(iX, 1)
            '   Close #17
            ' End If

            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            x28000(iX) 'SET OTHER VARIABLES SO VARIABLE PAGE WILL LOOK OK

            ReDim DDT(LG, 6) '0-5 is all we need
            ReDim Lcf(LG)
            For ixz = 1 To LG
                VOL(ixz) = VLM(ixz, iX)
                REV(ixz) = RVN(ixz, iX)
                If VOL(ixz) <> 0 Then
                    PCE(ixz) = REV(ixz) / VOL(ixz)
                ElseIf VOL(ixz) = 0 Then
                    PCE(ixz) = 0
                End If
                RLD(ixz) = REV(ixz)
                TAX(ixz) = 100
                GTax(ixz) = REV(ixz)
            Next ixz
            GoTo 25000
        End If
        '<< GDP 17 Dec 2002
        ' Code to calculate an excel linked variable
        ' First check if variable is linked to excel worksheet
        If IsVariableLinked(TD(iX, 1)) Then

            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            l_FiscalVariableIsRingFenced = FiscalDef_VariableIsRingFencedCheck(iX)
            If l_FiscalVariableIsRingFenced = True Then GoTo 10220

            wksCurrent = LoadExcelWorksheet(TD(iX, 1))
            ' If the workbook linked to this variable is the same as the last
            ' then don't resend the value to the workbook.
            If Not SameAsPreviousLinkedWorkbook(iX) Then
                SendAllDataToExcel(iX, wksCurrent)
            End If
            ' Retreive results from workbook
            bRVN = GetDataFromExcel(wksCurrent, TD(iX, 1), a_fRVNExcel)
            ' Commented out because we don't want volume data
            'bVLM = GetDataFromExcel(wksCurrent, TD$(iX, 1) & "VLM", a_fVLMExcel())
            ' If the preference item is set to save the workbook then call
            ' the save routine
            If GetSetting(gc_sREGNAME, "Preferences", "SaveFiscalExcelWorkbook", "true") = "true" And Not SameAsPreviousLinkedWorkbook(iX) Then
                SaveCurrentWorkbook(wksCurrent)
            End If
            ' Copy the values into the standard variable arrays.
            ReDim DDT(LG, 6) '0-5 is all we need
            ReDim Lcf(LG)
            For ixz = 1 To LG
                If bRVN Then
                    If ixz - 1 <= UBound(a_fRVNExcel) Then
                        RVN(ixz, iX) = a_fRVNExcel(ixz - 1)
                    Else
                        RVN(ixz, iX) = 0
                    End If
                End If
                If bVLM Then
                    If ixz - 1 <= UBound(a_fVLMExcel) Then
                        VLM(ixz, iX) = a_fVLMExcel(ixz - 1)
                    Else
                        VLM(ixz, iX) = 0
                    End If
                Else
                    ' GDP 20 Jan 2003
                    ' Use constant rather that numeric value for offset
                    If A(ixz, PPR + gc_nAPRICEOFFSET) <> 0 Then
                        VLM(ixz, iX) = RVN(ixz, iX) / A(ixz, PPR + gc_nAPRICEOFFSET)
                    Else
                        VLM(ixz, iX) = 0
                    End If
                End If
                VOL(ixz) = VLM(ixz, iX)
                REV(ixz) = RVN(ixz, iX)
                If VOL(ixz) <> 0 Then
                    PCE(ixz) = REV(ixz) / VOL(ixz)
                ElseIf VOL(ixz) = 0 Then
                    PCE(ixz) = 0
                End If
                RLD(ixz) = REV(ixz)
                TAX(ixz) = 100
                GTax(ixz) = REV(ixz)
            Next ixz
            GoTo 25000
        End If

        ' End of excel link changes GDP 17 Dec 2002 >>
10210:  ' LOOP THRU INCOMES
        '10220 If Left$(RF$(6), 3) = "FLD" Then GoTo 10227
        '      If Left$(TD$(ix, 18), 3) <> "FLD" Then GoTo 10227

        ' Note: RF$(8) contains the concatenated string of fiscal variables to read in
        ' If the run file param 5 has nothing in then skip the read
10220:  If Len(Trim(RF(8))) = 0 Then GoTo 10227
        ' If param 5 doesn't contain the current fiscal variable then skip read
        If InStr(1, Trim(RF(8)), Left(TD(iX, 1), 3), CompareMethod.Text) Mod 3 <> 1 Then GoTo 10227
        ' or if the file doesn't exist

        ' 12 Mar 2004 JWD FIle always exists now... If Len(Dir$(fRingFence$)) = 0 Then GoTo 10227  'does not exist

        ' 12 Mar 2004 JWD Add next replacing following
        ' Get the ring-fence values associated with this variable
        SearchCodeString(RF(8), Left(TD(iX, 1), 3), 3, iRFV)
        For ixz = 1 To LG
            RVN(ixz, iX) = RVN(ixz, iX) + RF_Values(iRFV, c_RF_RVN, ixz)
            VLM(ixz, iX) = VLM(ixz, iX) + RF_Values(iRFV, c_RF_VLM, ixz)
        Next ixz
        'was:
        'Open fRingFence$ For Input As #2
        '    ' RF$(10) contains the group (tag) for this fiscal variable
        '    ' CHECK TO SEE IF VARIABLE IS IN HERE
        '
        'While Not EOF(2)
        '   Input #2, sDumGrp, DUM$, DUMYR, DUMLIFE, sDumCur
        '   'Changed GDP 30 Apr 2002
        '   GetCurrencyConversionSpecific sDumCur, sCur, a_fCurFactor(), DUMYR, DUMLIFE
        '   'GetCurrencyConversion sDumCur, sCur, a_fCurFactor()
        '
        '
        '   If Left$(DUM$, 3) = Left$(TD$(iX, 1), 3) And _
        ''      StrComp(Trim$(sDumGrp), Trim$(RF$(10)), vbTextCompare) = 0 Then
        '      For ixz = 1 To DUMLIFE
        '         Input #2, DUMC(ixz), DUMD(ixz)
        '         RVN(DUMYR - YR + ixz, iX) = RVN(DUMYR - YR + ixz, iX) + (DUMC(ixz) * a_fCurFactor(ixz))
        '         VLM(DUMYR - YR + ixz, iX) = VLM(DUMYR - YR + ixz, iX) + DUMD(ixz)
        '      Next ixz
        '5-19-92---------------------------
        '   Else
        '      For ixz = 1 To DUMLIFE
        '         Input #2, DUMx, DUMx
        '      Next ixz
        '5-19-92---------------------------
        '   End If
        'Wend
        'Close #2
        ' End 12 Mar 2004 JWD

        ' 6 Dec 2002 JWD (C0642) Allocate the loss carry forward ceiling array
        'UPGRADE_WARNING: Lower bound of array l_CeilingAmounts was changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim l_CeilingAmounts(LG)
        ' End (C0642)

        ' SET OTHER VARIABLES SO VARIABLE PAGE WILL LOOK OK
        For ixz = 1 To LG
            VOL(ixz) = VLM(ixz, iX)
            REV(ixz) = RVN(ixz, iX)
            If VOL(ixz) = 0 Then
                PCE(ixz) = 0
            ElseIf VOL(ixz) <> 0 Then
                PCE(ixz) = REV(ixz) / VOL(ixz)
            End If
            RLD(ixz) = REV(ixz)
            TAX(ixz) = 100
            GTax(ixz) = REV(ixz)

            ' 6 Dec 2002 JWD (C0642) Zero out the ceiling amount
            l_CeilingAmounts(ixz) = 0
            ' End (C0642)

        Next ixz

        GoTo 25000

        '=======================================================================
10227:  ReDim VOL(LG)
        ReDim REV(LG)
10230:  For i = 1 To 2
10240:      If TD(iX, i + 4) = "" Then
                GoTo 12999 'jumps to the "NEXT I" statement
10500:      ElseIf TD(iX, i + 4) <> "DPR" Then
                GoTo 10600
10510:      End If

            'THIS HANDLES COST RECOVERY
10520:      bDPCR = True
            ' GDP 20 Jan 2003
            ' A(j, gc_nAOIL) to A(j, 1)
10525:      For j = 1 To LG
                VOL(j) = A(j, gc_nAOIL)
            Next j

            Depreciation((iX), bDPCR)

10540:      For j = 1 To LG
10550:          REV(j) = REV(j) + DPC(j)
                ' GDP 20 Jan 2003
                ' Use constant for offset
10560:          If A(j, PPR + gc_nAPRICEOFFSET) <> 0 Then
                    VOL(j) = REV(j) / A(j, PPR + gc_nAPRICEOFFSET)
10570:          ElseIf A(j, PPR + gc_nAPRICEOFFSET) = 0 Then
                    VOL(j) = 0
                End If
                PCE(j) = A(j, PPR + gc_nAPRICEOFFSET)
10580:      Next j
10590:      GoTo 12999
            '--------------------------------------------------------------------
10600:
10710:      ' SEE IF VARIABLE DEFINED
            '7-25-96  Add capability to reference forward variables for any variable.
            ' This will only work within a loop of course, but that way, any variable
            ' could refer to values of forward variables.  The one caviat is that any
            ' values retrieved from previous variables will be for the current year.
            ' However, the value for forward variables will return the prior year's
            ' values.  So, if processing variable 5 which references variable 6,
            ' the value for year 2 put into variable 5 will be the year 1 value of
            ' variable 6.  This is because, the current year for the subsequent variable
            ' isnt known yet.
            'This will work in the ITB/ITE loops.  Another reference (outside of a
            ' loop) will return zeros since the variable hasnt been calculated yet.

            '7-25-96 START OLD CODE-------------------------------------------------
            '10720    IF iX = 1 GOTO 11000
            '10730    iCK = 0
            '10740    FOR j = 1 TO iX - 1
            '10750       IF TD$(iX, i + 4) = FVAR$(j) THEN
            '10760          iCK = j
            '            END IF
            '10770    NEXT j
            '
            '10780    IF iCK > 0 THEN
            '            'USE VOLUMES, PRICES AND REVENUES OF PREVIOUSLY DEFINED VARIABLE
            '            FOR j = 1 TO LG
            '               VOL(j) = VOL(j) + VLM(j, iCK)
            '               REV(j) = REV(j) + RVN(j, iCK)
            '               PCE(j) = A(j, PPR + 6)
            '            NEXT j
            '            GOTO 12999
            '         END IF
            '7-25-96 END OLD CODE----------------------------------
            '7-25-96 START NEW CODE--------------------------------

10720:      If iX = 1 Then GoTo 11000
10730:      iCK = 0
10740:      For j = 1 To TDT 'iX - 1
10750:          If TD(iX, i + 4) = TD(j, 1) Then 'FVAR$(j) Then
10760:              iCK = j
                    Exit For
                End If
10770:      Next j

10780:      If iCK > 0 Then 'USE VOLUMES, PRICES AND REVENUES OF PREVIOUSLY DEFINED VARIABLE
                If iCK < iX Then 'it is a prior variable - do it the old way
                    For j = 1 To LG
                        VOL(j) = VOL(j) + VLM(j, iCK)
                        REV(j) = REV(j) + RVN(j, iCK)
                        ' GDP 20 Jan 2003
                        ' Use constant for offset
                        PCE(j) = A(j, PPR + gc_nAPRICEOFFSET)
                    Next j
                ElseIf iCK > iX Then  'it is a subsequent variable
                    For j = 2 To LG
                        VOL(j) = VOL(j) + VLM(j - 1, iCK)
                        REV(j) = REV(j) + RVN(j - 1, iCK)
                        'GDP 20 Jan 2003
                        ' Use constant for offset
                        PCE(j) = A(j - 1, PPR + gc_nAPRICEOFFSET)
                    Next j
                End If
                GoTo 12999
            End If

            '7-25-96 END NEW CODE ---------------------------------
11000:      ' get annual array for code entered
            'you are here if the income column (1-2) is NOT a previously
            '  defined variable. (ie. it is OIL, GAS, etc.)
            ReDim dumrev(LG)
11001:
            'UPGRADE_WARNING: Couldn't resolve default property of object dumrev(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            RetrieveValues(TD(iX, i + 4), TD(iX, 3), dumrev)
18754:
            For ii = 1 To LG
11002:          'UPGRADE_WARNING: Couldn't resolve default property of object dumrev(ii). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                REV(ii) = REV(ii) + dumrev(ii)
                ' GDP 20 Jan 2003
                ' Use constant for offset
11003:          If A(ii, PPR + gc_nAPRICEOFFSET) <> 0 Then
                    VOL(ii) = REV(ii) / A(ii, PPR + gc_nAPRICEOFFSET)
11004:          ElseIf A(ii, PPR + gc_nAPRICEOFFSET) = 0 Then
                    VOL(ii) = 0
                End If
11005:          PCE(ii) = A(ii, PPR + gc_nAPRICEOFFSET)
            Next ii
12999:  Next i
        '=======================================================================
13000:  'APPLY PROPER PRICE AND RECALCULATE REVENUES

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '12-8-92  If the income item = OIL, GAS, OV1, or OV2, AND there is
        '  an entry (PRC or other) in the price column, we want to leave
        '  the VOL() in thd units of the income item instead of dividing
        '  REV() by primary price and thereby converting the volumn to
        '  primary product equivalence.  This will allow the user to enter
        '  the Price Definition in the units of the income variable instead
        '  of entering the data in the units of the primary product.  We
        '  recalculate the VOL (REV() / A(1-LG, product price). Then once VOL
        '  is back in its own units, we process the price definition. After the
        '  REV() is recalculated, we change PRC() back to primary product
        '  price, then calculate VOL() = REV() / PRC().

        If TD(iX, 7) <> "" Then 'user entered a price code
            Select Case TD(iX, 5) 'first income column code
                Case "OIL"
                    If PPR <> 1 Then 'if OIL primary, VOL(), PCE(), & REV() are OK
                        For j = 1 To LG
                            ' GDP 20 Jan 2003
                            ' A(j, 7) to A(j, gc_nAOPC)
                            If A(j, gc_nAOPC) <> 0 Then
                                VOL(j) = REV(j) / A(j, gc_nAOPC)
                            Else
                                VOL(j) = 0
                            End If
                            If VOL(j) <> 0 Then
                                PCE(j) = REV(j) / VOL(j)
                            Else
                                PCE(j) = 0
                            End If
                        Next j
                    End If
                Case "GAS"
                    If PPR <> 2 Then 'if GAS primary, VOL(), PCE(), & REV() are OK
                        For j = 1 To LG
                            ' GDP 20 Jan 2003
                            ' A(j, 8) to A(j, gc_nAGPC)
                            If A(j, gc_nAGPC) <> 0 Then
                                VOL(j) = REV(j) / A(j, gc_nAGPC)
                            Else
                                VOL(j) = 0
                            End If
                            If VOL(j) <> 0 Then
                                PCE(j) = REV(j) / VOL(j)
                            Else
                                PCE(j) = 0
                            End If
                        Next j
                    End If
                Case "OV1"
                    For j = 1 To LG
                        ' GDP 20 Jan 2003
                        ' A(j, 9) to A(j, gc_nAOP1)
                        If A(j, gc_nAOP1) <> 0 Then
                            VOL(j) = REV(j) / A(j, gc_nAOP1)
                        Else
                            VOL(j) = 0
                        End If
                        If VOL(j) <> 0 Then
                            PCE(j) = REV(j) / VOL(j)
                        Else
                            PCE(j) = 0
                        End If
                    Next j
                Case "OV2"
                    For j = 1 To LG
                        ' GDP 20 Jan 2003
                        ' A(j, 10) to A(j, gc_nAOP2)
                        If A(j, gc_nAOP2) <> 0 Then
                            VOL(j) = REV(j) / A(j, gc_nAOP2)
                        Else
                            VOL(j) = 0
                        End If
                        If VOL(j) <> 0 Then
                            PCE(j) = REV(j) / VOL(j)
                        Else
                            PCE(j) = 0
                        End If
                    Next j
                    ' GDP 20 Jan 2003
                    ' Added extra case statements for additional volumes
                Case "OV3"
                    For j = 1 To LG
                        If A(j, gc_nAOP3) <> 0 Then
                            VOL(j) = REV(j) / A(j, gc_nAOP3)
                        Else
                            VOL(j) = 0
                        End If
                        If VOL(j) <> 0 Then
                            PCE(j) = REV(j) / VOL(j)
                        Else
                            PCE(j) = 0
                        End If
                    Next j
                Case "OV4"
                    For j = 1 To LG
                        If A(j, gc_nAOP4) <> 0 Then
                            VOL(j) = REV(j) / A(j, gc_nAOP4)
                        Else
                            VOL(j) = 0
                        End If
                        If VOL(j) <> 0 Then
                            PCE(j) = REV(j) / VOL(j)
                        Else
                            PCE(j) = 0
                        End If
                    Next j
                Case "OV5"
                    For j = 1 To LG
                        If A(j, gc_nAOP5) <> 0 Then
                            VOL(j) = REV(j) / A(j, gc_nAOP5)
                        Else
                            VOL(j) = 0
                        End If
                        If VOL(j) <> 0 Then
                            PCE(j) = REV(j) / VOL(j)
                        Else
                            PCE(j) = 0
                        End If
                    Next j
                Case "OV6"
                    For j = 1 To LG
                        If A(j, gc_nAOP6) <> 0 Then
                            VOL(j) = REV(j) / A(j, gc_nAOP6)
                        Else
                            VOL(j) = 0
                        End If
                        If VOL(j) <> 0 Then
                            PCE(j) = REV(j) / VOL(j)
                        Else
                            PCE(j) = 0
                        End If
                    Next j
                Case "OV7"
                    For j = 1 To LG
                        If A(j, gc_nAOP7) <> 0 Then
                            VOL(j) = REV(j) / A(j, gc_nAOP7)
                        Else
                            VOL(j) = 0
                        End If
                        If VOL(j) <> 0 Then
                            PCE(j) = REV(j) / VOL(j)
                        Else
                            PCE(j) = 0
                        End If
                    Next j
                Case "OV8"
                    For j = 1 To LG
                        If A(j, gc_nAOP8) <> 0 Then
                            VOL(j) = REV(j) / A(j, gc_nAOP8)
                        Else
                            VOL(j) = 0
                        End If
                        If VOL(j) <> 0 Then
                            PCE(j) = REV(j) / VOL(j)
                        Else
                            PCE(j) = 0
                        End If
                    Next j
                Case "OV9"
                    For j = 1 To LG
                        If A(j, gc_nAOP9) <> 0 Then
                            VOL(j) = REV(j) / A(j, gc_nAOP9)
                        Else
                            VOL(j) = 0
                        End If
                        If VOL(j) <> 0 Then
                            PCE(j) = REV(j) / VOL(j)
                        Else
                            PCE(j) = 0
                        End If
                    Next j
                Case "OV0"
                    For j = 1 To LG
                        If A(j, gc_nAOP0) <> 0 Then
                            VOL(j) = REV(j) / A(j, gc_nAOP0)
                        Else
                            VOL(j) = 0
                        End If
                        If VOL(j) <> 0 Then
                            PCE(j) = REV(j) / VOL(j)
                        Else
                            PCE(j) = 0
                        End If
                    Next j
                Case Else
                    'do nothing - VOL(), PCE(), & REV() are correct
            End Select
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        If TD(iX, 18) = "VOL" Or TD(iX, 18) = "VOP" Then
            For jp = 1 To LG
                'REV(jp) = VOL(jp)

                REV(jp) = EquivalencyVolumeProductionByProdType(jp, TD(iX, 5))

            Next jp
        Else

13080:      If TD(iX, 7) = "PRC" Then
                ' FVAR$(iX) = Variable Code
                ' MatchPrc% = 1 if there was a matching variable on the Price Definition form
                ' PCE() = annual prices returned from subroutine
                ReDim PCE(LG)
                PriceDef(FVAR(iX), MatchPrc, PCE)
                If MatchPrc = 0 Then ' No Match Found, Use Revenues
                    For j = 1 To LG
                        If VOL(j) <> 0 Then PCE(j) = REV(j) / VOL(j)
                        If VOL(j) = 0 Then PCE(j) = 0
                    Next j
                    GoTo 13540
                Else 'found match
                    For ixz = 1 To LG
                        REV(ixz) = VOL(ixz) * PCE(ixz)
                    Next ixz
                End If
13540:      ElseIf TD(iX, 7) <> "" Then  ' USE SPECIFIED PRICE
                RetrieveValues(TD(iX, 7), "", PCE)
                For jp = 1 To LG
                    REV(jp) = VOL(jp) * PCE(jp)
                Next jp
            End If
        End If
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'see description of this section from line 13000 above
        If TD(iX, 7) <> "" Then 'user entered a price code
            Select Case TD(iX, 5) 'first income column code
                Case "OIL"
                    If PPR <> 1 Then
                        For j = 1 To LG
                            ' GDP 20 Jan 2003
                            ' Use constant for offset
                            PCE(j) = A(j, PPR + gc_nAPRICEOFFSET)
                            If PCE(j) <> 0 Then
                                VOL(j) = REV(j) / PCE(j)
                            Else
                                VOL(j) = 0
                            End If
                        Next j
                    End If
                Case "GAS"
                    If PPR <> 2 Then
                        For j = 1 To LG
                            ' GDP 20 Jan 2003
                            ' Use constant for offset
                            PCE(j) = A(j, PPR + gc_nAPRICEOFFSET)
                            If PCE(j) <> 0 Then
                                VOL(j) = REV(j) / PCE(j)
                            Else
                                VOL(j) = 0
                            End If
                        Next j
                    End If
                    ' GDP 20 Jan 2003
                    ' Add extra volume streams into case statement
                Case "OV1", "OV2", "OV3", "OV4", "OV5", "OV6", "OV7", "OV8", "OV9", "OV0"
                    For j = 1 To LG
                        ' use constant for offset
                        PCE(j) = A(j, PPR + gc_nAPRICEOFFSET)
                        If PCE(j) <> 0 Then
                            VOL(j) = REV(j) / PCE(j)
                        Else
                            VOL(j) = 0
                        End If
                    Next j
            End Select
        End If
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'LOOP THRU DEDUCTIONS
        ReDim DDT(LG, 6)
        For i = 1 To 5
            If TD(iX, i + 7) = "" Then 'td$(8-12) deductions
                GoTo 22999 'NEXT I statement
            ElseIf TD(iX, i + 7) <> "DPR" Then
                GoTo 20300
            End If

            'CALCULATE DEPRECIATION AND ADD IT TO DDT()
            bDPCR = False

            Depreciation((iX), bDPCR)

            '--------------------------------------------------------------------
20130:      For j = 1 To LG
20140:          DDT(j, i) = DPC(j)
20145:          DDT(j, 0) = DDT(j, 0) + DDT(j, i)
20150:      Next j
20290:      GoTo 22999

            'you are here if: TD$(iX, I + 7) <> "DPR"
20300:
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '8-7-92  Rule change for processing DEF line. If "DPR" is an income
            '          variable (Cost Recovery) THEN anything in the deductions column
            '          has already been taken out of the recovery. We DO NOT deduct the
            '          item again. (In prior versions, we did not pull it out of the
            '          recovery, but we deducted it here.)
            '8-10-92
            If TD(iX, 5) = "DPR" Or TD(iX, 6) = "DPR" Then 'cost recovery variable
                GoTo 22999 'ignore the deductions - they were handled in DEPREC
            End If
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            If TD(iX, i + 7) = "DPL" Then 'CALCULATE DEPLETION
                'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
                'GoSub 7000

7000:           'THIS SUBROUTINE CALCULATES DEPLETION
7010:           ReDim DPC(LG)
7020:           If DLT = 0 Then
                    GoTo 7290
                End If
7030:           iXY = 0
                TP1 = 0
                TP2 = 0

7040:           iXY = iXY + 1
7050:           If iXY > DLT Then GoTo 7290
7060:           If FVAR(iX) = sDL(iXY) Then GoTo 7080
7070:           GoTo 7040



7080:           'FOUND A MATCH

                Dim bOil As Boolean
                Dim bGas As Boolean

                bOil = False
                bGas = False

                If TD(iX, 2) = "ALL" Then
                    bOil = True
                    bGas = True
                ElseIf TD(iX, 2) = "OIL" Then
                    bOil = True
                ElseIf TD(iX, 2) = "GAS" Then
                    bGas = True
                End If


7090:           For ixz = 1 To LG
7100:               If DL(iXY, 2) = 1 Then TP1 = REV(ixz) * (DL(iXY, 1) / 100)
7110:               If DL(iXY, 2) = 2 Then TP1 = (REV(ixz) - DDT(ixz, 0)) * (DL(iXY, 1) / 100)
7120:               If DL(iXY, 2) = 3 Then TP1 = DL(iXY, 1) * PCE(ixz) * (1 - PARTRATE(ixz)) * WIN(ixz)
                    If DL(iXY, 2) = 16 Then TP1 = EquivalencyVolumeProduction(ixz, bOil, bGas) * (DL(iXY, 1) / 100)

7130:               If DL(iXY, 5) = 1 Then TP2 = REV(ixz) * (DL(iXY, 4) / 100)
7140:               If DL(iXY, 5) = 2 Then TP2 = (REV(ixz) - DDT(ixz, 0)) * (DL(iXY, 4) / 100)
7150:               If DL(iXY, 5) = 3 Then TP2 = DL(iXY, 4) * PCE(ixz) * (1 - PARTRATE(ixz)) * WIN(ixz)
                    If DL(iXY, 5) = 16 Then TP2 = EquivalencyVolumeProduction(ixz, bOil, bGas) * (DL(iXY, 4) / 100)



7200:               If DL(iXY, 3) = 1 Then GoTo 7250

7210:               'USE GREATER OF TWO
7220:               If TP1 >= TP2 Then DPC(ixz) = TP1
7230:               If TP2 > TP1 Then DPC(ixz) = TP2
7240:               GoTo 7275

7250:               'USED LESSER OF TWO
7260:               If TP1 <= TP2 Then DPC(ixz) = TP1
7270:               If TP2 < TP1 Then DPC(ixz) = TP2
7275:               If DPC(ixz) < 0 Then DPC(ixz) = 0
7280:           Next ixz

                Dim RECOUP(LG) As Single

                ' CHECK FOR RECOUPMENT
                If DL(iXY, 6) > 0.0! Then
                    iPeriod = Int(DL(iXY, 8))
                    If iPeriod <= 0.0! Then
                        iPeriod = 1
                    End If
                    For ixz = 1 To LG
                        AMTPER = (DPC(ixz) * (DL(iXY, 6) / 100)) / iPeriod
                        iBGYE = ixz + Int(DL(iXY, 7))
                        iENYE = iBGYE + iPeriod - 1
                        For iXZA = iBGYE To iENYE
                            If iXZA <= LG Then
                                RECOUP(iXZA) = RECOUP(iXZA) + AMTPER
                            Else
                                RECOUP(LG) = RECOUP(LG) + AMTPER
                            End If
                        Next iXZA
                    Next ixz
                    For ixz = 1 To LG
                        DPC(ixz) = DPC(ixz) - RECOUP(ixz)
                    Next ixz
                End If
                'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
                System.Array.Clear(RECOUP, 0, RECOUP.Length)
7290:
                'end gosub 7000
                For j = 1 To LG
                    DDT(j, i) = DPC(j)
                    DDT(j, 0) = DDT(j, 0) + DDT(j, i)
                Next j
                GoTo 22999
            End If

20400:      If TD(iX, i + 7) = "INT" Then
                For j = 1 To LG
                    DDT(j, i) = DDT(j, i) + INTRST(j)
                    DDT(j, 0) = DDT(j, 0) + DDT(j, i)
                Next j
                GoTo 22999
            End If


            'SEE IF VARIABLE DEFINED
            '7-25-96  Add capability to reference forward variables for any variable.
            ' This will only work within a loop of course, but that way, any variable
            ' could refer to values of forward variables.  The one caviat is that any
            ' values retrieved from previous variables will be for the current year.
            ' However, the value for forward variables will return the prior year's
            ' values.  So, if processing variable 5 which references variable 6,
            ' the value for year 2 put into variable 5 will be the year 1 value of
            ' variable 6.  This is because, the current year for the subsequent variable
            ' isnt known yet.
            'This will work in the ITB/ITE loops.  Another reference (outside of a
            ' loop) will return zeros since the variable hasnt been calculated yet.

            '7-25-95 Start Old Code ---------------------------------
            '         IF iX <> 1 THEN
            '            iCK = 0
            '            FOR j = 1 TO iX - 1
            '               IF TD$(iX, i + 7) = FVAR$(j) THEN
            '                  iCK = j
            '               END IF
            '            NEXT j
            '            IF iCK <> 0 THEN   'USE VARIABLE FOUND
            '               FOR j = 1 TO LG
            '                  DDT(j, i) = DDT(j, i) + RVN(j, iCK)
            '                  DDT(j, 0) = DDT(j, 0) + DDT(j, i)
            '               NEXT j
            '               GOTO 22999
            '            END IF
            '         END IF
            '7-25-95 End Old Code -----------------------------------
            '7-25-95 Start New Code ---------------------------------
            If iX <> 1 Then
                'MsgBox "Processing line: " & iX & " TDT = " & TDT
                iCK = 0
                For j = 1 To TDT 'iX - 1
                    'MsgBox "Ded var = " & TD$(iX, i + 7) & "  TD$(" & j & ", 1)= " & TD$(j, 1)
                    If TD(iX, i + 7) = TD(j, 1) Then 'FVAR$(j) Then
                        iCK = j
                        Exit For
                    End If
                Next j

                'MsgBox "Found var: " & TD$(iX, i + 7) & " at line " & iCK

                If iCK <> 0 Then 'USE VARIABLE FOUND
                    If iCK < iX Then 'prior variable
                        For j = 1 To LG
                            DDT(j, i) = DDT(j, i) + RVN(j, iCK)
                            DDT(j, 0) = DDT(j, 0) + DDT(j, i)
                        Next j
                    ElseIf iCK > iX Then  'subsequent variable
                        'MsgBox "Later variable.  Process loop starts now.                  "
                        For j = 2 To LG
                            DDT(j, i) = DDT(j, i) + RVN(j - 1, iCK)
                            DDT(j, 0) = DDT(j, 0) + DDT(j, i)
                        Next j
                    End If
                    GoTo 22999
                End If
            End If

            '7-25-95 End New Code ---------------------------------




            '-------------------------------------------------------------
20700:      'MUST BE STANDARD CODE FROM DATA FILE OR COUNTRY ANNUAL FORECASTS
            ReDim ded(LG)
            'UPGRADE_WARNING: Couldn't resolve default property of object ded(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            RetrieveValues(TD(iX, i + 7), "", ded)
            For j = 1 To LG
                'UPGRADE_WARNING: Couldn't resolve default property of object ded(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                DDT(j, i) = ded(j)
                DDT(j, 0) = DDT(j, 0) + DDT(j, i)
            Next j
22999:  Next i
        '=======================================================================
        'You are here with Incomes and deductions processed
        '---------------------------------------------------
23000:  ' CALCULATE CREDITS
23010:  ' FIRST GET THE CREDITS FROM THE DEPRECIATION SCHEDULE

        ' 29 May 2003 JWD (C0703)
        ' Change to conditionally put credits from
        ' depreciation in cdt(). Only want them if
        ' normal depreciation (as deduction). The
        ' credits have already been considered if
        ' cost recovery (as income).
        If Not bDPCR Then
23020:      For i = 1 To LG
23030:          cdt(i) = FCRD(i)
23040:      Next i
        End If

23050:  ' NOW SUM THE VARIABLES SPECIFIED AS CREDITS
23060:  For i = 1 To 2
23070:      If TD(iX, i + 12) = "" Then GoTo 23180
            If TD(iX, i + 12) <> "INT" Then GoTo 23075
            For j = 1 To LG
                cdt(j) = cdt(j) + INTRST(j)
            Next j
            GoTo 23180

23075:      ' See if Variable Defined
            '7-25-96  Add capability to reference forward variables for any variable.
            ' This will only work within a loop of course, but that way, any variable
            ' could refer to values of forward variables.  The one caviat is that any
            ' values retrieved from previous variables will be for the current year.
            ' However, the value for forward variables will return the prior year's
            ' values.  So, if processing variable 5 which references variable 6,
            ' the value for year 2 put into variable 5 will be the year 1 value of
            ' variable 6.  This is because, the current year for the subsequent variable
            ' isnt known yet.
            'This will work in the ITB/ITE loops.  Another reference (outside of a
            ' loop) will return zeros since the variable hasnt been calculated yet.



            'Start Old Code ---------------------------------
            '23080    IF iX = 1 GOTO 23175
            '23090    k = 0
            '23100    k = k + 1
            '23110    IF k > iX - 1 GOTO 23175
            '23120    IF TD$(iX, i + 12) = TD$(k, 1) GOTO 23140
            '23130    GOTO 23100
            '
            '23140    ' FOUND MATCH, USE VARIABLE DEFINED AS CREDIT
            '23150    FOR iXJ = 1 TO LG
            '23160       cdt(iXJ) = cdt(iXJ) + RVN(iXJ, k)
            '23170    NEXT iXJ
            '         GOTO 23180
            '7-25-96 End Old Code ----------------------------------
            '7-25-95 Start New Code --------------------------------
            If iX <> 1 Then
                iCK = 0
                For j = 1 To TDT 'iX - 1
                    If TD(iX, i + 12) = TD(j, 1) Then 'FVAR$(j) Then
                        iCK = j
                        Exit For
                    End If
                Next j
                If iCK <> 0 Then 'USE VARIABLE FOUND
                    If iCK < iX Then 'prior variable
                        For j = 1 To LG
                            cdt(j) = cdt(j) + RVN(j, iCK)
                        Next j
                    ElseIf iCK > iX Then  'subsequent variable
                        For j = 2 To LG
                            cdt(j) = cdt(j) + RVN(j - 1, iCK)
                        Next j
                    End If
                End If
                GoTo 23180
            End If
            '7-25-95 Start New Code ---------------------------------

23175:      'MUST BE STANDARD CODE FROM DATA FILE OR COUNTRY ANNUAL FORECASTS
            ReDim CRE(LG)
            'UPGRADE_WARNING: Couldn't resolve default property of object CRE(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            RetrieveValues(TD(iX, i + 12), "", CRE())
            For j = 1 To LG
                'UPGRADE_WARNING: Couldn't resolve default property of object CRE(j). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                cdt(j) = cdt(j) + CRE(j)
            Next j
23180:  Next i

23300:  'CALCULATE RLD()
23310:  For iXY = 1 To LG
23320:      RLD(iXY) = REV(iXY) - DDT(iXY, 0)
23330:  Next iXY

23500:  'FIRST FIND LINE IN TM() THAT MATCHES, SET EQUAL TO iXYZ
23510:  iXYZ = 0

23520:  iXYZ = iXYZ + 1
23530:  If iXYZ > TMT Then GoTo 23560
23540:  If sTMV(iXYZ) = FVAR(iX) Then GoTo 23600
23550:  GoTo 23520

23560:  'SET DEFAULTS FOR MISCELLANEOUS IF NONE FOUND
23570:  iXYZ = 0
23580:  losscf = 5
        itl = 1
        GoTo 23608

23600:  losscf = Int(TM(iXYZ, 1))
        itl = Int(TM(iXYZ, 2))
        ' NOW LOOP THRU TAX RATES EACH YEAR
23608:  '
        If RTT = 0 Then
            For iPX = 1 To LG
3112:           TAX(iPX) = 100
            Next iPX
        End If
        If RTT = 0 Then GoTo 23609
3113:   Dim sRateInV(RTT) As String
3114:   Dim ratein(RTT, 6) As Single
3115:   Dim VarRates(LG) As Single
        param = 0
        ReDim inc1(LG)
        ReDim ded1(LG)
        'iPPX is the number of var rates line for this variable
        iPPX = 0
        For iPX = 1 To RTT
            If sRTV(iPX) = FVAR(iX) Then
3116:           iPPX = iPPX + 1
3117:           sRateInV(iPPX) = sRTV(iPX)
                For iZ = 1 To 6
3118:               ratein(iPPX, iZ) = RT(iPX, iZ)
                Next iZ
            End If
        Next iPX
        If iPPX = 0 Then
            For iPX = 1 To LG
3119:           TAX(iPX) = 100
            Next iPX
        End If

        If iPPX = 0 Then GoTo 23609
        DefAmount = 100
3120:   '
        '6-4-92
        'check if RLD() is all 0s. If so, then user did not
        '  fill in any income or deduction fields on the line.
        'IF ALL ZEROS, IN FINISHIT WE PLUG REV() AND DDT();
        '  IF NOT, WE LEAVE THEM ALONE!

        '------------------------------------------------
        '4/6/95  old test didnt work if 0 value item was
        '  in income or deduction fields
        IncDedEntered(iX, IncDedBlank)

        'IncDedBlank% = -1
        'FOR iQ = 1 TO LG
        '  IF RLD(iQ) <> 0 THEN
        '    IncDedBlank% = 0
        '    EXIT FOR
        '  END IF
        'NEXT iQ
        '4/6/95 end ---------------------------------------
16161:
        RateCalc(iX, FISCAL, FVAR(iX), DefAmount, sRateInV, ratein, iPPX, param, VarRates)
16162:
        ' GDP 20 Jan 2003
        ' Change rate param comparison to adjusted values for new volumes
        'If param% >= 59 And param% <= 61 Then GoTo 23609
        ' 27 May 2003 JWD (C0700) Replace numbers with symbols
        If param >= gc_nRtPrmRTO And param <= gc_nRtPrmIRR Then GoTo 23609
        ' for IRR, VarRates() returning from RateCalc
        ' are the Tax Amounts, not the tax Rates

        For j = 1 To LG
3121:       TAX(j) = VarRates(j)
        Next j
23609:  'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        finishit(iX, l_CeilingAmounts, param, GTax, TAX, VarRates, IncDedBlank, cdt, ald, Lcf, losscf, LCT, ipxy, iBGN, iENDE, TMA, itl, num, DUMC, DUMD, DUME, iFirstLoop)
        '-------------------------------------------------------------------------
        '3-9-93 OXY Database item
        If FVAR(iX) = "UST" Then
            USTxRate = TAX(LG)
        End If

        '-------------------------------------------------------------------------

     
25000:  ' THIS PRINTS VARIABLE REPORT
        'Giant 5.4 start --------------------
        'If we are in an iteration loop, we will end up here once for each
        '  year of the project.  We don't want a page for each loop through,
        '  we want a page only for the last iteration (final year loop).
        '  So, bail out if XIter = 1.
        If XIter = 1 And XYear <> LG Then
            'IF bDebugging THEN
            '  IF iX = 5 THEN
            '    OPEN "calc.log" FOR APPEND AS #17
            '    PRINT #17, "line 25000  iX = "; iX; "  goto 10010 (not in last iteration)"
            '    CLOSE #17
            '  END IF
            'END IF
            GoTo 10010
        ElseIf XFirst > 0 And XLast > 0 Then  'there is a loop!
            If iX >= XFirst And iX <= XLast Then
                If XYear <> LG Then
                    'IF bDebugging THEN
                    '  IF iX = 5 THEN
                    '    OPEN "calc.log" FOR APPEND AS #17
                    '      PRINT #17, "line 25000  iX = "; iX; "  goto 10010 (not in last iteration) 2nd test"
                    '    CLOSE #17
                    '  END IF
                    'END IF
                    GoTo 10010
                End If
            End If
        End If


        'Giant 5.4 end --------------------

        If (Left(RF(5), 3) = "ALL" Or Left(RF(5), 3) = "VAR") And TD(iX, 18) <> "NOP" And TD(iX, 18) <> "VOP" Then
            GoTo 25010
        Else
            GoTo 29940
        End If
25010:
25012:  'PRINT MULTIPLE DEDUCTIONS
25014:
25080:

        '<<<<<< 1 Apr 2002 JWD (C0528)
        ' <design change, content of page header changed>
        ' Assign the code to the page header title element
        TLL = TD(iX, 1)
        '~~~~~~ was:
        '25081        'THIS SUBROUTINE MATCHES REPORT TITLES WITH VARIABLES
        '      jj = 0
        '25082 jj = jj + 1
        '      If jj > TLT Then GoTo 25085
        '      If TD$(iX, 1) = TL$(jj, 1) Then GoTo 25087
        '      GoTo 25082
        '
        '25085 TLL$ = ""
        '      GoTo 25088
        '
        '25087 TLL$ = TL$(jj, 3)
        '25088
        '>>>>>> End (C0528)

        '-----------
        '     TTL$ = ""
        '     FOR jj = 1 TO TLT
        '       IF TD$(iX, 1) = TL$(jj, 1) THEN
        '         TLL$ = TL$(jj, 3)
        '         EXIT FOR
        '       END IF
        '     NEXT jj
        '--------------------
        'IF bDebugging THEN
        '  IF iX = 5 THEN
        '    OPEN "calc.log" FOR APPEND AS #17
        '      PRINT #17, "line 25092  iX = "; iX; "  print sheet.  RLD(5) = "; RLD(5)
        '    CLOSE #17
        '  END IF
        'END IF

25092:
25102:  ' PRINT USER DEFINED DEDUCTIONS

        If TD(iX, 18) = "VOL" Or TD(iX, 18) = "VOP" Then
            bUseVariableVolumetrics = True
        Else
            bUseVariableVolumetrics = False
        End If

        If Not bUseVariableVolumetrics Then
25125:      ColumnNm(1) = " PROD"
25130:      ColumnNm(2) = " PRIC"
25140:      ColumnNm(3) = "  INC"
25145:      ColumnNm(4) = "  " & TD(iX, 8)
25150:      ColumnNm(5) = "  " & TD(iX, 9)
25155:      ColumnNm(6) = "  " & TD(iX, 10)
25160:      ColumnNm(7) = "  " & TD(iX, 11)
25165:      ColumnNm(8) = "  " & TD(iX, 12)
25170:      ColumnNm(9) = " DEDT"
25175:      ColumnNm(10) = " INDE"
25180:      ColumnNm(11) = " RATE"
25185:      ColumnNm(12) = " GROS"
25190:      ColumnNm(13) = "  LCF"
25195:      ColumnNm(14) = "  NET"
            ' 19 Nov 2002 JWD (C0633)
            ' Add heading for loss carry forward ceiling column.
            ' Conveniently, ColumnNm$() is dimensioned to 15 columns
            ' already.
            ColumnNm(15) = "LCFCLNG"

        Else
            ColumnNm(1) = "VPROD"
            ColumnNm(2) = "VPRIC"
            ColumnNm(3) = " VINC"
            ColumnNm(4) = "  " & TD(iX, 8)
            ColumnNm(5) = "  " & TD(iX, 9)
            ColumnNm(6) = "  " & TD(iX, 10)
            ColumnNm(7) = "  " & TD(iX, 11)
            ColumnNm(8) = "  " & TD(iX, 12)
            ColumnNm(9) = "VDEDT"
            ColumnNm(10) = "VINDE"
            ColumnNm(11) = "VRATE"
            ColumnNm(12) = "VGROS"
            ColumnNm(13) = " VLCF"
            ColumnNm(14) = " VNET"
            ColumnNm(15) = "VLCFCLNG"
        End If


        ' End (C0633)

        '8-12-92 If Cost Recovery, we do not print values or titles in
        '  deduction columns on report . They were deducted from cost
        '  recovery in deprec.bas.
        If TD(iX, 5) = "DPR" Or TD(iX, 6) = "DPR" Then 'cost recovery variable
            For iQ = 4 To 8
                ColumnNm(iQ) = "  "
            Next iQ
        End If

25200:
25205:  'Page type, Start year, Page counter, life of field, number of columns, page title, column length
        ' 19 Nov 2002 JWD (C0633) Up the column count to 15 for addition of LCF ceiling column
25210:  ''''Write #5, 12, YR, PgCounter%, LG, 15, TLL$, 8, FinalWin, FINALPARTIC, sCur
        'Write #5, 12, YR, PgCounter%, LG, 14, TLL$, 8, FinalWin, FINALPARTIC, sCur
        ' End (C0633)

25215:  'columns titles
        ' 19 Nov 2002 JWD (C0633) Add output of LCF ceiling column heading
25217:  ''''Write #5, ColumnNm$(1), ColumnNm$(2), ColumnNm$(3), ColumnNm$(4), ColumnNm$(5), ColumnNm$(6), ColumnNm$(7), ColumnNm$(8), ColumnNm$(9), ColumnNm$(10), ColumnNm$(11), ColumnNm$(12), ColumnNm$(13), ColumnNm$(14), ColumnNm$(15)
        'Write #5, ColumnNm$(1), ColumnNm$(2), ColumnNm$(3), ColumnNm$(4), ColumnNm$(5), ColumnNm$(6), ColumnNm$(7), ColumnNm$(8), ColumnNm$(9), ColumnNm$(10), ColumnNm$(11), ColumnNm$(12), ColumnNm$(13), ColumnNm$(14)
        ' End (C0633)

        oPg1 = g_oReport.NewStandardRptPageSpecial(12)
        oPg1.SetPageHeader(12, YR, PgCounter, LG, 15, TLL, 8, FinalWin, FINALPARTIC, sCur)
        oPg1.SetProfileHeaders(ColumnNm(1), ColumnNm(2), ColumnNm(3), ColumnNm(4), ColumnNm(5), ColumnNm(6), ColumnNm(7), ColumnNm(8), ColumnNm(9), ColumnNm(10), ColumnNm(11), ColumnNm(12), ColumnNm(13), ColumnNm(14), ColumnNm(15))
25230:

        ' Added GDP 08 Jan 2003
        ' If this variable is a calc variable or a variable linked to an excel spreadsheet then
        ' write out everything as zero except for the net amount.
25300:  If TD(iX, 15) <> "" Or IsVariableLinked(TD(iX, 1)) Then
            For y = 1 To LG
                ''''Write #5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, RVN(y, iX), 0
                oPg1.SetProfileValues(y, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, RVN(y, iX), 0)
            Next y
        Else
            ' Added GDP 24 Feb 2003
            ' Is PRC present in price column?
            ' 5 Jun 2003 JWD (C0710) Change next
            ' Print VOL() and PCE() when any code in price column
            'If TD$(iX, 7) = "PRC" Then
            If Len(Trim(TD(iX, 7))) > 0 Then
                For y = 1 To LG
                    ' 19 Nov 2002 JWD (C0633) Add output of LCF ceiling column data
25320:              ''''Write #5, VOL(y), PCE(y), REV(y), DDT(y, 1), DDT(y, 2), DDT(y, 3), DDT(y, 4), DDT(y, 5), DDT(y, 0), RLD(y), TAX(y), GTax(y), Lcf(y), RVN(y, iX), l_CeilingAmounts(y)
                    'Write #5, VOL(y), PCE(y), REV(y), DDT(y, 1), DDT(y, 2), DDT(y, 3), DDT(y, 4), DDT(y, 5), DDT(y, 0), RLD(y), TAX(y), GTax(y), Lcf(y), RVN(y, iX)
                    ' End (C0633)

                    If bUseVariableVolumetrics Then
                        '                    If TD$(iX, 18) = "VOP" Then
                        '                        oPg1.SetProfileValues y, 0, 0, REV(y), DDT(y, 1), DDT(y, 2), DDT(y, 3), DDT(y, 4), DDT(y, 5), DDT(y, 0), RLD(y), TAX(y), GTax(y), Lcf(y), RVN(y, iX), l_CeilingAmounts(y)
                        '                    Else
                        'oPg1.SetProfileValues y, VOL(y), 0, REV(y), DDT(y, 1), DDT(y, 2), DDT(y, 3), DDT(y, 4), DDT(y, 5), DDT(y, 0), RLD(y), TAX(y), GTax(y), Lcf(y), RVN(y, iX), l_CeilingAmounts(y)
                        oPg1.SetProfileValues(y, 0, 0, REV(y), DDT(y, 1), DDT(y, 2), DDT(y, 3), DDT(y, 4), DDT(y, 5), DDT(y, 0), RLD(y), TAX(y), GTax(y), Lcf(y), RVN(y, iX), l_CeilingAmounts(y))
                        '                    End If
                    Else
                        oPg1.SetProfileValues(y, VOL(y), PCE(y), REV(y), DDT(y, 1), DDT(y, 2), DDT(y, 3), DDT(y, 4), DDT(y, 5), DDT(y, 0), RLD(y), TAX(y), GTax(y), Lcf(y), RVN(y, iX), l_CeilingAmounts(y))
                    End If

25345:          Next y
            Else
                ' Added GDP 24 Feb 2003
                ' Write out 0 for Production and Price if PRC not present in price column
                For y = 1 To LG
                    ''''Write #5, 0, 0, REV(y), DDT(y, 1), DDT(y, 2), DDT(y, 3), DDT(y, 4), DDT(y, 5), DDT(y, 0), RLD(y), TAX(y), GTax(y), Lcf(y), RVN(y, iX), l_CeilingAmounts(y)


                    If bUseVariableVolumetrics Then
                        '                    If TD$(iX, 18) = "VOP" Then
                        '                        oPg1.SetProfileValues y, 0, 0, REV(y), DDT(y, 1), DDT(y, 2), DDT(y, 3), DDT(y, 4), DDT(y, 5), DDT(y, 0), RLD(y), TAX(y), GTax(y), Lcf(y), RVN(y, iX), l_CeilingAmounts(y)
                        '                    Else
                        'oPg1.SetProfileValues y, VOL(y), 0, REV(y), DDT(y, 1), DDT(y, 2), DDT(y, 3), DDT(y, 4), DDT(y, 5), DDT(y, 0), RLD(y), TAX(y), GTax(y), Lcf(y), RVN(y, iX), l_CeilingAmounts(y)
                        oPg1.SetProfileValues(y, 0, 0, REV(y), DDT(y, 1), DDT(y, 2), DDT(y, 3), DDT(y, 4), DDT(y, 5), DDT(y, 0), RLD(y), TAX(y), GTax(y), Lcf(y), RVN(y, iX), l_CeilingAmounts(y))
                        '                    End If
                    Else
                        oPg1.SetProfileValues(y, 0, 0, REV(y), DDT(y, 1), DDT(y, 2), DDT(y, 3), DDT(y, 4), DDT(y, 5), DDT(y, 0), RLD(y), TAX(y), GTax(y), Lcf(y), RVN(y, iX), l_CeilingAmounts(y))
                    End If

                Next y
            End If
        End If
29940:  ' CHECK FOR WRITING OUT OF RING-FENCE VARIABLES
        ' Note RF$(9) is the concatenated string of fiscal variables to output
        ' RF$(11) is the group these variables belong to
        ' If the output string contains this fiscal variable then
        ' Added GDP 10/9/99 - Check flag appended to entityID part of the RUN file
        If Right(Trim(RF(2)), 1) = "N" Then
            ' see if doing special cost recovery variable recalc on PAR or WIN
            If bInApplyPAR Or bInApplyWIN Then
                GoTo 29998 ' skip ring fence write. it has already been done for this variable. don't write again
            End If
            If InStr(1, Trim(RF(9)), Left(TD(iX, 1), 3), CompareMethod.Text) Mod 3 = 1 Then
                With l_oRingFenceFile.OpenForAppend
                    .NextItem = Trim(RF(11))
                    .NextItem = Left(TD(iX, 1), 3)
                    .NextItem = YR
                    .NextItem = LG
                    .NextItemLineEnd = sCur
                    For ixz = 1 To LG
                        .NextItem = RVN(ixz, iX)
                        .NextItemLineEnd = VLM(ixz, iX)
                    Next ixz
                    .CloseFile()
                End With
            End If
        End If
        ' INCREMENT PAGE COUNTER
29998:  PgCounter = PgCounter + 1 'increment if next variable
29999:  GoTo 10010

        '=======================================================================




        '=======================================================================

35000:  ' Error Handler

        ' 9 Feb 2004 JWD (C0779) Replace with re-raise of error to caller
        Err.Raise(Err.Number) ' TerminateExecution


        ' 5 Dec 2002 JWD (C0640) Add next routine to test to see
        ' if a variable is ring-fenced.
        ' The exit sub is added before the routine to ensure that
        ' if there is a return from the above call (there should
        ' not be) the program will exit this procedure, and not
        ' fall through the following subroutine.

        Exit Sub


        ' End (C0640)

        ' 5 Dec 2002 JWD (C0640)
        ' Check to see if this variable is ring fenced
        ' and branch to the ring fence routine if it is.
        'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        l_FiscalVariableIsRingFenced = FiscalDef_VariableIsRingFencedCheck(iX)
        If l_FiscalVariableIsRingFenced = True Then GoTo 10220
        ' End (C0640)

    End Sub
    Sub finishit(ByVal iX As Short, ByRef l_CeilingAmounts As Single(), ByRef param As Single, ByRef GTax() As Single, ByRef TAX() As Single, ByRef VarRates() As Single, ByRef IncDedBlank As Short, ByRef cdt As Object, ByRef ald As Object, ByRef Lcf As Object, ByVal losscf As Short, ByRef LCT As Short, ByVal ipxy As Object, ByRef iBGN As Object, ByRef iENDE As Integer, ByRef TMA As Object, ByVal itl As Object, ByRef num As Object, ByRef DUMC() As Single, ByRef DUMD() As Single, ByRef DUME() As Single, ByRef iFirstLoop As Object)

        ' 19 Nov 2002 JWD (C0633)
        ' See if a ceiling on loss carry forward is defined
        ' for the variable.

        Dim l_CeilingIsDefined As Boolean
        l_CeilingIsDefined = zzzGetCeilingAmounts(iX, l_CeilingAmounts)

        Dim j As Short
        Dim jk As Short
        Dim js As Short
        Dim iQ As Short

        If Not l_CeilingIsDefined Then
            ' If no ceiling, clear any data that might
            ' be in the ceiling amounts vector
            For j = 1 To LG
                l_CeilingAmounts(j) = 0
            Next j
        End If

        ' End (C0633)


        ' NOW CALCULATE NET AMOUNTS
        Dim l_MaxReducibleAmount As Single
        For j = 1 To LG
            'If param% >= 59 And param% <= 61 Then       ' for IRR
            ' 27 May 2003 JWD (C0700) Replace numbers with symbols
            If param >= gc_nRtPrmRTO And param <= gc_nRtPrmIRR Then ' for IRR
                GTax(j) = VarRates(j)
                '           Rev(j) = INC1(j)
                '           Ddt(j, 1) = DED1(j)
                '           FOR jk = 2 TO 5
                '              Ddt(j, jk) = 0
                '           NEXT jk
                '- 6-4-92 ---------------------
11116:
                If IncDedBlank = -1 Then 'set before call to RateCalc
11117:              REV(j) = inc1(j)
11118:              DDT(j, 1) = ded1(j)
                    For jk = 2 To 5
11119:                  DDT(j, jk) = 0
                    Next jk
                End If


24040:          ''6-4-92 RLD(j) = INC1(j) - DED1(j)

24050:          If RLD(j) <> 0 Then
24060:              TAX(j) = (GTax(j) / RLD(j)) * 100
                Else
24070:              TAX(j) = 0
                End If
            Else ' normal calculations
24080:          GTax(j) = RLD(j) * (TAX(j) / 100)
                'If bDebugging Then
                '  Open "calc.log" For Append As #17
                '  Print #17, "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
                '  Print #17, "   24080 year = "; j; "RLD("; j; ") = "; RLD(j); "  Tax = "; TAX(j); "   GTax(j) = "; GTax(j)
                '  Close #17
                'End If

            End If

            ' 29 May 2003 JWD (C0703)
            ' Change to unconditionally deduct the
            ' credits. Test for cost recovery situation
            ' is now done after line 23010. This test
            ' prevented certain legitimate credits from
            ' reducing the amount.
24352:      'If Not bDPCR Then
            '   GTax(j) = GTax(j) - cdt(j)
            'End If
            GTax(j) = GTax(j) - cdt(j)
24360:      ald(j) = 0
            Lcf(j) = 0
            RVN(j, iX) = GTax(j)
            If iX = 4 Or iX = 3 Then
                Debug.Print("RVN(j, iX) = GTax(j) = " & iX & " = " & RVN(ipxy, iX))
            End If

24362:      If losscf < 0 Then GoTo 24450

24364:      'THIS IS FOR LOSS CARRY FORWARDS
24366:      If losscf >= LG Then
                LCT = LG
24368:      ElseIf losscf < LG Then
                LCT = Int(losscf)
            End If
24370:      If LCT > 0 Then GoTo 24378
24372:      Lcf(j) = 0
24374:      If RVN(j, iX) < 0 Then RVN(j, iX) = 0
24376:      GoTo 24432

24378:      If RVN(j, iX) >= 0 Then GoTo 24388
24380:      'THERE IS A TAX LOSS
24382:      ald(j) = 0.0! - RVN(j, iX)
24384:      RVN(j, iX) = 0
24386:      GoTo 24416

24388:      'THERE IS A POSITVE TAX THIS YEAR, CHECK FOR PRIOR YEAR LCF
24390:      iBGN = j - LCT
24392:      If iBGN < 1 Then iBGN = 1
24394:      iENDE = j - 1
24396:      If j = 1 Then GoTo 24432

            ' 19 Nov 2002 JWD (C0633) Add block to apply ceiling on
            ' loss carry forward.
            ' Did it this way to minimize impact on existing code.
            ' If no ceiling definition, the normal path is followed.
            If l_CeilingIsDefined Then
                ' Determine maximum reduction of current year tax
                TMA = RVN(j, iX) - l_CeilingAmounts(j)
                If TMA > 0 Then
                    ' Ceiling is in effect, only some tax can be offset
                    l_MaxReducibleAmount = l_CeilingAmounts(j)
                    ' Go ahead and reduce the tax by the maximum
                    RVN(j, iX) = TMA
                Else
                    ' Didn't hit the ceiling, all tax may be offset
                    l_MaxReducibleAmount = RVN(j, iX)
                    ' Go ahead and reduce the tax
                    RVN(j, iX) = 0
                    If iX = 4 Or iX = 3 Then
                        Debug.Print("RVN(j, iX) = GTax(j) = " & iX & " = " & RVN(ipxy, iX))
                    End If
                End If

                For js = iBGN To iENDE
                    ' Check each year prior to this for losses
                    ' that can be used to reduce the current year
                    ' tax.
                    If l_MaxReducibleAmount > 0 Then
                        ' Still have some tax that can be reduced
                        If ald(js) > 0 Then
                            ' Still have some prior year loss to offset with
                            TMA = l_MaxReducibleAmount - ald(js)
                            If TMA > 0 Then
                                ' All of the prior year's loss is used to reduce tax...
                                ald(js) = 0
                                ' ... and some tax remains.
                                l_MaxReducibleAmount = TMA
                            Else
                                ' All of the current year tax is reduced
                                ' by the prior year loss, and still some
                                ' prior year loss remains.
                                ald(js) = 0 - TMA
                                l_MaxReducibleAmount = 0
                            End If
                        End If
                    End If
                Next js

                ' At this point, if any reducible amount remains,
                ' it is added back to any tax that was not reducible
                ' (due to imposition of the ceiling).
                If l_MaxReducibleAmount > 0 Then
                    RVN(j, iX) = RVN(j, iX) + l_MaxReducibleAmount
                    If iX = 4 Or iX = 3 Then
                        Debug.Print("RVN(j, iX) = RVN(j, iX) + l_MaxReducibleAmount = " & iX & " = " & RVN(ipxy, iX))
                    End If
                End If

                ' Bypass normal block to go to loop accumulating
                ' current period's amount that is carried forward.
                GoTo 24416
            End If
            ' End (C0633)

24398:      For js = iBGN To iENDE
24400:          If RVN(j, iX) = 0 Then GoTo 24414
24402:          If ald(js) <= 0 Then GoTo 24414
24404:          TMA = RVN(j, iX) - ald(js)
24406:          If TMA >= 0 Then GoTo 24412
24408:          ald(js) = 0.0! - TMA : RVN(j, iX) = 0
                Debug.Print("ald(js) = 0! - TMA: RVN(j, iX) = 0")
                Debug.Print(RVN(j, iX))
24410:          GoTo 24414
24412:          ald(js) = 0 : RVN(j, iX) = TMA
                Debug.Print("ald(js) = 0: RVN(j, iX) = TMA")
                Debug.Print(RVN(j, iX))
24414:      Next js

24416:      'TOTAL THE LOSS CARRY FORWARDS FOR THIS YEAR
24418:      iBGN = j - LCT + 1
24420:      If iBGN < 1 Then iBGN = 1
            For js = iBGN To j
24424:          If itl <= 1 Then GoTo 24428
24426:          ald(js) = ald(js) * (1 + (Inflate(j, PPR) / 100)) ' this inflates tax losses
24428:          Lcf(j) = Lcf(j) + ald(js)
24430:      Next js
24432:      GoTo 24470

24450:      'CREDIT TAXES CURRENTLY
24460:      Lcf(j) = 0
            RVN(j, iX) = GTax(j)
24470:  Next j
        '--------------------------------------------------------------------
        'Prepaid / Deferred Tax section

        'search PD() for the current FVAR$()
        num = 0
        For iQ = 1 To PDTT
            If sPDV(iQ) = FVAR(iX) Then
                num = iQ
                Exit For
            End If
        Next iQ
        If num = 0 Or num > PDTT Then
            GoTo 24480 'var not in PD()
        End If

        'num% now points to PD() rec that matches
        If PD(num, 1) <= 0 Or PD(num, 2) <= 0 Then '0 prepaid
            ReDim DUMC(LG)
            ReDim DUMD(LG)
            ReDim DUME(LG)
            If PD(num, 4) > 0 And PD(num, 5) > 0 Then 'deferred% > 0 & years > 0
                'we do this part if there is NO prepaid % and there is
                '  a deferred % and deferred years
                For j = 1 To LG
                    Dim TEMP As Single
                    TEMP = RVN(j, iX) * ((100 - PD(num, 4)) / 100) 'amount of current year liability that is NOT deferred
                    If (j + Int(PD(num, 5))) > LG Then
                        DUMC(LG) = DUMC(LG) + (RVN(j, iX) - TEMP) 'amount of current year liability that IS deferred
                    Else
                        DUMC(j + Int(PD(num, 5))) = DUMC(j + Int(PD(num, 5))) + (RVN(j, iX) - TEMP)
                    End If
                    RVN(j, iX) = DUMC(j) + TEMP
                Next j
            End If
        Else 'use prepaid and deferred
            'DUMC() is the annual payments (prepaid $)
            'DUMD() is amount (subject to deferral) paid this year
            'DUME() array of deferred Balances
            ReDim DUMC(LG)
            ReDim DUMD(LG)
            ReDim DUME(LG)

            For j = 1 To LG
                Dim oblig As Single
                oblig = 0
                'current year obligation (before prepayment or deferral)

                Dim SumTax As Single
                SumTax = 0
                'total tax liability for prior n years

                Dim avgtax As Single
                avgtax = 0
                'average tax liability for prior n years

                Dim prepaid As Single
                prepaid = 0
                'prepaid amount for the current year

                Dim balance As Single
                balance = 0
                'deferred amount from current year

                Dim balcur As Single
                balcur = 0
                Dim baldef As Single
                baldef = 0
                Dim defyear As Single
                defyear = 0

                'determine average tax

                Dim iEndLoop As Object
                If PD(num, 3) = 1 Then 'YES
                    iFirstLoop = j - Int(PD(num, 2)) + 1
                    iEndLoop = j
                Else 'NO
                    iFirstLoop = j - Int(PD(num, 2))
                    iEndLoop = j - 1
                End If
                If iFirstLoop <= 1 Then
                    iFirstLoop = 1
                End If

                'oblig is the $ due if no deferral or prepayment is done
                oblig = RVN(j, iX)

                Dim jj As Short
                'sumtax is total obligation for the years to be averaged
                SumTax = 0
                If iEndLoop >= iFirstLoop Then
                    For jj = iFirstLoop To iEndLoop
                        SumTax = SumTax + RVN(jj, iX)
                    Next jj
                Else
                    SumTax = 0
                End If

                'avgtax is average obligation of the years specified
                avgtax = 0
                If PD(num, 2) > 0 Then
                    avgtax = SumTax / (Int(PD(num, 2))) 'Average Tax for Prepaid
                End If

                'prepaid is $ of current obligation paid in the current year
                prepaid = avgtax * (PD(num, 1) / 100) 'Amount of Tax Paid in Current Year

                'DUMC() is the annual payments (prepaid $)
                DUMC(j) = prepaid

                'balance is amount subject to deferral
                'OLD  Balance = AvgTax - prepaid
                balance = oblig - prepaid

                'balcur is amount (subject to deferral) paid this year
                balcur = balance * (1 - (PD(num, 4) / 100))

                'DUMD() is amount (subject to deferral) paid this year
                DUMD(j) = balcur

                'Balance deferred TO A LATER PERIOD
                baldef = balance - balcur

                'Defyear is the project year when the balance is to be paid
                defyear = j + Int(PD(num, 5)) 'Year in which deferred balance is placed
                If defyear > LG Then
                    defyear = LG
                End If

                'DUME() array of deferred Balances
                DUME(defyear) = baldef
            Next j

            Dim TaxTot As Object
            TaxTot = 0
            Dim DefTot As Object
            DefTot = 0
            For j = 1 To LG
                TaxTot = TaxTot + RVN(j, iX) 'sum of unadjusted Tax
                RVN(j, iX) = DUMC(j) + DUMD(j) + DUME(j)
                DefTot = DefTot + RVN(j, iX) 'sum of adjusted Tax
            Next j
            RVN(LG, iX) = RVN(LG, iX) + TaxTot - DefTot 'place leftovers in last year
        End If

        'we are through with prepaid / deferred taxes
        '--------------------------------------------------------------------
24480:  'CALCULATE NET AMOUNT AND VOLUME
        For j = 1 To LG
            If PCE(j) <> 0 Then
                VLM(j, iX) = RVN(j, iX) / PCE(j)
            ElseIf PCE(j) = 0 Then
                VLM(j, iX) = 0
            End If
        Next j

    End Sub

    Function FiscalDef_VariableIsRingFencedCheck(ByVal iX As Short)
        ' Test to see if variable is ring fenced
        ' Set flag accordingly.

        Dim l_FiscalVariableIsRingFenced As Boolean
        l_FiscalVariableIsRingFenced = False

        ' The following 3 tests are identical to the ones at
        ' line number 10220 (except for the action taken
        ' when the expression is true.)

        ' Note: RF$(8) contains the concatenated string of fiscal variables to read in
        ' If the run file param 5 has nothing in then skip the read
        'UPGRADE_WARNING: Return has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Len(Trim(RF(8))) = 0 Then Return l_FiscalVariableIsRingFenced

        ' If param 5 doesn't contain the current fiscal variable then skip read
        'UPGRADE_WARNING: Return has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If InStr(1, Trim(RF(8)), Left(TD(iX, 1), 3), CompareMethod.Text) Mod 3 <> 1 Then Return l_FiscalVariableIsRingFenced

        ' or if the file doesn't exist
        ' 12 Mar 2004 JWD Now it always exists If Len(Dir$(fRingFence$)) = 0 Then Return  'does not exist

        ' At this point,
        ' 1) the run file defines a list of fiscal variable codes
        '    that have been ring-fenced, AND
        ' 2) the current fiscal variable code is in the list of
        '    variables, AND
        ' 3) the ring-fence values data file actually exists,
        '
        ' THEREFORE...
        '    this fiscal variable is ring fenced. Set flag
        '    accordingly and return.
        l_FiscalVariableIsRingFenced = True
        Return l_FiscalVariableIsRingFenced
    End Function

    Sub x28000(ByVal iX As Short)  ' THIS IS CALLED IF CALC VALUES ARE ENTERED
        Dim j As Short
        Dim Dennis(LG) As Single
        CalcValues(iX, TD(iX, 15), TD(iX, 16), Trim(TD(iX, 17)), Dennis)
        For j = 1 To LG
            RVN(j, iX) = Dennis(j)
        Next j

        'If bDebugging Then
        '   Open "calc.log" For Append As #17
        '   Print #17, " "
        '   For j = 1 To LG
        '     Print #17, "28000 RVN("; j; ","; iX; ") = "; RVN(j, iX)
        '   Next j
        '   Close #17
        'End If

        'NOW CALCULATE VOLUME
        For j = 1 To LG
            ' GDP 20 Jan 2003
            ' Use constant for offset
            If A(j, PPR + gc_nAPRICEOFFSET) <> 0 Then
                VLM(j, iX) = RVN(j, iX) / A(j, PPR + gc_nAPRICEOFFSET)
            ElseIf A(j, PPR + gc_nAPRICEOFFSET) = 0 Then
                VLM(j, iX) = 0
            End If
        Next j
    End Sub

    Sub WriteRingfenceVar()
        '~^********************************************************************************
        '? Author=       Glyn Phillips
        '? Date_Created= 09-Sep-99
        '~^
        '~ Function to write out all the ringfence varaibles in one go after particiaption
        '~ has been applied.
        '~^
        '~^********************************************************************************

        Dim fRingFence As String
        Dim i As Short
        Dim ixz As Short

        fRingFence = TempDir & "RING.FNC"

        ' CHECK FOR WRITING OUT OF RING-FENCE VARIABLES
        ' Note RF$(9) is the concatenated string of fiscal variables to output
        ' RF$(11) is the group these variables belong to
        ' If the output string contains this fiscal variable then
        ' Loop through all variables

        Dim l_oRingFenceFile As IEFSFileSeq

        l_oRingFenceFile = g_oRingFenceFile

        With l_oRingFenceFile.OpenForAppend
            For i = 1 To TDT
                If InStr(1, Trim(RF(9)), Left(TD(i, 1), 3), CompareMethod.Text) Mod 3 = 1 Then
                    .NextItem = Trim(RF(11))
                    .NextItem = Left(TD(i, 1), 3)
                    .NextItem = YR
                    .NextItem = LG
                    .NextItemLineEnd = sCur
                    For ixz = 1 To LG
                        .NextItem = RVN(ixz, i)
                        .NextItemLineEnd = VLM(ixz, i)
                    Next ixz
                End If
            Next i
            .CloseFile()
        End With

    End Sub

    '
    ' 19 Nov 2002 JWD (C0633)
    '
    ' Modifications:
    '
    ' This routine returns any ceiling amounts defined for
    ' the specified variable. The function value is true
    ' if a ceiling was defined for the variable, and false
    ' if not. If the function returns true, CeilingAmounts()
    ' contains the calculated CeilingAmounts().
    '
    ' This code is a copy of code in Depreciation() that
    ' determines ceiling amounts used in Cost Recovery
    ' calculations. (Line numbers 4000-4806)
    '
    ' Parameters:
    ' VarIndex is index of FVAR$, this points to the current
    ' fiscal variable being calculated. This is the variable
    ' for which the ceilings are desired.
    ' CeilingAmounts() is the annual array of ceiling amounts
    ' for the variable. The values in this array are valid
    ' only when a ceiling definition for the variable
    ' identified by VarIndex exists.
    '
    Private Function zzzGetCeilingAmounts(ByVal VarIndex As Short, ByRef CeilingAmounts() As Single) As Boolean

        ' In this routine, references to the global variable
        ' symbol clngs() are replaced by references to the
        ' local variable symbol clngy(). References to the
        ' local symbol clngx() are replaced by the formal
        ' parameter symbol CeilingAmounts().

        Dim clngy() As Single
        Dim CLRA() As Single

        Dim j As Short
        Dim iX As Short
        Dim iYP As Short
        Dim iPX As Short
        Dim iPY As Short
        Dim iML As Short
        Dim CLEX As String
        Dim Fd As Short
        Dim Ratetot As Short

        Dim DefAmount As Single
        Dim Numvar As Short
        Dim searcher As String
        Dim param As Short

        Dim matcher() As String
        Dim ratein(,) As Single
        Dim sRateInV() As String
        Dim VarRates() As Single

        Dim l_result As Boolean

        l_result = False
        iX = VarIndex

4020:   ReDim clngy(LG)
        ReDim CeilingAmounts(LG)
        ReDim CLRA(LG)
4030:   iYP = 0
        CLEX = "Y"
        Fd = 0

4040:   iYP = iYP + 1
4048:   If iYP > CLGTT Then CLEX = "N"
4050:   If iYP > CLGTT Then GoTo 4472
4060:   If FVAR(iX) = CLG(iYP, 1) Then
            Fd = 1
            l_result = True
            GoTo 4080
        End If
4070:   GoTo 4040

4080:   ' LOOP THRU INCOME
        ReDim matcher(10)
        ReDim clngy(LG)
        For iPY = 1 To 10
            matcher(iPY) = CLG(iYP, iPY)
        Next iPY

        CeilDef("DEPREC", iX, matcher, clngy)

4472:   'when the user did not specify a ceiling def then GRP
        If Fd <> 1 Then 'fd is basically found%  1=true   0=not found
            For iML = 1 To LG
                '***s/b total rev not prim rev
                ' GDP 20 Jan 2003
                ' Use constant for offset
                clngy(iML) = clngy(iML) + (A(iML, PPR) * A(iML, PPR + gc_nAPRICEOFFSET)) * (1 - PARTRATE(iML)) * WIN(iML)
            Next iML
        End If

4480:   ' NOW DETERMINE CEILING RATES IN CGR()
        If CGRT > 0 Then GoTo contin
        Fd = 0
        GoTo default_Renamed

contin:
        Fd = 1
        Ratetot = CGRT

        ReDim sRateInV(Ratetot)
        ReDim ratein(Ratetot, 6)
        ReDim VarRates(LG)

        For iPX = 1 To Ratetot
            sRateInV(iPX) = sCGR(iPX)
            ratein(iPX, 1) = CGR(iPX, 1)
            ratein(iPX, 2) = CGR(iPX, 2)
            ratein(iPX, 3) = CGR(iPX, 3)
            ratein(iPX, 4) = CGR(iPX, 4)
            ratein(iPX, 5) = CGR(iPX, 5)
            ratein(iPX, 6) = CGR(iPX, 6)
        Next iPX

        DefAmount = 100
        Numvar = iX
        searcher = FVAR(iX)

        RateCalc(Numvar, "DEPREC", searcher, DefAmount, sRateInV, ratein, Ratetot, param, VarRates)

        For j = 1 To LG
            CLRA(j) = VarRates(j)
        Next j

        GoTo 4800

default_Renamed:
        'when the user did not specify the ceiling rates then
        If Fd <> 1 Then
            For iML = 1 To LG
                CLRA(iML) = 100
            Next iML
        End If

4800:   ' NOW COMPUTE CEILING
        For iML = 1 To LG
            CeilingAmounts(iML) = clngy(iML) * (CLRA(iML) / 100)
        Next iML

        zzzGetCeilingAmounts = l_result

    End Function
End Module