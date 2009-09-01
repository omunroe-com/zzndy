Attribute VB_Name = "MRepay"
'$linesize: 132
'$title:    'GIANT v6.1 - 1996                         REPAY.BAS'
'$subtitle: 'Repay program'
' **********************************************************************
' *        COPYRIGHT - PETROCONSULTANTS, INC. - 1985, 1995, 1996       *
' *                    ALL RIGHTS RESERVED                             *
' **********************************************************************
' *  This program file is proprietary information of Petroconsultants, *
' *  Incorporated.  Unauthorized use for any purpose is prohibited.    *
' **********************************************************************
'-----------------------------------------------------------------------
' Modifications:
' 6 Feb 1996 JWD
'          Replace include file SUBINCL.BAS with UTIL5.BI.
'          Add interface declaration include file REPAY.BI.
'          Replaced include file CTYIN.BAS with CTYIN1.BG.
'          Add explicit declaration of default storage class as Single.
' 19 Feb 1996 JWD
'          Changed references to common array RATE() to PARTRATE().
'       RATE is reserved function name in VB.
'
' 5 Feb 2003 JWD
'  -> Changed CalculateRepayment(). (C0657)
'
' 12 Jan 2004 JWD
'  -> Changed CalculateRepayment(). (C0772)
'
' 19 Jan 2004 JWD
'  -> Changed CalculateRepayment(). (C0774)
'
' 3 Feb 2004 JWD
'  -> Changed CalculateRepayment(). (C0776)
'
' 9 Feb 2004 JWD
'  -> Changed CalculateRepayment(). (C0779)
'
' 11 Feb 2005 JWD
'  -> Changed CalculateRepayment(). (C0858)
'-----------------------------------------------------------------------
' $DYNAMIC
DefSng A-Z
' $include: 'ctyin1.bg'
' $include: 'repay.bi'
' $include: 'util5.bi'
' $include: 'pgm9900.bi'

'-----------------------------------------------------------------------
'$subtitle: 'Procedure: CalculateRepayment'
'$page
'
' Modifications:
' 5 Feb 2003 JWD
'  -> Add initialization of RepayTimeLeft to LG in case
'     where REP is specified as Repay Accrual option. Not
'     initializing this was resulting in a failure to
'     properly add interest to the ceiling deferred repay-
'     ments because it was being handled as if the time
'     allowed for interest accrual had ended. (C0657)
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
' 3 Feb 2004 JWD
'  -> Remove explicit writes to report file. (C0776)
'
' 9 Feb 2004 JWD
'  -> Replace call to TerminateExecution with re-raise of
'     error to caller. (C0779)
'
' 11 Feb 2005 JWD
'  -> Change report section title, replace "Government"
'     with "NOC". (C0858)
'
Sub CalculateRepayment()
'-----------------------------------------------------------------------
' This program calculates government repayment.
'---------------------------------------------------------
' Modifications:
' 20 Feb 1996 JWD
'        Change CUR$ to sCur, duplicate definition (CUR()).
'        Change CGR$ to sCGR, duplicate definition (CGR()).
'        Change MDC$ to sMDC, duplicate definition (MDC()).
'        Change PC$ to sPCV, duplicate definition (PC()).
'        Change PR$ to sPRV, duplicate definition (PR()).
'        Change RT$ to sRTV, duplicate definition (RT()).
'        Change TM$ to sTMV, duplicate definition (TM()).
'---------------------------------------------------------
      Dim Numvar     As Integer
      Dim Ratetot    As Integer
      Dim j          As Integer
      Dim i          As Integer
      Dim iBYr       As Integer
      Dim k          As Integer
      Dim iLoopx     As Integer
      Dim iFini      As Integer
      Dim iStart     As Integer
      Dim iXZZ       As Integer
      Dim iZDB       As Integer
      Dim TooLong    As Integer
      ' Added GDP 10/9/99
      Dim L          As Integer

