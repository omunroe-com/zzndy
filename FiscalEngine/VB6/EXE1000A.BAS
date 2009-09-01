Attribute VB_Name = "EXE1000A"
' Name:        EXE1000A.BAS
' Function:    Main Control for Run of GIANT
'---------------------------------------------------------
' ********************************************************
' *        COPYRIGHT © 1986-2001 IHS ENERGY GROUP        *
' *                  ALL RIGHTS RESERVED                 *
' ********************************************************
' *   This program file is proprietary information of    *
' *                  IHS Energy Group                    *
' *   Unauthorized use for any purpose is prohibited.    *
' ********************************************************
'---------------------------------------------------------
' This file is modified from MAINEXEC.BAS.
'---------------------------------------------------------
' Modifications:
' 7 Feb 1996 JWD
'  -> Changed common include file from CTYIN.BAS to
'     CTYIN1.BG.
'  -> Add set of bDebugging (bDebugging is in common).
'  -> Replace explicit declaration of TRUE & FALSE with
'     TRUFALSE.BC
'  -> Add explicit declaration of default storage class
'     as Single.
'  -> Converted application mainline code to Sub Main().
'  -> Removed MS-DOS main module executable code to
'     include file MAINLINE.BAS.
'
' 13 Feb 1996 JWD
'  -> Replaced explicit subroutine declaration
'     statements with include file GNTCON.BI
'
' 19 Aug 1996 JWD
'  -> Removed remarks respecting dual environment (VB &
'     MS-DOS).  Program is now a VB app.
'  -> Add Option Explicit statement.
'  -> Changed Main().
'
' 29 Aug 1996 JWD
'  -> Changed Main().
'
' 24 Oct 1996 JWD
'  -> Changed Main(). (SCO0003)
'
' 28 Oct 1996 JWD
'  -> Changed Main(). (SCO0003)
'  -> Added MODULENAME symbol.
'
' 30 Oct 1996 JWD
'  -> Changed Main(). (SCO0004)
'
' 31 Oct 1996 JWD
'  -> Changed Main(). (SCO0009)
'
' 23 Nov 1996 JWD
'  -> Changed Main(). (SCO0010)
'
' 25 Aug 1998 JWD
'  -> Changed Main().
'
' 13 Jan 1999 JWD
'  -> Changed Main(). (SCO0075)
'  -> Add procedure ExportForecastedData(). (SCO0075)
'
' 28 Jan 1999 JWD
'  -> Rename procedure ExportForecastedData() to
'     ExportForecastedAnnual(). (SCO0075)
'  -> Add procedure ExportForecastedData(). (SCO0075)
'  -> Add procedure ExportForecastedMY3(). (SCO0075)
'  -> Change Main(). (SCO0075)
'
' 21 Jun 2001 JWD
'  -> Changed Main(). (C0339)
'
' 25 Jun 2001 JWD
'  -> Changed Main(). (C0341)
'
' 5 Jul 2001 JWD
'  -> Changed Main(). (C0341)
'
' 23 Jul 2001 JWD
'  -> Changed Main(). (C0354)
'
' 1 Aug 2001 JWD
'  -> Changed Main(). (C0363)
'
' 2 Aug 2001 JWD
'  -> Changed Main(). (C0365)
'
' 21 Sep 2001 JWD
'  -> Add public constant symbol gc_nMAXCAPEX to size
'     capex arrays. (C0454)
'  -> Changed Main(). (C0454)
'
' 20 Jan 2003 GDP
'  -> Changed ExportForecastedAnnual()
'
' 08 Apr 2003 GDP
'  -> Changed Main().
'
' 23 May 2003 JWD
'  -> Changed ExportForecastedAnnual() (C0700)
'
' 12 Jan 2004 JWD
'  -> Changed Main(). (C0772)
'
' 15 Jan 2004 JWD
'  -> Changed Main(). (C0772)
'
' 9 Feb 2004 JWD
'  -> Renamed Main() to Mainexec(). Main() is now in a
'     different module. Part of migration to ActiveX DLL.
'     (C0781)
'  -> Changed Mainexec(). (C0783)
'
' 27 Feb 2004 JWD
'  -> Changed Mainexec(). (C0776)
'
' 9 Mar 2004 JWD
'  -> Changed Mainexec().
'
' 15 Mar 2004 JWD
'  -> Changed Mainexec().
'
' 17 Mar 2004 JWD
'  -> Changed Mainexec().
'
' 23 Sep 2004 JWD
'  -> Changed Mainexec(). (C0839)
'
' 10 Feb 2005 JWD
'  -> Changed Mainexec(). (C0856)
'
' 11 May 2005 JWD
'  -> Changed ExportForecastedAnnual(). (C0876)
'
' 17 May 2005 JWD
'  -> Changed Mainexec(). (C0878)
'-----------------------------------------------------------------------
Option Explicit
'$dynamic
DefSng A-Z
'$include: 'trufalse.bc'

   Const MODULENAME = "EXE1000A.BAS"
   Public Const RUNFILECOLS As Integer = 11

   Const STACKSIZE& = 32000
   Global Const gc_nMAXLIFE = 100
   
   '<<<<<< 21 Sep 2001 JWD (C0454)
   Public Const gc_nMAXCAPEX = 3000
   '>>>>>> End (C0454)
   
'$include: 'gntcon.bi'

'$INCLUDE: 'run0200.bi'
'$INCLUDE: 'pgm9900.bi'

'$include: 'ctyin1.bg'    ' contains common block

'=======================================================================

'$include: 'mainline.bas'            ' the MS-DOS mainline code

'=======================================================================
Dim sStats()   As String

'
' Modifications:
' 23 May 2003 JWD
'  -> Add new adjustment categories AJ6-AJ0. (C0700)
'
' 11 May 2005 JWD
'  -> Changed to write based on UBound of dimension 2
'     rather than explicitly reference each dimension 2
'     element. This to accomodate changes to the number
'     of categories in the A() array. (C0876)
'
' Export the forecasted annual data to the file accessed
' by hfOut.
'
' 20 Jan 2003
' Changed for additional volumes
Sub ExportForecastedAnnual(hfOut As Integer)

   Dim i As Integer
    Dim j As Integer
    
   For i = 1 To LG
      ' GDP 20 Jan 2003
      ' Write out extra volumes and change references to constants
      ' 23 May 2003 JWD Add new categories
      'Write #hfOut, A(i, 1), A(i, 2), A(i, 3), A(i, 4), A(i, 5), A(i, 6), A(i, 7), A(i, 8), A(i, 9), A(i, 10), A(i, 11), A(i, 12), A(i, 13), A(i, 14), A(i, 15), A(i, 16), A(i, 17), A(i, 18), A(i, 19), A(i, 20)
      'Write #hfOut, A(i, gc_nAOIL), A(i, gc_nAGAS), A(i, gc_nAOV1), A(i, gc_nAOV2), A(i, gc_nAOV3), A(i, gc_nAOV4), _
                    A(i, gc_nAOV5), A(i, gc_nAOV6), A(i, gc_nAOV7), A(i, gc_nAOV8), A(i, gc_nAOV9), A(i, gc_nAOV0), _
                    A(i, gc_nARES), A(i, gc_nAWIN), A(i, gc_nAOPC), A(i, gc_nAGPC), A(i, gc_nAOP1), A(i, gc_nAOP2), _
                    A(i, gc_nAOP3), A(i, gc_nAOP4), A(i, gc_nAOP5), A(i, gc_nAOP6), A(i, gc_nAOP7), A(i, gc_nAOP8), _
                    A(i, gc_nAOP9), A(i, gc_nAOP0), A(i, gc_nAOX1), A(i, gc_nAOX2), A(i, gc_nAOX3), A(i, gc_nAOX4), _
                    A(i, gc_nAOX5), A(i, gc_nAAJ1), A(i, gc_nAAJ2), A(i, gc_nAAJ3), A(i, gc_nAAJ4), A(i, gc_nAAJ5), _
                    A(i, gc_nAAJ6), A(i, gc_nAAJ7), A(i, gc_nAAJ8), A(i, gc_nAAJ9), A(i, gc_nAAJ0)
        ' 11 May 2005 JWD (C0876) Write out extra categories
        For j = 1 To UBound(A, 2) - 1
            Write #hfOut, A(i, j);
        Next j
        Write #hfOut, A(i, UBound(A, 2))
   Next i

End Sub

'
' Export the forecasted capital expenditures data
' to the file accessed by hfOut.
'
Sub ExportForecastedCapex(hfOut As Integer)

   Dim i As Integer

   For i = 1 To MY3T
      Write #hfOut, my3(i, 1), my3(i, 2), my3(i, 3), my3(i, 4), my3(i, 5), my3(i, 6), my3(i, 7)
   Next i

End Sub

'
' Export the forecasted data to disk files.
'
Sub ExportForecastedData()

   Dim hfExport As Integer

   hfExport = FreeFile
   
   ' Annual data array
   Open sRptDir & "GIANTANN.TMP" For Output As #hfExport
   Call ExportForecastedAnnual(hfExport)
   Close #hfExport

   ' Capital expenditure data array
   Open sRptDir & "GIANTMY3.TMP" For Output As #hfExport
   Call ExportForecastedCapex(hfExport)
   Close #hfExport

End Sub

