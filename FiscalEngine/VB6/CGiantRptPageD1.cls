VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CGiantRptPageD1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' Name:         CGiantRptPageD1.cls
' Function:     Wrapper class for Giant Report Page Type
' Date:         27 Jan 2004 JWD
'---------------------------------------------------------
' This class wraps the after tax cash flow report page.
' A separate class is used for this because the file write
' statements are different from the standard page.
'---------------------------------------------------------
Option Explicit

Implements IDGiantRptPageD              ' For ASPE Engine
Implements IDGiantRptPageStd            ' For standard output
Implements IDPersistObject
'Implements IGiantTimeSeriesRptPage

'
' IDPersistObject attributes
'
Private Const m_lClassID As Long = 4
Private Const m_sClassName As String = "CGiantRptPageD1"
Private m_lObjectID As Long


Private m_oHeader As CGiantRptPageHdr1

Private ma_sTitles() As String
Private ma_rValues() As Single

' Following stores the "type" of the profile
' Type numbers represent predefined profiles
' (Positive cash flow total, negative cash flow
' total, Net cash flow, etc.) and user-defined
' profiles (fiscal variables). Values 1-7 are
' predefined, value 8 represents user-defined.
Private ma_iProfTypes() As Integer

' Following stores the user-defined profile
' titles to be used for ASPEEngine service. This
' array has a one-to-one correspondence with the
' ma_iProfType() array above and provides the
' text for those elements with a type of "8".
' The other types have predefined text that is
' retrieved from the CReportText object.
Private ma_sVarCodes() As String

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
    ReDim ma_iProfTypes(0 To ProfileCount - 1)
    ReDim ma_sVarCodes(0 To ProfileCount - 1)
    
End Sub

'
' This assumes that the ProfileNames array is dimensioned
' as ReDim ProfileNames(0 To ColumnCount) (Option Base 0)
' but that the usage is 1 To ColumnCount. This is the way
' the ColumnNm$() array that is passed in is dimensioned
' and used.
'
Public Sub SetProfileHeaders _
    ( _
    ByRef ProfileNames() As String _
    )
    
    Dim i As Integer
    Dim k As Integer
    
    k = 1 ' LBound(ProfileNames)
    
    For i = 0 To UBound(ma_sTitles)
        ma_sTitles(i) = ProfileNames(i + k)
    Next i
    
End Sub

'
' Set the profile type and user-defined variable title
' text.
'
' This is used to prepare text for the ASPEEngine
' OutputNames() array.
'
Public Sub SetProfileTypesAndTitles _
    ( _
    ByRef ProfileTypes() As Integer, _
    ByRef ProfileTitles() As String _
    )
    
    Dim i As Integer
    Dim k As Integer
    
    k = 1
    
    For i = 0 To UBound(ma_iProfTypes)
        ma_iProfTypes(i) = ProfileTypes(i + k)
        ma_sVarCodes(i) = ProfileTitles(i + k)
    Next i
        
End Sub

'
' Set the values for all profiles for the specified row
'
' This is to replace the Write statement that writes
' this data on the report file.
'
' This assumes that the ProfileValues array is dimensioned
' as ReDim ProfileValues(0 To LG, 0 To ColumnCount) (Option
' Base 0) but that the usage is 1 To LG, 1 To ColumnCount.
' This is the way the DUM() array that is passed in is
' dimensioned and used.
'
Public Sub SetProfileValues _
    ( _
    ByRef ProfileValues() As Single _
    )

    Dim i As Integer
    Dim j As Integer
    
    Dim k As Integer
    Dim L As Integer
    
    k = 1  ' LBound(ProfileValues, 1)
    L = 1  ' LBound(ProfileValues, 2)
    
    For i = 0 To UBound(ma_rValues, 2)
        For j = 0 To UBound(ma_rValues, 1)
            ma_rValues(j, i) = ProfileValues(i + k, j + L)
        Next j
    Next i
    
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
    
    Dim i As Integer
    
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
' IDGiantRptPageD Interface
'

Private Property Let IDGiantRptPageD_Headers _
    ( _
    ByRef NewValues() As String _
    )
    ma_sTitles = NewValues
End Property

Private Property Get IDGiantRptPageD_Headers _
    () As String()
    IDGiantRptPageD_Headers = ma_sTitles
End Property

Private Property Set IDGiantRptPageD_PageHeader _
    ( _
    ByVal NewObject As CGiantRptPageHdr1 _
    )
    Set m_oHeader = NewObject
End Property

Private Property Get IDGiantRptPageD_PageHeader _
    () As CGiantRptPageHdr1
    Set IDGiantRptPageD_PageHeader = m_oHeader
End Property

Private Property Let IDGiantRptPageD_ProfileCodes _
    ( _
    ByRef NewValues() As String _
    )
    ma_sVarCodes = NewValues
End Property

Private Property Get IDGiantRptPageD_ProfileCodes _
    () As String()
    IDGiantRptPageD_ProfileCodes = ma_sVarCodes
End Property

Private Property Let IDGiantRptPageD_ProfileTypes _
    ( _
    ByRef NewValues() As Integer _
    )
    ma_iProfTypes = NewValues
End Property

Private Property Get IDGiantRptPageD_ProfileTypes _
    () As Integer()
    IDGiantRptPageD_ProfileTypes = ma_iProfTypes
End Property

Private Property Let IDGiantRptPageD_Values _
    ( _
    ByRef NewValues() As Single _
    )
    ma_rValues = NewValues
End Property

Private Property Get IDGiantRptPageD_Values _
    () As Single()
    IDGiantRptPageD_Values = ma_rValues
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
'''
'''    Dim l_ttl As String
'''    Dim l_pt As Integer
'''    Dim l_vttl As String
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
'''            l_vttl = VariableTitles.LongTitle(ma_sVarCodes(j))
'''            If Len(l_vttl) > 0 Then
'''                l_vttl = l_vttl & " (" & ma_sVarCodes(j) & ")"
'''            Else
'''                l_vttl = ma_sVarCodes(j)
'''            End If
'''            NamesArray(j + StartRow, lb + 1) = Replace(.RowTitle(l_pt, ma_iProfTypes(j)), "|1|", l_vttl)
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


