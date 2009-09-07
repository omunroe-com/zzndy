Option Strict Off
Option Explicit On
Module GrossRpt
	'$linesize: 132
	'$title:    'GIANT v6.1 - 1996                         GROSSRPT.BAS'
	'$subtitle: 'Write Gross Report data to file'
	' Name:        GROSSRPT.BAS
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
	' 3 Mar 1995 JWD
	'          Converted module level executable code to subroutine.
	' 6 Feb 1996 JWD
	'          Changed GrossReport().
	'          Added interface declaration include file GROSSRPT.BI.
	'          Changed include file CTYIN.BAS to CTYIN1.BG.
	'          Add explicit declaration of default storage class as Single.
	'
	' 14 Jun 2001 JWD
	'  -> Changed GrossReport(). (C0333)
	'
	' 15 Jun 2001 JWD
	'  -> Changed GrossReport(). (C0333)
	'
	' 2 Aug 2001 JWD
	'  -> Changed GrossReport(). (C0363)
	'
	' 6 Aug 2001 JWD
	'  -> Changed GrossReport(). (C0373)
	'
	' 12 Jan 2004 JWD
	'  -> Changed GrossReport(). (C0772)
	'
	' 19 Jan 2004 JWD
	'  -> Changed GrossReport(). (C0774)
	'
	' 3 Feb 2004 JWD
	'  -> Changed GrossReport(). (C0774)
	'
	' 16 May 2005 JWD
	'  -> Changed GrossReport(). (C0877)
	'
	' 20 May 2005 JWD
	'  -> Changed GrossReport(). (C0878)
	'-----------------------------------------------------------------------
	'$DYNAMIC
	'UPGRADE_NOTE: DefSng A-Z statement was removed. Variables were explicitly declared as type Single. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="92AFD3E3-440D-4D49-A8BF-580D74A8C9F2"'
	'$include: 'ctyin1.bg'
	'$include: 'grossrpt.bi'
	
	'-----------------------------------------------------------------------
	'$subtitle: 'Procedure: GrossReport'
	'$page
	'
	' Modifications:
	' 6 Feb 1996 JWD
	'  -> Removed ReDim of arrays formerly in SCRA1.SCR, now in
	'     common.
	'  -> Removed include of SCRA1IN.BAS and SCRA1OUT.BAS.
	'     Data is now stored in common.
	'
	' 20 Feb 1996 JWD
	'  -> Change CUR$ to sCur, duplicate definition (CUR()).
	'
	' 14 Jun 2001 JWD
	'  -> Add additional columns to Total Capital Expenditures
	'     page for other capital items CP4-9 and Abandonment
	'     Cost ABN. (C0333)
	'  -> Add redimension of CX to additional columns for
	'     total capital expenditures page. (C0333)
	'
	' 15 Jun 2001 JWD
	'  -> Change range of items that are moved included in CX
	'     array to include new other capital items CP4-9 and
	'     abandonment ABN. (C0333)
	'
	' 2 Aug 2001 JWD
	'  -> Add additional columns to Total Capital Expenditures
	'     page for new capital items AB1 and AB2. (C0363)
	'  -> Change range of items that are included in CX array
	'     to include new capital items (abandonment) AB1 & AB2.
	'     (C0363)
	'
	' 6 Aug 2001 JWD
	'  -> Change to exclude abandonment accrual entry item AB2
	'     from Total Capital Expenditures page. AB2 should not
	'     have been output, it is a balance category. (C0373)
	'
	' 20 Jan 2003 GDP
	'  -> Changes for extra volume streams in report
	'
	' 12 Jan 2004 JWD
	'  -> Add references to CGiantReport1 object to collect
	'     report data in object rather than output directly to
	'     file. For consolidation engine development testing
	'     purposes. (C0772)
	'
	' 19 Jan 2004 JWD
	'  -> Changed report page object type from CGiantRptPageA1
	'     to interface IGiantRptPageAssignStd. (C0774)
	'
	' 3 Feb 2004 JWD
	'  -> Remove explicit writes to report file. (C0776)
	'
	' 16 May 2005 JWD
	'  -> Add new operating expense categories OX6-O20.
	'     (C0877)
	'
	' 20 May 2005 JWD
	'  -> Update capital expenditure grouping boundary values
	'     to accomodate new balance categories BL4-B20.
	'     (C0878)
	'
	Sub GrossReport()
		Dim price As Single
		'-----------------------------------------------------------------------
		Dim iCat As Short
		Dim iSST As Short
		Dim iX As Short
		Dim iY As Short
		'-----------------------------------------------------------------------
		
		' 20 Jan 2003
		' Extra columns in report for additional volumes
		Dim DNE(15) As Single
		Dim STM(30) As Single
		Dim cum(LG) As Single
		Dim CX(LG, 12) As Single
		Dim EP(LG) As Single
		Dim DV(LG) As Single
		Dim GTTL(12) As Single
		Dim GTTLT(7) As Single
		Dim REI(7) As Single
        Dim ColumnNm(36) As String
		
