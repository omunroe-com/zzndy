Option Strict Off
Option Explicit On
Friend Class CGiantRptPageC1
    Implements IDGiantRptPageStd
    Implements IDPersistObject
    ' Name:         CGiantRptPageC1.cls
    ' Function:     Wrapper class for Giant Report Page Type
    ' Date:         27 Jan 2004 JWD
    '---------------------------------------------------------
    ' This class wraps the PRT report page. A separate class
    ' is used for this because the file write statements are
    ' different from the standard page.
    '---------------------------------------------------------

    'Implements IGiantTimeSeriesRptPage

    '
    ' IDPersistObject attributes
    '
    Private Const m_lClassID As Integer = 3
    Private Const m_sClassName As String = "CGiantRptPageC1"
    Private m_lObjectID As Integer


    Private m_oHeader As CGiantRptPageHdr1

    Private ma_sTitles() As String
    Private ma_rValues(,) As Single

    '
    ' Set all elements of the page header.
    '
    ' This is to replace the Write statement that writes
    ' this data on the report file.
    '
    Public Sub SetPageHeader(ByVal PageType As Short, ByVal startyear As Short, ByVal PageCount As Short, ByVal ProjectLife As Short, ByVal ProfileCount As Short, ByVal PageTitle As String, ByVal ColumnWidth As Short, ByVal FinalWorkingInt As Single, ByVal FinalParticipation As Single, ByVal PageCurrency As String)


        With m_oHeader
            .PageType = PageType
            .year_Renamed = startyear
            .PageCounter = PageCount
            .Rows = ProjectLife
            .Columns = ProfileCount
            .PageTitle = PageTitle
            .ColumnWidth = ColumnWidth
            .CompanyWorkingInterest = FinalWorkingInt
            .GovernmentParticipation = FinalParticipation
            .CurrencyCode = PageCurrency
        End With

        ReDim ma_sTitles(ProfileCount - 1)
        ReDim ma_rValues(ProfileCount - 1, ProjectLife - 1)

    End Sub


    '
    ' Set a profile (column) name
    '
    ' This is to replace the Write statement that writes
    ' this data on the report file.
    '
    Public WriteOnly Property ProfileHeader(ByVal ProfileIndex As Short) As String
        Set(ByVal Value As String)

            ma_sTitles(ProfileIndex - 1) = Value

        End Set
    End Property

    '
    ' Set the value for a profile (column) at the specified row
    '
    ' This is to replace the Write statement that writes
    ' this data on the report file.
    '
    Public WriteOnly Property ProfileElementValue(ByVal ProfileIndex As Short, ByVal RowIndex As Short) As Single
        Set(ByVal Value As Single)

            ma_rValues(ProfileIndex - 1, RowIndex - 1) = Value

        End Set
    End Property


    '=========================================================
    '
    ' IDGiantRptPageStd Interface
    '



    Private Property IDGiantRptPageStd_Headers() As String() Implements IDGiantRptPageStd.Headers
        Get
            IDGiantRptPageStd_Headers = VB6.CopyArray(ma_sTitles)
        End Get
        Set(ByVal Value() As String)
            ma_sTitles = VB6.CopyArray(Value)
        End Set
    End Property


    Private Property IDGiantRptPageStd_PageHeader() As CGiantRptPageHdr1 Implements IDGiantRptPageStd.PageHeader
        Get
            IDGiantRptPageStd_PageHeader = m_oHeader
        End Get
        Set(ByVal Value As CGiantRptPageHdr1)
            m_oHeader = Value
        End Set
    End Property


    Private Property IDGiantRptPageStd_Values() As Single(,) Implements IDGiantRptPageStd.Values
        Get
            IDGiantRptPageStd_Values = VB6.CopyArray(ma_rValues)
        End Get
        Set(ByVal Value(,) As Single)
            ma_rValues = VB6.CopyArray(Value)
        End Set
    End Property


    '=========================================================
    '
    ' IDPersistObject Interface
    '

    '
    ' The first four are also attributes of IDObject
    ' since all IDPersistObjects are also IDObjects.
    '

    Private ReadOnly Property IDPersistObject_ClassIDNumber() As Integer Implements IDPersistObject.ClassIDNumber
        Get
            IDPersistObject_ClassIDNumber = m_lClassID
        End Get
    End Property

    Private ReadOnly Property IDPersistObject_ClassName() As String Implements IDPersistObject.ClassName
        Get
            IDPersistObject_ClassName = m_sClassName
        End Get
    End Property


    Private Property IDPersistObject_ObjectIDNumber() As Integer Implements IDPersistObject.ObjectIDNumber
        Get
            IDPersistObject_ObjectIDNumber = m_lObjectID
        End Get
        Set(ByVal Value As Integer)
            m_lObjectID = Value
        End Set
    End Property

    'UPGRADE_NOTE: Class_Initialize was upgraded to Class_Initialize_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    Private Sub Class_Initialize_Renamed()
        m_oHeader = New CGiantRptPageHdr1
    End Sub
    Public Sub New()
        MyBase.New()
        Class_Initialize_Renamed()
    End Sub

    '
    ' These methods are specific to IDPersistObjects
    '
    Private Function IDPersistObject_RegisterInTable(ByVal ObjectTable As IDPersistObjectTable) As Boolean Implements IDPersistObject.RegisterInTable
    End Function

    Private Function IDPersistObject_RegisterObjectConnections() As Boolean Implements IDPersistObject.RegisterObjectConnections
    End Function

    Private Function IDPersistObject_RestoreUsingFormat(ByVal TheStore As IDStore, ByVal TheMap As IDPersistClassMap) As Boolean Implements IDPersistObject.RestoreUsingFormat
    End Function

    Private Function IDPersistObject_StoreUsingFormat(ByVal TheStore As IDStore, ByVal TheMap As IDPersistClassMap) As Boolean Implements IDPersistObject.StoreUsingFormat

        'UPGRADE_WARNING: Couldn't resolve default property of object Me. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        TheMap.ClassFormat(Me).Store(TheStore, Me)

    End Function
	
	
	'=====================================================================
	' IGiantTimeSeriesRptPage Interface
	'
	
    ''
    '' Add the values to the specified array, beginning with specified row.
    ''
    '' StartRow is the subscript 1 index value into which the first
    '' page profile will be copied:
    ''   ValuesArray(StartRow, x) = ma_rValues(0, x),
    ''       for x = LBound(ma_rValues, 2) To UBound(ma_rValues,2)
    ''
    '' Assumes that the ValuesArray() is dimensioned to accomodate the
    '' data to be added.
    ''
    ''Private Sub IGiantTimeSeriesRptPage_AppendTimeSeriesValues(ByRef ValuesArray() As Single, ByRef StartRow As Long)
    ''
    ''    Dim i As Long
    ''    Dim j As Long
    ''    Dim L As Long
    ''
    ''    L = LBound(ValuesArray, 2)
    ''
    ''    For i = 0 To UBound(ma_rValues, 2)
    ''        For j = 0 To UBound(ma_rValues, 1)
    ''            ValuesArray(j + StartRow, i + L) = ma_rValues(j, i)
    ''        Next j
    ''    Next i
    ''
    ''    StartRow = StartRow + UBound(ma_rValues, 1) + 1
    ''
    ''End Sub
    ''
    ''
    '' Append the interests associated with the time series
    '' profiles for this page to the values array starting
    '' with the specified "row" (StartRow).
    ''
    ''Private Sub IGiantTimeSeriesRptPage_AppendTimeSeriesInterests(ByRef ValuesArray() As Single, ByRef StartRow As Long)
    ''
    ''    Dim j As Long
    ''    Dim ub As Long
    ''    Dim lb As Long
    ''
    ''    lb = LBound(ValuesArray, 2)
    ''    ub = UBound(ma_rValues, 1)
    ''
    ''    For j = 0 To ub
    ''        ValuesArray(j + StartRow, lb + 0) = m_oHeader.CompanyWorkingInterest
    ''        ValuesArray(j + StartRow, lb + 1) = m_oHeader.GovernmentParticipation
    ''    Next j
    ''
    ''    StartRow = StartRow + ub + 1
    ''
    ''End Sub
    ''
    ''
    '' Append the names associated with the time series
    '' profiles for this page to the names array starting
    '' with the specified "row" (StartRow).
    ''
    ''Private Sub IGiantTimeSeriesRptPage_AppendTimeSeriesNames _
    ''    ( _
    ''    ByRef NamesArray() As String, _
    ''    ByRef StartRow As Long, _
    ''    ByVal ReportText As CReportText, _
    ''    ByVal VariableTitles As IVariableTitlesA _
    ''    )
    ''
    ''    Dim j As Long
    ''    Dim ub As Long
    ''    Dim lb As Long
    ''    Dim l_ttl As String
    ''    Dim l_pt As Integer
    ''
    ''    lb = LBound(NamesArray, 2)
    ''    ub = UBound(ma_rValues, 1)
    ''
    ''    l_pt = m_oHeader.PageType
    ''
    ''    With ReportText
    ''        l_ttl = .SectionTitle(l_pt)
    ''        For j = 0 To ub
    ''            NamesArray(j + StartRow, lb + 0) = l_ttl
    ''            NamesArray(j + StartRow, lb + 1) = .RowTitle(l_pt, j)
    ''        Next j
    ''    End With
    ''
    ''    StartRow = StartRow + ub + 1
    ''
    ''End Sub
    ''
    ''Private Property Get IGiantTimeSeriesRptPage_ProfileElementCount() As Integer
    ''    IGiantTimeSeriesRptPage_ProfileElementCount = m_oHeader.Rows
    ''End Property
    ''
    ''Private Property Get IGiantTimeSeriesRptPage_TimeSeriesProfileCount() As Integer
    ''    IGiantTimeSeriesRptPage_TimeSeriesProfileCount = m_oHeader.Columns
    ''End Property
End Class