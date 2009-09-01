VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CXFSFileSeqMemVnt"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
'
' Implements a memory based sequential file
'
' This object holds the actual data. This is good for
' temporary (scratch) files.
'
' The actual storage is a variant array. This permits
' object references to be stored in the file.
'
Option Explicit

Implements IEFSFile
Implements IEFSFileSeq
Implements IEFSFileSeqIn
Implements IEFSFileSeqOut

Private ma_vValues() As Variant

Private m_lCursor As Long       ' current read/write element
Private m_lCount As Long        ' count of elements in file
Private m_bOpen As Boolean
Private m_oFile As IEFSFile

Public Property Get File() As IEFSFile

   Set File = m_oFile
   
End Property

Public Property Set File(ByVal NewFile As IEFSFile)

   Set m_oFile = NewFile
   
End Property

Private Sub Class_Initialize()
    m_lCount = 0
    m_lCursor = 1
    m_bOpen = False
End Sub

'=========================================================
'
' IEFSFile Interface
'
'
Private Property Get IEFSFile_FullName() As String

   IEFSFile_FullName = m_oFile.FullName
   
End Property


Private Property Get IEFSFile_Name() As String

   IEFSFile_Name = m_oFile.Name
   
End Property


Private Property Get IEFSFile_Path() As String

   IEFSFile_Path = m_oFile.path
   
End Property


'=========================================================
'
' IEFSFileSeq Interface
'
'

Private Property Get IEFSFileSeq_Length() As Long
    IEFSFileSeq_Length = m_lCount
End Property

Private Function IEFSFileSeq_OpenForAppend() As IEFSFileSeqOut
   
    If m_lCount = 0 Then
        ReDim ma_vValues(1 To 4096)
    End If
        
    m_lCursor = m_lCount + 1
    
    Set IEFSFileSeq_OpenForAppend = Me
    m_bOpen = True

End Function

Private Function IEFSFileSeq_OpenForInput() As IEFSFileSeqIn
   
    m_lCursor = 1
    
    Set IEFSFileSeq_OpenForInput = Me
    m_bOpen = True

End Function

Private Function IEFSFileSeq_OpenForOutput() As IEFSFileSeqOut
   
    m_lCursor = 1
    m_lCount = 0
    
    ReDim ma_vValues(1 To 4096)
    
    Set IEFSFileSeq_OpenForOutput = Me
    m_bOpen = True

End Function


'=========================================================
'
' IEFSFileSeqIn Interface
'
'

Private Function IEFSFileSeqIn_AtBeginning _
    () As Boolean
    IEFSFileSeqIn_AtBeginning = (m_lCursor = 1)
End Function

Private Function IEFSFileSeqIn_AtEnd _
    () As Boolean
    IEFSFileSeqIn_AtEnd = (m_lCursor > m_lCount)
End Function

Private Function IEFSFileSeqIn_CloseFile _
    () As IEFSFileSeq
    m_bOpen = False
    Set IEFSFileSeqIn_CloseFile = Me
End Function

Private Function IEFSFileSeqIn_NextItem _
    () As Variant
    
    If Not m_bOpen Then
        Err.Raise 52            ' bad file name or number
    End If
    
    IEFSFileSeqIn_NextItem = ma_vValues(m_lCursor)
    m_lCursor = m_lCursor + 1
   
End Function


'=========================================================
'
' IEFSFileSeqOut Interface
'
'
Private Function IEFSFileSeqOut_CloseFile _
    () As IEFSFileSeq
    m_bOpen = False
    Set IEFSFileSeqOut_CloseFile = Me
End Function

Private Property Let IEFSFileSeqOut_NextItem _
    ( _
    ByVal DataItem As Variant _
    )
    
    If Not m_bOpen Then
        Err.Raise 52
    End If
    
    ma_vValues(m_lCursor) = DataItem
    
    m_lCount = m_lCursor
    m_lCursor = m_lCursor + 1
    
    If m_lCursor > UBound(ma_vValues) Then
        ReDim Preserve ma_vValues(LBound(ma_vValues) To UBound(ma_vValues) + (UBound(ma_vValues) - LBound(ma_vValues) + 1))
    End If
    
End Property

'
' This is the same as the NextItem property
' because there is no concept of 'lines' in this
' storage scheme.
'
Private Property Let IEFSFileSeqOut_NextItemLineEnd _
    ( _
    ByVal DataItem As Variant _
    )
        
    If Not m_bOpen Then
        Err.Raise 52
    End If
    
    ma_vValues(m_lCursor) = DataItem
    
    m_lCount = m_lCursor
    m_lCursor = m_lCursor + 1
    
    If m_lCursor > UBound(ma_vValues) Then
        ReDim Preserve ma_vValues(LBound(ma_vValues) To UBound(ma_vValues) + (UBound(ma_vValues) - LBound(ma_vValues) + 1) \ 2)
    End If

End Property

