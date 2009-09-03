Option Strict Off
Option Explicit On
Module MCapExp1
	' Name:        MCapExp1.bas
	' Function:    Capital Expenditures Routines
	'---------------------------------------------------------
	' Module contains miscellaneous routines for the capital
	' expenditures array MY3().
	'---------------------------------------------------------
	' 25 Jun 2001 JWD New (C0341)
	'---------------------------------------------------------
	' Modifications:
	' 2 Aug 2001 JWD
	'  -> Change AddCapitalExpenditure(). (C0366)
	'
	' 23 Apr 2003 JWD
	'  -> Change AddCapitalExpenditure(). (C0689)
	'---------------------------------------------------------
	
	'
	' Modifications:
	' 2 Aug 2001 JWD
	'  -> Change MY3TT to MY3T as the new entry index for the
	'     MY3() array. At the time that this routine is called
	'     to add the Abandonment entries to the array, MY3TT
	'     has not been set yet (it is not done until
	'     CalculateBonus). As a consequence, the abandonment
	'     entries made would overlay the existing entries at
	'     the lower end of the MY3 array. This change was
	'     necessitated by the move of the call to
	'     ApplyAbandonmentFundingProvisions to precede the
	'     call to CalculateBonus in Main as provided for by
	'     C0354. (C0366)
	'
	' 23 Apr 2003 JWD
	'  -> Change subscript referencing the A() array working
	'     interest values to use symbolic constant rather than
	'     literal numeral. Additions to A() array positions
	'     caused shift of working interest and failure to
	'     update this reference caused incorrect results.
	'     (C0689)
	'
	' 25 Jun 2001 JWD New (C0341)
	'
	' Add the expenditure to the MY3() array.
	'
	Public Sub AddCapitalExpenditure(ByVal ExpenditureCode As Short, ByVal ExpenditureMonth As Short, ByVal ExpenditureYear As Short, ByVal TangiblePercentage As Single, ByVal GrossExpenditureAmount As Single, ByVal ApplicableWorkingInterest As Single, ByVal ExpenditureReimbursementPercentage As Single)
		
		'<<<<<< 2 Aug 2001 JWD (C0366)
		' Add an entry for the capital expenditure
		
		Dim the_entry As Short
		
		MY3T = MY3T + 1
		the_entry = MY3T
		
		my3(the_entry, 1) = ExpenditureCode
		my3(the_entry, 2) = ExpenditureMonth
		my3(the_entry, 3) = ExpenditureYear
		my3(the_entry, 4) = TangiblePercentage
		my3(the_entry, 5) = GrossExpenditureAmount
		
		If ApplicableWorkingInterest = -998 Then ' WIN code
			' Retrieve the actual working interest
			' from the forecast array A().
			' 23 Apr 2003 JWD Change next one.
			my3(the_entry, 6) = A(ExpenditureYear - YR + 1, gc_nAWIN) ' was:  6)
		Else
			my3(the_entry, 6) = ApplicableWorkingInterest
		End If
		
		my3(the_entry, 7) = ExpenditureReimbursementPercentage
		'~~~~~~ was:
		'' Add an entry for the capital expenditure
		'my3tt = my3tt + 1
		'
		'my3(my3tt, 1) = ExpenditureCode
		'my3(my3tt, 2) = ExpenditureMonth
		'my3(my3tt, 3) = ExpenditureYear
		'my3(my3tt, 4) = TangiblePercentage
		'my3(my3tt, 5) = GrossExpenditureAmount
		'
		'If ApplicableWorkingInterest = -998 Then   ' WIN code
		'   ' Retrieve the actual working interest
		'   ' from the forecast array A().
		'   my3(my3tt, 6) = A((ExpenditureYear - YR + 1), 6)
		'Else
		'   my3(my3tt, 6) = ApplicableWorkingInterest
		'End If
		'
		'my3(my3tt, 7) = ExpenditureReimbursementPercentage
		'>>>>>> End (C0366)
		
	End Sub
	
	
	'
	' 25 Jun 2001 JWD New (C0341)
	'
	' Put the capital expenditures in order by
	' expenditure date, ascending.
	'
	' This routine is a copy of the sort routine
	' in BONUS01A.BAS:CalculateBonus()
	'
	Public Sub OrderCapitalExpendituresByDate()
		
		Dim i As Short
		Dim iRow As Short
		Dim j As Short
		Dim rTmp As Single
		
		Dim bSORTED As Boolean
		Dim bSWAPPING As Boolean
		
		'sort the MY3() (CAPEX) by date
		For i = 2 To 3
			bSORTED = False
			While Not bSORTED
				bSWAPPING = False
				iRow = 1
				While iRow < my3tt ' Number of records
					If my3(iRow, i) > my3(iRow + 1, i) Then
						For j = 1 To 7
							rTmp = my3(iRow + 1, j)
							my3(iRow + 1, j) = my3(iRow, j)
							my3(iRow, j) = rTmp
						Next j
						bSWAPPING = True
					End If
					iRow = iRow + 1
				End While
				If Not bSWAPPING Then
					bSORTED = True
				End If
			End While
		Next i
		
	End Sub
End Module