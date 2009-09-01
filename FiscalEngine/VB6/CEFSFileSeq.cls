VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CEFSFileSeq"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
'
' Defines a Basic sequential file. This
' class just provides methods for processing the
' contents of a file that is sequentially organized
' text. It is not sophisticated enough to do a lot
' of fancy stuff, but will Input and Write items
' that are the elementary scalar Basic types.
'
' (don't know what will happen if an object or
' an array is passed or expected)
'
Option Explicit

Implements IDStore
Implements IDStoreSeq

Implements IEFSFile
Implements IEFSFileSeq
Implements IEFSFileSeqOut
Implements IEFSFileSeqIn

' Just something that identifies a file...
' An instance of this class is going to
' interpret the contents of this file as
' if it is a Basic sequentially organized
' text file (Write, Input).
Private m_oFile As IEFSFile

Private m_iHandle As Long

' This is the delegate and represents the
' state of the file
'Private m_oFile As IEFSFileState

Public Property Get File() As IEFSFile

   Set File = m_oFile
   
End Property

Public Property Set File(ByVal NewFile As IEFSFile)

   Set m_oFile = NewFile
   
End Property

Private Sub Class_Terminate()
    If m_iHandle <> 0 Then
        Close m_iHandle
    End If
End Sub

'=========================================================
'
' IDStoreSeq Interface
'


Private Property Let IDStoreSeq_NextItem _
    ( _
    ByVal DataItem As Variant _
    )
    
    Write #m_iHandle, DataItem;
    
End Property

Private Property Let IDStoreSeq_NextItemLineEnd _
    ( _
    ByVal DataItem As Variant _
    )
    
    Write #m_iHandle, DataItem

End Property

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
    IEFSFileSeq_Length = FileLen(m_oFile.FullName)
End Property

Private Function IEFSFileSeq_OpenForAppend() As IEFSFileSeqOut
   
    m_iHandle = FreeFile
    Open m_oFile.FullName For Append As #m_iHandle
    
    Set IEFSFileSeq_OpenForAppend = Me

End Function

Private Function IEFSFileSeq_OpenForInput() As IEFSFileSeqIn
   
    m_iHandle = FreeFile
    Open m_oFile.FullName For Input As #m_iHandle
    
    Set IEFSFileSeq_OpenForInput = Me
    
End Function

Private Function IEFSFileSeq_OpenForOutput() As IEFSFileSeqOut
   
    m_iHandle = FreeFile
    Open m_oFile.FullName For Output As #m_iHandle
    
    Set IEFSFileSeq_OpenForOutput = Me
   
End Function


'=========================================================
'
' IEFSFileSeqIn Interface
'
'

Private Function IEFSFileSeqIn_AtBeginning _
    ( _
    ) As Boolean
    IEFSFileSeqIn_AtBeginning = (Seek(m_iHandle) = 1)
End Function

Private Function IEFSFileSeqIn_AtEnd _
    ( _
    ) As Boolean
    IEFSFileSeqIn_AtEnd = EOF(m_iHandle)
End Function

Private Function IEFSFileSeqIn_CloseFile _
    ( _
    ) As IEFSFileSeq
    Close #m_iHandle
    m_iHandle = 0
    Set IEFSFileSeqIn_CloseFile = Me
End Function

Private Function IEFSFileSeqIn_NextItem _
    ( _
    ) As Variant

   Dim temp_var As Variant
   
   Input #m_iHandle, temp_var
   IEFSFileSeqIn_NextItem = temp_var
   
End Function


'=========================================================
'
' IEFSFileSeqOut Interface
'
'
Private Function IEFSFileSeqOut_CloseFile _
    () As IEFSFileSeq
    Close #m_iHandle
    m_iHandle = 0
    Set IEFSFileSeqOut_CloseFile = Me
End Function

Private Property Let IEFSFileSeqOut_NextItem _
    ( _
    ByVal DataItem As Variant _
    )
    
    Write #m_iHandle, DataItem;
    
End Property

Private Property Let IEFSFileSeqOut_NextItemLineEnd _
    ( _
    ByVal DataItem As Variant _
    )
    
    Write #m_iHandle, DataItem

End Property


