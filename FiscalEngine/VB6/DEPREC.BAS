Attribute VB_Name = "DEPREC"
' $linesize: 132
' $title: 'GIANT v6.1 - 1996                         DEPREC.BAS'
' $subtitle: 'Depreciation program'
' Name:        DEPREC.BAS
' Function:    Compute Depreciation and Cost Recovery
'---------------------------------------------------------
' ********************************************************
' *             COPYRIGHT © 1986-2003 IHS ENERGY         *
' *                  ALL RIGHTS RESERVED                 *
' ********************************************************
' *   This program file is proprietary information of    *
' *                     IHS Energy.                      *
' *   Unauthorized use for any purpose is prohibited.    *
' ********************************************************
'---------------------------------------------------------
' Modifications:
' 3 Mar 1995 JWD
'  -> Convert module level executable code to subroutine.
'
' 8 Feb 1996 JWD
'  -> Replaced common block include file CTYIN.BAS with
'     CTYIN1.BG.
'  -> Replaced include file SUBINCL with UTIL5.BI.
'  -> Added interface declaration include file DEPREC.BI.
'  -> Changed Depreciation().
'
' 19 Feb 1996 JWD
'  -> Changed references to common array RATE() to
'     PARTRATE(). RATE is reserved function name in VB.
'  -> Add explicit declaration of default storage class as
'     Single.
'
'  25 Aug 1998 JWD
'  -> Changed Depreciation().
'
' 26 Mar 2001 JWD
'  -> Add Option Explicit declaration.
'  -> Changed Depreciation(). (C0292, 0293)
'
' 27 Mar 2001 JWD
'  -> Changed Depreciation(). (C0295)
'
' 3 Jul 2001 JWD
'  -> Changed Depreciation(). (C0345)
'  -> Add zzz_AddMonthlyInterestToCostRecovery() (C0345)
'  -> Add zzz_Interest(). (C0345)
'  -> Changed zzz_Interest(). (C0346)
'
' 16 Aug 2001 JWD
'  -> Changed Depreciation(). (C0389)
'
' 20 Nov 2002 JWD
'  -> Changed Depreciation(). (C0634)
'
' 20 Jan 2003 GDP
'  -> Changed Depreciation().
'
' 29 May 2003 JWD
'  -> Changed Depreciation(). (C0702)
'
' 12 Jan 2004 JWD
'  -> Changed Depreciation(). (C0772)
'
' 19 Jan 2004 JWD
'  -> Changed Depreciation(). (C0774)
'
' 21 Jan 2004 JWD
'  -> Changed Depreciation(). (C0774)
'
' 3 Feb 2004 JWD
'  -> Changed Depreciation(). (C0776)
'
' 13 May 2005 JWD
'  -> Changed Depreciation(). (C0877)
'-----------------------------------------------------------------------
Option Explicit

' $DYNAMIC
DefSng A-Z


 ' $INCLUDE: 'CTYIN1.BG'
   ' $INCLUDE: 'deprec.bi'
   ' $INCLUDE: 'util5.bi'

'$subtitle: 'Procedure: Depreciation'
'$page
Sub Depreciation(iX As Integer, bDPCR As Integer)
'-----------------------------------------------------------------------
' This program calculates depreciation and cost recovery.
' Parameters:
'       iX      is line in Fiscal Definition currently being processed.
'       bDPCR   is cost recovery indicator switch.  True=Cost Recovery.
'-----------------------------------------------------------------------
' Modifications:
' 8 Feb 1996 JWD
'  -> Removed include of SCRA1IN.BAS and SCRA1OUT.BAS.
'     Variables recorded on file are now in common.
'  -> Removed redim of arrays stored on file SCRA1.SCR.
'  -> Added iX and bDPCR to parameter list, were passed
'     to this procedure in scratch file SCRA1.SCR.
'
' 20 Feb 1996 JWD
'  -> Change CUR$ to sCur, duplicate definition (CUR()).
'  -> Change CGR$ to sCGR, duplicate definition (CGR()).
'  -> Change DP$ to sDP, duplicate definition (DP()).
'  -> Change MDC$ to sMDC, duplicate definition (MDC()).
'  -> Commented out dimension of FRC(), not referenced.
'  -> Replace SWAP statements in sorting subroutine with
'     code to produce same result, SWAP not supported in
'     VB.
'
' 25 Aug 1998 JWD
'  -> Add If block to DEPREC.BAS:Depreciation() after line
'     2979 to branch to 2980 if there are no capital
'     expenditures (MY3TT = 0). This explicitly performs
'     that which occurrs implicitly in previous versions
'     of MAINEXEC. Compilation under VB5 changes the
'     previous behavior in that circumstance due to the
'     loop code being un-initialized and due to
'     differences in code generation across the versions
'     of Basic. Cause is a branch into the middle of the
'     loop code (to 1310) from If test after line 1020.
'
' 26 Mar 2001 JWD
'  -> Add symbol declarations as required to satisfy
'     Option Explicit directive.
'  -> Add code to calculate interest on unrecovered future
'     scheduled amounts when specified by new accrue
'     interest option entries. (C0292)
'  -> Correct calculation of interest on new method.
'     (C0293)
'
' 27 Mar 2001 JWD
'  -> Correct failure to calculate pre-prod interest
'     accrual for code YS2. (C0295)
'
' 3 Jul 2001 JWD
'  -> Add new pre-production interest accrual code
'     for cost recovery amounts. (C0345)
'  -> Add new accrue interest calculation method to
'     calculate interest monthly on post-production
'     unrecovered amounts. (C0345)
'
' 16 Aug 2001 JWD
'  -> Change loop checking the cost recovery sequence
'     to loop through as many as are in the array.
'     (C0389)
'
' 20 Nov 2002 JWD
'  -> Change to eliminate imposition of ceiling on
'     depreciation. Ceiling should only be imposed on cost
'     recovery. This is done by setting the flag CLEX$
'     to "N" if bDPCR is false. CLEX$ is the sole control
'     over whether or not the ceiling is applied. (C0634)
'
' 20 Jan 2003 GDP
'  -> Changed references to A array for additional volume streams and
'     A array references to use constants defined in modArrayConst.bas
'
' 29 May 2003 JWD
'  -> Changed references to A array for operating expense
'     categories in cost recovery section (between lines
'     4808 and 4810) to use symbols in place of numbers. (C0702)
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
'  -> Changed report page object type from CGiantRptPageA1
'     to interface IGiantRptPageAssignSub. (C0774)
'  -> Changed method call on report object to call
'     NewSubScheduleRptPage method to retrieve object of
'     IGiantRptPageAssignSub interface. (C0774)
'
' 3 Feb 2004 JWD
'  -> Remove explicit writes to report file. (C0776)
'
' 13 May 2005 JWD
'  -> Add new operating expense categories OX6-O20.
'     (C0877)
'-----------------------------------------------------------------------
   Dim bSORTED    As Integer
   Dim bSWAPPING  As Integer
   Dim bSwitch    As Integer
   Dim bTBL       As Integer
   Dim bMatch     As Integer
   
   Dim i          As Integer
   Dim iBASERV    As Integer
   Dim iBYr       As Integer
   Dim iDEPRLIFE  As Integer
   Dim iLIFE      As Integer
   Dim iML        As Integer
   Dim iPX        As Integer
   Dim iRow       As Integer
   Dim iSTRT      As Integer
   Dim iTB        As Integer
   Dim iXM        As Integer
   Dim iXMOI      As Integer
   Dim iXXX       As Integer
   Dim iXYZ       As Integer
   Dim iYM        As Integer
   Dim iYP        As Integer
   Dim iZDB       As Integer
   Dim iZZ        As Integer

   Dim iLL         As Integer
   Dim iLM         As Integer
   Dim Numvar     As Integer
   Dim Ratetot    As Integer
   Dim rRAT       As Single
   Dim rTmp       As Single
   Dim sDum       As String
   
   Dim unrecovered As Single
   Dim interest_on_unrecovered As Single
   Dim iXLM       As Long
   
   ' Declarations required by Option Explicit
   Dim cd$
   Dim cdtitle$
   Dim doit$
   Dim CapYr      As Single
   Dim CapYr1     As Single
   Dim DeprStrt   As Single
   Dim year1      As Single
   Dim DBSL       As Single
   Dim bse        As Single
   Dim interest   As Single
   Dim TIMEX      As Single
   Dim CRDT       As Single
   Dim FirstYearAllow As Single
   Dim lif1       As Single
   Dim lif2       As Single
   Dim rFrac      As Single
   Dim cdgt       As Single
   Dim BEGBAL     As Single
   Dim CDPR       As Single
   Dim CDepr      As Single
   Dim iLZ        As Single
   Dim DeprBase   As Single
   Dim SLDepr     As Single
   Dim REMVOL     As Single
   Dim ProdStart  As Single
   Dim reserves   As Single
   Dim UOPStrm    As Single
   Dim Equiva     As Single
   Dim TempSum    As Single
   Dim DumRes     As Single
   Dim REVAL      As Single
   Dim TEM%
   Dim TEM1%
   Dim CLEX$
   Dim Fd%
   Dim iPY        As Single
   Dim DefAmount  As Single
   Dim searcher$
   Dim param%
   Dim j          As Single
   Dim q%
   Dim w%
   Dim found%
   Dim e%
   Dim netopex    As Single
   Dim RPBASE     As Single
   Dim loopit     As Single
   Dim DEPR()     As Single
      
