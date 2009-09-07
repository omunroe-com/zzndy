Option Strict Off
Option Explicit On
Module CURRENCY
	' $linesize: 132
	' $title:    'GIANT v6.1 - 1995                         CURRENCY.BAS'
	' $subtitle: 'Calculate Currency Exchange'
	' **********************************************************************
	' *         COPYRIGHT - PETROCONSULTANTS, INC. - 1995, 1996            *
	' *                     ALL RIGHTS RESERVED                            *
	' **********************************************************************
	' *  This program file is proprietary information of Petroconsultants, *
	' *  Incorporated.  Unauthorized use for any purpose is prohibited.    *
	' **********************************************************************
	'-----------------------------------------------------------------------
	' Modifications:
	' 3 Mar 1995 JWD
	'          Converted module level executable code to subroutine.
	' 6 Feb 1996 JWD
	'          Changed ConvertCurrency().
	'          Add interface declaration include file CURRENCY.BI
	'          Replaced include file CTYIN.BAS with CTYIN1.BG.
	'          Add explicit declaration of default storage class as Single.
	'
	' 20 Jan 2003 GDP
	'  -> Changed ConvertInputData().
	'
	' 15 Mar 2004 JWD
	'  -> Add LoadCurrencyFile().
	'  -> Add module-level variable m_udtCurrency() to store
	'     loaded currency forecasts.
	'  -> Changed GetCurrencyConversion().
	'  -> Changed GetCurrencyConversionSpecific().
	'
	' 21 Jun 2005 JWD
	'  -> Add LoadCurrencyFileAMPE(). (C0880)
	'-----------------------------------------------------------------------
	'$dynamic
	'UPGRADE_NOTE: DefSng A-Z statement was removed. Variables were explicitly declared as type Single. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="92AFD3E3-440D-4D49-A8BF-580D74A8C9F2"'
	
	Public Structure tCURRENCY
		Dim sCurrCode As String ' The 3 letter code of the currency
		Dim sCurrStart As String ' The start year of the set of currency
		Dim nYears As Short ' Number of elements in the fCurr() array
		Dim fCurr() As Single ' The currency conversion rates to the dollar
	End Structure
	
	Private m_udtCurrency() As tCURRENCY
	
	
	'$include: 'ctyin1.bg'
	'$include: 'currency.bi'
	
	'-----------------------------------------------------------------------
	'$subtitle: 'Procedure: ConvertCurrency'
	'$page
	Sub ConvertCurrency()
		Dim foundit As String
		Dim CurrencyRecs As Short
		Dim currfile As Short
		'-----------------------------------------------------------------------
		' This program reads in currency file and returns CUR() to FiscalDef.
		'-----------------------------------------------------------------------
		' Modifications:
		' 6 Feb 1996 JWD
		'          Removed bDebugging from parameter list, now in common.
		'          Removed include of SCRA1IN.BAS and SCRA1OUT.BAS.
		'          Removed ReDims of array variables written on SCRA1.SCR.
		' 20 Feb 1996 JWD
		'        Change CUR$ to sCur, duplicate definition (CUR()).
		'        Change CurrExc$ to sCurrExch, duplicate def.
		'-----------------------------------------------------------------------
        Dim i As Short
		Dim ii As Short
		'-----------------------------------------------------------------------
		
		Dim SamCur(LG) As Single
		ReDim CUR(LG)
		
		If BDebugging Then
			FileOpen(16, "curr.log", OpenMode.Append)
			PrintLine(16, "in ConvertCurrency     code = " & sCur)
			FileClose(16)
		End If
		
		
		' ============================================================================
		
		If sCur = "USA" Then 'if it is USA
			For i = 1 To LG
				SamCur(i) = 1
				If LCur(i) <> 0 Then
					CUR(i) = SamCur(i) / LCur(i)
				Else
					CUR(i) = 1
				End If
			Next i
		Else
			
			If BDebugging Then
				FileOpen(16, "curr.log", OpenMode.Append)
				PrintLine(16, "curr <> USA  FExchng$ = " & FExchng)
				FileClose(16)
			End If
			
