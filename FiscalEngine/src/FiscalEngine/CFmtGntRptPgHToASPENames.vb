Option Strict Off
Option Explicit On
Friend Class CFmtGntRptPgHToASPENames
    Implements IDPersistFormat
    ' Name:         CFmtGntRptPgHToASPENames.cls
    ' Function:     Giant Report page format (table)
    ' Date:         29 Jan 2004 JWD
    '---------------------------------------------------------
    ' A sequential format for time series names. This outputs
    ' the names associated with the profiles in profile major
    ' order. There is a one-to-one correspondence between the
    ' 'rows' of the names array and the 'rows' of the amounts
    ' array.
    '---------------------------------------------------------
    ' This implementation provides names for the user-defined
    ' variable page and columns as provided by the CReportText
    ' object. This format performs substitutions of variable
    ' names into the section title and row titles.
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
    ' Put the time series interests on the store in
    ' profile major order.
    '
    Private Function IDPersistFormat_Store(ByVal TheStore As IDStore, ByVal TheObject As IDPersistObject) As Boolean Implements IDPersistFormat.Store

        Dim l_oStore As IDStoreSeq

        Dim i As Short
        Dim j As Short
        Dim l_sSection As String
        Dim l_sVarCode As String
        Dim l_sText As String
        Dim la_sCodes() As String
        Dim la_sTitles() As String

        Dim l_nPageType As Integer
        Dim l_nColumnCount As Integer

        Dim l_oObject As IDGiantRptPageH

        Const l_sReplStr As String = "|1|"


        l_oObject = TheObject
        With l_oObject
            l_sVarCode = .VariableCode
            With .PageHeader
                l_nPageType = .PageType
                l_nColumnCount = .Columns
            End With
            la_sCodes = VB6.CopyArray(.Headers)
            'UPGRADE_WARNING: Lower bound of array la_sTitles was changed from LBound(la_sCodes) to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ReDim la_sTitles(UBound(la_sCodes))
        End With



        ' Resolve the page title (variable title)
        l_sText = m_oVariableTitles.LongTitle(l_sVarCode)
        If Len(l_sText) > 0 Then
            l_sText = l_sText & " (" & l_sVarCode & ")"
        Else
            l_sText = l_sVarCode
        End If
        l_sSection = Replace(m_oReportText.SectionTitle(l_nPageType), l_sReplStr, l_sText)



        ' Resolve row titles
        ' First resolve deduction variable codes into titles text
        ' Reusing l_sVarCode symbol for deduction variable code
        For i = LBound(la_sCodes) + 3 To LBound(la_sCodes) + 7
            ' substitute deduction variable names
            ' Prepare the deductions subtitle text
            ' Get the code from the titles array
            l_sVarCode = Trim(la_sCodes(i))

            If Len(l_sVarCode) > 0 Then
                If Not m_oVariableTitles.IsUserVariable(l_sVarCode) Or (StrComp(l_sVarCode, "DPR", CompareMethod.Text) = 0) Then
                    ' Regardless of whether or not there is a
                    ' user-defined variable with the code DPR,
                    ' when it appears in the Variable report
                    ' page, it is the depreciation schedule.

                    l_sText = m_oReportText.ForecastTitle(l_sVarCode)

                    ' Hopefully TEMPORARY?????
                    ' See if this is a volume forecast name
                    If IsVolumeForecast(l_sVarCode) Then
                        ' This is a volume forecast, so substitute into the
                        ' generic revenue forecast.
                        l_sText = Replace(m_oReportText.ForecastTitle("REV"), l_sReplStr, l_sText)
                    End If
                Else
                    l_sText = m_oVariableTitles.LongTitle(l_sVarCode)
                    If Len(l_sText) > 0 Then
                        l_sText = l_sText & " (" & l_sVarCode & ")"
                    Else
                        l_sText = l_sVarCode
                    End If
                End If
            Else
                l_sText = vbNullString
            End If
            ' Save resolved variable title text
            la_sTitles(i) = l_sText
        Next i

        ' Compute adjustment for array element indexing
        j = LBound(la_sCodes) - 1

        ' Output to the store
        l_oStore = TheStore
        With l_oStore
            ' For each profile
            For i = 1 To l_nColumnCount
                .NextItem = l_sSection
                .NextItemLineEnd = Replace(m_oReportText.RowTitle(l_nPageType, i), l_sReplStr, la_sTitles(i + j))
            Next i
        End With

    End Function
End Class