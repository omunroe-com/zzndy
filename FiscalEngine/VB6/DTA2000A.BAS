Attribute VB_Name = "DTA2000A"
'Name:         DTA2000A.BAS
'Function:     Giant Data File Forecasting Support Routines
' ********************************************************
' *     COPYRIGHT © 1990-1998 PETROCONSULTANTS, INC.     *
' *                  ALL RIGHTS RESERVED                 *
' ********************************************************
' *  This program file is proprietary information of     *
' *  Petroconsultants, Incorporated.  Unauthorized use   *
' *  for any purpose is prohibited.                      *
' ********************************************************
'---------------------------------------------------------
' This module is modified from GNTFCST2.BAS.
'---------------------------------------------------------
' This module contains subroutines that generate forecasts
' of an item.
'---------------------------------------------------------
' 17 Mar 1995 JWD
'  -> Removed procedures: SetPrinter, GetRunFileLine,
'     ReplaceChar, GetRecord.
'
' 13 Feb 1996 JWD
'  -> Replaced common include file CTYIN.BAS with
'     CTYIN1.BG.
'  -> Add explicit declaration of default storage class
'     as Single.
'
' 14 Feb 1996 JWD
'  -> Renamed routines with same names as are contained
'     in CNTYFCST.BAS and CTYFCST2.BAS.
'
' 19 Feb 1996 JWD
'  -> Replaced explicit user type definitions with
'     include files.
'  -> Replaced variables True, False with constants
'     declared in 'trufalse.bc'.  Removed initialization
'     of variables in code.
'
' 22 Feb 1996 JWD
'  -> Removed procedure DTASearchString().  Replaced
'     with SearchCodeString().
'
' 21 Nov 1996 JWD
'  -> Modified and renamed from GNTFCST2.BAS.
'  -> Changed DTAForecastStep().  (SCO0013)
'  -> Removed $include and $dynamic metacommands.
'
' 11 Jun 1998 JWD
'  -> Changed DTAForecastStep(). (SCO0046)
'
' 25 Aug 1998 JWD
'  -> Change DTAVerifyDates().
' 18 Aug 1999 GDP
'  -> Changed lines calculating the working interest share of production,
'     WI share of Opex and WI share of revenue to give the company's net share
'     2 instances of each calc
'  -> Changed lines calculating the WI share of capex to give the companies net share
'     changed three instances of this.
' 12 Dec 2000 GDP
'  -> Added line LG=LGI at the end of ForecastSetLife
' 21 Dec 2000 GDP
'  -> Commented out code in EXTNameEntered see comment in that routine
' 11 Jan 2001 GDP
'  -> Added code just after 38860 in ConsolValues which resets the primary stream
'     of the consolidation to Oil from Gas
'
' 14 Jun 2001 JWD
'  -> Changed InflateCapex(). (C0332)
'  -> Changed PercentSens(). (C0332)
'
' 14 Sep 2001 JWD
'  -> Changed ConsolValues(). (C0443)
'
' 17 Sep 2001 JWD
'  -> Changed ConsolValues(). (C0443)
'
' 18 Sep 2001 JWD
'  -> Changed ConsolValues(). (C0443)
'
' 24 Sep 2001 JWD
'  -> Changed ConsolValues(). (C0460)
'
' 20 Jan 2003 GDP
'  -> Changed DTAConvertABtoA().
'  -> Changed ConsolValues().
'  -> Changed PercentSens()
'
' 27 May 2003 JWD
'  -> Changed PercentSens(). (C0700)
'
' 29 Dec 2004 JWD
'  -> Changed ConsolValues(). (C0846)
'
' 22 Apr 2005 JWD
'  -> Changed ConsolValues(). (C0873)
'
' 12 May 2005 JWD
'  -> Changed PercentSens(). (C0876)
'
' 13 May 2005 JWD
'  -> Changed PercentSens(). (C0877)
'---------------------------------------------------------
DefSng A-Z

'$subtitle: 'ApplyInflation'
'$Page:
Sub ApplyInflation(Datacol!(), infcol!())
'--------------------------------------------------------------------
'This sub applies inflation to to datacol!(). Datacol!() has the values
'  to be inflated and infcol!() contains the inflation percentages (per
'  year) to apply. NOTE: the values are the annual percentage inflation
'  values. The factor to apply to a given year in datacol!() is the
'  cumulative inflation to date. ie. if infcol!(1) = 10 and
'  infcol!(2) = 10 then we inflate datacol!(1) by 10%. Datacol!(2) will
'  be inflated by 21% (1 * 1.1 * 1.1)
100
   ReDim factor!(UBound(infcol!))
   factor!(0) = 1
   For i% = 1 To UBound(factor!)
     factor!(i%) = factor!(i% - 1) * (1 + infcol!(i%) / 100)
   Next i%
   For i% = 1 To UBound(Datacol!)
     Datacol!(i%) = Datacol!(i%) * factor!(i%)
   Next i%
   Erase factor!

End Sub

Rem $subtitle: 'ConsolValues'
Rem $Page:
'
' Modifications:
' 14 Sep 2001 JWD
'  -> Add summing of production values (A(x, 1-4)) into
'     CA(x, 26-29) to save the gross consolidated values
'     for optionally selecting sliding scale variable
'     rates. (C0443)
'
' 17 Sep 2001 JWD
'  -> Add variable symbol for number of columns in CA()
'     and D8() arrays. (C0443)
'  -> Change loop bounds for loops referencing CA() and
'     D8() to use symbol containing number of columns to
'     access. (C0443)
'
' 18 Sep 2001 JWD
'  -> Add calculation of gross (100%) revenues by product
'     and summing into CA(x, 30-33) to save the gross
'     revenue for calculating gross price equivalent
'     production for selecting sliding scale rates.
'     (C0443)
'
' 24 Sep 2001 JWD
'  -> Correct subscript used for target elements of
'     consolidation array when consolidating gross
'     production. (C0460)
'
' 20 Jan 2003 GDP
'  -> A array references now use constants defined in modArrayConst
'
' 29 Dec 2004 JWD
'  -> Add determination of 3rd party, NOC and Govt net
'     effective interests for cash flow to use to output
'     consolidated cash flows and indicators for same.
'     (C0846)
'
' 22 Apr 2005 JWD
'  -> Changed conditional expressions testing symbol
'     g_nFiscalEvents to perform bit-wise And before the
'     relational (>) operation. Was always executing the
'     conditional code if any financing events, not the
'     intended masking of events. (C0873)
'
Sub ConsolValues(TCC As Integer, rA, L10(), L20(), D8(), CA(), MYC(), MSPM(), PMMS$())
'--------------------------------------------------------------------
'This SUB replaces the GOSUB 38500 in FORECAST.EXE.
'This SUB stores values incase we need them for consolidaton.
'---------------------------------------------------------
   Dim AddTCC As Integer
   Dim bRVS As Boolean
   
   '<<<<<< 17 Sep 2001 JWD (C0443)
   Dim num_columns As Integer
   '>>>>>> End (C0443)
   
'---------------------------------------------------------
    ' 28 Dec 2004 JWD (C0846) Variable symbols for consolidating 3rd party, NOC and govt
    Dim rWinTmp As Single           ' company working interest
    Dim rParTmp As Single           ' NOC revenue participation rate
    Dim rPoxTmp As Single           ' NOC opex participation rate
    Dim rRevtmp As Single           ' gross revenues
    Dim rOPXTmp As Single           ' gross operating expense
    Dim rWinFin As Single           ' company working interest for financing calcs
    Dim rParFin As Single           ' NOC financing participation rate
    Dim rFinTmp As Single           ' gross financing amount for financing calcs
    ' End (C0846)
           

    Dim rAMTtmp As Single           ' generic amount
    
    Dim rTCXtmp As Single           ' gross-level capital expenditure amount
    Dim rOCXtmp As Single           ' operating-level capital expenditure amount
    Dim rGCXtmp As Single           ' group-level capital expenditure amount
    Dim rCCXtmp As Single           ' company-level capital expenditure amount
    
    Dim rGRPtmp As Single           ' group-level share (1-rParTmp)
    Dim rCMPtmp As Single           ' company-level share ((1-rParTmp) * rWinTmp)
    Dim rGOXtmp As Single           ' group-level opex share (1-rPoxTmp)
    Dim rCOXtmp As Single           ' company-level opex share ((1-rPoxTmp)*rWinTmp)
    
    Dim D8X() As Single             ' temp array for consolidating gna_ACX() values
    
    On Error GoTo err_ConsolValues
    
'MsgBox "    in consolvalues sub  ra = " & rA & " LG = " & LG & "   L10(2) = " & L10(2) & "   L20(2) = " & L20(2)
    ' Added GDP 13/9/99 for pre tax consol of repayment
    ReDim TOTREPAY(LG)
    ' Added GDP 14/11/2000 for consolidating of loan repayments
    ReDim TOTFINANCE(LG)
    ' Check for existence of RVS file and read it. GDP 18/8/99
    bRVS = Len(Dir$(TempDir$ & g_sDataFileNoExt & "." & c_sRVSEXT)) <> 0
    If bRVS Then
        ReadRVS
    End If
    ' MY3TT is read in ReadRVS. Assign to MY3T GDP 18/08/99
    MY3T = my3tt
    
    '<<<<<< 17 Sep 2001 JWD (C0443)
    ' Get the number of columns in CA() and D8()
    ' For loop control later...
    num_columns = UBound(CA, 2)
    '>>>>>> End (C0443)
    
4781
      L20(1) = mo:      L20(2) = YR
      L20(3) = M1:      L20(4) = Y1
      L20(10) = M3:     L20(11) = Y3
      L20(5) = LG:      L20(6) = L20(2) + ((L20(1) - 1) / 12)
      L20(7) = L20(4) + ((L20(3) - 1) / 12)
      L20(12) = L20(11) + ((L20(10) - 1) / 12)
      L20(8) = L20(2) + L20(5) - 1
      L20(9) = L20(7) + LFI

      If L20(2) < L10(2) Then GoTo 38690
'MsgBox "L20(2) >= L10(2)      "
      For x = 1 To L20(5)
        XI = L20(2) - L10(2) + x
        If XI > gc_nMAXLIFE Then XI = gc_nMAXLIFE
        ' GDP 20 Jan 2003
        ' Changed loop bounds to be constants
           
        ' 28 Dec 2004 JWD (C0846) Capture participation rates and win
        If bRVS Then
            rParTmp = PRTRTE(x)
            rPoxTmp = OPXRTE(x)
        Else
            rParTmp = 0
            rPoxTmp = 0
        End If

        If A(x, gc_nAWIN) = 0 Then
            rWinTmp = 1
        Else
            rWinTmp = A(x, gc_nAWIN) / 100
        End If
        ' End (C0846)
        
        rGRPtmp = (1 - rParTmp)
        rCMPtmp = rGRPtmp * rWinTmp
        rGOXtmp = (1 - rPoxTmp)
        rCOXtmp = rGOXtmp * rWinTmp
        
        ' Reserves additions for UOP
        gna_ACX(XI, gc_nARES, 0) = gna_ACX(XI, gc_nARES, 0) + A(x, gc_nARES)
        gna_ACX(XI, gc_nARES, 1) = gna_ACX(XI, gc_nARES, 1) + A(x, gc_nARES)
        gna_ACX(XI, gc_nARES, 2) = gna_ACX(XI, gc_nARES, 2) + A(x, gc_nARES)
        gna_ACX(XI, gc_nARES, 3) = gna_ACX(XI, gc_nARES, 3) + A(x, gc_nARES)
        
        For w = gc_nAMINVOL To gc_nAMAXVOL   ' accumulate working interest share of production
' Commented out GDP 18/08/99 changes for consolidation of net of participation amounts
'           If a(x, 6) <> 0 Then CA(XI, w) = CA(XI, w) + (a(x, w) * (a(x, 6) / 100))
'           If a(x, 6) = 0 Then CA(XI, w) = CA(XI, w) + a(x, w)
           ' GDP 20 Jan 2003
           ' Changed A(x, 6) to A(x, gc_nAWIN)
           If bRVS Then
               If A(x, gc_nAWIN) <> 0 Then CA(XI, w) = CA(XI, w) + (A(x, w) * (A(x, gc_nAWIN) / 100) * (1 - PRTRTE(x)))
               If A(x, gc_nAWIN) = 0 Then CA(XI, w) = CA(XI, w) + A(x, w) * (1 - PRTRTE(x))
           Else
               If A(x, gc_nAWIN) <> 0 Then CA(XI, w) = CA(XI, w) + (A(x, w) * (A(x, gc_nAWIN) / 100))
               If A(x, gc_nAWIN) = 0 Then CA(XI, w) = CA(XI, w) + A(x, w)
           End If
           
           '<<<<<< 14 Sep 2001 JWD (C0443)
           ' Accumulate the gross production
           ' A(X,1-4) (Oil, Gas, OV1, OV2) into CA(XI, 26-29)
           ' GDP 20 Jan 2003
           ' Changed CA references to use constants
           CA(XI, w + gc_nCAGROSSOFFSETVOLUME) = CA(XI, w + gc_nCAGROSSOFFSETVOLUME) + A(x, w)
           '>>>>>> End (C0443)
           
           rAMTtmp = A(x, w)
           gna_ACX(XI, w, 0) = gna_ACX(XI, w, 0) + rAMTtmp
           gna_ACX(XI, w, 1) = gna_ACX(XI, w, 1) + rAMTtmp
           gna_ACX(XI, w, 2) = gna_ACX(XI, w, 2) + rAMTtmp * rGRPtmp
           gna_ACX(XI, w, 3) = gna_ACX(XI, w, 3) + rAMTtmp * rCMPtmp
           
        Next w
        ' GDP 20 Jan 2003
        ' Loop bounds now defined as constants
        For w = gc_nAMINPRC To gc_nAMAXPRC          ' accumulate working interest revenues by product
