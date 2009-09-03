Option Strict Off
Option Explicit On
Interface IEFSFileSeq
    Function OpenForOutput() As IEFSFileSeqOut
    Function OpenForAppend() As IEFSFileSeqOut
    Function OpenForInput() As IEFSFileSeqIn
    ReadOnly Property Length() As Integer
End Interface
