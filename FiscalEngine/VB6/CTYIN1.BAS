Attribute VB_Name = "CTYIN1"
' $linesize: 132
' $title:    'GIANT v6.1 - 1996                CTYIN1.VBG'
' $subtitle: 'Global Data for GIANT Execution'
' Name:        CTYIN1.BAS
' Function:    GIANT Global Data
'---------------------------------------------------------
' ********************************************************
' *        COPYRIGHT © 1991-2001 IHS ENERGY GROUP        *
' *                 ALL RIGHTS RESERVED                  *
' ********************************************************
' *   This program file is proprietary information of    *
' *                  IHS Energy Group                    *
' *   Unauthorized use for any purpose is prohibited.    *
' ********************************************************
'-----------------------------------------------------------------------
' This file is for the VB version of the system.
'-----------------------------------------------------------------------
' Modifications:
' 6 Feb 1996 JWD
'  -> Add BDebugging indicator to common
'  -> Add variable items not already in common that were
'     written to and read from 'SCRA1.SCR'
'
' 19 Feb 1996 JWD
'  -> Changed array variable RATE() to PARTRATE(), RATE is
'     function in financial libraries and intrinsic
'     function in VB.
'  -> Added default default storage class declaration as
'     Single.
'  -> Change Common to Global for conversion to VB3.
'
' 20 Feb 1996 JWD
'  -> Change variables GNT.DRIVE$, CTY.DRIVE$, RUN.DRIVE$,
'     RPT.DRIVE$, EXCH.DRIVE$ to sGntDir, sCtyDir,
'     sRunDir, sRptDir, sExchDir respectively. VB
'     considered the old names undefined user types.
'  -> Change vlu$ to sVlu, duplicate definition (vlu).
'  -> Change CUR$ to sCur, duplicate definition (CUR()).
'  -> Change CGR$ to sCGR, duplicate definition (CGR()).
'  -> Change CPD$ to sCPD, duplicate definition (CPD()).
'  -> Change DL$ to sDL, duplicate definition (DL()).
'  -> Change DP$ to sDP, duplicate definition (DP()).
'  -> Change MDC$ to sMDC, duplicate definition (MDC()).
'  -> Change PC$ to sPCV, duplicate definition (PC()).
'  -> Change PD$ to sPDV, duplicate definition (PD()).
'  -> Change PR$ to sPRV, duplicate definition (PR()).
'  -> Change RT$ to sRTV, duplicate definition (RT()).
'  -> Change CurrExc$ to sCurrExch, duplicate def.
'
' 21 Feb 1996 JWD
'  -> Change variable type of LG and LFX to Integer.
'
' 25 Aug 1998 JWD
'  -> Change symbol name Decimal$ to strDecimal to
'     eliminate name conflict with reserved word in VB5.
'
' 18 Aug 1999 GDP
'  -> Added g_sDataFileNoExt Global variable to store the
'     data file name without any extension and without the
'     path. This is used in RVSCON to create / read the
'     file used for pre tax / net of participation consolidation
'     the .RVS file has to be in the TEMP dir and has to have
'     the same name as the data file.
'     eliminate name conflict with reserved word in VB5.
'
' 18 Aug 1999 GDP
'  -> Added GPRTE(),OPXRTE(),PRTRTE(),CAPWIN() to store values read from
'     .RVS file for consolidation (net of participation
'
' 14 Nov 2000 GDP
'  -> Added TOTFINANCE() to store values when consolidating FINANCING figures for
'     .RVS file for consolidation
'
' 13 Jun 2001 JWD
'  -> Add public symbol for detail capital expenditure
'     category codes string. (C0332)
'
' 14 Jun 2001 JWD
'  -> Add additional Other Capital items CP4-9 and an
'     Abandonment Capex item ABN to the capex category
'     codes string. (C0333)
'
' 21 Jun 2001 JWD
'  -> Add global symbols to hold values of BAL, BL2, BL3
'     (balance categories). Necessitated by addition of
'     new capital category codes that changed the actual
'     values of the BAL codes (because they moved in the
'     category codes string - OOPS!). (C0339)
'
' 1 Aug 2001 JWD
'  -> Add global symbols for code text for Cash/Accrual
'     option of abandonment funding payments. (C0363)
'  -> Add code text for cash/accrual options to full cpx
'     codes string. (C0363)
'
' 14 Sep 2001 JWD
'  -> Add GrossProduction() array to hold the 100% gross
'     production values across a pre-tax consolidation
'     and which are optionally used to provide the
'     values used to determine sliding scale variable
'     rates. (C0443)
'
' 17 Sep 2001 JWD
'  -> Add UseGrossProductionAmounts variable symbol.
'     (C0443)
'
' 18 Sep 2001 JWD
'  -> Add GrossRevenue() array to hold the 100% gross
'     revenue amounts across a pre-tax consolidation.
'     This will be used for calculating the equivalent
'     production (which is price equivalent) (C0443)
'
' 21 Jan 2004
'  -> Add BDACategoryCodesString symbol. (C0776)
'
' 29 Dec 2004 JWD
'  -> Add declaration of g_nFinanceEvents which is used
'     to record certain events that are reflected in the
'     FINANCE() array. Purpose is to reconstruct financing
'     amounts in pre-tax consolidation. (C0846)
'
' 17 May 2005 JWD
'  -> Add new balance categories BL4-B20 to capital
'     expenditures code string and add new category
'     code symbols. (C0878)
'-----------------------------------------------------------------------
DefSng A-Z

