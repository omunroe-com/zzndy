Attribute VB_Name = "PGM9010A"
'name:         PGM9010A.BAS
'function:     Program Status File
'date:         28 Apr 1995 JWD
'---------------------------------------------------------
' Reads and writes program status information to disk file.
'---------------------------------------------------------
' NOTE:  This module is used by DOS AND WINDOWS apps. Do
'        not use VB specific code or reserved words.
'---------------------------------------------------------
'$INCLUDE:     'pgm9010.bi'

' Modifications:
'   GDP 17 Dec 2002
'      -> Added new function WriteErrorLog().
Sub GetProgramStatus (fStatus$, lStat As Long, sPgm As String, lLine As Long)
'-----------------------------------------------------------------------
' Get Program Status from disk file fStatus$.
'-----------------------------------------------------------------------
Dim hfErr As Integer
Dim sTmp  As String * 8
'-----------------------------------------------------------------------

hfErr = FreeFile
Open fStatus$ For Binary As #hfErr
   Get #hfErr, , lStat
   Get #hfErr, , sTmp
   Get #hfErr, , lLine
Close #hfErr

sPgm = sTmp

End Sub

Sub PutProgramStatus (fStatus$, lStat As Long, sPgm As String, lLine As Long)
'---------------------------------------------------------
' Put Program Status in disk file fStatus$.
'---------------------------------------------------------
Dim hfErr As Integer
Dim sTmp  As String * 8
'---------------------------------------------------------

sTmp = sPgm

hfErr = FreeFile
Open fStatus$ For Binary As #hfErr
   Put #hfErr, , lStat
   Put #hfErr, , sTmp
   Put #hfErr, , lLine
Close #hfErr

End Sub

' Added GDP 17 Dec 2002
' This function was added so MAINEXEC can write out a text file called
' ERROR.LOG, to the temporary directory. This file contains a textual
' error description. This was added the same time as excel
' links so an error message infroming the user which file doesn't exist
' could be displayed in the calling program.
Public Sub WriteErrorLog(ByVal sErrorLog As String)
    Dim hFile As String
    hFile = FreeFile
    Open TempDir$ & "ERROR.LOG" For Output As hFile
    Write #hFile, sErrorLog
    Close #hFile
End Sub
