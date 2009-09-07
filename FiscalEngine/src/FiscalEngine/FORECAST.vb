Option Strict Off
Option Explicit On
Imports VB = Microsoft.VisualBasic
Module FORECAST
	'$linesize: 132
	'$title:    'GIANT v6.1 - 1996               FORECAST.BAS'
	'$subtitle: 'Forecasting program for data menu entries'
	' Name:        FORECAST.BAS
	' Function:    Base Data Forecasting Routines
	'---------------------------------------------------------
	' ********************************************************
	' *        COPYRIGHT © 1986-2001 IHS ENERGY GROUP        *
	' *                 ALL RIGHTS RESERVED                  *
	' ********************************************************
	' *  This program file is proprietary information of     *
	' *                  IHS Energy Group                    *
	' *  Unauthorized use for any purpose is prohibited.     *
	' ********************************************************
	'---------------------------------------------------------
	' Modifications:
	' 17 Mar 1995 JWD
	'  -> Removed procedures: GNTList
	'
	' 7 Feb 1996 JWD
	'  -> Replaced include file CTYIN.BAS with CTYIN1.BG.
	'  -> Removed explicit declaration of bDebugging, now in
	'     common.
	'  -> Removed set of bDebugging, now done in MAINEXEC.
	'  -> Add explicit declaration of default storage class
	'     as Single.
	'
	' 14 Feb 1996 JWD
	'  -> Renamed routines with same names as in CNTYFCST.BAS
	'     and CTYFCST2.BAS.
	'  -> Replaced explicit external subroutine declaration
	'     statements with include files FORECAST.BI and
	'     GNTFCST2.BI.
	'
	' 19 Feb 1996 JWD
	'  -> Replaced explicit user type definitions with include
	'     files.
	'  -> Replaced variables True, False with constants
	'     declared in 'trufalse.bc'.
	'  -> Removed initialization of variables True, False
	'     in code.
	'
	' 17 May 1996 MKD
	'  -> Changed LoadFcstEXB().
	'
	' 13 Aug 1996 MKD
	'  -> Changed ForecastData().
	'  -> Changed DTAResetVariables().
	'
	' 11 Jun 1998 JWD
	'  -> Changed DTAConvertData(). (SCO0047)
	'
	' 25 Aug 1998 JWD
	'  -> Changed ForecastData().
	'
	' 13 Jun 2001 JWD
	'  -> Changed DTAConvertData(). (C0332)
	'  -> Changed ForecastData(). (C0332)
	'  -> Changed Read4Gnt(). (C0332)
	'
	' 14 Jun 2001 JWD
	'  -> Changed ForecastData(). (C0333)
	'
	' 5 Jul 2001 JWD
	'  -> Changed Read5Gnt(). (C0341)
	'
	' 10 Jul 2001 JWD
	'  -> Changed Read5Gnt(). (C0348)
	'
	' 23 Jul 2001 JWD
	'  -> Changed ForecastData(). (C0354)
	'  -> Changed ForecastExec(). (C0354)
	'  -> Changed DTAResetVariables(). (C0354)
	'
	' 7 Aug 2001 JWD
	'  -> Changed Read5Gnt(). (C0374)
	'
	' 14 Sep 2001 JWD
	'  -> Changed ForecastData(). (C0443)
	'  -> Changed DTAResetVariables(). (C0443)
	'
	' 17 Sep 2001 JWD
	'  -> Changed ForecastData(). (C0443)
	'
	' 18 Sep 2001 JWD
	'  -> Changed ForecastData(). (C0443)
	'  -> Changed DTAResetVariables(). (C0443)
	'
	' 21 Sep 2001 JWD
	'  -> Changed ForecastData(). (C0454)
	'  -> Changed DTAConvertData(). (C0457)
	'
	' 3 Jan 2002 JWD
	'  -> Changed DTAResetVariables(). (C0485)
	'  -> Changed ForecastExec(). (C0485)
	'
	' 20 Jan 2003 GDP
	'  -> Changed DTAForecastFindRecs.
	'  -> Changed DTAResetVaraibles.
	'  -> Changed ForecastData.
	'  -> Changed LoadFcstEXI.
	'  -> Changed LoadFcstINF.
	'
	' 08 Apr 2003
	'  -> Changed ForecastData.
	'
	' 27 May 2003 JWD
	'  -> Changed ForecastData(). (C0700)
	'  -> Changed ForecastExec(). (C0700)
	'
	' 9 Feb 2004 JWD
	'  -> Changed ForecastData(). (C0780)
	'  -> Changed LoadForecastEXI(). (C0783)
	'
	' 4 Jan 2005 JWD
	'  -> Changed DTAResetVariables(). (C0846)
	'  -> Changed ForecastData(). (C0846)
	'
	' 11 Jan 2005 JWD
	'  -> Changed DTAResetVariables(). (C0846)
	'
	' 22 Mar 2005 JWD
	'  -> Changed DTAResetVariables(). (C0869)
	'
	' 12 May 2005
	'  -> Changed ForecastData(). (C0876)
	'  -> Changed ForecastExec(). (C0876)
	'
	' 13 May 2005
	'  -> Changed DTAForecastFindRecs(). (C0877)
	'  -> Changed DTAResetValues(). (C0877)
	'
	' 16 May 2005
	'  -> Changed ForecastData(). (C0877)
	'  -> Changed ForecastExec(). (C0877)
	'  -> Changed LoadForecastEXI(). (C0877)
	'  -> Changed LoadForecastINF(). (C0877)
	'-----------------------------------------------------------------------
	'$DYNAMIC
	'UPGRADE_NOTE: DefSng A-Z statement was removed. Variables were explicitly declared as type Single. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="92AFD3E3-440D-4D49-A8BF-580D74A8C9F2"'
	'$INCLUDE: 'trufalse.bc'
	
	'UPGRADE_NOTE: FORECAST was upgraded to FORECAST_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Const FORECAST_Renamed As String = "FORECAST"
	'$INCLUDE: 'pdctype.bu'
	'$INCLUDE: 'bdatype.bu'
	'$INCLUDE: 'parmtype.bu'
	'$INCLUDE: 'gnltype.bu'
	'$INCLUDE: 'cpxtype.bu'
	'$INCLUDE: 'saltype.bu'
	'$INCLUDE: 'ebltype.bu'
	'$INCLUDE: 'exbtype.bu'
	'$INCLUDE: 'ttltype.bu'
	
	'$INCLUDE: 'CTYIN1.BG'
	
	'$INCLUDE: 'forecast.bi'
	
	'$INCLUDE: 'gntfcst2.bi'
	
	'$INCLUDE: 'pgm9900.bi'
	
	'$INCLUDE: 'run0100.bi'
	
	'--------------------------------------------------------------------
	'These arrays (and records) are used to load a data file
	'The arrays can be erased after you are finished with them (after
	'  forecasting and data conversion is done
	Dim printnow As Short 'wether listing is printed to disk(0) or printer(-1)
	
	'UPGRADE_WARNING: Arrays in structure GNL may need to be initialized before they can be used. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
	Dim GNL As GNLType
	Dim EBLRecs, BDARecs, PDCRecs, CPXRecs, SALRecs As Short
	Dim INFRecs, DTLRecs As Short '# of misc titles from data file
	
	Dim PDC() As PDCType
	Dim BDA() As ParmType
	Dim INF() As ParmType
	Dim CPX() As CPXType
	Dim EBL() As EBLType
	Dim SAL() As SALType
	Dim DTL() As TTLType
	Dim DNT() As String
	
	'  DIM SHARED ProdYr%, ProdMo%, ProjYr%, ProjMo%, curvelife, proddelay
	'4-22-93  DIM SHARED arraysize%, updatea%, maxlife%
	Dim arraysize, updatea As Short
	'4-22-93  DIM SHARED PrimaryStart$  'start date pf primary product record #1 (PJM,PJY,PDM,PDY)
	
	Const HIVALUE As Short = -32760 'null integer field
	Const NULVALUE As Double = -3.4E+35 'denotes a NOT ENTERED field (NOT 0!)
	Const LIFE As Short = -999
	Const WIN As Short = -998
	Const REV As Short = -997
	Const PAR As Short = -996
	Const LIF As Short = -995
	Const PDY As Short = -993
	Const PDM As Short = -992
	Const PJY As Short = -991
	Const PJM As Short = -990
	Const DSC As Short = -989
	Const CAL As Short = -988
	Const PRD As Short = -987
	
	
	Dim o() As Single
	Dim EL() As String
	Dim dt() As String
	Dim L10() As Single
    Dim CA(,) As Single
	Dim MSPM() As Single
	Dim PMMS() As String
    Dim MYC(,) As Single
	
	Dim TCC As Short
	
	'$subtitle: 'DTAConvertData'
	'$Page:
	'
	' Modifications:
	' 11 Jun 1998 JWD
	'  -> Correct fixup of dates data on GNL, CPX, and SAL
	'     entries by trimming off any blanks prior to
	'     concatenation, extraction, and assignment. (SCO0047)
	'  -> Change concatenation operator in above statements to
	'     use ampersand (&).
	'
	' 13 Jun 2001 JWD
	'  -> Replace explicit occurrences of the detail capital
	'     expenditure category code string with the public
	'     symbol. (C0332)
	'
	' 21 Sep 2001 JWD
	'  -> Change the number of additional entries allocated in
	'     MY3() array. Long life projects with long
	'     abandonment funding schedules were running out of
	'     room causing subscript out of range errors. Extended
	'     to 120 additional entries. (C0457)
	'
	Sub DTAConvertData()
		Dim j As Short
		Dim ptr As Short
		Dim arg As String
		Dim C As String
		Dim DiscMo As Short
		Dim DiscYr As Short
		Dim DisMo As Short
		Dim DisYr As Short
		Dim i As Short
		'--------------------------------------------------------------------
		'this sub takes info from GIANT 5.0 common array and puts it into
		'  old giant format (ie MY1, MY3, EXPLOAN, AMTLOAN, etc.)
		'This is done to allow old routines on execution side to remain
		'  unaltered. They will be re-written in a future version.
		'  Base Data and Production decline curve data is kept in new form
		'  since the GNTFCST.BAS routines process data in the new format.
		
		'PDCrecs%   BDArecs%     DNTRecs%
		MY3T = CPXRecs 'CAPEX records
		EXPLT = EBLRecs 'Exp Based Loans
		AMTLT = SALRecs 'single amount loans
		
		'<<<<<< 21 Sep 2001 JWD (C0457)
		ReDim PN(4)
		ReDim dt(4)
		ReDim EL(2)
		ReDim gn(11)
		ReDim my3(MY3T + 120, 7)
		ReDim EXPLOAN(EXPLT, 8)
		'~~~~~~ was:
		'ReDim PN$(4), dt$(4), EL$(2), gn(11), my3(MY3T + 20, 7), EXPLOAN(EXPLT, 8)
		'>>>>>> End (C0457)
		
		ReDim AMTLOAN(AMTLT, 8)
		
		'--------------------------------------------------------------------
		For i = 1 To 4
			PN(i) = GNL.ttl(i)
		Next i
		
		For i = 1 To 4
			'<<<<<< 11 Jun 1998 JWD Add Trim$ of GNL.dt(i%)
			dt(i) = Right("0" & Trim(GNL.dt(i)), 5)
			'>>>>>>
		Next i
		
		ProjYr = Val(Right(GNL.dt(1), 2))
		ProjMo = Val(Left(GNL.dt(1), 2)) 'project start Mo/Yr
		DisYr = Val(Right(GNL.dt(2), 2))
		DisMo = Val(Left(GNL.dt(2), 2)) 'discovery date Mo/Mr
		ProdYr = Val(Right(GNL.dt(3), 2))
		ProdMo = Val(Left(GNL.dt(3), 2)) 'production start Mo/Yr
		DiscYr = Val(Right(GNL.dt(4), 2))
		DiscMo = Val(Left(GNL.dt(4), 2)) 'discount date Mo/Yr
		
		EL(1) = Left(GNL.og, 1) 'primary product (O/G)
		PPR = 1
		If GNL.og = "GAS" Then
			PPR = 2
		End If
		
		EL(2) = "" 'calendar year input (not used)
		
		C = "FRCBEGMIDEND"
		arg = GNL.dmtd
		SearchCodeString(C, arg, 3, ptr)
		DiscMthd = ptr
		
		gn(1) = GNL.wdepth
		
		If gn(1) = NULVALUE Then
			gn(1) = 0
		End If
		
		gn(2) = GNL.eqvl
		
		For i = 1 To 6
			gn(i + 3) = GNL.pval(i) 'discount factors
			If gn(i + 3) = NULVALUE Then
				gn(i + 3) = 0
			End If
		Next i
		
		'--------------------------------------------------------------------
		'Production decline curve data OK in new form
		'    FOR i% = 1 TO PDCrecs%
		'      PDC(i%).cat, PDC(i%).unit, PDC(i%).mtd
		'      PDC(i%).begprod, PDC(i%).rate, PDC(i%).hypexp, PDC(i%).endprod, PDC(i%).cumprod, PDC(i%).time
		'    NEXT i%
		
		'--------------------------------------------------------------------
		'Base data data OK in new form
		'    FOR i% = 1 TO BDArecs%
		'      BDA(i%).cat, BDA(i%).unit, BDA(i%).dat
		'      BDA(i%).mtd, BDA(i%).parm1, BDA(i%).PARM2, BDA(i%).parm3
		'      BDA(i%).parm4, BDA(i%).parm5, BDA(i%).parm6
		'    NEXT i%
		'--------------------------------------------------------------------
		'Inflation data OK in new form
		'    FOR i% = 1 TO INFrecs%
		'      INF(i%).cat, INF(i%).unit, INF(i%).dat
		'      INF(i%).mtd, INF(i%).parm1, INF(i%).parm2, INF(i%).parm3
		'      INF(i%).parm4, INF(i%).parm5, INF(i%).parm6
		'    NEXT i%
		'--------------------------------------------------------------------
		
		'CAPEX data
		For i = 1 To CPXRecs
			
			'<<<<<< 13 Jun 2001 JWD
			C = CPXCategoryCodesString
			'~~~~~~ was:
			'C$ = "BNSLSERENGEOEDHEDSADHASCDNPDVPPLFFCLTRNEORCP1CP2CP3BALBL2BL3"
			'>>>>>> End 13 Jun 2001
			
			arg = CPX(i).cat
			SearchCodeString(C, arg, 3, ptr)
			my3(i, 1) = ptr
			
			' Added GDP 13th December 2000
			' This should handle 4 digit years if they are passed in by AS$ET
			If Len(CPX(i).dat) > 5 Then
				CPX(i).dat = Right("0" & Trim(CPX(i).dat), 7)
				my3(i, 2) = Val(Left(CPX(i).dat, 2))
				my3(i, 3) = Val(Right(CPX(i).dat, 4))
			Else
				'<<<<<< 11 Jun 1998 JWD Add Trim$ of CPX(i%).dat
				CPX(i).dat = Right("0" & Trim(CPX(i).dat), 5)
				'>>>>>>
				
				my3(i, 2) = Val(Left(CPX(i).dat, 2))
				my3(i, 3) = Val(Right(CPX(i).dat, 2))
				If my3(i, 3) >= 50 Then
					my3(i, 3) = my3(i, 3) + 1900
				ElseIf my3(i, 3) < 50 Then 
					my3(i, 3) = my3(i, 3) + 2000
				End If
			End If
			my3(i, 4) = CPX(i).Tan
			my3(i, 5) = CPX(i).amt
			my3(i, 6) = CPX(i).WIN
			my3(i, 7) = CPX(i).remb
		Next i
		
		For i = 1 To UBound(my3, 1)
			For j = 1 To UBound(my3, 2)
				If my3(i, j) = NULVALUE Then
					my3(i, j) = 0
				End If
			Next j
		Next i
		
		
		'--------------------------------------------------------------------
		'EXPENDITURE BASED LOANS
		For i = 1 To EBLRecs
			
			'<<<<<< 13 Jun 2001 JWD
			C = "CPXEXPDEV" & CPXCategoryCodesString
			'~~~~~~ was:
			'C$ = "CPXEXPDEVBNSLSERENGEOEDHEDSADHASCDNPDVPPLFFCLTRNEORCP1CP2CP3BALBL2BL3"
			'>>>>>> End 13 Jun 2001
			
			arg = EBL(i).cat
			SearchCodeString(C, arg, 3, ptr)
			EXPLOAN(i, 1) = ptr
			C = "BEGDISPRDLIF"
			arg = EBL(i).endt
			SearchCodeString(C, arg, 3, ptr)
			EXPLOAN(i, 2) = ptr
			EXPLOAN(i, 3) = EBL(i).fin
			EXPLOAN(i, 4) = EBL(i).Del
			C = "TOTPRI"
			arg = EBL(i).mtd
			SearchCodeString(C, arg, 3, ptr)
			EXPLOAN(i, 5) = ptr
			EXPLOAN(i, 6) = EBL(i).int_Renamed
			EXPLOAN(i, 7) = EBL(i).mth
			C = "NO YES"
			arg = EBL(i).acc
			SearchCodeString(C, arg, 3, ptr)
			EXPLOAN(i, 8) = ptr
		Next i
		For i = 1 To UBound(EXPLOAN, 1)
			For j = 1 To UBound(EXPLOAN, 2)
				If EXPLOAN(i, j) = NULVALUE Then
					EXPLOAN(i, j) = 0
				End If
			Next j
		Next i
		
		'--------------------------------------------------------------------
		'SINGLE AMOUNT LOANS
		For i = 1 To SALRecs
			
			'<<<<<< 11 Jun 1998 JWD Add Trim$ of SAL(i%).dat
			SAL(i).dat = Right("0" & Trim(SAL(i).dat), 5)
			'>>>>>>
			
			AMTLOAN(i, 1) = Val(Left(SAL(i).dat, 2))
			AMTLOAN(i, 2) = Val(Right(SAL(i).dat, 2))
			AMTLOAN(i, 3) = SAL(i).amt
			AMTLOAN(i, 4) = SAL(i).Del
			C = "TOTPRI"
			arg = SAL(i).mtd
			SearchCodeString(C, arg, 3, ptr)
			AMTLOAN(i, 5) = ptr
			AMTLOAN(i, 6) = SAL(i).int_Renamed
			AMTLOAN(i, 7) = SAL(i).mth
			C = "NO YES"
			arg = SAL(i).acc
			SearchCodeString(C, arg, 3, ptr)
			AMTLOAN(i, 8) = ptr
		Next i
		For i = 1 To UBound(AMTLOAN, 1)
			For j = 1 To UBound(AMTLOAN, 2)
				If AMTLOAN(i, j) = NULVALUE Then
					AMTLOAN(i, j) = 0
				End If
			Next j
		Next i
		
		'--------------------------------------------------------------------
		
		'save OXY specific data for OXY Paradox database
		DsctMeth = GNL.dmtd
		Primary = GNL.og
		WatDepth = GNL.wdepth
		If WatDepth = -3.4E+35 Then 'denotes a NOT ENTERED field (NOT 0!)
			WatDepth = 0
		End If
		GTitl1 = LTrim(RTrim(GNL.ttl(1)))
		GTitl2 = LTrim(RTrim(GNL.ttl(2)))
		GTitl3 = LTrim(RTrim(GNL.ttl(3)))
		GTitl4 = LTrim(RTrim(GNL.ttl(4)))
		
	End Sub
	
	' $SubTitle:'DTAForecastBaseEXT - Driver sub for EXTERNAL TABLE records'
	' $Page
	Sub DTAForecastBaseEXT(ByRef EXTData() As EXBType, ByRef Datacol() As Single, ByRef curveoffset As Single, ByRef curvelife As Single, ByRef startyear As Short)
		Dim startmonth As Short
		'--------------------------------------------------------------------
		'This routine is called by ForecastDispatch.
		'  parameters: (bdadata() AS ParmType, datacol!(), curveoffset, curvelife)
		'  function: loops through bdadata(), calls DTAForecastStepEXT,
		'       and returns datacol()
