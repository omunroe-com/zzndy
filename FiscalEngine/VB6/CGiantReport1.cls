VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CGiantReport1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' Name:         CGiantReport1.cls
' Function:     Giant Output Report (prn)
' Date:         28 Jan 2004 JWD
'---------------------------------------------------------
' Modifications:
' 9 Mar 2004 JWD
'  -> Changed WriteReport().
'---------------------------------------------------------
Option Explicit

Implements IVariableTitlesA
Implements CReportText

Private m_oPages As Collection
Private m_oTSPages As Collection
Private m_oEcoSumm As CGiantRptPageB1

Private m_sUsrCodesList As String

Private Const m_sKeyWordsList As String = "CURFINITBITELMTPARWIN"

Private m_oReportText As CReportText

Public Property Set ReportText _
    ( _
    ByVal NewObject As CReportText _
    )
    
    Set m_oReportText = NewObject
    
End Property

Public Property Get ReportText _
    () As CReportText
    
    Set ReportText = m_oReportText
    
End Property


Public Function NewStandardRptPage _
    () As IGiantRptPageAssignStd

    Dim oPg As CGiantRptPageA1

    Set oPg = New CGiantRptPageA1
    m_oPages.Add oPg
    m_oTSPages.Add oPg
    
    Set NewStandardRptPage = oPg
    
End Function

Public Function NewStandardRptPageSpecial _
    ( _
    ByVal PageTypeCode As Integer _
    ) As IGiantRptPageAssignStd

    Dim oPg As IGiantRptPageAssignStd
    
    Select Case PageTypeCode
    Case 12             ' Variable page
        Set oPg = New CGiantRptPageH1
        m_oPages.Add oPg
        m_oTSPages.Add oPg
        
    Case 17             ' Venezuela Service Fee page
        Set oPg = New CGiantRptPageE1
        m_oPages.Add oPg
        
        ' Not adding to Time Series pages for now because
        ' this page is quarterly, and others are annual
        'm_oTSPages.Add oPg
    
    Case 22             ' Colombia Participation worksheet
        Set oPg = New CGiantRptPageF1
        m_oPages.Add oPg
        
        ' Not adding to Time Series pages for now because
        ' this page is monthly, and others are annual
        'm_oTSPages.Add oPg
    
    End Select
    
    Set NewStandardRptPageSpecial = oPg
    
End Function

Public Function NewEconomicSummaryPage() As CGiantRptPageB1

    Dim oPg As CGiantRptPageB1

    Set oPg = New CGiantRptPageB1
    'm_oPages.Add oPg
    
    Set m_oEcoSumm = oPg
    
    Set NewEconomicSummaryPage = oPg
    
End Function

Public Function NewPRTRptPage() As CGiantRptPageC1
    
    Dim oPg As CGiantRptPageC1

    Set oPg = New CGiantRptPageC1
    m_oPages.Add oPg
    m_oTSPages.Add oPg
    
    Set NewPRTRptPage = oPg
    
End Function

Public Function NewAfterTaxCashflowRptPage() As CGiantRptPageD1

    Dim oPg As CGiantRptPageD1

    Set oPg = New CGiantRptPageD1
    m_oPages.Add oPg
    m_oTSPages.Add oPg
    
    Set NewAfterTaxCashflowRptPage = oPg
    
End Function


'
' SubSchedule report pages are supporting schedule
' pages that are associated with another page,
' usually a user-defined variable page and as such
' have additional data to assign to the page, such
' as the code for the supported page.
'
Public Function NewSubScheduleRptPage _
    ( _
    ByVal PageTypeCode As Integer _
    ) As IGiantRptPageAssignSub
    
    Dim oPg As IGiantRptPageAssignSub
    
    Select Case PageTypeCode
    Case 7, 8               ' depreciation/cost recovery schedules
        Set oPg = New CGiantRptPageG1
        m_oPages.Add oPg
        m_oTSPages.Add oPg
        
    Case 18, 19, 20, 21     ' IRR/RTO/RT1 worksheet pages
        Set oPg = New CGiantRptPageG1
        m_oPages.Add oPg
        m_oTSPages.Add oPg
        
    End Select
    
    Set NewSubScheduleRptPage = oPg
    
End Function

Private Sub Class_Initialize()
    Set m_oPages = New Collection
    Set m_oTSPages = New Collection
End Sub

