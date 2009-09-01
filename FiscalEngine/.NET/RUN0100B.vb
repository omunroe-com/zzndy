Option Strict Off
Option Explicit On
Module RUN0100B
	'name:          RUN0100B.BAS
	'function:      GIANT Run File Routines
	'-----------------------------------------------------------------------
	' Modifications:
	' 13 Feb 1996 JWD
	'          Changed common block include file from CTYIN.BAS to CTYIN1.BG
	'-----------------------------------------------------------------------
	'$DYNAMIC
	'$INCLUDE: 'run0100.bi'
	'$INCLUDE: 'ctyin1.bg'
	
	'
	' Modifications:
	' 9 Feb 2004 JWD
	'  -> Replace explicit file i/o statements with class
	'     method calls for run file. (C0776)
	'
	Sub GetRunFileLine()
		'--------------------------------------------------------------------
		'reads the run file to get the next line
		'  replaces GOSUB 48000 in old giant
		
		Dim i As Short
		
		AR = AR + 1
		With g_oRunFileIn ' Input #3, RF$(1), RF$(2), RF$(3), RF$(4), RF$(5), RF$(6), RF$(7), RF$(8), RF$(9), RF$(10), RF$(11)
			'UPGRADE_WARNING: Couldn't resolve default property of object g_oRunFileIn.NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			RF(1) = .NextItem
			'UPGRADE_WARNING: Couldn't resolve default property of object g_oRunFileIn.NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			RF(2) = .NextItem
			'UPGRADE_WARNING: Couldn't resolve default property of object g_oRunFileIn.NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			RF(3) = .NextItem
			'UPGRADE_WARNING: Couldn't resolve default property of object g_oRunFileIn.NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			RF(4) = .NextItem
			'UPGRADE_WARNING: Couldn't resolve default property of object g_oRunFileIn.NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			RF(5) = .NextItem
			'UPGRADE_WARNING: Couldn't resolve default property of object g_oRunFileIn.NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			RF(6) = .NextItem
			'UPGRADE_WARNING: Couldn't resolve default property of object g_oRunFileIn.NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			RF(7) = .NextItem
			'UPGRADE_WARNING: Couldn't resolve default property of object g_oRunFileIn.NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			RF(8) = .NextItem
			'UPGRADE_WARNING: Couldn't resolve default property of object g_oRunFileIn.NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			RF(9) = .NextItem
			'UPGRADE_WARNING: Couldn't resolve default property of object g_oRunFileIn.NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			RF(10) = .NextItem
			'UPGRADE_WARNING: Couldn't resolve default property of object g_oRunFileIn.NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			RF(11) = .NextItem
		End With
		For i = 1 To 11
			RF(i) = UCase(LTrim(RTrim(RF(i))))
		Next i
	End Sub
End Module