'---------------------------------------------------------
'Dim dStart As Double
'dStart = Timer

      ReDim REPASING(LG), CXRE(LG, 5)
      ReDim clngx(LG), CX(LG, 5), clngs(LG), CLRA(LG), ColumnNm$(15)

10001 On Local Error GoTo 25000
10690
          '******** output file for testing
           'FileNm = FreeFile
           'Open "C:\Mobil\Test.TXT" For Output As #FileNm
          '***********************
          
          ' Added GDP 10/9/99 -
          If g_bPTCons Then GoTo 12840
          
          If PRTA = 0 And BURS = 0 Then GoTo 12840
          If PRTA = 0 Then GoTo 12460
          ReDim TOTPMT(LG)

10692           ' NOW CALCULATE CEILING FOR REPAYMENT
10694 GoSub 14000
10696           ' NOW APPLY PARTICIPATION TERMS

10700 If PTT = 0 Then GoTo 12460
10710           ' LOOP THRU CAPEX
10720 If my3tt = 0 Then GoTo 12840

10730 For i = 1 To my3tt
         TooLong = 0             'reset to zero
10740    ReDim REPA(LG)
10760           ' CHECK IF CAPEX CATEGORY DEFINED EXPLICITLY
10770    j = 0

10780    j = j + 1
10790    If j > PTT Then GoTo 10820
10800    If (my3(i, 1) + 3) = PT(j, 1) Then GoTo 11030
10810    GoTo 10780

10820           ' CHECK IF CAPEX CATEGORY DEFINED IN EXP OR DEV
10830    j = 0

10840    j = j + 1
10850    If j > PTT Then GoTo 10940
10860    If my3(i, 1) >= 15 Then GoTo 10940
10870    If my3(i, 1) >= 9 And my3(i, 1) <= 14 Then GoTo 10910
10880           ' CAPEX IS EXPLORATION - SEE IF EXPLORATION IS DEFINED
10890    If PT(j, 1) = 2 Then GoTo 11030
10900    GoTo 10840

10910           ' CAPEX IS DEVELOPMENT - SEE IF DEVELOPMENT IS DEFINED
10920    If PT(j, 1) = 3 Then GoTo 11030
10930    GoTo 10840

10940           ' CHECK IF ALL DEFINED
10950    j = 0

10960    j = j + 1
10970    If j > PTT Then GoTo 11000
10980    If PT(j, 1) = 1 Then GoTo 11030
10990    GoTo 10960

11000           ' NO MATCH WAS FOUND - THUS NO REPAYMENT
11010
11020    GoTo 12390