'---------------------------------------------------------
20
21   ReDim REPA(LG), rDum(LG) As Single
22   ReDim clngx(LG)
     ReDim XMO(my3tt) As Integer
23   ReDim DEPR(LG), dprrat(LG), ACT(LG, 12), clngs(LG)
'~~~~ReDim FRC(LG)
     ReDim CLRA(LG), ColumnNm$(16)

27
28
        ' SET LOOP INDICATORS iX FROM FISCAL TO iXXX FOR THIS PROGRAM
        ' SO THAT iX CAN BE USED HERE

      iXXX = iX

        'look up the long title for the current variable - we will show it
        '  on the depreciation schedule
      cd$ = TD$(iXXX, 1)         '3 LETTER VARIABLE FROM FISCAL DEFINITION LINE
      cdtitle$ = ""             'where we will put the long title
        'SEARCH TL$() for match
      For i = 1 To TLT
        If cd$ = TL$(i, 1) Then        'we have a match
          cdtitle$ = " - " + UCase$(TL$(i, 3))
          Exit For
        End If
      Next i
        'if there is no long title - use the variable code from fiscal definition
      If cdtitle$ = "" Then
        cdtitle$ = " - " + cd$
      End If

                'map capex lines into XMO()
      For i = 1 To my3tt
        XMO(i) = i
      Next i

      doit$ = "N"
      
      '<<<<<< 16 Aug 2001 JWD (C0389)
      For i = 1 To UBound(SEQ, 1)
        If SEQ(i, PPR) <> 0 Then doit$ = "Y"
      Next i
      '~~~~~~ was:
      'For i = 1 To 18
      '  If SEQ(i, PPR) <> 0 Then doit$ = "Y"
      'Next i
      '>>>>>> End (C0389)
      
      If doit$ = "N" Then
        GoTo 100
      End If

      bSORTED = False
      While Not bSORTED
        bSWAPPING = False
        iRow = 1
        While iRow < my3tt
          CapYr = my3(iRow, 3)
          CapYr1 = my3(iRow + 1, 3)
          If CapYr < Y1 Then
            CapYr = YR
          End If
          If CapYr1 < Y1 Then
            CapYr1 = YR
          End If
                '          IF my3(iRow, 3) = my3(iRow + 1, 3) THEN  *** old statement
          If CapYr = CapYr1 Then
            If SEQ(my3(XMO(iRow), 1), PPR) > SEQ(my3(XMO(iRow + 1), 1), PPR) Then
              rTmp = XMO(iRow + 1)
              XMO(iRow + 1) = XMO(iRow)
              XMO(iRow) = rTmp
'~~~~              SWAP XMO(iRow), XMO(iRow + 1)
              bSWAPPING = True
            End If
          End If
          iRow = iRow + 1
        Wend
        If Not bSWAPPING Then
          bSORTED = True
        End If
      Wend

100     ' DETERMINE IF FVAR$(iX) IS REFERENCED IN MISCELLANEOUS TAX PARAMETERS

105   ReDim FCRD(LG)
180   DeprStrt = 1
      year1 = 1
      DBSL = 1
190   GoSub 4000

      ' 20 Nov 2002 JWD (C0634)
      If bDPCR = False Then   ' False, this is a depreciation schedule
         CLEX$ = "N"          ' This controls imposition of ceiling
      End If                  ' Any ceiling defined should not be imposed
      ' End (C0634)           ' on depreciation schedules.
               
1000            'CALCULATE DEPRECIATION

1010  If DPT = 0 Then GoTo 1310

1020    ' LOOP THRU CAPEX
      If my3tt = 0 Then          'if there is no capex - goto 1310
         GoTo 1310
      End If

1040  For iXMOI = 1 To my3tt      'loop ends at 2979
1041     bTBL = True
1042     iXM = XMO(iXMOI)
1050             'FIND LINE THAT MATCHES IN DEPRECIATION INPUT
1060     iYM = 0

1070     iYM = iYM + 1
1080     If iYM > DPT Then GoTo 1110
1084     GoSub 200
1085     If bMatch = False Then GoTo 1070
1090     If (my3(iXM, 1) + 3) = dp(iYM, 1) Then GoTo 1320
1100     GoTo 1070

1110             ' CHECK IF CAPEX CATEGORY DEFINED IN EXP OR DEV
1120     iYM = 0

1130     iYM = iYM + 1
1140     If iYM > DPT Then GoTo 1230
1145     GoSub 200
1146     If bMatch = False Then GoTo 1130
1150     If my3(iXM, 1) <= 3 Or my3(iXM, 1) >= 15 Then GoTo 1230
1160     If my3(iXM, 1) >= 9 And my3(iXM, 1) <= 14 Then GoTo 1200
1170            'CAPEX IS EXPLORATION - SEE IF EXPLORATION IS DEFINED
1180     If dp(iYM, 1) = 2 Then
            GoTo 1320
         End If
         
1190     GoTo 1130

1200            'CAPEX IS DEVELOPMENT - SEE IF DEVELOPMENT IS DEFINED
1210     If dp(iYM, 1) = 3 Then
            GoTo 1320
         End If
1220     GoTo 1130

1230            'CHECK IF ALL DEFINED
1240     iYM = 0

1250     iYM = iYM + 1
1260     If iYM > DPT Then GoTo 1290
1265     GoSub 200
1266     If bMatch = False Then GoTo 1250
1270     If dp(iYM, 1) = 1 Then GoTo 1320
1280     GoTo 1250

1290            'NO MATCH WAS FOUND - THUS NO DEPRECIATION FOR THIS EXPENDITURE
1310     For iLM = 1 To LG
1312        DEPR(iLM) = 0
1314     Next iLM
1315     bse = 0
         iYM = 0
1316     GoTo 2850

1320            'YOU ARE HERE WITH A MATCH BETWEEN MY3 AND DP
                ' iXM=LINE# IN CAPEX INPUT
                ' iYM=MATCHING LINE IN DP
                ' iXYZ=MATCHING LINE TM
1330     GoSub 11050             ' this reads Misc. screen and sets DeprStrt, Year1 & DBSL



9999            'DEFINE BSE   (depreciable base)
         ReDim DEPR(gc_nMAXLIFE), dprrat(gc_nMAXLIFE)
         ' 30 Jun 2008 JWD Change variable containing depreciable base (was GPBASE())
         ' GPBASE() is also used to determine repaid carry, which may be different from
         ' what can be recovered/depreciated.
         bse = my3(iXM, 5) * GPDPCR(iXM) * (dp(iYM, 4) / 100)
'--------------------------------------------------------------------
'6-2-92 - treat reimbursement % > 100% as 100% to avoid adjusting
         'basis by excess reimbursement of carry
         'NOTE: reim() is NOT the reimburse % as entered on CPX screen
         '  (it is a function of standard WIN, CPX win entry, and
         '  the reimburse % as entered on CPX line)
         interest = REIM(iXM)
         If my3(iXM, 7) > 100 Then
            interest = interest / (my3(iXM, 7) / 100)
         End If

         bse = bse * (WINC(iXM) - interest)
                'BSE = BSE * (winc(iXM) - reim(iXM))
'--------------------------------------------------------------------
                ' CHECK FOR INTEREST ACCRUAL FROM DATE OF EXPENDITURE
         
         '<<<<<< 3 Jul 2001 JWD (C0345) Add method 3 (YS3)
         If dp(iYM, 11) = 2 Or dp(iYM, 11) = 4 Or dp(iYM, 11) = 6 Then
         '~~~~~~ was:
         'If dp(iYM, 11) = 2 Or dp(iYM, 11) = 4 Then
         '>>>>>> End (C0345)
            
            TIMEX = Y1 - my3(iXM, 3) + ((M1 - my3(iXM, 2)) / 12)
            If TIMEX > 0 Then
               bse = bse * (1 + (dp(iYM, 10) / 100)) ^ TIMEX
            End If
         End If
                'ADJUST FOR TAN/INT ONLY
         If bTBL = True Then
            bse = bse * (my3(iXM, 4) / 100)
         ElseIf bTBL = False Then
            bse = bse * (1 - (my3(iXM, 4) / 100))
         End If
                'DETERMINE START YEAR FOR DEPRECIATION
         If DeprStrt = 1 Then                      'PRD
            If my3(iXM, 3) <= Y1 Then                     '<= prod year
               iSTRT = Y1 - YR + 1
            Else                                         '> prod year
               iSTRT = my3(iXM, 3) - YR + 1
            End If
         ElseIf DeprStrt = 2 Then                  'EXP
            iSTRT = my3(iXM, 3) - YR + 1
         End If
         'Calculate investment tax credit
12110    If iSTRT > LG Then
            GoTo 2979             'end of main loop
         End If


         FCRD(iSTRT) = FCRD(iSTRT) + (bse * (dp(iYM, 9) / 100))
