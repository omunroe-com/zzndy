Option Strict Off
Option Explicit On
Interface IEFSFileSeqIn
    Function NextItem() As Object
    Function CloseFile() As IEFSFileSeq
    Function AtEnd() As Boolean
    Function AtBeginning() As Boolean
End Interface