'---------------------------------------------------------
' Modifications:
' 20 Feb 1996 JWD
'  -> Changed variables GNT.DRIVE$, CTY.DRIVE$,
'     RUN.DRIVE$, RPT.DRIVE$, EXCH.DRIVE$ to sGntDir,
'     sCtyDir, sRunDir, sRptDir, sExchDir respectively.
'     VB considered the old names undefined user types.
'
' 23 Apr 1996 MKD
'  -> Changed ProgVersion$ and VerNumber! to 6.10
'
' 19 Aug 1996 JWD
'  -> Add check of run file "RUN" line before call of
'     Bonus().  This is to support special export file
'     formats.  SP1 device option requires that the report
'     level be ALL.  Force it here if not correct.  Also
'     change device to FIL if specified as SP1.  This lets
'     file to be opened correctly.  This is intended as a
'     temporary measure until user interface is changed.
'     This change is made in 2 places in the code.
'  -> Add variable declarations required by Option
'     Explicit statement.
'
' 29 Aug 1996 JWD
'  -> Add check of "CONSOL" line before handling
'     consolidation processing.  Current code in CASHFLOW
'     does not handle "SP1" device code.  Force to "FIL"
'     so file is opened properly.
'
' 24 Oct 1996 JWD
'  -> Add subroutine FixupForExportCodes to check "RUN"
'     and "CONSOL" lines for special export file codes and
'     fixup device and level if special code is used for
'     device.  Add special code "OXY".  Replaced previous
'     inline test and fixup in three places used with call
'     to subroutine. (SCO0003).
'  -> Remove assignments to ProgVersion$ and vernumber,
'     not otherwise referenced.
'
' 28 Oct 1996 JWD
'  -> Add subroutine RenameVariableTitlesFile to rename
'     the file created by CountryForecast() for storage of
'     variable titles for the run.  All files need to be
'     kept until after the entire run file has been
'     processed.  These files are used by the special
'     export programs (OXY, etc.). (SCO0003)
'
' 30 Oct 1996 JWD
'  -> Comment out call to WriteOXYFiles().  The service
'     provided by the call has been replaced by special
'     OXY export file format.  (SCO0004)
'
' 31 Oct 1996 JWD
'  -> Add subroutine RenameMiscellaneousTitlesFile to
'     rename the file created for storage of the user's
'     titles for miscellaneous data for the current run.
'     The file is used by the report display program and
'     by the special export programs.  (SCO0009)
'
' 23 Nov 1996 JWD
'  -> Add code RFR to subroutine FixupForExportCodes to
'     force generation of required files.  (SCO0010)
'
' 25 Aug 1998 JWD
'  -> Change symbol name Decimal$ to strDecimal to
'     eliminate name conflict with reserved word in VB5.
'
' 13 Jan 1999 JWD
'  -> Add code to extract command line switch from startup
'     Command$ for controlling export of forecast data.
'     (SCO0075)
'  -> Add calls to ExportForecastedData(). (SCO0075)
'
' 28 Jan 1999 JWD
'  -> Remove parameter from call to ExportForecastedData().
'     ExportForecastedData() is now a wrapper for exporting
'     both annual and capital expenditure data. (SCO0075)
'  -> Move code opening and closing export file to
'     procedure ExportForecastedData(). (SCO0075)
'
' 21 Jun 2001 JWD
'  -> Add initialization of global symbols for balance
'     capex categories BAL, BL2, BL3. Necessitated by
'     addition of new capital category codes that changed
'     the actual values of the BAL codes.(C0339)
'
' 25 Jun 2001 JWD
'  -> Add calls to apply abandonment funding provisions.
'     (C0341)
'
' 5 Jul 2001 JWD
'  -> Add initialization of abandonment funding
'     provisions. (C0341)
'
' 23 Jul 2001 JWD
'  -> Move call to ApplyAbandonmentFundingProvisions() to
'     precede the call to CalculateBonus(). (2 places)
'     (C0354)
'
' 1 Aug 2001 JWD
'  -> Add initialization of global symbols for abandonment
'     cash/accrual option codes (part of capex codes).
'     (C0363)
'
' 2 Aug 2001 JWD
'  -> Comment out the second call to
'     ApplyAbandonmentFundingProvisions in Else block of
'     'If AR=0 Then .. Else'. This was intended when C0354
'     executed. (C0365)
'
' 21 Sep 2001 JWD
'  -> Change redimension of CC() to use public symbol for
'     sizing rather than literal 300. (C0454)
'
' GDP 08 Apr 2003
'  -> Commented out OXY code
'
' 12 Jan 2004 JWD
'  -> Add references to CGiantReport1 object to collect
'     report data in object rather than output directly to
'     file. For consolidation engine development testing
'     purposes. (C0772)
'
' 15 Jan 2004 JWD
'  -> Add capture of original project life after forecast
'     is completed and before imposition of fiscal terms
'     to use for sizing output array to original life.
'     This is for comparison with ASPEEngine OutputAmounts
'     array. (C0772)
'
' 30 Jan 2004 JWD
'  -> Add call to InitializeSystem to initialize system
'     objects. (C0776)
'
' 9 Feb 2004 JWD
'  -> Rename procedure to Mainexec() with command string
'     as parameter. (C0781)
'  -> Replace assignment from Command$ with CommandLine.
'     (C0781)
'  -> Replace call to TerminateExecution with re-raise of
'     error to caller. (C0779)
'  -> Remove output to OXFIL.DAT. File is obsolete and not
'     used in any application system. (C0783)
'  -> Remove input from xxxx.RNN (v 5.0, 5.1, 5.2 run
'     notes file). (C0783)
'
' 27 Feb 2004 JWD
'  -> Changed run file processing to use file object to
'     replace explicit disk file i/o statements. (C0776)
'
' 9 Mar 2004 JWD
'  -> Add condition on calls to WriteReport.
'
' 15 Mar 2004 JWD
'  -> Add call to LoadCurrencyFile().
'
' 17 Mar 2004 JWD
'  -> Add initialization of CCT at beginning of runfile,
'     was not being reset across runfiles.
'
' 23 Sep 2004 JWD
'  -> Changed to write run summary data out from here. The
'     direct write of the file from Cashflow() has been
'     removed and the data has been added to the economic
'     indicators page so that it can be returned from the
'     ASPEEngine interface. (C0839)
'
' 10 Feb 2005 JWD
'  -> Change dimension statements for AC() and CC() to use
'     symbols for dimension 2 size. (C0856)
'
' 17 Mar 2005 JWD
'  -> Add initialization of last balance category code.
'     (C0878)
'
' 24 July 2009 AV
'  -> Moved definition of gna_ACFX after first invocation of FiscalDef
'     which could increase LG (project life) in order to prevent raising of
'    'Index out of range' error.
'
'---------------------------------------------------------


Sub Mainexec _
    ( _
    ByVal CommandLine As String _
    )
'---------------------------------------------------------
   Dim lMxStack   As Long
   Dim lFreeMem   As Long
   Dim fLog       As String
   Dim fEnviron$
   Dim hfLog      As Integer
   Dim iDum       As Integer
   Dim i          As Integer
   Dim j          As Integer
   
      '~~~~ 19 Aug 1996 JWD Following declarations added.
   Dim ct%
   Dim ErrNo%
   Dim filen%
   Dim fileno%
   Dim RunFileNotes%
   Dim DUM$
   Dim dum0$
   Dim dum1$
   Dim dum2$
   Dim dum3$
   Dim dum4$
   Dim dum5$
   Dim dum6$
   Dim dum7$
   Dim dum8$
   Dim dum9$
   Dim dum10$
   Dim dum11$
   Dim goodname%
   Dim q%
   Dim RunDesc$
   Dim RunDirectory$
   Dim RunFileName$
   Dim RunNoteFile$
   Dim s$
   Dim strTmp     As String
   Dim zipfileno%
      '~~~~ End 19 Aug 1996
   
   '<<<<<< 13 Jan 1999 JWD
   Dim bExportForecasts As Integer
   Dim hfExport As Integer
   '>>>>>> End 13 Jan 1999
   
   ' 15 Jan 2004 JWD
   Dim l_natural_life As Long
   
   ' 23 Sep 2004 JWD (C0839) Add handle for run summary file
   Dim l_oRunSumOut As IEFSFileSeqOut

    Dim iiiX As Integer
    Dim iiiY As Integer
    Dim iiiZ As Integer
    Dim ReportLevelSaved As String
    Dim RunNameSaved As String
    
    
    Dim lpa_my3Ex() As Integer      ' array of indexes to link my3() and my3Ex()