12111
         CRDT = bse * (dp(iYM, 9) / 100)
         ' CHECK FOR FIRST YEAR ALLOWANCE
12112    FirstYearAllow = 0
         If dp(iYM, 8) <> 0 Then           'CALCULATE FIRST YEAR ALLOWANCE
12113       FirstYearAllow = bse * (dp(iYM, 8) / 100)
            bse = bse - FirstYearAllow
         End If
12114           'compute iLIFE
         If dp(iYM, 7) = -995 Then       ' LIF has been entered
            iLIFE = LG
         Else
12115       iLIFE = Int(Abs(dp(iYM, 7)) + iSTRT - 1)
         End If

         ' methods 11, 12 & 13 take the lesser of the entered depreciable
         ' iLIFE and the remaining iLIFE from the date of the cap expd.

12116    If dp(iYM, 5) = 11 Or dp(iYM, 5) = 12 Or dp(iYM, 5) = 13 Then
            lif1 = LG - iSTRT + 1          'remaining project life
12117       lif2 = dp(iYM, 7)
            If lif2 = -995 Then
               lif2 = lif1
            End If
            If lif1 < lif2 Then
               iLIFE = iSTRT + lif1 - 1
            Else
               iLIFE = iSTRT + lif2 - 1
            End If
         End If

1720                   ' DETERMINE FRACTION IN FIRST YEAR
         rFrac = 1
         If year1 = 1 Then                'MTH
            If DeprStrt = 1 Then                  'PRD
               If my3(iXM, 3) < Y1 Then                   '< prod year
                  rFrac = (13 - M1) / 12
               ElseIf my3(iXM, 3) > Y1 Then               '> prod year
                  rFrac = (13 - my3(iXM, 2)) / 12
               ElseIf my3(iXM, 3) = Y1 Then               '= prod year
                  If my3(iXM, 2) < M1 Then                   '< prod month
                     rFrac = (13 - M1) / 12
                  Else                                      '>= prod month
                     rFrac = (13 - my3(iXM, 2)) / 12
                  End If
               End If
            Else                                  'EXP
               rFrac = (13 - my3(iXM, 2)) / 12
            End If
         ElseIf year1 = 2 Then            'FUL
            rFrac = 1
         ElseIf year1 = 3 Then            'HLF
            rFrac = 0.5
         End If

1890            ' NOW BRANCH ON METHODS
         If bse = 0 Then               ' no depreciable basis
            ReDim DEPR(gc_nMAXLIFE)
         ElseIf dp(iYM, 5) = 6 Then     ' NON-depreciable
            ReDim DEPR(gc_nMAXLIFE)
         ElseIf dp(iYM, 5) = 2 Or dp(iYM, 5) = 12 Then   'methods SLN & SL1
            iDEPRLIFE = iLIFE - iSTRT + 1
            For iLM = iSTRT To iLIFE
               If iDEPRLIFE > 0 Then                   ' to avoid divide by zero
                  If iLM = iSTRT And rFrac <> 1 Then         ' in first depr. year, if partial
                     DEPR(iLM) = (bse / iDEPRLIFE) * rFrac
                  Else
                     DEPR(iLM) = bse / iDEPRLIFE
                  End If
               Else
                  DEPR(iLM) = 0
               End If
            Next iLM
               'if fractional first year, add a year and use up remaining depreciation
            If rFrac <> 1 Then
               iLIFE = iLIFE + 1
               If iDEPRLIFE > 0 Then
                  DEPR(iLIFE) = (bse / iDEPRLIFE) * (1 - rFrac)
               Else
                  DEPR(iLIFE) = 0
               End If
            End If
         ElseIf dp(iYM, 5) = 4 Or dp(iYM, 5) = 7 Or dp(iYM, 5) = 8 Or dp(iYM, 5) = 9 Or dp(iYM, 5) = 13 Then
                'the following five methods - SYD, DP1, DP2, DP3 & SY1
                'all have the same treatment of fractional first and last years
            iDEPRLIFE = iLIFE - iSTRT + 1
                ' determine rates for each method first
            If dp(iYM, 5) = 4 Or dp(iYM, 5) = 13 Then   'SYD and SY1 - methods 4 & 13
               cdgt = 0
               For iLM = 1 To iDEPRLIFE
                  cdgt = cdgt + iLM
               Next iLM
               If cdgt > 0 Then
                  For iLM = 1 To iDEPRLIFE
                     dprrat(iLM) = (iDEPRLIFE - iLM + 1) / cdgt
                  Next iLM
               Else                 'IF CDGT <= 0 THEN
                  For iLM = 1 To iDEPRLIFE
                     dprrat(iLM) = 0
                  Next iLM
               End If
            ElseIf dp(iYM, 5) = 7 Then                   'USE DP1 SCHEDULE
               For iLM = 1 To iDEPRLIFE
                  dprrat(iLM) = B(iLM, 7) / 100
               Next iLM
            ElseIf dp(iYM, 5) = 8 Then                   'USE DP2 SCHEDULE
               For iLM = 1 To iDEPRLIFE
                  dprrat(iLM) = B(iLM, 8) / 100
               Next iLM
            ElseIf dp(iYM, 5) = 9 Then                   'USE DP3 SCHEDULE
               For iLM = 1 To iDEPRLIFE
                  dprrat(iLM) = B(iLM, 9) / 100
               Next iLM
            End If
            'dprrat() now filled in with annual rates relative to time zero
2270                     ' NOW CALCULATE DEPRECIATION
            ReDim DumDepr(iDEPRLIFE + 1)       'Depreciation relative to time zero
            For iLM = 1 To iDEPRLIFE
               DumDepr(iLM) = bse * dprrat(iLM)
            Next iLM

                'check for fractional first year
                'if needed, shift depreciation based on rFrac
            If rFrac <> 1 Then
2272           ReDim TempDepr(iDEPRLIFE + 1)
               For iLM = 1 To iDEPRLIFE
2273              TempDepr(iLM) = DumDepr(iLM)
               Next iLM
               For iLM = 1 To iDEPRLIFE
                  If iLM = 1 Then
2274                 DumDepr(1) = TempDepr(1) * rFrac
                  Else
2275                 DumDepr(iLM) = (TempDepr(iLM - 1) * (1 - rFrac)) + (TempDepr(iLM) * rFrac)
                  End If
               Next iLM

               'now add an additional depreciation year to capture last fractional year
               iDEPRLIFE = iDEPRLIFE + 1
2276           DumDepr(iDEPRLIFE) = TempDepr(iDEPRLIFE - 1) * (1 - rFrac)
               Erase TempDepr
            End If
                'now shift DumDepr() into proper time frame for Depr()
            For iLM = 1 To iDEPRLIFE
2277           DEPR(iSTRT + iLM - 1) = DumDepr(iLM)
            Next iLM
            iLIFE = iSTRT + iDEPRLIFE - 1
            Erase DumDepr
         ElseIf dp(iYM, 5) = 1 Or dp(iYM, 5) = 11 Then          'DBL & DB1 - declining balance
            iDEPRLIFE = iLIFE - iSTRT + 1
                ' NOW CALCULATE DEPRECIATION
            BEGBAL = bse
            CDPR = 0
2278        ReDim DumDepr(iDEPRLIFE)
2279        rRAT = (dp(iYM, 6) / 100) / iDEPRLIFE       'declining balance rate

            For iLM = 1 To iDEPRLIFE
               If iLM = 1 Then                  'use fraction in first year only
2280              DumDepr(iLM) = BEGBAL * rRAT * rFrac
               Else
2281              DumDepr(iLM) = BEGBAL * rRAT
               End If
2282           CDPR = CDPR + DumDepr(iLM)
2283           BEGBAL = BEGBAL - DumDepr(iLM)
2570        Next iLM
            DumDepr(iDEPRLIFE) = DumDepr(iDEPRLIFE) + bse - CDPR    'sum remaining depr into last year
            If DBSL = 1 Then            'if switch to straight line
               bSwitch = False
               For iLM = 2 To iDEPRLIFE
                  If bSwitch Then GoTo nextlm
                  CDepr = 0
                  For iLZ = 1 To iLM - 1        'sum DB depreciation up through the end of last year
                     CDepr = CDepr + DumDepr(iLZ)
                  Next iLZ
                  DeprBase = bse - CDepr
                  SLDepr = DeprBase / (iDEPRLIFE - iLM + 1)
                  If SLDepr >= DumDepr(iLM) Then      'switch to straight line
                     bSwitch = True
                     For iLZ = iLM To iDEPRLIFE
                        DumDepr(iLZ) = SLDepr
                     Next iLZ
                  End If
nextlm:        Next iLM
            End If
                'now shift DumDepr() into proper time frame for Depr()
            For iLM = 1 To iDEPRLIFE
               DEPR(iSTRT + iLM - 1) = DumDepr(iLM)
            Next iLM
            Erase DumDepr
