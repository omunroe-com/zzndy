Option Strict Off
Option Explicit On
Module CTY2000A
	' Name:        CTY2000A.BAS
	' Function:    Country File Forecasting Support Routines
	'---------------------------------------------------------
	' ********************************************************
	' *            COPYRIGHT - PETROCONSULTANTS, INC.        *
	' *                 1986, 1991, 1995, 1996               *
	' *                  ALL RIGHTS RESERVED                 *
	' ********************************************************
	' *  This program file is proprietary information of     *
	' *  Petroconsultants, Incorporated.  Unauthorized use   *
	' *  for any purpose is prohibited.                      *
	' ********************************************************
	'---------------------------------------------------------
	' This file is modified from CTYFCST2.BAS.
	'---------------------------------------------------------
	' 17 Mar 1995 JWD
	'        Removed procedures: FixLen(), KeyIn(),
	'     SetPrinter(), RealToString(), DeleteStr(),
	'     IntegerToString(), InsertChar(), ReplaceChar(),
	'     GetRecord().
	'
	' 13 Feb 1996 JWD
	'        Changed common block include file from CTYIN.BAS
	'     to CTYIN1.BG
	'        Add explicit declaration of default storage class
	'     as Single.
	'
	' 14 Feb 1996 JWD
	'        Replaced explicit external subroutine declaration
	'     statements with include files CNTYFCST.BI and
	'     CTYFCST2.BI.
	'        Replaced explicit user type definitions with
	'     include files.
	'        Replaced explicit constant declaration of TRUE
	'     and FALSE with TRUFALSE.BC
	'        Removed unreferenced procedure LoadCodes().
	
	' 19 Feb 1996 JWD
	'        Changed CTYStepProj().
	'
	' 20 Feb 1996 JWD
	'        Removed procedure CTYForecastLoadA().  Is a
	'     duplicate of DTAForecastLoadA().
	'
	' 21 Nov 1996 JWD
	'        Modified and renamed from CTYFCST2.BAS.
	'        Removed obsolete Dynamic and Include metacommands.
	'        Corrected CTYForecastStep().  (SCO0014)
	'
	' 25 Aug 1998 JWD
	'  -> Changed CTYVerifyDates().
	'---------------------------------------------------------
	'UPGRADE_NOTE: DefSng A-Z statement was removed. Variables were explicitly declared as type Single. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="92AFD3E3-440D-4D49-A8BF-580D74A8C9F2"'
	'---------------------------------------------------------
	' global (this module) variables AND constants
	
	Dim arraysize, updatea As Short
	'4-22-93 - added to common   DIM SHARED Maxlife%
	
	'---------------------------------------------------------
	
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
	Const ONSHORE As Short = -994
	Const NON As Short = -986
	Const BEG As Short = -985
	Const ORI As Short = -984
	Const REP As Short = -983
	Const NOR As Short = -982
	Const NO As Short = -981
	
	'=========================================================
	
	' $subtitle: 'ConvertUserCode'
	' $Page:
	Sub ConvertUserCode(ByRef arg As String, ByRef l As Short)
		Dim i As Short
		'--------------------------------------------------------------------
		'this sub is called by CTYConvertData.
		'This sub is called if a user defined code is encountered while converting
		'  CLR, PRR, and RTE screen data. The conversion process searches TD$(n,1)
		'  and returns the line number in TD$(n,1) where the variable is
		'  referenced (the VAR column on Fiscal Definition)
		'PARAMETERS:    arg$ - the user code
		'               L% - line number in TD$() where var is referenced
11115: 
		l = 0
		For i = 1 To TDT
			If TD(i, 1) = arg Then
				l = i + 100
			End If
		Next i