8795: 
		'PRINT REPORTS
		
		
		'FIRST SUM EXPLORATION CAPITAL TO APPROPRIATE YEARS
		If my3tt = 0 Then
			GoTo 10830
		End If
		
		For iX = 1 To my3tt
8796: If my3(iX, 1) < 4 Or my3(iX, 1) > 8 Then
				GoTo 10820
			End If
8797: iSST = my3(iX, 3) - YR + 1
8798: iCat = my3(iX, 1) - 3
8799: CX(iSST, iCat) = CX(iSST, iCat) + my3(iX, 5)
10820: Next iX
10830: 'PRINT GROSS INCOME SHEDULE
		If Left(RF(5), 3) = "ALL" Then
			GoTo 10845
		End If
		GoTo 20000
10845: 
		'PRINT FIELD GROSS INCOME
		' GDP 20 Jan 2003
		' Added assignments for ColumnNm(13) to Column(36)
        ColumnNm(1) = ("OILPROD")
        ColumnNm(2) = (" OILPRC")
        ColumnNm(3) = (" OILREV")
        ColumnNm(4) = ("GASPROD")
        ColumnNm(5) = (" GASPRC")
        ColumnNm(6) = (" GASREV")
        ColumnNm(7) = ("OV1PROD")
        ColumnNm(8) = (" OV1PRC")
        ColumnNm(9) = (" OV1REV")
        ColumnNm(10) = ("OV2PROD")
        ColumnNm(11) = (" OV2PRC")
        ColumnNm(12) = (" OV2REV")
        ColumnNm(13) = ("OV3PROD")
        ColumnNm(14) = (" OV3PRC")
        ColumnNm(15) = (" OV3REV")
        ColumnNm(16) = ("OV4PROD")
        ColumnNm(17) = (" OV4PRC")
        ColumnNm(18) = (" OV4REV")
        ColumnNm(19) = ("OV5PROD")
        ColumnNm(20) = (" OV5PRC")
        ColumnNm(21) = (" OV5REV")
        ColumnNm(22) = ("OV6PROD")
        ColumnNm(23) = (" OV6PRC")
        ColumnNm(24) = (" OV6REV")
        ColumnNm(25) = ("OV7PROD")
        ColumnNm(26) = (" OV7PRC")
        ColumnNm(27) = (" OV7REV")
        ColumnNm(28) = ("OV8PROD")
        ColumnNm(29) = (" OV8PRC")
        ColumnNm(30) = (" OV8REV")
        ColumnNm(31) = ("OV9PROD")
        ColumnNm(32) = (" OV9PRC")
        ColumnNm(33) = (" OV9REV")
        ColumnNm(34) = ("OV0PROD")
        ColumnNm(35) = (" OV0PRC")
        ColumnNm(36) = (" OV0REV")
		' GDP 20 Jan 2003
		' Changed to report new volumes
		'Page type, Start Year, Page counter, life of field, number of columns, page title, column length
        ''Write #5, 1, YR, 0, LG, 36, "FIELD GROSS INCOME", 10, FinalWin, FINALPARTIC, sCur
        'columns titles
        ''Write #5, ColumnNm$(1), ColumnNm$(2), ColumnNm$(3), ColumnNm$(4), ColumnNm$(5), ColumnNm$(6), _
        'ColumnNm$(7), ColumnNm$(8), ColumnNm$(9), ColumnNm$(10), ColumnNm$(11), ColumnNm$(12), _
        'ColumnNm$(13), ColumnNm$(14), ColumnNm$(15), ColumnNm$(16), ColumnNm$(17), ColumnNm$(18), _
        'ColumnNm$(19), ColumnNm$(20), ColumnNm$(21), ColumnNm$(22), ColumnNm$(23), ColumnNm$(24), _
        'ColumnNm$(25), ColumnNm$(26), ColumnNm$(27), ColumnNm$(28), ColumnNm$(29), ColumnNm$(30), _
        'ColumnNm$(31), ColumnNm$(32), ColumnNm$(33), ColumnNm$(34), ColumnNm$(35), ColumnNm$(36)


        Dim oPg1 As IGiantRptPageAssignStd
        oPg1 = g_oReport.NewStandardRptPage
        oPg1.SetPageHeader(1, YR, 0, LG, 36, "FIELD GROSS INCOME", 10, FinalWin, FINALPARTIC, sCur)
        oPg1.SetProfileHeaders(ColumnNm(1), ColumnNm(2), ColumnNm(3), ColumnNm(4), ColumnNm(5), ColumnNm(6), ColumnNm(7), ColumnNm(8), ColumnNm(9), ColumnNm(10), ColumnNm(11), ColumnNm(12), ColumnNm(13), ColumnNm(14), ColumnNm(15), ColumnNm(16), ColumnNm(17), ColumnNm(18), ColumnNm(19), ColumnNm(20), ColumnNm(21), ColumnNm(22), ColumnNm(23), ColumnNm(24), ColumnNm(25), ColumnNm(26), ColumnNm(27), ColumnNm(28), ColumnNm(29), ColumnNm(30), ColumnNm(31), ColumnNm(32), ColumnNm(33), ColumnNm(34), ColumnNm(35), ColumnNm(36))

        ' GDP 20 JAN 2003
        ' Revenues for new volumes
        ' Uses and array instead of separate variables
        For iX = 1 To LG
