VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CDFGiantRptPgStdASPEInts"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
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
' Put the time series interests on the store in
' profile major order.
'
Private Function IDPersistFormat_Store _
    ( _
    ByVal TheStore As IDStore, _
    ByVal TheObject As IDPersistObject _
    ) As Boolean
    
    Dim i As Integer
        
    Dim l_oObject As IDGiantRptPageStd
    Dim l_oHdr As CGiantRptPageHdr1
    
    Dim l_oStore As IDStoreSeq
    
    Set l_oStore = TheStore
    
    Set l_oObject = TheObject
    Set l_oHdr = l_oObject.PageHeader
    
    With l_oStore
        ' For each profile
        For i = 1 To l_oHdr.Columns
            .NextItem = l_oHdr.CompanyWorkingInterest
            .NextItemLineEnd = l_oHdr.GovernmentParticipation
        Next i
    End With
    
End Function
