VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "IDStoreSeq"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'
' Adds another item to the current 'line'
'
Public Property Let NextItem _
    ( _
    ByVal DataItem As Variant _
    )
End Property

'
' Adds another item to the current 'line' then adds
' line-end sequence (CRLF)
'
Public Property Let NextItemLineEnd _
    ( _
    ByVal DataItem As Variant _
    )
End Property