10846:      GTTL(1) = A(iX, gc_nAOIL) * A(iX, gc_nAOPC)
            GTTL(2) = A(iX, gc_nAGAS) * A(iX, gc_nAGPC)
            GTTL(3) = A(iX, gc_nAOV1) * A(iX, gc_nAOP1)
            GTTL(4) = A(iX, gc_nAOV2) * A(iX, gc_nAOP2)
            GTTL(5) = A(iX, gc_nAOV3) * A(iX, gc_nAOP3)
            GTTL(6) = A(iX, gc_nAOV4) * A(iX, gc_nAOP4)
            GTTL(7) = A(iX, gc_nAOV5) * A(iX, gc_nAOP5)
            GTTL(8) = A(iX, gc_nAOV6) * A(iX, gc_nAOP6)
            GTTL(9) = A(iX, gc_nAOV7) * A(iX, gc_nAOP7)
            GTTL(10) = A(iX, gc_nAOV8) * A(iX, gc_nAOP8)
            GTTL(11) = A(iX, gc_nAOV9) * A(iX, gc_nAOP9)
            GTTL(12) = A(iX, gc_nAOV0) * A(iX, gc_nAOP0)
            ''Write #5, A(iX, gc_nAOIL), A(iX, gc_nAOPC), GTTL(1), A(iX, gc_nAGAS), A(iX, gc_nAGPC), GTTL(2), _
            'A(iX, gc_nAOV1), A(iX, gc_nAOP1), GTTL(3), A(iX, gc_nAOV2), A(iX, gc_nAOP2), GTTL(4), _
            'A(iX, gc_nAOV3), A(iX, gc_nAOP3), GTTL(5), A(iX, gc_nAOV4), A(iX, gc_nAOP4), GTTL(6), _
            'A(iX, gc_nAOV5), A(iX, gc_nAOP5), GTTL(7), A(iX, gc_nAOV6), A(iX, gc_nAOP6), GTTL(8), _
            'A(iX, gc_nAOV7), A(iX, gc_nAOP7), GTTL(9), A(iX, gc_nAOV8), A(iX, gc_nAOP8), GTTL(10), _
            'A(iX, gc_nAOV9), A(iX, gc_nAOP9), GTTL(11), A(iX, gc_nAOV0), A(iX, gc_nAOP0), GTTL(12)

            oPg1.SetProfileValues(iX, A(iX, gc_nAOIL), A(iX, gc_nAOPC), GTTL(1), A(iX, gc_nAGAS), A(iX, gc_nAGPC), GTTL(2), A(iX, gc_nAOV1), A(iX, gc_nAOP1), GTTL(3), A(iX, gc_nAOV2), A(iX, gc_nAOP2), GTTL(4), A(iX, gc_nAOV3), A(iX, gc_nAOP3), GTTL(5), A(iX, gc_nAOV4), A(iX, gc_nAOP4), GTTL(6), A(iX, gc_nAOV5), A(iX, gc_nAOP5), GTTL(7), A(iX, gc_nAOV6), A(iX, gc_nAOP6), GTTL(8), A(iX, gc_nAOV7), A(iX, gc_nAOP7), GTTL(9), A(iX, gc_nAOV8), A(iX, gc_nAOP8), GTTL(10), A(iX, gc_nAOV9), A(iX, gc_nAOP9), GTTL(11), A(iX, gc_nAOV0), A(iX, gc_nAOP0), GTTL(12))

        Next iX

        'PRINT EQUIVALENT INCOME AND OPERATING EXPENSES
