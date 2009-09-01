Attribute VB_Name = "MOutputText"
' Name:        MOutputText2.bas
' Function:    Program to generate Output Report Text
'              Resource Source Files
' Date:        13 May 2003 JWD
'---------------------------------------------------------
' This program generates AESOuttext.h and AESOuttext.rc
' files for use with the resource compiler to generate
' the AESFiscal.res file.
'
' These files (AESOuttext.h and AESOuttext.rc) contain the
' output report text for the fiscal engine.
'---------------------------------------------------------
' Modifications:
' 11 Jan 2005 JWD
'  -> Add constant symbol for output files folder (C0846)
'  -> Changed zzz_Generate_Header(). (C0846)
'  -> Changed zzz_Generate_Table(). (C0846)
'  -> Changed zzz_Generate_MapData(). (C0846)
'  -> Changed zzz_Initialize(). (C0846)
'---------------------------------------------------------
Option Explicit


Dim zzz_RH()  As String       ' The row headers
Dim zzz_PT() As Integer       ' Page (section) type ID numbers
                              ' corresponding to the row text

Dim zzz_map() As Integer      ' returns offset in zzz_rh where
                              ' page type's rows begin
                              ' i. e. zzz_map(PageTypeID) + RowID -> text
                              ' for RowID on PageTypeID.
                              
Dim zzz_RowCount As Integer

Dim zzz_IsInitialized As Boolean

Const zzz_PTCount = 21           ' Count of page types

Const zzz_Base = &H400           ' 1024 - Row headers begin at 1024+1

Const zzz_PTS_RT = "IDS_RT"      ' ID symbol name root

Const zzz_CR = "\012"            ' Used to divide sections

Private zzz_OutFolder As String

'
' Modifications:
' 11 Jan 2005 JWD
'  -> Add initialization of zzz_OutFolder variable to
'     create the output files in the application folder.
'     (C0846)
'
Public Sub Main()

   zzz_Initialize
   
   zzz_OutFolder = App.Path & "\"
   
   zzz_Generate_Header
   
   zzz_Generate_Table
   
   zzz_Generate_MapData
   
End Sub

'
' Modifications:
' 11 Jan 2005 JWD
'  -> Replace explicit output file folder with symbol.
'     (C0846)
'
Private Sub zzz_Generate_Header()

   Dim f As Integer
   Dim i As Integer
   
   f = FreeFile
   
   Open zzz_OutFolder & "AESouttext.h" For Output As #f
   
   Print #f, "// String IDs for Output Report Text"
   Print #f, "// Strings 1-" & Format(zzz_PTCount, "0") & " are section titles"
   Print #f, "//"
   
   For i = 1 To zzz_RowCount
      
      If i > zzz_PTCount Then
         If i - zzz_map(zzz_PT(i)) = 1 Then
            Print #f, ""
            Print #f, ""
            Print #f, "// "; IIf(zzz_RH(2, zzz_PT(i)) = "|1|", "USER-DEFINED VARIABLE REPORT", Replace(zzz_RH(2, zzz_PT(i)), "|1|", "variable"))
            Print #f, ""
         End If
      End If
      
      Print #f, "#define   "; zzz_IDS_RTSymbol(i); "   "; Format(i + zzz_Base, "###0"); "       // "; zzz_RH(2, i)
      
      If i = zzz_PTCount Then
         Print #f, ""
         Print #f, "// Row header text ids next"
         Print #f, ""
      End If
      
   Next i
   
   Close #f
   
End Sub

