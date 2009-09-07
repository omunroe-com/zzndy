Option Strict Off
Option Explicit On
Friend Class CDGiantRptPageMapSeqD
    Implements IDPersistClassMap
	' Name:         CDGiantRptPageMapSeqD.cls
	' Function:     Giant Report Page Format Map (sequential)
	' Date:         28 Jan 2004 JWD
	'---------------------------------------------------------
	' This provides formats for sequentially formatted Giant
	' report pages. This map supplies formats for the ASPE
	' Engine names for the pages and the profiles.
	'---------------------------------------------------------
	
	
    Private m_oReportText As CReportText
    Private m_oVariableTitles As IVariableTitlesA

    '
    ' The reference to the source of the names
    '
    Public WriteOnly Property ReportText() As CReportText
        Set(ByVal Value As CReportText)

            m_oReportText = Value

        End Set
    End Property
	
	'
	' For formats that need a reference to the variable
	' titles table to format the names.
	'
    Public WriteOnly Property VariableTitles() As IVariableTitlesA
        Set(ByVal Value As IVariableTitlesA)

            m_oVariableTitles = Value

        End Set
    End Property
	
	
	'=========================================================
	'
	' IDPersistClassMap Interface
	'
	
	
    Private Function IDPersistClassMap_ClassFormat(ByVal TheObject As IDPersistObject) As IDPersistFormat Implements IDPersistClassMap.ClassFormat

        Dim l_oFmtA As CFmtGntRptPgAToASPENames
        Dim l_oFmtD As CFmtGntRptPgDToASPENames
        Dim l_oFmtG As CFmtGntRptPgGToASPENames
        Dim l_oFmtH As CFmtGntRptPgHToASPENames

        Select Case TheObject.ClassIDNumber
            Case 1 ' Standard pages (A)
                l_oFmtA = New CFmtGntRptPgAToASPENames
                l_oFmtA.ReportText = m_oReportText
                IDPersistClassMap_ClassFormat = l_oFmtA

            Case 4 ' After-tax cash flow page (D)
                l_oFmtD = New CFmtGntRptPgDToASPENames
                l_oFmtD.ReportText = m_oReportText
                l_oFmtD.VariableTitles = m_oVariableTitles
                IDPersistClassMap_ClassFormat = l_oFmtD

            Case 7 ' Sub-schedule pages (G)
                l_oFmtG = New CFmtGntRptPgGToASPENames
                l_oFmtG.ReportText = m_oReportText
                l_oFmtG.VariableTitles = m_oVariableTitles
                IDPersistClassMap_ClassFormat = l_oFmtG

            Case 8 ' User-defined variable pages (H)
                l_oFmtH = New CFmtGntRptPgHToASPENames
                l_oFmtH.ReportText = m_oReportText
                l_oFmtH.VariableTitles = m_oVariableTitles
                IDPersistClassMap_ClassFormat = l_oFmtH

            Case Else
                ' Return the do-nothing format
                IDPersistClassMap_ClassFormat = New CDPersistFormatDummy

        End Select

    End Function
	
    Private Function IDPersistClassMap_MakeObjectOfClass(ByVal TheClassName As String) As IDPersistObject Implements IDPersistClassMap.MakeObjectOfClass
        Return Nothing
    End Function
End Class