'-----------------------------------------------------------------------
   Dim dStart     As Double
   
   dStart = Timer
   ReDim sStats(0) As String

      On Local Error GoTo 25000

        ' Set debugging switch - bDebugging is in common
      BDebugging = Environ$("GNTBUG") <> ""

        ' Program$ is in COMMON
      Program$ = "MAINEXEC"

      ' 9 Feb 2004 JWD (C0781) Assign CommandLine formal parameter, replacing Command$
      fEnviron$ = CommandLine       ' was: = Command$

      '<<<<<< 13 Jan 1999 JWD Extract any command line
      '                       switches from fEnviron$
      bExportForecasts = False
      i = InStr(UCase$(fEnviron$), "/XDAT")
      If i > 0 Then
         Mid$(fEnviron$, i, 5) = "     "
         bExportForecasts = True
         fEnviron$ = Trim$(fEnviron)
      End If
      '>>>>>> End 13 Jan 1999

      '<<<<<< 21 Jun 2001 JWD (C0339)
      ' Initialize the code values for
      ' the balance capex categories
      SearchCodeString CPXCategoryCodesString, "BAL", 3, CPXCategoryCodeBAL
      SearchCodeString CPXCategoryCodesString, "BL2", 3, CPXCategoryCodeBL2
      SearchCodeString CPXCategoryCodesString, "BL3", 3, CPXCategoryCodeBL3
      '>>>>>> End (C0339)
      
      '<<<<<< 1 Aug 2001 JWD (C0363)
      SearchCodeString CPXCategoryCodesString, CPXCategoryCodeString_AbandonmentCashExpenditure, 3, CPXCategoryCode_AbandonmentCashExpenditure
      SearchCodeString CPXCategoryCodesString, CPXCategoryCodeString_AbandonmentAccrualEntry, 3, CPXCategoryCode_AbandonmentAccrualEntry
      '>>>>>> End (C0363)
      
      ' 17 May 2005 JWD (C0878) Initialize the value of the last balance category code
      SearchCodeString CPXCategoryCodesString, CPXCategoryCodeString_BLn, 3, CPXCategoryCodeBLn
      
      InitializeExecution fEnviron$

      '-----------------------------------------------------------------
      ' Setup file specifications...
      FConfig$ = TempDir$ + "GNTCONFG.DAT"
      FExchng$ = TempDir$ + "GNTEXCH.DAT"
      FChuong$ = TempDir$ + "CHUONG.DAT"

      FOxfil$ = TempDir$ + "OXFIL.DAT"
      FOxyRun$ = TempDir$ + "OXYRUN~.FIL"
      '-----------------------------------------------------------------

        'erase any debugging files hanging around...
      If Len(Dir$("curr.log")) > 0 Then
        Kill "CURR.LOG"
      End If
19
20      '  Always read in Configuration data
      Open FConfig$ For Input As #1
            'printer type, condense, datefmt, GntDir, CtyDir, RunDir, RptDir, ExttblDir, Decimals, ExchDir, ParPort, SerPort, ExchName
         Input #1, strTmp, strTmp, strTmp, sGntDir, sCtyDir, sRunDir, sRptDir, sExtDir, iDum, sExchDir, strTmp, strTmp, ExchNm$
         '<<<<<< 25 Aug 1998 JWD Change symbol name
         strDecimal$ = LTrim$(str$(iDum))
         '>>>>>> End 25 Aug 1998
      Close #1
21
22
23
      Open TempDir$ + "RUNNAME.PRN" For Input As #1
         Input #1, RunDirectory$, RunFileName$
      Close #1
      sRunDir = RunDirectory$

        'OXY Paradox Database items
      RUNFile$ = RunDirectory$ + RunFileName$ + ".RUN"
      g_sConsolCur = GetConsolCurrency(RUNFile$)
24
      ReadFinder AR, Camefrom$

      LoadCurrencyFile
      
      If AR = 0 Then                          ' this if first time through
         ' Set a global reference to the ring fence file, and initialize it
         Set g_oRingFenceFile = g_oFileSystem.OpenForOutput(TempDir$ & "RING.FNC").CloseFile
         
         ReDim gfa_RingFenceFiles(0 To 4)
         
         Set gfa_RingFenceFiles(gfa_RingFenceFile_OPS) = g_oFileSystem.OpenForOutput(TempDir$ & "RINGO.FNC").CloseFile
         Set gfa_RingFenceFiles(gfa_RingFenceFile_GRP) = g_oFileSystem.OpenForOutput(TempDir$ & "RINGG.FNC").CloseFile
         Set gfa_RingFenceFiles(gfa_RingFenceFile_CMP) = g_oRingFenceFile   ' holds another reference, for when g_oRingFenceFile is assigned one of the others
         Set gfa_RingFenceFiles(gfa_RingFenceFile_DUM) = g_oFileSystem.OpenForOutput(TempDir$ & "DUMMY.FNC").CloseFile
         
                'these MUST be dimmed for GNTCON SUB
         ReDim RunLinesCon$(0)
         ReDim RunNotesCon$(0)
         ReDim ADataCon(57, 1)

         Maxlife% = gc_nMAXLIFE
         If Camefrom$ = "GNTMAIN" Then        ' has run file
            GoSub ReadRun                     ' Get name of Run File
         Else                                 ' is single run
            RN$ = "SINGLE"
            s$ = RN$ + ".RUN"
         End If

            'Now determine if first line of run file says "VERSION 5.0"
26
27
12129    Debug.Print sRunDir, s$
         Set g_oRunFileIn = g_oFileSystem.OpenForInput(sRunDir & s$)    ' Open sRunDir + S$ For Input As #3
         
49070    AR = 1
19190
         DUM$ = g_oRunFileIn.NextItem           ' Input #3, DUM$
19191    DUM$ = Left$(DUM$, 11)
         If Left$(DUM$, 9) <> "VERSION 5" Then   'not ver 5.x
            Error 255
         Else    ' Assign total number of lines in Run File to RFT
            If DUM$ = "VERSION 5.0" Or DUM$ = "VERSION 5.1" Or DUM$ = "VERSION 5.2" Then
                 'if the version = 5.0 or 5.1 or 5.2 then
               RFT = 0:    i = 0
19193          DUM$ = g_oRunFileIn.NextItem           ' Input #3, DUM$        'placeholder for description line
               Do While Not EOF(3)
                         'count and read run file lines
                  i = i + 1
19194             ' Input #3, dum1$, dum2$, dum3$, dum4$, dum5$, dum6$, dum7$
                  dum1$ = g_oRunFileIn.NextItem
                  dum2$ = g_oRunFileIn.NextItem
                  dum3$ = g_oRunFileIn.NextItem
                  dum4$ = g_oRunFileIn.NextItem
                  dum5$ = g_oRunFileIn.NextItem
                  dum6$ = g_oRunFileIn.NextItem
                  dum7$ = g_oRunFileIn.NextItem
                  
                    'OXY database items
                  ReDim Preserve RunLines$(i)
                  dum1$ = Left$(dum1$ + Space$(7), 7)
                  dum2$ = Left$(dum2$ + Space$(25), 25)
                  dum3$ = Left$(dum3$ + Space$(8), 8)
                  dum4$ = Left$(dum4$ + Space$(3), 3)
                  dum5$ = Left$(dum5$ + Space$(8), 8)
                  dum6$ = Left$(dum6$ + Space$(8), 8)
                  dum7$ = Left$(dum7$ + Space$(8), 8)
                  RunLines$(i) = dum1$ + " " + dum2$ + " " + dum3$ + " " + dum4$ + " " + dum5$ + " " + dum6$ + " " + dum7$
               Loop
               RFT = i      'RFT # of lines in Run File

'''               9 Feb 2004 JWD (C0783) Remove input of Run Notes (RNN) file
'''                    'RUN notes file
'''               RunNoteFile$ = sRunDir + Left$(s$, Len(s$) - 3) + "RNN"
'''               If Len(Dir$(RunNoteFile$)) > 0 Then
'''                  fileno% = FreeFile
'''                  Open RunNoteFile$ For Input As #fileno%
'''                  If ErrNo% = 0 Then
'''                     Input #fileno%, strTmp     'case description
'''                     Input #fileno%, RunNoteRecs%
'''                     ReDim RunNotes$(RunNoteRecs%)
'''                     For i = 1 To RunNoteRecs%
'''                        Input #fileno%, RunNotes$(i)
'''                     Next i
'''                     Close #fileno%
'''                  End If
'''               End If

19195       ElseIf Val(Right$(DUM$, 4)) > 5.2 Then        'i.e.VERSION 5.3


               RFT = 0:    i = 0
               RunFileNotes% = 0
               DUM$ = g_oRunFileIn.NextItem           ' Input #3, DUM$              'placeholder for description line

''' 9 Feb 2004 JWD (C0783) Remove output to OXFIL.DAT
'''                'store file names for OXY data base zipping
'''               zipfileno% = FreeFile
'''               Open FOxfil$ For Append As #zipfileno%
'''                   Print #zipfileno%, sRunDir + s$
'''               Close #zipfileno%

               Do While Not g_oRunFileIn.AtEnd          ' EOF(3)
                  DUM$ = g_oRunFileIn.NextItem          ' Input #3, DUM$            'command variable
                  Select Case DUM$
                  Case "GETDATA", "SNSDATA", "SNSPER", "GETCTRY", "SNSCTRY", "RUN", "GRAPH", "PLOT", "CONSOL"
                     i = i + 1
3234
                     ' Input #3, dum2$, dum3$, dum4$, dum5$, dum6$, dum7$, dum8$, dum9$, dum10$, dum11$
                     dum2$ = g_oRunFileIn.NextItem
                     dum3$ = g_oRunFileIn.NextItem
                     dum4$ = g_oRunFileIn.NextItem
                     dum5$ = g_oRunFileIn.NextItem
                     dum6$ = g_oRunFileIn.NextItem
                     dum7$ = g_oRunFileIn.NextItem
                     dum8$ = g_oRunFileIn.NextItem
                     dum9$ = g_oRunFileIn.NextItem
                     dum10$ = g_oRunFileIn.NextItem
                     dum11$ = g_oRunFileIn.NextItem

                        'OXY database items

