Option Strict Off
Option Explicit On
Module CASHFLO1
	' Name:        CASHFLOW.BAS
	' Function:    Cash flow calculation for Giant
	' ********************************************************
	' *      COPYRIGHT 1986-1996 PETROCONSULTANTS, INC.      *
	' *                  ALL RIGHTS RESERVED                 *
	' ********************************************************
	' *  This program file is proprietary information of     *
	' *  Petroconsultants, Incorporated.  Unauthorized use   *
	' *  for any purpose is prohibited.                      *
	' ********************************************************
	'---------------------------------------------------------
	' This program prints cash flow page.
	'---------------------------------------------------------
	' Modifications:
	' 7 Feb 1996 JWD
	'        Changed include file CTYIN.BAS to CTYIN1.BG.
	'        Removed declaration of bDebugging.  Now declared
	'     in common.
	'        Remove set of bDebugging.  Now done in MAINEXEC.
	'        Removed explicit declaration of constants TRUE
	'     and FALSE.  Now declared in TRUFALSE.BC.
	'        Add explicit declaration of default storage class
	'     as Single.
	' 13 Feb 1996 JWD
	'        Convert to subroutine.
	'        Replace explicit subroutine declarations with
	'     include files.
	' 19 Feb 1996 JWD
	'        Changed references to common array RATE() to
	'     PARTRATE().  RATE is reserved function name in VB.
	' 23 Aug 1996 JWD
	'        Correct Cashflow().
	' 18 Aug 1999 GDP
	'        Added call to WriteRVS - this writes out a temp file
	'        needed for consolidation (net of participation)
	' 14 Dec 2000 GDP
	'        Added code to handle a discount date in advance of the end of project life
	'
	' 21 Jun 2001 JWD
	'  -> Changed Cashflow(). (C0339)
	'
	' 4 Aug 2001 JWD
	'  -> Changed Cashflow(). (C0363)
	'
	' 1 Oct 2001 GDP
	'  -> Changed Cashflow(). Added Close #5.
	' before 10 Dec 2001 GDP
	'  -> Changed Cashflow(). Added call to WriteDtreeData.
	'
	' 19 Mar 2002 JWD
	'  -> Added symbolic constant for cash flow variable count
	'     exceeding maximum. (C0499)
	'  -> Changed Cashflow(). (C0499)
	'  -> Changed Cashflow(). (C0500)
	'
	' 29 Mar 2002 JWD
	'  -> Changed Cashflow(). (C0527)
	
	' 20 Jan 2003 GDP
	'  -> Changed Cashflow().
	'
	' 24 Feb 2003 GDP
	'  -> Change Cashflow().
	'
	' 08 Apr 2003 GDP
	'  -> Changed Cashflow().
	'
	' 24 Apr 2003 GDP
	'  -> Changed Cashflow().
	'
	' 12 Jan 2004 JWD
	'  -> Changed Cashflow() (C0772)
	'
	' 19 Jan 2004 JWD
	'  -> Changed Cashflow(). (C0773)
	'  -> Changdd Cashflow(). (C0774)
	'
	' 3 Feb 2004 JWD
	'  -> Changed Cashflow(). (C0776)
	'
	' 9 Feb 2004 JWD
	'  -> Changed Cashflow(). (C0779)
	'  -> Changed Cashflow(). (C0783)
	'
	' 22 Sep 2004 JWD
	'  -> Changed Cashflow(). (C0839)
	'
	' 1 Oct 2004 JWD
	'  -> Changed Cashflow(). (C0841)
	'
	' 14 Oct 2004 JWD
	'  -> Changed Cashflow(). (C0845)
	'
	' 6 Dec 2004 JWD
	'  -> Changed Cashflow(). (C0846)
	'
	' 15 Dec 2004 JWD
	'  -> Changed Cashflow(). (C0846)
	'
	' 10 Feb 2005 JWD
	'  -> Changed Cashflow(). (C0856)
	'
	' 11 Feb 2005 JWD
	'  -> Changed Cashflow(). (C0857)
	'
	' 22 Apr 2005 JWD
	'  -> Changed Cashflow(). (C0872)
	'  -> Changed Cashflow(). (C0873)
	'
	' 17 May 2005 JWD
	'  -> Changed Cashflow(). (C0878)
	'
	' 5 Jan 2006 JWD
	'  -> Changed Cashflow(). (C0891)
	'
	' 5 Jun 2006 JWD
	'  -> Changed Cashflow(). (C0898)
	'
	' 8 Jun 2006 JWD
	'  -> Changed Cashflow(). (C0900)
	'
	' 10 Jun 2008 JWD
	'  -> Changed Cashflow(). (080605-1527-01)
	'---------------------------------------------------------
	' $DYNAMIC
	'UPGRADE_NOTE: DefSng A-Z statement was removed. Variables were explicitly declared as type Single. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="92AFD3E3-440D-4D49-A8BF-580D74A8C9F2"'
	'$INCLUDE: 'trufalse.bc'
	
	Const CASHFLO As String = "CASHFLOW"
	
	'<<<<<< 19 Mar 2002 JWD (C0499)
	' Application Error codes used in this module
	Const ErrorCode_ExceededMaxCashFlowCount As Short = 249
	'>>>>>> End (C0499
	
	'$INCLUDE: 'ctyin1.bg'
	'$INCLUDE: 'pgm9900.bi'
	
	
	'OXY project items
	'$INCLUDE: 'gntoxyc.bi'
	'$INCLUDE: 'gntoxy1.bi'
	
	'$INCLUDE: 'run0200.bi'
	'$INCLUDE: 'gntcon.bi'
	
	'$INCLUDE: 'cashflow.bi'
	'$INCLUDE: 'cashflo2.bi'
	'---------------------------------------------------------
	
	Sub Cashflow()
		Dim SAVELG As Single
		Dim matchem As String
		Dim loopit As Single
		Dim AddCCT As Single
		Dim NewDim As Single
		Dim L2ddate As Single
		Dim L1ddate As Single
		Dim RunRev As Single
		Dim xdum As Single
		Dim ANSW As Single
		Dim XI As Single
		Dim x As Single
		Dim EF As Single
		Dim OTHB As Single
		Dim DVB As Single
		Dim EXB As Single
		Dim OXB As Single
		Dim OTH As Single
		Dim DV As Single
		Dim Ex As Single
		Dim OTH1 As Single
		Dim DV1 As Single
		Dim EX1 As Single
		Dim PRCE As Single
		Dim DUMK As Single
		Dim OPST As Single
		Dim OPS As Single
		Dim TTL2T As Single
		Dim TTL1T As Single
		Dim TTL2 As Single
		Dim TTL1 As Single
		Dim LIG As Single
		Dim LFITemp As Single
		Dim productiondelay As Single
		Dim ZN As Single
		Dim TS As String
		Dim ss As String
		Dim grp As Single
		Dim DiscTime As Single
		Dim dOffset As Single
		Dim D5D As Single
		Dim dZDF As Single
		Dim dXCF As Single
		Dim TempDiscMo As Single
		Dim TempDiscYr As Single
		Dim DateDisc As Single
		Dim DateProj As Single
		Dim CE As Single
		Dim iter As Short
		Dim cl As Single
		Dim U5L As Single
		Dim CU As Single
		Dim rl As Single
		Dim RE As Single
		Dim XG As Single
		Dim RU As Single
		Dim TP1 As Single
		Dim U5 As Single
		Dim RRB As Single
		Dim RR As Single
		Dim RRZ As Single
		Dim X7 As Single
		Dim DRT As Single
		Dim fco As Single
		Dim adjust As Short
		Dim TorULines As Short
		Dim NEGLP As Short
		Dim CM2 As Single
		Dim CM1 As Single
		Dim POT As Single
		Dim iZR As Single
		Dim OXYTtlPos As Single
		Dim OXYGrRev As Single
		Dim q As Short
		Dim firstyr As Single
		Dim AtcfCtr As Short
		Dim NegCtr As Short
		Dim cflo As String
		Dim PosCtr As Short
		Dim Counter As Short
		Dim UserNegatives As Short
		Dim fin As Short
		Dim REPY As Short
		Dim CPXP As Short
		Dim OPXP As Short
		Dim OUTFIL As String
		'---------------------------------------------------------
		' This program prints cash flow page.
		'---------------------------------------------------------
		' Modifications:
		' 19 Feb 1996 JWD
		'        Changed array PV() for economic summary data to
		'     ESPV().  PV is an intrinsic function name in VB.
		' 20 Feb 1996 JWD
		'        Renamed RPT.DRIVE$ to sRptDir, previous name not
		'     acceptable to VB.
		'        Change CUR$ to sCur, duplicate definition (CUR()).
		'        Change RT to DRT, duplicate definition with RT().
		'        Change GVCF to bGVCF, duplicate definition with
		'     GVCF().
		'        Change D5 to rD5X, duplicate definition (D5()).
		'        Change NGCFT% to iNGCFT, duplicate definition.
		'        Change NGCFA% to iNGCFA, duplicate definition.
		'        Change iNGCFU to iNGCFU, duplicate definition.
		' 21 Feb 1996 JWD
		'        Changed loop indexes from singles to integers.
		' 23 Apr 1996 MKD
		'        Added "FinalWin = 1" to this sub near top where
		'     the statement "If Left$(RF$(1), 6) <> "CONSOL" Then"
		'     is.  This will set Final WIN interest to 100% if
		'     this is a consolidation run.  It corrects a bug in
		'     calculating StateTake for consolidations.
		' 23 Aug 1996 JWD
		'        Move label 40160 from For statement to line
		'     preceding to dimension arrays prior to to executing
		'     loop.  If there was no capital in any data file in
		'     runs, would jump to For statement, never
		'     dimensioning arrays.
		'
		' 21 Jun 2001 JWD
		'  -> Replace explicit references to MY3() category codes
		'     18, 19, & 20 (BAL, BL2, BL3) with global symbols for
		'     the same. Necessitated by addition of new capital
		'     category codes that changed the actual values of the
		'     BAL codes.(C0339)
		'
		' 4 Aug 2001 JWD
		'  -> Added CPXCategoryCode_AbandonmentCashExpenditure as
		'     a 'non-BAL' item to conditional tests looking for
		'     same. (C0363)
		'
		' 19 Mar 2002 JWD
		'  -> Add check of number of positive and negative cash
		'     flow variables defined in the country file against
		'     the capacity of the cash flow arrays to ensure
		'     counts are within array bounds and issue error
		'     indication if not. (C0499)
		'  -> Change error code issued when number of cash flows
		'     exceeds the maximum limits in the check in print
		'     cash flows section (after line 7240). Change to use
		'     symbolic constant. (C0500)
		'
		' 29 Mar 2002 JWD
		'  -> Replace explicit literal values for sizing positive,
		'     negative and combined cash flow arrays with symbolic
		'     expressions. (C0527)
		'
		' 20 Jan 2003 GDP
		'  -> Changed references to A array for additional volume streams and
		'     A array references to use constants defined in modArrayConst.bas
		'
		' 24 Feb 2003 GDP
		'  -> Changed ReDim A(LG, 13) to ReDim A(LG, gc_nASIZE)
		'
		' 08 Apr 2003 GDP
		'  -> Commented out various sections of OXY code.
		'
		' 24 Apr 2003 GDP
		'  -> Reinstated the code commented out in previous change
		'     need CF array to be defined for results of Expected Value run
		'
		' 12 Jan 2004 JWD
		'  -> Add references to CGiantReport1 object to collect
		'     report data in object rather than output directly to
		'     file. For consolidation engine development testing
		'     purposes. (C0772)
		'
		' 19 Jan 2004 JWD
		'  -> Add collection of profile type and user titles to
		'     store in page object to support generation of text
		'     to match ASPEEngine OutputNames() array text.
		'     (C0773)
		'
		' 19 Jan 2004 JWD
		'  -> Changed report page object type for Deflated cash
		'     flow page from CGiantRptPageA1 to interface
		'     IGiantRptPageAssignStd. (C0774)
		'
		' 3 Feb 2004 JWD
		'  -> Remove open of report file. This has been replaced
		'     by the report object. (C0776)
		'  -> Remove writes to report file. (C0776)
		'
		' 9 Feb 2004 JWD
		'  -> Replace call to TerminateExecution with re-raise of
		'     error to caller. (C0779)
		'  -> Remove output of STATETAK.SUM. (C0783)
		'  -> Remove output of CHUONG.DAT. (C0783)
		'
		' 15 Mar 2004 JWD
		'  -> Change to condition output of run summary file on
		'     being a Mainexec run.
		'  -> Change to condition calls to WriteDTreeData(),
		'     WriteRVS(), and WriteRingFenceVar() on being a
		'     Mainexec run.
		'
		' 22 Sep 2004 JWD
		'  -> Add assignment of run summary (other indicator) item
		'     values to the economic indicators page for all run
		'     types (SPE and MAINEXEC). (C0839)
		'  -> Remove explicit write of run summary file in normal
		'     MAINEXEC runs only. (C0839)
		'
		' 1 Oct 2004 JWD
		'  -> Change calculation of ZN (actual project life) and
		'     LFITemp (actual producing life) to be based on LG
		'     and respective start dates. (C0841)
		'
		' 14 Oct 2004 JWD
		'  -> Changed to remove conditional on instantiation and
		'     population of the economic indicators page to always
		'     instantiate and populate it. Needed for run summary
		'     output for all runs. (C0845)
		'
		' 6 Dec 2004 JWD
		'  -> Add calculation of third party and NOC economics and
		'     change Government economics by removing the NOC part
		'     of cash flows. Change reports accordingly. (C0846)
		'  -> Add calculation of third party and NOC economic
		'     indicators for economic indicators report page, add
		'     to report page. (C0846)
		'  -> Change allocation of ESPV() array to provide storage
		'     for new 3rd party and NOC economic indicators.
		'     (C0846)
		'
		' 15 Dec 2004 JWD
		'  -> Add dimensioning of 3rd party and NOC cash flow
		'     arrays after 40000 for the post-tax consolidation
		'     processing. (C0846)
		'  -> Add calculations of discounted economic indicators.
		'     (C0846)
		'
		' 10 Feb 2005 JWD
		'  -> Add accumulation of 3rd party, NOC, and Government
		'     cash flows for after-tax consolidation. (C0856)
		'
		' 11 Feb 2005 JWD
		'  -> Correct the price index offset for the first run to
		'     be consolidated in post-tax consolidation to use the
		'     offset symbol rather than the literal number 6. This
		'     corrects failure to consolidate the revenues and
		'     equivalent reserves for the first run in the consol-
		'     idation. (C0857)
		'
		' 22 Apr 2005 JWD
		'  -> Changed symbol used as year subscript for EffInts()
		'     in calculating GFINAN() element values. Symbol used
		'     was not loop control symbol as it should have been,
		'     symbol used caused 'overflow' failure in operation.
		'     (C0872)
		'  -> Changed conditional expressions testing symbol
		'     g_nFiscalEvents to perform bit-wise And before the
		'     relational (>) operation. Was always executing the
		'     conditional code if any financing events, not the
		'     intended masking of events. (C0873)
		'
		' 17 May 2005 JWD
		'  -> Change to replace explicit individual comparisons
		'     with balance category codes in discounting section
		'     and summary preparation section to compare to a
		'     range (BAL to BLn). (C0878)
		'
		' 5 Jan 2006 JWD
		'  -> Change to enable the ReDim of AC() immediately after
		'     line 38760. Had been commented out for no documented
		'     reason. Consequently, the AC() was not reinitialized
		'     to zeroes after the data was copied to D8() when the
		'     data was shifted and would result in the accumulated
		'     values being left in the array and then added in a
		'     second time when D8() values were copied back in.
		'     (C0891)
		'
		' 5 Jun 2006 JWD
		'  -> Change to only compute EffInts() values when company
		'     + 3d party ncf is sufficiently large to be valid in
		'     setup for after-tax consolidation. Change comparison
		'     to consider a range of values zero instead of exact
		'     match to zero. Compensates for round-off errors that
		'     lead to divide by zero error. (C0898)
		'
		' 8 Jun 2006 JWD
		'  -> Change to only compute EffIntsC() values when the
		'     company + 3d party + noc capital expenditure is
		'     sufficiently large to be valid in setup for after-
		'     tax consolidation. Change comparison to consider a
		'     range of values zero instead of exact match to zero.
		'     Compensates for round-off errors that lead to divide
		'     by zero error. This change implements in the capital
		'     effective interests computation the corresponding
		'     correction made to annual cash flows by C0898.
		'     (C0900)
		'
		' 10 Jun 2008 JWD
		'  -> Change value expression assigned to the AC() accum-
		'     ulator array at the reserves position to set to
		'     energy equivalent reserves. (080605-1527-01)
		'---------------------------------------------------------
		
		' 21 Dec 2004 JWD (C0846)
		Const eEntityID_Company As Short = 1
		Const eEntityID_3rdParty As Short = 2
		Const eEntityID_NOC As Short = 3
		Const eEntityID_Government As Short = 4
		Dim iEntityID As Short
		
		Const rMaxNeg As Double = -3.4E+38 ' used to indicate that values are N/A on pv indicators table
		' End (C0846)
		
		'<<<<<< 29 Mar 2002 JWD (C0527)
		Const maximum_count_positive_cashflows As Short = 51 ' includes total
		Const maximum_count_negative_cashflows As Short = 51 ' includes total
		'>>>>>> End (C0527)
		
		Dim ww As Short
		Dim bGVCF As Short
		Dim dXDF As Double
		Dim i As Short
		Dim iCFCol As Short
		Dim iDET As Short
		Dim iNGCFT As Short
		Dim iNGCFA As Short
		Dim iNGCFU As Short
		Dim rD5X As Single
		Dim w As Short
		Dim y As Short
		
		Dim hfLog As Short
		Dim fLog As String
		Dim ZD As Short
		Dim zp As Short
		Dim DiscFlag As Short
		
		Dim a_fCurrFactor() As Single
		
		' 6 Dec 2004 JWD (C0846) Add declaration of new cash flow vectors
		Dim TDPCF() As Single ' Third party cash flow
		Dim TDPCCF() As Single ' Third party cumulative cash flow
		Dim TDPDCF() As Single ' Third party discounted cash flow
		Dim TDPCDCF() As Single ' Third party cumulative discounted cash flow
		Dim NOCCF() As Single ' NOC cash flow
		Dim NOCCCF() As Single ' NOC cumulative cash flow
		Dim NOCDCF() As Single ' NOC discounted cash flow
		Dim NOCCDCF() As Single ' NOC cumulative discounted cash flow
		Dim RRT As Single ' Third party rate of return
		Dim RRN As Single ' NOC rate of return
		Dim GREV() As Single ' Sales revenues grossed-up to 100%
		Dim GPCFV() As Single ' Positive cash flow variables (+) grossed-up to 100%
		Dim GNCFV() As Single ' Negative cash flow variables (- only) grossed-up to 100%
		Dim GOPEX() As Single ' Operating Expenses grossed-up to 100%
		Dim GMY3() As Single ' Capital expenditures, grossed-up to 100%
		Dim GREPAY() As Single ' NOC repayment of capital, grossed-up to 100%
		Dim GFINAN() As Single ' Financing cash flows grossed-up to 100%
		Dim CNREIM() As Single ' Portion of TOTPMT() that is partner reimbursement, net to company interest
		Dim C3DPTY() As Single ' Total company "T" cash flows (payments to 3rd party defined in fiscal model)
		Dim TATCF() As Single ' 3rd party after tax cash flow
		Dim NATCF() As Single ' NOC after tax cash flow
		Dim rTmp As Single ' scratch variable
		Dim U23 As Single ' 3rd party total capex (discounted)
		Dim U26 As Single ' 3rd party total net cash flow (discounted, excluding capex)
		Dim U27 As Single ' 3rd party total risk money (discounted)
		Dim U33 As Single ' NOC total capex (discounted)
		Dim U36 As Single ' NOC total net cash flow (discounted, excluding capex)
		Dim U37 As Single ' NOC total risk money (discounted)
		' Following are now explicitly declared, were implicitly declared previously
		Dim ATCF() As Single ' Company after-tax net cash flow
		Dim GOVT() As Single ' Government "after-tax" net cash flow
        Dim ESPV(,) As Single ' Present values table
        Dim rUDCF(,) As Single ' Undiscounted cash flows array (just explicitly declaring here, was implicitly declared previously)
		Dim PAY() As Single ' Cash flow for payout calculation
		Dim bRMN As Boolean ' Switch: Calculate risk money?
		Dim GVTK As Short ' Switch: Calculate Company Government Take? (1=Yes, otherwise No)
		Dim vpr() As Single ' working array for discounting
		Dim u1 As Single ' Company total revenues (discounted)
		Dim u2 As Single ' Company total opex (discounted)
		Dim U3 As Single ' Company total capex (discounted)
		Dim U4 As Single ' Company total royalty & tax (discounted)
		Dim U7 As Single ' Company total risk money (discounted)
		Dim U8 As Single ' Company government take denominator amount
		Dim U11 As Single ' Government total cash flow (discounted)
		Dim U12 As Single ' Government total capex (discounted)
		Dim U13 As Single ' Government total risk money (discounted)
		' End (C0846)
		'---------------------------------------------------------
		'<<<<<< 29 Mar 2002 JWD (C0527)
		ReDim L2(14)
		' 6 Dec 2004 JWD (C0846) Change upper limit of 1st dimension for new indicators
		ReDim ESPV(22, 6) ' was: (13, 6) also provided for StateTake at some point
        Dim ColumnNm(maximum_count_positive_cashflows + maximum_count_negative_cashflows + 1) As String
		Dim CF(LG) As Single
		ReDim PSCF(LG, maximum_count_positive_cashflows)
		ReDim NGCF(LG, maximum_count_negative_cashflows)
		Dim ERT(LG) As Single
		Dim D5(LG) As Single
		ReDim vpr(LG)
		
		ReDim PAY(LG)
		ReDim ATCF(LG)
		Dim DUM(LG, maximum_count_positive_cashflows + maximum_count_negative_cashflows + 1) As Single
		'~~~~~~ was:
		'ReDim L2(14), ESPV(13, 6), ColumnNm$(25)
		'ReDim CF(LG), PSCF(LG, 12), NGCF(LG, 12), ERT(LG), D5(LG), vpr(LG)
		'
		'ReDim PAY(LG), ATCF(LG), DUM(LG, 25)
		'>>>>>> End (C0527)
		
		'handled in forecast.bas     REDIM DFL(LG)
		Dim COCF(LG) As Single
		Dim COCCF(LG) As Single
		Dim CODCF(LG) As Single
		Dim COCDCF(LG) As Single
		ReDim GVCF(LG)
		Dim GVCCF(LG) As Single
		Dim GVDCF(LG) As Single
		Dim GVCDCF(LG) As Single
		Dim CP(LG) As Single
		ReDim GOVT(LG)
		
		' 6 Dec 2005 JWD (C0846) Allocate new 3rd party and NOC cash flow
		ReDim TDPCF(LG)
		ReDim TDPCCF(LG)
		ReDim TDPDCF(LG)
		ReDim TDPCDCF(LG)
		ReDim NOCCF(LG)
		ReDim NOCCCF(LG)
		ReDim NOCDCF(LG)
		ReDim NOCCDCF(LG)
		ReDim GREV(LG)
		ReDim GPCFV(LG)
		ReDim GNCFV(LG)
		ReDim GOPEX(LG)
		ReDim GREPAY(LG)
		ReDim GFINAN(LG)
		ReDim CNREIM(LG)
		ReDim C3DPTY(LG)
		ReDim TATCF(LG)
		ReDim NATCF(LG)
		ReDim GMY3(UBound(my3, 1))
		' End (C0846)
		
		'OXY special negative variables (T, A, U)
		'<<<<<< 29 Mar 2002 JWD (C0527)
		ReDim NGCFT(LG, maximum_count_negative_cashflows)
		ReDim NGCFA(LG, maximum_count_negative_cashflows)
		ReDim NGCFU(LG, maximum_count_negative_cashflows)
		'~~~~~~ was:
		'ReDim NGCFT(LG, 12), NGCFA(LG, 12), NGCFU(LG, 12)
		'>>>>>> End (C0527)
		
		' holds cashflows for discounting...
		' 21 Dec 2004 JWD (C0846) Change for new cash flows
		ReDim rUDCF(LG, 13) ' was: (LG, 11)
		
		Program = CASHFLO
		
		On Error GoTo ErrHandler
		
		If Left(RF(1), 6) <> "CONSOL" Then
			GoTo 6010
		End If
		'--------------------------------------------------------------------
		'you are here if RF$(1), 6) = "CONSOL"
		WINT = 1 'hard wire the net partners and government
		PRTA = 1
		FinalWin = 1 'used in State Take calculation
		
		' Set sCur ( current currency ) to consol currency  *** 4-27-99
		
		
		sCur = ConCur
		RNU = RNU + 1
		If Len(RN) > 6 Then
			RNP = Mid(RN, 1, 6) & Right(Str(RNU), Len(Str(RNU)) - 1)
		Else
			RNP = RN & Right(Str(RNU), Len(Str(RNU)) - 1)
		End If
		
		OUTFIL = sRptDir & RNP & ".PRN"
		If Left(RF(4), 3) = "FIL" Or Left(RF(4), 3) = "SCR" Or Left(RF(4), 3) = "PTR" Or Left(RF(4), 3) = "PTD" Then
			If Left(RF(5), 3) = "ALL" Or Left(RF(5), 3) = "VAR" Or Left(RF(5), 3) = "SUM" Then
				''''            Open OUTFIL$ For Output As #5
			End If
		End If
		
		'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        'GoSub 40000 'READS ACCUMULATED TOTALS AND SETS UP CONSOLIDATION

        If L1(2) = 10000 Then
            Error (250)
        End If
        '////////////////////////////////////////////////////////////

        ReDim GM(1, 2)
        mo = L1(1) : YR = L1(2)
        M1 = L1(3) : Y1 = L1(4)
        M2 = L1(13) : Y2 = L1(14) 'discovery date   2-17-92 MKD
        M3 = L1(10) : Y3 = L1(11)

        SAVELG = LG

        LG = L1(8) - L1(2) + 1

        If LG <> SAVELG Then 'proj life is different for consol that the last run!
            '<<<<<< 29 Mar 2002 JWD (C0527)
            ReDim CF(LG)
            ReDim PSCF(LG, maximum_count_positive_cashflows)
            ReDim NGCF(LG, maximum_count_negative_cashflows)
            ReDim ERT(LG)
            ReDim D5(LG)
            ReDim vpr(LG)
            ReDim NGCFT(LG, maximum_count_negative_cashflows)
            ReDim NGCFA(LG, maximum_count_negative_cashflows)
            ReDim NGCFU(LG, maximum_count_negative_cashflows)
            ReDim PAY(LG)
            ReDim ATCF(LG)
            ReDim DUM(LG, maximum_count_positive_cashflows + maximum_count_negative_cashflows + 1)
            '~~~~~~ was:
            'ReDim CF(LG), PSCF(LG, 12), NGCF(LG, 12), ERT(LG), D5(LG), vpr(LG)
            'ReDim NGCFT(LG, 12), NGCFA(LG, 12), NGCFU(LG, 12)
            'ReDim PAY(LG), ATCF(LG), DUM(LG, 25)
            '>>>>>> End (C0527)
            ''''REDIM DFL(LG)
            ReDim COCF(LG)
            ReDim COCCF(LG)
            ReDim CODCF(LG)
            ReDim COCDCF(LG)
            ReDim GVCF(LG)
            ReDim GVCCF(LG)
            ReDim GVDCF(LG)
            ReDim GVCDCF(LG)
            ReDim CP(LG)
            ReDim GOVT(LG)

            ' 15 Dec 2005 JWD (C0846) Allocate new 3rd party and NOC cash flow
            ReDim TDPCF(LG)
            ReDim TDPCCF(LG)
            ReDim TDPDCF(LG)
            ReDim TDPCDCF(LG)
            ReDim NOCCF(LG)
            ReDim NOCCCF(LG)
            ReDim NOCDCF(LG)
            ReDim NOCCDCF(LG)
            ReDim TATCF(LG)
            ReDim NATCF(LG)
            ReDim GREV(LG)
            ReDim GOPEX(LG)
            ReDim GPCFV(LG)
            ReDim GNCFV(LG)
            ReDim GREPAY(LG)
            ReDim GFINAN(LG)
            ReDim CNREIM(LG)
            ReDim C3DPTY(LG)
            '''            ReDim EffInts(LG, gc_nEffIntsSIZED2)
            ' End (C0846)

            ReDim gna_ACFX(LG, 26)

        End If

        LFI = L1(9) - L1(7)
        LIG = L1(9) - L1(6)
        LGI = LIG + ((mo - 1) / 12)
        PS = 1
        NEG = 3

        '11-16-92 OXY items ("T","A" & "U" cashflow items)
        iNGCFT = 1
        iNGCFA = 1
        iNGCFU = 1

        my3tt = CCT
        'WINT = 0   '*** watch out for this
        OPXP = 2
        CPXP = 3
        PPR = 1
        TDT = 2
        TLT = 2
        ReDim TD(2, 18)
        ReDim TL(2, 3)
        ' GDP 24 Feb 2003
        ' Redimension A array to hold extra volume data
        ReDim A(LG, gc_nASIZE)
        TD(1, 1) = "INC"
        TD(1, 4) = "+"
        TD(2, 1) = "TAX" : TD(2, 4) = "-"
        TL(1, 1) = "INC" : TL(1, 2) = "INCOME"
        TL(2, 1) = "TAX" : TL(2, 2) = "ROY/TAX"

        For x = 1 To LG
            PSCF(x, 1) = AC(x, 1)
            NGCF(x, 1) = AC(x, 2)
            NGCF(x, 2) = AC(x, 3)
            NGCF(x, 3) = AC(x, 4)
            ' GDP 20 Jan 2003
            ' Replaced numbers with constants when referencing the A array
            A(x, gc_nAOIL) = AC(x, 5) 'a(x,1) = OIL    ac(x,5) = equiv prod
            A(x, gc_nAGAS) = 0
            A(x, gc_nAOV1) = 0
            If AC(x, 5) <> 0 Then
                A(x, gc_nAOPC) = AC(x, 6) / AC(x, 5)
            End If
            '11-16-92 OXY Project. Modified AC() to hold GVCF()
            '  and "T","A", & "U" cashflow items
            GVCF(x) = AC(x, 8)
            'cumulative govt cashflow
            If x = 1 Then
                GVCCF(x) = GVCF(1)
            Else
                GVCCF(x) = GVCCF(x - 1) + GVCF(x)
            End If
            NGCFT(x, 1) = AC(x, 9)
            NGCFT(x, 2) = AC(x, 9)
            NGCFA(x, 1) = AC(x, 10)
            NGCFA(x, 2) = AC(x, 10)
            NGCFU(x, 1) = AC(x, 11)
            NGCFU(x, 2) = AC(x, 11)

            '''          ' 10 Feb 2005 JWD (C0856) Compute the total net cash flow and the respective effective interests
            '''          GPCFV(x) = AC(x, gc_nAC_CMPNCF) + AC(x, gc_nAC_3DPNCF) + AC(x, gc_nAC_NOCNCF)
            '''          ' 5 Jun 2006 JWD (C0898) Change to only compute EffInts() values when company + 3d party ncf is sufficiently large to be valid. Change to range of values considered zero instead of exact match to zero. Compensates for round-off errors that lead to divide by zero.
            '''          If Abs(AC(x, gc_nAC_CMPNCF) + AC(x, gc_nAC_3DPNCF)) > 0.0000009 Then
            '''            EffInts(x, gc_nEffInts_WIN) = AC(x, gc_nAC_CMPNCF) / (AC(x, gc_nAC_3DPNCF) + AC(x, gc_nAC_CMPNCF))
            '''          End If
            '''          If Abs(AC(x, gc_nAC_CMPNCF) + AC(x, gc_nAC_3DPNCF) + AC(x, gc_nAC_NOCNCF)) > 0.0000009 Then
            '''            EffInts(x, gc_nEffInts_PAR) = AC(x, gc_nAC_NOCNCF) / (AC(x, gc_nAC_3DPNCF) + AC(x, gc_nAC_CMPNCF) + AC(x, gc_nAC_NOCNCF))
            '''          End If
            '''          ' was:
            '''          'If AC(x, gc_nAC_CMPNCF) <> 0 Then
            '''          '  EffInts(x, gc_nEffInts_WIN) = AC(x, gc_nAC_CMPNCF) / (AC(x, gc_nAC_3DPNCF) + AC(x, gc_nAC_CMPNCF))
            '''          '  EffInts(x, gc_nEffInts_PAR) = AC(x, gc_nAC_NOCNCF) / (AC(x, gc_nAC_3DPNCF) + AC(x, gc_nAC_CMPNCF) + AC(x, gc_nAC_NOCNCF))
            '''          'End If
            '''          ' End (C0898)
            '''
            '''          GREV(x) = AC(x, gc_nAC_GRSREV) + GPCFV(x)
            '''          ' End (C0856)

            gna_ACFX(x, gna_ACFX_TPS) = AC(x, gc_nAC_TPS)
            gna_ACFX(x, gna_ACFX_TNG) = AC(x, gc_nAC_TNG)
            gna_ACFX(x, gna_ACFX_OPS) = AC(x, gc_nAC_OPS)
            gna_ACFX(x, gna_ACFX_ONG) = AC(x, gc_nAC_ONG)
            gna_ACFX(x, gna_ACFX_GPS) = AC(x, gc_nAC_GPS)
            gna_ACFX(x, gna_ACFX_GNG) = AC(x, gc_nAC_GNG)
            gna_ACFX(x, gna_ACFX_CPS) = AC(x, gc_nAC_CPS)
            gna_ACFX(x, gna_ACFX_CNG) = AC(x, gc_nAC_CNG)

        Next x

        '''        ' 7 Jan 2005 JWD (C0846) Add allocation of MY3 related arrays
        '''        ReDim GMY3(CCT)
        '''        ReDim EffIntsX(CCT, gc_nEffIntsXSIZED2)
        '''        ' End (C0846)

        If CCT = 0 Then
            GoTo 40160
        End If
        ReDim my3(CCT, 7)
        ReDim WINC(CCT)
        ReDim GPRATE(CCT)

        ReDim my3Ex(CCT, 3)

        For x = 1 To CCT
            my3(x, 1) = CC(x, 1)
            my3(x, 2) = CC(x, 2)
            my3(x, 3) = CC(x, 3)
            my3(x, 4) = 0
            my3(x, 5) = CC(x, 4)
            my3(x, 6) = 100
            WINC(x) = 1
            GPRATE(x) = 1

            '''          ' 10 Feb 2005 JWD (C0856)
            '''          GMY3(x) = CC(x, gc_nCC_AMT) + CC(x, gc_nCC_3DP) + CC(x, gc_nCC_NOC)
            '''          If CC(x, gc_nCC_AMT) <> 0 Then
            '''             ' 8 Jun 2006 JWD (C0900) Change to check divisor for zero + near-zero values before dividing
            '''             If Abs(CC(x, gc_nCC_CMP) + CC(x, gc_nCC_3DP)) > 0.0000009 Then
            '''               EffIntsX(x, gc_nEffIntsX_WIN) = CC(x, gc_nCC_CMP) / (CC(x, gc_nCC_CMP) + CC(x, gc_nCC_3DP))
            '''             End If
            '''             If Abs(CC(x, gc_nCC_CMP) + CC(x, gc_nCC_3DP) + CC(x, gc_nCC_NOC)) > 0.0000009 Then
            '''               EffIntsX(x, gc_nEffIntsX_PAR) = CC(x, gc_nCC_NOC) / (CC(x, gc_nCC_CMP) + CC(x, gc_nCC_3DP) + CC(x, gc_nCC_NOC))
            '''             End If
            '''             ' was:
            '''             '  EffIntsX(x, gc_nEffIntsX_WIN) = CC(x, gc_nCC_CMP) / (CC(x, gc_nCC_CMP) + CC(x, gc_nCC_3DP))
            '''             '  EffIntsX(x, gc_nEffIntsX_PAR) = CC(x, gc_nCC_NOC) / (CC(x, gc_nCC_CMP) + CC(x, gc_nCC_3DP) + CC(x, gc_nCC_NOC))
            '''             ' End (C0900)
            '''          End If
            '''          ' End (C0856)

            my3Ex(x, 0) = CC(x, gc_nCC_TCX)
            my3Ex(x, 1) = CC(x, gc_nCC_OCX)
            my3Ex(x, 2) = CC(x, gc_nCC_GCX)
            my3Ex(x, 3) = CC(x, gc_nCC_CCX)

        Next x

6010: For i = 1 To LG
			ERT(i) = 1
		Next i
		
		' 4 Jan 2005 JWD (C0846) Add next to fill EffInts() for normal run to calculate 3rd party and NOC cash flows
		If Not g_bPTCons Then
			'''            ' If this is not a pre-tax consolidation run, EffInts has not been allocated, so allocate and initialize...
			'''            ReDim EffInts(0 To gc_nMAXLIFE, 0 To gc_nEffIntsSIZED2)
			'''            For y = 1 To LG
			'''                EffInts(y, gc_nEffInts_WIN) = WIN(y)
			'''                EffInts(y, gc_nEffInts_PAR) = PARTRATE(y)
			'''                EffInts(y, gc_nEffInts_WOX) = WIN(y)
			'''                EffInts(y, gc_nEffInts_POX) = OPEXRATE(y)
			'''                ' Capture financing information
			'''                ' Default case assumes that only company has interest in financing cash flow
			'''                EffInts(y, gc_nEffInts_WFN) = 1
			'''                EffInts(y, gc_nEffInts_PFN) = 0
			'''
			'''                ' 22 Apr 2005 JWD (C0873) Change order of operations on expression, replace numeral with symbol
			'''                If (g_nFinanceEvents And gc_nFinanceEvents_WIN) > 0 Then  ' amounts are net of win, 3rd party shared in financing, i. e. WIN after FIN in fiscal def
			'''                    EffInts(y, gc_nEffInts_WFN) = WIN(y)
			'''                End If
			'''
			'''                ' 22 Apr 2005 JWD (C0873) Change order of operations on expression, replace numeral with symbol
			'''                If (g_nFinanceEvents And gc_nFinanceEvents_PAR) > 0 Then  ' amounts are net of par, NOC shared in financing, i. e. PAR after FIN in fiscal def
			'''                    EffInts(y, gc_nEffInts_PFN) = PARTRATE(y)
			'''                End If
			'''            Next y
			'''            ' Assign the effective interests for the capital expenditures
			'''            ReDim EffIntsX(0 To UBound(my3, 1), 0 To gc_nEffIntsXSIZED2)
			'''            For y = 1 To my3tt
			'''                GMY3(y) = my3(y, gc_nMY3_AMT)
			'''                EffIntsX(y, gc_nEffIntsX_WIN) = WINC(y)
			'''                EffIntsX(y, gc_nEffIntsX_PAR) = 1 - GPRATE(y)
			'''                i = my3(y, gc_nMY3_XYR) - YR + 1
			'''                EffIntsX(y, gc_nEffIntsX_BUR) = (WINC(y) - WIN(i)) * my3(y, gc_nMY3_BUR) / 100
			'''            Next y
			'''
			'''            ' Assign the opex to the grossed-up opex array
			'''            For y = 1 To LG
			'''                GOPEX(y) = OPEX(y)
			'''            Next y
			'''
			'''            ' Compute the grossed-up revenues
			'''            For y = 1 To LG
			'''                GREV(y) = ATotalRevenues(y)
			'''            Next y
			
			' Compute the group and company capex amounts, by expenditure
			'UPGRADE_WARNING: Lower bound of array my3Ex was changed from 0,gna_my3Ex_SizeD2_LB to 0,0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
			ReDim my3Ex(UBound(my3, 1), gna_my3Ex_SizeD2_UB)
			For y = 1 To my3tt
				' capture actual expenditure amounts (not balances, or the accrual abandonment)
				If my3(y, gc_nMY3_CAT) < CPXCategoryCodeBAL Or my3(y, gc_nMY3_CAT) = CPXCategoryCode_AbandonmentCashExpenditure Then
					If my3(y, gc_nMY3_CAT) <> 1 Then ' ignore bonus for Total Capex because it doesn't exit
						my3Ex(y, gna_my3Ex_TCX) = my3(y, gc_nMY3_AMT) ' the total-level capex
					End If
					my3Ex(y, gna_my3Ex_OCX) = my3(y, gc_nMY3_AMT) ' the operator-level capex
					my3Ex(y, gna_my3Ex_GCX) = my3(y, gc_nMY3_AMT) * GPRATE(y) ' the group (non-NOC) capex
					my3Ex(y, gna_my3Ex_CCX) = my3(y, gc_nMY3_AMT) * GPRATE(y) * WINC(y) ' the company capex
				End If
			Next y
			
		Else
			'''            ' Compute the grossed-up capital expenditures
			'''            ' Match up the EffIntsX interests with the MY3 amounts
			'''            ' For each item in MY3...
			'''            For y = 1 To my3tt
			'''                ' Find the corresponding item (based on category, exp. date and tangible percent)
			'''                For x = 1 To UBound(EffIntsX, 1)
			'''                    If EffIntsX(x, gc_nEffIntsX_CAT) = my3(y, gc_nMY3_CAT) And EffIntsX(x, gc_nEffIntsX_XMO) = my3(y, gc_nMY3_XMO) And EffIntsX(x, gc_nEffIntsX_XYR) = my3(y, gc_nMY3_XYR) And EffIntsX(x, gc_nEffIntsX_TAN) = my3(y, gc_nMY3_TAN) Then
			'''                        GMY3(x) = my3(y, gc_nMY3_AMT) / EffIntsX(x, gc_nEffIntsX_WIN) / (1 - EffIntsX(x, gc_nEffIntsX_PAR))
			'''                    End If
			'''                Next x
			'''            Next y
			'''
			'''            ' Compute the grossed-up operating expense amounts
			'''            For y = 1 To LG
			'''                GOPEX(y) = OPEX(y) / EffInts(y, gc_nEffInts_WOX) / (1 - EffInts(y, gc_nEffInts_POX))
			'''            Next y
			'''
			'''            ' Compute the grossed-up revenues
			'''            For y = 1 To LG
			'''                GREV(y) = ATotalRevenues(y) / EffInts(y, gc_nEffInts_WIN) / (1 - EffInts(y, gc_nEffInts_PAR))
			'''            Next y
			'''
			
			If xRunSwitches(RunSwitch_DCF) = RunSwitch_DCF_Only Then
				' doing the discounted cash flow and economic indicators only... skipping after-tax cash flow
				' have to fix up the company cash flows - for now only for g_bPTCons=true
				OPEX(0) = 0
				For i = 1 To LG
					PSCF(i, 1) = gna_ACFX(i, gna_ACFX_CPS)
					NGCF(i, 1) = gna_ACFX(i, gna_ACFX_CNG) - OPEX(i) * (1 - OPEXRATE(i)) * WIN(i)
					NGCF(i, 2) = OPEX(i) * (1 - OPEXRATE(i)) * WIN(i)
					OPEX(0) = OPEX(0) + OPEX(i)
				Next i
				For i = 1 To my3tt
					iDET = my3(i, 3) - YR + 1
					NGCF(iDET, 1) = NGCF(iDET, 1) - my3Ex(i, 3)
					NGCF(iDET, 3) = NGCF(iDET, 3) + my3Ex(i, 3)
				Next i
				PS = 1
				OPXP = 2
				CPXP = 3
				NEG = 3
				'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
                TotalArrays(CF, COCF, COCCF)
				GoTo AfDeflate
			End If
			
		End If
		' End (C0846)
		
		PS = 0
		'<<<<<< 29 Mar 2002 JWD (C0527)
		ReDim PSCF(LG, maximum_count_positive_cashflows)
		ReDim NGCF(LG, maximum_count_negative_cashflows)
		ReDim NGCFT(LG, maximum_count_negative_cashflows)
		ReDim NGCFA(LG, maximum_count_negative_cashflows)
		ReDim NGCFU(LG, maximum_count_negative_cashflows)
		'~~~~~~ was:
		'ReDim PSCF(LG, 12), NGCF(LG, 12), NGCFT(LG, 12), NGCFA(LG, 12), NGCFU(LG, 12)
		'>>>>>> End (C0527)
		
		For i = 1 To TDT
			If TD(i, 2) = "OIL" And PPR = 2 Then GoTo 6110
			If TD(i, 2) = "GAS" And PPR = 1 Then GoTo 6110
			If Left(TD(i, 4), 1) = "+" Then GoTo 6060
			GoTo 6110
