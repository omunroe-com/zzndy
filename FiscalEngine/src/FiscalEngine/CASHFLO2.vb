Option Strict Off
Option Explicit On
Module CASHFLO2
    ' $linesize: 132
    ' $title:    'GIANT v6.1 - 1996                         CashFlo2.bas'
    ' $subtitle: 'Cash Flow program - MODULE #2'
    ' **********************************************************************
    ' *        COPYRIGHT - PETROCONSULTANTS, INC. - 1986, 1995, 1996       *
    ' *                       ALL RIGHTS RESERVED                          *
    ' **********************************************************************
    ' *  This program file is proprietary information of Petroconsultants, *
    ' *  Incorporated.  Unauthorized use for any purpose is prohibited.    *
    ' **********************************************************************
    '-----------------------------------------------------------------------
    ' This module contains miscellaneous subs called by CASHFLOW.BAS.
    '-----------------------------------------------------------------------
    ' Modifications:
    ' 13 Feb 1996 JWD
    '          Change common block include file from CTYIN.BAS to CTYIN1.BG.
    '          Add interface declaration include file CASHFLO2.BI.
    '          Replace explicit external subroutine declaration statement
    '       with include file GNTOXY1.BI.
    ' 19 Feb 1996 JWD
    '          Add explicit declaration of default storage class as Single.
    '
    ' 21 Apr 1997 JWD
    '   -> Changed WriteOne().
    ' 23 Nov 2000 GDP
    '   -> Changed MatchTitles so it will set the title to the argument
    '      if no match is found
    ' 20 Jan 2003 GDP
    '   -> Commented out the procedure SaveOXYSerialNo
    '
    ' 9 Feb 2004 JWD
    '   -> Changed StoreGraphData(). (C0783)
    '-----------------------------------------------------------------------
    ' $DYNAMIC
    'UPGRADE_NOTE: DefSng A-Z statement was removed. Variables were explicitly declared as type Single. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="92AFD3E3-440D-4D49-A8BF-580D74A8C9F2"'
    '$INCLUDE: 'ctyin1.bg'
    '$INCLUDE: 'cashflo2.bi'
    '$INCLUDE: 'gntoxy1.bi'

    ' $subtitle: 'CountNegatives'
    ' $Page:
    Sub CountNegatives(ByRef i As Short, ByRef TRDT As Short, ByRef ADJT As Short, ByRef USTT As Short)
        '--------------------------------------------------------------------
        'counts -,T,A,U lines in Fiscal definition and sets flags TRDT, ADJT, USTT

        If Left(TD(i, 4), 1) = "-" Then 'NEGATIVE CASH FLOW
            NEG = NEG + 1
        ElseIf Left(TD(i, 4), 1) = "T" Then  'NEGATIVE CASH FLOW
            NEG = NEG + 1
            NEGT = NEGT + 1
            TRDT = 1 'flag
        ElseIf Left(TD(i, 4), 1) = "A" Then  'NEGATIVE CASH FLOW
            NEG = NEG + 1
            NEGA = NEGA + 1
            ADJT = 1 'flag
        ElseIf Left(TD(i, 4), 1) = "U" Then  'NEGATIVE CASH FLOW
            NEG = NEG + 1
            NEGU = NEGU + 1
            USTT = 1 'flag
        End If


    End Sub

    ' $subtitle: 'MatchTitles'
    ' $Page:
    Sub MatchTitles(ByRef arg As String, ByRef TLL As String)
        Dim bDone As Single
        '--------------------------------------------------------------------
        'THIS SUBROUTINE FINDS MATCHING SHORT REPORT TITLE
        'arg$ is variable from Fiscal Def we are trying to match
        'TLL$ is matching 7 character short title from misc titles
        'screen
        '---------------------------------------------------------
        Dim i As Short
        '---------------------------------------------------------
        ' GDP 23/11/00 - make sure that the after tax cashflow report
        ' items contain some title even if it is just the 3 letter code
        'TLL$ = ""          'initialize title
        TLL = arg
        For i = 1 To TLT
            If arg = TL(i, 1) Then
                TLL = TL(i, 2) 'assign title from misc titles data
                bDone = True
                Exit For
            End If
        Next i


        TLL = Left(TLL & Space(7), 7) 'pad to 7 in length

    End Sub


    ' GDP 20 Jan 2003
    ' Commented out this routine as it is no longer called
    'Rem $subtitle: 'SaveOXYSerialNo'
    'Rem $Page:
    'Sub SaveOXYSerialNo(DashSerial$)
    ''--------------------------------------------------------------------
    ''gets and stores oxy run serial number
    ''---------------------------------------------------------
    '   Dim sfile%
    ''---------------------------------------------------------
    '        QuerySerialNo
    '        DashSerial$ = Serial$
    '        sfile% = FreeFile
    '        Open TempDir$ + "serfile~" For Output As #sfile%
    '           Write #sfile%, DashSerial$
    '        Close #sfile%
    '
    'End Sub

    ' $subtitle: 'StoreGraphData'
    ' $Page:
    Sub StoreGraphData(ByRef PVDat(,) As Single)
        '--------------------------------------------------------------------
        ' THIS SUB IS TO WRITE THE DATA THAT THE
        '   USER CAN GRAPH WITH THE RUN FILE
        '--------------------------------------------------------------------
        ' Modifications:
        ' 19 Feb 1996 JWD
        '          Changed formal parameter PV() to PVDat().  PV is an
        '       intrinsic function name in VB.
        '
        ' 9 Feb 2004 JWD
        '  -> Remove output to CHUONG.DAT. File is obsolete
        '     and unused by any application system. (C0783)
        '--------------------------------------------------------------------
        '---------------------------------------------------------

        '' 9 Feb 2004 JWD (C0783) Remove output to CHUONG.DAT
        ''        Open FChuong$ For Append As #1
        ''        For i = 1 To 6
        ''          Write #1, PVDat(9, i) 'Net cash flow
        ''        Next i
        ''        For i = 1 To 6
        ''          Write #1, PVDat(10, i) 'Payout
        ''        Next i
        ''        For i = 1 To 6
        ''          Write #1, PVDat(11, i) ' Risk return Ratio
        ''        Next i
        ''        For i = 1 To 6
        ''          Write #1, PVDat(12, i) ' Profitability Index
        ''        Next i
        ''
        ''        For i = 1 To 6                  'Government Take
        ''          If PVDat(13, i) > -0.1 And PVDat(13, i) < 0.1 Then
        ''            Write #1, 0.005
        ''          ElseIf PVDat(13, i) > 100 Then
        ''            Write #1, 101
        ''          ElseIf PVDat(13, i) < -100 Then
        ''            Write #1, -100
        ''          Else
        ''            Write #1, PVDat(13, i)
        ''          End If
        ''        Next i
        ''        Close #1

    End Sub

    ' $subtitle: 'StoreOXYEconSumm'
    ' $Page:
    Sub StoreOXYEconSumm(ByRef PVDat(,) As Single, ByRef RR As Single)
        '--------------------------------------------------------------------
        'THESE ITEMS ARE USED IN GNTOXY1.EXE
        'GTAKEnn! govt take for given discount rate % (0, 10, 15, & 20)
        'GNTNCF! (GNTNPVnn!) = net present values
        'GNTPInn! = profitibility index (cashflow / discounted investment)
        '  giant PI = (cashflow-disc investment) / discounted investment
        '  so we subtract 1 to get OXY number 
        '--------------------------------------------------------------------
        ' Modifications:
        ' 19 Feb 1996 JWD
        '          Changed formal parameter PV() to PVDat().  PV is an
        '       intrinsic function name in VB.
        '--------------------------------------------------------------------
        Dim i As Short
        '---------------------------------------------------------

        For i = 1 To 6 'search 6 discount rates looking for 0, 10, 15, & 20
            If gn(i + 3) = 0 Then
                GTake = PVDat(13, i)
                GntNCF = PVDat(9, i)
                GntPI = PVDat(12, i) - 1
                GntPayot = PVDat(10, 1) 'payout at 0% discounting
            ElseIf gn(i + 3) = 10 Then
                GTake10 = PVDat(13, i)
                GntNPV10 = PVDat(9, i)
                GntPI10 = PVDat(12, i) - 1
            ElseIf gn(i + 3) = 15 Then
                GTake15 = PVDat(13, i)
                GntNPV15 = PVDat(9, i)
                GntPI15 = PVDat(12, i) - 1
            ElseIf gn(i + 3) = 20 Then
                GTake20 = PVDat(13, i)
                GntNPV20 = PVDat(9, i)
                GntPI20 = PVDat(12, i) - 1
            End If
        Next i
        GntROR = RR 'Company DCF Rate of Return

    End Sub

    ' $subtitle: 'TotalCFArrays'
    ' $Page:
    Sub TotalCFArrays()
        Dim O As Single
        '--------------------------------------------------------------------
        'This totals the PSCF,NGCF,NGCFT,NGCFA,NGCFU arrays (by row and column)
        '---------------------------------------------------------
        Dim x As Short
        Dim y As Short
        Dim iCFT As Short
        '---------------------------------------------------------

        'SUM POSITIVE CASH FLOWS
        iCFT = PS + 1
        If PS <> 0 Then '6440  IF PS% = 0 GOTO 6530
            For y = 1 To PS
                PSCF(0, y) = 0
                For x = 1 To LG
                    PSCF(x, iCFT) = PSCF(x, iCFT) + PSCF(x, y)
                    PSCF(0, y) = PSCF(O, y) + PSCF(x, y)
                Next x
            Next y
            PSCF(0, iCFT) = 0

            For x = 1 To LG
                PSCF(O, iCFT) = PSCF(0, iCFT) + PSCF(x, iCFT)
            Next x

        ElseIf PS = 0 Then
            For x = 1 To LG 'you are here if ps% = 0 (from line 6440)
                PSCF(x, iCFT) = 0
            Next x

        End If

        'SUM NEGATIVE CASH FLOWS
        ''      IF NEG% = 0 AND NEGT% = 0 AND NEGA% = 0 AND NEGU% = 0 GOTO 6650
        If NEG > 0 Or NEGT > 0 Or NEGA > 0 Or NEGU > 0 Then
            iCFT = NEG + 1
            NGCF(0, iCFT) = 0
            For y = 1 To NEG
                NGCF(0, y) = 0
                For x = 1 To LG
                    NGCF(x, iCFT) = NGCF(x, iCFT) + NGCF(x, y) 'total "-" column
                    NGCF(0, y) = NGCF(0, y) + NGCF(x, y) 'total line this column
                    NGCF(0, iCFT) = NGCF(0, iCFT) + NGCF(x, y) 'total line of total "-" column
                Next x
            Next y

            iCFT = NEGT + 1
            NGCFT(0, iCFT) = 0
            For y = 1 To NEGT 'sum negative "T" items
                NGCFT(0, y) = 0
                For x = 1 To LG
                    NGCFT(x, iCFT) = NGCFT(x, iCFT) + NGCFT(x, y)
                    NGCFT(0, y) = NGCFT(0, y) + NGCFT(x, y)
                    NGCFT(0, iCFT) = NGCFT(0, iCFT) + NGCFT(x, y)
                Next x
            Next y

            iCFT = NEGA + 1
            NGCFA(0, iCFT) = 0
            For y = 1 To NEGA 'sum negative "A" items
                NGCFA(0, y) = 0
                For x = 1 To LG
                    NGCFA(x, iCFT) = NGCFA(x, iCFT) + NGCFA(x, y)
                    NGCFA(0, y) = NGCFA(0, y) + NGCFA(x, y)
                    NGCFA(0, iCFT) = NGCFA(0, iCFT) + NGCFA(x, y)
                Next x
            Next y

            iCFT = NEGU + 1
            NGCFU(0, iCFT) = 0
            For y = 1 To NEGU 'sum negative "U" items
                NGCFU(0, y) = 0
                For x = 1 To LG
                    NGCFU(x, iCFT) = NGCFU(x, iCFT) + NGCFU(x, y)
                    NGCFU(0, y) = NGCFU(0, y) + NGCFU(x, y)
                    NGCFU(0, iCFT) = NGCFU(0, iCFT) + NGCFU(x, y)
                Next x
            Next y

        Else

            iCFT = 1
            For x = 1 To LG
                NGCF(x, iCFT) = 0
                NGCFT(x, iCFT) = 0
                NGCFA(x, iCFT) = 0
                NGCFU(x, iCFT) = 0
            Next x

        End If
   
    End Sub

End Module