' 30 Jun 2008 JWD
Public GPDPCR() As Single                   ' Depreciation/Recovery Rate difference, by item, %


' 25 Jun 2008 JWD                   ' array of references to ring fence files for different levels
Public gfa_RingFenceFiles() As IEFSFile     ' i.e. for Operating-, Group-, and Company-levels
                                            ' These are in addition to the g_oRingFenceFile symbol
                                            ' which refers to the primary ring fence data.
                                            ' These additional files are set up to keep the changes
                                            ' to the NOC/Govt cash flow calc as separate as possible.
                                            
Public Const gfa_RingFenceFile_OPS = 1      ' ring fence data for operating level
Public Const gfa_RingFenceFile_GRP = 2      ' ring fence data for group level
Public Const gfa_RingFenceFile_CMP = 3      ' ring fence data for company level
Public Const gfa_RingFenceFile_DUM = 4      ' dummy to catch unwanted ring fence output

' 20 Jun 2008 JWD

Public gna_ACX() As Single          ' pre-tax consolidated volumes, revenues, opex at gross-, operating-, group-, company-levels
Public my3Ex() As Single            ' stores computed capex amounts at Gross, Group and Company Level
Public Const gna_my3Ex_TCX = 0      ' Gross-level capital expenditure amount, currency
Public Const gna_my3Ex_OCX = 1      ' Operating-level capital expenditure amount, currency
Public Const gna_my3Ex_GCX = 2      ' Group-level capital expenditure amount, currency
Public Const gna_my3Ex_CCX = 3      ' Company-level capital expenditure amount, currency
Public Const gna_my3Ex_CAT = 4      ' Capital expenditure category
Public Const gna_my3Ex_XMO = 5      ' Month of capital expenditure (1-12)
Public Const gna_my3Ex_XYR = 6      ' Year of capital expenditure
Public Const gna_my3Ex_TAN = 7      ' Tangible portion of capital expenditure, percent

Public Const gna_my3Ex_SizeD2_LB = gna_my3Ex_TCX    ' dimension 2 lower bound
Public Const gna_my3Ex_SizeD2_UB = gna_my3Ex_TAN    ' dimension 2 upper bound

Public gna_ACFX() As Single         ' Positive & negative cash flow array (for discounted cash flow section)

' These following constants are second subscript values for gna_ACFX()
Public Const gna_ACFX_TPS = 1       ' Gross-level positive cash flow
Public Const gna_ACFX_TNG = 2       ' Gross-level negative cash flow

Public Const gna_ACFX_OPS = 3       ' Operating-level positive cash flow
Public Const gna_ACFX_ONG = 4       ' Operating-level negative cash flow

Public Const gna_ACFX_GPS = 5       ' Group-level positive cash flow
Public Const gna_ACFX_GNG = 6       ' Group-level negative cash flow

Public Const gna_ACFX_CPS = 7       ' Company-level positive cash flow
Public Const gna_ACFX_CNG = 8       ' Company-level negative cash flow

Public xxx_POSCF As Integer         ' index to receive POSCF from cash flow
Public xxx_NEGCF As Integer         ' index to receive NEGCF from cash flow


Public xRunSwitches() As Long


Public Const RunSwitch_LVL = 3              ' Calculation level control
Public Const RunSwitch_LVL_Total = 0        ' Total level - gross level cash flows in & out of project
Public Const RunSwitch_LVL_Operating = 1    ' Operating level - gross level cash flows for operating entities combined, no par or win applied
Public Const RunSwitch_LVL_Group = 2        ' Group level - Net of NOC: par applied, no win
Public Const RunSwitch_LVL_Company = 3      ' Company level - Net of NOC & 3d parties: par and win applied

Public Const RunSwitch_PAR = 4              ' Participation calculation control
Public Const RunSwitch_PAR_On = 0           ' apply PAR if present, at location in fiscal def
Public Const RunSwitch_PAR_Off = 1          ' ignore PAR if present in fiscal def for this run

Public Const RunSwitch_WIN = 5              ' Company Working Interest control
Public Const RunSwitch_WIN_On = 0           ' apply WIN if present
Public Const RunSwitch_WIN_Off = 1          ' ignore WIN if present in fiscal def for this run

Public Const RunSwitch_FIN = 6              ' Financing control
Public Const RunSwitch_FIN_On = 0           ' apply FIN if present
Public Const RunSwitch_FIN_Off = 1          ' ignore FIN if present in fiscal def for this run

Public Const RunSwitch_LMT = 7              ' Economic Limit control
Public Const RunSwitch_LMT_On = 0           ' apply LMT if present
Public Const RunSwitch_LMT_Off = 1          ' ignore LMT if present in fiscal def for this run