2640     ElseIf dp(iYM, 5) = 3 Then                 'UOP
            BEGBAL = bse
            REMVOL = 0
            ProdStart = Y1 - YR + 1
            reserves = 0
            ReDim UOPVol(LG), ResAdd(LG), RemRes(LG)
            'Determine UOPVol()
            Select Case UOPStrm      'misc depr cost recover parameters column 7
            Case 1               'PRD
               For iLM = 1 To LG
                  ' GDP 20 Jan 2003
                  ' Replaced revenue calc with call to ATotalRevenues()
                  'UOPVol(iLM) = (A(iLM, 1) * A(iLM, 7)) + (A(iLM, 2) * A(iLM, 8)) + (A(iLM, 3) * A(iLM, 9)) + (A(iLM, 4) * A(iLM, 10))
                  UOPVol(iLM) = ATotalRevenues(iLM) '(A(iLM, 1) * A(iLM, 7)) + (A(iLM, 2) * A(iLM, 8)) + (A(iLM, 3) * A(iLM, 9)) + (A(iLM, 4) * A(iLM, 10))
                  reserves = reserves + UOPVol(iLM)
               Next iLM
            Case 2               'PRE
               Equiva = gn(2)
               If gn(2) <= 0 Then
                  Equiva = 6
               End If
               For iLM = 1 To LG
                  ' GDP 20 Jan 2003
                  ' Added new volumes into the calc for UOPVol.
                  UOPVol(iLM) = A(iLM, gc_nAOIL) + (A(iLM, gc_nAGAS) / Equiva) _
                              + A(iLM, gc_nAOV1) + (A(iLM, gc_nAOV2) / Equiva) _
                              + A(iLM, gc_nAOV3) + (A(iLM, gc_nAOV4) / Equiva) _
                              + A(iLM, gc_nAOV5) + (A(iLM, gc_nAOV6) / Equiva) _
                              + A(iLM, gc_nAOV7) + (A(iLM, gc_nAOV8) / Equiva) _
                              + A(iLM, gc_nAOV9) + (A(iLM, gc_nAOV0) / Equiva)
                  reserves = reserves + UOPVol(iLM)
               Next iLM
            Case 3
               For iLM = 1 To LG
                  ' GDP 20 Jan 2003
                  ' Changed so A array ref uses constant instead of 1
                  UOPVol(iLM) = A(iLM, gc_nAOIL)
                  reserves = reserves + UOPVol(iLM)
               Next iLM
            Case 4
               For iLM = 1 To LG
                  ' GDP 20 Jan 2003
                  ' Changed so A Array ref uses constant instead of 2
                  UOPVol(iLM) = A(iLM, gc_nAGAS)
                  reserves = reserves + UOPVol(iLM)
               Next iLM
            Case 5
               For iLM = 1 To LG
                  ' GDP 20 Jan 2003
                  ' Changed so A Array ref uses constant instead of 2
                  UOPVol(iLM) = A(iLM, gc_nAOV1)
                  reserves = reserves + UOPVol(iLM)
               Next iLM
            Case 6
               For iLM = 1 To LG
                  ' GDP 20 Jan 2003
                  ' Changed so A Array ref uses constant instead of 2
                  UOPVol(iLM) = A(iLM, gc_nAOV2)
                  reserves = reserves + UOPVol(iLM)
               Next iLM
            ' GDP 20 Jan 2003
            ' Added extra case statements for extra volumes
            Case 7
               For iLM = 1 To LG
                  UOPVol(iLM) = A(iLM, gc_nAOV3)
                  reserves = reserves + UOPVol(iLM)
               Next iLM
            Case 8
               For iLM = 1 To LG
                  UOPVol(iLM) = A(iLM, gc_nAOV4)
                  reserves = reserves + UOPVol(iLM)
               Next iLM
            Case 9
               For iLM = 1 To LG
                  UOPVol(iLM) = A(iLM, gc_nAOV5)
                  reserves = reserves + UOPVol(iLM)
               Next iLM
            Case 10
               For iLM = 1 To LG
                  UOPVol(iLM) = A(iLM, gc_nAOV6)
                  reserves = reserves + UOPVol(iLM)
               Next iLM
            Case 11
               For iLM = 1 To LG
                  UOPVol(iLM) = A(iLM, gc_nAOV7)
                  reserves = reserves + UOPVol(iLM)
               Next iLM
            Case 12
               For iLM = 1 To LG
                  UOPVol(iLM) = A(iLM, gc_nAOV8)
                  reserves = reserves + UOPVol(iLM)
               Next iLM
            Case 13
               For iLM = 1 To LG
                  UOPVol(iLM) = A(iLM, gc_nAOV9)
                  reserves = reserves + UOPVol(iLM)
               Next iLM
            Case 14
               For iLM = 1 To LG
                  UOPVol(iLM) = A(iLM, gc_nAOV0)
                  reserves = reserves + UOPVol(iLM)
               Next iLM
            Case Else '3/24/97  added to allow UOP depreciation to be based on any previously defined variable
               If UOPStrm > 100 Then
                  For iLM = 1 To LG
                     UOPVol(iLM) = RVN(iLM, UOPStrm - 100)
                     reserves = reserves + UOPVol(iLM)
                  Next iLM
               End If

            End Select

            ' Now determine Reserve Additions
            TempSum = 0
            For iLM = 1 To LG
               ' GDP 20 Jan 2003
               ' Changed so A array ref uses constant
               TempSum = TempSum + A(iLM, gc_nARES)
            Next iLM
            If TempSum = 0 Then          ' No Reserve Additions entered in Base Data
               ResAdd(ProdStart) = reserves
               For iLM = (ProdStart + 1) To LG
                  ResAdd(iLM) = 0
               Next iLM
            Else                         ' Reserve Additions(%) entered in Base Data
               DumRes = 0
               For iLM = 1 To ProdStart   ' Accumulate Reserve Additions prior to production start
                  ' GDP 20 Jan 2003
                  ' Changed so A array ref uses constant
                  DumRes = DumRes + (reserves * (A(iLM, gc_nARES) / 100))
               Next iLM
               ResAdd(ProdStart) = DumRes
               For iLM = (ProdStart + 1) To LG           ' Now put in any reserve additions after production start year
                  ' GDP 20 Jan 2003
                  ' Changed so A array ref uses constant
                  ResAdd(iLM) = reserves * (A(iLM, gc_nARES) / 100)
               Next iLM
            End If

            ' Calculate Remaining Reserves
            RemRes(ProdStart) = ResAdd(ProdStart)

            For iLM = (ProdStart + 1) To LG
               RemRes(iLM) = RemRes(iLM - 1) - UOPVol(iLM - 1) + ResAdd(iLM)
            Next iLM

            ' Calculate UOP Depreciation Rate
            For iLM = ProdStart To LG
               If RemRes(iLM) > 0 Then
                  dprrat(iLM) = UOPVol(iLM) / RemRes(iLM)
               End If
               If RemRes(iLM) <= 0 Then
                  dprrat(iLM) = 0
               End If
            Next iLM

            ReDim UOPVol(LG), ResAdd(LG), RemRes(LG)
                '''       ERASE UOPVol, ResAdd, RemRes

            For iLM = iSTRT To LG
               DEPR(iLM) = BEGBAL * dprrat(iLM)
               CDPR = CDPR + DEPR(iLM)
               BEGBAL = BEGBAL - DEPR(iLM)
            Next iLM
         ElseIf dp(iYM, 5) = 5 Then         'EXPENSE THIS EXPENDITURE
            DEPR(iSTRT) = bse
         ElseIf dp(iYM, 5) = 10 Then        'CUMULATIVE CAPITAL
            For iTB = iSTRT To LG
               DEPR(iTB) = DEPR(iTB) + bse
            Next iTB
         End If
                'if there is a 1st year allowance, it is now put in DEPR(iSTRT)
         If FirstYearAllow > 0 Then
            DEPR(iSTRT) = DEPR(iSTRT) + FirstYearAllow
         End If

         'lump remaining depreciation in last year of project
         If iLIFE > LG Then
            For iLM = LG + 1 To iLIFE
               DEPR(LG) = DEPR(LG) + DEPR(iLM)
            Next iLM
         End If
         ' Is this cost recovery?
         If bDPCR Then
            ' Is interest rate non-zero?
            If dp(iYM, 10) <> 0 Then
               ' Is interest added to unrecovered, scheduled amounts?
               If (dp(iYM, 11) = 3 Or dp(iYM, 11) = 4) Then
                  ' Determine unrecovered balance
                  ' Start with next period after start. Assumes that
                  ' interest is not earned on "current" year recovery,
                  ' only on future, unrecovered amounts.
                  For iXLM = iSTRT + 1 To LG
                     unrecovered = unrecovered + DEPR(iXLM)
                  Next iXLM
                  
                  ' Compute and add interest on as yet unrecovered
                  ' amounts to scheduled recovery amount for period.
                  For iXLM = iSTRT + 1 To LG
                     interest_on_unrecovered = unrecovered * dp(iYM, 10) / 100
                     unrecovered = unrecovered - DEPR(iXLM)
                     DEPR(iXLM) = DEPR(iXLM) + interest_on_unrecovered
                  Next iXLM
               
               '<<<<<< 3 Jul 2001 JWD (C0345)
               ElseIf (dp(iYM, 11) = 5 Or dp(iYM, 11) = 6) Then
                  ' Compute the interest on a monthly basis and add to cost recovery
                  Dim months_in_first_year As Integer
                  
                  If rFrac <> 1 Then
                     ' First year is less than 12 months or
                     ' otherwise gets special treatment
                     months_in_first_year = CInt(12 * rFrac)
                  Else
                     months_in_first_year = 12
                  End If
                  
                  zzz_AddMonthlyInterestToCostRecovery DEPR(), dp(iYM, 10), iSTRT, months_in_first_year
               '>>>>>> End (C0345)
               End If
            End If
         End If
         
2850            'ADD CREDIT TO DEPRECIATION IF THIS IS COST RECOVERY

2852     If bDPCR Then
            DEPR(iSTRT) = DEPR(iSTRT) + CRDT
         End If
2854          ' APPLY REVALUATION TERMS TO DEPRECIATION / COST RECOVERY
         If GM(1, PPR) <> 0 Then
            If CURT < 2 Then        'we only do revaluation for 1st "CUR" line
               REVAL = 0
               iBASERV = my3(iXM, 3) - YR + 1
               For iLM = 1 To LG
                  If CUR(iBASERV) <> 0 Then
                     REVAL = ((CUR(iLM) / CUR(iBASERV)) - 1) * ((GM(1, PPR) / 100) * DEPR(iLM))
                  Else
                     REVAL = 0
                  End If
                  DEPR(iLM) = DEPR(iLM) + REVAL
               Next iLM
            End If
         End If

2858     Rem DEPRECIATION IS NOW CALCULATED - SUM TO CATEGORIES
         TEM% = my3(iXM, 3) - YR + 1


         If my3(iXM, 1) > 0 And my3(iXM, 1) <= 3 Then
            TEM1% = 1
         ElseIf my3(iXM, 1) >= 4 And my3(iXM, 1) <= 8 Then
            TEM1% = 2
         ElseIf my3(iXM, 1) >= 9 And my3(iXM, 1) <= 14 Then
            TEM1% = 3
         ElseIf my3(iXM, 1) >= 15 Then
            TEM1% = 4
         End If
         If TEM% <= 0 Then GoTo 2975
                'SUM TO EXPENDITURES

2930     ACT(TEM%, TEM1%) = ACT(TEM%, TEM1%) + bse + FirstYearAllow
         If dp(iYM, 8) <> 0 Then
            ACT(TEM%, TEM1%) = ACT(TEM%, TEM1%) + DEPR(iSTRT - 1)
         End If

2935            'APPLY CEILING
2936     If CLEX$ <> "N" Then GoSub 4840
2940            'SUM TO DEPRECIATION
2950     For iLM = 1 To LG
2960        ACT(iLM, TEM1% + 5) = ACT(iLM, TEM1% + 5) + DEPR(iLM)
2970     Next iLM
2975     If bTBL = False Then GoTo 2979
2978     bTBL = False
         GoTo 1042
2979            'you can get here from depreciation section AND if the capital
                '  was spent AFTER the end of the project! (from line 12110)
         
         '<<<<<< 25 Aug 1998 JWD Add next to skip Next stmt
         If my3tt = 0 Then
            GoTo 2980
         End If
         '>>>>>> End 25 Aug 1998
      
      Next iXMOI          'started loop at 1040
'---------------------------------------------------------
2980            'SUM THE EXPENDITURES TOGETHER AND THE DEPRECIATION TOGETHER
2990  For iLM = 1 To LG
3000     For iLL = 1 To 4
3010        ACT(iLM, 5) = ACT(iLM, 5) + ACT(iLM, iLL)
3020        ACT(iLM, 10) = ACT(iLM, 10) + ACT(iLM, iLL + 5)
3030     Next iLL
3032     ACT(iLM, 5) = ACT(iLM, 5) + ACT(iLM, 11)
3035     ACT(iLM, 10) = ACT(iLM, 10) + ACT(iLM, 12)
3040  Next iLM
3050            Rem SUM THE YEARS ACROSS ALL COLUMNS
3060  For iLL = 1 To 12
3070     For iLM = 1 To LG
3080        ACT(0, iLL) = ACT(0, iLL) + ACT(iLM, iLL)
3090     Next iLM
3100  Next iLL
3200            Rem PUT TOTAL IN DPC()
3210  For iXM = 1 To LG
3220     DPC(iXM) = ACT(iXM, 10)
3230  Next iXM
3240  GoTo 5000
5000            Rem PRINT DEPRECIATION SCHEDULE
5005  If Left$(RF$(5), 3) = "ALL" Then GoTo 5007
5006  GoTo 6999
5007
        'if "NOP" entered in last column of Fiscal Definition, do not print the report
      If TD$(iXXX, 18) = "NOP" Or TD$(iXXX, 18) = "VOP" Then GoTo 6999
        'if we are iterating, only print schedule on last iteration year
      If XIter = 1 And XYear < LG Then
         GoTo 6999
      ElseIf XIter = 1 And XYear = LG Then
         GoTo 5009        'print the schedule
      Else
         If XFirst > 0 And XLast > 0 Then
            If iXXX >= XFirst And iXXX <= XLast Then
               If XYear < LG Then
                  GoTo 6999
               End If
            End If
         End If
      End If

5009            ' PRINT DEPRECIATION SCHEDULE
5010
5120  If bDPCR Then GoTo 5530
5174  ColumnNm$(1) = "BASEREN"
5176  ColumnNm$(2) = "BASEEXP"
5177  ColumnNm$(3) = "BASEDEV"
5178  ColumnNm$(4) = "BASEOTH"
5180  ColumnNm$(5) = "BASETOT"
5182  ColumnNm$(6) = "  OPREN"
5184  ColumnNm$(7) = "  OPEXP"
5186  ColumnNm$(8) = "  OPDEV"
5188  ColumnNm$(9) = "  OPOTH"
5290  ColumnNm$(10) = "  OPTOT"
5298
5300                   'Page type, Year start, Page counter, life of field, number of columns, page title, column length
5302  ''''Write #5, 8, YR, 0, LG, 10, "DEPRECIATION SCHEDULE" + cdtitle$, 10, FinalWin, FINALPARTIC, sCur

