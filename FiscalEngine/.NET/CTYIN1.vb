Option Strict Off
Option Explicit On
Module CTYIN1
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
	'UPGRADE_NOTE: DefSng A-Z statement was removed. Variables were explicitly declared as type Single. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="92AFD3E3-440D-4D49-A8BF-580D74A8C9F2"'
	
	' 30 Jun 2008 JWD
	Public GPDPCR() As Single ' Depreciation/Recovery Rate difference, by item, %
	
	
	' 25 Jun 2008 JWD                   ' array of references to ring fence files for different levels
	Public gfa_RingFenceFiles() As IEFSFile ' i.e. for Operating-, Group-, and Company-levels
	' These are in addition to the g_oRingFenceFile symbol
	' which refers to the primary ring fence data.
	' These additional files are set up to keep the changes
	' to the NOC/Govt cash flow calc as separate as possible.
	
	Public Const gfa_RingFenceFile_OPS As Short = 1 ' ring fence data for operating level
	Public Const gfa_RingFenceFile_GRP As Short = 2 ' ring fence data for group level
	Public Const gfa_RingFenceFile_CMP As Short = 3 ' ring fence data for company level
	Public Const gfa_RingFenceFile_DUM As Short = 4 ' dummy to catch unwanted ring fence output
	
	' 20 Jun 2008 JWD
	
    Public gna_ACX(,,) As Single ' pre-tax consolidated volumes, revenues, opex at gross-, operating-, group-, company-levels
    Public my3Ex(,) As Single ' stores computed capex amounts at Gross, Group and Company Level
	Public Const gna_my3Ex_TCX As Short = 0 ' Gross-level capital expenditure amount, currency
	Public Const gna_my3Ex_OCX As Short = 1 ' Operating-level capital expenditure amount, currency
	Public Const gna_my3Ex_GCX As Short = 2 ' Group-level capital expenditure amount, currency
	Public Const gna_my3Ex_CCX As Short = 3 ' Company-level capital expenditure amount, currency
	Public Const gna_my3Ex_CAT As Short = 4 ' Capital expenditure category
	Public Const gna_my3Ex_XMO As Short = 5 ' Month of capital expenditure (1-12)
	Public Const gna_my3Ex_XYR As Short = 6 ' Year of capital expenditure
	Public Const gna_my3Ex_TAN As Short = 7 ' Tangible portion of capital expenditure, percent
	
	Public Const gna_my3Ex_SizeD2_LB As Short = gna_my3Ex_TCX ' dimension 2 lower bound
	Public Const gna_my3Ex_SizeD2_UB As Short = gna_my3Ex_TAN ' dimension 2 upper bound
	
    Public gna_ACFX(,) As Single ' Positive & negative cash flow array (for discounted cash flow section)
	
	' These following constants are second subscript values for gna_ACFX()
	Public Const gna_ACFX_TPS As Short = 1 ' Gross-level positive cash flow
	Public Const gna_ACFX_TNG As Short = 2 ' Gross-level negative cash flow
	
	Public Const gna_ACFX_OPS As Short = 3 ' Operating-level positive cash flow
	Public Const gna_ACFX_ONG As Short = 4 ' Operating-level negative cash flow
	
	Public Const gna_ACFX_GPS As Short = 5 ' Group-level positive cash flow
	Public Const gna_ACFX_GNG As Short = 6 ' Group-level negative cash flow
	
	Public Const gna_ACFX_CPS As Short = 7 ' Company-level positive cash flow
	Public Const gna_ACFX_CNG As Short = 8 ' Company-level negative cash flow
	
	Public xxx_POSCF As Short ' index to receive POSCF from cash flow
	Public xxx_NEGCF As Short ' index to receive NEGCF from cash flow
	
	
	Public xRunSwitches() As Integer
	
	
	Public Const RunSwitch_LVL As Short = 3 ' Calculation level control
	Public Const RunSwitch_LVL_Total As Short = 0 ' Total level - gross level cash flows in & out of project
	Public Const RunSwitch_LVL_Operating As Short = 1 ' Operating level - gross level cash flows for operating entities combined, no par or win applied
	Public Const RunSwitch_LVL_Group As Short = 2 ' Group level - Net of NOC: par applied, no win
	Public Const RunSwitch_LVL_Company As Short = 3 ' Company level - Net of NOC & 3d parties: par and win applied
	
	Public Const RunSwitch_PAR As Short = 4 ' Participation calculation control
	Public Const RunSwitch_PAR_On As Short = 0 ' apply PAR if present, at location in fiscal def
	Public Const RunSwitch_PAR_Off As Short = 1 ' ignore PAR if present in fiscal def for this run
	
	Public Const RunSwitch_WIN As Short = 5 ' Company Working Interest control
	Public Const RunSwitch_WIN_On As Short = 0 ' apply WIN if present
	Public Const RunSwitch_WIN_Off As Short = 1 ' ignore WIN if present in fiscal def for this run
	
	Public Const RunSwitch_FIN As Short = 6 ' Financing control
	Public Const RunSwitch_FIN_On As Short = 0 ' apply FIN if present
	Public Const RunSwitch_FIN_Off As Short = 1 ' ignore FIN if present in fiscal def for this run
	
	Public Const RunSwitch_LMT As Short = 7 ' Economic Limit control
	Public Const RunSwitch_LMT_On As Short = 0 ' apply LMT if present
	Public Const RunSwitch_LMT_Off As Short = 1 ' ignore LMT if present in fiscal def for this run
	
	Public Const RunSwitch_DCF As Short = 8 ' CASHFLOW discounted cash flow calculation control
	Public Const RunSwitch_DCF_On As Short = 0 ' normal operation, do atcf and dcf
	Public Const RunSwitch_DCF_Off As Short = 1 ' suppress DCF calc and output, exit before dcf code
	Public Const RunSwitch_DCF_Only As Short = 2 ' only do the DCF code
	
	Public Const RunSwitchesCount As Short = RunSwitch_DCF
	
	
	
	
	
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
	Public g_nFinanceEvents As Integer
	' Symbols for bit map position values
	Public Const gc_nFinanceEvents_FIN As Short = 1 ' FIN encountered
	Public Const gc_nFinanceEvents_WIN As Short = 2 ' FIN encountered, then WIN
	Public Const gc_nFinanceEvents_PAR As Short = 4 ' FIN encountered, then PAR
	
	
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
	Public Const gc_nEffInts_WIN As Short = 1
	Public Const gc_nEffInts_PAR As Short = 2
	Public Const gc_nEffInts_WOX As Short = 3
	Public Const gc_nEffInts_POX As Short = 4
	Public Const gc_nEffInts_WNR As Short = 5
	Public Const gc_nEffInts_WPR As Short = 6
	Public Const gc_nEffInts_WFN As Short = 7
	Public Const gc_nEffInts_PFN As Short = 8
	Public Const gc_nEffIntsSIZED2 As Short = gc_nEffInts_PFN
	
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
	Public Const gc_nEffIntsX_CAT As Short = 1
	Public Const gc_nEffIntsX_XMO As Short = 2
	Public Const gc_nEffIntsX_XYR As Short = 3
	Public Const gc_nEffIntsX_TAN As Short = 4
	Public Const gc_nEffIntsX_WIN As Short = 5
	Public Const gc_nEffIntsX_PAR As Short = 6
	Public Const gc_nEffIntsX_BUR As Short = 7
	Public Const gc_nEffIntsXSIZED2 As Short = gc_nEffIntsX_BUR
	
	' Symbols for MYC() subscript 2 values:
	Public Const gc_nMYC_CAT As Short = 1 ' capital expenditure category
	Public Const gc_nMYC_XMO As Short = 2 ' month of expenditure
	Public Const gc_nMYC_XYR As Short = 3 ' year of expenditure
	Public Const gc_nMYC_TAN As Short = 4 ' tangible portion of expenditure, percent
	Public Const gc_nMYC_AMT As Short = 5 ' consolidated company capital amount
	
	' Symbols for MYC() new columns (consolidated capital expenditures)
	Public Const gc_nMYC_GRS As Short = 8 ' consolidated gross expenditure
	Public Const gc_nMYC_CMP As Short = 9 ' consolidated company capital amount
	Public Const gc_nMYC_3DP As Short = 10 ' consolidated 3rd party capital amount
	Public Const gc_nMYC_NOC As Short = 11 ' consolidated NOC capital amount
	Public Const gc_nMYC_BUR As Short = 12 ' consolidated company reimbursement amount
	Public Const gc_nMYC_TCX As Short = 13 ' consolidated gross-level capex (from my3Ex())(doesn't include bonuses)
	Public Const gc_nMYC_OCX As Short = 14 ' consolidated operating-level capex (from my3Ex())
	Public Const gc_nMYC_GCX As Short = 15 ' consolidated group-level capex (from my3Ex())
	Public Const gc_nMYC_CCX As Short = 16 ' consolidated company-level capex (from my3Ex())
	Public Const gc_nMYCSIZED2 As Short = gc_nMYC_CCX
	
	' Symbols for existing columns of MY3() (capital expenditures)
	Public Const gc_nMY3_CAT As Short = 1 ' capital expenditure category
	Public Const gc_nMY3_XMO As Short = 2 ' month of expenditure
	Public Const gc_nMY3_XYR As Short = 3 ' year of expenditure
	Public Const gc_nMY3_TAN As Short = 4 ' tangible portion of expenditure, percent
	Public Const gc_nMY3_AMT As Short = 5 ' capital expenditure amount
	Public Const gc_nMY3_WIN As Short = 6 ' company working interest share of expenditure amount
	Public Const gc_nMY3_BUR As Short = 7 ' partner reimbursement amount expressed as percentage of carried amount
	' End (C0846)
	
	'<<<<<< 18 Sep 2001 JWD (C0443)
	' Store the consolidated gross (100%) revenues by product
	' for use in calculating the gross equivalent production.
    Public GrossRevenue(,) As Single
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
    Public GrossProduction(,) As Single
	'>>>>>> End (C0443)
	
	' 21 Jan 2004 JWD (C0776)
	' Base forecast codes
	Public Const BDACategoryCodesString As String = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0RESWINOPCGPCOP1OP2OP3OP4OP5OP6OP7OP8OP9OP0OX1OX2OX3OX4OX5AJ1AJ2AJ3AJ4AJ5"
	
	
	'<<<<<< 1 Aug 2001 JWD (C0363)
	Public Const CPXCategoryCodeString_AbandonmentCashExpenditure As String = "AB1"
	Public CPXCategoryCode_AbandonmentCashExpenditure As Short
	Public Const CPXCategoryCodeString_AbandonmentAccrualEntry As String = "AB2"
	Public CPXCategoryCode_AbandonmentAccrualEntry As Short
	
	' 17 May 2005 JWD
	Public Const CPXCategoryCodesString As String = "BNSLSERENGEOEDHEDSADHASCDNPDVPPLFFCLTRNEORCP1CP2CP3CP4CP5CP6CP7CP8CP9ABNBALBL2BL3BL4BL5BL6BL7BL8BL9BL0B11B12B13B14B15B16B17B18B19B20" & CPXCategoryCodeString_AbandonmentCashExpenditure & CPXCategoryCodeString_AbandonmentAccrualEntry
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
	Public CPXCategoryCodeBAL As Short
	Public CPXCategoryCodeBL2 As Short
	Public CPXCategoryCodeBL3 As Short
	'>>>>>> End C0339
	
	' 17 May 2005 JWD (C0878) Add symbols for last BAL code
	' These are used assuming that all of the balance category
	' codes will be, as other codes are, sequential and
	' contiguous integer values, the sequence uninterrupted
	' by other codes that are not balance categories.
	' As of this date (17 May 2005), B20 is the last balance
	' category that is provided for. Its value will also be
	' referenced as the last value (represented as BLn).
	Public Const CPXCategoryCodeString_BLn As String = "B20"
	Public CPXCategoryCodeBLn As Short
	' End (C0878)
	
	'GIANT 6.1 Start JWD 6 Feb 1996 ----------------------------------------
	' Debugging switch - was local
	Public BDebugging As Short
	
	'  Now store data items formerly recorded on SCRA1.SCR
    Public VLM(,) As Single
	Public VOL() As Single
	Public REV() As Single
	Public DPC() As Single
	Public PCE() As Single
	Public FCRD() As Single
	Public OthBon() As Single
    Public DDT(,) As Single
	'GIANT 6.1 End ---------------------------------------------------------
	
	'GIANT 6.0 Start JWD 25 Mar 1995
	Public AppDir As String ' location of system (shared) files
	Public TempDir As String ' location of temporary (scratch) files
	Public Program As String ' current executing program/module name
	Public FConfig As String ' filespec for GNTCONFG.DAT
	Public FOxfil As String ' filespec for OXFIL.DAT
	Public FExchng As String ' filespec for GNTEXCH.DAT
	Public FChuong As String ' filespec for CHUONG.DAT
	Public FOxyRun As String ' filespec for OXYRUN~.FIL
	Public FPStatus As String ' filespec for program completion status
	'GIANT 6.0 End -----------------
	
	'Giant 5.4 start -----------------------------------
	'This record type added to store the contents if the RATScreen
	'  (RTO/RT1 Ratio Definition screen) for use by RatioDefinition
	'  sub in Util5 (used when RatioDefinition called by CalcIRR)
	
    Class RATType
        'UPGRADE_WARNING: Fixed-length string size must fit in the buffer. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"'
        <VBFixedString(3), System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst:=3)> Public var() As Char 'variable
        'UPGRADE_WARNING: Fixed-length string size must fit in the buffer. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"'
        <VBFixedString(3), System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst:=3)> Public numden() As Char 'numerator/denominator
        'UPGRADE_ISSUE: Declaration type not supported: Array of fixed-length strings. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="934BD4FF-1FF9-47BD-888F-D411E47E78FA"'
        Public fnc(8) As String 'fnc
        'UPGRADE_ISSUE: Declaration type not supported: Array of fixed-length strings. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="934BD4FF-1FF9-47BD-888F-D411E47E78FA"'
        Public Cat(8) As String 'categories 1 - 8
    End Class

    Public RATRecs As Short
    Public rat() As RATType 'RTO/RT1 Ratio Definition

    'Giant 5.4 end -------------------------------------

    'Giant 5.4 Start --------------
    Public XYear As Short 'loop counter - year
    Public XFirst As Short 'line number of first variable after ITB (iteration begin)
    Public XLast As Short 'line number of last variable before ITE (iteration end)
    Public XIter As Short 'switch.  0 = we are NOT iterating, 1 = we are iterating.
    Public SubVarsNum() As Single
    Public SubVarsDen() As Single
    'Giant 5.4 End --------------


    '8-26-92 - put version # in common
    Public ver As String
    Public vernumber As Single
    Public ProgVersion As String

    '8-21-92 store final win % and partic rate in common for report display
    Public FINALPARTIC, FinalWin As Single

    'sensitivities data
    Public vlc, vlu, vlr As Single
    Public sVlu As String

    'general
    Public AR As Short 'used throughout to see where we came from
    Public PPR As Short 'primary product (1=oil 2=gas)
    Public SEC As Short 'secondary product (1=oil 2=gas)
    Public Camefrom As String 'program name that chained to the current program

    'these items are used by the file listing routines
    '  in FORECAST & CNTYFCST (when the user has requested a data
    '  or country file listing in his run file
    Public casenm, datapath, casedesc As String
    Public CtyNm, CtyPath, CtyDesc As String
    Public ExchNm, EXCDesc As String 'name & description of the exchange rate file

    Public DNT() As String
    Public DNTRecs As Short 'data file notes
    Public CNT() As String
    Public CNTRecs As Short 'country file notes
    Public EXTNote() As String
    Public ExtNotes As Short 'external file notes

    'user defined codes
    '   Global nousercodes%    'number of codes
    '   Global usercode$()     'list of user codes

    'files
    ' Added GDP 18/8/99
    Public g_sDataFileNoExt As String ' data file name with no ext or path

    Public N1 As String 'data file name from run file
    Public N1C As String 'country file name from run file

    'dates
    Public L1() As Single 'holds various dates needed for consolidation
    Public L2() As Single 'temporary version of L1()
    Public mo As Single 'project start month (from general parameters)
    Public YR As Single 'project start year (from general parameters)
    Public M1 As Single 'production start month (from general parameters)
    Public Y1 As Single 'production start year (from general parameters)
    Public M2 As Single 'discovery start month (from general parameters)
    Public Y2 As Single 'discovery start year (from general parameters)
    Public M3 As Single 'discount start month (from general parameters)
    Public Y3 As Single 'discount start year (from general parameters)

    'misc dates for forecasting routines
    Public ProjYr, ProdYr, ProdMo, ProjMo As Short
    Public curvelife, proddelay As Single

    'life
    Public LG As Short 'life (reporting years(integer)) calculated in forecast
    Public LFX As Short 'producing life (integer) (rounded up to whole year)  MKD 3-4-91 changed name from LF
    Public LFI As Single 'producing life (actual) (ie 23.15 years)
    Public LGI As Single 'project life (actual) (ie 27.5 years)
    Public AbandonmentPlacementOffset As Single ' is set to 1 when abandonment placement option Last Year + 1 is set

    'these two items are used by forecasting routines as flags wether
    '  forecasted curves are relative to PJM or PDM and wether the last
    '  year of curve is to be pro-rated to a fractional portion of the period
    Public AdjustLastYear As Short
    Public MonthRelative As Short

    'reporting
    '<<<<<< 25 Aug 1998 JWD Change following symbol name
    Public strDecimal As String 'decimal places on reports
    '>>>>>> End 25 Aug 1998

    'titles
    Public TL(,) As String 'Variable Titles screen
    Public TLT As Short '# items in TL()
    Public PN() As String 'Data File Titles from General Parameters (4 lines)
    Public PNC() As String 'Country File Titles from Country Misc Parameters screen (4 lines)

    'paths

    '~~~~ 20 Feb 1996 JWD - Changed variable names.
    Public sGntDir As String 'path for data files
    Public sCtyDir As String 'path for country files
    Public sRunDir As String 'path for run files
    Public sRptDir As String 'path for report files
    Public sExchDir As String 'path for exchange rate file
    '~~~~ 20 Feb 1996 JWD - Add external table directory
    Public sExtDir As String 'path for external tables files
    '~~~~ End of 20 Feb 1996

    'flags
    Public BURS As Short '(1 or 0 (T/F)) (if 1 then REIM() is NOT zero)
    Public PRTA As Short '(1 or 0 (T/F)) (PAR line(s) in FISCAL DEF screen)
    Public WINT As Short '(1 or 0 (T/F)) (WIN line(s) in FISCAL DEF screen)

    'currency
    Public ConCur As String 'currency code for consolidation (see sCur)
    Public sCur As String 'currency code
    Public CUR() As Single 'annual exchange rate - calculated in CURRENCY.BAS
    Public CURT As Short 'Counts "CUR" lines in country file. We only
    '  do Revaluation in DEPREC for first
    '  "CUR" entry
    'Added by Glyn 16/04/99
    Public g_sConsolCur As String ' Currency of input data (one per GETDATA)

    'Added by Glyn 27/08/99
    Public g_bPTCons As Boolean ' Flag used when current run is a consolidation

    Public LCur() As Single 'the factor of the last currency line
    'encountered.  That way if more than
    'one CUR line, we can convert back
    'to dollars before adjusting to new rate.

    '   Global CurrExc!()     'exchange rates data
    Public sCurrExch(,) As String 'exchange rates titles
    '   Global CurrencyRecs%  'number of exchange rate items

    'run file
    Public RF() As String 'array for a run file line
    Public RFT As Short '# lines in run file
    Public RN As String 'run file name
    Public RNU As Short 'run number (of the current run in case of consolidation)

    'interests
    Public WIN() As Single 'annual working interest  - calculated in FISCAL.BAS
    Public WINC() As Single 'working interest for each CAPEX item in MY3()

    ' Added GDP 18/08/99
    Public CAPWIN() As Single ' Capex working interest used in consolidation (read from RVSCON)

    'rate of return
    Public inc1() As Single 'these two are adjustments for reports used
    Public ded1() As Single '  with IRR, RTO, & RT1 calculations

    'miscellaneous (including screen  and output data from forecasting)
    Public A(,) As Single 'forecasted and inflated PDC and BDA recs
    Public AC(,) As Single 'holds consolidation of a() in FORECAST.BAS (temporarily)

    Public AMTLOAN(,) As Single 'single amount loan records
    Public AMTLT As Short '# items in AMTLOAN()

    Public B(,) As Single 'annual array from Country Annual Forecast form

    Public BN(,) As String 'bonuses
    Public BNT As Short '# items in BN$()

    Public BONS() As Single 'production bonuses by year - calculated in BONUS.BAS

    Public CC(,) As Single 'consolidated CAPEX lines (pointer is CCT)
    Public CCT As Short '# of concatenated CAPEX lines in consolidation

    Public CLG(,) As String 'Ceiling definition screen
    Public CLGTT As Short '# items in CLG()

    Public clngs() As Single 'annual cost recovery ceiling amounts

    Public CGR(,) As Single 'Ceiling Rates screen (all but VAR column)
    Public sCGR() As String 'VARiable column of Ceiling rates screen
    Public CGRT As Short '# items in CGR() & sCGR()

    Public sCPD() As String 'Compound Rates variable column

    ''' <summary>
    ''' Compound Rates other columns.
    ''' </summary>
    ''' <remarks></remarks>
    Public CPD(,) As Single
    Public CPDT As Short '# items in CPD()

    Public DFL() As Single 'deflator values 1-LG   'filled out in Forecast.BAS

    Public DIS As Single 'Discovery bonuses - calculated in BONUS.BAS

    Public DiscMthd As Short 'Discount Method from Discounting Parameters screen

    Public DL(,) As Single 'Depletion screen (all but VAR column)
    Public sDL() As String 'VARiable column of Depletion screen
    Public DLT As Short '# items in DL() & sDL()

    ''' <summary>
    ''' Depreciation screenn (all but VAR column).
    ''' </summary>
    ''' <remarks></remarks>
    Public dp(,) As Single
    Public sDP() As String 'VARiable column of Depreciation screen
    Public DPT As Short '# items in DPT() & DPT$()

    Public EXPLOAN(,) As Single 'exp based loan records
    Public EXPLT As Short '# items in EXPLOAN()

    Public FINANCE() As Single 'net financing effect - shown on ATLF page
    Public FVAR() As String 'contains the 3-letter variable code

    Public GM(,) As Single 'Country Misc Parameters screen

    Public gn() As Single 'GENERAL PARAMETERS array (OLD) - not all elements used!

    Public GPBASE() As Single 'group share of CAPEX - calculated in PARTIC.BAS

    Public GPRATE() As Single 'group participation - calculated in PARTIC.BAS

    ' added GDP 18/8/99
    Public GPRTE() As Single 'group participation used in consolidation (read from RVSCON)

    Public INTRST() As Single 'annual interest payments - calculated in LOAN.BAS
    Public Inflate(,) As Single 'inflation rates for oil and gas prices

    Public LOAN() As Single 'annual loan receipts - calculated in LOAN.BAS

    Public sMDC() As String 'Miscellaneous Depreciation/Cost Recovery parameters
    Public MDC(,) As Single
    Public MDCT As Short

    Public my3(,) As Single 'CAPEX array
    Public MY3T As Short '# items in data file MY3()
    Public my3tt As Short 'total CAPEX lines (# data file MY3() + country file my3() records)

    Public OPEX() As Single 'total annual OPEX - calculated in FORECAST

    ''' <summary>
    ''' Price Definition screen (all but VAR column).
    ''' </summary>
    ''' <remarks></remarks>
    Public PC(,) As Single
    Public sPCV() As String 'VARiable column of Price definition screen
    Public PCT As Short '# items in PC() & sPCV()


    ''' <summary>
    ''' Prepaid-Deferred tax screen (all but VAR column).
    ''' </summary>
    ''' <remarks></remarks>
    Public PD(,) As Single

    ''' <summary>
    ''' VARiable column of Prepaid-Deferred tax screen.
    ''' </summary>
    ''' <remarks></remarks>
    Public sPDV() As String

    ''' <summary>
    ''' # items in PDT() & PDT$().
    ''' </summary>
    ''' <remarks></remarks>
    Public PDTT As Short


    Public sPRV() As String 'categories in Govt. Participation
    ''' <summary>
    ''' Govt participation rates (screen image).
    ''' </summary>
    ''' <remarks></remarks>
    Public PR(,) As Single
    Public PRT As Short '# items in PR()

    Public PRINC() As Single 'annual principal payments - calculated in LOAN.BAS

    Public PT(,) As Single 'govt participation terms (screen image)
    Public PTT As Short '# items in PT()

    Public PARTRATE() As Single 'This is government share of income
    Public OPEXRATE() As Single 'This is government share of opex - Version 5.0

    ' Added GDP 18/8/99
    Public PRTRTE() As Single ' government share of income used in consolidation (read from RVSCON)
    Public OPXRTE() As Single ' government share of opex used in conmsolidation (read from RVSCON)

    Public REIM() As Single 'reimbursement of carried capital - calculated in FISCAL.BAS

    Public RLD() As Single 'this is Taxable Income in Fiscal (revenue less deductions)

    Public RT(,) As Single 'Variable Rates screen (all but VAR column)
    Public sRTV() As String 'VARiable column of Variable Rates screen
    Public RTT As Short '# items in RT() & sRTV()

    Public RVN(,) As Single 'annual values of each item in Fiscal Definition

    Public SEQ(,) As Single 'cost recovery sequence screen image

    Public SIG As Single 'Signature bonuses - calculated in BONUS.BAS

    Public TD(,) As String 'fiscal definition (screen image)
    Public TDT As Short '# items in TD()

    ''' <summary>
    ''' Misc Variable Parameters screen (all but VAR column).
    ''' </summary>
    ''' <remarks></remarks>
    Public TM(,) As Single
    ''' <summary>
    ''' VARiable column of Misc Variable Parameters screen.
    ''' </summary>
    ''' <remarks></remarks>
    Public sTMV() As String
    Public TMT As Short '# items in TM() & sTMV()

    Public TOTPMT() As Single 'total govt repayment - calculated in REPAY.BAS
    ' Added GDP 10/9/99
    Public TOTREPAY() As Single

    'Added GDP 14/11/2000
    Public TOTFINANCE() As Single

    'cashflows
    Public GVCF() As Single 'govt cashflows
    Public PSCF(,) As Single 'positive cashflows ("+" on FDef) revenues
    Public NGCF(,) As Single 'negative cashflows ("-" on FDef) govt rev

    'OXY ITEMS
    Public NGCFT(,) As Single 'negative cashflows ("T" on FDef) 3rd party
    Public NGCFA(,) As Single 'negative cashflows ("A" on FDef) adjustments
    Public NGCFU(,) As Single 'negative cashflows ("U" on FDef) US taxes
    'counters

    Public PS As Short 'positive cashflows ("+" on FDef) co. rev
    Public NEG As Short 'negative cashflows ("-" on FDef) govt rev
    Public NEGT As Short 'negative cashflows ("T" on FDef) 3d party rev
    Public NEGA As Short 'negative cashflows ("A" on FDef) adjustments
    Public NEGU As Short 'negative cashflows ("U" on FDef) US taxes
    Public TAUItems As Short 'sum of NEGT%, NEGA%, & NEGU%


    'flags
    Public USTT As Short 'presence of "U" in cashflow column in FDEF
    Public ADJT As Short 'presence of "U" (Adjustment) in cashflow column in FDEF
    Public TRDT As Short 'presence of "T" (Third Party) in cashflow column in FDEF


    '********************************************************************

    'unknown items
    Public PNCT, LGH, FO1, PPS, PPT, FO2, LFH, GMT As Single
    Public RNP As String
    Public PRCCD() As String

    '********************************************************************

    'new items for Occidental Petroleum project - Version 5.3  10-12-92

    'locations of network and local serial number files. These items
    '  are stored in GNTOXY.CFG in the current directory
    Public NetSerPath As String
    Public NetSerFile As String
    Public LocalSerPath As String
    Public LocalSerFile As String
    'serial number and suffix from network or local serial# file
    Public sno As Single
    Public snid As String


    'THE FOLLOWING ARE OXY DATA ITEMS (PARADOX DATABASE ITEMS)
    'This is the Paradox UNIQUE KEY. It appears in all 4 files
    Public Serial As String

    'store data put into GIANT.ANN file. This file contains the annual
    '  data about a run.
    'There is ONE entry per YEAR per RUN.
    Public AData() As Single

    'store data put into GIANT.PRJ file. This file contains the data
    '  about a run.
    'There is ONE entry per RUN.
    Public HistSer As String
    'Date$ - use the DATE$ function
    'UPGRADE_NOTE: Class was upgraded to Class_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    Public Country, Constat, Status, sType, Class_Renamed, Block As String
    Public FieldDesc, Field, sCase, Region As String
    'Time$ - use the TIME$ function
    Public Who, ID, Primary, OnOff, RunType, PriceBase As String
    Public Success As Single
    Public Tag4, Tag2, GVerNo, Tag1, Tag3, Tag5 As String
    Public WinNet, WinGovt, WinComp, WinPart, WinExp, WinRev As Single
    Public WinNCF, USTxRate As Single
    Public PUST, DsctDate, DsctMeth, PUSB As String
    Public PNIB, PNIA As String
    Public GTake15, GTake, WatDepth, GTake10, GTake20 As Single
    Public GntNPV15, GntNCF, GntROR, GntNPV10, GntNPV20 As Single
    Public GntPI15, GntPI, GntPI10, GntPI20 As Single
    Public GntPayot, GntDscvY As Single

    'store data put into GIANT.DOC file. This file contains the data
    '  about a run.  Contains info about the files used in a run.
    'There is ONE entry per RUN.

    Public GTitl1 As String
    Public GTitl2 As String
    Public GTitl3 As String
    Public GTitl4 As String
    Public CTitl1 As String
    Public CTitl2 As String
    Public CTitl3 As String
    Public CTitl4 As String
    Public GNTDesc As String
    Public EXTDesc As String
    Public GNTFile As String
    Public CTYFile As String
    Public RUNFile As String
    Public EXTFile As String
    Public EXCFile As String
    Public SensDat As String
    Public SensCty As String
    'store data put into GIANT.NOT file. This file contains the data
    '  about a run.  This relates to the notes fields in a file.
    'There is ONE entry per RUN.

    Public RunLines() As String
    Public RunNotes() As String
    Public RunNoteRecs As Short


    Public NoType As String
    Public LineNo As Short
    Public Notes As String

    'variables needed for oxy reports & database
    Public ttlcap As Single 'total capital (from grossrpt.exe)


    '4-22-93 - ver 5.3  added these items to common to facilitate moving
    '  ForecastSetLife from FORECAST.BAS to GNTFCST2.BAS
    Public Maxlife As Short
    Public PrimaryStart As String

    '-------------------------------------------------------------------
    'these are used to make x and XI in CASHFLOW:38500 available to
    '  GNTOXYC.EXE:LoadOXYConsol
    Public pointerX, pointerXI As Short

    'OXY database items (consolidated values)

    Public PriorGProdEq, PriorGExpCap As Single

    'store data put into GIANT.ANN file. This file contains the annual
    '  data about a run.
    'There is ONE entry per YEAR per RUN.
    Public ADataCon(,) As Single

    'store data put into GIANT.PRJ file. This file contains the data
    '  about a run.
    'There is ONE entry per RUN.
    Public HistSerCon As String
    Public ConstatCon, StatusCon, TypeCon, ClassCon As String
    Public FieldCon, CountryCon, BlockCon, CaseCon As String
    Public PrimaryCon, FieldDescCon, RegionCon, OnOffCon As String
    Public SuccessCon As Single
    Public WhoCon, IDCon, RunTypeCon, PriceBaseCon As String
    Public Tag4Con, Tag2Con, GVerNoCon, Tag1Con, Tag3Con, Tag5Con As String
    Public WinGovtCon, WinCompCon, WinPartCon, WinExpCon As Single
    Public WinRevCon, WinNetCon, WinNCFCon As Single
    Public DsctDateCon As String
    Public PUSTCon, DsctMethCon, PUSBCon As String
    Public USTxRateCon As Single
    Public PNIBCon, PNIACon As String
    Public WatDepthCon As Single
    Public GTake15Con, GTakeCon, GTake10Con, GTake20Con As Single
    Public GntNPV10Con, GntRORCon, GntNCFCon, GntNPV15Con As Single
    Public GntPI10Con, GntNPV20Con, GntPICon, GntPI15Con As Single
    Public GntPayotCon, GntPI20Con, GntDscvYCon As Single
    Public OXYGrRevCon, OXYTtlPosCon As Single

    'store data put into GIANT.DOC file. This file contains the data
    '  about a run.  Contains info about the files used in a run.
    'There is ONE entry per RUN.

    Public GTitl1Con As String
    Public GTitl2Con As String
    Public GTitl3Con As String
    Public GTitl4Con As String
    Public CTitl1Con As String
    Public CTitl2Con As String
    Public CTitl3Con As String
    Public CTitl4Con As String
    Public EXCDescCon, GNTDescCon, CtyDescCon, EXTDescCon As String
    Public GNTFileCon As String
    Public CTYFileCon As String
    Public RUNFileCon As String
    Public EXTFileCon As String
    Public EXCFileCon As String
    Public SensDatCon As String
    Public SensCtyCon As String
    'store data put into GIANT.NOT file. This file contains the data
    '  about a run.  This relates to the notes fields in a file.
    'There is ONE entry per RUN.

    Public RunLinesCon() As String
    Public RunNotesCon() As String


    Public NoTypeCon As String
    Public LineNoCon As Short
    Public NotesCon As String

    'variables needed for oxy reports & database
    Public ttlcapCon As Single 'total capital (from grossrpt.exe)

    'End of OXY Database items
    '********************************************************************
End Module