17300: 
		startyear = EXTData(1).dat 'ie. 1991
		startmonth = 1
		DTAForecastStepEXT(EXTData, startyear, startmonth, Datacol, curveoffset, curvelife)
		
	End Sub
	
	' $SubTitle:'ForecastDispatch - call subs to forecast a given item'
	' $Page
	Sub DTAForecastDispatch(ByRef cat As String, ByRef Datacol() As Single, ByRef updatea As Short)
		Dim infcat As String
		Dim infsearch As Short
		Dim startyear As Short
		Dim FileName As String
		Dim i As Short
		Dim curveoffset As Single
		Dim unit As String
		Dim arg As String
		Dim recurseflag As Short
		Dim found As Short
		'--------------------------------------------------------------------
		'  parameters: category$, datacol!(), UpdateA%
		'  parameters in: category$
		'  parameters out: category$ (unit of measure for this item), datacol!(),
		'                  UpdateA%
		'  function:    CALLS DTAForecastFindRecs
		'               dims datarecord array
		'               calls LoadFcstBaseData or LoadFcstDCL  (depending on found%)
		'               calls forecastbase or forecastdcl  (depending on found%)
		'               updatea% signals wether the variable was forecasted
		'  calls: DTAForecastFindRecs, LoadFcstBaseData, LoadFcstDCL
		'         forecastbase, forecastdcl
		'  returns: datacol!(), UpdateA%
		'---------------------------------------------------------
		' Modifications:
		' 20 Feb 1996 JWD
		'           Commented out curvelife%, duplicate definition
		'        and is otherwise not referenced.
		'---------------------------------------------------------
		Dim bEXTFile As Short
		'---------------------------------------------------------
7200: 
		
		
		'~~~~CurveLife% = 0
		found = False
		'if we are here after being called by forecastdispatch (to find
		'  inflation recs) then we send -999 to forecastfindrecs SUB as
		'  a signal to look for the category in INF instead of PDC and BDA
		If updatea = -999 Then
			recurseflag = True
			found = -999
		End If
		arg = cat
		
		DTAForecastFindRecs(arg, found) 'see if the item is in BDA() or PDC()
		'fln% = FreeFile
		'Open "FCST.LOG" For Append As #fln%
		'   Print #fln%, "  in ForecastDispatch cat$ =  "; cat$; "  found = "; found%
		'Close #fln%

        Dim extbdadata(1) As EXBType
        Dim BDAData(1) As ParmType
        Dim extdatacol(60) As Single

		unit = arg 'DTAForecastFindRecs returns UNITS in arg$ if found
		If found Then
			updatea = True
			Select Case found
				Case 1 'found in PDC()
					Dim pdcdata(1) As PDCType
					LoadFcstDCL(cat, pdcdata)
					ForecastDCL(pdcdata, Datacol, curveoffset, curvelife)
					PrimaryStart = "PDM"
					'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
					System.Array.Clear(pdcdata, 0, pdcdata.Length)
				Case 2 'found in BDA

                    ReDim BDAData(1)
					LoadFcstBaseData(cat, BDAData)
11750: 
					'fln% = FreeFile
					'Open "FCST.LOG" For Append As #fln%
					'   Print #fln%, "  ForecastDispatch bdadata loaded"
					'   For i% = 1 To UBound(bdadata)
					'      Print #fln%, "  bdadata("; i%; ").cat  = "; bdadata(i%).cat
					'      Print #fln%, "  bdadata("; i%; ").unit = "; bdadata(i%).unit
					'      Print #fln%, "  bdadata("; i%; ").dat  = "; bdadata(i%).dat
					'   Next i%
					'Close #fln%
					
					PrimaryStart = BDAData(1).dat
					'fln% = FreeFile
					'Open "FCST.LOG" For Append As #fln%
					'   Print #fln%, "  PrimaryStart$  = "; PrimaryStart$
					'Close #fln%
					
					
					'now bdadata is loaded. Search parm 1 recs to see if any
					'  ext file names are entered. If so,  redim bdadata to 1
					'  call loadfcstexb to fill out elements and return
					bEXTFile = False 'flag for ext table (T/F)
					For i = 1 To UBound(BDAData)
						If EXTNameEntered(BDAData(i).parm1) Then
							FileName = LTrim(RTrim(BDAData(i).parm1))
							'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
							System.Array.Clear(BDAData, 0, BDAData.Length)
                            ReDim extbdadata(1)

							LoadFcstEXB(cat, extbdadata, FileName)
							bEXTFile = True
							Exit For
						End If
					Next i
11752: 
					
					If bEXTFile Then
						If UBound(extbdadata) > 0 Then
							unit = extbdadata(1).unit 'return Units to ForecastExec
                            ReDim extdatacol(60)
							DTAForecastBaseEXT(extbdadata, extdatacol, curveoffset, curvelife, startyear)
							DTALoadExtDataCol(extbdadata(1).dat, Datacol, extdatacol)
							'DATACOL!() NOW CONTAINS THE FORECAST OF CAT$
							'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
							System.Array.Clear(extdatacol, 0, extdatacol.Length)
							'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
							System.Array.Clear(extbdadata, 0, extbdadata.Length)
						End If
					Else
						'fln% = FreeFile
						'Open "FCST.LOG" For Append As #fln%
						'   Print #fln%, "  call DTAForecastBase curveoffset = "; CurveOffset; "  CurveLife = "; CurveLife
						'Close #fln%
						
						DTAForecastBase(BDAData, Datacol, curveoffset, curvelife)
						'fln% = FreeFile
						'Open "FCST.LOG" For Append As #fln%
						'   Print #fln%, "  back from DTAForecastBase curveoffset = "; CurveOffset; "  CurveLife = "; CurveLife
						'   For q% = 1 To UBound(datacol!)
						'      Print #fln%, "  datacol!("; q%; ") = "; datacol!(q%)
						'   Next q%
						'Close #fln%
						
						
						
					End If
					If Not recurseflag Then 'dont inflate inflation!
						'now call ForecastDispatch (recursively) to see if there
						'  are any INFLATION records for this category - if so
						'  when we arrive back here, they will be forecasted
						infsearch = -999
						infcat = cat
						Dim infcol(arraysize) As Single
						DTAForecastDispatch(infcat, infcol, infsearch)
						If infsearch = True And infsearch <> HIVALUE Then
							'if OPC or GPC are inflated - save values in inflate() in common
							SaveInflation(cat, infcol)
							'there is a forecast for inflation for cat$ - it needs to be
							'  applied to datacol!()
							If UBound(infcol) <> UBound(Datacol) Then
								ReDim Preserve infcol(UBound(Datacol))
							End If
                            ApplyInflationDta(Datacol, infcol)
						End If
						'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
						System.Array.Clear(infcol, 0, infcol.Length)
						recurseflag = False
					End If
					'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
					System.Array.Clear(BDAData, 0, BDAData.Length)
					
				Case 5 'found in INF
					
					ReDim BDAData(1)
					LoadFcstINF(cat, BDAData)
					'now bdadata is loaded. Search parm 1 recs to see if any
					'  ext file names are entered. If so,  redim bdadata to 1
					'  call loadfcstexb to fill out elements and return
					bEXTFile = False 'flag for ext table (T/F)
					For i = 1 To UBound(BDAData)
						If EXTNameEntered(BDAData(i).parm1) Then
							FileName = LTrim(RTrim(BDAData(i).parm1))
							'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
							System.Array.Clear(BDAData, 0, BDAData.Length)
							ReDim extbdadata(1)
							LoadFcstEXI(cat, extbdadata, FileName)
							bEXTFile = True
							Exit For
						End If
					Next i
					If bEXTFile Then
						If UBound(extbdadata) > 0 Then
							ReDim extdatacol(60)
							DTAForecastBaseEXT(extbdadata, extdatacol, curveoffset, curvelife, startyear)
							DTALoadExtDataCol(extbdadata(1).dat, Datacol, extdatacol)
							'DATACOL!() NOW CONTAINS THE FORECAST OF CAT$
							'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
							System.Array.Clear(extdatacol, 0, extdatacol.Length)
							'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
							System.Array.Clear(extbdadata, 0, extbdadata.Length)
						End If
					Else
						DTAForecastBase(BDAData, Datacol, curveoffset, curvelife)
					End If
					'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
					System.Array.Clear(BDAData, 0, BDAData.Length)
			End Select
		Else 'NOT found
			updatea = HIVALUE
		End If
		cat = unit 'cat$ returns the unit of measure for the item
		
11762: 
		
	End Sub
	
	' $SubTitle:'ForeCastFindRecs'
	' $Page
	' Modifications
	' GDP 20 Jan 2003
	' Added additional volumes
	'
	' 13 May 2005 JWD
	'  -> Add new operating expense categories OX6-O20.
	'     (C0877)
	'
	Sub DTAForecastFindRecs(ByRef cat As String, ByRef found As Short)
		Dim unit As String
		Dim arg As String
		'--------------------------------------------------------------------
		'  parameters in: cat$ (ie. OIL, GAS, etc.)
		'  parameters out: found% [1 if PDC, 2 if BDA, 3 if both, 0 if not found]
		'                  cat$ - the unit of measure of the category
		'  function: examines PDC and BDA looking for the given category.
		'               returns 1 if in PDC, 2 if in BDA, 0 if not in found
		'               returns CAT$ as the unit of measure for the item
		'---------------------------------------------------------
		Dim i As Short
		Dim bINF As Short
		'---------------------------------------------------------
4300: 
		'see if we are here to look in INF array for inflation records
		If found = -999 Then
			bINF = True 'we are here to find inflation recs to forecast
		End If
		found = 0
		
		If bINF Then
			arg = cat
			'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            SearchINF(arg, found, unit)
			If Not found Then
				Select Case arg
					' GDP 20 Jan 2003
					' Added OP3-OP0 to case statement
					Case "OPC", "GPC", "OP1", "OP2", "OP3", "OP4", "OP5", "OP6", "OP7", "OP8", "OP9", "OP0" 'search for "PRC" in INF to use instead
						arg = "PRC"
						'Case "OX1", "OX2", "OX3", "OX4", "OX5"  'search for "OPX" in INF to use instead
						' 13 May 2005 JWD (C0877) Add new codes OX6-O20
					Case "OX1", "OX2", "OX3", "OX4", "OX5", "OX6", "OX7", "OX8", "OX9", "OX0", "O11", "O12", "O13", "O14", "O15", "O16", "O17", "O18", "O19", "O20"
						' End (C0877)
						arg = "OPX"
				End Select
				'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
                SearchINF(arg, found, unit)
			End If
		Else 'not here for inflation - looking in PDC & BDA
			For i = 1 To BDARecs
				If BDA(i).cat = cat Then
					found = 2
					unit = BDA(i).unit
					Exit For
				End If
			Next i
			For i = 1 To PDCRecs
				If PDC(i).cat = cat Then
					' GDP 20 Jan 2003
					' Added code for setting unit for additional volumes
					If cat = "GAS" Or cat = "OV2" Or cat = "OV4" Or cat = "OV6" Or cat = "OV8" Or cat = "OV0" Then
						unit = "BCF"
					ElseIf cat = "OIL" Or cat = "OV1" Or cat = "OV3" Or cat = "OV5" Or cat = "OV7" Or cat = "OV9" Then 
						unit = "MMB"
					End If
					found = found + 1
					Exit For
				End If
			Next i
		End If 'end of "IF bINF then" test
		cat = unit
		
		Exit Sub
		
		'--------------------------------------------------------------------

		
    End Sub
    Sub SearchINF(ByRef arg As String, ByRef found As Short, ByRef unit As String) 'gosub to search INF() for a category
        Dim i As Short
        For i = 1 To INFRecs
            If INF(i).Cat = arg Then
                found = 5
                unit = INF(i).unit
                Exit For
            End If
        Next i
    End Sub

    ' $SubTitle:'DTAForecastStepEXT - Forecast Mtds 1-6 - External Tables'
    ' $Page
    Sub DTAForecastStepEXT(ByRef EXTData() As EXBType, ByRef startyear As Short, ByRef startmonth As Short, ByRef Datacol() As Single, ByRef curveoffset As Single, ByRef curvelife As Single)
        Dim x As Single
        Dim zeroper As Single
        Dim qdecl As Single
        Dim avar As Single
        Dim delay As Single
        Dim qi As Single
        Dim dur As Single
        Dim d As Single
        Dim EscalRate As Single
        Dim i As Short
        Dim begamt As Single
        Dim qf As Single
        Dim p1 As String
        Dim ProjLife As Single
        Dim Continuous As Short
        Dim periods As Single
        Dim startperiod As Single
        Dim j As Short
        Dim e As Single
        '--------------------------------------------------------------------
29250:
        e = 2.71828
        Dim wrkcol(100) As Single

        For j = 1 To UBound(EXTData)
            'figure period in which this data record starts....
            If j = 1 Then 'only calc these values for the first record
                startperiod = 1
                curveoffset = startperiod - 1 '# of project periods prior to start of this curve
            ElseIf Continuous Then
                startperiod = startperiod + Int(periods)
            Else 'step method AND j% > 1
                startperiod = startperiod + UBound(wrkcol)
            End If
            ProjLife = LG

            Select Case EXTData(j).mtd
                Case 1 'constant amount, # years
                    Continuous = False
                    periods = EXTData(j).parm2
                    ReDim wrkcol(periods)
                    p1 = LTrim(RTrim(Str(EXTData(j).parm1)))
                    If p1 = "" Then
                        begamt = qf
                    Else
                        begamt = EXTData(j).parm1
                    End If
                    For i = 1 To periods
                        wrkcol(i) = begamt
                    Next i
                    qf = wrkcol(UBound(wrkcol))
                Case 2 'fixed amounts 1-6
29251:
                    Continuous = False
                    periods = 6
                    Dim DUM(periods) As Single
                    DUM(1) = EXTData(j).parm1
                    DUM(2) = EXTData(j).parm2
                    DUM(3) = EXTData(j).parm3 : DUM(4) = EXTData(j).parm4
                    DUM(5) = EXTData(j).parm5 : DUM(6) = EXTData(j).parm6
                    For i = 6 To 1 Step -1
                        If DUM(i) = NULVALUE Then
                            periods = periods - 1
                        Else
                            Exit For
                        End If
                    Next i

                    For i = 1 To periods
                        If DUM(i) = NULVALUE Then
                            DUM(i) = 0
                        End If
                    Next i
                    ReDim wrkcol(periods)
                    For i = 1 To periods
                        wrkcol(i) = DUM(i)
                    Next i
                    'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
                    System.Array.Clear(DUM, 0, DUM.Length)
                    qf = wrkcol(UBound(wrkcol))
                Case 3, 5 'initial amount, esc(mtd 3) / decl(mtd5) rate, # years
29252:              Continuous = False
                    periods = EXTData(j).parm3
                    ReDim wrkcol(periods)
                    p1 = LTrim(RTrim(Str(EXTData(j).parm1)))
                    If p1 = "" Then
                        p1 = LTrim(Str(qf))
                    End If
                    wrkcol(1) = Val(p1)
                    EscalRate = EXTData(j).parm2
                    If EXTData(j).mtd = 5 Then
                        EscalRate = EscalRate * -1
                    End If
                    Call DTAStepProj(EscalRate, wrkcol)
                    qf = wrkcol(UBound(wrkcol))
                Case 4, 6 'escalation rate, # years
29260:              Continuous = False
                    periods = EXTData(j).parm2
                    ReDim wrkcol(periods)
                    If EXTData(j).mtd = 6 Then
                        EscalRate = EXTData(j).parm1 * -1
                    Else
                        EscalRate = EXTData(j).parm1
                    End If
                    wrkcol(1) = qf * (1 + EscalRate * 0.01)
                    If j > 1 Then
                        DTAStepProj(EscalRate, wrkcol)
                    End If
                    qf = wrkcol(UBound(wrkcol))
                Case 7, 8, 9, 0 'continuous methods
29270:              Continuous = True
                    If EXTData(j).mtd = 7 Or EXTData(j).mtd = 9 Then
                        d = (EXTData(j).parm2) / 100 'decline rate for this record
                        dur = EXTData(j).parm3 'duration of this record
                        p1 = LTrim(RTrim(Str(EXTData(j).parm1)))
                        If p1 = "" Then
                            p1 = LTrim(Str(qf))
                        End If
                        qi = Val(p1)
                    ElseIf EXTData(j).mtd = 8 Or EXTData(j).mtd = 0 Then
                        d = EXTData(j).parm1 / 100 'decline rate for this record
                        dur = EXTData(j).parm2 'duration of this record
                        qi = qf
                    End If
                    If j = 1 Then
                        delay = (startmonth - 1) / 12
                    Else
                        delay = delay + periods
                        Do While delay >= 1
                            delay = delay - 1
                        Loop
                    End If
                    periods = dur
                    If EXTData(j).mtd = 7 Or EXTData(j).mtd = 8 Then
                        d = -1 * d
                    End If
                    'avar = nominal decline factor [avar = -ln(1-d)]
                    avar = -1 * System.Math.Log(1 - d)
                    qf = qi * e ^ (-avar * periods)
                    ReDim wrkcol(Int(periods + 1))
                    qdecl = (qi / avar) * (1 - (e ^ (-1 * avar * periods))) 'QDECL = cum production
                    DTAExponentialDecl(qf, delay, qi, qdecl, wrkcol)
            End Select
