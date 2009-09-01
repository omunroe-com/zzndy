Option Strict Off
Option Explicit On
Module MLoan
	'$linesize: 132
	'$title:    'GIANT v6.0 - 1995                               LOAN.BAS'
	'$subtitle: 'Loan calculation program'
	' Name:        Loan.bas
	' Function:    Loan Calculations
	'---------------------------------------------------------
	' ********************************************************
	' *        COPYRIGHT © 1989-2001 IHS ENERGY GROUP        *
	' *                  ALL RIGHTS RESERVED                 *
	' ********************************************************
	' *   This program file is proprietary information of    *
	' *                  IHS Energy Group                    *
	' *   Unauthorized use for any purpose is prohibited.    *
	' ********************************************************
	'---------------------------------------------------------
	' MODIFICATIONS:
	' 3 Mar 1995 JWD
	'          Converted module level executable code to subroutine.
	' 6 Feb 1996 JWD
	'          Changed Financing().
	'          Add interface declaration include file LOAN.BI.
	'          Replaced include file CTYIN.BAS with CTYIN1.BG.
	'          Add explicit declaration of default storage class as Single.
	'
	' 21 Jun 2001 JWD
	'  -> Changed Financing(). (C0339)
	'
	' 4 Aug 2001 JWD
	'  -> Changed Financing(). (C0363)
	'
	' 12 Jan 2004 JWD
	'  -> Changed Financing(). (C0772)
	'
	' 19 Jan 2004 JWD
	'  -> Changed Financing(). (C0774)
	'
	' 3 Feb 2004 JWD
	'  -> Changed Financing(). (C0776)
	'-----------------------------------------------------------------------
	'$dynamic
	'UPGRADE_NOTE: DefSng A-Z statement was removed. Variables were explicitly declared as type Single. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="92AFD3E3-440D-4D49-A8BF-580D74A8C9F2"'
	'$include: 'ctyin1.bg'
	'$include: 'loan.bi'
	
	'-----------------------------------------------------------------------
	'$subtitle: 'Procedure: Financing'
	'$page
	Sub Financing()
		Dim delay As Single
		Dim TempMo As Single
		Dim TempYr As Single
		Dim PRIN As Single
		Dim MLO As Single
		Dim Fed As String
		Dim YM As Single
		Dim XM As Single
		Dim j As Short
		Dim N As Short
		Dim YLO As Single
		Dim Mos As Short
		'-----------------------------------------------------------------------
		' Modifications:
		' 6 Feb 1996 JWD
		'          Removed include of SCRA1IN.BAS and SCRA1OUT.BAS.  The data
		'       variables read and written to the scratch file SCRA1.SCR are not
		'       referenced in this subroutine. Removed ReDims of array variables
		'       written on SCRA1.SCR.
		' 20 Feb 1996 JWD
		'        Change CUR$ to sCur, duplicate definition (CUR()).
		'
		' 21 Jun 2001 JWD
		'  -> Replace explicit references to MY3() category codes
		'     18, 19, & 20 (BAL, BL2, BL3) with global symbols for
		'     the same. Necessitated by addition of new capital
		'     category codes that changed the actual values of the
		'     BAL codes.(C0339)
		'
		' 4 Aug 2001 JWD
		'  -> Add CPXCategoryCode_AbandonmentAccrualEntry as an
		'     excluded category like BAL. (C0363)
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
		'-----------------------------------------------------------------------
		Dim x As Single
		Dim i As Short
		'-----------------------------------------------------------------------
		
		ReDim PRINC(LG)
		ReDim INTRST(LG)
		ReDim FINANCE(LG)
		ReDim LOAN(LG)
		'1-9-96  Dimming L() by 200 would crash if Repayment months > 200!
		'REDIM ColumnNm$(6), L(200, 4), LO(LG), LT(LG, 5), lot(LG, 5)
		Mos = (LG + 2) * 12
		Dim ColumnNm(6) As Single
		Dim L(Mos, 4) As Single
		Dim LO(LG) As Single
		Dim LT(LG, 5) As Single
		Dim lot(LG, 5) As Single
		
		
		If AMTLT > 0 Then 'if there are single amount loans
			For i = 1 To AMTLT
				If AMTLOAN(i, 2) > 49 Then 'converting the year to 1950 or 2049
					YLO = 1900 + AMTLOAN(i, 2)
				Else
					YLO = 2000 + AMTLOAN(i, 2)
				End If
				Call LoanCalc(YR, LG, AMTLOAN(i, 1), YLO, AMTLOAN(i, 3), AMTLOAN(i, 6), AMTLOAN(i, 7), AMTLOAN(i, 4), AMTLOAN(i, 5), AMTLOAN(i, 8), L, LO, LT)
				For N = 1 To LG 'summing the results
					LOAN(N) = LOAN(N) + LO(N)
					For j = 1 To 5
						lot(N, j) = lot(N, j) + LT(N, j)
					Next j
				Next N
				'1-9-96  Dimming L() by 200 would crash if Repayment months > 200!
				'REDIM L(200, 4), LO(LG), LT(LG, 5)      'erase array for next calculation
				Mos = (LG + 2) * 12
				ReDim L(Mos, 4)
				ReDim LO(LG)
				ReDim LT(LG, 5)
				
				YLO = 0
			Next i
		End If
		'if there are expenditure-based loans and capex
