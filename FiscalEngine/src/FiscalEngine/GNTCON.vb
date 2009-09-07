Option Strict Off
Option Explicit On
Module GNTCON
	' $linesize: 132
	' $title:    'GIANT v6.1 - 1996                         GNTCON.BAS'
	' $subtitle: 'GIANT Consolodation I/O Routines'
	' **********************************************************************
	' *     COPYRIGHT - PETROCONSULTANTS, INC. - 1986, 1995, 1996, 2001    *
	' *                       ALL RIGHTS RESERVED                          *
	' **********************************************************************
	' *  This program file is proprietary information of Petroconsultants, *
	' *  Incorporated.  Unauthorized use for any purpose is prohibited.    *
	' **********************************************************************
	'-----------------------------------------------------------------------
	' Routines to read and write the consolidation data file
	'-----------------------------------------------------------------------
	' Modifications:
	' 13 Feb 1996 JWD
	'          Replaced explicit subroutine declaration statements with
	'       include file GNTCON.BI.
	'          Changed common include file from CTYIN.BAS to CTYIN1.BG.
	' 19 Feb 1996 JWD
	'          Add explicit declaration of default storage class as Single.
	'
	' 21 Sep 2001 JWD
	'  -> Changed ReadGNTCon(). (C0454)
	' 08 Apr 2003 GDP
	'  -> Commented out OXY data read/write
	' 24 Apr 2003 GDP
	'  -> Commented out more OXY code
	'
	' 9 Feb 2005 JWD
	'  -> Changed ReadGNTCon(). (C0856)
	'  -> Changed WriteGNTCon(). (C0856)
	'-----------------------------------------------------------------------
	'$Dynamic
	'UPGRADE_NOTE: DefSng A-Z statement was removed. Variables were explicitly declared as type Single. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="92AFD3E3-440D-4D49-A8BF-580D74A8C9F2"'
	
	'UPGRADE_NOTE: GNTCON was upgraded to GNTCON_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Const GNTCON_Renamed As String = "GNTCON.PRN"
	Const GNTCON1 As String = "GNTCON"
	Const GNTCON2 As String = ".PRN"
	'$include:      'ctyin1.bg'
	'$include:      'gntcon.bi'
	
	Sub ReadGNTCon()
		'-----------------------------------------------------------------------
		' Read in consolidation data from previous runs.
		'-----------------------------------------------------------------------
		' Modifications:
		' 20 Feb 1996 JWD
		'  -> Renamed path names (GNT.DRIVE$, etc.) read
		'     from file, previous names not acceptable to VB.
		'
		' 25 Aug 1998 JWD
		'  -> Change symbol name Decimal$ to strDecimal to
		'     eliminate name conflict with reserved word in VB5.
		'
		' 21 Sep 2001 JWD
		'  -> Change redimension of CC() array to use public
		'     symbol for sizing rather than literal 300. (C0454)
		' 08 Apr 2003 GDP
		'  -> Commented out reading of OXY data
		'
		' 9 Feb 2005 JWD
		'  -> Change Redim of AC() to use symbol for dimension 2.
		'     (C0856)
		'  -> Change Input of AC() to input in loop to ubound
		'     of symbol for dimension 2. (C0856)
		'  -> Add declaration of loop index symbols i and j.
		'     (C0856)
		'---------------------------------------------------------
        ' 9 Feb 2005 JWD (C0856) Add symbols for loop indexes
		Dim i As Short
		Dim j As Short
		' End (C0856)
		'---------------------------------------------------------
		
        FileOpen(1, TempDir & GNTCON_Renamed, OpenMode.Input)
		'<<<<<< 25 Aug 1998 JWD Change name Decimal$
		Input(1, CCT)
		Input(1, RNU)
		Input(1, RFT)
		Input(1, PPR)
		Input(1, PPS)
		Input(1, PPT)
		Input(1, WINT)
		Input(1, RN)
		Input(1, sGntDir)
		Input(1, sCtyDir)
		Input(1, sRunDir)
		Input(1, sRptDir)
		Input(1, LGI)
		Input(1, N1)
		Input(1, N1C)
		Input(1, strDecimal)
		Input(1, LG)
		Input(1, YR)
		Input(1, ConCur)
		Input(1, DiscMthd)
		'>>>>>> End 25 Aug 1998