5304                   'columns titles
5306  ''''Write #5, ColumnNm$(1), ColumnNm$(2), ColumnNm$(3), ColumnNm$(4), ColumnNm$(5), ColumnNm$(6), ColumnNm$(7), ColumnNm$(8), ColumnNm$(9), ColumnNm$(10)
      
      Dim oPg1 As IGiantRptPageAssignSub
      Set oPg1 = g_oReport.NewSubScheduleRptPage(8)
      oPg1.SetPageHeader 8, YR, 0, LG, 10, "DEPRECIATION SCHEDULE" + cdtitle$, 10, FinalWin, FINALPARTIC, sCur
      oPg1.SetProfileHeaders ColumnNm$(1), ColumnNm$(2), ColumnNm$(3), ColumnNm$(4), ColumnNm$(5), ColumnNm$(6), ColumnNm$(7), ColumnNm$(8), ColumnNm$(9), ColumnNm$(10)
      oPg1.SetRptPageVariableCode cd$
5312
5314  For i = 1 To LG
5330     ''''Write #5, ACT(i, 1), ACT(i, 2), ACT(i, 3), ACT(i, 4), ACT(i, 5), ACT(i, 6), ACT(i, 7), ACT(i, 8), ACT(i, 9), ACT(i, 10)
         oPg1.SetProfileValues i, ACT(i, 1), ACT(i, 2), ACT(i, 3), ACT(i, 4), ACT(i, 5), ACT(i, 6), ACT(i, 7), ACT(i, 8), ACT(i, 9), ACT(i, 10)
5360  Next i
5440
5500  GoTo 6999
5502
'-------------------------------------------------------------
'THIS SECTION ALTERED TO FACILITATE ADDING AN ANNUAL CEILING COLUMN TO RECOVERY REPORT
'  We added a column to the recovery page that shows the annual cost recovery ceiling.
'    (column 13)


5504    ' PRINT COST RECOVERY SCHEDULE
5506
5530
5532  ColumnNm$(1) = "BASREN"
5534  ColumnNm$(2) = "BASEXP"
5536  ColumnNm$(3) = "BASDEV"
5538  ColumnNm$(4) = "BASOTH"
5540  ColumnNm$(5) = "BASOPX"
5542  ColumnNm$(6) = "BASTOT"
5544  ColumnNm$(7) = " CRREN"
5546  ColumnNm$(8) = " CREXP"
5548  ColumnNm$(9) = " CRDEV"
5550  ColumnNm$(10) = " CROTH"
5552  ColumnNm$(11) = " CROPX"
5554  ColumnNm$(12) = " CRTOT"
      ColumnNm$(13) = "  CLNG"
5556
5558                   'Page type, Start year, Page counter, life of field, number of columns, page title, column length
      'add a column and shring col didth to 8 to accommodate ceiling value
