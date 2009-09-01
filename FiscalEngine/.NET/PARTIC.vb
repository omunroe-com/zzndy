Option Strict Off
Option Explicit On
Module PARTIC
	' $linesize: 132
	' $title:    'GIANT v6.1 - 1996                         PARTIC.BAS'
	' $subtitle: 'Government Participation'
	' Name:        Partic.bas
	' Function:    Government Participation Calculation
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
	' Modifications:
	' 3 Mar 1995 JWD
	'          Converted module level executable code to subroutine.
	' 8 Feb 1996 JWD
	'          Changed Participation().
	'          Replaced include file SUBINCL.BAS with UTIL5.BI.
	'          Add interface declaration include file PARTIC.BI.
	'          Replaced include file CTYIN.BAS with CTYIN1.BG.
	'          Add explicit declaration of default storage class as Single.
	' 19 Feb 1996 JWD
	'          Changed references to common array RATE() to PARTRATE().
	'       RATE is reserved function name in VB.
	'
	' 21 Jun 2001 JWD
	'  -> Changed Participation(). (C0339)
	'
	' 4 Aug 2001 JWD
	'  -> Changed Participation(). (C0363)
	'
	' 8 Mar 2002 JWD
	'  -> Changed Participation(). (C0496)
	'
	' 12 Jan 2004 JWD
	'  -> Changed Participation(). (C0772)
	'
	' 19 Jan 2004 JWD
	'  -> Changed Participation(). (C0774)
	'
	' 3 Feb 2004 JWD
	'  -> Changed Participation(). (C0776)
	'-----------------------------------------------------------------------
	' $dynamic
	'UPGRADE_NOTE: DefSng A-Z statement was removed. Variables were explicitly declared as type Single. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="92AFD3E3-440D-4D49-A8BF-580D74A8C9F2"'
	'$include: 'ctyin1.bg'
	'$include: 'partic.bi'
	'$include: 'util5.bi'
	
	'-----------------------------------------------------------------------
	'$subtitle: 'Procedure: Participation'
	'$page
	Sub Participation()
		Dim y9 As Single
		Dim m9 As Single
		Dim param As Short
		Dim DefAmount As Single
		Dim VarRates As Object
		Dim ratein As Object
		Dim sRateInV As Object
		Dim ck As String
		'-----------------------------------------------------------------------
		' This program calculates government participation effects.
		'-----------------------------------------------------------------------
		' Modifications:
		' 8 Feb 1996 JWD
		'          Removed include of SCRA1IN.BAS and SCRA1OUT.BAS.  The data
		'       variables read and written to the scratch file SCRA1.SCR are not
		'       referenced in this subroutine. Removed ReDims of array variables
		'       written on SCRA1.SCR.
		' 20 Feb 1996 JWD
		'        Change CUR$ to sCur, duplicate definition (CUR()).
		'        Change PR$ to sPRV, duplicate definition (PR()).
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
		' 8 Mar 2002 JWD
		'  -> Changed test assigning the group expenditure
		'     category for capital expenditures to include the
		'     new capital categories CP4-9, ABN, and AB1. This
		'     change should have been made at the time the
		'     new categories were added, but was missed. (C0496)
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
		Dim i As Short
		Dim iPX As Short
		Dim iSCT As Short
		Dim iX As Short
		Dim iXZZ As Short
		Dim iY As Short
		Dim iZ As Short
		Dim j As Short
		
		Dim iZoo As Short
		Dim Numvar As Short
		Dim Ratetot As Short
		'-----------------------------------------------------------------------
16: Dim CX(LG, 5) As Object
		Dim STM(LG) As Object
		Dim ColumnNm(15) As Object
		ReDim TOTPMT(LG)
		
		' ON ERROR GOTO 25000
		
		' **********************************************************************
		' this section calculates PARTRATE(lg) and OPEXRATE(lg)
		' **********************************************************************
		
10010: ReDim PARTRATE(LG)
		ReDim OPEXRATE(LG)
		