1020: If EXPLT > 0 And my3tt > 0 Then
1035: YLO = 0 'reset Year
1040: For XM = 1 To my3tt 'LOOP THRU CAPITAL EXPENDITURES INPUT
1060: YM = 0
1070: YM = YM + 1
1080: If YM > EXPLT Then GoTo 1110
				If (my3(XM, 1) + 3) = EXPLOAN(YM, 1) Then
					GoTo 1320
				End If
1100: GoTo 1070
1110: 'CHECK IF CAPEX CATEGORY DEFINED IN EXP OR DEV
1120: YM = 0
1130: YM = YM + 1
1140: If YM > EXPLT Then GoTo 1230
1150: If my3(XM, 1) <= 3 Or my3(XM, 1) >= 15 Then GoTo 1230
1160: If my3(XM, 1) >= 9 And my3(XM, 1) <= 14 Then GoTo 1200
1170: 'CAPEX IS EXPLORATION - SEE IF EXPLORATION IS DEFINED
1180: If EXPLOAN(YM, 1) = 2 Then GoTo 1320
1190: GoTo 1130
1200: 'CAPEX IS DEVELOPMENT - SEE IF DEVELOPMENT IS DEFINED
1210: If EXPLOAN(YM, 1) = 3 Then GoTo 1320
1220: GoTo 1130
1230: 'CHECK IF ALL DEFINED
1240: YM = 0
1250: YM = YM + 1
1260: If YM > EXPLT Then
					GoTo 1920
1265: End If
1270: If EXPLOAN(YM, 1) = 1 Then GoTo 1320
1280: GoTo 1250
1285: 
1320: ' Found a matching line in Capex
				
1325: '<<<<<< 4 Aug 2001 JWD (C0363)
				If my3(XM, 1) < CPXCategoryCodeBAL Or my3(XM, 1) = CPXCategoryCode_AbandonmentCashExpenditure Then ' if BAL then skip the loan section
					GoTo 1910
				End If
				'~~~~~~ was:
				''<<<<<< 21 Jun 2001 JWD (C0339)
				'If my3(XM, 1) < CPXCategoryCodeBAL Then        ' if BAL then skip the loan section
				''~~~~~~ was:
				''If my3(XM, 1) = 18 Then        ' if BAL then skip the loan section
				''>>>>>> End (C0339)
				'  GoTo 1910
				'End If
				'>>>>>> End (C0363)
				
1335: Fed = "N"
				YLO = 0
