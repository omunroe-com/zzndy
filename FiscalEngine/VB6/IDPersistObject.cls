VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "IDPersistObject"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit


Public Property Get ClassIDNumber _
    () As Long
End Property

Public Property Get ClassName _
    () As String
End Property

Public Property Get ObjectIDNumber _
    () As Long
End Property

Public Property Let ObjectIDNumber _
    ( _
    ByVal NewIDNumber As Long _
    )
End Property

Public Function RegisterInTable _
    ( _
    ByVal ObjectTable As IDPersistObjectTable _
    ) As Boolean
End Function

Public Function RegisterObjectConnections _
    () As Boolean
End Function

Public Function RestoreUsingFormat _
    ( _
    ByVal TheStore As IDStore, _
    ByVal TheMap As IDPersistClassMap _
    ) As Boolean
End Function

Public Function StoreUsingFormat _
    ( _
    ByVal TheStore As IDStore, _
    ByVal TheMap As IDPersistClassMap _
    ) As Boolean
End Function
