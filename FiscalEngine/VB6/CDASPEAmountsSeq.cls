VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CDASPEAmountsSeq"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit


Implements IDStore
Implements IDStoreSeq

Private ma_dValues() As Double

Private m_lRow As Long
Private m_lColumn As Long

Public Sub Initialize _
    ( _
    ByVal Rows As Long, _
    ByVal Columns As Long _
    )
    
    ReDim ma_dValues(1 To Rows, 1 To Columns)
    ResetCursor
    
End Sub

Public Sub ResetCursor _
    ()
    
    m_lRow = 1
    m_lColumn = 1

End Sub

Public Property Get AllValues() As Double()
    AllValues = ma_dValues
End Property

'=========================================================
'
' IDStoreSeq Interface
'


Private Property Let IDStoreSeq_NextItem _
    ( _
    ByVal DataItem As Variant _
    )
    
    ma_dValues(m_lRow, m_lColumn) = DataItem
    m_lColumn = m_lColumn + 1

End Property

Private Property Let IDStoreSeq_NextItemLineEnd _
    ( _
    ByVal DataItem As Variant _
    )
    
    ma_dValues(m_lRow, m_lColumn) = DataItem
    m_lRow = m_lRow + 1
    m_lColumn = 1
    
End Property