6060: 'POSITIVE CASH FLOW
			PS = PS + 1
			For y = 1 To LG - AbandonmentPlacementOffset
				PSCF(y, PS) = RVN(y, i) * ERT(y)
			Next y
6110: Next i
		
		'''      ' 17 Dec 2004 JWD (C0846) Capture the positive cash flow variable amounts and gross-up
		'''      For y = 1 To LG
		'''          rTmp = 0
		'''          For i = 1 To PS%
		'''              ' Accumulate this year's amounts for all positive variables
		'''              rTmp = rTmp + PSCF(y, i)
		'''          Next i
		'''          ' Next assumes WIN(y) is never zero (it shouldn't be, it is set to 1 in FiscalDef) and PARTRATE(y) is never 1.
		'''          GPCFV(y) = rTmp / EffInts(y, gc_nEffInts_WIN) / (1 - EffInts(y, gc_nEffInts_PAR))
		'''      Next y
		'''      ' End (C0846)
		'''
		'PUT IN GOVERNMENT REPAYMENT
		If PRTA = 1 Or BURS = 1 Then
			GoTo 6125
		End If
		GoTo 6152
		
6125: PS = PS + 1
		REPY = PS
		
		For i = 1 To LG
			PSCF(i, PS) = TOTPMT(i) * ERT(i)
		Next i
		
		'''      ' 17 Dec 2004 JWD (C0846) Compute the gross amount of NOC repayment and net company partner reimbursement
		'''      ' First, compute the net company partner reimbursement amounts
		'''      For i = 1 To my3tt
		'''          If EffIntsX(i, gc_nEffIntsX_BUR) <> 0 Then
		'''              ' Compute the year of reimbursement, this is copied from Repay
		'''              y = my3(i, gc_nMY3_XYR) - YR + 1
		'''              If my3(i, gc_nMY3_XYR) < Y1 Then
		'''                  y = Y1 - YR + 1
		'''              End If
		'''              CNREIM(y) = CNREIM(y) + GMY3(i) * EffIntsX(i, gc_nEffIntsX_WIN) * (1 - EffIntsX(i, gc_nEffIntsX_PAR)) * EffIntsX(i, gc_nEffIntsX_BUR)
		'''          End If
		'''      Next i
		'''
		'''      ' Next, compute the grossed up NOC repayment. TOTPMT() is net to company and includes the partner reimbursement
		'''      For y = 1 To LG
		'''          ' Next assumes that WIN(y) is never zero (shouldn't be)
		'''          GREPAY(y) = (TOTPMT(y) - CNREIM(y)) / EffInts(y, gc_nEffInts_WIN)
		'''      Next y
		'''      ' End (C0846)
		'''
6152: 'FINANCE
		FINANCE(0) = 0
		
		For i = 1 To LG
			FINANCE(0) = FINANCE(0) + FINANCE(i)
		Next i
		
		If FINANCE(0) = 0 Then
			GoTo 6160
		End If
		
		PS = PS + 1
		fin = PS
		For i = 1 To LG
			PSCF(i, PS) = FINANCE(i) * ERT(i)
		Next i
		
		'''      ' 22 Dec 2004 JWD (C0846)
		'''      For i = 1 To LG
		'''          ' 22 Apr 2005 JWD (C0872) Change next 1
		'''          GFINAN(i) = FINANCE(i) / EffInts(i, gc_nEffInts_WFN) / (1 - EffInts(i, gc_nEffInts_PFN))
		'''          ' was:
		'''          'GFINAN(i) = FINANCE(i) / EffInts(y, gc_nEffInts_WFN) / (1 - EffInts(y, gc_nEffInts_PFN))
		'''          ' End (C0872)
		'''      Next i
6160: 
		
		NEG = 0
		For i = 1 To TDT
			If TD(i, 2) = "OIL" And PPR = 2 Then GoTo 6330
			If TD(i, 2) = "GAS" And PPR = 1 Then GoTo 6330
			If Left(TD(i, 4), 1) = "+" Or Left(TD(i, 4), 1) = " " Then GoTo 6330
			
			CountNegatives(i, TRDT, ADJT, USTT)
			
			For y = 1 To LG - AbandonmentPlacementOffset
				If Left(TD(i, 4), 1) = "-" Then
					NGCF(y, NEG) = RVN(y, i) * ERT(y)
				ElseIf Left(TD(i, 4), 1) = "T" Then  '3rd party cashflow
					NGCF(y, NEG) = RVN(y, i) * ERT(y)
					NGCFT(y, NEGT) = RVN(y, i) * ERT(y)
					' 20 Dec 2004 JWD (C0846) Add to total 3rd party cash flow
					C3DPTY(y) = C3DPTY(y) + NGCFT(y, NEGT)
				ElseIf Left(TD(i, 4), 1) = "A" Then  'adjustment cashflow
					NGCF(y, NEG) = RVN(y, i) * ERT(y)
					NGCFA(y, NEGA) = RVN(y, i) * ERT(y)
				ElseIf Left(TD(i, 4), 1) = "U" Then  'US Tax cashflow
					NGCF(y, NEG) = RVN(y, i) * ERT(y)
					NGCFU(y, NEGU) = RVN(y, i) * ERT(y)
				End If
			Next y