'
' Return the count of time series profiles in all
' pages of the report.
'
Public Function TimeSeriesProfileCount() As Long

    Dim l_count As Long
    Dim l_page As IDGiantRptPageStd
    
    l_count = 0
    For Each l_page In m_oTSPages
        l_count = l_count + l_page.PageHeader.Columns
    Next l_page
    
    TimeSeriesProfileCount = l_count
    
End Function

'
' Return the maximum count of elements in all of the
' time series profiles in all pages of the report.
'
Public Function MaxProfileElementCount() As Long

    Dim l_count As Long
    Dim l_max As Long
    Dim l_page As IDGiantRptPageStd
    
    l_max = 0
    For Each l_page In m_oTSPages
        l_count = l_page.PageHeader.Rows
        If l_count > l_max Then
            l_max = l_count
        End If
    Next l_page

    MaxProfileElementCount = l_max
    
End Function

'
' Modifications:
' 9 Mar 2004 JWD
'  -> Change to condition output of economic summary page
'     on existence of an instance of the page.
'
Public Sub WriteReport _
    ( _
    ByVal TheStore As IDStore, _
    ByVal TheMap As IDPersistClassMap _
    )
    
    Dim l_page As IDPersistObject
    Dim l_eco As IDGiantRptPageInd
    Dim l_std As IDGiantRptPageStd
    Dim l_obj As IDPersistObject
    
    ' TEMPORARY ??????
    zzzCreateUserVariableCodeList
    
    ' Output the regular report pages
    For Each l_page In m_oPages
        ' To get the page header to store it
        Set l_std = l_page
        Set l_obj = l_std.PageHeader
        l_obj.StoreUsingFormat TheStore, TheMap
        l_page.StoreUsingFormat TheStore, TheMap
    Next l_page
    
    ' Output the economic summary
    ' Condition on page existence
    If Not m_oEcoSumm Is Nothing Then
        Set l_eco = m_oEcoSumm
        
        Set l_obj = l_eco.PageHeader
        l_obj.StoreUsingFormat TheStore, TheMap
        
        Set l_obj = l_eco
        l_obj.StoreUsingFormat TheStore, TheMap
    End If
    
End Sub

''''Public Function TimeSeriesOutputValues _
''''    ( _
''''    ) As Single()
''''
''''    Dim l_rows As Long
''''    Dim l_columns As Long
''''    Dim a_rValues() As Single
''''    Dim l_page As IGiantTimeSeriesRptPage
''''
''''    ' Go through the report, getting the count of
''''    ' profiles (rows in the output array) and the
''''    ' 'life' (number of columns in output array).
''''    l_rows = TimeSeriesProfileCount
''''    l_columns = MaxProfileElementCount
''''
''''    ' Allocate the time series output arrays
''''    ReDim a_rValues(1 To l_rows, 1 To l_columns)
''''
''''    ' reuse l_rows as start row
''''    l_rows = 1
''''    For Each l_page In m_oTSPages
''''        With l_page
''''            .AppendTimeSeriesValues a_rValues(), l_rows
''''        End With
''''    Next l_page
''''
''''    TimeSeriesOutputValues = a_rValues
''''
''''End Function
''''
''''Public Function TimeSeriesOutputNames _
''''    ( _
''''    ByVal ReportText As CReportText _
''''    ) As String()
''''
''''    Dim l_rows As Long
''''    Dim a_sNames() As String
''''    Dim l_page As IGiantTimeSeriesRptPage
''''
''''    ' Create a user variable codes list for the
''''    ' country file for output report purposes
''''    zzzCreateUserVariableCodeList
''''
''''    ' Go through the report, getting the count of
''''    ' profiles (rows in the output array) and the
''''    ' 'life' (number of columns in output array).
''''    l_rows = TimeSeriesProfileCount
''''
''''    ' Allocate the time series output arrays
''''    ReDim a_sNames(1 To l_rows, 1 To 2)
''''
''''    ' reuse l_rows as start row
''''    l_rows = 1
''''    For Each l_page In m_oTSPages
''''        With l_page
''''            .AppendTimeSeriesNames a_sNames(), l_rows, ReportText, Me
''''        End With
''''    Next l_page
''''
''''    TimeSeriesOutputNames = a_sNames
''''
''''End Function
''''
''''Public Function TimeSeriesOutputInterests _
''''    ( _
''''    ) As Single()
''''
''''    Dim l_rows As Long
''''    Dim a_rValues() As Single
''''    Dim l_page As IGiantTimeSeriesRptPage
''''
''''    ' Go through the report, getting the count of
''''    ' profiles (rows in the output array) and the
''''    ' 'life' (number of columns in output array).
''''    l_rows = TimeSeriesProfileCount
''''
''''    ' Allocate the time series output arrays
''''    ReDim a_rValues(1 To l_rows, 1 To 2)
''''
''''    ' reuse l_rows as start row
''''    l_rows = 1
''''    For Each l_page In m_oTSPages
''''        With l_page
''''            .AppendTimeSeriesInterests a_rValues(), l_rows
''''        End With
''''    Next l_page
''''
''''    TimeSeriesOutputInterests = a_rValues
''''
''''End Function