' Commented out GDP 18/08/99 changes for consolidation of net of participation amounts
'           CA(XI, w) = CA(XI, w) + (a(x, w - 6) * a(x, w) * (a(x, 6) / 100))
           ' GDP 20 Jan 2003
           ' Changed to use constants defined in modArrayConst
           If bRVS Then
              CA(XI, w) = CA(XI, w) + (A(x, w - gc_nAPRICEOFFSET) * A(x, w) * (A(x, gc_nAWIN) / 100) * (1 - PRTRTE(x)))
           Else
              CA(XI, w) = CA(XI, w) + (A(x, w - gc_nAPRICEOFFSET) * A(x, w) * (A(x, gc_nAWIN) / 100))
           End If
           
           '<<<<<< 18 Sep 2001 JWD (C0443)
           ' Accumulate the gross (100%) revenues by product
           ' GDP 20 Jan 2003
       
           CA(XI, w + gc_nCAGROSSOFFSETREVENUE) = CA(XI, w + gc_nCAGROSSOFFSETREVENUE) + (A(x, w - gc_nAPRICEOFFSET) * A(x, w))
           '>>>>>> End (C0443)
           
           ' 28 Dec 2004 JWD (C0846) Add accumulation of revenues to 3rd party and NOC
           ' Compute the revenue for this production, accumulate gross
           rRevtmp = A(x, w) * A(x, w - gc_nAPRICEOFFSET)
           CA(XI, gc_nCA_GRSREV) = CA(XI, gc_nCA_GRSREV) + rRevtmp

           ' Add the revenue share to the respective interest
           CA(XI, gc_nCA_CMPREV) = CA(XI, gc_nCA_CMPREV) + rRevtmp * rWinTmp * (1 - rParTmp)
           CA(XI, gc_nCA_3DPREV) = CA(XI, gc_nCA_3DPREV) + rRevtmp * (1 - rWinTmp) * (1 - rParTmp)
           CA(XI, gc_nCA_NOCREV) = CA(XI, gc_nCA_NOCREV) + rRevtmp * rParTmp
           ' End (C0846)
           
           
           rAMTtmp = A(x, w) * A(x, w - gc_nAPRICEOFFSET)
           gna_ACX(XI, w, 0) = gna_ACX(XI, w, 0) + rAMTtmp
           gna_ACX(XI, w, 1) = gna_ACX(XI, w, 1) + rAMTtmp
           gna_ACX(XI, w, 2) = gna_ACX(XI, w, 2) + rAMTtmp * rGRPtmp
           gna_ACX(XI, w, 3) = gna_ACX(XI, w, 3) + rAMTtmp * rCMPtmp
           
        Next w
        ' GDP 20 Jan 2003
        ' Loop from Min Opex to Max Opex
        ' changed to use constants
        For w = gc_nAMINOPX To gc_nAMAXOPX         ' accumulate working interest opex
' Commented out GDP 18/08/99 changes for consolidation of net of participation amounts
'           If a(x, 6) <> 0 Then CA(XI, w) = CA(XI, w) + (a(x, w) * (a(x, 6) / 100))
'           If a(x, 6) = 0 Then CA(XI, w) = CA(XI, w) + a(x, w)
           ' GDP 20 Jan 2003
           ' Changed A(x, 6) to A(x, gc_nAWIN)
           If bRVS Then
               If A(x, gc_nAWIN) <> 0 Then CA(XI, w) = CA(XI, w) + (A(x, w) * (A(x, gc_nAWIN) / 100) * (1 - OPXRTE(x)))
           Else
               If A(x, gc_nAWIN) = 0 Then CA(XI, w) = CA(XI, w) + A(x, w)
           End If
           
            ' 28 Dec 2004 JWD (C0846) Add accumulation of operating expense to 3rd party and NOC
            ' Get operating expense, accumulate to gross operating expense
            rOPXTmp = A(x, w)
            CA(XI, gc_nCA_GRSOPX) = CA(XI, gc_nCA_GRSOPX) + rOPXTmp
    
            ' Add expense to respective interests
            CA(XI, gc_nCA_CMPOPX) = CA(XI, gc_nCA_CMPOPX) + rOPXTmp * rWinTmp * (1 - rPoxTmp)
            CA(XI, gc_nCA_3DPOPX) = CA(XI, gc_nCA_3DPOPX) + rOPXTmp * (1 - rWinTmp) * (1 - rPoxTmp)
            CA(XI, gc_nCA_NOCOPX) = CA(XI, gc_nCA_NOCOPX) + rOPXTmp * rPoxTmp
            ' End (C0846)

            CA(XI, gc_nca_GRPOPX) = CA(XI, gc_nca_GRPOPX) + rOPXTmp * (1 - rPoxTmp)
           
           rAMTtmp = A(x, w)
           gna_ACX(XI, w, 0) = gna_ACX(XI, w, 0) + rAMTtmp
           gna_ACX(XI, w, 1) = gna_ACX(XI, w, 1) + rAMTtmp
           gna_ACX(XI, w, 2) = gna_ACX(XI, w, 2) + rAMTtmp * rGOXtmp
           gna_ACX(XI, w, 3) = gna_ACX(XI, w, 3) + rAMTtmp * rCOXtmp
                
        Next w
        ' GDP 20 Jan 2003
        ' Changed to use constants from modArrayConst
        For w = gc_nAMINADJ To gc_nAMAXADJ         ' accumulate adjusting entries
           CA(XI, w) = CA(XI, w) + A(x, w)
        Next w

            '8/13/96 - store Inflate() & DFL()
        ' GDP 20 Jan 2003
        ' Changed to use constants from modArrayConst
        CA(XI, gc_nAINF1) = Inflate(x, 1)
        CA(XI, gc_nAINF2) = Inflate(x, 2)
        CA(XI, gc_nADFL) = DFL(1)
        ' Added GDP 13/9/99 - consolidation of repayment
        CA(XI, gc_nAREPAY) = CA(XI, gc_nAREPAY) + TOTREPAY(x)
        ' Added GDP 14/11/2000 - consolidation loan repayments etc
        CA(XI, gc_nAFIN) = CA(XI, gc_nAFIN) + TOTFINANCE(x)
        
        ' 29 Dec 2004 JWD (C0846) Capture financing information
        ' 22 Apr 2005 JWD (C0873) Correction: change order of operations, replace numeral with symbol
        If (g_nFinanceEvents And gc_nFinanceEvents_FIN) < 1 Then      ' do nothing, no financing
            rFinTmp = 0
        Else
            rWinFin = 1
            rParFin = 0
            ' 22 Apr 2005 JWD (C0873) Correction: change order of operations, replace numeral with symbol
            If (g_nFinanceEvents And gc_nFinanceEvents_WIN) > 0 Then  ' amounts are net of win
                rWinFin = rWinTmp
            End If
            ' 22 Apr 2005 JWD (C0873) Correction: change order of operations, replace numeral with symbol
            If (g_nFinanceEvents And gc_nFinanceEvents_PAR) > 0 Then  ' amounts are net of par
                rParFin = rParTmp
            End If
            ' Gross up the financing
            rFinTmp = TOTFINANCE(x) / rWinFin / (1 - rParFin)
        End If

        CA(XI, gc_nCA_GRSFIN) = CA(XI, gc_nCA_GRSFIN) + rFinTmp
        CA(XI, gc_nCA_CMPFIN) = CA(XI, gc_nCA_CMPFIN) + rFinTmp * rWinFin * (1 - rParFin)
        CA(XI, gc_nCA_3DPFIN) = CA(XI, gc_nCA_3DPFIN) + rFinTmp * (1 - rWinFin) * (1 - rParFin)
        CA(XI, gc_nCA_NOCFIN) = CA(XI, gc_nCA_NOCFIN) + rFinTmp * rParFin
        ' End (C0846)
        
      Next x

      GoTo 38860
'MsgBox "L20(2) < L10(2) line 38690"

38690
      If L10(2) = 10000 Then
'MsgBox "L10(2) = 1000  @38690  going to 38760        "
        GoTo 38760
      End If
      
      ReDim D8X(0 To gc_nMAXLIFE, 0 To gc_nAMAXOPX, 0 To 3)
      
      For x = 1 To L10(5)
        XI = L10(2) - L20(2) + x
        If XI > gc_nMAXLIFE Then XI = gc_nMAXLIFE
        
        '<<<<<< 17 Sep 2001 JWD (C0443)
        For w = 1 To num_columns
           D8(XI, w) = CA(x, w)
        Next w
        '~~~~~~ was:
        'For w = 1 To 25    '8/13/96  was:   20 ' GDP 13/9/99 up to 24 ' GDP 14/11/2000 up to 25
        '  D8(XI, w) = CA(x, w)
        'Next w
        '>>>>>> End (C0443)
      
        For w = gc_nAMINVOL To gc_nAMAXOPX
            D8X(XI, w, 0) = gna_ACX(x, w, 0)
            D8X(XI, w, 1) = gna_ACX(x, w, 1)
            D8X(XI, w, 2) = gna_ACX(x, w, 2)
            D8X(XI, w, 3) = gna_ACX(x, w, 3)
        Next w
      
      Next x
38760
'MsgBox "38760"
      For x = 1 To gc_nMAXLIFE
        '<<<<<< 17 Sep 2001 JWD (C0443)
        For w = 1 To num_columns
           CA(x, w) = 0
        Next w
        '~~~~~~ was:
        'For w = 1 To 25      '8/13/96 was: 20 ' GDP 13/9/99 up to 24 ' GDP 14/11/2000 up to 25
        '  CA(x, w) = 0
        'Next w
        '>>>>>> End (C0443)
        
        For w = gc_nAMINVOL To gc_nAMAXOPX
            gna_ACX(x, w, 0) = 0
            gna_ACX(x, w, 1) = 0
            gna_ACX(x, w, 2) = 0
            gna_ACX(x, w, 3) = 0
        Next w
        
      Next x

       For x = 1 To L20(5)
         
            ' 28 Dec 2004 JWD (C0846) Capture participation rates and win
            If bRVS Then
                rParTmp = PRTRTE(x)
                rPoxTmp = OPXRTE(x)
            Else
                rParTmp = 0
                rPoxTmp = 0
            End If
        
            If A(x, gc_nAWIN) = 0 Then
                rWinTmp = 1
            Else
                rWinTmp = A(x, gc_nAWIN) / 100
            End If
            ' End (C0846)
         
            rGRPtmp = (1 - rParTmp)
            rCMPtmp = rGRPtmp * rWinTmp
            rGOXtmp = (1 - rPoxTmp)
            rCOXtmp = rGOXtmp * rWinTmp
         
            ' Reserves additions for UOP
            gna_ACX(x, gc_nARES, 0) = gna_ACX(x, gc_nARES, 0) + A(x, gc_nARES)
            gna_ACX(x, gc_nARES, 1) = gna_ACX(x, gc_nARES, 1) + A(x, gc_nARES)
            gna_ACX(x, gc_nARES, 2) = gna_ACX(x, gc_nARES, 2) + A(x, gc_nARES)
            gna_ACX(x, gc_nARES, 3) = gna_ACX(x, gc_nARES, 3) + A(x, gc_nARES)
        
         ' GDP 20 Jan 2003
         ' Loop through volumes
         ' Changed from hard coded numeric values to constants
         For w = gc_nAMINVOL To gc_nAMAXVOL           ' accumulate working interest share of production
         'Commented out GDP 18/08/99 - net of participation consolidation routine
