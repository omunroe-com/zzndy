Option Strict Off
Option Explicit On
Module modExcel
	
	
	' Created GDP 17 Dec 2002
	' Module containing routines to send and retreive data from excel spreadsheets
	' This is achieved by placing tags in the comment field attached to each cell
	' eg <IN=OIL> would cause the oil production stream to be entered into this
	' cell and the required number of cells to the right.
	' <OUT=ABC> This is the net amount to be retieved from this workbook.
	' Modifications
	' 20 Jan 2003
	'   -> Changed definition of mc_sBDACats to include new volumes / prices
	'
	' 6 Oct 2003 JWD
	'  -> Changed symbol mc_sBDACats, adding codes AJ6-AJ0.
	'     (C0756)
	'
	' 8 Oct 2003 JWD
	'  -> Changed CleanUpExcel(). (C0760)
	'  -> Changed ReadExcelLinkData(). (C0761)
	'
	' 12 May 2005 JWD
	'  -> Changed symbol mc_sBDACats, added codes A11-A20.
	'     (C0876)
	'
	' 16 May 2005 JWD
	'  -> Changed symbol mc_sBDACats, added codes OX6-O20.
	'     (C0877)
	'
	Public Const gc_sREGNAME As String = "AS$ET2"
	
	Private Const xlToRight As Short = -4161
	' Variables which can be sent to excel - split into categories
	Private Const mc_sOtherCats As String = "INTPRDOPXEXPDEVCPX"
	Private Const mc_sCapexCats As String = CPXCategoryCodesString
	' GDP 20 Jan 2003
	' Added OV3-OV0 and OP3-OP0
	'Private Const mc_sBDACats As String = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0RESWINOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5"
	' 6 Oct 2003 JWD (C0756) Added codes AJ6-AJ0
	'Private Const mc_sBDACats As String = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0RESWINOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0"
	' 12 May 2005 JWD (C0876) Add codes A11-A20
	'Private Const mc_sBDACats As String = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0RESWINOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0A11A12A13A14A15A16A17A18A19A20"
	' 16 May 2005 JWD (C0877) Add codes OX6-O20
	Private Const mc_sBDACats As String = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0RESWINOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0OX1OX2OX3OX4OX5OX6OX7OX8OX9OX0O11O12O13O14O15O16O17O18O19O20AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0A11A12A13A14A15A16A17A18A19A20"
	Private Const mc_sANNCats As String = "PR1PR2PR3PR4PR5PRTDP1DP2DP3OT1OT2OT3OT4OT5"
	Private Const mc_sDTECats As String = "PJYPJMPDYPDMDIYDIMDDYDDM"
	' Keyword used for specifying whether link is an inbound one or an outbound one
	Private Const mc_sINPUTVAR As String = "IN"
	Private Const mc_sOUTPUTVAR As String = "OUT"
	
	
	'Global reference to Excel object
	Public g_appExcel As Object
	' Global Excel link data
	Public ga_sFiscalVariable() As String
	Public ga_sExcelWorkbook() As String
	Public g_nLinkedVariableCount As Short
	
	
	Private m_sCountryFilePath As String
	
	
	' Module level reference to the current workbook being used
	Private m_objExcelWorkbook As Object
	
	' Function which sends all data to excel workbook
	Public Function SendAllDataToExcel(ByVal nCurrentVar As Short, ByRef wbkExcel As Object) As Short
		On Error GoTo err_SendAllDataToExcel
		
		Dim wksCurrent As Object
		Dim objComment As Object
		
		Dim vntOutput As Object
		
		' Comments are specific to worksheets
		' Loop through all worksheets then all comments
		'UPGRADE_WARNING: Couldn't resolve default property of object wbkExcel.Worksheets. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		For	Each wksCurrent In wbkExcel.Worksheets
			'UPGRADE_WARNING: Couldn't resolve default property of object wksCurrent.Comments. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			For	Each objComment In wksCurrent.Comments
				' Send appropriate data for this comment
				SendDataToExcel(nCurrentVar, wksCurrent, objComment)
			Next objComment
		Next wksCurrent
		
		
		SendAllDataToExcel = 0
		
		Exit Function
