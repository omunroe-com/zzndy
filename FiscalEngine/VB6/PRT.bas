Attribute VB_Name = "PRTCalc"
' Name:        PRTCalc.BAS
' Function:    Gross Production, Revenue and Costs Reports
'---------------------------------------------------------
' ********************************************************
' *       COPYRIGHT © 1990-2001 IHS ENERGY GROUP         *
' *                 ALL RIGHTS RESERVED                  *
' ********************************************************
' *   This program file is proprietary information of    *
' *                  IHS Energy Group                    *
' *   Unauthorized use for any purpose is prohibited.    *
' ********************************************************
'---------------------------------------------------------
' Modifications:
'  -> 1 Oct 2001 GDP Changed CalcUKRoyLiability(). Added units encoded in the titles. Removed Resume.
'  -> 2 Oct 2001 GDP Changed order of mc_nBL1, mc_nBL2 module level constants
'  -> 2 Oct 2001 GDP Changed CalcPRTPayback a_fPBKINC(i)(Payback Income) now included AJ1 and AJ2
'  -> 2 Oct 2001 GDP Changed CalcSafeguardPRT Set ma_fUPLCPX() (Upliftable capex)
'     to capex available for uplift for uplift period and pre production period.
'  -> 2 Oct 2001 GDP Changed CalcSafeguardPRT - Set rev for PRT to not include AJ2
'  -> 2 Oct 2001 GDP Changed CalcSafeguardPRT - Include capex in Adjusted profit for pre-production
'     periods.
'  -> 2 Oct 2001 GDP Changed CalcSafeguardPRT - Opex for PRT is now 100% OP1 + 100% OP2
'  -> 2 Oct 2001 GDP Changed CalcPRTPayback - Code to output the uplift and safeguard periods
'  -> 4 Oct 2001 GDP Changed CalcProfitBeforeOilAllow - ma_fASSPRTPFT() is now module level
'  -> 4 Oct 2001 GDP Changed DeferCapexInSGD - Added a_fDECUMCPXAVAIL() variable - Cum Deferable capex avail
'  -> 4 Oct 2001 GDP Changed CalcCarrybackPRTLosses - Altered calc of a_fADJCUMPFTBEFOA
'  -> 4 Oct 2001 GDP Added SumArray routine.
'  -> 4 Oct 2001 GDP Added ma_fTOTPRTPAIDEXCINT() module level array.
'  -> 4 Oct 2001 GDP CalcUKRoyLiability - Now sets RVN() to be correct value.
'  -> 4 Oct 2001 GDP Changed CalcUKRoyaltyPaid - Removed spurious Exit Sub
'  -> 4 Oct 2001 GDP Changed CalcPRTLiability - Now sets RVN() to be correct value.
'  -> 4 Oct 2001 GDP Changed CalcPRTPaid - Now sets RVN() to be correct value.
'  -> 4 Oct 2001 GDP Added GetAllCapex routine
'  -> 4 Oct 2001 GDP Changed CalcIntOnPRTRefund - a_fTOTPRTPAIDEXCINT changed to module level
'                    Call to GetAllCapex,a_fINTREC calc changed, reporting changed
'  -> 5 Oct 2001 GDP Added ma_fBlank() for blank lines in report
'  -> 5 Oct 2001 GDP Changed CalcUKRoyLiability,CalcUKRoyaltyPaid,CalcPRTPayback,CalcSafeguardPRT
'                    CalcProfitBeforeOilAllow,AddToReport,DeferCapexInSGD,CalcOilAllowance
'                    MainStreamPRTCalc,CalcCarrybackPRTLosses,CalcIntOnPRTRefund to reformat
'                    the PRT report
'  -> 22 Jan 2002 GDP Changed CalcPRTPayback
'
' 12 Jan 2004 JWD
'  -> Changed GenerateReportPage(). (C0772)
'
' 3 Feb 2004 JWD
'  -> Changed GenerateReportPage(). (C0776)
'
' 17 May 2005 JWD
'  -> Changed CalcPRTPayback(). (C0878)
'  -> Changed CalcSafeguardPRT(). (C0878)
'  -> Undefined the constant symbols mc_nAB1 and mc_nAB2.
'     The values assigned are no longer correct with
'     addition of new balance categories. References in
'     code are replaced with global symbols that represent
'     the same thing. If there are any changes in the
'     future to the codes list, it should not be necessary
'     to make any changes here. (C0878)
'
Option Explicit

Public Type tSEMIANNUAL
    sCode As String
    a_fData() As Single
End Type

Public a_udtSemiAnnualInput() As tSEMIANNUAL

Public a_udtSemiAnnualReport() As tSEMIANNUAL
Private m_nReportItems As Integer

Private ma_fRYL() As Single
Private ma_fRYP() As Single
Private ma_fPRDREV() As Single
Private ma_fAJ4() As Single
Private ma_fAJ5() As Single
Private ma_fAJ1() As Single
Private ma_fTPTRecpt() As Single
Private ma_fADJPFT() As Single
Private ma_fREVPRT() As Single
Private ma_fSGDPRT() As Single
Private ma_fSGDBASE() As Single
Private ma_fUPLCPX() As Single
Private ma_fCUMUPLCPX() As Single
Private ma_fCPXAVLUPL() As Single
Private ma_fASSPFTBEFOA() As Single
Private ma_fCUMASSPFTBEFOA() As Single
Private ma_fDEFCPXALLCETAKEN() As Single
Private ma_fOA() As Single
Private ma_fPRTLIABLE() As Single
Private ma_fADJPRTPAID() As Single
Private ma_fPRTPAID() As Single
Private ma_fCUMPRTLIABLE() As Single
Private ma_fCUMADJPRTLBLWTHSGD() As Single
Private ma_fTOTPRTPAID() As Single
Private ma_fASSPRTPFT() As Single
Private ma_fTOTPRTPAIDEXCINT() As Single

Private ma_fBlank(0) As Single

Dim m_nUplift As Integer
Dim m_nSafeguard As Integer

Const mc_nBNS  As Integer = 1, mc_nLSE As Integer = 2, mc_nREN As Integer = 3, mc_nGEO As Integer = 4, mc_nEDH As Integer = 5, mc_nEDS As Integer = 6
Const mc_nADH  As Integer = 7, mc_nASC  As Integer = 8, mc_nDNP  As Integer = 9, mc_nDVP  As Integer = 10, mc_nPLF  As Integer = 11, mc_nFCL  As Integer = 12
Const mc_nTRN  As Integer = 13, mc_nEOR As Integer = 14, mc_nCP1 As Integer = 15, mc_nCP2 As Integer = 16, mc_nCP3 As Integer = 17, mc_nCP4 As Integer = 18
Const mc_nCP5  As Integer = 19, mc_nCP6 As Integer = 20, mc_nCP7 As Integer = 21, mc_nCP8 As Integer = 22, mc_nCP9 As Integer = 23, mc_nABN As Integer = 24
' 17 May 2005 JWD (C0878) 'Undefine' mc_nAB1, mc_nAB2
Const mc_nBAL  As Integer = 25, mc_nBL1 As Integer = 26, mc_nBL2 As Integer = 27    ' were: , mc_nAB1 As Integer = 28, mc_nAB2 As Integer = 29
Private Sub ReportInput()
    Dim i As Integer
    For i = 1 To 20
        AddToReport a_udtSemiAnnualInput(i).sCode, a_udtSemiAnnualInput(i).a_fData()
    Next i
