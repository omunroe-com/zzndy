Option Strict Off
Option Explicit On
Module MRepay
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
	'UPGRADE_NOTE: DefSng A-Z statement was removed. Variables were explicitly declared as type Single. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="92AFD3E3-440D-4D49-A8BF-580D74A8C9F2"'
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
		Dim param As Short
		Dim DefAmount As Single
		Dim Fd As Short
		Dim CLEX As String
		Dim iX As Single
		Dim TimeLeftYear As Single
		Dim IntExponent As Single
		Dim RelProdStart As Single
		Dim ANNR As Single
		Dim AnnRem As Single
		Dim STAT As Single
		Dim ANNS As Single
		Dim RepayTimeLeft As Single
		Dim AccTimeLimit As Single
		Dim AccTime As Single
		Dim TIMEX As Single
		Dim RPBASE As Single
		Dim REPA As Object
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
		Dim Numvar As Short
		Dim Ratetot As Short
		Dim j As Short
		Dim i As Short
		Dim iBYr As Short
		Dim k As Short
		Dim iLoopx As Short
		Dim iFini As Short
		Dim iStart As Short
		Dim iXZZ As Short
		Dim iZDB As Short
		Dim TooLong As Short
		' Added GDP 10/9/99
		Dim L As Short
		
		'---------------------------------------------------------
		'Dim dStart As Double
		'dStart = Timer
		
		Dim REPASING(LG) As Object
		Dim CXRE(LG, 5) As Object
		Dim clngx(LG) As Object
		Dim CX(LG, 5) As Object
		ReDim clngs(LG)
		Dim CLRA(LG) As Object
		Dim ColumnNm(15) As Object
		
10001: On Error GoTo 25000
10690: 
		'******** output file for testing
		'FileNm = FreeFile
		'Open "C:\Mobil\Test.TXT" For Output As #FileNm
		'***********************
		
		' Added GDP 10/9/99 -
		If g_bPTCons Then GoTo 12840
		
		If PRTA = 0 And BURS = 0 Then GoTo 12840
		If PRTA = 0 Then GoTo 12460
		ReDim TOTPMT(LG)
		
10692: ' NOW CALCULATE CEILING FOR REPAYMENT
10694: 'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        x14000(clngs, clngx, CLRA, CLEX, Fd, Ratetot, DefAmount, Numvar, param)
10696: ' NOW APPLY PARTICIPATION TERMS
		
10700: If PTT = 0 Then GoTo 12460
10710: ' LOOP THRU CAPEX
10720: If my3tt = 0 Then GoTo 12840
		
10730: For i = 1 To my3tt
			TooLong = 0 'reset to zero
10740: ReDim REPA(LG)
10760: ' CHECK IF CAPEX CATEGORY DEFINED EXPLICITLY
10770: j = 0
			
10780: j = j + 1
10790: If j > PTT Then GoTo 10820
10800: If (my3(i, 1) + 3) = PT(j, 1) Then GoTo 11030
10810: GoTo 10780
			
10820: ' CHECK IF CAPEX CATEGORY DEFINED IN EXP OR DEV
10830: j = 0
			
10840: j = j + 1
10850: If j > PTT Then GoTo 10940
10860: If my3(i, 1) >= 15 Then GoTo 10940
10870: If my3(i, 1) >= 9 And my3(i, 1) <= 14 Then GoTo 10910
10880: ' CAPEX IS EXPLORATION - SEE IF EXPLORATION IS DEFINED
10890: If PT(j, 1) = 2 Then GoTo 11030
10900: GoTo 10840
			
10910: ' CAPEX IS DEVELOPMENT - SEE IF DEVELOPMENT IS DEFINED
10920: If PT(j, 1) = 3 Then GoTo 11030
10930: GoTo 10840
			
10940: ' CHECK IF ALL DEFINED
10950: j = 0
			
10960: j = j + 1
10970: If j > PTT Then GoTo 11000
10980: If PT(j, 1) = 1 Then GoTo 11030
10990: GoTo 10960
			
11000: ' NO MATCH WAS FOUND - THUS NO REPAYMENT
11010: 
11020: GoTo 12390
			
