Option Strict Off
Option Explicit On
Module UTIL5A
	' Name:        UTIL5A.BAS
	' Function:    Miscellaneous Rates Subroutines
	' Date:        14 Jun 2001 JWD
	'---------------------------------------------------------
	' ********************************************************
	' *             COPYRIGHT © IHS ENERGY GROUP             *
	' *                1991, 1995, 1996, 2001                *
	' *                  ALL RIGHTS RESERVED                 *
	' ********************************************************
	' *   This program file is proprietary information of    *
	' *                  IHS Energy Group                    *
	' *   Unauthorized use for any purpose is prohibited.    *
	' ********************************************************
	'---------------------------------------------------------
	' This file is modified from UTIL5.BAS.
	'-----------------------------------------------------------------------
	' Modifications:
	' 2 Feb 1996 JWD
	'        Changed RetrieveValues().
	'
	' 6 Feb 1996 JWD
	'        Replace explicit Declare Sub statements with
	'     include UTIL5.BI.
	'        Replaced include file CTYIN.BAS with CTYIN1.BG.
	'        Add explicit declaration of default storage class
	'     as Single.
	'
	' 19 Feb 1996 JWD
	'        Changed references to common array RATE() to
	'     PARTRATE().  RATE is reserved function name in VB.
	'        Replace variables True/False with constants
	'     defined in include file TRUFALSE.BI.
	'
	' 31 Oct 1996 JWD
	'        Corrected CalcIrr().  (SCO0006 & SCO0008)
	'
	' 14 Jun 2001 JWD
	'  -> Change value assigned to constant symbol cCPXCats
	'     from literal text to public symbol. (C0332)
	'  -> Changed DefineRatio(). (C0332)
	'
	' 20 Jan 2003 GDP
	'  -> Changed CalcIRR().
	'  -> Changed DefineRatio().
	'  -> Changed cBDACats to include OV3-OV0, OP3-OP0
	'  -> Changed CeilDef().
	'  -> Changed PriceDef().
	'  -> Changed PrintIRRWksheet().
	'  -> Changed RateCalc().
	'  -> Changed Retreive Values.
	
	' 30 Jan 2003 JWD
	'  -> Changed CeilDef(). (C0652)
	'
	' 31 Jan 2003 JWD
	'  -> Changed CalcIRR(). (C0654)
	'
	' 27 Feb 2003 GDP
	'  -> Changed RateCalc()
	'
	' 27 May 2003 JWD
	'  -> Changed cBDACats definition. Added AJ6-AJ0. (C0700)
	'  -> Added symbols for code positions in Rate Parameter
	'     code string that are used to select rate calculation
	'     (C0700)
	'  -> Changed DefineRatio(). (C0700)
	'  -> Changed CalcIRR(). (C0700)
	'  -> Changed PrintIRRWksheet(). (C0700)
	'  -> Changed RateCalc(). (C0700)
	'
	' 5 Jun 2003 JWD
	'  -> Changed PriceDef(). (C0711)
	'
	' 12 Jan 2004 JWD
	'  -> Changed PrintIRRWksheet(). (C0772)
	'
	' 19 Jan 2004 JWD
	'  -> Changed PrintIRRWksheet(). (C0774)
	'
	' 3 Feb 2004 JWD
	'  -> Changed PrintIRRWksheet(). (C0776)
	'
	' 12 May 2005 JWD
	'  -> Changed cBDACats definition. Added A11-A20. (C0876)
	'  -> Changed symbol for last adjustment category from
	'     gc_nRtPrmAJ0 to gc_nRtPrmAJn. (C0876)
	'  -> Changed DefineRatio(). (C0876)
	'  -> Changed RateCalc(). (C0876)
	'
	' 16 May 2005 JWD
	'  -> Changed cBDACats definition. Added OX6-O20. (C0877)
	'  -> Changed DefineRatio(). (C0877)
	'  -> Changed RetrieveValues(). (C0877)
	'
	' 17 May 2005 JWD
	'  -> Changed CalcIRR(). (C0878)
	'
	' 31 May 2005 JWD
	'  -> Changed RetrieveValues(). (C0881)
	'
	' 10 Jun 2008 JWD
	'  -> Changed RateCalc(). (080609-1942-01)
	'  -> Changed RateCalc(). (080609-1936-01)
	'-----------------------------------------------------------------------
	'$DYNAMIC
	'UPGRADE_NOTE: DefSng A-Z statement was removed. Variables were explicitly declared as type Single. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="92AFD3E3-440D-4D49-A8BF-580D74A8C9F2"'
	'$include: 'trufalse.bc'
	
	'$include: 'ctyin1.bg'
	'$include: 'util5.bi'
	
	' The following define symbols for various Rate Parameter
	' codes. These symbolic values are the position in the
	' conversion code string used in loading the Ceiling
	' Rates, Participation Rates, and Variable Rates tables.
	'
	' IMPORTANT NOTE: These code values MUST be kept
	' consistent with the code lists used in CTY1000A.BAS:
	' CTYConvertData() for conversion of the Prm parameter of
	' the Ceiling Rates, Participation Rates and Variable
	' Rates tables. As new parameter codes are added, insert
	' them in the proper place here and base the value on
	' the immediate predecessor.
	'
	Public Const gc_nRtPrmOIL As Short = 1
	
	Public Const gc_nRtPrmOV0 As Integer = gc_nRtPrmOIL + 11
	Public Const gc_nRtPrmPRD As Integer = gc_nRtPrmOV0 + 1
	Public Const gc_nRtPrmOPC As Integer = gc_nRtPrmPRD + 1
	Public Const gc_nRtPrmOP0 As Integer = gc_nRtPrmOPC + 11
	Public Const gc_nRtPrmAJ1 As Integer = gc_nRtPrmOP0 + 1
	'Public Const gc_nRtPrmAJ0 = gc_nRtPrmAJ1 + 9
	'Public Const gc_nRtPrmPRC = gc_nRtPrmAJ0 + 1
	' 12 May 2005 JWD (C0876) Change symbol for last AJ and change expression assigned
	Public Const gc_nRtPrmAJn As Integer = gc_nRtPrmAJ1 + 19 ' Add 10 more
	Public Const gc_nRtPrmPRC As Integer = gc_nRtPrmAJn + 1 ' Change referenced symbol
	Public Const gc_nRtPrmPR1 As Integer = gc_nRtPrmPRC + 1
	Public Const gc_nRtPrmPR5 As Integer = gc_nRtPrmPR1 + 4
	Public Const gc_nRtPrmOT1 As Integer = gc_nRtPrmPR5 + 1
	Public Const gc_nRtPrmOT5 As Integer = gc_nRtPrmOT1 + 4
	Public Const gc_nRtPrmOLC As Integer = gc_nRtPrmOT5 + 1
	Public Const gc_nRtPrmV0C As Integer = gc_nRtPrmOLC + 11
	Public Const gc_nRtPrmCUM As Integer = gc_nRtPrmV0C + 1
	Public Const gc_nRtPrmYRS As Integer = gc_nRtPrmCUM + 1
	Public Const gc_nRtPrmDTE As Integer = gc_nRtPrmYRS + 1
	Public Const gc_nRtPrmCAL As Integer = gc_nRtPrmDTE + 1
	Public Const gc_nRtPrmILD As Integer = gc_nRtPrmCAL + 1
	Public Const gc_nRtPrmRTO As Integer = gc_nRtPrmILD + 1
	Public Const gc_nRtPrmRT1 As Integer = gc_nRtPrmRTO + 1
	Public Const gc_nRtPrmIRR As Integer = gc_nRtPrmRT1 + 1
	
	Public Const gc_nRtPrmPRV As Integer = gc_nRtPrmIRR + 1
	Public Const gc_nRtPrmCUV As Integer = gc_nRtPrmPRV + 1
	
	
	Public Const gc_nRtPrmCID As Short = gc_nRtPrmILD ' mapped to same code position
	
	' Following are symbols for the position of
	' certain Country Annual Forecast vectors
	' in the B() array.
	' These values should be kept consistent with
	' the position of the code in the code string.
	Public Const gc_nBPR1 As Short = 1
	Public Const gc_nBPR5 As Integer = gc_nBPR1 + 4
	Public Const gc_nBOT1 As Integer = gc_nBPR5 + 5
	Public Const gc_nBOT5 As Integer = gc_nBOT1 + 4
	
	' GDP 20 Jan 2003
	' Add OV3-OV0, OP3-OP0
	' Const cBDACats = "OILGASOV1OV2RESWINOPCGPCOP1OP2OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5"
	' 27 May 2003 JWD (C0700) Add AJ6-AJ0
	'Const cBDACats = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0RESWINOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0"
	' 12 May 2005 JWD (C0876) Add A11-A20
	'Const cBDACats = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0RESWINOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0A11A12A13A14A15A16A17A18A19A20"
	' 16 May 2005 JWD (C0877) Add OX6-O20
	Const cBDACats As String = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0RESWINOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0OX1OX2OX3OX4OX5OX6OX7OX8OX9OX0O11O12O13O14O15O16O17O18O19O20AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0A11A12A13A14A15A16A17A18A19A20"
	Const cANNCats As String = "PR1PR2PR3PR4PR5PRTDP1DP2DP3OT1OT2OT3OT4OT5"
	
	'<<<<<< 14 Jun 2001 JWD
	Const cCPXCats As String = CPXCategoryCodesString
	'~~~~~~ was:
	'Const cCPXCats = "BNSLSERENGEOEDHEDSADHASCDNPDVPPLFFCLTRNEORCP1CP2CP3BALBL2BL3"
	'>>>>>> End 14 Jun 2001
	
	'---------------------------------------------------------
	' Modifications:
	' 20 Feb 1996 JWD
	'        Change CPD$ to sCPD, duplicate definition (CPD()).
	'        Change TM$ to sTMV, duplicate definition (TM()).
	'
	' 31 Oct 1996 JWD
	'        Correct zero out of base column when breakpoint
	'     not reached by conditioning zeroing on whether or
	'     not the breakpoint was reached.  (SCO0006)
	'        Replace variable x used to index TD$() with
	'     numvar in If tests that condition call of
	'     PrintIRRWorksheet().  (SCO0008)
	' 14 Dec 2000 GDP
	'     Changed line towards the end of the NewFoundland code from
	'     TotTax(z) = TaxIncome * AltTaxRate(z) to
	'     TotTax(z) = RLD(z) * AltTaxRate(z)
	'
	' 20 Jan 2003 GDP
	'  -> The param variable contents has changed because of the addition on new volumes.
	'     Any comparison using param has been changed.
	
	' 31 Jan 2003 JWD
	'  -> Changed to exclude BAL, BL2, BL3 amounts in IRR
	'     compounding calculation. (C0654)
	'
	' 27 May 2003 JWD
	'  -> Replace literal numbers representing rate parameters
	'     with symbols. (C0700)
	'
	' 17 May 2005 JWD
	'  -> Change to replace explicit individual comparisons
	'     with balance category codes in compounding section
	'     to compare to a range (BAL to BLn). (C0878)
	'---------------------------------------------------------
    Sub CalcIrr(ByRef Numvar As Short, ByRef matchtot As Short, ByRef Breaks() As Object, ByRef BrkRates() As Object, ByRef param As Short, ByRef METHOD As Short, ByRef VarRates() As Single)
        Dim LastFracDate As Single
        Dim zx As Single
        Dim Breaksubcount As Short
        Dim BreakDatesub As Object
        Dim zy As Short
        Dim BreakTotal As Short
        Dim TimetoNext200 As Single
        Dim CumProdatNext100 As Single
        Dim YearofNext100 As Short
        Dim FractionYearNext100 As Single
        Dim YearbeforeNext100 As Short
        Dim TimetoNext100 As Single
        Dim CumProdatPayout As Single
        Dim YearofPayout As Short
        Dim FractionYearPayout As Single
        Dim YearbeforePayout As Short
        Dim TimetoPayout As Single
        Dim TimetoCum200 As Single
        Dim TimetoCum100 As Single
        Dim Reserves20 As Single
        Dim Timeto20Percent As Single
        Dim TimetoCum50 As Single
        Dim EquivalReserves As Single
        Dim Equival As Single
        Dim NFLFile As Single
        Dim TaxIncome As Single
        Dim wj As Short
        Dim Numer2 As Single
        Dim Denom As Single
        Dim Numer1 As Single
        Dim CurrentRate As Single
        Dim CurrentBreak As Single
        Dim CumLastYrTax As Single
        Dim CumSubDen As Single
        Dim CumSubNum As Single
        Dim CumPriDen As Single
        Dim CumPriNum As Single
        Dim LastRate As Single
        Dim LastBreak As Single
        Dim w As Short
        Dim bZeroTheBase As Single
        Dim DumTtls As String
        Dim SecondPos As Short
        Dim fraction As Single
        Dim incrrate As Single
        Dim FinalBase As Single
        Dim CarryOver As Single
        Dim kk As Short
        Dim taxpaid As Single
        Dim YrDt As Single
        Dim YM As Single
        Dim zz As Single
        Dim FirstPos As Short
        Dim Cmpt As Single
        Dim sngLYFinalBase As Single
        Dim CapCost As Object
        Dim CmpBreaks As Object
        Dim TaxBase As Object
        Dim profitsplit As Short
        Dim q As Short
        Dim IncDedBlank As Short
        Dim loop7 As Short
        Dim PgTtl As String
        '---------------------------------------------------------
        On Error GoTo LocalHandler
1400:


        '10/31/96 ---------------------------------------------------------------

        'The methods (SP1-8, YES, NO and SPC) do the following:
        'flag:                         Frac%        PrYearTax%  PrTranchTax%
        '
        '                              Fraction     Deduct      Deduct
        '                              In year      prior       Prior
        '                              break is     year's      tranch's
        '                              reached      taxes       Taxes
        '           Code    method%    (~line 392)  (~line 331) (~line 331)
        '           ----    -------    --------     --------    ----------
        'Old Codes
        '           YES       1        Y                Y           Y   (same as OP1)
        '           NO        2        N                Y           Y   (same as OP5)
        '           SPC       3        Y                Y           Y   (same as OP1)
        'New Codes
        '           SP1       4        Y                Y*          Y
        '           SP2       5        Y                Y           N
        '           SP3       6        Y                N           Y
        '           SP4       7        Y                N           N
        '           SP5       8        N                Y*          Y
        '           SP6       9        N                Y           N
        '           SP7       10       N                N           Y
        '           SP8       11       N                N           N

        ' * - PrYearTax% ignored. PrTranchTax% has precedence and will be done instead.
        '---------------------------------------------------------    end 10/31/96

        '----------------------------------------------------------------------------
        'begin 6/26/98

        'The above table of 10/31/96 has been changed because of the confusion that
        'so many alternatives have caused.  We are now going to only 4 choices for
        'IRR, RTO or RT1 parameters.  The Deduct Prior Tranche Tax choice is being
        'discontinued since it is now assumed to be the same answer as Deduct Prior
        'Tax.  This gets rid of SP2, SP3, SP6 and SP7.

        'For ratios as of March, 1998 the YES and NO were redefined to calculate them
        'differently.  For this version, the YES and NO will be eliminated and replaced
        'by the four valid entries SP1, SP4, SP5, and SP8.

        'We are not changing the codes (YES, NO, SPC, SP1-SP8) or the method numbers
        'associated with the codes so that the user interface does not have to be changed
        'and only minimal changes will be needed in the source code.  We are changing
        'only PrYearTax%, PrTranchTax% and Frac% to correspond with our new definitions.
        '
        'The new definitions are as follows:

        'flag:                         Frac%        PrYearTax%  PrTranchTax%
        '
        '                              Fraction     Deduct      Deduct
        '                              In year      prior       Prior
        '                              break is     year's      tranch's
        '                              reached      taxes       Taxes
        '           Code    method%    (~line 392)  (~line 331) (~line 331)
        '           ----    -------    --------     --------    ----------
        '           YES       1        Y                Y           Y
        '           NO        2        Y                Y           Y
        '           SPC       3        Y                Y           Y
        '           SP1       4        Y                Y           Y*
        '           SP2       5        Y                N           N*
        '           SP3       6        N                Y           Y*
        '           SP4       7        N                N           N*
        '           SP5       8        Y                Y           Y
        '           SP6       9        Y                Y           Y
        '           SP7       10       Y                Y           Y
        '           SP8       11       Y                Y           Y

        '* These four codes - SP1, SP2, SP2, SP4 - represent the four options available.
        '  The other codes are set to the same ones as SP1.

        Dim jj As Short
        Dim y As Short
        Dim yy As Short
        Dim z As Short
        Dim XM As Short ' (C0654)
        '---------------------------------------------------------
        Dim FirstTrancheInNum As Short
        Dim frac As Short
        Dim PrYearTax As Short
        Dim PrTranchTax As Short

        '  OLD CODE now set the three flags depending on the method
        '  (Frac%, PrYearTax%, PrTranchTax%)

        '         Select Case method%
        '           Case 1, 3, 4, 5, 6, 7
        '             Frac% = True
        '           Case Else
        '             Frac% = False
        '         End Select

        '         Select Case method%
        '           Case 1,2,3,4,5,8,9
        '             PrYearTax% = True
        '           Case Else
        '             PrYearTax% = False
        '         End Select
        '         Select Case method%
        '           Case 1, 2, 3, 4, 6, 8, 10
        '             PrTranchTax% = True
        '           Case Else
        '             PrTranchTax% = False
        '         End Select

        'modified 6/26/98 to replace above

        Select Case METHOD
            Case 6, 7
                frac = False
            Case Else
                frac = True
        End Select

        Select Case METHOD
            Case 5, 7
                PrYearTax = False
            Case Else
                PrYearTax = True
        End Select

        Select Case METHOD
            Case 5, 7
                PrTranchTax = False
            Case Else
                PrTranchTax = True
        End Select
        '----------- end of 6/26/98 modifications

        'If it a profit variable, we normally put the first tranche's
        'value into the primary numerator/cashflow column of the
        'worksheet.  However, if it is SP4 or SP8, we don't want to do that.
        '         Select Case method%
        '           Case 7, 11

        FirstTrancheInNum = True 'changed temporarily on 3/18/98.  Original is False.

        '           Case Else
        '             FirstTrancheInNum% = True
        '         End Select

        'These items are needed to print a worksheet detailing
        '  the rate based variables results
        Dim Ttls(14) As String
        Dim WkSht(LG, 14) As Single
        'This array stores the sun of the annual values for the
        '  subsequent variables
        ReDim SubVarsNum(LG)
        ReDim SubVarsDen(LG)

        PgTtl = TD(Numvar, 1)

        'See if Inflate IRR has been set

        Dim InfIrr(LG) As Object
        y = 0
