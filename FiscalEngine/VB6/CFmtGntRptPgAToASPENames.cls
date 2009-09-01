VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CFmtGntRptPgAToASPENames"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' Name:         CFmtGntRptPgAToASPENames.cls
' Function:     Giant Report page format (table)
' Date:         29 Jan 2004 JWD
'---------------------------------------------------------
' A sequential format for time series names. This outputs
' the names associated with the profiles in profile major
' order. There is a one-to-one correspondence between the
' 'rows' of the names array and the 'rows' of the amounts
' array.
'---------------------------------------------------------
' This implementation provides standard page names for
' the page and columns as provided by the CReportText
' object. This format does no text substitution.
'---------------------------------------------------------
Option Explicit

Implements IDPersistFormat

Private m_oReportText As CReportText


Public Property Set ReportText _
    ( _
    ByVal NewObject As CReportText _
    )
        
    Set m_oReportText = NewObject
    
End Property

'=========================================================
'
' IDPersistFormat
'

Private Function IDPersistFormat_Restore _
    ( _
    ByVal TheStore As IDStore, _
    ByVal TheObject As IDPersistObject _
    ) As Boolean
End Function

'
' Put the time series interests on the store in
' profile major order.
'
Private Function IDPersistFormat_Store _
    ( _
    ByVal TheStore As IDStore, _
    ByVal TheObject As IDPersistObject _
    ) As Boolean
    
    Dim l_oStore As IDStoreSeq
        
    Dim i As Integer
    Dim l_sSection As String
    
    Dim l_nPageType As Long
    Dim l_nColumnCount As Long
    
    
    Dim l_oObject As IDGiantRptPageStd
    
    Set l_oObject = TheObject
    With l_oObject.PageHeader
        l_nPageType = .PageType
        l_nColumnCount = .Columns
    End With
    
    l_sSection = m_oReportText.SectionTitle(l_nPageType)
    
    Set l_oStore = TheStore
    With l_oStore
        ' For each profile
        For i = 1 To l_nColumnCount
            .NextItem = l_sSection
            .NextItemLineEnd = m_oReportText.RowTitle(l_nPageType, i)
        Next i
    End With
    
End Function
