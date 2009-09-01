Attribute VB_Name = "CTY1000A"
' Name:        CTY1000A.BAS
' Function:    Country File Retrieval And Forecasting
'---------------------------------------------------------
' ********************************************************
' *          COPYRIGHT © 1986-2001 IHS ENERGY GROUP      *
' *                  ALL RIGHTS RESERVED                 *
' ********************************************************
' *   This program file is proprietary information of    *
' *                  IHS Energy Group.                   *
' *   Unauthorized use for any purpose is prohibited.    *
' ********************************************************
'---------------------------------------------------------
' This file is modified from CNTYFCST.BAS.
'---------------------------------------------------------
' Modifications:
' 9 Jul 1991 MKD
'        Do we need to convert forecast data for unit
'     correction?
'
' 17 Jul 1991 MKD
'        User defined codes are stored in UserCode$() -
'     nousercodes% stores how many there are (both are in
'     common CTYIN.BAS)
'
' 17 Mar 1995 JWD
'        Removed include:'INPUT5'.
'        Removed procedure CountryList().
'
' 7 Feb 1996 JWD
'        Replaced common include CTYIN.BAS with CTYIN1.BG.
'        Add explicit declaration of default storage class
'     as Single.
'        Removed declaration of bDebugging, now in common.
'        Removed set of bDebugging, now done in MAINEXEC.
'
' 14 Feb 1996 JWD
'        Replaced explicit external subroutine declaration
'     statements with include files CNTYFCST.BI and
'     CTYFCST2.BI.
'        Replaced explicit user type definitions with
'     include files.
'        Replaced explicit constant declaration of TRUE
'     and FALSE with TRUFALSE.BC
'
' 5 Mar 1996 JWD
'        Correct Read6Cty to include 4 additional Cost
'     recovery sequence fields new to Giant 6.x.
'
' 28 Oct 1996 JWD
'        Modified and renamed from CNTYFCST.BAS.
'        Changed CountryForecast().  (SCO0003)
'        Add symbol FILEVARTITLES.  (SCO0003)
'
' 31 Oct 1996 JWD
'        Correct ForecastExchange().  (SCO0007)
'        Correct CTYConvertData().  (SCO0011)
'
' 21 Nov 1996 JWD
'        Replace constant symbol CNTYFCST with MODULENAME.
'        Remove obsolete include and dynamic metacommands.
'
' 23 Mar 2001 JWD
'  -> Changed CTYConvertData(). (C0292)
'
' 26 Mar 2001 JWD
'  -> Changed CTYConvertData(). (C0294)
'
' 14 Jun 2001 JWD
'  -> Changed CTYConvertData(). (C0332)
'  -> Changed SingleValueSens(). (C0332)
'
' 5 Jul 2001 JWD
'  -> Changed Read5Cty(). (C0341)
'  -> Changed Read6Cty(). (C0341)
'
' 10 Jul 2001 JWD
'  -> Changed Read6Cty(). (C0349)
'  -> Changed CountryForecast(). (C0349)
'
' 2 Aug 2001 JWD
'  -> Changed Read6Cty(). (C0363)
'  -> Changed CountryForecast(). (C0363)
'
' 16 Aug 2001 JWD
'  -> Changed CTYConvertData(). (C0388)
'
' 17 Sep 2001 JWD
'  -> Changed CountryForecast(). (C0443)
'
' 21 Sep 2001 JWD
'  -> Changed CountryForecast(). (C0459)
'  -> Changed ForecastCTY().  (C0459)
'
' 17 Dec 2002 GDP
'  -> Changed CountryForecast().
'  -> Changed Read6Cty().
'
' 20 Jan 2003 GDP
'  -> Changed CTYConvertData()
'  -> Changed SingleValueSens()
'
' 27 May 2003 JWD
'  -> Changed CTYConvertData(). (C0700)
'
' 9 Feb 2004 JWD
'  -> Changed CountryForecast(). (C0780)
'  -> Changed CountryForecast(). (C0783)
'  -> Changed ForecastExchange(). (C0783)
'
' 12 May 2005 JWD
'  -> Changed CTYConvertData(). (C0876)
'
' 17 May 2005 JWD
'  -> Changed Read6Cty(). (C0878)
'
' 15 July 2009 AV
'  -> Changed ReadAdditionalParameters().
'---------------------------------------------------------
DefSng A-Z
   
Const MODULENAME = "CTY1000A.BAS"

      ' Name of variable titles temporary file created by
      ' CountryForecast() for use by export programs.  See
      ' CountryForecast() comments.
   Global Const FILEVARTITLES = "VARTITLS.DAT"

'---------------------------------------------------------
' global (this module) variables AND constants
   Dim arraysize%, updatea%
   '4-22-93 - added to common    DIM SHARED Maxlife%

   Const HIVALUE = -32760        'null integer field
   Const NULVALUE = -3.4E+35     'denotes a NOT ENTERED field (NOT 0!)
   Const LIFE = -999, WIN = -998, REV = -997, PAR = -996, LIF = -995
   Const PDY = -993, PDM = -992, PJY = -991, PJM = -990, DSC = -989
   Const CAL = -988, PRD = -987, ONSHORE = -994
   Const NON = -986, BEG = -985, ORI = -984, REP = -983, NOR = -982
   Const NO = -981

   Dim ANNRecs%, BNSRecs%, CLDRecs%, CLRRecs%, CMPRecs%
   Dim DEFRecs%, DPLRecs%, DPRRecs%, MRPRecs%
   Dim PDTRecs%, PRCRecs%, PRMRecs%, PRRRecs%, PRTRecs%, RTERecs%
   Dim TTLRecs%