29280:
            curvelife = curvelife + periods 'track total duration of this category
            zeroper = startperiod - 1
            If startmonth <> 1 And Not Continuous Then
                delay = (startmonth - 1) / 12.0!
                For i = 1 To UBound(wrkcol)
                    If zeroper > 0 And zeroper + i + 1 <= UBound(Datacol) Then
                        x = wrkcol(i) * delay 'X = amount delayed to next year
                        Datacol(zeroper + i) = Datacol(zeroper + i) + (wrkcol(i) - x)
                        Datacol(zeroper + i + 1) = Datacol(zeroper + i + 1) + x
                    End If
                Next i
            Else 'startmonth = 1 or step method (includes external tables
                For i = 1 To UBound(wrkcol)
                    If zeroper + i > 0 And zeroper + i <= UBound(Datacol) Then
                        Datacol(zeroper + i) = Datacol(zeroper + i) + wrkcol(i)
                    End If
                Next i
            End If
            zeroper = zeroper + UBound(wrkcol)
        Next j
        'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        System.Array.Clear(wrkcol, 0, wrkcol.Length)
        ReDim Preserve Datacol(zeroper)
    End Sub

    ' $SubTitle:'DTALoadExtDataCol'
    ' $Page
    Sub DTALoadExtDataCol(ByRef year1 As Short, ByRef Datacol() As Single, ByRef extdatacol() As Single)
        Dim i As Short
        Dim pers As Short
        Dim ptr As Short
        '--------------------------------------------------------------------
        'Extdatacol!() has values of the forecast. The first element in
        '  extdatacol is the calendar year of the first record if the category
        '  as entered in the external table (ie. 1991).
        'We need to shift extdatacol!() into the proper elements of datacol!().
        '  (datacol!()'s first element is projyr%)
16175:
        'YR = project start year  - ie. 1991 (integer) (in COMMON)
        'year1% = starting year of the external forecast
        'pers% = number of years data to transfer to datacol!()

        ReDim Datacol(LG)
        If year1 < YR Then 'ext forecast starts before project start year
            ptr = YR - year1 + 1 'first element in extdatacol() to copy
            pers = UBound(extdatacol) - ptr + 1
            If pers > LG Then
                pers = LG
            End If
            For i = 1 To pers
                Datacol(i) = extdatacol(i - 1 + ptr)
            Next i
        ElseIf year1 > YR Then  'ext forecast starts after project start year
            ptr = year1 - YR '# elements in datacol() to skip at front
            pers = LG - ptr '    UBOUND(extdatacol!) + ptr% + 1
            If pers > UBound(extdatacol) Then
                pers = UBound(extdatacol)
            End If
            For i = 1 To pers
                Datacol(i + ptr) = extdatacol(i)
            Next i
            '    ptr% = YR - year1% + 1     'first element in datacol() to copy into
            '    pers% = UBOUND(extdatacol!) + ptr% + 1
            '    IF pers% > LG - ptr% THEN
            '      pers% = LG - ptr%
            '    END IF
            '    FOR i% = 1 TO pers%
            '      datacol!(i%) = extdatacol!(i% - 1 + ptr%)
            '    NEXT i%
        ElseIf year1 = YR Then  'ext forecast starts on project start year
            pers = LG
            If pers > UBound(extdatacol) Then
                pers = UBound(extdatacol)
            End If
            For i = 1 To pers
                Datacol(i) = extdatacol(i)
            Next i
        End If

    End Sub

    '$subtitle: 'DTAResetVariables'
    '$Page:
    '
    ' Modifications:
    ' 13 Aug 1996 MKD
    '  -> See note this date for ForecastData()
    '
    ' 23 Jul 2001 JWD
    '  -> Change to set upper bound of arrays to Maxlife
    '     rather than LG. (C0354)
    '
    ' 14 Sep 2001 JWD
    '  -> Restore the consolidated gross production values
    '     to the public array GrossProduction(). (C0443)
    '
    ' 18 Sep 2001 JWD
    '  -> Restore the consolidated gross revenue values
    '     to the public array GrossRevenue(). (C0443)
    '
    ' 3 Jan 2002 JWD
    '  -> Change upper limit of loop initializing DFL()
    '     to set to upper bound of array rather than
    '     through LG. (C0485)
    '
    ' 20 Jan 2003 GDP
    '  -> Changes for additional volumes.
    '
    ' 4 Jan 2005 JWD
    '  -> Add computation of effective interests for
    '     calculations of consolidated cash flows for
    '     3rd party and NOC entities. (C0846)
    '
    ' 11 Jan 2005 JWD
    '  -> Changed variable from FINANCE() to TOTFINANCE().
    '     FINANCE() not the correct symbol. (C0846)
    '
    ' 22 Mar 2005 JWD
    '  -> Add allocation of EffIntsX() array before copy and
    '     computation of effective interests. (C0869)
    '
    ' 13 May 2005 JWD
    '  -> Add new operating expense categories OX6-O20.
    '     (C0877)
    '
    Sub DTAResetVariables()
        Dim i As Short
        Dim z As Single
        Dim FirstPrice As Single
        Dim Year1st As Single
        Dim Setit As Single
        Dim w As Single
        Dim x As Single
        Dim LIG As Single
        '--------------------------------------------------------------------
        'Replaced GOSUB 46000
        'THIS SUBROUTINE SETS CONSOLIDATED VARIABLES BACK FOR RUN

        ' 13 May 2005 JWD (C0877) Loop index
        Dim jj As Integer

