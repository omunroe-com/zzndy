VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CDFGiantRptPgStdASPEAmts"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
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

'
' Put the time series amounts on the store in
' profile major order.
'
Private Function IDPersistFormat_Store _
    ( _
    ByVal TheStore As IDStore, _
    ByVal TheObject As IDPersistObject _
    ) As Boolean
    
    Dim i As Integer
    Dim j As Long
        
    Dim la_rValues() As Single
    
    Dim l_oObject As IDGiantRptPageStd
    Dim l_oStore As IDStoreSeq
        
    Set l_oStore = TheStore
    Set l_oObject = TheObject
    
    la_rValues = l_oObject.Values
    
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