'dimension arrays needed by data reading routines
   Dim ANN()         As ParmType      'country annual forecasts
   Dim CLD()         As CLDType       'ceiling definition
   Dim CLR()         As CLRType       'ceilings
   Dim CMP()         As CMPType
        '''   REDIM SHARED CPX(1)  AS CPXType      'capital expenditure data
   Dim FDEF()         As DEFType      'fiscal definition data
   Dim DPL()          As DPLType      'depletion
   Dim DPR()          As DPRType      'deprec/cost recovery
   Dim MSCTTL$(), MSCData!()         'Country Misc Parameters
   Dim BNS()          As BNSType      'bonuses
   Dim PDT()          As PDTType      'prepaid/deferred tax
   Dim PRC()          As PRCType      'price definition
   Dim PRR()          As PRRType      'govt participation rates
   Dim PRTX()         As PRTType      'govt participation terms
   Dim RTE()          As RTEType      'variable rates
   Dim prm()          As PRMType      '
   Dim MRP()          As MRPType      '
   Dim CNT$()                         'country file notes
   Dim TTLX()         As TTLType

   Dim EXR()          As EXBType      'exchange rate data
   Dim ETL()          As TTLType      'exchange rate titles
   Dim ETLRecs%                        '# of exchange rate titles
   Dim EXRRecs%                        '# of exchange rate records

'=========================================================

'---------------------------------------------------------
' Modifications:
' 20 Feb 1996 JWD
'        Change CGR$ to sCGR, duplicate definition (CGR()).
'        Change CPD$ to sCPD, duplicate definition (CPD()).
'        Change DL$ to sDL, duplicate definition (DL()).
'        Change DP$ to sDP, duplicate definition (DP()).
'        Change MDC$ to sMDC, duplicate definition (MDC()).
'        Change PC$ to sPCV, duplicate definition (PC()).
'        Change PD$ to sPDV, duplicate definition (PD()).
'        Change PR$ to sPRV, duplicate definition (PR()).
'        Change RT$ to sRTV, duplicate definition (RT()).
'        Change TM$ to sTMV, duplicate definition (TM()).
'
' 31 Oct 1996 JWD
'        Move block of code converting ceiling rates data
'     to follow conversion of fiscal definition.  The call
'     to procedure ConvertUserCode() assumes that the data
'     for fiscal definition has been loaded when it tries
'     to find a user variable that has been specified on
'     the ceiling rates form.  (SCO0011)
'
' 23 Mar 2001 JWD
'  -> Add two additional codes for Accrue Interest item of
'     depreciation/cost recovery form to support new
'     interest calculation option. (C0292)
'
' 26 Mar 2001 JWD
'  -> Changed codes for new recovery option. (C0294)
'
' 14 Jun 2001 JWD
'  -> Replace explicit occurrences of the detail capital
'     expenditure category code string with the public
'     symbol. (C0332)
'
' 16 Aug 2001 JWD
'  -> Add two new codes for method 3 for Accrue Interest
'     options to support monthly interest calculation.
'     (C0388)
'
' 20 Jan 2003 GDP
'  -> Added new codes for extra volume streams.
'
' 27 May 2003 JWD
'  -> Add codes for new adjustment forecast categories
'     AJ6-AJ0. (C0700)
'
' 12 May 2005 JWD
'  -> Add codes for new adjustment forecast categories
'     A11-A20. (C0876)
'
' 27 Jun 2008 JWD
'  -> Add conversion of PRTX().dpcrRate parameter.
'---------------------------------------------------------
Sub CTYConvertData()
'---------------------------------------------------------
'this sub takes info from GIANT 5.0 country data and puts it into
'  old giant format (ie MY1, MY3, EXPLOAN, AMTLOAN, etc.)
'This is done to allow old routines on execution side to remain
'  unaltered. They will be re-written in a future version.
'  Country Annual Forecast (ANNScreen) data is kept in new form
'  since the GNTFCST.BAS routines process data in the new format.
'---------------------------------------------------------
12360

'CLR ceiling rates parm1 User codes allowed???
'PRR PARM1 Govt partic rates parm1 User codes allowed???
'rte PARM1 VARIABLE rates parm1 User codes allowed???
'mrp - LOSS CARRY FORWARD (no or a number)   how do we show???

  BNT = BNSRecs%:       CLGTT = CLDRecs%:       CGRT = CLRRecs%
  CPDT = CMPRecs%:      TDT = DEFRecs%:         DLT = DPLRecs%
  DPT = DPRRecs%:       TMT = MRPRecs%:         PDTT = PDTRecs%
  PCT = PRCRecs%:       MDCT = PRMRecs%:        PRT = PRRRecs%
  PTT = PRTRecs%:       RTT = RTERecs%:         TLT = TTLRecs%


  ReDim BN$(BNT, 6)
  If CLGTT > 0 Then ReDim CLG$(CLGTT, 10)
  If CGRT > 0 Then ReDim sCGR(CGRT), CGR(CGRT, 6)
  If CPDT > 0 Then ReDim sCPD(CPDT), CPD(CPDT, 3)
  If TDT > 0 Then ReDim TD$(TDT, 18)
  If DLT > 0 Then ReDim sDL(DLT), DL(DLT, 8)
  If DPT > 0 Then ReDim sDP(DPT), dp(DPT, 11)
  If TMT > 0 Then ReDim sTMV(TMT), TM(TMT, 3)
  ReDim PNC$(4)
  ReDim GM(4, 2)
  If PDTT > 0 Then ReDim sPDV(PDTT), PD(PDTT, 5)
  If PCT > 0 Then ReDim sPCV(PCT), PC(PCT, 5)
  If MDCT > 0 Then ReDim sMDC(MDCT), MDC(MDCT, 7)
  If PRT > 0 Then ReDim sPRV(PRT), PR(PRT, 6)
  If PTT > 0 Then ReDim PT(PTT, 9)
  If RTT > 0 Then ReDim sRTV(RTT), RT(RTT, 6)
  If TLT > 0 Then ReDim TL$(TLT, 3)

'--------------------------------------------------------------------
24050
'bonuses
  For i% = 1 To BNSRecs%
    BN$(i%, 1) = BNS(i%).cat
    BN$(i%, 2) = BNS(i%).fld
    BN$(i%, 3) = LTrim$(str$(BNS(i%).bnsamt))
    BN$(i%, 4) = LTrim$(str$(BNS(i%).wat))      'water depth
    If BN$(i%, 4) = "-994" Or Left$(BN$(i%, 4), 2) = "ON" Then
      BN$(i%, 4) = "ON"
    End If
    BN$(i%, 5) = BNS(i%).prm
    BN$(i%, 6) = LTrim$(str$(BNS(i%).amt))
  Next i%
24055
'ceiling definitions
  For i% = 1 To CLDRecs%
    CLG$(i%, 1) = CLD(i%).var
    CLG$(i%, 2) = CLD(i%).shr
    CLG$(i%, 3) = CLD(i%).in1
    CLG$(i%, 4) = CLD(i%).in2
    CLG$(i%, 5) = CLD(i%).price
    CLG$(i%, 6) = CLD(i%).de1
    CLG$(i%, 7) = CLD(i%).de2
    CLG$(i%, 8) = CLD(i%).de3
    CLG$(i%, 9) = CLD(i%).de4
    CLG$(i%, 10) = CLD(i%).de5
  Next i%
24060
'CMP   compound factors
  For i% = 1 To CMPRecs%
    sCPD(i%) = CMP(i%).var
    
    '<<<<<< 14 Jun 2001 JWD
    C$ = "CPXEXPDEV" & CPXCategoryCodesString
    '~~~~~~ was:
    'C$ = "CPXEXPDEVBNSLSERENGOEEDHEDSADHASCDNPDVPPLFFCLTRNEORCP1CP2CP3BALBL2BL3"                'codes 250-270
    '>>>>>> End 14 Jun 2001
    
    arg$ = CMP(i%).cat
    SearchCodeString C$, arg$, 3, ptr%
    CPD(i%, 1) = ptr%
    CPD(i%, 2) = CMP(i%).break
    CPD(i%, 3) = CMP(i%).RATE
  Next i%

'fiscal definition
  For i% = 1 To DEFRecs%
    TD$(i%, 1) = FDEF(i%).var
    TD$(i%, 2) = FDEF(i%).fld
    TD$(i%, 3) = FDEF(i%).str
    TD$(i%, 4) = FDEF(i%).csh
    TD$(i%, 5) = FDEF(i%).inc1
    TD$(i%, 6) = FDEF(i%).inc2
    TD$(i%, 7) = FDEF(i%).PRC
    TD$(i%, 8) = FDEF(i%).ded1
    TD$(i%, 9) = FDEF(i%).ded2
    TD$(i%, 10) = FDEF(i%).ded3
    TD$(i%, 11) = FDEF(i%).ded4
    TD$(i%, 12) = FDEF(i%).ded5
    TD$(i%, 13) = FDEF(i%).crd1
    TD$(i%, 14) = FDEF(i%).crd2
    TD$(i%, 15) = FDEF(i%).cal1
    TD$(i%, 16) = FDEF(i%).cal2
    TD$(i%, 17) = FDEF(i%).fnc
    TD$(i%, 18) = FDEF(i%).cde
        'OXY - check for presence of the following items (with no cashflow effect)
    If PUST$ <> "Y" Then
      PUST$ = "N"
    End If
    If PUSB$ <> "Y" Then
      PUSB$ = "N"
    End If
    If PNIA$ <> "Y" Then
      PNIA$ = "N"
    End If
    If PNIB$ <> "Y" Then
      PNIB$ = "N"
    End If
    If TD$(i%, 1) = "UST" Then      'AND LTRIM$(TD$(i%, 4)) = "" THEN
      PUST$ = "Y"
    ElseIf TD$(i%, 1) = "USB" Then  'AND LTRIM$(TD$(i%, 4)) = "" THEN
      PUSB$ = "Y"
    ElseIf TD$(i%, 1) = "NIB" Then  'AND LTRIM$(TD$(i%, 4)) = "" THEN
      PNIB$ = "Y"
    ElseIf TD$(i%, 1) = "NIA" Then  'AND LTRIM$(TD$(i%, 4)) = "" THEN
      PNIA$ = "Y"
    End If
  Next i%
  
      '<<<<<<<<<<<<<<<<<
      ' 31 Oct 1996 JWD Moved to here from line 24060
  For i% = 1 To CLRRecs%
    sCGR(i%) = CLR(i%).var
    CGR(i%, 1) = CLR(i%).rat
    C$ = "ALLOILGAS"
    arg$ = CLR(i%).typ
    SearchCodeString C$, arg$, 3, ptr%
    CGR(i%, 2) = ptr%
    CGR(i%, 3) = StringToReal!(CLR(i%).wat)
    ' GDP 20 Jan 2003
    ' Added new codes for extra volume streams
    'C$ = "OILGASOV1OV2PRDOPCGPCOP1OP2AJ1AJ2AJ3AJ4AJ5PRCPR1PR2PR3PR4PR5"
    'C$ = C$ + "OT1OT2OT3OT4OT5OLCGSCV1CV2CCUMYRSDTECALCID"        'codes 1168 - 1203
    ' 27 May 2003 JWD (C0700) Add AJ6-AJ0
    ' IMPORTANT NOTE!: Changes to these strings require changes to
    ' Util5a.bas code position symbols (gc_nRtPrmXXX)
    'C$ = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0PRDOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0PRCPR1PR2PR3PR4PR5"
    ' 12 May 2005 JWD (C0876) Add A11-A20
    C$ = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0PRDOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0A11A12A13A14A15A16A17A18A19A20PRCPR1PR2PR3PR4PR5"
    C$ = C$ + "OT1OT2OT3OT4OT5OLCGSCV1CV2CV3CV4CV5CV6CV7CV8CV9CV0CCUMYRSDTECALCID"        'codes 1168 - 1203
    arg$ = CLR(i%).prm
    SearchCodeString C$, arg$, 3, ptr%
    If ptr% = 0 Then    'code is user defined
       ConvertUserCode arg$, L%
       If L% <> 0 Then
          CGR(i%, 4) = L%
       Else
          CGR(i%, 4) = 0
       End If
    Else
       CGR(i%, 4) = ptr%
    End If
    CGR(i%, 5) = CLR(i%).amt
    If CLR(i%).recu = "" Then
       CGR(i%, 6) = 0
    Else
       C$ = "YNS"        'YES/NO/SPC               'codes 1100 - 1102
       arg$ = Left$(CLR(i%).recu, 1)
       SearchCodeString C$, arg$, 1, ptr%
       CGR(i%, 6) = ptr%
    End If
  Next i%
      '>>>>>>>>>>>>>>>>>

'depletion
  For i% = 1 To DPLRecs%
    sDL(i%) = DPL(i%).var
    DL(i%, 1) = DPL(i%).rat1
    ' GDP 20 Jan 2003
    ' Added new codes for extra volumes
    ' C$ = "INCILDPRDOILGASOV1OV2"
    C$ = "INCILDPRDOILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0PRV"
    arg$ = DPL(i%).prm1
    SearchCodeString C$, arg$, 3, ptr%
    DL(i%, 2) = ptr%
    C$ = "<>"
    arg$ = LTrim$(RTrim$(DPL(i%).code))
    SearchCodeString C$, arg$, 1, ptr%
    DL(i%, 3) = ptr%
    DL(i%, 4) = DPL(i%).rat2
    ' GDP 20 Jan 2003
    ' Added new codes for extra volumes
    'C$ = "INCILDPRDOILGASOV1OV2"
    C$ = "INCILDPRDOILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0PRV"
    arg$ = DPL(i%).prm2
    SearchCodeString C$, arg$, 3, ptr%
    DL(i%, 5) = ptr%
    DL(i%, 6) = DPL(i%).base
    DL(i%, 7) = DPL(i%).start
    DL(i%, 8) = DPL(i%).per
  Next i%

'depreciation/cost recovery
  For i% = 1 To DPRRecs%
    sDP(i%) = DPR(i%).var
    
    '<<<<<< 14 Jun 2001 JWD
    C$ = "CPXEXPDEV" & CPXCategoryCodesString
    '~~~~~~ was:
    'C$ = "CPXEXPDEVBNSLSERENGEOEDHEDSADHASCDNPDVP"
    'C$ = C$ + "PLFFCLTRNEORCP1CP2CP3BALBL2BL3"                'codes 250-270
    '>>>>>> End 14 Jun 2001
    
    arg$ = DPR(i%).cat
    SearchCodeString C$, arg$, 3, ptr%
    dp(i%, 1) = ptr%
    C$ = "ALLPREPST"
    arg$ = DPR(i%).pre
    SearchCodeString C$, arg$, 3, ptr%
    dp(i%, 2) = ptr%
    C$ = "ALLTANINT"
    arg$ = DPR(i%).tan
    SearchCodeString C$, arg$, 3, ptr%
    dp(i%, 3) = ptr%
    dp(i%, 4) = DPR(i%).DPR
    C$ = "DBLSLNUOPSYDXPSNONDP1DP2DP3CUMDB1SL1SY1"
    arg$ = DPR(i%).mtd
    SearchCodeString C$, arg$, 3, ptr%
    dp(i%, 5) = ptr%
    dp(i%, 6) = DPR(i%).dbr
    dp(i%, 7) = DPR(i%).PRD
    dp(i%, 8) = DPR(i%).all
                '    IF DP(i%, 8) = NULVALUE THEN DP(i%, 8) = 0
    dp(i%, 9) = DPR(i%).crd
                '    IF DP(i%, 9) = NULVALUE THEN DP(i%, 9) = 0
    dp(i%, 10) = DPR(i%).int
                '    IF DP(i%, 10) = NULVALUE THEN DP(i%, 10) = 0
    
    '<<<<<< 16 Aug 2001 JWD (C0388)
    C$ = "NO YESNO2YS2NO3YS3"
    '~~~~~~ was:
    'C$ = "NO YESNO2YS2"
    '>>>>>> End (C0388)
    
    C$ = "NO YESNO2YS2"
    arg$ = Left$(DPR(i%).acc & "   ", 3)
    SearchCodeString C$, arg$, 3, ptr%
    dp(i%, 11) = ptr%
  Next i%

'misc rate parameters
  For i% = 1 To MRPRecs%
    sTMV(i%) = MRP(i%).var
    TM(i%, 1) = MRP(i%).loss
    C$ = "NY"           'NO/YES
    arg$ = Left$(MRP(i%).Inflate, 1)
    SearchCodeString C$, arg$, 1, ptr%
    TM(i%, 2) = ptr%
    C$ = "NY"           'NO/YES
    arg$ = Left$(MRP(i%).inflateIRR, 1)
    SearchCodeString C$, arg$, 1, ptr%
    TM(i%, 3) = ptr%
  Next i%

'country miscellaneous parameters
  For i% = 1 To 4
    PNC$(i%) = MSCTTL$(i%)
  Next i%
  GM(1, 1) = MSCData!(1)
  GM(1, 2) = MSCData!(2)
  GM(2, 1) = MSCData!(3)
  GM(2, 2) = MSCData!(4)
  GM(3, 1) = MSCData!(5)
  GM(3, 2) = MSCData!(6)
  GM(4, 1) = MSCData!(7)
  GM(4, 2) = MSCData!(8)

'prepaid/deferred tax
  For i% = 1 To PDTRecs%
    sPDV(i%) = PDT(i%).var
    PD(i%, 1) = PDT(i%).pre
    PD(i%, 2) = PDT(i%).year
    C$ = "YN"                   'YES/NO
    arg$ = Left$(PDT(i%).CUR, 1)
    SearchCodeString C$, arg$, 1, ptr%
    PD(i%, 3) = ptr%
    PD(i%, 4) = PDT(i%).def1
    PD(i%, 5) = PDT(i%).def2
  Next i%

'price definition
  For i% = 1 To PRCRecs%
    sPCV(i%) = PRC(i%).var
    ' GDP 20 Jan 2003
    ' Added code for extra volumes
    ' C$ = "OPCGPCOP1OP2PR1PR2PR3PR4PR5"
    C$ = "OPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0PR1PR2PR3PR4PR5"
    arg$ = PRC(i%).prc1
    SearchCodeString C$, arg$, 3, ptr%
                '    IF ptr% = 9 THEN
                '      PC(i%, 1) = 0
                '    ELSE
                '      PC(i%, 1) = ptr%
                '    END IF
    PC(i%, 1) = ptr%
    C$ = "<>+-*/"
    arg$ = LTrim$(RTrim$(PRC(i%).code))
    SearchCodeString C$, arg$, 1, ptr%
    PC(i%, 2) = ptr%
    ' 20 Jan 2003
    ' Added codes for extra volumes
    'C$ = "OPCGPCOP1OP2PR1PR2PR3PR4PR5"
    C$ = "OPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0PR1PR2PR3PR4PR5"
    arg$ = PRC(i%).prc2
    SearchCodeString C$, arg$, 3, ptr%
    PC(i%, 3) = ptr%
    PC(i%, 4) = PRC(i%).base
    PC(i%, 5) = PRC(i%).inc
  Next i%

'misc deprec/cost recovery parameters
  For i% = 1 To PRMRecs%
    sMDC(i%) = prm(i%).var
    
    '<<<<<< 14 Jun 2001 JWD
    C$ = "CPXEXPDEV" & CPXCategoryCodesString
    '~~~~~~ was:
    'C$ = "CPXEXPDEVBNSLSERENGEOEDHEDSADHASCDNPDVP"
    'C$ = C$ + "PLFFCLTRNEORCP1CP2CP3BALBL2BL3"                'codes 250-270
    '>>>>>> End 14 Jun 2001
    
    arg$ = prm(i%).cat
    SearchCodeString C$, arg$, 3, ptr%
    MDC(i%, 1) = ptr%
    C$ = "ALLPREPST"
    arg$ = prm(i%).prod
    SearchCodeString C$, arg$, 3, ptr%
    MDC(i%, 2) = ptr%
    C$ = "ALLTANINT"
    arg$ = prm(i%).tan
    SearchCodeString C$, arg$, 3, ptr%
    MDC(i%, 3) = ptr%
    C$ = "PRDEXP"                       '7-9-91 was: "ALLVAR"
    arg$ = prm(i%).start
    SearchCodeString C$, arg$, 3, ptr%
    MDC(i%, 4) = ptr%
    C$ = "MTHFULHLF"
    arg$ = prm(i%).year1
    SearchCodeString C$, arg$, 3, ptr%
    MDC(i%, 5) = ptr%
    C$ = "STLEND"
    arg$ = prm(i%).db
    SearchCodeString C$, arg$, 3, ptr%
    MDC(i%, 6) = ptr%
    ' GDP 20 Jan 2003
    ' Added codes for extra volumes
    'C$ = "PRDPREOILGASOV1OV2"
    C$ = "PRDPREOILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0"
    arg$ = prm(i%).uop
    SearchCodeString C$, arg$, 3, ptr%
    ' 3/26/97 - added this section to handle UOP based on any previously defined variable
    
    If ptr% = 0 Then    'code is user defined
       ConvertUserCode arg$, L%
       If L% <> 0 Then
          MDC(i%, 7) = L%
       Else
          MDC(i%, 7) = 0
       End If
    Else
       MDC(i%, 7) = ptr%
    End If
         
  Next i%

'government participation rates
  For i% = 1 To PRRRecs%
    sPRV(i%) = PRR(i%).var
    PR(i%, 1) = PRR(i%).rat
    C$ = "ALLOILGAS"
    arg$ = PRR(i%).typ
    SearchCodeString C$, arg$, 3, ptr%
    PR(i%, 2) = ptr%
    PR(i%, 3) = StringToReal!(PRR(i%).wat)
    ' GDP 23 Jan 2003
    ' Added extra codes for extra volumes/prices
    'C$ = "OILGASOV1OV2PRDOPCGPCOP1OP2AJ1AJ2AJ3AJ4AJ5PRCPR1PR2PR3PR4PR5"
    'C$ = C$ + "OT1OT2OT3OT4OT5OLCGSCV1CV2CCUMYRSDTECAL"
    ' 27 May 2003 JWD (C0700) Add AJ6-AJ0
    ' IMPORTANT NOTE!: Changes to these strings require changes to
    ' Util5a.bas code position symbols (gc_nRtPrmXXX)
    'C$ = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0PRDOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0PRCPR1PR2PR3PR4PR5"
    ' 12 May 2005 JWD (C0876) Add A11-A20
    C$ = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0PRDOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0A11A12A13A14A15A16A17A18A19A20PRCPR1PR2PR3PR4PR5"
    C$ = C$ + "OT1OT2OT3OT4OT5OLCGSCV1CV2CV3CV4CV5CV6CV7CV8CV9CV0CCUMYRSDTECAL"
    C$ = C$ + "ILDRTORT1IRR"
    
    ' Added 19 nov 2007 for production volume and cum volume
    C$ = C$ + "PRECUV"
    
    arg$ = PRR(i%).prm
    SearchCodeString C$, arg$, 3, ptr%
    If ptr% = 0 Then    'code is user defined
       ConvertUserCode arg$, L%
       If L% <> 0 Then
          PR(i%, 4) = L%
       Else
          PR(i%, 4) = 0
       End If
    Else
        PR(i%, 4) = ptr%
    End If
    PR(i%, 5) = PRR(i%).amt
    If PRR(i%).recu = "" Then
       PR(i%, 6) = 0
    Else
       C$ = "YNS"                       'codes 111-112
       arg$ = Left$(PRR(i%).recu, 1)
       SearchCodeString C$, arg$, 1, ptr%
       PR(i%, 6) = ptr%
    End If
  Next i%

'govt participation terms
  For i% = 1 To PRTRecs%
    
    '<<<<<< 14 Jun 2001 JWD
    C$ = "CPXEXPDEV" & CPXCategoryCodesString
    '~~~~~~ was:
    'C$ = "CPXEXPDEVBNSLSERENGEOEDHEDSADHASCDNPDVPPLFFCLTRNEORCP1CP2CP3BALBL2BL3"
    '>>>>>> End 14 Jun 2001
    
    arg$ = PRTX(i%).cat
    SearchCodeString C$, arg$, 3, ptr%
    PT(i%, 1) = ptr%
    PT(i%, 2) = StringToReal!(PRTX(i%).st1)
    PT(i%, 3) = PRTX(i%).RATE
    PT(i%, 4) = StringToReal!(PRTX(i%).st2)
    PT(i%, 5) = PRTX(i%).repay
    PT(i%, 6) = PRTX(i%).PRD
    PT(i%, 7) = PRTX(i%).int
    PT(i%, 8) = StringToReal!(PRTX(i%).acc)
    PT(i%, 9) = PRTX(i%).dpcrRate
  Next i%

'variable rates
  For i% = 1 To RTERecs%
    sRTV(i%) = RTE(i%).var
    RT(i%, 1) = RTE(i%).rat
    C$ = "ALLOILGAS"
    arg$ = RTE(i%).typ
    SearchCodeString C$, arg$, 3, ptr%
    RT(i%, 2) = ptr%
    RT(i%, 3) = StringToReal!(RTE(i%).wat)      'water depth
    ' GDP 20 Jan 2003
    ' Added codes for extra volumes/prices
    'C$ = "OILGASOV1OV2PRDOPCGPCOP1OP2AJ1AJ2AJ3AJ4AJ5PRCPR1PR2PR3"
    'C$ = C$ + "PR4PR5OT1OT2OT3OT4OT5OLCGSCV1CV2CCUMYRSDTECALILD"
    ' 27 May 2003 JWD (C0700) Add AJ6-AJ0
    ' IMPORTANT NOTE!: Changes to these strings require changes to
    ' Util5a.bas code position symbols (gc_nRtPrmXXX)
    'C$ = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0PRDOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0PRCPR1PR2PR3"
    ' 12 May 2005 JWD (C0876) Add A11-A20
    C$ = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0PRDOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0A11A12A13A14A15A16A17A18A19A20PRCPR1PR2PR3PR4PR5"
    C$ = C$ + "OT1OT2OT3OT4OT5OLCGSCV1CV2CV3CV4CV5CV6CV7CV8CV9CV0CCUMYRSDTECALILD"
    C$ = C$ + "RTORT1IRR"
    
    ' Added 19 nov 2007 for production volume and cum volume
    C$ = C$ + "PRECUV"
    
    arg$ = RTE(i%).prm
    SearchCodeString C$, arg$, 3, ptr%
    If ptr% = 0 Then    'code is user defined
       ConvertUserCode arg$, L%
       If L% <> 0 Then
          RT(i%, 4) = L%
       Else
          RT(i%, 4) = 0
       End If
    Else
       RT(i%, 4) = ptr%
    End If
    RT(i%, 5) = RTE(i%).amt
    If RTE(i%).recu = "" Then
       RT(i%, 6) = 0
    Else
       C$ = "YESNO SPCSP1SP2SP3SP4SP5SP6SP7SP8"
       arg$ = Left$(RTE(i%).recu, 3)
       SearchCodeString C$, arg$, 3, ptr%
       RT(i%, 6) = ptr%
    End If
  Next i%

'variable titles
  For i% = 1 To TTLRecs%
    TL$(i%, 1) = TTLX(i%).var
    TL$(i%, 2) = TTLX(i%).short
    TL$(i%, 3) = TTLX(i%).long
  Next i%

'replace any NULVALUE or HIVALUE elements (replace with 0's)
  If CLGTT > 0 Then
    ConvStringNulls2 CLG$()
  End If
  If CGRT > 0 Then
    ConvStringNulls sCGR()
    ConvRealNulls CGR()
  End If
  If CPDT > 0 Then
    ConvStringNulls sCPD()
    ConvRealNulls CPD()
  End If
  ConvRealNulls SEQ()
  If TDT > 0 Then
    ConvStringNulls2 TD$()
  End If
  If DLT > 0 Then
    ConvStringNulls sDL()
    ConvRealNulls DL()
  End If
  If DPT > 0 Then
    ConvStringNulls sDP()
    ConvRealNulls dp()
  End If
  If TMT > 0 Then
    ConvStringNulls sTMV()
    ConvRealNulls TM()
  End If
  ConvStringNulls PNC$()        'allways redimmed to (4)
  ConvRealNulls GM()            'allways redimmed to (4 x 2)
  If PDTT > 0 Then
    ConvStringNulls sPDV()
    ConvRealNulls PD()
  End If
  If PCT > 0 Then
    ConvStringNulls sPCV()
    ConvRealNulls PC()
  End If
  If MDCT > 0 Then
    ConvStringNulls sMDC()
    ConvRealNulls MDC()
  End If
  If PRT > 0 Then
    ConvRealNulls PR()
  End If
  If PTT > 0 Then
    ConvRealNulls PT()
  End If
  If RTT > 0 Then
    ConvStringNulls sRTV()
    ConvRealNulls RT()
  End If
  If TLT > 0 Then
    ConvStringNulls2 TL$()
  End If
'--------------------------------------------------------------------
'store OXY data items that go into OXY's Paradox Database
  CTitl1$ = MSCTTL$(1)
  CTitl2$ = MSCTTL$(2)
  CTitl3$ = MSCTTL$(3)
  CTitl4$ = MSCTTL$(4)
'--------------------------------------------------------------------

'ERASE new format arrays after conversion is complete
                '''ERASE ANN,
  Erase CLD, CLR, CMP, FDEF, DPL, DPR, MSCTTL$, MSCData!
  Erase BNS, PDT, PRC, PRR, PRTX, RTE, prm, MRP
        'ERASE CNT$

