Option Strict Off
Option Explicit On
Friend Class CFmtGntRptPgGToASPENames
    Implements IDPersistFormat
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


    Private m_oReportText As _CReportText

    Private m_oVariableTitles As _IVariableTitlesA

    Public WriteOnly Property ReportText() As _CReportText
        Set(ByVal Value As _CReportText)

            m_oReportText = Value

        End Set
    End Property

    Public WriteOnly Property VariableTitles() As _IVariableTitlesA
        Set(ByVal Value As _IVariableTitlesA)

            m_oVariableTitles = Value

        End Set
    End Property


    '=========================================================
    '
    ' IDPersistFormat
    '

    Private Function IDPersistFormat_Restore(ByVal TheStore As IDStore, ByVal TheObject As IDPersistObject) As Boolean Implements IDPersistFormat.Restore
    End Function

    '
    ' Put the time series interests on the store in
    ' profile major order.
    '
    Private Function IDPersistFormat_Store(ByVal TheStore As IDStore, ByVal TheObject As IDPersistObject) As Boolean Implements IDPersistFormat.Store

        Dim l_oStore As IDStoreSeq

        Dim i As Short
        Dim k As Short
        Dim l_sSection As String
        Dim l_sVarCode As String
        Dim l_sText As String

        Dim l_nPageType As Integer
        Dim l_nColumnCount As Integer


        Dim l_oObject As IDGiantRptPageG

        l_oObject = TheObject
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

        l_oStore = TheStore
        With l_oStore
            ' For each profile
            For i = 1 To l_nColumnCount
                .NextItem = l_sSection
                .NextItemLineEnd = m_oReportText.RowTitle(l_nPageType, i)
            Next i
        End With

    End Function
End Class