Public Const RunSwitch_DCF = 8              ' CASHFLOW discounted cash flow calculation control
Public Const RunSwitch_DCF_On = 0           ' normal operation, do atcf and dcf
Public Const RunSwitch_DCF_Off = 1          ' suppress DCF calc and output, exit before dcf code
Public Const RunSwitch_DCF_Only = 2         ' only do the DCF code

Public Const RunSwitchesCount = RunSwitch_DCF





' 29 Dec 2004 JWD (C0846) Add next to record financing events
'       This is a bit map that stores information about the
'       financing for a particular entity.
'       Bit position:   1 - Indicates if financing command in fiscal def
'                       2 - Indicates if Win applied to financing
'                       3 - Indicates if Par applied to financing
'       Values:         0 - No financing
'                       1 - Financing. Amounts are net to the company only (FIN after PAR and WIN)
'                       3 - Financing. Amounts are net to company but gross shared by 3rd party only (FIN before WIN, FIN after PAR if PAR present)
'                       5 - Financing. Amounts are net to company but gross shared by NOC only (FIN before PAR applied, FIN after WIN if WIN present)
'                       7 - Financing. Amounts are net to company but gross shared by 3rd party and NOC (FIN before PAR and WIN if WIN and PAR present)
Public g_nFinanceEvents As Long
' Symbols for bit map position values
Public Const gc_nFinanceEvents_FIN = 1      ' FIN encountered
Public Const gc_nFinanceEvents_WIN = 2      ' FIN encountered, then WIN
Public Const gc_nFinanceEvents_PAR = 4      ' FIN encountered, then PAR


' Effective interests used to calculate
' 3rd party and NOC cash flow values for
' pre-tax consolidation calculations
' 2-d array: subscript 1 is period,
'            subscript 2 is a calcuted interest for an entity
'            Subscript 2 values are:
'                   1 - company effective working interest based on revenues
'                   2 - NOC effective participation based on revenues
'                   3 - company effective working interest based on operating expenses
'                   4 - NOC effective operating expense participation
'                   5 - company effective working interest share of total repayments (TOTPMT) that is company's NOC repayment
'                   6 - company effective working interest share of total repayments (TOTPMT) that is company's partner reimbursement
'                   7 - company effective interest in financing cash flow
'                   8 - NOC effective interest in financing cash flow
Public EffInts() As Single
' Symbols for EffInts() subscript 2 values:
Public Const gc_nEffInts_WIN = 1
Public Const gc_nEffInts_PAR = 2
Public Const gc_nEffInts_WOX = 3
Public Const gc_nEffInts_POX = 4
Public Const gc_nEffInts_WNR = 5
Public Const gc_nEffInts_WPR = 6
Public Const gc_nEffInts_WFN = 7
Public Const gc_nEffInts_PFN = 8
Public Const gc_nEffIntsSIZED2 = gc_nEffInts_PFN

' Effective interests to calculate
' 3rd party and NOC capital expenditure
' amounts for pre-tax consolidations
' 2-d array: Subscript 1 is capital expenditure item index
'               There is a one-to-one correspondence between EffIntsX()
'               and MY3() with respect to the first dimension of the array
'               i. e. Ubound(MY3, 1) = Ubound(EffIntsX, 1).
'            Subscript 2 is a calculated effective interest for the capex item
'            Subscript 2 values are:
'                   1 - expenditure category
'                   2 - expenditure month
'                   3 - expenditure year
'                   4 - tangible percentage
'                   5 - company effective working interest in expenditure
'                   6 - NOC effective working interest in expenditure
'                   7 - company reimbursement for partner carries relative to group expenditures
Public EffIntsX() As Single
' Symbols for EffIntsX() subscript 2 values:
Public Const gc_nEffIntsX_CAT = 1
Public Const gc_nEffIntsX_XMO = 2
Public Const gc_nEffIntsX_XYR = 3
Public Const gc_nEffIntsX_TAN = 4
Public Const gc_nEffIntsX_WIN = 5
Public Const gc_nEffIntsX_PAR = 6
Public Const gc_nEffIntsX_BUR = 7
Public Const gc_nEffIntsXSIZED2 = gc_nEffIntsX_BUR

' Symbols for MYC() subscript 2 values:
Public Const gc_nMYC_CAT = 1            ' capital expenditure category
Public Const gc_nMYC_XMO = 2            ' month of expenditure
Public Const gc_nMYC_XYR = 3            ' year of expenditure
Public Const gc_nMYC_TAN = 4            ' tangible portion of expenditure, percent
Public Const gc_nMYC_AMT = 5            ' consolidated company capital amount

' Symbols for MYC() new columns (consolidated capital expenditures)
Public Const gc_nMYC_GRS = 8            ' consolidated gross expenditure
Public Const gc_nMYC_CMP = 9            ' consolidated company capital amount
Public Const gc_nMYC_3DP = 10           ' consolidated 3rd party capital amount
Public Const gc_nMYC_NOC = 11           ' consolidated NOC capital amount
Public Const gc_nMYC_BUR = 12           ' consolidated company reimbursement amount
Public Const gc_nMYC_TCX = 13           ' consolidated gross-level capex (from my3Ex())(doesn't include bonuses)
Public Const gc_nMYC_OCX = 14           ' consolidated operating-level capex (from my3Ex())
Public Const gc_nMYC_GCX = 15           ' consolidated group-level capex (from my3Ex())
Public Const gc_nMYC_CCX = 16           ' consolidated company-level capex (from my3Ex())
Public Const gc_nMYCSIZED2 = gc_nMYC_CCX

