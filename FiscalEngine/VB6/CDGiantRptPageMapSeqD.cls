VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CDGiantRptPageMapSeqD"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' Name:         CDGiantRptPageMapSeqD.cls
' Function:     Giant Report Page Format Map (sequential)
' Date:         28 Jan 2004 JWD
'---------------------------------------------------------
' This provides formats for sequentially formatted Giant
' report pages. This map supplies formats for the ASPE
' Engine names for the pages and the profiles.
'---------------------------------------------------------
Option Explicit

Implements IDPersistClassMap

Private m_oReportText As CReportText
Private m_oVariableTitles As IVariableTitlesA

'
' The reference to the source of the names
'
Public Property Set ReportText _
    ( _
    ByVal NewObject As CReportText _
    )
    
    Set m_oReportText = NewObject
    
End Property

'
' For formats that need a reference to the variable
' titles table to format the names.
'
Public Property Set VariableTitles _
    ( _
    ByVal NewObject As IVariableTitlesA _
    )
    
    Set m_oVariableTitles = NewObject
    
End Property
    
    
'=========================================================
'
' IDPersistClassMap Interface
'


Private Function IDPersistClassMap_ClassFormat _
    ( _
    ByVal TheObject As IDPersistObject _
    ) As IDPersistFormat

    Dim l_oFmtA As CFmtGntRptPgAToASPENames
    Dim l_oFmtD As CFmtGntRptPgDToASPENames
    Dim l_oFmtG As CFmtGntRptPgGToASPENames
    Dim l_oFmtH As CFmtGntRptPgHToASPENames
    
    Select Case TheObject.ClassIDNumber
    Case 1                  ' Standard pages (A)
        Set l_oFmtA = New CFmtGntRptPgAToASPENames
        Set l_oFmtA.ReportText = m_oReportText
        Set IDPersistClassMap_ClassFormat = l_oFmtA
        
    Case 4                  ' After-tax cash flow page (D)
        Set l_oFmtD = New CFmtGntRptPgDToASPENames
        Set l_oFmtD.ReportText = m_oReportText
        Set l_oFmtD.VariableTitles = m_oVariableTitles
        Set IDPersistClassMap_ClassFormat = l_oFmtD
    
    Case 7                  ' Sub-schedule pages (G)
        Set l_oFmtG = New CFmtGntRptPgGToASPENames
        Set l_oFmtG.ReportText = m_oReportText
        Set l_oFmtG.VariableTitles = m_oVariableTitles
        Set IDPersistClassMap_ClassFormat = l_oFmtG
    
    Case 8                  ' User-defined variable pages (H)
        Set l_oFmtH = New CFmtGntRptPgHToASPENames
        Set l_oFmtH.ReportText = m_oReportText
        Set l_oFmtH.VariableTitles = m_oVariableTitles
        Set IDPersistClassMap_ClassFormat = l_oFmtH
        
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
