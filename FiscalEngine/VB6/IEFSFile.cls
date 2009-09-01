VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "IEFSFile"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'
' This is the fully qualified file specification
' (path and name)
'
Public Property Get FullName() As String
End Property

'
' This is the name part only of the file
'
Public Property Get Name() As String
End Property

'
' This is the path portion of the full name
'
Public Property Get Path() As String
End Property