err_SendAllDataToExcel: 
		SendAllDataToExcel = Err.Number
	End Function
	
	' Sends a particular variable to a workbook
	Private Function SendDataToExcel(ByVal nCurrentVar As Short, ByRef objWorksheet As Object, ByRef objComment As Object) As Boolean
		Dim sItem As String
		Dim a_fData() As Single
		Dim a_fExcelData() As Single
		Dim bFound As Boolean
		Dim objRange As Object
		Dim i As Short
		Dim nSingleValue As Short
		Dim bSingleValue As Boolean
		
		ReDim a_fData(LG)
		'UPGRADE_WARNING: Lower bound of array a_fExcelData was changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
		ReDim a_fExcelData(LG)
		
		' Get the 3 letter code from the comment text
		'UPGRADE_WARNING: Couldn't resolve default property of object objComment.Text. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		sItem = UCase(ParseCode(objComment.Text, mc_sINPUTVAR))
		If Len(sItem) = 0 Then
			SendDataToExcel = False
			Exit Function
		End If
		' Get appropriate values from global variables into a_fData()
		If IsCodePresent(sItem, mc_sBDACats) Or IsCodePresent(sItem, mc_sANNCats) Or IsCodePresent(sItem, mc_sCapexCats) Or IsCodePresent(sItem, mc_sOtherCats) Then
			' Call standard Sub to get various input data
			RetrieveValues(sItem, "", a_fData)
			bFound = True
		ElseIf GetPrevFiscalVariable(nCurrentVar, sItem, a_fData) Then 
			bFound = True
		ElseIf IsCodePresent(sItem, mc_sDTECats) Then 
			nSingleValue = GetDates(sItem)
			bFound = True
			bSingleValue = True
		End If
		
		If bFound Then
			'UPGRADE_WARNING: Couldn't resolve default property of object objComment.Parent. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			objRange = objComment.Parent
			
			If bSingleValue Then
				'UPGRADE_WARNING: Couldn't resolve default property of object objRange.Value. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				objRange.Value = nSingleValue
			Else
				' Setup range which will hold values
				'UPGRADE_WARNING: Couldn't resolve default property of object objRange.Offset. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				'UPGRADE_WARNING: Couldn't resolve default property of object objWorksheet.Range. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				objRange = objWorksheet.Range(objRange, objRange.Offset(0, UBound(a_fData) - 1))
				' Use another array with base of 1 to assign to range - RetreiveValues returns 0 based
				' array.
				For i = 1 To LG
					a_fExcelData(i) = a_fData(i)
				Next i
				'UPGRADE_WARNING: Couldn't resolve default property of object objRange.Value. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				objRange.Value = VB6.CopyArray(a_fExcelData)
			End If
		End If
	End Function
	' Retrieve a date
	Private Function GetDates(ByVal sCode As String) As Short
		
		Select Case sCode
			Case "PJY"
				GetDates = YR
			Case "PJM"
				GetDates = mo
			Case "PDY"
				GetDates = Y1
			Case "PDM"
				GetDates = M1
			Case "DIY"
				GetDates = Y2
			Case "DIM"
				GetDates = M2
			Case "DDY"
				GetDates = Y3
			Case "DDM"
				GetDates = M3
		End Select
	End Function
	' Retrieve data from an Excel workbook given an object and a comment tag
	Public Function GetDataFromExcel(ByVal wbkExcel As Object, ByVal sRange As String, ByRef a_fData() As Single) As Boolean
		
		
		Dim wksCurrent As Object
		Dim objRange As Object
		Dim objComment As Object
		Dim vntOutput As Object
		Dim i As Short
		Dim sCode As String
		Dim bFound As Boolean
		
		
		' Loop through worksheets and comments
		'UPGRADE_WARNING: Couldn't resolve default property of object wbkExcel.Worksheets. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		For	Each wksCurrent In wbkExcel.Worksheets
			'UPGRADE_WARNING: Couldn't resolve default property of object wksCurrent.Comments. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			For	Each objComment In wksCurrent.Comments
				'UPGRADE_WARNING: Couldn't resolve default property of object objComment.Text. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				sCode = UCase(ParseCode(objComment.Text, mc_sOUTPUTVAR))
				' Find the extent of the range where the values are
				If Left(sCode, Len(sRange)) = sRange Then
					'UPGRADE_WARNING: Couldn't resolve default property of object objComment.Parent. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					objRange = objComment.Parent
					'UPGRADE_WARNING: Couldn't resolve default property of object objRange.End. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					'UPGRADE_WARNING: Couldn't resolve default property of object wksCurrent.Range. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					objRange = wksCurrent.Range(objRange, objRange.End(xlToRight))
					bFound = True
					Exit For
				End If
			Next objComment
			If bFound Then Exit For
		Next wksCurrent
		
		If objRange Is Nothing Then
			GetDataFromExcel = False
		Else
			' Retrieve the Values
			'UPGRADE_WARNING: Couldn't resolve default property of object objRange.Value2. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			'UPGRADE_WARNING: Couldn't resolve default property of object vntOutput. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			vntOutput = objRange.Value2
			ReDim a_fData(UBound(vntOutput, 2) - 1)
			' Copy to a 1d zero based array.
			For i = 1 To UBound(vntOutput, 2)
				If IsNumeric(vntOutput(1, i)) Then
					'UPGRADE_WARNING: Couldn't resolve default property of object vntOutput(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					a_fData(i - 1) = vntOutput(1, i)
				End If
			Next i
			
			GetDataFromExcel = True
		End If
		
		
	End Function
	' Create an instance of Excel
	Public Function InitializeExcel() As Boolean
		On Error GoTo err_InitializeExcel
		
		
		If g_nLinkedVariableCount > 0 Then
			g_appExcel = CreateObject("Excel.Application")
		End If
		InitializeExcel = True
		Exit Function
err_InitializeExcel: 
		InitializeExcel = False
	End Function
	' Function to read in worksheet linked to variable (sCode)
	Public Function LoadExcelWorksheet(ByVal sCode As String) As Object
        Dim i As Short
		Dim bAlreadyLoaded As Boolean
		
		
		
		
		' Loop through variables with excel links
		For i = 0 To g_nLinkedVariableCount - 1
			' If this matches the code passed in
			If StrComp(ga_sFiscalVariable(i), sCode, CompareMethod.Text) = 0 Then
				
				' if no backslash exists in the workbook path then assume that the
				' .xls file is in the same directory as the country file. Append the
				' country file path to the workbook name
				If InStr(1, ga_sExcelWorkbook(i), "\", CompareMethod.Text) = 0 Then
					ga_sExcelWorkbook(i) = m_sCountryFilePath & ga_sExcelWorkbook(i)
				End If
				' Check the workbook exists
				'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
				If Len(Dir(ga_sExcelWorkbook(i), FileAttribute.Normal)) > 0 Then
					
					If Not m_objExcelWorkbook Is Nothing Then
						' Check if workbook is already loaded
						'UPGRADE_WARNING: Couldn't resolve default property of object m_objExcelWorkbook.FullName. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						If StrComp(m_objExcelWorkbook.FullName, ga_sExcelWorkbook(i), CompareMethod.Text) = 0 Then
							bAlreadyLoaded = True
						Else
							' if not then close old workbook
							bAlreadyLoaded = False
							'UPGRADE_WARNING: Couldn't resolve default property of object m_objExcelWorkbook.Close. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
							m_objExcelWorkbook.Close(False)
						End If
					Else
						bAlreadyLoaded = False
					End If
					
					If Not bAlreadyLoaded Then
						' If not already loaded then open
						'UPGRADE_NOTE: Object m_objExcelWorkbook may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
						m_objExcelWorkbook = Nothing
						
						' Parameters: Open(<workbook>, <external links update>, <read only)
						'UPGRADE_WARNING: Couldn't resolve default property of object g_appExcel.Workbooks. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						m_objExcelWorkbook = g_appExcel.Workbooks.Open(ga_sExcelWorkbook(i), 3, False)
						
					End If
					
				Else
					Err.Raise(53,  , "Cannot find '" & ga_sExcelWorkbook(i) & "'")
				End If
			End If
			
		Next i
		
		LoadExcelWorksheet = m_objExcelWorkbook
		
	End Function
	
	'
	' Modifications:
	' 2 May 2003 JWD
	'  -> Added check for trailing path directory separator
	'     character, appending if needed. References to path
	'     variable assume it is present.
	'
	' Store country file path in module level variable. This is used when
	' the workbook name in the country file does not include the path. In that
	' case the country file path used.
	Public Sub SetCountryFilePath(ByVal sCountryFilePath As String)
		m_sCountryFilePath = sCountryFilePath
		
		' 2 May 2003 JWD
		' Test for ending backslash.
		If Right(m_sCountryFilePath, 1) <> "\" Then
			m_sCountryFilePath = m_sCountryFilePath & "\"
		End If
		
	End Sub
	
	'
	' Modifications:
	' 8 Oct 2003 JWD
	'  -> Add alternative code block to be executed when at
	'     end of file on entry to this procedure that will
	'     initialize the linked variable count and the
	'     linked variable and workbook arrays. (C0761)
	'
	' Sub to read in extra data from the country file - the data for the excel
	' links is saved at the end of the country file so, in theory, there are no
	' compatability problems.
	Public Sub ReadExcelLinkData(ByVal hFile As Short)
		
		Dim sVersion As String
		Dim sDummy As String
		Dim i As Short
		
		If Not EOF(hFile) Then
			Input(hFile, sDummy)
			If sDummy = "STARTEXCELLINKDATA" Then
				Input(hFile, sVersion)
				Input(hFile, g_nLinkedVariableCount)
				If g_nLinkedVariableCount > 0 Then
					ReDim ga_sFiscalVariable(g_nLinkedVariableCount - 1)
					ReDim ga_sExcelWorkbook(g_nLinkedVariableCount - 1)
					
					For i = 0 To g_nLinkedVariableCount - 1
						Input(hFile, ga_sFiscalVariable(i))
						Input(hFile, ga_sExcelWorkbook(i))
					Next i
				End If
				
				Input(hFile, sDummy)
			End If
			' 8 Oct 2003 JWD (C0761) Add following for when file doesn't have links
		Else
			g_nLinkedVariableCount = 0
			Erase ga_sFiscalVariable
			Erase ga_sExcelWorkbook
			' End (C0761)
		End If
	End Sub
	
	' Function will return whether a particular variable has a linked spreadsheet
	Public Function IsVariableLinked(ByVal sVariable As String) As Boolean
		Dim i As Short
		
		For i = 0 To g_nLinkedVariableCount - 1
			If Left(ga_sFiscalVariable(i), 3) = Left(sVariable, 3) Then
				IsVariableLinked = True
				Exit For
			End If
		Next i
	End Function
	
	
	' Save the current workbook - this is only called if the preference item has
	' been set.
	Public Function SaveCurrentWorkbook(ByVal wbkThis As Object) As Boolean
		'UPGRADE_WARNING: Couldn't resolve default property of object wbkThis.ReadOnly. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		If Not wbkThis.ReadOnly Then
			'UPGRADE_WARNING: Couldn't resolve default property of object wbkThis.Save. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			wbkThis.Save()
		End If
	End Function
	
	'
	' Modifications:
	' 8 Oct 2003 JWD
	'  -> Add test to make call on m_objExcelWorkbook object
	'     conditional on object instantiation. Corrects
	'     'object variable not set' error in circumstance
	'     where Excel object is instantiated but workbook
	'     is not. (C0760)
	'
	' Clean up object variables and close any open workbooks
	Public Function CleanUpExcel() As Boolean
        If Not g_appExcel Is Nothing Then
            ' 8 Oct 2003 JWD (C0760) Wrap Close method call with conditional
            If Not m_objExcelWorkbook Is Nothing Then
                'UPGRADE_WARNING: Couldn't resolve default property of object m_objExcelWorkbook.Close. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                m_objExcelWorkbook.Close(False)
            End If
            ' End (C0760)
            'UPGRADE_NOTE: Object m_objExcelWorkbook may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            m_objExcelWorkbook = Nothing
            'UPGRADE_NOTE: Object g_appExcel may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            g_appExcel = Nothing
        End If
	End Function
	' Returns whether a 3 letter code is present in a string containing a list
	' of concatenated 3 letter codes.
	Private Function IsCodePresent(ByVal sCode As String, ByRef sList As String) As Boolean
		Dim nPos As Short
		
		nPos = InStr(1, sList, sCode, CompareMethod.Text)
		If nPos > 0 Then
			If (nPos - 1) Mod 3 = 0 Then
				IsCodePresent = True
			Else
				IsCodePresent = False
			End If
		Else
			IsCodePresent = False
		End If
		
		
	End Function
	' Given the text of an excel comment. This routing will find and return a particular tag
	Public Function ParseCode(ByVal sComment As String, ByVal sSectionName As String) As String
		Dim nStart As Short
		Dim nEnd As Short
        Dim sText As String
		Dim nStartCode As Short
		Dim sSearchValue As String
		Dim sCommentNoSpace As String
		
		sSearchValue = "<" & sSectionName & "="
		
		' Get rid of all spaces in comment
		sCommentNoSpace = Replace(sComment, " ", "")
		' Find occurance of eg <OIL=
		nStart = InStr(1, sCommentNoSpace, sSearchValue, CompareMethod.Text)
		If nStart > 0 Then
			' If found then extract code
			nStartCode = nStart + Len(sSearchValue)
			nEnd = InStr(nStartCode, sCommentNoSpace, ">", CompareMethod.Text)
			
			sText = Mid(sCommentNoSpace, nStartCode, nEnd - nStartCode)
			
			sText = Replace(sText, """", "")
		End If
		' Return code extracted from tag
		ParseCode = sText
	End Function
	' Given the current fiscal variable number, the 3 letter code for the fiscal variable being sought return
	' the net amount for any fiscal variable.
	Private Function GetPrevFiscalVariable(ByVal nCurrentVar As Short, ByVal sCode As String, ByRef a_fData() As Single) As Boolean
		Dim i As Short
		Dim nVarPos As Short
		
		' If the current fiscal variable is not the first
		If nCurrentVar <> 1 Then
			' Loop through from 1 to TDT - number of fiscal definition variables
			For i = 1 To TDT
				' If this is the variable you are looking for
				If StrComp(TD(i, 1), sCode, CompareMethod.Text) = 0 Then
					' Store the position of this variable in fiscal def
					nVarPos = i
					Exit For
				End If
			Next i
			ReDim a_fData(LG)
			' If the variable has been found
			If nVarPos <> 0 Then
				' If the sought variable is prior to the current variable then return contents of RVN
				If nVarPos < nCurrentVar Then
					For i = 1 To LG
						a_fData(i) = RVN(i, nVarPos)
					Next i
					GetPrevFiscalVariable = True
					' If the sought variable is after the current variable then return the contents of RVN
					' shifted back by one year.
				ElseIf nVarPos > nCurrentVar Then 
					For i = 2 To LG
						a_fData(i) = RVN(i - 1, nVarPos)
					Next i
					GetPrevFiscalVariable = True
				End If
			End If
		End If
	End Function
	'Is the current workbook the same of the previous linked workbook?
	Public Function SameAsPreviousLinkedWorkbook(ByVal nCurrentVar As Short) As Boolean
		Dim sPrevVar As String
		Dim i As Short
		Dim sCurrentWorkbook As String
		Dim sCurrVar As String
		
		
		
		' Find previous variable name
		If nCurrentVar > 1 Then
			
			' Find current variable name
			sCurrVar = TD(nCurrentVar, 1)
			
			' Find workbook associated with current variable
			For i = 0 To g_nLinkedVariableCount - 1
				If Left(ga_sFiscalVariable(i), 3) = Left(sCurrVar, 3) Then
					sCurrentWorkbook = ga_sExcelWorkbook(i)
				End If
			Next i
			
			sPrevVar = TD(nCurrentVar - 1, 1)
			For i = 0 To g_nLinkedVariableCount - 1
				If Left(ga_sFiscalVariable(i), 3) = Left(sPrevVar, 3) Then
					If StrComp(ga_sExcelWorkbook(i), sCurrentWorkbook, CompareMethod.Text) = 0 Then
						SameAsPreviousLinkedWorkbook = True
						Exit For
					End If
				End If
			Next i
		End If
		
	End Function
End Module