11030: 
11350: 'REPAYMENT SECTION - PT(j,6), PERIOD, IS NOT IMPLEMENTED
			'CALCULATE AMOUNT TO BE REPAID, IF ANY
			
			'RPBASE
			RPBASE = (GPRATE(i) - GPBASE(i)) * (PT(j, 5) / 100) * my3(i, 5)
			
			RPBASE = RPBASE * (WINC(i) - REIM(i))
			If RPBASE > 0 Then
				If PT(j, 8) = -984 Then 'ACCRUE INTEREST
					If PT(j, 4) = -989 Then 'TO DISCOVERY DATE
						TIMEX = Y2 - my3(i, 3) + ((M2 - my3(i, 2)) / 12)
					ElseIf PT(j, 4) = -987 Then  'OR TO PRODUCTION START
						
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
							AccTimeLimit = System.Math.Abs(PT(j, 6))
							
							If AccTime >= AccTimeLimit Then
								'all interest used up by the time production starts
								TIMEX = AccTimeLimit
								RepayTimeLeft = 0
							Else
								'interest may still be earned after production starts
								TIMEX = AccTime
								RepayTimeLeft = AccTimeLimit - AccTime 'time left for accrual of interest after prod start
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
				ElseIf PT(j, 8) = -983 Then  ' Accrue interest beginning at repay start
					RepayTimeLeft = LG
					' End (C0657)
					
				End If
				If PT(j, 4) = -989 Then 'REPAY AT DISCOVERY
					'UPGRADE_WARNING: Couldn't resolve default property of object REPA(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					REPA(Y2 - YR + 1) = RPBASE ' WITH NO CEILING APPLIED
				ElseIf PT(j, 4) = -987 Then  'REPAY AT PRODUCTION
					
					'If PT(j, 6) = 0 Then                  ' original statement
					' 7/9/98 following change made to accomodate possible entry of
					' negative number in PT(j,6)
					If PT(j, 6) <= 0 Then '7/9/98 new statement
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
					If iFini > LG Then 'repayment is past end of project
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
							'UPGRADE_WARNING: Couldn't resolve default property of object clngx(iBYr). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
							If ANNR <= clngx(iBYr) Then
								'UPGRADE_WARNING: Couldn't resolve default property of object REPASING(iBYr). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
								REPASING(iBYr) = ANNR
								'UPGRADE_WARNING: Couldn't resolve default property of object clngx(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
								'UPGRADE_WARNING: Couldn't resolve default property of object clngx(iBYr). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
								clngx(iBYr) = clngx(iBYr) - ANNR
								ANNR = 0
							Else
								'UPGRADE_WARNING: Couldn't resolve default property of object clngx(iBYr). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
								'UPGRADE_WARNING: Couldn't resolve default property of object REPASING(iBYr). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
								REPASING(iBYr) = clngx(iBYr)
								
								If RepayTimeLeft = LG Then 'this is when there is no time limit entered
									
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
									
									'UPGRADE_WARNING: Couldn't resolve default property of object clngx(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
									ANNR = (ANNR - clngx(iBYr)) * ((1 + (PT(j, 7) / 100)) ^ IntExponent)
									
								Else
									
									'When there is a limit put on the number of years that
									'interest may be accrued (negative number in Repay Period
									'column), then you come here.
									
									If RepayTimeLeft <= 0 Then 'No interest applied anymore
										
										'Write #FileNm, "iBYr = ", iBYr
										'Write #FileNm, "No Repayment Time Left", "RepayTimeLeft = ", RepayTimeLeft
										
										'UPGRADE_WARNING: Couldn't resolve default property of object clngx(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
										ANNR = ANNR - clngx(iBYr)
										'Write #FileNm, "Ending ANNR, after ceiling subtracted = ", ANNR
										
									Else 'There is still some time left
										
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
										'UPGRADE_WARNING: Couldn't resolve default property of object clngx(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
										ANNR = (ANNR - clngx(iBYr)) * ((1 + (PT(j, 7) / 100)) ^ AccTime)
										'Write #FileNm, "Ending ANNR = ", ANNR
										
									End If
									
								End If
								
								
								'***** end modifications 7/9/98
								
								
								'UPGRADE_WARNING: Couldn't resolve default property of object clngx(iBYr). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
								clngx(iBYr) = 0
								If iBYr = LG Then
									ANNR = 0
								End If
								
							End If
							
						End While
						
						For iZDB = iStart To LG
							'UPGRADE_WARNING: Couldn't resolve default property of object REPASING(iZDB). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
							'UPGRADE_WARNING: Couldn't resolve default property of object REPA(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
							REPA(iZDB) = REPA(iZDB) + REPASING(iZDB)
						Next iZDB
					Next k
				End If
			End If
			
12290: 'PUT CAPITAL EXPENDITURES AND REPAYMENTS IN CATEGORIES
12310: '   If MY3(i, 1) >= 1 And MY3(i, 1) <= 3 Then XZZ = 1
12320: '   If MY3(i, 1) >= 4 And MY3(i, 1) <= 8 Then XZZ = 2
12330: '   If MY3(i, 1) >= 9 And MY3(i, 1) <= 14 Then XZZ = 3
12340: '   If MY3(i, 1) >= 15 And MY3(i, 1) <= 17 Then XZZ = 4
12355: 
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
			
12360: For k = 1 To LG
12370: 'UPGRADE_WARNING: Couldn't resolve default property of object REPA(k). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				'UPGRADE_WARNING: Couldn't resolve default property of object CX(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				'UPGRADE_WARNING: Couldn't resolve default property of object CX(k, iXZZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				CX(k, iXZZ) = CX(k, iXZZ) + REPA(k)
12380: Next k
12390: Next i
		
12400: 
12410: For i = 1 To LG
12420: 'UPGRADE_WARNING: Couldn't resolve default property of object CX(i, 4). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			'UPGRADE_WARNING: Couldn't resolve default property of object CX(i, 3). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			'UPGRADE_WARNING: Couldn't resolve default property of object CX(i, 2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			'UPGRADE_WARNING: Couldn't resolve default property of object CX(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			'UPGRADE_WARNING: Couldn't resolve default property of object CX(i, 5). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			CX(i, 5) = CX(i, 1) + CX(i, 2) + CX(i, 3) + CX(i, 4)
12440: 'UPGRADE_WARNING: Couldn't resolve default property of object CX(i, 5). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			TOTPMT(i) = CX(i, 5)
12450: Next i
12452: 
12460: 'CALCULATE PARTNER REIMBURSEMENT OF CAPITAL
		
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
			
			'UPGRADE_WARNING: Couldn't resolve default property of object CXRE(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			'UPGRADE_WARNING: Couldn't resolve default property of object CXRE(j, iXZZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			CXRE(j, iXZZ) = CXRE(j, iXZZ) + (my3(i, 5) * GPRATE(i) * REIM(i))
		Next i
		
		For i = 1 To LG
			'UPGRADE_WARNING: Couldn't resolve default property of object CXRE(i, 4). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			'UPGRADE_WARNING: Couldn't resolve default property of object CXRE(i, 3). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			'UPGRADE_WARNING: Couldn't resolve default property of object CXRE(i, 2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			'UPGRADE_WARNING: Couldn't resolve default property of object CXRE(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			'UPGRADE_WARNING: Couldn't resolve default property of object CXRE(i, 5). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			CXRE(i, 5) = CXRE(i, 1) + CXRE(i, 2) + CXRE(i, 3) + CXRE(i, 4)
			'UPGRADE_WARNING: Couldn't resolve default property of object CXRE(i, 5). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			TOTPMT(i) = TOTPMT(i) + CXRE(i, 5)
		Next i
		
		' PRINT GOVERNMENT REPAYMENT & REIMBURSEMENT OF CAPITAL
12462: 
12470: If Left(RF(5), 3) = "ALL" Then GoTo 12480
12475: GoTo 12840
12480: '
12482: 'UPGRADE_WARNING: Couldn't resolve default property of object ColumnNm$(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		ColumnNm(1) = " REPREN"
12484: 'UPGRADE_WARNING: Couldn't resolve default property of object ColumnNm$(2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		ColumnNm(2) = " REPEXP"
12486: 'UPGRADE_WARNING: Couldn't resolve default property of object ColumnNm$(3). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		ColumnNm(3) = " REPDEV"
12488: 'UPGRADE_WARNING: Couldn't resolve default property of object ColumnNm$(4). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		ColumnNm(4) = " REPOTH"
12490: 'UPGRADE_WARNING: Couldn't resolve default property of object ColumnNm$(5). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		ColumnNm(5) = " REPTOT"
		'UPGRADE_WARNING: Couldn't resolve default property of object ColumnNm$(6). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		ColumnNm(6) = " REIREN"
		'UPGRADE_WARNING: Couldn't resolve default property of object ColumnNm$(7). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		ColumnNm(7) = " REIEXP"
		'UPGRADE_WARNING: Couldn't resolve default property of object ColumnNm$(8). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		ColumnNm(8) = " REIDEV"
		'UPGRADE_WARNING: Couldn't resolve default property of object ColumnNm$(9). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		ColumnNm(9) = " REIOTH"
		'UPGRADE_WARNING: Couldn't resolve default property of object ColumnNm$(10). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		ColumnNm(10) = " REITOT"
12492: 
12494: 'Page type, Start year, Page counter, life of field, number of columns, page title, column length
12496: ''''Write #5, 9, YR, 0, LG, 10, "GOVERNMENT REPAYMENT & PARTNER REIMBURSEMENT", 10, FinalWin, FINALPARTIC, sCur
		
		
12498: 'columns titles
12500: ''''Write #5, ColumnNm$(1), ColumnNm$(2), ColumnNm$(3), ColumnNm$(4), ColumnNm$(5), ColumnNm$(6), ColumnNm$(7), ColumnNm$(8), ColumnNm$(9), ColumnNm$(10)
12504: 
		
        Dim oPg1 As IGiantRptPageAssignStd
		oPg1 = g_oReport.NewStandardRptPage
		' 11 Feb 2005 JWD (C0858) Change "GOVERNMENT" to "NOC"
		oPg1.SetPageHeader(9, YR, 0, LG, 10, "NOC REPAYMENT & PARTNER REIMBURSEMENT", 10, FinalWin, FINALPARTIC, sCur)
		' Was:
		'oPg1.SetPageHeader 9, YR, 0, LG, 10, "GOVERNMENT REPAYMENT & PARTNER REIMBURSEMENT", 10, FinalWin, FINALPARTIC, sCur
		' End (C0858)
		oPg1.SetProfileHeaders(ColumnNm(1), ColumnNm(2), ColumnNm(3), ColumnNm(4), ColumnNm(5), ColumnNm(6), ColumnNm(7), ColumnNm(8), ColumnNm(9), ColumnNm(10))
		
12650: For i = 1 To LG
12690: ''''Write #5, CX(i, 1), CX(i, 2), CX(i, 3), CX(i, 4), CX(i, 5), CXRE(i, 1), CXRE(i, 2), CXRE(i, 3), CXRE(i, 4), CXRE(i, 5)
			oPg1.SetProfileValues(i, CX(i, 1), CX(i, 2), CX(i, 3), CX(i, 4), CX(i, 5), CXRE(i, 1), CXRE(i, 2), CXRE(i, 3), CXRE(i, 4), CXRE(i, 5))
12750: Next i
12752: 
12840: 'ALL DONE
12842: If FVAR(iX) <> "CMB" Then
			' Commented out GDP 18/8/99 -  for changes to consolidation - net of participation pre tax consolidation
			'13200    Erase CLG$, CGR, sCGR, FVAR$, Inflate, sMDC, MDC, PC, sPCV, sPRV, PR
13200: 'Erase CLG$, CGR, sCGR, FVAR$, sMDC, MDC, PC, sPCV, sPRV, PR
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

		'--------------------------------------------------------------------
25000: ' THIS PRINTS SYSTEM ERRORS
		Program = "REPAY"
		' 9 Feb 2004 JWD (C0779) Replace with re-raise of error to caller
		Err.Raise(Err.Number) ' TerminateExecution
		
    End Sub

    Sub x14000(ByRef clngs, ByRef clngx, ByRef CLRA, ByRef clex, ByRef fd, ByRef Ratetot, ByRef DefAmount, ByRef Numvar, ByRef param)
14000:  ' THIS SUBROUTINE CALCULATES CEILING FOR PARTICIPATION REPAYMENT
        ' FIND CEILING TO MATCH VARIABLE
        ReDim clngs(LG)
        ReDim clngx(LG)
        ReDim CLRA(LG)

        Dim i As Short
        Dim j As Short

14030:
        i = 0
        clex = "Y"
        fd = 0

14040:  i = i + 1
        If i > CLGTT Then
            clex = "N"
            GoTo 14472
        End If
        If CLG(i, 1) = "PAR" Then fd = 1 : GoTo 14080
        GoTo 14040


14080:  ' LOOP THRU INCOME
        Dim matcher(10) As Object
        ReDim clngs(LG)

        For j = 1 To 10
            'UPGRADE_WARNING: Couldn't resolve default property of object matcher$(j). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            matcher(j) = CLG(i, j)
        Next j
        CeilDef("REPAY", TDT, matcher, clngs)

14472:  'when the user did not specify a ceiling def then GRP
        If fd <> 1 Then
            For j = 1 To LG
                ' GDP 20 JAN 2003
                ' Use constant for offset
                clngs(j) = clngs(j) + (A(j, PPR) * A(j, PPR + gc_nAPRICEOFFSET)) * (1 - PARTRATE(j)) * WIN(j)
            Next j
        End If

14480:  ' NOW DETERMINE CEILING RATES IN CGR()
        If CGRT > 0 Then GoTo contin
        fd = 0
        GoTo default_Renamed

contin:
        fd = 1
        Ratetot = CGRT

        Dim sRateInV(Ratetot) As String
        Dim ratein(Ratetot, 6) As Single
        Dim VarRates(LG) As Single

        For j = 1 To Ratetot
            sRateInV(j) = sCGR(j)
            'UPGRADE_WARNING: Couldn't resolve default property of object ratein(j, 1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            ratein(j, 1) = CGR(j, 1)
            'UPGRADE_WARNING: Couldn't resolve default property of object ratein(j, 2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            ratein(j, 2) = CGR(j, 2)
            'UPGRADE_WARNING: Couldn't resolve default property of object ratein(j, 3). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            ratein(j, 3) = CGR(j, 3)
            'UPGRADE_WARNING: Couldn't resolve default property of object ratein(j, 4). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            ratein(j, 4) = CGR(j, 4)
            'UPGRADE_WARNING: Couldn't resolve default property of object ratein(j, 5). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            ratein(j, 5) = CGR(j, 5)
            'UPGRADE_WARNING: Couldn't resolve default property of object ratein(j, 6). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            ratein(j, 6) = CGR(j, 6)
        Next j
        defamount = 100
        RateCalc(numvar, "REPAY", "PAR", defamount, sRateInV, ratein, Ratetot, param, VarRates)

        For j = 1 To LG
            'UPGRADE_WARNING: Couldn't resolve default property of object VarRates(j). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            'UPGRADE_WARNING: Couldn't resolve default property of object CLRA(j). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            CLRA(j) = VarRates(j)
        Next j
        GoTo 14800

default_Renamed:
        'when the user did not specify the ceiling rates then
        If fd <> 1 Then
            For j = 1 To LG
                'UPGRADE_WARNING: Couldn't resolve default property of object CLRA(j). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                CLRA(j) = 100
            Next j
        End If

14800:  ' NOW COMPUTE CEILING
14802:  For j = 1 To LG
14804:      'UPGRADE_WARNING: Couldn't resolve default property of object CLRA(j). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            'UPGRADE_WARNING: Couldn't resolve default property of object clngx(j). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            clngx(j) = clngs(j) * (CLRA(j) / 100)
14806:  Next j

14999:  'UPGRADE_WARNING: Return has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
    End Sub
End Module