'----------------------------------------------------------------------
'1-20-93
'If this is a GETDATA or GETCNTY line, put the file name in the OXFIL file
'  for use by the OXY data base routines.  This file will be used as the
'  response file when GNTOXY1.EXE SHELLs to PKZIP.EXE to zip all of the
'  input files used in this run

'
'                    IF dum$ = "GETDATA" OR dum$ = "GETCTRY" THEN
'                       IF dum$ = "GETDATA" THEN
'                          tp$ = ".GNT"
'                       ELSEIF dum$ = "GETCTRY" THEN
'                          tp$ = ".CTY"
'                       END IF
'                       zipfileno% = FREEFILE
'                       OPEN FOxfil$ FOR APPEND AS #zipfileno%
'                          PRINT #zipfileno%, dum2$ + dum3$ + tp$
'                       CLOSE #zipfileno%
'                    END IF
'----------------------------------------------------------------------
                     ReDim Preserve RunLines$(i)
                     DUM$ = Left$(DUM$ + Space$(7), 7)
                     dum2$ = Left$(dum2$ + Space$(25), 25)
                     dum3$ = Left$(dum3$ + Space$(8), 8)
                     dum4$ = Left$(dum4$ + Space$(3), 3)
                     dum5$ = Left$(dum5$ + Space$(8), 8)
                     dum6$ = Left$(dum6$ + Space$(8), 8)
                     dum7$ = Left$(dum7$ + Space$(8), 8)
                     dum8$ = Left$(dum8$ + Space$(8), 8)
                     dum9$ = Left$(dum9$ + Space$(8), 8)
                     dum10$ = Left$(dum10$ + Space$(8), 8)
                     dum11$ = Left$(dum11$ + Space$(8), 8)
                     
                     RunLines$(i) = DUM$ + " " + dum2$ + " " + dum3$ + " " + dum4$ _
                                  + " " + dum5$ + " " + dum6$ + " " + dum7$ _
                                  + " " + dum8$ + " " + dum9$ + " " + dum10$ + " " + dum11$
                                  
                  
                  Case Else          'this is a notes line
                     RunNoteRecs% = RunNoteRecs% + 1
                     ReDim Preserve RunNotes$(RunNoteRecs%)
                     RunNotes$(RunNoteRecs%) = DUM$
                  End Select
               Loop
               
               RFT = i      'RFT # of lines in Run File
            End If
            g_oRunFileIn.CloseFile          ' Close #3
' GDP 08 Apr 2003
' Commented out OXY code
''''''''''''''''''''''''''''''''''''''''
''we must write the contents of the run file (including notes) to disk
''  so that in subsequent runs, we have an image of the complete run file
''  for the OXY NOT database file. This is the easiest way to do this since
''  when I need the info in GNTOXY1.EXE, the run file is open for other uses.
'
'            ct% = 0
'            If RunNoteRecs% > 0 Then
'               For i = RunNoteRecs% To 1 Step -1
'                  If RTrim$(LTrim$(RunNotes$(i))) <> "" Then
'                     ct% = i
'                     Exit For
'                  End If
'               Next i
'            End If
'            If ct% > 0 Then
'               RunNoteRecs% = ct%
'            Else
'               RunNoteRecs% = 1
'            End If
'            ReDim Preserve RunNotes$(RunNoteRecs%)
'
'            filen% = FreeFile
'            Open FOxyRun$ For Output As #filen%
'               Write #filen%, UBound(RunLines$), RunNoteRecs%
'               For i = 1 To UBound(RunLines$)
'                  Write #filen%, RunLines$(i)
'               Next i
'               For i = 1 To RunNoteRecs%
'                  Write #filen%, RTrim$(RunNotes$(i))
'               Next i
'            Close #filen%
''''''''''''''''''''''''''''''''''''''''


                'WE now have a count of the number of run lines (excluding notes)
19196
            Set g_oRunFileIn = g_oFileSystem.OpenForInput(sRunDir & s$) ' Open sRunDir + S$ For Input As #3
            DUM$ = g_oRunFileIn.NextItem            ' Input #3, DUM$
            RunDesc$ = g_oRunFileIn.NextItem        ' Input #3, RunDesc$            'case description
            ReDim RF$(RUNFILECOLS)                  'Now assign RF$()
            ReDim L1(14)                  'stores dates & durations for consolidations
            AR = 1
19197       ' Input #3, RF$(1), RF$(2), RF$(3), RF$(4), RF$(5), RF$(6), RF$(7), RF$(8), RF$(9), RF$(10), RF$(11)
            For j = 1 To RUNFILECOLS
                RF$(j) = g_oRunFileIn.NextItem
            Next j
            
            If Left$(RF$(1), 7) = "GETDATA" Then

               L1(6) = 10000
               L1(2) = 10000
               L1(7) = 10000
               L1(8) = 0
               L1(12) = 10000
               L1(14) = 10000
'1/8/92 ---------------------
'              N1$ = RF$(3)
'----------------------------
                                 
               ' 10 Feb 2005 JWD (C0856) Change to use symbols for 2 dim size
               ReDim AC(gc_nMAXLIFE, gc_nACSIZED2)
               ReDim CC(gc_nMAXCAPEX, gc_nCCSIZED2)
               ' Was:
               ''<<<<<< 21 Sep 2001 JWD (C0454)
               '   'consolidation arrays - filled out in cashflow
               'ReDim AC(gc_nMAXLIFE, 11), CC(gc_nMAXCAPEX, 4)
               ''~~~~~~ was:
               ''   'consolidation arrays - filled out in cashflow
               ''ReDim AC(gc_nMAXLIFE, 11), CC(300, 4)
               ''>>>>>> End (C0454)
               ' End (C0856)
               
               ' 17 Mar 2004 JWD Add initialization of CCT, was not being reset across runfiles
               CCT = 0
               
