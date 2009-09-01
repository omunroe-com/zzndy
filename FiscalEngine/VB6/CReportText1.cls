VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CReportText"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' Name:        CReportText.cls
' Function:    Encapsulate the Output Report Text Source
' Date:        13 May 2003 JWD
'---------------------------------------------------------
' Abstract the interface for how the output text is stored
' and retrieved.
'
' This should be a singleton, there should not be a need
' for multiple instances of this class (unless supporting
' multiple languages simultaneously?).
'---------------------------------------------------------
Option Explicit

' This is the array of output report text. It contains the
' page type titles as well as the row titles. The page
' (section) titles are the first n elements of the array
' (where n is the greatest cardinal number of the page
' types). As of 15 May 2003, the value of n is 21.
' The row titles follow in the array, in page type (by
' page type id number) then row id number order.
'
' zzz_OT(1) = Title for page type 1
' zzz_OT(2) = Title for page type 2
' ...
' zzz_OT(21) = Title for page type 21
' zzz_OT(22) = Title for row id 1 on page type 1
' zzz_OT(23) = Title for row id 2 on page type 1
' ...
'
Dim zzz_OT() As String

' Map is set of base values that correspond to the page
' types that are added to the row identifier. The value
' stored in zzz_map(n) is the subscript of the text that
' corresponds to the last row id for page type n-1.
' Ex. If page type 2 has 6 rows then
' zzz_map(3) = zzz_map(2) + 6. The value of zzz_map(1)
' is the number of page types for which there is text.
' There is one more element in zzz_map() than there
' are page types. The Ubound element contains the size
' of the text array (which is the same as the base value
' of the next page type, if there was one!).
Dim zzz_map() As Integer

Dim zzz_ForecastCodesString As String

Public Function SectionTitle(ByVal PageTypeID As Integer) As String

   ' The first text items in the output text array are
   ' the section titles. There can be no more titles
   ' than the "base value" (as stored in zzz_map()) of
   ' the first page (zzz_map(1)).
   
   On Error Resume Next
   
   If PageTypeID > zzz_map(1) Then
      Exit Function
   End If
   
   SectionTitle = zzz_OT(2, PageTypeID)
   
   If Err.number > 0 Then
      Err.Raise vbObjectError + Err.number, "CReportText", "SectionTitle"
   End If
   
End Function

Public Function RowTitle(ByVal PageTypeID As Integer, ByVal RowID As Integer) As String

   ' Text is at position identified by adding the row
   ' identifier to the base value for the page type.
   ' The base value for the page type is stored in
   ' the zzz_map() array.
   
   Dim i As Integer
   Dim s As String
   Dim p As Integer
   Dim sA() As String
   
   On Error GoTo RowTitle_Error
   
   If PageTypeID > zzz_map(1) Then
      ' See SectionTitle function for explanation of this
      Exit Function
   End If
   
   If RowID > zzz_map(PageTypeID + 1) Then
      Exit Function
   End If
   
   p = zzz_map(PageTypeID) + RowID
   
   ' If there are replacement codes, do the substitutions
   ' before returning the row title. Otherwise return
   ' as is, without modification.
   If zzz_OT(1, p) Like "<*>" Then  ' Replacement codes
      ' Get the codes
      s = Mid$(zzz_OT(1, p), 2, Len(zzz_OT(1, p)) - 2)
      ' Split into individual codes
      sA = Split(s, "|")
      ' Get the string for this row id
      s = zzz_OT(2, p)
      ' Perform the replacements
      For i = 0 To UBound(sA)
         s = Replace(s, "|1|", zzz_GetForecastTitle(sA(i)))
      Next
   Else
      s = zzz_OT(2, zzz_map(PageTypeID) + RowID)
   End If
   
   RowTitle = s
   
   Exit Function
   
RowTitle_Error:
   
   Err.Raise vbObjectError + Err.number, "CReportText", "RowTitle"

End Function

Public Function RowUnit(ByVal PageTypeID As Integer, ByVal RowID As Integer) As String

   If PageTypeID > zzz_map(1) Then
      ' See SectionTitle function for explanation of this
      Exit Function
   End If
   
   If RowID > zzz_map(PageTypeID + 1) Then
      Exit Function
   End If
   
   RowUnit = zzz_OT(3, zzz_map(PageTypeID) + RowID)
   
End Function

Public Property Get ForecastTitle(ByVal ForecastCode As String) As String

   ForecastTitle = zzz_GetForecastTitle(ForecastCode)
   
End Property

Public Property Let ForecastTitle(ByVal ForecastCode As String, ByVal NewTitle As String)

   Dim i As Integer
   
   SearchCodeString zzz_ForecastCodesString, ForecastCode, 3, i
   
   If i = 0 Then
      Exit Property
   End If
   
   zzz_OT(2, zzz_map(15) + i) = NewTitle
   
End Property

Private Sub Class_Initialize()

   Const id_base = &H400         ' 1024 for US English
   
   Dim i As Integer
   Dim p As Integer
   Dim s As String
   Dim sA() As String
   Dim sCR As String
   
   Dim map_dat() As Byte
   
   On Error GoTo InitializeError
   
   ' Load the row text map data.
   ' Map data is an integer array, but is returned as bytes
   ' from resource file.
   map_dat = LoadResData(1024, 10)
   
   ReDim zzz_map(1 To ((UBound(map_dat) - LBound(map_dat) + 1) / 2))
   
   ' Reconstruct the integer array data from the byte array.
   ' Bytes are integers stored in "little-endian" fashion,
   ' i. e. lower order byte is at lower subscript, high
   ' order byte is a succeeding subscript.
   For i = LBound(zzz_map) To UBound(zzz_map)
      p = (i - 1) * 2
      zzz_map(i) = map_dat(p) + map_dat(p + 1) * 256
   Next i
   
   ' Load the row text
   ReDim zzz_OT(1 To 3, 1 To zzz_map(UBound(zzz_map)))
   
   sCR = Chr$(10)
   
   For i = 1 To UBound(zzz_OT, 2)
      s = LoadResString(i + id_base)
      
      sA = Split(s, sCR)
      zzz_OT(1, i) = sA(0)
      zzz_OT(2, i) = sA(1)
      zzz_OT(3, i) = sA(2)
   
   Next i
      
   For i = zzz_map(15) + 1 To zzz_map(16)
      zzz_ForecastCodesString = zzz_ForecastCodesString & zzz_OT(1, i)
   Next i
   
   Exit Sub
   
InitializeError:

   Err.Raise vbObjectError + Err.number, "CReportText", "Initialize error"
   
End Sub


Private Function zzz_GetForecastTitle(ByRef ForecastCode As String) As String

   Dim i As Integer
   
   SearchCodeString zzz_ForecastCodesString, ForecastCode, 3, i
   
   If i = 0 Then
      Exit Function
   End If
   
   zzz_GetForecastTitle = zzz_OT(2, zzz_map(15) + i)
   
End Function
