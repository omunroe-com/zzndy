Option Strict Off
Option Explicit On
Module UDTDEF
	'---------------------------------------------------------
	' Modifications:
	' 7 May 2001 JWD
	'  -> Change definition of deduct columns to permit more
	'     than the current 3 characters. This to support
	'     numeric entries that might be longer than three
	'     characters and would otherwise be truncated. (C0305)
	'---------------------------------------------------------
	
	'record type for DEFScreen (Fiscal Definition)
	Structure DEFType
        Public var As String 'variable
        Public fld As String 'field type
        Public str_Renamed As String 'currency and stream
        Public csh As String 'cash flow
        Public inc1 As String 'income 1
        Public inc2 As String 'income 2
        Public PRC As String 'price
		'    ded1 As String * 3                 'deduction 1
		'    ded2 As String * 3                 'deduction 2
		'    ded3 As String * 3                 'deduction 3
		'    ded4 As String * 3                 'deduction 4
		'    ded5 As String * 3                 'deduction 5
		Dim ded1 As String 'deduction 1
		Dim ded2 As String 'deduction 2
		Dim ded3 As String 'deduction 3
		Dim ded4 As String 'deduction 4
		Dim ded5 As String 'deduction 5
        Public crd1 As String 'credit variable 1
        Public crd2 As String 'credit variable 2
        Public cal1 As String 'calc variable 1
        Public cal2 As String 'calc variable 2
        Public fnc As String 'function for calc
        Public cde As String 'code
	End Structure
End Module