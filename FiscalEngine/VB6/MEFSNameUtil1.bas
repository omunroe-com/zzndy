Attribute VB_Name = "MEFSNameUtil1"
' Name:        MEFSNameUtil1.bas
' Function:    Parse Windows File Specifications
' Date:        8 May 2002 JWD
'
' This module implements some routines that are
' commonly used for parsing apart Windows file
' specifications.
'
'---------------------------------------------------------
' Note that these are copies of procedures of the
' same name as found in As$et's global.bas (rev. 011)
' Problem is that the global.bas file has a number
' of things that are global only in the context of
' As$et, and couldn't be used in Estimator, so the
' module wasn't used, only the needed routines.
'
' This module does the same thing as some code
' done elsewhere (FIL0100A.BAS) but the code there
' has been altered significantly and these names in
' this module are used widely in As$et (by being in
' global.bas).
'
' At some time in the future, differently named
' routines performing the same basic function should
' be identified and reconciled and duplicate
' functionality removed.
'---------------------------------------------------------
Option Explicit

'
' Given a string that is a file specification,
' return the string that contains the name of
' the file itself, removing the path part of
' the specification.
'
Public Function GetFilePart(sFilePath As String) As String
   
   Dim i As Integer
   
   If Len(sFilePath) < 4 Then
      GetFilePart = ""
      Exit Function
   End If
   
   For i = Len(sFilePath) To 1 Step -1
      If Mid$(sFilePath, i, 1) = "\" Then
         If i = Len(sFilePath) Then
            GetFilePart = ""
         Else
            GetFilePart = Mid$(sFilePath, i + 1)
         End If
         Exit Function
      End If
   Next

End Function

'
' Given a string that is a file specification,
' return the string that gives the path to the
' file (the location or folder/directory in
' which the file is located), removing the name
' part of the specification.
'
Public Function GetPathPart(sFilePath As String) As String
   
   Dim i As Integer
   
   If Len(sFilePath) < 3 Then
      GetPathPart = ""
      Exit Function
   End If
   
   For i = Len(sFilePath) To 1 Step -1
      If Mid$(sFilePath, i, 1) = "\" Then
         If i = Len(sFilePath) Then
            GetPathPart = ""
         Else
            GetPathPart = Left$(sFilePath, i - 1)
         End If
         Exit Function
      End If
   Next

End Function