10847:  ColumnNm(1) = ("EQUPROD")
        ColumnNm(2) = (" EQUPRC")
        ColumnNm(3) = (" EQUREV")
        ColumnNm(4) = ("    OX1")
        ColumnNm(5) = ("    OX2")
        ColumnNm(6) = ("    OX3")
        ColumnNm(7) = ("    OX4")
        ColumnNm(8) = ("    OX5")
        ' 16 May 2005 JWD (C0877) Add new 'columns'
        'ColumnNm$(9) = "TOTOPEX"
        ColumnNm(9) = ("    OX6")
        ColumnNm(10) = ("    OX7")
        ColumnNm(11) = ("    OX8")
        ColumnNm(12) = ("    OX9")
        ColumnNm(13) = ("    OX0")
        ColumnNm(14) = ("    O11")
        ColumnNm(15) = ("    O12")
        ColumnNm(16) = ("    O13")
        ColumnNm(17) = ("    O14")
        ColumnNm(18) = ("    O15")
        ColumnNm(19) = ("    O16")
        ColumnNm(20) = ("    O17")
        ColumnNm(21) = ("    O18")
        ColumnNm(22) = ("    O19")
        ColumnNm(23) = ("    O20")
        ColumnNm(24) = ("TOTOPEX")
        ColumnNm(25) = ("EQUPRODVOL")

        ' End (C0877)

        'Page type, Start year, Page counter, life of field, number of columns, page title, column length
        ''Write #5, 2, YR, 0, LG, 9, "EQUIVALENT INCOME AND OPERATING EXPENSES", 10, FinalWin, FINALPARTIC, sCur
        'columns titles
        ''Write #5, ColumnNm$(1), ColumnNm$(2), ColumnNm$(3), ColumnNm$(4), ColumnNm$(5), ColumnNm$(6), ColumnNm$(7), ColumnNm$(8), ColumnNm$(9)

        oPg1 = g_oReport.NewStandardRptPage
        ' 16 May 2005 JWD (C0877) Change profile count argument, now 24, was 9.
        oPg1.SetPageHeader(2, YR, 0, LG, 25, "EQUIVALENT INCOME AND OPERATING EXPENSES", 10, FinalWin, FINALPARTIC, sCur)
        ' 16 May 2005 JWD (C0877) Add header names for OX6-O20
        oPg1.SetProfileHeaders(ColumnNm(25), ColumnNm(1), ColumnNm(2), ColumnNm(3), ColumnNm(4), ColumnNm(5), ColumnNm(6), ColumnNm(7), ColumnNm(8), ColumnNm(9), ColumnNm(10), ColumnNm(11), ColumnNm(12), ColumnNm(13), ColumnNm(14), ColumnNm(15), ColumnNm(16), ColumnNm(17), ColumnNm(18), ColumnNm(19), ColumnNm(20), ColumnNm(21), ColumnNm(22), ColumnNm(23), ColumnNm(24))

        Dim fEquivalencyVolProduction As Single
        Dim dEquivalencyFactor As Double

        If gn(2) <= 0 Then
            dEquivalencyFactor = 6
        Else
            dEquivalencyFactor = gn(2)
        End If


10848:  For iX = 1 To LG
            ' GDP 20 Jan 2003
            ' Changed revenue calc to use ATotalRevenues function
            ' use constants for setting primaty price
            'GTTL6 = (A(iX, 1) * A(iX, 7)) + (A(iX, 2) * A(iX, 8)) + (A(iX, 3) * A(iX, 9)) + (A(iX, 4) * A(iX, 10))
            GTTL(6) = ATotalRevenues(iX)
            If PPR = 1 Then 'OIL is primary product
                price = A(iX, gc_nAOPC)

                'fEquivalencyVolProduction = A(iX, gc_nAOIL) + (A(iX, gc_nAGAS) / dEquivalencyFactor)

            Else 'GAS is primary product
                price = A(iX, gc_nAGPC)
                'fEquivalencyVolProduction = (A(iX, gc_nAOIL) * dEquivalencyFactor) + A(iX, gc_nAGAS)
            End If


            fEquivalencyVolProduction = EquivalencyVolumeProduction(iX, True, True)


10849:      If price <> 0 Then
                GTTL(5) = GTTL(6) / price
            Else
                GTTL(5) = 0
            End If

            ' GDP 20 Jan 2003
            ' Write opex using a array constants
10850:      ''Write #5, GTTL(5), price, GTTL(6), A(iX, gc_nAOX1), A(iX, gc_nAOX2), A(iX, gc_nAOX3), A(iX, gc_nAOX4), A(iX, gc_nAOX5), OPEX(iX)
            ' 16 May 2005 JWD (C0877) Add OX6-O20
            oPg1.SetProfileValues(iX, fEquivalencyVolProduction, GTTL(5), price, GTTL(6), A(iX, gc_nAOX1), A(iX, gc_nAOX2), A(iX, gc_nAOX3), A(iX, gc_nAOX4), A(iX, gc_nAOX5), A(iX, gc_nAOX6), A(iX, gc_nAOX7), A(iX, gc_nAOX8), A(iX, gc_nAOX9), A(iX, gc_nAOX0), A(iX, gc_nAO11), A(iX, gc_nAO12), A(iX, gc_nAO13), A(iX, gc_nAO14), A(iX, gc_nAO15), A(iX, gc_nAO16), A(iX, gc_nAO17), A(iX, gc_nAO18), A(iX, gc_nAO19), A(iX, gc_nAO20), OPEX(iX))
        Next iX
