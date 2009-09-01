Option Strict Off
Option Explicit On
Friend Class CGiantRptPagePRTFormatSeq
    Implements IDPersistFormat
    ' Name:         CGiantRptPagePRTFormatSeq.cls
    ' Function:     Sequential text Giant Report page format
    ' Date:         28 Jan 2004 JWD
    '---------------------------------------------------------
    ' Format for PRT report page
    '---------------------------------------------------------



    '=========================================================
    '
    ' IDPersistFormat
    '

    Private Function IDPersistFormat_Restore(ByVal TheStore As IDStore, ByVal TheObject As IDPersistObject) As Boolean Implements IDPersistFormat.Restore
    End Function

    Private Function IDPersistFormat_Store(ByVal TheStore As IDStore, ByVal TheObject As IDPersistObject) As Boolean Implements IDPersistFormat.Store

        Dim i As Short
        Dim k As Short

        Dim la_sHeaders() As String
        Dim la_rValues(,) As Single

        Dim l_oObject As IDGiantRptPageStd
        Dim l_oStore As IDStoreSeq

        l_oStore = TheStore
        l_oObject = TheObject

        la_sHeaders = VB6.CopyArray(l_oObject.Headers)
        la_rValues = VB6.CopyArray(l_oObject.Values)

        With l_oStore

            For i = LBound(la_sHeaders) To UBound(la_sHeaders)
                .NextItemLineEnd = la_sHeaders(i) ' Write #FileHandle, ma_sTitles(i)
            Next i

            For k = LBound(la_rValues, 2) To UBound(la_rValues, 2)
                For i = LBound(la_rValues, 1) To UBound(la_rValues, 1)
                    .NextItemLineEnd = la_rValues(i, k) ' Write #FileHandle, ma_rValues(i, k)
                Next i
            Next k
        End With

    End Function
End Class