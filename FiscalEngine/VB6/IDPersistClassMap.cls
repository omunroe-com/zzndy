VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "IDPersistClassMap"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False

'
' Objects of this class map persistence formats to classes
'
Option Explicit

Public Function ClassFormat _
    ( _
    ByVal TheObject As IDPersistObject _
    ) As IDPersistFormat
End Function

Public Function MakeObjectOfClass _
    ( _
    ByVal TheClassName As String _
    ) As IDPersistObject
End Function
