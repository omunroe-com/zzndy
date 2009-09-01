Option Strict Off
Option Explicit On
Friend Class CGiantRptPageIndFormatSeq
    Implements IDPersistFormat
    ' Name:         CGiantRptPageIndFormatSeq.cls
    ' Function:     Sequential text Giant Report page format
    ' Date:         28 Jan 2004 JWD
    '---------------------------------------------------------
    ' Format for economic indicators page
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

        Dim la_sTitles() As String
        Dim la_rRates() As Single
        Dim la_rValues(,) As Single

        ' Interface references
        Dim l_oObject As IDGiantRptPageInd ' Stores Economic Indicator pages
        Dim l_oStore As IDStoreSeq ' in sequential text format

        ' Get the desired interfaces from the object and store
        l_oObject = TheObject
        l_oStore = TheStore

        ' Write out the Economic Summary Page

        With l_oObject
            la_sTitles = VB6.CopyArray(.Headers)
            la_rRates = VB6.CopyArray(.Rates)
            la_rValues = VB6.CopyArray(.Values)
        End With

        With l_oStore
            For i = LBound(la_rRates) To UBound(la_rRates) - 1
                .NextItem = la_rRates(i) ' Write #FileHandle, ma_rRates(i);
            Next i
            .NextItemLineEnd = la_rRates(UBound(la_rRates)) ' Write #FileHandle, ma_rRates(mc_nDiscountRateCount - 1)

            For k = LBound(la_rValues, 2) To UBound(la_rValues, 2)
                ' Indicator title, ...
                .NextItem = la_sTitles(k) ' Write #FileHandle, ma_sTitles(k);
                ' ... followed by the indicator values @ rates.
                For i = LBound(la_rValues, 1) To UBound(la_rValues, 1) - 1
                    .NextItem = la_rValues(i, k) ' Write #FileHandle, ma_rValues(i, k);
                Next i
                ' the last discount rate (column)
                .NextItemLineEnd = la_rValues(UBound(la_rValues), k) ' Write #FileHandle, ma_rValues(mc_nDiscountRateCount - 1, k)
            Next k

        End With

    End Function
End Class