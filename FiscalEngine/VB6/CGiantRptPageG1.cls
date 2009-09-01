VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CGiantRptPageG1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' Name:         CGiantRptPageG1.cls
' Function:     Wrapper class for Giant Report Page Type
' Date:         27 Jan 2004 JWD
'---------------------------------------------------------
Option Explicit

Implements IDGiantRptPageG          ' For ASPE Engine
Implements IDGiantRptPageStd        ' For standard output
Implements IDPersistObject
Implements IGiantRptPageAssignSub
'Implements IGiantTimeSeriesRptPage

'
' IDPersistObject attributes
'
Private Const m_lClassID As Long = 7
Private Const m_sClassName As String = "CGiantRptPageG1"
Private m_lObjectID As Long


Private m_oHeader As CGiantRptPageHdr1

Private ma_sTitles() As String
Private ma_rValues() As Single

Private m_sVarCode As String

Private Sub Class_Initialize()
    Set m_oHeader = New CGiantRptPageHdr1
End Sub


'=========================================================
'
' IDGiantRptPageG Interface
'

Private Property Let IDGiantRptPageG_Headers _
    ( _
    ByRef NewValues() As String _
    )
    ma_sTitles = NewValues
End Property

Private Property Get IDGiantRptPageG_Headers _
    () As String()
    IDGiantRptPageG_Headers = ma_sTitles
End Property

Private Property Set IDGiantRptPageG_PageHeader _
    ( _
    ByVal NewObject As CGiantRptPageHdr1 _
    )
    Set m_oHeader = NewObject
End Property

Private Property Get IDGiantRptPageG_PageHeader _
    () As CGiantRptPageHdr1
    Set IDGiantRptPageG_PageHeader = m_oHeader
End Property

Private Property Let IDGiantRptPageG_Values _
    ( _
    ByRef NewValues() As Single _
    )
    ma_rValues = NewValues
End Property

Private Property Get IDGiantRptPageG_Values _
    () As Single()
    IDGiantRptPageG_Values = ma_rValues
End Property

Private Property Let IDGiantRptPageG_VariableCode _
    ( _
    ByVal NewValue As String _
    )
    m_sVarCode = NewValue
End Property

Private Property Get IDGiantRptPageG_VariableCode _
    () As String
    IDGiantRptPageG_VariableCode = m_sVarCode
End Property


'=========================================================
'
' IDGiantRptPageStd Interface
'


Private Property Let IDGiantRptPageStd_Headers _
    ( _
    ByRef NewValues() As String _
    )
    ma_sTitles = NewValues
End Property

Private Property Get IDGiantRptPageStd_Headers _
    () As String()
    IDGiantRptPageStd_Headers = ma_sTitles
End Property

Private Property Set IDGiantRptPageStd_PageHeader _
    ( _
    ByVal NewObject As CGiantRptPageHdr1 _
    )
    Set m_oHeader = NewObject
End Property

Private Property Get IDGiantRptPageStd_PageHeader _
    () As CGiantRptPageHdr1
    Set IDGiantRptPageStd_PageHeader = m_oHeader
End Property

Private Property Let IDGiantRptPageStd_Values _
    ( _
    ByRef NewValues() As Single _
    )
    ma_rValues = NewValues
End Property

Private Property Get IDGiantRptPageStd_Values _
    () As Single()
    IDGiantRptPageStd_Values = ma_rValues
End Property


'=========================================================
'
' IDPersistObject Interface
'

'
' The first four are also attributes of IDObject
' since all IDPersistObjects are also IDObjects.
'

Private Property Get IDPersistObject_ClassIDNumber _
    () As Long
    IDPersistObject_ClassIDNumber = m_lClassID
End Property

Private Property Get IDPersistObject_ClassName _
    () As String
    IDPersistObject_ClassName = m_sClassName
End Property

Private Property Let IDPersistObject_ObjectIDNumber _
    ( _
    ByVal NewIDNumber As Long _
    )
    m_lObjectID = NewIDNumber
