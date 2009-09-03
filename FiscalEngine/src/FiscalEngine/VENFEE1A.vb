Option Strict Off
Option Explicit On
Module VENFEE1A
	' Name:        VENFEE1A.BAS
	' Function:    Calculate Venezuela Service Fee
	'---------------------------------------------------------
	' ********************************************************
	' *            COPYRIGHT - PETROCONSULTANTS, INC.        *
	' *                         1997                         *
	' *                  ALL RIGHTS RESERVED                 *
	' ********************************************************
	' *  This program file is proprietary information of     *
	' *  Petroconsultants, Incorporated.  Unauthorized use   *
	' *  for any purpose is prohibited.                      *
	' ********************************************************
	'---------------------------------------------------------
	'Modifications:
	' 20 Jan 2003 GDP
	'  -> Changed QtrConvert()
	'
	' 12 Jan 2004 JWD
	'  -> Changed ServiceFee(). (C0772)
	'
	' 19 Jan 2004 JWD
	'  -> Changed ServiceFee(). (C0774)
	'
	' 3 Feb 2004 JWD
	'  -> Changed ServiceFee(). (C0776)
	'
	'UPGRADE_NOTE: DefSng A-Z statement was removed. Variables were explicitly declared as type Single. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="92AFD3E3-440D-4D49-A8BF-580D74A8C9F2"'
	
	
	
	'=========================================================
    Sub AnnConvert(ByRef mo As Single, ByRef qtrlg As Single, ByRef LG As Short, ByRef BVen() As Single, ByRef IncrFee() As Single)
        '---------------------------------------------------------
        Dim izq As Single
        Dim izy As Single
        Dim qtrindex As Single
        Dim qtrstart As Single
        '---------------------------------------------------------
        'Debug.Print "beginning annconvert"
        'Debug.Print "lg = "; lg
        'Debug.Print "qtrlg = "; qtrlg


        ' now convert quarterly BVen() to annual IncrFee()

        For izy = 1 To LG

            If izy = 1 Then
                ' qtrstart is index in BVen() which begins the second year

                If mo = 1 Or mo = 2 Or mo = 3 Then
                    IncrFee(1) = BVen(1) + BVen(2) + BVen(3) + BVen(4)
                    qtrstart = 5

                ElseIf mo = 4 Or mo = 5 Or mo = 6 Then
                    IncrFee(1) = BVen(1) + BVen(2) + BVen(3)
                    qtrstart = 4

                ElseIf mo = 7 Or mo = 8 Or mo = 9 Then
                    IncrFee(1) = BVen(1) + BVen(2)
                    qtrstart = 3

                ElseIf mo = 10 Or mo = 11 Or mo = 12 Then
                    IncrFee(1) = BVen(1)
                    qtrstart = 2

                End If

            Else

                For izq = 1 To 4
                    qtrindex = ((izy - 2) * 4) + qtrstart - 1 + izq
                    IncrFee(izy) = IncrFee(izy) + BVen(qtrindex)
                Next izq

            End If

        Next izy



    End Sub
	
	
	'Modifications:
	' GDP 20 Jan 2003
	' Changed A() array index from numeric to constant
	'=========================================================
    Sub QtrConvert(ByRef A(,) As Single, ByRef TD(,) As String, ByRef RVN(,) As Single, ByRef my3(,) As Single, ByRef TDT As Short, ByRef my3tt As Short, ByRef LG As Short, ByRef mo As Single, ByRef YR As Single, ByRef gn() As Single, ByRef NHV() As Single, ByRef BaseOpex() As Single, ByRef IncrOpex() As Single, ByRef Capital() As Single, ByRef FeeDeflator() As Single, ByRef qtrlg As Single)
        '---------------------------------------------------------
        Dim AnnDflIncr As Single 'added 4/16/97
        Dim adjqtr As Single
        Dim BegMth As Single
        Dim BOCIndex As Single
        Dim DFLEntry As Single
        Dim DFLIndex As Single
        Dim EndMth As Single
        Dim FirstIncrMo As Single
        Dim FirstIncrYr As Single
        Dim InOpMonth() As Single
        Dim IOCIndex As Single
        Dim iZ As Single
        Dim izm As Single
        Dim izq As Single
        Dim izy As Single
        Dim lstqtr As Single 'added 4/17/97
        Dim monthlg As Single
        Dim mth As Single
        Dim NHVIndex As Single
        Dim NHVmonth() As Single
        Dim qtrcap As Single
        Dim qtrindex As Single
        Dim qtrstart As Single
        Dim RemMth As Single
        '---------------------------------------------------------


        qtrlg = LG * 4 ' maximum quarterly life before ajustment for Project Start Month

        ReDim NHV(qtrlg)
        ReDim BaseOpex(qtrlg)
        ReDim IncrOpex(qtrlg)
        ReDim Capital(qtrlg)
        ReDim FeeDeflator(qtrlg)

        'convert all standard annual variables into quarters for the service fee calculation

        ' find index in RVN() for each of the following variables: NHV, BOC, IOC and DFL
        NHVIndex = 0 : BOCIndex = 0 : IOCIndex = 0 : DFLIndex = 0

        For izy = 1 To TDT
            If Left(TD(izy, 1), 3) = "NHV" Then NHVIndex = izy
            If Left(TD(izy, 1), 3) = "BOC" Then BOCIndex = izy
            If Left(TD(izy, 1), 3) = "IOC" Then IOCIndex = izy
            If Left(TD(izy, 1), 3) = "DFL" Then DFLIndex = izy
        Next izy

        'now set default for FeeDeflator if no entry in any year
        DFLEntry = 0
        For izy = 1 To LG
            If RVN(izy, DFLIndex) <> 0 Then DFLEntry = 1
        Next izy
        If DFLEntry = 0 Then 'this means there was no input for DFL
            For izy = 1 To LG
                RVN(izy, DFLIndex) = 1
            Next izy
        End If

        '============================================
        ' this is beginning of new deflator code 4/16/97

        For izy = 1 To LG
            If izy = 1 Then
                'now adjust for project start month
                If mo = 1 Or mo = 2 Or mo = 3 Then
                    FeeDeflator(1) = RVN(1, DFLIndex) ^ (1 / 4)
                    FeeDeflator(2) = RVN(1, DFLIndex) ^ ((1 / 4) * 2)
                    FeeDeflator(3) = RVN(1, DFLIndex) ^ ((1 / 4) * 3)
                    FeeDeflator(4) = RVN(1, DFLIndex) ^ ((1 / 4) * 4)
                    lstqtr = 4 'lstqtr = last quarter defined in year 1
                    adjqtr = 0 'adjqtr = number of months that qtrlg is
                    'adjusted if project start month is not 1

                ElseIf mo = 4 Or mo = 5 Or mo = 6 Then
                    FeeDeflator(1) = RVN(1, DFLIndex) ^ (1 / 3)
                    FeeDeflator(2) = RVN(1, DFLIndex) ^ ((1 / 3) * 2)
                    FeeDeflator(3) = RVN(1, DFLIndex) ^ ((1 / 3) * 3)
                    lstqtr = 3
                    adjqtr = 1



                ElseIf mo = 7 Or mo = 8 Or mo = 9 Then
                    FeeDeflator(1) = RVN(1, DFLIndex) ^ (1 / 2)
                    FeeDeflator(2) = RVN(1, DFLIndex) ^ ((1 / 2) * 2)
                    lstqtr = 2
                    adjqtr = 2

                ElseIf mo = 10 Or mo = 11 Or mo = 12 Then
                    FeeDeflator(1) = RVN(1, DFLIndex)
                    lstqtr = 1
                    adjqtr = 3
                End If

            ElseIf izy = LG Then
                'now adjust for project start month
                If mo = 1 Or mo = 2 Or mo = 3 Then
                    qtrindex = (izy - 1) * 4
                Else
                    qtrindex = ((izy - 2) * 4) + lstqtr
                End If
                AnnDflIncr = (RVN(izy, DFLIndex) / RVN(izy - 1, DFLIndex)) 'calculate ratio of this years deflator to last years
                FeeDeflator(qtrindex + 1) = RVN(izy - 1, DFLIndex) * (AnnDflIncr ^ (0.25 * 1))
                FeeDeflator(qtrindex + 2) = RVN(izy - 1, DFLIndex) * (AnnDflIncr ^ (0.25 * 2))
                FeeDeflator(qtrindex + 3) = RVN(izy - 1, DFLIndex) * (AnnDflIncr ^ (0.25 * 3))
                FeeDeflator(qtrindex + 4) = RVN(izy - 1, DFLIndex) * (AnnDflIncr ^ (0.25 * 4))


                'ElseIf mo = 4 Or mo = 5 Or mo = 6 Then
                '  qtrindex = ((izy - 2) * 4) + lstqtr
                ' Debug.Print "izy = "; izy; "  lstqtr = "; lstqtr; "  qtrindex = "; qtrindex
                ' AnnDflIncr = (RVN(izy, DFLIndex) / RVN(izy - 1, DFLIndex)) 'calculate ratio of this years deflator to last years
                ' FeeDeflator(qtrindex + 1) = RVN(izy - 1, DFLIndex) * (AnnDflIncr ^ ((1 / 3) * 1))
                ' FeeDeflator(qtrindex + 2) = RVN(izy - 1, DFLIndex) * (AnnDflIncr ^ ((1 / 3) * 2))
                ' FeeDeflator(qtrindex + 3) = RVN(izy - 1, DFLIndex) * (AnnDflIncr ^ ((1 / 3) * 3))

                ' ElseIf mo = 7 Or mo = 8 Or mo = 9 Then
                '   qtrindex = ((izy - 2) * 4) + lstqtr
                '  AnnDflIncr = (RVN(izy, DFLIndex) / RVN(izy - 1, DFLIndex)) 'calculate ratio of this years deflator to last years
                ' FeeDeflator(qtrindex + 1) = RVN(izy - 1, DFLIndex) * (AnnDflIncr ^ ((1 / 2) * 1))
                'FeeDeflator(qtrindex + 2) = RVN(izy - 1, DFLIndex) * (AnnDflIncr ^ ((1 / 2) * 2))

                ' ElseIf mo = 10 Or mo = 11 Or mo = 12 Then
                '   qtrindex = ((izy - 2) * 4) + lstqtr
                '  AnnDflIncr = (RVN(izy, DFLIndex) / RVN(izy - 1, DFLIndex)) 'calculate ratio of this years deflator to last years
                ' FeeDeflator(qtrindex + 1) = RVN(izy - 1, DFLIndex)

                'End If

            Else

                qtrindex = ((izy - 2) * 4) + lstqtr
                'debug.Print "izy = "; izy; "  lstqtr = "; lstqtr; "  qtrindex = "; qtrindex
                AnnDflIncr = (RVN(izy, DFLIndex) / RVN(izy - 1, DFLIndex)) 'calculate ratio of this years deflator to last years
                FeeDeflator(qtrindex + 1) = RVN(izy - 1, DFLIndex) * (AnnDflIncr ^ (0.25 * 1))
                FeeDeflator(qtrindex + 2) = RVN(izy - 1, DFLIndex) * (AnnDflIncr ^ (0.25 * 2))
                FeeDeflator(qtrindex + 3) = RVN(izy - 1, DFLIndex) * (AnnDflIncr ^ (0.25 * 3))
                FeeDeflator(qtrindex + 4) = RVN(izy - 1, DFLIndex) * (AnnDflIncr ^ (0.25 * 4))

            End If

        Next izy

        'For izq = 1 To qtrlg
        '  Debug.Print "izq = "; izq; "  FeeDeflator(izq) = "; FeeDeflator(izq)
        'Next izq
        'Debug.Print ""


        '==================== this is end of new deflator code


        '========= this is beginning of old deflator code ============
        'convert annual deflator input to quarters
        'first convert to calendar quarters
        'For izy = 1 To lg

        '   qtrindex = (izy - 1) * 4
        '   FeeDeflator(qtrindex + 1) = RVN(izy, DFLIndex) ^ .25
        '   FeeDeflator(qtrindex + 2) = RVN(izy, DFLIndex) ^ .25
        '   FeeDeflator(qtrindex + 3) = RVN(izy, DFLIndex) ^ .25
        '   FeeDeflator(qtrindex + 4) = RVN(izy, DFLIndex) ^ .25

        'Next izy

        'Debug.Print "before shift"
        'For izq = 1 To qtrlg
        '   Debug.Print "izq = "; izq; "  FeeDeflator(izq) = "; FeeDeflator(izq)
        'Next izq
        'Debug.Print ""

        'now adjust for project start month
        'adjqtr = 0         ' number of quarters that quarterly life is adjusted
        ' if project start month is not "1"

        'If mo = 4 Or mo = 5 Or mo = 6 Then adjqtr = 1
        'If mo = 7 Or mo = 8 Or mo = 9 Then adjqtr = 2
        'If mo = 10 Or mo = 11 Or mo = 12 Then adjqtr = 3

        'If adjqtr = 1 Or adjqtr = 2 Or adjqtr = 3 Then

        '  For izq = 1 To qtrlg

        '    If (izq + adjqtr) <= qtrlg Then
        '      FeeDeflator(izq) = FeeDeflator(izq + adjqtr)
        '  Else
        '    FeeDeflator(izq) = 0
        'End If

        '   Next izq

        'End If

        'Debug.Print "after shift"
        'For izq = 1 To qtrlg
        '   Debug.Print "izq = "; izq; "  FeeDeflator(izq) = "; FeeDeflator(izq)
        'Next izq
        'Debug.Print ""

        '===================this is end of old deflator code =============
        ' divide NHV and IncrOpex into months equally

        monthlg = LG * 12
        ReDim NHVmonth(monthlg)
        ReDim InOpMonth(monthlg)

        For iZ = 1 To LG

            For izm = 1 To 12

                mth = ((iZ - 1) * 12) + izm
                NHVmonth(mth) = RVN(iZ, NHVIndex) / 12
                InOpMonth(mth) = RVN(iZ, IOCIndex) / 12

            Next izm

        Next iZ
        'Debug.Print ""
        'For izm = 1 To 24
        'Debug.Print "izm = "; izm; "  NHVmonth(izm) = "; NHVmonth(izm)
        'Debug.Print "izm = "; izm; "  InOpMonth(izm) = "; InOpMonth(izm)
        'Next izm
        'Debug.Print ""

        ' adjust for Project Start Month, which must be in the first calendar year
        ' reallocate NHV and IncrOpex over remaining months


        'zero out all months before mo
        BegMth = 1
        EndMth = mo - 1

        If EndMth > 0 Then

            For izm = BegMth To EndMth

                NHVmonth(izm) = 0
                InOpMonth(izm) = 0

            Next izm



            ' now divide the annual amounts over the remaining life

            BegMth = EndMth + 1
            EndMth = 12
            RemMth = EndMth - BegMth + 1

            For izm = BegMth To EndMth

                NHVmonth(izm) = RVN(1, NHVIndex) / RemMth
                InOpMonth(izm) = RVN(1, IOCIndex) / RemMth

            Next izm

        End If

        'For izm = 1 To 24
        'Debug.Print "izm = "; izm; "  NHVmonth(izm) = "; NHVmonth(izm)
        'Debug.Print "izm = "; izm; "  InOpMonth(izm) = "; InOpMonth(izm)
        'Next izm
        'Debug.Print ""

        ' now determine first year of incremental production

        FirstIncrYr = 0

        For izy = 1 To LG
            If RVN(izy, NHVIndex) > 0 Then
                FirstIncrYr = izy
                Exit For
            End If
        Next izy


        ' now allocate NHV and IncrOpex to remaining months after production start month
        ' note that we are using the water depth input as the month of first incremental production
        ' we already know the year from the data file forecast.

        ' 20 Jan 2003
        ' Changed A(1, 17) to A(1, gc_nAAJ2)
        If A(1, gc_nAAJ2) <= 0 Or A(1, gc_nAAJ2) > 12 Then
            FirstIncrMo = 1
        Else
            FirstIncrMo = A(1, gc_nAAJ2)
        End If

        'make sure that the production start month does not exceed the project start month in first year

        If FirstIncrYr = 1 Then
            If FirstIncrMo < mo Then
                FirstIncrMo = mo
            End If
        End If

        'Debug.Print ""
        'Debug.Print "FirstIncrMo = "; FirstIncrMo
        'Debug.Print "FirstIncrYr = "; FirstIncrYr


        'zero out all months before the first incremental production month

        BegMth = 1
        EndMth = ((FirstIncrYr - 1) * 12) + FirstIncrMo - 1

        If EndMth > 0 Then

            For izm = BegMth To EndMth

                NHVmonth(izm) = 0
                InOpMonth(izm) = 0

            Next izm

            ' now divide the annual amounts over the remaining months

            BegMth = EndMth + 1
            EndMth = FirstIncrYr * 12
            RemMth = EndMth - BegMth + 1

            For izm = BegMth To EndMth

                NHVmonth(izm) = RVN(FirstIncrYr, NHVIndex) / RemMth
                InOpMonth(izm) = RVN(FirstIncrYr, IOCIndex) / RemMth

            Next izm

        End If

        'For izm = 1 To 24
        'Debug.Print "izm = "; izm; "  NHVmonth(izm) = "; NHVmonth(izm)
        'Debug.Print "izm = "; izm; "  InOpMonth(izm) = "; InOpMonth(izm)
        'Next izm
        'Debug.Print ""


        ' now group monthly amounts by calendar quarters
        ' the quarters here are relative to the beginning of the first project year

        For izm = 1 To monthlg

            qtrindex = Int((izm - 1) / 3) + 1

            NHV(qtrindex) = NHV(qtrindex) + NHVmonth(izm)
            IncrOpex(qtrindex) = IncrOpex(qtrindex) + InOpMonth(izm)

        Next izm

        'shift back arrays by the number of quarters necessary

        If adjqtr = 1 Or adjqtr = 2 Or adjqtr = 3 Then

            For izq = 1 To qtrlg

                If (izq + adjqtr) <= qtrlg Then
                    NHV(izq) = NHV(izq + adjqtr)
                    IncrOpex(izq) = IncrOpex(izq + adjqtr)
                Else
                    NHV(izq) = 0
                    IncrOpex(izq) = 0
                End If

            Next izq

            qtrlg = qtrlg - adjqtr ' redefine quarterly life based on project start month

        End If

        'For izq = 1 To qtrlg
        'Debug.Print "izq = "; izq; "  NHV(izq) = "; NHV(izq)
        'Debug.Print "izq = "; izq; "  IncrOpex(izq) = "; IncrOpex(izq)
        'Next izq
        'Debug.Print ""

        '  Debug.Print "iz", "my3(iz,5)", "qtrcap", "Capital(qtrcap)"

        For iZ = 1 To my3tt

            If my3(iZ, 2) = 1 Or my3(iZ, 2) = 2 Or my3(iZ, 2) = 3 Then
                qtrcap = ((my3(iZ, 3) - YR) * 4) + 1 - adjqtr
            ElseIf my3(iZ, 2) = 4 Or my3(iZ, 2) = 5 Or my3(iZ, 2) = 6 Then
                qtrcap = ((my3(iZ, 3) - YR) * 4) + 2 - adjqtr
            ElseIf my3(iZ, 2) = 7 Or my3(iZ, 2) = 8 Or my3(iZ, 2) = 9 Then
                qtrcap = ((my3(iZ, 3) - YR) * 4) + 3 - adjqtr
            ElseIf my3(iZ, 2) = 10 Or my3(iZ, 2) = 11 Or my3(iZ, 2) = 12 Then
                qtrcap = ((my3(iZ, 3) - YR) * 4) + 4 - adjqtr
            End If

            If my3(iZ, 1) <> 1 Then ' exclude BNS from Capital()
                Capital(qtrcap) = Capital(qtrcap) + my3(iZ, 5)
                '     Debug.Print iz, my3(iz, 5), qtrcap, Capital(qtrcap)
            End If

        Next iZ


        'Debug.Print "qtrlg = "; qtrlg

        'Debug.Print "izq", "NHV", "BaseOpex", "IncrOpex", "Capital", "FeeDeflator"

        'For izq = 1 To qtrlg
        '  Debug.Print izq, NHV(izq), BaseOpex(izq), IncrOpex(izq), Capital(izq), FeeDeflator(izq)
        'Next izq

    End Sub
	
	'
	' Modifications:
	' 12 Jan 2004 JWD
	'  -> Add references to CGiantReport1 object to collect
	'     report data in object rather than output directly to
	'     file. For consolidation engine development testing
	'     purposes. (C0772)
	'
	' 19 Jan 2004 JWD
	'  -> Changed report page object from explicit object type
	'     to interface type. (C0774)
	'  -> Changed to call NewStandardRptPageSpecial() to
	'     create report page instance instead of
	'     NewStandardRptPage(). (C0774)
	'
	' 3 Feb 2004 JWD
	'  -> Remove explicit writes to report file. (C0776)
	'=========================================================
    Sub ServiceFee(ByRef varnum As Short, ByRef qtrlg As Single, ByRef mo As Single, ByRef DiscRate As Single, ByRef RF() As String, ByRef NHV() As Single, ByRef BaseOpex() As Single, ByRef IncrOpex() As Single, ByRef Capital() As Single, ByRef FeeDeflator() As Single, ByRef BVen() As Single)
        '---------------------------------------------------------
        Dim iv As Single
        Dim ivc As Single
        Dim jv As Single
        Dim BvenDum As Single
        Dim CalcDiscRate As Single
        Dim CompAmount As Single
        Dim CompRate As Single
        Dim currentqtr As Single
        Dim DflNegCf As Single
        Dim DflPosCf As Single
        Dim DiscAmount As Single
        Dim PgCounter As Short
        Dim QtrComRate As Single
        Dim QtrDisRate As Single
        Dim qtrstart As Single
        Dim reportlg As Single

        '---------------------------------------------------------
        'Debug.Print "entering Service Fee, DiscRate = "; DiscRate

        'hard code values for input variables for checking purposes

        'qtrlg = 7
        'DiscRate = 10

        'For iz = 1 To 2
        '   NHV(iz) = 0
        '   BaseOpex(iz) = 0
        '   IncrOpex(iz) = 0
        'Next iz

        'For iz = 3 To qtrlg
        '   NHV(iz) = 49.65
        '   BaseOpex(iz) = 3
        '   IncrOpex(iz) = 5
        'Next iz

        '   Capital(1) = 50
        '   Capital(2) = 50
        '   Capital(3) = 0
        '   Capital(4) = 0
        '   Capital(5) = 0
        '   Capital(6) = 0
        '   Capital(7) = 0
        '   FeeDeflator(1) = 1
        '   FeeDeflator(2) = 1.01
        '   FeeDeflator(3) = 1.02
        '   FeeDeflator(4) = 1.03
        '   FeeDeflator(5) = 1.04
        '   FeeDeflator(6) = 1.05
        '   FeeDeflator(7) = 1.06


        ' this calculates the incremental service fee for Venezuela
        ' all calculations are done on a quarterly basis


        'UPGRADE_NOTE: MIRR was upgraded to MIRR_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        Dim MIRR_Renamed(qtrlg) As Single
        Dim MIRRQtr(qtrlg) As Single
        Dim DA1(qtrlg) As Single
        Dim DA2(qtrlg) As Single
        Dim CFforFee(qtrlg) As Single
        Dim TVen(qtrlg) As Single
        Dim XVen(qtrlg) As Single
        Dim YVen(qtrlg) As Single
        Dim SFP(qtrlg) As Single
        Dim TTable(80) As Single

        TTable(1) = 0.999615 : TTable(2) = 0.998459 : TTable(3) = 0.996534 : TTable(4) = 0.993844
        TTable(5) = 0.990393 : TTable(6) = 0.986185 : TTable(7) = 0.981228 : TTable(8) = 0.975528
        TTable(9) = 0.969096 : TTable(10) = 0.96194 : TTable(11) = 0.954072 : TTable(12) = 0.945503
        TTable(13) = 0.932648 : TTable(14) = 0.92632 : TTable(15) = 0.915735 : TTable(16) = 0.904508
        TTable(17) = 0.892658 : TTable(18) = 0.880203 : TTable(19) = 0.867161 : TTable(20) = 0.853553
        TTable(21) = 0.8394 : TTable(22) = 0.824724 : TTable(23) = 0.809547 : TTable(24) = 0.793893
        TTable(25) = 0.777785 : TTable(26) = 0.761249 : TTable(27) = 0.744311 : TTable(28) = 0.726995
        TTable(29) = 0.70933 : TTable(30) = 0.691342 : TTable(31) = 0.673059 : TTable(32) = 0.654508
        TTable(33) = 0.63572 : TTable(34) = 0.616723 : TTable(35) = 0.597545 : TTable(36) = 0.578217
        TTable(37) = 0.558769 : TTable(38) = 0.53923 : TTable(39) = 0.51963 : TTable(40) = 0.5
        TTable(41) = 0.48037 : TTable(42) = 0.46077 : TTable(43) = 0.441231 : TTable(44) = 0.421783
        TTable(45) = 0.402455 : TTable(46) = 0.383277 : TTable(47) = 0.36428 : TTable(48) = 0.345492
        TTable(49) = 0.326941 : TTable(50) = 0.308658 : TTable(51) = 0.29067 : TTable(52) = 0.273005
        TTable(53) = 0.255689 : TTable(54) = 0.238751 : TTable(55) = 0.222215 : TTable(56) = 0.206107
        TTable(57) = 0.190453 : TTable(58) = 0.175276 : TTable(59) = 0.1606 : TTable(60) = 0.146447
        TTable(61) = 0.132839 : TTable(62) = 0.119797 : TTable(63) = 0.107342 : TTable(64) = 0.095492
        TTable(65) = 0.084265 : TTable(66) = 0.07368 : TTable(67) = 0.063752 : TTable(68) = 0.054497
        TTable(69) = 0.045928 : TTable(70) = 0.03806 : TTable(71) = 0.030904 : TTable(72) = 0.024472
        TTable(73) = 0.018772 : TTable(74) = 0.013815 : TTable(75) = 0.009607 : TTable(76) = 0.006156
        TTable(77) = 0.003466 : TTable(78) = 0.001541 : TTable(79) = 0.000385 : TTable(80) = 0


        For iv = 1 To qtrlg
            '  Debug.Print "iv = "; iv; "  FeeDeflator(iv) = "; FeeDeflator(iv)

            CFforFee(iv) = NHV(iv) - IncrOpex(iv) - Capital(iv)

            'Debug.Print "CFforFee(iv) = ", CFforFee(iv)

            If iv = 1 Then 'calculate effective annual compound rate
                CompRate = 0
            Else
                CompRate = MIRR_Renamed(iv - 1)
            End If

            If CompRate < 0 Then CompRate = 0

            If (CompRate * 100) < DiscRate Then 'changed based on redefinition by PDVSA
                CalcDiscRate = CompRate * 100
            Else
                CalcDiscRate = DiscRate
            End If

            '   Debug.Print "(CompRate * 100) = "; CompRate * 100; "   CalcDiscRate ="; CalcDiscRate

            QtrDisRate = ((1 + (CalcDiscRate / 100)) ^ (1 / 4)) - 1
            '  Debug.Print "QtrDisRate ="; QtrDisRate


            ' Calculate the cumulative present value of the deflated negative cash flows

            For jv = 1 To iv ' loop from the beginning to the current quarter
                '      Debug.Print "jv = ", jv

                If CFforFee(jv) <= 0 Then

                    If FeeDeflator(jv) > 0 Then
                        DflNegCf = CFforFee(jv) / FeeDeflator(jv)
                    Else
                        DflNegCf = CFforFee(jv)
                    End If

                    '         Debug.Print "CFforFee(jv) = ", CFforFee(jv), "FeeDeflator(jv) = ", FeeDeflator(jv), "DflNegCF = ", DflNegCf


                    DiscAmount = DflNegCf / ((1 + QtrDisRate) ^ (jv - 1))

                    '         Debug.Print "DiscAmount = "; DiscAmount

                    DA1(iv) = DA1(iv) + DiscAmount

                    '         Debug.Print "DA1(iv) = ", DA1(iv)

                End If

            Next jv

            '   Debug.Print "DA1(iv)", DA1(iv)

            ' Calculate the compounded amounts of the positive cash flows

            QtrComRate = ((1 + CompRate) ^ (1 / 4)) - 1 ' convert to quarterly compound rate

            '     Debug.Print "CompRate = ", CompRate
            '     Debug.Print "QtrComRate = ", QtrComRate

            For jv = 1 To iv
                '      Debug.Print "in DA2 calculation, jv = "; jv

                If CFforFee(jv) > 0 Then

                    If FeeDeflator(jv) > 0 Then ' get deflated amount
                        DflPosCf = CFforFee(jv) / FeeDeflator(jv)
                    Else
                        DflPosCf = CFforFee(jv)
                    End If
                    '         Debug.Print "FeeDeflator(jv) = ", FeeDeflator(jv)
                    '         Debug.Print "DflPosCF = ", DflPosCf

                    CompAmount = DflPosCf * ((1 + QtrComRate) ^ (iv - jv)) ' compound to current quarter
                    '         Debug.Print "CompAmount = ", CompAmount
                    DA2(iv) = DA2(iv) + CompAmount ' accumulate compounded amounts
                    '         Debug.Print "DA2(iv) = ", DA2(iv)

                End If

            Next jv

            '   Debug.Print "DA2(iv) = ", DA2(iv)

            If DA1(iv) <> 0 Then
                MIRRQtr(iv) = ((DA2(iv) / (System.Math.Abs(DA1(iv)))) ^ (1 / iv)) - 1 'nominal quarterly MIRR
            Else
                MIRRQtr(iv) = 0
            End If

            MIRR_Renamed(iv) = ((1 + MIRRQtr(iv)) ^ 4) - 1 ' annualized effective MIRR

            '   Debug.Print "MIRRQtr(iv) = ", MIRRQtr(iv), "MIRR(iv) = ", MIRR(iv)

            ' set time adjustment factor
            If iv < 80 Then
                TVen(iv) = TTable(iv)
            Else
                TVen(iv) = 0
            End If

            XVen(iv) = 0.75 - (0.75 * (MIRR_Renamed(iv)))

            YVen(iv) = 1 - (1.16667 * MIRR_Renamed(iv))
            '   Debug.Print "TVen(iv) = ", TVen(iv), "XVen(iv) = ", XVen(iv)
            '   Debug.Print "YVen(iv) = ", YVen(iv)

            ' now calculate Service Fee Percentage (actually a fraction)

            If MIRR_Renamed(iv) <= 0 Then
                SFP(iv) = 1
            ElseIf MIRR_Renamed(iv) < 0.6 Then
                SFP(iv) = YVen(iv) + (TVen(iv) * (XVen(iv) - YVen(iv)))
            Else
                SFP(iv) = 0.3
            End If

            '   Debug.Print "SFP(iv) = ", SFP(iv)

            ' calculate incremental service fee amount

            BvenDum = (CFforFee(iv) * SFP(iv)) + IncrOpex(iv) + Capital(iv)

            If NHV(iv) <= BvenDum Then
                BVen(iv) = NHV(iv)
            Else
                BVen(iv) = BvenDum
            End If

            '   Debug.Print "iv = "; iv; "  BVen(iv) = "; BVen(iv)

        Next iv

        ' write out variables for printing Service Fee worksheet
        ' if RF$(5) = "ALL"
        '   Debug.Print "RF$(5) = "; RF$(5)
        '   Debug.Print "varnum = "; varnum
        '   Debug.Print "TD$(numvar,18) = "; TD$(varnum, 18)

        Dim oPg1 As IGiantRptPageAssignStd
        If Left(RF(5), 3) = "ALL" And TD(varnum, 18) <> "NOP" And TD(varnum, 18) <> "VOP" Then


            Dim ColHdr(14) As Single

            For ivc = 1 To 14
                ColHdr(ivc) = CSng("dummy")
            Next ivc

            ColHdr(7) = CSng(VB6.Format(QtrDisRate, "##.###%"))

            qtrstart = 0
            If mo = 1 Or mo = 2 Or mo = 3 Then qtrstart = 1
            If mo = 4 Or mo = 5 Or mo = 6 Then qtrstart = 2
            If mo = 7 Or mo = 8 Or mo = 9 Then qtrstart = 3
            If mo = 10 Or mo = 11 Or mo = 12 Then qtrstart = 4

            'reports are limited to 40 quarters
            reportlg = qtrlg
            'If qtrlg > 40 Then reportlg = 40

            Dim WorkSht(reportlg, 14) As Single

            For iv = 1 To reportlg

                'compute quarter numbers for printout
                If iv = 1 Then
                    currentqtr = qtrstart
                Else
                    currentqtr = currentqtr + 1
                    If currentqtr > 4 Then currentqtr = 1
                End If
                '   Debug.Print "iv = "; iv; "  FeeDeflator(iv) = "; FeeDeflator(iv)



                WorkSht(iv, 1) = currentqtr
                WorkSht(iv, 2) = NHV(iv)
                WorkSht(iv, 3) = IncrOpex(iv)
                WorkSht(iv, 4) = Capital(iv)
                WorkSht(iv, 5) = FeeDeflator(iv)
                WorkSht(iv, 6) = CFforFee(iv)
                WorkSht(iv, 7) = DA1(iv)
                WorkSht(iv, 8) = DA2(iv)
                '   WorkSht(iv, 9) = MIRRQtr(iv)
                WorkSht(iv, 9) = MIRR_Renamed(iv) * 100
                WorkSht(iv, 10) = TVen(iv) * 100
                WorkSht(iv, 11) = XVen(iv) * 100
                WorkSht(iv, 12) = YVen(iv) * 100
                WorkSht(iv, 13) = SFP(iv) * 100
                WorkSht(iv, 14) = BVen(iv)

                'zero out T, X and Y calculations when not used

                If MIRR_Renamed(iv) <= 0 Or MIRR_Renamed(iv) >= 0.6 Then
                    WorkSht(iv, 10) = 0
                    WorkSht(iv, 11) = 0
                    WorkSht(iv, 12) = 0
                End If

            Next iv

            ''''Write #5, 17, YR, PgCounter%, reportlg, 14, "VENEZUELA SERVICE FEE CALCULATION", 8, FinalWin, FINALPARTIC, sCur
            ''''Write #5, ColHdr$(1), ColHdr$(2), ColHdr$(3), ColHdr$(4), ColHdr$(5), ColHdr$(6), ColHdr$(7), ColHdr$(8), ColHdr$(9), ColHdr$(10), ColHdr$(11), ColHdr$(12), ColHdr$(13), ColHdr$(14)

            oPg1 = g_oReport.NewStandardRptPageSpecial(17)
            oPg1.SetPageHeader(17, YR, PgCounter, reportlg, 14, "VENEZUELA SERVICE FEE CALCULATION", 8, FinalWin, FINALPARTIC, sCur)
            oPg1.SetProfileHeaders(ColHdr(1), ColHdr(2), ColHdr(3), ColHdr(4), ColHdr(5), ColHdr(6), ColHdr(7), ColHdr(8), ColHdr(9), ColHdr(10), ColHdr(11), ColHdr(12), ColHdr(13), ColHdr(14))

            For iv = 1 To reportlg
                ''''Write #5, WorkSht(iv, 1), WorkSht(iv, 2), WorkSht(iv, 3), WorkSht(iv, 4), WorkSht(iv, 5), WorkSht(iv, 6), WorkSht(iv, 7), WorkSht(iv, 8), WorkSht(iv, 9), WorkSht(iv, 10), WorkSht(iv, 11), WorkSht(iv, 12), WorkSht(iv, 13), WorkSht(iv, 14)
                oPg1.SetProfileValues(iv, WorkSht(iv, 1), WorkSht(iv, 2), WorkSht(iv, 3), WorkSht(iv, 4), WorkSht(iv, 5), WorkSht(iv, 6), WorkSht(iv, 7), WorkSht(iv, 8), WorkSht(iv, 9), WorkSht(iv, 10), WorkSht(iv, 11), WorkSht(iv, 12), WorkSht(iv, 13), WorkSht(iv, 14))
            Next iv

        End If

    End Sub
End Module