11030
11350           'REPAYMENT SECTION - PT(j,6), PERIOD, IS NOT IMPLEMENTED
                'CALCULATE AMOUNT TO BE REPAID, IF ANY

                'RPBASE
         RPBASE = (GPRATE(i) - GPBASE(i)) * (PT(j, 5) / 100) * my3(i, 5)

         RPBASE = RPBASE * (WINC(i) - REIM(i))
         If RPBASE > 0 Then
            If PT(j, 8) = -984 Then                   'ACCRUE INTEREST
               If PT(j, 4) = -989 Then                'TO DISCOVERY DATE
                  TIMEX = Y2 - my3(i, 3) + ((M2 - my3(i, 2)) / 12)
               ElseIf PT(j, 4) = -987 Then            'OR TO PRODUCTION START
                  
                  '************ BEGIN NEW CODE 7/9/98
                  If PT(j, 6) < 0 Then
                  
                     'New Code to allow a time limit to be used for the accrual
                     'of interest for each capital item.  This limit is entered
                     'in the Repay Period column of Govt Partic Terms form as a
                     'NEGATIVE number, so that it can be differentiated from the
                     'standard definition of repayment period.
                  
                     'First step is to see if the limit is reached prior to the
                     'production start date (i.e. repayment start date).
                   
                     'AccTime = time from capex date to prod start date
                     AccTime = Y1 - my3(i, 3) + ((M1 - my3(i, 2)) / 12)
                     
                     'AccTimeLimit = time limit entered on GP Terms form
                     AccTimeLimit = Abs(PT(j, 6))

                     If AccTime >= AccTimeLimit Then
                        'all interest used up by the time production starts
                        TIMEX = AccTimeLimit
                        RepayTimeLeft = 0
                     Else
                        'interest may still be earned after production starts
                        TIMEX = AccTime
                        RepayTimeLeft = AccTimeLimit - AccTime    'time left for accrual of interest after prod start
                     End If

                  Else

                     TIMEX = Y1 - my3(i, 3) + ((M1 - my3(i, 2)) / 12) 'original code
                     RepayTimeLeft = LG

                  End If
                  '************** 7/9/98  end of new code

                  If TIMEX < 0 Then
                     TIMEX = 0
                  End If
               Else
                  TIMEX = 0
               End If
               If TIMEX > 0 Then
                  RPBASE = RPBASE * ((1 + (PT(j, 7) / 100))) ^ TIMEX
               End If
               
            ' 5 Feb 2003 JWD (C0657)
            ElseIf PT(j, 8) = -983 Then            ' Accrue interest beginning at repay start
               RepayTimeLeft = LG
            ' End (C0657)
            
            End If
            If PT(j, 4) = -989 Then                      'REPAY AT DISCOVERY
               REPA(Y2 - YR + 1) = RPBASE             ' WITH NO CEILING APPLIED
            ElseIf PT(j, 4) = -987 Then                  'REPAY AT PRODUCTION
               
               'If PT(j, 6) = 0 Then                  ' original statement
               ' 7/9/98 following change made to accomodate possible entry of
               ' negative number in PT(j,6)
               If PT(j, 6) <= 0 Then                  '7/9/98 new statement
                  ANNS = RPBASE
                  iLoopx = 1
               Else
                  ANNS = RPBASE / Int(PT(j, 6))
                  iLoopx = Int(PT(j, 6))
               End If
               STAT = Y1
               If my3(i, 3) > Y1 Then
                  STAT = my3(i, 3)
               End If
               iStart = STAT - YR + 1
               iFini = iStart + iLoopx - 1

'------------------------------------------------------
'4/10/97  Make sure that iFini does not point beyond the end of the project!!!!!
'------------------------------------------------------
               If iFini > LG Then       'repayment is past end of project
                  TooLong = 1
                  AnnRem = ANNS * (iFini - LG + 1)
                  iFini = LG
               End If
