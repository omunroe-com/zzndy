Option Strict Off
Option Explicit On
Friend Class CGiantRptPageE1
    Implements IDGiantRptPageStd
    Implements IDPersistObject
    Implements IGiantRptPageAssignStd
    ' Name:         CGiantRptPageE1.cls
    ' Function:     Wrapper class for Giant Report Page Type
    ' Date:         27 Jan 2004 JWD
    '---------------------------------------------------------
    ' Wraps the Venezuela Service Fee Page (17). This is
    ' essentially just a copy of CGiantRptPageA1. However, it
    ' does not implement the IGiantTimeSeriesRptPage interface
    ' because the Time Series pages are of annual time period
    ' while this page is quarterly. Also, it is not included
    ' in the Outputxxx arrays produced by the ASPEEngine.
    '---------------------------------------------------------


    '
    ' IDPersistObject attributes
    '
    Private Const m_lClassID As Integer = 5
    Private Const m_sClassName As String = "CGiantRptPageE1"
    Private m_lObjectID As Integer


    Private m_oHeader As CGiantRptPageHdr1

    Private ma_sTitles() As String
    Private ma_rValues(,) As Single


    'UPGRADE_NOTE: Class_Initialize was upgraded to Class_Initialize_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    Private Sub Class_Initialize_Renamed()
        m_oHeader = New CGiantRptPageHdr1
    End Sub
    Public Sub New()
        MyBase.New()
        Class_Initialize_Renamed()
    End Sub


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
    Private Sub IGiantRptPageAssignStd_SetPageHeader(ByVal PageType As Short, ByVal startyear As Short, ByVal PageCount As Short, ByVal ProjectLife As Short, ByVal ProfileCount As Short, ByVal PageTitle As String, ByVal ColumnWidth As Short, ByVal FinalWorkingInt As Single, ByVal FinalParticipation As Single, ByVal PageCurrency As String) Implements IGiantRptPageAssignStd.SetPageHeader

        Dim i As Short

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
    ' Set the profile (column) names
    '
    ' This is to replace the Write statement that writes
    ' this data on the report file.
    '
    'UPGRADE_WARNING: ParamArray ProfileNames was changed from ByRef to ByVal. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="93C6A0DC-8C99-429A-8696-35FC4DCEFCCC"'
    Private Sub IGiantRptPageAssignStd_SetProfileHeaders(ByVal ParamArray ProfileNames() As Object) Implements IGiantRptPageAssignStd.SetProfileHeaders

        Dim i As Short
        Dim k As Short

        k = LBound(ProfileNames)

        For i = 0 To UBound(ma_sTitles)
            'UPGRADE_WARNING: Couldn't resolve default property of object ProfileNames(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
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
    'UPGRADE_WARNING: ParamArray ProfileValues was changed from ByRef to ByVal. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="93C6A0DC-8C99-429A-8696-35FC4DCEFCCC"'
    Private Sub IGiantRptPageAssignStd_SetProfileValues(ByVal RowIndex As Short, ByVal ParamArray ProfileValues() As Object) Implements IGiantRptPageAssignStd.SetProfileValues

        Dim i As Short
        Dim k As Short

        k = LBound(ProfileValues)

        For i = 0 To UBound(ma_rValues, 1)
            'UPGRADE_WARNING: Couldn't resolve default property of object ProfileValues(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            ma_rValues(i, RowIndex - 1) = ProfileValues(i + k)
        Next i

    End Sub
End Class