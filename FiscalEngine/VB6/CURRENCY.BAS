Attribute VB_Name = "CURRENCY"
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
DefSng A-Z

Public Type tCURRENCY
    sCurrCode As String  ' The 3 letter code of the currency
    sCurrStart As String    ' The start year of the set of currency
    nYears As Integer       ' Number of elements in the fCurr() array
    fCurr() As Single       ' The currency conversion rates to the dollar
End Type

Private m_udtCurrency() As tCURRENCY


'$include: 'ctyin1.bg'
'$include: 'currency.bi'

'-----------------------------------------------------------------------
'$subtitle: 'Procedure: ConvertCurrency'
'$page
Sub ConvertCurrency()
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
      Dim x As Single
      Dim i As Integer
      Dim ii As Integer
'-----------------------------------------------------------------------

      ReDim SamCur(LG), CUR(LG)

      If BDebugging Then
        Open "curr.log" For Append As #16
          Print #16, "in ConvertCurrency     code = "; sCur
        Close #16
      End If


' ============================================================================

      If sCur = "USA" Then                    'if it is USA
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
        Open "curr.log" For Append As #16
          Print #16, "curr <> USA  FExchng$ = "; FExchng$
        Close #16
      End If

29010   If Len(Dir$(FExchng$)) > 0 Then
           currfile% = FreeFile
           Open FExchng$ For Input As #currfile%
              Input #currfile%, CurrencyRecs%
              If BDebugging Then
                Open "curr.log" For Append As #16
                  Print #16, "CurrencyRecs% = "; CurrencyRecs%
                Close #16
              End If
              ReDim sCurrExch(3, CurrencyRecs%)
              For i = 1 To CurrencyRecs%
                 Input #currfile%, sCurrExch(1, i)
                 Input #currfile%, sCurrExch(2, i)
                 Input #currfile%, sCurrExch(3, i)
                 If BDebugging Then
                    Open "curr.log" For Append As #16
                       Print #16, "sCurrExch(1,"; i; ") = "; sCurrExch(1, i)
                    Close #16
                 End If
              Next i
              ReDim CurrExc!(LG, CurrencyRecs%)
              For i = 1 To CurrencyRecs%
                 For ii = 1 To LG
                    Input #currfile%, CurrExc!(ii, i)
                    If BDebugging Then
                      Open "curr.log" For Append As #16
                        Print #16, "Currexc!("; ii; ","; i; ") = "; CurrExc!(ii, i)
                      Close #16
                    End If
                 Next ii
              Next i
           Close #currfile%

          foundit$ = "N"
          For i = 1 To CurrencyRecs%
            If foundit$ = "N" Then
              If sCur = sCurrExch(1, i) Then
                If BDebugging Then
                  Open "curr.log" For Append As #16
                    Print #16, "Found the code!"
                  Close #16
                End If
                For ii = 1 To LG
                  SamCur(ii) = CurrExc!(ii, i)
                  If BDebugging Then
                    Open "curr.log" For Append As #16
                      Print #16, "copy CurrExc() into samcur()   SamCur(ii) = "; SamCur(ii)
                    Close #16
                  End If
                Next ii
                foundit$ = "Y"
                Exit For
              End If
            End If
          Next i
          If foundit$ = "Y" Then
            For i = 1 To LG
              If CURT = 0 Then  'this is the 1st CUR line in fiscal definition
                CUR(i) = SamCur(i)
                If BDebugging Then
                  Open "curr.log" For Append As #16
                    Print #16, "fill in CUR()   Cur("; i; ") = "; CUR(i)
                  Close #16
                End If
              Else  'this is the 2nd - nth CUR line in fiscal definition
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
        Open "curr.log" For Append As #16
          For i = 1 To LG
            Print #16, "end of ConvertCurrency SUB   CUR("; i; ") = "; CUR(i)
          Next i
        Close #16
      End If

      If BDebugging Then
        Open "curr.log" For Append As #16
          Print #16, "leaving ConvertCurrency"
        Close #16
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
    
    Dim nInIndex As Integer ' Contains the index in udtCurrency of the input currency
    Dim nOutIndex As Integer ' Contains the index in udtCurrency of the output currency
    Dim i As Integer
    Dim j As Integer
    
    
    nInIndex = -1
    nOutIndex = -1
    ' Read in the currencies and find a match
    For i = 1 To UBound(m_udtCurrency)
        With m_udtCurrency(i)
            
            If StrComp(.sCurrCode, sInCode, vbTextCompare) = 0 Then nInIndex = i
            If StrComp(.sCurrCode, sOutCode, vbTextCompare) = 0 Then nOutIndex = i
            
        End With
    Next i
    
    ReDim fConvert(LG)
    
    If nInIndex = -1 Then ' If not found then assume USA
        For i = 1 To LG
            fConvert(i) = 1
        Next i
    Else
        For i = 1 To LG
            With m_udtCurrency(nInIndex)
                Dim conIndex As Single
                conIndex = i + (YR - CInt(.sCurrStart))
                
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
                If .fCurr(i + (YR - CInt(.sCurrStart))) > 0 Then
                    fConvert(i) = .fCurr(i + (YR - CInt(.sCurrStart))) / fConvert(i)
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
    Dim i As Integer
    Dim j As Integer
    Dim fConversionFactor() As Single
    
    On Error GoTo err_ConvertInputData
    
    
    GetCurrencyConversion "USA", sCode, fConversionFactor()
    
    
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
    Err.Raise Err.Description & ": ConvertInputData @ " & Err.Source
    
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
Public Sub GetCurrencyConversionSpecific(ByVal sInCode As String, ByVal sOutCode As String, ByRef fConvert() As Single, ByVal nStartYear As Integer, ByVal nDuration As Integer)
    '~^********************************************************************************
    '? Author:       Glyn
    '? Date Created: 23-Apr-02
    '~^
    '~ Copied from GetCurrencyConversion but added a startyear and duration
    '~ The routine will now use these rather than YR and LG
    '~
    '~^
    '~^********************************************************************************
        
    
    Dim nInIndex As Integer ' Contains the index in udtCurrency of the input currency
    Dim nOutIndex As Integer ' Contains the index in udtCurrency of the output currency
    Dim i As Integer
    Dim j As Integer
    
    
    nInIndex = -1
    nOutIndex = -1
    ' Read in the currencies and find a match
    For i = 1 To UBound(m_udtCurrency)          ' nCurrencies
        With m_udtCurrency(i)
            
            If StrComp(.sCurrCode, sInCode, vbTextCompare) = 0 Then nInIndex = i
            If StrComp(.sCurrCode, sOutCode, vbTextCompare) = 0 Then nOutIndex = i
            
            
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
                fConvert(i) = .fCurr(i + (nStartYear - CInt(.sCurrStart)))
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
                If .fCurr(i + (nStartYear - CInt(.sCurrStart))) > 0 Then
                    fConvert(i) = .fCurr(i + (nStartYear - CInt(.sCurrStart))) / fConvert(i)
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
    
    Dim hFile As Integer
    Dim i As Integer
    Dim j As Integer
    
    Dim nCurrencies As Integer ' Number of elements in the udtCurrency array
    
    hFile = FreeFile
    
    Open TempDir & c_sCURRENCYFILE For Input As hFile
    
        Input #hFile, nCurrencies
        
        ReDim m_udtCurrency(1 To nCurrencies)
        
        ' Read in the currencies
        For i = 1 To nCurrencies
            With m_udtCurrency(i)
                Input #hFile, .sCurrCode, .sCurrStart, .nYears
                ReDim .fCurr(.nYears)
                
                For j = 1 To .nYears
                    Input #hFile, .fCurr(j)
                Next j
                
            End With
        Next i

    Close #hFile
    
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

    ReDim m_udtCurrency(0 To 0)
    
End Sub
