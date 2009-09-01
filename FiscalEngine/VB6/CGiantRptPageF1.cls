VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CGiantRptPageF1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' Name:         CGiantRptPageF1.cls
' Function:     Wrapper class for Giant Report Page Type
' Date:         27 Jan 2004 JWD
'---------------------------------------------------------
' Wraps the Colombia Participation Worksheet Page (22).
' This is essentially just a copy of CGiantRptPageE1.
' Like CGiantRptPageE1, it does not implement the time
' series report page interface IGiantTimeSeriesRptPage
' because the Time Series pages are of annual time period
' while this page is monthly. Also, it is not included
' in the Outputxxx arrays produced by the ASPEEngine.
'---------------------------------------------------------
Option Explicit

Implements IDGiantRptPageStd
Implements IDPersistObject
Implements IGiantRptPageAssignStd

'
' IDPersistObject attributes
'
Private Const m_lClassID As Long = 6
Private Const m_sClassName As String = "CGiantRptPageF1"
Private m_lObjectID As Long


Private m_oHeader As CGiantRptPageHdr1

Private ma_sTitles() As String
Private ma_rValues() As Single


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


'=========================================================
'
' IGiantRptPageAssignStd Interface
'

'
' Set all elements of the page header.
'
' This is to replace the Write statement that writes
' this data on the report file.
'
Private Sub IGiantRptPageAssignStd_SetPageHeader _
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
Private Sub IGiantRptPageAssignStd_SetProfileHeaders _
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
Private Sub IGiantRptPageAssignStd_SetProfileValues _
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