10851:
        'NOW PRINT EXPLORATION CAPITAL

        ColumnNm(1) = ("    GEO")
        ColumnNm(2) = ("    EDH")
        ColumnNm(3) = ("    EDS")
        ColumnNm(4) = ("    ADH")
        ColumnNm(5) = ("    ASC")
        ColumnNm(6) = (" TOTEXP")
10852:
        'Page type, Start year, Page counter, life of field, number of columns, page title, column length
        ''Write #5, 3, YR, 0, LG, 6, "EXPLORATION CAPITAL", 10, FinalWin, FINALPARTIC, sCur
10853:  'columns titles
        ''Write #5, ColumnNm$(1), ColumnNm$(2), ColumnNm$(3), ColumnNm$(4), ColumnNm$(5), ColumnNm$(6)

        oPg1 = g_oReport.NewStandardRptPage
        oPg1.SetPageHeader(3, YR, 0, LG, 6, "EXPLORATION CAPITAL", 10, FinalWin, FINALPARTIC, sCur)
        oPg1.SetProfileHeaders(ColumnNm(1), ColumnNm(2), ColumnNm(3), ColumnNm(4), ColumnNm(5), ColumnNm(6))

        For iX = 1 To LG
            For iY = 1 To 5
10854:          CX(iX, 6) = CX(iX, 6) + CX(iX, iY)
10855:          EP(iX) = CX(iX, 6)
            Next iY
10856:      ''Write #5, CX(iX, 1), CX(iX, 2), CX(iX, 3), CX(iX, 4), CX(iX, 5), CX(iX, 6)
            oPg1.SetProfileValues(iX, CX(iX, 1), CX(iX, 2), CX(iX, 3), CX(iX, 4), CX(iX, 5), CX(iX, 6))
        Next iX
        'FIRST SUM DEVELOPMENT CAPITAL TO APPROPRIATE YEARS

        ReDim CX(LG, 12) 'mkd 3-5-91 ERASE CX
10857:  If my3tt = 0 Then
            GoTo 11740
        End If
        For iX = 1 To my3tt
10858:      If my3(iX, 1) < 9 Or my3(iX, 1) > 14 Then
                GoTo 11730
            End If
10859:      iSST = my3(iX, 3) - YR + 1
10860:      iCat = my3(iX, 1) - 8
            If iSST <= LG Then 'dont go past LG in duration!
10861:          CX(iSST, iCat) = CX(iSST, iCat) + my3(iX, 5)
            End If
