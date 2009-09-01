VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CXFileSystemMem1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
'
' This is a factory for memory-based files
'
Option Explicit

Implements ITFileSystem1

Private Function ITFileSystem1_OpenForInput _
    ( _
    ByVal FileName As String _
    ) As IEFSFileSeqIn

    Set ITFileSystem1_OpenForInput = zzzGetSeqFile(FileName).OpenForInput

End Function

Private Function ITFileSystem1_OpenForOutput _
    ( _
    ByVal FileName As String _
    ) As IEFSFileSeqOut

    Set ITFileSystem1_OpenForOutput = zzzGetSeqFile(FileName).OpenForOutput

End Function

Private Function ITFileSystem1_OpenForAppend _
    ( _
    ByVal FileName As String _
    ) As IEFSFileSeqOut

    Set ITFileSystem1_OpenForAppend = zzzGetSeqFile(FileName).OpenForAppend

End Function

'
' Return memory file object
'
Private Function zzzGetSeqFile _
    ( _
    ByVal FileName As String _
    ) As IEFSFileSeq
    
    Dim l_oName As CEFSFileSpec
    Dim l_oFile As CXFSFileSeqMemVnt      ' memory file object
    
    Set l_oName = New CEFSFileSpec
    l_oName.FullName = FileName
    
    Set l_oFile = New CXFSFileSeqMemVnt
    Set l_oFile.File = l_oName
    
    Set zzzGetSeqFile = l_oFile
    
End Function