11116: 
	End Sub
	
	' $subtitle: 'ConvRealNulls'
	' $Page:
    Sub ConvRealNulls(ByRef arg(,) As Single)
        '--------------------------------------------------------------------
        'examine a real array (2 dimensions) replacing NULVALUE with 0
        '---------------------------------------------------------
        Dim i As Short
        Dim j As Short
        Dim u1 As Short
        Dim u2 As Short
        '---------------------------------------------------------
        u1 = UBound(arg, 1)
        u2 = UBound(arg, 2)
        For i = 1 To u1
            For j = 1 To u2
                If arg(i, j) = NULVALUE Or arg(i, j) = HIVALUE Then
                    arg(i, j) = 0
                End If
            Next j
        Next i

    End Sub
	
	' $subtitle: 'ConvStringNulls'
	' $Page:
	Sub ConvStringNulls(ByRef arg() As String)
		'--------------------------------------------------------------------
		'examine arg(i) in a loop converting CHR$(0)'s to ""
		'---------------------------------------------------------
		Dim i As Short
		Dim u1 As Short
		Dim sNull As String
		'---------------------------------------------------------
		sNull = Chr(0)
		u1 = UBound(arg)
		For i = 1 To u1
			If arg(i) = New String(sNull, Len(arg(i))) Then
				arg(i) = ""
			Else
				arg(i) = Trim(arg(i))
			End If
		Next i
		
	End Sub
	
	' $subtitle: 'ConvStringNulls2'
	' $Page:
    Sub ConvStringNulls2(ByRef arg(,) As String)
        '--------------------------------------------------------------------
        'examine arg(i,j) in a loop converting CHR$(0)'s to ""
        '---------------------------------------------------------
        Dim i As Short
        Dim j As Short
        Dim u1 As Short
        Dim u2 As Short
        Dim sNull As String
        '---------------------------------------------------------
        sNull = Chr(0)
        u1 = UBound(arg, 1)
        u2 = UBound(arg, 2)
        For i = 1 To u1
            For j = 1 To u2
                If arg(i, j) = New String(sNull, Len(arg(i, j))) Then
                    arg(i, j) = ""
                Else
                    arg(i, j) = Trim(arg(i, j))
                End If
            Next j
        Next i

    End Sub
	
	' $subtitle: 'CTYConvertABtoA'
	' $Page:
    Sub CTYConvertABtoA(ByRef ab(,) As Object, ByRef units() As String)
        Dim ptr As Short
        Dim m As Short
        Dim j As Short
        Dim convert As Short
        Dim i As Short
        Dim recs As Short
        '--------------------------------------------------------------------
        'This sub takes AB(LG,15) and (using UNITS$()) fills out a(LG,15)
        'AB() comes in in the units of the category as entered by the user
        'A() is returnes in primary units (ie. MMB, BCF, $MM, etc.)
        '---------------------------------------------------------
        Dim iScale As Short
        '---------------------------------------------------------
12345:

        recs = UBound(units)
        Dim fact(LG) As Object

        For i = 1 To recs
            convert = False
            j = i
            If i < 7 Or i > 10 Then 'do not gosub for prices
                'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'

                For m = 1 To LG
                    'UPGRADE_WARNING: Couldn't resolve default property of object fact(m). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    fact(m) = 1
                Next m
                'see if this category in a ratio of another category
                ptr = 0
                If InStr(units(j), "/") > 0 Then
                    Select Case units(j)
                        Case "M/B", "$/B" 'mcf/bbl, $/bbl
                            ptr = 1 'OIL MMBbl volume
                            iScale = 1
                        Case "B/M", "$/M" 'bbl/mcf, $/mcf
                            ptr = 2 'GAS BCF volume
                            iScale = 0.001
                    End Select
                End If
                'if units$() is a ratio (ptr% > 0) then assign correct
                '  primary category volumes to fact!() (and scale appropriately)
                If ptr > 0 Then
                    convert = True
                    For m = 1 To LG
                        'UPGRADE_WARNING: Couldn't resolve default property of object fact(m). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        fact(m) = A(m, ptr) * iScale
                    Next m
                End If


            End If
            If convert Then
                For m = 1 To LG
                    'UPGRADE_WARNING: Couldn't resolve default property of object fact(m). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    A(m, i) = ab(m, i) * fact(m)
                Next m
            Else
                For m = 1 To LG
                    A(m, i) = ab(m, i)
                Next m
            End If
        Next i

        Exit Sub

        '--------------------------------------------------------------------
