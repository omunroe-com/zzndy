VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CDArray1DDblA"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False

Option Explicit


Implements IDStore
Implements IDStoreSeq

Private ma_dValues() As Double

Private m_lIndex As Long

Public Sub Initialize _
    ( _
    ByVal LowerBound As Long, _
    ByVal UpperBound As Long _
    )
    
    ReDim ma_dValues(LowerBound To UpperBound)
    ResetCursor
    
End Sub

Public Sub ResetCursor _
    ()
    
    m_lIndex = -1
    On Error Resume Next
    m_lIndex = LBound(ma_dValues, 1)

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
    
    ma_dValues(m_lIndex) = CDbl(DataItem)
    m_lIndex = m_lIndex + 1

End Property

Private Property Let IDStoreSeq_NextItemLineEnd _
    ( _
    ByVal DataItem As Variant _
    )
        
    ' No "lines" in this storage, so same as NextItem
    ma_dValues(m_lIndex) = CDbl(DataItem)
    m_lIndex = m_lIndex + 1
    
End Property