' Symbols for existing columns of MY3() (capital expenditures)
Public Const gc_nMY3_CAT = 1            ' capital expenditure category
Public Const gc_nMY3_XMO = 2            ' month of expenditure
Public Const gc_nMY3_XYR = 3            ' year of expenditure
Public Const gc_nMY3_TAN = 4            ' tangible portion of expenditure, percent
Public Const gc_nMY3_AMT = 5            ' capital expenditure amount
Public Const gc_nMY3_WIN = 6            ' company working interest share of expenditure amount
Public Const gc_nMY3_BUR = 7            ' partner reimbursement amount expressed as percentage of carried amount
' End (C0846)

'<<<<<< 18 Sep 2001 JWD (C0443)
' Store the consolidated gross (100%) revenues by product
' for use in calculating the gross equivalent production.
Public GrossRevenue() As Single
'>>>>>> End (C0443)

'<<<<<< 17 Sep 2001 JWD (C0443)
' Store the client's preference that sliding scale rates
' for consolidations use the gross rather than the net
' production.
Public UseGrossProductionAmounts As Boolean
'>>>>>> End (C0443)

'<<<<<< 14 Sep 2001 JWD (C0443)
' Stores the consolidated gross (100%) production for use
' in selecting sliding scale rates based on production.
Public GrossProduction() As Single
'>>>>>> End (C0443)

' 21 Jan 2004 JWD (C0776)
' Base forecast codes
Public Const BDACategoryCodesString = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0RESWINOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5"


'<<<<<< 1 Aug 2001 JWD (C0363)
Public Const CPXCategoryCodeString_AbandonmentCashExpenditure = "AB1"
Public CPXCategoryCode_AbandonmentCashExpenditure As Integer
Public Const CPXCategoryCodeString_AbandonmentAccrualEntry = "AB2"
Public CPXCategoryCode_AbandonmentAccrualEntry As Integer

' 17 May 2005 JWD
Public Const CPXCategoryCodesString = "BNSLSERENGEOEDHEDSADHASCDNPDVPPLFFCLTRNEORCP1CP2CP3CP4CP5CP6CP7CP8CP9ABNBALBL2BL3BL4BL5BL6BL7BL8BL9BL0B11B12B13B14B15B16B17B18B19B20" & CPXCategoryCodeString_AbandonmentCashExpenditure & CPXCategoryCodeString_AbandonmentAccrualEntry
' Was:
'Public Const CPXCategoryCodesString = "BNSLSERENGEOEDHEDSADHASCDNPDVPPLFFCLTRNEORCP1CP2CP3CP4CP5CP6CP7CP8CP9ABNBALBL2BL3" & CPXCategoryCodeString_AbandonmentCashExpenditure & CPXCategoryCodeString_AbandonmentAccrualEntry
'~~~~~~ was:
''<<<<<< 14 Jun 2001 JWD
'Public Const CPXCategoryCodesString = "BNSLSERENGEOEDHEDSADHASCDNPDVPPLFFCLTRNEORCP1CP2CP3CP4CP5CP6CP7CP8CP9ABNBALBL2BL3"
''~~~~~~ was:
'''<<<<<< 13 Jun 2001 JWD                ....+....1....+....2....+....3....+....4....+....5....+....6....+....7....+....8....+....9....+....0
''Public Const CPXCategoryCodesString = "BNSLSERENGEOEDHEDSADHASCDNPDVPPLFFCLTRNEORCP1CP2CP3BALBL2BL3"
'''>>>>>> End 13 Jun 2001
''>>>>>> End 14 Jun 2001
'>>>>>> End (C0363)

'<<<<<< 21 Jun 2001 JWD (C0339)
Public CPXCategoryCodeBAL As Integer
Public CPXCategoryCodeBL2 As Integer
Public CPXCategoryCodeBL3 As Integer
'>>>>>> End C0339

' 17 May 2005 JWD (C0878) Add symbols for last BAL code
' These are used assuming that all of the balance category
' codes will be, as other codes are, sequential and
' contiguous integer values, the sequence uninterrupted
' by other codes that are not balance categories.
' As of this date (17 May 2005), B20 is the last balance
' category that is provided for. Its value will also be
' referenced as the last value (represented as BLn).
Public Const CPXCategoryCodeString_BLn = "B20"
Public CPXCategoryCodeBLn As Integer
' End (C0878)

'GIANT 6.1 Start JWD 6 Feb 1996 ----------------------------------------
   ' Debugging switch - was local
   Global BDebugging As Integer

   '  Now store data items formerly recorded on SCRA1.SCR
   Global VLM()      As Single
   Global VOL()      As Single
   Global REV()      As Single
   Global DPC()      As Single
   Global PCE()      As Single
   Global FCRD()     As Single
   Global OthBon()   As Single
   Global DDT()      As Single