End Sub

Rem $SubTitle:'CTYForecastDispatch - call subs to forecast a given item'
Rem $Page
Sub CTYForecastDispatch(cat$, Datacol!(), updatea%)
'--------------------------------------------------------------------
7200
'  parameters: category$, datacol!(), UpdateA%
'  parameters in: category$
'  parameters out: category$ (unit of measure for this item), datacol!(),
'                  UpdateA%
'  function:    CALLS CTYForecastFindRecs
'               dims datarecord array
'               calls LoadFcstBaseData or LoadFcstDCL  (depending on found%)
'               calls forecastbase or forecastdcl  (depending on found%)
'               updatea% signals wether the variable was forecasted
'  calls: CTYForecastFindRecs, LoadFcstBaseData, LoadFcstDCL
'         forecastbase, forecastdcl
'  returns: datacol!(), UpdateA%
'---------------------------------------------------------
' Modifications:
' 20 Feb 1996 JWD
'           Commented out curvelife%, duplicate definition
'        and is otherwise not referenced.
'---------------------------------------------------------


'~~~~curvelife% = 0
  found% = False
  arg$ = cat$

  CTYForecastFindRecs arg$, found%         'see if the item is in BDA() or PDC()

  unit$ = arg$
  If found% Then
    updatea% = True
    ReDim ANNdata(1) As ParmType
    LoadFcstANN cat$, ANNdata()
    CTYForecastBase ANNdata(), Datacol!(), curveoffset, curvelife
    Erase ANNdata
  Else                          'NOT found
    updatea% = HIVALUE
  End If

  cat$ = unit$  'cat$ returns the unit of measure for the item

End Sub

Rem $SubTitle:'ForeCastFindRecs'
Rem $Page
Sub CTYForecastFindRecs(cat$, found%)
'--------------------------------------------------------------------
'  parameters in:  cat$ (ie. OIL, GAS, etc.)
'  parameters out: found% TRUE if in ANN, FALSE if not found]
'                  cat$ - the unit of measure of the category
'  function:       returns CAT$ as the unit of measure for the item

  found% = False
  For i% = 1 To ANNRecs%
    If ANN(i%).cat = cat$ Then
      found% = True
      unit$ = ANN(i%).unit
      Exit For
    End If
  Next i%
  cat$ = unit$

End Sub

Rem $SubTitle:'ForecastCTY - Main Forecasting for country forecast'
Rem $Page
'
' Modifications:
' 21 Sep 2001 JWD
'  -> Remove redimension of B() array. It is dimensioned
'     in the caller before passing to this procedure.
'     (C0459)
'  -> Removed redimension zb() array. It is unreferenced
'     in the project. (C0459)
Sub ForecastCTY(B(), units$(), Rc%)
'--------------------------------------------------------------------
'This sub is VERY similar to ForecastMain except that this sub
'  is called from CNTYFCST.BAS during run execution.

'  parameters: b(), units$(), rc%       'b() is the B() in common
'  parameters in: ---
'  parameters out: b(), rc%
'  function:   loops through all categories calling forecastdispatcher
'              calls CTYForecastLoadA to put contents of datacol!()
'                into b(n,n)
'                       b() is in stated units of the category
'                       units$ = unit of measure for each category


'  calls: CTYForecastDispatcher, CTYForecastLoadA
'---------------------------------------------------------
   Dim i As Integer
'---------------------------------------------------------
  AdjustLastYear% = False

        'is proddelay even used???
  proddelay = ((ProdYr% - ProjYr%) * 12) + (ProdMo% - ProjMo%)

  curvelife = 0
  arraysize% = 60
  category$ = "PR1PR2PR3PR4PR5PRTDP1DP2DP3OT1OT2OT3OT4OT5"
  nocategories% = Len(category$) \ 3     '14
  ReDim cats$(nocategories%)
  
  '<<<<<< 21 Sep 2001 JWD (C0459)
  ' Remove following. B() is dimensioned in caller,
  ' zb() is never otherwise referenced.
  'ReDim B(LG, nocategories%), zb(LG, nocategories%)
  '>>>>>> End (C0459)

  For i = 1 To nocategories%
    cats$(i) = Mid$(category$, (i - 1) * 3 + 1, 3)
  Next i
  category$ = ""

  For i = 1 To UBound(cats$)
    ReDim Datacol!(LG)   'make array exist
    updatea% = False
    arg$ = cats$(i)
    CTYForecastDispatch arg$, Datacol!(), updatea%
'//////////////////////////////////////////////////////////////////
    If AdjustLastYear% Then
      AdjustLastYear% = False
      portion = LGI + ((ProjMo% - 1) / 12)
      Port = Int(portion)
      If Port = portion Then
        portion = 1
      Else
        portion = portion - Port
      End If
      Datacol!(LG) = Datacol!(LG) * portion
    End If
'/////////////////////////////////////////////////////////////////
    units$(i) = arg$
    If updatea% <> 0 Then
      ForecastLoadA i, Datacol!(), B()
    End If
  Next i

  Erase cats$, Datacol!
  If Not updatea% Then
    Rc% = False
  End If

End Sub

'---------------------------------------------------------
' Modifications:
' 20 Feb 1996 JWD
'        Change CurrExc$ to sCurrExch, duplicate def.
'
' 31 Oct 1996 JWD
'        Comment out code writing file CHUONG4.DAT.  If
'     fiscal def contained one CUR line, would cause a
'     subscript out of range failure.  (SCO0007)
'
' 9 Feb 2004 JWD
'  -> Remove output to OXFIL.DAT. File is obsolete and not
'     used in any application system. (C0783)
'
' 16 Mar 2004 JWD
'  -> Condition Kill of FExchng$ on the variable having a
'     value (not a zero-length string).
'---------------------------------------------------------
Sub ForecastExchange()
'---------------------------------------------------------
' The user can enter an exchange rate in the currency and
' stream column of Fiscal definition screen (FDEF(x).str).
' Search FDEF() looking for exchange rate entries.  If one
' is found, look in gntconfg.dat

' First look for and count exchange lines in FDEF() & fill
' in cats$() with exchange rate codes to forecast
' (duplicates are ok - we will forecast the duplicates so
' currency.bas can find the correct rate based on every
' occurrence of CUR in fiscal def.
'---------------------------------------------------------
   Dim iDum As Integer
'---------------------------------------------------------

27950

  CurrencyRecs% = 0               'counter
  ReDim cats$(CurrencyRecs%)
  For i% = 1 To DEFRecs%
    If FDEF(i%).var = "CUR" Then
      CurrencyRecs% = CurrencyRecs% + 1
      ReDim Preserve cats$(CurrencyRecs%)
      cats$(CurrencyRecs%) = FDEF(i%).str
'      IF bDEBUGGING THEN
'        OPEN "curr.log" FOR APPEND AS #16
'          PRINT #16, "in ForecastExchange     cats$("; CurrencyRecs%; ") = "; Cats$(CurrencyRecs%)
'        CLOSE #16
'      END IF

    End If
  Next i%
'  IF bDEBUGGING THEN
'    OPEN "curr.log" FOR APPEND AS #16
'      PRINT #16, "in ForecastExchange  CurrencyRecs% = "; CurrencyRecs%
'    CLOSE #16
'  END IF

                'if CurrencyRecs%  > 0 then there are lines to forecast
  If CurrencyRecs% > 0 Then
                'dimension arrays to store data in common
    ReDim CurrExc!(LG, CurrencyRecs%), sCurrExch(4, CurrencyRecs%)
                'get exchange rate path and filename
    p% = FreeFile
    Open FConfig$ For Input As #p%
       Input #p%, x$, x$, x$, x$, x$, x$, x$, x$, iDum, ExcPath$, x$, x$, ExcNm$
    Close #p%
    FileName$ = ExcPath$ + ExcNm$ + ".EXC"
'IF bDEBUGGING THEN
'  OPEN "curr.log" FOR APPEND AS #16
'    PRINT #16, "in ForecastExchange  eschange filename = "; filename$
'  CLOSE #16
'END IF

