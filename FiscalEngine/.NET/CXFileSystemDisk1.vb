Option Strict Off
Option Explicit On
Friend Class CXFileSystemDisk1
    Implements ITFileSystem1
    '
    ' Implementation of disk based file system.
    ' Returns objects representing disk files.
    '


    Private Function ITFileSystem1_OpenForInput(ByVal FileName As String) As IEFSFileSeqIn Implements ITFileSystem1.OpenForInput

        ITFileSystem1_OpenForInput = zzzGetSeqFile(FileName).OpenForInput

    End Function

    Private Function ITFileSystem1_OpenForOutput(ByVal FileName As String) As IEFSFileSeqOut Implements ITFileSystem1.OpenForOutput

        ITFileSystem1_OpenForOutput = zzzGetSeqFile(FileName).OpenForOutput

    End Function

    Private Function ITFileSystem1_OpenForAppend(ByVal FileName As String) As IEFSFileSeqOut Implements ITFileSystem1.OpenForAppend

        ITFileSystem1_OpenForAppend = zzzGetSeqFile(FileName).OpenForAppend

    End Function

    '
    ' Return disk file object
    '
    Private Function zzzGetSeqFile(ByVal FileName As String) As IEFSFileSeq

        Dim l_oName As CEFSFileSpec
        Dim l_oFile As CEFSFileSeq ' disk file object

        l_oName = New CEFSFileSpec
        l_oName.FullName = FileName

        l_oFile = New CEFSFileSeq
        l_oFile.File = l_oName

        zzzGetSeqFile = l_oFile

    End Function
End Class