1340: YLO = my3(XM, 3) 'assigning the year
1344: 'check finance end date
1350: If EXPLOAN(YM, 2) = 1 Then 'BEG
1360: MLO = 0
1370: YLO = 0
1380: ElseIf EXPLOAN(YM, 2) = 2 Then  'DIS
1390: If YLO > Y2 Then
1400: MLO = 0
1410: YLO = 0
1420: ElseIf YLO = Y2 And my3(XM, 2) > M2 Then 
1430: MLO = 0
1440: YLO = 0
1450: Else
1455: Fed = "Y"
1460: MLO = my3(XM, 2)
1480: End If
1490: ElseIf EXPLOAN(YM, 2) = 3 Then  'PRD
1495: If YLO > Y1 Then
1500: MLO = 0
1510: YLO = 0
1520: ElseIf YLO = Y1 And my3(XM, 2) > M1 Then 
1530: MLO = 0
1540: YLO = 0
1550: Else
1555: Fed = "Y"
1560: MLO = my3(XM, 2)
1580: End If
1590: ElseIf EXPLOAN(YM, 2) = 4 Then  'LIF         ''''said 4 OK
1595: Fed = "Y"
1600: MLO = my3(XM, 2)
1620: End If
1630: 
1635: 'check delay
1640: If Fed = "Y" Then
1650: PRIN = (EXPLOAN(YM, 3) / 100) * my3(XM, 5) * GPRATE(XM) * WINC(XM)
1660: If EXPLOAN(YM, 4) = -989 Then 'DIS
1670: TempYr = (Y2 - YLO) * 12
1680: TempMo = M2 - MLO
1690: delay = TempYr + TempMo
1700: ElseIf EXPLOAN(YM, 4) = -987 Then  'PRD
1710: TempYr = (Y1 - YLO) * 12
1720: TempMo = M1 - MLO
1730: delay = TempYr + TempMo
1740: Else
1750: delay = EXPLOAN(YM, 4)
1760: End If
					'If delay < 0 set delay = 0. This occurs when loan
					'  repayment delay says PRD or DIS and the loan occurs
					'  after that date (i.e. capital spent after prod start. In
					'  prior versions of GIANT, this resulted in loan repayment
					'  beginning BEFORE the loan receipt.  This would also happen
					'  if user entered a negative number in delay column.
					If delay < 0 Then
						delay = 0
					End If
					
1785: Call LoanCalc(YR, LG, MLO, YLO, PRIN, EXPLOAN(YM, 6), EXPLOAN(YM, 7), delay, EXPLOAN(YM, 5), EXPLOAN(YM, 8), L, LO, LT)
1790: 
1800: 'summing the results
1810: For N = 1 To LG
1820: LOAN(N) = LOAN(N) + LO(N)
1830: For j = 1 To 5
1840: lot(N, j) = lot(N, j) + LT(N, j)
1850: Next j
1860: Next N
1870: 'erase array for next calculation
					
					
					
1880: 
					Mos = (LG + 2) * 12
					ReDim L(Mos, 4)
					ReDim LO(LG)
					ReDim LT(LG, 5) 'REDIM L(200, 4), LO(LG), LT(LG, 5)
					
1881: MLO = 0 'reset month, year, principal, delay
1882: YLO = 0
1883: PRIN = 0
1884: delay = 0
1890: 
1900: End If
1910: 
1920: Next XM
1930: 
1940: End If
		
		For x = 1 To LG
			lot(x, 5) = LOAN(x) - lot(x, 4)
		Next x
		
		For x = 1 To LG
			PRINC(x) = lot(x, 2)
			INTRST(x) = lot(x, 3)
			FINANCE(x) = lot(x, 5)
		Next x
		
        Dim oPg1 As IGiantRptPageAssignStd
		If RF(5) = "ALL" Or RF(5) = "VAR" Then 'print loan schedule
			ColumnNm(1) = CSng("LOANAMT")
			ColumnNm(2) = CSng("LOANBAL")
			ColumnNm(3) = CSng("PRINCIP")
			ColumnNm(4) = CSng("INTPAYM")
			ColumnNm(5) = CSng("TOTPAYM")
			ColumnNm(6) = CSng("CFEFFEC")
			'Page type, Start Year, Page counter, life of field, number of columns, page title, column length
			'''       WRITE #5, 14, YR, 0, LG, 6, "LOAN SCHEDULE", 10, WINT, PRTA, sCur
			''''Write #5, 14, YR, 0, LG, 6, "LOAN SCHEDULE", 10, FinalWin, FINALPARTIC, sCur
			
			
			
			''''Write #5, ColumnNm$(1), ColumnNm$(2), ColumnNm$(3), ColumnNm$(4), ColumnNm$(5), ColumnNm$(6)
			oPg1 = g_oReport.NewStandardRptPage
			oPg1.SetPageHeader(14, YR, 0, LG, 6, "LOAN SCHEDULE", 10, FinalWin, FINALPARTIC, sCur)
			oPg1.SetProfileHeaders(ColumnNm(1), ColumnNm(2), ColumnNm(3), ColumnNm(4), ColumnNm(5), ColumnNm(6))
			For x = 1 To LG
				''''Write #5, LOAN(x), lot(x, 1), lot(x, 2), lot(x, 3), lot(x, 4), lot(x, 5)
				oPg1.SetProfileValues(x, LOAN(x), lot(x, 1), lot(x, 2), lot(x, 3), lot(x, 4), lot(x, 5))
			Next x
		End If
		
	End Sub
	
	' $subtitle: 'LoanCalc'
	' $Page:
    Sub LoanCalc(ByRef YR As Single, ByRef LG As Short, ByRef MLO As Single, ByRef YLO As Single, ByRef PRIN As Single, ByRef EFFINTR As Single, ByRef PERIOD As Single, ByRef delay As Single, ByRef METHOD As Single, ByRef ACCRUE As Single, ByRef L(,) As Single, ByRef LO() As Single, ByRef LT(,) As Single)
        Dim y As Single
        Dim YRS As Single
        Dim MNTH As Single
        Dim MOLN As Single
        Dim SUM4 As Single
        Dim SUM3 As Single
        Dim SUM2 As Single
        Dim SUM1 As Single
        Dim PRINPMT As Single
        Dim x As Single
        Dim INTR As Single
        Dim DUM As Single '''STATIC
        '----------------------------------------------------------------------------
        ' MLO = MONTH OF LOAN RECEIPT
        ' YLO = YEAR OF LOAN RECEIPT
        ' PRIN =  LOAN AMOUNT
        ' EFFINTR = EFFECTIVE ANNUAL INTEREST RATE (%)
        ' PERIOD = LOAN REPAYMENT PERIOD (MONTHS)
        ' DELAY =  # MONTHS DELAY FROM RECEIPT TO REPAY
        ' METHOD   1 = TOT, 2 = PRI
        ' ACCRUE   2 = YES, 1 = NO
        ' INTR =  NOMINAL MONTHLY INTEREST RATE FRACTION
        ' CONVERT EFFECTIVE INTEREST TO NOMINAL INTEREST
        '---------------------------------------------------------
        ' Modifications:
        ' 20 Feb 1996 JWD
        '           Replaced variable PMT with rTotalPmt.  PMT is
        '        a function name in VB.
        '---------------------------------------------------------
        Dim rTotalPmt As Single ' total monthly payment
        '---------------------------------------------------------
