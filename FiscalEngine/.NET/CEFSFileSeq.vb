Option Strict Off
Option Explicit On
Friend Class CEFSFileSeq
    Implements IDStore
    Implements IDStoreSeq
    Implements IEFSFile
    Implements IEFSFileSeq
    Implements IEFSFileSeqOut
    Implements IEFSFileSeqIn
	'
	' Defines a Basic sequential file. This
	' class just provides methods for processing the
	' contents of a file that is sequentially organized
	' text. It is not sophisticated enough to do a lot
	' of fancy stuff, but will Input and Write items
	' that are the elementary scalar Basic types.
	'
	' (don't know what will happen if an object or
	' an array is passed or expected)
	'
	
	
	
	' Just something that identifies a file...
	' An instance of this class is going to
	' interpret the contents of this file as
	' if it is a Basic sequentially organized
	' text file (Write, Input).
    Private m_oFile As IEFSFile
	
	Private m_iHandle As Integer
	
	' This is the delegate and represents the
	' state of the file
	'Private m_oFile As IEFSFileState
	
	
    Public Property File() As IEFSFile
        Get

            File = m_oFile

        End Get
        Set(ByVal Value As IEFSFile)

            m_oFile = Value

        End Set
    End Property
	
	'=========================================================
	'
	' IDStoreSeq Interface
	'
	
	
    Private WriteOnly Property IDStoreSeq_NextItem() As Object Implements IDStoreSeq.NextItem
        Set(ByVal Value As Object)

            Write(m_iHandle, Value)

        End Set
    End Property
	
    Private WriteOnly Property IDStoreSeq_NextItemLineEnd() As Object Implements IDStoreSeq.NextItemLineEnd
        Set(ByVal Value As Object)

            WriteLine(m_iHandle, Value)

        End Set
    End Property
	
	'=========================================================
	'
	' IEFSFile Interface
	'
	'
    Private ReadOnly Property IEFSFile_FullName() As String Implements IEFSFile.FullName
        Get

            IEFSFile_FullName = m_oFile.FullName

        End Get
    End Property
	
	
    Private ReadOnly Property IEFSFile_Name() As String Implements IEFSFile.Name
        Get

            IEFSFile_Name = m_oFile.Name

        End Get
    End Property
	
	
    Private ReadOnly Property IEFSFile_Path() As String Implements IEFSFile.Path
        Get

            IEFSFile_Path = m_oFile.Path

        End Get
    End Property
	
	
	'=========================================================
	'
	' IEFSFileSeq Interface
	'
	'
	
    Private ReadOnly Property IEFSFileSeq_Length() As Integer Implements IEFSFileSeq.Length
        Get
            IEFSFileSeq_Length = FileLen(m_oFile.FullName)
        End Get
    End Property
	
    Private WriteOnly Property IEFSFileSeqOut_NextItem() As Object Implements IEFSFileSeqOut.NextItem
        Set(ByVal Value As Object)

            Write(m_iHandle, Value)

        End Set
    End Property
	
    Private WriteOnly Property IEFSFileSeqOut_NextItemLineEnd() As Object Implements IEFSFileSeqOut.NextItemLineEnd
        Set(ByVal Value As Object)

            WriteLine(m_iHandle, Value)

        End Set
    End Property
	
	'UPGRADE_NOTE: Class_Terminate was upgraded to Class_Terminate_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Private Sub Class_Terminate_Renamed()
		If m_iHandle <> 0 Then
			FileClose(m_iHandle)
		End If
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
    Private Function IEFSFileSeq_OpenForAppend() As IEFSFileSeqOut Implements IEFSFileSeq.OpenForAppend

        m_iHandle = FreeFile()
        FileOpen(m_iHandle, m_oFile.FullName, OpenMode.Append)

        IEFSFileSeq_OpenForAppend = Me

    End Function
	
    Private Function IEFSFileSeq_OpenForInput() As IEFSFileSeqIn Implements IEFSFileSeq.OpenForInput

        m_iHandle = FreeFile()
        FileOpen(m_iHandle, m_oFile.FullName, OpenMode.Input)

        IEFSFileSeq_OpenForInput = Me

    End Function
	
    Private Function IEFSFileSeq_OpenForOutput() As IEFSFileSeqOut Implements IEFSFileSeq.OpenForOutput

        m_iHandle = FreeFile()
        FileOpen(m_iHandle, m_oFile.FullName, OpenMode.Output)

        IEFSFileSeq_OpenForOutput = Me

    End Function
	
	
	'=========================================================
	'
	' IEFSFileSeqIn Interface
	'
	'
	
    Private Function IEFSFileSeqIn_AtBeginning() As Boolean Implements IEFSFileSeqIn.AtBeginning
        IEFSFileSeqIn_AtBeginning = (Seek(m_iHandle) = 1)
    End Function
	
    Private Function IEFSFileSeqIn_AtEnd() As Boolean Implements IEFSFileSeqIn.AtEnd
        IEFSFileSeqIn_AtEnd = EOF(m_iHandle)
    End Function
	
    Private Function IEFSFileSeqIn_CloseFile() As IEFSFileSeq Implements IEFSFileSeqIn.CloseFile
        FileClose(m_iHandle)
        m_iHandle = 0
        IEFSFileSeqIn_CloseFile = Me
    End Function
	
    Private Function IEFSFileSeqIn_NextItem() As Object Implements IEFSFileSeqIn.NextItem

        Dim temp_var As Object

        Input(m_iHandle, temp_var)
        'UPGRADE_WARNING: Couldn't resolve default property of object temp_var. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        'UPGRADE_WARNING: Couldn't resolve default property of object IEFSFileSeqIn_NextItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        IEFSFileSeqIn_NextItem = temp_var

    End Function
	
	
	'=========================================================
	'
	' IEFSFileSeqOut Interface
	'
	'
    Private Function IEFSFileSeqOut_CloseFile() As IEFSFileSeq Implements IEFSFileSeqOut.CloseFile
        FileClose(m_iHandle)
        m_iHandle = 0
        IEFSFileSeqOut_CloseFile = Me
    End Function
End Class