'----------------------------------------------------------------------
'       put file name in the OXFIL file - for use by the OXY data base routines
'1-20-93
''' 9 Feb 2004 JWD (C0783) Remove output to OXFIL.DAT
'''zipfileno% = FreeFile
'''Open FOxfil$ For Append As #zipfileno%
'''   Print #zipfileno%, filename$
'''Close #zipfileno%
'----------------------------------------------------------------------

                'read the exchange rate file
    ReadExchangeFile FileName$

'IF bDEBUGGING THEN
'  OPEN "curr.log" FOR APPEND AS #16
'    PRINT #16, "in ForecastExchange  file was read  ubound(Cats$) = "; UBOUND(Cats$)
'  CLOSE #16
'END IF

                'now loop through cats$() and forecast each category
    For i% = 1 To UBound(cats$)
      ForecastCat% = True
                'see if this category has been forecasted before
      For j% = 1 To i% - 1
        If cats$(i%) = cats$(j%) Then  'a match! - copy the previous forecast
          For k% = 1 To LG
            CurrExc!(k%, i%) = CurrExc!(k%, j%)
          Next k%
          ForecastCat% = False
          Exit For       'we have the forecast - quit looking!
        End If
      Next j%
'IF bDEBUGGING THEN
'  OPEN "curr.log" FOR APPEND AS #16
'    PRINT #16, "in ForecastExchange   forecastcat% = "; ForecastCat%
'  CLOSE #16
'END IF

                'if forecastcat% is TRUE, we need to forecast the category
      If ForecastCat% Then
        SAVELG = LG
        If cats$(i%) = "USA" Then
          LG = 60
          ReDim extdatacol!(LG)
          For q% = 1 To LG
            extdatacol!(q%) = 1
          Next q%
        Else
          ReDim parmdata(1) As EXBType
          LoadFcstEXR cats$(i%), parmdata()
'IF bDEBUGGING THEN
'  OPEN "curr.log" FOR APPEND AS #16
'    PRINT #16, "   ForecastExchange   parmdata(1).cat = "; parmdata(1).Cat
'  CLOSE #16
'END IF

                  'now forecast the records for the category
          ReDim Datacol!(60)
                  'first - store LG and plug a high value for LG (exchange
                  '  rate schedules may be long since they can start at any
                  '  arbitrary time
          LG = 60
          CTYForecastBaseEXT parmdata(), Datacol!(), curveoffset, curvelife, startyear%
'IF bDEBUGGING THEN
'  OPEN "curr.log" FOR APPEND AS #16
'  FOR q% = 1 TO UBOUND(datacol!)
'    PRINT #16, "in ForecastExchange   datacol!(q%) = "; datacol!(q%)
'  NEXT q%
'  CLOSE #16
'END IF

                  'copy datacol() into extdatacol() (shifting the years)
          ReDim extdatacol!(LG)
          CTYLoadExtDataCol startyear%, Datacol!(), extdatacol!()
'IF bDEBUGGING THEN
'  OPEN "curr.log" FOR APPEND AS #16
'  FOR q% = 1 TO UBOUND(extdatacol!)
'    PRINT #16, "in ForecastExchange   extdatacol!(q%) = "; extdatacol!(q%)
'  NEXT q%
'  CLOSE #16
'END IF

        End If
                  'restore LG to actual value
        LG = SAVELG
                  'now copy ExtDataCol!() into CurrExc!()
        For j% = 1 To LG
          CurrExc!(j%, i%) = extdatacol!(j%)
'IF bDEBUGGING THEN
'  OPEN "curr.log" FOR APPEND AS #16
'    PRINT #16, "in ForecastExchange   CurrExc!("; j%; ", "; i%; ") = "; CurrExc!(j%, i%)
'  CLOSE #16
'END IF

        Next j%
      End If
    Next i%
                'now fill in sCurrExch() with the units & display (short name)
    For i% = 1 To UBound(cats$)
      sCurrExch(1, i%) = cats$(i%)    'category
      For j% = 1 To UBound(ETL)
        If cats$(i%) = ETL(j%).var Then
          sCurrExch(3, i%) = ETL(j%).short
          Exit For
        End If
      Next j%
      For j% = 1 To EXRRecs%
        If EXR(j%).cat = cats$(i%) Then
          sCurrExch(2, i%) = EXR(j%).unit
          Exit For
        End If
      Next j%
    Next i%
    Erase cats$, parmdata, Datacol!, extdatacol!
  End If
                'write data out to file (display.exe & rundispl.exe
                '  need the data on disk since they don't have
                '  a COMMON (they are RUN))
  ' 16 Mar 2004 JWD Condition on having a file spec
  If Len(FExchng$) > 0 Then
    If Len(Dir$(FExchng$)) > 0 Then
      Kill FExchng$
    End If
  End If
  If CurrencyRecs% > 0 Then
    fileno% = FreeFile
    Open FExchng$ For Output As #fileno%
       Write #fileno%, CurrencyRecs%
       For i% = 1 To CurrencyRecs%
         Write #fileno%, sCurrExch(1, i%)
         Write #fileno%, sCurrExch(2, i%)
         Write #fileno%, sCurrExch(3, i%)
       Next i%
       For i% = 1 To CurrencyRecs%
          For j% = 1 To LG
             Write #fileno%, CurrExc!(j%, i%)
          Next j%
       Next i%
    Close #fileno%

         '<<<<<<<<<<<<<<<
         ' 31 Oct 1996 JWD Comment out 4 following not used
    'flno% = FreeFile
    'Open TempDir$ + "CHUONG4.DAT" For Output As #flno%
    '   Write #flno%, sCurrExch(UBound(sCurrExch, 1), 2)      'used by MGRAPH.BAS
    'Close #flno%
         '>>>>>>>>>>>>>>>
  End If

End Sub

Rem $SubTitle:'LoadFsctANN - Load FcstData from Ann'
Rem $Page
Sub LoadFcstANN(category$, ANNdata() As ParmType)
'--------------------------------------------------------------------
'  parameters: category$, ANNdata() AS ParmType, found%
'  parameters in: category$
'  parameters out: ANNdata() AS ParmType
'  function: searches ANN() in common searching for the given category
'  returns: ANNData() with all of the recs of the given category

  Count% = 0
  For i% = 1 To ANNRecs%
    If ANN(i%).cat = category$ Then
      Count% = Count% + 1
    End If
  Next i%

  ReDim ANNdata(Count%) As ParmType

  fcstptr% = 0
  For i% = 1 To ANNRecs%
    If ANN(i%).cat = category$ Then
      fcstptr% = fcstptr% + 1
      ANNdata(fcstptr%).cat = ANN(i%).cat
      ANNdata(fcstptr%).unit = ANN(i%).unit
      ANNdata(fcstptr%).dat = ANN(i%).dat
      ANNdata(fcstptr%).mtd = ANN(i%).mtd
      ANNdata(fcstptr%).parm1 = ANN(i%).parm1
      ANNdata(fcstptr%).parm2 = ANN(i%).parm2
      ANNdata(fcstptr%).parm3 = ANN(i%).parm3
      ANNdata(fcstptr%).parm4 = ANN(i%).parm4
      ANNdata(fcstptr%).parm5 = ANN(i%).parm5
      ANNdata(fcstptr%).parm6 = ANN(i%).parm6
    End If
  Next i%

End Sub

Rem $SubTitle:'LoadFsctEXR - Load FcstData from EXR()'
Rem $Page
Sub LoadFcstEXR(category$, parmdata() As EXBType)
'--------------------------------------------------------------------
'This sub is called by ForecastExchange when user has referenced an
'  exchange rate in fiscal definition screen. This sub opens the
'  exchange rate file, reads the records and then loads the
'  records for the category into parmdata() and returns to
'  ForecastExchange.

'  parameters: category$, parmdata() AS ParmType
'  parameters in: category$
'  parameters out: parmdata() AS ParmType
'  function: searches EXR() in common searching for the given category
'  returns: parmdata() with all of the recs of the given category
9125
'IF bDEBUGGING THEN
'  OPEN "curr.log" FOR APPEND AS #16
'    PRINT #16, "   LoadFsctEXC   category$ = "; category$
'  CLOSE #16
'END IF


        'count records for given category
  Count% = 0
  For i% = 1 To EXRRecs%
    If EXR(i%).cat = category$ Then
      Count% = Count% + 1
    End If
  Next i%
        'resize parmdata (if no records match category, redim to 0 recs
  ReDim parmdata(Count%) As EXBType
        'now load parmdata() with records from EXR() that match category$
  fcstptr% = 0
  For i% = 1 To EXRRecs%
'IF bDEBUGGING THEN
'  OPEN "curr.log" FOR APPEND AS #16
'    PRINT #16, "   LoadFsctEXC   EXR("; i%; ").Cat = "; EXR(i%).Cat
'  CLOSE #16
'END IF
    If EXR(i%).cat = category$ Then
      fcstptr% = fcstptr% + 1
      parmdata(fcstptr%).cat = EXR(i%).cat
      parmdata(fcstptr%).unit = EXR(i%).unit
      parmdata(fcstptr%).dat = EXR(i%).dat
      parmdata(fcstptr%).mtd = EXR(i%).mtd
      parmdata(fcstptr%).parm1 = EXR(i%).parm1
      parmdata(fcstptr%).parm2 = EXR(i%).parm2
      parmdata(fcstptr%).parm3 = EXR(i%).parm3
      parmdata(fcstptr%).parm4 = EXR(i%).parm4
      parmdata(fcstptr%).parm5 = EXR(i%).parm5
      parmdata(fcstptr%).parm6 = EXR(i%).parm6
    End If
  Next i%
End Sub

Rem $SubTitle:'Read5Cty'
Rem $Page
'
' Modifications:
' 5 Jul 2001 JWD
'  -> Change redim of cost recovery sequence array to
'     accomodate additional capital expenditure category
'     items introduced in version. (C0341)
'  -> Add handling of balance item in read of cost
'     recovery sequence items to put in proper place in
'     array. (C0341)
'
'
' 27 Jun 2008 JWD
'  -> Add set of default PRTX().dpcrRate on load of terms definition.
'     Default is same as the RATE parameter.
'--------------------------------------------------------------------
Sub Read5Cty(filenum%, ver$)
'--------------------------------------------------------------------
'This sub reads a version 5 country file
13125
  Input #filenum%, CtyDesc$             'case description
  Input #filenum%, DEFRecs%, DPRRecs%, RTERecs%, CLDRecs%, CLRRecs%, TTLRecs%, BNSRecs%
  Input #filenum%, PRTRecs%, PRRRecs%, PRCRecs%, DPLRecs%, PDTRecs%, CMPRecs%
  If ver$ = "VERSION 5.4" Then
    Input #filenum%, PRMRecs%, MRPRecs%, CNTRecs%, ANNRecs%, RATRecs%
  Else
    Input #filenum%, PRMRecs%, MRPRecs%, CNTRecs%, ANNRecs%
  End If
13130
  Input #filenum%, nousercodes%

  ReDim FDEF(DEFRecs%) As DEFType, DPR(DPRRecs%) As DPRType
  ReDim RTE(RTERecs%) As RTEType, CLD(CLDRecs%) As CLDType
  ReDim CLR(CLRRecs%) As CLRType, TTLX(TTLRecs%) As TTLType
  ReDim BNS(BNSRecs%) As BNSType, PRTX(PRTRecs%) As PRTType
  ReDim PRR(PRRRecs%) As PRRType, PRC(PRCRecs%) As PRCType
  ReDim DPL(DPLRecs%) As DPLType, PDT(PDTRecs%) As PDTType
  ReDim CMP(CMPRecs%) As CMPType, prm(PRMRecs%) As PRMType
  ReDim MRP(MRPRecs%) As MRPType, ANN(ANNRecs%) As ParmType
  ReDim rat(RATRecs%) As RATType
  ReDim MSCTTL$(4), MSCData!(8), CNT$(CNTRecs%)
  
  '<<<<<< 5 Jul 2001 JWD (C0341)
  ReDim SEQ(Len(CPXCategoryCodesString) \ 3, 2)   'cost recovery sequence
  '~~~~~~ was:
  'ReDim SEQ(18, 2)              'cost recovery sequence
  '>>>>>> End (C0341)
  
  ReDim UserCode$(nousercodes%)
        'list of user defined codes
  For i% = 1 To nousercodes%
    Input #filenum%, UserCode$(i%)
  Next i%
13135
        'record type for DEFScreen (Fiscal Definition)
  For i% = 1 To DEFRecs%
     Input #filenum%, FDEF(i%).var, FDEF(i%).fld, FDEF(i%).str, FDEF(i%).csh, FDEF(i%).inc1
     Input #filenum%, FDEF(i%).inc2, FDEF(i%).PRC, FDEF(i%).ded1, FDEF(i%).ded2
     Input #filenum%, FDEF(i%).ded3, FDEF(i%).ded4, FDEF(i%).ded5, FDEF(i%).crd1
     Input #filenum%, FDEF(i%).crd2, FDEF(i%).cal1, FDEF(i%).cal2, FDEF(i%).fnc, FDEF(i%).cde
  Next i%
13140
'record type for DPRScreen (Depreciation/Cost Recovery)

  For i% = 1 To DPRRecs%
      Input #filenum%, DPR(i%).var, DPR(i%).cat, DPR(i%).pre, DPR(i%).tan, DPR(i%).DPR
      Input #filenum%, DPR(i%).mtd, DPR(i%).dbr, DPR(i%).PRD, DPR(i%).all, DPR(i%).crd, DPR(i%).int, DPR(i%).acc
  Next i%
'record type for RTEScreen (Variable Rates)
   For i% = 1 To RTERecs%
      Input #filenum%, RTE(i%).var, RTE(i%).rat, RTE(i%).typ, RTE(i%).wat, RTE(i%).prm, RTE(i%).amt, RTE(i%).recu
   Next i%
'record type for CLDScreen (Ceiling Definition)
13145
  For i% = 1 To CLDRecs%
    Input #filenum%, CLD(i%).var, CLD(i%).shr, CLD(i%).in1, CLD(i%).in2, CLD(i%).price
    Input #filenum%, CLD(i%).de1, CLD(i%).de2, CLD(i%).de3, CLD(i%).de4, CLD(i%).de5
  Next i%
'record type for CLRScreen (Ceilings)
    For i% = 1 To CLRRecs%
      Input #filenum%, CLR(i%).var, CLR(i%).rat, CLR(i%).typ, CLR(i%).wat, CLR(i%).prm, CLR(i%).amt, CLR(i%).recu
    Next i%
13150
'record type for TTLScreen (Variable Titles)
    For i% = 1 To TTLRecs%
     Input #filenum%, TTLX(i%).var, TTLX(i%).short, TTLX(i%).long
    Next i%
'record type for BNSScreen (Bonuses)
   For i% = 1 To BNSRecs%
     Input #filenum%, BNS(i%).cat, BNS(i%).fld, BNS(i%).bnsamt, BNS(i%).wat
     Input #filenum%, BNS(i%).prm, BNS(i%).amt
   Next i%
13155
'record type for PRTScreen (Government Participation Terms)
  For i% = 1 To PRTRecs%
     Input #filenum%, PRTX(i%).cat, PRTX(i%).st1, PRTX(i%).RATE, PRTX(i%).st2
     Input #filenum%, PRTX(i%).repay, PRTX(i%).PRD, PRTX(i%).int, PRTX(i%).acc
     PRTX(i%).dpcrRate = PRTX(i%).RATE          ' default is same as the RATE parameter
  Next i%
13160
'record type for PRRScreen (Government Participation Rates)
  For i% = 1 To PRRRecs%
    Input #filenum%, PRR(i%).var, PRR(i%).rat, PRR(i%).typ, PRR(i%).wat, PRR(i%).prm, PRR(i%).amt, PRR(i%).recu
  Next i%
'record type for PRCScreen (Price Definition)
  For i% = 1 To PRCRecs%
     Input #filenum%, PRC(i%).var, PRC(i%).prc1, PRC(i%).code, PRC(i%).prc2
     Input #filenum%, PRC(i%).base, PRC(i%).inc
  Next i%
13165
'record type for DPLScreen (Depletion)
  For i% = 1 To DPLRecs%
     Input #filenum%, DPL(i%).var, DPL(i%).rat1, DPL(i%).prm1, DPL(i%).code, DPL(i%).rat2
     Input #filenum%, DPL(i%).prm2, DPL(i%).base, DPL(i%).start, DPL(i%).per
  Next i%
'record type for PDTScreen (Prepaid/Deferred Tax)
  For i% = 1 To PDTRecs%
     Input #filenum%, PDT(i%).var, PDT(i%).pre, PDT(i%).year, PDT(i%).CUR, PDT(i%).def1, PDT(i%).def2
  Next i%
13170
' CRSscreen
   
   '<<<<<< 5 Jul 2001 JWD (C0341)
   For i% = 1 To 17
     For j% = 1 To 2
       Input #filenum%, SEQ(i%, j%)
     Next j%
   Next i%
   ' Read in the balance item, it was moved
   ' to make room for more 'other' capital
   ' categories and abandonment
   For j% = 1 To 2
      Input #filenum%, SEQ(CPXCategoryCodeBAL, j%)
   Next j%
   '~~~~~~ was:
   'For i% = 1 To 18
   '  For j% = 1 To 2
   '    Input #filenum%, SEQ(i%, j%)
   '  Next j%
   'Next i%
   '>>>>>> End (C0341)
      
'CMPscreen
   For i% = 1 To CMPRecs%
     Input #filenum%, CMP(i%).var, CMP(i%).cat, CMP(i%).break, CMP(i%).RATE
   Next i%
13175
'RECORD FOR PRMscreen
   For i% = 1 To PRMRecs%

     Input #filenum%, prm(i%).var, prm(i%).cat, prm(i%).prod, prm(i%).tan, prm(i%).start
     Input #filenum%, prm(i%).year1, prm(i%).db, prm(i%).uop
   Next i%

'RECORD for MRPScreen

   For i% = 1 To MRPRecs%
     Input #filenum%, MRP(i%).var, MRP(i%).loss, MRP(i%).Inflate, MRP(i%).inflateIRR
   Next i%
13180
'MSC screen input
   For i% = 1 To 4
     Input #filenum%, MSCTTL$(i%)
   Next i%
   For i% = 1 To 8
     Input #filenum%, MSCData!(i%)
   Next i%
'ANN Screen input
   For i% = 1 To ANNRecs%
     Input #filenum%, ANN(i%).cat, ANN(i%).unit, ANN(i%).dat, ANN(i%).mtd, ANN(i%).parm1, ANN(i%).parm2
     Input #filenum%, ANN(i%).parm3, ANN(i%).parm4, ANN(i%).parm5, ANN(i%).parm6
   Next i%
13185
        'RAT Screen input
  If ver$ = "VERSION 5.4" Then
    For i% = 1 To RATRecs%
      Input #filenum%, rat(i%).var, rat(i%).numden
      For k% = 1 To 8
        Input #filenum%, rat(i%).fnc(k%), rat(i%).cat(k%)
      Next k%
    Next i%
  End If
13190
'COUNTRY FILE NOTES
   For i% = 1 To CNTRecs%
     Input #filenum%, CNT$(i%)
   Next i%
13195
End Sub

Rem $SubTitle:'Read6Cty'
Rem $Page
Sub Read6Cty(filenum%, ver$)
'--------------------------------------------------------------------
' 5 Mar 1996 JWD
'  -> Correct redim of cost recovery sequence array SEQ().
'     Was 18 x 2 (as for v5), now 20 x 2.
'
' 5 Jul 2001 JWD
'  -> Change redim of cost recovery sequence array to
'     accomodate additional capital expenditure category
'     items introduced in version. (C0341)
'  -> Add handling of balance items in read of cost
'     recovery sequence items to put in proper place in
'     array. (C0341)
'  -> Add section to read abandonment funding provisions
'     data. (C0341)
'
' 10 Jul 2001 JWD
'  -> Condition call to read abandonment expenditure data
'     on version number. (C0349)
'
' 2 Aug 2001 JWD
'  -> Add case for version 6.1 files in cost recovery
'     sequence section. Changed number of capital
'     expenditures for version 6.2. (C0363)
'
' 17 Dec 2002 GDP
'  -> Added call to ReadExcelLinkData() after rest of Country File
'     has been read. This reads in the extra data which contains data
'     about any excel links.
'
' 17 May 2005 JWD
'  -> Add case for version 6.2 files in cost recovery
'     sequence section. Changed number of balance
'     categories in capital. These were inserted ahead of
'     the abandonment (AB1 and AB2) entries in the
'     category code string. (C0878)
'
' 27 Jun 2008 JWD
'  -> Add set of default PRTX().dpcrRate on load of terms definition.
'     Default is same as the RATE parameter.
'--------------------------------------------------------------------
'This sub reads a version 6 country file

  Input #filenum%, CtyDesc$             'case description
  Input #filenum%, DEFRecs%, DPRRecs%, RTERecs%, CLDRecs%, CLRRecs%, TTLRecs%, BNSRecs%
  Input #filenum%, PRTRecs%, PRRRecs%, PRCRecs%, DPLRecs%, PDTRecs%, CMPRecs%
  Input #filenum%, PRMRecs%, MRPRecs%, CNTRecs%, ANNRecs%, RATRecs%

  Input #filenum%, nousercodes%

  ReDim FDEF(DEFRecs%) As DEFType, DPR(DPRRecs%) As DPRType
  ReDim RTE(RTERecs%) As RTEType, CLD(CLDRecs%) As CLDType
  ReDim CLR(CLRRecs%) As CLRType, TTLX(TTLRecs%) As TTLType
  ReDim BNS(BNSRecs%) As BNSType, PRTX(PRTRecs%) As PRTType
  ReDim PRR(PRRRecs%) As PRRType, PRC(PRCRecs%) As PRCType
  ReDim DPL(DPLRecs%) As DPLType, PDT(PDTRecs%) As PDTType
  ReDim CMP(CMPRecs%) As CMPType, prm(PRMRecs%) As PRMType
  ReDim MRP(MRPRecs%) As MRPType, ANN(ANNRecs%) As ParmType
  ReDim rat(RATRecs%) As RATType
  ReDim MSCTTL$(4), MSCData!(8), CNT$(CNTRecs%)
  
  '<<<<<< 5 Jul 2001 JWD (C0341)
  ReDim SEQ(Len(CPXCategoryCodesString) \ 3, 2)   'cost recovery sequence
  '~~~~~~ was:
  'ReDim SEQ(20, 2)              'cost recovery sequence
  '>>>>>> End (C0341)
  
  ReDim UserCode$(nousercodes%)
        'list of user defined codes
  For i% = 1 To nousercodes%
    Input #filenum%, UserCode$(i%)
  Next i%

        'record type for DEFScreen (Fiscal Definition)
  For i% = 1 To DEFRecs%
     Input #filenum%, FDEF(i%).var, FDEF(i%).fld, FDEF(i%).str, FDEF(i%).csh, FDEF(i%).inc1
     Input #filenum%, FDEF(i%).inc2, FDEF(i%).PRC, FDEF(i%).ded1, FDEF(i%).ded2
     Input #filenum%, FDEF(i%).ded3, FDEF(i%).ded4, FDEF(i%).ded5, FDEF(i%).crd1
     Input #filenum%, FDEF(i%).crd2, FDEF(i%).cal1, FDEF(i%).cal2, FDEF(i%).fnc, FDEF(i%).cde
  Next i%

'record type for DPRScreen (Depreciation/Cost Recovery)
  For i% = 1 To DPRRecs%
      Input #filenum%, DPR(i%).var, DPR(i%).cat, DPR(i%).pre, DPR(i%).tan, DPR(i%).DPR
      Input #filenum%, DPR(i%).mtd, DPR(i%).dbr, DPR(i%).PRD, DPR(i%).all, DPR(i%).crd, DPR(i%).int, DPR(i%).acc
  Next i%

'record type for RTEScreen (Variable Rates)
   For i% = 1 To RTERecs%
      Input #filenum%, RTE(i%).var, RTE(i%).rat, RTE(i%).typ, RTE(i%).wat, RTE(i%).prm, RTE(i%).amt, RTE(i%).recu
   Next i%

'record type for CLDScreen (Ceiling Definition)
  For i% = 1 To CLDRecs%
    Input #filenum%, CLD(i%).var, CLD(i%).shr, CLD(i%).in1, CLD(i%).in2, CLD(i%).price
    Input #filenum%, CLD(i%).de1, CLD(i%).de2, CLD(i%).de3, CLD(i%).de4, CLD(i%).de5
  Next i%

'record type for CLRScreen (Ceilings)
    For i% = 1 To CLRRecs%
      Input #filenum%, CLR(i%).var, CLR(i%).rat, CLR(i%).typ, CLR(i%).wat, CLR(i%).prm, CLR(i%).amt, CLR(i%).recu
    Next i%

'record type for TTLScreen (Variable Titles)
    For i% = 1 To TTLRecs%
     Input #filenum%, TTLX(i%).var, TTLX(i%).short, TTLX(i%).long
    Next i%

'record type for BNSScreen (Bonuses)
   For i% = 1 To BNSRecs%
     Input #filenum%, BNS(i%).cat, BNS(i%).fld, BNS(i%).bnsamt, BNS(i%).wat
     Input #filenum%, BNS(i%).prm, BNS(i%).amt
   Next i%

    Dim sStuff As String


'record type for PRTScreen (Government Participation Terms)
  For i% = 1 To PRTRecs%
     Input #filenum%, PRTX(i%).cat, PRTX(i%).st1, PRTX(i%).RATE, PRTX(i%).st2
     Input #filenum%, PRTX(i%).repay, PRTX(i%).PRD, PRTX(i%).int, PRTX(i%).acc
     PRTX(i%).dpcrRate = PRTX(i%).RATE          ' default is same as the RATE parameter
  Next i%

'record type for PRRScreen (Government Participation Rates)
  For i% = 1 To PRRRecs%
    Input #filenum%, PRR(i%).var, PRR(i%).rat, PRR(i%).typ, PRR(i%).wat, PRR(i%).prm, PRR(i%).amt, PRR(i%).recu
  Next i%

'record type for PRCScreen (Price Definition)
  For i% = 1 To PRCRecs%
     Input #filenum%, PRC(i%).var, PRC(i%).prc1, PRC(i%).code, PRC(i%).prc2
     Input #filenum%, PRC(i%).base, PRC(i%).inc
  Next i%

'record type for DPLScreen (Depletion)
  For i% = 1 To DPLRecs%
     Input #filenum%, DPL(i%).var, DPL(i%).rat1, DPL(i%).prm1, DPL(i%).code, DPL(i%).rat2
     Input #filenum%, DPL(i%).prm2, DPL(i%).base, DPL(i%).start, DPL(i%).per
  Next i%

'record type for PDTScreen (Prepaid/Deferred Tax)
  For i% = 1 To PDTRecs%
     Input #filenum%, PDT(i%).var, PDT(i%).pre, PDT(i%).year, PDT(i%).CUR, PDT(i%).def1, PDT(i%).def2
  Next i%

' CRSscreen
      '<<<<<< 5 Jul 2001 JWD (C0341)
      If ver$ = "VERSION 6.0" Then
         For i% = 1 To 17
           For j% = 1 To 2
             Input #filenum%, SEQ(i%, j%)
           Next j%
         Next i%
         ' Read in the balance items, they were moved
         ' to make room for more 'other' capital
         ' categories and abandonment
         For i% = CPXCategoryCodeBAL To CPXCategoryCodeBL3
            For j% = 1 To 2
               Input #filenum%, SEQ(i%, j%)
            Next j%
         Next i%
      '<<<<<< 2 Aug 2001 JWD (C0363)
      ElseIf ver$ = "VERSION 6.1" Then
         For i% = 1 To CPXCategoryCodeBL3
            For j% = 1 To 2
               Input #filenum%, SEQ(i%, j%)
            Next j%
         Next i%
      '>>>>>> End (C0363)
      ' 18 May 2005 JWD (C0878)
      ElseIf ver$ = "VERSION 6.2" Then
        For i% = 1 To CPXCategoryCodeBL3
            For j% = 1 To 2
               Input #filenum%, SEQ(i%, j%)
            Next j%
        Next i%
        ' Set BL4 to BLn to same sequence value as for BAL
        ' This set of categories is not explicitly in the file
        ' so that a new file version does not have to be
        ' created (along with the issues that raises).
        For i% = CPXCategoryCodeBL3 + 1 To CPXCategoryCodeBLn
            SEQ(i%, 1) = SEQ(CPXCategoryCodeBAL, 1)
            SEQ(i%, 2) = SEQ(CPXCategoryCodeBAL, 2)
        Next i%
        ' Read in the abandonment AB1 and AB2
        ' These follow on after BL3 in 6.2
        For i% = CPXCategoryCode_AbandonmentCashExpenditure To CPXCategoryCode_AbandonmentAccrualEntry
            For j% = 1 To 2
               Input #filenum%, SEQ(i%, j%)
            Next j%
        Next i%
      ' End (C0878)
      Else
         For i% = 1 To UBound(SEQ, 1)
            For j% = 1 To 2
               Input #filenum%, SEQ(i%, j%)
            Next j%
         Next i%
      End If
      '~~~~~~ was:
      'For i% = 1 To 20
      '  For j% = 1 To 2
      '    Input #filenum%, SEQ(i%, j%)
      '  Next j%
      'Next i%
      '>>>>>> End (C0341)

'CMPscreen
   For i% = 1 To CMPRecs%
     Input #filenum%, CMP(i%).var, CMP(i%).cat, CMP(i%).break, CMP(i%).RATE
   Next i%

'RECORD FOR PRMscreen
   For i% = 1 To PRMRecs%
     Input #filenum%, prm(i%).var, prm(i%).cat, prm(i%).prod, prm(i%).tan, prm(i%).start
     Input #filenum%, prm(i%).year1, prm(i%).db, prm(i%).uop
   Next i%

'RECORD for MRPScreen
   For i% = 1 To MRPRecs%
     Input #filenum%, MRP(i%).var, MRP(i%).loss, MRP(i%).Inflate, MRP(i%).inflateIRR
   Next i%

'MSC screen input
   For i% = 1 To 4
     Input #filenum%, MSCTTL$(i%)
   Next i%
   For i% = 1 To 8
     Input #filenum%, MSCData!(i%)
   Next i%

'ANN Screen input
   For i% = 1 To ANNRecs%
     Input #filenum%, ANN(i%).cat, ANN(i%).unit, ANN(i%).dat, ANN(i%).mtd, ANN(i%).parm1, ANN(i%).parm2
     Input #filenum%, ANN(i%).parm3, ANN(i%).parm4, ANN(i%).parm5, ANN(i%).parm6
   Next i%

        'RAT Screen input
  For i% = 1 To RATRecs%
    Input #filenum%, rat(i%).var, rat(i%).numden
    For k% = 1 To 8
      Input #filenum%, rat(i%).fnc(k%), rat(i%).cat(k%)
    Next k%
  Next i%
  
  '<<<<<< 10 Jul 2001 JWD (C0349)
  If ver$ = "VERSION 6.0" Then
     ' doesn't have abandonment fund provisions section
  Else
     ReadAbandonmentFundingProvisions filenum%
  End If
  '~~~~~~ was:
  ''<<<<<< 5 Jul 2001 JWD (C0341)
  'ReadAbandonmentFundingProvisions filenum%
  ''>>>>>> End (C0341)
  '>>>>>> End (C0349)
  

'COUNTRY FILE NOTES
   For i% = 1 To CNTRecs%
     Input #filenum%, CNT$(i%)
   Next i%

   ' Begin - Added GDP 17 Dec 2002
   ReadExcelLinkData filenum
   ReadAdditionalParameters filenum
   ' End
End Sub

Private Sub ReadAdditionalParameters(ByVal hFile As Integer)
    
    Dim i As Integer
    Dim sHeader As String
    Dim sVersion As String
    Dim sRepayRate As String
    Dim bSectionExists As Boolean
    
    bSectionExists = False
    
    ' always add new parameters at the end - start a new section
    If Not EOF(hFile) Then
        Input #hFile, sHeader
        Input #hFile, sVersion
        
        If sHeader = "STARTADDITIONALPARAMSDATA" Then
            ' record extra parameters for PRTScreen (NOC Participation Terms) - NOC Depr/CostRec Rates
            ' added JK 09th October 2007
            For i = 1 To UBound(PRTX)
                Dim termDprc As String
                Input #hFile, termDprc
                If IsNumeric(termDprc) Then
                    PRTX(i).dpcrRate = Val(termDprc)
                End If
            Next i
            
            bSectionExists = True
        End If
        
        Input #hFile, sHeader
    End If
    
End Sub

Rem $SubTitle:'ReadExchangeFile'
Rem $Page
Sub ReadExchangeFile(FileName$)
'--------------------------------------------------------------------
'This sub reads the exchange rate file into EXR() and ETL() for use
'  by the exchange rate forecasting routines.


  filenum% = FreeFile
  If Len(Dir$(FileName$)) > 0 Then
    Open FileName$ For Input As filenum%
    If ErrNo% = 0 Then
      EXCFile$ = FileName$              'OXY database item
      Input #filenum%, ver$
      ver$ = RTrim$(ver$)
'IF bDEBUGGING THEN
'  OPEN "curr.log" FOR APPEND AS #16
'    PRINT #16, "       ReadExchangeFile   ver$ = "; ver$
'  CLOSE #16
'END IF

      If ver$ = "VERSION 5.0" Or ver$ = "VERSION 5.1" Or ver$ = "VERSION 5.2" Or ver$ = "VERSION 5.3" Or ver$ = "VERSION 6.0" Then
        Input #filenum%, DUM$           'case description
        EXCDesc$ = DUM$                 'OXY database item
'IF bDEBUGGING THEN
'  OPEN "curr.log" FOR APPEND AS #16
'    PRINT #16, "       ReadExchangeFile   EXRDesc$ = "; EXRDesc$
'  CLOSE #16
'END IF

        DUM$ = ""
        Input #filenum%, EXRRecs%, ETLRecs%
'IF bDEBUGGING THEN
'  OPEN "curr.log" FOR APPEND AS #16
'    PRINT #16, "       ReadExchangeFile   EXRRecs% = "; EXRRecs%; "  ETLRecs% = "; ETLRecs%
'  CLOSE #16
'END IF
        If ver$ = "VERSION 6.0" Then
          Input #filenum%, EXNRecs%
'IF bDEBUGGING THEN
'  OPEN "curr.log" FOR APPEND AS #16
'    PRINT #16, "       ReadExchangeFile   EXNRecs% = "; EXNRecs%
'  CLOSE #16
'END IF

        End If
        ReDim EXR(EXRRecs%) As EXBType, ETL(ETLRecs%) As TTLType
                'record for EXRScreen (EXTERNAL EXCHANGE RATES)
'IF bDEBUGGING THEN
'  OPEN "curr.log" FOR APPEND AS #16
'    PRINT #16, "       ReadExchangeFile   EXRRecs% = "; EXRRecs%
'  CLOSE #16
'END IF

        For i% = 1 To EXRRecs%
          Input #filenum%, EXR(i%).cat, EXR(i%).unit, EXR(i%).dat, EXR(i%).mtd, EXR(i%).parm1
          Input #filenum%, EXR(i%).parm2, EXR(i%).parm3, EXR(i%).parm4, EXR(i%).parm5, EXR(i%).parm6
'IF bDEBUGGING THEN
'  OPEN "curr.log" FOR APPEND AS #16
'    PRINT #16, "       ReadExchangeFile   EXR("; i%; ").Cat = "; EXR(i%).Cat
'  CLOSE #16
'END IF

        Next i%
        For i% = 1 To ETLRecs%   'ETL screen (titles)
          Input #filenum%, ETL(i%).var, ETL(i%).short, ETL(i%).long
        Next i%
        Close filenum%
      End If
    End If
  Else
    EXBRecs% = 0
  End If

End Sub

Rem $subtitle: 'SingleValueSens'
Rem $Page:
'
' Modifications:
' 14 Jun 2001 JWD
'  -> Replace explicit occurrences of the detail capital
'     expenditure category code string with the public
'     symbol. (C0332)
'
' 20 Jan 2003 GDP
'  -> Added new codes for extra volume streams.

Sub SingleValueSens()
'--------------------------------------------------------------------
'This sub is called from the module level and processes single value
'  sensitivity changes.
'We examing RF$(4) to determine the form, RF$(5) states the row # on
'  the form, RF$(6) is the column number on the form (excluding the
'  row number column) and RF$(7) contains the new value.

 VLF$ = Left$(RF$(4), 3)              'form name
 vlr = Val(RF$(5))                    'row number
 vlc = Val(RF$(6))                    'column number
 sVlu = LTrim$(RTrim$(RF$(7)))        'value (string)
 vlu = Val(sVlu)                      'value numeric

        'if user entered no form name or invalid row or column number - EXIT
 If Len(VLF$) = 0 Or vlr < 1 Or vlc < 1 Then
   Exit Sub
 End If

 Select Case VLF$
   Case "DEF"         'fiscal definition    FDEF()
       If vlr > DEFRecs% Then
         Exit Sub
       End If
       If vlc = 1 Then
         FDEF(vlr).var = sVlu                  'variable
       ElseIf vlc = 2 Then
         SearchCodeString "ALLOILGAS", sVlu, 3, ptr%
         If ptr% > 0 Then
           FDEF(vlr).fld = sVlu
         End If
       ElseIf vlc = 3 Then
         FDEF(vlr).str = sVlu
       ElseIf vlc = 4 Then
         If sVlu = " " Or sVlu = "" Or sVlu = "+" Or sVlu = "-" Then
           FDEF(vlr).csh = sVlu
         End If
       ElseIf vlc = 5 Then
           FDEF(vlr).inc1 = sVlu
       ElseIf vlc = 6 Then
           FDEF(vlr).inc2 = sVlu
       ElseIf vlc = 7 Then
         If sVlu = "" Then
           FDEF(vlr).PRC = sVlu
         Else
           ' GDP 20 Jan 2003
           ' Added codes for extra prices
           'SearchCodeString "OPCGPCOP1OP2PRCPR1PR2PR3PR4PR5", sVlu, 3, ptr%
           SearchCodeString "OPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0PRCPR1PR2PR3PR4PR5", sVlu, 3, ptr%
           If ptr% > 0 Then
             FDEF(vlr).str = sVlu
           End If
         End If
       ElseIf vlc = 8 Then
         FDEF(vlr).ded1 = sVlu
       ElseIf vlc = 9 Then
         FDEF(vlr).ded2 = sVlu
       ElseIf vlc = 10 Then
         FDEF(vlr).ded3 = sVlu
       ElseIf vlc = 11 Then
         FDEF(vlr).ded4 = sVlu
       ElseIf vlc = 12 Then
         FDEF(vlr).ded5 = sVlu
       ElseIf vlc = 13 Then
         FDEF(vlr).crd1 = sVlu
       ElseIf vlc = 14 Then
         FDEF(vlr).crd2 = sVlu
       ElseIf vlc = 15 Then
         FDEF(vlr).cal1 = sVlu
       ElseIf vlc = 16 Then
         FDEF(vlr).cal2 = sVlu
       ElseIf vlc = 17 Then
         If sVlu = "" Then
           FDEF(vlr).fnc = ""
         Else
           SearchCodeString "<>+-*/CT", sVlu, 1, ptr%
           If ptr% > 0 Then
             FDEF(vlr).fnc = sVlu
           End If
         End If
       ElseIf vlc = 18 Then
         If sVlu = "" Then
           FDEF(vlr).cde = ""
         Else
           SearchCodeString "FLDNOP", sVlu, 3, ptr%
           If ptr% > 0 Then
             FDEF(vlr).cde = sVlu
           End If
         End If
       End If
   Case "DPR"         'depreciation
       If vlr > DPRRecs% Then
         Exit Sub
       End If

       If vlc = 1 Then          'variable
         DPR(vlr).var = sVlu
       ElseIf vlc = 2 Then      'category
         
         '<<<<<< 14 Jun 2001 JWD
         List$ = "CPXEXPDEV" & CPXCategoryCodesString
         '~~~~~~ was:
         'List$ = "CPXEXPDEVBNSLSERENGEOEDHEDSADHASCDNPDVPPLFFCLTRNEORCP1CP2CP3BAL"
         '>>>>>> End 14 Jun 2001
         
         SearchCodeString List$, sVlu, 3, ptr%
         If ptr% > 0 Then
           DPR(vlr).cat = sVlu
         End If
       ElseIf vlc = 3 Then      'pre / post
         SearchCodeString "ALLPREPST", sVlu, 3, ptr%
         If ptr% > 0 Then
           DPR(vlr).pre = sVlu
         End If
       ElseIf vlc = 4 Then      'tangible / intangible
         SearchCodeString "ALLTANINT", sVlu, 3, ptr%
         If ptr% > 0 Then
           DPR(vlr).tan = sVlu
         End If
       ElseIf vlc = 5 Then      'deprec %
         DPR(vlr).DPR = vlu
       ElseIf vlc = 6 Then      'method
         SearchCodeString "DBLSLNUOPSYDXPSNONDP1DP2DP3CUM", sVlu, 3, ptr%
         If ptr% > 0 Then
           DPR(vlr).mtd = sVlu
         End If
       ElseIf vlc = 7 Then      'DB RATE
         DPR(vlr).dbr = vlu
       ElseIf vlc = 8 Then      'period (years)
         If sVlu = "LIF" Then
           DPR(vlr).PRD = LIF   '-995
         Else
           DPR(vlr).PRD = vlu
         End If
       ElseIf vlc = 9 Then      'year 1 allowance
         DPR(vlr).all = vlu
       ElseIf vlc = 10 Then     'credit %
         DPR(vlr).crd = vlu
       ElseIf vlc = 11 Then
         DPR(vlr).int = vlu
       ElseIf vlc = 12 Then
         If Left$(sVlu, 1) = "N" Then
           DPR(vlr).acc = "NO"
         ElseIf Left$(sVlu, 1) = "Y" Then
           DPR(vlr).acc = "YES"
         End If
       End If
   Case "PRM"         'misc depreciation / cost recovery parameters
       If vlr > PRMRecs% Then
         Exit Sub
       End If

       If vlc = 1 Then
         prm(vlr).var = sVlu
       ElseIf vlc = 2 Then
       
         '<<<<<< 14 Jun 2001 JWD
         List$ = "CPXEXPDEV" & CPXCategoryCodesString
         '~~~~~~ was:
         'List$ = "CPXEXPDEVBNSLSERENGEOEDHEDSADHASCDNPDVPPLFFCLTRNEORCP1CP2CP3BAL"
         '>>>>>> End 14 Jun 2001
         
         SearchCodeString List$, sVlu, 3, ptr%
         If ptr% > 0 Then
           prm(vlr).cat = sVlu
         End If
       ElseIf vlc = 3 Then              'pre/post prod
         SearchCodeString "ALLPREPST", sVlu, 3, ptr%
         If ptr% > 0 Then
           prm(vlr).prod = sVlu
         End If
       ElseIf vlc = 4 Then              'tangible/intangible
         SearchCodeString "ALLTANINT", sVlu, 3, ptr%
         If ptr% > 0 Then
           prm(vlr).tan = sVlu
         End If
       ElseIf vlc = 5 Then              'start
         SearchCodeString "PRDEXP", sVlu, 3, ptr%
         If ptr% > 0 Then
           prm(vlr).start = sVlu
         End If
       ElseIf vlc = 6 Then              'year 1
         SearchCodeString "MTHFULHLF", sVlu, 3, ptr%
         If ptr% > 0 Then
           prm(vlr).year1 = sVlu
         End If
       ElseIf vlc = 7 Then
         SearchCodeString "STLEND", sVlu, 3, ptr%
         If ptr% > 0 Then
           prm(vlr).db = sVlu
         End If
       ElseIf vlc = 8 Then              'unit of production
         ' GDP 20 Jan 2003
         ' Added codes for extra volumes
         'SearchCodeString "PRDPREOILGASOV1OV2", sVlu, 3, ptr%
         SearchCodeString "PRDPREOILGASOV1OV2OV3VO4OV5OV6OV7OV8OV9", sVlu, 3, ptr%
         If ptr% > 0 Then
           prm(vlr).uop = sVlu
         End If
       End If
   Case "CRS"         'cost recovery sequence

       If vlc < 3 Then          'first two columns
         SEQ(vlr, vlc) = vlu
       ElseIf vlc < 5 Then              'there are only 4 columns on CRS
         SEQ(vlr + 9, vlc - 2) = vlu
       End If
   Case "RTE"         'variable rates
       If vlr > RTERecs% Then
         Exit Sub
       End If

       If vlc = 1 Then                                  'var column
         RTE(vlr).var = sVlu
       ElseIf vlc = 2 Then                              'rate %
         RTE(vlr).rat = vlu
       ElseIf vlc = 3 Then                              'field type
         SearchCodeString "ALLOILGAS", sVlu, 3, ptr%
         If ptr% > 0 Then
           RTE(vlr).typ = sVlu
         End If
       ElseIf vlc = 4 Then                              'water depth
         RTE(vlr).wat = sVlu
       ElseIf vlc = 5 Then                              'prm 1
         RTE(vlr).prm = sVlu
       ElseIf vlc = 6 Then                              'amount
         RTE(vlr).amt = vlu
       ElseIf vlc = 7 Then                              'sliding scale
         SearchCodeString "YESNO SPCSP1SP2SP3SP4SP5SP6SP7SP8", sVlu, 3, ptr%
         If ptr% > 0 Then
           RTE(vlr).recu = sVlu
         End If
       End If
   Case "MRP"         'misc rate parameters
       If vlr > MRPRecs% Then
         Exit Sub
       End If

       If vlc = 1 Then
         MRP(vlr).var = sVlu
       ElseIf vlc = 2 Then
         If sVlu = "NO" Then
           MRP(vlr).loss = NO           '-981
         Else
           MRP(vlr).loss = vlu
         End If
       ElseIf vlc = 3 Then
         SearchCodeString "YESNO ", sVlu, 3, ptr%
         If ptr% > 0 Then
           MRP(vlr).Inflate = sVlu
         End If
       ElseIf vlc = 4 Then
         SearchCodeString "YESNO ", sVlu, 3, ptr%
         If ptr% > 0 Then
           MRP(vlr).inflateIRR = sVlu
         End If
       End If
   Case "CMP"         'compound factors
       If vlr > CMPRecs% Then
         Exit Sub
       End If

       If vlc = 1 Then
         CMP(vlr).var = sVlu
       ElseIf vlc = 2 Then
         
         '<<<<<< 14 Jun 2001 JWD
         List$ = "CPXEXPDEV" & CPXCategoryCodesString
         '~~~~~~ was:
         'List$ = "CPXEXPDEVBNSLSERENGEOEDHEDSADHASCDNPDVPPLFFCLTRNEORCP1CP2CP3BAL"
         '>>>>>> End 14 Jun 2001
         
         SearchCodeString List$, sVlu, 3, ptr%
         If ptr% > 0 Then
           CMP(vlr).cat = sVlu
         End If
       ElseIf vlc = 3 Then
         CMP(vlr).break = vlu
       ElseIf vlc = 4 Then
         CMP(vlr).RATE = vlu
       End If
   Case "CLD"         'ceiling definition
       If vlr > CLDRecs% Then
         Exit Sub
       End If

       If vlc = 1 Then
         CLD(vlr).var = sVlu                   'variable
       ElseIf vlc = 2 Then
         If sVlu = "" Then
           CLD(vlr).shr = ""
         Else
           SearchCodeString "GRSGRPGVT", sVlu, 3, ptr%
           If ptr% > 0 Then
             CLD(vlr).shr = sVlu
           End If
         End If
       ElseIf vlc = 3 Then
         CLD(vlr).in1 = sVlu
       ElseIf vlc = 4 Then
         CLD(vlr).in2 = sVlu
       ElseIf vlc = 5 Then
         If sVlu = "" Then
           CLD(vlr).price = ""
         Else
           ' GDP 20 Jan 2003
           ' Added codes for extra volume streams
           'SearchCodeString "OPCGPCOP1OP2PRCPR1PR2PR3PR4PR5", sVlu, 3, ptr%
           SearchCodeString "OPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0PRCPR1PR2PR3PR4PR5", sVlu, 3, ptr%
           If ptr% > 0 Then
             CLD(vlr).price = sVlu
           End If
         End If
       ElseIf vlc = 6 Then
         CLD(vlr).de1 = sVlu
       ElseIf vlc = 7 Then
         CLD(vlr).de2 = sVlu
       ElseIf vlc = 8 Then
         CLD(vlr).de3 = sVlu
       ElseIf vlc = 9 Then
         CLD(vlr).de4 = sVlu
       ElseIf vlc = 10 Then
         CLD(vlr).de5 = sVlu
       End If
   Case "CLR"         'ceiling rates
       If vlr > CLRRecs% Then
         Exit Sub
       End If

       If vlc = 1 Then                                  'var column
         CLR(vlr).var = sVlu
       ElseIf vlc = 2 Then                              'rate %
         CLR(vlr).rat = vlu
       ElseIf vlc = 3 Then                              'field type
         SearchCodeString "ALLOILGAS", sVlu, 3, ptr%
         If ptr% > 0 Then
           CLR(vlr).typ = sVlu
         End If
       ElseIf vlc = 4 Then                              'water depth
         CLR(vlr).wat = sVlu
       ElseIf vlc = 5 Then                              'prm 1
         CLR(vlr).prm = sVlu
       ElseIf vlc = 6 Then                              'amount
         CLR(vlr).amt = vlu
       ElseIf vlc = 7 Then                              'sliding scale
         SearchCodeString "YESNO SPC", sVlu, 3, ptr%
         If ptr% > 0 Then
           CLR(vlr).recu = sVlu
         End If
       End If
   Case "TTL"         'variable titles
       If vlr > TTLRecs% Then
         Exit Sub
       End If

       If vlc = 1 Then
         TTLX(vlr).var = sVlu
       ElseIf vlc = 2 Then
         TTLX(vlr).short = sVlu
       ElseIf vlc = 3 Then
         TTLX(vlr).long = sVlu
       End If
   Case "BNS"                   'bonuses
       If vlr > BNSRecs% Then
         Exit Sub
       End If
       If vlc = 1 Then          'bonus category
         SearchCodeString "SIGDISPRD", sVlu, 3, ptr%
         If ptr% > 0 Then
           BNS(vlr).cat = sVlu
         End If
       ElseIf vlc = 2 Then                      'field type
         SearchCodeString "ALLOILGAS", sVlu, 3, ptr%
         If ptr% > 0 Then
           BNS(vlr).fld = sVlu
         End If
       ElseIf vlc = 3 Then                      'bonus amount
         BNS(vlr).bnsamt = vlu
       ElseIf vlc = 4 Then
         If sVlu = "ON" Then
           BNS(vlr).wat = ONSHORE                      'water depth
         Else
           BNS(vlr).wat = vlu
         End If
       ElseIf vlc = 5 Then
         ' GDP 20 Jan 2003
         ' Added new codes for extra volumes
         'SearchCodeString "OILGASOV1OV2PRDOLCGSCV1CV2CCUM", sVlu, 3, ptr%
         SearchCodeString "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0PRDOLCGSCV1CV2CCUM", sVlu, 3, ptr%
         If ptr% > 0 Then
           BNS(vlr).prm = sVlu                   'production stream
         End If
       ElseIf vlc = 6 Then
          BNS(vlr).amt = vlu                      'amount
       End If
   Case "PRT"         'govt participation terms
       If vlr > PRTRecs% Then
         Exit Sub
       End If

       If vlc = 1 Then                          'category
         
         '<<<<<< 14 Jun 2001 JWD
         List$ = "CPXEXPDEV" & CPXCategoryCodesString
         '~~~~~~ was:
         'List$ = "CPXEXPDEVBNSLSERENGEOEDHEDSADHASCDNPDVPPLFFCLTRNEORCP1CP2CP3BAL"
         '>>>>>> End 14 Jun 2001
         
         SearchCodeString List$, sVlu, 3, ptr%
         If ptr% > 0 Then
           PRTX(vlr).cat = sVlu
         End If
       ElseIf vlc = 2 Then                       'participation start date
         SearchCodeString "BEGDISPRDNON", sVlu, 3, ptr%
         If ptr% > 0 Then
           PRTX(vlr).st1 = sVlu
         End If
       ElseIf vlc = 3 Then                            'participation rate
         If sVlu = "PAR" Then
           PRTX(vlr).RATE = PAR                        '-996
         Else
           PRTX(vlr).RATE = vlu
         End If
       ElseIf vlc = 4 Then                            'repayment start date
         SearchCodeString "DISPRDNOR", sVlu, 3, ptr%
         If ptr% > 0 Then
           PRTX(vlr).st2 = sVlu
         End If
       ElseIf vlc = 5 Then                            'repayment %
         PRTX(vlr).repay = vlu
       ElseIf vlc = 6 Then                            'period
         If sVlu = "LIF" Then
           PRTX(vlr).PRD = LIF
         Else
           PRTX(vlr).PRD = vlu
         End If
       ElseIf vlc = 7 Then                            'interest %
         PRTX(vlr).int = vlu
       ElseIf vlc = 8 Then                             'accrual
         If sVlu = "" Or sVlu = "REP" Or sVlu = "ORI" Then
           PRTX(vlr).acc = sVlu
         End If
       End If
   Case "PRR"         'govt participation rates
       If vlr > PRRRecs% Then
         Exit Sub
       End If

       If vlc = 1 Then                                  'var column
         SearchCodeString "PARPOX", sVlu, 3, ptr%
         If ptr% > 0 Then
           PRR(vlr).var = sVlu
         End If
       ElseIf vlc = 2 Then                              'rate %
         PRR(vlr).rat = vlu
       ElseIf vlc = 3 Then                              'field type
         SearchCodeString "ALLOILGAS", sVlu, 3, ptr%
         If ptr% > 0 Then
           PRR(vlr).typ = sVlu
         End If
       ElseIf vlc = 4 Then                              'water depth
         PRR(vlr).wat = sVlu
       ElseIf vlc = 5 Then                              'prm 1
         PRR(vlr).prm = sVlu
       ElseIf vlc = 6 Then                              'amount
         PRR(vlr).amt = vlu
       ElseIf vlc = 7 Then                              'sliding scale
         SearchCodeString "YESNO SPC", sVlu, 3, ptr%
         If ptr% > 0 Then
           PRR(vlr).recu = sVlu
         End If
       End If
   Case "MSC"         'country misc parameters

       If vlr < 5 Then                  'rows 1-4 are titles
         MSCTTL$(vlr) = sVlu
       ElseIf vlr = 5 Then
         If vlc = 1 Then
           MSCData!(1) = vlu
         ElseIf vlc = 2 Then
           MSCData!(2) = vlu
         End If
       ElseIf vlr = 6 Then                     'GM
         If vlc = 1 Then
           MSCData!(3) = vlu
         ElseIf vlc = 2 Then
           MSCData!(4) = vlu
         End If
       ElseIf vlr = 7 Then
         If vlc = 1 Then
           MSCData!(5) = vlu
         ElseIf vlc = 2 Then
           MSCData!(6) = vlu
         End If
       ElseIf vlr = 8 Then
         If vlc = 1 Then
           MSCData!(7) = vlu
         ElseIf vlc = 2 Then
           MSCData!(8) = vlu
         End If
       End If
   Case "ANN"         'country annual forecast
       If vlr > ANNRecs% Then
         Exit Sub
       End If
       If vlc = 1 Then
         SearchCodeString "PR1PR2PR3PR4PR5PRTDP1DP2DP3OT1OT2OT3OT4OT5", sVlu, 3, ptr%
         If ptr% > 0 Then
           ANN(vlr).cat = sVlu
         End If
       ElseIf vlc = 2 Then                     'units
          Select Case ANN(vlr).cat
            Case "PR1", "PR2", "PR3", "PR4", "PR5"
              List$ = "$/B"
              SearchCodeString List$, sVlu, 3, ptr%
            Case "PRT", "DP1", "DP2", "DP3"
              List$ = "%"
              SearchCodeString List$, sVlu, 1, ptr%
            Case "OT1", "OT2", "OT3", "OT4", "OT5"
              List$ = "$MM"
              SearchCodeString List$, sVlu, 3, ptr%
          End Select
          If ptr% > 0 Then
            ANN(vlr).unit = sVlu
          End If
       ElseIf vlc = 3 Then                  'ANN
          SearchCodeString "PDMPJMPDYPJY", sVlu, 3, ptr%
          If ptr% > 0 Then
            ANN(vlr).dat = sVlu
          End If
       ElseIf vlc = 4 Then
          SearchCodeString "1234567890", sVlu, 1, ptr%
          If ptr% > 0 Then
            ANN(vlr).mtd = vlu
          End If
       ElseIf vlc = 5 Then
          ANN(vlr).parm1 = sVlu
       ElseIf vlc = 6 Then                 'ANN
          If Left$(sVlu, 1) = "L" Then
            ANN(vlr).parm2 = LIFE               '-999
          Else
            ANN(vlr).parm2 = vlu
          End If
       ElseIf vlc = 7 Then
          If Left$(sVlu, 1) = "L" Then
            ANN(vlr).parm3 = LIFE               '-999
          Else
            ANN(vlr).parm3 = vlu
          End If
       ElseIf vlc = 8 Then
          ANN(vlr).parm4 = vlu
       ElseIf vlc = 9 Then
          ANN(vlr).parm5 = vlu
       ElseIf vlc = 10 Then
          ANN(vlr).parm6 = vlu
       End If
   Case "PRC"         'price definition
       If vlr > PRCRecs% Then
         Exit Sub
       End If

       If vlc = 1 Then
         PRC(vlr).var = sVlu
       ElseIf vlc = 2 Then
         If sVlu = "" Then
           PRC(vlr).prc1 = sVlu                  'price 1
         Else
           ' GDP 20 Jan 2003
           ' Added codes for new prices
           'SearchCodeString "OPCGPCOP1OP2PR1PR2PR3PR4PR5", sVlu, 3, ptr%
           SearchCodeString "OPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0PR1PR2PR3PR4PR5", sVlu, 3, ptr%
           If ptr% > 0 Then
             PRC(vlr).prc1 = sVlu                  'price 1
           End If
         End If
       ElseIf vlc = 3 Then              'PRC
         If sVlu = "" Then
           PRC(vlr).code = sVlu                  'code
         Else
           SearchCodeString "<>+-*/", sVlu, 1, ptr%
           If ptr% > 0 Then
             PRC(vlr).code = sVlu                  'code
           End If
         End If
       ElseIf vlc = 4 Then
         If sVlu = "" Then
           PRC(vlr).prc2 = sVlu                  'price 1
         Else
           ' 20 Jan 2003
           ' Added codes for new prices
           'SearchCodeString "OPCGPCOP1OP2PR1PR2PR3PR4PR5", sVlu, 3, ptr%
           SearchCodeString "OPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0PR1PR2PR3PR4PR5", sVlu, 3, ptr%
           If ptr% > 0 Then
             PRC(vlr).prc2 = sVlu                  'price 1
           End If
         End If
       ElseIf vlc = 5 Then
         PRC(vlr).base = vlu                     'base %
       ElseIf vlc = 6 Then             'PRC
         PRC(vlr).inc = vlu                       'increment
       End If
   Case "DPL"         'depletion
       If vlr > DPLRecs% Then
         Exit Sub
       End If
       If vlc = 1 Then
         DPL(vlr).var = sVlu                   'variable name
       ElseIf vlc = 2 Then
         DPL(vlr).rat1 = vlu                     'rate 1
       ElseIf vlc = 3 Then          'DPL
         ' 20 Jan 2003
         ' Added codes for extra volumes
         'SearchCodeString "INCILDPRDOILGASOV1OV2", sVlu, 3, ptr%
         SearchCodeString "INCILDPRDOILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0", sVlu, 3, ptr%
         If ptr% > 0 Then
           DPL(vlr).prm1 = sVlu                 'parameter 1
         End If
       ElseIf vlc = 4 Then
         SearchCodeString "<>", sVlu, 1, ptr%
         If ptr% > 0 Then
           DPL(vlr).code = sVlu                  'code
         End If
       ElseIf vlc = 5 Then
         DPL(vlr).rat2 = vlu                      'rate 2
       ElseIf vlc = 6 Then              'DPL
         ' 20 Jan 2003
         ' Added codes for extra volumes
         'SearchCodeString "INCILDPRDOILGASOV1OV2", sVlu, 3, ptr%
         SearchCodeString "INCILDPRDOILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0", sVlu, 3, ptr%
         If ptr% > 0 Then
           DPL(vlr).prm2 = sVlu                 'parameter 1
         End If
       ElseIf vlc = 7 Then
         DPL(vlr).base = vlu                      'recoup base %
       ElseIf vlc = 8 Then
         DPL(vlr).start = vlu                     'recoup start years
       ElseIf vlc = 9 Then
         DPL(vlr).per = vlu                       'recoup period years
       End If
   Case "PDT"         'prepaid / deferred taxes
        If vlr > PDTRecs% Then
          Exit Sub
        End If
        If vlc = 1 Then
          PDT(vlr).var = sVlu                   'variable name
        ElseIf vlc = 2 Then
          PDT(vlr).pre = vlu                           'prepaid %
        ElseIf vlc = 3 Then     'PDT
          PDT(vlr).year = vlu                          'number of prior years
        ElseIf vlc = 4 Then
          SearchCodeString "YESNO ", sVlu, 3, ptr%
          If ptr% > 0 Then
            PDT(vlr).CUR = sVlu                   'include current year
          End If
        ElseIf vlc = 5 Then
          PDT(vlr).def1 = vlu                      'deferred %
        ElseIf vlc = 6 Then
          PDT(vlr).def2 = vlu                      'deferred years
        End If
   Case "RAT"         'prepaid / deferred taxes
        If vlr > RATRecs% Then
          Exit Sub
        End If
        If vlc = 1 Then
          rat(vlr).var = sVlu                   'variable name
        ElseIf vlc = 2 Then
          rat(vlr).numden = sVlu
        ElseIf vlc >= 3 And vlc <= 20 Then
          Select Case vlc Mod 2
            Case 0          'cashflow columns
              rat(vlr).fnc((vlc - 2) / 2) = sVlu
            Case 1          'category columns
              rat(vlr).cat((vlc - 3) / 2) = sVlu
          End Select
        End If
 End Select

End Sub

'---------------------------------------------------------
' Modifications:
' 28 Oct 1996 JWD
'        Add call to procedure PutVariableTitles() to
'     export variable titles to temporary file before exit
'     (line 8060).  This is to support retrieval of titles
'     by report display/export routines and reflects any
'     changes by SNSCTRY commands. (SCO0003).
'
' 21 Nov 1996 JWD
'        Change assignment of Program$ to use MODULENAME.
'
' 10 Jul 2001 JWD
'  -> Add VERSION 6.1 as accepted file version. 6.1 adds
'     abandonment funding provisions section. (C0349)
'
' 17 Sep 2001 JWD
'  -> Set the UseGrossProductionAmounts based on the value
'     of parameter 2 on the GETCTRY run file command line.
'     This controls whether gross or net production
'     amounts are used to determine the sliding scale rate
'     for fiscal definition variables in consolidations.
'     Prior to this change, it always used the net amount.
'     (C0443)
'
' 21 Sep 2001 JWD
'  -> Change number of years in B() array to gc_nMAXLIFE.
'     Was getting subscript out of range in Participation
'     because the life was adjusted for abandonment after
'     B() was dimensioned here. (C0459)
'
' 17 Dec 2002 GDP
' -> After the country file has been read in call the InitializeExcel
'    function, which if any excel links are present in the country file
'    gets a global reference to the Excel.Application object.
' -> Added call to SetCountryFilePath after the call to InitializeExcel
'    This just sets a module level variable in modExcel to the
'    country file path for use later.
'
' 9 Feb 2004 JWD
'  -> Replace call to TerminateExecution with re-raise of
'     error to caller. (C0779)
'  -> Remove End statement, not permitted for ActiveX DLL
'     use. (C0780)
'  -> Remove output to OXFIL.DAT. File is obsolete and not
'     used in any application system. (C0783)
'
' 15 Mar 2004 JWD
'  -> Condition write of variable titles file on being a
'     Mainexec run.
'---------------------------------------------------------
Sub CountryForecast()
'---------------------------------------------------------
' Get Country file specified for run, apply sensitivities
' from run file and perform any required forecasting.
'---------------------------------------------------------
   Dim fno  As Integer        ' temporary file number
   Dim iY   As Integer
   Dim iX   As Integer
   Dim dStart As Double
'---------------------------------------------------------
   dStart = Timer

        'these variables are referred to in forecasting SUBS.
        '  To avoid changing all of the references, this will
        '  set then to YR,Y1,MO,M1
   ProjMo% = mo            'project start month (from general parameters)
   ProjYr% = Val(Right$(LTrim$(str$(YR)), 2))    'project start year
   ProdMo% = M1             'production start month (from general parameters)
   ProdYr% = Val(Right$(LTrim$(str$(Y1)), 2))    'production start year


   ReDim D1(50), D2(50), CKA(30), y(30), t(9), R(9), ab(30, 14), TT(30)
   ReDim MY2(25, 9)              ''', SEQ(18, 2) seq dimmed in Read5Cty or Read6Cty
   
   '<<<<<< 21 Sep 2001 JWD (C0459)
   ReDim B(gc_nMAXLIFE, 14)                  'B() is in common
   '~~~~~~ was:
   'ReDim B(LG, 14)                  'B() is in common
   '>>>>>> End (C0459)
   
