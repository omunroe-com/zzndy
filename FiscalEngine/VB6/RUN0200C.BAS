Attribute VB_Name = "RUN0200C"
'name:          RUN0200C.BAS
'function:      GIANT Finder.Prn Routines
' $title:    'RUN0200C'
' $subtitle: 'Placeholder'
'-----------------------------------------------------------------------
' Modifications:
' 13 Feb 1996 JWD
'          Changed common block include file from CTYIN.BAS to CTYIN1.BG
' 21 Feb 1996 JWD
'          Changed type of AR to integer
'-----------------------------------------------------------------------
'$DYNAMIC
'$INCLUDE: 'run0200.bi'
'$INCLUDE: 'ctyin1.bg'

Const FINDER = "FINDER.PRN"

Sub ReadFinder (iLine As Integer, sCameFrom As String)
'--------------------------------------------------------------------

      Dim hfFinder As Integer

      rLine = 0
      sCameFrom = ""

      On Local Error GoTo ReadHandler
      hfFinder = FreeFile
      Open TempDir$ + FINDER For Input As #hfFinder
         Input #hfFinder, iLine, sCameFrom
      Close #hfFinder

Exit Sub
'-----------------------------------------------------------------------
ReadHandler:
      If Err = 53 Or Err = 62 Then
         Exit Sub
      Else
         Error Err
      End If
End Sub

Sub WriteFinder (iLine As Integer, sCameFrom As String)
'-----------------------------------------------------------------------

      Dim hfFinder As Integer

      hfFinder = FreeFile
      Open TempDir$ + FINDER For Output As #hfFinder
         Write #hfFinder, iLine, sCameFrom
      Close #hfFinder

End Sub