10020: ' CHECK IF PARTICIPATION RATES ARE ENTERED IN ANNUAL VARIABLES
		ck = "N"
		For iX = 1 To LG
			If B(iX, 6) <> 0 Then
				ck = "Y"
			End If
		Next iX
		
		If ck = "Y" Then ' SET PARTRATE(iX) = B(iX,6)
			
			For iX = 1 To LG
				PARTRATE(iX) = B(iX, 6) / 100
				OPEXRATE(iX) = PARTRATE(iX)
			Next iX
			
		Else
			
			If PRT > 0 Then
				
				' we loop through the first time to set PARTRATE() and OPEXRATE()
				' we loop through a second time to see if OPEXRATE() will be changed
				' by a POX entry in the Category column
				
				Ratetot = PRT
				For iZoo = 1 To 2
					'UPGRADE_ISSUE: As String was removed from ReDim sRateInV(Ratetot) statement. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="19AFCB41-AA8E-4E6B-A441-A3E802E5FD64"'
					ReDim sRateInV(Ratetot)
					ReDim ratein(Ratetot, 6)
					ReDim VarRates(LG)
					For iPX = 1 To Ratetot
						'UPGRADE_WARNING: Couldn't resolve default property of object sRateInV(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						sRateInV(iPX) = sPRV(iPX)
						'UPGRADE_WARNING: Couldn't resolve default property of object ratein(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						ratein(iPX, 1) = PR(iPX, 1)
						'UPGRADE_WARNING: Couldn't resolve default property of object ratein(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						ratein(iPX, 2) = PR(iPX, 2)
						'UPGRADE_WARNING: Couldn't resolve default property of object ratein(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						ratein(iPX, 3) = PR(iPX, 3)
						'UPGRADE_WARNING: Couldn't resolve default property of object ratein(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						ratein(iPX, 4) = PR(iPX, 4)
						'UPGRADE_WARNING: Couldn't resolve default property of object ratein(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						ratein(iPX, 5) = PR(iPX, 5)
						'UPGRADE_WARNING: Couldn't resolve default property of object ratein(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						ratein(iPX, 6) = PR(iPX, 6)
					Next iPX
					
					'DefAmount is the amount returned if no match is found
					If iZoo = 1 Then
						DefAmount = 0
						'UPGRADE_WARNING: Couldn't resolve default property of object VarRates(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						'UPGRADE_WARNING: Couldn't resolve default property of object ratein(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						'UPGRADE_WARNING: Couldn't resolve default property of object sRateInV(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						RateCalc(Numvar, "PARTIC", "PAR", DefAmount, sRateInV(), ratein(), Ratetot, param, VarRates())
						For j = 1 To LG
							'UPGRADE_WARNING: Couldn't resolve default property of object VarRates(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
							PARTRATE(j) = VarRates(j) / 100
							'UPGRADE_WARNING: Couldn't resolve default property of object VarRates(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
							OPEXRATE(j) = VarRates(j) / 100
						Next j
					ElseIf iZoo = 2 Then 
						DefAmount = -999
						'UPGRADE_WARNING: Couldn't resolve default property of object VarRates(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						'UPGRADE_WARNING: Couldn't resolve default property of object ratein(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						'UPGRADE_WARNING: Couldn't resolve default property of object sRateInV(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						RateCalc(Numvar, "PARTIC", "POX", DefAmount, sRateInV(), ratein(), Ratetot, param, VarRates())
						For j = 1 To LG
							'UPGRADE_WARNING: Couldn't resolve default property of object VarRates(j). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
							If VarRates(j) <> -999 Then
								'UPGRADE_WARNING: Couldn't resolve default property of object VarRates(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
								OPEXRATE(j) = VarRates(j) / 100
							End If
						Next j
					End If
				Next iZoo
			End If
		End If
		'--------------------------------------------------
		'8-21-92 store final partic rate in FINALPARTIC for report display
		FINALPARTIC = PARTRATE(LG)
		'--------------------------------------------------
		
		' *********************************************************************
		' this section sets GPBASE(My3tt) and GPRATE(My3tt)
		' *********************************************************************
		
		'GPRATE - money actually spent by company
		'GPBASE - company basis (depreciable basis (1 = 100%))
		
		
		
		
		ReDim GPBASE(my3tt)
		ReDim GPRATE(my3tt)
		Dim nSetPartRate As Short
		If my3tt > 0 Then
			For iX = 1 To my3tt 'loop ends at 12390
				GPRATE(iX) = 1 'start every capex with no gov't part.
				GPBASE(iX) = 1
				If PTT > 0 Then
					'LPRINT " "
					'LPRINT "iX = "; iX; TAB(10); "MY3("; iX; ", 1) = "; MY3(iX, 1)
					
					'<<<<<< 4 Aug 2001 JWD (C0363)
					If my3(iX, 1) < CPXCategoryCodeBAL Or my3(iX, 1) = CPXCategoryCode_AbandonmentCashExpenditure Then
						'~~~~~~ was:
						''<<<<<< 21 Jun 2001 JWD (C0339)
						'If my3(iX, 1) < CPXCategoryCodeBAL Then                 'BAL
						''~~~~~~ was:
						''If my3(iX, 1) < 18 Then                 'BAL
						''>>>>>> End (C0339)
						'>>>>>> End (C0363)
						
						'LPRINT "category <> 18"
						'this section looks for the matching line in PT()
						iY = 0 ' check if category defined explicitly
						For j = 1 To PTT
							If (my3(iX, 1) + 3) = PT(j, 1) Then
								iY = j ' iY = line number in PT()
								Exit For
							End If
						Next j
						
						If iY = 0 Then
							If my3(iX, 1) >= 4 And my3(iX, 1) <= 8 Then 'check if EXP defined
								For j = 1 To PTT
									If PT(j, 1) = 2 Then
										iY = j
										Exit For
									End If
								Next j
							ElseIf my3(iX, 1) >= 9 And my3(iX, 1) <= 14 Then  'check if DEV defined
								For j = 1 To PTT
									If PT(j, 1) = 3 Then
										iY = j
										Exit For
									End If
								Next j
							End If
						End If
						
						If iY = 0 Then 'CHECK IF ALL DEFINED
							For j = 1 To PTT
								If PT(j, 1) = 1 Then 'CPX
									iY = j
									Exit For
								End If
							Next j
						End If
						
						If iY > 0 Then 'we have found a matching line
							
							'determine start date of participation
							If PT(iY, 2) = -985 Then ' beg
								m9 = mo : y9 = YR
							ElseIf PT(iY, 2) = -989 Then  ' dsc
								m9 = M2 : y9 = Y2
							ElseIf PT(iY, 2) = -987 Then  ' prd
								m9 = M1 : y9 = Y1
							ElseIf PT(iY, 2) = -986 Then  ' non
								m9 = 12 : y9 = 3000
							End If
							
							'now set part. rate for capex on or after the Start Date
							If (my3(iX, 3) > y9) Or (my3(iX, 3) = y9 And my3(iX, 2) >= m9) Then
								If PT(iY, 3) = -996 Then ' PAR was entered under Rate column
									iSCT = my3(iX, 3) - YR + 1
									GPRATE(iX) = 1 - PARTRATE(iSCT)
								Else ' a rate % was entered
									GPRATE(iX) = (100 - PT(iY, 3)) / 100
									
									If FINALPARTIC = 0 Then
										FINALPARTIC = PT(iY, 3) / 100
										
										
										For nSetPartRate = 1 To LG
											PARTRATE(nSetPartRate) = FINALPARTIC
											OPEXRATE(nSetPartRate) = FINALPARTIC
										Next nSetPartRate
									End If
									
									'PARTRATE(iSCT) = PT(iY, 3) / 100
								End If
							End If
						End If
						
						
						' ************************************************************************
						
						'SET GROUP PARTICIPATION BASE
						GPBASE(iX) = GPRATE(iX)
						
						If PT(iY, 4) <> -982 Then 'skip if NOR entered
							'keep going only for capex earlier than Start Date
							If my3(iX, 3) < y9 Or (my3(iX, 3) = y9 And my3(iX, 2) < m9) Then
								If PT(iY, 3) = -996 Then ' PAR was entered under Rate column
									iSCT = my3(iX, 3) - YR + 1
									GPBASE(iX) = (GPRATE(iX) - PARTRATE(iSCT)) '''''* (PT(iY, 5) / 100)
								Else ' a Rate % was entered
									GPBASE(iX) = (GPRATE(iX) - (PT(iY, 3) / 100)) '''''* (PT(iY, 5) / 100)
								End If
							End If
						End If
						
						' Figure the recoverable/depreciable share net of NOC
						' since NOC may recover/depreciate at a rate different
						' from the participation rate in the capital item
						' (ie NOC may recover/depreciate more or less than paid)
						If iY > 0 Then
							If PT(iY, 9) = -996 Then ' PAR was entered under dpcrRate column
								iSCT = my3(iX, 3) - YR + 1
								GPDPCR(iX) = 1 - PARTRATE(iSCT)
							Else ' a dpcrRate % was entered
								GPDPCR(iX) = (100 - PT(iY, 9)) / 100
							End If
						End If
					End If
				End If
			Next iX
		End If
		
		
		'PRINT "-------------------------------------------------------"
		'FOR q = 1 TO MY3TT
		'PRINT "GPBASE("; q; ") = "; GPBASE(q), "GPRATE = "; GPRATE(q), "REIM = "; REIM(q), WINC(q)
		'NEXT q
		
		'PUT CAPITAL EXPENDITURES AND REPAYMENTS IN CATEGORIES
		'LPRINT " "
		'LPRINT "assign capex to categories 1-4"
		If my3tt > 0 Then
			
			For iX = 1 To my3tt
				
				'<<<<<< 4 Aug 2001 JWD (C0363)
				If my3(iX, 1) < CPXCategoryCodeBAL Or my3(iX, 1) = CPXCategoryCode_AbandonmentCashExpenditure Then
					'~~~~~~ was:
					''<<<<<< 21 Jun 2001 JWD (C0339)
					'If my3(iX, 1) < CPXCategoryCodeBAL Then                 'BAL
					''~~~~~~ was:
					''If my3(iX, 1) < 18 Then                 'BAL
					''>>>>>> End (C0339)
					'>>>>>> End (C0363)
					
					iZ = my3(iX, 3) - YR + 1 'relative project year
					If my3(iX, 1) >= 1 And my3(iX, 1) <= 3 Then
						iXZZ = 1
					ElseIf my3(iX, 1) >= 4 And my3(iX, 1) <= 8 Then 
						iXZZ = 2
					ElseIf my3(iX, 1) >= 9 And my3(iX, 1) <= 14 Then 
						iXZZ = 3
						'<<<<<< 8 Mar 2002 JWD (C0496)
					ElseIf my3(iX, 1) >= 15 And my3(iX, 1) < CPXCategoryCodeBAL Or my3(iX, 1) = CPXCategoryCode_AbandonmentCashExpenditure Then 
						'~~~~~~ was:
						'ElseIf my3(iX, 1) >= 15 And my3(iX, 1) <= 17 Then
						'>>>>>> End (C0496)
						iXZZ = 4
					End If
					'UPGRADE_WARNING: Couldn't resolve default property of object CX(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					'UPGRADE_WARNING: Couldn't resolve default property of object CX(iZ, iXZZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					CX(iZ, iXZZ) = CX(iZ, iXZZ) + (my3(iX, 5) * GPRATE(iX)) * (WINC(iX) - REIM(iX))
					'PRINT " capex line "; iX, "Category: "; iXZZ, iZ, cx(iZ, iXZZ)
				End If
			Next iX
		End If
		
		' *************************************************************************
		
		For iZ = 1 To LG
			'UPGRADE_WARNING: Couldn't resolve default property of object CX(iZ, 4). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			'UPGRADE_WARNING: Couldn't resolve default property of object CX(iZ, 3). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			'UPGRADE_WARNING: Couldn't resolve default property of object CX(iZ, 2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			'UPGRADE_WARNING: Couldn't resolve default property of object CX(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			'UPGRADE_WARNING: Couldn't resolve default property of object CX(iZ, 5). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			CX(iZ, 5) = CX(iZ, 1) + CX(iZ, 2) + CX(iZ, 3) + CX(iZ, 4)
		Next iZ
		
        Dim oPg1 As IGiantRptPageAssignStd
		If Left(RF(5), 3) = "ALL" Or Left(RF(5), 3) = "VAR" Then ' PRINT GOVERNMENT PARTICIPATION
			'UPGRADE_WARNING: Couldn't resolve default property of object ColumnNm$(1). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			ColumnNm(1) = " GRPREN"
			'UPGRADE_WARNING: Couldn't resolve default property of object ColumnNm$(2). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			ColumnNm(2) = " GRPEXP"
			'UPGRADE_WARNING: Couldn't resolve default property of object ColumnNm$(3). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			ColumnNm(3) = " GRPDEV"
			'UPGRADE_WARNING: Couldn't resolve default property of object ColumnNm$(4). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			ColumnNm(4) = " GRPOTH"
			'UPGRADE_WARNING: Couldn't resolve default property of object ColumnNm$(5). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			ColumnNm(5) = " GRPTOT"
			'Page type, Start year, Page counter, life of field, number of columns, page title, column length
			'''        WRITE #5, 6, YR, 0, LG, 5, "GROUP EXPENDITURES", 10, WINT, PRTA, sCur
			''''Write #5, 6, YR, 0, LG, 5, "GROUP EXPENDITURES", 10, FinalWin, FINALPARTIC, sCur
			
			'OLD version   WRITE #5, 6, yr, 0, LG, 5, "GOVERNMENT PARTICIPATION", 10, WINT, PRTA, sCur
			'columns titles
			''''Write #5, ColumnNm$(1), ColumnNm$(2), ColumnNm$(3), ColumnNm$(4), ColumnNm$(5)
			
			oPg1 = g_oReport.NewStandardRptPage
			oPg1.SetPageHeader(6, YR, 0, LG, 5, "GROUP EXPENDITURES", 10, FinalWin, FINALPARTIC, sCur)
			oPg1.SetProfileHeaders(ColumnNm(1), ColumnNm(2), ColumnNm(3), ColumnNm(4), ColumnNm(5))
			For iX = 1 To LG
				''''Write #5, CX(iX, 1), CX(iX, 2), CX(iX, 3), CX(iX, 4), CX(iX, 5)
				oPg1.SetProfileValues(iX, CX(iX, 1), CX(iX, 2), CX(iX, 3), CX(iX, 4), CX(iX, 5))
			Next iX
12840: End If
		
	End Sub
End Module