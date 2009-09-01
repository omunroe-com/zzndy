Option Strict Off
Option Explicit On
Friend Class CEFSFileSpec
    Implements IEFSFile
    '
    ' Instances of this class are ordinary files in
    ' the operating system's file system.
    '


    Private zzz_FullName As String

    '
    ' For now, this property assumes that the new name
    ' specified is valid in the file system.
    '

    '
    ' Return the file specification for the
    ' temporary file.
    '
    Public Property FullName() As String
        Get

            FullName = zzz_FullName

        End Get
        Set(ByVal Value As String)

            zzz_FullName = Value

        End Set
    End Property


    '=========================================================
    '
    ' IEFSFile Interface
    '
    '
    Private ReadOnly Property IEFSFile_FullName() As String Implements IEFSFile.FullName
        Get

            IEFSFile_FullName = zzz_FullName

        End Get
    End Property


    Private ReadOnly Property IEFSFile_Name() As String Implements IEFSFile.Name
        Get

            Dim p As Integer

            p = InStrRev(zzz_FullName, "\")
            If p > 0 Then
                IEFSFile_Name = Mid(zzz_FullName, p + 1)
            End If

            'IEFSFile_Name = GetFilePart(zzz_FullName)

        End Get
    End Property


    Private ReadOnly Property IEFSFile_Path() As String Implements IEFSFile.Path
        Get

            Dim p As Integer

            p = InStrRev(zzz_FullName, "\")
            If p > 0 Then
                IEFSFile_Path = Left(zzz_FullName, p - 1)
            End If

            'IEFSFile_Path = GetPathPart(zzz_FullName)

        End Get
    End Property
	
	
	'UPGRADE_NOTE: Class_Initialize was upgraded to Class_Initialize_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Private Sub Class_Initialize_Renamed()
		
		zzz_FullName = vbNullString
		
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
End Class