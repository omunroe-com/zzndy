Option Strict Off
Option Explicit On
Module RUN0200C
	'name:          RUN0200C.BAS
	'function:      GIANT Finder.Prn Routines
	' $title:    'RUN0200C'
	' $subtitle: 'Placeholder'
	'-----------------------------------------------------------------------
	' Modifications:
	' 13 Feb 1996 JWD
	'          Changed common block include file from CTYIN.BAS to CTYIN1.BG
	' 21 Feb 1996 JWD
	'          Changed type of AR to integer
	'-----------------------------------------------------------------------
	'$DYNAMIC
	'$INCLUDE: 'run0200.bi'
	'$INCLUDE: 'ctyin1.bg'
	
	Const FINDER As String = "FINDER.PRN"
	
	Sub ReadFinder(ByRef iLine As Short, ByRef sCameFrom As String)
		Dim rLine As Object
		'--------------------------------------------------------------------
		
		Dim hfFinder As Short
		
		'UPGRADE_WARNING: Couldn't resolve default property of object rLine. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		rLine = 0
		sCameFrom = ""
		
		On Error GoTo ReadHandler
		hfFinder = FreeFile
		FileOpen(hfFinder, TempDir & FINDER, OpenMode.Input)
		Input(hfFinder, iLine)
		Input(hfFinder, sCameFrom)
		FileClose(hfFinder)
		
		Exit Sub
		'-----------------------------------------------------------------------
ReadHandler: 
		If Err.Number = 53 Or Err.Number = 62 Then
			Exit Sub
		Else
			Error(Err.Number)
		End If
	End Sub
	
	Sub WriteFinder(ByRef iLine As Short, ByRef sCameFrom As String)
		'-----------------------------------------------------------------------
		
		Dim hfFinder As Short
		
		hfFinder = FreeFile
		FileOpen(hfFinder, TempDir & FINDER, OpenMode.Output)
		WriteLine(hfFinder, iLine, sCameFrom)
		FileClose(hfFinder)
		
	End Sub
End Module