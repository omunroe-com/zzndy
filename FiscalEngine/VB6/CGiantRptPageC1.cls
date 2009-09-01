VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CGiantRptPageC1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' Name:         CGiantRptPageC1.cls
' Function:     Wrapper class for Giant Report Page Type
' Date:         27 Jan 2004 JWD
'---------------------------------------------------------
' This class wraps the PRT report page. A separate class
' is used for this because the file write statements are
' different from the standard page.
'---------------------------------------------------------
Option Explicit

Implements IDGiantRptPageStd
Implements IDPersistObject
'Implements IGiantTimeSeriesRptPage

'
' IDPersistObject attributes
'
Private Const m_lClassID As Long = 3
Private Const m_sClassName As String = "CGiantRptPageC1"
Private m_lObjectID As Long


Private m_oHeader As CGiantRptPageHdr1

Private ma_sTitles() As String
Private ma_rValues() As Single

'
' Set all elements of the page header.
'
' This is to replace the Write statement that writes
' this data on the report file.
'
Public Sub SetPageHeader _
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
' Set a profile (column) name
'
' This is to replace the Write statement that writes
' this data on the report file.
'
Public Property Let ProfileHeader _
    ( _
    ByVal ProfileIndex As Integer, _
    ByVal ProfileName As String _
    )
    
    ma_sTitles(ProfileIndex - 1) = ProfileName
    
End Property

'
' Set the value for a profile (column) at the specified row
'
' This is to replace the Write statement that writes
' this data on the report file.
'
Public Property Let ProfileElementValue _
    ( _
    ByVal ProfileIndex As Integer, _
    ByVal RowIndex As Integer, _
    ByVal NewValue As Single _
    )

    ma_rValues(ProfileIndex - 1, RowIndex - 1) = NewValue
    
End Property

Private Sub Class_Initialize()
    Set m_oHeader = New CGiantRptPageHdr1
End Sub


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
'''Private Sub IGiantTimeSeriesRptPage_AppendTimeSeriesValues(ByRef ValuesArray() As Single, ByRef StartRow As Long)
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
'''Private Sub IGiantTimeSeriesRptPage_AppendTimeSeriesInterests(ByRef ValuesArray() As Single, ByRef StartRow As Long)
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
'''        l_ttl = .SectionTitle(l_pt)
'''        For j = 0 To ub
'''            NamesArray(j + StartRow, lb + 0) = l_ttl
'''            NamesArray(j + StartRow, lb + 1) = .RowTitle(l_pt, j)
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