31112: 
		
		' 9 Feb 2005 JWD (C0856) Change redim of AC() and CC() to use symbols, rearrange to dimension only one array per statement
		ReDim PN(4)
		ReDim PNC(4)
		ReDim RF(RUNFILECOLS)
		ReDim L1(14)
		ReDim gn(12)
		ReDim AC(gc_nMAXLIFE, gc_nACSIZED2) ' was: ReDim AC(gc_nMAXLIFE, 11)
		ReDim CC(gc_nMAXCAPEX, gc_nCCSIZED2) ' was: ReDim CC(gc_nMAXCAPEX, 4)
		' Was:
		''<<<<<< 21 Sep 2001 JWD (C0454)
		'ReDim PN$(4), PNC$(4), RF$(RUNFILECOLS), L1(14), gn(12), AC(gc_nMAXLIFE, 11), CC(gc_nMAXCAPEX, 4)
		''~~~~~~ was:
		''ReDim PN$(4), PNC$(4), RF$(RUNFILECOLS), L1(14), gn(12), AC(gc_nMAXLIFE, 11), CC(300, 4)
		''>>>>>> End (C0454)
		' End (C0856)
		
		For i = 1 To 4
31113: 
			Input(1, PN(i))
			Input(1, PNC(i))
		Next i
		For i = 1 To 7
31114: 
			Input(1, RF(i))
		Next i
		For i = 1 To 12
31115: 
			Input(1, L1(i))
			Input(1, gn(i))
		Next i
		For i = 1 To 2
31116: 
			Input(1, L1(i + 12))
		Next i
		For i = 1 To gc_nMAXLIFE
31117: 
			' 9 Feb 2005 JWD (C0856) Change to input in a loop
			For j = LBound(AC, 2) To UBound(AC, 2)
				Input(1, AC(i, j))
			Next j
			' Was:
			'Input #1, AC(i%, 1), AC(i%, 2), AC(i%, 3), AC(i%, 4), AC(i%, 5), AC(i%, 6), AC(i%, 7), AC(i%, 8), AC(i%, 9), AC(i%, 10), AC(i%, 11)
			' End (C0856)
		Next i
		For i = 1 To CCT
31118: 
			' 9 Feb 2005 JWD (C0856) Change to input in a loop
			For j = LBound(CC, 2) To UBound(CC, 2)
				Input(1, CC(i, j))
			Next j
			' Was:
			'Input #1, CC(i%, 1), CC(i%, 2), CC(i%, 3), CC(i%, 4)
			' End (C0856)
		Next i
		
		' GDP 08 Apr 2003
		' Commented out OXY data
		'        '-------------------------------------------------------------
		'                'OXY database items (consolidated values)
		'         Input #1, iNGCFT, iNGCFA, iNGCFU
		'         Input #1, ADim1%, ADim2%
		'
		'         ReDim ADataCon(ADim1%, ADim2%)
		'         For i% = 1 To ADim1%
		'           For j% = 1 To ADim2%
		'             Input #1, ADataCon(i%, j%)
		'           Next j%
		'         Next i%
		'        'store data put into GIANT.PRJ file. This file contains the data
		'        '  about a run.
		'        'There is ONE entry per RUN.
		'         Input #1, HistSerCon$
		'         Input #1, StatusCon$, TypeCon$, ConstatCon$, ClassCon$
		'         Input #1, CountryCon$, BlockCon$, FieldCon$, CaseCon$
		'         Input #1, FieldDescCon$, RegionCon$, PrimaryCon$, OnOffCon$
		'         Input #1, SuccessCon!, IDCon$, RunTypeCon$, WhoCon$, PriceBaseCon$
		'         Input #1, GVerNoCon$, Tag1Con$, Tag2Con$, Tag3Con$, Tag4Con$, Tag5Con$
		'         Input #1, WinCompCon!, WinPartCon!, WinGovtCon!, WinExpCon!
		'         Input #1, WinNetCon!, WinRevCon!, WinNCFCon!, DsctDateCon$
		'         Input #1, DsctMethCon$, USTxRateCon!, PUSTCon$, PUSBCon$
		'         Input #1, PNIBCon$, PNIACon$, WatDepthCon!
		'         Input #1, GTakeCon!, GTake10Con!, GTake15Con!, GTake20Con!
		'         Input #1, GntRORCon!, GntNCFCon!, GntNPV10Con!, GntNPV15Con!
		'         Input #1, GntNPV20Con!, GntPICon!, GntPI10Con!, GntPI15Con!
		'         Input #1, GntPI20Con!, GntPayotCon!, GntDscvYCon!
		'         Input #1, OXYGrRevCon!, OXYTtlPosCon!
		'         Input #1, PriorGProdEq!, PriorGExpCap!
		'
		'
		'        'store data put into GIANT.DOC file. This file contains the data
		'        '  about a run.  Contains info about the files used in a run.
		'        'There is ONE entry per RUN.
		'
		'         Input #1, GTitl1Con$, GTitl2Con$, GTitl3Con$, GTitl4Con$, CTitl1Con$
		'         Input #1, CTitl2Con$, CTitl3Con$, CTitl4Con$
		'         Input #1, GNTDescCon$, CtyDescCon$, EXCDescCon$, EXTDescCon$
		'         Input #1, GNTFileCon$, CTYFileCon$, RUNFileCon$, EXTFileCon$
		'         Input #1, EXCFileCon$, SensDatCon$, SensCtyCon$
		'        'store data put into GIANT.NOT file. This file contains the data
		'        '  about a run.  This relates to the notes fields in a file.
		'        'There is ONE entry per RUN.
		'
		'         Input #1, norunlines%
		'31122
		'         ReDim RunLinesCon$(norunlines%)
		'         For i% = 1 To norunlines%
		'31123
		'           Input #1, RunLinesCon$(i%)
		'         Next i%
		'         Input #1, nonoteslines%
		'31124
		'         ReDim RunNotesCon$(nonoteslines%)
		'         For i% = 1 To nonoteslines%
		'31125
		'           Input #1, RunNotesCon$(i%)
		'         Next i%
		'         Input #1, NoTypeCon$, LineNoCon%, NotesCon$
		'         Input #1, ttlcapCon!        'total capital (from grossrpt.exe)
		'                'End of OXY Database items