3712:
        ' Added GDP 27/8/99
        g_bPTCons = True

        mo = L10(1) : YR = L10(2) : M1 = L10(3) : Y1 = L10(4)
        M3 = L10(10) : Y3 = L10(11)
        LG = L10(8) - L10(2) + 1
        LFI = L10(9) - L10(7)
        LIG = L10(9) - L10(6) : LGI = LIG + ((mo - 1) / 12)
        '5-18-92 -------------------------------
        LFX = Int(LFI)
        If LFX < LFI Then
            LFX = LFX + 1
        End If
        '5-18-92 -------------------------------

        '<<<<<< 14 Sep 2001 JWD (C0443)
        ' GDP 20 JAN 2003
        ' gc_nAMAXVOL now represents the number of volume streams
        ReDim GrossProduction(gc_nMAXLIFE, gc_nAMAXVOL)
        '>>>>>> End (C0443)

        '<<<<<< 18 Sep 2001 JWD (C0443)
        ReDim GrossRevenue(gc_nMAXLIFE, gc_nAMAXVOL)
        '>>>>>> End (C0443)

        '<<<<<<< 23 Jul 2001 JWD (C0354)
        ReDim Inflate(gc_nMAXLIFE, 2)
        ReDim DFL(gc_nMAXLIFE)
        ' Added GDP 13/9/99 - ReDim to Life new variable for repayment
        ReDim TOTREPAY(gc_nMAXLIFE)
        ' Added GDP 14/11/2000 - ReDim to Life new variable for repayment
        ReDim TOTFINANCE(gc_nMAXLIFE)
        ' GDP 20 JAN 2003
        ' A array extended for extra volumes
        ReDim A(gc_nMAXLIFE, gc_nASIZE)
        ReDim OPEX(gc_nMAXLIFE)
        ReDim gn(9)
        ReDim PN(4)
        ReDim my3(TCC + 20, 7)
        '~~~~~~ was:
        ''8/13/96 we are now storing Inflate(x,y) and DFL(x) in the CA array.
        'ReDim Inflate(LG, 2)
        'ReDim DFL(LG)
        '' Added GDP 13/9/99 - ReDim to Life new variable for repayment
        'ReDim TOTREPAY(LG)
        '' Added GDP 14/11/2000 - ReDim to Life new variable for repayment
        'ReDim TOTFINANCE(LG)
        '
        'ReDim A(LG, 20), OPEX(LG), gn(9), PN$(4), my3(TCC + 20, 7)
        '>>>>>> End (C0354)

        For x = 1 To LG
            ' GDP 20 JAN 2003
            ' Changed to use a constant for A array size
            For w = 1 To gc_nASIZE
                A(x, w) = CA(x, w)
            Next w


            ' revenues by product are stored instead of the prices in CA(x,7-10)
            ' convert them back to prices in A(x,7-10)
            ' GDP 20 JAN 2003
            ' Use constants for loop bounds and offset
            For w = gc_nAMINVOL To gc_nAMAXVOL
                If CA(x, w) = 0 Then
                    A(x, w + gc_nAPRICEOFFSET) = 0
                Else
                    A(x, w + gc_nAPRICEOFFSET) = CA(x, w + gc_nAPRICEOFFSET) / CA(x, w)
                End If

                If gna_ACX(x, w, 0) = 0 Then
                    gna_ACX(x, w + gc_nAPRICEOFFSET, 0) = 0
                Else
                    gna_ACX(x, w + gc_nAPRICEOFFSET, 0) = gna_ACX(x, w + gc_nAPRICEOFFSET, 0) / gna_ACX(x, w, 0)
                End If
                If gna_ACX(x, w, 1) = 0 Then
                    gna_ACX(x, w + gc_nAPRICEOFFSET, 1) = 0
                Else
                    gna_ACX(x, w + gc_nAPRICEOFFSET, 1) = gna_ACX(x, w + gc_nAPRICEOFFSET, 1) / gna_ACX(x, w, 1)
                End If
                If gna_ACX(x, w, 2) = 0 Then
                    gna_ACX(x, w + gc_nAPRICEOFFSET, 2) = 0
                Else
                    gna_ACX(x, w + gc_nAPRICEOFFSET, 2) = gna_ACX(x, w + gc_nAPRICEOFFSET, 2) / gna_ACX(x, w, 2)
                End If
                If gna_ACX(x, w, 3) = 0 Then
                    gna_ACX(x, w + gc_nAPRICEOFFSET, 3) = 0
                Else
                    gna_ACX(x, w + gc_nAPRICEOFFSET, 3) = gna_ACX(x, w + gc_nAPRICEOFFSET, 3) / gna_ACX(x, w, 3)
                End If
            Next w

            ' GDP 20 JAN 2003
            ' Use constants instead of numeric values
            'OPEX(x) = A(x, gc_nAOX1) + A(x, gc_nAOX2) + A(x, gc_nAOX3) + A(x, gc_nAOX4) + A(x, gc_nAOX5)
            ' 13 May 2005 JWD (C0877) Include additional categories in total
            For jj = gc_nAOX1 To gc_nAO20
                OPEX(x) = OPEX(x) + A(x, jj)
            Next jj
            ' End (C0877)

            A(x, gc_nAWIN) = 100

            gna_ACX(x, gc_nAWIN, 0) = 100
            gna_ACX(x, gc_nAWIN, 1) = 100
            gna_ACX(x, gc_nAWIN, 2) = 100
            gna_ACX(x, gc_nAWIN, 3) = 100


            '8-13-96 Put values back in Inflate() and DFL() from Pre-tax Consolidation
            Inflate(x, 1) = CA(x, gc_nAINF1)
            Inflate(x, 2) = CA(x, gc_nAINF2)
            DFL(x) = CA(x, gc_nADFL)
            TOTREPAY(x) = CA(x, gc_nAREPAY)
            ' Added GDP 14/11/2000 - Consolidation of Financing
            TOTFINANCE(x) = CA(x, gc_nAFIN)

            '<<<<<< 14 Sep 2001 JWD (C0443)
            ' Restore the consolidated gross production
            GrossProduction(x, 1) = CA(x, gc_nAGOIL)
            GrossProduction(x, 2) = CA(x, gc_nAGGAS)
            GrossProduction(x, 3) = CA(x, gc_nAGOV1)
            GrossProduction(x, 4) = CA(x, gc_nAGOV2)
            GrossProduction(x, 5) = CA(x, gc_nAGOV3)
            GrossProduction(x, 6) = CA(x, gc_nAGOV4)
            GrossProduction(x, 7) = CA(x, gc_nAGOV5)
            GrossProduction(x, 8) = CA(x, gc_nAGOV6)
            GrossProduction(x, 9) = CA(x, gc_nAGOV7)
            GrossProduction(x, 10) = CA(x, gc_nAGOV8)
            GrossProduction(x, 11) = CA(x, gc_nAGOV9)
            GrossProduction(x, 12) = CA(x, gc_nAGOV0)
            '>>>>>> End (C0443)

            '<<<<<< 18 Sep 2001 JWD (C0443)
            ' Restore the consolidated gross revenue
            GrossRevenue(x, 1) = CA(x, gc_nAGOILREV)
            GrossRevenue(x, 2) = CA(x, gc_nAGGASREV)
            GrossRevenue(x, 3) = CA(x, gc_nAGOV1REV)
            GrossRevenue(x, 4) = CA(x, gc_nAGOV2REV)
            GrossRevenue(x, 5) = CA(x, gc_nAGOV3REV)
            GrossRevenue(x, 6) = CA(x, gc_nAGOV4REV)
            GrossRevenue(x, 7) = CA(x, gc_nAGOV5REV)
            GrossRevenue(x, 8) = CA(x, gc_nAGOV6REV)
            GrossRevenue(x, 9) = CA(x, gc_nAGOV7REV)
            GrossRevenue(x, 10) = CA(x, gc_nAGOV8REV)
            GrossRevenue(x, 11) = CA(x, gc_nAGOV9REV)
            GrossRevenue(x, 12) = CA(x, gc_nAGOV0REV)
            '>>>>>> End (C0443)
        Next x


        '''        ' 4 Jan 2005 JWD (C0846)
        '''        ReDim EffInts(0 To gc_nMAXLIFE, 0 To gc_nEffInts_PFN)
        '''
        '''        For x = 1 To LG
        '''            If CA(x, gc_nCA_GRSREV) <> 0 Then
        '''                ' Compute the effective working interest for the company based on revenue
        '''                EffInts(x, gc_nEffInts_WIN) = CA(x, gc_nCA_CMPREV) / (CA(x, gc_nCA_GRSREV) - CA(x, gc_nCA_NOCREV))
        '''                ' Compute the effective participation interest for the NOC based on revenue
        '''                EffInts(x, gc_nEffInts_PAR) = CA(x, gc_nCA_NOCREV) / CA(x, gc_nCA_GRSREV)
        '''            Else
        '''                EffInts(x, gc_nEffInts_WIN) = 1
        '''                EffInts(x, gc_nEffInts_PAR) = 0
        '''            End If
        '''
        '''            If OPEX(x) <> 0 Then
        '''                ' Compute the effective working interest for the company based on operating expense
        '''                EffInts(x, gc_nEffInts_WOX) = OPEX(x) / (OPEX(x) + CA(x, gc_nCA_3DPOPX))
        '''                ' Compute the effective operating expense participation interest for the NOC
        '''                EffInts(x, gc_nEffInts_POX) = CA(x, gc_nCA_NOCOPX) / (OPEX(x) + CA(x, gc_nCA_3DPOPX) + CA(x, gc_nCA_NOCOPX))
        '''            Else
        '''                EffInts(x, gc_nEffInts_WOX) = 1
        '''                EffInts(x, gc_nEffInts_POX) = 0
        '''            End If
        '''
        '''            If TOTFINANCE(x) <> 0 Then
        '''                ' Compute effective company working interest for determining Finance shares
        '''                EffInts(x, gc_nEffInts_WFN) = TOTFINANCE(x) / (TOTFINANCE(x) + CA(x, gc_nCA_3DPFIN))
        '''            Else
        '''                EffInts(x, gc_nEffInts_WFN) = 1
        '''                EffInts(x, gc_nEffInts_PFN) = 0
        '''            End If
        '''            If CA(x, gc_nCA_NOCFIN) <> 0 And TOTFINANCE(x) <> 0 Then
        '''                ' Compute effective NOC participation for determining Finance share
        '''                EffInts(x, gc_nEffInts_PFN) = CA(x, gc_nCA_NOCFIN) / (TOTFINANCE(x) + CA(x, gc_nCA_3DPFIN) + CA(x, gc_nCA_NOCFIN))
        '''            End If
        '''        Next x
        '''        ' End (C0846)

        '****************************** added 10 January 2001
        'In a consolidation of projects with differing primary streams,
        'it could be that the span of the primary stream price doesn't extend
        'all of the way to include the span of the non-primary stream.
        'For this reason, we must check this and add prices to either the
        'front and/or end of the project so that when equivalent production
        'is calculated later on, it won't get zeroed out.

        'To do this, we will check if there is a zero price in any production year.
        'We will set prices for any year prior to the first non-zero price year
        'equal to the price in the first non-zero price year.
        'Likewise, we will set any zero prices at the end of the project
        'to the last non-zero price that we find.

        PPR = MSPM(8) 'PPR =1 for oil, 2 for gas primary stream

        'determine first year of non-zero prices of primary stream
        Setit = 0
        For x = 1 To LG
            If Setit = 0 Then
                ' GDP 20 Jan 2003
                ' Use constant for price offset
                If A(x, PPR + gc_nAPRICEOFFSET) <> 0 Then
                    Year1st = x
                    Setit = 1
                    FirstPrice = A(x, PPR + gc_nAPRICEOFFSET)
                End If
            End If
        Next x

        'Put first non-zero price in all years that have zero prices but some production
        If Year1st <> 1 Then
            For x = 1 To Year1st
                ' GDP 20 Jan 2003
                ' Additional conditions for extra volumes
                If A(x, gc_nAOIL) <> 0 Or A(x, gc_nAGAS) <> 0 Or A(x, gc_nAOV1) <> 0 Or A(x, gc_nAOV2) <> 0 Or A(x, gc_nAOV3) <> 0 Or A(x, gc_nAOV4) <> 0 Or A(x, gc_nAOV5) <> 0 Or A(x, gc_nAOV6) <> 0 Or A(x, gc_nAOV7) <> 0 Or A(x, gc_nAOV8) <> 0 Or A(x, gc_nAOV9) <> 0 Or A(x, gc_nAOV0) <> 0 Then
                    A(x, PPR + gc_nAPRICEOFFSET) = FirstPrice
                End If
            Next x
        End If

        'fill in any zero primary stream price years at the end
        'with the previous year's price

        For x = 2 To LG
            ' GDP 20 Jan 2003
            ' Use constant for offset
            If A(x, PPR + gc_nAPRICEOFFSET) = 0 Then
                A(x, PPR + gc_nAPRICEOFFSET) = A(x - 1, PPR + gc_nAPRICEOFFSET)
            End If
        Next x
        '******************************* end of addition 10 January 2001

        gn(1) = MSPM(1)

        For z = 1 To 6
            gn(z + 3) = MSPM(z + 1)
        Next z

        PPR = MSPM(8)
        gn(2) = MSPM(9)
        DiscMthd = MSPM(10)

        MY3T = TCC
        For z = 1 To 4
            PN(z) = PMMS(z)
        Next z

        'UPGRADE_WARNING: Lower bound of array my3Ex was changed from 0,gna_my3Ex_SizeD2_LB to 0,0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim my3Ex(UBound(my3, 1), gna_my3Ex_SizeD2_UB)

        For x = 1 To MY3T
            For w = 1 To 7
                my3(x, w) = MYC(x, w)
            Next w

            my3Ex(x, gna_my3Ex_TCX) = MYC(x, gc_nMYC_TCX)
            my3Ex(x, gna_my3Ex_OCX) = MYC(x, gc_nMYC_OCX)
            my3Ex(x, gna_my3Ex_GCX) = MYC(x, gc_nMYC_GCX)
            my3Ex(x, gna_my3Ex_CCX) = MYC(x, gc_nMYC_CCX)
            my3Ex(x, gna_my3Ex_CAT) = MYC(x, gc_nMYC_CAT)
            my3Ex(x, gna_my3Ex_XMO) = MYC(x, gc_nMYC_XMO)
            my3Ex(x, gna_my3Ex_XYR) = MYC(x, gc_nMYC_XYR)
            my3Ex(x, gna_my3Ex_TAN) = MYC(x, gc_nMYC_TAN)
        Next x

        '''        ' 22 Mar 2005 JWD (C0869) Add allocation of EffIntsX array
        '''        ReDim EffIntsX(0 To UBound(my3, 1), 0 To gc_nEffIntsXSIZED2)
        '''
        '''        ' 4 Jan 2004 JWD (C0846)
        '''        For x = 1 To MY3T
        '''            ' Save the identifying attributes of the expenditure, this is for
        '''            ' locating the MY3() element that corresponds to these interests,
        '''            ' this is because the MY3() is sorted between here and CashFlow.
        '''            EffIntsX(x, gc_nEffIntsX_CAT) = MYC(x, gc_nMYC_CAT)
        '''            EffIntsX(x, gc_nEffIntsX_XMO) = MYC(x, gc_nMYC_XMO)
        '''            EffIntsX(x, gc_nEffIntsX_XYR) = MYC(x, gc_nMYC_XYR)
        '''            EffIntsX(x, gc_nEffIntsX_TAN) = MYC(x, gc_nMYC_TAN)
        '''            ' Compute effective interest for company in respect of 3rd party expenditures
        '''            EffIntsX(x, gc_nEffIntsX_WIN) = MYC(x, gc_nMYC_AMT) / (MYC(x, gc_nMYC_AMT) + MYC(x, gc_nMYC_3DP))
        '''            ' Compute effective interest for NOC
        '''            EffIntsX(x, gc_nEffIntsX_PAR) = MYC(x, gc_nMYC_NOC) / (MYC(x, gc_nMYC_AMT) + MYC(x, gc_nMYC_3DP) + MYC(x, gc_nMYC_NOC))
        '''            ' Compute effective partner reimbursement portion of expenditure
        '''            EffIntsX(x, gc_nEffIntsX_BUR) = MYC(x, gc_nMYC_BUR) / (MYC(x, gc_nMYC_AMT) + MYC(x, gc_nMYC_3DP))
        '''        Next x
        '''        ' End (C0846)

        '5-18-92 can't handle DFL for pre-tax consolidated runs
        '<<<<<< 23 Jul 2001 JWD (C0354)
        ReDim DFL(gc_nMAXLIFE)
        '~~~~~~ was:
        'ReDim DFL(LG)
        '>>>>>> End (c0354)

        '<<<<<< 3 Jan 2002 JWD (C0485)
        For i = 1 To gc_nMAXLIFE 'initialize to 1
            DFL(i) = 1
        Next i
        '~~~~~~ was:
        'For i% = 1 To LG      'initialize to 1
        '  DFL(i%) = 1
        'Next i%
        '>>>>>> End (C0485)
        '----------------------------------------
    End Sub

    '
    ' Modifications:
    ' 13 Aug 1996 MKD
    '  -> Changed DIM of CA() and D8() to store Inflate() and
    '     DLF() which had been ignored before. This caused the
    '     dimension of the Inflate() and DLF() to be the
    '     length of the last data file in years, not the
    '     length of the consolidated case during a pre-tax
    '     Consol.
    '
    ' 25 Aug 1998 JWD
    '  -> Change symbol name In$ to strIn$ to eliminate name
    '     conflict with reserved word in VB5.
    '
    ' 6 Sep 1999 GDP
    '  -> Added bRVS variable, added code to read RVS file at end even
    '     if there is only one GETDATA statement - just checks for the
    '     existence of a RVS file.
    '
    ' 9 Sep 1999 GDP
    '  -> Commented out code added on 6/9/99
    '
    ' 13 Jun 2001 JWD
    '  -> Replace explicit occurrences of the detail capital
    '     expenditure category code string with the public
    '     symbol. (C0332)
    '
    ' 14 Jun 2001 JWD
    '  -> Add additional Other Capital items CP4-9 to
    '     miscellaneous titles single value sensitivity
    '     changes. (C0333)
    '
    ' 23 Jul 2001 JWD
    '  -> Change upper bound of OPEX() dimensioning to set
    '     to gc_nMAXLIFE rather than LG. (C0354)
    '
    ' 14 Sep 2001 JWD
    '  -> Change upper bounds of CA() array to store the
    '     consolidated gross production values. (C0443)
    '
    ' 17 Sep 2001 JWD
    '  -> Change dimensions of D8() to be the same size
    '     as CA(). (C0443)
    '
    ' 18 Sep 2001 JWD
    '  -> Change upper bounds of CA() array to store the
    '     consolidated gross revenue values. (C0443)
    '
    ' 21 Sep 2001 JWD
    '  -> Change redimension of MYC() array to use public
    '     symbol for sizing rather than literal 300. (C0454)
    '
    ' 20 Jan 2003 GDP
    '  -> Changes for additional volumes.
    '
    ' 08 Apr 2003 GDP
    '  -> Commented out some OXY code
    '
    ' 27 May 2003 JWD
    '  -> Add new adjustment forecast categories AJ6-AJ0.
    '     (C0700)
    '
    ' 9 Feb 2004 JWD
    '  -> Replace call to TerminateExecution with re-raise of
    '     error to caller. (C0779)
    '  -> Remove End statement, not permitted for ActiveX DLL
    '     use. (C0780)
    '
    ' 4 Jan 2005 JWD
    '  -> Change upper bound of MYC() dimension 2 to hold
    '     consolidated capital expenditures. (C0846)
    '
    ' 12 May 2005 JWD
    '  -> Add new adjustment categories A11-A20. (C0876)
    '
    ' 16 May 2005 JWD
    '  -> Add new operating expense categories OX6-O20.
    '     (C0877)
    '
    ' 17 May 2005 JWD
    '  -> Add new capital balance categories BL4-B20.
    '     (C0878)
    '
    Sub ForecastData()
        Dim YP As Single
        Dim SLO As Single
        Dim cat As String
        Dim DTES As Single
        Dim x As Single
        Dim NextProgram As String
        Dim rA As Single
        Dim ERO As Single
        Dim z As Short
        Dim zeros As Short
        Dim Rc As Short
        Dim RAP As Single
        Dim YX As Single
        Dim MX As Single
        Dim strIn As String
        Dim List As String
        Dim ptr As Short
        Dim ErrNo As Short
        Dim N2 As String
        '-----------------------------------------------------------------------
        ' Load data file and perform forecasting.
        '-----------------------------------------------------------------------
        Dim rPct As Single
        Dim bRVS As Boolean

        ' 16 May 2005 JWD (C0877) New loop index variable
        Dim jj As Short

        '---------------------------------------------------------
        Maxlife = gc_nMAXLIFE

        '-----------------------------------------------------------------

        Program = FORECAST_Renamed

        Dim dStart As Double
        dStart = VB.Timer()

        '-----------------------------------------------------------------

        ReDim PDC(1)
        ReDim BDA(1)
        ReDim INF(1)
        ReDim CPX(1)
        ReDim EBL(1)
        ReDim SAL(1)
        ReDim DTL(1)
        ReDim DNT(1)
32760:
        ReDim o(9)
        ReDim EL(2)
        ReDim dt(4)
        ReDim L10(12)

        '<<<<<< 14-18 Sep 2001 JWD (C0443)
        ' Changed to 29 (14) to hold gross production and
        ' then to 33 (18) to hold gross revenue for pre-tax
        ' consolidation.
        ' GDP 20 JAN 2003
        ' Use constant for size of CA array
        ReDim CA(gc_nMAXLIFE, gc_nCASIZE)
        '~~~~~~ was:
        ''Aug 13, 1996  Changed DIM of CA() to store Inflate() and DLF()
        ''  during a pre-tax Consol
        '' GDP 13/9/99 - Changed to 24 to hold repayment for pre-tax consoldiation
        '' GDP 14/11/2000 - Changed to 25 to hold finance for pre-tax consoldiation
        'ReDim CA(gc_nMAXLIFE, 25)
        '         'ReDim CA(40, 20)
        '>>>>>> End (C0443)

        ReDim gna_ACX(gc_nMAXLIFE, gc_nAMAXOPX, 3)

        ' 4 Jan 2005 JWD (C0846)
        ReDim MSPM(10)
        ReDim PMMS(4)
        ' Change upper bound of dimension 2
        ReDim MYC(gc_nMAXCAPEX, gc_nMYCSIZED2)
        ' Was:
        ''<<<<<< 21 Sep 2001 JWD (C0454)
        'ReDim MSPM(10), PMMS$(4), MYC(gc_nMAXCAPEX, 7)
        ''~~~~~~ was:
        ''ReDim MSPM(10), PMMS$(4), MYC(300, 7)
        ''>>>>>> End (C0454)
        ' End (C0846)
32763:
        '--------------------------------------------------------------------
        'THIS READS IN DATA FILE
        L10(6) = 10000 : L10(2) = 10000 : L10(7) = 10000
        L10(8) = 0 : L10(12) = 10000
        TCC = 0

        '----------- top of big loop ---------------------------
6010:   On Error GoTo 30000
        g_sDataFileNoExt = RF(3)
        '      If rA > 0 And UCase$(Left$(g_sDataFileNoExt, 5)) = "DUMMY" Then
        '        GetRunFileLine
        '        GoTo 8206
        '      End If
        N1 = RF(2) & RF(3)
        N2 = N1 & ".GNT"

6100:   'read statements here for the .GNT file

        FileOpen(2, N2, OpenMode.Input) 'open data file

        ' GDP 08 Apr 2003
        ' Commented out
        ''----------------------------------------------------------------------
        ''       put file name in the OXFIL file - for use by the OXY data base routines
        ''1-20-93
        '6110  zipfileno% = FreeFile
        '      Open FOxfil$ For Append As #zipfileno%
        '         Print #zipfileno%, N2$
        '      Close #zipfileno%
        ''----------------------------------------------------------------------
        If ErrNo = 0 Then
            GNTFile = N2 'OXY database item
6120:       Input(2, ver)
            If Left(ver, 9) = "VERSION 5" Then 'ver will be VERSION 5.n)
6130:           Read5Gnt(2)
            ElseIf Left(ver, 9) = "VERSION 4" Then
6140:           Read4Gnt(2)
            Else 'saved prior to version 4.0 - cannot use
6150:           Error (255)
            End If
        End If
        FileClose(2)
        ' GDP 20 JAN 2003
        ' Use constant for size of A array
        ReDim A(gc_nMAXLIFE, gc_nASIZE)

        '-----------------------------------------------------------------------
        'you are here with a good file and the data has been read

6500:   'READ NEXT LINE AND CHECK FOR SENSITIVITIES
6510:
        GetRunFileLine()
        If BDebugging Then
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            'GoSub DebugPrint
        End If

        'when all single value sensitivities are completed - goto 7900

        If Left(RF(1), 7) <> "SNSDATA" And Left(RF(1), 6) <> "SNSPER" Then
            SensDat = "N" 'OXY data item
            GoTo 7900
        Else 'next line is SNSDATA or SNSPER (sensitivities)
            SensDat = "Y" 'OXY data item
        End If
        'rf$(1) = "SNSDATA"
        If Left(RF(1), 6) = "SNSPER" Then 'must be percentage sensitivity
            GoTo 7900
        End If
        '--------------------------------------------------------------------
        'APPLY SINGLE VALUE SENSITIVITY (IF ANY)
        If RF(4) <> "GNL" Then
            GoTo 6640
        End If
        'CHANGE GENERAL PARAMETERS
        vlr = Val(RF(5)) 'row number for single value sensitivity
        vlc = Val(RF(6)) 'column number for single value sensitivity
        sVlu = RF(7)
        vlu = Val(sVlu)
        If vlc = 1 Then 'project start date
            GNL.dt(1) = Right("0" & sVlu, 5)
        ElseIf vlc = 2 Then  'discovery date
            GNL.dt(2) = Right("0" & sVlu, 5)
        ElseIf vlc = 3 Then  'production start date
            GNL.dt(3) = Right("0" & sVlu, 5)
        ElseIf vlc = 4 Then  'discount date
            GNL.dt(4) = Right("0" & sVlu, 5)
        ElseIf vlc = 5 Then  'Oil/Gas
            SearchCodeString("OILGAS", sVlu, 3, ptr)
            If ptr > 0 Then
                GNL.og = sVlu
            End If
        ElseIf vlc = 6 Then  'equivalance code
            GNL.eqvl = vlu
        End If
        GoTo 7700

6640:   If RF(4) <> "DSC" Then
            GoTo 6650
        End If
        'discounting parameters
        vlr = Val(RF(5)) 'row number for single value sensitivity
        vlc = Val(RF(6)) 'column number for single value sensitivity
        sVlu = RF(7)
        vlu = Val(sVlu)
        If vlc > 0 And vlc < 7 Then 'discount dates (1 - 6)
            GNL.pval(vlc) = vlu
        ElseIf vlc = 7 Then  'discount method
            SearchCodeString("FRCBEGMIDEND", sVlu, 3, ptr)
            If ptr > 0 Then
                GNL.dmtd = sVlu
            End If
        End If
        GoTo 7700

6650:   If RF(4) <> "TTS" Then
            GoTo 6660
        End If
        'titles screen
        vlr = Val(RF(5)) 'row number for single value sensitivity
        vlc = Val(RF(6)) 'column number for single value sensitivity
        sVlu = RF(7)
        vlu = Val(sVlu)
        If vlr > 0 And vlr < 5 Then 'titles 1 - 4
            GNL.ttl(vlr) = sVlu
        End If
        GoTo 7700

6660:   If LTrim(RTrim(RF(4))) <> "PDC" Then 'version 5.2 said RF$(3) - 7-1-92 MKD
            GoTo 6820
        End If
        'CHANGE PRODUCTION DECLINE CURVES
        vlr = Val(RF(5)) 'row number for single value sensitivity
        vlc = Val(RF(6)) 'column number for single value sensitivity
        sVlu = RF(7) 'LEFT$(RF$(7), 3)
        vlu = Val(sVlu)
        If vlr > PDCRecs Or vlc > 9 Then
            GoTo 7700
        End If
        Select Case vlc
            Case 1 'category
                SearchCodeString("OILGAS", sVlu, 3, ptr)
                If ptr > 0 Then
                    PDC(vlr).cat = sVlu
                End If
            Case 2
                SearchCodeString("DAYMONYRS", sVlu, 3, ptr)
                If ptr > 0 Then
                    PDC(vlr).unit = sVlu
                End If
            Case 3
                SearchCodeString("EXPHYPHAR", sVlu, 3, ptr)
                If ptr > 0 Then
                    PDC(vlr).unit = sVlu
                End If
            Case 4
                PDC(vlr).begprod = vlu
            Case 5
                PDC(vlr).RATE_Renamed = vlu
            Case 6
                PDC(vlr).hypexp = vlu
            Case 7
                PDC(vlr).endprod = vlu
            Case 8
                PDC(vlr).cumprod = vlu
            Case 9
                PDC(vlr).time = vlu
        End Select
        GoTo 7700

6820:   If RF(4) <> "BDA" Then
            GoTo 7100
        End If
        'CHANGE BASE DATA
        vlr = Val(RF(5))
        vlc = Val(RF(6))
        sVlu = RF(7)
        vlu = Val(sVlu)
        If vlr > BDARecs Or vlc > 10 Then
            GoTo 7700
        End If
        Select Case vlc 'column number
            Case 1 'category
                ' GDP 20 JAN 2003
                ' Added OV3-OV0 and OP3-OP0
                'List$ = "OILGASOV1OV2RESWINOPCGPCOP1OP2OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5"
                ' 27 May 2003 JWD (C0700) Add AJ6-AJ0
                'List$ = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0RESWINOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0"
                ' 12 May 2005 JWD (C0876) Add A11-A20
                'List$ = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0RESWINOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0A11A12A13A14A15A16A17A18A19A20"
                ' 16 May 2005 JWD (C0877) Add OX6-O20
                List = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0RESWINOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0OX1OX2OX3OX4OX5OX6OX7OX8OX9OX0O11O12O13O14O15O16O17O18O19O20AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0A11A12A13A14A15A16A17A18A19A20"
                SearchCodeString(List, sVlu, 3, ptr)
                If ptr > 0 Then
                    BDA(vlr).Cat = sVlu
                End If
                GoTo 7700
            Case 2 'units
                ' GDP 20 JAN 2003
                ' Alter Case statements to handle OV3-OV0 and OP3-OP0
                ' 27 May 2003 JWD (C0700) Add AJ6-AJ0
                ' 12 May 2005 JWD (C0876) Add A11-A20
                ' 16 May 2005 JWD (C0877) Add OX6-O20
                Select Case BDA(vlr).Cat
                    Case "OIL", "OV1", "OV3", "OV5", "OV7", "OV9"
                        List = "MMBB/M"
                    Case "GAS", "OV2", "OV4", "OV6", "OV8", "OV0"
                        List = "BCFM/B"
                    Case "RES", "WIN"
                        List = " % "
                    Case "OPC", "OP1", "OP3", "OP5", "OP7", "OP9"
                        List = "$/B"
                    Case "GPC", "OP2", "OP4", "OP6", "OP8", "OP0"
                        List = "$/M"
                    Case "OX1", "OX2", "OX3", "OX4", "OX5", "OX6", "OX7", "OX8", "OX9", "OX0", "O11", "O12", "O13", "O14", "O15", "O16", "O17", "O18", "O19", "O20"
                        List = "$MM$/B$/M"
                    Case "AJ1", "AJ2", "AJ3", "AJ4", "AJ5", "AJ6", "AJ7", "AJ8", "AJ9", "AJ0", "A11", "A12", "A13", "A14", "A15", "A16", "A17", "A18", "A19", "A20"
                        List = "$MM"
                End Select
                SearchCodeString(List, sVlu, 3, ptr)
                If ptr > 0 Then
                    BDA(vlr).unit = sVlu
                End If
                GoTo 7700
            Case 3 'date
                SearchCodeString("PDMPJMPDYPJY", sVlu, 3, ptr)
                If ptr > 0 Then
                    BDA(vlr).dat = sVlu
                End If
                GoTo 7700
            Case 4 'METHOD
                SearchCodeString("1234567890", sVlu, 1, ptr)
                If ptr > 0 Then
                    BDA(vlr).mtd = vlu
                End If
                GoTo 7700
            Case 5 'parameter 1
                BDA(vlr).parm1 = sVlu
            Case 6
                If Left(sVlu, 1) = "L" Then
                    BDA(vlr).parm2 = LIFE '-999
                Else
                    BDA(vlr).parm2 = vlu
                End If
            Case 7
                If Left(sVlu, 1) = "L" Then
                    BDA(vlr).parm3 = LIFE '-999
                Else
                    BDA(vlr).parm3 = vlu
                End If
            Case 8
                BDA(vlr).parm4 = vlu
            Case 9
                BDA(vlr).parm5 = vlu
            Case 10
                BDA(vlr).parm6 = vlu
        End Select
        GoTo 7700

7100:   If RF(4) <> "INF" Then
            GoTo 7260
        End If
        'CHANGE INFLATION SCREEN DATA
        vlr = Val(RF(5))
        vlc = Val(RF(6))
        sVlu = RF(7)
        vlu = Val(sVlu)
        If vlr > INFRecs Or vlc > 10 Then
            GoTo 7700
        End If
        Select Case vlc 'column number
            Case 1 'category

                '<<<<<< 13 Jun 2001 JWD
                ' GDP 20 JAN 2003
                ' Added OP3-OP0
                'List$ = "OPCGPCOP1OP2PRCOX1OX2OX3OX4OX5OPXAJ1AJ2AJ3AJ4AJ5"
                ' 27 May 2003 JWD (C0700) Add AJ6-AJ0
                'List$ = "OPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0PRCOX1OX2OX3OX4OX5OPXAJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0"
                ' 12 May 2005 JWD (C0876) Add A11-A20
                'List$ = "OPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0PRCOX1OX2OX3OX4OX5OPXAJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0A11A12A13A14A15A16A17A18A19A20"
                ' 16 May 2005 JWD (C0877) Add OX6-O20
                List = "OPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0PRCOX1OX2OX3OX4OX5OX6OX7OX8OX9OX0O11O12O13O14O15O16O17O18O19O20OPXAJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0A11A12A13A14A15A16A17A18A19A20"

                List = List & CPXCategoryCodesString
                List = List & "DFL"
                '~~~~~~ was:
                'List$ = "OPCGPCOP1OP2PRCOX1OX2OX3OX4OX5OPXAJ1AJ2AJ3AJ4AJ5"
                'List$ = List$ + "CPXEXPDEVBNSLSERENGEOEDHEDSADHASCDNPDVPPLFFCL"
                'List$ = List$ + "TRNEORCP1CP2CP3BALBL2BL3DFL"
                '>>>>>> End 13 Jun 2001

                SearchCodeString(List, sVlu, 3, ptr)
                If ptr > 0 Then
                    INF(vlr).Cat = sVlu
                End If
                GoTo 7700
            Case 2 'units
                If LTrim(RTrim(sVlu)) = "%" Then
                    INF(vlr).unit = sVlu
                End If
                GoTo 7700
            Case 3 'date
                SearchCodeString("PDMPJMPDYPJY", sVlu, 3, ptr)
                If ptr > 0 Then
                    INF(vlr).dat = sVlu
                End If
                GoTo 7700
            Case 4 'METHOD
                SearchCodeString("1234567890", sVlu, 1, ptr)
                If ptr > 0 Then
                    INF(vlr).mtd = vlu
                End If
                GoTo 7700
            Case 5 'parameter 1
                INF(vlr).parm1 = sVlu
            Case 6
                If Left(sVlu, 1) = "L" Then
                    INF(vlr).parm2 = LIFE '-999
                Else
                    INF(vlr).parm2 = vlu
                End If
            Case 7
                If Left(sVlu, 1) = "L" Then
                    INF(vlr).parm3 = LIFE '-999
                Else
                    INF(vlr).parm3 = vlu
                End If
            Case 8
                INF(vlr).parm4 = vlu
            Case 9
                INF(vlr).parm5 = vlu
            Case 10
                INF(vlr).parm6 = vlu
        End Select
        GoTo 7700
        '--------------------------------------------------------------------
        'capex changes
7260:   If Left(RF(4), 3) <> "CPX" Then
            GoTo 7692
        End If
        'CHANGE CAPITAL EXPENDITURES
        vlr = Val(RF(5))
        vlc = Val(RF(6))
        vlu = Val(RF(7))
        sVlu = RF(7)
        If vlr > CPXRecs Or vlc > 6 Then
            GoTo 7692
        End If
        If vlc = 1 Then
            'CHANGE CATEGORY

            '<<<<<< 13 Jun 2001 JWD
            List = CPXCategoryCodesString
            '~~~~~~ was:
            'List$ = "BNSLSERENGEOEDHEDSADHASCDNPDVPPLFFCLTRNEORCP1CP2CP3BALBL2BL3"
            '>>>>>> End 13 Jun 2001

            SearchCodeString(List, sVlu, 3, ptr)
            If ptr > 0 Then
                CPX(vlr).cat = sVlu
            End If
            GoTo 7700
        ElseIf vlc = 2 Then  'CHANGE DATE
            If Len(CPX(vlr).dat) > 5 Then
                CPX(vlr).dat = Right("0" & sVlu, 7)
            Else
                CPX(vlr).dat = Right("0" & sVlu, 5)
            End If
            GoTo 7700
        ElseIf vlc = 3 Then  'CHANGE TANGIBLE %
            CPX(vlr).tan = vlu
            GoTo 7700
        ElseIf vlc = 4 Then  'CHANGE amount
            CPX(vlr).amt = vlu
            GoTo 7700
        ElseIf vlc = 5 Then  'CHANGE WORKING INTEREST
            If Left(sVlu, 2) = "WI" Then
                CPX(vlr).WIN = WIN
            Else
                CPX(vlr).WIN = vlu
            End If
            GoTo 7700
        ElseIf vlc = 6 Then  'CHANGE reimbursement
            CPX(vlr).remb = vlu
        End If
        GoTo 7700

7692:   If Left(RF(4), 3) <> "SAL" Then
            GoTo 7694
        End If
        'Change Single Amount Loans
        vlr = Val(RF(5))
        vlc = Val(RF(6))
        vlu = Val(RF(7))
        sVlu = RF(7)
        If vlr > SALRecs Then
            GoTo 7694
        ElseIf vlc > 7 Then
            GoTo 7694
        End If
        If vlc = 1 Then 'date
            SAL(vlr).dat = sVlu
        ElseIf vlc = 2 Then
            SAL(vlr).amt = vlu
        ElseIf vlc = 3 Then
            SAL(vlr).Del = vlu
        ElseIf vlc = 4 Then
            SAL(vlr).mtd = sVlu
        ElseIf vlc = 5 Then
            SAL(vlr).int_Renamed = vlu
        ElseIf vlc = 6 Then
            SAL(vlr).mth = vlu
        ElseIf vlc = 7 Then
            SAL(vlr).acc = sVlu
        End If
        GoTo 7700

7694:   If Left(RF(4), 3) <> "EBL" Then
            GoTo 7696
        End If
        'Change Expenditure-Based Loans
        vlr = Val(RF(5))
        vlc = Val(RF(6))
        vlu = Val(RF(7))
        sVlu = RF(7)

        If vlr > EBLRecs Or vlc > 8 Then
            GoTo 7700
        End If
        If vlc = 1 Then 'category

            '<<<<<< 13 Jun 2001 JWD
            List = "ALLEXPDEV" & CPXCategoryCodesString
            '~~~~~~ was:
            'List$ = "ALLEXPDEVBNSLSERENGEOEDHEDSADHASCDNPDVPPLFFCLTRNEORCP1CP2CP3BALBL2BL3"
            '>>>>>> End 13 Jun 2001

            SearchCodeString(List, sVlu, 3, ptr)
            If ptr > 0 Then
                EBL(vlr).cat = sVlu
            End If
        ElseIf vlc = 2 Then  'date
            List = "PRDBEGDISLIF"
            SearchCodeString(List, sVlu, 3, ptr)
            If ptr > 0 Then
                EBL(vlr).endt = sVlu
            End If
        ElseIf vlc = 3 Then  'fin %
            EBL(vlr).fin = vlu
        ElseIf vlc = 4 Then  'endt - end date
            If sVlu = "DIS" Then
                EBL(vlr).Del = -989
            ElseIf sVlu = "PRD" Then
                EBL(vlr).Del = -987
            Else
                EBL(vlr).Del = vlu
            End If
        ElseIf vlc = 5 Then  'method
            List = "TOTPRI"
            SearchCodeString(List, sVlu, 3, ptr)
            If ptr > 0 Then
                EBL(vlr).mtd = sVlu
            End If
        ElseIf vlc = 6 Then  'int
            EBL(vlr).int_Renamed = vlu
        ElseIf vlc = 7 Then  'months
            EBL(vlr).mth = vlu
        ElseIf vlc = 8 Then  'accrual
            List = "YN"
            SearchCodeString(List, Left(sVlu, 1), 1, ptr)
            If ptr > 0 Then
                EBL(vlr).acc = sVlu
            End If
        End If



7696:   If Left(RF(4), 3) <> "DTL" Then
            GoTo 7700
        End If
        'Change Expenditure-Based Loans
        vlr = Val(RF(5))
        vlc = Val(RF(6))
        vlu = Val(RF(7))
        sVlu = RF(7)

        If vlr > DTLRecs Or vlc > 3 Then
            GoTo 7700
        End If
        If vlc = 1 Then 'category

            '<<<<<< 14 Jun 2001 JWD
            ' GDP 20 Jan 2003
            ' Added OV3-OV0
            'List$ = "OV1OV2OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5CP1CP2CP3CP4CP5CP6CP7CP8CP9BALBL2BL3"
            ' 27 May 2003 JWD (C0700) Add AJ6-AJ0
            'List$ = "OV1OV2OV3OV4OV5OV6OV7OV8OV9OV0OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0CP1CP2CP3CP4CP5CP6CP7CP8CP9BALBL2BL3"
            ' 12 May 2005 JWD (C0876) Add A11-A20
            'List$ = "OV1OV2OV3OV4OV5OV6OV7OV8OV9OV0OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0A11A12A13A14A15A16A17A18A19A20CP1CP2CP3CP4CP5CP6CP7CP8CP9BALBL2BL3"
            ' 16 May 2005 JWD (C0877) Add OX6-O20
            'List$ = "OV1OV2OV3OV4OV5OV6OV7OV8OV9OV0OX1OX2OX3OX4OX5OX6OX7OX8OX9OX0O11O12O13O14O15O16O17O18O19O20AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0A11A12A13A14A15A16A17A18A19A20CP1CP2CP3CP4CP5CP6CP7CP8CP9BALBL2BL3"
            ' 17 May 2005 JWD (C0878) Add BL4-B20
            List = "OV1OV2OV3OV4OV5OV6OV7OV8OV9OV0OX1OX2OX3OX4OX5OX6OX7OX8OX9OX0O11O12O13O14O15O1O17O18O19O20AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0A11A12A13A14A15A16A17A18A19A20CP1CP2CP3CP4CP5CP6CP7CP8CP9" & Mid(CPXCategoryCodesString, (CPXCategoryCodeBAL - 1) * 3 + 1, (CPXCategoryCodeBLn - CPXCategoryCodeBAL + 1) * 3)
            '~~~~~~ was:
            'List$ = "OV1OV2OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5CP1CP2CP3BALBL2BL3"
            '>>>>>> End 14 Jun 2001

            SearchCodeString(List, sVlu, 3, ptr)
            If ptr > 0 Then
                DTL(vlr).var = sVlu
            End If
        ElseIf vlc = 2 Then  'short title
            DTL(vlr).short_Renamed = sVlu
        ElseIf vlc = 3 Then  'long title
            DTL(vlr).long_Renamed = sVlu
        End If

7700:   GoTo 6510
        '---------------------------------------------------------------------
        'you are here after single value sensitivities changes are made
7900:
        DTAConvertData() 'convert data to "old" format

        '<<<<<< 25 Aug 1998 Change In$ to strIn$
        strIn = dt(1)
        DTAVerifyDates(strIn, MX, YX)
        mo = MX : YR = YX

        strIn = dt(2)
        DTAVerifyDates(strIn, MX, YX)
        M2 = MX : Y2 = YX

        strIn = dt(3)
        DTAVerifyDates(strIn, MX, YX)
        M1 = MX : Y1 = YX

        strIn = dt(4)
        DTAVerifyDates(strIn, MX, YX)
        '>>>>>> End 25 Aug 1998

        M3 = MX : Y3 = YX
        'If (Y3 - YR + ((M3 - MO) / 12)) >= 0 Then
        GoTo 7940
        'End If
        'if discount date < proj date then set discount date = proj date
        M3 = mo : Y3 = YR
7940:
        'This program checks for errors in the data and fills A(x,y)
        RAP = 0

        'FORECAST BaseData & Production Decline Curve data
        ' GDP 20 Jan 2003
        ' Use constant for A array size.
        ReDim A(gc_nMAXLIFE, gc_nASIZE)
        Dim ab(gc_nMAXLIFE, gc_nASIZE) As Single
        Dim units(gc_nASIZE) As String
        Rc = 0

7950:   ForecastExec(ab, units, Rc)
7953:
        '<<<<<< 23 Jul 2001 JWD (C0354)
        ReDim OPEX(gc_nMAXLIFE)
        '~~~~~~ was:
        'ReDim OPEX(LG)
        '>>>>>> End (C0354)

7960:   DTAConvertABtoA(ab, units)

        'if user has not entered anything in Base data for WIN,
        '  load a(1-LG,6) = 1 else leave a(1-LG,6) as is
        zeros = -1 'TRUE
        For z = 1 To LG
            ' GDP 20 Jan 2003
            ' A(z%, 6) to A(z%, gc_nAWIN)
            If A(z, gc_nAWIN) <> 0 Then
                zeros = 0 'false
                Exit For
            End If
        Next z
        If zeros = -1 Then
            For z = 1 To LG
                ' GDP 20 Jan 2003
                ' A(z%, 6) to A(z%, gc_nAWIN)
                A(z, gc_nAWIN) = 100
            Next z
        End If


7990:   If ERO <> 0 Then 'ero was an error flag from forecasting routine
            Error (ERO)
        End If

        'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        'GoSub 40000 'inflates CAPEX and apply Pct sensitivities
        On Error Resume Next
40001:
        If MY3T = 0 Then GoTo 41000

        InflateCAPEX() 'inflate CAPEX records as needed

        'REPLACE 999 WITH ACTUAL WORKING INTEREST
        For x = 1 To MY3T
40383:      DTES = my3(x, 3) - YR + 1
            ' GDP 20 JAN 2003
            ' A(DTES, 6) to A(DTES, gc_nAWIN)
40384:      If my3(x, 6) = WIN Then my3(x, 6) = A(DTES, gc_nAWIN) 'used to be category 10 - now is # 6
40490:  Next x
        '--------------------------------------------------------------------
41000:  'THIS APPLIES PERCENTAGE SENSITIVITIES
        If Left(RF(1), 6) <> "SNSPER" Then GoTo 42000

        cat = RF(4)
        rPct = Val(RF(5))
        ''PctSensitivity
        PercentSens(cat, rPct)

        '--------------------------------------------------------------------
41500:  ' READ NEXT LINE AND LOOP BACK THRU IF SNSDATA

        GetRunFileLine() 'get next run file line
        If BDebugging Then
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            '			'GoSub DebugPrint
        End If
        'there are pct sensitivity items on this line
        If Left(RF(1), 6) = "SNSPER" Then GoTo 41000

42000:  For x = 1 To LG
            ' GDP 20 Jan 2003
            ' Use constants for A array opex refs
            'OPEX(x) = A(x, gc_nAOX1) + A(x, gc_nAOX2) + A(x, gc_nAOX3) + A(x, gc_nAOX4) + A(x, gc_nAOX5)
            ' 16 May 2005 JWD (C0877)
            For jj = gc_nAOX1 To gc_nAO20
                OPEX(x) = OPEX(x) + A(x, jj)
            Next jj
            ' End (C0877)
        Next x


        If Left(RF(1), 7) <> "GETDATA" Then
            GoTo 8100 'if not GETDATA it nust be SNSDATA
        End If

        rA = rA + 1 'ra = 0 you had 1 getdata,  if ra > 0 then you had at least two getdata

        'we are here because RF$(1) is "GETDATA"
        'MsgBox "call 38500 because another getdata  lg = " & lg

        'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        ''GoSub 38500 'stores (basically) a(n,n) and other data from last getdata
        sub38500(rA)

        'MsgBox "call 45000 to flush last runs values"

        'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        'GoSub 45000 'flush variables from last GETDATA

        ' Directly copied 45000 here
        SLO = 0 : LFI = 0 : LGI = 0 : LFH = 0
        LGH = 0 : LFX = 0 : LG = 0 : PPR = 0
        SEC = 0 : ERO = 0 : YP = 0

        'MsgBox "goto 6010 to read next file"

        GoTo 6010

8100:
8200:
        If rA = 0 Then 'one GETDATA stmt
8201:       'ERASE ab
8202:       'ERASE O, EL$, dt$, L10, CA
8203:       'ERASE MSPM, PMMS$, MYC
8204:       Camefrom = FORECAST_Renamed
16160:
16161:      NextProgram = AppDir & "CNTYFCST"
            ' Added GDP 6/9/99
            ' Commented out 9/9/99 GDP
            ' Begin
            '        bRVS = Len(Dir$(TempDir$ & g_sDataFileNoExt & "." & c_sRVSEXT)) <> 0
            '        If bRVS Then
            '            GoSub 38500
            '            DTAResetVariables
            '        End If
            ' End
            ''PushStats "FORECAST", dStart
            Exit Sub
16162:  End If

8205:   'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        'GoSub 38500 'adds (basically) a(n,n) and other data from last getdata
        sub38500(rA)

        '38500 also applies percentage sensitivities
        '38500 also applies inflation to capex

8206:   DTAResetVariables() 'reassign temp arrays to a(n,n) and misc arrays

8207:   'ERASE ab
8208:   'ERASE O, EL$, dt$, L10, CA
8209:   'ERASE MSPM, PMMS$, MYC

        Camefrom = FORECAST_Renamed
        NextProgram = AppDir & "CNTYFCST"
        ''PushStats "FORECAST", dStart
        Exit Sub

        '-----------------------------------------------------------------------
30000:  ' Main Error Handler
        ' 9 Feb 2004 JWD (C0779) Replace with re-raise of error to caller
        Err.Raise(Err.Number, Err.Source & " < ForecastData", Err.Description) ' TerminateExecution
        ' 9 Feb 2004 JWD (C0780) Remove End statement
        ' End
        '-----------------------------------------------------------------------
38500:  'HOLD VARIABLES FOR CONSOLIDATION
        'This accumulates consolidation data
        'MsgBox "  in 38500  redim l20()"
        Dim L20(12) As Single

        '<<<<<< 17 Sep 2001 JWD (C0443)
        ' Make D8() same size as CA().
        Dim D8(gc_nMAXLIFE, UBound(CA, 2)) As Single
        '~~~~~~ was:
        ''Aug 13, 1996 - Changed DIM of D8()
        'ReDim D8(gc_nMAXLIFE, 25)     'OLD: D8(40, 20) GDP OLD: D8(40, 24)  - 14/11/2000 - consolidation of finance
        '>>>>>> End (C0443)

        'this SUB replaces 38500-39100
        ConsolValues(TCC, rA, L10, L20, D8, CA, MYC, MSPM, PMMS)

        'UPGRADE_WARNING: Return has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        Return
    End Sub
    Sub sub38500(ByRef rA As Single)
        Dim L20(12) As Single

        '<<<<<< 17 Sep 2001 JWD (C0443)
        ' Make D8() same size as CA().
        Dim D8(gc_nMAXLIFE, UBound(CA, 2)) As Single
        '~~~~~~ was:
        ''Aug 13, 1996 - Changed DIM of D8()
        'ReDim D8(gc_nMAXLIFE, 25)     'OLD: D8(40, 20) GDP OLD: D8(40, 24)  - 14/11/2000 - consolidation of finance
        '>>>>>> End (C0443)

        'this SUB replaces 38500-39100
        ConsolValues(TCC, rA, L10, L20, D8, CA, MYC, MSPM, PMMS)
    End Sub

    ' $SubTitle:'ForecastExec - Main Forecasting for execution'
    ' $Page
    '
    ' Modifications:
    ' 23 Jul 2001 JWD
    '  -> Change to leave the A() and AB() arrays dimensioned
    '     the same as on entry, not redimensioned to LG. It is
    '     not necessary to redim the arrays down, and leaves
    '     A() long to accomodate the extension of LG by the
    '     abandonment expenditure timing. (C0354)
    '  -> Dimension Inflate() to Maxlife rather than LG.
    '     (C0354)
    '  -> Dimension DFL() to Maxlife rather than LG. (C0354)
    '
    ' 3 Jan 2002 JWD
    '  -> Change upper limit of loop initializing DFL()
    '     to set to upper bound of array rather than
    '     through LG. (C0485)
    '
    ' 20 Jan 2003 GDP
    '  -> Changes for additional volumes
    '
    ' 27 May 2003 JWD
    '  -> Add codes for new adjustment categories AJ6-AJ0.
    '     (C0700)
    '
    ' 12 May 2005 JWD
    '  -> Add codes for new adjustment categories A11-A20.
    '     (C0876)
    '
    ' 16 May 2005 JWD
    '  -> Add new operating expense categories OX6-O20.
    '     (C0877)
    '
    Sub ForecastExec(ByRef ab(,) As Single, ByRef units() As String, ByRef Rc As Short)
        Dim Port As Single
        Dim portion As Single
        Dim ProjYear As Short
        Dim ProdYear As Short
        Dim lastper As Short
        Dim arg As String
        Dim category As String
        Dim nocategories As Short
        '--------------------------------------------------------------------
        'This sub is VERY similar to ForecastMain except that this sub
        '  is called from FORECAST.BAS during run execution.
        '  parameters: ab(), units$(), rc%
        '  parameters in: ---
        '  parameters out: ab(), rc%
        '  function:   loops through all categories calling forecastdispatcher
        '              calls DTAForecastLoadA to put contents of datacol!()
        '                into ab(n,n)
        '                       ab() is in stated units of the category
        '                       units$ = unit of measure for each category
        '  calls: ForecastDispatcher, DTAForecastLoadA
        '---------------------------------------------------------
        ' Modifications:
        ' 20 Feb 1996 JWD
        '           Commented out curvelife%, duplicate definition
        '        and is otherwise not referenced.
        '---------------------------------------------------------
        Dim i As Short
        Dim uD As Short
        '---------------------------------------------------------
7210:

        '////////////////////////////////////////////////////////////////////
        AdjustLastYear = False 'pro-rate last year (T/F)
        '////////////////////////////////////////////////////////////////////

        curvelife = 0
        arraysize = gc_nMAXLIFE
        nocategories = gc_nASIZE
        If LG = 0 Then 'if the first time here - initialize LG
            LG = arraysize
        End If

        Dim Datacol(arraysize) As Single
        Dim cats(nocategories) As String 'make arrays exist

        'NOTE: these are base data categories (+ PDC (oil & gas))
        ' GDP 20 Jan 2003
        ' Added OV3 - OV0, OP3 - OP0
        '
        ' category$ = "OILGASOV1OV2RESWINOPCGPCOP1OP2OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5"
        ' 27 May 2003 JWD (C0700) Add AJ6-AJ0
        'category$ = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0RESWINOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0"
        ' 12 May 2005 JWD (C0876) Add A11-A20
        'category$ = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0RESWINOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0A11A12A13A14A15A16A17A18A19A20"
        ' 16 May 2005 JWD (C0877) Add OX6-O20
        category = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0RESWINOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0OX1OX2OX3OX4OX5OX6OX7OX8OX9OX0O11O12O13O14O15O16O17O18O19O20AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0A11A12A13A14A15A16A17A18A19A20"
        For i = 1 To nocategories
            cats(i) = Mid(category, (i - 1) * 3 + 1, 3)
        Next i
        category = ""
        '--------------------------------------------------------------------
        'forecast primary product first (to set life)
        updatea = False
        arg = CStr(cats(PPR))
31786:
        'fln% = FreeFile
        'Open "FCST.LOG" For Append As #fln%
        '   Print #fln%, "call ForecastDispatch for primary product  "; arg$; "  PPR = "; PPR
        'Close #fln%

        DTAForecastDispatch(arg, Datacol, updatea)
17100:
        'fln% = FreeFile
        'Open "FCST.LOG" For Append As #fln%
        '   For q% = 1 To UBound(datacol!)
        '      Print #fln%, "back from ForecastDispatch DataCol!("; qq%; ") = "; datacol!(qq%)
        '   Next q%
        'Close #fln%


        units(PPR) = arg
        uD = UBound(Datacol)
        For i = uD To 1 Step -1
            If Datacol(i) <> 0 Then
                lastper = i
                Exit For
            End If
        Next i
17110:
        'examine primarystart$ (in common) and set proddelay (in common)
        '  proddelay is used to determine the delay between project start
        '  month and the start of primary product production start
        ProdYear = ProdYr
        If ProdYear < 50 Then
            ProdYear = ProdYear + 100
        End If
        ProjYear = ProjYr
        If ProjYear < 50 Then
            ProjYear = ProjYear + 100
        End If
17120:
        Select Case PrimaryStart
            Case "PJY", "PJM"
                proddelay = 0
            Case "PDY"
                proddelay = ((ProdYear - ProjYear) * 12) + (1 - ProjMo)
            Case "PDM"
                proddelay = ((ProdYear - ProjYear) * 12) + (ProdMo - ProjMo)
        End Select
17130:

        ForecastSetLife() 'set life of project based on producing life

17140:

        '  REDIM PRESERVE datacol!(arraysize%)
        '<<<<<< 23 Jul 2001 JWD (C0354)
        ReDim Inflate(gc_nMAXLIFE, 2)
        '~~~~~~ was:
        'ReDim A(LG, 20), ab(LG, 20), Inflate(LG, 2)
        '>>>>>> End (C0354)

17150:
        ForecastLoadA(PPR, Datacol, ab)
17160:
        ReDim Datacol(arraysize) 'flush array
        '--------------------------------------------------------------------
        'forecast secondary product next
        'pass cats$(3 - ppr) as the category -
        '  (if OIL is primary, (3 - PPR) = 2 (GAS))
        '  (if GAS is primary, (3 - PPR) = 1 (OIL))
        updatea = False
        arg = CStr(cats(3 - PPR))
17180:
        '~~~~CurveLife% = 0
        DTAForecastDispatch(arg, Datacol, updatea)
        units(3 - PPR) = arg

        If updatea <> 0 Then
            '////////////////////////////////////////////////////////////////////
            AdjustPeriod(portion, Port, Datacol)
            '////////////////////////////////////////////////////////////////////
            ForecastLoadA((3 - PPR), Datacol, ab)
        End If
        ReDim Datacol(arraysize) 'flush array
        '--------------------------------------------------------------------
        'forecast remaining categories
        For i = 3 To UBound(cats)
            updatea = False
            arg = CStr(cats(i))
            DTAForecastDispatch(arg, Datacol, updatea)
            units(i) = arg
            If updatea <> 0 Then
                'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
                AdjustPeriod(portion, Port, Datacol)
                ForecastLoadA(i, Datacol, ab)
            End If
            ReDim Datacol(arraysize)
        Next i

        'now see if there is a DFL (deflator) record on the Inflation
        '  screen. If there is - put the values in DFL() (DFL() is in common).

        '<<<<<< 23 Jul 2001 JWD (C0354)
        ReDim DFL(gc_nMAXLIFE)
        '~~~~~~ was:
        'ReDim DFL(LG)
        '>>>>>> End (C0354)

        '<<<<<< 3 Jan 2002 JWD (C0485)
        For i = 1 To gc_nMAXLIFE 'initialize to 1
            DFL(i) = 1
        Next i
        '~~~~~~ was:
        'For i = 1 To LG      'initialize to 1
        '   DFL(i) = 1
        'Next i
        '>>>>>> End (C0485)

        arg = "DFL"
        updatea = -999

31789:
        DTAForecastDispatch(arg, Datacol, updatea)

        If updatea <> 0 Then
            'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
            AdjustPeriod(portion, Port, Datacol)
            For i = 1 To LG
                If i <= UBound(Datacol) Then
                    DFL(i) = Datacol(i)
                End If
            Next i
            'convert the annual percentage amounts (results of forecasting)
            '  to the decimal factor year by year for use in CASHFLOW.BAS)
            DFL(1) = 1 + (DFL(1) / 100)
            For i = 2 To LG
                DFL(i) = (1 + (DFL(i) / 100)) * DFL(i - 1)
            Next i
        End If

        'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        System.Array.Clear(cats, 0, cats.Length)
        'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        System.Array.Clear(Datacol, 0, Datacol.Length)
        If Not updatea Then
            Rc = False
        End If
17999:

        Exit Sub

        '--------------------------------------------------------------------
        'GOSUBS HERE


    End Sub

    Sub AdjustPeriod(ByRef portion As Single, ByRef Port As Single, ByRef Datacol() As Single)
        If AdjustLastYear Then
            AdjustLastYear = False
            'portion = part of year with activity in last calendar year of project
            portion = LGI + ((ProjMo - 1) / 12)
            Port = Int(portion)
            If Port = portion Then
                portion = 1
            Else
                portion = portion - Port
            End If
            Datacol(LG) = Datacol(LG) * portion
        End If

        'UPGRADE_WARNING: Return has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
    End Sub

    ' $SubTitle:'LoadFsctBaseData - Load FcstData from Base Data'
    ' $Page
    Sub LoadFcstBaseData(ByRef category As String, ByRef BDAData() As ParmType)
        Dim fcstptr As Short
        Dim i As Short
        Dim Count As Short
        '--------------------------------------------------------------------
        '  parameters: category$, BDAdata() AS ParmType, found%
        '  parameters in: category$
        '  parameters out: BDAdata() AS ParmType
        '  function: searches BDA() in common searching for the given category
        '  returns: BDAData() with all of the recs of the given category
17190:
        Count = 0
        For i = 1 To BDARecs
            If BDA(i).Cat = category Then
                Count = Count + 1
            End If
        Next i

        ReDim BDAData(Count)

        fcstptr = 0
        For i = 1 To BDARecs
            If BDA(i).Cat = category Then
                fcstptr = fcstptr + 1
                BDAData(fcstptr).Cat = BDA(i).Cat
                BDAData(fcstptr).unit = BDA(i).unit
                BDAData(fcstptr).dat = BDA(i).dat
                BDAData(fcstptr).mtd = BDA(i).mtd
                BDAData(fcstptr).parm1 = BDA(i).parm1
                BDAData(fcstptr).parm2 = BDA(i).parm2
                BDAData(fcstptr).parm3 = BDA(i).parm3
                BDAData(fcstptr).parm4 = BDA(i).parm4
                BDAData(fcstptr).parm5 = BDA(i).parm5
                BDAData(fcstptr).parm6 = BDA(i).parm6
            End If
        Next i

    End Sub

    ' $SubTitle:'LoadFsctBaseData - Load FcstData from Base Data'
    ' $Page
    Sub LoadFcstDCL(ByRef category As String, ByRef pdcdata() As PDCType)
        Dim fcstptr As Short
        Dim i As Short
        Dim Count As Short
        '--------------------------------------------------------------------
18190:
        Count = 0
        For i = 1 To PDCRecs
            If PDC(i).cat = category Then
                Count = Count + 1
            End If
        Next i

        ReDim pdcdata(Count)

        fcstptr = 0
        For i = 1 To PDCRecs
            If PDC(i).cat = category Then
                fcstptr = fcstptr + 1
                pdcdata(fcstptr).cat = PDC(i).cat
                pdcdata(fcstptr).unit = PDC(i).unit
                pdcdata(fcstptr).mtd = PDC(i).mtd
                pdcdata(fcstptr).begprod = PDC(i).begprod
                pdcdata(fcstptr).RATE_Renamed = PDC(i).RATE_Renamed
                pdcdata(fcstptr).hypexp = PDC(i).hypexp
                pdcdata(fcstptr).endprod = PDC(i).endprod
                pdcdata(fcstptr).cumprod = PDC(i).cumprod
                pdcdata(fcstptr).time = PDC(i).time
            End If
        Next i

    End Sub

    ' $SubTitle:'LoadFsctEXB - Load FcstData from EXB()'
    ' $Page
    '
    ' Modifications:
    ' 20 Feb 1996 JWD
    '  -> Renamed EXT.DRIVE$ to sExtDir, previous name
    '     not acceptable to VB.
    '  -> Remove read of configuration file to get the
    '     external table directory, sExtDir is now global.
    '
    ' 17 May 1996 MKD
    '  -> Changed "DIM rdummy as integer" to "DIM rdummy as
    '     single" Dimming it as an integer caused program
    '     crash when reading the External Inflation records
    '     into the dummy placeholder variables.
    '
    Sub LoadFcstEXB(ByRef category As String, ByRef parmdata() As EXBType, ByRef FileName As String)
        Dim fcstptr As Short
        Dim Count As Short
        Dim ErrNo As Short
        Dim i As Short
        Dim EXBRecs As Short
        Dim dummy As String
        Dim nm As String
        Dim filenum As Short
        '--------------------------------------------------------------------
        'This sub is called by LoadFcstBDA when user has referenced an external
        '  file name in parm 1.  This sub opens the external file, reads the
        '  records and then loads the records for the category into parmdata()
        '  and returns to LoadFcstBaseData

        '  parameters: category$, parmdata() AS ParmType, filename$
        '  parameters in: category$
        '  parameters out: parmdata() AS ParmType
        '  function: searches EXB() in common searching for the given category
        '  returns: parmdata() with all of the recs of the given category
        '---------------------------------------------------------
        Dim iDum As Short
        Dim iDummy As Short
        Dim rdummy As Single
        Dim sDum As String
        Dim EXB() As EXBType 'record for EXTERNAL BASE DATA
        '---------------------------------------------------------
9125:
9135:

        filenum = FreeFile()
        nm = sExtDir & FileName & ".EXT"
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Len(Dir(nm)) > 0 Then

            ' GDP 08 Apr 2003
            ' Commented out
            ''----------------------------------------------------------------------
            ''       put file name in the OXFIL file - for use by the OXY data base routines
            ''1-20-93
            '   zipfileno% = FreeFile
            '   Open FOxfil$ For Append As #zipfileno%
            '      Print #zipfileno%, nm$
            '   Close #zipfileno%
            ''----------------------------------------------------------------------


9145:
            FileOpen(filenum, nm, OpenMode.Input)
9155:
            If ErrNo = 0 Then
                EXTFile = nm 'OXY database item
                Input(filenum, ver)
                ver = RTrim(ver)
                If ver = "VERSION 5.0" Or ver = "VERSION 5.1" Or ver = "VERSION 5.2" Or ver = "VERSION 5.3" Then
                    Input(filenum, dummy) 'case description
                    EXTDesc = dummy 'oxy DATABASE ITEM
                    Input(filenum, EXBRecs)
                    Input(filenum, iDum)
                    Input(filenum, ExtNotes)
                    ReDim EXB(EXBRecs) 'record for EXTERNAL BASE DATA
                    ReDim EXTNote(ExtNotes)
                    For i = 1 To EXBRecs
                        Input(filenum, EXB(i).Cat)
                        Input(filenum, EXB(i).unit)
                        Input(filenum, EXB(i).dat)
                        Input(filenum, EXB(i).mtd)
                        Input(filenum, EXB(i).parm1)
                        Input(filenum, EXB(i).parm2)
                        Input(filenum, EXB(i).parm3)
                        Input(filenum, EXB(i).parm4)
                        Input(filenum, EXB(i).parm5)
                        Input(filenum, EXB(i).parm6)
                    Next i
                    For i = 1 To iDum
                        Input(filenum, sDum)
                        Input(filenum, sDum)
                        Input(filenum, iDummy)
                        Input(filenum, iDummy)
                        Input(filenum, rdummy)
                        Input(filenum, rdummy)
                        Input(filenum, rdummy)
                        Input(filenum, rdummy)
                        Input(filenum, rdummy)
                        Input(filenum, rdummy)
                    Next i
                    For i = 1 To ExtNotes
                        Input(filenum, EXTNote(i))
                    Next i


                    FileClose(filenum)

                End If
            End If
        Else
            EXBRecs = 0
        End If
        'count records for given category
        Count = 0
        For i = 1 To EXBRecs
            If EXB(i).Cat = category Then
                Count = Count + 1
            End If
        Next i
        'resize parmdata (if no records match category, redim to 0 recs
        ReDim parmdata(Count)

        fcstptr = 0
        For i = 1 To EXBRecs
            If EXB(i).Cat = category Then
                fcstptr = fcstptr + 1
                parmdata(fcstptr).Cat = EXB(i).Cat
                parmdata(fcstptr).unit = EXB(i).unit
                parmdata(fcstptr).dat = EXB(i).dat
                parmdata(fcstptr).mtd = EXB(i).mtd
                parmdata(fcstptr).parm1 = EXB(i).parm1
                parmdata(fcstptr).parm2 = EXB(i).parm2
                parmdata(fcstptr).parm3 = EXB(i).parm3
                parmdata(fcstptr).parm4 = EXB(i).parm4
                parmdata(fcstptr).parm5 = EXB(i).parm5
                parmdata(fcstptr).parm6 = EXB(i).parm6
            End If
        Next i
9156:
    End Sub

    ' $SubTitle:'LoadFsctEXI - Load FcstData from EXI()'
    ' $Page
    Sub LoadFcstEXI(ByRef category As String, ByRef parmdata() As EXBType, ByRef FileName As String)
        Dim fcstptr As Short
        Dim Count As Short
        Dim arg As String
        Dim ErrNo As Short
        Dim i As Short
        Dim EXIRecs As Short
        Dim EXBRecs As Short
        Dim nm As String
        Dim filenum As Short
        '--------------------------------------------------------------------
        '  parameters: category$, parmdata() AS ParmType, found%
        '  parameters in: category$
        '  parameters out: parmdata() AS ParmType
        '  function: searches EXI() in common searching for the given category
        '  returns: parmdata() with all of the recs of the given category
        '---------------------------------------------------------
        ' Modifications:
        ' 20 Feb 1996 JWD
        '           Renamed EXT.DRIVE$ to sExtDir, previous name
        '        not acceptable to VB.
        '           Remove read of configuration file to get the
        '        external table directory, sExtDir is now global.
        '
        ' 20 Jan 2003 GDP
        '    -> Changes for extra volumes OV3-OV0
        '
        ' 9 Feb 2004 JWD
        '  -> Remove output to OXFIL.DAT. File is obsolete and not
        '     used in any application system. (C0783)
        '
        ' 16 May 2005 JWD
        '  -> Add new operating expense categories OX6-O20.
        '     (C0877)
        '---------------------------------------------------------
        Dim iDummy As Short
        Dim rdummy As Single
        Dim sDummy As String
        Dim EXI() As EXBType
        '---------------------------------------------------------
2322:

        filenum = FreeFile()
        nm = sExtDir & FileName & ".EXT"
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Len(Dir(nm)) > 0 Then

            '----------------------------------------------------------------------
            '       put file name in the OXFIL file - for use by the OXY data base routines
            '1-20-93
            ''' 9 Feb 2004 JWD (C0783) Remove output to OXFIL
            '''zipfileno% = FreeFile
            '''Open FOxfil$ For Append As #zipfileno%
            '''   Print #zipfileno%, nm$
            '''Close #zipfileno%
            '----------------------------------------------------------------------

            FileOpen(filenum, nm, OpenMode.Input)
            If ErrNo = 0 Then
                EXTFile = nm 'OXY database item
                Input(filenum, ver)
                ver = RTrim(ver)
                If ver = "VERSION 5.0" Or ver = "VERSION 5.1" Or ver = "VERSION 5.2" Or ver = "VERSION 5.3" Then
                    Input(filenum, sDummy) 'case description
                    EXTDesc = sDummy 'oxy DATABASE ITEM
                    Input(filenum, EXBRecs)
                    Input(filenum, EXIRecs)
                    Input(filenum, ExtNotes)
                    ReDim EXI(EXIRecs)
                    ReDim EXTNote(ExtNotes)
                    'record for EXBScreen (EXTERNAM BASE DATA)
                    For i = 1 To EXBRecs
                        Input(filenum, sDummy)
                        Input(filenum, sDummy)
                        Input(filenum, iDummy)
                        Input(filenum, iDummy)
                        Input(filenum, rdummy)
                        Input(filenum, rdummy)
                        Input(filenum, rdummy)
                        Input(filenum, rdummy)
                        Input(filenum, rdummy)
                        Input(filenum, rdummy)
                    Next i
                    'record for EXIScreen (EXTERNAL INFLATION)
                    For i = 1 To EXIRecs
                        Input(filenum, EXI(i).Cat)
                        Input(filenum, EXI(i).unit)
                        Input(filenum, EXI(i).dat)
                        Input(filenum, EXI(i).mtd)
                        Input(filenum, EXI(i).parm1)
                        Input(filenum, EXI(i).parm2)
                        Input(filenum, EXI(i).parm3)
                        Input(filenum, EXI(i).parm4)
                        Input(filenum, EXI(i).parm5)
                        Input(filenum, EXI(i).parm6)
                    Next i
                    For i = 1 To ExtNotes
                        Input(filenum, EXTNote(i))
                    Next i

                    FileClose(filenum)
                End If
            End If
        Else
            EXBRecs = 0
        End If

        'now external table is loaded - begin search
        arg = category
        Count = 0
        For i = 1 To EXIRecs
            If EXI(i).Cat = arg Then
                Count = Count + 1
            End If
        Next i

        If Count = 0 Then
            Select Case arg
                ' GDP 20 JAN 2003
                ' Added OP3-OP0
                Case "OPC", "GPC", "OP1", "OP2", "OP3", "OP4", "OP5", "OP6", "OP7", "OP8", "OP9", "OP0" 'search for "PRC" in INF to use instead
                    arg = "PRC"
                    ' 16 May 2005 JWD (C0877) Add OX6-O20
                Case "OX1", "OX2", "OX3", "OX4", "OX5", "OX6", "OX7", "OX8", "OX9", "OX0", "O11", "O12", "O13", "O14", "O15", "O16", "O17", "O18", "O19", "O20" 'search for "OPX" in INF to use instead
                    arg = "OPX"
            End Select
            For i = 1 To EXIRecs
                If EXI(i).Cat = arg Then
                    Count = Count + 1
                End If
            Next i
        End If

        ReDim parmdata(Count)

        fcstptr = 0
        For i = 1 To EXIRecs
            If EXI(i).Cat = arg Then
                fcstptr = fcstptr + 1
                parmdata(fcstptr).Cat = EXI(i).Cat
                parmdata(fcstptr).unit = EXI(i).unit
                parmdata(fcstptr).dat = EXI(i).dat
                parmdata(fcstptr).mtd = EXI(i).mtd
                parmdata(fcstptr).parm1 = EXI(i).parm1
                parmdata(fcstptr).parm2 = EXI(i).parm2
                parmdata(fcstptr).parm3 = EXI(i).parm3
                parmdata(fcstptr).parm4 = EXI(i).parm4
                parmdata(fcstptr).parm5 = EXI(i).parm5
                parmdata(fcstptr).parm6 = EXI(i).parm6
            End If
        Next i

    End Sub

    ' $SubTitle:'LoadFsctINF - Load FcstData from INF()'
    ' $Page

    'Modifications
    ' 20 Jan 2003 GDP
    '    -> Changes for extra volumes
    '
    ' 16 May 2005 JWD
    '  -> Add new operating expense categories OX6-O20.
    '     (C0877)
    '
    Sub LoadFcstINF(ByRef category As String, ByRef parmdata() As ParmType)
        Dim fcstptr As Short
        Dim i As Short
        Dim Count As Short
        Dim arg As String
        '--------------------------------------------------------------------
        '  parameters: category$, parmdata() AS ParmType, found%
        '  parameters in: category$
        '  parameters out: parmdata() AS ParmType
        '  function: searches INF() in common searching for the given category
        '  returns: parmdata() with all of the recs of the given category

        arg = category

        Count = 0
        For i = 1 To INFRecs
            If INF(i).Cat = arg Then
                Count = Count + 1
            End If
        Next i

        If Count = 0 Then
            Select Case arg
                ' GDP 20 JAN 2003
                ' Added OP3 - OP0
                Case "OPC", "GPC", "OP1", "OP2", "OP3", "OP4", "OP5", "OP6", "OP7", "OP8", "OP9", "OP0" 'search for "PRC" in INF to use instead
                    arg = "PRC"
                    ' 16 May 2005 JWD (C0877) Add OX6-O20
                Case "OX1", "OX2", "OX3", "OX4", "OX5", "OX6", "OX7", "OX8", "OX9", "OX0", "O11", "O12", "O13", "O14", "O15", "O16", "O17", "O18", "O19", "O20" 'search for "OPX" in INF to use instead
                    arg = "OPX"
            End Select
            For i = 1 To INFRecs
                If INF(i).Cat = arg Then
                    Count = Count + 1
                End If
            Next i
        End If

        ReDim parmdata(Count)
        If Count > 0 Then
            fcstptr = 0
            For i = 1 To INFRecs
                If INF(i).Cat = arg Then
                    fcstptr = fcstptr + 1
                    parmdata(fcstptr).Cat = INF(i).Cat
                    parmdata(fcstptr).unit = INF(i).unit
                    parmdata(fcstptr).dat = INF(i).dat
                    parmdata(fcstptr).mtd = INF(i).mtd
                    parmdata(fcstptr).parm1 = INF(i).parm1
                    parmdata(fcstptr).parm2 = INF(i).parm2
                    parmdata(fcstptr).parm3 = INF(i).parm3
                    parmdata(fcstptr).parm4 = INF(i).parm4
                    parmdata(fcstptr).parm5 = INF(i).parm5
                    parmdata(fcstptr).parm6 = INF(i).parm6
                End If
            Next i
        End If

    End Sub

    ' $SubTitle:'Read4Gnt - Read Giant 4.0 & 4.1 Data files'
    ' $Page
    '
    ' Modifications:
    ' 19 Feb 1996 JWD
    '  -> Changed month$ and year$ to sMonth and sYear. Month
    '     and Year are functions in VB.
    '
    ' 13 Jun 2001 JWD
    '  -> Replace explicit occurrences of the detail capital
    '     expenditure category code string with the public
    '     symbol. (C0332)
    '  -> Base dimensioning of cpxcat$() and eblcat$() arrays
    '     on number of codes in string as reflected in the
    '     codes string length. (C0332)
    '
    Sub Read4Gnt(ByRef filenum As Short)
        Dim Goodlife As Short
        Dim GiantLife As Short
        Dim ptr As Short
        Dim val8 As Single
        Dim val7 As Single
        Dim val6 As Single
        Dim val5 As Single
        Dim val4 As Single
        Dim val3 As Single
        Dim val2 As Single
        Dim val1 As Single
        Dim amt8 As Single
        Dim amt5 As Single
        Dim y As Single
        Dim m As Single
        Dim j As Short
        Dim d As Single
        Dim bdaptr As Short
        Dim C As Single
        Dim calyear As String
        Dim OILGAS As String
        Dim DiscMo As Short
        Dim DiscYr As Short
        Dim DisMo As Short
        Dim DisYr As Short
        Dim dt4 As String
        Dim dt3 As String
        Dim dt2 As String
        Dim dt1 As String
        Dim z As String
        '--------------------------------------------------------------------
        'reads (filenum%) data file (version 4.0)
        '---------------------------------------------------------
        Dim i As Short
        Dim iA As Short
        Dim iB As Short
        Dim rDum As Single
        Dim sDum As String
        Dim sMonth As String
        Dim sYear As String
        '---------------------------------------------------------
        'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        '		GoSub InitDataFile 'initialize data file items with default values

        casenm = "<NONE>" 'data file name
        casedesc = "" 'data file long description
        Maxlife = gc_nMAXLIFE
        GiantLife = 30 'project life (reports) in years
        LG = 30
        Goodlife = False
        PPR = 1
        ProjYr = 91 : ProjMo = 1 'project start Mo/Yr
        ProdYr = 91 : ProdMo = 1 'production start Mo/Yr
        DisYr = 91 : DisMo = 1 'discovery date mo/yr
        DiscYr = 91 : DiscMo = 1 'discount date
        '********************************************************************
        'variables used by DATA file screens (GntDScrn.BAS)
        '--------------------------------------------------------------------
        'GNLScreen - general parameters screen
        GNL.dt(1) = "01/91" : GNL.dt(2) = "01/91"
        GNL.dt(3) = "01/91" : GNL.dt(4) = "01/91"
        GNL.wdepth = 0
        GNL.pval(1) = 0 : GNL.pval(2) = 8 : GNL.pval(3) = 10
        GNL.pval(4) = 12 : GNL.pval(5) = 15 : GNL.pval(6) = 20
        GNL.dmtd = "FRC" : GNL.og = "OIL"
        GNL.eqvl = 6
        For i = 1 To 4
            GNL.ttl(i) = ""
        Next i
        ' end sub InitDataFile


        'DIM ARRAYS CONTAINING CODES

        '<<<<<< 13 Jun 2001 JWD
        Dim salacc(2) As Single
        Dim eblendt(4) As Single
        Dim mtd(2) As Single
        Dim eblacc(2) As Single

        z = CPXCategoryCodesString
        iA = Len(z) \ 3
        'UPGRADE_WARNING: Lower bound of array cpxcat was changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        Dim cpxcat(iA) As Single
        For i = 1 To iA
            cpxcat(i) = CSng(Mid(z, ((i - 1) * 3) + 1, 3))
        Next i
        '~~~~~~ was:
        'ReDim cpxcat$(20), salacc$(2), eblendt$(4), mtd$(2), eblacc$(2), eblcat$(23)
        '
        'z$ = "BNSLSERENGEOEDHEDSADHASCDNPDVPPLFFCLTRNEORCP1CP2CP3BALBL2BL3"  '20 capex items
        'For i = 1 To 20
        '  cpxcat$(i) = Mid$(z$, ((i - 1) * 3) + 1, 3)
        'Next i
        '>>>>>> End 13 Jun 2001


        mtd(1) = ("TOT") : mtd(2) = ("PRI")
        salacc(1) = ("NO") : salacc(2) = ("YES")
        eblendt(1) = ("PRD") : eblendt(2) = ("BEG")
        eblendt(3) = ("DIS") : eblendt(4) = ("LIF")

        mtd(1) = ("TOT") : mtd(2) = ("PRI")

        eblacc(1) = ("YES") : eblacc(2) = ("NO")

        '<<<<<< 13 Jun 2001 JWD
        z = "ALLEXPDEV" & CPXCategoryCodesString
        iA = Len(z) \ 3
        'UPGRADE_WARNING: Lower bound of array eblcat was changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        Dim eblcat(iA) As Single
        For i = 1 To iA
            eblcat(i) = CSng(Mid(z, ((i - 1) * 3) + 1, 3))
        Next i
        z = ""
        '~~~~~~ was:
        'z$ = "ALLEXPDEVBNSLSERENGEOEDHEDSADHASCDNPDVPPLFFCLTRNEORCP1CP2CP3BALBL2BL3"
        'For i = 1 To 21
        '  eblcat$(i) = Mid$(z$, ((i - 1) * 3) + 1, 3)
        'Next i
        'z$ = ""
        '>>>>>> End 13 Jun 2001

        '--------------------------------------------------------------------
        For i = 1 To 4
            Input(filenum, GNL.ttl(i))
        Next i

        '  FOR i = 1 TO 4
        '    INPUT #filenum%, GNL.dt(i)
        '  NEXT i
        Input(filenum, dt1)
        Input(filenum, dt2)
        Input(filenum, dt3)
        Input(filenum, dt4)
        GNL.dt(1) = Right("0" & RTrim(dt1), 5)
        GNL.dt(2) = Right("0" & RTrim(dt2), 5)
        GNL.dt(3) = Right("0" & RTrim(dt3), 5)
        GNL.dt(4) = Right("0" & RTrim(dt4), 5)

        ProjYr = Val(Right(GNL.dt(1), 2)) : ProjMo = Val(Left(GNL.dt(1), 2))
        DisYr = Val(Right(GNL.dt(2), 2)) : DisMo = Val(Left(GNL.dt(2), 2))
        ProdYr = Val(Right(GNL.dt(3), 2)) : ProdMo = Val(Left(GNL.dt(3), 2))
        DiscYr = Val(Right(GNL.dt(4), 2)) : DiscMo = Val(Left(GNL.dt(4), 2))

        Input(filenum, sDum)
        Input(filenum, sDum)
        Input(filenum, OILGAS)
        Input(filenum, calyear) '''''prccd$(1),prccd$(2),el$(1),el$(2)
        Input(filenum, PDCRecs)
        Input(filenum, BDARecs)
        Input(filenum, CPXRecs)
        Input(filenum, SALRecs)
        Input(filenum, EBLRecs)
        GNL.og = "OIL"
        If OILGAS = "G" Then
            GNL.og = "GAS"
        End If

        DNTRecs = gc_nMAXLIFE

        ReDim PDC(PDCRecs)
        ReDim BDA(BDARecs)
        ReDim CPX(CPXRecs)
        ReDim SAL(SALRecs)
        ReDim EBL(EBLRecs)
        ReDim DNT(DNTRecs)
        '--------------------------------------------------------------------
        'GENERAL PARAMETERS & DISCOUNTING PARAMETERS
        Input(filenum, GNL.wdepth)
        Input(filenum, rDum)
        Input(filenum, rDum) 'gn(1), gn(2), gn(3)
        For i = 1 To 6
            Input(filenum, GNL.pval(i)) 'gn(4-9)
        Next i
        Input(filenum, rDum)
        Input(filenum, rDum) 'gn(10-11)
        '--------------------------------------------------------------------
        'PRODUCTION DECLINE CURVES
        For i = 1 To PDCRecs
            Input(filenum, iA)
            Input(filenum, iB)
            Input(filenum, C)
            Input(filenum, PDC(i).begprod)
            Input(filenum, PDC(i).RATE_Renamed)
            Input(filenum, PDC(i).hypexp)
            Input(filenum, PDC(i).endprod)
            Input(filenum, PDC(i).cumprod)
            Input(filenum, PDC(i).time)
            Select Case iA
                Case 1
                    PDC(i).cat = "OIL"
                Case 2
                    PDC(i).cat = "GAS"
            End Select

            Select Case iB
                Case 1
                    PDC(i).unit = "DAY"
                Case 2
                    PDC(i).unit = "MON"
                Case 3
                    PDC(i).unit = "YRS"
            End Select

            Select Case C
                Case 1
                    PDC(i).mtd = "EXP"
                Case 2
                    PDC(i).mtd = "HYP"
                Case 3
                    PDC(i).mtd = "HAR"
            End Select
        Next i
        For i = 1 To PDCRecs
            If PDC(i).begprod = 0 Then PDC(i).begprod = NULVALUE
            If PDC(i).RATE_Renamed = 0 Then PDC(i).RATE_Renamed = NULVALUE
            If PDC(i).hypexp = 0 Then PDC(i).hypexp = NULVALUE
            If PDC(i).endprod = 0 Then PDC(i).endprod = NULVALUE
            If PDC(i).cumprod = 0 Then PDC(i).cumprod = NULVALUE
            If PDC(i).time = 0 Then PDC(i).time = NULVALUE
        Next i
        '--------------------------------------------------------------------
        'BASE DATA
        bdaptr = 0 'this points to the element in BDA to put the
        '  records read (not all old BDA items are put
        '  into BDA (some go into INF)

        For i = 1 To BDARecs
            bdaptr = bdaptr + 1
            Input(filenum, iA)
            Input(filenum, iB) 'category, unit
            Input(filenum, BDA(bdaptr).mtd)
            Input(filenum, d)
            Input(filenum, BDA(bdaptr).parm2)
            Input(filenum, BDA(bdaptr).parm3)
            Input(filenum, BDA(bdaptr).parm4)
            Input(filenum, BDA(bdaptr).parm5)
            Input(filenum, BDA(bdaptr).parm6)
            BDA(bdaptr).parm1 = FixLen(d, 8)

            BDA(bdaptr).dat = "PJY"

            Select Case iA
                Case 1
                    BDA(bdaptr).Cat = "OIL"
                    sDum = "MMBB/M"
                    BDA(bdaptr).unit = Mid(sDum, ((iB - 1) * 3) + 1, 3)
                Case 2
                    BDA(bdaptr).Cat = "GAS"
                    sDum = "BCFM/B"
                    BDA(bdaptr).unit = Mid(sDum, ((iB - 1) * 3) + 1, 3)
                Case 3
                    BDA(bdaptr).Cat = "OPC"
                    sDum = "$/B$/M"
                    BDA(bdaptr).unit = Mid(sDum, ((iB - 1) * 3) + 1, 3)
                Case 4
                    BDA(bdaptr).Cat = "GPC"
                    BDA(bdaptr).unit = "$/M"
                Case 5
                    BDA(bdaptr).Cat = "OX1"
                    sDum = "$MM$/B$/M"
                    BDA(bdaptr).unit = Mid(sDum, ((iB - 1) * 3) + 1, 3)
                Case 6
                    BDA(bdaptr).Cat = "OX2"
                    sDum = "$MM$/B$/M"
                    BDA(bdaptr).unit = Mid(sDum, ((iB - 1) * 3) + 1, 3)
                Case 7
                    BDA(bdaptr).Cat = "OX3"
                    sDum = "$MM$/B$/M"
                    BDA(bdaptr).unit = Mid(sDum, ((iB - 1) * 3) + 1, 3)
                Case 8
                    BDA(bdaptr).Cat = "OX4"
                    sDum = "$MM$/B$/M"
                    BDA(bdaptr).unit = Mid(sDum, ((iB - 1) * 3) + 1, 3)
                Case 9
                    BDA(bdaptr).Cat = "OX5"
                    sDum = "$MM$/B$/M"
                    BDA(bdaptr).unit = Mid(sDum, ((iB - 1) * 3) + 1, 3)
                Case 10
                    BDA(bdaptr).Cat = "WIN"
                    BDA(bdaptr).unit = " % "
                Case 11 'ING   GOES ON THE INFLATION SCREEN!
                    AddINFRec(iA, bdaptr)
                    bdaptr = bdaptr - 1
                Case 12 'IOX   GOES ON THE INFLATION SCREEN!
                    AddINFRec(iA, bdaptr)
                    bdaptr = bdaptr - 1
                Case 13 'ICX   GOES ON THE INFLATION SCREEN!
                    AddINFRec(iA, bdaptr)
                    bdaptr = bdaptr - 1
                Case 14
                    BDA(bdaptr).Cat = "OV1"
                    sDum = "MMBB/MBCFM/B"
                    BDA(bdaptr).unit = Mid(sDum, ((iB - 1) * 3) + 1, 3)
                Case 15
                    BDA(bdaptr).Cat = "OP1"
                    sDum = "$/B$/M"
                    BDA(bdaptr).unit = Mid(sDum, ((iB - 1) * 3) + 1, 3)
                Case 16
                    BDA(bdaptr).Cat = "OV2"
                    BDA(bdaptr).unit = " % "
                Case 17
                    BDA(bdaptr).Cat = "OP2"
                    sDum = "$/B$/M"
                Case 18 'DFL  GOES ON THE INFLATION SCREEN!
                    AddINFRec(iA, bdaptr)
                    bdaptr = bdaptr - 1
                Case 19
                    BDA(bdaptr).Cat = "RES"
                    BDA(bdaptr).unit = " % "
            End Select
        Next i


        For i = 1 To BDARecs
            'For these categories, the default date is PDM if and only if
            '  the data file said calendar year input = N
            '  (OIL, GAS, OV1, OV2, OX1, OX2, OX3)
            If UCase(calyear) = "N" Then
                Select Case BDA(i).Cat
                    Case "OIL", "GAS", "OV1", "OV2", "OX1", "OX2", "OX3"
                        If InStr(BDA(i).unit, "/") = 0 Then
                            BDA(i).dat = "PDM"
                        End If
                End Select
            End If
            'Also, convert 0s to NULVALUE in parm fields
            If BDA(i).parm4 = 0 Then BDA(i).parm4 = NULVALUE
            If BDA(i).parm5 = 0 Then BDA(i).parm5 = NULVALUE
            If BDA(i).parm6 = 0 Then BDA(i).parm6 = NULVALUE
            Select Case BDA(i).mtd
                Case 1, 4, 6, 8, 0
                    If BDA(i).parm3 = 0 Then BDA(i).parm3 = NULVALUE
                Case 2
                    If BDA(i).parm2 = 0 Then BDA(i).parm2 = NULVALUE
                    If BDA(i).parm3 = 0 Then BDA(i).parm3 = NULVALUE
            End Select
        Next i
        'make sure BDA() is dimmed correctly
        BDARecs = bdaptr
        ReDim Preserve BDA(BDARecs)

        'THERE MAY BE INFLATION RECS, SO now convert 0s to NULVALUE in parm fields
        For j = 1 To INFRecs
            If INF(j).parm4 = 0 Then INF(j).parm4 = NULVALUE
            If INF(j).parm5 = 0 Then INF(j).parm5 = NULVALUE
            If INF(j).parm6 = 0 Then INF(j).parm6 = NULVALUE
            Select Case INF(j).mtd
                Case 1, 4, 6, 8, 0
                    If INF(j).parm3 = 0 Then INF(j).parm3 = NULVALUE
                Case 2
                    If INF(j).parm2 = 0 Then INF(j).parm2 = NULVALUE
                    If INF(j).parm3 = 0 Then INF(j).parm3 = NULVALUE
            End Select
        Next j
        '--------------------------------------------------------------------
        'CAPITAL EXPENDITURES
        For i = 1 To CPXRecs
            Input(filenum, iA)
            Input(filenum, m)
            Input(filenum, y)
            Input(filenum, CPX(i).tan)
            Input(filenum, CPX(i).amt)
            Input(filenum, CPX(i).WIN)
            Input(filenum, CPX(i).remb)
            sMonth = LTrim(Str(m))
            sYear = LTrim(Str(y))
            If Len(sMonth) = 1 Then
                sMonth = "0" & sMonth
            End If
            If Len(sYear) = 1 Then
                sYear = "0" & sYear
            End If
            CPX(i).dat = sMonth & "/" & sYear
            CPX(i).cat = CStr(cpxcat(iA))
        Next i
        For i = 1 To CPXRecs
            If CPX(i).tan = 0 Then CPX(i).tan = NULVALUE
            If CPX(i).amt = 0 Then CPX(i).amt = NULVALUE
            If CPX(i).WIN = 0 Then
                CPX(i).WIN = NULVALUE
            ElseIf CPX(i).WIN = 999 Then  'GIANT 4.0 stored WIN as 999
                CPX(i).WIN = WIN
            End If
            If CPX(i).remb = 0 Then CPX(i).remb = NULVALUE
        Next i

        '--------------------------------------------------------------------
        'SINGLE AMOUNT LOANS
        For i = 1 To SALRecs
            Input(filenum, m)
            Input(filenum, y)
            Input(filenum, SAL(i).amt)
            Input(filenum, SAL(i).Del)
            Input(filenum, amt5)
            Input(filenum, SAL(i).int_Renamed)
            Input(filenum, SAL(i).mth)
            Input(filenum, amt8)
            sMonth = Right("00" & LTrim(Str(m)), 2)
            sYear = Right("00" & LTrim(Str(y)), 2)
            SAL(i).dat = sMonth & "/" & sYear
            SAL(i).mtd = CStr(mtd(amt5))
            SAL(i).acc = CStr(salacc(amt5))
        Next i
        For i = 1 To SALRecs
            If SAL(i).amt = 0 Then SAL(i).amt = NULVALUE
            If SAL(i).Del = 0 Then SAL(i).Del = NULVALUE
            If SAL(i).int_Renamed = 0 Then SAL(i).int_Renamed = NULVALUE
            If SAL(i).mth = 0 Then SAL(i).mth = NULVALUE
        Next i

        '--------------------------------------------------------------------
        'EXPENDITURE BASED LOANS
        For i = 1 To EBLRecs
            Input(filenum, val1)
            Input(filenum, val2)
            Input(filenum, val3)
            Input(filenum, val4)
            Input(filenum, val5)
            Input(filenum, val6)
            Input(filenum, val7)
            Input(filenum, val8)
            EBL(i).cat = CStr(eblcat(val1))
            EBL(i).endt = CStr(eblendt(val2))
            EBL(i).fin = val3
            Select Case val4
                Case 998
                    EBL(i).Del = DSC
                Case 999
                    EBL(i).Del = PRD
                Case Else
                    EBL(i).Del = val4
            End Select
            EBL(i).mtd = CStr(mtd(val5))
            EBL(i).int_Renamed = val6
            EBL(i).mth = val7
            EBL(i).acc = CStr(eblacc(val8))
        Next i
        For i = 1 To EBLRecs
            If EBL(i).fin = 0 Then EBL(i).fin = NULVALUE
            If EBL(i).Del = 0 Then EBL(i).Del = NULVALUE
            If EBL(i).int_Renamed = 0 Then EBL(i).int_Renamed = NULVALUE
            If EBL(i).mth = 0 Then EBL(i).mth = NULVALUE
        Next i

        '--------------------------------------------------------------------
        For i = 1 To DNTRecs
            Input(filenum, DNT(i))
        Next i
        For i = UBound(DNT) To 1 Step -1
            If RTrim(DNT(i)) <> "" Then
                ptr = i
                Exit For
            End If
        Next i
        DNTRecs = ptr
        ReDim Preserve DNT(DNTRecs)
    End Sub

    Sub AddINFRec(ByRef iA As Short, ByRef bdaptr As Short)
        'If we encounter an inflation item in BDA in a version 4 data file, we
        '  add that item to the inflation (INF) array. These items were moved
        '  to INF in version 5.0 - IOX, ICX, ING, DFL

        INFRecs = INFRecs + 1
        ReDim Preserve INF(INFRecs)
        Select Case iA
            Case 11 'ING
                INF(INFRecs).Cat = "PRC"
            Case 12 'IOX
                INF(INFRecs).Cat = "OPX"
            Case 13 'ICX
                INF(INFRecs).Cat = "CPX"
            Case 18 'DFL
                INF(INFRecs).Cat = "DFL"
        End Select
        INF(INFRecs).unit = "%"
        INF(INFRecs).dat = "PJY"
        INF(INFRecs).mtd = BDA(bdaptr).mtd
        INF(INFRecs).parm1 = BDA(bdaptr).parm1
        INF(INFRecs).parm2 = BDA(bdaptr).parm2
        INF(INFRecs).parm3 = BDA(bdaptr).parm3
        INF(INFRecs).parm4 = BDA(bdaptr).parm4
        INF(INFRecs).parm5 = BDA(bdaptr).parm5
        INF(INFRecs).parm6 = BDA(bdaptr).parm6
    End Sub

    ' $SubTitle:'Read5Gnt - Read Giant 5.0 Data files'
    ' $Page
    '
    ' Modifications:
    ' 5 Jul 2001 JWD
    '  -> Update to handle file version 5.4 (addition of
    '     abandonment cost data section). (C0341)
    '
    ' 10 Jul 2001 JWD
    '  -> Add version 5.4 as additional file version for
    '     statements specific to version 5.3 for input of
    '     certain data. Version 5.4 is same as 5.3 with
    '     addition of abandonment expenditure data. Failed
    '     to add 5.4 in several places which resulted in
    '     subscript out of range in ForecastExec. (C0348)
    '
    ' 7 Aug 2001 JWD
    '  -> Add call to ReadAbandonmentInflationData() after
    '     ReadAbandonmentExpenditureData(). (C0374)
    ''

    Sub Read5Gnt(ByRef filenum As Short)
        Dim q As Short
        Dim tfile As Short
        Dim DiscMo As Short
        Dim DiscYr As Short
        Dim DisMo As Short
        Dim DisYr As Short
        Dim i As Short

        If GNL Is Nothing Then
            GNL = New GNLType()
        End If


        '--------------------------------------------------------------------
        'reads Giant data (.GNT) file (version 5.0)
        'called by GntRData sub
        Input(filenum, casedesc) 'case description
        For i = 1 To 4
            Input(filenum, GNL.ttl(i))
        Next i

        For i = 1 To 4
            Input(filenum, GNL.dt(i))
        Next i

        ProjYr = Val(Right(GNL.dt(1), 2)) 'project start Mo/Yr
        ProjMo = Val(Left(GNL.dt(1), 2))
        DisYr = Val(Right(GNL.dt(2), 2)) 'discovery date Mo/Mr
        DisMo = Val(Left(GNL.dt(2), 2))
        ProdYr = Val(Right(GNL.dt(3), 2)) 'production start Mo/Yr
        ProdMo = Val(Left(GNL.dt(3), 2))
        DiscYr = Val(Right(GNL.dt(4), 2)) 'discount date
        DiscMo = Val(Left(GNL.dt(4), 2))

        Input(filenum, GNL.og)
        Input(filenum, GNL.dmtd)
        Input(filenum, GNL.wdepth)
        Input(filenum, GNL.eqvl)
        Input(filenum, PDCRecs)
        Input(filenum, BDARecs)
        Input(filenum, INFRecs)
        Input(filenum, CPXRecs)
        Input(filenum, EBLRecs)
        Input(filenum, SALRecs)
        Input(filenum, DNTRecs)


        'misc titles introduced in version 5.3
        '<<<<<< 10 Jul 2001 JWD (C0348)
        If ver = "VERSION 5.3" Or ver = "VERSION 5.4" Then
            '~~~~~~ was:
            'If ver$ = "VERSION 5.3" Then
            '>>>>>> End (C0348)
            Input(filenum, DTLRecs)
        End If

        ReDim PDC(PDCRecs)
        ReDim BDA(BDARecs)
        ReDim INF(INFRecs)
        ReDim CPX(CPXRecs)
        ReDim SAL(SALRecs)
        ReDim EBL(EBLRecs)
        ReDim DTL(DTLRecs)
        ReDim DNT(DNTRecs)

        For i = 1 To 6
            Input(filenum, GNL.pval(i)) 'dim ok
        Next i

        '<<<<<< 5 Jul 2001 JWD (C0341)
        ' Abandonment data
        If ver = "VERSION 5.4" Then
            ReadAbandonmentExpenditureData(filenum)
            '<<<<<< 7 Aug 2001 JWD (C0374)
            ReadAbandonmentInflationData(filenum)
            '>>>>>> End (C0374)
        End If
        '>>>>>> End (C0341)

        'production decline curve data
        For i = 1 To PDCRecs
            Input(filenum, PDC(i).cat)
            Input(filenum, PDC(i).unit)
            Input(filenum, PDC(i).mtd)
            Input(filenum, PDC(i).begprod)
            Input(filenum, PDC(i).RATE_Renamed)
            Input(filenum, PDC(i).hypexp)
            Input(filenum, PDC(i).endprod)
            Input(filenum, PDC(i).cumprod)
            Input(filenum, PDC(i).time)
        Next i
12350:
        'base data
        For i = 1 To BDARecs
            Input(filenum, BDA(i).Cat)
            Input(filenum, BDA(i).unit)
            Input(filenum, BDA(i).dat)
            Input(filenum, BDA(i).mtd)
            Input(filenum, BDA(i).parm1)
            Input(filenum, BDA(i).parm2)
            Input(filenum, BDA(i).parm3)
            Input(filenum, BDA(i).parm4)
            Input(filenum, BDA(i).parm5)
            Input(filenum, BDA(i).parm6)
        Next i
12360:
        'inflation data
        For i = 1 To INFRecs
            Input(filenum, INF(i).Cat)
            Input(filenum, INF(i).unit)
            Input(filenum, INF(i).dat)
            Input(filenum, INF(i).mtd)
            Input(filenum, INF(i).parm1)
            Input(filenum, INF(i).parm2)
            Input(filenum, INF(i).parm3)
            Input(filenum, INF(i).parm4)
            Input(filenum, INF(i).parm5)
            Input(filenum, INF(i).parm6)
        Next i
12370:
        'capital expenditure data
        For i = 1 To CPXRecs
            Input(filenum, CPX(i).cat)
            Input(filenum, CPX(i).dat)
            Input(filenum, CPX(i).tan)
            Input(filenum, CPX(i).amt)
            Input(filenum, CPX(i).WIN)
            Input(filenum, CPX(i).remb)
            '<<<<<< 10 Jul 2001 JWD (C0348)
            If ver = "VERSION 5.3" Or ver = "VERSION 5.4" Then
                '~~~~~~ was:
                'If ver$ = "VERSION 5.3" Then
                '>>>>>> End (C0348)
                Input(filenum, CPX(i).desc)
            End If
        Next i
12380:
        'expenditure based loan data
        For i = 1 To EBLRecs
            Input(filenum, EBL(i).cat)
            Input(filenum, EBL(i).endt)
            Input(filenum, EBL(i).fin)
            Input(filenum, EBL(i).Del)
            Input(filenum, EBL(i).mtd)
            Input(filenum, EBL(i).int_Renamed)
            Input(filenum, EBL(i).mth)
            Input(filenum, EBL(i).acc)
        Next i

        'single amount loan data
        For i = 1 To SALRecs
            Input(filenum, SAL(i).dat)
            Input(filenum, SAL(i).amt)
            Input(filenum, SAL(i).Del)
            Input(filenum, SAL(i).mtd)
            Input(filenum, SAL(i).int_Renamed)
            Input(filenum, SAL(i).mth)
            Input(filenum, SAL(i).acc)
        Next i
12390:
        'misc titles - introduced version 5.3
        '<<<<<< 10 Jul 2001 JWD (C0348)
        If ver = "VERSION 5.3" Or ver = "VERSION 5.4" Then
            '~~~~~~ was:
            'If ver$ = "VERSION 5.3" Then
            '>>>>>> End (C0348)
            For i = 1 To DTLRecs
                Input(filenum, DTL(i).var)
                Input(filenum, DTL(i).short_Renamed)
                Input(filenum, DTL(i).long_Renamed)
            Next i
        End If
12392:
        'we write the user titles out to disk. These will be read by
        '  DISPLAY.BAS so they can be used as column heads on the reports.
        Dim fUSERTITL As String
        fUSERTITL = TempDir & "USERTITL.DAT"
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Len(Dir(fUSERTITL)) > 0 Then
            Kill(fUSERTITL)
        End If

        '<<<<<< 10 Jul 2001 JWD (C0348)
        If ver = "VERSION 5.3" Or ver = "VERSION 5.4" Then
            '~~~~~~ was:
            'If ver$ = "VERSION 5.3" Then
            '>>>>>> End (C0348)
            tfile = FreeFile()
            FileOpen(tfile, fUSERTITL, OpenMode.Output)
            PrintLine(tfile, DTLRecs)
            For q = 1 To DTLRecs
                WriteLine(tfile, LTrim(RTrim(DTL(q).var)), LTrim(RTrim(DTL(q).short_Renamed)), LTrim(RTrim(DTL(q).long_Renamed)))
            Next q
            FileClose(tfile)
        End If

12394:
        'data file notes
        For i = 1 To DNTRecs
            Input(filenum, DNT(i))
        Next i
12397:
        '<<<<<< 10 Jul 2001 JWD (C0348)
        If ver = "VERSION 5.3" Or ver = "VERSION 5.4" Then
            '~~~~~~ was:
            'If ver$ = "VERSION 5.3" Then        'read OXY specific items
            '>>>>>> End (C0348)
            Input(filenum, HistSer)
            Input(filenum, sType)
            Input(filenum, Constat)
            Input(filenum, Class_Renamed)
            Input(filenum, Country)
            Input(filenum, Block)
            Input(filenum, Field)
            Input(filenum, FieldDesc)
            Input(filenum, Region)
            Input(filenum, OnOff)
            Input(filenum, Success)
            Input(filenum, ID)
            Input(filenum, Who)
            Input(filenum, PriceBase)
            Input(filenum, WinExp)
            Input(filenum, WinNet)
        End If
        'save OXY specific data for OXY Paradox database

        '<<<<<< 10 Jul 2001 JWD (C0348)
        If ver = "VERSION 5.3" Or ver = "VERSION 5.4" Then
            '~~~~~~ was:
            'If ver$ = "VERSION 5.3" Then
            '>>>>>> End (C0348)
            GTitl1 = LTrim(RTrim(GNL.ttl(1)))
            GTitl2 = LTrim(RTrim(GNL.ttl(2)))
            GTitl3 = LTrim(RTrim(GNL.ttl(3)))
            GTitl4 = LTrim(RTrim(GNL.ttl(4)))
            GNTDesc = LTrim(RTrim(casedesc))
        End If

12399:

    End Sub
End Module