6330: Next i
		
		'''      ' 17 Dec 2004 JWD (C0846) Capture the negative cash flow variable amounts and gross-up
		'''      For y = 1 To LG
		'''          rTmp = 0
		'''          For i = 1 To NEG%
		'''              ' Accumulate this year's amounts for all negative variables (less T variables)
		'''              rTmp = rTmp + NGCF(y, i) - NGCFT(y, i)
		'''          Next i
		'''          ' Next assumes WIN(y) is never zero (it shouldn't be, it is set to 1 in FiscalDef) and PARTRATE(y) is never 1.
		'''          GNCFV(y) = rTmp / EffInts(y, gc_nEffInts_WIN) / (1 - EffInts(y, gc_nEffInts_PAR))
		'''      Next y
		'''      ' End (C0846)
		
		UserNegatives = NEG '# of user defined negative (-) cash flows
		TAUItems = NEGT + NEGA + NEGU '# of user def neg (T,A,U) cash flows
		
		
		'OPERATING EXPENSES
		OPEX(0) = 0
		
		For i = LG - AbandonmentPlacementOffset + 1 To UBound(OPEX)
			OPEX(i) = 0
		Next i
		
		For i = 1 To LG - AbandonmentPlacementOffset
			OPEX(0) = OPEX(0) + OPEX(i)
		Next i
		
		If OPEX(0) = 0 Then GoTo 6380
		
		NEG = NEG + 1
		OPXP = NEG
		For i = 1 To LG
			NGCF(i, NEG) = OPEX(i) * (1 - OPEXRATE(i)) * ERT(i) * WIN(i)
		Next i
		
6380: 'CAPITAL EXPENDITURES
		If my3tt = 0 Then GoTo 6430
		
		NEG = NEG + 1
		CPXP = NEG
		For i = 1 To my3tt
			'<<<<<< 4 Aug 2001 JWD (C0363)
			If my3(i, 1) < CPXCategoryCodeBAL Or my3(i, 1) = CPXCategoryCode_AbandonmentCashExpenditure Then
				'~~~~~~ was:
				''<<<<<< 21 Jun 2001 JWD (C0339)
				'If my3(i, 1) < CPXCategoryCodeBAL Or my3(i, 1) > CPXCategoryCodeBL3 Then        'skip BAL, BL2 & BL3
				''~~~~~~ was:
				''If my3(i, 1) < 18 Or my3(i, 1) > 20 Then        'skip BAL, BL2 & BL3
				''>>>>>> End (C0339)
				'>>>>>> End (C0363)
				iDET = my3(i, 3) - YR + 1
				NGCF(iDET, NEG) = NGCF(iDET, NEG) + my3(i, 5) * GPRATE(i) * WINC(i)
			End If
6420: Next i
		
6430: 'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        TotalArrays(CF, COCF, COCCF)
		
		' capture the positive and negative cash flows into designated array locations
		If xxx_POSCF > 0 And xxx_NEGCF > 0 Then
			For i = 1 To LG
				gna_ACFX(i, xxx_POSCF) = PSCF(i, PS + 1)
				gna_ACFX(i, xxx_NEGCF) = NGCF(i, NEG + 1)
			Next i
		End If
		
AfDeflate: 
		' -----------------------------------------------------------------------
		' CALCULATE UNDISCOUNTED VARIABLES FOR DEFLATED CASH FLOW PAGE
		' FIRST DO COMPANY CASH FLOW AND CUMULATIVE CASH FLOW
		
		ReDim COCCF(LG)
		
		For i = 1 To LG
			If DFL(i) <> 0 Then
				COCF(i) = CF(i) / DFL(i)
			Else
				COCF(i) = 0
			End If
		Next i
		
		COCCF(1) = COCF(1)
		For i = 2 To LG
			COCCF(i) = COCCF(i - 1) + COCF(i)
		Next i
		
6432: 
		
		'''        ' 17 Dec 2004 JWD (C0846) Add calculation of 3rd party and NOC cash flows
		'''
		'''        ' Do third party cash flow
		'''        ' Third party cash flow is:
		'''        ' GPCFV is 100%; is cash received by group (COMPANY and 3rd party) and by NOC (participation)
		'''        ' GNCFV, OPEX are 100%; & is cash paid out by group (COMPANY and 3rd party) and by NOC (participation)
		'''        ' GREPAY is 100%; is paid by NOC to group (COMPANY and 3rd Party) for carried capital
		'''        ' GFINAN is 100%; net financing (loans and repayment of) is received & paid by group and by NOC (participation)
		'''        ' CNREIM is net to the COMPANY; is the amount of reimbursement of carried capital between COMPANY and 3rd party (amounts received by COMPANY are positive, paid by COMPANY are negative)
		'''        ' C3DPTY is net to the COMPANY; is amount of payments to 3rd party that are defined in fiscal model
		
		' Condition on not a pre-tax consolidation
		For i = 1 To LG
			'''            TDPCF(i) = ((GPCFV(i) - GNCFV(i)) * (1 - EffInts(i, gc_nEffInts_PAR)) + GREPAY(i)) * (1 - EffInts(i, gc_nEffInts_WIN)) - GOPEX(i) * (1 - EffInts(i, gc_nEffInts_POX)) * (1 - EffInts(i, gc_nEffInts_WOX)) - CNREIM(i) + C3DPTY(i) + (GFINAN(i) * (1 - EffInts(i, gc_nEffInts_PFN)) * (1 - EffInts(i, gc_nEffInts_WFN)))
			TDPCF(i) = gna_ACFX(i, gna_ACFX_GPS) - gna_ACFX(i, gna_ACFX_GNG) - gna_ACFX(i, gna_ACFX_CPS) + gna_ACFX(i, gna_ACFX_CNG)
		Next i
		
		'''        ' Add in the capital expenditure amounts
		'''        For i = 1 To my3tt
		'''            y = my3(i, gc_nMY3_XYR) - YR + 1
		'''            If my3(i, gc_nMY3_CAT) < CPXCategoryCodeBAL Or my3(i, gc_nMY3_CAT) = CPXCategoryCode_AbandonmentCashExpenditure Then
		'''                TDPCF(y) = TDPCF(y) - GMY3(i) * (1 - EffIntsX(i, gc_nEffIntsX_PAR)) * (1 - EffIntsX(i, gc_nEffIntsX_WIN))
		'''            End If
		'''        Next i
		
		For i = 1 To LG
			If DFL(i) <> 0 Then
				TDPCF(i) = TDPCF(i) / DFL(i)
			Else
				TDPCF(i) = 0
			End If
		Next i
		
		TDPCCF(1) = TDPCF(1)
		For i = 2 To LG
			TDPCCF(i) = TDPCCF(i - 1) + TDPCF(i)
		Next i
		
		' Do NOC cash flow
		
		For i = 1 To LG
			'''           NOCCF(i) = (GPCFV(i) - GNCFV(i)) * EffInts(i, gc_nEffInts_PAR) - GOPEX(i) * EffInts(i, gc_nEffInts_POX) - GREPAY(i) + (GFINAN(i) * EffInts(i, gc_nEffInts_PFN))
			NOCCF(i) = gna_ACFX(i, gna_ACFX_OPS) - gna_ACFX(i, gna_ACFX_ONG) - gna_ACFX(i, gna_ACFX_GPS) + gna_ACFX(i, gna_ACFX_GNG)
		Next i
		
		'''        ' Add in the capital expenditure amounts
		'''        Debug.Print "NOCCF(y) = NOCCF(y) - GMY3(i) * EffIntsX(i, gc_nEffIntsX_PAR)"
		
		'''        For i = 1 To my3tt
		'''            y = my3(i, gc_nMY3_XYR) - YR + 1
		'''            If my3(i, gc_nMY3_CAT) < CPXCategoryCodeBAL Or my3(i, gc_nMY3_CAT) = CPXCategoryCode_AbandonmentCashExpenditure Then
		'''                NOCCF(y) = NOCCF(y) - GMY3(i) * EffIntsX(i, gc_nEffIntsX_PAR)
		'''            End If
		'''        Next i
		
		For i = 1 To LG
			If DFL(i) <> 0 Then
				NOCCF(i) = NOCCF(i) / DFL(i)
			Else
				NOCCF(i) = 0
			End If
		Next i
		
		NOCCCF(1) = NOCCF(1)
		For i = 2 To LG
			NOCCCF(i) = NOCCCF(i - 1) + NOCCF(i)
		Next i
		' End (C0846)
		
		' =======================================================================
		' NOW DO GOVERNMENT CASH FLOW
		' =======================================================================
		
		'''         'compute capex
		'''      For i = 1 To my3tt
		'''         '<<<<<< 4 Aug 2001 JWD (C0363)
		'''         If my3(i, 1) < CPXCategoryCodeBAL Or my3(i, 1) = CPXCategoryCode_AbandonmentCashExpenditure Then
		'''         '~~~~~~ was:
		'''         ''<<<<<< 21 Jun 2001 JWD (C0339)
		'''         'If my3(i, 1) < CPXCategoryCodeBAL Or my3(i, 1) > CPXCategoryCodeBL3 Then ' BAL, BL2, BL3
		'''         ''~~~~~~ was:
		'''         ''If my3(i, 1) <> 18 Then
		'''         ''>>>>>> End (C0339)
		'''         '>>>>>> End (C0363)
		'''            y = my3(i, 3) - YR + 1
		'''            If my3(i, 1) = 1 Then       'if categ. 1 then Gov. gets 100%
		'''               CP(y) = CP(y) - my3(i, 5)
		'''            Else
		'''               CP(y) = CP(y) + (my3(i, 5) * (1 - GPRATE(i)))
		'''            End If
		'''         End If
		'''      Next i
		
6434: 
		'do positive cash flows for government
		'if its repayment or finance, they do not apply to government
		' 17 Dec 2004 JWD (C0846) Replaced government cash flow calc with code following
		'
		'      For i = 1 To LG
		'                                'govt starts at 100% gross revenues
		'         ' Changed GDP 20 Jan 2003
		'         ' GVCF(i) = (A(i, 1) * A(i, 7)) + (A(i, 2) * A(i, 8)) + (A(i, 3) * A(i, 9)) + (A(i, 4) * A(i, 10))
		'         GVCF(i) = ATotalRevenues(i)
		'
		'         For y = 1 To PS%
		'            If y = REPY% Or y = fin% Then
		'            Else
		'               If WIN(i) <> 0 Then
		'                  GVCF(i) = GVCF(i) - (PSCF(i, y) / WIN(i))
		'               End If
		'            End If
		'         Next y
		'6436
		'                                'take out capital and opex
		'         NEGLP% = NEG%
		'         If my3tt <> 0 Then
		'            NEGLP% = NEGLP% - 1
		'         End If
		'         If OPEX(0) <> 0 Then
		'            NEGLP% = NEGLP% - 1
		'         End If
		'         For y = 1 To NEGLP%             'do negative cash flows for government
		'            If WIN(i) <> 0 Then
		'               GVCF(i) = GVCF(i) + ((NGCF(i, y) / WIN(i)))
		'            End If
		'         Next y
		'6438
		'                'Back out T & U negatives from GOVT
		'                'T & U negatives do not go to (or come from)
		'                '  the GOVT (Even though they are in NGCF())
		'         For y = 1 To NEGT%             'do negative cash flows for government
		'            If WIN(i) <> 0 Then
		'               GVCF(i) = GVCF(i) - ((NGCFT(i, y) / WIN(i)))
		'            End If
		'         Next y
		'6439
		'         For y = 1 To NEGU%             'do negative cash flows for government
		'            If WIN(i) <> 0 Then
		'               GVCF(i) = GVCF(i) - ((NGCFU(i, y) / WIN(i)))
		'            End If
		'         Next y
		'6442
		'                'do capex
		'         GVCF(i) = GVCF(i) - CP(i)
		'         GVCF(i) = GVCF(i) - (OPEX(i) * OPEXRATE(i))     'do opex
		'         If WIN(i) <> 0 Then                             'do TOTPMT()
		'            GVCF(i) = GVCF(i) - (TOTPMT(i) / WIN(i))
		'         Else
		'            GVCF(i) = 0
		'         End If
		'6446
		' End (C0846)
		
		For i = 1 To LG
			' Government cash flow is total revenues less all positive cash flows
			' (going to company & partners (3rd party)) plus any negative cash flows
			' (excluding T cash flows, opex and capex)
			'''         GVCF(i) = GREV(i) - GPCFV(i) + GNCFV(i)
			GVCF(i) = gna_ACFX(i, gna_ACFX_TPS) - gna_ACFX(i, gna_ACFX_TNG) - gna_ACFX(i, gna_ACFX_OPS) + gna_ACFX(i, gna_ACFX_ONG)
		Next i
		
		'''      ' Add in the capital expenditure amounts (bonuses)
		'''      For i = 1 To my3tt
		'''          y = my3(i, gc_nMY3_XYR) - YR + 1
		'''          If my3(i, gc_nMY3_CAT) = 1 Then
		'''              GVCF(y) = GVCF(y) + GMY3(i)
		'''          End If
		'''      Next i
		
		For i = 1 To LG
			'applying the deflator
			If DFL(i) <> 0 Then
				GVCF(i) = GVCF(i) / DFL(i)
			Else
				GVCF(i) = 0
			End If
		Next i
		
		'accumulate gov.
		GVCCF(1) = GVCF(1)
		For i = 2 To LG
			GVCCF(i) = GVCCF(i - 1) + GVCF(i)
		Next i
		
		
		' GDP 20 Jan 2003
		' Commented out code below - not used
		'6448
		'        'save OXY Database items
		'      If RF$(3) = "OXY" Then          'Camefrom$ = "DISPLAY" THEN
		'         SaveOXYSerialNo DashSerial$
		'      Else
		'         DashSerial$ = ""
		'      End If
6450: 
		' GDP 08 Apr 2003
		' Commented out - removed below functions
		' only used for OXY
		'LoadAnnData
		'LoadPrjData
		'LoadDocData
		'++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		'++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		'++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		
		'if RF$(1) = "CONSOL", you rejoin normal run here from 6008
6735: 
		' GDP 20 Jan 2003
		' Commented out below code - not used
		
		'      If RF$(1) = "CONSOL" And RF$(3) = "OXY" Then
		'         SaveOXYSerialNo DashSerial$
		'      End If
		If xRunSwitches(RunSwitch_DCF) = RunSwitch_DCF_Only Then
			GoTo 24000
		End If
		
		If RF(5) = "ALL" Or RF(5) = "VAR" Or RF(5) = "SUM" Then
			GoTo 6740
		End If
		GoTo 24000
		
		'--------------------------------------------------------------------
6740: 
		' Add for capturing column data for ASPEEngine
		Dim l_iTypes() As Short
		Dim l_sCodes() As String
		'UPGRADE_WARNING: Lower bound of array l_iTypes was changed from LBound(ColumnNm$) to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
		ReDim l_iTypes(UBound(ColumnNm))
		'UPGRADE_WARNING: Lower bound of array l_sCodes was changed from LBound(ColumnNm$) to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
		ReDim l_sCodes(UBound(ColumnNm))
		
		'PRINT POSITIVE CASH FLOW TITLES
		'setting the ColumnNm$() counter
		Counter = 1
		For i = 1 To TDT
			If TD(i, 2) = "OIL" And PPR = 2 Then GoTo 7180
			If TD(i, 2) = "GAS" And PPR = 1 Then GoTo 7180
			
			If Left(TD(i, 4), 1) = "+" Then
				'here if doing primary product AND if cash flow effect = "+"
				MatchTitles(TD(i, 1), CStr(ColumnNm(Counter))) 'FINDS MATCHING SHORT REPORT TITLE
				' Capture type and user-defined variable code for this profile
				l_iTypes(Counter) = 8
				l_sCodes(Counter) = TD(i, 1)
				Counter = Counter + 1
			End If
7180: Next i
		
		Counter = Counter - 1 'to offset the for/next loop for incrementing an extra 1
		'assigning repay if there is any
		If REPY <> 0 Then
			Counter = Counter + 1
			ColumnNm(Counter) = CSng(" REPAY ")
			l_iTypes(Counter) = 1
		End If
		If fin <> 0 Then
			Counter = Counter + 1
			ColumnNm(Counter) = CSng("FINANCE")
			l_iTypes(Counter) = 2
		End If
		Counter = Counter + 1
		ColumnNm(Counter) = CSng(" POSTOT")
		l_iTypes(Counter) = 3
		
		'assigning the number of postive cf
		PosCtr = Counter
		
		Counter = Counter + 1
		For i = 1 To TDT
			If TD(i, 2) = "OIL" And PPR = 2 Then GoTo 7240
			If TD(i, 2) = "GAS" And PPR = 1 Then GoTo 7240
			cflo = Left(TD(i, 4), 1)
			
			If cflo = "-" Or cflo = "T" Or cflo = "A" Or cflo = "U" Then
				MatchTitles(TD(i, 1), CStr(ColumnNm(Counter))) 'FINDS MATCHING SHORT REPORT TITLE
				' Capture type and user-defined variable code for this profile
				l_iTypes(Counter) = 8
				l_sCodes(Counter) = TD(i, 1)
				Counter = Counter + 1
			End If
7240: Next i
		
		Counter = Counter - 1 'to offset the for/next loop for incrementing an extra 1
		If OPXP <> 0 Then
			Counter = Counter + 1
			ColumnNm(Counter) = CSng("OPEREXP")
			l_iTypes(Counter) = 5
		End If
		If CPXP <> 0 Then
			Counter = Counter + 1
			ColumnNm(Counter) = CSng("CAPITAL")
			l_iTypes(Counter) = 4
		End If
		Counter = Counter + 1
		ColumnNm(Counter) = CSng(" NEGTOT")
		l_iTypes(Counter) = 6
		
		'assigning the number of negative cf
		NegCtr = Counter - PosCtr
		
		Counter = Counter + 1 'page total column
		ColumnNm(Counter) = CSng("    NCF")
		l_iTypes(Counter) = 7
		
		'checking to see if user have pass allowable limits on POS or NEG
		'<<<<<< 29 Mar 2002 JWD (C0527)
		If PS > maximum_count_positive_cashflows - 1 Or NEG > maximum_count_negative_cashflows - 1 Then
			'~~~~~~ was:
			'If PS% > 11 Or NEG% > 11 Then
			'>>>>>> End (C0527)
			'<<<<<< 19 Mar 2002 JWD (C0500)
			Error(ErrorCode_ExceededMaxCashFlowCount)
			'~~~~~~ was:
			'Error 251
			'>>>>>> End (C0500)
		End If
		
		'Write out the Values to DUM()
		For i = 1 To LG
			AtcfCtr = 1
			For y = 1 To PS
				DUM(i, AtcfCtr) = PSCF(i, y)
				AtcfCtr = AtcfCtr + 1
			Next y
			DUM(i, AtcfCtr) = PSCF(i, PS + 1) 'total positives
			
			AtcfCtr = AtcfCtr + 1
			For y = 1 To NEG
				DUM(i, AtcfCtr) = NGCF(i, y)
				AtcfCtr = AtcfCtr + 1
			Next y
			DUM(i, AtcfCtr) = NGCF(i, NEG + 1) 'total negatives column
			AtcfCtr = AtcfCtr + 1
			DUM(i, AtcfCtr) = CF(i) 'page total column
		Next i
		
		
		'<<<<<< 29 Mar 2002 JWD (C0527)
		'<Note: No longer concerned with splitting the section if
		' more than 12 columns. Output all columns in one section>
		ConCur = sCur 'for consolidation
		
		' 3 Feb 2004 JWD (C0776) Remove writes, replaced with report object
		''''Write #5, 10, YR, 0, LG, Counter%, "AFTER TAX CASH FLOW", 10, FinalWin, FINALPARTIC, sCur
		''''Call WriteFour(Counter%, ColumnNm$())    'COLUMN HEADS
		''''Call WriteOne(AtcfCtr%, DUM())           'DATA
		
		Dim oPg1 As CGiantRptPageD1
		oPg1 = g_oReport.NewAfterTaxCashflowRptPage
		With oPg1
			.SetPageHeader(10, YR, 0, LG, Counter, "AFTER TAX CASH FLOW", 10, FinalWin, FINALPARTIC, sCur)
			.SetProfileHeaders(ColumnNm)
			.SetProfileTypesAndTitles(l_iTypes, l_sCodes)
			.SetProfileValues(DUM)
		End With
		'~~~~~~ was:
		'   ' Checking to see if there are more than 12 columns for the Afcf report
		'   ' if there is more than write out 2 pages.
		'   ' -----------------------------------------------------------------------
		'ConCur$ = sCur             'for consolidation
		'If Counter% <= 12 Then
		'                    'Page type, Start year, Page counter, life of field, number of columns, page title, column length
		'   Write #5, 10, YR, 0, LG, Counter%, "AFTER TAX CASH FLOW", 10, FinalWin, FINALPARTIC, sCur
		'   Call WriteFour(Counter%, ColumnNm$())    'COLUMN HEADS
		'   Call WriteOne(AtcfCtr%, DUM())           'DATA
		'Else                       'write out 2 pages
		'   'do the positive cashflows first
		'
		'   NoPosCol% = PosCtr% + 2
		'   If NoPosCol% > 12 Then
		'      NoPosCol% = 12
		'   End If
		'                    'Page type, Start year, Page counter, life of field, number of columns, page title, column length
		'   Write #5, 10, YR, 0, LG, NoPosCol%, "AFTER TAX CASH FLOW", 10, FinalWin, FINALPARTIC, sCur
		'   Call WriteFive(PosCtr%, Counter%, REPY%, FIN%, ColumnNm$())      'Heads
		'   Call WriteTwo(PosCtr%, AtcfCtr%, REPY%, FIN%, DUM())     'data
		'          'now do the negative cashflows
		'   Nonegcol% = NegCtr% + 2
		'   If Nonegcol% > 12 Then
		'      Nonegcol% = 12
		'   End If
		'                    'Page type, Start year, Page counter, life of field, number of columns, page title, column length
		'   Write #5, 15, YR, 0, LG, Nonegcol%, "AFTER TAX CASH FLOW", 10, FinalWin, FINALPARTIC, sCur
		'   Call WriteSix(NegCtr%, Counter%, ColumnNm$())    'columns titles
		'   Call WriteThree(NegCtr%, AtcfCtr%, DUM())        'write out the values
		'End If     'end of WRITE 1 OR 2 pages test
		'>>>>>> End (C0527)
		
		'--------------------------------------------------------------------------------
24000: 'THIS CALCULATES PRESENT VALUES AND ROR
		'=====================================================================
		'OXY specific items need to be calculated here
		'THESE ITEMS ARE USED IN GNTOXY1.EXE
		
		If xRunSwitches(RunSwitch_DCF) = RunSwitch_DCF_Off Then
			GoTo 39000
		End If
		
		
		'calculate WINREV! (sum co. cashflows / proj gross revenues)
		firstyr = Y3 - YR + 1 'points at discount year in arrays
		If Left(RF(1), 6) <> "CONSOL" Then
			'if CONSOL, we get the values of these items from the
			'  consolidated items in common
			
			' Added GDP 24/01/2000 to make sure that this code runs through
			' OK when the discount date is prior to the project start date
			If firstyr < 1 Then
				firstyr = 1
			End If
			For q = firstyr To LG
				' GDP 20 Jan 2003
				' Replaced revenue calculation with call to ATotalRevenues
				'OXYGrRev! = OXYGrRev! + ((A(q%, 1) * A(q%, 7)) + (A(q%, 2) * A(q%, 8)) + (A(q%, 3) * A(q%, 9)) + (A(q%, 4) * A(q%, 10)))
				OXYGrRev = OXYGrRev + ATotalRevenues(q)
				
				If PS > 0 Then
					OXYTtlPos = OXYTtlPos + PSCF(q, PS + 1)
				End If
			Next q
			'(atcf cum + "T" items) / (Fld grs income - grs OPEX - grs CAPEX)
			'ttlcap! has total capital from grossrpt.exe
			'OPEX(0) has total operating cost from above
			
		End If
		
		If OXYGrRev > 0 Then
			WinRev = OXYTtlPos / OXYGrRev
		End If
		
		'=====================================================================
		
		' ie last run of RUN file.
		'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        x27100(rUDCF, ZD, zp, TorULines, OPXP, CPXP, DRT, ESPV, u1, u2, U3, U11, U12, U4, U7, U13, PAY, ATCF, CODCF, POT, GOVT, GVDCF, iEntityID, eEntityID_Company, iCFCol, X7, U8, GVTK, vpr, iDET, bRMN, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, U26, U23, TATCF, TDPCDCF, U27, U36, U33, NATCF, NOCDCF, U37, RR, RRZ, RRT, RRN, RRB, U5, RU, RE, rl, CU, cl, U5L, CM1, CM2) ' CALCULATES ESPV(1 - 13,6) FOR ECONOMIC SUMMARY PAGE


        GoTo 30000


        '--------------------------------------------------------------------

