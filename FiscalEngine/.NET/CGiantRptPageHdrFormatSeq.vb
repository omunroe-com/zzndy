Option Strict Off
Option Explicit On
Friend Class CGiantRptPageHdrFormatSeq
    Implements IDPersistFormat
	
	
	
	'=========================================================
	'
	' IDPersistFormat
	'
	
    Private Function IDPersistFormat_Restore(ByVal TheStore As IDStore, ByVal TheObject As IDPersistObject) As Boolean Implements IDPersistFormat.Restore
    End Function
	
    Private Function IDPersistFormat_Store(ByVal TheStore As IDStore, ByVal TheObject As IDPersistObject) As Boolean Implements IDPersistFormat.Store

        Dim l_oStore As IDStoreSeq
        Dim l_oObject As CGiantRptPageHdr1

        l_oStore = TheStore
        l_oObject = TheObject

        With l_oStore
            .NextItem = l_oObject.PageType
            .NextItem = l_oObject.year_Renamed
            .NextItem = l_oObject.PageCounter
            .NextItem = l_oObject.Rows
            .NextItem = l_oObject.Columns
            .NextItem = l_oObject.PageTitle
            .NextItem = l_oObject.ColumnWidth
            .NextItem = l_oObject.CompanyWorkingInterest
            .NextItem = l_oObject.GovernmentParticipation
            .NextItemLineEnd = l_oObject.CurrencyCode
        End With

    End Function
End Class