TestConvert:
        'This GOSUB examines the units$() for the category and sets the
        '  convert% flag TRUE if the category's values in ab() need to be
        '  converted (and sets ptr% to point at the category in ab() to
        '  multiply the values in ab() against).
        '  IE. if ab(n, 7) [OPC] is in $/B units then the values in
        '  ab(n,7) must be multiplied by the values in a(n,1) [OIL BBLS]
        '  to convert OPC $/Bbl into $mm.

        For m = 1 To LG
            'UPGRADE_WARNING: Couldn't resolve default property of object fact(m). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            fact(m) = 1
        Next m
        'see if this category in a ratio of another category
        ptr = 0
        If InStr(units(j), "/") > 0 Then
            Select Case units(j)
                Case "M/B", "$/B" 'mcf/bbl, $/bbl
                    ptr = 1 'OIL MMBbl volume
                    iScale = 1
                Case "B/M", "$/M" 'bbl/mcf, $/mcf
                    ptr = 2 'GAS BCF volume
                    iScale = 0.001
            End Select
        End If
        'if units$() is a ratio (ptr% > 0) then assign correct
        '  primary category volumes to fact!() (and scale appropriately)
        If ptr > 0 Then
            convert = True
            For m = 1 To LG
                'UPGRADE_WARNING: Couldn't resolve default property of object fact(m). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                fact(m) = A(m, ptr) * iScale
            Next m
        End If

        'UPGRADE_WARNING: Return has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        Return


    End Sub
	
	' $SubTitle:'CTYExponentialDecl - Calc prod using a constant % decline'
	' $Page
	Sub CTYExponentialDecl(ByRef qf As Single, ByRef delay As Single, ByRef qi As Single, ByRef qdecl As Single, ByRef column() As Single)
		Dim qt As Single
		Dim prevq As Single
		Dim i As Short
		Dim t As Single
		Dim q As Single
		Dim dr As Single
		Dim xlog As Single
		'--------------------------------------------------------------------
		
		'     Routine to calculate production using a constant percent decline.
		'     Given:
		'         QI     - initial production rate in units (bbls or mmcf)
		'                  per period
		'         QF     - final production rate in units per period
		'         QDECL  - total reserves to be recovered in this decline
		'         DELAY  - delay of start of production from beginning of first
		'                  production period,  as a fraction of a period
		'                  (0 <= DELAY < 1)
		'     Return:
		'         COLUMN - is the production stream (array)
		'     Local Variables:
		'         DR    - decline rate in percent
		'         XLOG  - logarithm of decline rate
		'         T     - time
		'         QT    - production rate at time T
		'         Q     - cumulative production through current time period.
		'         PREVQ - cumulative production through previous time period.
31100: 
		'Calculate the log of the decline rate and the decline rate...
		xlog = (qf - qi) / qdecl
		dr = System.Math.Exp(xlog)
		q = 0
		t = 0 - delay 'This sets up T so that on first iteration, T = DELAY
		For i = 1 To UBound(column)
			If (qdecl - q) < 0.001 Then
				Exit For
			End If
			prevq = q
			qt = qi * dr ^ (t + i)
			q = (qt - qi) / xlog
			If q > qdecl Then
				q = qdecl
			End If
			column(i) = q - prevq
		Next i
		
	End Sub
	
	' $SubTitle:'CTYForecastBase - Driver sub for BaseData type records'
	' $Page
	Sub CTYForecastBase(ByRef ANNdata() As ParmType, ByRef Datacol() As Single, ByRef curveoffset As Single, ByRef curvelife As Single)
		Dim startmonth As Short
		Dim startyear As Short
		'--------------------------------------------------------------------
		'This routine is called by CTYForecastDispatch.
		'  parameters: (ANNdata() AS ParmType, datacol!(), curveoffset, curvelife)
		'  function: loops through ANNdata(), calls CTYForecastStep,
		'       and returns datacol()
7300: 
		
		Select Case ANNdata(1).dat
			Case "PJY"
				startyear = ProjYr : startmonth = 1
				'//////////////////////////////////////////////////////////////////
				MonthRelative = False
				'//////////////////////////////////////////////////////////////////
			Case "PJM"
				startyear = ProjYr : startmonth = ProjMo
				'//////////////////////////////////////////////////////////////////
				MonthRelative = True
				'//////////////////////////////////////////////////////////////////
			Case "PDY"
				startyear = ProdYr : startmonth = 1
				'//////////////////////////////////////////////////////////////////
				MonthRelative = False
				'//////////////////////////////////////////////////////////////////
			Case "PDM"
				startyear = ProdYr : startmonth = ProdMo
				'//////////////////////////////////////////////////////////////////
				MonthRelative = True
				'//////////////////////////////////////////////////////////////////
		End Select
		CTYForecastStep(ANNdata, startyear, startmonth, Datacol, curveoffset, curvelife)
		
		'//////////////////////////////////////////////////////////////////
		MonthRelative = True
		'//////////////////////////////////////////////////////////////////
7301: 
		
	End Sub
	
	' $SubTitle:'CTYForecastBaseEXT - Driver sub for EXTERNAL TABLE records'
	' $Page
	Sub CTYForecastBaseEXT(ByRef EXTData() As EXBType, ByRef Datacol() As Single, ByRef curveoffset As Single, ByRef curvelife As Single, ByRef startyear As Short)
		Dim startmonth As Short
		'--------------------------------------------------------------------
		'This routine is called by CTYForecastDispatch.
		'  parameters: (bdAData() AS ParmType, datacol!(), curveoffset, curvelife)
		'  function: loops through BDAData(), calls CTYForecastStep,
		'       and returns datacol()
