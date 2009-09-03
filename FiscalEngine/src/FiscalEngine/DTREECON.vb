Option Strict Off
Option Explicit On
Module DTREECON
	
	' WriteDtreeData - write out the data need for the decision tree
	
	Public Sub WriteDtreeData()
		Const c_nVersion As Short = 1
		Dim hFile As Short
		Dim i As Short
		
		'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
		If Len(Dir(TempDir & "DTREE.FLG")) <> 0 Then
			hFile = FreeFile
			FileOpen(hFile, TempDir & "DTREE.PRN", OpenMode.Output)
			
			WriteLine(hFile, "DTREE_DATA")
			WriteLine(hFile, c_nVersion)
			' Write out life
			WriteLine(hFile, LG)
			
			' Write out dates
			WriteLine(hFile, YR, mo)
			WriteLine(hFile, Y1, M1)
			WriteLine(hFile, Y2, M2)
			WriteLine(hFile, Y3, M3)
			
			' Write capex
			WriteLine(hFile, CCT)
			For i = 1 To CCT
				WriteLine(hFile, CC(i, 1), CC(i, 2), CC(i, 3), CC(i, 4))
			Next i
			For i = 1 To LG
				WriteLine(hFile, AC(i, 1), AC(i, 2), AC(i, 3), AC(i, 4), AC(i, 5), AC(i, 6), AC(i, 7), AC(i, 8), AC(i, 9), AC(i, 10), AC(i, 11))
			Next i
			WriteLine(hFile, UBound(gn))
			For i = 1 To UBound(gn)
				WriteLine(hFile, gn(i))
			Next i
			WriteLine(hFile, DiscMthd)
			FileClose(hFile)
		End If
	End Sub
End Module