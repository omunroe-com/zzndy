Option Strict Off
Option Explicit On
Module MGiantReport1
	' Name:         MGiantReport1.bas
	' Function:     Test module for CGiantReport1.cls
	' Date:         16 Jan 2004 JWD
	'---------------------------------------------------------
	' This module provides support routines for testing the
	' CGiantReport1 and CGiantRptPagex objects in MAINEXEC.
	' This is for migration to implementing these objects in
	' the AS$ET Single Project Economics Engine (ASPEEngine).
	'---------------------------------------------------------
	
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

    Public Function ReportFileSpec(ByVal ReportFolder As String, ByVal ReportName As String, ByVal RunNumber As Short, ByVal Extension As String) As String

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
            l_sBase = Mid(ReportName, 1, 6) & Right(Str(RunNumber), Len(Str(RunNumber)) - 1)
        Else
            l_sBase = ReportName & Right(Str(RunNumber), Len(Str(RunNumber)) - 1)
        End If

        l_sExt = Extension
        If Left(l_sExt, 1) = "." Then
            ' ok
        Else
            l_sExt = "." & l_sExt
        End If

        ReportFileSpec = l_sFolder & l_sBase & l_sExt

    End Function

    'UPGRADE_NOTE: FileSystem was upgraded to FileSystem_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    Public Sub WriteOutputReport(ByRef TheReport As CGiantReport1, ByRef ReportFileSpec As String, ByRef FileSystem_Renamed As ITFileSystem1)

        Dim l_oMap As IDPersistClassMap

        Dim l_oFileSeqOut As IEFSFileSeqOut

        l_oMap = New CDGiantRptPageMapSeqA ' standard prn format

        l_oFileSeqOut = FileSystem_Renamed.OpenForOutput(ReportFileSpec)
        'UPGRADE_WARNING: Couldn't resolve default property of object l_oFileSeqOut. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        TheReport.WriteReport(l_oFileSeqOut, l_oMap)
        l_oFileSeqOut.CloseFile()

    End Sub
	
	'
	' Changes precision of 2-d array
	' Returns double precision copy of
	' argument array.
	'
    Public Function Array2DSingleAsDouble(ByRef ValuesArray(,) As Single) As Double(,)

        Dim i As Integer
        Dim j As Integer

        Dim a_dValues(,) As Double

        'UPGRADE_WARNING: Lower bound of array a_dValues was changed from LBound(ValuesArray, 1),LBound(ValuesArray, 2) to 0,0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim a_dValues(UBound(ValuesArray, 1), UBound(ValuesArray, 2))

        ' Copy the values to the return array, changing precision
        For j = LBound(ValuesArray, 2) To UBound(ValuesArray, 2)
            For i = LBound(ValuesArray, 1) To UBound(ValuesArray, 1)
                a_dValues(i, j) = CDbl(ValuesArray(i, j))
            Next i
        Next j

        Array2DSingleAsDouble = VB6.CopyArray(a_dValues)

    End Function
	
	Public Function IsVolumeForecast(ByVal VariableCode As String) As Boolean
		
		Dim i As Short
		
		SearchCodeString(Left(BDACategoryCodesString, gc_nAMAXVOL * 3), VariableCode, 3, i)
		
		IsVolumeForecast = (Not (i = 0))
		
	End Function
End Module