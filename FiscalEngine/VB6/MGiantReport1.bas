Attribute VB_Name = "MGiantReport1"
' Name:         MGiantReport1.bas
' Function:     Test module for CGiantReport1.cls
' Date:         16 Jan 2004 JWD
'---------------------------------------------------------
' This module provides support routines for testing the
' CGiantReport1 and CGiantRptPagex objects in MAINEXEC.
' This is for migration to implementing these objects in
' the AS$ET Single Project Economics Engine (ASPEEngine).
'---------------------------------------------------------
Option Explicit

Public g_oReport As CGiantReport1

Public g_oFileSystem As ITFileSystem1

Public g_oRunFileIn As IEFSFileSeqIn

Public g_oRingFenceFile As IEFSFile

' Indicates that the engine is being
' run from the IMainexec interface.
' When true, enables certain code to
' execute that produces output specific
' to Mainexec processing.
Public g_bIsMainexecRun As Boolean

Public Function ReportFileSpec _
    ( _
    ByVal ReportFolder As String, _
    ByVal ReportName As String, _
    ByVal RunNumber As Integer, _
    ByVal Extension As String _
    ) As String
    
    Dim l_sFolder As String
    Dim l_sBase As String
    Dim l_sExt As String
    
    l_sFolder = ReportFolder
    If Right(l_sFolder, 1) = "\" Then
        ' ok
    Else
        l_sFolder = l_sFolder & "\"
    End If
    
    If Len(ReportName) > 6 Then
       l_sBase = Mid$(ReportName, 1, 6) + Right$(str$(RunNumber), Len(str$(RunNumber)) - 1)
    Else
       l_sBase = ReportName + Right$(str$(RunNumber), Len(str$(RunNumber)) - 1)
    End If

    l_sExt = Extension
    If Left(l_sExt, 1) = "." Then
        ' ok
    Else
        l_sExt = "." & l_sExt
    End If
    
    ReportFileSpec = l_sFolder + l_sBase & l_sExt
    
End Function

Public Sub WriteOutputReport _
    ( _
    TheReport As CGiantReport1, _
    ReportFileSpec As String, _
    ByRef FileSystem As ITFileSystem1 _
    )

    Dim l_oMap As IDPersistClassMap
    
    Dim l_oFileSeqOut As IEFSFileSeqOut
    
    Set l_oMap = New CDGiantRptPageMapSeqA      ' standard prn format
    
    Set l_oFileSeqOut = FileSystem.OpenForOutput(ReportFileSpec)
    TheReport.WriteReport l_oFileSeqOut, l_oMap
    l_oFileSeqOut.CloseFile
   
End Sub

'
' Changes precision of 2-d array
' Returns double precision copy of
' argument array.
'
Public Function Array2DSingleAsDouble _
    ( _
    ValuesArray() As Single _
    ) As Double()

    Dim i As Long
    Dim j As Long

    Dim a_dValues() As Double

    ReDim a_dValues(LBound(ValuesArray, 1) To UBound(ValuesArray, 1), LBound(ValuesArray, 2) To UBound(ValuesArray, 2))

    ' Copy the values to the return array, changing precision
    For j = LBound(ValuesArray, 2) To UBound(ValuesArray, 2)
        For i = LBound(ValuesArray, 1) To UBound(ValuesArray, 1)
            a_dValues(i, j) = CDbl(ValuesArray(i, j))
        Next i
    Next j

    Array2DSingleAsDouble = a_dValues

End Function

Public Function IsVolumeForecast _
    ( _
    ByVal VariableCode As String _
    ) As Boolean
    
    Dim i As Integer
    
    SearchCodeString Left(BDACategoryCodesString, gc_nAMAXVOL * 3), VariableCode, 3, i
    
    IsVolumeForecast = (Not (i = 0))
    
End Function