End Sub



Public Sub CalcUKRoyLiability(ByVal nCtyFileLine As Integer)
    Dim nPRTStart As Integer
On Error GoTo err_CalcUKRoyLiability
    
    Dim a_sRateVarTitle() As String
    Dim a_fRateData() As Single
    Dim a_fVarRates() As Single
    Dim a_fOPCAllce() As Single
    Dim a_fTPTRecpt() As Single
    Dim a_fCaTAllce() As Single
    
    Dim a_fRYL() As Single
    Dim a_fRYLBase() As Single
    Dim a_fTEMP() As Single
    
    
    Dim i As Integer, j As Integer
    Dim nCount As Integer
    Dim nParam As Integer
    
    Dim fRoyRate As Single
    Dim fCaTPct As Single
    
    ' Need to read semi annual data
    ReadSemiAnnualData
    
    ConvertSemiAnnualData RF$(3)
    'ReportInput
    
    
    ReDim a_fTEMP(1 To LG * 2) As Single
    ReDim a_fCaTAllce(1 To LG * 2) As Single
    ReDim a_fRYLBase(1 To LG * 2) As Single
    
    
    
    
    
    ReDim a_fOPCAllce(1 To LG * 2)
    
    
    GetTPTREC ma_fTPTRecpt()
    GetPrdRev ma_fPRDREV()
    
    For i = 1 To LG * 2
        
        a_fOPCAllce(i) = a_udtSemiAnnualInput(11).a_fData(i) * 0.6 _
                       + a_udtSemiAnnualInput(12).a_fData(i) * 1 _
                       + a_udtSemiAnnualInput(13).a_fData(i) * 0
        
        a_fCaTAllce(i) = Max(a_fOPCAllce(i) - ma_fTPTRecpt(i), 0)
        
    Next i
    AddToReport "ROYALTY|Production Revenues|" & sCur, ma_fPRDREV()
    
    AddToReport "OPC allowances|" & sCur, a_fOPCAllce()
    AddToReport "Third Party Tariff receipts|" & sCur, ma_fTPTRecpt()
    
    'CarryForward a_fOPCAllce(), ma_fTPTRecpt(), a_fCaTAllce()
    
    AddToReport "C & T Allowances|" & sCur, a_fCaTAllce()
    
    Select Case ma_fAJ4(1)
        Case 1
            fRoyRate = 0.125
            fCaTPct = 1
        Case 2
            fRoyRate = 0.125
            fCaTPct = 0
        Case 3
            fRoyRate = 0
            fCaTPct = 0
        Case 4
            fRoyRate = 0.125
            fCaTPct = 1
    End Select
    
    For i = 1 To LG * 2
        a_fCaTAllce(i) = a_fCaTAllce(i) * fCaTPct
        a_fTEMP(i) = ma_fPRDREV(i) - a_fCaTAllce(i)
    Next i
    
    SingleCarryForward a_fTEMP(), a_fRYLBase()
    'CarryForward ma_fPRDREV(), a_fCaTAllce(), a_fRYLBase()
    'AddToReport "RLY Base without lcf|" & sCur, a_fRYLBase()
    AddToReport "Royalty Base|" & sCur, a_fRYLBase()
    
    ReDim ma_fRYL(1 To LG * 2)
    For i = 1 To LG * 2
        ma_fRYL(i) = a_fRYLBase(i) * fRoyRate
    Next i
    AddToReport "Royalty Liability|" & sCur, ma_fRYL()
    
    
    For i = 1 To LG
        RVN(i, nCtyFileLine) = ma_fRYL(i * 2) + ma_fRYL(i * 2 - 1)
    Next i
    
    Exit Sub
    
    
err_CalcUKRoyLiability:
    MsgBox "CalcUKRoyLiability" & Err.Description
    
    Stop
   
    
End Sub

Private Sub GetPrdRev(ByRef a_fPRDREV() As Single)
    Dim i As Integer
    
    ReDim a_fPRDREV(1 To LG * 2)

    For i = 1 To LG * 2
        
        a_fPRDREV(i) = a_udtSemiAnnualInput(1).a_fData(i) * a_udtSemiAnnualInput(7).a_fData(i) _
                     + a_udtSemiAnnualInput(2).a_fData(i) * a_udtSemiAnnualInput(8).a_fData(i) _
                     + a_udtSemiAnnualInput(3).a_fData(i) * a_udtSemiAnnualInput(9).a_fData(i) _
                     + a_udtSemiAnnualInput(4).a_fData(i) * a_udtSemiAnnualInput(10).a_fData(i)
    Next i
End Sub
Private Sub GetTPTREC(ByRef a_fTPTRecpt() As Single)
    Dim i As Integer
    
    ReDim a_fTPTRecpt(1 To LG * 2)

    For i = 1 To LG * 2
        
        a_fTPTRecpt(i) = a_udtSemiAnnualInput(16).a_fData(i) _
                       + a_udtSemiAnnualInput(17).a_fData(i)
    Next i
End Sub

Public Sub CalcUKRoyaltyPaid(ByVal nCtyFileLine As Integer)
    Dim a_fAJ4() As Single
    Dim i As Integer
    
    
    
    ReDim ma_fRYP(1 To LG * 2)
    For i = 1 To LG * 2
        If i = 1 Then
            ma_fRYP(i) = ma_fAJ4(5)
        Else
            ma_fRYP(i) = ma_fRYL(i - 1)
        End If
    Next i
    
    
    For i = 1 To LG
        RVN(i, nCtyFileLine) = ma_fRYP(i * 2) + ma_fRYP(i * 2 - 1)
    Next i
    
    
    AddToReport "Royalty Paid|" & sCur, ma_fRYP()
    AddToReport "", ma_fBlank()
    
End Sub
Public Sub CalcPRTInterest(ByVal nCtyFileLine As Integer)
    Dim i As Integer
    
    CalcPRTPayback
    CalcSafeguardPRT
    CalcProfitBeforeOilAllow
    DeferCapexInSGD
    CalcOilAllowance
    MainStreamPRTCalc nCtyFileLine
    CalcCarrybackPRTLosses nCtyFileLine
    CalcIntOnPRTRefund
    
    For i = 1 To LG
        RVN(i, nCtyFileLine) = ma_fTOTPRTPAIDEXCINT(i * 2) + ma_fTOTPRTPAIDEXCINT(i * 2 - 1)
    Next i
    
    
End Sub

Public Sub CalcPRTPaid(ByVal nCtyFileLine As Integer)
    Dim i As Integer
    
    For i = 1 To LG
        RVN(i, nCtyFileLine) = ma_fTOTPRTPAID(i * 2) + ma_fTOTPRTPAID(i * 2 - 1)
    Next i
End Sub

