VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CGiantRptPagePRTFormatSeq"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' Name:         CGiantRptPagePRTFormatSeq.cls
' Function:     Sequential text Giant Report page format
' Date:         28 Jan 2004 JWD
'---------------------------------------------------------
' Format for PRT report page
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
    
    Dim la_sHeaders() As String
    Dim la_rValues() As Single
    
    Dim l_oObject As IDGiantRptPageStd
    Dim l_oStore As IDStoreSeq
        
    Set l_oStore = TheStore
    Set l_oObject = TheObject
    
    la_sHeaders = l_oObject.Headers
    la_rValues = l_oObject.Values
    
    With l_oStore
        
        For i = LBound(la_sHeaders) To UBound(la_sHeaders)
            .NextItemLineEnd = la_sHeaders(i)         ' Write #FileHandle, ma_sTitles(i)
        Next i
    
        For k = LBound(la_rValues, 2) To UBound(la_rValues, 2)
            For i = LBound(la_rValues, 1) To UBound(la_rValues, 1)
                .NextItemLineEnd = la_rValues(i, k)  ' Write #FileHandle, ma_rValues(i, k)
            Next i
        Next k
    End With
    
End Function