'
' Modifications:
' 11 Jan 2005 JWD
'  -> Replace explicit output file folder with symbol.
'     (C0846)
'  -> Changed extension of created file from "rc" to
'     "str" to match name expected. (C0846)
'
Private Sub zzz_Generate_Table()

   Dim f As Integer
   Dim i As Integer
   
   Const zCR = "\012"
   
   f = FreeFile
   
   Open zzz_OutFolder & "AESouttext.str" For Output As #f
   
   Print #f, "   // Strings for Output Report Row Header Text"
   Print #f, "   // Strings 1-" & Format(zzz_PTCount, "0") & " are section titles"
   Print #f, "   //"
   
   For i = 1 To zzz_RowCount
      
      If i > zzz_PTCount Then
         If i - zzz_map(zzz_PT(i)) = 1 Then
            Print #f, ""
            Print #f, ""
            Print #f, "   // "; IIf(zzz_RH(2, zzz_PT(i)) = "|1|", "USER-DEFINED VARIABLE REPORT", Replace(zzz_RH(2, zzz_PT(i)), "|1|", ""))
            Print #f, ""
         End If
      End If
      
      Print #f, "   "; zzz_IDS_RTSymbol(i); "   "; """"; zzz_RH(1, i) & zCR & zzz_RH(2, i) & zCR & zzz_RH(3, i); """"
      
      If i = zzz_PTCount Then
         Print #f, ""
         Print #f, "   // Row header text next"
      End If
      
   Next i
   
   Close #f

End Sub

'
' Modifications:
' 11 Jan 2005 JWD
'  -> Replace explicit output file folder with symbol.
'     (C0846)
'
Private Sub zzz_Generate_MapData()
   
   Dim f As Integer
   Dim i As Integer
   
   f = FreeFile
   
   Open zzz_OutFolder & "AESouttext.map" For Output As #f
   
   For i = 1 To zzz_PTCount
      Print #f, Format(zzz_map(i), "0") & ","
   Next i
   
   ' Then output the size of the array
   Print #f, Format(zzz_RowCount, "0")
   
   Close #f
   
End Sub

'
' Compose the IDS_RT (report text) symbol for the
' specific text identified by the index. Symbol
' identifies the page type and row id of the output
' report text.
'
Private Function zzz_IDS_RTSymbol(i)

   zzz_IDS_RTSymbol = zzz_PTS_RT & "_P" & Format(zzz_PT(i), "00") & "R" & Format(IIf(i > zzz_PTCount, i - zzz_map(zzz_PT(i)), 0), "000")
   
End Function