'--------------------------------------------------------------------
'dimension arrays needed by data reading routines
   ReDim ANN(1) As ParmType       'country annual forecasts
   ReDim CLD(1) As CLDType        'ceiling definition
   ReDim CLR(1) As CLRType        'ceilings
   ReDim CMP(1) As CMPType
        '''   REDIM SHARED CPX(1)  AS CPXType      'capital expenditure data
   ReDim FDEF(1) As DEFType       'fiscal definition data
   ReDim DPL(1) As DPLType        'depletion
   ReDim DPR(1) As DPRType        'deprec/cost recovery
   ReDim MSCTTL$(1), MSCData!(1)  'Country Misc Parameters
   ReDim BNS(1) As BNSType        'bonuses
   ReDim PDT(1) As PDTType        'prepaid/deferred tax
   ReDim PRC(1) As PRCType        'price definition
   ReDim PRR(1) As PRRType        'govt participation rates
   ReDim PRTX(1) As PRTType       'govt participation terms
   ReDim RTE(1) As RTEType        'variable rates
   ReDim prm(1) As PRMType        '
   ReDim MRP(1) As MRPType        '
   ReDim CNT$(1)                  'country file notes
   ReDim TTLX(1) As TTLType

   ReDim EXR(1) As EXBType         'exchange rate data
   ReDim ETL(1) As TTLType         'exchange rate titles
   '--------------------------------------------------------------------
   Program$ = MODULENAME
   '--------------------------------------------------------------------

   'MAIN PROGRAM STARTS HERE
100

105
6000  ' THIS READS IN COUNTRY FILE
6010  On Local Error GoTo 30000
6020  If Left$(RF$(1), 7) <> "GETCTRY" Then
         Error 252                'Expecting GETCTRY
      End If

'--------------------------------------------------------------------
6060  N1C$ = RF$(2) + RF$(3)
6070  N2$ = N1C$ + ".CTY"
      SensCty$ = "N"    'OXY data item
'----------------------------------------------------------------------
'       put file name in the OXFIL file - for use by the OXY data base routines
'1-20-93

      zipfileno% = FreeFile
6130
'''      9 Feb 2004 JWD (C0783) Remove output to OXFIL.DAT
'''      Open FOxfil$ For Append As #zipfileno%
'''         Print #zipfileno%, N2$
'''      Close #zipfileno%

      CTYFile$ = N2$    'OXY database item
'----------------------------------------------------------------------

      ' Read Country File
6140  Open N2$ For Input As #2
6150     Input #2, ver$
         
         '<<<<<< 2 Aug 2001 JWD (C0363)
         If ver$ = "VERSION 6.0" Or ver$ = "VERSION 6.1" Or ver$ = "VERSION 6.2" Then
         '~~~~~~ was:
         ''<<<<<< 10 Jul 2001 JWD (C0349)
         'If ver$ = "VERSION 6.0" Or ver$ = "VERSION 6.1" Then
         ''~~~~~~ was:
         ''If ver$ = "VERSION 6.0" Then
         ''>>>>>> End (C0349)
         '>>>>>> End (C0363)
         
            Read6Cty 2, ver$    'file number = 2
         ElseIf ver$ = "VERSION 5.4" Or ver$ = "VERSION 5.0" Or ver$ = "VERSION 5.1" Or ver$ = "VERSION 5.2" Or ver$ = "VERSION 5.3" Then
            Read5Cty 2, ver$    'file number = 2
         Else             'prior to version 5.0 data file
            Error 255
         End If
      Close #2

      ' Start - Added [GDP 17 Dec 2002]
      ' Get reference to Excel object if necessary
      InitializeExcel
      ' Set country file path - module level variable in modExcel
      SetCountryFilePath RF(2)
      ' End
      '<<<<<< 17 Sep 2001 JWD (C0443)
      If StrComp(Left$(RF$(5), 3), "GRS", vbTextCompare) = 0 Then
         UseGrossProductionAmounts = True
      Else
         UseGrossProductionAmounts = False
      End If
      '>>>>>> End (C0443)
      
'--------------------------------------------------------------------
8000  ' Do Country Sensitivities

8040  GetRunFileLine    'get next run file line  (replaced old GOSUB 48000)
      Do While Left$(RF$(1), 7) = "SNSCTRY"
         If BDebugging Then
            GoSub DebugPrint
         End If
8045     SingleValueSens            'single value sensitivities
         SensCty$ = "Y"    'OXY data item
8050     GetRunFileLine
      Loop

8060
    ' 15 Mar 2004 JWD Condition on being a Mainexec run
    If g_bIsMainexecRun Then
           ' 28 Oct 1996 JWD
           ' Store variable titles to temp file for report
           ' display/export.
        fno = FreeFile
        Open TempDir$ & FILEVARTITLES For Output As #fno
           Write #fno, TTLRecs%
           PutVariableTitles fno, TTLRecs%, TTLX()
        Close #fno
    End If
    
8100  If Left$(RF$(1), 3) <> "RUN" Then
         Error 251
      End If
'--------------------------------------------------------------------
8160  ' Run Economics
      If BDebugging Then
         GoSub DebugPrint
      End If
      'if there are any exchange rates to forecast, forecast now
      ForecastExchange

      CTYConvertData               'convert data to "old" arrays

      RAP = 0

      'This checks General Parameters (Dates)
      Dm = ((Y1 - YR) * 12) + (M1 - mo)
      DQ = Int((mo + Dm - 1) / 12)
      DN = ((Dm + mo - 1) / 12) - DQ
      DZ = 1 - DN

      'Forecast ANN screen
      If ANNRecs% <> 0 Then
         ReDim units$(UBound(B, 2))
         ForecastCTY B!(), units$(), Rc%
      End If

      If MY2T <> 0 Then
8220     GoSub 40000               'inflation
      End If
8230
      Erase D1, D2, CKA, y, t, R, ab, TT
      Erase MY2

      NextProgram$ = AppDir$ + "FISCAL"

      '''PushStats "COUNTRY", dStart
      Exit Sub
