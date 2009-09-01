VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "IDGiantRptPageG"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

Public Property Get PageHeader _
    () As CGiantRptPageHdr1
End Property

Public Property Set PageHeader _
    ( _
    ByVal NewObject As CGiantRptPageHdr1 _
    )
End Property

Public Property Get Headers _
    () As String()
End Property

Public Property Let Headers _
    ( _
    ByRef NewValues() As String _
    )
End Property

Public Property Get Values _
    () As Single()
End Property

Public Property Let Values _
    ( _
    ByRef NewValues() As Single _
    )
End Property

Public Property Get VariableCode _
    () As String
End Property

Public Property Let VariableCode _
    ( _
    ByVal NewValue As String _
    )
End Property

