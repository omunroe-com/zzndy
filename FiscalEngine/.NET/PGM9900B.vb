Option Strict Off
Option Explicit On
Module PGM9900B
	'$linesize: 132
	'$title:    'GIANT v6.1 - 1996                       PGM9900B.BAS'
	'$subtitle: 'Program Initialization/Termination'
	' **********************************************************************
	' *          COPYRIGHT - PETROCONSULTANTS, INC. - 1995, 1996           *
	' *                      ALL RIGHTS RESERVED                           *
	' **********************************************************************
	' *  This program file is proprietary information of Petroconsultants, *
	' *  Incorporated.  Unauthorized use for any purpose is prohibited.    *
	' **********************************************************************
	'name:          PGM9900B.BAS
	'function:      GIANT Program Control
	'-----------------------------------------------------------------------
	' Modifications:
	' 7 Feb 1996 JWD
	'          Replaced explicit declaration of TRUE & FALSE constants with
	'       include file TRUFALSE.BC.
	'          Changed include file CTYIN.BAS to CTYIN1.BG.
	'          Add explicit declaration of default storage class as Single.
	'
	' 17 Dec 2002 GDP
	'   -> Changed TerminateExecution(). Added call to WriteErrorLog and
	'      CleanUpExcel.
	'
	' 9 Feb 2004 JWD
	'   -> Changed TerminateExecution(). (C0780)
	'-----------------------------------------------------------------------
	'$dynamic
	'UPGRADE_NOTE: DefSng A-Z statement was removed. Variables were explicitly declared as type Single. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="92AFD3E3-440D-4D49-A8BF-580D74A8C9F2"'
	'$include: 'trufalse.bc'
	
	'$INCLUDE: 'ctyin1.bg'
	'$INCLUDE: 'pgm9900.bi'
	'$INCLUDE: 'pgm9010.bi'
	
	Sub InitializeExecution(ByRef fEnviron As String)
		'-----------------------------------------------------------------------
		' Start Execution.  fEnviron is complete filespec (drive+path+file) of
		' file containing execution environment.  This file contains paths of
		' application (shared), working, and temporary (scratch) directories,
		' and the complete filespec for the program status file.  This routine
		' changes the current drive/directory to that specified if not null.
		'-----------------------------------------------------------------------
		Dim hEnv As Short
		Dim tmp As String
		
		hEnv = FreeFile
		FileOpen(hEnv, fEnviron, OpenMode.Input)
		' Get Application system directory (shared files)
		Input(hEnv, AppDir)
		' Get working directory (current/default)
		Input(hEnv, tmp)
		' Get temporary directory (scratch files)
		Input(hEnv, TempDir)
		' Get filespec of execution status file
		Input(hEnv, FPStatus)
		FileClose(hEnv)
		
		'   If tmp$ <> "" Then
		'      ' Check for and remove any trailing backslashes ("\")...
		'      If Len(tmp$) > 3 And Right$(tmp$, 1) = "\" Then
		'         tmp$ = Left$(tmp$, Len(tmp$) - 1)
		'      End If
		'      ChDrive tmp$
		'      ChDir tmp$
		'   End If
		
	End Sub
	
	' Modifications:
	' 17 Dec 2002 GDP
	'   -> Added call to WriteErrorLog and CleanUpExcel.
	'
	' 9 Feb 2004 JWD
	'  -> Remove End statement, not permitted for ActiveX DLL
	'     use. (C0780)
	'
	Sub TerminateExecution()
		'-----------------------------------------------------------------------
		' Terminate program execution.  Put program status into status file,
		' close files, clean up and exit to OS.
		'-----------------------------------------------------------------------
		
		PutProgramStatus(FPStatus, Err.Number, Program, Erl())
		' GDP 17 Dec 2002
		
		WriteErrorLog(Err.Description)
		' Get rid of references to Excel.
		CleanUpExcel()
		' End - GDP 17 Dec 2002
		FileClose()
		
		If RN = "SINGLE" Then
			Kill(sRunDir & "SINGLE.RUN")
			
		End If
		
		' KILL "~GNT*.TMP"
		
		' 9 Feb 2004 JWD (C780) Remove End statement
		' End
		
	End Sub
End Module