'
' Function to return the present value table for the run.
'
Public Function PresentValueTable() As Single()
    PresentValueTable = m_oEcoSumm.PresentValueTable
End Function

'
' Property to return the company rate of return for the run
'
Public Property Get CompanyRateOfReturn() As Single
    CompanyRateOfReturn = m_oEcoSumm.CompanyRateOfReturn
End Property

'
' Property to return the government rate of return for the run
'
Public Property Get GovernmentRateOfReturn() As Single
    GovernmentRateOfReturn = m_oEcoSumm.GovernmentRateOfReturn
End Property

'
'
'
Private Sub zzzCreateUserVariableCodeList()
      
    Dim i As Integer
    Dim j As Integer
    
    ' Create a code string containing the user-defined
    ' variable codes. This is for the output report.
    For i = 1 To TDT
       ' See if the code is one of the reserved codes
       SearchCodeString m_sKeyWordsList, TD$(i, 1), 3, j
       If j = 0 Then        ' It is a user defined variable
          m_sUsrCodesList = m_sUsrCodesList & TD$(i, 1)
       End If
    Next i

End Sub


'=========================================================
'
' CReportText Interface
'

Private Property Get CReportText_ForecastTitle _
    ( _
    ByVal ForecastCode As String _
    ) As String

    ' Delegate to actual report text object
    CReportText_ForecastTitle = m_oReportText.ForecastTitle(ForecastCode)
    
End Property

Private Property Let CReportText_ForecastTitle _
    ( _
    ByVal ForecastCode As String, _
    ByVal NewValue As String _
    )
    
    ' This object exposes this interface for retrieval
    ' only.
    
End Property

Private Function CReportText_RowTitle _
    ( _
    ByVal PageTypeID As Integer, _
    ByVal RowID As Integer _
    ) As String

    ' Delegate to actual report text object
    CReportText_RowTitle = m_oReportText.RowTitle(PageTypeID, RowID)
    
End Function

Private Function CReportText_RowUnit _
    ( _
    ByVal PageTypeID As Integer, _
    ByVal RowID As Integer _
    ) As String

    ' Delegate to actual report text object
    CReportText_RowUnit = m_oReportText.RowUnit(PageTypeID, RowID)
    
End Function

Private Function CReportText_SectionTitle _
    ( _
    ByVal PageTypeID As Integer _
    ) As String

    ' Delegate to actual report text object
    CReportText_SectionTitle = m_oReportText.SectionTitle(PageTypeID)
    
End Function

'=========================================================
'
' IVariableTitlesA Interface
'
' Just makes this class look like a variable titles table
' object to the report pages.
'

'
' Confirm whether or not the variable code specified
' is a user-defined variable. Abstracts how this is
' done.
'
Private Function IVariableTitlesA_IsUserVariable _
    ( _
    ByVal VariableCode As String _
    ) As Boolean

    Dim i As Integer
    
    SearchCodeString m_sUsrCodesList, VariableCode, 3, i
    
    IVariableTitlesA_IsUserVariable = (Not (i = 0))
    
End Function

'
' Return the long title of the variable identified
' by the VariableCode.
'
' Done this way to localize the references to the global
' symbols TL$() & TLT.
'
Private Function IVariableTitlesA_LongTitle _
    ( _
    ByVal VariableCode As String _
    ) As String
    
    Dim i As Integer
    Dim long_title As String
    
    long_title = vbNullString      ' VariableCode
    
    For i = 1 To TLT
        If StrComp(TL$(i, 1), VariableCode, vbTextCompare) = 0 Then
            long_title = Trim(TL$(i, 3))    ' & " (" & VariableCode & ")"
            Exit For
        End If
    Next i
    
    IVariableTitlesA_LongTitle = long_title

End Function