'--------------------------------------------------------

               For k = iStart To iFini
                  iBYr = k - 1
                  ANNR = ANNS
                  '------------------------------------
                  ' these lines added 4/10/97 to adjust last year amount of repay
                  If TooLong = 1 And k = iFini Then
                     ANNR = AnnRem
                  End If
                  '------------------------------------

                  ReDim REPASING(LG)
                  While ANNR
                     iBYr = iBYr + 1
                     If ANNR <= clngx(iBYr) Then
                        REPASING(iBYr) = ANNR
                        clngx(iBYr) = clngx(iBYr) - ANNR
                        ANNR = 0
                     Else
                        REPASING(iBYr) = clngx(iBYr)
                        
                        If RepayTimeLeft = LG Then         'this is when there is no time limit entered

                           'ANNR = (ANNR - clngx(iBYr)) * (1 + (PT(j, 7) / 100))  *** original statement
                        

                           '***** modified 7/9/98
                           'these changes were made because interest for a full year was being accrued
                           'if the repayment in the first year of production exceeded the ceiling.
                           'It now only accrues interest from production start month.
                        
                           'if 1st year of production
                           RelProdStart = Y1 - YR + 1
                           
                           If iBYr = RelProdStart Then
                              IntExponent = (12 - (M1 - 1)) / 12
                           Else
                              IntExponent = 1
                           End If

                           ANNR = (ANNR - clngx(iBYr)) * ((1 + (PT(j, 7) / 100)) ^ IntExponent)
                        
                        Else
                           
                           'When there is a limit put on the number of years that
                           'interest may be accrued (negative number in Repay Period
                           'column), then you come here.

                           If RepayTimeLeft <= 0 Then     'No interest applied anymore
                              
                              'Write #FileNm, "iBYr = ", iBYr
                              'Write #FileNm, "No Repayment Time Left", "RepayTimeLeft = ", RepayTimeLeft
                              
                              ANNR = ANNR - clngx(iBYr)
                              'Write #FileNm, "Ending ANNR, after ceiling subtracted = ", ANNR

                           Else                           'There is still some time left
                           
                              'Write #FileNm, "RepayTimeLeft Should be > 0 but less than lg"
                              'Write #FileNm, "RepayTimeLeft = ", RepayTimeLeft


                              'Calculate how much time is left in this year for accrual of interest
                              RelProdStart = Y1 - YR + 1
                              If iBYr = RelProdStart Then
                                 TimeLeftYear = (12 - (M1 - 1)) / 12 'Fraction of time left in 1st year of prod
                              Else
                                 TimeLeftYear = 1
                              End If

                              'Write #FileNm, "iBYr = ", iBYr, "RelProdStart = ", RelProdStart
                              'Write #FileNm, "RepayTimeLeft = ", RepayTimeLeft, "TimeLeftYear = ", TimeLeftYear


                              If RepayTimeLeft <= TimeLeftYear Then
                                 'Remaining time used up before end of 1st
                                 AccTime = RepayTimeLeft
                                 RepayTimeLeft = 0

                              Else
                                 'Still time remaining after this year
                                 AccTime = TimeLeftYear
                                 RepayTimeLeft = RepayTimeLeft - TimeLeftYear

                              End If

                            'Write #FileNm, "Beginning ANNR = ", ANNR, " clngx(iBYr) = ", clngx(iBYr), "AccTime = ", AccTime
                            ANNR = (ANNR - clngx(iBYr)) * ((1 + (PT(j, 7) / 100)) ^ AccTime)
                            'Write #FileNm, "Ending ANNR = ", ANNR

                           End If
                       
                        End If
                        
                        
                        '***** end modifications 7/9/98

                        
                        clngx(iBYr) = 0
                        If iBYr = LG Then
                           ANNR = 0
                        End If
                     
                     End If
                  
                  Wend
                  
                  For iZDB = iStart To LG
                     REPA(iZDB) = REPA(iZDB) + REPASING(iZDB)
                  Next iZDB
               Next k
            End If
         End If

12290           'PUT CAPITAL EXPENDITURES AND REPAYMENTS IN CATEGORIES
12310 '   If MY3(i, 1) >= 1 And MY3(i, 1) <= 3 Then XZZ = 1
12320 '   If MY3(i, 1) >= 4 And MY3(i, 1) <= 8 Then XZZ = 2
12330 '   If MY3(i, 1) >= 9 And MY3(i, 1) <= 14 Then XZZ = 3
12340 '   If MY3(i, 1) >= 15 And MY3(i, 1) <= 17 Then XZZ = 4
12355
         Select Case my3(i, 1)
            Case 1 To 3
               iXZZ = 1
            Case 4 To 8
               iXZZ = 2
            Case 9 To 14
               iXZZ = 3
            Case 15 To 23
               iXZZ = 4
         End Select

12360    For k = 1 To LG
12370       CX(k, iXZZ) = CX(k, iXZZ) + REPA(k)
12380    Next k
12390 Next i

