VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CGiantRptPageHdr1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' Name:         CGiantRptPageHdr1.cls
' Function:     Defines Page Header common to all pages
' Date:         28 Jan 2004 JWD
'---------------------------------------------------------
Option Explicit

Implements IDPersistObject

'
' IDPersistObject attributes
'
Private Const m_lClassID As Long = 9
Private Const m_sClassName As String = "CGiantRptPageHdr1"
Private m_lObjectID As Long


Public Columns As Integer
Public ColumnWidth As Integer
Public CompanyWorkingInterest As Single
Public CurrencyCode As String

Public GovernmentParticipation As Single

Public PageCounter As Integer
Public PageTitle As String
Public PageType As Integer

Public Rows As Integer

Public year As Integer


'=========================================================
'
' IDPersistObject Interface
'


Private Property Get IDPersistObject_ClassIDNumber _
    () As Long
    IDPersistObject_ClassIDNumber = m_lClassID
End Property

Private Property Get IDPersistObject_ClassName _
    () As String
    IDPersistObject_ClassName = m_sClassName
End Property

Private Property Let IDPersistObject_ObjectIDNumber _
    ( _
    ByVal NewIDNumber As Long _
    )
    m_lObjectID = NewIDNumber
End Property

Private Property Get IDPersistObject_ObjectIDNumber _
    () As Long
    IDPersistObject_ObjectIDNumber = m_lObjectID
End Property

Private Function IDPersistObject_RegisterInTable _
    ( _
    ByVal ObjectTable As IDPersistObjectTable _
    ) As Boolean
    ObjectTable.RegisterObject Me
End Function

Private Function IDPersistObject_RegisterObjectConnections _
    () As Boolean
End Function

Private Function IDPersistObject_RestoreUsingFormat _
    ( _
    ByVal TheStore As IDStore, _
    ByVal TheMap As IDPersistClassMap _
    ) As Boolean
End Function

Private Function IDPersistObject_StoreUsingFormat _
    ( _
    ByVal TheStore As IDStore, _
    ByVal TheMap As IDPersistClassMap _
    ) As Boolean
    
    TheMap.ClassFormat(Me).Store TheStore, Me
    
End Function

