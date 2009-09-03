Option Strict Off
Option Explicit On
Interface IEFSFileSeqOut
    Function CloseFile() As IEFSFileSeq
    WriteOnly Property NextItem() As Object
    WriteOnly Property NextItemLineEnd() As Object
End Interface
