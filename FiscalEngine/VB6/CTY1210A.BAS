' Name:        CTY1210A.BAS
' Function:    Country File Load/Store Support Procedures
'---------------------------------------------------------
' This module contains procedures for loading and storing
' country file data from and to disk files.
'---------------------------------------------------------
Option Explicit

'---------------------------------------------------------
Sub GetVariableTitles (ByVal fno As Integer, ByVal intCount As Integer, VTTL() As TTLType)
'---------------------------------------------------------
' Get the Variable Titles (TTLScreen) from the disk file
' specified, store in the array VTTL().
'---------------------------------------------------------
   Dim i As Integer
'---------------------------------------------------------
   
   For i = 1 To intCount
      Input #fno, VTTL(i).var, VTTL(i).short, VTTL(i).long
   Next i

End Sub

'---------------------------------------------------------
Sub PutVariableTitles (ByVal fno As Integer, ByVal intCount As Integer, VTTL() As TTLType)
'---------------------------------------------------------
' Put the Variable Titles (TTLScreen) to the disk file
' specified from the array VTTL().
'---------------------------------------------------------
   Dim i As Integer
'---------------------------------------------------------
   
   For i = 1 To intCount
      Write #fno, VTTL(i).var, VTTL(i).short, VTTL(i).long
   Next i
   
End Sub