'           If a(x, 6) <> 0 Then CA(x, w) = CA(x, w) + (a(x, w) * (a(x, 6) / 100))
'           If a(x, 6) = 0 Then CA(x, w) = CA(x, w) + a(x, w)
           ' GDP 20 Jan 2003
           ' Change A(x, 6) to A(x, gc_nAWIN)
           If bRVS Then
              If A(x, gc_nAWIN) <> 0 Then CA(x, w) = CA(x, w) + (A(x, w) * (A(x, gc_nAWIN) / 100) * (1 - PRTRTE(x)))
              If A(x, gc_nAWIN) = 0 Then CA(x, w) = CA(x, w) + A(x, w) * (1 - PRTRTE(x))
           Else
              If A(x, gc_nAWIN) <> 0 Then CA(x, w) = CA(x, w) + (A(x, w) * (A(x, gc_nAWIN) / 100))
              If A(x, gc_nAWIN) = 0 Then CA(x, w) = CA(x, w) + A(x, w)
           End If
           
           '<<<<<< 24 Sep 2001 JWD (C0460)
           ' GDP 20 Jan 2003
           ' Changed to use constants from modArrayConst
           CA(x, w + gc_nCAGROSSOFFSETVOLUME) = CA(x, w + gc_nCAGROSSOFFSETVOLUME) + A(x, w)
           '~~~~~~ was:
           ''<<<<<< 14 Sep 2001 JWD (C0443)
           '' Accumulate the gross production
           '' A(X,1-4) (Oil, Gas, OV1, OV2) into CA(XI, 26-29)
           'CA(XI, w + 25) = CA(XI, w + 25) + A(x, w)
           ''>>>>>> End (C0443)
           '>>>>>> End (C0460)
           
           rAMTtmp = A(x, w)
           gna_ACX(x, w, 0) = gna_ACX(x, w, 0) + rAMTtmp
           gna_ACX(x, w, 1) = gna_ACX(x, w, 1) + rAMTtmp
           gna_ACX(x, w, 2) = gna_ACX(x, w, 2) + rAMTtmp * rGRPtmp
           gna_ACX(x, w, 3) = gna_ACX(x, w, 3) + rAMTtmp * rCMPtmp
           
         Next w
         ' Note - I need to add currency conversion here - the consolidation currency
         ' GDP 20 Jan 2003
         ' Changed loop bounds to constants
         For w = gc_nAMINPRC To gc_nAMAXPRC          ' accumulate working interest revenues by product
            'Commented out GDP 18/08/99 - net of participation consolidation routine
'           CA(x, w) = CA(x, w) + (a(x, w - 6) * a(x, w) * (a(x, 6) / 100))
           ' GDP 20 Jan 2003
           ' Changed to use constants - modArrayConst
           If bRVS Then
              CA(x, w) = CA(x, w) + (A(x, w - gc_nAPRICEOFFSET) * A(x, w) * (A(x, gc_nAWIN) / 100) * (1 - PRTRTE(x)))
           Else
              CA(x, w) = CA(x, w) + (A(x, w - gc_nAPRICEOFFSET) * A(x, w) * (A(x, gc_nAWIN) / 100))
           End If
           
           '<<<<<< 18 Sep 2001 JWD (C0443)
           ' Accumulate the gross (100%) revenues by product
           ' GDP 20 Jan 2003
           ' Changed to use constants
           CA(x, w + gc_nCAGROSSOFFSETREVENUE) = CA(x, w + gc_nCAGROSSOFFSETREVENUE) + (A(x, w - gc_nAPRICEOFFSET) * A(x, w))
           '>>>>>> End (C0443)
           
           ' 28 Dec 2004 JWD (C0846) Add accumulation of revenues to 3rd party and NOC
           ' Compute the revenue for this production, accumulate gross
           rRevtmp = A(x, w) * A(x, w - gc_nAPRICEOFFSET)
           CA(x, gc_nCA_GRSREV) = CA(x, gc_nCA_GRSREV) + rRevtmp

           ' Add the revenue share to the respective interest
           CA(x, gc_nCA_CMPREV) = CA(x, gc_nCA_CMPREV) + rRevtmp * rWinTmp * (1 - rParTmp)
           CA(x, gc_nCA_3DPREV) = CA(x, gc_nCA_3DPREV) + rRevtmp * (1 - rWinTmp) * (1 - rParTmp)
           CA(x, gc_nCA_NOCREV) = CA(x, gc_nCA_NOCREV) + rRevtmp * rParTmp
           ' End (C0846)
           
           rAMTtmp = A(x, w) * A(x, w - gc_nAPRICEOFFSET)
           gna_ACX(x, w, 0) = gna_ACX(x, w, 0) + rAMTtmp
           gna_ACX(x, w, 1) = gna_ACX(x, w, 1) + rAMTtmp
           gna_ACX(x, w, 2) = gna_ACX(x, w, 2) + rAMTtmp * rGRPtmp
           gna_ACX(x, w, 3) = gna_ACX(x, w, 3) + rAMTtmp * rCMPtmp
         
         Next w
         ' GDP 20 Jan 2003
         ' Loop bounds defined using constants
         For w = gc_nAMINOPX To gc_nAMAXOPX         ' accumulate working interest opex
            'Commented out GDP 18/08/99 - net of participation consolidation routine
'           If a(x, 6) <> 0 Then CA(x, w) = CA(x, w) + (a(x, w) * (a(x, 6) / 100))
'           If a(x, 6) = 0 Then CA(x, w) = CA(x, w) + a(x, w)
           ' GDP 20 Jan 2003
           ' A(x, 6) to A(x, gc_nAWIN)
           If bRVS Then
              If A(x, gc_nAWIN) <> 0 Then CA(x, w) = CA(x, w) + (A(x, w) * (A(x, gc_nAWIN) / 100) * (1 - OPXRTE(x)))
              If A(x, gc_nAWIN) = 0 Then CA(x, w) = CA(x, w) + A(x, w) * (1 - OPXRTE(x))
           Else
              If A(x, gc_nAWIN) <> 0 Then CA(x, w) = CA(x, w) + (A(x, w) * (A(x, gc_nAWIN) / 100))
              If A(x, gc_nAWIN) = 0 Then CA(x, w) = CA(x, w) + A(x, w)
           End If
           
           ' 28 Dec 2004 JWD (C0846) Add accumulation of operating expense to 3rd party and NOC
           ' Get operating expense, accumulate to gross operating expense
           rOPXTmp = A(x, w)
           CA(x, gc_nCA_GRSOPX) = CA(x, gc_nCA_GRSOPX) + rOPXTmp

           ' Add expense to respective interests
           CA(x, gc_nCA_CMPOPX) = CA(x, gc_nCA_CMPOPX) + rOPXTmp * rWinTmp * (1 - rPoxTmp)
           CA(x, gc_nCA_3DPOPX) = CA(x, gc_nCA_3DPOPX) + rOPXTmp * (1 - rWinTmp) * (1 - rPoxTmp)
           CA(x, gc_nCA_NOCOPX) = CA(x, gc_nCA_NOCOPX) + rOPXTmp * rPoxTmp
           ' End (C0846)

           rAMTtmp = A(x, w)
           gna_ACX(x, w, 0) = gna_ACX(x, w, 0) + rAMTtmp
           gna_ACX(x, w, 1) = gna_ACX(x, w, 1) + rAMTtmp
           gna_ACX(x, w, 2) = gna_ACX(x, w, 2) + rAMTtmp * rGOXtmp
           gna_ACX(x, w, 3) = gna_ACX(x, w, 3) + rAMTtmp * rCOXtmp
         
         Next w
         ' GDP 20 Jan 2003
         ' Loop bounds defined using constants
         For w = gc_nAMINADJ To gc_nAMINADJ         ' accumulate adjusting entries
           CA(x, w) = CA(x, w) + A(x, w)
         Next w

            '8/13/96 - store Inflate() & DFL()
         ' GDP 20 Jan 2003
         ' Changed to use constants modArrayConst
         CA(XI, gc_nAINF1) = Inflate(x, 1)
         CA(XI, gc_nAINF2) = Inflate(x, 2)
         CA(XI, gc_nADFL) = DFL(1)
         ' Added GDP 13/9/99 - consolidation of repayment
         CA(x, gc_nAREPAY) = CA(x, 24) + TOTREPAY(x)
         
         ' Added GDP 14/11/2000
         CA(x, gc_nAFIN) = CA(x, 25) + TOTFINANCE(x)
      
            ' 29 Dec 2004 JWD (C0846) Capture financing information
            ' 22 Apr 2005 JWD (C0873) Correction: change order of operations, replace numeral with symbol
            If (g_nFinanceEvents And gc_nFinanceEvents_FIN) < 1 Then      ' do nothing, no financing
                rFinTmp = 0
            Else
                rWinFin = 1
                rParFin = 0
                ' 22 Apr 2005 JWD (C0873) Correction: Change order of operations, make second test (ElseIf) independent (If) instead of alternate, use symbols
                If (g_nFinanceEvents And gc_nFinanceEvents_WIN) > 0 Then  ' amounts are net of win
                    rWinFin = rWinTmp
                End If
                If (g_nFinanceEvents And gc_nFinanceEvents_PAR) > 0 Then  ' amounts are net of par
                    rParFin = rParTmp
                End If
                ' Was:
                'If g_nFinanceEvents And 2 > 0 Then  ' amounts are net of win
                '    rWinFin = rWinTmp
                'ElseIf g_nFinanceEvents And 4 > 0 Then  ' amounts are net of par
                '    rParFin = rParTmp
                'End If
                ' End (C0873)
                ' Gross up the financing
                rFinTmp = TOTFINANCE(x) / rWinFin / (1 - rParFin)
            End If
        
            CA(x, gc_nCA_GRSFIN) = CA(x, gc_nCA_GRSFIN) + rFinTmp
            CA(x, gc_nCA_CMPFIN) = CA(x, gc_nCA_CMPFIN) + rFinTmp * rWinFin * (1 - rParFin)
            CA(x, gc_nCA_3DPFIN) = CA(x, gc_nCA_3DPFIN) + rFinTmp * (1 - rWinFin) * (1 - rParFin)
            CA(x, gc_nCA_NOCFIN) = CA(x, gc_nCA_NOCFIN) + rFinTmp * rParFin
            ' End (C0846)
      
      Next x

      If L10(2) = 10000 Then
        GoTo 38860
      End If
      For x = 1 To gc_nMAXLIFE
        
        '<<<<<< 17 Sep 2001 JWD (C0443)
        For w = 1 To num_columns
           CA(x, w) = CA(x, w) + D8(x, w)
        Next w
        '~~~~~~ was:
        'For w = 1 To 25    '8/13/96 was: 20 ' 13/9/99 GDP - to 24 ' GDP 14/11/2000 up to 25
        '  CA(x, w) = CA(x, w) + D8(x, w)
        'Next w
        '>>>>>> End (C0443)
        
        For w = gc_nAMINVOL To gc_nAMAXOPX
            gna_ACX(x, w, 0) = gna_ACX(x, w, 0) + D8X(x, w, 0)
            gna_ACX(x, w, 1) = gna_ACX(x, w, 1) + D8X(x, w, 1)
            gna_ACX(x, w, 2) = gna_ACX(x, w, 2) + D8X(x, w, 2)
            gna_ACX(x, w, 3) = gna_ACX(x, w, 3) + D8X(x, w, 3)
        Next w
        
      Next x

38860 If L20(6) < L10(6) Then
        L10(6) = L20(6)
      End If
      If L20(7) < L10(7) Then
        L10(7) = L20(7)
      End If
      If L20(12) < L10(12) Then
        L10(12) = L20(12)
      End If
      If L20(8) > L10(8) Then
        L10(8) = L20(8)
      End If
      If L20(9) > L10(9) Then
        L10(9) = L20(9)
      End If
      L10(2) = Int(L10(6)):     L10(4) = Int(L10(7))
      L10(11) = Int(L10(12)):   L10(5) = Int(L10(8)) - Int(L10(6)) + 1
      L10(1) = ((L10(6) - L10(2)) * 12) + 1
      L10(3) = ((L10(7) - L10(4)) * 12) + 1
      L10(10) = ((L10(12) - L10(11)) * 12) + 1

      ' Added GDP 11th January 2001
      ' This code is to set the consolidation primary stream to be Oil
      ' if it is currently gas and the next GNT file being consolidated is Oil
      If MSPM(8) = 2 And PPR = 1 Then ' Gas
         MSPM(8) = 1
      End If
      'MsgBox "GetData " & rA & " - Primary Stream Current: " & IIf(PPR = 1, "Oil", "Gas") & "    Consoldation: " & IIf(MSPM(8) = 1, "Oil", "Gas")

      'CONCATENATE CAPITAL EXPENDITURES
      If MY3T = 0 Then GoTo 39000
      AddTCC = 0
      For x = 1 To MY3T
        loopit = TCC + AddTCC  'TCC = total number of CC() so far
                
        ' 29 Dec 2004 JWD (C0846) Get the company working interest for this item
        rWinTmp = A(my3(x, gc_nMY3_XYR) - YR + 1, gc_nAWIN) / 100
        If rWinTmp = 0 Then
            rWinTmp = 1
        End If
        ' End (C0846)
        
'''        If bRVS Then
'''            rTCXtmp = my3Ex(x, 0)
'''            rOCXtmp = my3Ex(x, 1)
'''            rGCXtmp = my3Ex(x, 2)
'''            rCCXtmp = my3Ex(x, 3)
'''        Else
            rAMTtmp = my3(x, gc_nMY3_AMT)
            If my3(x, 1) = 1 Then           ' bonuses aren't added to gross-level total
                rTCXtmp = 0
            Else
                rTCXtmp = rAMTtmp
            End If
            rOCXtmp = rAMTtmp
            rGCXtmp = rAMTtmp * GPRTE(x)
            rCCXtmp = rAMTtmp * GPRTE(x) * CAPWIN(x)