5560  ''''Write #5, 7, YR, 0, LG, 13, "COST RECOVERY SCHEDULE" + cdtitle$, 8, FinalWin, FINALPARTIC, sCur

5562                   'columns titles
5566  ''''Write #5, ColumnNm$(1), ColumnNm$(2), ColumnNm$(3), ColumnNm$(4), ColumnNm$(5), ColumnNm$(6), ColumnNm$(7), ColumnNm$(8), ColumnNm$(9), ColumnNm$(10), ColumnNm$(11), ColumnNm$(12), ColumnNm$(13)
      
      Set oPg1 = g_oReport.NewSubScheduleRptPage(7)
      oPg1.SetPageHeader 7, YR, 0, LG, 13, "COST RECOVERY SCHEDULE" + cdtitle$, 8, FinalWin, FINALPARTIC, sCur
        oPg1.SetProfileHeaders ColumnNm$(1), ColumnNm$(2), ColumnNm$(3), ColumnNm$(4), ColumnNm$(5), ColumnNm$(6), _
            ColumnNm$(7), ColumnNm$(8), ColumnNm$(9), ColumnNm$(10), ColumnNm$(11), ColumnNm$(12), ColumnNm$(13)
            
      oPg1.SetRptPageVariableCode cd$
5571
5700  For i = 1 To LG
5730     ''''Write #5, ACT(i, 1), ACT(i, 2), ACT(i, 3), ACT(i, 4), ACT(i, 11), ACT(i, 5), ACT(i, 6), ACT(i, 7), ACT(i, 8), ACT(i, 9), ACT(i, 12), ACT(i, 10), (clngs(i) * CLRA(i) / 100)
            oPg1.SetProfileValues i, ACT(i, 1), ACT(i, 2), ACT(i, 3), ACT(i, 4), ACT(i, 11), ACT(i, 5), ACT(i, 6), _
                ACT(i, 7), ACT(i, 8), ACT(i, 9), ACT(i, 12), ACT(i, 10), (clngs(i) * CLRA(i) / 100)
            
5760  Next i
5762

'ORIGINAL CODE HERE
'5504    ' PRINT COST RECOVERY SCHEDULE
'5506
'5530
'5532 ColumnNm$(1) = "BASEREN"
'5534 ColumnNm$(2) = "BASEEXP"
'5536 ColumnNm$(3) = "BASEDEV"
'5538 ColumnNm$(4) = "BASEOTH"
'5540 ColumnNm$(5) = "BASEOPX"
'5542 ColumnNm$(6) = "BASETOT"
'5544 ColumnNm$(7) = "  CRREN"
'5546 ColumnNm$(8) = "  CREXP"
'5548 ColumnNm$(9) = "  CRDEV"
'5550 ColumnNm$(10) = "  CROTH"
'5552 ColumnNm$(11) = "  CROPX"
'5554 ColumnNm$(12) = "  CRTOT"
'5556
'5558                   'Page type, Start year, Page counter, life of field, number of columns, page title, column length
'5560 WRITE #5, 7, yr, 0, LG, 12, "COST RECOVERY SCHEDULE", 10, WINT, PRTA, sCur
'5562                   'columns titles
'5566 WRITE #5, ColumnNm$(1), ColumnNm$(2), ColumnNm$(3), ColumnNm$(4), ColumnNm$(5), ColumnNm$(6), ColumnNm$(7), ColumnNm$(8), ColumnNm$(9), ColumnNm$(10), ColumnNm$(11), ColumnNm$(12)
'5571
'5700 FOR i = 1 TO LG
'5730   WRITE #5, ACT(i, 1), ACT(i, 2), ACT(i, 3), ACT(i, 4), ACT(i, 11), ACT(i, 5), ACT(i, 6), ACT(i, 7), ACT(i, 8), ACT(i, 9), ACT(i, 12), ACT(i, 10)
'5760 NEXT i
'5762
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

5999 GoTo 6999
6000
6999

Exit Sub

'====================================================================
'GOSUBS START HERE
'--------------------------------------------------------------------
                'GOSUB to here from #1084, 1145, and 1265
200             'COMPARE FVAR$(iX) AND PRE/POST PRODUCTION SPECIFICATIONS
210   bMatch = False
220             'THIS LINE FOR THIS TAX

230   If FVAR$(iX) <> sDP(iYM) Then
         GoTo 420
      End If
240             'CHECK PRE/POST PRODUCTION
250   If dp(iYM, 2) = 1 Then
         GoTo 360
      ElseIf dp(iYM, 2) = 3 Then
         GoTo 320
      End If
270             'PRE-PRODUCTION ONLY
280   If my3(iXM, 3) > Y1 Then
         GoTo 420
      ElseIf my3(iXM, 3) < Y1 Then
         GoTo 360
      ElseIf my3(iXM, 2) >= M1 Then
         GoTo 420
      Else
         GoTo 360
      End If
320             'POST PRODUCTION ONLY
330   If my3(iXM, 3) < Y1 Then GoTo 420
340   If my3(iXM, 3) > Y1 Then GoTo 360
350   If my3(iXM, 2) < M1 Then GoTo 420
360
370   If dp(iYM, 3) = 1 Then GoTo 410
380   If dp(iYM, 3) = 2 And bTBL = True Then GoTo 410
390   If dp(iYM, 3) = 3 And bTBL = False Then GoTo 410
400   GoTo 420
410   bMatch = True
420   Return
'--------------------------------------------------------------------
4000    'THIS SUBROUTINE APPLIES CEILING TO COST RECOVERY/DEPRECIATION
4010    'FIND CEILING TO MATCH VARIABLE
4020  ReDim clngs(LG), clngx(LG), CLRA(LG)
4030  iYP = 0
      CLEX$ = "Y"
      Fd% = 0
4040  iYP = iYP + 1
4048  If iYP > CLGTT Then CLEX$ = "N"
4050  If iYP > CLGTT Then GoTo 4472
4060  If FVAR$(iX) = CLG$(iYP, 1) Then
         Fd% = 1
         GoTo 4080
      End If
4070  GoTo 4040
4080  ' LOOP THRU INCOME
      ReDim matcher$(10), clngs(LG)
      For iPY = 1 To 10
         matcher$(iPY) = CLG$(iYP, iPY)
      Next iPY

      CeilDef "DEPREC", iX, matcher$(), clngs()
4472               'when the user did not specify a ceiling def then GRP
      If Fd% <> 1 Then   'fd is basically found%  1=true   0=not found
         For iML = 1 To LG
                        '***s/b total rev not prim rev
            ' GDP 20 Jan 2003
            ' A(iML, PPR + 6) referenced the primary stream price
            ' changed this to use a constant instead of 6
            ' clngs(iML) = clngs(iML) + (A(iML, PPR) * A(iML, PPR + 6)) * (1 - PARTRATE(iML)) * WIN(iML)
            clngs(iML) = clngs(iML) + (A(iML, PPR) * A(iML, PPR + gc_nAPRICEOFFSET)) * (1 - PARTRATE(iML)) * WIN(iML)
         Next iML
      End If

4480            Rem NOW DETERMINE CEILING RATES IN CGR()
      If CGRT > 0 Then GoTo contin
      Fd% = 0
      GoTo default
contin:
      Fd% = 1
      Ratetot = CGRT

      ReDim sRateInV(Ratetot) As String
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
      searcher$ = FVAR$(iX)
      RateCalc Numvar, "DEPREC", searcher$, DefAmount, sRateInV(), ratein(), Ratetot, param%, VarRates()

      For j = 1 To LG
         CLRA(j) = VarRates(j)
      Next j
      GoTo 4800

default:
                'when the user did not specify the ceiling rates then
      If Fd% <> 1 Then
         For iML = 1 To LG
            CLRA(iML) = 100
         Next iML
      End If

4800            Rem NOW COMPUTE CEILING
4802  For iML = 1 To LG
4804     clngx(iML) = clngs(iML) * (CLRA(iML) / 100)
4806  Next iML

4808  If Not bDPCR Then GoTo 4839 'False = deprec (not cost recovery)

        'THIS IS THE COST RECOVERY SECTION
        '8-7-92  If user has put the code for an OPEX item in a
        '  deductions column, we use the values of OPEX() - the annual
        '  value of the item entered as a deduction.  This result goes in
        '  RPBASE in this section. (The item is excluded from recovery)
        'The Fiscal Definition deduction columns are cols 8-12
      ReDim opexded(LG)
      For q% = 1 To 5         '5 deduction columns
         sDum = TD$(iX, q% + 7)
         Select Case sDum
         Case "OX1"
            For w% = 1 To LG
               ' 29 May 2003 JWD (C0702) Replace number with symbol
               opexded(w%) = opexded(w%) + A(w%, gc_nAOX1)     ' was: A(w%, 11)
            Next w%
         Case "OX2"
            For w% = 1 To LG
               ' 29 May 2003 JWD (C0702) Replace number with symbol
               opexded(w%) = opexded(w%) + A(w%, gc_nAOX2)     ' was: A(w%, 12)
            Next w%
         Case "OX3"
            For w% = 1 To LG
               ' 29 May 2003 JWD (C0702) Replace number with symbol
               opexded(w%) = opexded(w%) + A(w%, gc_nAOX3)     ' was: A(w%, 13)
            Next w%
         Case "OX4"
            For w% = 1 To LG
               ' 29 May 2003 JWD (C0702) Replace number with symbol
               opexded(w%) = opexded(w%) + A(w%, gc_nAOX4)     ' was: A(w%, 14)
            Next w%
         Case "OX5"
            For w% = 1 To LG
               ' 29 May 2003 JWD (C0702) Replace number with symbol
               opexded(w%) = opexded(w%) + A(w%, gc_nAOX5)     ' was: A(w%, 15)
            Next w%
         ' 13 May 2005 JWD (C0877) Add cases for OX6-O20
         Case "OX6"
            For w% = 1 To LG
               opexded(w%) = opexded(w%) + A(w%, gc_nAOX6)
            Next w%
         Case "OX7"
            For w% = 1 To LG
               opexded(w%) = opexded(w%) + A(w%, gc_nAOX7)
            Next w%
         Case "OX8"
            For w% = 1 To LG
               opexded(w%) = opexded(w%) + A(w%, gc_nAOX8)
            Next w%
         Case "OX9"
            For w% = 1 To LG
               opexded(w%) = opexded(w%) + A(w%, gc_nAOX9)
            Next w%
         Case "OX0"
            For w% = 1 To LG
               opexded(w%) = opexded(w%) + A(w%, gc_nAOX0)
            Next w%
         Case "O11"
            For w% = 1 To LG
               opexded(w%) = opexded(w%) + A(w%, gc_nAO11)
            Next w%
         Case "O12"
            For w% = 1 To LG
               opexded(w%) = opexded(w%) + A(w%, gc_nAO12)
            Next w%
         Case "O13"
            For w% = 1 To LG
               opexded(w%) = opexded(w%) + A(w%, gc_nAO13)
            Next w%
         Case "O14"
            For w% = 1 To LG
               opexded(w%) = opexded(w%) + A(w%, gc_nAO14)
            Next w%
         Case "O15"
            For w% = 1 To LG
               opexded(w%) = opexded(w%) + A(w%, gc_nAO15)
            Next w%
         Case "O16"
            For w% = 1 To LG
               opexded(w%) = opexded(w%) + A(w%, gc_nAO16)
            Next w%
         Case "O17"
            For w% = 1 To LG
               opexded(w%) = opexded(w%) + A(w%, gc_nAO17)
            Next w%
         Case "O18"
            For w% = 1 To LG
               opexded(w%) = opexded(w%) + A(w%, gc_nAO18)
            Next w%
         Case "O19"
            For w% = 1 To LG
               opexded(w%) = opexded(w%) + A(w%, gc_nAO19)
            Next w%
         Case "O20"
            For w% = 1 To LG
               opexded(w%) = opexded(w%) + A(w%, gc_nAO20)
            Next w%
         ' End (C0877)
         Case "OPX"
            For w% = 1 To LG
               opexded(w%) = opexded(w%) + OPEX(w%)
            Next w%
         Case Else           'user defined variable
            'search previous lines of FDEF to find the row,
            '  then get the values from RVN()
            found% = 0          ' flag that we found a matching line in TD$()
            For w% = 1 To iXXX - 1
               If TD$(w%, 1) = sDum Then  'the FDEF line matches the deduct variable
                  For e% = 1 To LG
                     opexded(e%) = opexded(e%) + RVN(e%, w%)
                  Next e%
                  found% = -1
                  Exit For
               End If
               If found% = -1 Then
                  Exit For
               End If
            Next w%
         End Select
      Next q%

'''''IF TD$(iX, i + 4) <> "DPR"

