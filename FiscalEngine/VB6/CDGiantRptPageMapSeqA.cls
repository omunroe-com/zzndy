VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CDGiantRptPageMapSeqA"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' Name:         CDGiantRptPageMapSeqA.cls
' Function:     Giant Report Page Format Map (sequential)
' Date:         28 Jan 2004 JWD
'---------------------------------------------------------
' This provides formats for sequentially formatted Giant
' report pages
'---------------------------------------------------------
Option Explicit

Implements IDPersistClassMap


'=========================================================
'
' IDPersistClassMap Interface
'


Private Function IDPersistClassMap_ClassFormat _
    ( _
    ByVal TheObject As IDPersistObject _
    ) As IDPersistFormat

    Select Case TheObject.ClassIDNumber
    Case 1, 4, 5, 6, 7, 8
        Set IDPersistClassMap_ClassFormat = New CGiantRptPageStdFormatSeq
    Case 2
        Set IDPersistClassMap_ClassFormat = New CGiantRptPageIndFormatSeq
    Case 3
        Set IDPersistClassMap_ClassFormat = New CGiantRptPagePRTFormatSeq
    Case 9
        Set IDPersistClassMap_ClassFormat = New CGiantRptPageHdrFormatSeq
    Case Else
        ' Return the do-nothing format
        Set IDPersistClassMap_ClassFormat = New CDPersistFormatDummy
    End Select
    
End Function

Private Function IDPersistClassMap_MakeObjectOfClass _
    ( _
    ByVal TheClassName As String _
    ) As IDPersistObject

End Function