'''        End If
        
        
        If loopit = 0 Then
          AddTCC = 1           'AddTCC = # of new rows added to CC()
          MYC(1, 1) = my3(x, 1)
          MYC(1, 2) = my3(x, 2)
          MYC(1, 3) = my3(x, 3)
          MYC(1, 4) = my3(x, 4)
          'Commented out GDP 18/08/99 - net of participation consolidation routine
          'MYC(1, 5) = MY3(x, 5) * (MY3(x, 6) / 100)
          If bRVS Then
             MYC(1, 5) = my3(x, 5) * (CAPWIN(x) * GPRTE(x))
          Else
             MYC(1, 5) = my3(x, 5) * (my3(x, 6) / 100)
          End If
          MYC(1, 6) = 100
          MYC(1, 7) = 0
          
'''          ' 29 Dec 2004 JWD (C0846) Add capture of company, 3d party, noc, and repaid capex
'''          MYC(1, gc_nMYC_GRS) = my3(x, gc_nMY3_AMT)         ' capture gross amount
'''          MYC(1, gc_nMYC_CMP) = my3(x, gc_nMY3_AMT) * CAPWIN(x) * GPRTE(x)          ' company amount
'''          MYC(1, gc_nMYC_3DP) = my3(x, gc_nMY3_AMT) * (1 - CAPWIN(x)) * GPRTE(x)   ' 3d party
'''          MYC(1, gc_nMYC_NOC) = my3(x, gc_nMY3_AMT) * (1 - GPRTE(x))               ' NOC amount
'''          ' Compute the amount to be repaid to/by company by/to 3d party for capital carries
'''          MYC(1, gc_nMYC_BUR) = my3(x, gc_nMY3_AMT) * (CAPWIN(x) - rWinTmp) * (my3(x, gc_nMY3_BUR) / 100) * GPRTE(x)
'''          ' End (C0846)
            
            
          MYC(1, gc_nMYC_TCX) = rTCXtmp
          MYC(1, gc_nMYC_OCX) = rOCXtmp
          MYC(1, gc_nMYC_GCX) = rGCXtmp
          MYC(1, gc_nMYC_CCX) = rCCXtmp
        
        Else
          matchem$ = "N"
          For y = 1 To loopit        ' loop through all MYC() from previous runs
                                       ' and all MY3() so far in this run
            If matchem$ = "Y" Then GoTo nexty4
            If my3(x, 1) = MYC(y, 1) And my3(x, 2) = MYC(y, 2) And my3(x, 3) = MYC(y, 3) And my3(x, 4) = MYC(y, 4) Then
                       ' found perfect match, thus sum this expenditure into matching MYC()
              'Commented out GDP 18/08/99 - net of participation consolidation routine
              'MYC(y, 5) = MYC(y, 5) + (MY3(x, 5) * (MY3(x, 6) / 100))
              If bRVS Then
                 MYC(y, 5) = MYC(y, 5) + (my3(x, 5) * (CAPWIN(x) * GPRTE(x)))
              Else
                 MYC(y, 5) = MYC(y, 5) + (my3(x, 5) * (my3(x, 6) / 100))
              End If
                
'''              ' 29 Dec 2004 JWD (C0846) Add capture of company, 3d party, noc, and repaid capex
'''              MYC(y, gc_nMYC_GRS) = MYC(y, gc_nMYC_GRS) + my3(x, gc_nMY3_AMT)      ' capture gross amount
'''              MYC(y, gc_nMYC_CMP) = MYC(y, gc_nMYC_CMP) + my3(x, gc_nMY3_AMT) * CAPWIN(x) * GPRTE(x)       ' company amount
'''              MYC(y, gc_nMYC_3DP) = MYC(y, gc_nMYC_3DP) + my3(x, gc_nMY3_AMT) * (1 - CAPWIN(x)) * GPRTE(x) ' 3d party
'''              MYC(y, gc_nMYC_NOC) = MYC(y, gc_nMYC_NOC) + my3(x, gc_nMY3_AMT) * (1 - GPRTE(x))             ' NOC amount
'''              ' Compute the amount to be repaid to/by company by/to 3d party for capital carries
'''              MYC(y, gc_nMYC_BUR) = MYC(y, gc_nMYC_BUR) + my3(x, gc_nMY3_AMT) * (CAPWIN(x) - rWinTmp) * (my3(x, gc_nMY3_BUR) / 100) * GPRTE(x)
'''              ' End (C0846)
                
              
              MYC(y, gc_nMYC_TCX) = MYC(y, gc_nMYC_TCX) + rTCXtmp
              MYC(y, gc_nMYC_OCX) = MYC(y, gc_nMYC_OCX) + rOCXtmp
              MYC(y, gc_nMYC_GCX) = MYC(y, gc_nMYC_GCX) + rGCXtmp
              MYC(y, gc_nMYC_CCX) = MYC(y, gc_nMYC_CCX) + rCCXtmp

              matchem$ = "Y"
            End If
nexty4:   Next y
          If matchem$ = "N" Then
              ' no match found, increment line counter
            AddTCC = AddTCC + 1
            MYC(AddTCC + TCC, 1) = my3(x, 1)
            MYC(AddTCC + TCC, 2) = my3(x, 2)
            MYC(AddTCC + TCC, 3) = my3(x, 3)
            MYC(AddTCC + TCC, 4) = my3(x, 4)
            'Commented out GDP 18/08/99 - net of participation consolidation routine
            'MYC(AddTCC + TCC, 5) = MY3(x, 5) * (MY3(x, 6) / 100)
            If bRVS Then
               MYC(AddTCC + TCC, 5) = my3(x, 5) * (CAPWIN(x) * GPRTE(x))
            Else
               MYC(AddTCC + TCC, 5) = my3(x, 5) * (my3(x, 6) / 100)
            End If
            MYC(AddTCC + TCC, 6) = 100
            MYC(AddTCC + TCC, 7) = 0
            
'''            ' 29 Dec 2004 JWD (C0846) Add capture of company, 3d party, noc, and repaid capex
'''            MYC(AddTCC + TCC, gc_nMYC_GRS) = my3(x, gc_nMY3_AMT)         ' capture gross amount
'''            MYC(AddTCC + TCC, gc_nMYC_CMP) = my3(x, gc_nMY3_AMT) * CAPWIN(x) * GPRTE(x)          ' company amount
'''            MYC(AddTCC + TCC, gc_nMYC_3DP) = my3(x, gc_nMY3_AMT) * (1 - CAPWIN(x)) * GPRTE(x)   ' 3d party
'''            MYC(AddTCC + TCC, gc_nMYC_NOC) = my3(x, gc_nMY3_AMT) * (1 - GPRTE(x))               ' NOC amount
'''            ' Compute the amount to be repaid to/by company by/to 3d party for capital carries
'''            MYC(AddTCC + TCC, gc_nMYC_BUR) = my3(x, gc_nMY3_AMT) * (CAPWIN(x) - rWinTmp) * (my3(x, gc_nMY3_BUR) / 100) * GPRTE(x)
'''            ' End (C0846)
            
            MYC(AddTCC + TCC, gc_nMYC_TCX) = rTCXtmp
            MYC(AddTCC + TCC, gc_nMYC_OCX) = rOCXtmp
            MYC(AddTCC + TCC, gc_nMYC_GCX) = rGCXtmp
            MYC(AddTCC + TCC, gc_nMYC_CCX) = rCCXtmp
            
          End If
        End If
      Next x

      TCC = TCC + AddTCC
      
      
         
39000 'NOW SET VARIABLES THAT DON'T CHANGE RUN TO RUN
      If rA <> 1 Then GoTo 39100
      MSPM(1) = gn(1)
      For z = 1 To 6
        MSPM(z + 1) = gn(z + 3)
      Next z
      ' Set the default primary stream of the consolidation
      If MSPM(8) = 0 Then
         MSPM(8) = PPR
      End If
        
      
      MSPM(9) = gn(2)
      MSPM(10) = DiscMthd
      For z = 1 To 4
        PMMS$(z) = PN$(z)
      Next z
39100
'MsgBox "Laving ConsolValues Sub"

err_ConsolValues:

    ''MsgBox Err.Description & ": ConsolValues"
    Err.Raise Err.number, "ConsolValues", Err.Description
    
End Sub

Rem $subtitle: 'DeleteStr'
Rem $Page:
Sub DeleteStr(s$, p%, N%)
'--------------------------------------------------------------------
  ' Delete N% characters from S$ beginning at position P%
  s$ = Left$(s$, p% - 1) + Mid$(s$, p% + N%)

End Sub

'$subtitle: 'DTAConvertABtoA'
'$Page:
Sub DTAConvertABtoA(ab(), units$())
'--------------------------------------------------------------------
'This sub takes AB(LG,20) and (using UNITS$()) fills out a(LG,20)
'AB() comes in in the units of the category as entered by the user
'A() is returnes in primary units (ie. MMB, BCF, $MM, etc.)
'---------------------------------------------------------
   Dim rScale As Single
'---------------------------------------------------------
200


  recs% = UBound(units$)
  ReDim fact#(LG)

        'CALC primary product first then do the rest
        '  (some categories are dependent on primary product being
        '  done first (ie. OIL as a ratio of GAS if GAS is primary))
  For m% = 1 To LG
    A(m%, PPR) = ab(m%, PPR)
  Next m%


        'now loop through the rest of the categories converting
        '  those that are ratios

  For i% = 1 To recs%
    If i% <> PPR Then   'don't redo primary product!
      ptr% = 0            'reset pointer in A() to factor this category against
      convert% = False    'FALSE means do not convert this category's data
      j% = i%
      ' GDP 20 Jan 2003
      ' Changed to use constants
      If i% < gc_nAMINPRC Or i% > gc_nAMAXPRC Then   'do not gosub for prices
        GoSub TestConvert
      End If
      If convert% Then
        For m% = 1 To LG
          A(m%, i%) = ab(m%, i%) * fact#(m%)
        Next m%
      Else
        For m% = 1 To LG
          A(m%, i%) = ab(m%, i%)
        Next m%
      End If
    End If
  Next i%

Exit Sub

'--------------------------------------------------------------------
TestConvert:
'This GOSUB examines the units$() for the category and sets the
'  convert% flag TRUE if the category's values in ab() need to be
'  converted (and sets ptr% to point at the category in ab() to
'  multiply the values in ab() against).
'  IE. if ab(n, 7) [OPC] is in $/B units then the values in
'  ab(n,7) must be multiplied by the values in a(n,1) [OIL BBLS]
'  to convert OPC $/Bbl into $mm.

  For m% = 1 To LG
    fact#(m%) = 1
  Next m%
        'see if this category in a ratio of another category
  ptr% = 0
  If InStr(units$(j%), "/") > 0 Then
    Select Case units$(j%)
      Case "M/B", "$/B"         'mcf/bbl, $/bbl
        ptr% = 1                'OIL MMBbl volume
        rScale = 1
      Case "B/M"                'bbl/mcf
        ptr% = 2                'GAS BCF volume
        rScale = 0.001
      Case "$/M"                '$/mcf
        ptr% = 2                'GAS BCF volume
        rScale = 1                              '.001
    End Select
  End If

        ' if units$() is a ratio (ptr% > 0) then assign correct
        ' primary category volumes to fact!() (and scale appropriately)
  If ptr% > 0 Then
    convert% = True
    For m% = 1 To LG
      fact#(m%) = A(m%, ptr%) * rScale
    Next m%
  End If

Return

End Sub

Rem $SubTitle:'DTAExponentialDecl - Calc prod using a constant % decline'
Rem $Page
Sub DTAExponentialDecl(qf, delay, qi, qdecl, column!())
'--------------------------------------------------------------------
'     Routine to calculate production using a constant percent decline.
'     Given:
'         QI     - initial production rate in units (bbls or mmcf)
'                  per period
'         QF     - final production rate in units per period
'         QDECL  - total reserves to be recovered in this decline
'         DELAY  - delay of start of production from beginning of first
'                  production period,  as a fraction of a period
'                  (0 <= DELAY < 1)
'     Return:
'         COLUMN - is the production stream (array)
'     Local Variables:
'         DR    - decline rate in percent
'         XLOG  - logarithm of decline rate
'         T     - time
'         QT    - production rate at time T
'         Q     - cumulative production through current time period.
'         PREVQ - cumulative production through previous time period.
300

        'Calculate the log of the decline rate and the decline rate...
  xlog = (qf - qi) / qdecl
  dr = Exp(xlog)
  q = 0
  t = 0 - delay     'This sets up T so that on first iteration, T = DELAY
  For i% = 1 To UBound(column!)
    If (qdecl - q) < 0.001 Then
      Exit For
    End If
    prevq = q
    qt = qi * dr ^ (t + i%)
    q = (qt - qi) / xlog
    If q > qdecl Then
      q = qdecl
    End If
    column!(i%) = q - prevq
  Next i%

End Sub

Rem $SubTitle:'DTAForecastBase'
Rem $Page
Sub DTAForecastBase(BDAData() As ParmType, Datacol!(), curveoffset, curvelife)
'--------------------------------------------------------------------
'This routine is called by DTAForecastDispatch.
'  parameters: (bdadata() AS ParmType, datacol!(), curveoffset, curvelife)
'  function: loops through bdadata(), calls DTAForecastStep,
'       and returns datacol()
7300


'fln% = FreeFile
'OPEN "FCST.LOG" FOR APPEND AS #fln%
'   PRINT #fln%, "     in forecastbase  BDAData(1).dat = "; BDAData(1).dat
'CLOSE #fln%

  Select Case BDAData(1).dat
    Case "PJY"
      startyear% = ProjYr%:     startmonth% = 1
      MonthRelative% = False
    Case "PJM"
      startyear% = ProjYr%:     startmonth% = ProjMo%
      MonthRelative% = True
    Case "PDY"
      startyear% = ProdYr%:     startmonth% = 1
      MonthRelative% = False
    Case "PDM"
      startyear% = ProdYr%:     startmonth% = ProdMo%
      MonthRelative% = True
  End Select
'fln% = FreeFile
'OPEN "FCST.LOG" FOR APPEND AS #fln%
'   PRINT #fln%, "     in forecastbase  startyear% = "; StartYear%
'CLOSE #fln%


'  StartYr% = StartYr%
  If startyear% < 50 Then       'in 21st century
    startyear% = startyear% + 100
  End If

  DTAForecastStep BDAData(), startyear%, startmonth%, Datacol!(), curveoffset, curvelife
7301

  MonthRelative% = True
7303

End Sub

'---------------------------------------------------------
' Modifications:
' 21 Nov 1996 JWD
'  -> Add check of projyr% after assignment of startperiod
'     for first record of BDAData(). If projyr% < 50 then
'     correct startperiod by subtracting 100 (StartYear%
'     is already corrected in caller). This is to correct
'     failure caused by project start > 1999. (SCO0013)
'
' 11 Jun 1998 JWD
'  -> Change calculation of Periods when forecasting entry
'     parameter 2 is LIFE on methods 4 & 6. Periods needed
'     to be incremented by 1. At sometime it was this way,
'     and then the increment was commented out without
'     explanation. This may be a problem. (SCO0046)
'---------------------------------------------------------
Sub DTAForecastStep(BDAData() As ParmType, startyear%, startmonth%, Datacol!(), curveoffset, curvelife)
'---------------------------------------------------------
   Dim i             As Integer
   Dim j             As Integer
   Dim startperiod   As Integer
   Dim ProjLife      As Integer
   Dim uD            As Integer
   Dim uW            As Integer
   Dim zeroper       As Integer
'---------------------------------------------------------
30250

'fln% = FreeFile
'OPEN "FCST.LOG" FOR APPEND AS #fln%
'   PRINT #fln%, "       in forecaststep  startyear% = "; StartYear%
'CLOSE #fln%


  HIVALUE = -32760        'null integer field
  NULVALUE = -3.4E+35     'denotes a NOT ENTERED field (NOT 0!)
  LIFE = -999: PAR = -996: LIF = -995
  PDY = -993:  PDM = -992: PJY = -991: PJM = -990: DSC = -989
  CAL = -988:  PRD = -987

   e = 2.71828
   ProjYr% = Val(Right$(LTrim$(str$(YR)), 2))        'project start year (ie. 91)
   ReDim wrkcol!(LG + 1)
30251

   For j = 1 To UBound(BDAData)
30253

'fln% = FreeFile
'OPEN "FCST.LOG" FOR APPEND AS #fln%
'   PRINT #fln%, "       in forecaststep  projyr% = "; projyr%
'CLOSE #fln%

        'figure period in which this data record starts....
      If j = 1 Then      'only calc these values for the first record
         startperiod = startyear% + 1 - ProjYr%    '9-22-95
         '<<<<<< 21 Nov 1996 JWD Add following to correct
         '           project start > 1999 failure.
         If ProjYr% < 50 Then
            startperiod = startperiod - 100
         End If
         '>>>>>>
         curveoffset = startperiod - 1     '# of project periods prior to start of this curve
      ElseIf Continuous% Then
30225
         startperiod = startperiod + Int(periods)
      Else                        'step method AND j > 1
30236
         startperiod = startperiod + UBound(wrkcol!)
      End If
30252
      ProjLife = LG

'fln% = FreeFile
'OPEN "FCST.LOG" FOR APPEND AS #fln%
'   PRINT #fln%, "       in forecaststep  startperiod = "; startperiod
'CLOSE #fln%


      Select Case BDAData(j).mtd
      Case 1                    'constant amount, # years
         Continuous% = False
         periods = ProjLife - startperiod + 1
         If BDAData(j).parm2 <= periods And BDAData(j).parm2 <> LIFE Then
            periods = BDAData(j).parm2
         Else
            If MonthRelative% Then
               AdjustLastYear% = True
            End If
         End If
         If periods > LG - startperiod + 1 Then
            periods = LG - startperiod + 1
         End If
         ReDim wrkcol!(periods)
30254
         p1$ = LTrim$(RTrim$(BDAData(j).parm1))
         If p1$ = "" Then
            begamt! = qf
         Else
30255       begamt! = Val(BDAData(j).parm1)
         End If
         
         For i = 1 To periods
30256       wrkcol!(i) = begamt!
         Next i

30258
         qf = wrkcol!(UBound(wrkcol!))

      Case 2                    'fixed amounts 1-6
30259    Continuous% = False
         periods = 6
         ReDim DUM!(periods)
14131
         DUM!(1) = Val(BDAData(j).parm1)
         DUM!(2) = BDAData(j).parm2
         DUM!(3) = BDAData(j).parm3
         DUM!(4) = BDAData(j).parm4
         DUM!(5) = BDAData(j).parm5
         DUM!(6) = BDAData(j).parm6
         For i = 6 To 1 Step -1
13132       If DUM!(i) = NULVALUE Then
               periods = periods - 1
            Else
               Exit For
            End If
         Next i
14133
         For i = 1 To periods
            If DUM!(i) = NULVALUE Then
               DUM!(i) = 0
            End If
         Next i
         If periods >= LG - startperiod + 1 Then
            periods = LG - startperiod + 1
            If MonthRelative% Then
               AdjustLastYear% = True
            End If
         End If
14134
         ReDim wrkcol!(periods)
14135
         For i = 1 To periods
            wrkcol!(i) = DUM!(i)
         Next i
         Erase DUM!
14136
         qf = wrkcol!(UBound(wrkcol!))
      Case 3, 5         'initial amount, esc(mtd 3) / decl(mtd5) rate, # years
30269    Continuous% = False
         periods = BDAData(j).parm3
         If periods = LIFE Then
            periods = LG - startperiod + 1
            If MonthRelative% Then
               AdjustLastYear% = True
            End If
         End If
         If periods > LG - startperiod + 1 Then
            periods = LG - startperiod + 1
            If MonthRelative% Then
               AdjustLastYear% = True
            End If
         End If
11234    ReDim wrkcol!(periods)
11235    p1$ = LTrim$(RTrim$(BDAData(j).parm1))
         If p1$ = "" Then
            p1$ = LTrim$(str$(qf))
         ElseIf p1$ = Space$(Len(p1$)) Then
            p1$ = LTrim$(str$(qf))
         End If
         wrkcol!(1) = Val(p1$)
         EscalRate = BDAData(j).parm2
         If BDAData(j).mtd = 5 Then
            EscalRate = EscalRate * -1
         End If

         Call DTAStepProj(EscalRate, wrkcol!())

         qf = wrkcol!(UBound(wrkcol!))

      Case 4, 6                 'escalation rate, # years
30260    Continuous% = False
         If BDAData(j).parm2 = LG Then
            periods = ProjLife - startperiod + 1
         Else
            periods = BDAData(j).parm2
         End If
         If periods = LIFE Then
            '<<<<<< 11 Jun 1998 JWD Restore increment by 1
            periods = LG - startperiod + 1
            '>>>>>>
         End If
         If periods > LG - startperiod + 1 Then
            periods = LG - startperiod + 1
         End If
         ReDim wrkcol!(periods)
         If BDAData(j).mtd = 6 Then
            EscalRate = Val(BDAData(j).parm1) * -1
         Else
            EscalRate = Val(BDAData(j).parm1)
         End If
         wrkcol!(1) = qf * (1 + EscalRate * 0.01)
         If j > 1 Then
            DTAStepProj EscalRate, wrkcol!()
         End If
         qf = wrkcol!(UBound(wrkcol!))
      Case 7, 8, 9, 0           'continuous methods
30270    Continuous% = True
         If BDAData(j).mtd = 7 Or BDAData(j).mtd = 9 Then
            d = (BDAData(j).parm2) / 100    'decline rate for this record
            dur = BDAData(j).parm3      'duration of this record
            p1$ = LTrim$(RTrim$(BDAData(j).parm1))
            If p1$ = "" Then
               p1$ = LTrim$(str$(qf))
            ElseIf p1$ = Space$(Len(p1$)) Then
               p1$ = LTrim$(str$(qf))
            End If
            qi = Val(p1$)
         ElseIf BDAData(j).mtd = 8 Or BDAData(j).mtd = 0 Then
            d = Val(BDAData(j).parm1) / 100      'decline rate for this record
            dur = BDAData(j).parm2      'duration of this record
            qi = qf
         End If
         If j = 1 Then
            delay = (startmonth% - 1) / 12
         Else
            delay = delay + periods
            Do While delay >= 1
               delay = delay - 1
            Loop
         End If
         If dur = LIFE Then
            dur = LG - startperiod        '+ 1
         End If
         If dur >= LG - startperiod + 1 Then
            periods = ProjLife - startperiod + 1
         Else
            periods = dur
         End If
         If BDAData(j).mtd = 7 Or BDAData(j).mtd = 8 Then
            d = -1 * d
         End If
         avar = -1 * Log(1 - d)  'avar = nominal decline factor [avar = -ln(1-d)]
         qf = qi * e ^ (-avar * periods)
         If periods > LG - startperiod Then
            periods = LG - startperiod
         End If
         ReDim wrkcol!(Int(periods + 1))
                        'QDECL = cum production
         qdecl = (qi / avar) * (1 - (e ^ (-1 * avar * periods)))
         DTAExponentialDecl qf, delay, qi, qdecl, wrkcol!()
      End Select
30280

      curvelife = curvelife + periods     'track total duration of this category
      zeroper = startperiod - 1

'fln% = FreeFile
'OPEN "FCST.LOG" FOR APPEND AS #fln%
'   PRINT #fln%, "       forecast finished  zeroper = "; zeroper
'CLOSE #fln%
         
      uW = UBound(wrkcol!)
      uD = UBound(Datacol!)

      If startmonth% <> 1 And Not Continuous% Then
         delay = (startmonth% - 1) / 12!
         For i = 1 To uW
            x = wrkcol!(i) * delay   'X = amount delayed to next year
            If zeroper + i <= uD Then
               Datacol!(zeroper + i) = Datacol!(zeroper + i) + (wrkcol!(i) - x)
            End If
            If zeroper + i + 1 <= uD Then
               Datacol!(zeroper + i + 1) = Datacol!(zeroper + i + 1) + x
            End If
         Next i
      Else
30281
         For i = 1 To uW
            If zeroper + i > 0 And zeroper + i <= uD Then
               Datacol!(zeroper + i) = Datacol!(zeroper + i) + wrkcol!(i)

'fln% = FreeFile
'OPEN "FCST.LOG" FOR APPEND AS #fln%
'   PRINT #fln%, "       fill in datacol i = "; i; "  datacol!("; i + zeroper; ") = "; datacol!(i + zeroper); "   wrkcol!("; i; ") = "; wrkcol!(i)
'CLOSE #fln%

            End If
         Next i
      End If
   Next j

30290

'  ERASE wrkcol!

End Sub

Rem $SubTitle:'DTAStepProj'
Rem $Page
Sub DTAStepProj(ChgRate, column!())
'--------------------------------------------------------------------
'Called by DTAForecastStep for methods 3, 4, 5, & 6


'     Routine to project values using a step escalation method.
'     Initial value should be in column!(1).
'     Values are escalated according to the escalation rate for
'     whole periods. This routine is also used for step declines.
'
'     Given:
'         ChgRate - rate of change for steps in percent per period.
'     Return:
'         COLUMN  - the projected (output) stream of values.
'--------------------------------------------------------------------
'     Local Variables:
'         FACTOR  - the escalation rate expressed as a factor.
'--------------------------------------------------------------------
' Modifications:
'          Changed formal parameter from Rate to ChgRate.  Rate is
'       function name in VB.
'--------------------------------------------------------------------
1000
  facto = 1! + (ChgRate * 0.01)

  For i% = 2 To UBound(column)
    column!(i%) = column!(i% - 1) * facto
  Next i%

End Sub

'$subtitle: 'DTAVerifyDates'
'$Page:
'
' Modifications:
' 25 Aug 1998 JWD
'  -> Change symbol name In$ to strIn$ to eliminate name
'     conflict with reserved word in VB5.
'
Sub DTAVerifyDates(strIn$, MX, YX)
'--------------------------------------------------------------------
' replaced GOSUB 47100
' THIS SUBROUTINE VERIFIES THE DATE ENTERED

      ERO% = 0
      S1% = InStr(strIn$, "/")
      If S1% = 2 Or S1% = 3 Then GoTo 47150
      GoTo 47200
47150 MX = Val(Mid$(strIn$, 1, S1% - 1))
      If MX < 1 Or MX > 12 Then GoTo 47200
      YX = Val(Mid$(strIn$, S1% + 1, 2))
      If YX < 0 Or YX > 99 Then GoTo 47200
      GoTo 47210
47200 ERO% = 1
47210 If YX >= 50 Then YX = YX + 1900
      If YX < 50 Then YX = YX + 2000




End Sub

Rem $SubTitle:'EXTNameEntered%'
Rem $Page
Function EXTNameEntered%(arg$)
'--------------------------------------------------------------------
'This function is called by ValidateLine.
'This Function examines the inbound string argument and
'returns TRUE/FALSE whether the arg$ is a (possible) file
'name
'NOTE: It does not check the disk to see if the file
' exists!
'
'this gosub checks if an external file name has been
'entered in parameter 1 on BDA, INF, or ANN
'---------------------------------------------------------
   Dim bEXTFile As Integer
   Dim i       As Integer
   Dim alpha   As String
   Dim sTmp    As String
   Dim iLArg   As Integer
'---------------------------------------------------------
   bEXTFile = False

' Commented out GDP - 21 th Dec 2000
' This was telling calling routine that first parameter is a filename
' when scientific notation number was entered as first param

'   If Len(arg$) > 0 Then
'      sTmp = UCase(arg$)
'      iLArg = Len(sTmp)
'      alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
'      For i = 1 To iLArg
'         If InStr(alpha, Mid$(sTmp, i, 1)) <> 0 Then
'            bEXTFile = True
'            Exit For
'         End If
'      Next i
'   End If

   EXTNameEntered% = bEXTFile

End Function

Rem $subtitle: 'FixLen$'
Rem $PAGE
Function FixLen$(R!, fl%)
'--------------------------------------------------------------------
'Create unpadded numeric string
'---------------------------------------------------------
   Dim dp   As Integer

  If R! < 0 Then
    wk$ = str$(R!)
  Else
    wk$ = LTrim$(str$(R!))
  End If
  If InStr(wk$, "E+") > 0 Then
    Expon% = Val(Right$(wk$, 2))
    wk$ = Left$(wk$, Len(wk$) - 4)
    wk$ = wk$ + String$(Expon%, "0")
    dp = InStr(wk$, ".")
        'Shift Decimal point right
    If dp > 0 Then
      Call DeleteStr(wk$, dp, 1)
      Call InsertChar(".", wk$, dp + Expon%)
    Else
      Call InsertChar(".", wk$, Expon% + 2)
    End If
    wk$ = Left$(wk$, fl%)
  ElseIf InStr(wk$, "E-") > 0 Then
    Expon% = Val(Right$(wk$, 2))
    wk$ = Left$(wk$, Len(wk$) - 4)
    wk$ = String$(Expon%, "0") + wk$
    dp = InStr(wk$, ".")
        'Shift Decimal point left
    If dp > 0 Then
      Call DeleteStr(wk$, dp, 1)
      Call InsertChar(".", wk$, dp - Expon%)
    End If
  End If
  FixLen$ = wk$

End Function

Rem $SubTitle:'ForecastDCL - Driver sub for Prod Decline type records'
Rem $Page
Sub ForecastDCL(pdcdata() As PDCType, Datacol!(), curveoffset, curvelife)
'--------------------------------------------------------------------
'This routine is called by DTAForecastDispatch
'---------------------------------------------------------
   Dim iErrorLine As Integer
   Dim A       As Double
'---------------------------------------------------------
23010

  Maxlife% = gc_nMAXLIFE


  HIVALUE = -32760        'null integer field
  NULVALUE = -3.4E+35     'denotes a NOT ENTERED field (NOT 0!)
  LIFE = -999: PAR = -996: LIF = -995
  PDY = -993:  PDM = -992: PJY = -991: PJM = -990: DSC = -989
  CAL = -988:  PRD = -987

  e = 2.71828

  MaxDuration = LG
  dur = 0
  iErrorLine = 0

  startyear% = ProdYr%
  If startyear% < 50 Then                       'in 21st century
    startyear% = startyear% + 100
  End If

  startmonth% = ProdMo%
  ProjLife = LG
  delay = (startmonth% - 1) / 12
  curvelife = 0         'track total duration of this category
  prevd = 0.000001

  For j% = 1 To UBound(pdcdata)
        'if delay + the fractional part of the last curve life >= 1 then
        '  add 1 to the startperiod of the next curve record to get
        '  phasing OK
    fracdur = dur - Int(dur)
    If fracdur + delay >= 1 Then
      startperiod = startperiod + 1
    End If


        'updata delay for the next record
    delay = delay + dur
    Do While delay >= 1
      delay = delay - 1
    Loop        'this will make -    0 < delay < 1

        'figure period in which this data record starts....
    If j% = 1 Then      'only calc these values for the first record
      startperiod = startyear% - ProjYr% + 1
      curveoffset = startperiod - 1   '# of project periods prior to start of this curve

    Else
      startperiod = startperiod + Int(periods)
    End If

        '3-24-93  avoid s/s out of range in projects beginning in or after 2000
    If startperiod > 100 Then
      startperiod = startperiod - 100
    End If


    MaxDuration = MaxDuration - dur            '(startperiod - 1)
        'set fact to adjust entered initial & final rates
        '(convert to annual amounts)
    Select Case pdcdata(j%).unit
        Case "DAY"
          fact = 365
        Case "MON"
          fact = 12
        Case "YRS"
          fact = 1
    End Select
30190
                'ALSO - divide by 1000 - values are MBBL-reported in MMBBL
    If pdcdata(j%).begprod <> NULVALUE Then
      pdcdata(j%).begprod = pdcdata(j%).begprod * fact / 1000
    End If
    If pdcdata(j%).endprod <> NULVALUE Then
      pdcdata(j%).endprod = pdcdata(j%).endprod * fact / 1000
    End If
'--------------------------------------------------------------------
                'set d = decline (d > 0 for DECLINE)
    If pdcdata(j%).RATE = NULVALUE Then
      d = prevd      '.0000001                              'pdcdata(j%).rate = -.0000001
    Else
      d = ((pdcdata(j%).RATE) / 100) * -1
    End If
    If d = 1 Then
      d = 0.99999
    End If
'--------------------------------------------------------------------
'load local variables with record data
                'qi = initial rate
    qi = pdcdata(j%).begprod
    If qi = NULVALUE Then
      If j% > 1 Then    'if initial amt NULVALUE, use ending amt from previous record
        qi = qf
      Else
        qi = 0
      End If
    End If
                'xn = hyperbolic exponent
    xn = pdcdata(j%).hypexp
                'qf = final rate
    qf = pdcdata(j%).endprod
    If qf = qi Then
      iErrorLine = j%
    End If
                'qdecl = final rate
    qdecl = pdcdata(j%).cumprod
                'dur = time for curve
    dur = pdcdata(j%).time

'--------------------------------------------------------------------
                'test validity of entered final, cumulative, or time value
                'AND load missing variables!
    Select Case pdcdata(j%).mtd
      Case "EXP"
8110    If qdecl <> NULVALUE Then         'cumulative entered - solve for time - see if it is within LG
          qd = qdecl          'entered value of cumulative
          qf = (qdecl * Log(1 - d)) + qi
          If qf <= 0 Then
            qf = 0.000001
          End If
          If d < 0.0001 And d > -0.0001 Then
            qf = qi
            dur = qdecl / qi
            If dur > MaxDuration Then
              qdecl = qi * MaxDuration
              dur = MaxDuration
            End If
          Else
            dur = Log(qf / qi) / Log(1 - d)
8120        If dur > MaxDuration Then      'if calc time > LG reset cumprod to max attainable
              qf = qi * ((1 - d) ^ Maxlife%)
              dur = MaxDuration
            End If
            qdecl = (qf - qi) / Log(1 - d)
            If qdecl > qd Then
              qdecl = qd
            End If
          End If
        ElseIf dur <> NULVALUE Then  'time entered - assure time <= LG
8130      If dur > MaxDuration Then
            dur = MaxDuration
          End If
          If d < 0.0001 And d > -0.0001 Then
            qf = qi
            qdecl = qi * dur
          Else
            qf = qi * ((1 - d) ^ dur)
            qdecl = (qf - qi) / Log(1 - d)
          End If
        ElseIf qf <> NULVALUE Then       'make sure that the curve will reach qf within LG
          If d < 0.0001 And d > -0.0001 Then
            qf = qi
            qdecl = 0
            dur = 0
          Else
8140        z = qf
            atime = Log(qf / qi) / Log(1 - d)
            If atime > MaxDuration Then      'if calc time > LG reset qf to max attainable
              qf = qi * ((1 - d) ^ MaxDuration)
            End If
            qdecl = (qf - qi) / Log(1 - d)
            dur = Log(qf / qi) / Log(1 - d)
          End If
        End If
      Case "HAR"        'harmonic decline curves
        If d < 0 Then
          iErrorLine = j%
        Else
          If d < 0.0001 Then
            d = 0
          End If

          A = d / (1 - d)
9110      If qdecl <> NULVALUE Then       'cumulative entered - solve for time - see if it is within LG
            If d = 0 Then
              qf = qi
              dur = qdecl / qi
              If dur > MaxDuration Then
                dur = MaxDuration
                qdecl = qi * dur
              End If
            Else
              qd = qdecl                    'entered value of cumulative
              qf = qi / Exp((qdecl * A) / qi)
              dur = ((qi / qf) - 1) / A
9120          If dur > MaxDuration Then      'if calc time > LG reset cumprod to max attainable
                dur = MaxDuration
                qf = qi / (1 + (A * dur))
              End If
              qdecl = (qi / A) * Log(qi / qf)
            End If
          ElseIf dur <> NULVALUE Then  'time entered - assure time <= LG
9130        If dur > MaxDuration Then
              dur = MaxDuration
            End If
            If d = 0 Then
              qdecl = qi * dur
            Else
              qf = qi / (1 + (A * dur))
              qdecl = (qi / A) * Log(qi / qf)
            End If
          ElseIf qf <> NULVALUE Then       'make sure that the curve will reach qf within LG
            If d = 0 Then
              qf = qi
              qdecl = 0
              dur = 0
            Else
              If qf > qi Then
                iErrorLine = j%
              End If
              qdecl = (qi / A) * Log(qi / qf)
              dur = ((qi / qf) - 1) / A
              If dur > MaxDuration Then         'if calc time > LG reset qf to max attainable
                dur = MaxDuration
                qf = qi / (1 + (A * dur))
                qdecl = (qi / A) * Log(qi / qf)
              End If
            End If
            If qf > qi Then
              iErrorLine = j%
            End If
          End If
        End If
      Case "HYP"
        If d < 0 Then
          iErrorLine = j%
        Else
          If d < 0.0001 Then
            d = 0
          End If
          A = (((1 - d) ^ -xn) - 1) / xn
10110     If qdecl <> NULVALUE Then       'cumulative entered - solve for time - see if it is within LG
            If d = 0 Then
              dur = qdecl / qi
              If dur > MaxDuration Then
                dur = MaxDuration
                qdecl = qi * dur
              End If
            Else
              qd = qdecl                    'entered value of cumulative
              qf = ((qi ^ xn) - (((qdecl * (1 - xn)) * A) / (qi ^ xn))) ^ (1 / xn)
              dur = (((qf / qi) ^ -xn) - 1) / (xn * A)
10120         If dur > MaxDuration Then                'if calc time > LG reset cumprod to max attainable
                dur = MaxDuration
                qf = qi * (1 + (xn * A * dur)) ^ (-1 / xn)
                qdecl = ((qi ^ xn) / ((1 - xn) * A)) * ((qi ^ (1 - xn)) - (qf ^ (1 - xn)))
              End If
            End If
          ElseIf dur <> NULVALUE Then  'time entered - assure time <= LG
10130       If dur > MaxDuration Then
              dur = MaxDuration
            End If
            If d = 0 Then
              qf = qi
              qdecl = qi * dur
            Else
              qf = qi * (1 + (xn * A * dur)) ^ (-1 / xn)
              qdecl = ((qi ^ xn) / ((1 - xn) * A)) * ((qi ^ (1 - xn)) - (qf ^ (1 - xn)))
            End If
          ElseIf qf <> NULVALUE Then       'make sure that the curve will reach qf within LG
            If qf > qi Then
              iErrorLine = j%
            End If
            If d = 0 Then
              qf = qi
              qdecl = 0
              dur = 0
            Else
              qdecl = ((qi ^ xn) / ((1 - xn) * A)) * (qi ^ (1 - xn) - qf ^ (1 - xn))
              dur = (((qf / qi) ^ -xn) - 1) / (xn * A)
              If dur > MaxDuration Then      'if calc time > LG reset qf to max attainable
                dur = MaxDuration
                qf = qi * (1 + (xn * A * dur)) ^ (-1 / xn)
                qdecl = ((qi ^ xn) / ((1 - xn) * A)) * ((qi ^ (1 - xn)) - (qf ^ (1 - xn)))
              End If
            End If
          End If
          If qf > qi Then
            iErrorLine = j%
          End If
        End If
    End Select
'--------------------------------------------------------------------
8150


    MaxDur% = Maxlife% - startperiod + 1
    If dur > MaxDur% Then
      periods = MaxDur%
    Else
      periods = dur
    End If

    curvelife = curvelife + periods     'track total duration of this category

    If periods + 2 > MaxDur% + 1 Then
      periods = MaxDur% - 2
    End If
    If periods + 2 > LG + 1 Then
      periods = LG - 2
    End If
    If periods < 0 Then
      periods = 0
    End If
8151
    ReDim wrkcol!(Int(periods + 2))
8152
    If iErrorLine > 0 Then
      ReDim Datacol!(1)
      Exit Sub
    End If


    Select Case pdcdata(j%).mtd
      Case "EXP"
        If d > -0.001 And d < 0.001 Then
          PlateauPDC qi, qdecl, delay, wrkcol!()      'production plateau
        Else
          DTAExponentialDecl qf, delay, qi, qdecl, wrkcol!()
        End If
      Case "HYP"
        If d > -0.001 And d < 0.001 Then
          PlateauPDC qi, qdecl, delay, wrkcol!()      'production plateau
        Else
          HyperbolicDecl qi, qf, qdecl, xn, delay, wrkcol!(), d
        End If
      Case "HAR"
        If d > -0.001 And d < 0.001 Then
          PlateauPDC qi, qdecl, delay, wrkcol!()      'production plateau
        Else
          HarmonicDecl qi, qf, qdecl, delay, wrkcol!(), d
        End If
    End Select

      '  delay = delay + (dur - INT(dur))
      '  delay = delay - INT(delay)
30209

    zeroper = startperiod - 1

    For i% = 1 To UBound(wrkcol)
      If zeroper + i% <= UBound(Datacol!) Then
        Datacol!(zeroper + i%) = Datacol!(zeroper + i%) + wrkcol(i%)
      End If
    Next i%
    prevd = d   'store previous decline (incase next record has decline blank)
  Next j%       'forecastdcl

  Erase wrkcol

End Sub

Rem $SubTitle:'DTAForecastLoadA - loads A() with values from datacol!()'
Rem $Page
Sub ForecastLoadA(item As Integer, Datacol() As Single, z() As Single)
'--------------------------------------------------------------------
'  parameters: Category$, datacol!(), z()
'  function: takes datacol() and fills out z(1-LG, item)
'  returns: ---
'---------------------------------------------------------
   Dim p As Integer
'---------------------------------------------------------
4350
   For p = 1 To LG
      z(p, item) = Datacol(p)
   Next p

End Sub

Rem $SubTitle:'ForecastSetLife - set life variables (LG, LGI, LFX, LFI)'
Rem $Page
Sub ForecastSetLife()
'--------------------------------------------------------------------
'NOTE - LFX used to be called LF (conflicted with CONSTANT LF (line feed)

'This sub sets the "life" variables LG, LGI, LFX & LFI during the
'  forecasting process (after forecasting the primary product)
4390

        'LFI - producing life (actual) (ie 23.15 years)
  LFI = curvelife
  If LFI > Maxlife% Then
    LFI = Maxlife%
  End If
        'LFX - producing life (integer) (rounded up to whole year)
  LFX = Int(LFI)
  If LFX < LFI Then
    LFX = LFX + 1
  End If
  If LFX > Maxlife% Then
    LFX = Maxlife%
  End If

        'LGI - project life (actual) (ie 27.5 years)
  LGI = LFI + (proddelay / 12)
  If LGI > Maxlife% Then
    LGI = Maxlife%
  End If

        'LG - project life (reporting years(integer))
  LG = Int(LGI)
  If LG < LGI Then
    LG = LG + 1
  ElseIf Right$(PrimaryStart$, 1) = "M" And ProjMo% > 1 Then
    LG = LG + 1
  End If
  If LG > Maxlife% Then
    LG = Maxlife%
  End If
  If ProjMo% > ProdMo% Then
    DUM = (ProjMo% - 1) / 12 + LGI
    dum2 = Int(DUM)
    If dum2 < DUM Then
      dum2 = dum2 + 1
    End If
    LG = dum2
  End If

        'if LG contains a partial year (ie 32.8875), then make sure that the
        '  fraction will "fit" in the fractional year in LG already.
        '  If not, add 1 to LG again.
        'example:  proj start 4/92   producing life 32.8875
        '  aAt this point, LG is set to 33. However, the project will run over
        '  into the 34th calendar year, so make LG = 34
  remmos = 12 - ProjMo% + 1     'remaining months in year
  fracyear = remmos / 12        'fractional year in first calendar year
  partyear = LGI - Int(LGI)     'fractional part of project life
  If fracyear < partyear Then
    LG = LG + 1
  End If
  ' Added GDP 12/12/2000
  ' in AS$ET the primary stream always starts at PJY and there is a bug in the above code where if the
  ' project start month > production start month then an extra year is added unnecessarily
  LG = LGI
  curvelife = 0

End Sub

Rem $SubTitle:'HarmonicDecl'
Rem $Page
Sub HarmonicDecl(qi, qf, qdecl, delay, column!(), d)
'--------------------------------------------------------------------
'     Routine to calculate production stream using a harmonic decline.
'     Given:
'         qi     - initial production rate in units (bbls or mmcf)
'                  per period.
'         qf     - final production rate in units per period
'         qdecl  - total reserves to be recovered in this decline
'         delay  - delay of start of production from beginning of first
'                  production period,  as a fraction of a period
'                  (0 <= DELAY < 1)
'         d      - instantaneous initial nominal decline
'     Return:
'         COLUMN!() - is the production stream (array)
'     Local Variables:
'         rA    - intermediate function of decline rate
'         t     - time
'         q     - cumulative production through current time period.
'         qprev - cumulative production through previous time period.
'         qt    - ending rate at the end of given period
'---------------------------------------------------------
   Dim rA      As Single
   Dim q       As Single
   Dim qt      As Single
   Dim qprev   As Single
   Dim t       As Single
'---------------------------------------------------------
400
      
      If qf > qi Then
        Exit Sub
      End If
      q = 0
      qprev = 0                             'cum thru end of (i%-1) years
      rA = d / (1 - d)
      t = 0 - delay     'This sets up T so that on first iteration, T = -DELAY
      For i% = 1 To UBound(column!)
        If qprev >= qdecl Then
          Exit For
        End If
        qt = qi / (1 + (rA * (i% + t)))
        q = (qi / rA) * Log(qi / qt)   'cum thru end of i% years
        If q > qdecl Then
          q = qdecl
        End If
        column!(i%) = q - qprev
        qprev = q
      Next i%

End Sub

Rem $SubTitle:'HyperbolicDecl'
Rem $Page
Sub HyperbolicDecl(qi, qf, qdecl, xn, delay, column!(), d)
'--------------------------------------------------------------------
'     Routine to calculate production stream using a hyperbolic decline.
'     Given:
'         QI     - initial production rate in units (bbls or mmcf)
'                  per period.
'         QF     - final production rate in units per period
'         QDECL  - total reserves to be recovered in this decline
'         XN     - the exponent. XN >= 0 AND XN <> 1.
'         DELAY  - delay of start of production from beginning of first
'                  production period,  as a fraction of a period
'                  (0 <= DELAY < 1)
'         D      - instantaneous initial nominal decline
'     Return:
'         COLUMN - is the production stream (array)
'     Local Variables:
                '''''         DI    - instantaneous initial nominal decline
