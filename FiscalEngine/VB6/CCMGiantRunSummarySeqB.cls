VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CCMGiantRunSummarySeqB"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' Name:         CCMGiantRunSummarySeqB.cls
' Function:     Giant Run Summary Report Format Map (sequential)
' Date:         23 Sep 2004 JWD
'---------------------------------------------------------
' This provides formats for sequentially formatted Giant
' report pages
'
' This specific map provides for the output of the run
' summary data for an individual run. This data is stored
' in the economic summary page. Other pages of the output
' are not output.
'---------------------------------------------------------
Option Explicit

Implements IDPersistClassMap


'=========================================================
'
' IDPersistClassMap Interface
'

Public Property Get AsIDPersistClassMap() As IDPersistClassMap
    Set AsIDPersistClassMap = Me
End Property


Private Function IDPersistClassMap_ClassFormat _
    ( _
    ByVal TheObject As IDPersistObject _
    ) As IDPersistFormat

    Select Case TheObject.ClassIDNumber
    Case 2
        Set IDPersistClassMap_ClassFormat = New CSFGiantRunSummarySeqB
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