30000:

        DUM = EFFINTR / 100
        INTR = System.Math.Exp(System.Math.Log(DUM + 1) / 12) - 1

        ' GET BEGINNING LOAN BALANCE AT END OF MONTH BEFORE FIRST PAYMENT

        L(0, 1) = PRIN
        ' INFLATE PRINCIPAL BALANCE IF ACCRUE IS YES
        If ACCRUE = 2 Then L(0, 1) = L(0, 1) * ((1 + INTR) ^ delay)
        If METHOD = 1 Then
            '  THIS SECTION DOES EQUAL PAYMENTS TO PRINCIPAL
            ' Calculate total monthly payment
            If INTR <> 0 And PERIOD <> 0 Then
                rTotalPmt = L(0, 1) * (INTR / (1 - ((1 / (1 + INTR)) ^ PERIOD)))
            End If
            For x = 1 To PERIOD 'Calculate interest
                L(x, 3) = L(x - 1, 1) * INTR
                '   Principal is rTotalPmt - Interest
                L(x, 2) = rTotalPmt - L(x, 3)
                '   Ending Principal Balance
                L(x, 1) = L(x - 1, 1) - L(x, 2)
                '   Total Payment
                L(x, 4) = rTotalPmt
            Next x
        Else
            ' THIS SECTION DOES EQUAL PAYMENTS TO PRINCIPAL
            If PERIOD <> 0 Then
                PRINPMT = L(0, 1) / PERIOD
            End If
            For x = 1 To PERIOD
                '  Interest Payments
                L(x, 3) = L(x - 1, 1) * INTR
                '  Principal Payments
                L(x, 2) = PRINPMT
                '  Total Payment
                L(x, 4) = L(x, 2) + L(x, 3)
                '  Ending Principal Balance
                L(x, 1) = L(x - 1, 1) - L(x, 2)
            Next x
        End If
        SUM1 = 0
        SUM2 = 0
        SUM3 = 0
        SUM4 = 0

        For x = 0 To PERIOD
            SUM1 = SUM1 + L(x, 1)
            SUM2 = SUM2 + L(x, 2)
            SUM3 = SUM3 + L(x, 3)
            SUM4 = SUM4 + L(x, 4)
        Next x
        'ACCUMULATE IN YEARS
        LO(YLO - YR + 1) = LO(YLO - YR + 1) + PRIN
        'MOLN = # OF MONTHS FROM 1ST MONTH OF PROJECT START YEAR TO LOAN RECEIPT DATE
        MOLN = ((YLO - YR) * 12) + MLO - 1
        'Add delay period (months)
        MOLN = MOLN + delay + 1
        'Loop through monthly payments and convert to years
        For x = 1 To PERIOD
            MNTH = MOLN + x
            YRS = Int((MNTH - 1) / 12) + 1
            If YRS <= LG Then
                LT(YRS, 1) = L(x, 1)
                For y = 2 To 4
                    LT(YRS, y) = LT(YRS, y) + L(x, y)
                Next y
            Else
                For y = 2 To 4
                    LT(LG, y) = LT(LG, y) + L(x, y)
                Next y
            End If
        Next x

    End Sub
	' Added GDP 14/11/2000
	Sub ConsolidateLoan()
		Dim L As Single
		If g_bPTCons Then
			' Set TOTFINANCE to variable from the RVS file
			' Note that at this point FINANCE contains the loan finance column for cashflow
			ReDim FINANCE(LG)
			For L = 1 To LG
				FINANCE(L) = TOTFINANCE(L)
			Next L
		End If
	End Sub
End Module