'         T     - time
'         QT    - production rate at time T
'         Q     - cumulative production through current time period.
'         PREVQ - cumulative production through previous time period.
'---------------------------------------------------------
   Dim i       As Integer
   Dim A       As Double
   Dim prevq   As Double
   Dim q       As Double
   Dim qt      As Double
   Dim t       As Double
'---------------------------------------------------------
500
  If qf > qi Then
    Exit Sub
  ElseIf xn <= 0 Or xn >= 1 Then
    Exit Sub
  End If
                '''''  di = (qi ^ xn / ((1 - xn) * qdecl)) * (qi ^ (1 - xn) - qf ^ (1 - xn))
  q = 0
  A = (((1 - d) ^ -xn) - 1) / xn
  t = 0 - delay         'This sets up T, on first iteration T = -DELAY
  For i = 1 To UBound(column!)
    If (qdecl - q) < 0.001 Then
      Exit For
    End If
    prevq = q
    qt = qi * (1 + (xn * A * (t + i))) ^ (-1 / xn)
    q = ((qi ^ xn) / ((1 - xn) * A)) * ((qi ^ (1 - xn)) - (qt ^ (1 - xn)))
    If q > qdecl Then
      q = qdecl
    End If
    column!(i) = q - prevq
  Next i

End Sub

'$subtitle: 'InflateCAPEX'
'$Page:
'
' 14 Jun 2001 JWD
'  -> Replace explicit occurrences of the detail capital
'     expenditure category code string with the public
'     symbol. (C0332)
'
Sub InflateCAPEX()
'--------------------------------------------------------------------
'NOTE: If the capex item occurs on the month/year (MO-YR) that the
'  project begins, we do not inflate the item.

'This sub scans MY3() (capex) for each record, then takes the category
'  for the record and searches INF() (inflation) for that category.
'If the category is in INF() then we forecast the inflation and apply
'  the inflation to the capital amount (MY3(n,5)). If the category
'  is not found, we search INF() for EXP (exploration) or DEV
'  (development) to see if the user has specified an inflation for
'  the group. If EXP or DEV has not been entered, we search INF() for
'  CPX (to inflate all capital items).
600
  HIVALUE = -32760
        'forecast EXP, DEV, and CPX group inflators
  ReDim cats$(3), grpfound%(3)
  cats$(1) = "EXP":     cats$(2) = "DEV":       cats$(3) = "CPX"
  ReDim infgrp!(LG, 3)  'to store group annual amounts
  For i% = 1 To 3
    ReDim Datacol!(LG + 5)
    updatea% = -999     'signal to DTAForecastDispatch to search INF for category
    arg$ = cats$(i%)    'forecastdispatch changes value of arg$ (returns units)
    DTAForecastDispatch arg$, Datacol!(), updatea%
    If updatea% And updatea% <> HIVALUE Then
      grpfound%(i%) = True
      For j% = 1 To LG
        infgrp!(j%, i%) = Datacol!(j%)
      Next j%
    End If
  Next i%
  Erase cats$

        'now group inflation factors are stored in infgrp!() and flags
        ' grpfound%() contains T/F for each category whether found

        'load cats$() with category codes allowable in CAPEX
        '  (except CPX, EXP, DEV)
   
   '<<<<<< 14 Jun 2001 JWD
   Dim kk As Integer
   C$ = CPXCategoryCodesString
   kk = Len(C$) \ 3
   ReDim cats$(kk)
   For i% = 1 To kk
     cats$(i%) = Mid$(C$, (i% - 1) * 3 + 1, 3)
   Next i%
   C$ = ""
   '~~~~~~ was:
   'ReDim cats$(20)       '9-28-92  added BL2 & BL3      (18)
   'C$ = "BNSLSERENGEOEDHEDSADHASCDNPDVPPLFFCLTRNEORCP1CP2CP3BALBL2BL3" '20 items
   'For i% = 1 To 20
   '  cats$(i%) = Mid$(C$, (i% - 1) * 3 + 1, 3)
   'Next i%
   'C$ = ""
   '>>>>>> End 14 Jun 2001
   
        'now begin searching MY3() for categories
  For i% = 1 To MY3T
    whichgroup% = 0                     'reset group signal
    fact! = 1                           'adjustment factor of expense
    ct% = (my3(i%, 3) - YR) + 1         '# years into project of this expense
    If ct% > LG Then
      ct% = LG
    End If
    If my3(i%, 2) = mo And my3(i%, 3) = YR Then 'do not inflate
        'expenses in the first month of the project are NOT inflated
    Else                  'inflate expense by applicable factor
      ReDim Datacol!(LG + 5)
      updatea% = -999     'signal to DTAForecastDispatch to search INF for category
      ptr% = my3(i%, 1)   'CAPEX category for this line
      arg$ = cats$(ptr%)  'forecastdispatch changes value of arg$ (returns units)
      DTAForecastDispatch arg$, Datacol!(), updatea%
      If updatea% = HIVALUE Then  'not found
        Select Case cats$(ptr%)
          Case "GEO", "EDH", "EDS", "ADH", "ASC"        'EXPLORATION CATEGORIES
            whichgroup% = 1
          Case "DNP", "DVP", "PLF", "FCL", "TRN", "EOR" 'DEVELOPMENT CATEGORIES
            whichgroup% = 2
          Case Else             'cat = BNS,LSE,REN,CP1,CP2,CP3,BAL,BL2,BL3
            whichgroup% = 3     'treat as CPX
        End Select
        GoSub CalculateFactor
      Else              'category was found in INF
        For m% = 1 To ct%
          If m% <= LG Then
            fact! = fact! * (1 + (Datacol!(m%) / 100))
          End If
        Next m%
      End If
        'now multiply amount (MY3(i%,5)) by fact!
      my3(i%, 5) = my3(i%, 5) * fact!
      Erase Datacol!
    End If
  Next i%

  Erase grpfound%, infgrp!, cats$

  Exit Sub

'--------------------------------------------------------------------
'GOSUBS HERE
CalculateFactor:  'this gosub calculates the factor to apply to the expense
  If whichgroup% < 3 And grpfound%(whichgroup%) Then        'EXP or DEV inflation category found
        'if whichgroup% = 1 or 2  AND  grpfound%(whichgroup%) = TRUE then
        '  the category is an EXP or DEV item and there was anEXP or DEV
        '  record found on the inflation screen so we use it
    For m% = 1 To ct%
      fact! = fact! * (1 + (infgrp!(m%, whichgroup%) / 100))
    Next m%
  ElseIf grpfound%(3) Then    'CPX inflation category found
        'if whichgroup% = 1 or 2  AND  grpfound%(whichgroup%) = FALSE OR
        '  whichgroup% = 3 and  grpfound%(whichgroup%) = TRUE then
        '  the category is an EXP or DEV item and there was anEXP or DEV
        '  record found on the inflation screen so we use it
    For m% = 1 To ct%
      fact! = fact! * (1 + (infgrp!(m%, 3) / 100))
    Next m%
  End If

Return

End Sub

Rem $subtitle: 'InsertChar'
Rem $Page:
Sub InsertChar(Add$, s$, p%)
'--------------------------------------------------------------------
'Insert Add$ into S$ at position P%

  s$ = Left$(s$, p% - 1) + Add$ + Mid$(s$, p%)

End Sub

'$subtitle: 'PercentSens'
'$Page:
'
' 14 Jun 2001 JWD
'  -> Replace explicit occurrences of the detail capital
'     expenditure category code string with the public
'     symbol. (C0332)
' 20 Jan 2003 GDP
'  -> Added extra volumes / prices to 3 letter code strings
'
' 27 May 2003 JWD
'  -> Add codes for new adjustment forecast categories
'     AJ6-AJ0. (C0700)
'
' 12 May 2005 JWD
'  -> Add codes for new adjustment categories A11-A20.
'     (C0876)
'
' 13 May 2005 JWD
'  -> Add codes for new operating expense categories
'     OX6-O20. (C0876)
'
Sub PercentSens(Cat$, rPct As Single)
'--------------------------------------------------------------------
'This sub applies percentage sensitivities to A() array and to MY3()
'This sub is called by the module level of this program.
'The following rules apply in this sub to determine if the amounts
'  in A() and MY3() are to be adjusted by the amount supplied (rPct)
'  1. A() {costs refer to costs, production, etc. for the specified
'     category.
'     1. Costs incurred before the year of the discount date (Y3)
'        are not adjusted.
'     2. Costs incurred the year of the discount date are adjusted.
'        However, the portion of the years costs that occur during or
'        after the discount month (M3) are the only part of the costs
'        that are adjusted.
'     3  Costs that occur after the discount year are adjusted in full.
'  2. MY3() - capital expenditures.
'     1. Costs incurred prior to the month/year of the discount date
'        (M3/Y3) are not adjusted.
'     2. Costs incurred on or after the discount date are adjusted by
'        the amount specified (rPct).
700
  ptr% = 0
  bdaptr% = 0:  cpxptr% = 0
740
        'set discyr% = element in a() that represents year of discount date
  DiscYr% = Y3 - Y1 + 1
  discyrpct! = (13 - M3) / 12          'ie. July (7) = .5
  yrpct! = rPct / 100                       'decimal amt for years after disc year
760
   
   '<<<<<< 14 Jun 2001 JWD
   Dim number_of_bda_codes As Integer
   Dim number_of_cpx_codes As Integer
   ' GDP 20 Jan 2003
   ' Added codes OV3-OV0 and OP3-OP0
   'C$ = "OILGASOV1OV2RESWINOPCGPCOP1OP2OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5" '20 bda items
   ' 27 May 2003 JWD (C0700) Add AJ6-AJ0
   'C$ = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0RESWINOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0" '41 bda items
   ' 12 May 2005 JWD (C0876) Add A11-A20
   'C$ = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0RESWINOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0A11A12A13A14A15A16A17A18A19A20" '51 bda items
   ' 13 May 2005 JWD (C0877) Add OX6-O20
   C$ = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0RESWINOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0OX1OX2OX3OX4OX5OX6OX7OX8OX9OX0O11O12O13O14O15O16O17O18O19O20AJ1AJ2AJ3AJ4AJ5AJ6AJ7AJ8AJ9AJ0A11A12A13A14A15A16A17A18A19A20" '66 bda items
   
   number_of_bda_codes = Len(C$) \ 3
   
   C$ = C$ + CPXCategoryCodesString
   number_of_cpx_codes = Len(C$) \ 3 - number_of_bda_codes
   '~~~~~~ was:
   'C$ = "OILGASOV1OV2RESWINOPCGPCOP1OP2OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5" '20 bda items
   'C$ = C$ + "BNSLSERENGEOEDHEDSADHASCDNPDVPPLFFCLTRNEORCP1CP2CP3BALBL2BL3"  '20 apex items
   '>>>>>> End 14 Jun 2001
   
   C$ = C$ + "EXPDEVCPXPRDPRCOPX"      '3 group capex items
765
  For i% = 1 To Len(C$) / 3
    If Cat$ = Mid$(C$, (i% - 1) * 3 + 1, 3) Then
      ptr% = i%
      Exit For
    End If
  Next i%
  C$ = ""                       'flush c$
770
   
   '<<<<<< 14 Jun 2001 JWD
   ' Changed to accomodate varying
   ' numbers of bda & cpx codes
   If ptr% <> 0 Then
      If ptr% < number_of_bda_codes + 1 Then
         bdaptr% = ptr%
      ElseIf ptr% < number_of_bda_codes + number_of_cpx_codes + 1 Then
         cpxptr% = i% - number_of_bda_codes
      End If
   End If
   '~~~~~~ was:
   'If ptr% <> 0 Then
   '  If ptr% < 21 Then
   '    bdaptr% = ptr%
   '  ElseIf ptr% < 41 Then
   '    cpxptr% = i% - 20
   '  End If
   'End If
   '>>>>>> End 14 Jun 2001
   
783
        'if ptr% = 41 then EXP  (adjust GEO,EDH,EDS,ADH,ASC)
        'if ptr% = 42 then DEV  (adjust DNP,DVP,PLF,FCL,TRN,EOR)
        'if ptr% = 43 then CPX  (adjust EXP GROUP,DEV GROUP,BNS,LSE,REN,CP1,CP2,CP3,BAL)
        'if ptr% = 44 then PRD  (adjust OIL,GAS,OV1,OV2)
        'if ptr% = 45 then PRC  (adjust OPC,GPC,OP1,OP2)
        'if ptr% = 46 then OPX  (adjust OX1,OX2,OX3,OX4,OX5)
  If bdaptr% > 0 Then   'item is in A() - it is a Base Data item
    For i% = 1 To LG
      GoSub SetAmount
      A(i%, bdaptr%) = A(i%, bdaptr%) * mult!
    Next i%
785
  ElseIf cpxptr% > 0 Then
    For j% = 1 To MY3T
      If my3(j%, 1) = cpxptr% Then      'this is a matching record
        GoSub SetCPXAmount
        my3(j%, 5) = my3(j%, 5) * mult!
      End If
    Next j%
  ElseIf ptr% > 0 Then
      
      '<<<<<< 14 Jun 2001 JWD
      ' Changed values that ptr% compares to
      ' so as to accomodate varying numbers
      ' of codes.
      If ptr% = number_of_bda_codes + number_of_cpx_codes + 1 Then       'EXP  (adjust GEO,EDH,EDS,ADH,ASC)
         C$ = "GEOEDHEDSADHASC"
      ElseIf ptr% = number_of_bda_codes + number_of_cpx_codes + 2 Then   'DEV  (adjust DNP,DVP,PLF,FCL,TRN,EOR)
         C$ = "DNPDVPPLFFCLTRNEOR"
      ElseIf ptr% = number_of_bda_codes + number_of_cpx_codes + 3 Then   'CPX  (adjust EXP GROUP,DEV GROUP,BNS,LSE,REN,CP1,CP2,CP3,BAL)
         C$ = CPXCategoryCodesString
      ElseIf ptr% = number_of_bda_codes + number_of_cpx_codes + 4 Then   'PRD  (adjust OIL,GAS,OV1,OV2)
         ' GDP 20 Jan 2003
         ' Added OV3-OV0
         'C$ = "OILGASOV1OV2"
         C$ = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0"
      ElseIf ptr% = number_of_bda_codes + number_of_cpx_codes + 5 Then   'PRC  (adjust OPC,GPC,OP1,OP2)
         ' GDP 20 Jan 2003
         ' Added OP3-OP0
         'C$ = "OPCGPCOP1OP2"
         C$ = "OPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0"
      ElseIf ptr% = number_of_bda_codes + number_of_cpx_codes + 6 Then   'OPX  (adjust OX1,OX2,OX3,OX4,OX5)
         'C$ = "OX1OX2OX3OX4OX5"
         ' 13 May 2005 JWD (C0877) Add new opex categories
         C$ = "OX1OX2OX3OX4OX5OX6OX7OX8OX9OX0O11O12O13O14O15O16O17O18O19O20"
      End If
      
      kk = Len(C$) \ 3
      ReDim cats$(1 To kk)
      '~~~~~~ was:
      'If ptr% = 41 Then         'EXP  (adjust GEO,EDH,EDS,ADH,ASC)
      '  ReDim cats$(5)
      '  C$ = "GEOEDHEDSADHASC"
      'ElseIf ptr% = 42 Then         'DEV  (adjust DNP,DVP,PLF,FCL,TRN,EOR)
      '  ReDim cats$(6)
      '  C$ = "DNPDVPPLFFCLTRNEOR"
      'ElseIf ptr% = 43 Then         'CPX  (adjust EXP GROUP,DEV GROUP,BNS,LSE,REN,CP1,CP2,CP3,BAL)
      '  ReDim cats$(20)
      '  C$ = "GEOEDHEDSADHASCDNPDVPPLFFCLTRNEORBNSLSERENCP1CP2CP3BALBL2BL3"
      'ElseIf ptr% = 44 Then         'PRD  (adjust OIL,GAS,OV1,OV2)
      '  ReDim cats$(4)
      '  C$ = "OILGASOV1OV2"
      'ElseIf ptr% = 45 Then         'PRC  (adjust OPC,GPC,OP1,OP2)
      '  ReDim cats$(4)
      '  C$ = "OPCGPCOP1OP2"
      'ElseIf ptr% = 46 Then         'OPX  (adjust OX1,OX2,OX3,OX4,OX5)
      '  ReDim cats$(5)
      '  C$ = "OX1OX2OX3OX4OX5"
      'End If
      '>>>>>> End 14 Jun 2001
787
    GoSub FillCats
    C$ = ""
    For i% = 1 To UBound(cats$)
      PercentSens cats$(i%), rPct
    Next i%
    Erase cats$
  End If
789
  Exit Sub

'--------------------------------------------------------------------
SetAmount:      'sets mult! with the factor to multiply an amount by
790
  mult! = 1
  If i% = DiscYr% Then
    mult! = 1 + (discyrpct! * yrpct!)
  ElseIf i% > DiscYr% Then
    mult! = 1 + yrpct!
  End If
Return

SetCPXAmount:
795
  mult! = 1
  If my3(j%, 3) = DiscYr% And my3(j%, 2) >= M3 Then
    mult! = 1 + yrpct!
  ElseIf my3(j%, 3) > DiscYr% Then
    mult! = 1 + yrpct!
  End If
Return

FillCats:
798
  For i% = 1 To Len(C$) / 3
    cats$(i%) = Mid$(C$, (i% - 1) * 3 + 1, 3)
  Next i%
799
Return

End Sub

Rem $SubTitle:'PlateauPDC'
Rem $Page
Sub PlateauPDC(qi, qdecl, delay, column!())
'--------------------------------------------------------------------
'     Routine to calculate production plateau values where
'       -.1% < decline rate < .1%.
'     Given:
'         QI     - initial production rate in units (bbls or mmcf)
'                  per period
'         QDECL  - total reserves to be recovered in this decline
'         DELAY  - delay of start of production from beginning of first
'                  production period,  as a fraction of a period
'                  (0 <= DELAY < 1)
'     Return:
'         COLUMN - is the production stream (array)
'     Local Variables:
'         T     - time
'         QT    - production rate at time T
'         Q     - cumulative production through current time period.
'         PREVQ - cumulative production through previous time period.
800


  q = 0                 'cum produced thru i% periods
  qprev = 0             'cum produced thru (i% - 1) periods
  For i% = 1 To UBound(column!)
    If (qdecl - q) < 0.000001 Then
      Exit For
    End If
    q = q + qi
    If q > qdecl Then
      q = qdecl
    End If
    amt1! = qi * (1 - delay)
    amt2! = qi * delay
    If prevq + amt1! > q Then
      amt1! = q - prevq
      amt2! = 0
    End If
    If prevq + amt1! + amt2! > p Then
      amt2! = q - prevq - amt1!
    End If
    column!(i%) = column!(i%) + amt1!
    If i% + 1 <= UBound(column!) Then
      column!(i% + 1) = column!(i% + 1) + amt2!
    End If
    prevq = q
  Next i%

End Sub

Rem $subtitle: 'SaveInflation'
Rem $Page:
Sub SaveInflation(Cat$, infcol!())
'--------------------------------------------------------------------
'if inflation category = OPC or GPC, we save the annual percentage
'  inflation values in Inflate() in common.
900
  If Cat$ = "OPC" Then
    For i% = 1 To LG
      Inflate(i%, 1) = infcol!(i%)
    Next i%
  ElseIf Cat$ = "GPC" Then
    For i% = 1 To LG
      Inflate(i%, 2) = infcol!(i%)
    Next i%
  End If

End Sub