'
' Modifications:
' 11 Jan 2005 JWD
'  -> Add 3rd party and NOC cash flow and economic
'     indicators. (C0846)
'
Private Sub zzz_Initialize()

   Const unit_currency = "<CUR>"
   Const unit_volume = "<VOL>"
   Const unit_price = "<PRC>"
   
   Dim i As Integer
   Dim page_type As Integer
   Dim nCount As Integer
   
   ReDim zzz_PT(1 To 500)
   ReDim zzz_RH(1 To 3, 1 To 500)
   ReDim zzz_map(1 To zzz_PTCount)
   
   ' Initialize the section (page) titles
   zzz_RH(2, 1) = "FIELD GROSS INCOME"
   zzz_RH(2, 2) = "EQUIVALENT INCOME AND OPERATING EXPENSES"
   zzz_RH(2, 3) = "EXPLORATION CAPITAL"
   zzz_RH(2, 4) = "DEVELOPMENT CAPITAL"
   zzz_RH(2, 5) = "TOTAL CAPITAL EXPENDITURES"
   zzz_RH(2, 6) = "GROUP EXPENDITURES"
   zzz_RH(2, 7) = "COST RECOVERY SCHEDULE FOR |1|"
   zzz_RH(2, 8) = "DEPRECIATION SCHEDULE FOR |1|"
   zzz_RH(2, 9) = "GOVERNMENT REPAYMENT & PARTNER REIMBURSEMENT"
   zzz_RH(2, 10) = "AFTER TAX CASH FLOW"
   zzz_RH(2, 11) = "ECONOMIC INDICATORS"
   zzz_RH(2, 12) = "|1|"
   zzz_RH(2, 13) = "DEFLATED CASH FLOW"
   zzz_RH(2, 14) = "LOAN SCHEDULE"
   zzz_RH(2, 15) = "|1|"
   zzz_RH(2, 16) = "|1|"
   zzz_RH(2, 17) = "|1|"
   zzz_RH(2, 18) = "RATE OF RETURN BASED WORKSHEET FOR |1|"
   zzz_RH(2, 19) = "RATE OF RETURN BASED WORKSHEET FOR |1|"
   zzz_RH(2, 20) = "RATIO BASED WORKSHEET FOR |1|"
   zzz_RH(2, 21) = "RATIO BASED WORKSHEET FOR |1|"

   For i = 1 To zzz_PTCount
      zzz_PT(i) = i
   Next i
   
   ' Initialize the row titles

   ' This initialization has been done this
   ' way to aid in maintenance of the code.
   ' If it is necessary to add or remove
   ' a row from a section (page) just add
   ' the assignments where needed. Precede
   ' the block of lines with the increment
   ' of the row index i.
   
   
   '===================================================================
   ' Page type 1 FIELD GROSS INCOME
   
   page_type = 1
   zzz_map(page_type) = zzz_PTCount
   
   i = zzz_PTCount + 1
   zzz_RH(1, i) = "<OIL>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Oil Production"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<OPC>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Oil Price"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<REV|OIL>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Oil Revenues"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<GAS>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Gas Production"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<GPC>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Gas Price"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<REV|GAS>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Gas Revenues"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<OV1>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Volume Other 1"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<OP1>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Price Other 1 "
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<REV|OV1>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Revenue Other 1 "
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<OV2>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Volume Other 2"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<OP2>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Price Other 2"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<REV|OV2>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Revenue Other 2"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<OV3>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Volume Other 3"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<OP3>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Price Other 3"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<REV|OV3>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Revenue Other 3"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<OV4>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Volume Other 4"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<OP4>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Price Other 4"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<REV|OV4>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Revenue Other 4"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<OV5>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Volume Other 5"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<OP5>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Price Other 5"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<REV|OV5>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Revenue Other 5"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<OV6>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Volume Other 6"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<OP6>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Price Other 6"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<REV|OV6>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Revenue Other 6"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<OV7>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Volume Other 7"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<OP7>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Price Other 7"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<REV|OV7>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Revenue Other 7"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<OV8>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Volume Other 8"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<OP8>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Price Other 8"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<REV|OV8>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Revenue Other 8"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<OV9>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Volume Other 9"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<OP9>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Price Other 9"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<REV|OV9>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Revenue Other 9"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<OV0>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Volume Other 10"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<OP0>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Price Other 10"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "<REV|OV0>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Revenue Other 10"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   '===================================================================
   ' Page type 2 EQUIVALENT INCOME AND OPERATING EXPENSES
   
   page_type = 2
   zzz_map(page_type) = i       ' Save the last value of i as the base for this page

   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Equivalent Volume Production"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Equivalent Revenue Production"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

