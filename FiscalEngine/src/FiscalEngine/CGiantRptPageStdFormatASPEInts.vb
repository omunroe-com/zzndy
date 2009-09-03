Option Strict Off
Option Explicit On
Friend Class CDFGiantRptPgStdASPEInts
    Implements IDPersistFormat
	' Name:         CGiantRptPageStdFormatASPEInts.cls
	' Function:     Giant Report page format (table)
	' Date:         29 Jan 2004 JWD
	'---------------------------------------------------------
	' A format for time series interests. This outputs the
	' interests associated with the profiles in profile major
	' order. There is a one-to-one correspondence between the
	' 'rows' of the interests array and the 'rows' of the
	' amounts array.
	'---------------------------------------------------------
	' Modifications:
	' 27 Feb 2004 JWD
	'  -> Changed class name to CDFGiantRptPgStdASPEInts
	'     to fit in 39 character programmatic ID limit.
	'---------------------------------------------------------
	
	
	
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

        Dim i As Short

        Dim l_oObject As IDGiantRptPageStd
        Dim l_oHdr As CGiantRptPageHdr1

        Dim l_oStore As IDStoreSeq

        l_oStore = TheStore

        l_oObject = TheObject
        l_oHdr = l_oObject.PageHeader

        With l_oStore
            ' For each profile
            For i = 1 To l_oHdr.Columns
                .NextItem = l_oHdr.CompanyWorkingInterest
                .NextItemLineEnd = l_oHdr.GovernmentParticipation
            Next i
        End With

    End Function
End Class