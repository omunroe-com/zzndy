Option Strict Off
Option Explicit On
Friend Class CFmtGntRptPgDToASPENames
    Implements IDPersistFormat
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


    Private m_oReportText As CReportText

    Private m_oVariableTitles As IVariableTitlesA

    Public WriteOnly Property ReportText() As CReportText
        Set(ByVal Value As CReportText)

            m_oReportText = Value

        End Set
    End Property

    Public WriteOnly Property VariableTitles() As IVariableTitlesA
        Set(ByVal Value As IVariableTitlesA)

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
    ' Put the time series profile names on the store in
    ' profile major order.
    '
    Private Function IDPersistFormat_Store(ByVal TheStore As IDStore, ByVal TheObject As IDPersistObject) As Boolean Implements IDPersistFormat.Store

        Dim l_oStore As IDStoreSeq

        Dim i As Short
        Dim j As Short

        Dim l_sSection As String
        Dim l_sText As String

        Dim l_nPageType As Integer
        Dim l_nColumnCount As Integer

        Dim la_iProfTypes() As Short
        Dim la_sVarCodes() As String

        Dim l_oObject As IDGiantRptPageD

        l_oObject = TheObject
        With l_oObject
            With .PageHeader
                l_nPageType = .PageType
                l_nColumnCount = .Columns
            End With
            la_iProfTypes = VB6.CopyArray(.ProfileTypes)
            la_sVarCodes = VB6.CopyArray(.ProfileCodes)
        End With

        l_sSection = m_oReportText.SectionTitle(l_nPageType)

        ' Compute adjustment for array element indexing
        j = LBound(la_sVarCodes) - 1

        l_oStore = TheStore
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
End Class