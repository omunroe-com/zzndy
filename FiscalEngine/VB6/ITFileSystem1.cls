VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "ITFileSystem1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
'
' Abstract file system (Interface)
'
' Declares functions that return
' abstract sequential file objects.
'
' The actual nature of the files
' (disk or memory based) is determined
' by the object that implements the
' interface.
'
' This is incomplete
'
Option Explicit

Public Function OpenForInput _
    ( _
    ByVal FileName As String _
    ) As IEFSFileSeqIn
    
End Function

Public Function OpenForOutput _
    ( _
    ByVal FileName As String _
    ) As IEFSFileSeqOut
    
End Function

Public Function OpenForAppend _
    ( _
    ByVal FileName As String _
    ) As IEFSFileSeqOut
    
End Function


