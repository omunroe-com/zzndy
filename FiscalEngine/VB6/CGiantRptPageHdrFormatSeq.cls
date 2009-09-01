VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CGiantRptPageHdrFormatSeq"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
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
    
    Dim l_oStore As IDStoreSeq
    Dim l_oObject As CGiantRptPageHdr1
    
    Set l_oStore = TheStore
    Set l_oObject = TheObject
    
    With l_oStore
        .NextItem = l_oObject.PageType
        .NextItem = l_oObject.year
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