30000:
        If RF(5) = "ALL" Or RF(5) = "VAR" Or RF(5) = "SUM" Then
            GoTo 30010
        Else
            GoTo 30240 'else skip this part
        End If
30010:  ' Deflated Cash Flow Report

        'Page type, Start year, Page counter, life of field, number of columns, page title, column length
        ' 3 Feb 2004 JWD (C0776) Remove writes
        ''''Write #5, 13, YR, 0, LG, 9, "DEFLATED CASH FLOW", 10, FinalWin, FINALPARTIC, sCur
        '''30040  WRITE #5, 13, YR, 0, LG, 9, "DEFLATED CASH FLOW", 10, WINT, PRTA, sCur

        ' Deflated Cashflow Column Names

        ColumnNm(1) = CSng("DEFLATE")
        ColumnNm(2) = CSng(" COMPCF")
        ColumnNm(3) = CSng("COMPCCF")
        ColumnNm(4) = CSng("COMPDCF")
        ColumnNm(5) = CSng(" COCDCF")
        ' 6 Dec 2005 JWD (C0846)  Add new cash flow columns
        ColumnNm(6) = CSng("3DPCF")
        ColumnNm(7) = CSng("3DPCCF")
        ColumnNm(8) = CSng("3DPDCF")
        ColumnNm(9) = CSng("3DPCDCF")
        ColumnNm(10) = CSng("NOCCF")
        ColumnNm(11) = CSng("NOCCCF")
        ColumnNm(12) = CSng("NOCDCF")
        ColumnNm(13) = CSng("NOCCDCF")
        ' End (C0846)
        ColumnNm(14) = CSng(" GOVTCF")
        ColumnNm(15) = CSng("GOVTCCF")
        ColumnNm(16) = CSng("GOVTDCF")
        ColumnNm(17) = CSng("GOVCDCF")


        ' 3 Feb 2004 JWD (C0776) Remove writes
        ''''Write #5, ColumnNm$(1), ColumnNm$(2), ColumnNm$(3), ColumnNm$(4), ColumnNm$(5), ColumnNm$(6), ColumnNm$(7), ColumnNm$(8), ColumnNm$(9)

        Dim oPg2 As IGiantRptPageAssignStd
        oPg2 = g_oReport.NewStandardRptPage

        ' 6 Dec 2005 JWD (C0846) Change the row count for the new cash flows and add to SetProfileHeaders()
        oPg2.SetPageHeader(13, YR, 0, LG, 17, "DEFLATED CASH FLOW", 10, FinalWin, FINALPARTIC, sCur)
        oPg2.SetProfileHeaders(ColumnNm(1), ColumnNm(2), ColumnNm(3), ColumnNm(4), ColumnNm(5), ColumnNm(6), ColumnNm(7), ColumnNm(8), ColumnNm(9), ColumnNm(10), ColumnNm(11), ColumnNm(12), ColumnNm(13), ColumnNm(14), ColumnNm(15), ColumnNm(16), ColumnNm(17))
        ' was:
        'oPg2.SetPageHeader 13, YR, 0, LG, 9, "DEFLATED CASH FLOW", 10, FinalWin, FINALPARTIC, sCur
        'oPg2.SetProfileHeaders ColumnNm$(1), ColumnNm$(2), ColumnNm$(3), ColumnNm$(4), ColumnNm$(5), ColumnNm$(6), ColumnNm$(7), ColumnNm$(8), ColumnNm$(9)
        ' End (C0846)

        ' COMPUTE CUMULATIVE DISCOUNTED VARIABLES

        COCDCF(1) = CODCF(1)
        GVCDCF(1) = GVDCF(1)
        ' 6 Dec 2005 JWD (C0846)
        TDPCDCF(1) = TDPDCF(1)
        NOCCDCF(1) = NOCDCF(1)
        ' End (C0846)
        For i = 2 To LG
            COCDCF(i) = COCDCF(i - 1) + CODCF(i)
            GVCDCF(i) = GVCDCF(i - 1) + GVDCF(i)
            ' 6 Dec 2005 JWD (C0846)
            TDPCDCF(i) = TDPCDCF(i - 1) + TDPDCF(i)
            NOCCDCF(i) = NOCCDCF(i - 1) + NOCDCF(i)
            ' End (C0846)
        Next i

        For i = 1 To LG
            ' 3 Feb 2004 JWD (C0776) Remove writes
            ''''Write #5, DFL(i), COCF(i), COCCF(i), CODCF(i), COCDCF(i), GVCF(i), GVCCF(i), GVDCF(i), GVCDCF(i)
            ' 6 Dec 2005 JWD (C0846) Add new cash flow symbols to call
            oPg2.SetProfileValues(i, DFL(i), COCF(i), COCCF(i), CODCF(i), COCDCF(i), TDPCF(i), TDPCCF(i), TDPDCF(i), TDPCDCF(i), NOCCF(i), NOCCCF(i), NOCDCF(i), NOCCDCF(i), GVCF(i), GVCCF(i), GVDCF(i), GVCDCF(i))
            ' was:
            'oPg2.SetProfileValues i, DFL(i), COCF(i), COCCF(i), CODCF(i), COCDCF(i), GVCF(i), GVCCF(i), GVDCF(i), GVCDCF(i)
            ' End (C0846)
        Next i

30240:  'End of Deflated Cash Flow Report

        'store OXY data that comes from economic summary page
        'THESE ITEMS ARE USED IN GNTOXY1.EXE
        'GTAKEnn! govt take for given discount rate % (0, 10, 15, & 20)
        'GNTNCF! (GNTNPVnn!) = net present values
        'GNTPInn! = profitibility index (cashflow / discounted investment)
        '  giant PI = (cashflow-disc investment) / discounted investment
        '  so we subtract 1 to get OXY number
        StoreOXYEconSumm(ESPV, RR)



        '====================================================================




        StoreGraphData(ESPV) ' TO STORE THE DATA FOR GRAPH


        'MKD 2-24-94  State Take added per G Kellas' request for use
        '  in generating Review of Fiscal Regimes.  He has defined
        '  state take as: (govt cashflow) / (govt cashflow + company cashflow)
        'We calculate the value for each of discount rates 1-6 and store the
        '  results in a file called STATETAK.SUM which will be read
        '  when we load the run summary pages.  These values have also been
        '  added to the economic summary page.
        'Both files are temporary and will be erased at the end of the run execution.

        'UPGRADE_WARNING: Lower bound of array StateTake was changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        Dim StateTake(6) As Single

        '10-18-95  If PAR never turned on, use Govt take for state
        '            take - they should be the same.
        '          If PAR is on, gross up company cashflow to "group level" by
        '            dividing by FinalWin.  This isn't perfect since WIN can
        '            change throughout the project.  But it is close.  It can
        '            also be wrong depending on repayment (or not) of carries.
        '
        'OLD      FOR q% = 1 TO 6
        'OLD        StateTake!(i) = (ESPV(5, i) / (ESPV(5, i) + ESPV(9, i))) * 100
        'OLD      NEXT i

        Dim fCashTotalCashFlow As Single
        For i = 1 To 6
            If PRTA = 0 Then 'PAR not on
                StateTake(i) = ESPV(13, i)
            ElseIf PRTA = 1 Then  'PAR is turned on
                If WINT = 1 Then 'WIN turned on
                    If FinalWin <> 0 Then
                        grp = ESPV(9, i) / FinalWin
                    Else
                        grp = 0
                    End If
                Else
                    grp = ESPV(9, i) 'WIN not turned on
                End If
                If (ESPV(5, i) + grp) <> 0 Then

                    fCashTotalCashFlow = ESPV(5, i) + ESPV(19, i)

                    StateTake(i) = (fCashTotalCashFlow / (fCashTotalCashFlow + grp)) * 100
                    'StateTake!(i) = (ESPV(5, i) / (ESPV(5, i) + grp!)) * 100
                Else
                    StateTake(i) = 0
                End If
            End If

            '         If bDebugging Then
            '            Open "statetak.log" For Append As #17
            '               Print #17, " "
            '               Print #17, "fill out state take("; i; ") "
            '               Print #17, "   co   = "; ESPV(9, i)
            '               Print #17, "   grp  = "; grp!
            '               Print #17, "   gvt  = "; ESPV(5, i)
            '               Print #17, "   win  = "; FinalWin
            '               Print #17, "   take = "; StateTake!(i)
            '            Close #17
            '         End If
        Next i
        '10-18-95  end of changed section

        ''' 9 Feb 2004 JWD (C0783) Remove output of STATETAK.SUM file
        '''        'now, write the values out to file
        '''      flno% = FreeFile
        '''        'values used for Run summary page for each run
        '''      If RNU = 1 Then
        '''         Open sRptDir + "STATETAK.SUM" For Output As #flno%
        '''      Else
        '''         Open sRptDir + "STATETAK.SUM" For Append As #flno%
        '''      End If
        '''      Write #flno%, StateTake!(1), StateTake!(2), StateTake!(3), StateTake!(4), StateTake!(5), StateTake!(6)
        '''      Close #flno%

        ' 14 Oct 2004 JWD (C0845) Remove conditional, always do indicators page
        'If RF$(5) = "ALL" Or RF$(5) = "VAR" Or RF$(5) = "SUM" Then
        '   GoTo 36505
        'Else
        '   GoTo 38000
        'End If
        ' End (C0845)
36505:
        'Printing the present value rates
        'Page type, Start year, Page counter, life of field, number of columns, page title, column length

        ' 3 Feb 2004 JWD (C0776) Remove writes
        ''''Write #5, 11, YR, 0, LG, 6, "ECONOMIC SUMMARY", 12, FinalWin, FINALPARTIC, sCur

        'present values
        ''''Write #5, gn(4), gn(5), gn(6), gn(7), gn(8), gn(9)
        ss = "########.###" : TS = "#########.##"
        ''''Write #5, "Co. Income", ESPV(1, 1), ESPV(1, 2), ESPV(1, 3), ESPV(1, 4), ESPV(1, 5), ESPV(1, 6)
        ''''Write #5, "Co. Oper. Costs", ESPV(2, 1), ESPV(2, 2), ESPV(2, 3), ESPV(2, 4), ESPV(2, 5), ESPV(2, 6)
        ''''Write #5, "Co. Capital", ESPV(3, 1), ESPV(3, 2), ESPV(3, 3), ESPV(3, 4), ESPV(3, 5), ESPV(3, 6)
        ''''Write #5, "Co. Royalty/Tax", ESPV(4, 1), ESPV(4, 2), ESPV(4, 3), ESPV(4, 4), ESPV(4, 5), ESPV(4, 6)
        ''''Write #5, "Co. Cash Flow", ESPV(9, 1), ESPV(9, 2), ESPV(9, 3), ESPV(9, 4), ESPV(9, 5), ESPV(9, 6)
        ''''Write #5, "Co. Payout, Years", ESPV(10, 1), ESPV(10, 2), ESPV(10, 3), ESPV(10, 4), ESPV(10, 5), ESPV(10, 6)
        ''''Write #5, "Co. Risk Return Ratio", ESPV(11, 1), ESPV(11, 2), ESPV(11, 3), ESPV(11, 4), ESPV(11, 5), ESPV(11, 6)
        ''''Write #5, "Co. Profitability Index", ESPV(12, 1), ESPV(12, 2), ESPV(12, 3), ESPV(12, 4), ESPV(12, 5), ESPV(12, 6)
        ''''Write #5, "Co. Government Take, %", ESPV(13, 1), ESPV(13, 2), ESPV(13, 3), ESPV(13, 4), ESPV(13, 5), ESPV(13, 6)
        ''''Write #5, "Co. State Take, %", StateTake!(1), StateTake!(2), StateTake!(3), StateTake!(4), StateTake!(5), StateTake!(6)
        ''''Write #5, "Gv. Cash Flow", ESPV(5, 1), ESPV(5, 2), ESPV(5, 3), ESPV(5, 4), ESPV(5, 5), ESPV(5, 6)
        ''''Write #5, "Gv. Payout, Years", ESPV(6, 1), ESPV(6, 2), ESPV(6, 3), ESPV(6, 4), ESPV(6, 5), ESPV(6, 6)
        ''''Write #5, "Gv. Risk Return Ratio", ESPV(7, 1), ESPV(7, 2), ESPV(7, 3), ESPV(7, 4), ESPV(7, 5), ESPV(7, 6)
        ''''Write #5, "Gv. Profitability Index", ESPV(8, 1), ESPV(8, 2), ESPV(8, 3), ESPV(8, 4), ESPV(8, 5), ESPV(8, 6)
        ''''Write #5, "   Company DCF Rate of Return:", RR, 0, 0, 0, 0, 0
        ''''Write #5, "Government DCF Rate of Return:", RRB, M3, Y3, 0, 0, 0  'M3 is month, Y3 is year of discount date

        ''''Close #5


        Dim oPg3 As CGiantRptPageB1
        oPg3 = g_oReport.NewEconomicSummaryPage
        With oPg3
            .SetPageHeader(11, YR, 0, LG, 6, "ECONOMIC SUMMARY", 12, FinalWin, FINALPARTIC, sCur)
            .SetDiscountRates(gn(4), gn(5), gn(6), gn(7), gn(8), gn(9))
            '6 Dec 2005 JWD (C0846) Replace abbreviations for Co. and Gv. with Company and Government, remove units from titles
            .SetProfileValues(1, "Company Income", ESPV(1, 1), ESPV(1, 2), ESPV(1, 3), ESPV(1, 4), ESPV(1, 5), ESPV(1, 6))
            .SetProfileValues(2, "Company Oper. Costs", ESPV(2, 1), ESPV(2, 2), ESPV(2, 3), ESPV(2, 4), ESPV(2, 5), ESPV(2, 6))
            .SetProfileValues(3, "Company Capital", ESPV(3, 1), ESPV(3, 2), ESPV(3, 3), ESPV(3, 4), ESPV(3, 5), ESPV(3, 6))
            .SetProfileValues(4, "Company Royalty/Tax", ESPV(4, 1), ESPV(4, 2), ESPV(4, 3), ESPV(4, 4), ESPV(4, 5), ESPV(4, 6))
            .SetProfileValues(5, "Company Cash Flow", ESPV(9, 1), ESPV(9, 2), ESPV(9, 3), ESPV(9, 4), ESPV(9, 5), ESPV(9, 6))
            .SetProfileValues(6, "Company Payout", ESPV(10, 1), ESPV(10, 2), ESPV(10, 3), ESPV(10, 4), ESPV(10, 5), ESPV(10, 6))
            .SetProfileValues(7, "Company Risk Return Ratio", ESPV(11, 1), ESPV(11, 2), ESPV(11, 3), ESPV(11, 4), ESPV(11, 5), ESPV(11, 6))
            .SetProfileValues(8, "Company Profitability Index", ESPV(12, 1), ESPV(12, 2), ESPV(12, 3), ESPV(12, 4), ESPV(12, 5), ESPV(12, 6))
            .SetProfileValues(9, "Company Government Take", ESPV(13, 1), ESPV(13, 2), ESPV(13, 3), ESPV(13, 4), ESPV(13, 5), ESPV(13, 6))
            .SetProfileValues(10, "Company State Take", StateTake(1), StateTake(2), StateTake(3), StateTake(4), StateTake(5), StateTake(6))
            ' 6 Dec 2005 JWD (C0846) Add new indicators and abbreviations changes as above
            .SetProfileValues(11, "3rd Party Cash Flow", ESPV(15, 1), ESPV(15, 2), ESPV(15, 3), ESPV(15, 4), ESPV(15, 5), ESPV(15, 6))
            .SetProfileValues(12, "3rd Party Payout", ESPV(16, 1), ESPV(16, 2), ESPV(16, 3), ESPV(16, 4), ESPV(16, 5), ESPV(16, 6))
            .SetProfileValues(13, "3rd Party Risk Return Ratio", ESPV(17, 1), ESPV(17, 2), ESPV(17, 3), ESPV(17, 4), ESPV(17, 5), ESPV(17, 6))
            .SetProfileValues(14, "3rd Party Profitability Index", ESPV(18, 1), ESPV(18, 2), ESPV(18, 3), ESPV(18, 4), ESPV(18, 5), ESPV(18, 6))
            .SetProfileValues(15, "NOC Cash Flow", ESPV(19, 1), ESPV(19, 2), ESPV(19, 3), ESPV(19, 4), ESPV(19, 5), ESPV(19, 6))
            .SetProfileValues(16, "NOC Payout", ESPV(20, 1), ESPV(20, 2), ESPV(20, 3), ESPV(20, 4), ESPV(20, 5), ESPV(20, 6))
            .SetProfileValues(17, "NOC Risk Return Ratio", ESPV(21, 1), ESPV(21, 2), ESPV(21, 3), ESPV(21, 4), ESPV(21, 5), ESPV(21, 6))
            .SetProfileValues(18, "NOC Profitability Index", ESPV(22, 1), ESPV(22, 2), ESPV(22, 3), ESPV(22, 4), ESPV(22, 5), ESPV(22, 6))
            .SetProfileValues(19, "Government Cash Flow", ESPV(5, 1), ESPV(5, 2), ESPV(5, 3), ESPV(5, 4), ESPV(5, 5), ESPV(5, 6))
            .SetProfileValues(20, "Government Payout", ESPV(6, 1), ESPV(6, 2), ESPV(6, 3), ESPV(6, 4), ESPV(6, 5), ESPV(6, 6))
            .SetProfileValues(21, "Government Risk Return Ratio", ESPV(7, 1), ESPV(7, 2), ESPV(7, 3), ESPV(7, 4), ESPV(7, 5), ESPV(7, 6))
            .SetProfileValues(22, "Government Profitability Index", ESPV(8, 1), ESPV(8, 2), ESPV(8, 3), ESPV(8, 4), ESPV(8, 5), ESPV(8, 6))
            .SetProfileValues(23, "Company Rate of Return", RR, 0, 0, 0, 0, 0)
            .SetProfileValues(24, "Government Rate of Return", RRB, M3, Y3, 0, 0, 0) 'M3 is month, Y3 is year of discount date
            'was:
            '.SetProfileValues 1, "Co. Income", ESPV(1, 1), ESPV(1, 2), ESPV(1, 3), ESPV(1, 4), ESPV(1, 5), ESPV(1, 6)
            '.SetProfileValues 2, "Co. Oper. Costs", ESPV(2, 1), ESPV(2, 2), ESPV(2, 3), ESPV(2, 4), ESPV(2, 5), ESPV(2, 6)
            '.SetProfileValues 3, "Co. Capital", ESPV(3, 1), ESPV(3, 2), ESPV(3, 3), ESPV(3, 4), ESPV(3, 5), ESPV(3, 6)
            '.SetProfileValues 4, "Co. Royalty/Tax", ESPV(4, 1), ESPV(4, 2), ESPV(4, 3), ESPV(4, 4), ESPV(4, 5), ESPV(4, 6)
            '.SetProfileValues 5, "Co. Cash Flow", ESPV(9, 1), ESPV(9, 2), ESPV(9, 3), ESPV(9, 4), ESPV(9, 5), ESPV(9, 6)
            '.SetProfileValues 6, "Co. Payout, Years", ESPV(10, 1), ESPV(10, 2), ESPV(10, 3), ESPV(10, 4), ESPV(10, 5), ESPV(10, 6)
            '.SetProfileValues 7, "Co. Risk Return Ratio", ESPV(11, 1), ESPV(11, 2), ESPV(11, 3), ESPV(11, 4), ESPV(11, 5), ESPV(11, 6)
            '.SetProfileValues 8, "Co. Profitability Index", ESPV(12, 1), ESPV(12, 2), ESPV(12, 3), ESPV(12, 4), ESPV(12, 5), ESPV(12, 6)
            '.SetProfileValues 9, "Co. Government Take, %", ESPV(13, 1), ESPV(13, 2), ESPV(13, 3), ESPV(13, 4), ESPV(13, 5), ESPV(13, 6)
            '.SetProfileValues 10, "Co. State Take, %", StateTake!(1), StateTake!(2), StateTake!(3), StateTake!(4), StateTake!(5), StateTake!(6)
            '.SetProfileValues 11, "Gv. Cash Flow", ESPV(5, 1), ESPV(5, 2), ESPV(5, 3), ESPV(5, 4), ESPV(5, 5), ESPV(5, 6)
            '.SetProfileValues 12, "Gv. Payout, Years", ESPV(6, 1), ESPV(6, 2), ESPV(6, 3), ESPV(6, 4), ESPV(6, 5), ESPV(6, 6)
            '.SetProfileValues 13, "Gv. Risk Return Ratio", ESPV(7, 1), ESPV(7, 2), ESPV(7, 3), ESPV(7, 4), ESPV(7, 5), ESPV(7, 6)
            '.SetProfileValues 14, "Gv. Profitability Index", ESPV(8, 1), ESPV(8, 2), ESPV(8, 3), ESPV(8, 4), ESPV(8, 5), ESPV(8, 6)
            '.SetProfileValues 15, "   Company DCF Rate of Return:", RR, 0, 0, 0, 0, 0
            '.SetProfileValues 16, "Government DCF Rate of Return:", RRB, M3, Y3, 0, 0, 0 'M3 is month, Y3 is year of discount date
            ' End (C0846)
        End With
