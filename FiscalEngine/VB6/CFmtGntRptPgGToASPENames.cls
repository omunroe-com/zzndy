VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CFmtGntRptPgGToASPENames"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' Name:         CFmtGntRptPgGToASPENames.cls
' Function:     Giant Report page format (table)
' Date:         29 Jan 2004 JWD
'---------------------------------------------------------
' A sequential format for time series names. This outputs
' the names associated with the profiles in profile major
' order. There is a one-to-one correspondence between the
' 'rows' of the names array and the 'rows' of the amounts
' array.
'---------------------------------------------------------
' This implementation provides names for the depreciation
' and cost recovery sub-schedules page and columns as
' provided by the CReportText object. This format performs
' substitutions of variable names into the section title.
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
    Dim k As Integer
    Dim l_sSection As String
    Dim l_sVarCode As String
    Dim l_sText As String
    
    Dim l_nPageType As Long
    Dim l_nColumnCount As Long
    
    
    Dim l_oObject As IDGiantRptPageG
    
    Set l_oObject = TheObject
    With l_oObject
        l_sVarCode = .VariableCode
        With .PageHeader
            l_nPageType = .PageType
            l_nColumnCount = .Columns
        End With
    End With
    
       
    ' Get the variable text. This is the title or name of
    ' the user-defined variable with which this
    ' sub-schedule is associated.
    If l_nPageType = 7 Or l_nPageType = 8 Then
        ' Depreciation/Cost Recovery has full variable title in section title
        l_sText = m_oVariableTitles.LongTitle(l_sVarCode)
    Else
        ' IRR worksheets have variable code only in section title
        l_sText = l_sVarCode
    End If
    
    ' Substitute the variable text (code or title) into the section title
    l_sSection = Replace(m_oReportText.SectionTitle(l_nPageType), "|1|", l_sText)
    
    Set l_oStore = TheStore
    With l_oStore
        ' For each profile
        For i = 1 To l_nColumnCount
            .NextItem = l_sSection
            .NextItemLineEnd = m_oReportText.RowTitle(l_nPageType, i)
        Next i
    End With
    
End Function