'GIANT 6.1 End ---------------------------------------------------------

'GIANT 6.0 Start JWD 25 Mar 1995
   Global AppDir$         ' location of system (shared) files
   Global TempDir$        ' location of temporary (scratch) files
   Global Program$        ' current executing program/module name
   Global FConfig$        ' filespec for GNTCONFG.DAT
   Global FOxfil$         ' filespec for OXFIL.DAT
   Global FExchng$        ' filespec for GNTEXCH.DAT
   Global FChuong$        ' filespec for CHUONG.DAT
   Global FOxyRun$        ' filespec for OXYRUN~.FIL
   Global FPStatus$       ' filespec for program completion status
'GIANT 6.0 End -----------------

'Giant 5.4 start -----------------------------------
'This record type added to store the contents if the RATScreen
'  (RTO/RT1 Ratio Definition screen) for use by RatioDefinition
'  sub in Util5 (used when RatioDefinition called by CalcIRR)

   Type RATType
      var As String * 3                  'variable
      numden As String * 3               'numerator/denominator
      fnc(1 To 8) As String * 1           'fnc
      Cat(1 To 8)  As String * 3         'categories 1 - 8
   End Type
   Global RATRecs%, rat() As RATType      'RTO/RT1 Ratio Definition

'Giant 5.4 end -------------------------------------

'Giant 5.4 Start --------------
   Global XYear As Integer    'loop counter - year
   Global XFirst As Integer   'line number of first variable after ITB (iteration begin)
   Global XLast As Integer    'line number of last variable before ITE (iteration end)
   Global XIter As Integer    'switch.  0 = we are NOT iterating, 1 = we are iterating.
   Global SubVarsNum() As Single, SubVarsDen()  As Single