End Property

Private Property Get IDPersistObject_ObjectIDNumber _
    () As Long
    IDPersistObject_ObjectIDNumber = m_lObjectID
End Property

'
' These methods are specific to IDPersistObjects
'
Private Function IDPersistObject_RegisterInTable _
    ( _
    ByVal ObjectTable As IDPersistObjectTable _
    ) As Boolean
End Function

Private Function IDPersistObject_RegisterObjectConnections _
    () As Boolean
End Function

Private Function IDPersistObject_RestoreUsingFormat _
    ( _
    ByVal TheStore As IDStore, _
    ByVal TheMap As IDPersistClassMap _
    ) As Boolean
End Function

Private Function IDPersistObject_StoreUsingFormat _
    ( _
    ByVal TheStore As IDStore, _
    ByVal TheMap As IDPersistClassMap _
    ) As Boolean
    
    TheMap.ClassFormat(Me).Store TheStore, Me
    
End Function


'=========================================================
'
' IGiantRptPageAssignSub Interface
'

'
' Set all elements of the page header.
'
' This is to replace the Write statement that writes
' this data on the report file.
'
Private Sub IGiantRptPageAssignSub_SetPageHeader _
    ( _
    ByVal PageType As Integer, _
    ByVal startyear As Integer, _
    ByVal PageCount As Integer, _
    ByVal ProjectLife As Integer, _
    ByVal ProfileCount As Integer, _
    ByVal PageTitle As String, _
    ByVal ColumnWidth As Integer, _
    ByVal FinalWorkingInt As Single, _
    ByVal FinalParticipation As Single, _
    ByVal PageCurrency As String _
    )

    Dim i As Integer
    
    With m_oHeader
        .PageType = PageType
        .year = startyear
        .PageCounter = PageCount
        .Rows = ProjectLife
        .Columns = ProfileCount
        .PageTitle = PageTitle
        .ColumnWidth = ColumnWidth
        .CompanyWorkingInterest = FinalWorkingInt
        .GovernmentParticipation = FinalParticipation
        .CurrencyCode = PageCurrency
    End With
    
    ReDim ma_sTitles(0 To ProfileCount - 1)
    ReDim ma_rValues(0 To ProfileCount - 1, 0 To ProjectLife - 1)
    
End Sub

'
' Set the profile (column) names
'
' This is to replace the Write statement that writes
' this data on the report file.
'
Private Sub IGiantRptPageAssignSub_SetProfileHeaders _
    ( _
    ParamArray ProfileNames() As Variant _
    )
    
    Dim i As Integer
    Dim k As Integer
    
    k = LBound(ProfileNames)
    
    For i = 0 To UBound(ma_sTitles)
        ma_sTitles(i) = ProfileNames(i + k)
    Next i
    
End Sub

'
' Set the values for all profiles for the specified row
'
' This is to replace the Write statement that writes
' this data on the report file.
'
' Assumes that the RowIndex is in the range 1-ProjectLife
' (1 To Ubound(ma_rValues, 1) + 1).
'
Private Sub IGiantRptPageAssignSub_SetProfileValues _
    ( _
    ByVal RowIndex As Integer, _
    ParamArray ProfileValues() As Variant _
    )

    Dim i As Integer
    Dim k As Integer
    
    k = LBound(ProfileValues)
    
    For i = 0 To UBound(ma_rValues, 1)
        ma_rValues(i, RowIndex - 1) = ProfileValues(i + k)
    Next i
    
End Sub

Private Sub IGiantRptPageAssignSub_SetRptPageVariableCode _
    ( _
    ByVal VariableCode As String _
    )
    
    m_sVarCode = VariableCode
End Sub


'=====================================================================
' IGiantTimeSeriesRptPage Interface
'

