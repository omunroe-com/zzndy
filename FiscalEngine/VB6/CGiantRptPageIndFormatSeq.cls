VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CGiantRptPageIndFormatSeq"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' Name:         CGiantRptPageIndFormatSeq.cls
' Function:     Sequential text Giant Report page format
' Date:         28 Jan 2004 JWD
'---------------------------------------------------------
' Format for economic indicators page
'---------------------------------------------------------
Option Explicit

Implements IDPersistFormat



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

Private Function IDPersistFormat_Store _
    ( _
    ByVal TheStore As IDStore, _
    ByVal TheObject As IDPersistObject _
    ) As Boolean
    
    Dim i As Integer
    Dim k As Integer
    
    Dim la_sTitles() As String
    Dim la_rRates() As Single
    Dim la_rValues() As Single

    ' Interface references
    Dim l_oObject As IDGiantRptPageInd      ' Stores Economic Indicator pages
    Dim l_oStore As IDStoreSeq              ' in sequential text format
        
    ' Get the desired interfaces from the object and store
    Set l_oObject = TheObject
    Set l_oStore = TheStore
    
    ' Write out the Economic Summary Page

    With l_oObject
        la_sTitles = .Headers
        la_rRates = .Rates
        la_rValues = .Values
    End With
    
    With l_oStore
        For i = LBound(la_rRates) To UBound(la_rRates) - 1
            .NextItem = la_rRates(i)                             ' Write #FileHandle, ma_rRates(i);
        Next i
        .NextItemLineEnd = la_rRates(UBound(la_rRates))          ' Write #FileHandle, ma_rRates(mc_nDiscountRateCount - 1)
    
        For k = LBound(la_rValues, 2) To UBound(la_rValues, 2)
            ' Indicator title, ...
            .NextItem = la_sTitles(k)                            ' Write #FileHandle, ma_sTitles(k);
            ' ... followed by the indicator values @ rates.
            For i = LBound(la_rValues, 1) To UBound(la_rValues, 1) - 1
                .NextItem = la_rValues(i, k)                     ' Write #FileHandle, ma_rValues(i, k);
            Next i
            ' the last discount rate (column)
            .NextItemLineEnd = la_rValues(UBound(la_rValues), k) ' Write #FileHandle, ma_rValues(mc_nDiscountRateCount - 1, k)
        Next k

    End With

End Function