'-----------------------------------------------------------------------
30000 'CountryForecast Error Handler
      ' 9 Feb 2004 JWD (C0779) Replace with re-raise of error to caller
      Err.Raise Err.number                  ' TerminateExecution
      ' 9 Feb 2004 JWD (C0780) Remove End statement
      ' End

'-----------------------------------------------------------------------
40000 Rem SET B( , ) AND APPLY INFLATION TO PRICES

40010 For iY = 1 To 14
40015   MD = 1
40020   If iY <= 4 Then GoTo 40080
40030   For iX = 1 To LG
40040     B(iX, iY) = ab(iX, iY)
40050     B(0, iY) = B(0, iY) + B(iX, iY)
40060   Next iX
40070   GoTo 40120
40080   For iX = 1 To LG
40085           'MD = MD * (1 + (A(iX, 9) / 100))  THIS IS THE OLD LINE
          MD = MD * 1       ' THIS IS NEW TEMPORARY LINE
40090     B(iX, iY) = ab(iX, iY) * MD
40100     B(0, iY) = B(0, iY) + B(iX, iY)
40110   Next iX
40120 Next iY
40130 Return
'--------------------------------------------------------------------
DebugPrint:
      '''Print "EXECUTING LINE   "; AR; "  OF  "; RFT
      Return

End Sub