'
' Modifications:
' 17 May 2005 JWD
'  -> Replace use of symbols mc_nAB1 and mc_nAB2 with
'     global symbols that represent the same thing.
'     Index values of the position of the codes in the
'     category string have changed with the addition of
'     new balance categories. (C0878)
'
Private Sub CalcPRTPayback()
    Dim i As Integer
    Dim bPaybackfound As Boolean
    Dim fTempSafeguard As Single
    Dim nFirstProduction As Integer
    Dim nPreProduction As Integer
    Dim bFoundProduction As Boolean
    
    Dim a_fPBKINC() As Single
    Dim a_fPBKOPX() As Single
    Dim a_fPBKCPXUPL() As Single
    Dim a_fPBKCPX() As Single
    Dim a_fPBKPFT() As Single
    Dim a_fCUMPBKPFT() As Single
    Dim a_fSafeguard() As Single
    Dim a_fPayback() As Single
    
    
    ReDim a_fPBKINC(1 To LG * 2)
    ReDim a_fPBKOPX(1 To LG * 2)
    ReDim a_fPBKCPXUPL(1 To LG * 2)
    ReDim a_fPBKPFT(1 To LG * 2)
    ReDim a_fCUMPBKPFT(1 To LG * 2)
    ReDim a_fSafeguard(1 To LG * 2)
    ReDim a_fPayback(1 To LG * 2)
    
    
    ' 17 May 2005 JWD (C0878) Change argument symbols for AB1, AB2
    GetCapex a_fPBKCPX(), mc_nASC, mc_nFCL, mc_nPLF, mc_nDVP, mc_nDNP, mc_nTRN, mc_nABN, CPXCategoryCode_AbandonmentCashExpenditure, CPXCategoryCode_AbandonmentAccrualEntry
    ' was:
    'GetCapex a_fPBKCPX(), mc_nASC, mc_nFCL, mc_nPLF, mc_nDVP, mc_nDNP, mc_nTRN, mc_nABN, mc_nAB1, mc_nAB2 ' note add abandonment
    ' End (C0878)
    
    m_nUplift = LG * 2
    For i = 1 To LG * 2
        a_fPBKCPXUPL(i) = a_fPBKCPX(i) * 1.35
        a_fPBKINC(i) = ma_fPRDREV(i) + a_udtSemiAnnualInput(16).a_fData(i) + a_udtSemiAnnualInput(17).a_fData(i)
        a_fPBKOPX(i) = a_udtSemiAnnualInput(11).a_fData(i) _
                     + a_udtSemiAnnualInput(12).a_fData(i)
        If i = 1 Then
            a_fPBKPFT(i) = a_fPBKINC(i) - (ma_fRYL(i) + a_fPBKOPX(i) + a_fPBKCPXUPL(i)) + ma_fAJ5(5)
        Else
            a_fPBKPFT(i) = a_fPBKINC(i) - (ma_fRYL(i) + a_fPBKOPX(i) + a_fPBKCPXUPL(i))
        End If
        
        If i > 1 Then
            a_fCUMPBKPFT(i) = a_fCUMPBKPFT(i - 1) + a_fPBKPFT(i)
        Else
            a_fCUMPBKPFT(i) = a_fPBKPFT(i)
        End If
        
        If a_fCUMPBKPFT(i) > 0 And Not bPaybackfound Then
            bPaybackfound = True
            m_nUplift = i
            
        End If
        
        If ma_fPRDREV(i) > 0 And Not bFoundProduction Then
            bFoundProduction = True
            nFirstProduction = i
        End If
        
    Next i
    ' GDP / JA 22/01/2001 - Temp adjustment to allow for AJ5(3) historic periods
'    If nFirstProduction - 1 > 1 Then
        nPreProduction = nFirstProduction - 1
'    Else
'        nPreProduction = nFirstProduction
'    End If
    
    fTempSafeguard = ((m_nUplift - nPreProduction) + ma_fAJ5(3)) * 1.5
    If (fTempSafeguard) - Int(fTempSafeguard) > 0 Then
        m_nSafeguard = Int(fTempSafeguard) + 1 + nPreProduction
    Else
        m_nSafeguard = fTempSafeguard + nPreProduction
    End If
    ' GDP / JA 22/01/2002 - Temp adjustment to allow for AJ5(3) historic periods
    m_nSafeguard = m_nSafeguard - ma_fAJ5(3)
    For i = 1 To LG * 2
        If ma_fPRDREV(i) > 0 And i <= m_nUplift Then
            a_fPayback(i) = 1
        Else
            a_fPayback(i) = 0
        End If
        If ma_fPRDREV(i) > 0 And i <= m_nSafeguard Then
            a_fSafeguard(i) = 1
        Else
            a_fSafeguard(i) = 0
        End If
    Next i
    AddToReport "SAFEGUARD CALCS|Payback Periods|years", a_fPayback()
    AddToReport "Safeguard Periods|years", a_fSafeguard()
    AddToReport "*Payback Income|" & sCur, a_fPBKINC()
    AddToReport "*Payback Opex|" & sCur, a_fPBKOPX()
    AddToReport "*Payback Capex with uplift|" & sCur, a_fPBKCPXUPL()
    AddToReport "*Payback Profit|" & sCur, a_fPBKPFT()
    AddToReport "*Cumulative Payback Profit|" & sCur, a_fCUMPBKPFT()
    
End Sub

'
' Modifications:
' 17 May 2005 JWD
'  -> Replace use of symbols mc_nAB1 and mc_nAB2 with
'     global symbols that represent the same thing.
'     Index values of the position of the codes in the
'     category string have changed with the addition of
'     new balance categories. (C0878)
'
Private Sub CalcSafeguardPRT()
    
    Dim a_fAJ1() As Single
    Dim a_fOPX() As Single
    
    
    Dim i As Integer
    
    
    ' 17 May 2005 JWD (C0878) Change argument symbols for AB1, AB2
    GetCapex ma_fCPXAVLUPL(), mc_nASC, mc_nFCL, mc_nPLF, mc_nDVP, mc_nDNP, mc_nTRN, mc_nABN, CPXCategoryCode_AbandonmentCashExpenditure, CPXCategoryCode_AbandonmentAccrualEntry
    ' was:
    'GetCapex ma_fCPXAVLUPL(), mc_nASC, mc_nFCL, mc_nPLF, mc_nDVP, mc_nDNP, mc_nTRN, mc_nABN, mc_nAB1, mc_nAB2
    ' End (C0878)
    
    ma_fCPXAVLUPL(1) = ma_fCPXAVLUPL(1) + ma_fAJ5(6)
    
    'AddToReport "Capex Avl for Upl|" & sCur, ma_fCPXAVLUPL()
    
    
    ReDim ma_fUPLCPX(1 To LG * 2)
    ReDim ma_fCUMUPLCPX(1 To LG * 2)
    ReDim ma_fAJ1(1 To LG * 2)
    ReDim ma_fREVPRT(1 To LG * 2)
    ReDim ma_fADJPFT(1 To LG * 2)
    ReDim ma_fSGDPRT(1 To LG * 2)
    ReDim ma_fSGDBASE(1 To LG * 2)
    ReDim a_fOPX(1 To LG * 2)
    
    
    
    
    
    For i = 1 To LG * 2
'        If i <= m_nUplift And ma_fPRDREV(i) > 0 Then
'            ma_fUPLCPX(i) = ma_fCPXAVLUPL(i)
'        End If
        If i <= m_nUplift Then
            ma_fUPLCPX(i) = ma_fCPXAVLUPL(i)
        End If
        If i = 1 Then
            ma_fCUMUPLCPX(i) = ma_fUPLCPX(i)
        Else
            ma_fCUMUPLCPX(i) = ma_fCUMUPLCPX(i - 1) + ma_fUPLCPX(i)
        End If
        ma_fREVPRT(i) = ma_fPRDREV(i) + a_udtSemiAnnualInput(16).a_fData(i)
        a_fOPX(i) = a_udtSemiAnnualInput(11).a_fData(i) _
                  + a_udtSemiAnnualInput(12).a_fData(i)
        If i <= m_nUplift Then
            ma_fADJPFT(i) = ma_fREVPRT(i) - (ma_fRYL(i) + a_fOPX(i))
                          
        Else
            ma_fADJPFT(i) = ma_fREVPRT(i) - (ma_fRYL(i) + a_fOPX(i) + ma_fCPXAVLUPL(i))
        End If
        ma_fSGDBASE(i) = ma_fADJPFT(i) - 0.15 * ma_fCUMUPLCPX(i)
        ma_fSGDPRT(i) = Max(0, 0.8 * ma_fSGDBASE(i))