'Giant 5.4 End --------------


        '8-26-92 - put version # in common
   Global ver$
   Global vernumber  As Single
   Global ProgVersion$

        '8-21-92 store final win % and partic rate in common for report display
   Global FINALPARTIC As Single, FinalWin As Single

        'sensitivities data
   Global vlu As Single, vlc As Single, vlr As Single, sVlu As String

        'general
   Global AR As Integer    'used throughout to see where we came from
   Global PPR As Integer   'primary product (1=oil 2=gas)
   Global SEC As Integer   'secondary product (1=oil 2=gas)
   Global Camefrom$        'program name that chained to the current program

        'these items are used by the file listing routines
        '  in FORECAST & CNTYFCST (when the user has requested a data
        '  or country file listing in his run file
   Global datapath$, casenm$, casedesc$
   Global CtyPath$, CtyNm$, CtyDesc$
   Global ExchNm$, EXCDesc$   'name & description of the exchange rate file

   Global DNT$(), DNTRecs%       'data file notes
   Global CNT$(), CNTRecs%       'country file notes
   Global EXTNote$(), ExtNotes%  'external file notes

                'user defined codes
                '   Global nousercodes%    'number of codes
                '   Global usercode$()     'list of user codes

        'files
   ' Added GDP 18/8/99
   Global g_sDataFileNoExt As String ' data file name with no ext or path
   
   Global N1$            'data file name from run file
   Global N1C$           'country file name from run file

        'dates
   Global L1()           'holds various dates needed for consolidation
   Global L2()           'temporary version of L1()
   Global mo             'project start month (from general parameters)
   Global YR             'project start year (from general parameters)
   Global M1             'production start month (from general parameters)
   Global Y1             'production start year (from general parameters)
   Global M2             'discovery start month (from general parameters)
   Global Y2             'discovery start year (from general parameters)
   Global M3             'discount start month (from general parameters)
   Global Y3             'discount start year (from general parameters)

        'misc dates for forecasting routines
   Global ProdYr%, ProdMo%, ProjYr%, ProjMo%, curvelife, proddelay

        'life
   Global LG As Integer  'life (reporting years(integer)) calculated in forecast
   Global LFX As Integer 'producing life (integer) (rounded up to whole year)  MKD 3-4-91 changed name from LF
   Global LFI As Single  'producing life (actual) (ie 23.15 years)
   Global LGI As Single  'project life (actual) (ie 27.5 years)
   Global AbandonmentPlacementOffset As Single ' is set to 1 when abandonment placement option Last Year + 1 is set
   
        'these two items are used by forecasting routines as flags wether
        '  forecasted curves are relative to PJM or PDM and wether the last
        '  year of curve is to be pro-rated to a fractional portion of the period
   Global AdjustLastYear%
   Global MonthRelative%

        'reporting
   '<<<<<< 25 Aug 1998 JWD Change following symbol name
   Global strDecimal$    'decimal places on reports
   '>>>>>> End 25 Aug 1998

        'titles
   Global TL$()          'Variable Titles screen
   Global TLT As Integer '# items in TL()
   Global PN$()          'Data File Titles from General Parameters (4 lines)
   Global PNC$()         'Country File Titles from Country Misc Parameters screen (4 lines)

        'paths

   '~~~~ 20 Feb 1996 JWD - Changed variable names.
   Global sGntDir As String   'path for data files
   Global sCtyDir As String   'path for country files
   Global sRunDir As String   'path for run files
   Global sRptDir As String   'path for report files
   Global sExchDir As String  'path for exchange rate file
   '~~~~ 20 Feb 1996 JWD - Add external table directory
   Global sExtDir As String   'path for external tables files
   '~~~~ End of 20 Feb 1996

        'flags
   Global BURS As Integer '(1 or 0 (T/F)) (if 1 then REIM() is NOT zero)
   Global PRTA As Integer '(1 or 0 (T/F)) (PAR line(s) in FISCAL DEF screen)
   Global WINT As Integer '(1 or 0 (T/F)) (WIN line(s) in FISCAL DEF screen)

        'currency
   Global ConCur$          'currency code for consolidation (see sCur)
   Global sCur As String   'currency code
   Global CUR() As Single  'annual exchange rate - calculated in CURRENCY.BAS
   Global CURT As Integer  'Counts "CUR" lines in country file. We only
                           '  do Revaluation in DEPREC for first
                           '  "CUR" entry
   'Added by Glyn 16/04/99
   Global g_sConsolCur As String ' Currency of input data (one per GETDATA)
   
   'Added by Glyn 27/08/99
   Global g_bPTCons As Boolean ' Flag used when current run is a consolidation
   
   Global LCur() As Single 'the factor of the last currency line
                           'encountered.  That way if more than
                           'one CUR line, we can convert back
                           'to dollars before adjusting to new rate.

        '   Global CurrExc!()     'exchange rates data
   Global sCurrExch() As String   'exchange rates titles
        '   Global CurrencyRecs%  'number of exchange rate items

        'run file
   Global RF$()          'array for a run file line
   Global RFT As Integer '# lines in run file
   Global RN$            'run file name
   Global RNU As Integer 'run number (of the current run in case of consolidation)

        'interests
   Global WIN() As Single  'annual working interest  - calculated in FISCAL.BAS
   Global WINC() As Single 'working interest for each CAPEX item in MY3()
   
   ' Added GDP 18/08/99
   Global CAPWIN() As Single ' Capex working interest used in consolidation (read from RVSCON)

        'rate of return
   Global inc1() As Single 'these two are adjustments for reports used
   Global ded1() As Single '  with IRR, RTO, & RT1 calculations

        'miscellaneous (including screen  and output data from forecasting)
   Global A() As Single    'forecasted and inflated PDC and BDA recs
   Global AC() As Single   'holds consolidation of a() in FORECAST.BAS (temporarily)

   Global AMTLOAN() As Single 'single amount loan records
   Global AMTLT As Integer    '# items in AMTLOAN()

   Global B() As Single    'annual array from Country Annual Forecast form

   Global BN$()            'bonuses
   Global BNT As Integer   '# items in BN$()

   Global BONS() As Single 'production bonuses by year - calculated in BONUS.BAS

   Global CC() As Single   'consolidated CAPEX lines (pointer is CCT)
   Global CCT As Integer   '# of concatenated CAPEX lines in consolidation

   Global CLG$()           'Ceiling definition screen
   Global CLGTT As Integer '# items in CLG()

   Global clngs() As Single 'annual cost recovery ceiling amounts

   Global CGR() As Single  'Ceiling Rates screen (all but VAR column)
   Global sCGR() As String 'VARiable column of Ceiling rates screen
   Global CGRT As Integer  '# items in CGR() & sCGR()

   Global sCPD() As String 'Compound Rates variable column
   Global CPD() As Single  'Compound Rates other columns
   Global CPDT As Integer  '# items in CPD()

   Global DFL() As Single  'deflator values 1-LG   'filled out in Forecast.BAS

   Global DIS As Single    'Discovery bonuses - calculated in BONUS.BAS

   Global DiscMthd As Integer 'Discount Method from Discounting Parameters screen

   Global DL() As Single   'Depletion screen (all but VAR column)
   Global sDL() As String  'VARiable column of Depletion screen
   Global DLT As Integer   '# items in DL() & sDL()

   Global dp() As Single   'Depreciation screenn (all but VAR column)
   Global sDP() As String  'VARiable column of Depreciation screen
   Global DPT As Integer   '# items in DPT() & DPT$()

   Global EXPLOAN() As Single 'exp based loan records
   Global EXPLT As Integer    '# items in EXPLOAN()

   Global FINANCE() As Single 'net financing effect - shown on ATLF page
   Global FVAR$()             'contains the 3-letter variable code

   Global GM() As Single      'Country Misc Parameters screen

   Global gn() As Single      'GENERAL PARAMETERS array (OLD) - not all elements used!

   Global GPBASE() As Single  'group share of CAPEX - calculated in PARTIC.BAS

   Global GPRATE() As Single  'group participation - calculated in PARTIC.BAS
   
   ' added GDP 18/8/99
   Global GPRTE() As Single   'group participation used in consolidation (read from RVSCON)
   
   Global INTRST() As Single  'annual interest payments - calculated in LOAN.BAS
   Global Inflate() As Single 'inflation rates for oil and gas prices

   Global LOAN() As Single    'annual loan receipts - calculated in LOAN.BAS

   Global sMDC() As String    'Miscellaneous Depreciation/Cost Recovery parameters
   Global MDC() As Single
   Global MDCT As Integer

   Global my3() As Single  'CAPEX array
   Global MY3T As Integer  '# items in data file MY3()
   Global my3tt As Integer 'total CAPEX lines (# data file MY3() + country file my3() records)

   Global OPEX() As Single 'total annual OPEX - calculated in FORECAST

   Global PC() As Single   'Price Definition screen (all but VAR column)
   Global sPCV() As String 'VARiable column of Price definition screen
   Global PCT As Integer   '# items in PC() & sPCV()

   Global PD() As Single   'Prepaid-Deferred tax screen (all but VAR column)
   Global sPDV() As String 'VARiable column of Prepaid-Deferred tax screen
   Global PDTT As Integer  '# items in PDT() & PDT$()

   Global sPRV() As String 'categories in Govt. Participation
   Global PR() As Single   'govt participation rates (screen image)
   Global PRT As Integer   '# items in PR()

   Global PRINC() As Single   'annual principal payments - calculated in LOAN.BAS

   Global PT() As Single   'govt participation terms (screen image)
   Global PTT As Integer   '# items in PT()

   Global PARTRATE() As Single   'This is government share of income
   Global OPEXRATE() As Single   'This is government share of opex - Version 5.0
   
   ' Added GDP 18/8/99
   Global PRTRTE() As Single ' government share of income used in consolidation (read from RVSCON)
   Global OPXRTE() As Single ' government share of opex used in conmsolidation (read from RVSCON)

   Global REIM() As Single 'reimbursement of carried capital - calculated in FISCAL.BAS

   Global RLD() As Single  'this is Taxable Income in Fiscal (revenue less deductions)

   Global RT() As Single   'Variable Rates screen (all but VAR column)
   Global sRTV() As String 'VARiable column of Variable Rates screen
   Global RTT As Integer   '# items in RT() & sRTV()

   Global RVN() As Single  'annual values of each item in Fiscal Definition

   Global SEQ() As Single  'cost recovery sequence screen image

   Global SIG As Single    'Signature bonuses - calculated in BONUS.BAS

   Global TD$()            'fiscal definition (screen image)
   Global TDT As Integer   '# items in TD()

   Global TM() As Single   'Misc Variable Parameters screen (all but VAR column)
   Global sTMV() As String 'VARiable column of Misc Variable Parameters screen
   Global TMT As Integer   '# items in TM() & sTMV()

   Global TOTPMT() As Single  'total govt repayment - calculated in REPAY.BAS
   ' Added GDP 10/9/99
   Global TOTREPAY() As Single
   
   'Added GDP 14/11/2000
   Global TOTFINANCE() As Single
   
        'cashflows
   Global GVCF() As Single 'govt cashflows
   Global PSCF() As Single 'positive cashflows ("+" on FDef) revenues
   Global NGCF() As Single 'negative cashflows ("-" on FDef) govt rev

