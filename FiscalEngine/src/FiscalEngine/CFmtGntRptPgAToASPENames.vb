Option Strict Off
Option Explicit On
Friend Class CFmtGntRptPgAToASPENames
    Implements IDPersistFormat
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


    Private m_oReportText As CReportText


    Public WriteOnly Property ReportText() As CReportText
        Set(ByVal Value As CReportText)

            m_oReportText = Value

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
        Dim l_sSection As String

        Dim l_nPageType As Integer
        Dim l_nColumnCount As Integer


        Dim l_oObject As IDGiantRptPageStd

        l_oObject = TheObject
        With l_oObject.PageHeader
            l_nPageType = .PageType
            l_nColumnCount = .Columns
        End With

        l_sSection = m_oReportText.SectionTitle(l_nPageType)

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