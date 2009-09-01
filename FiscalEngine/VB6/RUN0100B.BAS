Attribute VB_Name = "RUN0100B"
'name:          RUN0100B.BAS
'function:      GIANT Run File Routines
'-----------------------------------------------------------------------
' Modifications:
' 13 Feb 1996 JWD
'          Changed common block include file from CTYIN.BAS to CTYIN1.BG
'-----------------------------------------------------------------------
'$DYNAMIC
'$INCLUDE: 'run0100.bi'
'$INCLUDE: 'ctyin1.bg'

'
' Modifications:
' 9 Feb 2004 JWD
'  -> Replace explicit file i/o statements with class
'     method calls for run file. (C0776)
'
Sub GetRunFileLine()
'--------------------------------------------------------------------
'reads the run file to get the next line
'  replaces GOSUB 48000 in old giant

      Dim i As Integer

      AR = AR + 1
    With g_oRunFileIn      ' Input #3, RF$(1), RF$(2), RF$(3), RF$(4), RF$(5), RF$(6), RF$(7), RF$(8), RF$(9), RF$(10), RF$(11)
        RF$(1) = .NextItem
        RF$(2) = .NextItem
        RF$(3) = .NextItem
        RF$(4) = .NextItem
        RF$(5) = .NextItem
        RF$(6) = .NextItem
        RF$(7) = .NextItem
        RF$(8) = .NextItem
        RF$(9) = .NextItem
        RF$(10) = .NextItem
        RF$(11) = .NextItem
    End With
      For i = 1 To 11
         RF$(i) = UCase$(LTrim$(RTrim$(RF$(i))))
      Next i
End Sub