'OXY ITEMS
   Global NGCFT() As Single   'negative cashflows ("T" on FDef) 3rd party
   Global NGCFA() As Single   'negative cashflows ("A" on FDef) adjustments
   Global NGCFU() As Single   'negative cashflows ("U" on FDef) US taxes
        'counters

   Global PS%            'positive cashflows ("+" on FDef) co. rev
   Global NEG%           'negative cashflows ("-" on FDef) govt rev
   Global NEGT%          'negative cashflows ("T" on FDef) 3d party rev
   Global NEGA%          'negative cashflows ("A" on FDef) adjustments
   Global NEGU%          'negative cashflows ("U" on FDef) US taxes
   Global TAUItems%      'sum of NEGT%, NEGA%, & NEGU%


        'flags
   Global USTT As Integer 'presence of "U" in cashflow column in FDEF
   Global ADJT As Integer 'presence of "U" (Adjustment) in cashflow column in FDEF
   Global TRDT As Integer 'presence of "T" (Third Party) in cashflow column in FDEF


'********************************************************************

'unknown items
   Global PPS, PPT, FO1, FO2, LGH, LFH, RNP$, PRCCD$(), PNCT, GMT

'********************************************************************

'new items for Occidental Petroleum project - Version 5.3  10-12-92

        'locations of network and local serial number files. These items
        '  are stored in GNTOXY.CFG in the current directory
   Global NetSerPath$
   Global NetSerFile$
   Global LocalSerPath$
   Global LocalSerFile$
        'serial number and suffix from network or local serial# file
   Global sno!
   Global snid$