31126: 
		
		FileClose(1)
		
	End Sub
	
	Sub WriteGNTCon()
		'-----------------------------------------------------------------------
		' Write consolidation data between runs
		'-----------------------------------------------------------------------
		' Modifications:
		' 20 Feb 1996 JWD
		'  -> Renamed path names (GNT.DRIVE$, etc.) written
		'     to file, previous names not acceptable to VB.
		'
		' 25 Aug 1998 JWD
		'  -> Change symbol name Decimal$ to strDecimal to
		'     eliminate name conflict with reserved word in VB5.
		' 08 Apr 2003 GDP
		'  -> Commented out writing of OXY data
		'  -> Replaced call to second write with FileCopy
		' 24 Apr 2003 GDP
		'  -> Commented out more OXY code
		'
		' 9 Feb 2005 JWD
		'  -> Change to write AC() elements in inner loop using
		'     LBound and UBound on dimension 2. (C0856)
		'  -> Change to write CC() elements in inner loop using
		'     LBound and UBound on dimension 2. (C0856)
		'  -> Add symbol declaration for loop indexes i and j.
		'     (C0856)
		'---------------------------------------------------------
        ' 9 Feb 2005 JWD (C0856) Add symbols for loop indexes
        ' End (C0856)
		'---------------------------------------------------------
17900: 
        FileOpen(1, TempDir & GNTCON_Renamed, OpenMode.Output)
		'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        WriteFile()
		FileClose(1)
		Dim strFileName As String
		strFileName = GNTCON1 & VB6.Format(RNU) & GNTCON2
		' GDP 08 Apr 2003
		' Replaced 2nd write code with FileCopy
        FileCopy(TempDir & GNTCON_Renamed, TempDir & strFileName)
		'         Open TempDir$ + strFileName For Output As #1
		'           GoSub WriteFile
		'         Close #1
		
		Exit Sub
		'------------------------------------

17930: 
		
    End Sub

    Sub WriteFile()
        Dim i As Short
        Dim j As Short

        '<<<<<< 25 Aug 1998 JWD
        WriteLine(1, CCT, RNU, RFT, PPR, PPS, PPT, 0, RN, sGntDir, sCtyDir, sRunDir, sRptDir, LGI, N1, N1C, strDecimal, LG, YR, ConCur, DiscMthd)
        '>>>>>> End 25 Aug 1998

        ReDim Preserve PN(4)
        ReDim Preserve PNC(4)
        For i = 1 To 4
17902:
            WriteLine(1, PN(i), PNC(i))
        Next i
        ReDim Preserve RF(RUNFILECOLS)
        For i = 1 To 7
17904:
            WriteLine(1, RF(i))
        Next i
        ReDim Preserve L1(14)
        ReDim Preserve gn(12)
        For i = 1 To 12
17906:
            WriteLine(1, L1(i), gn(i))
        Next i
        For i = 1 To 2
17908:
            WriteLine(1, L1(i + 12)) 'write L1(13) & L1(14)  (discovery dates)
        Next i
        '        REDIM PRESERVE AC(30, 7)
        For i = 1 To gc_nMAXLIFE
