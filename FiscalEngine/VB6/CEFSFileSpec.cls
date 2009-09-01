VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CEFSFileSpec"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
'
' Instances of this class are ordinary files in
' the operating system's file system.
'
Option Explicit

Implements IEFSFile

Private zzz_FullName As String

'
' For now, this property assumes that the new name
' specified is valid in the file system.
'
Public Property Let FullName(ByVal NewName As String)

   zzz_FullName = NewName
   
End Property

'
' Return the file specification for the
' temporary file.
'
Public Property Get FullName() As String

   FullName = zzz_FullName
   
End Property


Private Sub Class_Initialize()

   zzz_FullName = vbNullString
   
End Sub


'=========================================================
'
' IEFSFile Interface
'
'
Private Property Get IEFSFile_FullName() As String

   IEFSFile_FullName = zzz_FullName
   
End Property


Private Property Get IEFSFile_Name() As String

    Dim p As Long
    
    p = InStrRev(zzz_FullName, "\")
    If p > 0 Then
        IEFSFile_Name = Mid(zzz_FullName, p + 1)
    End If
    
    'IEFSFile_Name = GetFilePart(zzz_FullName)
   
End Property


Private Property Get IEFSFile_Path() As String

    Dim p As Long
    
    p = InStrRev(zzz_FullName, "\")
    If p > 0 Then
        IEFSFile_Path = Left(zzz_FullName, p - 1)
    End If
    
    'IEFSFile_Path = GetPathPart(zzz_FullName)
    
End Property