4810  For iZZ = 1 To LG
        'OPEX() = annual costs   OPEXRATE() = govt rate share of OPEX
         netopex = OPEX(iZZ) - opexded(iZZ)
                '''8-7-92   4812   RPBASE = OPEX(iZZ) * (1 - OPEXRATE(iZZ))
4812     RPBASE = netopex * (1 - OPEXRATE(iZZ))
4815     RPBASE = RPBASE * WIN(iZZ)
4818     ACT(iZZ, 11) = RPBASE     'ACT() has grp OPEX only as this point
         ReDim REPA(LG)
             'COMPARE REPAYMENT TO CEILING
         iBYr = iZZ - 1
         While RPBASE
            iBYr = iBYr + 1
            If RPBASE <= clngx(iBYr) Then
               REPA(iBYr) = RPBASE
               clngx(iBYr) = clngx(iBYr) - RPBASE
               RPBASE = 0
            Else
               REPA(iBYr) = clngx(iBYr)
               RPBASE = RPBASE - clngx(iBYr)
               clngx(iBYr) = 0
               If iBYr = LG Then RPBASE = 0
            End If
         Wend
         For iZDB = iZZ To iBYr
            ACT(iZDB, 12) = ACT(iZDB, 12) + REPA(iZDB)
         Next iZDB
4830  Next iZZ
      Erase opexded

4839  Return

'--------------------------------------------------------------------
4840 ' NOW APPLY CEILING TO DEPRECIATION
         ' BASIC DEPR() IS COMING IN, DEPR() ADJUSTED FOR CEILING IS GOING OUT

      ReDim rDum(LG)
      For iZZ = 1 To LG
         RPBASE = DEPR(iZZ)
         ReDim REPA(LG)
                   'COMPARE REPAYMENT TO CEILING
         iBYr = iZZ - 1
         While RPBASE
            iBYr = iBYr + 1
            If RPBASE <= clngx(iBYr) Then
               REPA(iBYr) = RPBASE
               clngx(iBYr) = clngx(iBYr) - RPBASE
               RPBASE = 0
            Else
               REPA(iBYr) = clngx(iBYr)
               RPBASE = (RPBASE - clngx(iBYr)) * (1 + (dp(iYM, 10) / 100))
               clngx(iBYr) = 0
               If iBYr = LG Then RPBASE = 0
            End If
         Wend
         For iZDB = iZZ To iBYr
            rDum(iZDB) = rDum(iZDB) + REPA(iZDB)
         Next iZDB
      Next iZZ
      For iZZ = 1 To LG
         DEPR(iZZ) = rDum(iZZ)
      Next iZZ
4999  Return
'--------------------------------------------------------------------
11050 'This subroutine finds matching line in Miscellaneous
      '  Depreciation screen
      ' and sets DeprStrt, Year1, DBSL & UOPStrm
      loopit = 0
      bMatch = False
      If MDCT = 0 Then GoTo finisher

looper:
      loopit = loopit + 1
      If loopit > 3 Then GoTo finisher
      iXYZ = 0

loopxyz:
      iXYZ = iXYZ + 1
      If iXYZ > MDCT Then GoTo looper
      If FVAR$(iX) = sMDC(iXYZ) Then     'matching variable name
         If loopit = 1 Then            'first loop looks for exact match on category
            If (my3(iXM, 1) + 3) = MDC(iXYZ, 1) Then      'matching category code
               If MDC(iXYZ, 2) = 1 Then                  'Pre/Post = ALL
                  If MDC(iXYZ, 3) = 1 Then                'Tan/Int = ALL
                     bMatch = True
                  ElseIf MDC(iXYZ, 3) = 2 And bTBL = True Then      'Tan/Int = Tan
                     bMatch = True
                  ElseIf MDC(iXYZ, 3) = 3 And bTBL = False Then      'Tan/Int = Int
                     bMatch = True
                  End If
               ElseIf MDC(iXYZ, 2) = 2 Then                        'Pre/Post = Pre
                  If my3(iXM, 3) < Y1 Or (my3(iXM, 3) = Y1 And my3(iXM, 2) < M1) Then
                     If MDC(iXYZ, 3) = 1 Then                'Tan/Int = ALL
                        bMatch = True
                     ElseIf MDC(iXYZ, 3) = 2 And bTBL = True Then      'Tan/Int = Tan
                        bMatch = True
                     ElseIf MDC(iXYZ, 3) = 3 And bTBL = False Then      'Tan/Int = Int
                        bMatch = True
                     End If
                  End If
               ElseIf MDC(iXYZ, 3) = 3 Then                       'Pre/Post = Pst
                  If my3(iXM, 3) > Y1 Or (my3(iXM, 3) = Y1 And my3(iXM, 2) > M1) Then
                     If MDC(iXYZ, 3) = 1 Then                'Tan/Int = ALL
                        bMatch = True
                     ElseIf MDC(iXYZ, 3) = 2 And bTBL = True Then      'Tan/Int = Tan
                        bMatch = True
                     ElseIf MDC(iXYZ, 3) = 3 And bTBL = False Then      'Tan/Int = Int
                        bMatch = True
                     End If
                  End If
               End If
            End If
         ElseIf loopit = 2 Then                 'second loop looks at EXP & DEV
            If (my3(iXM, 1) >= 1 And my3(iXM, 1) <= 8 And MDC(iXYZ, 1) = 2) Or (my3(iXM, 1) >= 9 And my3(iXM, 1) <= 14 And MDC(iXYZ, 1) = 3) Then 'matching category code
               If MDC(iXYZ, 2) = 1 Then                  'Pre/Post = ALL
                  If MDC(iXYZ, 3) = 1 Then                'Tan/Int = ALL
                     bMatch = True
                  ElseIf MDC(iXYZ, 3) = 2 And bTBL = True Then      'Tan/Int = Tan
                     bMatch = True
                  ElseIf MDC(iXYZ, 3) = 3 And bTBL = False Then      'Tan/Int = Int
                     bMatch = True
                  End If
               ElseIf MDC(iXYZ, 2) = 2 Then                        'Pre/Post = Pre
                  If my3(iXM, 3) < Y1 Or (my3(iXM, 3) = Y1 And my3(iXM, 2) < M1) Then
                     If MDC(iXYZ, 3) = 1 Then                'Tan/Int = ALL
                        bMatch = True
                     ElseIf MDC(iXYZ, 3) = 2 And bTBL = True Then      'Tan/Int = Tan
                        bMatch = True
                     ElseIf MDC(iXYZ, 3) = 3 And bTBL = False Then      'Tan/Int = Int
                        bMatch = True
                     End If
                  End If
               ElseIf MDC(iXYZ, 3) = 3 Then                       'Pre/Post = Pst
                  If my3(iXM, 3) > Y1 Or (my3(iXM, 3) = Y1 And my3(iXM, 2) > M1) Then
                     If MDC(iXYZ, 3) = 1 Then                'Tan/Int = ALL
                        bMatch = True
                     ElseIf MDC(iXYZ, 3) = 2 And bTBL = True Then      'Tan/Int = Tan
                        bMatch = True
                     ElseIf MDC(iXYZ, 3) = 3 And bTBL = False Then      'Tan/Int = Int
                        bMatch = True
                     End If
                  End If
               End If
            End If
         ElseIf loopit = 3 Then                 'last loop looks for ALL
            If MDC(iXYZ, 1) = 1 Then             'matching category code
               If MDC(iXYZ, 2) = 1 Then                  'Pre/Post = ALL
                  If MDC(iXYZ, 3) = 1 Then                'Tan/Int = ALL
                     bMatch = True
                  ElseIf MDC(iXYZ, 3) = 2 And bTBL = True Then      'Tan/Int = Tan
                     bMatch = True
                  ElseIf MDC(iXYZ, 3) = 3 And bTBL = False Then      'Tan/Int = Int
                     bMatch = True
                  End If
               ElseIf MDC(iXYZ, 2) = 2 Then                        'Pre/Post = Pre
                  If my3(iXM, 3) < Y1 Or (my3(iXM, 3) = Y1 And my3(iXM, 2) < M1) Then
                     If MDC(iXYZ, 3) = 1 Then                'Tan/Int = ALL
                        bMatch = True
                     ElseIf MDC(iXYZ, 3) = 2 And bTBL = True Then      'Tan/Int = Tan
                        bMatch = True
                     ElseIf MDC(iXYZ, 3) = 3 And bTBL = False Then      'Tan/Int = Int
                        bMatch = True
                     End If
                  End If
               ElseIf MDC(iXYZ, 3) = 3 Then                       'Pre/Post = Pst
                  If my3(iXM, 3) > Y1 Or (my3(iXM, 3) = Y1 And my3(iXM, 2) > M1) Then
                     If MDC(iXYZ, 3) = 1 Then                'Tan/Int = ALL
                        bMatch = True
                     ElseIf MDC(iXYZ, 3) = 2 And bTBL = True Then      'Tan/Int = Tan
                        bMatch = True
                     ElseIf MDC(iXYZ, 3) = 3 And bTBL = False Then      'Tan/Int = Int
                        bMatch = True
                     End If
                  End If
               End If
            End If
         End If
      End If
      If Not bMatch Then GoTo loopxyz

finisher:
      If bMatch Then
         DeprStrt = MDC(iXYZ, 4)
         year1 = MDC(iXYZ, 5)
         DBSL = MDC(iXYZ, 6)
         UOPStrm = MDC(iXYZ, 7)
      Else
         DeprStrt = 1
         year1 = 1
         DBSL = 1
         UOPStrm = 1
      End If
      Return

End Sub


' 3 Jul 2001 JWD New procedure (C0345)
'
' Compute the interest earned on unrecovered
' scheduled cost recovery amounts and add to
' the calculated cost recovery.
'
' Monthly calculation of interest on
' unrecovered, scheduled amounts
' For purposes of this calculation,
' the recovery month-to-month is
' straightline, regardless of how
' the actual annual amount is calculated.
' Furthermore, the monthly interest rate
' is 1/12 of the rate specified on the
' dp form.
'
' CostRecoveryAmounts() is the calculated
' array of cost recovery.
' InterestRate is the amount from the DP form.
' FirstYearMonths is number of months in
' the first year.
'
Private Sub zzz_AddMonthlyInterestToCostRecovery _
               ( _
               ByRef CostRecoveryAmounts() As Single, _
               ByVal InterestRate As Single, _
               ByVal startperiod As Integer, _
               ByVal FirstYearMonths As Integer _
               )

   ' Uses: LG
   
   Dim i As Integer
   Dim last_period As Integer
   Dim months_in_year As Integer
   
   Dim interest_on_unrecovered As Single
   Dim periodic_recovery_amount As Single
   Dim periodic_interest_rate As Single
   Dim unrecovered As Single
   
   ' Determine total amount to be recovered
   unrecovered = 0
   For i = startperiod To LG
      unrecovered = unrecovered + CostRecoveryAmounts(i)
      If CostRecoveryAmounts(i) <> 0 Then
         last_period = i
      End If
   Next i
   
   periodic_interest_rate = InterestRate / 1200
   
   months_in_year = FirstYearMonths
   i = startperiod
   GoSub DoInterestCalculationMonthly
   
   ' Do subsequent years, these have 12 months (except maybe the last)
   months_in_year = 12
   For i = startperiod + 1 To last_period - 1
      GoSub DoInterestCalculationMonthly
   Next i
   
   ' Do the last year
   ' Assume that if the first year was 12 months
   ' then the last year is 12 months also.
   ' Otherwise, assume that the number of months
   ' in the last year is 12 - number in first.
   If FirstYearMonths < 12 Then
      months_in_year = 12 - FirstYearMonths
   Else
      months_in_year = 12
   End If
   
   i = last_period
   GoSub DoInterestCalculationMonthly

   Exit Sub
   
DoInterestCalculationMonthly:
   
   periodic_recovery_amount = CostRecoveryAmounts(i) / months_in_year
   interest_on_unrecovered = zzz_Interest(unrecovered, periodic_recovery_amount, periodic_interest_rate, months_in_year)
   unrecovered = unrecovered - CostRecoveryAmounts(i)
   CostRecoveryAmounts(i) = CostRecoveryAmounts(i) + interest_on_unrecovered
   
   Return
   
End Sub

'
' 3 Jul 2001 JWD New procedure (C0345)
'
' Modifications:
' 3 Jul 2001 JWD
'  -> Add switch to set for early payments or
'     late payments. Change order of calculation
'     of interest relative to reducing unrecovered
'     amount depending on switch. (C0346)
'
' Compute total interest earned on an amount,
' assuming the principal amount is reduced by
' equal installments over the number periods
' specified. It is assumed that the periodic
' recovery amount is applied to the unrecoved
' amount before the calculation of the interest
' on the remaining unrecovered amount.
'
' This routine only calculates the interest
' earned over the number of periods. It does
' not actually change the unrecovered
' (principal) amount.
'
' PeriodicRateFactor is a factor, not percent.
'
Private Function zzz_Interest _
               ( _
               ByVal TotalUnrecoveredAmount As Single, _
               ByVal PeriodicRecoveryAmount As Single, _
               ByVal PeriodicRateFactor As Single, _
               ByVal NumberOfPeriods As Integer _
               ) As Single

   '<<<<<< 3 Jul 2001 JWD (C0346)
   Const early_payments = False
   '>>>>>> End (C0346)
   
   Dim i As Integer
   Dim interest_on_unrecovered As Single
   Dim unrecovered_amount As Single
   
   ' Total amount to earn interest
   unrecovered_amount = TotalUnrecoveredAmount
   
   '<<<<<< 3 Jul 2001 JWD (C0346)
   If early_payments Then
   '>>>>>> End (C0346)
      For i = 1 To NumberOfPeriods
         ' Reduce the amount to earn interest
         If unrecovered_amount > PeriodicRecoveryAmount Then
            unrecovered_amount = unrecovered_amount - PeriodicRecoveryAmount
         Else
            unrecovered_amount = 0
         End If
         ' Accumulate the interest earned over the number of periods
         interest_on_unrecovered = interest_on_unrecovered + unrecovered_amount * PeriodicRateFactor
      Next i
   '<<<<<< 3 Jul 2001 JWD (C0346
   Else
      ' Payments occur at end of period, so
      ' interest is earned on entire amount.
      For i = 1 To NumberOfPeriods
         ' Accumulate the interest earned over the number of periods
         interest_on_unrecovered = interest_on_unrecovered + unrecovered_amount * PeriodicRateFactor
         ' Reduce the amount to earn interest
         If unrecovered_amount > PeriodicRecoveryAmount Then
            unrecovered_amount = unrecovered_amount - PeriodicRecoveryAmount
         Else
            unrecovered_amount = 0
         End If
      Next i
   End If
   '>>>>>> End (C0346)
   
   zzz_Interest = interest_on_unrecovered
   
End Function