12400
12410 For i = 1 To LG
12420    CX(i, 5) = CX(i, 1) + CX(i, 2) + CX(i, 3) + CX(i, 4)
12440    TOTPMT(i) = CX(i, 5)
12450 Next i
12452
12460           'CALCULATE PARTNER REIMBURSEMENT OF CAPITAL

      If BURS = 0 Then GoTo 12462

      For i = 1 To my3tt

                ' PUT CAPITAL EXPENDITURES AND REPAYMENTS IN CATEGORIES
         'If MY3(i, 1) >= 1 And MY3(i, 1) <= 3 Then XZZ = 1
         'If MY3(i, 1) >= 4 And MY3(i, 1) <= 8 Then XZZ = 2
         'If MY3(i, 1) >= 9 And MY3(i, 1) <= 14 Then XZZ = 3
         'If MY3(i, 1) >= 15 And MY3(i, 1) <= 17 Then XZZ = 4
         Select Case my3(i, 1)
         Case 1 To 3
            iXZZ = 1
         Case 4 To 8
            iXZZ = 2
         Case 9 To 14
            iXZZ = 3
         Case 15 To 23
            iXZZ = 4
         End Select

         j = my3(i, 3) - YR + 1
         If my3(i, 3) < Y1 Then j = Y1 - YR + 1

         CXRE(j, iXZZ) = CXRE(j, iXZZ) + (my3(i, 5) * GPRATE(i) * REIM(i))
      Next i

      For i = 1 To LG
         CXRE(i, 5) = CXRE(i, 1) + CXRE(i, 2) + CXRE(i, 3) + CXRE(i, 4)
         TOTPMT(i) = TOTPMT(i) + CXRE(i, 5)
      Next i

          ' PRINT GOVERNMENT REPAYMENT & REIMBURSEMENT OF CAPITAL
12462
12470 If Left$(RF$(5), 3) = "ALL" Then GoTo 12480
12475 GoTo 12840
12480 '
12482 ColumnNm$(1) = " REPREN"
12484 ColumnNm$(2) = " REPEXP"
12486 ColumnNm$(3) = " REPDEV"
12488 ColumnNm$(4) = " REPOTH"
12490 ColumnNm$(5) = " REPTOT"
      ColumnNm$(6) = " REIREN"
      ColumnNm$(7) = " REIEXP"
      ColumnNm$(8) = " REIDEV"
      ColumnNm$(9) = " REIOTH"
      ColumnNm$(10) = " REITOT"
12492
12494                   'Page type, Start year, Page counter, life of field, number of columns, page title, column length
12496 ''''Write #5, 9, YR, 0, LG, 10, "GOVERNMENT REPAYMENT & PARTNER REIMBURSEMENT", 10, FinalWin, FINALPARTIC, sCur


12498                   'columns titles
12500 ''''Write #5, ColumnNm$(1), ColumnNm$(2), ColumnNm$(3), ColumnNm$(4), ColumnNm$(5), ColumnNm$(6), ColumnNm$(7), ColumnNm$(8), ColumnNm$(9), ColumnNm$(10)
12504

      Dim oPg1 As IGiantRptPageAssignStd
      Set oPg1 = g_oReport.NewStandardRptPage
      ' 11 Feb 2005 JWD (C0858) Change "GOVERNMENT" to "NOC"
      oPg1.SetPageHeader 9, YR, 0, LG, 10, "NOC REPAYMENT & PARTNER REIMBURSEMENT", 10, FinalWin, FINALPARTIC, sCur
      ' Was:
      'oPg1.SetPageHeader 9, YR, 0, LG, 10, "GOVERNMENT REPAYMENT & PARTNER REIMBURSEMENT", 10, FinalWin, FINALPARTIC, sCur
      ' End (C0858)
      oPg1.SetProfileHeaders ColumnNm$(1), ColumnNm$(2), ColumnNm$(3), ColumnNm$(4), ColumnNm$(5), ColumnNm$(6), ColumnNm$(7), ColumnNm$(8), ColumnNm$(9), ColumnNm$(10)

