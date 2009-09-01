Option Strict Off
Option Explicit On
Module PGM9010A
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
	Sub GetProgramStatus(ByRef fStatus As String, ByRef lStat As Integer, ByRef sPgm As String, ByRef lLine As Integer)
		'-----------------------------------------------------------------------
		' Get Program Status from disk file fStatus$.
		'-----------------------------------------------------------------------
		Dim hfErr As Short
		Dim sTmp As New VB6.FixedLengthString(8)
		'-----------------------------------------------------------------------
		
		hfErr = FreeFile
		FileOpen(hfErr, fStatus, OpenMode.Binary)
		'UPGRADE_WARNING: Get was upgraded to FileGet and has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
		FileGet(hfErr, lStat)
		'UPGRADE_WARNING: Get was upgraded to FileGet and has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
		FileGet(hfErr, sTmp.Value)
		'UPGRADE_WARNING: Get was upgraded to FileGet and has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
		FileGet(hfErr, lLine)
		FileClose(hfErr)
		
		sPgm = sTmp.Value
		
	End Sub
	
	Sub PutProgramStatus(ByRef fStatus As String, ByRef lStat As Integer, ByRef sPgm As String, ByRef lLine As Integer)
		'---------------------------------------------------------
		' Put Program Status in disk file fStatus$.
		'---------------------------------------------------------
		Dim hfErr As Short
		Dim sTmp As New VB6.FixedLengthString(8)
		'---------------------------------------------------------
		
		sTmp.Value = sPgm
		
		hfErr = FreeFile
		FileOpen(hfErr, fStatus, OpenMode.Binary)
		'UPGRADE_WARNING: Put was upgraded to FilePut and has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
		FilePut(hfErr, lStat)
		'UPGRADE_WARNING: Put was upgraded to FilePut and has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
		FilePut(hfErr, sTmp.Value)
		'UPGRADE_WARNING: Put was upgraded to FilePut and has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
		FilePut(hfErr, lLine)
		FileClose(hfErr)
		
	End Sub
	
	' Added GDP 17 Dec 2002
	' This function was added so MAINEXEC can write out a text file called
	' ERROR.LOG, to the temporary directory. This file contains a textual
	' error description. This was added the same time as excel
	' links so an error message infroming the user which file doesn't exist
	' could be displayed in the calling program.
	Public Sub WriteErrorLog(ByVal sErrorLog As String)
		Dim hFile As String
		hFile = CStr(FreeFile)
		FileOpen(CInt(hFile), TempDir & "ERROR.LOG", OpenMode.Output)
		WriteLine(CInt(hFile), sErrorLog)
		FileClose(CInt(hFile))
	End Sub
End Module