38000:  'THIS WRITES VALUES TO SUMMARY FILE


        ' 22 Sep 2004 JWD (C0839) Remove explicit file write
        '    ' 15 Mar 2004 JWD Condition on being a Mainexec run
        '    If g_bIsMainexecRun Then
        '        If RNU = 1 Then
        '           Open sRptDir + RN$ + ".SUM" For Output As #2
        '        ElseIf RNU > 1 Then
        '           Open sRptDir + RN$ + ".SUM" For Append As #2
        '        End If
        '    End If
        ' End (C0839)

        'NOW CALCULATE ANY VARIABLES NEEDED
        ZN = LGI - ((mo - 1) / 12) '1/3/92 = ZN is WRONG if proj month <> 1  (CORRECTED BELOW)

        '11-14-91 MKD ------------------------------------------------------------
        'This section calculates a value for LFITemp.
        'LFITemp is basically the same as LFI.
        'However, in cases where the primary production forecast
        '  is relative to a date other than Production Start Month,
        '  the value for LFI is the time between the date that the
        '  primary production is relative to and the end of production.
        'If the primary stream is relative to Proj start, we want to
        '  show a producing life as the duration between the user entered
        '  Production start date (General Parameters) and the end of
        '  production.
        'In cases where LFITemp is less than LFI, the LFI number is technically
        '  correct but to conform with GIANT 4, we want to use LFITemp.

        '   Lg  = Project life integer
        '   Lgi = Project life actual
        '   Lfx = Production life integer
        '   Lfi = Production life actual

        productiondelay = ((Y1 - YR) * 12) + (M1 - mo) 'productiondelay is in months!
        LFITemp = LGI - (productiondelay / 12)

        '1/3/92 = ZN is wrong if proj month <> 1
        ZN = LGI 'LFITemp + (productiondelay / 12)
        If Left(RF(1), 6) = "CONSOL" Then
            LFITemp = LFI
            ZN = LIG
        End If
        TTL1 = 0 'equiv prod - current run
        TTL2 = 0 'revenue - current run
        TTL1T = 0 'equiv prod cum
        TTL2T = 0 'sum revenue cum
        OPS = 0
        OPST = 0

        ' 1 Oct 2004 JWD (C0841) Add next to override previous
        ' calculations of ZN (project life actual) and LFITemp
        ' (producing life actual). Theory is that currently all
        ' projects run through the end of the last project year
        ' (31 Dec). Thus the respective life values are simply
        ' LG - delay for the start from the beginning (1 Jan) of
        ' the first year of the project.
        ZN = LG - ((mo - 1) / 12) ' actual project life
        LFITemp = LG - (((Y1 - YR) * 12) + (M1 - 1)) / 12 ' actual producing life
        ' End (C0841)

        For i = 1 To LG
            ' GDP 20 Jan 2003
            ' Replaced the commented out lines with one call
            ' to ATotalRevenues
            'DUMK = A(i, 1) * A(i, 7)
            'DUMK = DUMK + (A(i, 2) * A(i, 8))
            'DUMK = DUMK + (A(i, 3) * A(i, 9))
            'DUMK = DUMK + (A(i, 4) * A(i, 10))
            'DUMK = DUMK * (1 - PARTRATE(i))
            DUMK = ATotalRevenues(i) * (1 - PARTRATE(i))
            If A(i, PPR + gc_nAPRICEOFFSET) <> 0 Then
                TTL1 = DUMK / A(i, PPR + gc_nAPRICEOFFSET) 'equiv prod this run
            Else
                TTL1 = 0
            End If

            TTL1 = TTL1 * WIN(i)
            ' GDP 20 Jan 2003
            ' Replace the hardcoded 6 with gc_nAPRICEOFFSET
            TTL2 = TTL1 * A(i, PPR + gc_nAPRICEOFFSET) 'equiv rev this run
            TTL1T = TTL1T + TTL1 'cum equiv prod
            TTL2T = TTL2T + TTL2 'cum equiv rev

            If OPXP <> 0 Then
                OPS = NGCF(i, OPXP)
            End If
            OPST = OPST + OPS
        Next i

        TTL1T = 0

        For i = 1 To LG

            TTL1 = EquivalencyVolumeProduction(i, True, True)
            TTL1 = TTL1 * (1 - PARTRATE(i))
            TTL1 = TTL1 * WIN(i)

            TTL1T = TTL1T + TTL1

        Next i


        If TTL1T = 0 Then
            PRCE = 0
        ElseIf TTL1T <> 0 Then
            PRCE = TTL2T / TTL1T
        End If
        For i = 1 To my3tt

            ' 17 May 2005 JWD (C0878) Change to compare range rather than individually
            ' Exclude BAL categories from summary
            If (my3(i, gc_nMY3_CAT) >= CPXCategoryCodeBAL) And (my3(i, gc_nMY3_CAT) <= CPXCategoryCodeBLn) Then
                GoTo 38128
            End If
            ' was:
            ''<<<<<< 21 Jun 2001 JWD (C0339)
            'If my3(i, 1) = CPXCategoryCodeBAL Then GoTo 38128   ' BAL
            'If my3(i, 1) = CPXCategoryCodeBL2 Then GoTo 38128   ' BL2
            'If my3(i, 1) = CPXCategoryCodeBL3 Then GoTo 38128   ' BL3
            ''~~~~~~ was:
            ''If my3(i, 1) = 18 Then                          'BAL
            ''  GoTo 38128
            ''ElseIf my3(i, 1) = 19 Or my3(i, 1) = 20 Then    'BL2 & BL3
            ''  GoTo 38128
            ''End If
            ''>>>>>> End (C0339)
            ' End (C0878)

            '<<<<<< 4 Aug 2001 JWD (C0363)
            If my3(i, 1) = CPXCategoryCode_AbandonmentAccrualEntry Then
                GoTo 38128
            End If
            '>>>>>> End (C0363)

            EX1 = 0
            DV1 = 0
            OTH1 = 0
            If my3(i, 1) < 9 Then
                EX1 = my3(i, 5) * GPRATE(i)
            ElseIf my3(i, 1) >= 9 And my3(i, 1) <= 14 Then
                DV1 = my3(i, 5) * GPRATE(i)
            ElseIf my3(i, 1) > 14 Then
                OTH1 = my3(i, 5) * GPRATE(i)
            End If
            If WINT <> 1 Then
                GoTo 38120
            End If
            EX1 = EX1 * (my3(i, 6) / 100)
            DV1 = DV1 * (my3(i, 6) / 100)
            OTH1 = OTH1 * (my3(i, 6) / 100)
38120:      Ex = Ex + EX1 : DV = DV + DV1 : OTH = OTH + OTH1
38128:  Next i

        If TTL1T = 0 Then
            GoTo 38190
        End If
        OXB = OPST / TTL1T
        EXB = Ex / TTL1T
        DVB = DV / TTL1T
        OTHB = OTH / TTL1T
        GoTo 38200
38190:  OXB = 0
        EXB = 0
        DVB = 0
        OTHB = 0
38200:

        '    Dim fEquivalencyVolProduction As Single
        '    Dim dEquivalencyFactor As Double
        '    Dim nYears As Integer
        '
        '    dEquivalencyFactor = gn(2)
        '
        '    For nYears = 1 To LG
        '        fEquivalencyVolProduction = fEquivalencyVolProduction + EquivalencyVolumeProduction(nYears, True, True)
        '    Next nYears

        ' 22 Sep 2004 JWD (C0839) Assign other indicators properties for all runs
        With oPg3.AsIDGiantRunSummaryA
            .RunTitle = RF(2)
            .TotalEquivalentReserves = TTL1T
            '.TotalEquivalentVolumetricReserves = fEquivalencyVolProduction
            .TotalOperatingExpenses = OPST
            .TotalExplorationCapital = Ex
            .TotalDevelopmentCapital = DV
            .TotalOtherCapital = OTH
            .AverageEquivalentPrice = PRCE
            .UnitAverageOperatingExpenses = OXB
            .UnitAverageExplorationCapital = EXB
            .UnitAverageDevelopmentCapital = DVB
            .UnitAverageOtherCapital = OTHB
            .ProductionLife = LFITemp
            .ProjectLife = ZN
            ' 6 Dec 2005 JWD (C0846) Add new rate of return indicators
            .NOCRateOfReturn = RRN
            .ThirdPartyRateOfReturn = RRT
            ' End (C0846)
        End With
        ' Was:
        '' 15 Mar 2004 JWD Condition on being a Mainexec run
        'If g_bIsMainexecRun Then
        '    'RF$(2) = Run Title from runline.  If OXY run, then
        '    '  dashserial contains the serial number of the run - append it
        '    '  to the run title.
        '    Dm$ = RF$(2)
        '    If Len(DashSerial$) > 0 Then
        '      Dm$ = Dm$ + " - " + DashSerial$
        '    End If
        '    Write #2, Dm$, sCur
        '    Write #2, TTL1T, OPST, Ex, DV, OTH, PRCE, OXB, EXB, DVB, OTHB, LFITemp, ZN, RRB, RR
        '    Write #2, ESPV(1, 1), ESPV(2, 1), ESPV(3, 1), ESPV(4, 1)
        '    Write #2, ESPV(9, 1), ESPV(10, 1), ESPV(11, 1), ESPV(12, 1), ESPV(13, 1)
        '    Write #2, ESPV(1, 5), ESPV(2, 5), ESPV(3, 5), ESPV(4, 5)
        '    Write #2, ESPV(9, 5), ESPV(10, 5), ESPV(11, 5), ESPV(12, 5), ESPV(13, 5)
        '    Close #2
        'End If
        ' End (C0839)

        '--------------------------------------------------------------------
38205:
        'HOLD VARIABLES FOR CONSOLIDATION
        If RF(7) = "" Then GoTo 39000 'do not consolidate (no weighting)
        EF = Val(RF(7)) / 100 'weighting for this run

        ' 10 Feb 2005 JWD (C0856) Replace literal for 2nd dimension with symbol
        Dim D8(gc_nMAXLIFE, gc_nACSIZED2) As Single 'dummy array (holds AC() for shifting)
        ' WAS:
        'ReDim D8(gc_nMAXLIFE, 11)   'dummy array (holds AC() for shifting)
        ' End (C0856)

        ReDim Preserve ADataCon(57, gc_nMAXLIFE) 'HOLD CONSOL DATA FOR oxy DATABASE
        'This accumulates consolidation data
        'L2() is temporary (L1() is permanent)

        L2(1) = mo 'project start month
        L2(2) = YR 'project start year
        L2(3) = M1 'production start month
        L2(4) = Y1 'production start year
        L2(13) = M2 'discovery month
        L2(14) = Y2 'discovery year
        L2(10) = M3 'discount month
        L2(11) = Y3 'discount year

        L2(5) = LG
        L2(6) = L2(2) + ((L2(1) - 1) / 12) '4/91 would be 1991.25  PROJ STRT
        L2(7) = L2(4) + ((L2(3) - 1) / 12) '4/91 would be 1991.25
        L2(12) = L2(11) + ((L2(10) - 1) / 12) '4/91 would be 1991.25
        L2(8) = L2(2) + L2(5) - 1 'ending reporting year
        L2(9) = L2(7) + LFI 'L2(7) + producing life actual

        ' Added GDP 16/4/99
        GetCurrencyConversion(sCur, g_sConsolCur, a_fCurrFactor)


        '2-16-92 MKD
        If L2(9) > L2(6) + LG Then
            L2(9) = L2(6) + LG 'if production recs are relative to proj start; this
        End If '  value is too high
        If L2(2) < L1(2) Then
            GoTo 38690 'if run starts before prev runs GOTO 38690

        End If
        '-------------------------------------------------
        'this run starts on or after previous runs
        'fill in the year elements of the OXY Database
        If UCase(Left(RF(3), 3)) = "OXY" Then
            For q = 1 To gc_nMAXLIFE
                ADataCon(2, q) = YR - 1 + q
            Next q
        End If



        For x = 1 To L2(5) 'L2(5) holds the LG value
            XI = L2(2) - L1(2) + x 'points to correct year for summing values
            If XI > gc_nMAXLIFE Then
                XI = gc_nMAXLIFE
            End If
            AC(XI, 1) = AC(XI, 1) + (PSCF(x, PS + 1) * EF * a_fCurrFactor(x))
            ANSW = NGCF(x, NEG + 1)
            If OPXP <> 0 Then
                ANSW = ANSW - NGCF(x, OPXP)
            End If
            If CPXP <> 0 Then
                ANSW = ANSW - NGCF(x, CPXP)
            End If

            AC(XI, 2) = AC(XI, 2) + (ANSW * EF * a_fCurrFactor(x))
            'deducts (opex) (col 3)
            If OPXP <> 0 Then
                AC(XI, 3) = AC(XI, 3) + (NGCF(x, OPXP) * EF * a_fCurrFactor(x))
            End If
            'deducts (capex)
            If CPXP <> 0 Then
                AC(XI, 4) = AC(XI, 4) + (NGCF(x, CPXP) * EF * a_fCurrFactor(x))
            End If


            ' 10 Jun 2008 JWD change to compute the 'energy' equivalent reserves instead of revenue equivalent
            ANSW = EquivalencyVolumeProduction(x, True, True)
            xdum = ANSW * (1 - PARTRATE(x)) * WIN(x) * EF

            '' GDP 20 Jan 2003
            '' Replaced the commented out revenue calc line with
            '' a call to ATotalRevenues
            ''ANSW = (A(x, 1) * A(x, 7)) + (A(x, 2) * A(x, 8)) + (A(x, 3) * A(x, 9)) + (A(x, 4) * A(x, 10))
            ANSW = ATotalRevenues(x)
            RunRev = (ANSW * (1 - PARTRATE(x)) * WIN(x) * EF * a_fCurrFactor(x))
            '        'equivalent production for year XI = total revenues (net
            '        '  of WIN and participation) divided by the primary
            '        '  product price (cumulative across runs)
            'If A(x, PPR + gc_nAPRICEOFFSET) <> 0 Then
            '  xdum = (RunRev / A(x, PPR + gc_nAPRICEOFFSET))
            'Else
            '  xdum = 0
            'End If

            AC(XI, 5) = AC(XI, 5) + xdum
            AC(XI, 6) = AC(XI, 6) + RunRev
            AC(XI, 7) = DFL(x)

            '10/13/97 - comment out the check for OXY
            'If Left$(RF$(3), 3) = "OXY" Then
            '11-16-92 For OXY PROJECT
            'Store govt cashflow and T,A,& U cashflow items in the
            '  AC() for consolidations
            '  store GVCF(x) in AC(XI,8)
            '  store NGCFT(x, iNGCFT + 1) in AC(XI,9)
            '  store NGCFA(x, iNGCFA + 1) in AC(XI,10)
            '  store NGCFU(x, iNGCFU + 1) in AC(XI,11)
            AC(XI, 8) = AC(XI, 8) + GVCF(x) 'govt cashflow
            If iNGCFT > 0 Then '"T" cashflow items
                AC(XI, 9) = AC(XI, 9) + NGCFT(x, iNGCFT + 1)
            End If
            If iNGCFA > 0 Then '"A" cashflow items
                AC(XI, 10) = AC(XI, 10) + NGCFA(x, iNGCFA + 1)
            End If
            If iNGCFU > 0 Then '"U" cashflow items
                AC(XI, 11) = AC(XI, 11) + NGCFU(x, iNGCFU + 1)
            End If

            '''          ' 10 Feb 2005 JWD (C0856) Add accumulation of net cash flows for 3d party, NOC, & Govt
            '''          AC(XI, gc_nAC_CMPNCF) = AC(XI, gc_nAC_CMPNCF) + (((GPCFV(x) - GNCFV(x)) * (1 - EffInts(x, gc_nEffInts_PAR)) + GREPAY(x)) * EffInts(x, gc_nEffInts_WIN) - GOPEX(x) * (1 - EffInts(x, gc_nEffInts_POX)) * EffInts(x, gc_nEffInts_WOX) + CNREIM(x) - C3DPTY(x) + GFINAN(x) * (1 - EffInts(x, gc_nEffInts_PFN)) * EffInts(x, gc_nEffInts_WFN)) * EF * a_fCurrFactor(x)
            '''          AC(XI, gc_nAC_3DPNCF) = AC(XI, gc_nAC_3DPNCF) + (((GPCFV(x) - GNCFV(x)) * (1 - EffInts(x, gc_nEffInts_PAR)) + GREPAY(x)) * (1 - EffInts(x, gc_nEffInts_WIN)) - GOPEX(x) * (1 - EffInts(x, gc_nEffInts_POX)) * (1 - EffInts(x, gc_nEffInts_WOX)) - CNREIM(x) + C3DPTY(x) + GFINAN(x) * (1 - EffInts(x, gc_nEffInts_PFN)) * (1 - EffInts(x, gc_nEffInts_WFN))) * EF * a_fCurrFactor(x)
            '''          AC(XI, gc_nAC_NOCNCF) = AC(XI, gc_nAC_NOCNCF) + ((GPCFV(x) - GNCFV(x)) * EffInts(x, gc_nEffInts_PAR) - GREPAY(x) - GOPEX(x) * EffInts(x, gc_nEffInts_POX) + GFINAN(x) * EffInts(x, gc_nEffInts_PFN)) * EF * a_fCurrFactor(x)
            '''          AC(XI, gc_nAC_GRSREV) = AC(XI, gc_nAC_GRSREV) + (GREV(x) - GPCFV(x) + GNCFV(x)) * EF * a_fCurrFactor(x)
            '''          ' End (C0856)

            AC(XI, gc_nAC_TPS) = AC(XI, gc_nAC_TPS) + gna_ACFX(x, gna_ACFX_TPS) * EF * a_fCurrFactor(x)
            AC(XI, gc_nAC_TNG) = AC(XI, gc_nAC_TNG) + gna_ACFX(x, gna_ACFX_TNG) * EF * a_fCurrFactor(x)
            AC(XI, gc_nAC_OPS) = AC(XI, gc_nAC_OPS) + gna_ACFX(x, gna_ACFX_OPS) * EF * a_fCurrFactor(x)
            AC(XI, gc_nAC_ONG) = AC(XI, gc_nAC_ONG) + gna_ACFX(x, gna_ACFX_ONG) * EF * a_fCurrFactor(x)
            AC(XI, gc_nAC_GPS) = AC(XI, gc_nAC_GPS) + gna_ACFX(x, gna_ACFX_GPS) * EF * a_fCurrFactor(x)
            AC(XI, gc_nAC_GNG) = AC(XI, gc_nAC_GNG) + gna_ACFX(x, gna_ACFX_GNG) * EF * a_fCurrFactor(x)
            AC(XI, gc_nAC_CPS) = AC(XI, gc_nAC_CPS) + gna_ACFX(x, gna_ACFX_CPS) * EF * a_fCurrFactor(x)
            AC(XI, gc_nAC_CNG) = AC(XI, gc_nAC_CNG) + gna_ACFX(x, gna_ACFX_CNG) * EF * a_fCurrFactor(x)

            pointerX = x
            pointerXI = XI
            ' GDP 08 Apr 2003
            ' Commented out - removal of OXY code
            'LoadAnnConsol
            'End If

        Next x
        GoTo 38860
        '----------------------------------------------------------------
38690:  'this run starts before previous runs or is the first run

        Dim D8Con(57, gc_nMAXLIFE) As Single

        If L1(2) = 10000 Then 'current run is the first run
            GoTo 38760
        End If
        For x = 1 To L1(5)
            XI = L1(2) - L2(2) + x
            If XI > gc_nMAXLIFE Then
                XI = gc_nMAXLIFE
            End If

            D8(XI, 1) = AC(x, 1)
            D8(XI, 2) = AC(x, 2)
            D8(XI, 3) = AC(x, 3)
            D8(XI, 4) = AC(x, 4)
            D8(XI, 5) = AC(x, 5)
            D8(XI, 6) = AC(x, 6)
            D8(XI, 7) = AC(x, 7)
            D8(XI, 8) = AC(x, 8)
            D8(XI, 9) = AC(x, 9)
            D8(XI, 10) = AC(x, 10)
            D8(XI, 11) = AC(x, 11)

            '''        ' 10 Feb 2005 JWD (C0856)
            '''        D8(XI, gc_nAC_CMPNCF) = AC(x, gc_nAC_CMPNCF)
            '''        D8(XI, gc_nAC_3DPNCF) = AC(x, gc_nAC_3DPNCF)
            '''        D8(XI, gc_nAC_NOCNCF) = AC(x, gc_nAC_NOCNCF)
            '''        D8(XI, gc_nAC_GRSREV) = AC(x, gc_nAC_GRSREV)
            '''        ' End (C0856)

            D8(XI, gc_nAC_TPS) = AC(x, gc_nAC_TPS)
            D8(XI, gc_nAC_TNG) = AC(x, gc_nAC_TNG)
            D8(XI, gc_nAC_OPS) = AC(x, gc_nAC_OPS)
            D8(XI, gc_nAC_ONG) = AC(x, gc_nAC_ONG)
            D8(XI, gc_nAC_GPS) = AC(x, gc_nAC_GPS)
            D8(XI, gc_nAC_GNG) = AC(x, gc_nAC_GNG)
            D8(XI, gc_nAC_CPS) = AC(x, gc_nAC_CPS)
            D8(XI, gc_nAC_CNG) = AC(x, gc_nAC_CNG)

            For q = 1 To 57
                D8Con(q, XI) = ADataCon(q, x)
            Next q
        Next x

38760:
        ' 10 Feb 2005 JWD (C0856) Change to use symbol for size
        ' 5 Jan 2006 JWD (C0891) Change to "un-comment" redim statement following. Was commented out for unknown reason.
        ReDim AC(gc_nMAXLIFE, gc_nACSIZED2)
        ' Was:
        'ReDim AC(gc_nMAXLIFE, 11)
        ' End (C0856)
        ReDim ADataCon(57, gc_nMAXLIFE)
        If L1(2) <> 10000 Then 'don't bother doing this for the first run
            For ww = 1 To gc_nMAXLIFE 'this is not the first run!
                For q = 1 To 57
                    ADataCon(q, ww) = D8Con(q, ww)
                Next q
            Next ww
        End If
        'fill in the year elements of the OXY Database
        If UCase(Left(RF(3), 3)) = "OXY" Then
            For q = 1 To gc_nMAXLIFE
                ADataCon(2, q) = YR - 1 + q
            Next q
        End If

        For x = 1 To L2(5) '1 to LG
            AC(x, 1) = AC(x, 1) + (PSCF(x, PS + 1) * EF * a_fCurrFactor(x))
            ANSW = NGCF(x, NEG + 1)
            If OPXP <> 0 Then
                ANSW = ANSW - NGCF(x, OPXP)
            End If
            If CPXP <> 0 Then
                ANSW = ANSW - NGCF(x, CPXP)
            End If
            AC(x, 2) = AC(x, 2) + (ANSW * EF * a_fCurrFactor(x))
            If OPXP <> 0 Then
                AC(x, 3) = AC(x, 3) + (NGCF(x, OPXP) * EF * a_fCurrFactor(x))
            End If
            If CPXP <> 0 Then
                AC(x, 4) = AC(x, 4) + (NGCF(x, CPXP) * EF * a_fCurrFactor(x))
            End If

            ' 10 Jun 2008 JWD change to compute the 'energy' equivalent reserves instead of revenue equivalent
            ANSW = EquivalencyVolumeProduction(x, True, True)
            xdum = ANSW * (1 - PARTRATE(x)) * WIN(x) * EF

            '' 11 Feb 2005 JWD (C0857) Correct to use the total revenues from all streams
            ANSW = ATotalRevenues(x)
            '' Was:
            ''ANSW = (A(x, 1) * A(x, 7)) + (A(x, 2) * A(x, 8)) + (A(x, 3) * A(x, 9)) + (A(x, 4) * A(x, 10))
            '' End (C0857)
            '
            RunRev = (ANSW * (1 - PARTRATE(x)) * WIN(x) * EF * a_fCurrFactor(x))
            '
            '' 11 Feb 2005 JWD (C0857) Correct the price index offset to use the symbol
            'If A(x, PPR + gc_nAPRICEOFFSET) <> 0 Then
            '    AC(x, gc_nAC_EQVPRD) = AC(x, gc_nAC_EQVPRD) + (RunRev / A(x, PPR + gc_nAPRICEOFFSET))
            'End If
            '' Was:
            ''If A(x, PPR + 6) <> 0 Then      'prim prod price
            ''  AC(x, 5) = AC(x, 5) + (RunRev / A(x, PPR + 6))      'equi prod
            ''End If
            '' End (C0857)

            AC(x, gc_nAC_EQVPRD) = AC(x, gc_nAC_EQVPRD) + xdum
            AC(x, 6) = AC(x, 6) + RunRev
            AC(x, 7) = DFL(x)
            '11-16-92 For OXY PROJECT
            'Store govt cashflow and T,A,& U cashflow items in the
            '  AC() for consolidations
            '  store GVCF(x) in AC(XI,8)
            '  store NGCFT(x, iNGCFT + 1) in AC(XI,9)
            '  store NGCFA(x, iNGCFA + 1) in AC(XI,10)
            '  store NGCFU(x, iNGCFU + 1) in AC(XI,11)
            AC(x, 8) = AC(XI, 8) + GVCF(x) 'govt cashflow
            If iNGCFT > 0 Then '"T" cashflow items
                AC(x, 9) = AC(x, 9) + NGCFT(x, iNGCFT + 1)
            End If
            If iNGCFA > 0 Then '"A" cashflow items
                AC(x, 10) = AC(x, 10) + NGCFA(x, iNGCFA + 1)
            End If
            If iNGCFU > 0 Then '"U" cashflow items
                AC(x, 11) = AC(x, 11) + NGCFU(x, iNGCFU + 1)
            End If

            '''        ' 10 Feb 2005 JWD (C0856) Add accumulation of net cash flows for 3d party, NOC, & Govt
            '''        AC(x, gc_nAC_CMPNCF) = (((GPCFV(x) - GNCFV(x)) * (1 - EffInts(x, gc_nEffInts_PAR)) + GREPAY(x)) * EffInts(x, gc_nEffInts_WIN) - GOPEX(x) * (1 - EffInts(x, gc_nEffInts_POX)) * EffInts(x, gc_nEffInts_WOX) + CNREIM(x) - C3DPTY(x) + GFINAN(x) * (1 - EffInts(x, gc_nEffInts_PFN)) * EffInts(x, gc_nEffInts_WFN)) * EF * a_fCurrFactor(x)
            '''        AC(x, gc_nAC_3DPNCF) = (((GPCFV(x) - GNCFV(x)) * (1 - EffInts(x, gc_nEffInts_PAR)) + GREPAY(x)) * (1 - EffInts(x, gc_nEffInts_WIN)) - GOPEX(x) * (1 - EffInts(x, gc_nEffInts_POX)) * (1 - EffInts(x, gc_nEffInts_WOX)) - CNREIM(x) + C3DPTY(x) + GFINAN(x) * (1 - EffInts(x, gc_nEffInts_PFN)) * (1 - EffInts(x, gc_nEffInts_WFN))) * EF * a_fCurrFactor(x)
            '''        AC(x, gc_nAC_NOCNCF) = ((GPCFV(x) - GNCFV(x)) * EffInts(x, gc_nEffInts_PAR) - GREPAY(x) - GOPEX(x) * EffInts(x, gc_nEffInts_POX) + GFINAN(x) * EffInts(x, gc_nEffInts_PFN)) * EF * a_fCurrFactor(x)
            '''        AC(x, gc_nAC_GRSREV) = (GREV(x) - GPCFV(x) + GNCFV(x)) * EF * a_fCurrFactor(x)
            '''        ' End (C0856)

            AC(x, gc_nAC_TPS) = AC(x, gc_nAC_TPS) + gna_ACFX(x, gna_ACFX_TPS) * EF * a_fCurrFactor(x)
            AC(x, gc_nAC_TNG) = AC(x, gc_nAC_TNG) + gna_ACFX(x, gna_ACFX_TNG) * EF * a_fCurrFactor(x)
            AC(x, gc_nAC_OPS) = AC(x, gc_nAC_OPS) + gna_ACFX(x, gna_ACFX_OPS) * EF * a_fCurrFactor(x)
            AC(x, gc_nAC_ONG) = AC(x, gc_nAC_ONG) + gna_ACFX(x, gna_ACFX_ONG) * EF * a_fCurrFactor(x)
            AC(x, gc_nAC_GPS) = AC(x, gc_nAC_GPS) + gna_ACFX(x, gna_ACFX_GPS) * EF * a_fCurrFactor(x)
            AC(x, gc_nAC_GNG) = AC(x, gc_nAC_GNG) + gna_ACFX(x, gna_ACFX_GNG) * EF * a_fCurrFactor(x)
            AC(x, gc_nAC_CPS) = AC(x, gc_nAC_CPS) + gna_ACFX(x, gna_ACFX_CPS) * EF * a_fCurrFactor(x)
            AC(x, gc_nAC_CNG) = AC(x, gc_nAC_CNG) + gna_ACFX(x, gna_ACFX_CNG) * EF * a_fCurrFactor(x)

            pointerX = x
            pointerXI = x
            'GDP 08 Apr 2003
            'Commented out - removal of OXY code
            'LoadAnnConsol
        Next x

        If L1(2) = 10000 Then
            GoTo 38850
        End If

        For x = 1 To gc_nMAXLIFE
            AC(x, 1) = AC(x, 1) + D8(x, 1)
            AC(x, 2) = AC(x, 2) + D8(x, 2)
            AC(x, 3) = AC(x, 3) + D8(x, 3)
            AC(x, 4) = AC(x, 4) + D8(x, 4)
            AC(x, 5) = AC(x, 5) + D8(x, 5)
            AC(x, 6) = AC(x, 6) + D8(x, 6)
            If D8(x, 7) <> 0 Then
                AC(x, 7) = D8(x, 7)
            End If
            AC(x, 8) = AC(x, 8) + D8(x, 8)
            AC(x, 9) = AC(x, 9) + D8(x, 9)
            AC(x, 10) = AC(x, 10) + D8(x, 10)
            AC(x, 11) = AC(x, 11) + D8(x, 11)

            '''        ' 10 Feb 2005 JWD (C0856) Add new positions
            '''        AC(x, gc_nAC_CMPNCF) = AC(x, gc_nAC_CMPNCF) + D8(x, gc_nAC_CMPNCF)
            '''        AC(x, gc_nAC_3DPNCF) = AC(x, gc_nAC_3DPNCF) + D8(x, gc_nAC_3DPNCF)
            '''        AC(x, gc_nAC_NOCNCF) = AC(x, gc_nAC_NOCNCF) + D8(x, gc_nAC_NOCNCF)
            '''        AC(x, gc_nAC_GRSREV) = AC(x, gc_nAC_GRSREV) + D8(x, gc_nAC_GRSREV)
            '''        ' End (C0856)

            AC(x, gc_nAC_TPS) = AC(x, gc_nAC_TPS) + D8(x, gc_nAC_TPS)
            AC(x, gc_nAC_TNG) = AC(x, gc_nAC_TNG) + D8(x, gc_nAC_TNG)
            AC(x, gc_nAC_OPS) = AC(x, gc_nAC_OPS) + D8(x, gc_nAC_OPS)
            AC(x, gc_nAC_ONG) = AC(x, gc_nAC_ONG) + D8(x, gc_nAC_ONG)
            AC(x, gc_nAC_GPS) = AC(x, gc_nAC_GPS) + D8(x, gc_nAC_GPS)
            AC(x, gc_nAC_GNG) = AC(x, gc_nAC_GNG) + D8(x, gc_nAC_GNG)
            AC(x, gc_nAC_CPS) = AC(x, gc_nAC_CPS) + D8(x, gc_nAC_CPS)
            AC(x, gc_nAC_CNG) = AC(x, gc_nAC_CNG) + D8(x, gc_nAC_CNG)

        Next x

