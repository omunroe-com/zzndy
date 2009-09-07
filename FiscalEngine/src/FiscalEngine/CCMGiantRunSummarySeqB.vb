Option Strict Off
Option Explicit On
Friend Class CCMGiantRunSummarySeqB
    Implements IDPersistClassMap
	' Name:         CCMGiantRunSummarySeqB.cls
	' Function:     Giant Run Summary Report Format Map (sequential)
	' Date:         23 Sep 2004 JWD
	'---------------------------------------------------------
	' This provides formats for sequentially formatted Giant
	' report pages
	'
	' This specific map provides for the output of the run
	' summary data for an individual run. This data is stored
	' in the economic summary page. Other pages of the output
	' are not output.
	'---------------------------------------------------------
	
	
	
	'=========================================================
	'
	' IDPersistClassMap Interface
	'
	
    Public ReadOnly Property AsIDPersistClassMap() As IDPersistClassMap
        Get
            AsIDPersistClassMap = Me
        End Get
    End Property


    Private Function IDPersistClassMap_ClassFormat(ByVal TheObject As IDPersistObject) As IDPersistFormat Implements IDPersistClassMap.ClassFormat

        Select Case TheObject.ClassIDNumber
            Case 2
                IDPersistClassMap_ClassFormat = New CSFGiantRunSummarySeqB
            Case Else
                ' Return the do-nothing format
                IDPersistClassMap_ClassFormat = New CDPersistFormatDummy
        End Select

    End Function

    Private Function IDPersistClassMap_MakeObjectOfClass(ByVal TheClassName As String) As IDPersistObject Implements IDPersistClassMap.MakeObjectOfClass
        Throw New NotImplementedException()
    End Function
End Class