11730:  Next iX
11740:  'NOW PRINT DEVELOPMENT CAPITAL
        ColumnNm(1) = ("    DNP")
        ColumnNm(2) = ("    DVP")
        ColumnNm(3) = ("    PLF")
        ColumnNm(4) = ("    FCL")
        ColumnNm(5) = ("    TRN")
        ColumnNm(6) = ("    EOR")
        ColumnNm(7) = (" TOTDEV")
        'Page type, Start year, Page counter, life of field, number of columns, page title, column length
        ''Write #5, 4, YR, 0, LG, 7, "DEVELOPMENT CAPITAL", 10, FinalWin, FINALPARTIC, sCur
        'columns titles
        ''Write #5, ColumnNm$(1), ColumnNm$(2), ColumnNm$(3), ColumnNm$(4), ColumnNm$(5), ColumnNm$(6), ColumnNm$(7)

        oPg1 = g_oReport.NewStandardRptPage
        oPg1.SetPageHeader(4, YR, 0, LG, 7, "DEVELOPMENT CAPITAL", 10, FinalWin, FINALPARTIC, sCur)
        oPg1.SetProfileHeaders(ColumnNm(1), ColumnNm(2), ColumnNm(3), ColumnNm(4), ColumnNm(5), ColumnNm(6), ColumnNm(7))

        For iX = 1 To LG
            For iY = 1 To 6
                CX(iX, 7) = CX(iX, 7) + CX(iX, iY)
                DV(iX) = CX(iX, 7)
            Next iY
            ''Write #5, CX(iX, 1), CX(iX, 2), CX(iX, 3), CX(iX, 4), CX(iX, 5), CX(iX, 6), CX(iX, 7)
            oPg1.SetProfileValues(iX, CX(iX, 1), CX(iX, 2), CX(iX, 3), CX(iX, 4), CX(iX, 5), CX(iX, 6), CX(iX, 7))
        Next iX

        'FIRST SUM OTHER CAPITAL TO APPROPRIATE YEARS

        '<<<<<< 14 Jun 2001 JWD
        Dim column_count As Short

        '<<<<<< 6 Aug 2001 JWD (C0373)
        column_count = 20
        '~~~~~~ was:
        ''<<<<<< 2 Aug 2001 JWD (C0363)
        'column_count = 21
        ''~~~~~~ was:
        ''column_count = 19
        ''>>>>>> End (C0363)
        '>>>>>> End (C0373)

        ReDim CX(LG, column_count)
        '~~~~~~ was:
        'ReDim CX(LG, 12)              'mkd 3-5-91 ERASE cx
        '>>>>>> End 14 Jun 2001

        If my3tt = 0 Then
            GoTo 12190
        End If
        For iX = 1 To my3tt

            '<<<<<< 6 Aug 2001 JWD (C0373)
            If my3(iX, 1) = CPXCategoryCode_AbandonmentAccrualEntry Then
                ' Skip this category, it is treated like a balance.
                GoTo 12120
            End If
            '>>>>>> End (C0373)

            ' 20 May 2005 JWD (C0878) Update category boundary values
            ' Ignore balance categories
            If (my3(iX, gc_nMY3_CAT) >= CPXCategoryCodeBAL) And (my3(iX, gc_nMY3_CAT) <= CPXCategoryCodeBLn) Then
                GoTo 12120
            End If

            ' Ignore Pre-expl, expl, and dev
            If my3(iX, gc_nMY3_CAT) < 15 Then
                GoTo 12120
            End If
            ' was:
            ''<<<<<< 2 Aug 2001 JWD (C0363)
            'If my3(iX, 1) < 15 Or (my3(iX, 1) > 24 And my3(iX, 1) < 28) Then
            '   GoTo 12120
            'End If
            ''~~~~~~ was:
            ''<<<<<< 15 Jun 2001 JWD
            ''If my3(iX, 1) < 15 Or my3(iX, 1) > 24 Then
            ''   GoTo 12120
            ''End If
            ''~~~~~~ was:
            ''If my3(iX, 1) < 15 Or my3(iX, 1) > 17 Then
            ''  GoTo 12120
            ''End If
            ''>>>>>> End 15 Jun 2001
            ''>>>>>> End (C0363)
            ' End (C0878)

            iSST = my3(iX, 3) - YR + 1

            ' 20 May 2005 JWD (C0878) Update category boundary values and mapped-to value
            If my3(iX, gc_nMY3_CAT) = CPXCategoryCode_AbandonmentCashExpenditure Then
                iCat = 19
            Else
                iCat = my3(iX, gc_nMY3_CAT) - 6
            End If
            ' Was:
            ''<<<<<< 2 Aug 2001 JWD (C0363)
            'If my3(iX, 1) > 27 Then
            '   iCat = my3(iX, 1) - 9
            'Else
            '   iCat = my3(iX, 1) - 6
            'End If
            ''~~~~~~ was:
            ''iCat = my3(iX, 1) - 6
            ''>>>>>> End (C0363)
            ' End (C0878)

            CX(iSST, iCat) = CX(iSST, iCat) + my3(iX, 5)
12120:  Next iX
        For iX = 1 To my3tt
            If my3(iX, 1) < 1 Or my3(iX, 1) > 3 Then
                GoTo 12180
            End If
            iSST = my3(iX, 3) - YR + 1
            iCat = my3(iX, 1) + 3
            If iCat <> 4 Then
                CX(iSST, iCat) = CX(iSST, iCat) + my3(iX, 5)
            End If
12180:  Next iX

        'Set the Other Bonus
        'Commented out GDP 27/08/99

        '      For iX = 1 To LG
        '        cx(iX, 4) = OthBon(iX)
        '      Next iX

        ' Add GDP 27/08/99
        If Not g_bPTCons Then
            For iX = 1 To LG
                CX(iX, 4) = OthBon(iX)
            Next iX
        End If