38850:
38860:
        'early and later runs rejoin here
        ' 08 Apr 2003
        ' Commented out - Remove OXY code
        ' LoadPrjConsol     'consolidate OXY database values for PRJ, DOC & NOT files

        If L2(6) < L1(6) Then L1(6) = L2(6)
        If L2(7) < L1(7) Then L1(7) = L2(7)
        If L2(12) < L1(12) Then L1(12) = L2(12)

        If L2(8) > L1(8) Then L1(8) = L2(8)
        If L2(9) > L1(9) Then L1(9) = L2(9)
        L1(2) = Int(L1(6))
        L1(4) = Int(L1(7))

        L1(11) = Int(L1(12))
        L1(5) = Int(L1(8)) - Int(L1(6)) + 1
        L1(1) = ((L1(6) - L1(2)) * 12) + 1
        L1(3) = ((L1(7) - L1(4)) * 12) + 1
        L1(10) = ((L1(12) - L1(11)) * 12) + 1
        L1ddate = ((L1(13) - 1) / 12) + L1(14)
        L2ddate = ((L2(13) - 1) / 12) + L2(14)
        If L2ddate < L1ddate Then
            L1(13) = L2(13)
            L1(14) = L2(14)
        End If

        'CONCATENATE CAPITAL EXPENDITURES
        If my3tt = 0 Then 'no capital
            GoTo 38999
        End If
        NewDim = my3tt + CCT
        '  REDIM PRESERVE CC(NewDim, 4)
        For x = 1 To my3tt
            loopit = CCT + AddCCT 'CCT = total number of CC() so far
            If loopit = 0 Then
                AddCCT = 1 'AddCCT = # of new rows added to CC()
                CC(1, 1) = my3(x, 1)
                CC(1, 2) = my3(x, 2)
                CC(1, 3) = my3(x, 3)
                CC(1, 4) = my3(x, 5) * WINC(x) * EF * a_fCurrFactor(my3(x, 3) - YR + 1) * GPRATE(x)

                '''          ' 10 Feb 2005 JWD (C0856) Add capture of 3d party & NOC capital amounts
                '''          CC(1, gc_nCC_CMP) = (my3(x, gc_nMY3_AMT) * (1 - EffIntsX(x, gc_nEffIntsX_PAR)) * EffIntsX(x, gc_nEffIntsX_WIN)) * EF * a_fCurrFactor(my3(x, gc_nMY3_XYR) - YR + 1)
                '''          CC(1, gc_nCC_3DP) = (my3(x, gc_nMY3_AMT) * (1 - EffIntsX(x, gc_nEffIntsX_PAR)) * (1 - EffIntsX(x, gc_nEffIntsX_WIN))) * EF * a_fCurrFactor(my3(x, gc_nMY3_XYR) - YR + 1)
                '''          CC(1, gc_nCC_NOC) = (my3(x, gc_nMY3_AMT) * EffIntsX(x, gc_nEffIntsX_PAR)) * EF * a_fCurrFactor(my3(x, gc_nMY3_XYR) - YR + 1)
                '''          ' End (C0856)

                CC(1, gc_nCC_TCX) = my3Ex(x, 0) * EF * a_fCurrFactor(my3(x, 3) - YR + 1)
                CC(1, gc_nCC_OCX) = my3Ex(x, 1) * EF * a_fCurrFactor(my3(x, 3) - YR + 1)
                CC(1, gc_nCC_GCX) = my3Ex(x, 2) * EF * a_fCurrFactor(my3(x, 3) - YR + 1)
                CC(1, gc_nCC_CCX) = my3Ex(x, 3) * EF * a_fCurrFactor(my3(x, 3) - YR + 1)

            Else
                matchem = "N"
                For y = 1 To loopit ' loop through all CC() from previous runs
                    ' and all MY3() so far in this run
                    If matchem = "Y" Then
                        GoTo nexty
                    End If
                    If my3(x, 1) = CC(y, 1) And my3(x, 2) = CC(y, 2) And my3(x, 3) = CC(y, 3) Then
                        ' found perfect match, thus sum this expenditure into matching CC()

                        CC(y, 4) = CC(y, 4) + (my3(x, 5) * WINC(x) * EF * a_fCurrFactor(my3(x, 3) - YR + 1) * GPRATE(x))

                        '''              ' 10 Feb 2005 JWD (C0856) Add capture of 3d party & NOC capital amounts
                        '''              CC(y, gc_nCC_CMP) = CC(y, gc_nCC_CMP) + (my3(x, gc_nMY3_AMT) * (1 - EffIntsX(x, gc_nEffIntsX_PAR)) * EffIntsX(x, gc_nEffIntsX_WIN)) * EF * a_fCurrFactor(my3(x, gc_nMY3_XYR) - YR + 1)
                        '''              CC(y, gc_nCC_3DP) = CC(y, gc_nCC_3DP) + (my3(x, gc_nMY3_AMT) * (1 - EffIntsX(x, gc_nEffIntsX_PAR)) * (1 - EffIntsX(x, gc_nEffIntsX_WIN))) * EF * a_fCurrFactor(my3(x, gc_nMY3_XYR) - YR + 1)
                        '''              CC(y, gc_nCC_NOC) = CC(y, gc_nCC_NOC) + (my3(x, gc_nMY3_AMT) * EffIntsX(x, gc_nEffIntsX_PAR)) * EF * a_fCurrFactor(my3(x, gc_nMY3_XYR) - YR + 1)
                        '''              ' End (C0856)

                        CC(y, gc_nCC_TCX) = CC(y, gc_nCC_TCX) + my3Ex(x, 0) * EF * a_fCurrFactor(my3(x, 3) - YR + 1)
                        CC(y, gc_nCC_OCX) = CC(y, gc_nCC_OCX) + my3Ex(x, 1) * EF * a_fCurrFactor(my3(x, 3) - YR + 1)
                        CC(y, gc_nCC_GCX) = CC(y, gc_nCC_GCX) + my3Ex(x, 2) * EF * a_fCurrFactor(my3(x, 3) - YR + 1)
                        CC(y, gc_nCC_CCX) = CC(y, gc_nCC_CCX) + my3Ex(x, 3) * EF * a_fCurrFactor(my3(x, 3) - YR + 1)

                        matchem = "Y"
                    End If
nexty:          Next y
                If matchem = "N" Then
                    ' no match found, increment line counter
                    AddCCT = AddCCT + 1
                    NewDim = AddCCT + CCT
                    CC(AddCCT + CCT, 1) = my3(x, 1)
                    CC(AddCCT + CCT, 2) = my3(x, 2)
                    CC(AddCCT + CCT, 3) = my3(x, 3)
                    CC(AddCCT + CCT, 4) = my3(x, 5) * WINC(x) * EF * a_fCurrFactor(my3(x, 3) - YR + 1) * GPRATE(x)

                    '''            ' 10 Feb 2005 JWD (C0856) Add capture of 3d party & NOC capital amounts
                    '''            CC(AddCCT + CCT, gc_nCC_CMP) = (my3(x, gc_nMY3_AMT) * (1 - EffIntsX(x, gc_nEffIntsX_PAR)) * EffIntsX(x, gc_nEffIntsX_WIN)) * EF * a_fCurrFactor(my3(x, gc_nMY3_XYR) - YR + 1)
                    '''            CC(AddCCT + CCT, gc_nCC_3DP) = (my3(x, gc_nMY3_AMT) * (1 - EffIntsX(x, gc_nEffIntsX_PAR)) * (1 - EffIntsX(x, gc_nEffIntsX_WIN))) * EF * a_fCurrFactor(my3(x, gc_nMY3_XYR) - YR + 1)
                    '''            CC(AddCCT + CCT, gc_nCC_NOC) = (my3(x, gc_nMY3_AMT) * EffIntsX(x, gc_nEffIntsX_PAR)) * EF * a_fCurrFactor(my3(x, gc_nMY3_XYR) - YR + 1)
                    '''            ' End (C0856)

                    CC(AddCCT + CCT, gc_nCC_TCX) = my3Ex(x, 0) * EF * a_fCurrFactor(my3(x, 3) - YR + 1)
                    CC(AddCCT + CCT, gc_nCC_OCX) = my3Ex(x, 1) * EF * a_fCurrFactor(my3(x, 3) - YR + 1)
                    CC(AddCCT + CCT, gc_nCC_GCX) = my3Ex(x, 2) * EF * a_fCurrFactor(my3(x, 3) - YR + 1)
                    CC(AddCCT + CCT, gc_nCC_CCX) = my3Ex(x, 3) * EF * a_fCurrFactor(my3(x, 3) - YR + 1)

                End If
            End If
        Next x
        CCT = CCT + AddCCT

38999:
        ' 15 Mar 2004 JWD Condition on being a Mainexec run
        If g_bIsMainexecRun Then
            WriteDtreeData()
        End If

39000:  'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        System.Array.Clear(D8, 0, D8.Length)

        ' 15 Mar 2004 JWD Condition on being a Mainexec run
        If g_bIsMainexecRun Then

            ' Write the RING.FNC file here if flag is in run file
            ' GDP 10/9/99
            If Right(Trim(RF(2)), 1) = "P" Then
                WriteRingfenceVar()
            End If

            ' Write the pre tax / net of participation consolidation file
            ' GDP 18/08/99
            'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
            If Left(RF(1), 6) <> "CONSOL" And Len(Dir(TempDir & g_sDataFileNoExt & "." & c_sRVSEXT)) = 0 Then
                WriteRVS()
            End If
            'Erase Inflate ' can now erase inflate() after the RVS file has been written
        End If

        Exit Sub

        '--------------------------------------------------------------------
40000:  ' THIS SUBROUTINE READS ACCUMULATED TOTALS AND SETS UP CONSOLIDATION
        '//////////////////////////////////////////////////////////
        'if there were no runs with consolidation weights, then
        '  L1(2) will have the value 10000. If so, we cannot
        '  consolidate and we exit back to GNTMAIN.EXE
        If L1(2) = 10000 Then
            Error (250)
        End If
        '////////////////////////////////////////////////////////////

        ReDim GM(1, 2)
        mo = L1(1) : YR = L1(2)
        M1 = L1(3) : Y1 = L1(4)
        M2 = L1(13) : Y2 = L1(14) 'discovery date   2-17-92 MKD
        M3 = L1(10) : Y3 = L1(11)

        SAVELG = LG

        LG = L1(8) - L1(2) + 1

        If LG <> SAVELG Then 'proj life is different for consol that the last run!
            '<<<<<< 29 Mar 2002 JWD (C0527)
            ReDim CF(LG)
            ReDim PSCF(LG, maximum_count_positive_cashflows)
            ReDim NGCF(LG, maximum_count_negative_cashflows)
            ReDim ERT(LG)
            ReDim D5(LG)
            ReDim vpr(LG)
            ReDim NGCFT(LG, maximum_count_negative_cashflows)
            ReDim NGCFA(LG, maximum_count_negative_cashflows)
            ReDim NGCFU(LG, maximum_count_negative_cashflows)
            ReDim PAY(LG)
            ReDim ATCF(LG)
            ReDim DUM(LG, maximum_count_positive_cashflows + maximum_count_negative_cashflows + 1)
            '~~~~~~ was:
            'ReDim CF(LG), PSCF(LG, 12), NGCF(LG, 12), ERT(LG), D5(LG), vpr(LG)
            'ReDim NGCFT(LG, 12), NGCFA(LG, 12), NGCFU(LG, 12)
            'ReDim PAY(LG), ATCF(LG), DUM(LG, 25)
            '>>>>>> End (C0527)
            ''''REDIM DFL(LG)
            ReDim COCF(LG)
            ReDim COCCF(LG)
            ReDim CODCF(LG)
            ReDim COCDCF(LG)
            ReDim GVCF(LG)
            ReDim GVCCF(LG)
            ReDim GVDCF(LG)
            ReDim GVCDCF(LG)
            ReDim CP(LG)
            ReDim GOVT(LG)

            ' 15 Dec 2005 JWD (C0846) Allocate new 3rd party and NOC cash flow
            ReDim TDPCF(LG)
            ReDim TDPCCF(LG)
            ReDim TDPDCF(LG)
            ReDim TDPCDCF(LG)
            ReDim NOCCF(LG)
            ReDim NOCCCF(LG)
            ReDim NOCDCF(LG)
            ReDim NOCCDCF(LG)
            ReDim TATCF(LG)
            ReDim NATCF(LG)
            ReDim GREV(LG)
            ReDim GOPEX(LG)
            ReDim GPCFV(LG)
            ReDim GNCFV(LG)
            ReDim GREPAY(LG)
            ReDim GFINAN(LG)
            ReDim CNREIM(LG)
            ReDim C3DPTY(LG)
            '''            ReDim EffInts(LG, gc_nEffIntsSIZED2)
            ' End (C0846)

            ReDim gna_ACFX(LG, 26)

        End If

        LFI = L1(9) - L1(7)
        LIG = L1(9) - L1(6)
        LGI = LIG + ((mo - 1) / 12)
        PS = 1
        NEG = 3

        '11-16-92 OXY items ("T","A" & "U" cashflow items)
        iNGCFT = 1
        iNGCFA = 1
        iNGCFU = 1

        my3tt = CCT
        'WINT = 0   '*** watch out for this
        OPXP = 2
        CPXP = 3
        PPR = 1
        TDT = 2
        TLT = 2
        ReDim TD(2, 18)
        ReDim TL(2, 3)
        ' GDP 24 Feb 2003
        ' Redimension A array to hold extra volume data
        ReDim A(LG, gc_nASIZE)
        TD(1, 1) = "INC"
        TD(1, 4) = "+"
        TD(2, 1) = "TAX" : TD(2, 4) = "-"
        TL(1, 1) = "INC" : TL(1, 2) = "INCOME"
        TL(2, 1) = "TAX" : TL(2, 2) = "ROY/TAX"

        For x = 1 To LG
            PSCF(x, 1) = AC(x, 1)
            NGCF(x, 1) = AC(x, 2)
            NGCF(x, 2) = AC(x, 3)
            NGCF(x, 3) = AC(x, 4)
            ' GDP 20 Jan 2003
            ' Replaced numbers with constants when referencing the A array
            A(x, gc_nAOIL) = AC(x, 5) 'a(x,1) = OIL    ac(x,5) = equiv prod
            A(x, gc_nAGAS) = 0
            A(x, gc_nAOV1) = 0
            If AC(x, 5) <> 0 Then
                A(x, gc_nAOPC) = AC(x, 6) / AC(x, 5)
            End If
            '11-16-92 OXY Project. Modified AC() to hold GVCF()
            '  and "T","A", & "U" cashflow items
            GVCF(x) = AC(x, 8)
            'cumulative govt cashflow
            If x = 1 Then
                GVCCF(x) = GVCF(1)
            Else
                GVCCF(x) = GVCCF(x - 1) + GVCF(x)
            End If
            NGCFT(x, 1) = AC(x, 9)
            NGCFT(x, 2) = AC(x, 9)
            NGCFA(x, 1) = AC(x, 10)
            NGCFA(x, 2) = AC(x, 10)
            NGCFU(x, 1) = AC(x, 11)
            NGCFU(x, 2) = AC(x, 11)

            '''          ' 10 Feb 2005 JWD (C0856) Compute the total net cash flow and the respective effective interests
            '''          GPCFV(x) = AC(x, gc_nAC_CMPNCF) + AC(x, gc_nAC_3DPNCF) + AC(x, gc_nAC_NOCNCF)
            '''          ' 5 Jun 2006 JWD (C0898) Change to only compute EffInts() values when company + 3d party ncf is sufficiently large to be valid. Change to range of values considered zero instead of exact match to zero. Compensates for round-off errors that lead to divide by zero.
            '''          If Abs(AC(x, gc_nAC_CMPNCF) + AC(x, gc_nAC_3DPNCF)) > 0.0000009 Then
            '''            EffInts(x, gc_nEffInts_WIN) = AC(x, gc_nAC_CMPNCF) / (AC(x, gc_nAC_3DPNCF) + AC(x, gc_nAC_CMPNCF))
            '''          End If
            '''          If Abs(AC(x, gc_nAC_CMPNCF) + AC(x, gc_nAC_3DPNCF) + AC(x, gc_nAC_NOCNCF)) > 0.0000009 Then
            '''            EffInts(x, gc_nEffInts_PAR) = AC(x, gc_nAC_NOCNCF) / (AC(x, gc_nAC_3DPNCF) + AC(x, gc_nAC_CMPNCF) + AC(x, gc_nAC_NOCNCF))
            '''          End If
            '''          ' was:
            '''          'If AC(x, gc_nAC_CMPNCF) <> 0 Then
            '''          '  EffInts(x, gc_nEffInts_WIN) = AC(x, gc_nAC_CMPNCF) / (AC(x, gc_nAC_3DPNCF) + AC(x, gc_nAC_CMPNCF))
            '''          '  EffInts(x, gc_nEffInts_PAR) = AC(x, gc_nAC_NOCNCF) / (AC(x, gc_nAC_3DPNCF) + AC(x, gc_nAC_CMPNCF) + AC(x, gc_nAC_NOCNCF))
            '''          'End If
            '''          ' End (C0898)
            '''
            '''          GREV(x) = AC(x, gc_nAC_GRSREV) + GPCFV(x)
            '''          ' End (C0856)

            gna_ACFX(x, gna_ACFX_TPS) = AC(x, gc_nAC_TPS)
            gna_ACFX(x, gna_ACFX_TNG) = AC(x, gc_nAC_TNG)
            gna_ACFX(x, gna_ACFX_OPS) = AC(x, gc_nAC_OPS)
            gna_ACFX(x, gna_ACFX_ONG) = AC(x, gc_nAC_ONG)
            gna_ACFX(x, gna_ACFX_GPS) = AC(x, gc_nAC_GPS)
            gna_ACFX(x, gna_ACFX_GNG) = AC(x, gc_nAC_GNG)
            gna_ACFX(x, gna_ACFX_CPS) = AC(x, gc_nAC_CPS)
            gna_ACFX(x, gna_ACFX_CNG) = AC(x, gc_nAC_CNG)

        Next x

        '''        ' 7 Jan 2005 JWD (C0846) Add allocation of MY3 related arrays
        '''        ReDim GMY3(CCT)
        '''        ReDim EffIntsX(CCT, gc_nEffIntsXSIZED2)
        '''        ' End (C0846)

        If CCT = 0 Then
            GoTo 40160
        End If
        ReDim my3(CCT, 7)
        ReDim WINC(CCT)
        ReDim GPRATE(CCT)

        ReDim my3Ex(CCT, 3)

        For x = 1 To CCT
            my3(x, 1) = CC(x, 1)
            my3(x, 2) = CC(x, 2)
            my3(x, 3) = CC(x, 3)
            my3(x, 4) = 0
            my3(x, 5) = CC(x, 4)
            my3(x, 6) = 100
            WINC(x) = 1
            GPRATE(x) = 1

            '''          ' 10 Feb 2005 JWD (C0856)
            '''          GMY3(x) = CC(x, gc_nCC_AMT) + CC(x, gc_nCC_3DP) + CC(x, gc_nCC_NOC)
            '''          If CC(x, gc_nCC_AMT) <> 0 Then
            '''             ' 8 Jun 2006 JWD (C0900) Change to check divisor for zero + near-zero values before dividing
            '''             If Abs(CC(x, gc_nCC_CMP) + CC(x, gc_nCC_3DP)) > 0.0000009 Then
            '''               EffIntsX(x, gc_nEffIntsX_WIN) = CC(x, gc_nCC_CMP) / (CC(x, gc_nCC_CMP) + CC(x, gc_nCC_3DP))
            '''             End If
            '''             If Abs(CC(x, gc_nCC_CMP) + CC(x, gc_nCC_3DP) + CC(x, gc_nCC_NOC)) > 0.0000009 Then
            '''               EffIntsX(x, gc_nEffIntsX_PAR) = CC(x, gc_nCC_NOC) / (CC(x, gc_nCC_CMP) + CC(x, gc_nCC_3DP) + CC(x, gc_nCC_NOC))
            '''             End If
            '''             ' was:
            '''             '  EffIntsX(x, gc_nEffIntsX_WIN) = CC(x, gc_nCC_CMP) / (CC(x, gc_nCC_CMP) + CC(x, gc_nCC_3DP))
            '''             '  EffIntsX(x, gc_nEffIntsX_PAR) = CC(x, gc_nCC_NOC) / (CC(x, gc_nCC_CMP) + CC(x, gc_nCC_3DP) + CC(x, gc_nCC_NOC))
            '''             ' End (C0900)
            '''          End If
            '''          ' End (C0856)

            my3Ex(x, 0) = CC(x, gc_nCC_TCX)
            my3Ex(x, 1) = CC(x, gc_nCC_OCX)
            my3Ex(x, 2) = CC(x, gc_nCC_GCX)
            my3Ex(x, 3) = CC(x, gc_nCC_CCX)

        Next x
        '~~~~ 23 Aug 1996 JWD Move label from For stmt to following
40160:  ReDim PARTRATE(LG)
        ReDim OPEXRATE(LG)
        ReDim ERT(LG)
        ReDim WIN(LG)
        ReDim OPEX(LG)
        ReDim TOTPMT(LG)
        ReDim DFL(LG)
        For x = 1 To LG
            PARTRATE(x) = 0
            OPEXRATE(x) = 0
            ERT(x) = 1
            DFL(x) = AC(x, 7)
            WIN(x) = 1
        Next x
        ReDim PN(4)
        ReDim PNC(4)
        For x = 1 To 4
            PN(x) = ""
            PNC(x) = ""
        Next x
        N1 = ""
        N1C = ""
        ' GDP 08 Apr 2003
        ' Commented out - Speed up, remove unnecessary code
        ' GDP 24 Apr 2003
        ' Reinstated the below code - CF() array is written out as the Cashflow column
        ' in the After Tax Cashflow report when a CONSOL line is encountered
        ''' 9 Feb 2004 JWD (C0783) Remove output to CHUONG.DAT
        '''        flno% = FreeFile
        '''        Open FChuong$ For Output As #flno%
        For x = 1 To LG
            CF(x) = PSCF(x, PS + 1) - NGCF(x, NEG + 1)
            '''          Write #flno%, CF(x)
            CF(0) = CF(0) + CF(x)
        Next x
        '''        Close #flno%

        'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        'GoSub TotalArrays 'total PSCF(), NGCF(), NGCFY(), NGCFA(), NGCFU()
        TotalArrays(CF, COCF, COCCF)

        ' GDP 08 Apr 2003
        ' Commented out - Remove OXY code
        '        GetOxyConsol

        'UPGRADE_WARNING: Return has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        Return
        '--------------------------------------------------------------------