'        If 0.8 * (ma_fADJPFT(i) - 0.15 * a_fCUMUPLCPX(i)) < 0 Then
'            ma_fSGDPRT(i) = 0
'        Else
'            ma_fSGDPRT(i) = 0.8 * (ma_fADJPFT(i) - 0.15 * a_fCUMUPLCPX(i))
'        End If
        
        
    Next i
    
    AddToReport "Upliftable Capex|" & sCur, ma_fUPLCPX()
    AddToReport "Cum. Upliftable Capex|" & sCur, ma_fCUMUPLCPX()
    AddToReport "Revenue for PRT|" & sCur, ma_fREVPRT()
    AddToReport "Opex for PRT|" & sCur, a_fOPX()
    AddToReport "Adjusted Profit|" & sCur, ma_fADJPFT()
    AddToReport "Safeguard Base|" & sCur, ma_fSGDBASE()
    AddToReport "Safeguard Profit|" & sCur, ma_fSGDPRT()
    AddToReport "", ma_fBlank()
End Sub
Private Sub CalcProfitBeforeOilAllow()
    Dim i As Integer
    Dim a_fPRTCAPINCUPL() As Single
    Dim a_fMAINPRTALLOW() As Single
    
    Dim a_fTEMPREV() As Single
    
    ReDim a_fPRTCAPINCUPL(LG * 2) As Single
    ReDim a_fMAINPRTALLOW(LG * 2) As Single
    ReDim ma_fASSPRTPFT(LG * 2) As Single
    ReDim ma_fASSPFTBEFOA(LG * 2) As Single
    ReDim a_fTEMPREV(LG * 2) As Single
    ReDim ma_fCUMASSPFTBEFOA(LG * 2) As Single
    
    For i = 1 To LG * 2
        a_fPRTCAPINCUPL(i) = ma_fCPXAVLUPL(i) + ma_fUPLCPX(i) * 0.35
        a_fMAINPRTALLOW(i) = ma_fRYL(i) + a_fPRTCAPINCUPL(i) _
                           + a_udtSemiAnnualInput(11).a_fData(i) + a_udtSemiAnnualInput(12).a_fData(i) _
                           + a_udtSemiAnnualInput(18).a_fData(i)
        If i = 1 Then
            ma_fASSPRTPFT(i) = ma_fREVPRT(i) - a_fMAINPRTALLOW(i) + ma_fAJ5(4)
            
        Else
            ma_fASSPRTPFT(i) = ma_fREVPRT(i) - a_fMAINPRTALLOW(i)
            
        End If
    Next i
    
    SingleCarryForward ma_fASSPRTPFT(), ma_fASSPFTBEFOA()
    
    For i = 1 To LG * 2
        If i = 1 Then
            ma_fCUMASSPFTBEFOA(i) = ma_fASSPFTBEFOA(i)
        Else
            ma_fCUMASSPFTBEFOA(i) = ma_fCUMASSPFTBEFOA(i - 1) + ma_fASSPFTBEFOA(i)
        End If
    Next i
    
    AddToReport "MAINSTREAM PRT CALCS|PRT Cap inc Uplift|" & sCur, a_fPRTCAPINCUPL()
    AddToReport "Mainstream PRT Allowance|" & sCur, a_fMAINPRTALLOW()
    AddToReport "*Assessable PRT Profit|" & sCur, ma_fASSPRTPFT()
    AddToReport "Assessable profit bef OA|" & sCur, ma_fASSPFTBEFOA()
    AddToReport "*Cum Ass prof bef OA|" & sCur, ma_fCUMASSPFTBEFOA()
    
End Sub


Private Sub GetCapex(ByRef a_fCapex() As Single, ParamArray nCodes() As Variant)
    Dim i As Integer
    Dim j As Integer
    Dim a_fTEMP() As Single
    ReDim a_fCapex(1 To LG * 2)
    
    If Not IsEmpty(nCodes) Then
        
        For i = 0 To UBound(nCodes)
            SemiAnnualCapex nCodes(i), a_fTEMP()
            For j = 1 To LG * 2
                a_fCapex(j) = a_fCapex(j) + a_fTEMP(j)
            Next j
        Next i
    End If
End Sub

Private Sub GetAllCapex(ByRef a_fCapex() As Single)
    Dim i As Integer
    'Capital Expenditure category numbers (as seen in MY3())
    
    ReDim a_fCapex(LG * 2)
    
    For i = 1 To MY3T
        If my3(i, 3) <= (YR + LG) And _
           my3(i, 3) >= YR Then
            If my3(i, 2) <= 6 Then
                a_fCapex((my3(i, 3) - YR + 1) * 2) = a_fCapex((my3(i, 3) - YR + 1) * 2) + my3(i, 5)
            Else
                a_fCapex(((my3(i, 3) - YR + 1) * 2) - 1) = a_fCapex(((my3(i, 3) - YR + 1) * 2) - 1) + my3(i, 5)
            End If
        End If
    Next i
End Sub
Private Sub PRTRates(ByVal nCtyFileLine As Integer, ByRef a_fVarRates() As Single)
'Note:
'     RTT - total number of rates in the rate section of the country file
'     sRTV() - Array containing the rate variable names (1st column)
'     FVAR() - Fiscal variable names (3 letter codes)
    Dim i As Integer
    Dim j As Integer
    Dim nCount As Integer
    ReDim a_sRateVarTitle(1 To RTT) As String
    ReDim a_fRateData(1 To RTT, 1 To 6) As Single
    ReDim a_fVarRates(1 To LG) As Single

    For i = 1 To RTT
        If sRTV(i) = FVAR$(nCtyFileLine) Then
            nCount = nCount + 1
            a_sRateVarTitle(nCount) = sRTV(i)
            For j = 1 To 6
                a_fRateData(nCount, j) = RT(i, j)
            Next j
        End If
    Next i
    RateCalc nCtyFileLine, FVAR$(nCtyFileLine), FVAR$(nCtyFileLine), 100, a_sRateVarTitle(), a_fRateData(), nCount, 33, a_fVarRates()
End Sub

Public Function ReadSemiAnnualData() As Boolean
    Dim hFile As Integer
    Dim i As Integer
    Dim j As Integer
    Dim sCurrency As String
    
    hFile = FreeFile

    Open TempDir$ & "EXTDAT.DAT" For Input As hFile
    ReDim a_udtSemiAnnualInput(20)
    Input #hFile, sCurrency
    For i = 1 To 20
        With a_udtSemiAnnualInput(i)
            Input #hFile, .sCode
            ReDim .a_fData(1 To gc_nMAXLIFE * 2)
            For j = 1 To gc_nMAXLIFE * 2
                Input #hFile, .a_fData(j)
                If j > LG * 2 Then
                    .a_fData(j) = 0
                End If
                
            Next j
        End With
        
    Next i
    
    