17300: 
		startyear = EXTData(1).dat 'ie. 1991
		startmonth = 1
		CTYForecastStepEXT(EXTData, startyear, startmonth, Datacol, curveoffset, curvelife)
		
	End Sub
	
	'---------------------------------------------------------
	' Modifications:
	' 21 Nov 1996 JWD
	'        Add code to ensure that startperiod is not less
	'     than 1, which causes subscript out of range error
	'     when startyear is production start, startyear is
	'     0-49 (>=2000), and project start is 50-99 (<2000).
	'     (SCO0014)
	'---------------------------------------------------------
	Sub CTYForecastStep(ByRef ANNdata() As ParmType, ByRef startyear As Short, ByRef startmonth As Short, ByRef Datacol() As Single, ByRef curveoffset As Single, ByRef curvelife As Single)
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
		Dim projlife As Single
		Dim Continuous As Short
		Dim periods As Single
		Dim startperiod As Single
		Dim j As Short
		Dim E As Single
		'---------------------------------------------------------
		' Perform standard forecasting methods
		'---------------------------------------------------------
30250: 
		E = 2.71828
        Dim wrkcol(LG) As Single
		
		For j = 1 To UBound(ANNdata)
			
			'figure period in which this data record starts....
			If j = 1 Then 'only calc these values for the first record
				startperiod = startyear - ProjYr + 1
				'<<<<<<<<<<<<<<<<<<<<
				' 21 Nov 1996 JWD Add follow to ensure correctness
				'           of startperiod.
				If startperiod < 1 Then
					startperiod = startperiod + 100
				End If
				'>>>>>>>>>>>>>>>>>>>>
				curveoffset = startperiod - 1 '# of project periods prior to start of this curve
			ElseIf Continuous Then 
				startperiod = startperiod + Int(periods)
			Else 'step method AND j% > 1
				startperiod = startperiod + UBound(wrkcol)
			End If
			
			projlife = LG
			
			Select Case ANNdata(j).mtd
				Case 1 'constant amount, # years
					Continuous = False
					periods = projlife - startperiod + 1
					If ANNdata(j).parm2 <= periods And ANNdata(j).parm2 <> LIFE Then
						periods = ANNdata(j).parm2
						'////////////////////////////////////////////////////////////////////
					Else
						If MonthRelative Then
							AdjustLastYear = True
						End If
						'////////////////////////////////////////////////////////////////////
					End If
					If periods > LG - startperiod + 1 Then
						periods = LG - startperiod + 1
					End If
					
					ReDim wrkcol(periods)
					p1 = LTrim(RTrim(ANNdata(j).parm1))
					If p1 = "" Then
						begamt = qf
					Else
						begamt = Val(ANNdata(j).parm1)
					End If
					For i = 1 To periods
						'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						wrkcol(i) = begamt
					Next i
					'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(UBound()). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					qf = wrkcol(UBound(wrkcol))
				Case 2 'fixed amounts 1-6