z1400:
        y = y + 1
        If y > TMT Then GoTo 1401
        If sTMV(y) <> FVAR(Numvar) Then GoTo z1400
        If TM(y, 3) = 2 Then
            For z = 1 To LG
                'UPGRADE_WARNING: Couldn't resolve default property of object InfIrr(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                InfIrr(z) = Inflate(z, PPR)
            Next z
        End If

1401:   loop7 = 1

looper7:
        Dim AnnInc(LG) As Object

        'Giant 5.4 Start --------------------------
        'We are going to allow reference to variables that follow
        '  the current line of Fiscal Def for the "forward variables"
        '  problem.  Instead of looping from 1 to the line prior to the
        '  current one,  we need to allow the loop to go from 1 to the
        '  last variable prior to the ITE (iteration end or XLast) line.


        If XIter = 1 Then ' this is executed if within iteration loop
            For jj = 1 To XLast
                'If we are "iterating", we only want prior years for
                '  the current and subsequent variable
                If jj <> Numvar Then 'skip the current variable
                    If jj < Numvar Then 'sum current year values for all variables prior to Numvar
                        For z = 1 To LG
                            If TD(jj, 4) = "+" Then
                                'UPGRADE_WARNING: Couldn't resolve default property of object AnnInc(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object AnnInc(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                AnnInc(z) = AnnInc(z) + RVN(z, jj)
                            End If
                            If TD(jj, 4) = "-" Then
                                'UPGRADE_WARNING: Couldn't resolve default property of object AnnInc(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object AnnInc(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                AnnInc(z) = AnnInc(z) - RVN(z, jj)
                            End If
                        Next z
                    Else
                        'sum values for current and subsequent variable
                        'pick up values for all years prior to current year iteration
                        'NOTE: The canned RTO/RT1 ratios put ALL Subsequent vars
                        '  in the numerator of the ratio.
                        For z = 1 To XYear - 1 '7-19-95 added "- 1"
                            If TD(jj, 4) = "+" Then
                                SubVarsNum(z) = SubVarsNum(z) + RVN(z, jj)
                            End If
                            If TD(jj, 4) = "-" Then
                                SubVarsNum(z) = SubVarsNum(z) - RVN(z, jj)
                            End If
                        Next z
                    End If
                End If
            Next jj
            '--------------Giant 5.4 end -----------------------------
        Else 'this is executed if not in iteration loop
            For jj = 1 To (Numvar - 1)
                For z = 1 To LG
1403:               If TD(jj, 4) = "+" Then
                        'UPGRADE_WARNING: Couldn't resolve default property of object AnnInc(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object AnnInc(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        AnnInc(z) = AnnInc(z) + RVN(z, jj)
                    End If
                    If TD(jj, 4) = "-" Then
                        'UPGRADE_WARNING: Couldn't resolve default property of object AnnInc(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object AnnInc(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        AnnInc(z) = AnnInc(z) - RVN(z, jj)
                    End If
                Next z
            Next jj
        End If

        For z = 1 To LG
            'for RTO & IRR, subtract opex from AnnInc()
            'for RT1, opex is considered part of capital
            ' GDP 20 Jan 2003
            ' Changed the rate parameter for new volumes
            'If param% = 59 Or param% = 61 Then
            ' 27 May 2003 JWD (C0700) Replace numbers with symbols
            If param = gc_nRtPrmRTO Or param = gc_nRtPrmIRR Then
                'UPGRADE_WARNING: Couldn't resolve default property of object AnnInc(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object AnnInc(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                AnnInc(z) = AnnInc(z) - (OPEX(z) * (1 - OPEXRATE(z)) * WIN(z))
            End If
            'add Govt Repayment to AnnInc()
            'UPGRADE_WARNING: Couldn't resolve default property of object AnnInc(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            'UPGRADE_WARNING: Couldn't resolve default property of object AnnInc(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            AnnInc(z) = AnnInc(z) + (TOTPMT(z) * WIN(z))
        Next z

        'Set Inc1() and Ded1(), which will show up as
        'Income and Deductions columns on Variable
        'Output Report
        For z = 1 To LG
            'UPGRADE_WARNING: Couldn't resolve default property of object AnnInc(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            inc1(z) = AnnInc(z)
        Next z
        RetrieveValues("CPX", "", ded1)
        'redefine Ded1() for RT1 only
        ' GDP 20 Jan 2003
        ' Changed the rate parameter for new volumes
        'If param% = 60 Then
        ' 27 May 2003 JWD (C0700) Replace numbers with symbols
        If param = gc_nRtPrmRT1 Then
            For z = 1 To LG
                ded1(z) = ded1(z) + (OPEX(z) * (1 - OPEXRATE(z)) * WIN(z))
            Next z
        End If

        'At this point, we are going to re-define
        '  Inc1() and Ded1() if the user has defined
        '  then himself (or herself) on the RTO/RT1 Ratio
        '  Definition form (RATScreen)

        DefineRatio(Numvar)

        'check if and entries are in Income or deduct columns.
        'If nothing was entered, we will use Inc1 - Ded1 as RLD.
        'If anything was entered, use those values as RLD().
        IncDedEntered(Numvar, IncDedBlank)

        If IncDedBlank = -1 Then
            RLD(1) = inc1(1) - ded1(1)
            For q = 2 To LG
                RLD(q) = inc1(q) - ded1(q) + SubVarsNum(q - 1) - SubVarsDen(q - 1)
            Next q
        End If

        'IF the FISCAL DEF variable is a positive cash variable (ie.
        '  profit oil based on rate of return) we are going to take
        '  the user entries and split them up.  We will calculate
        '  the first line of the entry.  Then we will create entries
        '  that are based on the one we just calculated. This is simulate
        '  the user entering the calculations as two different variables.
        'NOTE: The variable is a profit split variable if either:
        '     1 - The variable is a "+" cashflow variable.
        '     2 - The variable is a " " cashflow variable and the
        '         breakrates rates decrease from item 1 to 2 (this
        '         is an assumption that tax rates increase and profit
        '         splits decrease.)

        'FIRST: see if this is a profit split variable
        profitsplit = False
        If TD(Numvar, 4) = "+" Then
            profitsplit = True
        ElseIf LTrim(TD(Numvar, 4)) = "" Then
            If UBound(BrkRates) > 1 Then
                If BrkRates(1) > BrkRates(2) Then
                    profitsplit = True
                End If
            End If
        End If

        Dim inc2(LG) As Single
        Dim ded2(LG) As Single
        Dim rld2(LG) As Single

        'Next: calculate the value of the first line (ie. 100% rate)
        If profitsplit = True Then

            For q = 1 To LG
                'UPGRADE_WARNING: Couldn't resolve default property of object inc2(q). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                inc2(q) = inc1(q)
                'UPGRADE_WARNING: Couldn't resolve default property of object ded2(q). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                ded2(q) = ded1(q)
                'UPGRADE_WARNING: Couldn't resolve default property of object rld2(q). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                rld2(q) = RLD(q)
                If FirstTrancheInNum = True Then
                    RLD(q) = RLD(q) * BrkRates(1) / 100
                End If
                inc1(q) = inc1(q) + RLD(q)
                'UPGRADE_WARNING: Couldn't resolve default property of object AnnInc(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object AnnInc(q). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                AnnInc(q) = AnnInc(q) + RLD(q)
            Next q
            For q = 2 To matchtot
                BrkRates(q) = (BrkRates(1) - BrkRates(q)) * (100 / BrkRates(1))
            Next q
            BrkRates(1) = 0 'a 0 rate line first
            Breaks(1) = -10
        End If
        'End if the special code for profit splits -------------------
19741:

        'This holds the incremental tax amounts 1-life, 0 to tranches
        Dim TaxAmt(LG, matchtot) As Object

        'Compound Rates are only used for IRR parameter
        'If not using this parameter, then set CPDT = 0
        ' GDP 20 Jan 2003
        ' Changed the rate paramter for new volumes
        'If param% = 59 Or param% = 60 Then    'if RTO or RT1
        ' 27 May 2003 JWD (C0700) Replace numbers with symbols
        If param = gc_nRtPrmRTO Or param = gc_nRtPrmRT1 Then 'if RTO or RT1
            CPDT = 0
        End If

        Dim Cmpline(CPDT) As Object

        For jj = loop7 To matchtot ' loop through each IRR line
            sngLYFinalBase = 0 'used to store the prior year's finalbase for use in fraction calculation
            ReDim TaxBase(LG, CPDT)
            ReDim CmpBreaks(CPDT) ' beginning with the second one
            Cmpt = 0
            FirstPos = 1 'switch for first positive FinalBase for each breakpoint
            ReDim CapCost(LG, 1)
            If CPDT <> 0 Then
                ReDim CapCost(LG, CPDT)
            End If
            'no matter what, CmpBreaks(0) = the standard compound rate from RTE
            'UPGRADE_WARNING: Couldn't resolve default property of object CmpBreaks(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            CmpBreaks(0) = Breaks(jj)
            If CPDT = 0 Then ' if there are no Compund Rates
                For zz = 1 To LG
                    'UPGRADE_WARNING: Couldn't resolve default property of object CapCost(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    CapCost(zz, 0) = ded1(zz)
                Next zz
19745:      Else

                ReDim CmpBreaks(CPDT)
                For z = 1 To CPDT
                    If FVAR(Numvar) = sCPD(z) Then
                        If jj = Int(CPD(z, 2)) Then
                            Cmpt = Cmpt + 1 ' total number of CPD() lines for this variable and Breakpoint
                            'UPGRADE_WARNING: Couldn't resolve default property of object Cmpline(Cmpt). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            Cmpline(Cmpt) = z ' line numbers in CPD() for this variable and Breakpoint
                            'UPGRADE_WARNING: Couldn't resolve default property of object CmpBreaks(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            CmpBreaks(Cmpt) = CPD(z, 3) 'compound rate in matching CPD() line
                        End If
                    End If
                Next z
19749:
                Dim XM0 As Short
                XM0 = 1
                'LOOP THRU CAPEX
                If my3tt = 0 Then
                    XM0 = 2
                End If
                For XM = XM0 To my3tt 'FIND LINE THAT MATCHES IN COMPOUND RATES
                    ' 17 May 2005 JWD (C0878) Change to compare range, rather than individual, increased categories
                    ' Exclude BAL amounts from compounding
                    ' (Assumes all balance categories are sequential and contiguous)
                    If my3(XM, gc_nMY3_CAT) >= CPXCategoryCodeBAL And my3(XM, gc_nMY3_CAT) <= CPXCategoryCodeBLn Then
                        Continue For
                    End If
                    ' was:
                    '' 31 Jan 2003 JWD (C0654) Exclude BAL amounts from compounding
                    'If my3(XM, 1) = CPXCategoryCodeBAL Then GoTo a1350
                    'If my3(XM, 1) = CPXCategoryCodeBL2 Then GoTo a1350
                    'If my3(XM, 1) = CPXCategoryCodeBL3 Then GoTo a1350
                    '' End (C0654)
                    ' End (C0878)
                    YM = 0
a1070:              YM = YM + 1
                    If YM > Cmpt Then
                        GoTo a1110
                    End If
                    'UPGRADE_WARNING: Couldn't resolve default property of object Cmpline(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    If (my3(XM, 1) + 3) = CPD(Cmpline(YM), 1) Then
                        GoTo a1320 'match on specific category
                    End If
                    GoTo a1070
a1110:              'CHECK IF CAPEX CATEGORY DEFINED IN EXP OR DEV
                    YM = 0
a1130:              YM = YM + 1
                    If YM > Cmpt Then
                        GoTo a1230
                    End If
                    If my3(XM, 1) <= 3 Or my3(XM, 1) >= 15 Then
                        GoTo a1230
                    End If
                    If my3(XM, 1) >= 9 And my3(XM, 1) <= 14 Then
                        GoTo a1200
                    End If
                    ' CAPEX IS EXPLORATION - SEE IF EXPLORATION IS DEFINED
                    'UPGRADE_WARNING: Couldn't resolve default property of object Cmpline(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    If CPD(Cmpline(YM), 1) = 2 Then
                        GoTo a1320
                    End If
                    GoTo a1130
a1200:              ' CAPEX IS DEVELOPMENT - SEE IF DEVELOPMENT IS DEFINED
                    'UPGRADE_WARNING: Couldn't resolve default property of object Cmpline(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    If CPD(Cmpline(YM), 1) = 3 Then
                        GoTo a1320
                    End If
                    GoTo a1130
a1230:              'CHECK IF ALL DEFINED
                    YM = 0
a1250:              YM = YM + 1
                    If YM > Cmpt Then GoTo a1290
                    'UPGRADE_WARNING: Couldn't resolve default property of object Cmpline(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    If CPD(Cmpline(YM), 1) = 1 Then GoTo a1320
                    GoTo a1250
19751:
a1290:              'NO MATCH WAS FOUND - THUS THIS EXPENDITURE STAYS IN CapCost()
                    YrDt = my3(XM, 3) - YR + 1
                    'UPGRADE_WARNING: Couldn't resolve default property of object CapCost(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    CapCost(YrDt, 0) = CapCost(YrDt, 0) + (my3(XM, 5) * WINC(XM) * GPRATE(XM))
                    Continue For

a1320:              'XM=LINE# IN CAPEX INPUT, Cmpline(YM) = MATCHING LINE IN CPD
                    YrDt = my3(XM, 3) - YR + 1
                    'UPGRADE_WARNING: Couldn't resolve default property of object CapCost(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    CapCost(YrDt, YM) = CapCost(YrDt, YM) + (my3(XM, 5) * WINC(XM) * GPRATE(XM))
a1350:          Next XM
            End If

            'THIS BEGINS THE STANDARD ROUTINE
            For z = 1 To LG
                taxpaid = 0
                ' GDP 20 Jan 2003
                ' Changed the rate parameter for new volumes
                'If param% = 61 Then        'for IRR calculations
                ' 27 May 2003 JWD (C0700) Replace numbers with symbols
                If param = gc_nRtPrmIRR Then 'for IRR calculations
                    If XIter = 1 And FirstPos = 1 Then
                        'UPGRADE_WARNING: Couldn't resolve default property of object CapCost(z, 0). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object InfIrr(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object TaxBase(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        TaxBase(z, 0) = ((TaxBase(z - 1, 0) + SubVarsNum(z - 1) - SubVarsDen(z - 1)) * (1 + ((Breaks(jj) + InfIrr(z)) / 100))) + inc1(z) - CapCost(z, 0)
                    Else
                        'UPGRADE_WARNING: Couldn't resolve default property of object CapCost(z, 0). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object InfIrr(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object TaxBase(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        TaxBase(z, 0) = (TaxBase(z - 1, 0) * (1 + ((Breaks(jj) + InfIrr(z)) / 100))) + inc1(z) - CapCost(z, 0)
                    End If
                    ' GDP 20 Jan 2003
                    ' Changed the rate parameter for new volumes
                    'ElseIf param% = 59 Or param% = 60 Then    'for RTO & RT1 calculations
                    ' 27 May 2003 JWD (C0700) Replace numbers with symbols
                ElseIf param = gc_nRtPrmRTO Or param = gc_nRtPrmRT1 Then  'for RTO & RT1 calculations
                    If XIter = 1 And FirstPos = 1 Then
                        'UPGRADE_WARNING: Couldn't resolve default property of object CapCost(z, 0). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object TaxBase(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        TaxBase(z, 0) = TaxBase(z - 1, 0) + SubVarsNum(z - 1) + inc1(z) - ((CapCost(z, 0) + SubVarsDen(z - 1)) * Breaks(jj))
                    Else
                        'UPGRADE_WARNING: Couldn't resolve default property of object CapCost(z, 0). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object TaxBase(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        TaxBase(z, 0) = TaxBase(z - 1, 0) + inc1(z) - (CapCost(z, 0) * Breaks(jj))
                    End If
                End If

                If jj <> 1 Then ' subtract any previous tranch taxes
                    For kk = 1 To jj - 1
                        'we assume that the variable we are calculating is a negative cash flow
                        If PrTranchTax = True Then
                            'UPGRADE_WARNING: Couldn't resolve default property of object TaxAmt(z, kk). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            'UPGRADE_WARNING: Couldn't resolve default property of object TaxBase(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            TaxBase(z, 0) = TaxBase(z, 0) - TaxAmt(z, kk)
                            '6-11-96 --------------------------------------------
                        ElseIf PrYearTax = True Then  'deduct prior years values for prior tranches
                            If z > 1 Then
                                'UPGRADE_WARNING: Couldn't resolve default property of object TaxAmt(z - 1, kk). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object TaxBase(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                TaxBase(z, 0) = TaxBase(z, 0) - TaxAmt(z - 1, kk)
                            End If
                            '6-11-96 --------------------------------------------
                        End If
                        'UPGRADE_WARNING: Couldn't resolve default property of object TaxAmt(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        taxpaid = taxpaid + TaxAmt(z, kk)
                    Next kk
                End If
                'Compute Carryover for Compounding Specific Capital Costs
                'UPGRADE_WARNING: Couldn't resolve default property of object TaxBase(z, 0). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                If TaxBase(z, 0) >= 0 Then
                    'UPGRADE_WARNING: Couldn't resolve default property of object TaxBase(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    CarryOver = TaxBase(z, 0)
                    'UPGRADE_WARNING: Couldn't resolve default property of object TaxBase(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    TaxBase(z, 0) = 0
                Else
                    CarryOver = 0
                End If
19755:
                'Compound Specified Capital costs
                For yy = 1 To Cmpt
                    'UPGRADE_WARNING: Couldn't resolve default property of object CapCost(z, yy). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: Couldn't resolve default property of object InfIrr(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: Couldn't resolve default property of object CmpBreaks(yy). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: Couldn't resolve default property of object TaxBase(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    TaxBase(z, yy) = (TaxBase(z - 1, yy) * (1 + ((CmpBreaks(yy) + InfIrr(z)) / 100))) - CapCost(z, yy) + CarryOver
                    'UPGRADE_WARNING: Couldn't resolve default property of object TaxBase(z, yy). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    If TaxBase(z, yy) >= 0 Then
                        'UPGRADE_WARNING: Couldn't resolve default property of object TaxBase(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        CarryOver = TaxBase(z, yy)
                        'UPGRADE_WARNING: Couldn't resolve default property of object TaxBase(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        TaxBase(z, yy) = 0
                    Else
                        CarryOver = 0
                    End If
                Next yy
                'Compute final base
                FinalBase = 0
                For yy = 0 To Cmpt
                    'UPGRADE_WARNING: Couldn't resolve default property of object TaxBase(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    FinalBase = FinalBase + TaxBase(z, yy)
                Next yy
                FinalBase = FinalBase + CarryOver
                'If FinalBase has ever been positive , calculate tax amount
                If FinalBase > 0 Or FirstPos = 0 Then
1407:               incrrate = (BrkRates(jj) - BrkRates(jj - 1)) / (100 - BrkRates(jj - 1))
                    If FirstPos = 1 Then 'first FinalBase > 0 for this breakpoint

                        '6-11-96 commented this out. replaced below with new fraction calculation
                        '     '7-26 -----------------------------------------
                        '                    If PrYearTax% = True Then   'deduct prior year's tax        'IF method% = 1 OR method% = 2 THEN  'YES / NO
                        '                      denum = inc1(z) - ded1(z) + SubVarsNum(z - 1) - SubVarsDen(z - 1) - taxpaid
                        '                    ElseIf PrYearTax% = False Then      'ELSEIF method% = 3 OR method% = 4 THEN  'SPC/SP1
                        '                      denum = inc1(z) - ded1(z) + SubVarsNum(z - 1) - SubVarsDen(z - 1)
                        '                    End If
                        '                    'IF method% = 1 OR method% = 2 THEN  'YES / NO 'deduct prior year's tax
                        '                    '  denum = inc1(z) - ded1(z) + SubVarsNum(z - 1) - SubVarsDen(z - 1) - taxpaid
                        '                    'ELSEIF method% = 3 OR method% = 4 THEN  'SPC/SP1
                        '                    '  denum = inc1(z) - ded1(z) + SubVarsNum(z - 1) - SubVarsDen(z - 1)
                        '                    'END IF
                        '     '7-26 end -------------------------------------

                        If FinalBase - sngLYFinalBase <> 0 Then
                            fraction = FinalBase / (FinalBase - sngLYFinalBase)
                            If fraction > 1 Then
                                fraction = 1
                            End If
                        Else
                            fraction = 0
                        End If


                        'the difference betwen frac% True/false is that
                        '  for false, we dont calc a fractional tax amount
                        '  for the transition year (like the old NO)
                        If frac = False Then
                            fraction = 0
                        End If
                        FirstPos = 0 'reset FirstPos% to zero after first time through
                        SecondPos = 1
                    Else 'all subsequent years after first positive year
                        fraction = 1
                    End If
                    'UPGRADE_WARNING: Couldn't resolve default property of object TaxAmt(z, jj). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    TaxAmt(z, jj) = (RLD(z) - taxpaid) * incrrate * fraction

                    For yy = 0 To Cmpt
                        'UPGRADE_WARNING: Couldn't resolve default property of object TaxBase(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        TaxBase(z, yy) = 0
                    Next yy
                End If
                '6-12-96 Start ----------------------------------------------
                'If the user has put a negatine amount in for the breakpoint,
                'we handle the case differently.  This is a signal to Giant that
                'The Tax rate for the tranch should be used for every year instead
                'of waiting for the base to go positive. This will allow Giant to model
                'as case such as a tax that is 30% until IRR = 10 then change the rate
                'to 40%.  Previously, the user had to model this as 2 different variables.

                'To do this, we are going to re-calculate TaxAmt(z,jj) and finalbase.
                'Final base will be set to zero since it is meaningless in this case.
                If jj = 1 Then 'only do for first tranche
                    If Breaks(jj) < 0 Then 'if breakpoint is negative (special signal)
                        'UPGRADE_WARNING: Couldn't resolve default property of object TaxAmt(z, jj). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        TaxAmt(z, jj) = RLD(z) * (BrkRates(jj) / 100)
                        FinalBase = 0
                    End If
                End If

                '6-12-96 End --------------------------------------------------

                If jj <= 3 Then
                    'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, jj * 2 + 2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    WkSht(z, (jj * 2) + 2) = FinalBase 'tax base amount  12-15-94 ver 5.4
                End If
                sngLYFinalBase = FinalBase 'stores this value for use in next year's fraction

                'if this is the 2nd - nth positive year, we have mistakenly
                '  deducted the compounded cap cost from the base (for the worksheet).
                '  we want to correct the base number on the worksheet.
                'Ideally, we cound just use the FirstPos% variable to tell
                '  us when we are positive, but it was modified above.  As a result,
                '  this code was executing on the first and all subsequent years.
                '  SecondPos% is set to 1 the first positive year.  When we get here
                '  and SecondPos% = 1, we change it back to 0.  This means that
                '  we skip the first pos year and do this only in years 2-LG.
                If SecondPos = 1 Then
                    SecondPos = 0
                Else
                    If FirstPos = 0 Then
                        If jj <= 3 Then
                            ' GDP 20 Jan 2003
                            ' Changed the rate param for new volumes
                            'If param% = 59 Or param% = 60 Then    'for RTO & RT1 calculations
                            ' 27 May 2003 JWD (C0700) Replace numbers with symbols
                            If param = gc_nRtPrmRTO Or param = gc_nRtPrmRT1 Then 'for RTO & RT1 calculations
                                'UPGRADE_WARNING: Couldn't resolve default property of object CapCost(z, 0). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, jj * 2 + 2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                WkSht(z, (jj * 2) + 2) = WkSht(z, (jj * 2) + 5) + (CapCost(z, 0) * Breaks(jj)) - CapCost(z, 0)
                            End If
                        End If
                    End If
                End If
            Next z 'end of loop on year
            '~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
19759:
            '12-15-94 5.4 these two appear in the col heads of the worksheet
            ' GDP 20 Jan 2003
            ' Changed the rate parameter for new volumes
            'If param% = 61 Then   'only do this for IRR
            ' 27 May 2003 JWD (C0700) Replace numbers with symbols
            If param = gc_nRtPrmIRR Then 'only do this for IRR
                If jj <= 3 Then
                    DumTtls = Left(LTrim(RTrim(Str(Breaks(jj)))), 6) 'base cmpd rate
                    'UPGRADE_WARNING: Couldn't resolve default property of object Ttls$(jj * 2 + 2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Ttls((jj * 2) + 2) = LTrim(Left(DumTtls, 5)) & "%"

                    incrrate = (BrkRates(jj) - BrkRates(jj - 1)) / (100 - BrkRates(jj - 1))

                    DumTtls = Left(LTrim(RTrim(Str(incrrate * 100))), 6) 'tax rate
                    'UPGRADE_WARNING: Couldn't resolve default property of object Ttls$(jj * 2 + 3). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Ttls(jj * 2 + 3) = LTrim(Left(DumTtls, 5)) & "%"

                End If
            End If
        Next jj 'end of loop on tranche

        '#######################################################################################
        'We are now out of the loops and are ready to finish it.

19762:

        Dim TotTax(LG) As Object
        For z = 1 To LG
            For y = 1 To matchtot
                'UPGRADE_WARNING: Couldn't resolve default property of object TaxAmt(z, y). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object TotTax(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object TotTax(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                TotTax(z) = TotTax(z) + TaxAmt(z, y)
            Next y
        Next z

        '12-15-94 ver 5.4  capture Inc1(), Ded1(), RLD(), etc for worksheet
        For z = 1 To LG
            'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            WkSht(z, 1) = inc1(z) - ded1(z) 'Primary Cash Flow
            'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            WkSht(z, 2) = SubVarsNum(z) - SubVarsDen(z) 'Subsequent Cash Flow

            'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 3). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            WkSht(z, 3) = RLD(z) 'taxable income column
            'UPGRADE_WARNING: Couldn't resolve default property of object TotTax(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 11). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            WkSht(z, 11) = TotTax(z) 'total column

            If RLD(z) > 0 Then
                'UPGRADE_WARNING: Couldn't resolve default property of object TotTax(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 10). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                WkSht(z, 10) = (TotTax(z) / RLD(z)) * 100
            Else
                'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 10). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                WkSht(z, 10) = 0
            End If

            'This fills in cols 5, 7, 9 (Tax columns)
            For yy = 1 To matchtot
                If yy <= 3 Then 'only do 3 rates
                    'UPGRADE_WARNING: Couldn't resolve default property of object TaxAmt(z, yy). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, yy * 2 + 3). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    WkSht(z, (yy * 2) + 3) = TaxAmt(z, yy)
                End If
            Next yy

            If profitsplit = True Then

                'UPGRADE_WARNING: Couldn't resolve default property of object rld2(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 12). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                WkSht(z, 12) = rld2(z) 'original available profit oil
                'UPGRADE_WARNING: Couldn't resolve default property of object TotTax(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 14). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                WkSht(z, 14) = RLD(z) - TotTax(z) 'net amount of profit oil

                'Make sure Available Profit Oil is not zero
                'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 12). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                If WkSht(z, 12) <> 0 Then
                    'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 12). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 13). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    WkSht(z, 13) = (WkSht(z, 14) / WkSht(z, 12)) * 100
                Else
                    'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 13). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    WkSht(z, 13) = 0
                End If

            Else

                'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 12). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                WkSht(z, 12) = 0
                'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 13). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                WkSht(z, 13) = 0
                'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 14). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                WkSht(z, 14) = 0

            End If

        Next z


        '6/12/96 Start ----------------------------------------------
        'we want to zero out the base column for the second positive - nth years
        For yy = 1 To matchtot
            If yy <= 3 Then 'only do 3 rates
                bZeroTheBase = 0 'False
                For z = 1 To LG
                    'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, yy * 2 + 2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    If WkSht(z, yy * 2 + 2) > 0 Then
                        bZeroTheBase = z + 1 'start zeroing out the following year
                        Exit For
                    End If
                Next z
                '<<<<<<<<<<<<<<
                ' 31 Oct 1996 JWD Add if block around
                '                 loop execution.
                ' Do not zero out base if breakpoint
                ' never reached
                If bZeroTheBase > 0 Then
                    For z = bZeroTheBase To LG
                        'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, yy * 2 + 2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        WkSht(z, (yy * 2) + 2) = 0
                    Next z
                End If
                '>>>>>>>>>>>>>>
            Else
                Exit For
            End If
        Next yy

        '6/12/96 End ------------------------------------------------

        Dim AltDenom(LG) As Single
        Dim AltTaxRate(LG) As Single

        '4/29/98 Begin Alternative Methodology for Profit-Based Rates Calculation
        'This section has been put in so that ratios which get lower over the course
        'of the project life and cross back over a breakpoint that had already been
        'reached, would assume the new rate.  The standard methodology assumes that
        'once a breakpoint is reached and the new rate applied, you can never go back
        'to the original rate.

        'To implement this, we are letting the standard calculations be done completely
        'but are then replacing the ratio and Total Tax column with recalculated
        'numbers.  All of the figures in the other columns are the same.  In addition,
        'we will zero out the three Base and Tax columns on the Profit-based Variable
        'Worksheet since they aren't used for this calculation.

        'To execute this section of code, the user must have entered either RT0 or RT1 in
        'the Variable Rates Parameter column.  This Alternative Methodology is NOT used with the
        'IRR parameter.  It only works with ratios (R-factors).

        ' GDP 20 Jan 2003
        ' Changed the rate param for new volumes
        'If param% = 59 Or param% = 60 Then          'param% must be either RT0(35) or RT1(36)
        ' 27 May 2003 JWD (C0700) Replace numbers with symbols
        Dim AltRatio(LG) As Single
        Dim AltNumer(LG) As Single

        If param = gc_nRtPrmRTO Or param = gc_nRtPrmRT1 Then 'param% must be either RT0(35) or RT1(36)

            Dim BrkRatesFrac(matchtot) As Object



            'First, convert the breakpoint rates into fractions
            For w = 1 To matchtot
                'UPGRADE_WARNING: Couldn't resolve default property of object BrkRatesFrac(w). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                BrkRatesFrac(w) = BrkRates(w) / 100
            Next w

            LastBreak = -999 'This is the breakpoint reached in the prior year
            LastRate = 0 'This is the tax rate corresponding to the breakpoint reached in the prior year

            'if frac%=false then the new tax rate is not applied until the year after
            'the breakpoint is reached.  Because of this, we must assume that the rate
            'to be applied in the first year is the first rate specified.
            'We must do this here before the yearly loop begins

            'UPGRADE_WARNING: Couldn't resolve default property of object BrkRatesFrac(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            'UPGRADE_WARNING: Couldn't resolve default property of object AltTaxRate(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            If frac = False Then AltTaxRate(1) = BrkRatesFrac(1)

            'Begin yearly loop
            For z = 1 To LG
                'Accumulate the numerator and denominator for the ratio calculation
                If z = 1 Then
                    CumPriNum = inc1(z)
                    CumPriDen = ded1(z)
                    CumSubNum = 0
                    CumSubDen = 0
                    CumLastYrTax = 0
                Else
                    CumPriNum = CumPriNum + inc1(z) 'Cumulative Primary Numerator
                    CumPriDen = CumPriDen + ded1(z) 'Cumulative Primary Denominator
                    CumSubNum = CumSubNum + SubVarsNum(z - 1) 'Cumulative Subsequent Numerator
                    CumSubDen = CumSubDen + SubVarsDen(z - 1) 'Cumulative Subsequent Denominator
                    'UPGRADE_WARNING: Couldn't resolve default property of object TotTax(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    CumLastYrTax = CumLastYrTax + TotTax(z) 'Cumulative Tax Paid through Last Year
                End If

                If PrYearTax = True Then 'Decide whether or not to deduct last years tax
                    'UPGRADE_WARNING: Couldn't resolve default property of object AltNumer(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    AltNumer(z) = CumPriNum + CumSubNum - CumLastYrTax 'Final Numerator
                Else
                    'UPGRADE_WARNING: Couldn't resolve default property of object AltNumer(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    AltNumer(z) = CumPriNum + CumSubNum
                End If

                'UPGRADE_WARNING: Couldn't resolve default property of object AltDenom(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                AltDenom(z) = CumPriDen + CumSubDen 'Final Denominator

                'AltRatio() is the ratio for the year.  It includes a reduction
                'of the numerator for last years tax, if applicable

                'UPGRADE_WARNING: Couldn't resolve default property of object AltDenom(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                If AltDenom(z) <> 0 Then
                    'UPGRADE_WARNING: Couldn't resolve default property of object AltDenom(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: Couldn't resolve default property of object AltNumer(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: Couldn't resolve default property of object AltRatio(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    AltRatio(z) = AltNumer(z) / AltDenom(z)
                Else
                    'UPGRADE_WARNING: Couldn't resolve default property of object AltRatio(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    AltRatio(z) = 0
                End If

                'Make sure ratio is NEVER less than zero
                'UPGRADE_WARNING: Couldn't resolve default property of object AltRatio(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                If AltRatio(z) < 0 Then AltRatio(z) = 0

                'Now branch on whether or not the new rate is applied in the current year or next year
                If frac = True Then

                    'When frac%=True, it means that the tax rate is applied within the year in which the corresponding
                    'breakpoint is reached.  A weighted average of the two rates will then be calculated
                    'based on an interpolation of when the new rate was reached.

                    For w = 1 To matchtot 'matchtot% = number of breakpoints
                        If AltRatio(z) >= Breaks(w) Then
                            CurrentBreak = Breaks(w) 'highest breakpoint reached in current year
                            'UPGRADE_WARNING: Couldn't resolve default property of object BrkRatesFrac(w). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            CurrentRate = BrkRatesFrac(w) 'rate associated with current breakpoint
                        End If
                    Next w

                    If CurrentBreak > LastBreak Then

                        'Under normal circumstances, the current breakpoint reached will be greater than
                        'the prior breakpoint reached.  If this is the case, calculate the weighted average
                        'tax rate for this transition year.


                        'UPGRADE_WARNING: Couldn't resolve default property of object AltRatio(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Numer1 = CurrentBreak - AltRatio(z - 1)
                        'UPGRADE_WARNING: Couldn't resolve default property of object AltRatio(z - 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object AltRatio(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Denom = AltRatio(z) - AltRatio(z - 1)
                        'UPGRADE_WARNING: Couldn't resolve default property of object AltRatio(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Numer2 = AltRatio(z) - CurrentBreak


                        If Denom > 0 Then
                            'UPGRADE_WARNING: Couldn't resolve default property of object AltTaxRate(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            AltTaxRate(z) = ((Numer1 / Denom) * LastRate) + ((Numer2 / Denom) * CurrentRate)
                        Else
                            'UPGRADE_WARNING: Couldn't resolve default property of object AltTaxRate(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            AltTaxRate(z) = CurrentRate
                        End If

                        LastBreak = CurrentBreak
                        LastRate = CurrentRate

                        'When a new breakpoint has been reached, assume the new rate will be applicable
                        'for all subsequent years.

                        For wj = z + 1 To LG
                            'UPGRADE_WARNING: Couldn't resolve default property of object AltTaxRate(wj). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            AltTaxRate(wj) = CurrentRate
                        Next wj

                    ElseIf CurrentBreak < LastBreak Then  'if Current Breakpoint < Last Breakpoint

                        'If the breakpoint that is reached for this year is less than a breakpoint already reached,
                        'then we must adjust the algorithm so as to keep the denominator as a positive number.

                        'UPGRADE_WARNING: Couldn't resolve default property of object AltRatio(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Numer1 = AltRatio(z - 1) - LastBreak
                        'UPGRADE_WARNING: Couldn't resolve default property of object AltRatio(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object AltRatio(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Denom = AltRatio(z - 1) - AltRatio(z)
                        'UPGRADE_WARNING: Couldn't resolve default property of object AltRatio(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Numer2 = LastBreak - AltRatio(z)

                        If Denom > 0 Then
                            'UPGRADE_WARNING: Couldn't resolve default property of object AltTaxRate(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            AltTaxRate(z) = ((Numer1 / Denom) * LastRate) + ((Numer2 / Denom) * CurrentRate)
                        Else
                            'UPGRADE_WARNING: Couldn't resolve default property of object AltTaxRate(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            AltTaxRate(z) = CurrentRate
                        End If

                        LastBreak = CurrentBreak
                        LastRate = CurrentRate

                        For wj = z + 1 To LG
                            'UPGRADE_WARNING: Couldn't resolve default property of object AltTaxRate(wj). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            AltTaxRate(wj) = CurrentRate
                        Next wj

                    End If

                Else 'frac%=false
                    'With frac%=false, if you hit a breakpoint within a certain year, the corresponding rate
                    'is not applied until the following year.  There is no weighted average calculated.

                    For w = 1 To matchtot
                        If AltRatio(z) >= Breaks(w) Then

                            For wj = z + 1 To LG
                                'UPGRADE_WARNING: Couldn't resolve default property of object BrkRatesFrac(w). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object AltTaxRate(wj). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                AltTaxRate(wj) = BrkRatesFrac(w)
                            Next wj

                        End If
                    Next w

                End If

                TaxIncome = RLD(z)
                'UPGRADE_WARNING: Couldn't resolve default property of object AltTaxRate(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object TotTax(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                TotTax(z) = TaxIncome * AltTaxRate(z)

                'Now define columns of the worksheet

                'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                WkSht(z, 1) = inc1(z)
                'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                WkSht(z, 2) = ded1(z)
                'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 3). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                WkSht(z, 3) = SubVarsNum(z)
                'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 4). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                WkSht(z, 4) = SubVarsDen(z)

                'UPGRADE_WARNING: Couldn't resolve default property of object AltNumer(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 5). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                WkSht(z, 5) = AltNumer(z)
                'UPGRADE_WARNING: Couldn't resolve default property of object AltDenom(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 6). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                WkSht(z, 6) = AltDenom(z)
                'UPGRADE_WARNING: Couldn't resolve default property of object AltRatio(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 7). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                WkSht(z, 7) = AltRatio(z)
                'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 8). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                WkSht(z, 8) = TaxIncome
                'UPGRADE_WARNING: Couldn't resolve default property of object AltTaxRate(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 9). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                WkSht(z, 9) = AltTaxRate(z) * 100
                'UPGRADE_WARNING: Couldn't resolve default property of object TotTax(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 10). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                WkSht(z, 10) = TotTax(z)


                'fill in remaining columns
                If profitsplit = True Then

                    'UPGRADE_WARNING: Couldn't resolve default property of object rld2(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 11). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    WkSht(z, 11) = rld2(z) 'original available profit oil
                    'UPGRADE_WARNING: Couldn't resolve default property of object TotTax(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 13). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    WkSht(z, 13) = RLD(z) - TotTax(z) 'net amount of profit oil

                    'Make sure Available Profit Oil is not zero
                    'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 11). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    If WkSht(z, 11) <> 0 Then
                        'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 11). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 12). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        WkSht(z, 12) = (WkSht(z, 13) / WkSht(z, 11)) * 100
                    Else
                        'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 12). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        WkSht(z, 12) = 0
                    End If

                Else

                    'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 11). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    WkSht(z, 11) = 0
                    'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 12). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    WkSht(z, 12) = 0
                    'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 13). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    WkSht(z, 13) = 0

                End If

            Next z

        End If


        ' Begin ------------ Special Newfoundland case 18/10/2000 - DS/GDP
        ' This section is only used with the special Newfoundland ratio.
        ' We do this only if the variable name is NFR (generic offshore) or NFT (Terra Nova only).
        ' We are going to redefine the Rates for this variable after simple payout.

        If Left(TD(Numvar, 1), 3) = "NFR" Or Left(TD(Numvar, 1), 3) = "NFT" Then

            Kill(("c:\Newfoundland\NflDebug"))
            NFLFile = FreeFile()
            FileOpen(NFLFile, "c:\Newfoundland\NflDebug", OpenMode.Append)

            'Determine cumulative gross equivalent oil production by year
            Dim CmOil(LG) As Object

            Equival = gn(2)
            If gn(2) <= 0 Then
                Equival = 6
            End If
            ' GDP 20 Jan 2003
            ' Added new volumes to CmOil and Print # statements
            For z = 1 To LG 'Determine cumulative equivalent oil production
                'UPGRADE_WARNING: Couldn't resolve default property of object CmOil(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object CmOil(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                CmOil(z) = CmOil(z - 1) + A(z, gc_nAOIL) + (A(z, gc_nAGAS) / Equival) + A(z, gc_nAOV1) + (A(z, gc_nAOV2) / Equival) + A(z, gc_nAOV3) + (A(z, gc_nAOV4) / Equival) + A(z, gc_nAOV5) + (A(z, gc_nAOV6) / Equival) + A(z, gc_nAOV7) + (A(z, gc_nAOV8) / Equival) + A(z, gc_nAOV9) + (A(z, gc_nAOV0) / Equival)
                PrintLine(NFLFile, "Equivalence Factor = " & Equival)
                PrintLine(NFLFile, "z = " & z & "  Oil Prod = " & A(z, gc_nAOIL) & "  Gas Prod = " & A(z, gc_nAGAS))
                PrintLine(NFLFile, "  OV1 = " & A(z, gc_nAOV1) & "  OV2 = " & A(z, gc_nAOV2))
                PrintLine(NFLFile, "  OV3 = " & A(z, gc_nAOV3) & "  OV4 = " & A(z, gc_nAOV4))
                PrintLine(NFLFile, "  OV5 = " & A(z, gc_nAOV5) & "  OV6 = " & A(z, gc_nAOV6))
                PrintLine(NFLFile, "  OV7 = " & A(z, gc_nAOV7) & "  OV8 = " & A(z, gc_nAOV8))
                PrintLine(NFLFile, "  OV9 = " & A(z, gc_nAOV9) & "  OV0 = " & A(z, gc_nAOV0))
                PrintLine(NFLFile, "  CmOil(z) = " & CmOil(z))

            Next z
            'UPGRADE_WARNING: Couldn't resolve default property of object CmOil(LG). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            EquivalReserves = CmOil(LG)


            'Determine the time that it takes to get to 50 MMBbl of oil production'

            TimetoCum50 = 0
            For z = 1 To LG
                'UPGRADE_WARNING: Couldn't resolve default property of object CmOil(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                If CmOil(z) > 50 Then
                    If TimetoCum50 = 0 Then
                        'UPGRADE_WARNING: Couldn't resolve default property of object CmOil(z - 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object CmOil(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object CmOil(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        TimetoCum50 = z - 1 + ((50 - CmOil(z - 1)) / (CmOil(z) - CmOil(z - 1)))
                        PrintLine(NFLFile, "TimetoCum50 = " & TimetoCum50)
                    End If
                End If
            Next z

            If TimetoCum50 = 0 Then
                PrintLine(NFLFile, "50 MMBbls not reached")
            End If


            'Determine the time that it takes to get to 20% of reserves'

            Timeto20Percent = 0
            Reserves20 = EquivalReserves * 0.2
            For z = 1 To LG
                'UPGRADE_WARNING: Couldn't resolve default property of object CmOil(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                If CmOil(z) > Reserves20 Then
                    If Timeto20Percent = 0 Then
                        'UPGRADE_WARNING: Couldn't resolve default property of object CmOil(z - 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object CmOil(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object CmOil(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Timeto20Percent = z - 1 + ((Reserves20 - CmOil(z - 1)) / (CmOil(z) - CmOil(z - 1)))
                        PrintLine(NFLFile, "Timeto20Percent = " & Timeto20Percent)
                    End If
                End If
            Next z

            'If this is NOT Terra Nova (thus is generic offshore), then see if 20% of reserves occurs before 50 MMBBL
            If Left(TD(Numvar, 1), 3) = "NFR" Then
                'Take lesser of TimetoCum50 and Timeto20Percent and assign to TimetoCum50
                If Timeto20Percent < TimetoCum50 Then
                    TimetoCum50 = Timeto20Percent
                End If
                PrintLine(NFLFile, "Sooner of TimetoCum50 or Timeto20Percent = " & TimetoCum50)
            End If


            'Determine the time that it takes to get to 100 MMBbl of oil production'

            TimetoCum100 = 0
            For z = 1 To LG
                'UPGRADE_WARNING: Couldn't resolve default property of object CmOil(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                If CmOil(z) > 100 Then
                    If TimetoCum100 = 0 Then
                        'UPGRADE_WARNING: Couldn't resolve default property of object CmOil(z - 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object CmOil(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object CmOil(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        TimetoCum100 = z - 1 + ((100 - CmOil(z - 1)) / (CmOil(z) - CmOil(z - 1)))
                        PrintLine(NFLFile, "TimetoCum100 = " & TimetoCum100)
                    End If
                End If
            Next z

            If TimetoCum100 = 0 Then
                PrintLine(NFLFile, "100 MMBbls not reached")
            End If


            'Determine the time that it takes to get to 200 MMBbl of oil production'

            TimetoCum200 = 0
            For z = 1 To LG
                'UPGRADE_WARNING: Couldn't resolve default property of object CmOil(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                If CmOil(z) > 200 Then
                    If TimetoCum200 = 0 Then
                        'UPGRADE_WARNING: Couldn't resolve default property of object CmOil(z - 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object CmOil(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object CmOil(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        TimetoCum200 = z - 1 + ((200 - CmOil(z - 1)) / (CmOil(z) - CmOil(z - 1)))
                        PrintLine(NFLFile, "TimetoCum200 = " & TimetoCum200)
                    End If
                End If
            Next z

            If TimetoCum200 = 0 Then
                PrintLine(NFLFile, "200 MMBbls not reached")
            End If


            ' TimetoPayout is the relative fractional time that it takes for the project to reach payout.
            ' We depend on the RTO being used correctly with the NFR variable.
            ' We only want the first breakpoint.

            TimetoPayout = 0
            For z = 1 To LG
                If TimetoPayout = 0 Then
                    'UPGRADE_WARNING: Couldn't resolve default property of object AltRatio(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    If AltRatio(z) > 1 Then
                        'UPGRADE_WARNING: Couldn't resolve default property of object AltRatio(z - 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object AltRatio(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object AltRatio(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        TimetoPayout = z - 1 + ((1 - AltRatio(z - 1)) / (AltRatio(z) - AltRatio(z - 1)))
                        'UPGRADE_WARNING: Couldn't resolve default property of object AltRatio(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    ElseIf AltRatio(z) = 1 Then
                        TimetoPayout = z
                    End If
                End If
            Next z
            PrintLine(NFLFile, "TimetoPayout = " & TimetoPayout)

            If TimetoPayout = 0 Then
                PrintLine(NFLFile, "Payout not reached" & TimetoPayout)
            End If

            'Determine various things associated with payout
            If TimetoPayout > 0 Then
                YearbeforePayout = Int(TimetoPayout)
                'Print #NFLFile, "YearbeforePayout% = "; YearbeforePayout%
                FractionYearPayout = TimetoPayout - Int(TimetoPayout)
                'Print #NFLFile, "FractionYearPayout = "; FractionYearPayout
                YearofPayout = YearbeforePayout + 1
                'Print #NFLFile, "YearofPayout% = "; YearofPayout%
                ' GDP 20 Jan 2003
                ' A(YearofPayout%, 1) to A(YearofPayout%, gc_nAOIL)
                'UPGRADE_WARNING: Couldn't resolve default property of object CmOil(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                CumProdatPayout = CmOil(YearbeforePayout) + (FractionYearPayout * A(YearofPayout, gc_nAOIL))
                PrintLine(NFLFile, "CumProdatPayout = " & CumProdatPayout)

                'Determine the time at which the next 100 MMBbls after payout would be reached

                Dim CumProdNext100(LG) As Object

                TimetoNext100 = 0
                ' GDP 20 Jan 2003
                ' A(YearofPayout%, 1) to A(YearofPayout%, gc_nAOIL)
                'UPGRADE_WARNING: Couldn't resolve default property of object CumProdNext100(YearofPayout). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                CumProdNext100(YearofPayout) = (1 - FractionYearPayout) * A(YearofPayout, gc_nAOIL)

                'UPGRADE_WARNING: Couldn't resolve default property of object CumProdNext100(YearofPayout). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                If CumProdNext100(YearofPayout) < 100 Then

                    For z = YearofPayout + 1 To LG
                        ' GDP 20 Jan 2003
                        ' A(z, 1) to A(z, gc_nAOIL)
                        'UPGRADE_WARNING: Couldn't resolve default property of object CumProdNext100(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object CumProdNext100(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        CumProdNext100(z) = CumProdNext100(z - 1) + A(z, gc_nAOIL)
                        PrintLine(NFLFile, "z = " & z & "  CumProdNext100(z) = " & CumProdNext100(z))
                        'UPGRADE_WARNING: Couldn't resolve default property of object CumProdNext100(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        If CumProdNext100(z) > 100 Then
                            If TimetoNext100 = 0 Then
                                'UPGRADE_WARNING: Couldn't resolve default property of object CumProdNext100(z - 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object CumProdNext100(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object CumProdNext100(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                TimetoNext100 = z - 1 + ((100 - CumProdNext100(z - 1)) / (CumProdNext100(z) - CumProdNext100(z - 1)))
                                PrintLine(NFLFile, "TimetoNext100 = " & TimetoNext100)
                            End If
                        End If
                    Next z

                    If TimetoNext100 = 0 Then
                        PrintLine(NFLFile, "Payout + 100 MMBbls not reached")
                    End If

                Else 'If the 100 MMBbls are produced in the remaining portion of the year of payout
                    ' A(YearofPayout%, 1) to A(YearofPayout%, gc_nAOIL)
                    TimetoNext100 = TimetoPayout + (100 / A(YearofPayout, gc_nAOIL))
                    PrintLine(NFLFile, "Next 100 MMBbls produced in same year as payout")
                    PrintLine(NFLFile, "TimetoPayout = " & TimetoPayout & "  YearofPayout% = " & YearofPayout & "  TimetoNext100 = " & TimetoNext100)

                End If

            End If


            'For Terra Nova only, Calculate the time to reach Payout + 200 MMBbls
            If Left(TD(Numvar, 1), 3) = "NFT" Then

                'THIS DETERMINES THE SECOND SET OF NEXT 100 MMBBLS AFTER PAYOUT
                'THIS WILL BE REFFERED TO AS THE NEXT 200 MMBBLS
                'Determine various things associated with payout + 100 MMBBLS
                If TimetoNext100 > 0 Then
                    YearbeforeNext100 = Int(TimetoNext100)
                    'Print #NFLFile, "YearbeforeNext100% = "; YearbeforeNext100%
                    FractionYearNext100 = TimetoNext100 - Int(TimetoNext100)
                    'Print #NFLFile, "FractionYearNext100 = "; FractionYearNext100
                    YearofNext100 = YearbeforeNext100 + 1
                    'Print #NFLFile, "YearofNext100% = "; YearofNext100%
                    ' GDP 20 Jan 2003
                    ' A(YearofNext100%, 1) to A(YearofNext100%, gc_nAOIL)
                    'UPGRADE_WARNING: Couldn't resolve default property of object CmOil(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    CumProdatPayout = CmOil(YearbeforeNext100) + (FractionYearNext100 * A(YearofNext100, gc_nAOIL))
                    PrintLine(NFLFile, "CumProdatNext100 = " & CumProdatNext100)

                    'Determine the time at which the second 100 MMBbls after Payout+ 100 MMBbls would be reached

                    Dim CumProdNext200(LG) As Object

                    TimetoNext200 = 0
                    ' GDP 20 Jan 2003
                    ' A(YearofNext100%, 1) to A(YearofNext100%, gc_nAOIL)
                    'UPGRADE_WARNING: Couldn't resolve default property of object CumProdNext200(YearofNext100). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    CumProdNext200(YearofNext100) = (1 - FractionYearNext100) * A(YearofNext100, gc_nAOIL)

                    'UPGRADE_WARNING: Couldn't resolve default property of object CumProdNext200(YearofNext100). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    If CumProdNext200(YearofNext100) < 100 Then

                        For z = YearofNext100 + 1 To LG
                            ' GDP 20 Jan 2003
                            ' A(z, 1) to A(z, gc_nAOIL)
                            'UPGRADE_WARNING: Couldn't resolve default property of object CumProdNext200(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            'UPGRADE_WARNING: Couldn't resolve default property of object CumProdNext200(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            CumProdNext200(z) = CumProdNext200(z - 1) + A(z, gc_nAOIL)
                            PrintLine(NFLFile, "z = " & z & "  CumProdNext200(z) = " & CumProdNext200(z))
                            'UPGRADE_WARNING: Couldn't resolve default property of object CumProdNext200(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            If CumProdNext200(z) > 100 Then
                                If TimetoNext200 = 0 Then
                                    'UPGRADE_WARNING: Couldn't resolve default property of object CumProdNext200(z - 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                    'UPGRADE_WARNING: Couldn't resolve default property of object CumProdNext200(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                    'UPGRADE_WARNING: Couldn't resolve default property of object CumProdNext200(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                    TimetoNext200 = z - 1 + ((100 - CumProdNext200(z - 1)) / (CumProdNext200(z) - CumProdNext200(z - 1)))
                                    PrintLine(NFLFile, "TimetoNext200 = " & TimetoNext200)
                                End If
                            End If
                        Next z

                        If TimetoNext200 = 0 Then
                            PrintLine(NFLFile, "Payout + 200 MMBbls not reached")
                        End If

                    Else 'If the 100 MMBbls are produced in the remaining portion of the year of payout + 100 MMBBLS
                        ' GDP 20 Jan 2003
                        ' A(YearofNext100%, 1) to A(YearofNext100%, gc_nAOIL)
                        TimetoNext200 = TimetoNext100 + (100 / A(YearofNext100, gc_nAOIL))
                        PrintLine(NFLFile, "Second 100 MMBbls produced in same year as first 100 MMBbls.")
                        PrintLine(NFLFile, "TimetoNext100 = " & TimetoNext100 & "  YearofNext100% = " & YearofNext100 & "  TimetoNext200 = " & TimetoNext200)

                    End If

                End If

            End If



            'We now know all of the times that each of the cumulative productions and payouts occur.
            'We can now determine the rates for each year
            'Reset all times that weren't reached to the project life (lg)
            'so that we may line up all of the relevant dates in order.

            If TimetoPayout = 0 Then TimetoPayout = LG
            If TimetoNext100 = 0 Then TimetoNext100 = LG
            If TimetoNext200 = 0 Then TimetoNext200 = LG
            If TimetoCum50 = 0 Then TimetoCum50 = LG
            If TimetoCum100 = 0 Then TimetoCum100 = LG
            If TimetoCum200 = 0 Then TimetoCum200 = LG

            Dim Breakdates(2, 2) As Single

            'Apply these rates for Generic Offshore
            If Left(TD(Numvar, 1), 3) = "NFR" Then

                'Arrange the dates and amounts of rate changes in chronological order
                If TimetoPayout < TimetoCum50 Then
                    ReDim Breakdates(2, 2)
                    Breakdates(1, 1) = TimetoPayout
                    Breakdates(1, 2) = 0.05
                    Breakdates(2, 1) = TimetoNext100
                    Breakdates(2, 2) = 0.075
                    BreakTotal = 2

                ElseIf TimetoPayout < TimetoCum100 Then
                    ReDim Breakdates(3, 2)
                    Breakdates(1, 1) = TimetoCum50
                    Breakdates(1, 2) = 0.025
                    Breakdates(2, 1) = TimetoPayout
                    Breakdates(2, 2) = 0.05
                    Breakdates(3, 1) = TimetoNext100
                    Breakdates(3, 2) = 0.075
                    BreakTotal = 3

                Else 'Payout was not reached before 100 MMBBLS
                    ReDim Breakdates(3, 2)
                    Breakdates(1, 1) = TimetoCum50
                    Breakdates(1, 2) = 0.025
                    Breakdates(2, 1) = TimetoCum100
                    Breakdates(2, 2) = 0.05
                    Breakdates(3, 1) = TimetoCum200
                    Breakdates(3, 2) = 0.075
                    BreakTotal = 3

                End If

            Else 'apply these rates for Terra Nova

                'Arrange the dates and amounts of rate changes in chronological order
                If TimetoPayout < TimetoCum50 Then
                    ReDim Breakdates(3, 2)
                    Breakdates(1, 1) = TimetoPayout
                    Breakdates(1, 2) = 0.05
                    Breakdates(2, 1) = TimetoNext100
                    Breakdates(2, 2) = 0.075
                    Breakdates(3, 1) = TimetoNext200
                    Breakdates(3, 2) = 0.1
                    BreakTotal = 3

                ElseIf TimetoPayout < TimetoCum100 Then
                    ReDim Breakdates(4, 2)
                    Breakdates(1, 1) = TimetoCum50
                    Breakdates(1, 2) = 0.025
                    Breakdates(2, 1) = TimetoPayout
                    Breakdates(2, 2) = 0.05
                    Breakdates(3, 1) = TimetoNext100
                    Breakdates(3, 2) = 0.075
                    Breakdates(4, 1) = TimetoNext200
                    Breakdates(4, 2) = 0.1
                    BreakTotal = 4

                Else 'Payout was not reached before 100 MMBBLS
                    ReDim Breakdates(3, 2)
                    Breakdates(1, 1) = TimetoCum50
                    Breakdates(1, 2) = 0.025
                    Breakdates(2, 1) = TimetoCum100
                    Breakdates(2, 2) = 0.05
                    Breakdates(3, 1) = TimetoCum200
                    Breakdates(3, 2) = 0.075
                    BreakTotal = 3

                End If

            End If


            For zy = 1 To BreakTotal
                PrintLine(NFLFile, "Breakdates(zy%,1) = " & Breakdates(zy, 1) & "  Breakdates(zy%,2) = " & Breakdates(zy, 2))
            Next zy

            ' Now set up the appropriate royalty rates in each year

            LastRate = 0.01
            ReDim AltTaxRate(LG)

            For z = 1 To LG

                ' Determine how many Breakdates occur in this year
                ' and put them into BreakDatesub()

                ReDim BreakDatesub(3, 2)

                Breaksubcount = 0

                For zx = 1 To BreakTotal
                    If (Int(CDbl(Breakdates(zx, 1))) + 1) = z Then
                        Breaksubcount = Breaksubcount + 1
                        'UPGRADE_WARNING: Couldn't resolve default property of object Breakdates(zx, 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object BreakDatesub(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        BreakDatesub(Breaksubcount, 1) = Breakdates(zx, 1)
                        'UPGRADE_WARNING: Couldn't resolve default property of object Breakdates(zx, 2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object BreakDatesub(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        BreakDatesub(Breaksubcount, 2) = Breakdates(zx, 2)
                    End If
                Next zx



                If Breaksubcount = 0 Then
                    'UPGRADE_WARNING: Couldn't resolve default property of object AltTaxRate(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    AltTaxRate(z) = LastRate

                Else

                    For zy = 1 To Breaksubcount
                        '   Print #NFLFile, "z = "; z; "zy% = "; zy%; "  BreakDatesub(zy%,1) = "; BreakDatesub(zy%, 1); "  BreakDatesub(zy%,2) = "; BreakDatesub(zy%, 2)
                    Next zy

                    LastFracDate = z - 1
                    For zx = 1 To Breaksubcount
                        'UPGRADE_WARNING: Couldn't resolve default property of object BreakDatesub(zx, 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object AltTaxRate(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object AltTaxRate(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        AltTaxRate(z) = AltTaxRate(z) + (LastRate * (BreakDatesub(zx, 1) - LastFracDate))
                        'Print #NFLFile, "z = "; z; "zx = "; zx; "  AltTaxRate(z) = "; AltTaxRate(z)
                        'UPGRADE_WARNING: Couldn't resolve default property of object BreakDatesub(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        LastRate = BreakDatesub(zx, 2)
                        'UPGRADE_WARNING: Couldn't resolve default property of object BreakDatesub(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        LastFracDate = BreakDatesub(zx, 1)
                    Next zx
                    'UPGRADE_WARNING: Couldn't resolve default property of object AltTaxRate(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: Couldn't resolve default property of object AltTaxRate(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    AltTaxRate(z) = AltTaxRate(z) + (LastRate * (z - LastFracDate))
                    PrintLine(NFLFile, "z = " & z & "  AltTaxRate(z) = " & AltTaxRate(z))

                End If

            Next z

            'Reset some variables for the RT0 worksheet and the standard variable page

            For z = 1 To LG
                ' GDP 14th December 2000 - Changed TaxIncome in following line to RLD(z)
                'UPGRADE_WARNING: Couldn't resolve default property of object AltTaxRate(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object TotTax(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                TotTax(z) = RLD(z) * AltTaxRate(z)
                'UPGRADE_WARNING: Couldn't resolve default property of object AltTaxRate(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 9). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                WkSht(z, 9) = AltTaxRate(z) * 100
                'UPGRADE_WARNING: Couldn't resolve default property of object TotTax(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object WkSht(z, 10). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                WkSht(z, 10) = TotTax(z)
            Next z


            FileClose(NFLFile)

        End If
        ' End  ------------ Special Newfoundland case 18/10/2000 - DS/GDP

        'We are done with the calculations, now do some housekeeping before we leave
        '------------------------------------------------------------------------
9250:
        ' Set variable rate to pass back to RateCalc
        ' Note that VarRates() contains the Tax Amount, not the Rate()
        For z = 1 To LG
            'UPGRADE_WARNING: Couldn't resolve default property of object TotTax(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            VarRates(z) = TotTax(z)
        Next z

        If profitsplit = True Then
            For z = 1 To LG
                'UPGRADE_WARNING: Couldn't resolve default property of object TotTax(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                VarRates(z) = RLD(z) - TotTax(z)
            Next z
            For q = 1 To LG
                'UPGRADE_WARNING: Couldn't resolve default property of object inc2(q). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                inc1(q) = inc2(q)
                'UPGRADE_WARNING: Couldn't resolve default property of object ded2(q). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                ded1(q) = ded2(q)
                'UPGRADE_WARNING: Couldn't resolve default property of object rld2(q). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                RLD(q) = rld2(q)
            Next q
            'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
            System.Array.Clear(inc2, 0, inc2.Length)
            'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
            System.Array.Clear(ded2, 0, ded2.Length)
            'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
            System.Array.Clear(rld2, 0, rld2.Length)
        End If
        'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        System.Array.Clear(TotTax, 0, TotTax.Length)
        'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        System.Array.Clear(TaxAmt, 0, TaxAmt.Length)
        'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        System.Array.Clear(AnnInc, 0, AnnInc.Length)
        Erase CapCost
        Erase TaxBase
        'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        System.Array.Clear(Cmpline, 0, Cmpline.Length)
        Erase CmpBreaks
1499:
        'print the worksheet
        '          For z = 5 To 14
        '            If Val(Ttls$(z)) > 0 Then
        '              Select Case z Mod 2
        '                Case 1  'odd columns are base columns - add % sign if IRR
        '                  If param% = 37 Then
        '                    Ttls$(z) = LTrim$(Left$(Ttls$(z), 5)) + "%"
        '                  End If
        '                Case 0  'even columns are tax columns - add a % sign always
        '                  Ttls$(z) = LTrim$(Left$(Ttls$(z), 5)) + "%"
        '              End Select
        '            End If
        '          Next z

        If XFirst > 0 And XLast > 0 Then
            If Numvar < XFirst Or Numvar > XLast Then
                '<<<<<<<<<<<<<
                ' 31 Oct 1996 JWD Replace x with numvar - 1/21/97 - DES - should not print worksheet when VAR selected
                If Left(RF(5), 3) = "ALL" And TD(Numvar, 18) <> "NOP" And TD(Numvar, 18) <> "VOP" Then
                    '>>>>>>>>>>>>>
                    PrintIRRWksheet(Ttls, WkSht, PgTtl, param, profitsplit) '6/26/98
                End If
            ElseIf XIter = 1 And XYear = LG Then
                '<<<<<<<<<<<<<
                ' 31 Oct 1996 JWD Replace x with numvar - 1/21/97 - DES - should not print worksheet when VAR selected
                If Left(RF(5), 3) = "ALL" And TD(Numvar, 18) <> "NOP" And TD(Numvar, 18) <> "VOP" Then
                    '>>>>>>>>>>>>>
                    PrintIRRWksheet(Ttls, WkSht, PgTtl, param, profitsplit)
                End If
            End If
        Else 'there is not a loop, print a worksheet for the variable
            '<<<<<<<<<<<<<
            ' 31 Oct 1996 JWD Replace x with numvar - 1/21/97 - DES - should not print worksheet when VAR selected
            If Left(RF(5), 3) = "ALL" And TD(Numvar, 18) <> "NOP" And TD(Numvar, 18) <> "VOP" Then
                '>>>>>>>>>>>>>
                PrintIRRWksheet(Ttls, WkSht, PgTtl, param, profitsplit)
            End If
        End If

        Exit Sub


        '--------------------------------------------------------------
LocalHandler:
        'PRINT #irrlog, "Error: "; ERR; "  Line Number: "; ERL
        '     END
        Resume Next

    End Sub
	
	' $SubTitle:'DefineRatio'
	' $Page
	'
	' Modifications:
	' 25 Aug 1998 JWD
	'  -> Comment out assignment to cashflow$. This appears to
	'     be the only reference to the symbol.
	'
	' 14 Jun 2001 JWD
	'  -> Replace explicit occurrences of the detail capital
	'     expenditure category code string with the public
	'     symbol. (C0332)
	'
	' 20 Jan 2003 GDP
	'  -> Changed definition on DUM$ to include OV3-OV0, OP3-OP0
	'
	' 27 May 2003 JWD
	'  -> Add codes for new adjustment forecast categories
	'     AJ6-AJ0. (C0700)
	'
	' 12 May 2005 JWD
	'  -> Add codes for new adjustment forecast categories
	'     A11-A20. (C0876)
	'
	Sub DefineRatio(ByRef Numvar As Short)
		Dim m As Short
        Dim NoCats As Short
        Dim k As Short
		Dim PreDefined As Short
		Dim stream As String
		Dim SubDataCol As Object
		Dim Datacol As Object
		Dim j As Short
		Dim i As Short
		Dim matchrecs As Short
		Dim share As String
		Dim numver As Single
        Dim var As String
        Dim definedcats() As Object
		'--------------------------------------------------------------------
		'This sub is called by CalcIRR.  It checks to see if the RTO/RT1 Ratio
		'  Definition Screen (RATScreen) has any entries for the current
		'  variable.  If there are, then we want to replace the Inc1() and DED1()
		'  with the values the user has specified.
		'Stream$ is the stream entry from Fiscal Definition ("", GRS, GRP or GVT)
		'When processing the entries, we must look in the A(), B() and MY3()
		'  arrays using RetrieveValues sub.  Then, we search FDEF() (user
		'  variables)for the entry.  If found, we get the values from RVN(). When
		'  pulling in values from RVN(), we must take into account the cashflow
		'  of the item.  If cashflow is "+" or blank, we add the item to the proper
		'  array (Inc1() or Ded1()),  if cashflow = "-", we subtract the values
		'  from the proper array.
		'---------------------------------------------------------
		Dim z As Short
		Dim jj As Short
		'---------------------------------------------------------
		
		'information about the current TD$() variable being processed
		var = TD(Numvar, 1) 'variable code
		'  cashflow$ = TD$(numvar, 4)    'cashflow of current variable
		share = TD(numver, 3) 'share of current variable
1112: 
		'assemble an array of just matching records from RAT()
		Dim RatioRecs(RATRecs) As RATType
		matchrecs = 0
		For i = 1 To RATRecs
			If rat(i).var = var Then 'there is an entry
				matchrecs = matchrecs + 1
				RatioRecs(matchrecs).var = rat(i).var
				RatioRecs(matchrecs).numden = rat(i).numden
				For j = 1 To 8
					RatioRecs(matchrecs).fnc(j) = rat(i).fnc(j)
					RatioRecs(matchrecs).Cat(j) = rat(i).Cat(j)
				Next j
			End If
		Next i
1113: 
		If matchrecs > 0 Then
			ReDim Preserve RatioRecs(matchrecs)
		Else 'no Ratio Definition entries for this variable
1124: 
			'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
			System.Array.Clear(RatioRecs, 0, RatioRecs.Length)
			Exit Sub
		End If
1187: 
		'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        definedcats = BuildListOfDefinedVariables(NoCats)
1114: 
		'since we have entries, flush Inc1(), Ded1() and SubVarsNum()
		'  so we can redefine them
		ReDim inc1(LG)
		ReDim ded1(LG)
		ReDim SubVarsNum(LG)
        ReDim SubVarsDen(LG)
        ReDim definedcats(NoCats)
		
		'now, begin retrieving the values
		For i = 1 To matchrecs 'process each line
			For j = 1 To 8 'loop through the 8 fields on the line
				'if the current category is blank, skip the rest of this line
				If LTrim(RatioRecs(i).Cat(j)) = "" Then
					Exit For
				End If
1115: 
				'Datacol!() stores the values for the variable
				'SubDAtaCol!() satores the value if the variable is a subsequent variable
				ReDim Datacol(LG)
				ReDim SubDataCol(LG)
				
				'if the category is from A(), B() or MY3(), this call will get the values
				'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
                'GoSub CheckIfPredefined

                'This gosub checks if user entry is a pre-defined category.
                '  If not, it must be a user defined variable.
                PreDefined = 0 'False
                For m = 1 To UBound(definedcats)
                    If RatioRecs(i).Cat(j) = definedcats(m) Then
                        PreDefined = True
                        Exit For
                    End If
                Next m


				If PreDefined = True Then
					stream = TD(Numvar, 3)
					'UPGRADE_WARNING: Couldn't resolve default property of object Datacol(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					RetrieveValues(RatioRecs(i).Cat(j), stream, Datacol())
				Else
					'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'

                    For jj = 1 To TDT
                        If TD(jj, 1) = RatioRecs(i).Cat(j) Then
                            'This variable matches the Ratio Definition code, get its values
                            If XIter = 1 Then ' this is executed if within iteration loop
                                'If we are "iterating", we only want prior years for
                                '  the current and subsequent variable
                                If jj <> Numvar Then 'skip the current variable
                                    If jj < Numvar Then 'sum current year values for all variables prior to Numvar
                                        For z = 1 To LG
                                            'UPGRADE_WARNING: Couldn't resolve default property of object Datacol(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                            Datacol(z) = Datacol(z) + RVN(z, jj)
                                        Next z
                                    Else

                                        'sum values for current and subsequent variable
                                        'pick up values for all years prior to current year iteration
                                        For z = 1 To XYear
                                            'UPGRADE_WARNING: Couldn't resolve default property of object SubDataCol(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                            SubDataCol(z) = SubDataCol(z) + RVN(z, jj)
                                        Next z
                                    End If
                                End If
                            Else 'this is executed if not in iteration loop
                                For z = 1 To LG
                                    'UPGRADE_WARNING: Couldn't resolve default property of object Datacol(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                    Datacol(z) = Datacol(z) + RVN(z, jj)
                                Next z
                            End If
                            Exit For
                        End If
                    Next jj


				End If
1116: 
				
				'if fnc = "-" change sign of values so we can just add the values
				If RatioRecs(i).fnc(j) = "-" Then
					For k = 1 To LG
						'UPGRADE_WARNING: Couldn't resolve default property of object Datacol(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						Datacol(k) = Datacol(k) * -1
						'UPGRADE_WARNING: Couldn't resolve default property of object SubDataCol(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						SubDataCol(k) = SubDataCol(k) * -1
					Next k
				End If
1117: 
				'now add the values to the proper array
				If RatioRecs(i).numden = "NUM" Then
					For k = 1 To LG
						'UPGRADE_WARNING: Couldn't resolve default property of object Datacol(k). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						inc1(k) = inc1(k) + Datacol(k)
						'UPGRADE_WARNING: Couldn't resolve default property of object SubDataCol(k). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						SubVarsNum(k) = SubVarsNum(k) + SubDataCol(k)
					Next k
				ElseIf RatioRecs(i).numden = "DEN" Then 
					For k = 1 To LG
						'UPGRADE_WARNING: Couldn't resolve default property of object Datacol(k). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						ded1(k) = ded1(k) + Datacol(k)
						'UPGRADE_WARNING: Couldn't resolve default property of object SubDataCol(k). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						SubVarsDen(k) = SubVarsDen(k) + SubDataCol(k)
					Next k
				End If
			Next j
		Next i
1118: 
		'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		System.Array.Clear(RatioRecs, 0, RatioRecs.Length)
		Exit Sub
		'--------------------------------------------------------
        '---------------------------------------------------------
CheckIfPredefined: 
12124: 
		'This gosub checks if user entry is a pre-defined category.
		'  If not, it must be a user defined variable.
		PreDefined = 0 'False
		For m = 1 To UBound(definedcats)
			If RatioRecs(i).Cat(j) = definedcats(m) Then
				PreDefined = True
				Exit For
			End If
		Next m
1122: 
		'UPGRADE_WARNING: Return has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
		Return 
		'-------------------------------------------
RetrieveUserValues: 
		'This gosub retrieves the annual values of a user defined variable
		'3-1-95  search td$() for corrent lines to put in datacol!()
1123: 
		For jj = 1 To TDT
			If TD(jj, 1) = RatioRecs(i).Cat(j) Then
				'This variable matches the Ratio Definition code, get its values
				If XIter = 1 Then ' this is executed if within iteration loop
					'If we are "iterating", we only want prior years for
					'  the current and subsequent variable
					If jj <> Numvar Then 'skip the current variable
						If jj < Numvar Then 'sum current year values for all variables prior to Numvar
							For z = 1 To LG
								'UPGRADE_WARNING: Couldn't resolve default property of object Datacol(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
								Datacol(z) = Datacol(z) + RVN(z, jj)
							Next z
						Else
							
							'sum values for current and subsequent variable
							'pick up values for all years prior to current year iteration
							For z = 1 To XYear
								'UPGRADE_WARNING: Couldn't resolve default property of object SubDataCol(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
								SubDataCol(z) = SubDataCol(z) + RVN(z, jj)
							Next z
						End If
					End If
				Else 'this is executed if not in iteration loop
1125: 
					For z = 1 To LG
						'UPGRADE_WARNING: Couldn't resolve default property of object Datacol(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						Datacol(z) = Datacol(z) + RVN(z, jj)
					Next z
				End If
				Exit For
			End If
		Next jj
		'UPGRADE_WARNING: Return has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
		Return 
1126: 
    End Sub

    Function BuildListOfDefinedVariables(ByRef nocats As Single) As Object()
        'The valid categories include these items which are "reserved"
        '  words in Giant.  If the entered category is not one of
        '  these, it must be a user defined variable.  RetrieveValues
        '  will be called to get these items.  User variables will have
        '  to be handled seperately.

        Dim Dum As String
        Dim L As Short
1119:



        'a() ENTRIES    "BDA and INF screen entries)
        ' GDP 20 Jan 2003
        ' Added OV3 to OV0, OP3 to OP0
        'DUM$ = "OILGASOV1OV2RESWINOPCGPCOP1OP2OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5"
        ' 27 May 2003 JWD (C0700) Add AJ6-AJ0
        'DUM$ = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0RESWINOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0"
        ' 12 May 2005 JWD (C0876) Add A11-A20
        'DUM$ = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0RESWINOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0OX1OX2OX3OX4OX5OX6OX7OX8OX9OX0O11O12O13O14O15O16O17O18O19O20AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0A11A12A13A14A15A16A17A18A19A20"
        ' 16 May 2005 JWD (C0877) Add OX6-O20
        Dum = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0RESWINOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0OX1OX2OX3OX4OX5OX6OX7OX8OX9OX0O11O12O13O14O15O16O17O18O19O20AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0A11A12A13A14A15A16A17A18A19A20"
        'B() ENTRIES (CTY ANN Forecast)
        Dum = Dum & "PR1PR2PR3PR4PR5PRTDP1DP2DP3OT1OT2OT3OT4OT5"
        'MY3() (capex) entries

        '<<<<<< 14 Jun 2001 JWD
        Dum = Dum & CPXCategoryCodesString
        '~~~~~~ was:
        'DUM$ = DUM$ + "BNSLSERENGEOEDHEDSADHASCDNPDVPPLFFCLTRNEORCP1CP2CP3BALBL2BL3"
        '>>>>>> End 14 Jun 2001

        'calculated values (RetrieveValues will handle them)
        Dum = Dum & "PRDOPXEXPDEVCPX"
1120:
        nocats = Len(Dum) / 3
        Dim definedcats(nocats) As Object
        For L = 1 To nocats
            'UPGRADE_WARNING: Couldn't resolve default property of object definedcats$(L). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            definedcats(L) = Mid(Dum, (L - 1) * 3 + 1, 3)
        Next L
        Dum = ""

1121:
        Return definedcats
    End Function

    ' $SubTitle:'FilterDepths'
    ' $Page
    Sub FilterDepths(ByRef ck() As Short, ByRef ratein(,) As Single, ByRef matchtot As Short)
        Dim tempct As Short
        Dim i As Short
        Dim Count As Short
        Dim depth As Short
        '--------------------------------------------------------------------
        'This sub examines ratein() and resets ck%() to only point at the
        '  records in ratein() that are of the largest water depth
        'Ratein() contains ALL of the rate screen lines. Upon arrival, ck%()
        '  points at the lines that match the criteria of category, field
        '  type, and ALL water depths <= GN(1) [water depth of this case].
        'This sub modifies ck%() to only point at the records that have the
        '  largest water depth (still assuming the other criteria)

        depth = 0 'stores the water depth we want (deepest found so far)
        Count = 0 'how many records match the depth% water depth
        'search ratein(ck%(1-matchtot%),3) & find the largest water depth
        For i = 1 To matchtot
            If ratein(ck(i), 3) > depth Then
                depth = ratein(ck(i), 3)
                Count = 1
            ElseIf ratein(ck(i), 3) = depth Then
                Count = Count + 1
            End If
        Next i

        'we now have the depth and count of records of that depth
        Dim tempck(Count) As Object
        tempct = 0
        For i = 1 To matchtot
            If ratein(ck(i), 3) = depth Then
                tempct = tempct + 1
                'UPGRADE_WARNING: Couldn't resolve default property of object tempck(tempct). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                tempck(tempct) = ck(i)
            End If
        Next i

        'reset inbound variables [ ck%() and matchtot% ] with the
        '  correct values
        matchtot = Count
        ReDim ck(matchtot)
        For i = 1 To matchtot
            'UPGRADE_WARNING: Couldn't resolve default property of object tempck(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            ck(i) = tempck(i)
        Next i

    End Sub

    ' $STATIC
    Sub IncDedEntered(ByRef varnumber As Short, ByRef IncDedBlank As Short)
        Dim q As Short

        'check if RLD() is all 0s. If so, then user did not
        '  fill in any income or deduction fields on the line.
        'IF ALL ZEROS, IN FINISHIT, WE PLUG REV() AND DDT();
        '  IF NOT, WE LEAVE THEM ALONE!
        'First assume that there are no inc/ded entries
        '  on this fiscal def line.
        IncDedBlank = -1
        'Now, search the inc and ded fields to see if ant entries are there.
        If LTrim(RTrim(TD(varnumber, 5))) <> "" Then 'Income 1
            IncDedBlank = 0 'false
        End If
        If LTrim(RTrim(TD(varnumber, 6))) <> "" Then 'Income 2
            IncDedBlank = 0 'false
        End If
        'now, the deduction columns
        For q = 8 To 12
            If LTrim(RTrim(TD(varnumber, q))) <> "" Then 'Income 2
                IncDedBlank = 0 'false
                Exit For
            End If
        Next q

    End Sub

    ' $SubTitle:'CeilDef'
    ' $Page
    '
    ' Modifications:
    ' 20 Jan 2003
    '  -> Changed to use constant for price offset in A Array
    '
    ' 30 Jan 2003
    '  -> Change the upper bound of the fiscal definition
    '     table search for deduction column codes to be
    '     determined by the calling routine, in a fashion
    '     similar that for the income columns. (C0652)
    '

    Sub CeilDef(ByRef PGMCall As String, ByRef x As Short, ByRef matcher() As String, ByRef CeilAns() As Single)
        Dim MatchPrc As Short
        Dim loopy As Single
        Dim dumt() As Single
        '--------------------------------------------------------------------
        Dim ck As Short
        Dim i As Short
        Dim j As Short
        '---------------------------------------------------------

1775:
        Dim ceilvol(LG) As Single
        Dim Ceilpce(LG) As Single
        Dim ceilrev(LG) As Single
        For i = 1 To 2
            If Left(matcher(i + 2), 3) = "" Then
                GoTo a12999
            End If
            'SEE IF VARIABLE DEFINED
            If PGMCall = "DEPREC" And x = 1 Then GoTo a11000
            ck = 0
            If PGMCall = "DEPREC" Then loopy = x - 1
            If PGMCall = "REPAY" Then loopy = TDT
            For j = 1 To loopy
                If matcher(i + 2) <> TD(j, 1) Then GoTo a10770
                ck = j
a10770:     Next j
            If ck = 0 Then GoTo a11000

            'USE VOLUMES, PRICES AND REVENUES OF PREVIOUSLY DEFINED VARIABLE
            For j = 1 To LG
                CeilAns(j) = CeilAns(j) + RVN(j, ck)
            Next j
            GoTo a12999

a11000:     ' get annual array for code entered
            ReDim dumt(LG)
            'UPGRADE_WARNING: Couldn't resolve default property of object dumt(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            RetrieveValues(matcher(i + 2), matcher(2), dumt)

            For j = 1 To LG
                'UPGRADE_WARNING: Couldn't resolve default property of object dumt(j). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                CeilAns(j) = CeilAns(j) + dumt(j)
                ' GDP 20 Jan 2003
                ' Use constant for offset
                'UPGRADE_WARNING: Couldn't resolve default property of object ceilvol(j). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                If A(j, PPR + gc_nAPRICEOFFSET) <> 0 Then ceilvol(j) = CeilAns(j) / A(j, PPR + gc_nAPRICEOFFSET)
                'UPGRADE_WARNING: Couldn't resolve default property of object ceilvol(j). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                If A(j, PPR + gc_nAPRICEOFFSET) = 0 Then ceilvol(j) = 0.0!
            Next j
a12999: Next i


        'APPLY PROPER PRICE AND RECALCULATE REVENUES

        If matcher(5) = "PRC" Then
            ReDim Ceilpce(LG)
            PriceDef(matcher(5), MatchPrc, Ceilpce)
            If MatchPrc = 0 Then GoTo a13540
            For j = 1 To LG
                'UPGRADE_WARNING: Couldn't resolve default property of object Ceilpce(j). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object ceilvol(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object ceilrev(j). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                ceilrev(j) = ceilvol(j) * Ceilpce(j)
            Next j
a13540: ElseIf matcher(5) <> "" Then  'USE SPECIFIED PRICE
            RetrieveValues(matcher(5), "xxx", Ceilpce)
            For j = 1 To LG
                'UPGRADE_WARNING: Couldn't resolve default property of object Ceilpce(j). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object ceilvol(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                CeilAns(j) = ceilvol(j) * Ceilpce(j)
            Next j
        End If

        'LOOP THRU DEDUCTIONS
        ' 30 Jan 2003 JWD (C0652)
        ' Set upper limit of FiscalDef table search.
        loopy = x - 1
        If PGMCall = "REPAY" Then loopy = TDT
        ' End (C0652)
        For i = 1 To 5
            If matcher(i + 5) = "" Then GoTo a22999
            If matcher(i + 5) <> "INT" Then GoTo a20460
            For j = 1 To LG
                CeilAns(j) = CeilAns(j) - INTRST(j)
            Next j
            GoTo a22999

a20460:     'SEE IF VARIABLE DEFINED
            If x = 1 Then GoTo a20700
            ck = 0
            For j = 1 To loopy ' Was:  To x - 1 (C0652)
                If matcher(i + 5) <> TD(j, 1) Then GoTo a20560
                ck = j
a20560:     Next j
            If ck = 0 Then GoTo a20700
            'USE VARIABLE FOUND
            For j = 1 To LG
                CeilAns(j) = CeilAns(j) - RVN(j, ck)
            Next j
            GoTo a22999

a20700:     ' MUST BE STANDARD CODE FROM DATA FILE OR COUNTRY ANNUAL FORECASTS
            ReDim dumt(LG)
            'UPGRADE_WARNING: Couldn't resolve default property of object dumt(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            RetrieveValues(matcher(i + 5), "", dumt) '"" means use whatever level we are at (WIN, PAR)
            For j = 1 To LG
                'UPGRADE_WARNING: Couldn't resolve default property of object dumt(j). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                CeilAns(j) = CeilAns(j) - dumt(j)
            Next j

a22999: Next i


    End Sub

    ' $DYNAMIC
    ' $SubTitle:'PriceDef'
    ' $Page



    Sub PriceDef(ByRef VarMatch As String, ByRef MatchPrc As Short, ByRef PrcAmt() As Single)
        '--------------------------------------------------------------------
        'This sub is called from FISCAL.BAS when the user has entered PRC in the
        '  column. This triggers a search in the Price Definition Form for a match
        '  of the variable in the variable column of Fiscal Definition (the line
        '  we are processing.)
        'This sub searches PC() looking for the variable. If found, we look at the
        '  two categories on that line in PC(). We retrieve the annual values from
        '  the A() and/or B() arrays and return the desired annual values (based on
        '  the operator (<>+-*/) the user entered.

        ' VarMatch$ = Variable Code
        ' MatchPrc% = 1 if there was a matching variable on the Price Definition form
        ' PCE() = annual prices returned from subroutine (PrcAmt() locally)

        'possible codes in PC(xyz,1&3) (categories 1 & 2):
        '      1 = OPC
        '      2 = GPC
        '      3 = OP1
        '      4 = OP2
        '      5 = PR1
        '      6 = PR2
        '      7 = PR3
        '      8 = PR4
        '      9 = PR5

        'possible codes for PC(xyz,2) (operator)
        '      1 = <
        '      2 = >
        '      3 = +
        '      4 = -
        '      5 = *
        '      6 = /

        '---------------------------------------------------------
        ' Modifications:
        ' 20 Feb 1996 JWD
        '        Change PC$ to sPCV, duplicate definition (PC()).
        ' 20 Jan 2003 GDP
        '  -> Use constants for ranges used in comparisons
        '
        ' 5 Jun 2003 JWD
        '  -> Correct the offset values added to the symbols in
        '     the decode of the price code column 1 for the PR1-
        '     PR5 codes. (C0711)
        '---------------------------------------------------------
        Dim ZYX As Short
        Dim xz As Short
        '---------------------------------------------------------


        MatchPrc = 0 'signal back to calling routine wether found or not
        Dim tp(2) As Object 'holds a year's data for variable 1 and 2
        ReDim PrcAmt(LG) 'returned values array

        'USE SPECIAL PRICES
        'LOOP searching for a match on the variable
        ZYX = 0
a13110: ZYX = ZYX + 1
        If ZYX > PCT Then GoTo endprc
        If VarMatch = sPCV(ZYX) Then GoTo a13170
        GoTo a13110

a13170:  'ZYX= ROW IN PC TO BE USED
        MatchPrc = 1

        For xz = 1 To LG

            'UPGRADE_WARNING: Couldn't resolve default property of object tp(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            tp(1) = 0 'this years data for code #1
            'UPGRADE_WARNING: Couldn't resolve default property of object tp(2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            tp(2) = 0 'this years data for code #2

            ' GDP 20 Jan 2003
            ' Use constants for ranges used in these comparisons
            If PC(ZYX, 1) >= gc_nAMINVOL And PC(ZYX, 1) <= gc_nAMAXVOL Then 'OPC,GPC,OP1,OP2
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                tp(1) = A(xz, PC(ZYX, 1) + gc_nAPRICEOFFSET)
                ' 5 Jun 2003 JWD (C0711) Change next. Correct offset values
                'ElseIf PC(ZYX, 1) >= gc_nAMAXVOL + 2 And PC(ZYX, 1) <= gc_nAMAXVOL + 6 Then 'PR1-5
            ElseIf PC(ZYX, 1) >= gc_nAMAXVOL + 1 And PC(ZYX, 1) <= gc_nAMAXVOL + 5 Then  'PR1-5
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                tp(1) = B(xz, PC(ZYX, 1) - 12)
            End If
            If PC(ZYX, 3) >= gc_nAMINVOL And PC(ZYX, 3) <= gc_nAMAXVOL Then 'OPC,GPC,OP1,OP2
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                tp(2) = A(xz, PC(ZYX, 3) + gc_nAPRICEOFFSET)
            ElseIf PC(ZYX, 3) >= gc_nAMAXVOL + 1 And PC(ZYX, 3) <= gc_nAMAXVOL + 5 Then  'PR1-5
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                tp(2) = B(xz, PC(ZYX, 3) - 12)
            End If



a13240:     If PC(ZYX, 2) = 0 Then 'no operator
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                PrcAmt(xz) = tp(1)
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            ElseIf PC(ZYX, 2) = 1 And tp(1) <= tp(2) Then  '<
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                PrcAmt(xz) = tp(1)
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            ElseIf PC(ZYX, 2) = 1 And tp(2) < tp(1) Then  '<
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                PrcAmt(xz) = tp(2)
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            ElseIf PC(ZYX, 2) = 2 And tp(1) >= tp(2) Then  '>
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                PrcAmt(xz) = tp(1)
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            ElseIf PC(ZYX, 2) = 2 And tp(2) > tp(1) Then  '>
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                PrcAmt(xz) = tp(2)
            ElseIf PC(ZYX, 2) = 3 Then  '+
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                PrcAmt(xz) = tp(1) + tp(2)
            ElseIf PC(ZYX, 2) = 4 Then  '-
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                PrcAmt(xz) = tp(1) - tp(2)
            ElseIf PC(ZYX, 2) = 5 Then  '*
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                PrcAmt(xz) = tp(1) * tp(2)
            ElseIf PC(ZYX, 2) = 6 Then  '/
                'UPGRADE_WARNING: Couldn't resolve default property of object tp(2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                If tp(2) <> 0 Then
                    'UPGRADE_WARNING: Couldn't resolve default property of object tp(2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: Couldn't resolve default property of object tp(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    PrcAmt(xz) = tp(1) / tp(2)
                Else
                    PrcAmt(xz) = 0
                End If
            End If
            'now make the assignment
            PrcAmt(xz) = (PrcAmt(xz) * (PC(ZYX, 4) / 100)) + PC(ZYX, 5)
        Next xz

endprc:
        'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        System.Array.Clear(tp, 0, tp.Length)

    End Sub

    ' $SubTitle:'PrintIrrWksheet'
    ' $Page
    Sub PrintIRRWksheet(ByRef Ttls() As String, ByRef WkSht(,) As Single, ByRef PgTtl As String, ByRef param As Short, ByRef profitsplit As Short)
        Dim y As Single
        Dim PgCounter As Short
        '-----------------------------------------------------------------------
        'This sub is called by CalcIRR.
        'This SUB prints a worksheet page in the PRN file showing the
        '  detail behind the rate based variable's page in the reports.
        'Parameters:
        'WkSht!()     - The data for all years and all columns
        'Ttls$()      - Data for the second header line in the prn file
        'PgTtl$       - Variable name for inclusion in page title
        'param%       - 35 is RTO, 36 is RT1, 37 is IRR
        'profitsplit% - true is profit sharing variable, false is tax variable
        '---------------------------------------------------------
        ' Modifications:
        ' 20 Feb 1996 JWD
        '        Change CUR$ to sCur, duplicate definition (CUR()).
        ' 20 Jan 2003 GDP
        '   -> param% comparison line changed - additional volumes
        '
        ' 27 May 2003 JWD
        '  -> Replace literal numbers representing rate parameters
        '     with symbols. (C0700)
        '
        ' 13 Jan 2004 JWD
        '  -> Add references to CGiantReport1 object to collect
        '     report data in object rather than output directly to
        '     file. For consolidation engine development testing
        '     purposes. (C0772)
        '
        ' 19 Jan 2004 JWD
        '  -> Changed report page object type for Deflated cash
        '     flow page from CGiantRptPageA1 to interface
        '     IGiantRptPageAssignStd. (C0774)
        '
        ' 21 Jan 2004 JWD
        '  -> Change type of page object to IGiantRptPageAssignSub
        '     (C0774)
        '  -> Change page instantiation method call to call
        '     NewSubScheduleRptPage instead of NewStandardRptPage.
        '     (C0774)
        '  -> Change to instantiate specific page in each place
        '     rather than a single instantiation, because the
        '     instantiation is parameterized on page type code.
        '     (C0774)
        '
        ' 3 Feb 2004 JWD
        '  -> Remove explicit writes to report file. (C0776)
        '---------------------------------------------------------
        Dim oPg1 As IGiantRptPageAssignSub

        'Page type, Start year, Page counter (actually type), life of field, number of columns, page title, column length. final win %, final PAR %, Currency code

        ' GDP 20 Jan 2003
        ' Changed the rate parameter for new volumes
        'If param% = 61 Then
        ' 27 May 2003 JWD (C0700) Replace number with symbol
        If param = gc_nRtPrmIRR Then

            If profitsplit = False Then

                'Tax Based on IRR
                ''Write #5, 18, YR, PgCounter%, LG, 11, "RATE OF RETURN BASED WORKSHEET FOR " + PgTtl$, 8, FinalWin, FINALPARTIC, sCur

                ''Write #5, Ttls$(1), Ttls$(2), Ttls$(3), Ttls$(4), Ttls$(5), Ttls$(6), Ttls$(7), Ttls$(8), Ttls$(9), Ttls$(10), Ttls$(11)

                oPg1 = g_oReport.NewSubScheduleRptPage(18)
                oPg1.SetPageHeader(18, YR, PgCounter, LG, 11, "RATE OF RETURN BASED WORKSHEET FOR " & PgTtl, 8, FinalWin, FINALPARTIC, sCur)
                oPg1.SetProfileHeaders(Ttls(1), Ttls(2), Ttls(3), Ttls(4), Ttls(5), Ttls(6), Ttls(7), Ttls(8), Ttls(9), Ttls(10), Ttls(11))
                oPg1.SetRptPageVariableCode(PgTtl)

                For y = 1 To LG
                    ''Write #5, WkSht!(y, 1), WkSht!(y, 2), WkSht!(y, 3), WkSht!(y, 4), WkSht!(y, 5), WkSht!(y, 6), WkSht!(y, 7), WkSht!(y, 8), WkSht!(y, 9), WkSht!(y, 10), WkSht!(y, 11)
                    oPg1.SetProfileValues(y, WkSht(y, 1), WkSht(y, 2), WkSht(y, 3), WkSht(y, 4), WkSht(y, 5), WkSht(y, 6), WkSht(y, 7), WkSht(y, 8), WkSht(y, 9), WkSht(y, 10), WkSht(y, 11))
                Next y

            Else

                'Profit Share Based on IRR
                ''Write #5, 19, YR, PgCounter%, LG, 14, "RATE OF RETURN BASED WORKSHEET FOR " + PgTtl$, 8, FinalWin, FINALPARTIC, sCur

                ''Write #5, Ttls$(1), Ttls$(2), Ttls$(3), Ttls$(4), Ttls$(5), Ttls$(6), Ttls$(7), Ttls$(8), Ttls$(9), Ttls$(10), Ttls$(11), Ttls$(12), Ttls$(13), Ttls$(14)

                oPg1 = g_oReport.NewSubScheduleRptPage(19)
                oPg1.SetPageHeader(19, YR, PgCounter, LG, 14, "RATE OF RETURN BASED WORKSHEET FOR " & PgTtl, 8, FinalWin, FINALPARTIC, sCur)
                oPg1.SetProfileHeaders(Ttls(1), Ttls(2), Ttls(3), Ttls(4), Ttls(5), Ttls(6), Ttls(7), Ttls(8), Ttls(9), Ttls(10), Ttls(11), Ttls(12), Ttls(13), Ttls(14))
                oPg1.SetRptPageVariableCode(PgTtl)

                For y = 1 To LG
                    ''Write #5, WkSht!(y, 1), WkSht!(y, 2), WkSht!(y, 3), WkSht!(y, 4), WkSht!(y, 5), WkSht!(y, 6), WkSht!(y, 7), WkSht!(y, 8), WkSht!(y, 9), WkSht!(y, 10), WkSht!(y, 11), WkSht!(y, 12), WkSht!(y, 13), WkSht!(y, 14)
                    oPg1.SetProfileValues(y, WkSht(y, 1), WkSht(y, 2), WkSht(y, 3), WkSht(y, 4), WkSht(y, 5), WkSht(y, 6), WkSht(y, 7), WkSht(y, 8), WkSht(y, 9), WkSht(y, 10), WkSht(y, 11), WkSht(y, 12), WkSht(y, 13), WkSht(y, 14))
                Next y

            End If


        Else

            If profitsplit = False Then

                'Tax Based on Ratio
                ''Write #5, 20, YR, PgCounter%, LG, 10, "RATIO BASED WORKSHEET FOR " + PgTtl$, 8, FinalWin, FINALPARTIC, sCur

                ''Write #5, Ttls$(1), Ttls$(2), Ttls$(3), Ttls$(4), Ttls$(5), Ttls$(6), Ttls$(7), Ttls$(8), Ttls$(9), Ttls$(10)

                oPg1 = g_oReport.NewSubScheduleRptPage(20)
                oPg1.SetPageHeader(20, YR, PgCounter, LG, 10, "RATIO BASED WORKSHEET FOR " & PgTtl, 8, FinalWin, FINALPARTIC, sCur)
                oPg1.SetProfileHeaders(Ttls(1), Ttls(2), Ttls(3), Ttls(4), Ttls(5), Ttls(6), Ttls(7), Ttls(8), Ttls(9), Ttls(10))
                oPg1.SetRptPageVariableCode(PgTtl)

                For y = 1 To LG
                    ''Write #5, WkSht!(y, 1), WkSht!(y, 2), WkSht!(y, 3), WkSht!(y, 4), WkSht!(y, 5), WkSht!(y, 6), WkSht!(y, 7), WkSht!(y, 8), WkSht!(y, 9), WkSht!(y, 10)
                    oPg1.SetProfileValues(y, WkSht(y, 1), WkSht(y, 2), WkSht(y, 3), WkSht(y, 4), WkSht(y, 5), WkSht(y, 6), WkSht(y, 7), WkSht(y, 8), WkSht(y, 9), WkSht(y, 10))
                Next y

            Else

                'Profit Share Based on Ratio
                ''Write #5, 21, YR, PgCounter%, LG, 13, "RATIO BASED WORKSHEET FOR " + PgTtl$, 8, FinalWin, FINALPARTIC, sCur

                ''Write #5, Ttls$(1), Ttls$(2), Ttls$(3), Ttls$(4), Ttls$(5), Ttls$(6), Ttls$(7), Ttls$(8), Ttls$(9), Ttls$(10), Ttls$(11), Ttls$(12), Ttls$(13)

                oPg1 = g_oReport.NewSubScheduleRptPage(21)
                oPg1.SetPageHeader(21, YR, PgCounter, LG, 13, "RATIO BASED WORKSHEET FOR " & PgTtl, 8, FinalWin, FINALPARTIC, sCur)
                oPg1.SetProfileHeaders(Ttls(1), Ttls(2), Ttls(3), Ttls(4), Ttls(5), Ttls(6), Ttls(7), Ttls(8), Ttls(9), Ttls(10), Ttls(11), Ttls(12), Ttls(13))
                oPg1.SetRptPageVariableCode(PgTtl)

                For y = 1 To LG
                    ''Write #5, WkSht!(y, 1), WkSht!(y, 2), WkSht!(y, 3), WkSht!(y, 4), WkSht!(y, 5), WkSht!(y, 6), WkSht!(y, 7), WkSht!(y, 8), WkSht!(y, 9), WkSht!(y, 10), WkSht!(y, 11), WkSht!(y, 12), WkSht!(y, 13)
                    oPg1.SetProfileValues(y, WkSht(y, 1), WkSht(y, 2), WkSht(y, 3), WkSht(y, 4), WkSht(y, 5), WkSht(y, 6), WkSht(y, 7), WkSht(y, 8), WkSht(y, 9), WkSht(y, 10), WkSht(y, 11), WkSht(y, 12), WkSht(y, 13))
                Next y

            End If

        End If


    End Sub

    ' $SubTitle:'RateCalc'
    ' $Page
    '
    ' Modifications:
    ' 18 Sep 2001 JWD
    '  -> Change cases for production, equivalent production
    '     cumulative production and cumulative equivalent
    '     production to select the gross production as the
    '     basis value when specified by the flag indicating
    '     that gross production should be used rather than
    '     net. (C0443)
    '
    ' 20 Jan 2003 GDP
    '  -> Changed to cater for additional volumes
    ' 27 Feb 2003 GDP
    '  -> Changed GrossRevenue sum to call TotalGrossRevenue rather than sum
    '     array elements
    '
    ' 27 May 2003 JWD
    '  -> Add new adjustment forecast categories. (C0700)
    '  -> Replace numbers for Select Case with symbols for the
    '     code positions. (C0700)
    '
    ' 12 May 2005 JWD
    '  -> Replace reference to symbol gc_nRtPrmAJ0 with symbol
    '     gc_nRtPrmAJn. (C0876)
    '
    ' 10 Jun 2008 JWD
    '  -> Change cases for PRE and CUV to remove the case on field
    '     type. Should compute Value() as total equivalent production
    '     regardless of field type. (080609-1942-01)
    '  -> Change case for PRE to compute Values() as daily rate rather
    '     than annual rate. (080609-1936-01)
    '
    Sub RateCalc(ByRef Numvar As Short, ByRef PGMCall As String, ByRef searcher As String, ByRef DefAmount As Single, ByRef sRateInV() As String, ByRef ratein(,) As Single, ByRef Ratetot As Short, ByRef param As Short, ByRef VarRates() As Single)
        Dim doneit As Short
        Dim zz As Short
        Dim tmp As Short
        Dim ppy As Short
        Dim Endp As Short
        Dim Startp As Short
        Dim q As Short
        Dim p As Short
        Dim z As Short
        Dim qq As Short
        Dim cumvar As Single
        Dim prdst As Single
        Dim MatchPrc As Short
        Dim px As Single
        Dim METHOD As Short
        Dim xp As Single
        Dim matchtot As Short
        '--------------------------------------------------------------------
        ' NumVar     number of variable in Fiscal Definition
        ' PgmCall$   which program called this subroutine
        ' Searcher$  Variable name as defined in Var column of Fiscal Definition
        ' DefAmount  default amount of rate if there are no entries on the Rates form
        ' sRateInV    array of Variable names on Rates screen that match variable name
        ' Ratein()   array of Rates screen that match variable name
        ' Ratetot    number of lines in Rates screen that match the variable name
        ' param%     returns the entry in the Param column to the calling program
        ' VarRates() returns the annual calculated rates to the calling program

        ' when this subroutine is called, Ratetot contains the number of lines in the
        ' Rates screen that match the code for the Variable being currently calculated.
        ' sRateInV() and Ratein() are the actual lines in the rate screen that match.
        '---------------------------------------------------------
        Dim bCK As Short
        Dim pz As Short
        Dim zp As Short
        Dim xz As Short
        Dim nFieldType As Short

        '<<<<<< 14 Sep 2001 JWD (C0443)
        Dim production_amount As Single
        '>>>>>> End (C0443)

        Dim subtot As Single

        '---------------------------------------------------------

        On Error GoTo ratecalcerror


1900:
startit:

        Dim ck(1) As Short

        ' Find matching lines in Rates form
        ' Branch based on the parameter entered on that first line
        bCK = False
        xz = 0
        matchtot = 0
loop1:
        xz = xz + 1
        If xz > Ratetot Then GoTo loop2
        If sRateInV(xz) = searcher Then
            If ratein(xz, 2) = 1 Or (ratein(xz, 2) = 2 And PPR = 1) Or (ratein(xz, 2) = 3 And PPR = 2) Then
                If gn(1) <= 0 Then 'water depth from data file = 0
                    If ratein(xz, 3) = 0 Or ratein(xz, 3) = -994 Then
                        bCK = True
                        matchtot = matchtot + 1
                        ReDim Preserve ck(matchtot)
                        'UPGRADE_WARNING: Couldn't resolve default property of object ck(matchtot). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        ck(matchtot) = xz
                    End If
                Else 'water depth from data file > 0
                    If ratein(xz, 3) <> -994 Then 'first check that onshore (-994) is not specified
                        If gn(1) >= ratein(xz, 3) Then
                            bCK = True
                            matchtot = matchtot + 1
                            ReDim Preserve ck(matchtot)
                            'UPGRADE_WARNING: Couldn't resolve default property of object ck(matchtot). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            ck(matchtot) = xz
                        End If
                    End If
                End If
            End If
        End If

        GoTo loop1

loop2:

        If bCK = False Then
            For xp = 1 To LG
                VarRates(xp) = DefAmount
            Next xp
        End If
        If bCK = False Then GoTo endit
        'ck%() includes records of the wrong water depth - this
        '  sub filters ck%() to include only the records in ratein()
        '  that are of the correct water depth
        FilterDepths(ck, ratein, matchtot)

        ' we now know there is a match
        'UPGRADE_WARNING: Couldn't resolve default property of object ck(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        param = ratein(ck(1), 4) 'parameter from first matching line
        'UPGRADE_WARNING: Couldn't resolve default property of object ck(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        METHOD = ratein(ck(1), 6) 'sliding scale from first matching line
        'UPGRADE_WARNING: Couldn't resolve default property of object ck(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        nFieldType = ratein(ck(1), 2)

        Dim BrkRates(matchtot) As Object
        For zp = 1 To matchtot
            'UPGRADE_WARNING: Couldn't resolve default property of object ck(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            'UPGRADE_WARNING: Couldn't resolve default property of object BrkRates(zp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            BrkRates(zp) = ratein(ck(zp), 1)
        Next zp

        ' check to see if no parameter is specified
        If param = 0 Then
            For px = 1 To LG
                'UPGRADE_WARNING: Couldn't resolve default property of object BrkRates(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                VarRates(px) = BrkRates(1)
            Next px
        End If
        If param = 0 Then GoTo endit

        ' we now know that the parameter specified is not an IRR type
        ' assign Values(lg) based on entry in Prm 1
        param = param
        Dim Values(LG) As Object
        Dim dEquivalencyFactor As Double

        ' 20 Jan 2003 GDP
        ' Updated each Case statement to reference new numbers
        ' 27 May 2003 JWD (C0700) Changed to reference symbols instead of numbers
        Select Case param

            Case gc_nRtPrmOIL To gc_nRtPrmOV0 ' OIL, GAS, OV1, OV2, OV3, OV4, OV5, OV6, OV7, OV8, OV9, OV0
                For zp = 1 To LG

                    '<<<<<< 14 Sep 2001 JWD (C0443)
                    If UseGrossProductionAmounts Then
                        production_amount = GrossProduction(zp, param)
                    Else
                        production_amount = A(zp, param)
                    End If

                    If zp = (Y1 - YR + 1) Then ' adjust in first year of production
                        'UPGRADE_WARNING: Couldn't resolve default property of object Values(zp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Values(zp) = production_amount / (0.365 * ((13 - M1) / 12))
                    Else
                        'UPGRADE_WARNING: Couldn't resolve default property of object Values(zp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Values(zp) = production_amount / 0.365
                    End If
                    '~~~~~~ was:
                    'If zp = (Y1 - YR + 1) Then      ' adjust in first year of production
                    '  Values(zp) = A(zp, param%) / (0.365 * ((13 - M1) / 12))
                    'Else
                    '  Values(zp) = A(zp, param%) / 0.365
                    'End If
                    '>>>>>> End (C0443)

                Next zp

            Case gc_nRtPrmPRD 'PRD

                '<<<<<< 18 Sep 2001 JWD (C0443)
                If UseGrossProductionAmounts Then
                    For zp = 1 To LG

                        ' Compute total gross revenues for the consolidation
                        ' 27 Feb 2003 GDP
                        ' Changed to use TotalGrossRevenue
                        ' subtot = GrossRevenue(zp, 1) + GrossRevenue(zp, 2) + GrossRevenue(zp, 3) + GrossRevenue(zp, 4)
                        subtot = TotalGrossRevenue(zp)
                        ' Compute total gross (price) equivalent production
                        ' subtot is total consolidated gross revenues
                        If GrossRevenue(zp, PPR) <> 0 Then
                            subtot = subtot * GrossProduction(zp, PPR) / GrossRevenue(zp, PPR)
                        Else
                            subtot = 0
                        End If

                        If zp = (Y1 - YR + 1) Then ' adjust in first year of production
                            subtot = subtot / (0.365 * ((13 - M1) / 12))
                        Else
                            subtot = subtot / 0.365
                        End If

                        'UPGRADE_WARNING: Couldn't resolve default property of object Values(zp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Values(zp) = subtot
                    Next zp
                Else
                    For zp = 1 To LG
                        ' 20 Jan 2003
                        ' Changed to use ATotalRevenues()
                        'subtot = (A(zp, 1) * A(zp, 7)) + (A(zp, 2) * A(zp, 8))
                        subtot = ATotalRevenues(zp)
                        ' Changed A(zp, PPR + 6) to A(zp, PPR + gc_nAPRICEOFFSET)
                        If A(zp, PPR + gc_nAPRICEOFFSET) <> 0 Then
                            subtot = subtot / A(zp, PPR + gc_nAPRICEOFFSET)
                        Else
                            subtot = 0
                        End If

                        If zp = (Y1 - YR + 1) Then ' adjust in first year of production
                            subtot = subtot / (0.365 * ((13 - M1) / 12))
                        Else
                            subtot = subtot / 0.365
                        End If

                        'UPGRADE_WARNING: Couldn't resolve default property of object Values(zp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Values(zp) = subtot
                    Next zp
                End If
                '~~~~~~ was:
                'For zp = 1 To LG
                '   subtot = (A(zp, 1) * A(zp, 7)) + (A(zp, 2) * A(zp, 8))
                '   subtot = subtot + (A(zp, 3) * A(zp, 9)) + (A(zp, 4) * A(zp, 10))
                '
                '   If A(zp, PPR + 6) <> 0 Then
                '          subtot = subtot / A(zp, PPR + 6)
                '   Else
                '          subtot = 0
                '   End If
                '
                '   If zp = (Y1 - YR + 1) Then      ' adjust in first year of production
                '          subtot = subtot / (0.365 * ((13 - M1) / 12))
                '   Else
                '          subtot = subtot / 0.365
                '   End If
                '
                '   Values(zp) = subtot
                'Next zp
                '>>>>>> End (C0443)

            Case gc_nRtPrmOPC To gc_nRtPrmOP0 'OPC, GPC, OP1, OP2, OP3, OP4, OP5, OP6, OP7, OP8, OP9, OP0
                For zp = 1 To LG
                    ' 27 May 2003 JWD (C0700) Make offsets relative to range rather than literal number
                    'Values(zp) = A(zp, (param% + 1))
                    'UPGRADE_WARNING: Couldn't resolve default property of object Values(zp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Values(zp) = A(zp, (param - gc_nRtPrmOPC) + gc_nAOPC)
                Next zp

                'Case gc_nRtPrmAJ1 To gc_nRtPrmAJ0     'AJ1, AJ2, AJ3, AJ4, AJ5, AJ6, AJ7, AJ8, AJ9, AJ0
                ' 12 May 2005 JWD (C0876) Change referenced symbol
            Case gc_nRtPrmAJ1 To gc_nRtPrmAJn 'AJ1-AJ0 + A11-A20
                For zp = 1 To LG
                    ' 27 May 2003 JWD (C0700) Make offsets relative to range rather than literal number
                    'Values(zp) = A(zp, (param% + 6))
                    'UPGRADE_WARNING: Couldn't resolve default property of object Values(zp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Values(zp) = A(zp, (param - gc_nRtPrmAJ1) + gc_nAAJ1)
                Next zp

            Case gc_nRtPrmPRC 'PRC
                Dim PrcAmt(LG) As Single
                PriceDef(searcher, MatchPrc, PrcAmt)
                If MatchPrc = 1 Then
                    For zp = 1 To LG
                        'UPGRADE_WARNING: Couldn't resolve default property of object PrcAmt(zp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object Values(zp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Values(zp) = PrcAmt(zp)
                    Next zp
                End If
                'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
                System.Array.Clear(PrcAmt, 0, PrcAmt.Length)

            Case gc_nRtPrmPR1 To gc_nRtPrmPR5 'PR1, PR2, PR3, PR4, PR5
                For zp = 1 To LG
                    ' 27 May 2003 JWD (C0700) Make offsets relative to range rather than literal number
                    'Values(zp) = B(zp, (param% - 15))
                    'UPGRADE_WARNING: Couldn't resolve default property of object Values(zp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Values(zp) = B(zp, (param - gc_nRtPrmPR1) + gc_nBPR1)
                Next zp

            Case gc_nRtPrmOT1 To gc_nRtPrmOT5 'OT1, OT2, OT3, OT4, OT5
                For zp = 1 To LG
                    ' 27 May 2003 JWD (C0700) Make offsets relative to range rather than literal number
                    'Values(zp) = B(zp, (param% - 11))
                    'UPGRADE_WARNING: Couldn't resolve default property of object Values(zp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Values(zp) = B(zp, (param - gc_nRtPrmOT1) + gc_nBOT1)
                Next zp

            Case gc_nRtPrmOLC To gc_nRtPrmV0C 'OLC, GSA, V1C, V2C, V3C, V4C, V5C, V6C, V7C, V8C, V9C, V0C

                '<<<<<< 14 Sep 2001 JWD (C0443)
                If UseGrossProductionAmounts Then
                    ' Use the consolidated gross production
                    For pz = 1 To LG
                        For zp = 1 To pz
                            ' GDP 20 Jan 2003
                            ' Changed (param% - 25) to (param% - 41)
                            ' 27 May 2003 JWD (C0700) Make offsets relative to range rather than literal number
                            'Values(pz) = Values(pz) + GrossProduction(zp, (param% - 41))
                            'UPGRADE_WARNING: Couldn't resolve default property of object Values(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            'UPGRADE_WARNING: Couldn't resolve default property of object Values(pz). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            Values(pz) = Values(pz) + GrossProduction(zp, (param - gc_nRtPrmOLC) + gc_nAOIL)
                        Next zp
                    Next pz
                Else
                    ' Use the consolidated net production
                    For pz = 1 To LG
                        For zp = 1 To pz
                            ' GDP 20 Jan 2003
                            ' Changed (param% - 25) to (param% - 41)
                            ' 27 May 2003 JWD (C0700) Make offsets relative to range rather than literal number
                            'Values(pz) = Values(pz) + A(zp, (param% - 41))
                            'UPGRADE_WARNING: Couldn't resolve default property of object Values(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            'UPGRADE_WARNING: Couldn't resolve default property of object Values(pz). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            Values(pz) = Values(pz) + A(zp, (param - gc_nRtPrmOLC) + gc_nAOIL)
                        Next zp
                    Next pz
                End If
                '~~~~~~ was:
                'For pz = 1 To LG
                '   For zp = 1 To pz
                '      Values(pz) = Values(pz) + A(zp, (param% - 25))
                '   Next zp
                'Next pz
                '>>>>>> End (C0443)

            Case gc_nRtPrmPRV 'Production volume#

                dEquivalencyFactor = gn(2)


                For pz = 1 To LG

                    subtot = EquivalencyVolumeProduction(pz, True, True)

                    If pz = (Y1 - YR + 1) Then ' adjust in first year of production
                        subtot = subtot / (0.365 * ((13 - M1) / 12))
                    Else
                        subtot = subtot / 0.365
                    End If

                    'UPGRADE_WARNING: Couldn't resolve default property of object Values(pz). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Values(pz) = subtot

                    'If nFieldType = 1 Then ' ALL
                    '    Values(pz) = Values(pz) + EquivalencyVolumeProduction(pz, True, True)
                    'ElseIf nFieldType = 2 Then ' OIL
                    '    Values(pz) = Values(pz) + EquivalencyVolumeProduction(pz, True, False)
                    'ElseIf nFieldType = 3 Then ' GAS
                    '    Values(pz) = Values(pz) + EquivalencyVolumeProduction(pz, False, True)
                    'End If
                Next pz

            Case gc_nRtPrmCUV 'Cumulative production volume

                dEquivalencyFactor = gn(2)

                For pz = 1 To LG
                    For zp = 1 To pz

                        'UPGRADE_WARNING: Couldn't resolve default property of object Values(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object Values(pz). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Values(pz) = Values(pz) + EquivalencyVolumeProduction(zp, True, True)

                        'If nFieldType = 1 Then ' ALL
                        '    Values(pz) = Values(pz) + EquivalencyVolumeProduction(zp, True, True)
                        'ElseIf nFieldType = 2 Then ' OIL
                        '    Values(pz) = Values(pz) + EquivalencyVolumeProduction(zp, True, False)
                        'ElseIf nFieldType = 3 Then ' GAS
                        '    Values(pz) = Values(pz) + EquivalencyVolumeProduction(zp, False, True)
                        'End If

                    Next zp
                Next pz



            Case gc_nRtPrmCUM 'CUM

                '<<<<<< 14 Sep 2001 JWD (C0443)
                If UseGrossProductionAmounts Then
                    For pz = 1 To LG
                        For zp = 1 To pz

                            ' Compute total gross revenues
                            ' GDP 27 Feb 2003
                            ' Changed to use TotalGrossRevenue
                            ' subtot = GrossRevenue(zp, 1) + GrossRevenue(zp, 2) + GrossRevenue(zp, 3) + GrossRevenue(zp, 4)
                            ' 1 May 2003 JWD (C0691) Change next 1, correct symbol spelling
                            subtot = TotalGrossRevenue(zp) ' was: suttot = TotalGrossRevenue(zp)
                            ' Compute total (price) equivalent production
                            If GrossRevenue(zp, PPR) <> 0 Then
                                subtot = subtot * GrossProduction(zp, PPR) / GrossRevenue(zp, PPR)
                            Else
                                subtot = 0
                            End If
                            'UPGRADE_WARNING: Couldn't resolve default property of object Values(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            'UPGRADE_WARNING: Couldn't resolve default property of object Values(pz). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            Values(pz) = Values(pz) + subtot
                        Next zp
                    Next pz
                Else
                    For pz = 1 To LG
                        For zp = 1 To pz
                            ' GDP 20 Jan 2003
                            ' Changed revenue calc to use ATotalRevenues
                            '                         subtot = (A(zp, 1) * A(zp, 7)) + (A(zp, 2) * A(zp, 8))
                            '                         subtot = subtot + (A(zp, 3) * A(zp, 9)) + (A(zp, 4) * A(zp, 10))

                            subtot = ATotalRevenues(zp)
                            ' Changed A(zp, PPR + 6) to A(zp, PPR + gc_nAPRICEOFFSET)
                            If A(zp, PPR + gc_nAPRICEOFFSET) <> 0 Then
                                subtot = subtot / A(zp, PPR + gc_nAPRICEOFFSET)
                            Else
                                subtot = 0
                            End If
                            'UPGRADE_WARNING: Couldn't resolve default property of object Values(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            'UPGRADE_WARNING: Couldn't resolve default property of object Values(pz). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            Values(pz) = Values(pz) + subtot
                        Next zp
                    Next pz
                End If
                '~~~~~~ was:
                'For pz = 1 To LG
                '   For zp = 1 To pz
                '      subtot = (A(zp, 1) * A(zp, 7)) + (A(zp, 2) * A(zp, 8))
                '      subtot = subtot + (A(zp, 3) * A(zp, 9)) + (A(zp, 4) * A(zp, 10))
                '      If A(zp, PPR + 6) <> 0 Then
                '         subtot = subtot / A(zp, PPR + 6)
                '      Else
                '         subtot = 0
                '      End If
                '      Values(pz) = Values(pz) + subtot
                '   Next zp
                'Next pz
                '>>>>>> End (C0443)

            Case gc_nRtPrmYRS 'YRS
                prdst = Y1 - YR + 1 'production start year
                For zp = 1 To LG
                    'UPGRADE_WARNING: Couldn't resolve default property of object Values(zp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Values(zp) = zp - prdst
                    '  IF Values(zp) < 0 THEN Values(zp) = 0
                Next zp

            Case gc_nRtPrmDTE ' DTE
                For zp = 1 To LG
                    'UPGRADE_WARNING: Couldn't resolve default property of object Values(zp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Values(zp) = zp
                Next zp

            Case gc_nRtPrmCAL 'CAL
                For zp = 1 To LG
                    'UPGRADE_WARNING: Couldn't resolve default property of object Values(zp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Values(zp) = YR + zp - 1
                Next zp

            Case gc_nRtPrmILD, gc_nRtPrmCID 'ILD for Variable Rates, CID for Ceiling Rates
                If PGMCall = "PARTIC" Then
12300:
                    ReDim RLD(1)
                    ReDim clngs(1)
                ElseIf PGMCall = "FISCAL" Then
12310:              For zp = 1 To LG
                        'UPGRADE_WARNING: Couldn't resolve default property of object Values(zp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Values(zp) = RLD(zp)
                    Next zp
                Else
12330:              For zp = 1 To LG
                        'UPGRADE_WARNING: Couldn't resolve default property of object Values(zp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Values(zp) = clngs(zp)
                    Next zp
                End If
12350:      Case gc_nRtPrmRTO To gc_nRtPrmRT1 'RTO & RT1
                For zp = 1 To LG
12360:              'UPGRADE_WARNING: Couldn't resolve default property of object Values(zp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Values(zp) = 0
                Next zp
            Case gc_nRtPrmIRR 'IRR - Values() not used
                For zp = 1 To LG
12370:              'UPGRADE_WARNING: Couldn't resolve default property of object Values(zp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Values(zp) = 0
                Next zp
1030:       Case Else 'VARIABLES DEFINED IN FISCAL DEFINITION
1031:           If Numvar <> 1 Then
                    For zp = 1 To LG
1032:                   'UPGRADE_WARNING: Couldn't resolve default property of object Values(zp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Values(zp) = RVN(zp, param - 100)
1033:               Next zp
                End If

        End Select

fillin:

        ' Breaks(matchtot%) are usually found under the Amount column
        ' For DTE though, these must be calculated
        Dim Breaks(matchtot) As Object
        ' GDP 20 Jan 2003
        ' Changed numberic value of param% in comparison
        ' 27 May 2003 JWD (C0700) Replace number with symbol
        If param = gc_nRtPrmDTE Then 'DTE     ' was: = 56 Then
            For xp = 1 To matchtot
                'UPGRADE_WARNING: Couldn't resolve default property of object ck(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                If ratein(ck(xp), 5) = -991 Then
                    'UPGRADE_WARNING: Couldn't resolve default property of object Breaks(xp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Breaks(xp) = 0
                    'UPGRADE_WARNING: Couldn't resolve default property of object ck(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                ElseIf ratein(ck(xp), 5) = -990 Then
                    'UPGRADE_WARNING: Couldn't resolve default property of object Breaks(xp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Breaks(xp) = (mo - 1) / 12
                    'UPGRADE_WARNING: Couldn't resolve default property of object ck(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                ElseIf ratein(ck(xp), 5) = -989 Then
                    'UPGRADE_WARNING: Couldn't resolve default property of object Breaks(xp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Breaks(xp) = (Y2 - YR) + ((M2 - 1) / 12)
                    'UPGRADE_WARNING: Couldn't resolve default property of object ck(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                ElseIf ratein(ck(xp), 5) = -993 Then
                    'UPGRADE_WARNING: Couldn't resolve default property of object Breaks(xp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Breaks(xp) = Y1 - YR
                    'UPGRADE_WARNING: Couldn't resolve default property of object ck(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                ElseIf ratein(ck(xp), 5) = -992 Then
                    'UPGRADE_WARNING: Couldn't resolve default property of object Breaks(xp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Breaks(xp) = (Y1 - YR) + ((M1 - 1) / 12)
                End If
            Next xp

        Else
            For xp = 1 To matchtot
                'UPGRADE_WARNING: Couldn't resolve default property of object ck(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Couldn't resolve default property of object Breaks(xp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                Breaks(xp) = ratein(ck(xp), 5)
            Next xp
        End If

        ' Fill in Rates

        ' param% = code in Prm column
        ' matchtot% = # of matching lines on Rate Screen
        ' Values(lg) = values associated with code specified in Prm
        ' Breaks(matchtot%) = amounts entered under Amount column
        ' BrkRates(matchtot%) = rates entered under Rate column
        ' method% = method entered under Sliding Scale column
        ' VarRates(lg) = annual array of rates returned to Fiscal

        ReDim Preserve Breaks(matchtot + 1)
1715:
        '6-2-92
        'If the parameter is a USER DEFINED VARIABLE (param% > 100)
        '  then we need to see if the user has specified ANNUAL or CUM
        '  on the MRP Misc Rate Parameters screen.  If CUM was selected,
        '  treat the var like the other CUM vars (param% 26 - 33)
        '  else treat it like an annual var.
        'First, search TM() (from MRPScreen) and see if TM(n, 3) = 2
        '  (YES/CUM). THE DEFAULT IS NO/ANNUAL.
        cumvar = 0 'flag - TRUE = CUM  FALSE = ANNUAL
        If param > 100 Then 'only do for user defined variables
            For qq = 1 To TMT
                If sTMV(qq) = FVAR(Numvar) Then 'found the variable in MRPScreen
                    If TM(qq, 3) = 2 Then 'CUMULATIVE
                        cumvar = -1
                    End If
                    Exit For
                End If
            Next qq
        End If
1720:
        ' GDP 20 Jan 2003
        ' Changed numeric value of param% in comparison
        'If (param% >= 42 And param% <= 57) Or cumvar = -1 Then 'cumulatives
        ' 27 May 2003 JWD (C0700) Replace numbers with symbols
        If ((param >= gc_nRtPrmOLC And param <= gc_nRtPrmCAL) Or param = gc_nRtPrmCUV) Or cumvar = -1 Then 'cumulatives
            '   SELECT CASE param%
            '    CASE 26, 27, 28, 29, 30, 31, 32, 33          ' cumulatives
            If METHOD = 2 Or METHOD = 3 Then 'no fractional years
                For z = 1 To LG
                    'UPGRADE_WARNING: Couldn't resolve default property of object BrkRates(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    VarRates(z) = BrkRates(1)
                Next z
                p = 2

                For z = 1 To LG
nextp21:            If p > matchtot Then
                        GoTo nextz21
                    End If
                    'UPGRADE_WARNING: Couldn't resolve default property of object Breaks(p). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: Couldn't resolve default property of object Values(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    If Values(z) >= Breaks(p) Then
                        For q = z To LG
                            'UPGRADE_WARNING: Couldn't resolve default property of object BrkRates(p). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            VarRates(q) = BrkRates(p)
                        Next q
                        p = p + 1
                        GoTo nextp21
                    End If
nextz21:        Next z
1730:
            ElseIf METHOD = 0 Or METHOD = 1 Then  ' with fractional years
                For z = 1 To LG
                    'UPGRADE_WARNING: Couldn't resolve default property of object BrkRates(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    VarRates(z) = BrkRates(1)
                Next z
                p = 2
                For z = 1 To LG
                    'Determine how many cumulative breakpoints are reached this year
                    'Startp% = first breakpoint reached
                    '  Endp% = last breakpoint reached
                    Startp = 0
                    Endp = 0
loopp1:             If p > matchtot Then GoTo loopp2
                    'UPGRADE_WARNING: Couldn't resolve default property of object Breaks(p). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: Couldn't resolve default property of object Values(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    If Values(z) < Breaks(p) Then GoTo loopp2 'did not hit breakpoint
                    If Startp = 0 Then 'check if first breakpoint reached within the year
                        Startp = p
                        Endp = p
                    Else 'if more than one breakpoint, redefine Endp%
                        Endp = p
                    End If
                    p = p + 1
                    GoTo loopp1
loopp2:             ' We now know Startp% & Endp% for this year
                    If Startp = 0 Then GoTo nextz5 ' if no breakpoints reached, skip it
                    For ppy = Startp To Endp
                        'UPGRADE_WARNING: Couldn't resolve default property of object Values(z - 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object Values(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        If Values(z) - Values(z - 1) <> 0 Then
                            If ppy = Startp Then
                                'UPGRADE_WARNING: Couldn't resolve default property of object BrkRates(ppy - 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object Values(z - 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object Values(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object Breaks(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                VarRates(z) = ((Breaks(ppy) - Values(z - 1)) / (Values(z) - Values(z - 1))) * BrkRates(ppy - 1)
                            ElseIf ppy <> Startp Then
                                'UPGRADE_WARNING: Couldn't resolve default property of object BrkRates(ppy - 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object Values(z - 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object Values(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object Breaks(ppy - 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object Breaks(ppy). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                VarRates(z) = VarRates(z) + (((Breaks(ppy) - Breaks(ppy - 1)) / (Values(z) - Values(z - 1))) * BrkRates(ppy - 1))
                            End If
                        End If
                        If ppy = Endp Then
                            'UPGRADE_WARNING: Couldn't resolve default property of object Values(z - 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            'UPGRADE_WARNING: Couldn't resolve default property of object Values(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            If Values(z) - Values(z - 1) <> 0 Then
                                'UPGRADE_WARNING: Couldn't resolve default property of object BrkRates(ppy). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object Values(z - 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object Values(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object Breaks(ppy). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                VarRates(z) = VarRates(z) + (((Values(z) - Breaks(ppy)) / (Values(z) - Values(z - 1))) * BrkRates(ppy))
                            End If
                        End If
                    Next ppy
1740:
                    ' flood remaining years with current rate
                    tmp = z + 1
                    For zz = tmp To LG
                        'UPGRADE_WARNING: Couldn't resolve default property of object BrkRates(Endp). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        VarRates(zz) = BrkRates(Endp)
                    Next zz
nextz5:         Next z
            End If
            ' GDP 20 Jan 2003
            ' Changed numeric value of param% in comparison
            'ElseIf param% >= 59 And param% <= 61 Then            ' IRR, RTO & RT1
            ' 27 May 2003 JWD (C0700) Replace numbers with symbols
        ElseIf param >= gc_nRtPrmRTO And param <= gc_nRtPrmIRR Then  ' IRR, RTO & RT1
            'CASE 35, 36, 37                       ' IRR, RTO & RT1
            CalcIrr(Numvar, matchtot, Breaks, BrkRates, param, METHOD, VarRates)

        Else 'annual amounts
            'CASE ELSE                    ' annual amounts
            If METHOD = 1 Then ' sliding scale = YES
                For z = 1 To LG
                    doneit = 0
                    p = 0
extp:               p = p + 1
                    If p > matchtot Then GoTo nextz
                    'UPGRADE_WARNING: Couldn't resolve default property of object Values(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    If Values(z) = 0 Then
                        'UPGRADE_WARNING: Couldn't resolve default property of object BrkRates(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        VarRates(z) = BrkRates(1)
                        doneit = 1
                    Else
1745:
                        'UPGRADE_WARNING: Couldn't resolve default property of object Breaks(p + 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object Values(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        If (Values(z) < Breaks(p + 1)) Or (p = matchtot) Then
                            'UPGRADE_WARNING: Couldn't resolve default property of object Values(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            If Values(z) > 0 Then
                                'UPGRADE_WARNING: Couldn't resolve default property of object Values(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object Breaks(p). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object BrkRates(p). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                VarRates(z) = VarRates(z) + (BrkRates(p) * ((Values(z) - Breaks(p)) / Values(z)))
                            Else
                                VarRates(z) = 0
                            End If
                            doneit = 1
                        Else
                            'UPGRADE_WARNING: Couldn't resolve default property of object Values(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            If Values(z) > 0 Then
                                'UPGRADE_WARNING: Couldn't resolve default property of object Values(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object Breaks(p). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object Breaks(p + 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object BrkRates(p). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                VarRates(z) = VarRates(z) + (BrkRates(p) * ((Breaks(p + 1) - Breaks(p)) / Values(z)))
                            Else
                                VarRates(z) = 0
                            End If
                        End If
                        If doneit <> 1 Then GoTo extp
                    End If
nextz:          Next z
            ElseIf METHOD = 2 Then  ' Sliding Scale = NO
                For z = 1 To LG
                    'UPGRADE_WARNING: Couldn't resolve default property of object BrkRates(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    VarRates(z) = BrkRates(1)
                Next z
                For z = 1 To LG
                    p = 0
nextp1:             p = p + 1
                    If p > matchtot Then GoTo nextz1
                    'UPGRADE_WARNING: Couldn't resolve default property of object Breaks(p). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: Couldn't resolve default property of object Values(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    If Values(z) >= Breaks(p) Then
                        'UPGRADE_WARNING: Couldn't resolve default property of object BrkRates(p). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        VarRates(z) = BrkRates(p)
                        GoTo nextp1
                    End If
nextz1:         Next z
            Else ' Sliding Scale = SPC
1755:
                p = 1
                For z = 1 To LG
nextp2:             If p > matchtot Then GoTo nextz2
                    'UPGRADE_WARNING: Couldn't resolve default property of object Breaks(p). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: Couldn't resolve default property of object Values(z). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    If Values(z) >= Breaks(p) Then
                        For q = z To LG
                            'UPGRADE_WARNING: Couldn't resolve default property of object BrkRates(p). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            VarRates(q) = BrkRates(p)
                        Next q
                        p = p + 1
                        GoTo nextp2
                    End If
nextz2:         Next z
            End If
        End If
        'END SELECT

endit:

        'do some housekeeping.  we are runnung out of stack space
        ReDim BrkRates(1)
        ReDim Values(1)
        ReDim Breaks(1)
        'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        System.Array.Clear(BrkRates, 0, BrkRates.Length)
        'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        System.Array.Clear(Values, 0, Values.Length)
        'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        System.Array.Clear(Breaks, 0, Breaks.Length)



        Erase sRateInV
        Erase ratein
1760:
        'PRINT "LeavingRateCalc  stack = "; FRE(-2)

        Exit Sub
        '------------------------------------------------------------------------

ratecalcerror:
        ''Print "RateCalc SUB Error: "; Err; " at line number: "; Erl
        MsgBox("RateCalc SUB Error: " & Err.Number & " at line number: " & Erl())

        Resume Next

    End Sub


    Public Function EquivalencyVolumeProduction(ByVal nYear As Short, ByVal bIncludeOilProduction As Boolean, ByVal bIncludeGasProduction As Boolean) As Double

        Dim dProduction As Double
        Dim dEquivalencyFactor As Double

        Dim dTotalOilProduction As Double
        Dim dTotalGasProduction As Double

        dTotalOilProduction = A(nYear, gc_nAOIL) + A(nYear, gc_nAOV1) + A(nYear, gc_nAOV3) + A(nYear, gc_nAOV5) + A(nYear, gc_nAOV7) + A(nYear, gc_nAOV9)

        dTotalGasProduction = A(nYear, gc_nAGAS) + A(nYear, gc_nAOV2) + A(nYear, gc_nAOV4) + A(nYear, gc_nAOV6) + A(nYear, gc_nAOV8) + A(nYear, gc_nAOV0)

        If gn(2) <= 0 Then
            dEquivalencyFactor = 6
        Else
            dEquivalencyFactor = gn(2)
        End If


        If PPR = 1 Then 'OIL is primary product

            If bIncludeOilProduction Then
                dProduction = dTotalOilProduction
            End If

            If bIncludeGasProduction Then
                dProduction = dProduction + (dTotalGasProduction / dEquivalencyFactor)
            End If

        Else 'GAS is primary product

            If bIncludeOilProduction Then
                dProduction = dTotalOilProduction * dEquivalencyFactor
            End If

            If bIncludeGasProduction Then
                dProduction = dProduction + dTotalGasProduction
            End If

        End If

        EquivalencyVolumeProduction = dProduction

    End Function

    Public Function EquivalencyVolumeProductionByProdType(ByVal nYear As Short, ByVal sProdType As String) As Double

        Dim dProduction As Double
        Dim dEquivalencyFactor As Double

        Dim dTotalOilProduction As Double
        Dim dTotalGasProduction As Double

        Select Case sProdType

            Case "PRD"
                dTotalOilProduction = A(nYear, gc_nAOIL) + A(nYear, gc_nAOV1) + A(nYear, gc_nAOV3) + A(nYear, gc_nAOV5) + A(nYear, gc_nAOV7) + A(nYear, gc_nAOV9)

                dTotalGasProduction = A(nYear, gc_nAGAS) + A(nYear, gc_nAOV2) + A(nYear, gc_nAOV4) + A(nYear, gc_nAOV6) + A(nYear, gc_nAOV8) + A(nYear, gc_nAOV0)
            Case "OIL"
                dTotalOilProduction = A(nYear, gc_nAOIL)
            Case "GAS"
                dTotalGasProduction = A(nYear, gc_nAGAS)
            Case "OV1"
                dTotalOilProduction = A(nYear, gc_nAOV1)
            Case "OV2"
                dTotalGasProduction = A(nYear, gc_nAOV2)
            Case "OV3"
                dTotalOilProduction = A(nYear, gc_nAOV3)
            Case "OV4"
                dTotalGasProduction = A(nYear, gc_nAOV4)
            Case "OV5"
                dTotalOilProduction = A(nYear, gc_nAOV5)
            Case "OV6"
                dTotalGasProduction = A(nYear, gc_nAOV6)
            Case "OV7"
                dTotalOilProduction = A(nYear, gc_nAOV7)
            Case "OV8"
                dTotalGasProduction = A(nYear, gc_nAOV8)
            Case "OV9"
                dTotalOilProduction = A(nYear, gc_nAOV9)
            Case "OV0"
                dTotalGasProduction = A(nYear, gc_nAOV0)
        End Select

        If gn(2) <= 0 Then
            dEquivalencyFactor = 6
        Else
            dEquivalencyFactor = gn(2)
        End If

        If PPR = 1 Then 'OIL is primary product
            dProduction = dTotalOilProduction + (dTotalGasProduction / dEquivalencyFactor)
        Else 'GAS is primary product
            dProduction = (dTotalOilProduction * dEquivalencyFactor) + dTotalGasProduction
        End If

        EquivalencyVolumeProductionByProdType = dProduction

    End Function




    ' $subtitle: 'CalcValues'
    ' $Page:
    Sub CalcValues(ByRef x As Short, ByRef Arg1 As String, ByRef Arg2 As String, ByRef op As String, ByRef Datacol() As Single)
        Dim cum As Single
        Dim i As Short
        Dim stream As String
        Dim j As Single
        Dim ck As Single
        Dim YRS As Short
        Dim both As Short
        '-----------------------------------------------------------------------
        'This sub is passed two categories and an operator (>,<,+,-,*,/,C(CUM),T(TOT))
        'The values of for each category is calculated (using RetrieveValues)
        '  and then the two arrays are calculated (depending on the operator)
        '  and the result is returned in datacol!()
        '-----------------------------------------------------------------------
        ' Modifications:
        ' 7 Feb 1996 JWD
        '          Removed declaration and setting of bDebugging.  Now declared
        '       in common and set in MAINEXEC.
        '-----------------------------------------------------------------------
        Dim bArg1 As Short
        Dim bArg2 As Short
        '---------------------------------------------------------
1500:

        If BDebugging Then
            FileOpen(17, "calc.log", OpenMode.Append)
            PrintLine(17, "#####################################################")
            PrintLine(17, " in CalcValues SUB")
            PrintLine(17, "X = " & x & "  Arg1$ = " & Arg1 & "  Arg2$ = " & Arg2 & "  op$ = " & op)
            FileClose(17)
        End If


        both = False 'flag if both arguments were entered
        YRS = UBound(Datacol)
        ReDim Datacol(YRS) 'make sure it is empty
        Dim wrk1(YRS) As Single
        Dim wrk2(YRS) As Single 'temp arrays to hold values for each category

        'see if user entered arg1$, arg2$ or both
        If Arg1 <> "" Then
            bArg1 = True
        End If
        If Arg2 <> "" Then
            bArg2 = True
        End If
        If bArg1 And bArg2 Then
            both = True
        End If
        If BDebugging Then
            FileOpen(17, "calc.log", OpenMode.Append)
            PrintLine(17, " in CalcValues SUB")
            PrintLine(17, "bArg1 = " & bArg1 & "  bArg2 = " & bArg2 & "  both% = " & both)
            FileClose(17)
        End If


        'get the values for the entered categories
        If bArg1 Then
            ck = 0
            For j = 1 To TDT ' SEE IF VARIABLE DEFINED (10/95 used to go to X-1)
                If BDebugging Then
                    FileOpen(17, "calc.log", OpenMode.Append)
                    PrintLine(17, " in CalcValues SUB")
                    PrintLine(17, "TD$(" & j & ", 1) = " & TD(j, 1))
                    FileClose(17)
                End If

                If Arg1 = TD(j, 1) Then
                    ck = j
                End If
            Next j
            If ck = 0 Then
                RetrieveValues(Arg1, stream, wrk1)
            Else ' USE VARIABLE FOUND
                For j = 1 To LG
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrk1(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrk1(j). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    wrk1(j) = wrk1(j) + RVN(j, ck)
                Next j

            End If
        End If
        If bArg2 Then
            ck = 0
            For j = 1 To TDT ' SEE IF VARIABLE DEFINED
                If Arg2 = TD(j, 1) Then
                    ck = j

                End If
            Next j
            If ck = 0 Then
                RetrieveValues(Arg2, stream, wrk2)
            Else ' USE VARIABLE FOUND

                For j = 1 To LG
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrk2(j). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    wrk2(j) = RVN(j, ck)
                Next j

            End If
        End If
        If BDebugging Then
            FileOpen(17, "calc.log", OpenMode.Append)
            For j = 1 To UBound(wrk1)
                PrintLine(17, "wrk1!(" & j & ") = ", TAB(10), wrk1(j), TAB(25), "wrk2!(" & j & ") = ", TAB(35), wrk2(j))
            Next j
            FileClose(17)
        End If




        'now perform requested operation on the two data arrays
        Select Case op
            Case "<" 'element by element, return lesser value
                For i = 1 To YRS
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrk2(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrk1(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    If wrk1(i) < wrk2(i) Then
                        'UPGRADE_WARNING: Couldn't resolve default property of object wrk1(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Datacol(i) = wrk1(i)
                    Else
                        'UPGRADE_WARNING: Couldn't resolve default property of object wrk2(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Datacol(i) = wrk2(i)
                    End If
                Next i
            Case ">" 'element by element, return greater value
                For i = 1 To YRS
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrk2(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrk1(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    If wrk1(i) > wrk2(i) Then
                        'UPGRADE_WARNING: Couldn't resolve default property of object wrk1(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Datacol(i) = wrk1(i)
                    Else
                        'UPGRADE_WARNING: Couldn't resolve default property of object wrk2(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Datacol(i) = wrk2(i)
                    End If
                Next i
            Case "+" 'element by element, return sum of two values
                For i = 1 To YRS
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrk2(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrk1(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Datacol(i) = wrk1(i) + wrk2(i)
                Next i
            Case "-" 'element by element, return difference
                For i = 1 To YRS
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrk2(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrk1(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Datacol(i) = wrk1(i) - wrk2(i)
                Next i
            Case "*" 'element by element, return product
                For i = 1 To YRS
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrk2(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrk1(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Datacol(i) = wrk1(i) * wrk2(i)
                Next i
            Case "/" 'element by element, return dividend
                If bArg2 Then 'don't divide by 0!
                    For i = 1 To YRS
                        'UPGRADE_WARNING: Couldn't resolve default property of object wrk2(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        If wrk2(i) <> 0 Then
                            'UPGRADE_WARNING: Couldn't resolve default property of object wrk2(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            'UPGRADE_WARNING: Couldn't resolve default property of object wrk1(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            Datacol(i) = wrk1(i) / wrk2(i)
                        End If
                    Next i
                End If
            Case "C" 'running cum of values (one or two categories)
                cum = 0 'initialize accumulator
                For i = 1 To YRS
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrk1(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    cum = cum + wrk1(i)
                    Datacol(i) = cum
                Next i
            Case "T" 'sums one or both categories and returns datacol!()
                '  with every element containing the total
                cum = 0
                For i = 1 To YRS 'add up all values
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrk1(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    cum = cum + wrk1(i)
                Next i
                For i = 1 To YRS
                    Datacol(i) = cum 'put total in every element of datacol!()
                Next i
            Case "" ' when no operator code, take 1st parameter
                For i = 1 To YRS
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrk1(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Datacol(i) = wrk1(i)
                Next i
        End Select

        If BDebugging Then
            FileOpen(17, "calc.log", OpenMode.Append)
            For j = 1 To UBound(Datacol)
                PrintLine(17, "DataCol!(" & j & ") = ", TAB(10), Datacol(j))
            Next j
            FileClose(17)
        End If


1599:

    End Sub

    ' $subtitle: 'RetrieveValues'
    ' $Page:
    Sub RetrieveValues(ByRef Cat As String, ByRef stream As String, ByRef Datacol() As Single)
        Dim j As Short
        Dim x As Short
        Dim yrptr As Short
        Dim i As Short
        Dim foundint As Short
        Dim foundmy3 As Short
        Dim foundann As Short
        Dim foundbda As Short
        Dim ptr As Short
        '----------------------------------------------------------------------
        '       RetrieveValues - receive category and return an array of
        '         the annual values for the category from A(), B(), MY3()
        '         Parameters:
        '               category$ - from the list of choices for a column
        '               datacol!(1-LG) - carries the returned values
        '***** NOTE: *****
        ' For this sub to work, there can be NO overlap between the array categories.
        '  Namely: a variable cannot be in two or more of: BDA(), ANN() and MY2().
        '----------------------------------------------------------------------
        ' Modifications:
        ' 2 Feb 1996 JWD
        '          Changed ON localerror GOTO to ON LOCAL ERROR GOTO.  Previous
        '       version was a computed branch statement which was never taken
        '       because localerror was alway 0.  Correction establishes
        '       intended local error handler.
        ' 20 Jan 2003 GDP
        '    -> Changed required for extra volumes
        '
        ' 16 May 2005 JWD
        '  -> Add new operating expense categories OX6-O20.
        '     (C0877)
        '
        ' 31 May 2005 JWD
        '  -> Correct subscript value for O20 in block summing,
        '     OPX, typo had 2 instead of 20 as subscript value.
        '     (C0881)
        '----------------------------------------------------------------------
9700:
        '~~~~ 2 Feb 1996 JWD Change next statement.
        On Error GoTo RetrieveValuesError

        'make sure results array is empty
        ReDim Datacol(UBound(Datacol))
        'count of the number of categories by type
        '  annct% = 14
        '  bdact% = 20
        '  cpxct% = 20   '1-7-92  added BL2 & BL3 to CAPEX categories
        'DPR (depreciation), DPR (depletion) cannot be handled here
        'if arg$ is NULL then don't bother - the answer is 0
        If Len(Cat) = 0 Or Cat = "DPL" Or Cat = "DPR" Then
            Exit Sub
        End If
        'dim arrays for valid codes (so we can search in a loop)
9702:
        ' ReDim bdacats$(bdact%), anncats$(annct%), cpxcats$(cpxct%)
        '--------------------------------------------------------------------
        'Build local arrays of all category codes.
        'We will search these lists to determine which
        'array in common to use [A(), B(), MY3()]. By searching
        'these arrays, we not only find out which array to use,
        'but also which element in A() or B() to retrieve.
        'BDA categories (A())
        '  dum$ = "OILGASOV1OV2RESWINOPCGPCOP1OP2OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5"
        '  For i% = 1 To bdact%
        '    bdacats$(i%) = Mid$(dum$, (i% - 1) * 3 + 1, 3)
        '  Next i%
        'ANN  categories  (B())
        '  dum$ = "PR1PR2PR3PR4PR5PRTDP1DP2DP3OT1OT2OT3OT4OT5"
        '  For i% = 1 To annct%
        '    anncats$(i%) = Mid$(dum$, (i% - 1) * 3 + 1, 3)
        '  Next i%
        'CPX categories   (MY3())
        '  dum$ = "BNSLSERENGEOEDHEDSADHASCDNPDVPPLFFCLTRNEORCP1CP2CP3BALBL2BL3"
        '  For i% = 1 To cpxct%
        '    cpxcats$(i%) = Mid$(dum$, (i% - 1) * 3 + 1, 3)
        '  Next i%
        '  dum$ = ""
9703:
        '--------------------------------------------------------------------
        'see which array cat$ is in (A(), B(), or MY3())
        ptr = 0
        foundbda = False : foundann = False : foundmy3 = False : foundint = False
        '  For i% = 1 To bdact%
        '    If bdacats$(i%) = cat$ Then
        '      foundbda% = True
        '      ptr% = i%
        '    End If
        '  Next i%
        SearchCodeString(cBDACats, Cat, 3, ptr)
        foundbda = Not (ptr = 0)
9704:
        If ptr = 0 Then 'not found yet
            '    For i% = 1 To annct%
            '      If anncats$(i%) = cat$ Then
            '        foundann% = True
            '        ptr% = i%
            '      End If
            '    Next i%
            SearchCodeString(cANNCats, Cat, 3, ptr)
            foundann = Not (ptr = 0)
        End If

9705:
        If ptr = 0 Then 'not found yet
            '         For i% = 1 To cpxct%
            '            If cpxcats$(i%) = cat$ Then
            '        foundmy3% = True
            '        ptr% = i%               'item number in the MY3() list
            '      End If
            '    Next i%
            SearchCodeString(cCPXCats, Cat, 3, ptr)
            foundmy3 = Not (ptr = 0)
        End If
9706:
        If ptr = 0 Then 'we need to see if the category is "INT" (loan interest)
            If Cat = "INT" Then
                foundint = True
                ptr = 1
            End If
        End If

        '--------------------------------------------------------------------

        'if found in bda or ann or my3() then load datacol!()
9707:


        If ptr > 0 Then 'cat$ was found

            If foundbda Then 'BDA category
                'if Stream$ = "", returned values are A() unchanged
                For i = 1 To UBound(Datacol)
                    Datacol(i) = A(i, ptr)
                Next i
                ' GDP 20 Jan 2003
                ' Changed numeric values to constants in following If.. ElseIf...ElseIF
                If stream = "GRS" Then ' this means A() * working interest
                    If (ptr >= gc_nAMINVOL And ptr <= gc_nAMAXVOL) Or (ptr >= gc_nAMINOPX And ptr <= gc_nAMAXOPX) Then
                        For i = 1 To UBound(Datacol)
                            Datacol(i) = Datacol(i) * WIN(i)
                        Next i
                    End If
9708:
                ElseIf stream = "GRP" Or Len(stream) = 0 Then  ' this means value at current status
                    If (ptr >= gc_nAMINVOL And ptr <= gc_nAMAXVOL) Then
                        For i = 1 To UBound(Datacol)
                            Datacol(i) = Datacol(i) * WIN(i) * (1 - PARTRATE(i))
                        Next i
                    ElseIf (ptr >= gc_nAMINOPX And ptr <= gc_nAMAXOPX) Then
                        For i = 1 To UBound(Datacol)
                            Datacol(i) = Datacol(i) * WIN(i) * (1 - OPEXRATE(i))
                        Next i
                    End If
9709:
                ElseIf stream = "GVT" Then  ' this means government share
                    If (ptr >= gc_nAMINVOL And ptr <= gc_nAMAXVOL) Then
                        For i = 1 To UBound(Datacol)
                            Datacol(i) = Datacol(i) * PARTRATE(i) * WIN(i)
                        Next i
                    ElseIf (ptr >= gc_nAMINOPX And ptr <= gc_nAMAXOPX) Then
                        For i = 1 To UBound(Datacol)
                            Datacol(i) = Datacol(i) * OPEXRATE(i) * WIN(i)
                        Next i
                    End If
                End If
9710:           ' an xxx entry is A() unchanged
            ElseIf foundann Then  'ANN category
                For i = 1 To UBound(Datacol)
                    Datacol(i) = B(i, ptr)
                Next i
            ElseIf foundmy3 Then
                'Search MY3 array for ALL records that match cat$. If any are found,
                '  put the gross amount into datacol!(). YR in common is the project
                '  start year. Expenses in the project start year go in first element
                '  of datacol!(). This puts the gross amount(s) in the
                '  proper element of datacol!().
                yrptr = 1 - YR 'if YR = 91, then yrptr% = -90
                For i = 1 To my3tt
9711:
                    If my3(i, 1) = ptr Then
                        x = my3(i, 3)
                        If x < 50 Then
                            x = x + 100
                        End If
                        x = x + yrptr
                        If x <= UBound(Datacol) And x > 0 Then
                            If Len(stream) = 0 Or stream = "GRP" Then
                                Datacol(x) = Datacol(x) + (my3(i, 5) * WINC(i) * GPRATE(i))
                            ElseIf stream = "GRS" Then
                                Datacol(x) = Datacol(x) + (my3(i, 5) * WINC(i))
                            ElseIf stream = "GVT" Then
                                Datacol(x) = Datacol(x) + (my3(i, 5) * (1 - GPRATE(i)) * WINC(i))
                            Else
                                Datacol(x) = Datacol(x) + my3(i, 5)
                            End If
                        End If
                    End If
                Next i
9712:
            ElseIf foundint Then  'interest from loans
                For i = 1 To UBound(Datacol)
                    Datacol(i) = INTRST(i)
                Next i
                If stream = "GRS" Then ' this means A() * working interest
                    For i = 1 To UBound(Datacol)
                        Datacol(i) = Datacol(i) * WIN(i)
                    Next i
                ElseIf stream = "GRP" Or Len(stream) = 0 Then  ' this means value at current status
                    For i = 1 To UBound(Datacol)
                        Datacol(i) = Datacol(i) * WIN(i) * (1 - PARTRATE(i))
                    Next i
9713:
                ElseIf stream = "GVT" Then  ' this means government share
                    For i = 1 To UBound(Datacol)
                        Datacol(i) = Datacol(i) * PARTRATE(i) * WIN(i)
                    Next i
                End If
            End If
        End If

        '--------------------------------------------------------------------
        'these categories require calling this sub again to retrieve another
        '  category's data to add to or multiply by to return the desired values.
        '  ie. if FISCAL.BAS calls this sub with OIL then we also need to
        '     get OPC and multiply the two together to return the datacol!()
9714:
        Dim wrkcol(UBound(Datacol)) As Single
        Dim codes(12) As String

        Select Case Cat
            Case "OIL" 'OIL = OIL * OPC
9715:
                RetrieveValues("OPC", stream, wrkcol)
                For i = 1 To UBound(Datacol)
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Datacol(i) = Datacol(i) * wrkcol(i)
                Next i
                '         Erase wrkcol!

            Case "GAS" 'GAS = GAS * GPC
9716:
                RetrieveValues("GPC", stream, wrkcol)
                For i = 1 To UBound(Datacol)
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Datacol(i) = Datacol(i) * wrkcol(i)
                Next i
                '         Erase wrkcol!

            Case "OV1" 'OV1 = OV1 * OP1
9717:
                RetrieveValues("OP1", stream, wrkcol)
                For i = 1 To UBound(Datacol)
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Datacol(i) = Datacol(i) * wrkcol(i)
                Next i
                '         Erase wrkcol!

            Case "OV2" 'OV2 = OV2 * OP2
9718:
                RetrieveValues("OP2", stream, wrkcol)
                For i = 1 To UBound(Datacol)
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Datacol(i) = Datacol(i) * wrkcol(i)
                Next i
                ' GDP 20 Jan 2003
                ' Added extra case statements for OV3 - OV0
            Case "OV3"
                RetrieveValues("OP3", stream, wrkcol)
                For i = 1 To UBound(Datacol)
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Datacol(i) = Datacol(i) * wrkcol(i)
                Next i
            Case "OV4"
                RetrieveValues("OP4", stream, wrkcol)
                For i = 1 To UBound(Datacol)
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Datacol(i) = Datacol(i) * wrkcol(i)
                Next i
            Case "OV5"
                RetrieveValues("OP5", stream, wrkcol)
                For i = 1 To UBound(Datacol)
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Datacol(i) = Datacol(i) * wrkcol(i)
                Next i
            Case "OV6"
                RetrieveValues("OP6", stream, wrkcol)
                For i = 1 To UBound(Datacol)
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Datacol(i) = Datacol(i) * wrkcol(i)
                Next i

            Case "OV7"
                RetrieveValues("OP7", stream, wrkcol)
                For i = 1 To UBound(Datacol)
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Datacol(i) = Datacol(i) * wrkcol(i)
                Next i
            Case "OV8"
                RetrieveValues("OP8", stream, wrkcol)
                For i = 1 To UBound(Datacol)
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Datacol(i) = Datacol(i) * wrkcol(i)
                Next i
            Case "OV9"
                RetrieveValues("OP9", stream, wrkcol)
                For i = 1 To UBound(Datacol)
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Datacol(i) = Datacol(i) * wrkcol(i)
                Next i
            Case "OV0"
                RetrieveValues("OP0", stream, wrkcol)
                For i = 1 To UBound(Datacol)
                    'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Datacol(i) = Datacol(i) * wrkcol(i)
                Next i

                '         Erase wrkcol!

            Case "PRD" 'PRD = OIL + GAS + OV1 + OV2
9719:
                ' 20 Jan 2003
                ' Increased size of codes and added "OV3" to "OV0"
                ReDim codes(12)
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(1) = "OIL"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(2) = "GAS"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(3). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(3) = "OV1"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(4). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(4) = "OV2"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(5). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(5) = "OV3"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(6). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(6) = "OV4"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(7). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(7) = "OV5"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(8). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(8) = "OV6"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(9). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(9) = "OV7"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(10). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(10) = "OV8"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(11). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(11) = "OV9"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(12). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(12) = "OV0"


                For j = 1 To 12
                    ReDim wrkcol(UBound(Datacol))
                    'UPGRADE_WARNING: Couldn't resolve default property of object codes$(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    RetrieveValues(codes(j), stream, wrkcol)
                    For i = 1 To UBound(Datacol)
                        'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Datacol(i) = Datacol(i) + wrkcol(i)
                    Next i
                Next j
                '         Erase wrkcol!, Codes$

            Case "OPX" 'OPX = OX1 + OX2 + OX3 + OX4 + OX5
9720:
                ' 16 May 2005 JWD (C0877) Add OX6-O20
                ReDim codes(20) ' was 5
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(1) = "OX1"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(2) = "OX2"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(3). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(3) = "OX3"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(4). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(4) = "OX4"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(5). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(5) = "OX5"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(6). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(6) = "OX6"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(7). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(7) = "OX7"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(8). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(8) = "OX8"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(9). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(9) = "OX9"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(10). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(10) = "OX0"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(11). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(11) = "O11"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(12). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(12) = "O12"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(13). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(13) = "O13"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(14). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(14) = "O14"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(15). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(15) = "O15"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(16). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(16) = "O16"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(17). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(17) = "O17"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(18). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(18) = "O18"
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(19). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(19) = "O19"
                ' 31 May 2005 JWD (C0881) correct subscript value
                'UPGRADE_WARNING: Couldn't resolve default property of object codes$(20). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                codes(20) = "O20" ' was: codes$(2) = "O20"
                ' End (C0881)
                For j = 1 To 20 ' was 5
                    ' End (C0877)
                    ReDim wrkcol(UBound(Datacol))
                    'UPGRADE_WARNING: Couldn't resolve default property of object codes$(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    RetrieveValues(codes(j), stream, wrkcol)
                    For i = 1 To UBound(Datacol)
                        'UPGRADE_WARNING: Couldn't resolve default property of object wrkcol(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Datacol(i) = Datacol(i) + wrkcol(i)
                    Next i
                Next j
                '         Erase wrkcol!, Codes$

            Case "EXP" 'EXP = GEO+EDH+EDS+ADH+ASC
9721:
                For i = 1 To my3tt
                    If my3(i, 1) >= 4 And my3(i, 1) <= 8 Then
                        x = my3(i, 3) - YR + 1
                        If Len(stream) = 0 Or stream = "GRP" Then
                            Datacol(x) = Datacol(x) + (my3(i, 5) * WINC(i) * GPRATE(i))
                        ElseIf stream = "GRS" Then
                            Datacol(x) = Datacol(x) + (my3(i, 5) * WINC(i))
                        ElseIf stream = "GVT" Then
                            Datacol(x) = Datacol(x) + (my3(i, 5) * (1 - GPRATE(i)) * WINC(i))
                        Else
                            Datacol(x) = Datacol(x) + my3(i, 5)
                        End If
                    End If
                Next i

            Case "DEV" 'DEV = DNP+DVP+PLF+FCL+TRN+EOR
9722:
                For i = 1 To my3tt
                    If my3(i, 1) >= 9 And my3(i, 1) <= 14 Then
                        x = my3(i, 3) - YR + 1
                        If Len(stream) = 0 Or stream = "GRP" Then
                            Datacol(x) = Datacol(x) + (my3(i, 5) * WINC(i) * GPRATE(i))
                        ElseIf stream = "GRS" Then
                            Datacol(x) = Datacol(x) + (my3(i, 5) * WINC(i))
                        ElseIf stream = "GVT" Then
                            Datacol(x) = Datacol(x) + (my3(i, 5) * (1 - GPRATE(i)) * WINC(i))
                        Else
                            Datacol(x) = Datacol(x) + my3(i, 5)
                        End If
                    End If
                Next i

            Case "CPX" 'CPX = BNS+LSE+REN+EXP+DEV+CP1+CP2+CP3
9723:
                For i = 1 To my3tt
                    x = my3(i, 3) - YR + 1
                    If Len(stream) = 0 Or stream = "GRP" Then
                        Datacol(x) = Datacol(x) + (my3(i, 5) * WINC(i) * GPRATE(i))
                    ElseIf stream = "GRS" Then
                        Datacol(x) = Datacol(x) + (my3(i, 5) * WINC(i))
                    ElseIf stream = "GVT" Then
                        Datacol(x) = Datacol(x) + (my3(i, 5) * (1 - GPRATE(i)) * WINC(i))
                    Else
                        Datacol(x) = Datacol(x) + my3(i, 5)
                    End If
                Next i
        End Select
9724:

        '--------------------------------------------------------------------
        'datacol!() is now filled out (if there was data to put in datacol!())

        '  Erase bdacats$, anncats$, cpxcats$

9725:


        Exit Sub

RetrieveValuesError:
        ''Print "RetrieveValues SUB Error: "; Err; " at line number: "; Erl
        MsgBox("RetrieveValues SUB Error: " & Err.Number & " at line number: " & Erl())
        Resume Next

    End Sub
End Module