19198
               Call ForecastData
               
               
               ' 15 Jan 2004 JWD
               ' Capture project natural life
               l_natural_life = LG
               
               '<<<<<< 13 Jan 1999 JWD Add to export forecasted data
               If bExportForecasts Then
                  ExportForecastedData
               End If
               '>>>>>> End 13 Jan 1999
               
               '<<<<<< 5 Jul 2001 JWD (C0341)
               InitializeAbandonmentFundingProvisions
               '>>>>>> End (C0341)
               
               Call CountryForecast
               
               
                  ' 24 Oct 1996 JWD Replace inline test
                  '  with following GoSub
               GoSub FixupForExportCodes
                           
                           
               '** Currency conversion of input here
               
               If Len(RF$(3)) > 0 Then
                  ConvertInputData RF$(3)
               End If

               '<<<<<< 23 Jul 2001 JWD (C0354)
               ApplyAbandonmentFundingProvisions
               '>>>>>> End (C0354)
               
               Call CalculateBonus
                  
               '<<<<<< 23 Jul 2001 JWD (C0354)
               ''<<<<<< 25 Jun 2001 JWD (C0341)
               'ApplyAbandonmentFundingProvisions
               ''>>>>>> End (C0341)
               '>>>>>> End (C0354)
               
                  ' 31 Oct 1996 JWD See comments in Gosub
               GoSub RenameMiscellaneousTitlesFile

                  ' 28 Oct 1996 JWD See comments in Gosub
               GoSub RenameVariableTitlesFile
               ' 4th Jul 2000 GDP Changed for AS$ET so full currency names are passed
               'If Len(RF$(3)) > 0 Then sCur = Left$(RF$(3), 3)
               If Len(RF$(3)) > 0 Then sCur = RF$(3)
    
                Call GrossReport
                                
                ReDim xRunSwitches(1 To RunSwitchesCount)
                
                ' go ahead and do the normal calls through the
                ' generation of after-tax cash flow page while the
                ' data is absolutely right at this point.
                xRunSwitches(RunSwitch_LVL) = RunSwitch_LVL_Company
                xRunSwitches(RunSwitch_PAR) = RunSwitch_PAR_On
                xRunSwitches(RunSwitch_WIN) = RunSwitch_WIN_On
                xRunSwitches(RunSwitch_FIN) = RunSwitch_FIN_On
                xRunSwitches(RunSwitch_LMT) = RunSwitch_LMT_On
                xRunSwitches(RunSwitch_DCF) = RunSwitch_DCF_Off

                Call FiscalDef
                ReDim gna_ACFX(0 To LG, 0 To 26)
                
                Call ConsolidateLoan
                Call CalculateRepayment
                
                ' capture the company cash flow values now
                xxx_POSCF = gna_ACFX_CPS
                xxx_NEGCF = gna_ACFX_CNG
                
                Call Cashflow
                
                ' Now go back and generate the data for the dcf page
                ' copy the operating-level numbers to A() and MY3()
                If g_bPTCons Then
                
                    ' establish correspondence between my3() and my3Ex()
                    ' when done lpa_my3Ex values are the index of the
                    ' my3Ex() row that corresponds to the my3() row index
                    ' value.
                    ' i. e. my3Ex(lpa_my3Ex(my3tt), gna_my3Ex_GCX) is the value
                    ' of the group-level capital for the expenditure my3(my3tt)
                    ReDim lpa_my3Ex(LBound(my3, 1) To UBound(my3, 1))
                    For iiiX = 1 To my3tt
                        ' Find the corresponding item (based on category, exp. date and tangible percent)
                        ' in this case (g_bPTCons is true) each entry is supposed to be unique
                        ' pointer is initialized to point to row 0,
                        ' which should be zero valued for all levels in my3Ex()
                        lpa_my3Ex(iiiX) = 0
                        For iiiY = 1 To UBound(my3Ex, 1)
                            If my3Ex(iiiY, gna_my3Ex_CAT) = my3(iiiX, gc_nMY3_CAT) And my3Ex(iiiY, gna_my3Ex_XMO) = my3(iiiX, gc_nMY3_XMO) And my3Ex(iiiY, gna_my3Ex_XYR) = my3(iiiX, gc_nMY3_XYR) And my3Ex(iiiY, gna_my3Ex_TAN) = my3(iiiX, gc_nMY3_TAN) Then
                                lpa_my3Ex(iiiX) = iiiY
                            End If
                        Next iiiY
                    Next iiiX
                   
                    
                    ' Now go back and generate the data for the dcf page
                    ' capture total-level
                    ReDim OPEX(gc_nMAXLIFE)
                    For iiiX = 1 To LG
                        For iiiY = 1 To gc_nAMAXOPX
                            A(iiiX, iiiY) = gna_ACX(iiiX, iiiY, 0)
                        Next iiiY
                        For iiiY = gc_nAMINOPX To gc_nAMAXOPX
                            OPEX(iiiX) = OPEX(iiiX) + A(iiiX, iiiY)
                        Next iiiY
                        
                        gna_ACFX(iiiX, gna_ACFX_TPS) = ATotalRevenues(iiiX)
                        gna_ACFX(iiiX, gna_ACFX_TNG) = OPEX(iiiX)
                        
                    Next iiiX
                    
                    For iiiX = 1 To my3tt
                        my3(iiiX, 5) = my3Ex(lpa_my3Ex(iiiX), gna_my3Ex_TCX)
                        iiiY = my3(iiiX, 3) - YR + 1
                        gna_ACFX(iiiY, gna_ACFX_TNG) = gna_ACFX(iiiY, gna_ACFX_TNG) + my3Ex(lpa_my3Ex(iiiX), gna_my3Ex_TCX)
                    Next iiiX
                    
                    ' load operating-level values
                    ReDim OPEX(gc_nMAXLIFE)
                    For iiiX = 1 To LG
                        For iiiY = 1 To gc_nAMAXOPX
                            A(iiiX, iiiY) = gna_ACX(iiiX, iiiY, 1)
                        Next iiiY
                        For iiiY = gc_nAMINOPX To gc_nAMAXOPX
                            OPEX(iiiX) = OPEX(iiiX) + A(iiiX, iiiY)
                        Next iiiY
                    Next iiiX
                    
                    For iiiX = 1 To my3tt
                        my3(iiiX, 5) = my3Ex(lpa_my3Ex(iiiX), gna_my3Ex_OCX)
                    Next iiiX
                    
                    xRunSwitches(RunSwitch_LVL) = RunSwitch_LVL_Operating
                    xRunSwitches(RunSwitch_PAR) = RunSwitch_PAR_Off
                    xRunSwitches(RunSwitch_WIN) = RunSwitch_WIN_Off
                    xRunSwitches(RunSwitch_LMT) = RunSwitch_LMT_Off
                    xRunSwitches(RunSwitch_DCF) = RunSwitch_DCF_Off
    
                    ReportLevelSaved = RF$(5)
                    RF$(5) = "   "      ' set to no report output
                    
                    ' Change the ring fence file to use for this level
                    ' Used from here through the next call of Cashflow()
                    Set g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_OPS)
                    
                    Call FiscalDef
                    Call ConsolidateLoan
                    Call CalculateRepayment
       
                    xxx_POSCF = gna_ACFX_OPS
                    xxx_NEGCF = gna_ACFX_ONG
    
                    Call Cashflow
               
                    ' load group-level values
                    ReDim OPEX(gc_nMAXLIFE)
                    For iiiX = 1 To LG
                        For iiiY = 1 To gc_nAMAXOPX
                            A(iiiX, iiiY) = gna_ACX(iiiX, iiiY, 2)
                        Next iiiY
                        For iiiY = gc_nAMINOPX To gc_nAMAXOPX
                            OPEX(iiiX) = OPEX(iiiX) + A(iiiX, iiiY)
                        Next iiiY
                    Next iiiX
                    
                    For iiiX = 1 To my3tt
                        my3(iiiX, 5) = my3Ex(lpa_my3Ex(iiiX), gna_my3Ex_GCX)
                    Next iiiX
                    
                    xRunSwitches(RunSwitch_LVL) = RunSwitch_LVL_Group
                    xRunSwitches(RunSwitch_PAR) = RunSwitch_PAR_On
    
                    ' Change the ring fence file to use for this level
                    ' Used from here through the next call of Cashflow()
                    Set g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_GRP)
                    
                    Call FiscalDef
                    Call ConsolidateLoan
                    Call CalculateRepayment
    
                    xxx_POSCF = gna_ACFX_GPS
                    xxx_NEGCF = gna_ACFX_GNG
    
                    Call Cashflow
                    
                    ' load company-level values
                    ReDim OPEX(gc_nMAXLIFE)
                    For iiiX = 1 To LG
                        For iiiY = 1 To gc_nAMAXOPX
                            A(iiiX, iiiY) = gna_ACX(iiiX, iiiY, 3)
                        Next iiiY
                        For iiiY = gc_nAMINOPX To gc_nAMAXOPX
                            OPEX(iiiX) = OPEX(iiiX) + A(iiiX, iiiY)
                        Next iiiY
                    Next iiiX
                    
                    For iiiX = 1 To my3tt
                        my3(iiiX, 5) = my3Ex(lpa_my3Ex(iiiX), gna_my3Ex_CCX)
                    Next iiiX
                    
                    xRunSwitches(RunSwitch_LVL) = RunSwitch_LVL_Company
                    xRunSwitches(RunSwitch_WIN) = RunSwitch_WIN_On
                    xRunSwitches(RunSwitch_LMT) = RunSwitch_LMT_Off
                    xRunSwitches(RunSwitch_DCF) = RunSwitch_DCF_Off
                    
                    ' Change the ring fence file to use for this level
                    Set g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_CMP)
                    
                    ' Now a bit of a hack, keeps FiscalDef from writing to the ring fence file for this part of the run
                    ' don't want to write to the file for company-level ring fence because done
                    ' above already. But still have to have a reference to the company-level for reading
                    RunNameSaved = RF$(2)           ' save the actual name
                    RF$(2) = RunNameSaved & "X"     ' fixup to fool ring fence output code
                    Call FiscalDef
                    RF$(2) = RunNameSaved           ' restore the actual name
                    Call ConsolidateLoan
                    Call CalculateRepayment
                    
                    xxx_POSCF = 0
                    xxx_NEGCF = 0
                    
                    ' Change the ring fence file to use for this level
                    ' Don't want the company level data written again, was done above
                    Set g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_DUM)
                    
                    Call Cashflow
                    
                Else
                
                    ' single data run (non pre-tax consol)
                    For iiiX = 1 To LG
                        gna_ACFX(iiiX, gna_ACFX_TPS) = ATotalRevenues(iiiX)
                        gna_ACFX(iiiX, gna_ACFX_TNG) = OPEX(iiiX)
                    Next iiiX
                    
                    For iiiX = 1 To my3tt
                        If (my3(iiiX, 1) > 1 And my3(iiiX, 1) < CPXCategoryCodeBAL) Or my3(iiiX, 1) = CPXCategoryCode_AbandonmentCashExpenditure Then
                            iiiY = my3(iiiX, 3) - YR + 1
                            gna_ACFX(iiiY, gna_ACFX_TNG) = gna_ACFX(iiiY, gna_ACFX_TNG) + my3(iiiX, 5)
                        End If
                    Next iiiX
                    
                    xRunSwitches(RunSwitch_LVL) = RunSwitch_LVL_Operating
                    xRunSwitches(RunSwitch_PAR) = RunSwitch_PAR_Off
                    xRunSwitches(RunSwitch_WIN) = RunSwitch_WIN_Off
                    xRunSwitches(RunSwitch_LMT) = RunSwitch_LMT_Off
                    xRunSwitches(RunSwitch_DCF) = RunSwitch_DCF_Off
    
                    ReportLevelSaved = RF$(5)
                    RF$(5) = "   "      ' set to no report output
                   
                    ' Change the ring fence file to use for this level
                    ' Used from here through the next call of Cashflow()
                    Set g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_OPS)
                    
                    Call FiscalDef
                    Call ConsolidateLoan
                    Call CalculateRepayment
       
                    xxx_POSCF = gna_ACFX_OPS
                    xxx_NEGCF = gna_ACFX_ONG
    
                    Call Cashflow
                    
                    xRunSwitches(RunSwitch_LVL) = RunSwitch_LVL_Group
                    xRunSwitches(RunSwitch_PAR) = RunSwitch_PAR_On
    
                    ' Change the ring fence file to use for this level
                    ' Used from here through the next call of Cashflow()
                    Set g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_GRP)
                    
                    Call FiscalDef
                    Call ConsolidateLoan
                    Call CalculateRepayment
    
                    xxx_POSCF = gna_ACFX_GPS
                    xxx_NEGCF = gna_ACFX_GNG
    
                    Call Cashflow
    
                    xRunSwitches(RunSwitch_LVL) = RunSwitch_LVL_Company
                    xRunSwitches(RunSwitch_WIN) = RunSwitch_WIN_On
                    
                    ' Change the ring fence file to use for this level
                    ' Used from here through the next call of Cashflow()
                    Set g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_CMP)
                    
                    ' Now a bit of a hack, keeps FiscalDef from writing to the ring fence file for this part of the run
                    ' don't want to write to the file for company-level ring fence because done
                    ' above already. But still have to have a reference to the company-level for reading
                    RunNameSaved = RF$(2)           ' save the actual name
                    RF$(2) = RunNameSaved & "X"     ' fixup to fool ring fence output code
                    Call FiscalDef
                    RF$(2) = RunNameSaved           ' restore the actual name
                    Call ConsolidateLoan
                    Call CalculateRepayment
    
                    xxx_POSCF = 0
                    xxx_NEGCF = 0
    
                    ' Change the ring fence file to use for this level
                    ' Don't want the company level data written again, was done above
                    Set g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_DUM)
                    
                    Call Cashflow
        
                End If
                

                ' do the discounted cash flow page
                xRunSwitches(RunSwitch_DCF) = RunSwitch_DCF_Only
                RF$(5) = ReportLevelSaved

                ' Change the ring fence file to use for this level
                ' Used from here through the next call of Cashflow()
                Set g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_DUM)
                    
                Call Cashflow


               Call WriteGNTCon
                  
                  ' 30 Oct 1996 JWD Comment out following
                  ' OXY files replaced by OXY export format
              ' Call WriteOXYFiles
              
               ' Add condition on report write
               If RF$(5) = "ALL" Or RF$(5) = "VAR" Or RF$(5) = "SUM" Then
                   WriteOutputReport g_oReport, ReportFileSpec(sRptDir, RN$, RNU, "PRN"), g_oFileSystem
               End If
                              
               ' 23 Sep 2004 JWD (C0839) Write run summary data
               Set l_oRunSumOut = g_oFileSystem.OpenForOutput(sRptDir & RN$ & ".SUM")
               g_oReport.WriteReport l_oRunSumOut, New CCMGiantRunSummarySeqA
               l_oRunSumOut.CloseFile
               ' End (C0839)

            Else          'RF$(1) <> "GETDATA"