End Function
Public Sub ConvertSemiAnnualData(ByVal sCurrencyCode As String)
    Dim fConvert() As Single
    Dim i As Integer, j As Integer
    
    GetCurrencyConversion "USA", sCurrencyCode, fConvert()
    
    For i = 1 To 20
        With a_udtSemiAnnualInput(i)
            Select Case .sCode
                Case "OPC", "GPC", "OP1", "OP2", _
                     "OX1", "OX2", "OX3", "OX4", "OX5", _
                     "AJ1", "AJ2", "AJ3", "AJ4", "AJ5"
                
                    For j = 1 To LG * 2
                        .a_fData(j) = .a_fData(j) * fConvert(Int((j - 1) / 2) + 1)
                    Next j
                        
            End Select
        End With
    Next i
    
    ReDim ma_fAJ4(1 To 7)
    ReDim ma_fAJ5(1 To 9)
    For i = 1 To 6
        ma_fAJ4(i) = a_udtSemiAnnualInput(19).a_fData(i * 2 - 1) + a_udtSemiAnnualInput(19).a_fData(i * 2)
    Next i
    For i = 1 To 9
        ma_fAJ5(i) = a_udtSemiAnnualInput(20).a_fData(i * 2 - 1) + a_udtSemiAnnualInput(20).a_fData(i * 2)
    Next i
    
End Sub
Sub SingleCarryForward(ByRef a_fIn() As Single, ByRef a_fOut() As Single)
    Dim i As Integer
    Dim fCarry As Single
    
    ReDim a_fOut(1 To LG * 2)
    For i = 1 To LG * 2
        If a_fIn(i) + fCarry > 0 Then
            a_fOut(i) = a_fIn(i) + fCarry
            fCarry = 0
        Else
            fCarry = fCarry + a_fIn(i)
            a_fOut(i) = 0
        End If
    Next i
    
End Sub
Sub CarryForward(a_fCeiling() As Single, a_fDeduction() As Single, a_fDeductionTaken() As Single)
     Dim i%
    Dim nMin%, nMax%
    
   
    ' Defined any temp Arrays
    Dim a_fTEMP() As Double
    Dim fExcess As Double
    Dim fExcessInt As Double
    Dim a_fDeductionCF() As Double
    Dim nDivisor As Integer
    
    
    nMax = UBound(a_fDeduction)
    
    ReDim a_fTEMP(1 To nMax)
    ReDim a_fDeductionCF(1 To nMax)
    ReDim a_fDeductionTaken(1 To nMax)
    
    fExcess = 0
    
    For i = 1 To nMax
        
        a_fTEMP(i) = a_fDeduction(i) + fExcess
        
        
        fExcess = 0
        If a_fCeiling(i) > a_fTEMP(i) Then
            fExcess = 0
        Else
            fExcess = (a_fTEMP(i) - a_fCeiling(i))
            
            
            
        End If
        a_fDeductionCF(i) = a_fTEMP(i) - fExcess
        
        
    Next i
    
    For i = 1 To nMax
        a_fDeductionTaken(i) = a_fDeductionCF(i)
    Next i
    
      
End Sub

Private Sub SemiAnnualCapex(ByVal nCode As Integer, ByRef a_fCapex() As Single)
    Dim i As Integer
    'Capital Expenditure category numbers (as seen in MY3())
    
    ReDim a_fCapex(LG * 2)
    
    For i = 1 To MY3T
        If my3(i, 1) = nCode Then
            If my3(i, 3) <= (YR + LG) And _
               my3(i, 3) >= YR Then
                If my3(i, 2) <= 6 Then
                    a_fCapex((my3(i, 3) - YR + 1) * 2) = a_fCapex((my3(i, 3) - YR + 1) * 2) + my3(i, 5)
                Else
                    a_fCapex(((my3(i, 3) - YR + 1) * 2) - 1) = a_fCapex(((my3(i, 3) - YR + 1) * 2) - 1) + my3(i, 5)
                End If
            End If
        End If
    Next i
        
End Sub

Private Sub AddToReport(ByVal sTitle As String, ByRef a_fData() As Single)
    Dim i As Integer
    m_nReportItems = m_nReportItems + 1

    ReDim Preserve a_udtSemiAnnualReport(1 To m_nReportItems)
    
    With a_udtSemiAnnualReport(m_nReportItems)
        .sCode = sTitle

        ReDim .a_fData(1 To LG * 2)
        If Len(sTitle) > 0 Then
            For i = 1 To LG * 2
                .a_fData(i) = a_fData(i)
            Next i
        End If
    End With
        
    
End Sub
Public Sub ShowReport()
    Dim i As Integer
    Dim j As Integer
    Dim sFilename As String
    Dim hFile As Integer
    
    hFile = FreeFile
    
    Open TempDir$ & "prt.prn" For Output As hFile
    If m_nReportItems > 0 Then
        For i = 1 To m_nReportItems
            With a_udtSemiAnnualReport(i)
                Print #hFile, .sCode & ","; Tab(30);
                For j = 1 To UBound(.a_fData)
                    If j = UBound(.a_fData) Then
                        Print #hFile, Format$(.a_fData(j), "0.000")
                    Else
                        Print #hFile, Format$(.a_fData(j), "0.000"); Tab;
                    End If
                Next j
            End With
        Next i
    End If
    
    Close #hFile
    'sFilename = "notepad " & TempDir$ & "prt.prn"
    'Shell sFilename
End Sub