'   i = i + 1
'   zzz_RH(1, i) = ""
'   zzz_RH(2, i) = "Equivalent Production"
'   zzz_RH(3, i) = unit_volume
'   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Equivalent Price"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Equivalent Revenue"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   
   For nCount = 1 To 20
        i = i + 1
        zzz_RH(1, i) = "<OX1>"
        
        If nCount < 10 Then
            zzz_RH(1, i) = "<OX" & nCount & ">"
        ElseIf nCount = 10 Then
            zzz_RH(1, i) = "<OX0>"
        Else
            zzz_RH(1, i) = "<O" & nCount & ">"
        End If
                
        zzz_RH(2, i) = "|1|"
        'zzz_RH(2, i) = "Operating Expense 1"
        zzz_RH(3, i) = unit_currency
        zzz_PT(i) = page_type
   Next nCount
  
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Total Gross Operating Expenses"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   
   '===================================================================
   ' Page type 3 EXPLORATION CAPITAL
   
   page_type = 3
   zzz_map(page_type) = i       ' Save the last value of i as the base for this page

   i = i + 1
   zzz_RH(1, i) = "<GEO>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Geological & Geophysical"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<EDH>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Exploration Dry Holes"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<EDS>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Exploration Discovery Wells"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<ADH>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Appraisal Dry Holes"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<ASC>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Appraisal Successful Wells"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Total Exploration Capital"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   '===================================================================
   ' Page type 4 DEVELOPMENT CAPITAL
   
   page_type = 4
   zzz_map(page_type) = i       ' Save the last value of i as the base for this page

   i = i + 1
   zzz_RH(1, i) = "<DNP>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Development Non-producing Wells"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<DVP>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Development Producing Wells"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<PLF>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Platform & Design"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<FCL>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Facilities"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<TRN>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Transportation"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<EOR>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Enhanced Recovery"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Total Development Capital"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   '===================================================================
   ' Page type 5 TOTAL CAPITAL EXPENDITURES
   
   page_type = 5
   zzz_map(page_type) = i       ' Save the last value of i as the base for this page

   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Signature Bonus"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Discovery Bonus"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Production Bonus"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<BNS>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Other Bonus"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<LSE>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Leasehold"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<REN>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Rentals"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Total Exploration "
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Total Development"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<CP1>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Other Capital Expenditures 1"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<CP2>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Other Capital Expenditures 2"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<CP3>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Other Capital Expenditures 3"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<CP4>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Other Capital Expenditures 4"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<CP5>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Other Capital Expenditures 5"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<CP6>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Other Capital Expenditures 6"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<CP7>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Other Capital Expenditures 7"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<CP8>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Other Capital Expenditures 8"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<CP9>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Other Capital Expenditures 9"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<ABN>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Abandonment"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = "<AB1>"
   zzz_RH(2, i) = "|1|"
   'zzz_RH(2, i) = "Abandonment - Cash Expenditure"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Total Capital Expenditures"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
      
   '===================================================================
   ' Page type 6 GROUP EXPENDITURES
   
   page_type = 6
   zzz_map(page_type) = i       ' Save the last value of i as the base for this page

   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Group Rental"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Group Exploration"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Group Development"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Group Other"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Group Total"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   '===================================================================
   ' Page type 7 COST RECOVERY SCHEDULE
   
   page_type = 7
   zzz_map(page_type) = i       ' Save the last value of i as the base for this page

   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Base Rental"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Base Exploration"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Base Development"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Base Other"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Base Operating Expense"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Base Total"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Recovery Rental"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Recovery Exploration"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Recovery Development"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Recovery Other"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Recovery Operating Expense"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Recovery Total"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Recovery Ceiling"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   '===================================================================
   ' Page type 8 DEPRECIATION SCHEDULE
   
   page_type = 8
   zzz_map(page_type) = i       ' Save the last value of i as the base for this page

   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Base Rental"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Base Exploration"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Base Development"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Base Other"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Base Total"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Depreciation Rental"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Depreciation Exploration"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Depreciation Development"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Depreciation Other"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Depreciation Total"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   '===================================================================
   ' Page type 9 GOVERNMENT REPAYMENT AND PARTNER REIMBURSEMENT
   
   page_type = 9
   zzz_map(page_type) = i       ' Save the last value of i as the base for this page

   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Government Rental"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Government Exploration"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Government Development"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Government Other"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Government Total Repayment"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Partner Rental"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Partner Exploration"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Partner Development"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Partner Other"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Partner Total Reimbursement"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   '===================================================================
   ' Page type 10 AFTER TAX CASH FLOW
   
   page_type = 10
   zzz_map(page_type) = i       ' Save the last value of i as the base for this page

   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Government Repayment"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Financing"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Positive Cash Flow Total"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Capital Expenditure"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Operating Expense"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Negative Cash Flow Total"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Net Cash Flow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "|1|"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   '=============================================================
   ' Page type 11 ECONOMIC INDICATORS
   '

   page_type = 11
   zzz_map(page_type) = i
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Company Income"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Company Operating Costs"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Company Capital"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Company Royalty/Tax"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Company Cash Flow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Company Payout"
   zzz_RH(3, i) = "Years"
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Company Risk Return Ratio"
   zzz_RH(3, i) = ""
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Company Profitability Index"
   zzz_RH(3, i) = ""
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Company Government Take"
   zzz_RH(3, i) = "%"
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Company State Take"
   zzz_RH(3, i) = "%"
   zzz_PT(i) = page_type
   
   ' 11 Jan 2005 JWD (C0846) Add 3rd party and NOC indicators
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "3rd Party Cash Flow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "3rd Party Payout"
   zzz_RH(3, i) = "Years"
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "3rd Party Risk Return Ratio"
   zzz_RH(3, i) = ""
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "3rd Party Profitability Index"
   zzz_RH(3, i) = ""
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "NOC Cash Flow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "NOC Payout"
   zzz_RH(3, i) = "Years"
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "NOC Risk Return Ratio"
   zzz_RH(3, i) = ""
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "NOC Profitability Index"
   zzz_RH(3, i) = ""
   zzz_PT(i) = page_type
   ' End (C0846)
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Government Cash Flow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Government Payout"
   zzz_RH(3, i) = "Years"
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Government Risk Return Ratio"
   zzz_RH(3, i) = ""
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Government Profitability Index"
   zzz_RH(3, i) = ""
   zzz_PT(i) = page_type
   
   '===================================================================
   ' Page type 12 USER DEFINED FISCAL VARIABLE
   
   page_type = 12
   zzz_map(page_type) = i       ' Save the last value of i as the base for this page

   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Production"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Price"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Income"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Dedn. |1|"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Dedn. |1|"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Dedn. |1|"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Dedn. |1|"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Dedn. |1|"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Total Deductions"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Income less Deductions"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Rate"
   zzz_RH(3, i) = "%"
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Gross Amount"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Loss Carry-Forward"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Net Amount"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "LCF Ceiling"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   '===================================================================
   ' Page type 13 DEFLATED CASH FLOW
   
   page_type = 13
   zzz_map(page_type) = i       ' Save the last value of i as the base for this page

   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Deflator"
   zzz_RH(3, i) = "%"
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Company Cash Flow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Company Cumulative Cash Flow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Company Discounted Cash Flow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Company Cumulative Discounted Cash Flow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   ' 11 Jan 2005 JWD (C0846) Add 3rd party and NOC cash flows
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "3rd Party Cash Flow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "3rd Party Cumulative Cash Flow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "3rd Party Discounted Cash Flow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "3rd Party Cumulative Discounted Cash Flow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "NOC Cash Flow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "NOC Cumulative Cash Flow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "NOC Discounted Cash Flow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "NOC Cumulative Discounted Cash Flow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   ' End (C0846)
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Government Cash Flow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Government Cumulative Cash Flow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Government Discounted Cash Flow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Government Cumulative Discounted Cash Flow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   '===================================================================
   ' Page type 14 LOAN SCHEDULE
   
   page_type = 14
   zzz_map(page_type) = i       ' Save the last value of i as the base for this page

   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Loan Amount"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Loan Balance"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Principal"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Interest Payments"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Total Payments"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Cash flow Effect"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   '===================================================================
   ' Page type 15 FORECAST DEFAULT NAMES
   
   page_type = 15
   zzz_map(page_type) = i       ' Save the last value of i as the base for this page

   i = i + 1
   zzz_RH(1, i) = "OIL"
   zzz_RH(2, i) = "Oil Production"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "GAS"
   zzz_RH(2, i) = "Gas Production"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OV1"
   zzz_RH(2, i) = "Other Volume 1"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OV2"
   zzz_RH(2, i) = "Other Volume 2"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OV3"
   zzz_RH(2, i) = "Other Volume 3"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OV4"
   zzz_RH(2, i) = "Other Volume 4"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OV5"
   zzz_RH(2, i) = "Other Volume 5"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OV6"
   zzz_RH(2, i) = "Other Volume 6"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OV7"
   zzz_RH(2, i) = "Other Volume 7"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OV8"
   zzz_RH(2, i) = "Other Volume 8"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OV9"
   zzz_RH(2, i) = "Other Volume 9"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OV0"
   zzz_RH(2, i) = "Other Volume 10"
   zzz_RH(3, i) = unit_volume
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OPC"
   zzz_RH(2, i) = "Oil Price"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "GPC"
   zzz_RH(2, i) = "Gas Price"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OP1"
   zzz_RH(2, i) = "Other Price 1"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OP2"
   zzz_RH(2, i) = "Other Price 2"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OP3"
   zzz_RH(2, i) = "Other Price 3"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OP4"
   zzz_RH(2, i) = "Other Price 4"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OP5"
   zzz_RH(2, i) = "Other Price 5"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OP6"
   zzz_RH(2, i) = "Other Price 6"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OP7"
   zzz_RH(2, i) = "Other Price 7"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OP8"
   zzz_RH(2, i) = "Other Price 8"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OP9"
   zzz_RH(2, i) = "Other Price 9"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OP0"
   zzz_RH(2, i) = "Other Price 10"
   zzz_RH(3, i) = unit_price
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "REV"
   zzz_RH(2, i) = "|1| Revenues"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "PRD"
   zzz_RH(2, i) = "Total Revenues"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   
   For nCount = 1 To 20
        
        i = i + 1
        
        If nCount < 10 Then
            zzz_RH(1, i) = "OX" & nCount
        ElseIf nCount = 10 Then
            zzz_RH(1, i) = "OX0"
        Else
            zzz_RH(1, i) = "O" & nCount
        End If
        
        zzz_RH(2, i) = "Operating Expense " & nCount
        zzz_RH(3, i) = unit_currency
        zzz_PT(i) = page_type
        
    Next nCount

   i = i + 1
   zzz_RH(1, i) = "OPX"
   zzz_RH(2, i) = "Total Operating Expense"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "BNS"
   zzz_RH(2, i) = "Bonus"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "LSE"
   zzz_RH(2, i) = "Leasehold Acquisition"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "REN"
   zzz_RH(2, i) = "Rentals"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "GEO"
   zzz_RH(2, i) = "Geological & Geophysical"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "EDH"
   zzz_RH(2, i) = "Exploration Dry Hole"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "EDS"
   zzz_RH(2, i) = "Exploration Discovery"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "ADH"
   zzz_RH(2, i) = "Appraisal Dry Hole"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "ASC"
   zzz_RH(2, i) = "Appraisal Success"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "DNP"
   zzz_RH(2, i) = "Development Non-Producing"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "DVP"
   zzz_RH(2, i) = "Development Producing"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "PLF"
   zzz_RH(2, i) = "Platform & Design"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "FCL"
   zzz_RH(2, i) = "Facilities"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "TRN"
   zzz_RH(2, i) = "Transportation"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "EOR"
   zzz_RH(2, i) = "Enhanced Recovery"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "CP1"
   zzz_RH(2, i) = "Other Capital 1"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "CP2"
   zzz_RH(2, i) = "Other Capital 2"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "CP3"
   zzz_RH(2, i) = "Other Capital 3"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "CP4"
   zzz_RH(2, i) = "Other Capital 4"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "CP5"
   zzz_RH(2, i) = "Other Capital 5"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "CP6"
   zzz_RH(2, i) = "Other Capital 6"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "CP7"
   zzz_RH(2, i) = "Other Capital 7"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "CP8"
   zzz_RH(2, i) = "Other Capital 8"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "CP9"
   zzz_RH(2, i) = "Other Capital 9"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "ABN"
   zzz_RH(2, i) = "Abandonment"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "AB1"
   zzz_RH(2, i) = "Abandonment - Cash Expenditure"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "AB2"
   zzz_RH(2, i) = "Abandonment - Accrual Entry"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "CPX"
   zzz_RH(2, i) = "Total Capital Expenditure"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "EXP"
   zzz_RH(2, i) = "Total Exploration Capital"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "DEV"
   zzz_RH(2, i) = "Total Development Capital"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "BAL"
   zzz_RH(2, i) = "Balance 1"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "BL1"
   zzz_RH(2, i) = "Balance 2"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "BL2"
   zzz_RH(2, i) = "Balance 3"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OT1"
   zzz_RH(2, i) = "Other 1"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OT2"
   zzz_RH(2, i) = "Other 2"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OT3"
   zzz_RH(2, i) = "Other 3"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OT4"
   zzz_RH(2, i) = "Other 4"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "OT5"
   zzz_RH(2, i) = "Other 5"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "AJ1"
   zzz_RH(2, i) = "Adjustment 1"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "AJ2"
   zzz_RH(2, i) = "Adjustment 2"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "AJ3"
   zzz_RH(2, i) = "Adjustment 3"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "AJ4"
   zzz_RH(2, i) = "Adjustment 4"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "AJ5"
   zzz_RH(2, i) = "Adjustment 5"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "DPL"
   zzz_RH(2, i) = "Depletion"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "DPR"
   zzz_RH(2, i) = "Depreciation"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   i = i + 1
   zzz_RH(1, i) = "INT"
   zzz_RH(2, i) = "Interest on Loan"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   '===================================================================
   ' Page type 16 (not used)
   
   page_type = 16
   zzz_map(page_type) = i       ' Save the last value of i as the base for this page

   '===================================================================
   ' Page type 17 (not used)
   
   page_type = 17
   zzz_map(page_type) = i       ' Save the last value of i as the base for this page

   '===================================================================
   ' Page type 18 RATE OF RETURN BASED WORKSHEET (TAX)
   
   page_type = 18
   zzz_map(page_type) = i       ' Save the last value of i as the base for this page

   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Primary Cashflow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Subseq Cashflow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Adjusted Cashflow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Base #1"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Tax #1"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Base #2"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Tax #2"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Base #3"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Tax #3"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Tax Rate"
   zzz_RH(3, i) = "%"
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Total Tax"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   '===================================================================
   ' Page type 19 RATE OF RETURN BASED WORKSHEET (PROFIT SHARE)
   
   page_type = 19
   zzz_map(page_type) = i       ' Save the last value of i as the base for this page

   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Primary Cashflow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Subseq Cashflow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Adjusted Cashflow"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Base #1"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Reduct #1"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Base #2"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Reduct #2"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Base #3"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Reduct #3"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Reduct Rate"
   zzz_RH(3, i) = "%"
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Total Reduct"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Avail Pft Share"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Pft Shr Rate"
   zzz_RH(3, i) = "%"
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Net Pft Share"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   '===================================================================
   ' Page type 20 RATIO BASED WORKSHEET (TAX)
   
   page_type = 20
   zzz_map(page_type) = i       ' Save the last value of i as the base for this page

   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Primary Numerator"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Primary Denominator"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Subseq Numerator"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Subseq Denominator"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Final Numerator"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Final Denominator"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Ratio"
   zzz_RH(3, i) = "%"
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Taxable Income"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Tax Rate"
   zzz_RH(3, i) = "%"
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Total Tax"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type

   '===================================================================
   ' Page type 21 RATIO BASED WORKSHEET (PROFIT SHARE)
   
   page_type = 21
   zzz_map(page_type) = i       ' Save the last value of i as the base for this page
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Primary Numerator"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Primary Denominator"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Subseq Numerator"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Subseq Denominator"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Final Numerator"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Final Denominator"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Ratio"
   zzz_RH(3, i) = "%"
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Adjusted Profit Share"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Pft Shr Reduct"
   zzz_RH(3, i) = "%"
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Total Reduct"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Avail Pft Share"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Pft Share Rate"
   zzz_RH(3, i) = "%"
   zzz_PT(i) = page_type
   
   i = i + 1
   zzz_RH(1, i) = ""
   zzz_RH(2, i) = "Net Pft Share"
   zzz_RH(3, i) = unit_currency
   zzz_PT(i) = page_type
   
   
   ' save the row count
   zzz_RowCount = i
   
   zzz_IsInitialized = True
   
End Sub

