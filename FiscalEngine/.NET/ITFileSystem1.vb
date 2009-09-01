Option Strict Off
Option Explicit On
Interface ITFileSystem1
    Function OpenForInput(ByVal FileName As String) As IEFSFileSeqIn
    Function OpenForOutput(ByVal FileName As String) As IEFSFileSeqOut
    Function OpenForAppend(ByVal FileName As String) As IEFSFileSeqOut
End Interface