'THE FOLLOWING ARE OXY DATA ITEMS (PARADOX DATABASE ITEMS)
        'This is the Paradox UNIQUE KEY. It appears in all 4 files
   Global Serial$

        'store data put into GIANT.ANN file. This file contains the annual
        '  data about a run.
        'There is ONE entry per YEAR per RUN.
   Global AData!()

        'store data put into GIANT.PRJ file. This file contains the data
        '  about a run.
        'There is ONE entry per RUN.
   Global HistSer$
                'Date$ - use the DATE$ function
   Global Status$, sType As String, Constat$, Class$, Country$, Block$
   Global Field$, sCase As String, FieldDesc$, Region$
                'Time$ - use the TIME$ function
   Global Primary$, OnOff$, Success!, ID$, RunType$, Who$, PriceBase$
   Global GVerNo$, Tag1$, Tag2$, Tag3$, Tag4$, Tag5$
   Global WinComp!, WinPart!, WinGovt!, WinExp!, WinNet!, WinRev!
   Global WinNCF!, DsctDate$, DsctMeth$, USTxRate!, PUST$, PUSB$
   Global PNIB$, PNIA$, WatDepth!, GTake!, GTake10!, GTake15!, GTake20!
   Global GntROR!, GntNCF!, GntNPV10!, GntNPV15!, GntNPV20!
   Global GntPI!, GntPI10!, GntPI15!, GntPI20!
   Global GntPayot!, GntDscvY!

        'store data put into GIANT.DOC file. This file contains the data
        '  about a run.  Contains info about the files used in a run.
        'There is ONE entry per RUN.

   Global GTitl1$
   Global GTitl2$
   Global GTitl3$
   Global GTitl4$
   Global CTitl1$
   Global CTitl2$
   Global CTitl3$
   Global CTitl4$
   Global GNTDesc$
   Global EXTDesc$
   Global GNTFile$
   Global CTYFile$
   Global RUNFile$
   Global EXTFile$
   Global EXCFile$
   Global SensDat$
   Global SensCty$
        'store data put into GIANT.NOT file. This file contains the data
        '  about a run.  This relates to the notes fields in a file.
        'There is ONE entry per RUN.

   Global RunLines$()
   Global RunNotes$()
   Global RunNoteRecs%


   Global NoType$
   Global LineNo%
   Global Notes$

'variables needed for oxy reports & database
   Global ttlcap!         'total capital (from grossrpt.exe)


'4-22-93 - ver 5.3  added these items to common to facilitate moving
'  ForecastSetLife from FORECAST.BAS to GNTFCST2.BAS
   Global Maxlife%, PrimaryStart$

'-------------------------------------------------------------------
        'these are used to make x and XI in CASHFLOW:38500 available to
        '  GNTOXYC.EXE:LoadOXYConsol
   Global pointerX%, pointerXI%

'OXY database items (consolidated values)

   Global PriorGProdEq!, PriorGExpCap!

        'store data put into GIANT.ANN file. This file contains the annual
        '  data about a run.
        'There is ONE entry per YEAR per RUN.
   Global ADataCon!()

        'store data put into GIANT.PRJ file. This file contains the data
        '  about a run.
        'There is ONE entry per RUN.
   Global HistSerCon$
   Global StatusCon$, TypeCon$, ConstatCon$, ClassCon$
   Global CountryCon$, BlockCon$, FieldCon$, CaseCon$
   Global FieldDescCon$, RegionCon$, PrimaryCon$, OnOffCon$
   Global SuccessCon!, IDCon$, RunTypeCon$, WhoCon$, PriceBaseCon$
   Global GVerNoCon$, Tag1Con$, Tag2Con$, Tag3Con$, Tag4Con$, Tag5Con$
   Global WinCompCon!, WinPartCon!, WinGovtCon!, WinExpCon!
   Global WinNetCon!, WinRevCon!, WinNCFCon!, DsctDateCon$
   Global DsctMethCon$, USTxRateCon!, PUSTCon$, PUSBCon$
   Global PNIBCon$, PNIACon$, WatDepthCon!
   Global GTakeCon!, GTake10Con!, GTake15Con!, GTake20Con!
   Global GntRORCon!, GntNCFCon!, GntNPV10Con!, GntNPV15Con!
   Global GntNPV20Con!, GntPICon!, GntPI10Con!, GntPI15Con!
   Global GntPI20Con!, GntPayotCon!, GntDscvYCon!
   Global OXYGrRevCon!, OXYTtlPosCon!

        'store data put into GIANT.DOC file. This file contains the data
        '  about a run.  Contains info about the files used in a run.
        'There is ONE entry per RUN.

   Global GTitl1Con$
   Global GTitl2Con$
   Global GTitl3Con$
   Global GTitl4Con$
   Global CTitl1Con$
   Global CTitl2Con$
   Global CTitl3Con$
   Global CTitl4Con$
   Global GNTDescCon$, CtyDescCon$, EXCDescCon$, EXTDescCon$
   Global GNTFileCon$
   Global CTYFileCon$
   Global RUNFileCon$
   Global EXTFileCon$
   Global EXCFileCon$
   Global SensDatCon$
   Global SensCtyCon$
        'store data put into GIANT.NOT file. This file contains the data
        '  about a run.  This relates to the notes fields in a file.
        'There is ONE entry per RUN.

   Global RunLinesCon$()
   Global RunNotesCon$()


   Global NoTypeCon$
   Global LineNoCon%
   Global NotesCon$

'variables needed for oxy reports & database
   Global ttlcapCon!         'total capital (from grossrpt.exe)

'End of OXY Database items
'********************************************************************