12190:  'SET OTHER VARIABLES

        ' Commented out GDP 27/8/99

        '      CX(1, 1) = SIG
        '      iSST = Y2 - YR + 1
        '      CX(iSST, 2) = DIS
        '      For iX = 1 To LG
        '         CX(iX, 3) = BONS(iX)
        '         CX(iX, 7) = EP(iX)
        '         CX(iX, 8) = DV(iX)
        '      Next iX

        ' Added GDP 27/08/99
        ' Begin
        If g_bPTCons Then
            For iX = 1 To my3tt
                If my3(iX, 1) = 1 Then
                    iSST = my3(iX, 3) - YR + 1
                    CX(iSST, 4) = CX(iSST, 4) + my3(iX, 5)
                End If
            Next iX
            For iX = 1 To LG
                ' Removed line GDP 7/6/99
                'cx(iX, 3) = BONS(iX)
                CX(iX, 7) = EP(iX)
                CX(iX, 8) = DV(iX)
            Next iX
        Else
            CX(1, 1) = SIG
            iSST = Y2 - YR + 1
            CX(iSST, 2) = DIS
            For iX = 1 To LG
                CX(iX, 3) = BONS(iX)
                CX(iX, 7) = EP(iX)
                CX(iX, 8) = DV(iX)
            Next iX
        End If
        ' End
        'NOW PRINT TOTAL CAPITAL EXPENDITURES

        '<<<<<< 14 Jun 2001 JWD
        'UPGRADE_WARNING: Lower bound of array ColumnNm was changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim ColumnNm(column_count)

        ColumnNm(1) = ("SIGBONS")
        ColumnNm(2) = ("DISBONS")
        ColumnNm(3) = ("PROBONS")
        ColumnNm(4) = ("    BNS")
        ColumnNm(5) = ("    LSE")
        ColumnNm(6) = ("    REN")
        ColumnNm(7) = (" TOTEXP")
        ColumnNm(8) = (" TOTDEV")
        ColumnNm(9) = ("    CP1")
        ColumnNm(10) = ("    CP2")
        ColumnNm(11) = ("    CP3")
        ColumnNm(12) = ("    CP4")
        ColumnNm(13) = ("    CP5")
        ColumnNm(14) = ("    CP6")
        ColumnNm(15) = ("    CP7")
        ColumnNm(16) = ("    CP8")
        ColumnNm(17) = ("    CP9")
        ColumnNm(18) = ("    ABN")

        '<<<<<< 6 Aug 2001 JWD (C0373)
        ColumnNm(19) = ("    " & CPXCategoryCodeString_AbandonmentCashExpenditure)
        ColumnNm(20) = (" TOTCAP")
        '~~~~~~ was:
        ''<<<<<< 2 Aug 2001 JWD (C0363)
        'ColumnNm$(19) = "    " & CPXCategoryCodeString_AbandonmentCashExpenditure
        'ColumnNm$(20) = "    " & CPXCategoryCodeString_AbandonmentAccrualEntry
        'ColumnNm$(21) = " TOTCAP"
        ''~~~~~~ was:
        ''ColumnNm$(19) = " TOTCAP"
        ''>>>>>> End (C0363)
        '>>>>>> End (C0373)

        'Page type, Start year, Page counter, life of field, number of columns, page title, column length
        ''Write #5, 5, YR, 0, LG, column_count, "TOTAL CAPITAL EXPENDITURES", 10, FinalWin, FINALPARTIC, sCur
        'columns titles

        '<<<<<< 6 Aug 2001 JWD (C0373)
        ''Write #5, ColumnNm$(1), ColumnNm$(2), ColumnNm$(3), ColumnNm$(4), ColumnNm$(5), ColumnNm$(6), ColumnNm$(7), ColumnNm$(8), ColumnNm$(9), ColumnNm$(10), ColumnNm$(11), ColumnNm$(12), ColumnNm$(13), ColumnNm$(14), ColumnNm$(15), ColumnNm$(16), ColumnNm$(17), ColumnNm$(18), ColumnNm$(19), ColumnNm$(20)
        '~~~~~~ was:
        ''<<<<<< 2 Aug 2001 JWD (C0363)
        'Write #5, ColumnNm$(1), ColumnNm$(2), ColumnNm$(3), ColumnNm$(4), ColumnNm$(5), ColumnNm$(6), ColumnNm$(7), ColumnNm$(8), ColumnNm$(9), ColumnNm$(10), ColumnNm$(11), ColumnNm$(12), ColumnNm$(13), ColumnNm$(14), ColumnNm$(15), ColumnNm$(16), ColumnNm$(17), ColumnNm$(18), ColumnNm$(19), ColumnNm$(20), ColumnNm$(21)
        ''~~~~~~ was:
        ''Write #5, ColumnNm$(1), ColumnNm$(2), ColumnNm$(3), ColumnNm$(4), ColumnNm$(5), ColumnNm$(6), ColumnNm$(7), ColumnNm$(8), ColumnNm$(9), ColumnNm$(10), ColumnNm$(11), ColumnNm$(12), ColumnNm$(13), ColumnNm$(14), ColumnNm$(15), ColumnNm$(16), ColumnNm$(17), ColumnNm$(18), ColumnNm$(19)
        ''>>>>>> End (C0363)
        '>>>>>> End (C0373)

        oPg1 = g_oReport.NewStandardRptPage
        oPg1.SetPageHeader(5, YR, 0, LG, column_count, "TOTAL CAPITAL EXPENDITURES", 10, FinalWin, FINALPARTIC, sCur)
        oPg1.SetProfileHeaders(ColumnNm(1), ColumnNm(2), ColumnNm(3), ColumnNm(4), ColumnNm(5), ColumnNm(6), ColumnNm(7), ColumnNm(8), ColumnNm(9), ColumnNm(10), ColumnNm(11), ColumnNm(12), ColumnNm(13), ColumnNm(14), ColumnNm(15), ColumnNm(16), ColumnNm(17), ColumnNm(18), ColumnNm(19), ColumnNm(20))

        For iX = 1 To LG
            For iY = 1 To column_count - 1
                CX(iX, column_count) = CX(iX, column_count) + CX(iX, iY)
            Next iY

            '<<<<<< 6 Aug 2001 JWD (C0373)
            ''Write #5, CX(iX, 1), CX(iX, 2), CX(iX, 3), CX(iX, 4), CX(iX, 5), CX(iX, 6), CX(iX, 7), CX(iX, 8), CX(iX, 9), CX(iX, 10), CX(iX, 11), CX(iX, 12), CX(iX, 13), CX(iX, 14), CX(iX, 15), CX(iX, 16), CX(iX, 17), CX(iX, 18), CX(iX, 19), CX(iX, 20)
            oPg1.SetProfileValues(iX, CX(iX, 1), CX(iX, 2), CX(iX, 3), CX(iX, 4), CX(iX, 5), CX(iX, 6), CX(iX, 7), CX(iX, 8), CX(iX, 9), CX(iX, 10), CX(iX, 11), CX(iX, 12), CX(iX, 13), CX(iX, 14), CX(iX, 15), CX(iX, 16), CX(iX, 17), CX(iX, 18), CX(iX, 19), CX(iX, 20))
            '~~~~~~ was:
            ''<<<<<< 2 Aug 2001 JWD (C0363)
            'Write #5, CX(iX, 1), CX(iX, 2), CX(iX, 3), CX(iX, 4), CX(iX, 5), CX(iX, 6), CX(iX, 7), CX(iX, 8), CX(iX, 9), CX(iX, 10), CX(iX, 11), CX(iX, 12), CX(iX, 13), CX(iX, 14), CX(iX, 15), CX(iX, 16), CX(iX, 17), CX(iX, 18), CX(iX, 19), CX(iX, 20), CX(iX, 21)
            ''~~~~~~ was:
            ''Write #5, CX(iX, 1), CX(iX, 2), CX(iX, 3), CX(iX, 4), CX(iX, 5), CX(iX, 6), CX(iX, 7), CX(iX, 8), CX(iX, 9), CX(iX, 10), CX(iX, 11), CX(iX, 12), CX(iX, 13), CX(iX, 14), CX(iX, 15), CX(iX, 16), CX(iX, 17), CX(iX, 18), CX(iX, 19)
            ''>>>>>> End (C0363)
            '>>>>>> End (C0373)

            ttlcap = ttlcap + CX(iX, column_count) 'total capital for OXY reports
        Next iX
		'~~~~~~ was:
		'ColumnNm$(1) = "SIGBONS"
		'ColumnNm$(2) = "DISBONS"
		'ColumnNm$(3) = "PROBONS"
		'ColumnNm$(4) = "    BNS"
		'ColumnNm$(5) = "    LSE"
		'ColumnNm$(6) = "    REN"
		'ColumnNm$(7) = " TOTEXP"
		'ColumnNm$(8) = " TOTDEV"
		'ColumnNm$(9) = "    CP1"
		'ColumnNm$(10) = "    CP2"
		'ColumnNm$(11) = "    CP3"
		'ColumnNm$(12) = " TOTCAP"
		'
		'          'Page type, Start year, Page counter, life of field, number of columns, page title, column length
		'Write #5, 5, YR, 0, LG, 12, "TOTAL CAPITAL EXPENDITURES", 10, FinalWin, FINALPARTIC, sCur
		'          'columns titles
		'Write #5, ColumnNm$(1), ColumnNm$(2), ColumnNm$(3), ColumnNm$(4), ColumnNm$(5), ColumnNm$(6), ColumnNm$(7), ColumnNm$(8), ColumnNm$(9), ColumnNm$(10), ColumnNm$(11), ColumnNm$(12)
		'For iX = 1 To LG
		'  For iY = 1 To 11
		'    CX(iX, 12) = CX(iX, 12) + CX(iX, iY)
		'  Next iY
		'  Write #5, CX(iX, 1), CX(iX, 2), CX(iX, 3), CX(iX, 4), CX(iX, 5), CX(iX, 6), CX(iX, 7), CX(iX, 8), CX(iX, 9), CX(iX, 10), CX(iX, 11), CX(iX, 12)
		'  ttlcap! = ttlcap! + CX(iX, 12)   'total capital for OXY reports
		'Next iX
		'>>>>>> End 14 Jun 2001
20000: 
		
	End Sub
End Module