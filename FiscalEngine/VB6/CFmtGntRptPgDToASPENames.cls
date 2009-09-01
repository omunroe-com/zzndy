VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CFmtGntRptPgDToASPENames"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' Name:         CFmtGntRptPgDToASPENames.cls
' Function:     Giant Report page format (table)
' Date:         29 Jan 2004 JWD
'---------------------------------------------------------
' A sequential format for time series names. This outputs
' the names associated with the profiles in profile major
' order. There is a one-to-one correspondence between the
' 'rows' of the names array and the 'rows' of the amounts
' array.
'---------------------------------------------------------
' This implementation provides after-tax cash flow page
' names for the page and columns as provided by the
' CReportText object. This format performs text
' substitution for variable names on row profile names.
'---------------------------------------------------------
Option Explicit

Implements IDPersistFormat

Private m_oReportText As CReportText

Private m_oVariableTitles As IVariableTitlesA

Public Property Set ReportText _
    ( _
    ByVal NewObject As CReportText _
    )
        
    Set m_oReportText = NewObject
    
End Property

Public Property Set VariableTitles _
    ( _
    ByVal NewObject As IVariableTitlesA _
    )
    
    Set m_oVariableTitles = NewObject
    
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
' Put the time series profile names on the store in
' profile major order.
'
Private Function IDPersistFormat_Store _
    ( _
    ByVal TheStore As IDStore, _
    ByVal TheObject As IDPersistObject _
    ) As Boolean
    
    Dim l_oStore As IDStoreSeq
        
    Dim i As Integer
    Dim j As Integer
    
    Dim l_sSection As String
    Dim l_sText As String
    
    Dim l_nPageType As Long
    Dim l_nColumnCount As Long
    
    Dim la_iProfTypes() As Integer
    Dim la_sVarCodes() As String
    
    Dim l_oObject As IDGiantRptPageD
    
    Set l_oObject = TheObject
    With l_oObject
        With .PageHeader
            l_nPageType = .PageType
            l_nColumnCount = .Columns
        End With
        la_iProfTypes = .ProfileTypes
        la_sVarCodes = .ProfileCodes
    End With
    
    l_sSection = m_oReportText.SectionTitle(l_nPageType)
    
    ' Compute adjustment for array element indexing
    j = LBound(la_sVarCodes) - 1
    
    Set l_oStore = TheStore
    With l_oStore
        ' For each profile
        For i = 1 To l_nColumnCount
            .NextItem = l_sSection
            
            l_sText = m_oVariableTitles.LongTitle(la_sVarCodes(i + j))
            If Len(l_sText) > 0 Then
                l_sText = l_sText & " (" & la_sVarCodes(i + j) & ")"
            Else
                l_sText = la_sVarCodes(i + j)
            End If
            .NextItemLineEnd = Replace(m_oReportText.RowTitle(l_nPageType, la_iProfTypes(i + j)), "|1|", l_sText)
        Next i
    End With
    
End Function
