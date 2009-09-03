Option Strict Off
Option Explicit On
Friend Class CDFGiantRptPgStdASPEAmts
    Implements IDPersistFormat
    ' Name:         CGiantRptPageStdFormatASPEAmts.cls
    ' Function:     Giant Report page format (table)
    ' Date:         28 Jan 2004 JWD
    '---------------------------------------------------------
    ' A format for time series amounts. This outputs the
    ' amounts in profile major order.
    '---------------------------------------------------------
    ' Modifications:
    ' 27 Feb 2004 JWD
    '  -> Changed class name to CDFGiantRptPgStdASPEAmts
    '     to fit in 39 character programmatic ID limit.
    '---------------------------------------------------------



    '=========================================================
    '
    ' IDPersistFormat
    '

    Private Function IDPersistFormat_Restore(ByVal TheStore As IDStore, ByVal TheObject As IDPersistObject) As Boolean Implements IDPersistFormat.Restore
    End Function

    '
    ' Put the time series amounts on the store in
    ' profile major order.
    '
    Private Function IDPersistFormat_Store(ByVal TheStore As IDStore, ByVal TheObject As IDPersistObject) As Boolean Implements IDPersistFormat.Store

        Dim i As Short
        Dim j As Integer

        Dim la_rValues(,) As Single

        Dim l_oObject As IDGiantRptPageStd
        Dim l_oStore As IDStoreSeq

        l_oStore = TheStore
        l_oObject = TheObject

        la_rValues = VB6.CopyArray(l_oObject.Values)

        With l_oStore
            ' For each profile
            For i = LBound(la_rValues, 1) To UBound(la_rValues, 1)
                ' for each year
                For j = LBound(la_rValues, 2) To UBound(la_rValues, 2) - 1
                    .NextItem = la_rValues(i, j)
                Next j
                .NextItemLineEnd = la_rValues(i, UBound(la_rValues, 2))
            Next i
        End With

    End Function
End Class