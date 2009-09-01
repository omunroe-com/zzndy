VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CFmtGntRptPgHToASPENames"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
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
Option Explicit

Implements IDPersistFormat

Private m_oReportText As CReportText

Private m_oVariableTitles As IVariableTitlesA

Public Property Set ReportText _
    ( _
    ByVal NewObject As CReportText _
    )
        
    Set m_oReportText = NewObject
    
End Property

Public Property Set VariableTitles _
    ( _
    ByVal NewObject As IVariableTitlesA _
    )
    
    Set m_oVariableTitles = NewObject
    
End Property
    
    
'=========================================================
'
' IDPersistFormat
'

Private Function IDPersistFormat_Restore _
    ( _
    ByVal TheStore As IDStore, _
    ByVal TheObject As IDPersistObject _
    ) As Boolean
End Function

'
' Put the time series interests on the store in
' profile major order.
'
Private Function IDPersistFormat_Store _
    ( _
    ByVal TheStore As IDStore, _
    ByVal TheObject As IDPersistObject _
    ) As Boolean
    
    Dim l_oStore As IDStoreSeq
        
    Dim i As Integer
    Dim j As Integer
    Dim l_sSection As String
    Dim l_sVarCode As String
    Dim l_sText As String
    Dim la_sCodes() As String
    Dim la_sTitles() As String
    
    Dim l_nPageType As Long
    Dim l_nColumnCount As Long
    
    Dim l_oObject As IDGiantRptPageH
    
    Const l_sReplStr As String = "|1|"
    
    
    Set l_oObject = TheObject
    With l_oObject
        l_sVarCode = .VariableCode
        With .PageHeader
            l_nPageType = .PageType
            l_nColumnCount = .Columns
        End With
        la_sCodes = .Headers
        ReDim la_sTitles(LBound(la_sCodes) To UBound(la_sCodes))
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
            If Not m_oVariableTitles.IsUserVariable(l_sVarCode) Or (StrComp(l_sVarCode, "DPR", vbTextCompare) = 0) Then
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
    Set l_oStore = TheStore
    With l_oStore
        ' For each profile
        For i = 1 To l_nColumnCount
            .NextItem = l_sSection
            .NextItemLineEnd = Replace(m_oReportText.RowTitle(l_nPageType, i), l_sReplStr, la_sTitles(i + j))
        Next i
    End With
    
End Function
