Option Strict Off
Option Explicit On
Friend Class CGiantRptPageStdFormatSeq
    Implements IDPersistFormat
    ' Name:         CGiantRptPageStdFormatSeq.cls
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
        Dim ub As Short

        Dim la_sHeaders() As String
        Dim la_rValues(,) As Single

        Dim l_oObject As IDGiantRptPageStd
        Dim l_oStore As IDStoreSeq

        l_oStore = TheStore
        l_oObject = TheObject

        la_sHeaders = VB6.CopyArray(l_oObject.Headers)
        la_rValues = VB6.CopyArray(l_oObject.Values)

        With l_oStore
            ' compute upper bound of columns dimension of arrays (zero based)
            ub = UBound(la_sHeaders)

            If ub = 0 Then
                ' only one column
                .NextItemLineEnd = la_sHeaders(0) ' replaces: Write #FileHandle, Titles(0)

                For k = LBound(la_rValues, 2) To UBound(la_rValues, 2)
                    .NextItemLineEnd = la_rValues(0, k) ' replaces: Write #FileHandle, Values(0, k)
                Next k

            Else
                ' Write the column headers
                For i = 0 To ub - 1
                    .NextItem = la_sHeaders(i) ' replaces: Write #FileHandle, Titles(i);
                Next i
                ' the last column
                .NextItemLineEnd = la_sHeaders(ub) ' replaces: Write #FileHandle, Titles(ub)

                ' Write the column data, row by row
                For k = LBound(la_rValues, 2) To UBound(la_rValues, 2)
                    For i = 0 To ub - 1
                        .NextItem = la_rValues(i, k) ' replaces: Write #FileHandle, Values(i, k);
                    Next i
                    ' the last column
                    .NextItemLineEnd = la_rValues(ub, k) ' replaces: Write #FileHandle, Values(ub, k)
                Next k

            End If
        End With

    End Function
End Class