12650 For i = 1 To LG
12690    ''''Write #5, CX(i, 1), CX(i, 2), CX(i, 3), CX(i, 4), CX(i, 5), CXRE(i, 1), CXRE(i, 2), CXRE(i, 3), CXRE(i, 4), CXRE(i, 5)
         oPg1.SetProfileValues i, CX(i, 1), CX(i, 2), CX(i, 3), CX(i, 4), CX(i, 5), CXRE(i, 1), CXRE(i, 2), CXRE(i, 3), CXRE(i, 4), CXRE(i, 5)
12750 Next i
12752
12840           'ALL DONE
12842 If FVAR$(iX) <> "CMB" Then
' Commented out GDP 18/8/99 -  for changes to consolidation - net of participation pre tax consolidation
'13200    Erase CLG$, CGR, sCGR, FVAR$, Inflate, sMDC, MDC, PC, sPCV, sPRV, PR
13200    'Erase CLG$, CGR, sCGR, FVAR$, sMDC, MDC, PC, sPCV, sPRV, PR
         'Erase PT, RLD, RT, sRTV, TM, sTMV
      End If
      '''PushStats "REPAY", dStart
      'Close #FileNm
      ' Added GDP 10/9/99
      ' Begin
      If g_bPTCons Then
        ' Set TOTPMT to variable from the RVS file
        ' Note that at this point TOTPMT contains the rapayment and the capital reimbursement.
        ReDim TOTPMT(LG)
        For L = 1 To LG
            TOTPMT(L) = TOTREPAY(L)
        Next L
      End If
      'End
      Exit Sub

'-----------------------------------------------------------------------
14000           ' THIS SUBROUTINE CALCULATES CEILING FOR PARTICIPATION REPAYMENT
                ' FIND CEILING TO MATCH VARIABLE
      ReDim clngs(LG), clngx(LG), CLRA(LG)

14030 i = 0
      CLEX$ = "Y"
      Fd% = 0

14040 i = i + 1
      If i > CLGTT Then
         CLEX$ = "N"
         GoTo 14472
      End If
      If CLG$(i, 1) = "PAR" Then Fd% = 1:  GoTo 14080
      GoTo 14040


14080           ' LOOP THRU INCOME
      ReDim matcher$(10), clngs(LG)

      For j = 1 To 10
         matcher$(j) = CLG$(i, j)
      Next j
      CeilDef "REPAY", TDT, matcher$(), clngs()

14472                    'when the user did not specify a ceiling def then GRP
      If Fd% <> 1 Then
         For j = 1 To LG
            ' GDP 20 JAN 2003
            ' Use constant for offset
            clngs(j) = clngs(j) + (A(j, PPR) * A(j, PPR + gc_nAPRICEOFFSET)) * (1 - PARTRATE(j)) * WIN(j)
         Next j
      End If

14480           ' NOW DETERMINE CEILING RATES IN CGR()
      If CGRT > 0 Then GoTo contin
      Fd% = 0
      GoTo default

contin:
      Fd% = 1
      Ratetot = CGRT

      ReDim sRateInV(Ratetot) As String
      ReDim ratein(Ratetot, 6)
      ReDim VarRates(LG)

      For j = 1 To Ratetot
         sRateInV(j) = sCGR(j)
         ratein(j, 1) = CGR(j, 1)
         ratein(j, 2) = CGR(j, 2)
         ratein(j, 3) = CGR(j, 3)
         ratein(j, 4) = CGR(j, 4)
         ratein(j, 5) = CGR(j, 5)
         ratein(j, 6) = CGR(j, 6)
      Next j
      DefAmount = 100
      RateCalc Numvar, "REPAY", "PAR", DefAmount, sRateInV(), ratein(), Ratetot, param%, VarRates()

      For j = 1 To LG
         CLRA(j) = VarRates(j)
      Next j
      GoTo 14800

default:
                'when the user did not specify the ceiling rates then
      If Fd% <> 1 Then
         For j = 1 To LG
            CLRA(j) = 100
         Next j
      End If

14800           ' NOW COMPUTE CEILING
14802 For j = 1 To LG
14804    clngx(j) = clngs(j) * (CLRA(j) / 100)
14806 Next j

14999 Return
'--------------------------------------------------------------------
25000           ' THIS PRINTS SYSTEM ERRORS
      Program$ = "REPAY"
      ' 9 Feb 2004 JWD (C0779) Replace with re-raise of error to caller
      Err.Raise Err.number                  ' TerminateExecution

End Sub