17910:
            ' 9 Feb 2005 JWD (C0856) Write in inner loop using L/Ubound
            For j = LBound(AC, 2) To UBound(AC, 2) - 1
                Write(1, AC(i, j))
            Next j
            WriteLine(1, AC(i, UBound(AC, 2)))
            ' Was:
            'Write #1, AC(i%, 1), AC(i%, 2), AC(i%, 3), AC(i%, 4), AC(i%, 5), AC(i%, 6), AC(i%, 7), AC(i%, 8), AC(i%, 9), AC(i%, 10), AC(i%, 11)
            ' End (C0856)
        Next i
        '         REDIM PRESERVE CC(CCT, 4)
        For i = 1 To CCT
17912:
            ' 9 Feb 2005 JWD (C0856) Write in inner loop using L/Ubound
            For j = LBound(CC, 2) To UBound(CC, 2) - 1
                Write(1, CC(i, j))
            Next j
            WriteLine(1, CC(i, UBound(CC, 2)))
            ' Was:
            'Write #1, CC(i%, 1), CC(i%, 2), CC(i%, 3), CC(i%, 4)
            ' End (C0856)
        Next i
        ' GDP 24 Apr 2003
        ' Commented out additional OXY code
        '        '-------------------------------------------------------------
        '                'OXY database items (consolidated values)
        '         Write #1, iNGCFT, iNGCFA, iNGCFU
        '17914
        '         Write #1, UBound(ADataCon, 1), UBound(ADataCon, 2)
        '         For i% = 1 To UBound(ADataCon, 1)
        '           For j% = 1 To UBound(ADataCon, 2)
        '17916
        '             Write #1, ADataCon(i%, j%)
        '           Next j%
        '         Next i%
        '17918

        ' GDP 08 Apr 2003
        ' Commented out OXY data
        '        'store data put into GIANT.PRJ file. This file contains the data
        '        '  about a run.
        '        'There is ONE entry per RUN.
        '         Write #1, HistSerCon$
        '         Write #1, StatusCon$, TypeCon$, ConstatCon$, ClassCon$
        '         Write #1, CountryCon$, BlockCon$, FieldCon$, CaseCon$
        '         Write #1, FieldDescCon$, RegionCon$, PrimaryCon$, OnOffCon$
        '         Write #1, SuccessCon!, IDCon$, RunTypeCon$, WhoCon$, PriceBaseCon$
        '         Write #1, GVerNoCon$, Tag1Con$, Tag2Con$, Tag3Con$, Tag4Con$, Tag5Con$
        '         Write #1, WinCompCon!, WinPartCon!, WinGovtCon!, WinExpCon!
        '         Write #1, WinNetCon!, WinRevCon!, WinNCFCon!, DsctDateCon$
        '         Write #1, DsctMethCon$, USTxRateCon!, PUSTCon$, PUSBCon$
        '         Write #1, PNIBCon$, PNIACon$, WatDepthCon!
        '         Write #1, GTakeCon!, GTake10Con!, GTake15Con!, GTake20Con!
        '         Write #1, GntRORCon!, GntNCFCon!, GntNPV10Con!, GntNPV15Con!
        '         Write #1, GntNPV20Con!, GntPICon!, GntPI10Con!, GntPI15Con!
        '         Write #1, GntPI20Con!, GntPayotCon!, GntDscvYCon!
        '         Write #1, OXYGrRevCon!, OXYTtlPosCon!
        '         Write #1, PriorGProdEq!, PriorGExpCap!
        '
        '        'store data put into GIANT.DOC file. This file contains the data
        '        '  about a run.  Contains info about the files used in a run.
        '        'There is ONE entry per RUN.
        '17920
        '
        '         Write #1, GTitl1Con$, GTitl2Con$, GTitl3Con$, GTitl4Con$, CTitl1Con$
        '         Write #1, CTitl2Con$, CTitl3Con$, CTitl4Con$
        '         Write #1, GNTDescCon$, CtyDescCon$, EXCDescCon$, EXTDescCon$
        '         Write #1, GNTFileCon$, CTYFileCon$, RUNFileCon$, EXTFileCon$
        '         Write #1, EXCFileCon$, SensDatCon$, SensCtyCon$
        '        'store data put into GIANT.NOT file. This file contains the data
        '        '  about a run.  This relates to the notes fields in a file.
        '        'There is ONE entry per RUN.
        '17922
        '         Write #1, UBound(RunLinesCon$)
        '         For i% = 1 To UBound(RunLinesCon$)
        '17924
        '           Write #1, RunLinesCon$(i%)
        '         Next i%
        '17926
        '         Write #1, UBound(RunNotesCon$)
        '         For i% = 1 To UBound(RunNotesCon$)
        '17928
        '           Write #1, RunNotesCon$(i%)
        '         Next i%
        '         Write #1, NoTypeCon$, LineNoCon%, NotesCon$
        '         Write #1, ttlcapCon!        'total capital (from grossrpt.exe)
        '                'End of OXY Database items
    End Sub
End Module