'
' Modifications:
'
' 12 Jan 2004 JWD
'  -> Add references to CGiantReport1 object to collect
'     report data in object rather than output directly to
'     file. For consolidation engine development testing
'     purposes. (C0772)
'
' 3 Feb 2004 JWD
'  -> Remove explicit writes to report file. (C0776)
'
Public Sub GenerateReportPage()
    Dim i As Integer
    Dim j As Integer
    
    ''''Write #5, 23, YR, 0, LG * 2, m_nReportItems, "PRT REPORT", 8, 0, 0, sCur
    
    Dim oPg1 As CGiantRptPageC1
    Set oPg1 = g_oReport.NewPRTRptPage
    oPg1.SetPageHeader 23, YR, 0, LG * 2, m_nReportItems, "PRT REPORT", 8, 0, 0, sCur
    
    For i = 1 To m_nReportItems
        ''''Write #5, a_udtSemiAnnualReport(i).sCode
        oPg1.ProfileHeader(i) = a_udtSemiAnnualReport(i).sCode
    Next i
    For j = 1 To LG * 2
        For i = 1 To m_nReportItems
            
            ''''Write #5, a_udtSemiAnnualReport(i).a_fData(j)
            oPg1.ProfileElementValue(i, j) = a_udtSemiAnnualReport(i).a_fData(j)
            
        Next i
    Next j
End Sub
    
Public Function Min(ParamArray vntParams() As Variant) As Variant
    Dim vntMin As Variant
    Dim i As Integer
    
    vntMin = 3.123E+44
    For i = 0 To UBound(vntParams)
        If vntParams(i) < vntMin Then
            vntMin = vntParams(i)
        End If
    Next i
    Min = vntMin
End Function

Public Function Max(ParamArray vntParams() As Variant) As Variant
    Dim vntMax As Variant
    Dim i As Integer
    
    vntMax = -3.123E+44
    For i = 0 To UBound(vntParams)
        If vntParams(i) > vntMax Then
            vntMax = vntParams(i)
        End If
    Next i
    
    Max = vntMax
End Function

Private Sub DeferCapexInSGD()
    Dim i As Integer
    Dim a_fDEFCPX() As Single
    Dim a_fDEFCPXALLCE() As Single
    Dim a_fCUMDEFCPXALL() As Single
    Dim a_fCUMDEFCPXAVAIL() As Single
    Dim a_fDECUMCPXAVAIL() As Single
    
    ReDim a_fDEFCPX(1 To LG * 2) As Single
    ReDim a_fDEFCPXALLCE(1 To LG * 2) As Single
    ReDim a_fCUMDEFCPXALL(1 To LG * 2) As Single
    ReDim a_fCUMDEFCPXAVAIL(1 To LG * 2) As Single
    ReDim ma_fDEFCPXALLCETAKEN(1 To LG * 2) As Single
    ReDim a_fDECUMCPXAVAIL(1 To LG * 2) As Single
    
    For i = 1 To LG * 2
        If i > m_nUplift Then
            a_fDEFCPX(i) = ma_fCPXAVLUPL(i)
        End If
        If ma_fSGDBASE(i) < 0 Then
            a_fDEFCPXALLCE(i) = Min(a_fDEFCPX(i), -ma_fSGDBASE(i))
        Else
            a_fDEFCPXALLCE(i) = 0
        End If
        
        If i > 1 Then
            a_fCUMDEFCPXALL(i) = a_fDEFCPXALLCE(i) + a_fCUMDEFCPXALL(i - 1)
        Else
            a_fCUMDEFCPXALL(i) = a_fDEFCPXALLCE(i)
        End If
        If i > 13 Then
            a_fCUMDEFCPXAVAIL(i) = a_fCUMDEFCPXALL(i) - a_fCUMDEFCPXALL(i - 13)
        Else
            a_fCUMDEFCPXAVAIL(i) = a_fCUMDEFCPXALL(i)
        End If
        
        If i > 1 Then
            a_fDECUMCPXAVAIL(i) = a_fCUMDEFCPXAVAIL(i) - a_fCUMDEFCPXAVAIL(i - 1)
        Else
            a_fDECUMCPXAVAIL(i) = a_fCUMDEFCPXAVAIL(i)
        End If
        
        If i <= m_nSafeguard Then
            ma_fDEFCPXALLCETAKEN(i) = a_fDECUMCPXAVAIL(i)
        End If
    
    Next i
    
    AddToReport "*Deferable Capex|" & sCur, a_fDEFCPX()
    AddToReport "*Deferable capex allowable|" & sCur, a_fDEFCPXALLCE()
    AddToReport "*Cum Deferable capex all|" & sCur, a_fCUMDEFCPXALL()
    AddToReport "*Cum Deferable capex avail|" & sCur, a_fCUMDEFCPXAVAIL()
    AddToReport "*Deferable capex avail|" & sCur, a_fDECUMCPXAVAIL()
    AddToReport "Deferred capex allce taken|" & sCur, ma_fDEFCPXALLCETAKEN()
    
End Sub


Private Sub CalcOilAllowance()
    Dim i As Integer
    
    Dim a_fP110OAAVL() As Single
    Dim a_fAVGLIQPCE() As Single
    Dim a_fAVGGASPCE() As Single
    Dim a_fOAASLIQ() As Single
    Dim a_fOAASGAS() As Single
    Dim a_fTOTOALIQ() As Single
    Dim a_fCUMOA() As Single
    Dim a_fCUMOALIM() As Single
    Dim a_fANNOALIM() As Single
    Dim a_fPFTBEFOALIQ() As Single
    
    ReDim a_fP110OAAVL(1 To LG * 2) As Single
    ReDim a_fAVGLIQPCE(1 To LG * 2) As Single
    ReDim a_fAVGGASPCE(1 To LG * 2) As Single
    ReDim a_fOAASLIQ(1 To LG * 2) As Single
    ReDim a_fOAASGAS(1 To LG * 2) As Single
    ReDim a_fTOTOALIQ(1 To LG * 2) As Single
    ReDim a_fCUMOA(1 To LG * 2) As Single
    ReDim a_fCUMOALIM(1 To LG * 2) As Single
    ReDim a_fANNOALIM(1 To LG * 2) As Single
    ReDim ma_fOA(1 To LG * 2) As Single
    ReDim a_fPFTBEFOALIQ(1 To LG * 2) As Single
    
    For i = 1 To LG * 2
        If a_udtSemiAnnualInput(6).a_fData(i) = 0 Then
            a_fP110OAAVL(i) = Min(ma_fAJ5(1) * 1, ma_fAJ5(2) * 1)
        Else
            a_fP110OAAVL(i) = Min(ma_fAJ5(1) * a_udtSemiAnnualInput(6).a_fData(i) / 100, ma_fAJ5(2) * a_udtSemiAnnualInput(6).a_fData(i) / 100)
        End If
        
        If (a_udtSemiAnnualInput(1).a_fData(i) + a_udtSemiAnnualInput(3).a_fData(i)) > 0 Then
            a_fAVGLIQPCE(i) = ((a_udtSemiAnnualInput(1).a_fData(i) * a_udtSemiAnnualInput(7).a_fData(i)) _
                            + (a_udtSemiAnnualInput(3).a_fData(i) * a_udtSemiAnnualInput(9).a_fData(i))) _
                            / (a_udtSemiAnnualInput(1).a_fData(i) + a_udtSemiAnnualInput(3).a_fData(i))
        End If
        If (a_udtSemiAnnualInput(2).a_fData(i) + a_udtSemiAnnualInput(4).a_fData(i)) > 0 Then
            a_fAVGGASPCE(i) = ((a_udtSemiAnnualInput(2).a_fData(i) * a_udtSemiAnnualInput(8).a_fData(i)) _
                            + (a_udtSemiAnnualInput(4).a_fData(i) * a_udtSemiAnnualInput(10).a_fData(i))) _
                            / (a_udtSemiAnnualInput(2).a_fData(i) + a_udtSemiAnnualInput(4).a_fData(i))
        End If
        If a_fAVGLIQPCE(i) = 0 Then
            If a_fAVGGASPCE(i) > 0 Then
                a_fPFTBEFOALIQ(i) = ma_fASSPFTBEFOA(i) / (a_fAVGGASPCE(i) * 5.333)
            End If
        Else
            If a_fAVGLIQPCE(i) > 0 Then
                a_fPFTBEFOALIQ(i) = ma_fASSPFTBEFOA(i) / a_fAVGLIQPCE(i)
            End If
        End If
        
        a_fOAASLIQ(i) = Min(a_fP110OAAVL(i), a_fPFTBEFOALIQ(i), a_udtSemiAnnualInput(1).a_fData(i) + a_udtSemiAnnualInput(3).a_fData(i))
        If a_fAVGLIQPCE(i) = 0 Then
            a_fOAASGAS(i) = Min(a_fP110OAAVL(i) - a_fOAASLIQ(i), a_fPFTBEFOALIQ(i) - a_fOAASLIQ(i), _
                            (a_udtSemiAnnualInput(2).a_fData(i) + a_udtSemiAnnualInput(4).a_fData(i)) / 5.333)
                            
        Else
            If a_fAVGGASPCE(i) > 0 Then
                a_fOAASGAS(i) = Min(a_fP110OAAVL(i) - a_fOAASLIQ(i), (a_fPFTBEFOALIQ(i) - a_fOAASLIQ(i)) * (a_fAVGLIQPCE(i) / (a_fAVGGASPCE(i) * 5.333)), _
                                (a_udtSemiAnnualInput(2).a_fData(i) + a_udtSemiAnnualInput(4).a_fData(i)) / 5.333)
            End If
        End If
        
        a_fTOTOALIQ(i) = a_fOAASLIQ(i) + a_fOAASGAS(i)
        
        If i > 1 Then
            a_fCUMOA(i) = a_fTOTOALIQ(i) + a_fCUMOA(i - 1)
        Else
            a_fCUMOA(i) = a_fTOTOALIQ(i)
        End If
        
        If a_udtSemiAnnualInput(6).a_fData(i) = 0 Then
            a_fCUMOALIM(i) = Min(ma_fAJ5(2) * 1, a_fCUMOA(i))
        Else
            a_fCUMOALIM(i) = Min(ma_fAJ5(2) * a_udtSemiAnnualInput(6).a_fData(i) / 100, a_fCUMOA(i))
        End If
        
        If i > 1 Then
            a_fANNOALIM(i) = a_fCUMOALIM(i) - a_fCUMOALIM(i - 1)
        Else
            a_fANNOALIM(i) = a_fCUMOALIM(i)
        End If
        If a_fTOTOALIQ(i) <> 0 Then
            ma_fOA(i) = a_fANNOALIM(i) * (a_fOAASLIQ(i) * a_fAVGLIQPCE(i) + a_fOAASGAS(i) * a_fAVGGASPCE(i) * 5.333) / a_fTOTOALIQ(i)
        End If
            
    Next i
    
    AddToReport "*OA available|MMBBL", a_fP110OAAVL()
    AddToReport "*Average Liq Price|" & sCur & "/MMBBL", a_fAVGLIQPCE()
    AddToReport "*Average Gas Price|" & sCur & "/BCF", a_fAVGGASPCE()
    AddToReport "*Prof. bef OA as Liq|MMBBL", a_fPFTBEFOALIQ()
    AddToReport "*OA taken as liq|MMBBL", a_fOAASLIQ()
    AddToReport "*OA taken as gas|BCF", a_fOAASGAS()
    AddToReport "Total OA as liq|MMBBL", a_fTOTOALIQ()
    AddToReport "*Cumulative OA as liq|MMBBL", a_fCUMOA()
    AddToReport "*Cum OA to lim|MMBBL", a_fCUMOALIM()
    AddToReport "*Ann OA to lim|MMBBL", a_fANNOALIM()
    AddToReport "OA|" & sCur, ma_fOA()
    
    
End Sub


Private Sub MainStreamPRTCalc(ByVal nCountryFileLine As Integer)
    Dim i As Integer
    Dim a_fPRTPFTAFTOA() As Single
    Dim a_fPRTPFTAFTDEFCAPALL() As Single
    Dim a_fPRTLBLBEFSGD() As Single
    
    Dim a_fTEMP() As Single
    Dim a_fRates() As Single
        
    ReDim a_fPRTPFTAFTOA(1 To LG * 2) As Single
    ReDim a_fPRTPFTAFTDEFCAPALL(1 To LG * 2) As Single
    ReDim a_fPRTLBLBEFSGD(1 To LG * 2) As Single
    ReDim ma_fPRTLIABLE(1 To LG * 2) As Single
    ReDim ma_fCUMPRTLIABLE(1 To LG * 2) As Single
    ReDim a_fTEMP(1 To LG * 2) As Single
    ReDim ma_fPRTPAID(1 To LG * 2) As Single
    
    PRTRates nCountryFileLine, a_fRates()
    
    For i = 1 To LG * 2
        a_fPRTPFTAFTOA(i) = ma_fASSPFTBEFOA(i) - ma_fOA(i)
        a_fTEMP(i) = a_fPRTPFTAFTOA(i) - ma_fDEFCPXALLCETAKEN(i)
    Next i
    SingleCarryForward a_fTEMP(), a_fPRTPFTAFTDEFCAPALL()
    
    For i = 1 To LG * 2
        a_fPRTLBLBEFSGD(i) = a_fPRTPFTAFTDEFCAPALL(i) * a_fRates((i + 1) \ 2) / 100
        If i <= m_nSafeguard Then
            ma_fPRTLIABLE(i) = Min(a_fPRTLBLBEFSGD(i), ma_fSGDPRT(i))
        Else
            ma_fPRTLIABLE(i) = a_fPRTLBLBEFSGD(i)
        End If
        
        If i > 1 Then
            ma_fCUMPRTLIABLE(i) = ma_fPRTLIABLE(i) + ma_fCUMPRTLIABLE(i - 1)
        Else
            ma_fCUMPRTLIABLE(i) = ma_fPRTLIABLE(i)
        End If
        If i = 1 Then
            ma_fPRTPAID(i) = 1.625 * ma_fAJ5(7) - 0.625 * ma_fAJ5(8)
        ElseIf i = 2 Then
            ma_fPRTPAID(i) = 1.625 * ma_fPRTLIABLE(i - 1) - 0.625 * ma_fAJ5(8)
        Else
            ma_fPRTPAID(i) = 1.625 * ma_fPRTLIABLE(i - 1) - 0.625 * ma_fPRTLIABLE(i - 2)
        End If
    Next i
    
    AddToReport "PRT Prof aft OA|" & sCur, a_fPRTPFTAFTOA()
    AddToReport "*PRT Prof aft def cap all|" & sCur, a_fPRTPFTAFTDEFCAPALL()
    AddToReport "PRT liable before sgd|" & sCur, a_fPRTLBLBEFSGD()
    AddToReport "PRT liable with sgd|" & sCur, ma_fPRTLIABLE()
    AddToReport "*Cumulative PRT liable with sgd|" & sCur, ma_fCUMPRTLIABLE()
    AddToReport "*PRT Paid|" & sCur, ma_fPRTPAID()
    
End Sub

Private Sub CalcCarrybackPRTLosses(ByVal nCountryFileLine As Integer)
    Dim i As Integer
    
    
    Dim a_fADJCUMPFTBEFOA() As Single
    Dim a_fADJPFTBEFOA() As Single
    Dim a_fADJPFTAFTOA() As Single
    Dim a_fADJPFTAFTOADEFCAP() As Single
    Dim a_fADJPRTLBLBEFSGD() As Single
    Dim a_fADJPRTLBLWTHSGD() As Single
    
    Dim a_fADJOA() As Single
    Dim a_fTEMP() As Single
    
    Dim a_fRates() As Single
    
    
    ReDim a_fADJCUMPFTBEFOA(1 To LG * 2) As Single
    ReDim a_fADJPFTBEFOA(1 To LG * 2) As Single
    ReDim a_fADJPFTAFTOA(1 To LG * 2) As Single
    ReDim a_fADJPFTAFTOADEFCAP(1 To LG * 2) As Single
    ReDim a_fADJPRTLBLBEFSGD(1 To LG * 2) As Single
    ReDim a_fADJPRTLBLWTHSGD(1 To LG * 2) As Single
    ReDim a_fADJOA(1 To LG * 2) As Single
    ReDim ma_fCUMADJPRTLBLWTHSGD(1 To LG * 2) As Single
    ReDim ma_fADJPRTPAID(1 To LG * 2) As Single
    ReDim a_fTEMP(1 To LG * 2) As Single
    
    PRTRates nCountryFileLine, a_fRates()
    
    For i = 1 To LG * 2
        a_fADJCUMPFTBEFOA(i) = Min(ma_fCUMASSPFTBEFOA(i), SumArray(ma_fASSPRTPFT()))
        
        If i > 1 Then
            a_fADJPFTBEFOA(i) = a_fADJCUMPFTBEFOA(i) - a_fADJCUMPFTBEFOA(i - 1)
        Else
            a_fADJPFTBEFOA(i) = a_fADJCUMPFTBEFOA(i)
        End If
        
        a_fADJOA(i) = Min(ma_fOA(i), a_fADJPFTBEFOA(i))
        
        a_fADJPFTAFTOA(i) = a_fADJPFTBEFOA(i) - a_fADJOA(i)
        
        a_fTEMP(i) = a_fADJPFTAFTOA(i) - ma_fDEFCPXALLCETAKEN(i)
    Next i
    
    SingleCarryForward a_fTEMP(), a_fADJPFTAFTOADEFCAP()
    
    For i = 1 To LG * 2
        a_fADJPRTLBLBEFSGD(i) = a_fADJPFTAFTOADEFCAP(i) * a_fRates((i + 1) \ 2) / 100
        If i <= m_nSafeguard Then
            a_fADJPRTLBLWTHSGD(i) = Min(a_fADJPRTLBLBEFSGD(i), ma_fSGDPRT(i))
        Else
            a_fADJPRTLBLWTHSGD(i) = a_fADJPRTLBLBEFSGD(i)
        End If
        If i > 1 Then
            ma_fCUMADJPRTLBLWTHSGD(i) = a_fADJPRTLBLWTHSGD(i) + ma_fCUMADJPRTLBLWTHSGD(i - 1)
        Else
            ma_fCUMADJPRTLBLWTHSGD(i) = a_fADJPRTLBLWTHSGD(i)
        End If
        
        If i = 2 Then
            ma_fADJPRTPAID(i) = 1.625 * a_fADJPRTLBLWTHSGD(i - 1) - 0.625 * ma_fAJ5(8)
        ElseIf i = 1 Then
            ma_fADJPRTPAID(i) = 1.625 * ma_fAJ5(7) - 0.625 * ma_fAJ5(8)
        Else
            ma_fADJPRTPAID(i) = 1.625 * a_fADJPRTLBLWTHSGD(i - 1) - 0.625 * a_fADJPRTLBLWTHSGD(i - 2)
            
        End If
        
            
    Next i
    
    AddToReport "*Cum Adj prof bef OA|" & sCur, a_fADJCUMPFTBEFOA()
    AddToReport "*Adj prof bef OA|" & sCur, a_fADJPFTBEFOA()
    AddToReport "*Adjt OA|" & sCur, a_fADJOA()
    AddToReport "*Adj PRT prof aft OA|" & sCur, a_fADJPFTAFTOA()
    AddToReport "*Adj PRT prof aft OA, def cap|" & sCur, a_fADJPFTAFTOADEFCAP()
    AddToReport "*Adj PRT liable bef SGD|" & sCur, a_fADJPRTLBLBEFSGD()
    AddToReport "*Adj PRT liable with SGD|" & sCur, a_fADJPRTLBLWTHSGD()
    AddToReport "*Cum Adj PRT liable with SGD|" & sCur, ma_fCUMADJPRTLBLWTHSGD()
    AddToReport "*Adj PRT Paid|" & sCur, ma_fADJPRTPAID()
End Sub


Private Sub CalcIntOnPRTRefund()
    Dim i As Integer
    
    Dim a_fADJPRTPAID() As Single
    Dim a_fCUMADJPRTPAID() As Single
    Dim a_fINTINPERIOD() As Single
    Dim a_fCLOSGBALINCINT() As Single
    Dim a_fCUMINTREC() As Single
    Dim a_fINTREC() As Single
    
    Dim a_fALLCAPEX() As Single
    
    ReDim a_fADJPRTPAID(1 To LG * 2) As Single
    ReDim a_fCUMADJPRTPAID(1 To LG * 2) As Single
    ReDim a_fINTINPERIOD(1 To LG * 2) As Single
    ReDim a_fCLOSGBALINCINT(1 To LG * 2) As Single
    ReDim a_fCUMINTREC(1 To LG * 2) As Single
    ReDim a_fINTREC(1 To LG * 2) As Single
    ReDim ma_fTOTPRTPAIDEXCINT(1 To LG * 2) As Single
    ReDim ma_fTOTPRTPAID(1 To LG * 2) As Single
    ReDim a_fALLCAPEX(1 To LG * 2) As Single
    
    
    GetAllCapex a_fALLCAPEX()
    
    For i = 1 To LG * 2
        a_fADJPRTPAID(i) = ma_fPRTPAID(i) - ma_fADJPRTPAID(i)
        If i = 1 Then
            a_fCUMADJPRTPAID(i) = a_fADJPRTPAID(i)
        Else
            a_fCUMADJPRTPAID(i) = a_fADJPRTPAID(i) + a_fCUMADJPRTPAID(i - 1)
        End If
        a_fINTINPERIOD(i) = (a_fCUMADJPRTPAID(i) + a_fADJPRTPAID(i) / 2) * ma_fAJ5(9) / 200
        a_fCLOSGBALINCINT(i) = a_fCUMADJPRTPAID(i) + a_fINTINPERIOD(i)
        If i = 1 Then
            a_fCUMINTREC(i) = a_fINTINPERIOD(i)
        Else
            a_fCUMINTREC(i) = a_fINTINPERIOD(i) + a_fCUMINTREC(i - 1)
        End If
        a_fINTREC(i) = Min(a_fCUMINTREC(LG * 2), 0.15 * (a_fALLCAPEX(LG * 2) + a_fALLCAPEX(LG * 2 - 1)))
        ma_fTOTPRTPAIDEXCINT(i) = ma_fPRTLIABLE(i) - a_fCUMADJPRTPAID(LG * 2)
        ma_fTOTPRTPAID(i) = ma_fPRTPAID(i) - a_fINTREC(i) - a_fCUMADJPRTPAID(LG * 2)
    Next i
    
    
    
    
    AddToReport "*Adjt to PRT Paid|" & sCur, a_fADJPRTPAID()
    AddToReport "*Cum Adjt to PRT Paid|" & sCur, a_fCUMADJPRTPAID()
    AddToReport "*Int in perd|" & sCur, a_fINTINPERIOD()
    AddToReport "*Closg bal inc int|" & sCur, a_fCLOSGBALINCINT()
    AddToReport "*Cum Int receivable|" & sCur, a_fCUMINTREC()
    AddToReport "*All Capex|" & sCur, a_fALLCAPEX()
    AddToReport "*Int Received|" & sCur, a_fINTREC()
    AddToReport "Tot PRT Paid (exclg int)|" & sCur, ma_fTOTPRTPAIDEXCINT()
    AddToReport "Tot PRT Paid|" & sCur, ma_fTOTPRTPAID()
    
End Sub


Private Function SumArray(ByRef a_fThis() As Single) As Single
    Dim i As Integer
    Dim fSum As Single
    
    
    For i = 1 To LG * 2
        fSum = fSum + a_fThis(i)
    Next i
    SumArray = fSum
    
End Function