TotalArrays:

        '<<<<<< 19 Mar 2002 JWD (C0499)
        ' Check counts of cash flows
        If PS + 1 > UBound(PSCF, 2) Or NEG + 1 > UBound(NGCF, 2) Then
            Error (ErrorCode_ExceededMaxCashFlowCount)
        End If
        '>>>>>> End (C0499)

        'SUM POSITIVE CASH FLOWS
        TotalCFArrays()

        'CALCULATE NET CASH FLOW
        CF(0) = 0
        FileClose(1)

        ' GDP 08 Apr 2003
        ' Commented out - Speed up, remove unnecessary code
        ' GDP 24 Apr 2003
        ' Reinstated below code as the CF() array is used in reporting the
        ' after tax cash flow when CONSOL is in country file
        ''' 9 Feb 2004 JWD (C0783) Remove output to CHUONG.DAT
        '''      Open FChuong$ For Output As #1
        For x = 1 To LG
            CF(x) = PSCF(x, PS + 1) - NGCF(x, NEG + 1)
            '''        Write #1, CF(x)
            CF(0) = CF(0) + CF(x)
        Next x
        '''      Close #1

        For x = 1 To LG
            If DFL(x) <> 0 Then
                COCF(x) = CF(x) / DFL(x)
            Else
                COCF(x) = 0
            End If
            For y = 1 To x
                COCCF(x) = COCCF(x) + COCF(y)
            Next y
        Next x
        'UPGRADE_WARNING: Return has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        Return
        '---------------------------------------------------------
ErrHandler:

        If Err.Number = 53 Then
            For i = 1 To LG
                DFL(i) = 1
            Next i
            Resume AfDeflate
        Else
            ' 9 Feb 2004 JWD (C0779) Replace with re-raise of error to caller
            Err.Raise(Err.Number) ' TerminateExecution

        End If

    End Sub

    Sub TotalArrays(ByRef CF() As Single, ByRef COCF() As Single, ByRef COCCF() As Single)
        '<<<<<< 19 Mar 2002 JWD (C0499)
        ' Check counts of cash flows
        If PS + 1 > UBound(PSCF, 2) Or NEG + 1 > UBound(NGCF, 2) Then
            Error (ErrorCode_ExceededMaxCashFlowCount)
        End If
        '>>>>>> End (C0499)

        'SUM POSITIVE CASH FLOWS
        TotalCFArrays()

        'CALCULATE NET CASH FLOW
        CF(0) = 0
        FileClose(1)

        ' GDP 08 Apr 2003
        ' Commented out - Speed up, remove unnecessary code
        ' GDP 24 Apr 2003
        ' Reinstated below code as the CF() array is used in reporting the
        ' after tax cash flow when CONSOL is in country file
        ''' 9 Feb 2004 JWD (C0783) Remove output to CHUONG.DAT
        '''      Open FChuong$ For Output As #1

        Dim x As Single, y As Single

        For x = 1 To LG
            CF(x) = PSCF(x, PS + 1) - NGCF(x, NEG + 1)
            '''        Write #1, CF(x)
            CF(0) = CF(0) + CF(x)
        Next x
        '''      Close #1

        For x = 1 To LG
            If DFL(x) <> 0 Then
                COCF(x) = CF(x) / DFL(x)
            Else
                COCF(x) = 0
            End If
            For y = 1 To x
                COCCF(x) = COCCF(x) + COCF(y)
            Next y
        Next x
    End Sub
    Sub x27400(ByRef X7 As Integer, ByRef U5 As Object, ByVal vpr As Object, ByVal DRT As Object, ByVal ZD As Object, ByVal zp As Object, ByVal rUDCF As Object, ByRef iDET As Single, ByVal bRMN As Boolean, ByVal iEntityID As Short, ByVal eEntityID_Company As Short, ByVal GVTK As Short, ByVal eEntityID_3rdParty As Short, ByVal eEntityID_NOC As Short, ByVal eEntityID_Government As Short) ' This drives discounting for ROR calculation
        ' It returns U5 set to discounted NPV

        ' iCFCol, bRMN, and bGVCF should be set by caller

        ' discount the cashflow...
        X7 = 1
        'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        x28000(vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, X7)
        U5 = vpr(0)

        ' discount the CAPEX
        X7 = 3
        'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'

        x28000(vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, X7)

        U5 = U5 - vpr(0)

        'UPGRADE_WARNING: Return has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
    End Sub

    Sub x28000(ByRef vpr() As Single, ByVal DRT As Object, ByVal ZD As Object, ByVal zp As Object, ByVal rUDCF As Object, ByRef iDET As Single, ByVal bRMN As Boolean, ByVal iEntityID As Short, ByVal eEntityID_Company As Short, ByVal GVTK As Short, ByVal eEntityID_3rdParty As Short, ByVal eEntityID_NOC As Short, ByVal eEntityID_Government As Short, ByVal X7 As Integer)
        '28000:  ' THIS SUBROUTINE DOES DISCOUNTING
        '    YR - project start year      MO - project start month
        '    Y1 - production start year   M1 - production start month
        '    Y2 - discovery start year    M2 - discovery start month
        '    Y3 - discount start year     M3 - discount start month

        '    ZP and ZD are calculated in 27000
        '    ZD & ZP are pointers into cash flow years
        '    ZD = Int(Y1 - YR) + 1  year # (relative to proj year) of prod start year
        '    ZP = Y3 - YR + 1       year # (relative to proj year) of discount year

        '---------------------------------------------------------------------------------------------------------------------
        ' this code added 1/7/2000 to allow the Discount Date to be prior to the Project Start Date

        Dim iCFCol As Short
        Dim D5D As Single
        Dim dZDF As Single
        Dim dXCF As Single
        Dim dXDF As Double
        Dim DateDisc As Single
        Dim DateProj As Single
        Dim DiscFlag As Integer
        Dim TempDiscYr As Single
        Dim TempDiscMo As Single

        ' Set DiscFlag = 1 if the Discount Date is prior to the Project Start Date
        DiscFlag = 0

        'Since month's only matter for the FRC Discount Method, we must check differently
        If DiscMthd = 1 Then

            DateProj = YR + ((mo - 1) / 12)
            DateDisc = Y3 + ((M3 - 1) / 12)

            If DateDisc < DateProj Then

                DiscFlag = 1

                'Since the Discount Date is prior to the Project Start Date, we are going to temporarily set the                                    'Discount Date equal to the Project Start Date so that the PV's will be as of the Project Start Date.                                                                     'At the bottom, we will then discount these PV's back to the real discount date.  We will also reset                                            'the Discount Date  at the bottom so that it will be correct in all subsequent processing.

                TempDiscYr = Y3
                TempDiscMo = M3

                Y3 = YR
                M3 = mo

            End If
        Else 'This checks if the Discount Year is prior to the Project Start Year for                                                                                                                                                                  'Discounting Methods 2, 3, 4
            If Y3 < YR Then

                DiscFlag = 1
                TempDiscYr = Y3
                Y3 = YR

            End If
        End If



        ' zero out the working array

        Dim i As Short

        For i = 0 To LG
            vpr(i) = 0
        Next i

        ' compute the fractional annual discount factor
        dXDF = (1 + (DRT / 100))

        'computed GOTO - if X7 = 1 then Operating Cash Flow
        '              - X7 is NEVER 2 ?????
        '              - if X7 = 3 then CAPEX Cash Flow
        Select Case System.Math.Round(X7)
            Case Is < 0
                Error (5)
            Case 1
                GoTo 28030
            Case 2
                GoTo 28030
            Case 3
                GoTo 28220
        End Select

        'Operating Cash Flows --------------------------------
28030:

        If DiscMthd = 1 Then
            ' compounding factor to correct to discount year
            dXCF = dXDF ^ (Y3 - YR + ((M3 - mo) / 12))
            dZDF = dXDF ^ (ZD - 2 + ((13 - mo) / 12))

            For i = zp To LG
                D5D = rUDCF(i, iCFCol)

                If D5D <> 0 Then
                    If i > ZD Then
                        D5D = D5D / (dXDF ^ (i - ZD + 0.5)) / dZDF
                    Else
                        If i = ZD Then
                            D5D = D5D / (dXDF ^ (1 - (((13 - M1) * 0.5) / 12))) / dZDF
                        Else
                            If i = 1 Then
                                D5D = D5D / (dXDF ^ (((13 - mo) * 0.5) / 12))
                            ElseIf i > 1 Then
                                D5D = D5D / (dXDF ^ ((i - 1.5) + ((13 - mo) / 12)))
                            End If
                        End If
                    End If
                    vpr(i) = D5D * dXCF
                    vpr(0) = vpr(0) + vpr(i)
                End If
            Next i
        Else 'method 2,3,4 (BEG,MID,END)
            ' compounding factor to correct to discount year
            dXCF = dXDF ^ (zp - 1)

            ' compute offset for different discounting methods

            Dim dOffset As Short
            If DiscMthd = 2 Then 'Beginning of the year discounting
                dOffset = 1
            ElseIf DiscMthd = 3 Then 'Middle of the year discounting
                dOffset = 0.5
            ElseIf DiscMthd = 4 Then 'End of the year discounting
                dOffset = 0
            End If

            ' if prior to discount year - disc cash flow = 0
            For i = zp To LG
                D5D = rUDCF(i, iCFCol) / (dXDF ^ (i - dOffset))
                'above formula discounts to project start year,
                '  now compound up to discount year (ignore month)
                vpr(i) = D5D * dXCF
                vpr(0) = vpr(0) + vpr(i)
            Next i
        End If

        GoTo 28420

        'Capital Expenditures section ---------------------------------------
28220:  'This discounts capital

        For i = 1 To my3tt
            Dim rD5X As Single
            rD5X = 0
            D5D = 0
            'Do standard calculation first
            iDET = my3(i, 3) - YR + 1
            If DFL(iDET) <= 0 Then GoTo 28410

            ' 17 May 2005 JWD (C0878) Change to compare range, rather than individually
            ' (Assumes code values are sequential and contiguous)
            ' Exclude BAL from discounting
            If (my3(i, gc_nMY3_CAT) >= CPXCategoryCodeBAL) And (my3(i, gc_nMY3_CAT) <= CPXCategoryCodeBLn) Then
                GoTo 28410
            End If
            'was:
            ''<<<<<< 21 Jun 2001 JWD (C0339)
            'If my3(i, 1) = CPXCategoryCodeBAL Then GoTo 28410  ' BAL
            'If my3(i, 1) = CPXCategoryCodeBL2 Then GoTo 28410  ' BL2
            'If my3(i, 1) = CPXCategoryCodeBL3 Then GoTo 28410  ' BL3
            ''~~~~~~ was:
            ''If my3(i, 1) = 18 Then GoTo 28410
            ''If my3(i, 1) = 19 Then GoTo 28410
            ''If my3(i, 1) = 20 Then GoTo 28410
            ''>>>>>> End (C0339)
            ' End (C0878)

            '<<<<<< 4 Aug 2001 JWD (C0363)
            If my3(i, 1) = CPXCategoryCode_AbandonmentAccrualEntry Then
                GoTo 28410
            End If
            '>>>>>> End (C0363)

            'Now exclude capex prior to Discount Date
            If DiscMthd = 1 Then
                If my3(i, 3) < Y3 Or (my3(i, 3) = Y3 And my3(i, 2) < M3) Then
                    GoTo 28410
                End If
            ElseIf DiscMthd = 2 Or DiscMthd = 3 Or DiscMthd = 4 Then
                If my3(i, 3) < Y3 Then
                    GoTo 28410
                End If
            End If

            If bRMN Then ' Risk Money - exclude capex after discovery date
                If DiscMthd = 1 Then
                    If my3(i, 3) > Y2 Or (my3(i, 3) = Y2 And my3(i, 2) >= M2) Then
                        GoTo 28410
                    End If
                Else ' for discount methods 2,3,4
                    If my3(i, 3) >= Y2 Then
                        GoTo 28410
                    End If
                End If
            End If

            ' 21 Dec 2004 JWD (C0846) Add 3rd party and NOC capex cases, revise selection of interests to apply to amount
            rD5X = my3(i, gc_nMY3_AMT)
            Select Case iEntityID
                Case eEntityID_Company
                    If GVTK = 1 Then ' calculating government take
                        If my3(i, gc_nMY3_CAT) = 1 Then
                            rD5X = 0
                        Else
                            rD5X = rD5X * (1 - PARTRATE(iDET)) * WINC(i)
                        End If
                    Else
                        rD5X = rD5X * GPRATE(i) * WINC(i)
                    End If
                Case eEntityID_3rdParty
                    '''             ' Use grossed-up amount
                    '''             rD5X = GMY3(i) * (1 - EffIntsX(i, gc_nEffIntsX_PAR)) * (1 - EffIntsX(i, gc_nEffIntsX_WIN))
                    rD5X = my3Ex(i, 2) - my3Ex(i, 3) ' 3dparty = Group - Company
                Case eEntityID_NOC
                    '''             ' Use grossed-up amount
                    '''             'rD5X = GMY3(i) * EffIntsX(i, gc_nEffIntsX_PAR)
                    rD5X = my3Ex(i, 1) - my3Ex(i, 2) ' NOC = Operating - Group
                Case eEntityID_Government
                    '''             ' Use grossed-up amount
                    '''             If my3(i, gc_nMY3_CAT) = 1 Then      ' only thing for government is bonuses
                    '''                 rD5X = 0 - GMY3(i)     ' and these are incomes to government.
                    '''             Else
                    '''                 rD5X = 0
                    '''             End If
                    rD5X = my3Ex(i, 0) - my3Ex(i, 1) ' govt = total - operating
            End Select

            rD5X = rD5X / DFL(iDET) ' apply deflator

            ' was:
            'rD5X = (my3(i, 5) * GPRATE(i) * WINC(i)) / DFL(iDET)
            '
            'If rD5X <> 0 Then
            '   If GVTK = 1 Then     ' Government Take Calculation
            '      rD5X = (my3(i, 5) * (1 - PARTRATE(iDET)) * WINC(i)) / DFL(iDET)
            '      If my3(i, 1) = 1 Then
            '         GoTo 28410
            '      End If
            '
            '   ElseIf bGVCF Then    ' Government Cash Flow
            '      If my3(i, 1) = 1 Then
            '         rD5X = 0 - my3(i, 5)
            '      Else
            '         rD5X = (my3(i, 5) * (1 - GPRATE(i))) / DFL(iDET)
            '      End If
            '   End If
            'End If
            ' End (C0846)

            'rD5X now calculated - now calculate fractional years for discounting
            If rD5X <> 0 Then
                If DiscMthd = 1 Then
                    D5D = rD5X / (dXDF ^ (my3(i, 3) - Y3 + ((my3(i, 2) - M3) / 12)))
                ElseIf DiscMthd = 2 Then  'beg
                    D5D = rD5X / (dXDF ^ (my3(i, 3) - Y3))
                ElseIf DiscMthd = 3 Then  'mid
                    D5D = rD5X / (dXDF ^ (my3(i, 3) - Y3 + 0.5))
                ElseIf DiscMthd = 4 Then  'end
                    D5D = rD5X / (dXDF ^ (my3(i, 3) - Y3 + 1))
                End If
                vpr(iDET) = vpr(iDET) + D5D
                vpr(0) = vpr(0) + D5D
            End If

