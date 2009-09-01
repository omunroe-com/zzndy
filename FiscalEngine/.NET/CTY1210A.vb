Option Strict Off
Option Explicit On
Module CTY1210A
	' Name:        CTY1210A.BAS
	' Function:    Country File Load/Store Support Procedures
	'---------------------------------------------------------
	' This module contains procedures for loading and storing
	' country file data from and to disk files.
	'---------------------------------------------------------
	
	'---------------------------------------------------------
	Sub GetVariableTitles(ByVal fno As Short, ByVal intCount As Short, ByRef VTTL() As TTLType)
		'---------------------------------------------------------
		' Get the Variable Titles (TTLScreen) from the disk file
		' specified, store in the array VTTL().
		'---------------------------------------------------------
		Dim i As Short
		'---------------------------------------------------------
		
		For i = 1 To intCount
			Input(fno, VTTL(i).var)
			Input(fno, VTTL(i).short_Renamed)
			Input(fno, VTTL(i).long_Renamed)
		Next i
		
	End Sub
	
	'---------------------------------------------------------
	Sub PutVariableTitles(ByVal fno As Short, ByVal intCount As Short, ByRef VTTL() As TTLType)
		'---------------------------------------------------------
		' Put the Variable Titles (TTLScreen) to the disk file
		' specified from the array VTTL().
		'---------------------------------------------------------
		Dim i As Short
		'---------------------------------------------------------
		
		For i = 1 To intCount
			WriteLine(fno, VTTL(i).var, VTTL(i).short_Renamed, VTTL(i).long_Renamed)
		Next i
		
	End Sub
End Module