29010: 'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
			If Len(Dir(FExchng)) > 0 Then
				currfile = FreeFile
				FileOpen(currfile, FExchng, OpenMode.Input)
				Input(currfile, CurrencyRecs)
				If BDebugging Then
					FileOpen(16, "curr.log", OpenMode.Append)
					PrintLine(16, "CurrencyRecs% = " & CurrencyRecs)
					FileClose(16)
				End If
				ReDim sCurrExch(3, CurrencyRecs)
				For i = 1 To CurrencyRecs
					Input(currfile, sCurrExch(1, i))
					Input(currfile, sCurrExch(2, i))
					Input(currfile, sCurrExch(3, i))
					If BDebugging Then
						FileOpen(16, "curr.log", OpenMode.Append)
						PrintLine(16, "sCurrExch(1," & i & ") = " & sCurrExch(1, i))
						FileClose(16)
					End If
				Next i
				Dim CurrExc(LG, CurrencyRecs) As Single
				For i = 1 To CurrencyRecs
					For ii = 1 To LG
						Input(currfile, CurrExc(ii, i))
						If BDebugging Then
							FileOpen(16, "curr.log", OpenMode.Append)
							PrintLine(16, "Currexc!(" & ii & "," & i & ") = " & CurrExc(ii, i))
							FileClose(16)
						End If
					Next ii
				Next i
				FileClose(currfile)
				
				foundit = "N"
				For i = 1 To CurrencyRecs
					If foundit = "N" Then
						If sCur = sCurrExch(1, i) Then
							If BDebugging Then
								FileOpen(16, "curr.log", OpenMode.Append)
								PrintLine(16, "Found the code!")
								FileClose(16)
							End If
							For ii = 1 To LG
								SamCur(ii) = CurrExc(ii, i)
								If BDebugging Then
									FileOpen(16, "curr.log", OpenMode.Append)
									PrintLine(16, "copy CurrExc() into samcur()   SamCur(ii) = " & SamCur(ii))
									FileClose(16)
								End If
							Next ii
							foundit = "Y"
							Exit For
						End If
					End If
				Next i
				If foundit = "Y" Then
					For i = 1 To LG
						If CURT = 0 Then 'this is the 1st CUR line in fiscal definition
							CUR(i) = SamCur(i)
							If BDebugging Then
								FileOpen(16, "curr.log", OpenMode.Append)
								PrintLine(16, "fill in CUR()   Cur(" & i & ") = " & CUR(i))
								FileClose(16)
							End If
						Else 'this is the 2nd - nth CUR line in fiscal definition
							If LCur(i) <> 0 Then
								CUR(i) = SamCur(i) / LCur(i) 'new currenry factor = this one / the last one
							Else
								CUR(i) = 1
							End If
						End If
					Next i
					'saving the last table into LCur()
					For i = 1 To LG
						LCur(i) = SamCur(i)
					Next i
				Else
					For i = 1 To LG
						CUR(i) = 1
					Next i
				End If
			Else
				For i = 1 To LG
					CUR(i) = 1
				Next i
			End If
		End If
		If BDebugging Then
			FileOpen(16, "curr.log", OpenMode.Append)
			For i = 1 To LG
				PrintLine(16, "end of ConvertCurrency SUB   CUR(" & i & ") = " & CUR(i))
			Next i
			FileClose(16)
		End If
		
		If BDebugging Then
			FileOpen(16, "curr.log", OpenMode.Append)
			PrintLine(16, "leaving ConvertCurrency")
			FileClose(16)
		End If
		
	End Sub
	
	'
	' Modifications:
	' 15 Mar 2004 JWD
	'  -> Remove file read code, now performed by
	'     LoadCurrencyFile once per run. Replace with code
	'     to search for the sought currencies in the loaded
	'     m_udtCurrency() array.
	'     NOTE: This routine now assumes that the array was
	'     previously loaded.
	'
	' 24 July 2009 AV
	'  -> Currency conversion rates were throwing 'Index out of range'
	'     error. Fixed that by using last available conversion rate if
	'     not enough conversion rates defined.
	'
	Public Sub GetCurrencyConversion(ByVal sInCode As String, ByVal sOutCode As String, ByRef fConvert() As Single)
		'~^********************************************************************************
		'? Author:       Glyn
		'? Date Created: 16-Apr-99
		'~^
		'~ Routine to read in a currency file into an array of user defined types and produce
		'~ a currency conversion based on the two 3 letter code strings being passed in. The
		'~ param fConvert() will be filled with this conversion factor.
		'~^
		'~^********************************************************************************
		
		Dim nInIndex As Short ' Contains the index in udtCurrency of the input currency
		Dim nOutIndex As Short ' Contains the index in udtCurrency of the output currency
		Dim i As Short

		
		nInIndex = -1
		nOutIndex = -1
		' Read in the currencies and find a match
		For i = 1 To UBound(m_udtCurrency)
			With m_udtCurrency(i)
				
				If StrComp(.sCurrCode, sInCode, CompareMethod.Text) = 0 Then nInIndex = i
				If StrComp(.sCurrCode, sOutCode, CompareMethod.Text) = 0 Then nOutIndex = i
				
			End With
		Next i
		
		ReDim fConvert(LG)
		
		Dim conIndex As Single
		If nInIndex = -1 Then ' If not found then assume USA
			For i = 1 To LG
				fConvert(i) = 1
			Next i
		Else
			For i = 1 To LG
				With m_udtCurrency(nInIndex)
					conIndex = i + (YR - CShort(.sCurrStart))
					
					' Use last available conversion rate if too few of them defined
					If conIndex > UBound(.fCurr) Then
						conIndex = UBound(.fCurr)
					End If
					
					fConvert(i) = .fCurr(conIndex)
				End With
			Next i
		End If
		
		If nOutIndex = -1 Then ' If not found then assume USA
			For i = 1 To LG
				If fConvert(i) > 0 Then
					fConvert(i) = 1 / fConvert(i)
				End If
			Next i
		Else
			For i = 1 To LG
				With m_udtCurrency(nOutIndex)
					If .fCurr(i + (YR - CShort(.sCurrStart))) > 0 Then
						fConvert(i) = .fCurr(i + (YR - CShort(.sCurrStart))) / fConvert(i)
					End If
				End With
			Next i
		End If
	End Sub
	' Modifications
	
	' GDP 20 Jan 2003
	'   -> Changed loop bounds for A array conversion loop
	Public Sub ConvertInputData(ByVal sCode As String)
		'~^********************************************************************************
		'? Author:       Glyn
		'? Date Created: 16-Apr-99
		'~^
		'~ Sub to convert the input data into the appropriate currency ( defined on GETDATA
		'~ line of run file in param 5.
		'~^
		'~^********************************************************************************
		Dim i As Short
		Dim j As Short
		Dim fConversionFactor() As Single
		
		On Error GoTo err_ConvertInputData
		
		
		GetCurrencyConversion("USA", sCode, fConversionFactor)
		
		
		If MY3T <> 0 Then
			For i = 1 To MY3T
				If my3(i, 3) - YR + 1 <= LG Then
					my3(i, 5) = my3(i, 5) * fConversionFactor(my3(i, 3) - YR + 1)
				End If
				
			Next i
		End If
		
		For i = 1 To LG
			' GDP 20 Jan 2003
			' Changed the lower and upper bounds of the loop
			' from 7 and 20 to constants
			For j = gc_nAMINPRC To gc_nASIZE
				
				A(i, j) = A(i, j) * fConversionFactor(i)
				
			Next j
		Next i
		
		For i = 1 To LG
			OPEX(i) = OPEX(i) * fConversionFactor(i)
			' GDP 13 Nov 2001
			' Bug fix OMV - consolidated Government Repayment currency was incorrect
			If g_bPTCons Then
				TOTREPAY(i) = TOTREPAY(i) * fConversionFactor(i)
				TOTFINANCE(i) = TOTFINANCE(i) * fConversionFactor(i)
			End If
		Next i
		
		
		Exit Sub