28410:  Next i ' goto target!!!!!
28420:  'Check to see if flag set for Discount Date being prior to Project Start Date

        If DiscFlag = 1 Then

            'Reset M3 and Y3 to proper values and calculate Discounting Time from
            'Project Start Date back to Discounting Date, depending on Discounting Method


            Dim DiscTime As Single
            If DiscMthd = 1 Then
                Y3 = TempDiscYr
                M3 = TempDiscMo
                DiscTime = DateProj - DateDisc
                'discounting time if Discounting Method = FRC
            Else
                Y3 = TempDiscYr
                DiscTime = YR - Y3
                'discounting time if Discounting Method <> FRC
            End If

            'Discount the already calculated PV's back to the Discount Date
            vpr(0) = 0
            For i = 1 To LG
                vpr(i) = vpr(i) / (dXDF ^ DiscTime)
                vpr(0) = vpr(0) + vpr(i)
            Next i

        End If
    End Sub
    Sub x27000(ByRef rUDCF As Object(,), ByRef ZD As Single, ByRef zp As Object, ByRef TorULines As Boolean, ByVal OPXP As Object, ByVal CPXP As Object) ' This subroutine sets up for discounting...
        ' it copies the discountable cashflows into a holding array
        ' and applies any deflator factor and any cashflow adjustment
        ' factors that are independent of data or discount rate....
        ' Array rUDCF holds discountable cashflows.  Non-discountable
        ' (i.e. prior to discount year) cashflows are not stored.
        ' Columns
        '   1  = Company revenues
        '   2  = Company operating expense
        '   3  = Not used
        '   4  = Company royalty/tax
        '   5  = Company net income (1-2-4) exclusive of CAPEX
        ' 6-8  = Not used
        '   9  = Special OXY T & U cashflows
        '  10  = Government Cashflow for Govt Take calculation
        '  11  = Government Cashflow
        '     20 Dec 2004 JWD (C0846) Adding new cash flows for 3rd party and NOC
        '  12  = 3rd party net cash flow (exclusive of capital)
        '  13  = NOC net cash flow (exclusive of capital)
        '--------------------------------------------------------------


        ' 20 Dec 2004 JWD (C0846) Change bounds, added columns for new cash flows
        ReDim rUDCF(LG, 13) ' was: (LG, 11)

        ZD = Int(Y1 - YR) + 1 'year # (relative to proj year) of prod start year
        zp = Y3 - YR + 1 'year # (relative to proj year) of discount year

        'added 1/24/00  - don't let zp be less than 1 when discount date is prior to project start date
        If zp < 1 Then zp = 1


        'check for existing opex and capex in negative cf

        Dim NEGLP As Short

        NEGLP = NEG
        If my3tt <> 0 Then
            NEGLP = NEGLP - 1
        End If
        If OPEX(0) <> 0 Then
            NEGLP = NEGLP - 1
        End If

        '--------------------------------------------------------------
        'OXY Version 5.3
        'If this run contains "T" (Third party) and/or "U"
        '(US Government Tax payments) the we have to calculate
        'GOVT TAKE a little differently.  We calculate GOVT
        'TAKE as though there were not any "T" or "U" variables
        'on the Fiscal Definition form.
        '
        'Store cashflows attributable to "T" or "U" items

        TorULines = False
        Dim i As Short
        For i = 1 To TDT
            Dim adjust As Boolean

            adjust = True
            If TD(i, 2) = "OIL" And PPR = 2 Then
                adjust = False
                'don't count this line
            ElseIf TD(i, 2) = "GAS" And PPR = 1 Then
                adjust = False
                'don't count this line
            End If

            If adjust = True Then
                If Left(TD(i, 4), 1) = "T" Or Left(TD(i, 4), 1) = "U" Then
                    TorULines = True
                    Dim ww As Short
                    For ww = zp To LG
                        If DFL(ww) <> 0 Then
                            rUDCF(ww, 9) = rUDCF(ww, 9) + RVN(ww, i)
                        End If
                    Next ww
                End If
            End If
        Next i
        '--------------------------------------------------------------

        ' Accumulate remaining discountable cashflows
        For i = zp To LG

            ' check deflator ....
            If DFL(i) = 0 Then GoTo 27099

            '-------------------------------------
            ' Accumulate company cashflows...
            '   company cashflows
            rUDCF(i, 1) = PSCF(i, PS + 1) ' u1  - revenues
            rUDCF(i, 2) = NGCF(i, OPXP) ' u2  - opex
            rUDCF(i, 4) = NGCF(i, NEG + 1) - NGCF(i, OPXP) - NGCF(i, CPXP) ' u4 - royalty/tax

            '-------------------------------------
            ' Accumulate Government cashflows...
            'govt starts at 100% gross revenues
            ' GDP 20 Jan 2003
            ' Replaced revenue calc with call to ATotalRevenues
            'rUDCF(i, 11) = (A(i, 1) * A(i, 7)) + (A(i, 2) * A(i, 8)) + (A(i, 3) * A(i, 9)) + (A(i, 4) * A(i, 10))
            rUDCF(i, 11) = ATotalRevenues(i)

            ' Cashflow for government take calculation
            ' do it now while we have 100% gross revenues in column 11
            rUDCF(i, 10) = rUDCF(i, 11) * WIN(i) * (1 - PARTRATE(i)) - NGCF(i, OPXP)


            ' 20 Dec 2004 JWD (C0846) Redefine government cash flow
            ' Starts with total revenues as defined above,
            ' less any positive cash flows to Group (Company & 3rd party),
            ' plus negative cash flows defined in fiscal model (excluding
            ' "T" cash flow variables, which go from Company to third party)

            '''         rUDCF(i, 11) = GREV(i) - GPCFV(i) + GNCFV(i)
            rUDCF(i, 11) = gna_ACFX(i, gna_ACFX_TPS) - gna_ACFX(i, gna_ACFX_TNG) - gna_ACFX(i, gna_ACFX_OPS) + gna_ACFX(i, gna_ACFX_ONG)

            '!!! TODO: Need to add in the capex attributable to the govt so this is net of capex

            ' was:
            '' Now reduce column 11 to come to government cashflow...
            'For y = 1 To PS%
            '   If y = REPY% Or y = fin% Then
            '   Else
            '      If WIN(i) <> 0 Then
            '         rUDCF(i, 11) = rUDCF(i, 11) - (PSCF(i, y) / WIN(i))
            '      End If
            '   End If
            'Next y
            '
            'For y = 1 To NEGLP%
            '   If WIN(i) <> 0 Then
            '      rUDCF(i, 11) = rUDCF(i, 11) + (NGCF(i, y) / WIN(i))
            '   End If
            'Next y
            '
            'For y = 1 To NEGT%      'Third Party Pmts do not go to Govt!
            '   If WIN(i) <> 0 Then
            '      rUDCF(i, 11) = rUDCF(i, 11) - (NGCFT(i, y) / WIN(i))
            '   End If
            'Next y
            '
            'For y = 1 To NEGU%     'US TAX does not go to host govt!
            '   If WIN(i) <> 0 Then
            '      rUDCF(i, 11) = rUDCF(i, 11) - (NGCFU(i, y) / WIN(i))
            '   End If
            'Next y
            '                        'do opex
            'rUDCF(i, 11) = rUDCF(i, 11) - (OPEX(i) * OPEXRATE(i))
            '
            'If WIN(i) <> 0 Then
            '   rUDCF(i, 11) = rUDCF(i, 11) - (TOTPMT(i) / WIN(i))
            'End If
            ' End (C0846)

            ' 20 Dec 2004 JWD (C0846) New code to determine cash flows for 3rd pary and NOC
            ' Third party cash flow
            '''         rUDCF(i, 12) = ((GPCFV(i) - GNCFV(i)) * (1 - EffInts(i, gc_nEffInts_PAR)) + GREPAY(i)) * (1 - EffInts(i, gc_nEffInts_WIN)) - GOPEX(i) * (1 - EffInts(i, gc_nEffInts_POX)) * (1 - EffInts(i, gc_nEffInts_WOX)) - CNREIM(i) + C3DPTY(i) + (GFINAN(i) * (1 - EffInts(i, gc_nEffInts_WFN)) * (1 - EffInts(i, gc_nEffInts_PFN)))
            rUDCF(i, 12) = gna_ACFX(i, gna_ACFX_GPS) - gna_ACFX(i, gna_ACFX_CPS) - gna_ACFX(i, gna_ACFX_GNG) + gna_ACFX(i, gna_ACFX_CNG)

            ' NOC cash flow
            '''         rUDCF(i, 13) = (GPCFV(i) - GNCFV(i)) * EffInts(i, gc_nEffInts_PAR) - GOPEX(i) * EffInts(i, gc_nEffInts_POX) - GREPAY(i) + (GFINAN(i) * EffInts(i, gc_nEffInts_PFN))
            rUDCF(i, 13) = gna_ACFX(i, gna_ACFX_OPS) - gna_ACFX(i, gna_ACFX_GPS) - gna_ACFX(i, gna_ACFX_ONG) + gna_ACFX(i, gna_ACFX_GNG)
            ' End (C0846)

            ' adjust amounts for deflator and
            rUDCF(i, 1) = rUDCF(i, 1) / DFL(i)
            rUDCF(i, 2) = rUDCF(i, 2) / DFL(i)
            rUDCF(i, 4) = rUDCF(i, 4) / DFL(i)
            rUDCF(i, 9) = rUDCF(i, 9) / DFL(i)
            rUDCF(i, 10) = rUDCF(i, 10) / DFL(i)
            rUDCF(i, 11) = rUDCF(i, 11) / DFL(i)

            ' 20 Dec 2004 JWD (C0846) New cash flows (3rd party & NOC)
            rUDCF(i, 12) = rUDCF(i, 12) / DFL(i)
            rUDCF(i, 13) = rUDCF(i, 13) / DFL(i)
            ' End (C0846)

            rUDCF(i, 5) = rUDCF(i, 1) - rUDCF(i, 2) - rUDCF(i, 4)

27099:  Next i ' target of GoTo !!!!!


        Dim www As Short

        For www = 1 To LG
            gna_ACFX(www, 10) = rUDCF(www, 5)
            gna_ACFX(www, 12) = rUDCF(www, 12)
            gna_ACFX(www, 14) = rUDCF(www, 13)
            gna_ACFX(www, 16) = rUDCF(www, 11)
            gna_ACFX(www, 18) = rUDCF(www, 10)
            gna_ACFX(www, 20) = ATotalRevenues(www)
            gna_ACFX(www, 21) = A(www, gc_nAOIL)
            gna_ACFX(www, 22) = A(www, gc_nAOPC)
            gna_ACFX(www, 23) = A(www, gc_nAOPC) * A(www, gc_nAOIL)
        Next www

        ' need to add the capex back into the discountable cash flow
        ' to get the cash flow exclusive of capex,
        ' capex will be deducted as part of discounting
        ' zp is the discount start period, any capex before is ignored

        For i = 1 To my3tt
            www = my3(i, 3) - YR + 1
            If www >= zp Then
                ' Govt
                rUDCF(www, 11) = rUDCF(www, 11) + my3Ex(i, 0) - my3Ex(i, 1)
                ' 3d party
                rUDCF(www, 12) = rUDCF(www, 12) + my3Ex(i, 2) - my3Ex(i, 3)
                ' NOC
                rUDCF(www, 13) = rUDCF(www, 13) + my3Ex(i, 1) - my3Ex(i, 2)
            End If
        Next i

        '''        For www = 1 To LG
        '''            gna_ACFX(www, 18) = rUDCF(www, 5)
        '''            gna_ACFX(www, 20) = rUDCF(www, 12)
        '''            gna_ACFX(www, 22) = rUDCF(www, 13)
        '''            gna_ACFX(www, 24) = rUDCF(www, 11)
        '''        Next www



        ' Now adjust cashflows in discount year
        ' compute cashflow adjustment factor for discount year...


        Dim fco As Single
        fco = 1

        'added 1/24/00 to allow adjustments to be made only if discount date is on or after project start date
        If Y3 >= YR Then
            If zp = ZD Then 'discount year is prod start year
                If M3 > M1 Then ' discount date after prod start
                    fco = (13 - M3) / (13 - M1)
                End If
            Else 'NOT production start year
                If zp = 1 Then 'proj start year
                    fco = (13 - M3) / (13 - mo)
                Else 'not proj or prod start year
                    fco = (13 - M3) / 12
                End If
            End If
        End If

        If fco <> 1 Then
            rUDCF(zp, 1) = rUDCF(zp, 1) * fco
            rUDCF(zp, 2) = rUDCF(zp, 2) * fco
            rUDCF(zp, 4) = rUDCF(zp, 4) * fco
            rUDCF(zp, 5) = rUDCF(zp, 5) * fco
            rUDCF(zp, 9) = rUDCF(zp, 9) * fco
            rUDCF(zp, 10) = rUDCF(zp, 10) * fco
            rUDCF(zp, 11) = rUDCF(zp, 11) * fco
            ' 20 Dec 2004 JWD (C0846) Add new cash flows
            rUDCF(zp, 12) = rUDCF(zp, 12) * fco
            rUDCF(zp, 13) = rUDCF(zp, 13) * fco
            ' End (C0846)
        End If
    End Sub
    Sub x27100(ByRef rUDCF As Object, ByRef ZD As Object, ByRef zp As Object, ByRef TorULines As Boolean, ByVal OPXP As Object, ByVal CPXP As Object, ByRef DRT As Object, ByRef ESPV As Object, ByVal u1 As Object, ByVal u2 As Object, ByVal U3 As Object, ByVal U11 As Object, ByVal U12 As Object, ByVal U4 As Object, ByVal U7 As Object, ByVal U13 As Object, ByRef PAY As Object, ByVal ATCF As Object, ByRef CODCF As Object, ByVal POT As Object, ByVal GOVT As Object, ByRef GVDCF As Object, ByRef iEntityID As Object, ByVal eEntityID_Company As Object, ByRef iCFCol As Integer, ByRef X7 As Integer, ByRef U8 As Object, ByRef GVTK As Object, ByRef vpr As Single(), ByRef iDET As Single, ByRef bRMN As Boolean, ByVal eEntityID_3rdParty As Object, ByVal eEntityID_NOC As Object, ByVal eEntityID_Government As Object, ByVal U26 As Object, ByVal U23 As Object, ByVal TATCF As Object, ByRef TDPDCF As Object, ByVal U27 As Object, ByVal U36 As Object, ByVal U33 As Object, ByVal NATCF As Object, ByRef NOCDCF As Object, ByVal U37 As Object, ByRef RR As Object, ByVal RRZ As Object, ByRef RRT As Object, ByRef RRN As Object, ByRef RRB As Object, ByVal U5 As Object, ByRef RU As Object, ByRef RE As Object, ByRef rl As Object, ByRef CU As Object, ByRef cl As Object, ByRef U5L As Object, ByRef CM1 As Object, ByRef CM2 As Object) ' THIS CALCULATES ESPV(1 - 13,6) FOR ECONOMIC SUMMARY PAGE

        'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        x27000(rUDCF, ZD, zp, TorULines, OPXP, CPXP) ' accumulate discountable cashflows


        Dim w As Short


        For w = 1 To 6
            DRT = gn(w + 3)
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            'GoSub 27300

27300:      '  THIS CALCULATES U1 - U8 FOR A GIVEN DISCOUNT RATE
            '------------------------------------
            ' Cashflow discounting....

            ' company revenues
            iCFCol = 1
            X7 = 1
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            x28000(vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, X7)
            u1 = vpr(0)
            Dim i As Short
            For i = 1 To LG
                ATCF(i) = vpr(i)
            Next i

            ' company expenses
            iCFCol = 2
            X7 = 1
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            x28000(vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, X7)
            u2 = vpr(0)

            For i = 1 To LG
                ATCF(i) = ATCF(i) - vpr(i)
            Next i

            ' company royalty/tax payments
            iCFCol = 4
            X7 = 1
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            x28000(vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, X7)
            U4 = vpr(0)

            For i = 1 To LG
                ATCF(i) = ATCF(i) - vpr(i)
            Next i

            ' government cashflow
            iCFCol = 11
            X7 = 1
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            x28000(vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, X7)
            U11 = vpr(0)

            For i = 1 To LG
                GOVT(i) = vpr(i)
            Next i

            ' 21 Dec 2004 JWD (C0846) Add 3rd party & NOC cash flows
            ' third-party operating net cash flow (excludes capex)
            iCFCol = 12
            X7 = 1
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            x28000(vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, X7)
            U26 = vpr(0)

            For i = 1 To LG
                TATCF(i) = vpr(i)
            Next i

            ' NOC operating net cash flow (excludes capex)
            iCFCol = 13
            X7 = 1
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            x28000(vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, X7)
            U36 = vpr(0)

            For i = 1 To LG
                NATCF(i) = vpr(i)
            Next i
            ' End (C0846)

            ' 21 Dec 2004 JWD (C0846) PV of 3rd party and NOC CAPEX and new Company & Govt
            bRMN = False
            ' Company capex
            iEntityID = eEntityID_Company
            X7 = 3
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            x28000(vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, X7)
            U3 = vpr(0)
            For i = 1 To LG
                ATCF(i) = ATCF(i) - vpr(i)
            Next i

            iEntityID = eEntityID_3rdParty
            X7 = 3
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            x28000(vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, X7)
            U23 = vpr(0)
            For i = 1 To LG
                TATCF(i) = TATCF(i) - vpr(i)
            Next i

            iEntityID = eEntityID_NOC
            X7 = 3
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            x28000(vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, X7)
            U33 = vpr(0)
            For i = 1 To LG
                NATCF(i) = NATCF(i) - vpr(i)
            Next i

            ' Government capex
            ' Government has no capital expenditures, but
            ' bonuses (actually income to the government)
            ' are in the capital expenditure array (my3())
            iEntityID = eEntityID_Government
            X7 = 3
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            x28000(vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, X7)
            U11 = U11 - vpr(0)
            ' Add to operating cash flow
            U12 = 0
            ' No capital expenditures for government
            For i = 1 To LG
                GOVT(i) = GOVT(i) - vpr(i)
            Next i


            ' Now PV of risk money
            bRMN = True
            ' Company risk money
            iEntityID = eEntityID_Company
            X7 = 3
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            x28000(vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, X7)
            U7 = vpr(0)

            ' 3rd party risk money
            iEntityID = eEntityID_3rdParty
            X7 = 3
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            x28000(vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, X7)
            U27 = vpr(0)

            ' NOC risk money
            iEntityID = eEntityID_NOC
            X7 = 3
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            x28000(vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, X7)
            U37 = vpr(0)

            ' Government risk money
            ' Government has no money at risk, it does not have ANY expenditures
            U13 = 0

            ' was:
            ''------------------------------------------
            '' Capex discounting...
            '    ' PV OF GOVT CAPEX
            'bRMN = False
            'bGVCF = True
            'X7 = 3
            'x28000(vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, X7)
            'U12 = vpr(0)
            '
            'For i = 1 To LG
            '  GOVT(i) = GOVT(i) - vpr(i)
            'Next i
            '
            '    ' PV OF GOVT RISK MONEY
            'bRMN = True
            'bGVCF = True
            'X7 = 3
            'x28000(vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, X7)
            'U13 = vpr(0)
            '
            '   ' PV of company capex
            'bRMN = False
            'bGVCF = False
            'X7 = 3
            'x28000(vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, X7)
            'u3 = vpr(0)
            '
            'For i = 1 To LG
            '  ATCF(i) = ATCF(i) - vpr(i)
            'Next i
            '
            'bRMN = True
            'X7 = 3
            'x28000(vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, X7)
            'U7 = vpr(0)
            '
            ' End (C0846)

            'end gosyb 27300

            ESPV(1, w) = u1
            ESPV(2, w) = u2
            ESPV(3, w) = U3
            ESPV(4, w) = U4
            ESPV(5, w) = U11 - U12
            ESPV(9, w) = u1 - u2 - U3 - U4
            If U3 = 0 Then GoTo 27205
            ESPV(12, w) = (u1 - u2 - U4) / U3
            GoTo 27210

27205:      ESPV(12, w) = 0

27210:      If U7 <= 0 Then GoTo 27216
            ESPV(11, w) = (u1 - u2 - U3 - U4) / U7
            GoTo 27218

27216:      ESPV(11, w) = 101

27218:
            ' GOVT PI AND RRR
            If U12 <> 0 Then
                ESPV(8, w) = U11 / U12
            Else
                ESPV(8, w) = 0
            End If

            If U13 > 0 Then
                ESPV(7, w) = (U11 - U12) / U13
            Else
                ESPV(7, w) = 101
            End If

            For i = 1 To LG
                PAY(i) = ATCF(i)
                If w = 5 Then
                    CODCF(i) = ATCF(i)
                End If
            Next i
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            x26200(POT, CM1, PAY, CM2)

            ESPV(10, w) = POT

            ' GOVT PAYOUT
            For i = 1 To LG
                PAY(i) = GOVT(i)
                If w = 5 Then
                    GVDCF(i) = GOVT(i)
                End If
            Next i
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            x26200(POT, CM1, PAY, CM2)
            ESPV(6, w) = POT

            ' 21 Dec 2004 JWD (C0846) Add next to select company capex for government take calculations
            iEntityID = eEntityID_Company
            ' Government take
            iCFCol = 10
            X7 = 1
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            x28000(vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, X7)
            U8 = vpr(0)
            GVTK = 1

            X7 = 3
            bRMN = False
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            x28000(vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, X7)
            GVTK = 0
            U8 = U8 - vpr(0)
            If U8 <> 0 Then
                '             'OXY Version 5.3
                '             'If this run contains "T" (Third party) and/or "U"
                '             '(US Government Tax payments) the we have to calculate
                '             'GOVT TAKE a little differently.  We calculate GOVT
                '             'TAKE as though there were not any "T" or "U" variables
                '             'on the Fiscal Definition form.  Right now, ESPV(9, w)
                '             'contains the discounted Company Cash Flow
                '
                '            TorULines% = False
                '            For i = 1 To TDT
                '               adjust% = True
                '               If TD$(i, 2) = "OIL" And PPR = 2 Then
                '                  adjust% = False          'don't count this line
                '               ElseIf TD$(i, 2) = "GAS" And PPR = 1 Then
                '                  adjust% = False          'don't count this line
                '               End If
                '               If adjust% = True Then
                '                  If Left$(TD$(i, 4), 1) = "T" Or Left$(TD$(i, 4), 1) = "U" Then
                '                     TorULines% = True
                '                     For ww = 1 To LG
                '                        D5(ww) = D5(ww) + RVN(ww, i)
                '                     Next ww
                '                  End If
                '               End If
                '            Next i

                If TorULines = True Then 'adjust GOVT TAKE for "T" and "U" lines
                    iCFCol = 9
                    X7 = 1
                    'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
                    x28000(vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government, X7)
                    ''               ESPV(13, w) = (1 - (ESPV(9, w) / U8)) * 100
                    ESPV(13, w) = (1 - ((ESPV(9, w) + vpr(0)) / U8)) * 100
                Else 'this is the regular case
                    ESPV(13, w) = (1 - (ESPV(9, w) / U8)) * 100
                End If
            End If

            ' 20 Dec 2004 JWD (C0846)
            '            If Not g_bPTCons Then
            ' 3rd party net after-tax cash flow
            ESPV(15, w) = U26 - U23 ' Operating ncf - capex

            ' 3rd party payout
            For i = 1 To LG
                PAY(i) = TATCF(i)
            Next i
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            x26200(POT, CM1, PAY, CM2)
            ESPV(16, w) = POT

            ' Capture discounted cash flow vector
            If w = 5 Then
                For i = 1 To LG
                    TDPDCF(i) = TATCF(i)
                Next i
            End If

            ' 3rd party risk return ratio
            ESPV(17, w) = 101 ' set the default value as "NO RISK"
            If U27 <> 0 Then
                ESPV(17, w) = (U26 - U23) / U27
            End If

            ' 3rd party profitability index
            ESPV(18, w) = 0 ' set the default value
            If U23 <> 0 Then
                ESPV(18, w) = U26 / U23
            End If

            ' NOC net after-tax cash flow
            ESPV(19, w) = U36 - U33 ' Operating ncf - capex

            ' NOC payout
            For i = 1 To LG
                PAY(i) = NATCF(i)
            Next i
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            x26200(POT, CM1, PAY, CM2)
            ESPV(20, w) = POT

            ' Capture discounted cash flow vector
            If w = 5 Then
                For i = 1 To LG
                    NOCDCF(i) = NATCF(i)
                Next i
            End If

            ' NOC risk return ratio
            ESPV(21, w) = 101 ' set the default value as "NO RISK"
            If U37 <> 0 Then
                ESPV(21, w) = (U36 - U33) / U37
            End If

            ' NOC profitability index
            ESPV(22, w) = 0 ' set the default value
            If U33 <> 0 Then
                ESPV(22, w) = U36 / U33
            End If
            '            Else
            '                ' these values are "N/A" in pre-tax consolidation run
            '                For i = 15 To 22
            '                    ESPV(i, w) = rMaxNeg
            '                Next i
            '            End If
            ' End (C0846)
        Next w 'for at line 27102

        '----------------------------------------
        ' Calculate Rates of Return
        '----------------------------------------

        ' 21 Dec 2004 JWD (C0846) Add ROR calcs for 3rd party and NOC and new for Company & Govt
        iEntityID = eEntityID_Company
        iCFCol = 5
        'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'



        RR = RRZ

        '        If Not g_bPTCons Then
        iEntityID = eEntityID_3rdParty
        iCFCol = 12
        'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        x27500(DRT, U5, RRZ, RU, RE, rl, CU, cl, U5L, X7, vpr, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government)
        RRT = RRZ

        iEntityID = eEntityID_NOC
        iCFCol = 13
        'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        x27500(DRT, U5, RRZ, RU, RE, rl, CU, cl, U5L, X7, vpr, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government)
        RRN = RRZ
        '        Else
        '            RRT = rMaxNeg
        '            RRN = rMaxNeg
        '        End If

        iEntityID = eEntityID_Government
        iCFCol = 11
        'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        x27500(DRT, U5, RRZ, RU, RE, rl, CU, cl, U5L, X7, vpr, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government)
        RRB = RRZ

        ' was:
        ' Government Rate of Return
        'XR = 0
        'x27500(DRT, U5, RRZ, RU, RE, rl, CU, cl, U5L)       'to skip govt ROR, comment out this call
        '   ' RRB = GOVT ROR
        'RRB = RRZ
        '
        '' Company After Tax Rate of Return...
        'XR = 1
        'x27500(DRT, U5, RRZ, RU, RE, rl, CU, cl, U5L)
        '   ' RR = COMPANY AFTER-TAX ROR
        'RR = RRZ
        '
        ' End (C0846)
    End Sub
    Sub x27500(ByRef DRT As Object, ByVal U5 As Object, ByRef RRZ As Object, ByRef RU As Object, ByRef RE As Object, ByRef rl As Object, ByRef CU As Object, ByRef cl As Object, ByRef U5L As Object, ByRef X7 As Integer, ByVal vpr As Object, ByVal ZD As Object, ByVal zp As Object, ByVal rUDCF As Object, ByRef iDET As Single, ByVal bRMN As Boolean, ByVal iEntityID As Short, ByVal eEntityID_Company As Short, ByVal GVTK As Short, ByVal eEntityID_3rdParty As Short, ByVal eEntityID_NOC As Short, ByVal eEntityID_Government As Short)
        ' THIS IS THE ROR ALGORITHM

        Dim TP1 As Single
        TP1 = 0

        ' 21 Dec 2004 JWD (C0846) Remove selection code, now set by caller
        ' was:
        ' Setup for ROR discounting driver
        'bRMN = False
        '
        'If XR = 1 Then
        '   iCFCol = 5           ' company net revenues exclusive of capex
        '   bGVCF = False
        'ElseIf XR = 0 Then
        '   iCFCol = 11          ' government revenues exclusive of capex
        '   bGVCF = True
        'End If
        ' End (C0846)

        DRT = 0

        'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        x27400(X7, U5, vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government) 'CALCULATES U5 FOR A GIVEN DISCOUNT RATE

        TP1 = U5
        If TP1 <= 0 Then
            RRZ = 0
        End If
        If TP1 <= 0 Then
            GoTo 27799
        End If

        RU = 101
        TP1 = 0
        Dim XG As Object
        For XG = 1 To 4
            If TP1 = 1 Then
                GoTo 27595
            End If
            RE = 25 * XG
            DRT = RE
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            x27400(X7, U5, vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government)
            'CALCULATES U5 FOR A GIVEN DISCOUNT RATE
            If U5 > 0 Then
                GoTo 27590
            End If
            RU = RE
            rl = RE - 25
            CU = U5
            cl = U5L
            TP1 = 1
            GoTo 27595

27590:      U5L = U5

27595:  Next XG

        If RU = 101 Then
            RRZ = 101
            GoTo 27799
        End If

        Dim iter As Short
        iter = 0 'set iter% to count iterations - iterate max of 500 times
27620:  iter = iter + 1
        RE = rl + ((cl / (cl - CU)) * (RU - rl))
        DRT = RE
        'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        x27400(X7, U5, vpr, DRT, ZD, zp, rUDCF, iDET, bRMN, iEntityID, eEntityID_Company, GVTK, eEntityID_3rdParty, eEntityID_NOC, eEntityID_Government) 'CALCULATES U5 FOR A GIVEN DISCOUNT RATE

        Dim CE As Single
        CE = U5
        If System.Math.Abs(CE) <= 0.01 Or iter > 500 Then
            GoTo 27760
        End If
        If CE < 0 Then
            GoTo 27720
        End If
        rl = RE
        cl = CE
        GoTo 27620 'this is a looping goto!!!!!

27720:  RU = RE
        CU = CE
        GoTo 27620 'this is a looping goto!!!!!

27760:  RRZ = RE

27799:
    End Sub
    Sub x26200(ByRef POT As Object, ByRef CM1 As Object, ByVal PAY As Object, ByRef CM2 As Object) 'THIS CALCULATES PAYOUT - PAY(X) COMES IN AND POT GOES OUT
        Dim i As Short
        Dim iZR As Single
        iZR = Y3 - YR + 1
        ' Added 1/24/00
        'if the discount year is prior to the project start year, then we must not let iZR go below 1.
        If iZR < 1 Then iZR = 1

        POT = 100
        ' Added GDP 14th December 2000 (if the discount date is after the end of life then need to skip)
        If iZR > LG Then GoTo 26320
        CM1 = PAY(iZR)
        If CM1 <= 0 Then GoTo 26230
        POT = iZR
        GoTo 26310

26230:  For i = iZR + 1 To LG
            If POT < 100 Then GoTo 26290
            CM2 = CM1 + PAY(i)
            If CM2 < 0 Or PAY(i) <= 0 Then GoTo 26280
            POT = (i - 1) + System.Math.Abs(CM1 / PAY(i))
26280:      CM1 = CM2
26290:  Next i

        If POT = 100 Then GoTo 26320


26310:  'added 1/24/00
        'adjustment only needed to payout if discount date is on or after project start date
        If Y3 >= YR Then
            POT = POT - ((M3 - 1) / 12) - Y3 + YR
        End If

26320:
    End Sub
End Module