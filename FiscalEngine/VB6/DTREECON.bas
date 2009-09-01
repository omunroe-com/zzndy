Attribute VB_Name = "DTREECON"
Option Explicit

' WriteDtreeData - write out the data need for the decision tree

Public Sub WriteDtreeData()
    Const c_nVersion As Integer = 1
    Dim hFile As Integer
    Dim i As Integer
    
    If Len(Dir$(TempDir & "DTREE.FLG")) <> 0 Then
        hFile = FreeFile
        Open TempDir & "DTREE.PRN" For Output As hFile
        
        Write #hFile, "DTREE_DATA"
        Write #hFile, c_nVersion
        ' Write out life
         Write #hFile, LG
         
        ' Write out dates
        Write #hFile, YR, mo
        Write #hFile, Y1, M1
        Write #hFile, Y2, M2
        Write #hFile, Y3, M3
        
        ' Write capex
        Write #hFile, CCT
        For i = 1 To CCT
            Write #hFile, CC(i, 1), CC(i, 2), CC(i, 3), CC(i, 4)
        Next i
        For i = 1 To LG
            Write #hFile, AC(i, 1), AC(i, 2), AC(i, 3), AC(i, 4), _
                          AC(i, 5), AC(i, 6), AC(i, 7), AC(i, 8), _
                          AC(i, 9), AC(i, 10), AC(i, 11)
        Next i
        Write #hFile, UBound(gn)
        For i = 1 To UBound(gn)
            Write #hFile, gn(i)
        Next i
        Write #hFile, DiscMthd
        Close #hFile
    End If
End Sub