19199          Error 254
            End If

         End If

         'OPEN "badfile.log" FOR OUTPUT AS #13
         'PRINT #13, "Mainexec 49000 RF$(1) = "; RF$(1)
         'PRINT #13, "Mainexec 49000 Rf$(2) = "; RF$(2)
         'PRINT #13, "Mainexec 49000 RF$(3) = "; RF$(3)
         'CLOSE #13

      Else     'ar > 0  'this means that this is not the first time here
49061
         ' Set a global reference to the ring fence file. If it doesn't exist, error
         Set g_oRingFenceFile = g_oFileSystem.OpenForInput(TempDir$ & "RING.FNC").CloseFile
         
         ReDim gfa_RingFenceFiles(0 To 4)
         
         Set gfa_RingFenceFiles(gfa_RingFenceFile_OPS) = g_oFileSystem.OpenForInput(TempDir$ & "RINGO.FNC").CloseFile
         Set gfa_RingFenceFiles(gfa_RingFenceFile_GRP) = g_oFileSystem.OpenForInput(TempDir$ & "RINGG.FNC").CloseFile
         Set gfa_RingFenceFiles(gfa_RingFenceFile_CMP) = g_oRingFenceFile   ' holds another reference, for when g_oRingFenceFile is assigned one of the others
         Set gfa_RingFenceFiles(gfa_RingFenceFile_DUM) = g_oFileSystem.OpenForInput(TempDir$ & "DUMMY.FNC").CloseFile
         
         ReadGNTCon
         AR = AR + 1
         If AR > RFT Then                      'no more lines in Run File
            g_oRunFileIn.CloseFile          ' Close #3
         Else        'AR is not greater than RFT
               ' read all past lines - must do this because all files are closed coming back from DISPLAY
            s$ = RN$ + ".RUN"
