VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "IEFSFileSeq"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

Public Function OpenForOutput() As IEFSFileSeqOut
End Function

Public Function OpenForAppend() As IEFSFileSeqOut
End Function

Public Function OpenForInput() As IEFSFileSeqIn
End Function

Public Property Get Length() As Long
End Property