''''
'''' Add the values to the specified array, beginning with specified row.
''''
'''' StartRow is the subscript 1 index value into which the first
'''' page profile will be copied:
''''   ValuesArray(StartRow, x) = ma_rValues(0, x),
''''       for x = LBound(ma_rValues, 2) To UBound(ma_rValues,2)
''''
'''' Assumes that the ValuesArray() is dimensioned to accomodate the
'''' data to be added.
''''
'''Private Sub IGiantTimeSeriesRptPage_AppendTimeSeriesValues _
'''    ( _
'''    ByRef ValuesArray() As Single, _
'''    ByRef StartRow As Long _
'''    )
'''
'''    Dim i As Long
'''    Dim j As Long
'''    Dim L As Long
'''
'''    L = LBound(ValuesArray, 2)
'''
'''    For i = 0 To UBound(ma_rValues, 2)
'''        For j = 0 To UBound(ma_rValues, 1)
'''            ValuesArray(j + StartRow, i + L) = ma_rValues(j, i)
'''        Next j
'''    Next i
'''
'''    StartRow = StartRow + UBound(ma_rValues, 1) + 1
'''
'''End Sub
'''
''''
'''' Append the interests associated with the time series
'''' profiles for this page to the values array starting
'''' with the specified "row" (StartRow).
''''
'''Private Sub IGiantTimeSeriesRptPage_AppendTimeSeriesInterests _
'''    ( _
'''    ByRef ValuesArray() As Single, _
'''    ByRef StartRow As Long _
'''    )
'''
'''    Dim j As Long
'''    Dim ub As Long
'''    Dim lb As Long
'''
'''    lb = LBound(ValuesArray, 2)
'''    ub = UBound(ma_rValues, 1)
'''
'''    For j = 0 To ub
'''        ValuesArray(j + StartRow, lb + 0) = m_oHeader.CompanyWorkingInterest
'''        ValuesArray(j + StartRow, lb + 1) = m_oHeader.GovernmentParticipation
'''    Next j
'''
'''    StartRow = StartRow + ub + 1
'''
'''End Sub
'''
''''
'''' Append the names associated with the time series
'''' profiles for this page to the names array starting
'''' with the specified "row" (StartRow).
''''
'''Private Sub IGiantTimeSeriesRptPage_AppendTimeSeriesNames _
'''    ( _
'''    ByRef NamesArray() As String, _
'''    ByRef StartRow As Long, _
'''    ByVal ReportText As CReportText, _
'''    ByVal VariableTitles As IVariableTitlesA _
'''    )
'''
'''    Dim j As Long
'''    Dim ub As Long
'''    Dim lb As Long
'''    Dim l_ttl As String
'''    Dim l_pt As Integer
'''
'''    lb = LBound(NamesArray, 2)
'''    ub = UBound(ma_rValues, 1)
'''
'''    l_pt = m_oHeader.PageType
'''
'''    With ReportText
'''        If l_pt = 7 Or l_pt = 8 Then
'''            ' Depreciation/Cost Recovery has full variable title in section title
'''            l_ttl = VariableTitles.LongTitle(m_sVarCode)
'''        Else
'''            ' IRR worksheets have variable code only in section title
'''            l_ttl = m_sVarCode
'''        End If
'''        For j = 0 To ub
'''            NamesArray(j + StartRow, lb + 0) = Replace(.SectionTitle(l_pt), "|1|", l_ttl)
'''            NamesArray(j + StartRow, lb + 1) = .RowTitle(l_pt, j + 1)
'''        Next j
'''    End With
'''
'''    StartRow = StartRow + ub + 1
'''
'''End Sub
'''
'''Private Property Get IGiantTimeSeriesRptPage_ProfileElementCount() As Integer
'''    IGiantTimeSeriesRptPage_ProfileElementCount = m_oHeader.Rows
'''End Property
'''
'''Private Property Get IGiantTimeSeriesRptPage_TimeSeriesProfileCount() As Integer
'''    IGiantTimeSeriesRptPage_TimeSeriesProfileCount = m_oHeader.Columns
'''End Property