30251: Continuous = False
					periods = 6
					Dim dum(periods) As Object
					'UPGRADE_WARNING: Couldn't resolve default property of object dum(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					dum(1) = Val(ANNdata(j).parm1)
					'UPGRADE_WARNING: Couldn't resolve default property of object dum(2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					dum(2) = ANNdata(j).parm2
					'UPGRADE_WARNING: Couldn't resolve default property of object dum(3). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					dum(3) = ANNdata(j).parm3
					'UPGRADE_WARNING: Couldn't resolve default property of object dum(4). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					dum(4) = ANNdata(j).parm4
					'UPGRADE_WARNING: Couldn't resolve default property of object dum(5). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					dum(5) = ANNdata(j).parm5
					'UPGRADE_WARNING: Couldn't resolve default property of object dum(6). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					dum(6) = ANNdata(j).parm6
					For i = 6 To 1 Step -1
						If dum(i) = NULVALUE Then
							periods = periods - 1
						Else
							Exit For
						End If
					Next i
					
					For i = 1 To periods
						If dum(i) = NULVALUE Then
							'UPGRADE_WARNING: Couldn't resolve default property of object dum(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
							dum(i) = 0
						End If
					Next i
					If periods >= LG - startperiod + 1 Then
						periods = LG - startperiod + 1
						'////////////////////////////////////////////////////////////////////
						If MonthRelative Then
							AdjustLastYear = True
						End If
						'////////////////////////////////////////////////////////////////////
						
					End If
					ReDim wrkcol(periods)
					For i = 1 To periods
						'UPGRADE_WARNING: Couldn't resolve default property of object dum(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						wrkcol(i) = dum(i)
					Next i
					'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
					System.Array.Clear(dum, 0, dum.Length)
					'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(UBound()). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					qf = wrkcol(UBound(wrkcol))
				Case 3, 5 'initial amount, esc(mtd 3) / decl(mtd5) rate, # years
30252: Continuous = False
					periods = ANNdata(j).parm3
					If periods = LIFE Then
						periods = LG - startperiod + 1
						'////////////////////////////////////////////////////////////////////
						If MonthRelative Then
							AdjustLastYear = True
						End If
						'////////////////////////////////////////////////////////////////////
						
					End If
					If periods > LG - startperiod + 1 Then
						periods = LG - startperiod + 1
						'////////////////////////////////////////////////////////////////////
						If MonthRelative Then
							AdjustLastYear = True
						End If
						'////////////////////////////////////////////////////////////////////
					End If
					
11234: ReDim wrkcol(periods)
11235: 
					p1 = LTrim(RTrim(ANNdata(j).parm1))
					If p1 = "" Then
						p1 = LTrim(Str(qf))
					ElseIf p1 = Space(Len(p1)) Then 
						p1 = LTrim(Str(qf))
					End If
					'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					wrkcol(1) = Val(p1)
					EscalRate = ANNdata(j).parm2
					If ANNdata(j).mtd = 5 Then
						EscalRate = EscalRate * -1
					End If
					Call CTYStepProj(EscalRate, wrkcol)
					'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(UBound()). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					qf = wrkcol(UBound(wrkcol))
				Case 4, 6 'escalation rate, # years
30260: 
					Continuous = False
					If ANNdata(j).parm2 = LG Then
						periods = projlife - startperiod + 1
					Else
						periods = ANNdata(j).parm2
					End If
					If periods = LIFE Then
						periods = LG - startperiod '+ 1
					End If
					If periods > LG - startperiod + 1 Then
						periods = LG - startperiod + 1
					End If
					ReDim wrkcol(periods)
					If ANNdata(j).mtd = 6 Then
						EscalRate = Val(ANNdata(j).parm1) * -1
					Else
						EscalRate = Val(ANNdata(j).parm1)
					End If
					'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					wrkcol(1) = qf * (1 + EscalRate * 0.01)
					If j > 1 Then
						CTYStepProj(EscalRate, wrkcol)
					End If
					'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(UBound()). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					qf = wrkcol(UBound(wrkcol))
				Case 7, 8, 9, 0 'continuous methods
30270: 
					Continuous = True
					If ANNdata(j).mtd = 7 Or ANNdata(j).mtd = 9 Then
						d = (ANNdata(j).parm2) / 100 'decline rate for this record
						dur = ANNdata(j).parm3 'duration of this record
						p1 = LTrim(RTrim(ANNdata(j).parm1))
						If p1 = "" Then
							p1 = LTrim(Str(qf))
						ElseIf p1 = Space(Len(p1)) Then 
							p1 = LTrim(Str(qf))
						End If
						qi = Val(p1)
					ElseIf ANNdata(j).mtd = 8 Or ANNdata(j).mtd = 0 Then 
						d = Val(ANNdata(j).parm1) / 100 'decline rate for this record
						dur = ANNdata(j).parm2 'duration of this record
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
					If dur = LIFE Then
						dur = LG - startperiod '+ 1
					End If
					If dur >= LG - startperiod + 1 Then
						periods = projlife - startperiod + 1
					Else
						periods = dur
					End If
					If ANNdata(j).mtd = 7 Or ANNdata(j).mtd = 8 Then
						d = -1 * d
					End If
					'avar = nominal decline factor [avar = -ln(1-d)]
					avar = -1 * System.Math.Log(1 - d)
					qf = qi * E ^ (-avar * periods)
					If periods > LG - startperiod Then
						periods = LG - startperiod
					End If
					ReDim wrkcol(Int(periods + 1))
					'QDECL = cum production
					qdecl = (qi / avar) * (1 - (E ^ (-1 * avar * periods)))
					CTYExponentialDecl(qf, delay, qi, qdecl, wrkcol)
			End Select
30280: 
			curvelife = curvelife + periods 'track total duration of this category
			zeroper = startperiod - 1
			If startmonth <> 1 And Not Continuous Then
				delay = (startmonth - 1) / 12!
				For i = 1 To UBound(wrkcol)
					'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					x = wrkcol(i) * delay 'X = amount delayed to next year
30281: 
					If (zeroper + i) <= UBound(Datacol) Then
30282: 
						'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						Datacol(zeroper + i) = Datacol(zeroper + i) + (wrkcol(i) - x)
					End If
30283: 
					If (zeroper + i + 1) <= UBound(Datacol) Then
30284: 
						Datacol(zeroper + i + 1) = Datacol(zeroper + i + 1) + x
					End If
				Next i
			Else
30285: 
				For i = 1 To UBound(wrkcol)
30286: 
					If (zeroper + i) <= UBound(Datacol) Then
30287: 
						'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						Datacol(zeroper + i) = Datacol(zeroper + i) + wrkcol(i)
					End If
				Next i
			End If
		Next j
30290: 
		'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		System.Array.Clear(wrkcol, 0, wrkcol.Length)
		
	End Sub
	
	' $SubTitle:'CTYForecastStepEXT - Forecast Mtds 1-6 - External Tables'
	' $Page
	Sub CTYForecastStepEXT(ByRef EXTData() As EXBType, ByRef startyear As Short, ByRef startmonth As Short, ByRef Datacol() As Single, ByRef curveoffset As Single, ByRef curvelife As Single)
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
		Dim Continuous As Short
		Dim periods As Single
		Dim startperiod As Single
		Dim j As Short
		Dim E As Single
		'--------------------------------------------------------------------
29250: 
		
		E = 2.71828
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
						'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						wrkcol(i) = begamt
					Next i
					'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(UBound()). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					qf = wrkcol(UBound(wrkcol))
					
				Case 2 'fixed amounts 1-6
29251: 
					Continuous = False
					periods = 6
					Dim dum(periods) As Object
					'UPGRADE_WARNING: Couldn't resolve default property of object dum(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					dum(1) = EXTData(j).parm1
					'UPGRADE_WARNING: Couldn't resolve default property of object dum(2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					dum(2) = EXTData(j).parm2
					'UPGRADE_WARNING: Couldn't resolve default property of object dum(3). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					dum(3) = EXTData(j).parm3
					'UPGRADE_WARNING: Couldn't resolve default property of object dum(4). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					dum(4) = EXTData(j).parm4
					'UPGRADE_WARNING: Couldn't resolve default property of object dum(5). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					dum(5) = EXTData(j).parm5
					'UPGRADE_WARNING: Couldn't resolve default property of object dum(6). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					dum(6) = EXTData(j).parm6
					For i = 6 To 1 Step -1
						If dum(i) = NULVALUE Then
							periods = periods - 1
						Else
							Exit For
						End If
					Next i
					
					For i = 1 To periods
						If dum(i) = NULVALUE Then
							'UPGRADE_WARNING: Couldn't resolve default property of object dum(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
							dum(i) = 0
						End If
					Next i
					ReDim wrkcol(periods)
					For i = 1 To periods
						'UPGRADE_WARNING: Couldn't resolve default property of object dum(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						wrkcol(i) = dum(i)
					Next i
					'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
					System.Array.Clear(dum, 0, dum.Length)
					'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(UBound()). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					qf = wrkcol(UBound(wrkcol))
				Case 3, 5 'initial amount, esc(mtd 3) / decl(mtd5) rate, # years
29252: Continuous = False
					periods = EXTData(j).parm3
					ReDim wrkcol(periods)
					p1 = LTrim(RTrim(Str(EXTData(j).parm1)))
					If p1 = "" Then
						p1 = LTrim(Str(qf))
					End If
					'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					wrkcol(1) = Val(p1)
					EscalRate = EXTData(j).parm2
					If EXTData(j).mtd = 5 Then
						EscalRate = EscalRate * -1
					End If
					Call CTYStepProj(EscalRate, wrkcol)
					'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(UBound()). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					qf = wrkcol(UBound(wrkcol))
				Case 4, 6 'escalation rate, # years
29260: Continuous = False
					periods = EXTData(j).parm2
					ReDim wrkcol(periods)
					If EXTData(j).mtd = 6 Then
						EscalRate = EXTData(j).parm1 * -1
					Else
						EscalRate = EXTData(j).parm1
					End If
					'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					wrkcol(1) = qf * (1 + EscalRate * 0.01)
					If j > 1 Then
						CTYStepProj(EscalRate, wrkcol)
					End If
					'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(UBound()). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					qf = wrkcol(UBound(wrkcol))
				Case 7, 8, 9, 0 'continuous methods
29270: Continuous = True
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
					qf = qi * E ^ (-avar * periods)
					ReDim wrkcol(Int(periods + 1))
					qdecl = (qi / avar) * (1 - (E ^ (-1 * avar * periods))) 'QDECL = cum production
					CTYExponentialDecl(qf, delay, qi, qdecl, wrkcol)
			End Select
			
			
29280: 
			curvelife = curvelife + periods 'track total duration of this category
			zeroper = startperiod - 1
			If startmonth <> 1 And Not Continuous Then
				delay = (startmonth - 1) / 12!
				For i = 1 To UBound(wrkcol)
					If zeroper > 0 And zeroper + i + 1 <= UBound(Datacol) Then
						'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						x = wrkcol(i) * delay 'X = amount delayed to next year
						'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						Datacol(zeroper + i) = Datacol(zeroper + i) + (wrkcol(i) - x)
						Datacol(zeroper + i + 1) = Datacol(zeroper + i + 1) + x
					End If
				Next i
			Else 'startmonth = 0 or step method (includes external tables
				For i = 1 To UBound(wrkcol)
					If zeroper + i > 0 And zeroper + i <= UBound(Datacol) Then
						'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
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
	
	' $SubTitle:'CTYLoadExtDataCol'
	' $Page
	Sub CTYLoadExtDataCol(ByRef year1 As Short, ByRef rawdatacol() As Single, ByRef extdatacol() As Single)
		Dim i As Short
		Dim pers As Short
		Dim ptr As Short
		'--------------------------------------------------------------------
		'rawdatacol!() has values of the forecast. The first element in
		'  rawdatacol is the calendar year of the first record if the category
		'  as entered in the external table (ie. 1991).
		'We need to shift rawdatacol!() into the proper elements of extdatacol!().
		'  (extdatacol!()'s first element is projyr%)
16175: 
		'YR = project life (integer) in years (in COMMON)
		'pers% = number of years data to transfer to datacol!()
		'year1% = starting year of the external forecast
		
		
		ReDim Preserve extdatacol(LG) 'size it properly
		If year1 < YR Then 'ext forecast starts before project start year
			
			
			ptr = YR - year1 + 1 'first element in extdatacol() to copy
			pers = UBound(rawdatacol) - ptr + 1
			If pers > LG Then
				pers = LG
			End If
			For i = 1 To pers
				extdatacol(i) = rawdatacol(i - 1 + ptr)
			Next i
		ElseIf year1 > YR Then  'ext forecast starts after project start year
			
			ptr = YR - year1 + 1 'first element in datacol() to copy into
			pers = UBound(rawdatacol) + ptr + 1
			If pers > LG - ptr Then
				pers = LG - ptr
			End If
			For i = 1 To pers
				extdatacol(i) = rawdatacol(i - 1 + ptr)
			Next i
		ElseIf year1 = YR Then  'ext forecast starts on project start year
			pers = LG
			If pers > UBound(rawdatacol) Then
				pers = UBound(rawdatacol)
			End If
			For i = 1 To pers
				extdatacol(i) = rawdatacol(i)
			Next i
		End If
		
	End Sub
	
	' $SubTitle:'CTYStepProj - project values using step escalation method'
	' $Page
	Sub CTYStepProj(ByRef ChgRate As Single, ByRef column() As Single)
		Dim i As Short
		Dim facto As Single
		'--------------------------------------------------------------------
		'Called by CTYForecastStep for methods 3, 4, 5, & 6
		
		'     Routine to project values using a step escalation method.
		'     Initial value should be in column!(1).
		'     Values are escalated according to the escalation rate for
		'     whole periods. This routine is also used for step declines.
		'
		'     Given:
		'         ChgRate - rate of change for steps in percent per period.
		'     Return:
		'         COLUMN  - the projected (output) stream of values.
		'--------------------------------------------------------------------
		'     Local Variables:
		'         FACTOR  - the escalation rate expressed as a factor.
		'--------------------------------------------------------------------
		' Modifications:
		' 19 Feb 1996 JWD
		'          Changed formal parameter RATE to ChgRate. RATE is function
		'       in financial libraries and intrinsic function in VB.
		'--------------------------------------------------------------------
		
		
		facto = 1! + (ChgRate * 0.01)
		
		For i = 2 To UBound(column)
			column(i) = column(i - 1) * facto
		Next i
		
		
	End Sub
	
	'$subtitle: 'CTYVerifyDates'
	'$Page:
	'
	' Modifications:
	' 25 Aug 1998 JWD
	'  -> Change symbol name In$ to strIn$ to eliminate name
	'     conflict with reserved word in VB5.
	'
	Sub CTYVerifyDates(ByRef strIn As String, ByRef MX As Single, ByRef YX As Single)
		Dim S1 As Short
		Dim ERO As Short
		'--------------------------------------------------------------------
		' replaced GOSUB 47100
		' THIS SUBROUTINE VERIFIES THE DATE ENTERED
		
		ERO = 0
		S1 = InStr(strIn, "/")
		If S1 = 2 Or S1 = 3 Then GoTo 47150
		GoTo 47200
47150: MX = Val(Mid(strIn, 1, S1 - 1))
		If MX < 1 Or MX > 12 Then GoTo 47200
		YX = Val(Mid(strIn, S1 + 1, 2))
		If YX < 0 Or YX > 99 Then GoTo 47200
		GoTo 47210
47200: ERO = 1
47210: If YX >= 50 Then YX = YX + 1900
		If YX < 50 Then YX = YX + 2000
		
	End Sub
	
	'$subtitle: 'SearchCodeString'
	'$Page:
	Sub SearchCodeString(ByRef List As String, ByRef arg As String, ByRef cl As Short, ByRef ptr As Short)
		'--------------------------------------------------------------------
		' Search a string (arg$) for a specified code (sVlu) of a
		'  specified length (cl%) and return ptr% telling which
		'  item in the list the arg appears
		'NOTE: ptr% returns 0 if arg$ does not appear in List$
		'EXAMPLE:
		'  assume the following call:
		'    SearchCodeString ("OILGASXYZ", "GAS", 3, ptr%)
		'    ptr% would return as 2 (the second 3 character item
		'    in the list)
		'---------------------------------------------------------
		Dim i As Short
		Dim sTmp As String
		Dim sLst As String
		Dim iLArg As Short
		Dim u As Short
		'---------------------------------------------------------
		'   Const bNew = True
		
		'   If bNew Then
		sTmp = Trim(arg)
		iLArg = Len(sTmp)
		sLst = Trim(List)
		u = Len(sLst)
		
		ptr = 0
		For i = 1 To u Step cl
			If Mid(sLst, i, iLArg) = sTmp Then
				ptr = i
				Exit For
			End If
		Next i
		
		If ptr <> 0 Then
			ptr = (ptr - 1) \ cl + 1
		End If
		
		'   Else
		'
		'      u = Len(List$) \ cl%
		'      ptr% = 0
		'      For i = 1 To u
		'         If Mid$(List$, (i - 1) * cl% + 1, cl%) = arg$ Then
		'            ptr% = i
		'            Exit For
		'         End If
		'      Next i
		'   End If
		
	End Sub
	
	' $SubTitle:'StringToReal! - convert string to real or NULVALUE'
	' $Page
	Function StringToReal(ByRef arg As String) As Single
		'--------------------------------------------------------------------
		'this sub examines a string representation of a real number. If the
		'  string = "" then NULVALUE is placed in the real variable, else the
		'  VAL of the string is placed in the real variable.
		
		If arg = "" Then
			StringToReal = 0 'NULVALUE
		ElseIf arg = "LIFE" Then 
			StringToReal = LIFE
		ElseIf arg = "LIF" Then 
			StringToReal = LIF
		ElseIf arg = "WIN" Then 
			StringToReal = WIN
		ElseIf arg = "PAR" Then 
			StringToReal = PAR
		ElseIf arg = "PDY" Then 
			StringToReal = PDY
		ElseIf arg = "PJY" Then 
			StringToReal = PJY
		ElseIf arg = "PDM" Then 
			StringToReal = PDM
		ElseIf arg = "PJM" Then 
			StringToReal = PJM
		ElseIf arg = "DIS" Then 
			StringToReal = DSC
		ElseIf arg = "CAL" Then 
			StringToReal = CAL
		ElseIf arg = "PRD" Then 
			StringToReal = PRD
		ElseIf arg = "NON" Then 
			StringToReal = NON
		ElseIf arg = "BEG" Then 
			StringToReal = BEG
		ElseIf arg = "ORI" Then 
			StringToReal = ORI
		ElseIf arg = "REP" Then 
			StringToReal = REP
		ElseIf arg = "NOR" Then 
			StringToReal = NOR
		Else
			StringToReal = Val(arg)
		End If
		
	End Function
End Module