49075       Set g_oRunFileIn = g_oFileSystem.OpenForInput(sRunDir & s$) ' Open sRunDir + S$ For Input As #3
'----------------------------------------------------------------------
'       put run file name in the OXFIL file - for use by the OXY data base routines
'1-20-93
49076
''' 9 Feb 2004 JWD (C0783) Remove output to OXFIL.DAT
'''            zipfileno% = FreeFile
'''            Open FOxfil$ For Append As #zipfileno%
'''                Print #zipfileno%, sRunDir + s$
'''            Close #zipfileno%
'----------------------------------------------------------------------
49080       DUM$ = g_oRunFileIn.NextItem            ' Input #3, DUM$
            dum0$ = g_oRunFileIn.NextItem           ' Input #3, dum0$
            
            For i = 1 To AR - 1
               ' Input #3, RF$(1), RF$(2), RF$(3), RF$(4), RF$(5), RF$(6), RF$(7), RF$(8), RF$(9), RF$(10), RF$(11)
                For j = 1 To RUNFILECOLS
                    RF$(j) = g_oRunFileIn.NextItem
                Next j
            Next i
                ' read next line in run file
            ' Input #3, RF$(1), RF$(2), RF$(3), RF$(4), RF$(5), RF$(6), RF$(7), RF$(8), RF$(9), RF$(10), RF$(11)
            For j = 1 To RUNFILECOLS
                RF$(j) = g_oRunFileIn.NextItem
            Next j
                'store run line data for OXY database
            ReDim Preserve RunLines$(1)
            DUM$ = Left$(RF$(1) + Space$(7), 7)
            dum2$ = Left$(RF$(2) + Space$(25), 25)
            dum3$ = Left$(RF$(3) + Space$(8), 8)
            dum4$ = Left$(RF$(4) + Space$(3), 3)
            dum5$ = Left$(RF$(5) + Space$(8), 8)
            dum6$ = Left$(RF$(6) + Space$(8), 8)
            dum7$ = Left$(RF$(7) + Space$(8), 8)
            dum8$ = Left$(RF$(8) + Space$(8), 8)
            dum9$ = Left$(RF$(9) + Space$(8), 8)
            dum10$ = Left$(RF$(10) + Space$(8), 8)
            dum11$ = Left$(RF$(11) + Space$(8), 8)
            RunLines$(1) = DUM$ + dum2$ + dum3$ + dum4$ + dum5$ + dum6$ + dum7$ + dum8$ + dum9$ + dum10$ + dum11$

            If Left$(RF$(1), 7) = "GETDATA" Then
               LG = 0
49081       Call ForecastData
               
               ' 15 Jan 2004 JWD
               ' Capture project natural life
               l_natural_life = LG
               
               '<<<<<< 13 Jan 1999 JWD Add to export forecasted data
               If bExportForecasts Then
                  ExportForecastedData
               End If
               '>>>>>> End 13 Jan 1999
               
               '<<<<<< 5 Jul 2001 JWD (C0341)
               InitializeAbandonmentFundingProvisions
               '>>>>>> End (C0341)
               
               Call CountryForecast
                  
                  ' 24 Oct 1996 JWD Replace inline test
                  '  with following GoSub
               GoSub FixupForExportCodes
               
               '** Currency conversion of input here
               If Len(RF$(3)) > 0 Then
                  ConvertInputData RF$(3)
               End If
               
               '<<<<<< 23 Jul 2001 JWD (C0354)
               ApplyAbandonmentFundingProvisions
               '>>>>>> End (C0354)
               
               Call CalculateBonus
                  
               '<<<<<< 2 Aug 2001 JWD (C0365)
               ''<<<<<< 25 Jun 2001 JWD (C0341)
               'ApplyAbandonmentFundingProvisions
               ''>>>>>> End (C0341)
               '>>>>>> End (C0365)
               
                  ' 31 Oct 1996 JWD See comments in Gosub
               GoSub RenameMiscellaneousTitlesFile

                  ' 28 Oct 1996 JWD See comments in Gosub
               GoSub RenameVariableTitlesFile
               'If Len(RF$(3)) > 0 Then sCur = Left$(RF$(3), 3)
               'Changed GDP 11/7/00 so full currency string is passed through
               If Len(RF$(3)) > 0 Then sCur = RF$(3)
                
                Call GrossReport
                
                ReDim gna_ACFX(0 To LG, 0 To 26)
                ReDim xRunSwitches(1 To RunSwitchesCount)
                
                ' go ahead and do the normal calls through the
                ' generation of after-tax cash flow page while the
                ' data is absolutely right at this point.
                xRunSwitches(RunSwitch_LVL) = RunSwitch_LVL_Company
                xRunSwitches(RunSwitch_PAR) = RunSwitch_PAR_On
                xRunSwitches(RunSwitch_WIN) = RunSwitch_WIN_On
                xRunSwitches(RunSwitch_FIN) = RunSwitch_FIN_On
                xRunSwitches(RunSwitch_LMT) = RunSwitch_LMT_On
                xRunSwitches(RunSwitch_DCF) = RunSwitch_DCF_Off

                Call FiscalDef
                Call ConsolidateLoan
                Call CalculateRepayment
                
                ' capture the company cash flow values now
                xxx_POSCF = gna_ACFX_CPS
                xxx_NEGCF = gna_ACFX_CNG
                
                Call Cashflow
                
                ' Now go back and generate the data for the dcf page
                ' copy the operating-level numbers to A() and MY3()
                If g_bPTCons Then
                
                    ' establish correspondence between my3() and my3Ex()
                    ' when done lpa_my3Ex values are the index of the
                    ' my3Ex() row that corresponds to the my3() row index
                    ' value.
                    ' i. e. my3Ex(lpa_my3Ex(my3tt), gna_my3Ex_GCX) is the value
                    ' of the group-level capital for the expenditure my3(my3tt)
                    ReDim lpa_my3Ex(LBound(my3, 1) To UBound(my3, 1))
                    For iiiX = 1 To my3tt
                        ' Find the corresponding item (based on category, exp. date and tangible percent)
                        ' in this case (g_bPTCons is true) each entry is supposed to be unique
                        ' pointer is initialized to point to row 0,
                        ' which should be zero valued for all levels in my3Ex()
                        lpa_my3Ex(iiiX) = 0
                        For iiiY = 1 To UBound(my3Ex, 1)
                            If my3Ex(iiiY, gna_my3Ex_CAT) = my3(iiiX, gc_nMY3_CAT) And my3Ex(iiiY, gna_my3Ex_XMO) = my3(iiiX, gc_nMY3_XMO) And my3Ex(iiiY, gna_my3Ex_XYR) = my3(iiiX, gc_nMY3_XYR) And my3Ex(iiiY, gna_my3Ex_TAN) = my3(iiiX, gc_nMY3_TAN) Then
                                lpa_my3Ex(iiiX) = iiiY
                            End If
                        Next iiiY
                    Next iiiX
                   
                    
                    ' Now go back and generate the data for the dcf page
                    ' capture total-level
                    ReDim OPEX(gc_nMAXLIFE)
                    For iiiX = 1 To LG
                        For iiiY = 1 To gc_nAMAXOPX
                            A(iiiX, iiiY) = gna_ACX(iiiX, iiiY, 0)
                        Next iiiY
                        For iiiY = gc_nAMINOPX To gc_nAMAXOPX
                            OPEX(iiiX) = OPEX(iiiX) + A(iiiX, iiiY)
                        Next iiiY
                        
                        gna_ACFX(iiiX, gna_ACFX_TPS) = ATotalRevenues(iiiX)
                        gna_ACFX(iiiX, gna_ACFX_TNG) = OPEX(iiiX)
                        
                    Next iiiX
                    
                    For iiiX = 1 To my3tt
                        my3(iiiX, 5) = my3Ex(lpa_my3Ex(iiiX), gna_my3Ex_TCX)
                        iiiY = my3(iiiX, 3) - YR + 1
                        gna_ACFX(iiiY, gna_ACFX_TNG) = gna_ACFX(iiiY, gna_ACFX_TNG) + my3Ex(lpa_my3Ex(iiiX), gna_my3Ex_TCX)
                    Next iiiX
                    
                    ' load operating-level values
                    ReDim OPEX(gc_nMAXLIFE)
                    For iiiX = 1 To LG
                        For iiiY = 1 To gc_nAMAXOPX
                            A(iiiX, iiiY) = gna_ACX(iiiX, iiiY, 1)
                        Next iiiY
                        For iiiY = gc_nAMINOPX To gc_nAMAXOPX
                            OPEX(iiiX) = OPEX(iiiX) + A(iiiX, iiiY)
                        Next iiiY
                    Next iiiX
                    
                    For iiiX = 1 To my3tt
                        my3(iiiX, 5) = my3Ex(lpa_my3Ex(iiiX), gna_my3Ex_OCX)
                    Next iiiX
                    
                    xRunSwitches(RunSwitch_LVL) = RunSwitch_LVL_Operating
                    xRunSwitches(RunSwitch_PAR) = RunSwitch_PAR_Off
                    xRunSwitches(RunSwitch_WIN) = RunSwitch_WIN_Off
                    xRunSwitches(RunSwitch_LMT) = RunSwitch_LMT_Off
                    xRunSwitches(RunSwitch_DCF) = RunSwitch_DCF_Off
    
                    ReportLevelSaved = RF$(5)
                    RF$(5) = "   "      ' set to no report output
                                        
                    ' Change the ring fence file to use for this level
                    ' Used from here through the next call of Cashflow()
                    Set g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_OPS)

                    Call FiscalDef
                    Call ConsolidateLoan
                    Call CalculateRepayment
       
                    xxx_POSCF = gna_ACFX_OPS
                    xxx_NEGCF = gna_ACFX_ONG
    
                    Call Cashflow
               
                    ' load group-level values
                    ReDim OPEX(gc_nMAXLIFE)
                    For iiiX = 1 To LG
                        For iiiY = 1 To gc_nAMAXOPX
                            A(iiiX, iiiY) = gna_ACX(iiiX, iiiY, 2)
                        Next iiiY
                        For iiiY = gc_nAMINOPX To gc_nAMAXOPX
                            OPEX(iiiX) = OPEX(iiiX) + A(iiiX, iiiY)
                        Next iiiY
                    Next iiiX
                    
                    For iiiX = 1 To my3tt
                        my3(iiiX, 5) = my3Ex(lpa_my3Ex(iiiX), gna_my3Ex_GCX)
                    Next iiiX
                    
                    
                    xRunSwitches(RunSwitch_LVL) = RunSwitch_LVL_Group
                    xRunSwitches(RunSwitch_PAR) = RunSwitch_PAR_On
    
                    ' Change the ring fence file to use for this level
                    ' Used from here through the next call of Cashflow()
                    Set g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_GRP)
                    
                    Call FiscalDef
                    Call ConsolidateLoan
                    Call CalculateRepayment
    
                    xxx_POSCF = gna_ACFX_GPS
                    xxx_NEGCF = gna_ACFX_GNG
    
                    Call Cashflow
    
                    ' load company-level values
                    ReDim OPEX(gc_nMAXLIFE)
                    For iiiX = 1 To LG
                        For iiiY = 1 To gc_nAMAXOPX
                            A(iiiX, iiiY) = gna_ACX(iiiX, iiiY, 3)
                        Next iiiY
                        For iiiY = gc_nAMINOPX To gc_nAMAXOPX
                            OPEX(iiiX) = OPEX(iiiX) + A(iiiX, iiiY)
                        Next iiiY
                    Next iiiX
                    
                    For iiiX = 1 To my3tt
                        my3(iiiX, 5) = my3Ex(lpa_my3Ex(iiiX), gna_my3Ex_CCX)
                    Next iiiX
                    
                    xRunSwitches(RunSwitch_LVL) = RunSwitch_LVL_Company
                    xRunSwitches(RunSwitch_WIN) = RunSwitch_WIN_On
                    
                    ' Change the ring fence file to use for this level
                    ' Used from here through the next call of Cashflow()
                    Set g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_CMP)
                    
                    ' Now a bit of a hack, keeps FiscalDef from writing to the ring fence file for this part of the run
                    ' don't want to write to the file for company-level ring fence because done
                    ' above already. But still have to have a reference to the company-level for reading
                    RunNameSaved = RF$(2)           ' save the actual name
                    RF$(2) = RunNameSaved & "X"     ' fixup to fool ring fence out code
                    Call FiscalDef
                    RF$(2) = RunNameSaved           ' restore the actual name
                    Call ConsolidateLoan
                    Call CalculateRepayment
                    
                    xxx_POSCF = 0
                    xxx_NEGCF = 0
                    
                    ' Change the ring fence file to use for this level
                    ' Don't want the company level data written again, was done above
                    Set g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_DUM)
                    
                    Call Cashflow
                    
                Else
                
                    ' single data run (non pre-tax consol)
                    For iiiX = 1 To LG
                        gna_ACFX(iiiX, gna_ACFX_TPS) = ATotalRevenues(iiiX)
                        gna_ACFX(iiiX, gna_ACFX_TNG) = OPEX(iiiX)
                    Next iiiX
                    
                    For iiiX = 1 To my3tt
                        If (my3(iiiX, 1) > 1 And my3(iiiX, 1) < CPXCategoryCodeBAL) Or my3(iiiX, 1) = CPXCategoryCode_AbandonmentCashExpenditure Then
                            iiiY = my3(iiiX, 3) - YR + 1
                            gna_ACFX(iiiY, gna_ACFX_TNG) = gna_ACFX(iiiY, gna_ACFX_TNG) + my3(iiiX, 5)
                        End If
                    Next iiiX
                    
                    xRunSwitches(RunSwitch_LVL) = RunSwitch_LVL_Operating
                    xRunSwitches(RunSwitch_PAR) = RunSwitch_PAR_Off
                    xRunSwitches(RunSwitch_WIN) = RunSwitch_WIN_Off
                    xRunSwitches(RunSwitch_LMT) = RunSwitch_LMT_Off
                    xRunSwitches(RunSwitch_DCF) = RunSwitch_DCF_Off
    
                    ReportLevelSaved = RF$(5)
                    RF$(5) = "   "      ' set to no report output
                   
                    ' Change the ring fence file to use for this level
                    ' Used from here through the next call of Cashflow()
                    Set g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_OPS)
                    
                    Call FiscalDef
                    Call ConsolidateLoan
                    Call CalculateRepayment
       
                    xxx_POSCF = gna_ACFX_OPS
                    xxx_NEGCF = gna_ACFX_ONG
    
                    Call Cashflow
                    
                    xRunSwitches(RunSwitch_LVL) = RunSwitch_LVL_Group
                    xRunSwitches(RunSwitch_PAR) = RunSwitch_PAR_On
    
                    ' Change the ring fence file to use for this level
                    ' Used from here through the next call of Cashflow()
                    Set g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_GRP)
                    
                    Call FiscalDef
                    Call ConsolidateLoan
                    Call CalculateRepayment
    
                    xxx_POSCF = gna_ACFX_GPS
                    xxx_NEGCF = gna_ACFX_GNG
    
                    Call Cashflow
                    
                    xRunSwitches(RunSwitch_LVL) = RunSwitch_LVL_Company
                    xRunSwitches(RunSwitch_WIN) = RunSwitch_WIN_On
                    
                    ' Change the ring fence file to use for this level
                    ' Used from here through the next call of Cashflow()
                    Set g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_CMP)
                    
                    ' Now a bit of a hack, keeps FiscalDef from writing to the ring fence file for this part of the run
                    ' don't want to write to the file for company-level ring fence because done
                    ' above already. But still have to have a reference to the company-level for reading
                    RunNameSaved = RF$(2)           ' save the actual name
                    RF$(2) = RunNameSaved & "X"     ' fixup to fool ring fence out code
                    Call FiscalDef
                    RF$(2) = RunNameSaved           ' restore the actual name
                    Call ConsolidateLoan
                    Call CalculateRepayment
    
                    xxx_POSCF = 0
                    xxx_NEGCF = 0
    
                    ' Change the ring fence file to use for this level
                    ' Don't want the company level data written again, was done above
                    Set g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_DUM)
                    
                    Call Cashflow
        
                End If
                
              
                ' do the discounted cash flow page
                xRunSwitches(RunSwitch_DCF) = RunSwitch_DCF_Only
                RF$(5) = ReportLevelSaved
                    
                ' Change the ring fence file to use. Don't want ring fence output from here.
                Set g_oRingFenceFile = gfa_RingFenceFiles(gfa_RingFenceFile_DUM)

                Call Cashflow

               Call WriteGNTCon
                  ' 30 Oct 1996 JWD Comment out following
                  ' OXY files replaced by OXY export format
               'Call WriteOXYFiles
              
               ' Add condition on report write
               If RF$(5) = "ALL" Or RF$(5) = "VAR" Or RF$(5) = "SUM" Then
                   WriteOutputReport g_oReport, ReportFileSpec(sRptDir, RN$, RNU, "PRN"), g_oFileSystem
               End If

               ' 23 Sep 2004 JWD (C0839) Write run summary data
               Set l_oRunSumOut = g_oFileSystem.OpenForAppend(sRptDir & RN$ & ".SUM")
               g_oReport.WriteReport l_oRunSumOut, New CCMGiantRunSummarySeqA
               l_oRunSumOut.CloseFile
               ' End (C0839)

            ElseIf Left$(RF$(1), 6) = "CONSOL" Then
49082
                  ' 24 Oct 1996 JWD Replace inline test
                  '  with following GoSub
               GoSub FixupForExportCodes
               
               ReDim xRunSwitches(1 To RunSwitchesCount)
               xRunSwitches(RunSwitch_DCF) = RunSwitch_DCF_On
               
               If Len(RF$(3)) > 0 Then ConCur = Left$(RF$(3), 3)
               Call Cashflow
               
               Call WriteGNTCon
                  
                  ' 30 Oct 1996 JWD Comment out following
                  ' OXY files replaced by OXY export format
               'Call WriteOXYFiles
              
               ' Add condition on report write
               If RF$(5) = "ALL" Or RF$(5) = "VAR" Or RF$(5) = "SUM" Then
                   WriteOutputReport g_oReport, ReportFileSpec(sRptDir, RN$, RNU, "PRN"), g_oFileSystem
               End If
                
               ' 23 Sep 2004 JWD (C0839) Write run summary data
               Set l_oRunSumOut = g_oFileSystem.OpenForAppend(sRptDir & RN$ & ".SUM")
               g_oReport.WriteReport l_oRunSumOut, New CCMGiantRunSummarySeqA
               l_oRunSumOut.CloseFile
               ' End (C0839)

            Else
               MsgBox "MAINEXEC.EXE:Main  Error 253.  Encountered command other than GETDATA or CONSOL in run file.  Program ending"
               Error 253           'unexpected runfile command
            End If
         End If
      End If

99      ' Normal end of run

      g_oRunFileIn.CloseFile
      
      WriteFinder AR, "CASHFLOW"
      '''PushStats "MAINEXEC", dStart
      '''GoSub PrintStats
      TerminateExecution
      Exit Sub
'=======================================================================
100
105
'PrintStats:
' output some execution statistics
'module$ = Date$ & "  " & Time$ & "  " & "LG=" & Right(Str$(LG), 2) & "  CAPEX recs=" & Right$(Space$(3) & Str$(MY3TT), 3)

'hfLog = FreeFile
'fLog = "mainexec.log"
'Open fLog For Append As #hfLog
'   For i = 1 To UBound(sStats)
'      Print #hfLog, module$ & "   " & sStats(i)
'   Next i
'Close #hfLog
'Return
'-----------------------------------------------------------------------
ReadRun:  'READ RUN FILE
      goodname% = False
      'runfilename$ has the run file name
      sRunDir = RunDirectory$
      strTmp = RunDirectory$ + RunFileName$ + ".RUN"
      q% = Len(Dir$(strTmp))
      If q% > 0 Then
         goodname% = True
         strTmp = RunFileName$
      Else
         strTmp = ""
      End If

      RN$ = strTmp
'      DATFIL1$ = RN$
      s$ = RN$ + ".RUN"

      Return
               
'---------------------------------------------------------
FixupForExportCodes:
      ' ~~~~ 24 Oct 1996 JWD
      '   Added this code to test for special export codes
      '   and coerce device type to FIL.  This code was
      '   originally duplicated each place required.
      ' 14 Nov 1996 JWD Add RFR to list of export codes.
   Select Case Left$(RF$(4), 3)
   Case "SP1", "OXY"
      RF$(4) = "FIL"
      RF$(5) = "ALL"
   Case "RFR"
      RF$(4) = "SCR"
      RF$(5) = "   "
   Case Else
   End Select
   Return

'---------------------------------------------------------
RenameMiscellaneousTitlesFile:
      ' Miscellaneous titles file is created in file read
      ' routine but the run number is not determined until
      ' after the procedure is exited and the titles data
      ' is lost, therefore rename the file created data
      ' file read routine to a run specific filename. This
      ' will preserve the titles data for a specific run.
   If Dir(TempDir$ & "USERTITL.DAT") <> "" Then
      strTmp = TempDir$ & "USRTTL" & Format(RNU) & ".DAT"
      If Dir(strTmp) <> "" Then
         Kill strTmp
      End If
      Name TempDir$ & "USERTITL.DAT" As strTmp
   End If
   Return

'---------------------------------------------------------
RenameVariableTitlesFile:
      ' Variable titles file is created in CountryForecast
      ' but the run number is not determined until after
      ' the procedure is exited and the titles data is
      ' lost, therefore rename the file created by
      ' CountryForecast() to a run specific filename. This
      ' will preserve the titles data for a specific run.
   If Dir(TempDir$ & FILEVARTITLES) <> "" Then
      strTmp = TempDir$ & "VARTTL" & Format(RNU) & ".DAT"
      If Dir(strTmp) <> "" Then
         Kill strTmp
      End If
      Name TempDir$ & FILEVARTITLES As strTmp
   End If
   Return
'---------------------------------------------------------
25000 Rem THIS PRINTS SYSTEM ERRORS
      TerminateExecution

End Sub

Sub PushStats(sPgm As String, dStart As Double)

'   Dim u As Integer
'
'   u = UBound(sStats) + 1
'   ReDim Preserve sStats(u) As String
'
'   sStats(u) = Left$(sPgm & Space$(10), 10) & Str$(Timer - dStart)
'
End Sub

Private Function GetConsolCurrency(ByVal sFilename As String) As String
    
    Dim a_sRunFileLine(11) As String
    Dim i As Integer
    Dim hFile As Integer
    Dim sDum As String
    
    hFile = FreeFile
    
    Open sFilename For Input As hFile
    
    Input #hFile, sDum 'Version line
    Input #hFile, sDum 'blank line
    
    Do While Not EOF(hFile)
        Input #hFile, a_sRunFileLine(1), a_sRunFileLine(2), a_sRunFileLine(3), _
                      a_sRunFileLine(4), a_sRunFileLine(5), a_sRunFileLine(6), _
                      a_sRunFileLine(7), a_sRunFileLine(8), a_sRunFileLine(9), _
                      a_sRunFileLine(10), a_sRunFileLine(11)
                      
        If StrComp(Left$(a_sRunFileLine(1), 6), "CONSOL") = 0 Then
            GetConsolCurrency = a_sRunFileLine(3)
        End If
        
    Loop
    Close #hFile
    
End Function