err_ConvertInputData: 
		Err.Raise(CInt(Err.Description & ": ConvertInputData @ " & Err.Source))
		
		Exit Sub
	End Sub
	
	'
	' Modifications:
	' 15 Mar 2004 JWD
	'  -> Remove file read code, now performed by
	'     LoadCurrencyFile once per run. Replace with code
	'     to search for the sought currencies in the loaded
	'     m_udtCurrency() array.
	'     NOTE: This routine now assumes that the array was
	'     previously loaded.
	'
	Public Sub GetCurrencyConversionSpecific(ByVal sInCode As String, ByVal sOutCode As String, ByRef fConvert() As Single, ByVal nStartYear As Short, ByVal nDuration As Short)
		'~^********************************************************************************
		'? Author:       Glyn
		'? Date Created: 23-Apr-02
		'~^
		'~ Copied from GetCurrencyConversion but added a startyear and duration
		'~ The routine will now use these rather than YR and LG
		'~
		'~^
		'~^********************************************************************************
		
		
		Dim nInIndex As Short ' Contains the index in udtCurrency of the input currency
		Dim nOutIndex As Short ' Contains the index in udtCurrency of the output currency
		Dim i As Short

		nInIndex = -1
		nOutIndex = -1
		' Read in the currencies and find a match
		For i = 1 To UBound(m_udtCurrency) ' nCurrencies
			With m_udtCurrency(i)
				
				If StrComp(.sCurrCode, sInCode, CompareMethod.Text) = 0 Then nInIndex = i
				If StrComp(.sCurrCode, sOutCode, CompareMethod.Text) = 0 Then nOutIndex = i
				
				
			End With
		Next i
		
		ReDim fConvert(nDuration)
		
		If nInIndex = -1 Then ' If not found then assume USA
			For i = 1 To nDuration
				fConvert(i) = 1
			Next i
		Else
			For i = 1 To nDuration
				With m_udtCurrency(nInIndex)
					fConvert(i) = .fCurr(i + (nStartYear - CShort(.sCurrStart)))
				End With
			Next i
		End If
		
		If nOutIndex = -1 Then ' If not found then assume USA
			For i = 1 To nDuration
				If fConvert(i) > 0 Then
					fConvert(i) = 1 / fConvert(i)
				End If
			Next i
		Else
			For i = 1 To nDuration
				With m_udtCurrency(nOutIndex)
					If .fCurr(i + (nStartYear - CShort(.sCurrStart))) > 0 Then
						fConvert(i) = .fCurr(i + (nStartYear - CShort(.sCurrStart))) / fConvert(i)
					End If
				End With
			Next i
		End If
	End Sub
	
	'
	' 15 Mar 2004 JWD New
	'
	' Split out the reading of the file into storage so
	' it is only done once per run.
	'
	Public Sub LoadCurrencyFile()
		
		Const c_sCURRENCYFILE As String = "CURRENCY.DAT"
		
		Dim hFile As Short
		Dim i As Short
		Dim j As Short
		
		Dim nCurrencies As Short ' Number of elements in the udtCurrency array
		
		hFile = FreeFile
		
		FileOpen(hFile, TempDir & c_sCURRENCYFILE, OpenMode.Input)
		
		Input(hFile, nCurrencies)
		
		'UPGRADE_WARNING: Lower bound of array m_udtCurrency was changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
		ReDim m_udtCurrency(nCurrencies)
		
		' Read in the currencies
		For i = 1 To nCurrencies
			With m_udtCurrency(i)
				Input(hFile, .sCurrCode)
				Input(hFile, .sCurrStart)
				Input(hFile, .nYears)
				ReDim .fCurr(.nYears)
				
				For j = 1 To .nYears
					Input(hFile, .fCurr(j))
				Next j
				
			End With
		Next i
		
		FileClose(hFile)
		
	End Sub
	
	'
	' Modifications:
	'
	'
	' 21 Jun 2005 JWD New (C0880)
	'
	' Allocate the m_udtCurrency() array for use by
	' ASPEEngineEx interface in post-tax consolidations.
	'
	Public Sub LoadCurrencyFileAMPE()
		
		ReDim m_udtCurrency(0)
		
	End Sub
End Module