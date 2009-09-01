Attribute VB_Name = "MAbandonment"
' Name:        MAbandonment.bas
' Function:    Apply abandonment funding provisions
'---------------------------------------------------------
' 5 Jul 2001 JWD New (C0341)
'---------------------------------------------------------
' Modifications:
' 17 Jul 2001 JWD
'  -> Changed zzzGetTriggerPeriod(). (C0352)
'  -> Changed ApplyAbandonmentFundingProvisions(). (C0353)
'
' 24 Jul 2001 JWD
'  -> Add abandonment expenditure inflation array. (C0355)
'  -> Change InitializeAbandonmentExpenditureData. (C0355)
'  -> Change ReadAbandonmentExpenditureData(). (C0355)
'
' 25 Jul 2001 JWD
'  -> Change ReadAbandonmentFundingProvisions(). (C0357)
'  -> Add symbolic constants for method values. (C0357)
'  -> Add symbolic constants for method parameter default
'     values. (C0357)
'
' 30 Jul 2001 JWD
'  -> Change zzzGetEventPeriod(). (C0359, C0360)
'
' 1 Aug 2001 JWD
'  -> Add private symbol zzz_CashAccrualOption. (C0363)
'  -> Add constant symbols for different values that
'     zzz_CashAccrualOption may take and symbol for the
'     default value to assign if 'no entry' is made.
'     (C0363)
'  -> Change ReadAbandonmentFundingProvisions(). (C0363)
'
' 2 Aug 2001 JWD
'  -> Add zzzRemoveAbandonmentEntriesFromMY3(). (C0363)
'  -> Add TriggerAbandonmentProvisionsEconomicLimitEvent().
'     (C0363)
'  -> Change ApplyAbandonmentFundingProvisions(). (C0363)
'
' 7 Aug 2001 JWD
'  -> Change ReadAbandonmentExpenditureData(). (C0374)
'  -> Add ReadAbandonmentInflationData(). (C0374)
'
' 8 Aug 2001 JWD
'  -> Change ApplyAbandonmentFundingProvisions(). (C0375)
'
' 20 Aug 2001 JWD
'  -> Change ApplyAbandonmentFundingProvisions(). (C0391)
'  -> Change TriggerAbandonmentProvisionsEconomicLimit-
'     Event(). (C0391)
'
' 21 Aug 2001 JWD
'  -> Add new procedure zzzGetInflatedFutureValueFactor().
'     (C0392)
'
' 25 Sep 2001 JWD
'  -> Change ApplyAbandonmentFundingProvisions(). (C0467)
'
' 10 Sep 2002 JWD
'  -> Add TriggerAbandonmentProvisionsModelEvent(). (C0588)
'  -> Add zzzRescheduleAbandonment(). (C0588)
'  -> Add public symbol for project life change due to
'     license terms. (C0588)
'
' 12 Sep 2002 JWD
'  -> Add public symbol for project life change due to
'     economic limit. (C0592)
'  -> Change TriggerAbandonmentProvisionsModelEvent().
'     (C0592)
'  -> Change TriggerAbandonmentProvisionsEconomicLimit-
'     Event(). (C0592)
'  -> Add zzzRecomputeWINCValues(). (C0592)
'  -> Add zzzDetermineMY3Count(). (C0592)
'
' 13 Sep 2002 JWD
'  -> Add zzzUpdateAbandonmentFunding_AccrualMethod().
'     (C0592)
'  -> Add zzzUpdateAbandonmentFunding_CashMethod().
'     (C0592)
'
' 11 Feb 2003 GDP
'  -> Changed ApplyAbandonmentFundingProvisions().
'
' 10 Sep 2003
'  -> Added SetAbandonmentExpenditurePeriodOffset().
'     (C0744)
'  -> Added SetAbandonmentInflationForecast(). (C0744)
'  -> Added SetGrossAbandonmentExpenditure(). (C0744)
'
' 20 May 2004 JWD
'  -> Changed TriggerAbandonmentProvisionsModelEvent().
'     (C0799)
'---------------------------------------------------------
Option Explicit

Const zzz_WIN = -998

' 10 Sep 2002 JWD (C0588)
Public Const OnChangeOfProjectLifeForLicenseTerm As Long = 4

' 12 Set 2002 JWD (C0592)
Public Const OnChangeOfProjectLifeForEconomicLimit As Long = 6

' The following items are specified by the country file:

' Indicates whether or not the country file contained
' abandonment funding provisions
Private zzz_HaveAbandonmentFundingProvisions As Boolean

' Identifies, for each trigger, the item that will serve
' as a trigger for funding start.
' As of 3 Jul 2001 the possible trigger parameters are:
' 1 = years from production start
' 2 = years prior to production end
' 3 = calendar year
' 4 = cumulative oil production
' 5 = cumulative gas production
' 6 = cumulative other volume 1 production
' 7 = cumulative other volume 2 production
' 8 = cumulative equivalent production
' 9 = percent of oil reserves
' 10 = percent of gas reserves
' 11 = percent of other volume 1 reserves
' 12 = percent of other volume 2 reserves
' 13 = percent of equivalent reserves
Private zzz_StartTriggerParameters() As Integer

' The trigger point values associated with the parameters.
' These are the values of the associated parameter that
' would trigger funding start.
Private zzz_StartTriggerParameterValues() As Single

' Specifies which, among 2 or more trigger parameters
' will determine the start period
' 0 = earliest
' 1 = latest
Private zzz_StartTriggerFunction As Integer

' When the funding starts relative to the period in which
' the trigger occurs
' 0 = same period
' 1 = next period
Private zzz_StartOption As Integer

' Specifies the method used to fund,
' 1 = Straightline (no parameters)
'<<<<<< 25 Jul 2001 JWD (C0357) Correct next
' 2 = Unit Of Production (uses 1 parameter, basis stream identifier)
' 3 = Declining Balance (uses 1 parameter, rate)
'~~~~~~ was: (incorrectly)
' 2 = Declining Balance (uses 1 parameter, rate)
' 3 = Unit Of Production (uses 1 parameter, basis stream identifier)
'>>>>>> End 25 Jul 2001 JWD
Private zzz_Method As Integer

'<<<<<< 25 Jul 2001 JWD (C0357)
' Symbols for different methods
Private Const zzz_Method_Straightline = 1
Private Const zzz_Method_UnitOfProduction = 2
Private Const zzz_Method_DecliningBalance = 3
'>>>>>> End (C0357)

' The method parameter. Interpretation is dependent
' upon the method.
Private zzz_MethodParameter As Single

'<<<<<< 25 Jul 2001 JWD (C0357)
' Symbols for method parameter default values
Private Const zzz_MethodParameterDefaultValue_Straightline = 0
Private Const zzz_MethodParameterDefaultValue_UnitOfProduction = 1
Private Const zzz_MethodParameterDefaultValue_DecliningBalance = 100


' Following item are specified the project data file:

' Indicates whether or not abandonment expenditure data
' was supplied.
Private zzz_HaveAbandonmentExpenditureData As Boolean

' The gross abandonment fund amount, the total
' amount to be funded. This item comes from the
' specific project file.
Private zzz_GrossAbandonmentExpenditure As Single

' The timing of the actual expenditure, relative
' to the end of the producing life. This is used
' if the funding provisions are not specified by
' the country file and in the circumstance when
' the project life is truncated due to the economic
' limit.
Private zzz_AbandonmentExpenditurePeriodOffset As Integer

'<<<<<< 24 Jul 2001 JWD (C0355)
' The forecast of inflation factors, by period, to be
' applied to abandonment expenditures.
Private zzz_InflationForecast() As Single
'>>>>>> End (C0355)

'<<<<<< 1 Aug 2001 JWD (C0363)
' Indicates whether or the funding payments are considered
' cash expenditures (to government/third parties) or just
' internal bookkeeping transactions with no cash flow
' effect.
Private zzz_CashAccrualOption As Integer
' Following are the values that the above may assume.
Private Const zzz_CashAccrualOption_Cash = 1
Private Const zzz_CashAccrualOption_Accrual = 2
'
Private Const zzz_CashAccrualOption_Default = zzz_CashAccrualOption_Cash
'>>>>>> End (C0363)

'
' 26 Jun 2001 JWD New (C0341)
'
' Modifications:
' 17 Jul 2001 JWD
'  -> Change numbers for UOP and declining balance
'     methods. Numbers were swapped, i. e. declining
'     balance was done when uop should have been and
'     vice versa. (C0353)
'
' 2 Aug 2001 JWD
'  -> Add handling of cash/accrual option. (C0363)
'
' 6 Aug 2001 JWD
'  -> Change to assign expenditures in last year + 1
'     only when no abandonment provisions specified.
'     (C0371)
'  -> Change to explicitly supply the applicable working
'     interest when adding the capital expenditure for
'     the no provisions case. This corrects problem when
'     WIN is applied, but the default working interest is
'     zero because it is beyond the end of the WIN fore-
'     cast as when the expenditure is put in last year + 1
'     (C0372)
'
' 8 Aug 2001 JWD
'  -> Add inflation of expenditure amounts before
'     scheduling payments. Add local variable for
'     inflated amount. Change the calls to the
'     scheduling methods to reference the inflated
'     amount. (C0375)
'
' 20 Aug 2001 JWD
'  -> Change to condition execution of ApplyAbandonment-
'     FundingProvisions() procedure on whether or not this
'     is a pre-tax consolidation run. This is to prevent
'     'double-dipping' the provisions as well as handle
'     fact that the expediture would only be for the last
'     project file read in (with the current design, which
'     assumes that the abandonment is only applied on a
'     single project basis). (C0391)
'     (Note: It was decided to put this here rather than
'     in Main() to limit the number of modules affected
'     by the change. Putting it in Main() would have
'     meant two changes in that procedure, i. e. by
'     conditioning the call of this procedure. However,
'     the call from the economic limit event also has to
'     be prevented in the event of a consolidation run
'     as well, which meant either this module or the
'     the module containing FiscalDef() gets changed.)
'
' 21 Aug 2001 JWD
'  -> Replace the reference of the inflation forecast
'     array with zzzGetInflatedFutureValueFactor() to
'     return the correct compounded inflation factor to
'     apply to the abandonment expenditure. (C0392)
'
' 21 Sep 2001 JWD
'  -> Add code to extend the final working interest in the
'     A() array for as many additional periods as are
'     added by the abandonment expenditure period offset.
'     This is a patch to correct the extension of life
'     after the forecasting is completed. It is applied
'     here because the abandonment is what is causing the
'     change in life. The assumption will be that the
'     final working interest value is extended as needed.
'     (C0458)
'
' 25 Sep 2001 JWD
'  -> Ensure that the abandonment provisions provide for
'     at least the scheduling of costs in the final year
'     of the project, even if the trigger occurs in the
'     final year and the start option states last year
'     + 1. (C0467)
'
' 11 Feb 2003
'  -> Changed reference to A array working interest. Now uses
'     constant gc_nAWIN instead of 6.
'
' This routine adds the abandonment costs to the
' capital expenditures forecast array (MY3())
'
Sub ApplyAbandonmentFundingProvisions()
   
   Dim i As Integer
   Dim the_code As Integer
   Dim start_period As Integer
   Dim number_of_periods As Integer
   
   '<<<<<< 8 Aug 2001 JWD (C0375)
   Dim inflated_expenditure As Single
   Dim inflation_factor As Single
   '>>>>>> End (C0375)
   
   '<<<<<< 20 Aug 2001 JWD (C0391)
   ' Is this run a pre-tax consolidation?
   If g_bPTCons Then
      ' It is so, ...
      Exit Sub
   End If
   '>>>>>> End (C0391)
   
   If Not zzz_HaveAbandonmentExpenditureData Then
      Exit Sub
   End If
   
   '<<<<<< 2 Aug 2001 JWD (C0363)
   ' Determine the numerical code for abandonment
   'SearchCodeString CPXCategoryCodesString, "ABN", 3, the_code
   '>>>>>> End (C0363)
   
   
   If zzz_HaveAbandonmentFundingProvisions Then
   
      '<<<<<< 8 Aug 2001 JWD (C0375)
      ' Determine the inflation factor
      
      '<<<<<< 21 Aug 2001 JWD (C0392)
      inflation_factor = zzzGetInflatedFutureValueFactor(LG)
      '~~~~~~ was:
      'i = UBound(zzz_InflationForecast)
      'If LG < i Then
      '   i = LG
      'End If
      'inflation_factor = zzz_InflationForecast(i)
      '>>>>>> End (C0392)
      
      ' Compute the inflated expenditure amount
      inflated_expenditure = zzz_GrossAbandonmentExpenditure * inflation_factor
      '>>>>>> End (C0375)
      
      '<<<<<< 2 Aug 2001 JWD (C0363)
      If zzz_CashAccrualOption = zzz_CashAccrualOption_Accrual Then
         the_code = CPXCategoryCode_AbandonmentAccrualEntry
      Else
         the_code = CPXCategoryCode_AbandonmentCashExpenditure
      End If
      '>>>>>> End (C0363)
      
      start_period = YR + zzzGetTriggerPeriod() - 1
      If zzz_StartOption > 0 Then
         start_period = start_period + 1
      End If
      
      number_of_periods = YR + LG - start_period
      
      '<<<<<< 25 Sep 2001 JWD (C0467)
      ' Ensure at least one period of scheduled abandonment funding.
      If number_of_periods < 1 Then
         ' Adjust start and number of periods.
         number_of_periods = 1
         start_period = start_period - 1
      End If
      '>>>>>> End (C0467)
      
      '<<<<<< 8 Aug 2001 JWD (C0375)
      Select Case zzz_Method
      Case 1
         zzzStraightline the_code, start_period, number_of_periods, inflated_expenditure
      Case 2
         zzzUnitOfProduction the_code, start_period, number_of_periods, inflated_expenditure, zzz_MethodParameter
      Case 3
         zzzDecliningBalance the_code, start_period, number_of_periods, inflated_expenditure, zzz_MethodParameter
      End Select
      '~~~~~~ was:
      ''<<<<<< 17 Jul 2001 JWD (C0353)
      'Select Case zzz_Method
      'Case 1
      '   zzzStraightline the_code, start_period, number_of_periods, zzz_GrossAbandonmentExpenditure
      'Case 2
      '   zzzUnitOfProduction the_code, start_period, number_of_periods, zzz_GrossAbandonmentExpenditure, zzz_MethodParameter
      'Case 3
      '   zzzDecliningBalance the_code, start_period, number_of_periods, zzz_GrossAbandonmentExpenditure, zzz_MethodParameter
      'End Select
      ''~~~~~~ was:
      ''Select Case zzz_Method
      ''Case 1
      ''   zzzStraightline the_code, start_period, number_of_periods, zzz_GrossAbandonmentExpenditure
      ''Case 2
      ''   zzzDecliningBalance the_code, start_period, number_of_periods, zzz_GrossAbandonmentExpenditure, zzz_MethodParameter
      ''Case 3
      ''   zzzUnitOfProduction the_code, start_period, number_of_periods, zzz_GrossAbandonmentExpenditure, zzz_MethodParameter
      ''End Select
      ''>>>>>> End (C0353)
      '>>>>>> End (C0375)
      
      '<<<<<< 2 Aug 2001 JWD (C0363)
      If zzz_CashAccrualOption = zzz_CashAccrualOption_Accrual Then
         ' The scheduled accrual entries just made do not have a
         ' cash flow effect, so schedule the actual lump-sum
         ' abandonment expenditure. This is the same as if there
         ' were no abandonment provisions made at all.
      
         '<<<<<< 6 Aug 2001 JWD (C03710
         '' Increase project life by period offset.
         'LG = LG + zzz_AbandonmentExpenditurePeriodOffset
         '>>>>>> End (C0371)
   
         ' Add an entry for the abandonment cost
         '<<<<<< 8 Aug 2001 JWD (C0375)
         AddCapitalExpenditure CPXCategoryCode_AbandonmentCashExpenditure, 7, YR + LG - 1, 0, inflated_expenditure, zzz_WIN, 0
         '~~~~~~ was:
         'AddCapitalExpenditure CPXCategoryCode_AbandonmentCashExpenditure, 7, YR + LG - 1, 0, zzz_GrossAbandonmentExpenditure, zzz_WIN, 0
         '>>>>>> End (C0375)
         
      End If
      '>>>>>> End (C0363)
         
   Else
      ' No early funding provisions, so add
      ' an entry for the gross abandonment cost
      ' in the period as specified.
   
      '<<<<<< 6 Aug 2001 JWD (C0372)
      Dim the_working_interest As Single
      
      
      ' Retrieve the last forecast working interest.
      ' It is assumed this would still apply when
      ' life is extended (if it is).
      
      ' GDP 11 Feb 2003
      ' Changed A(LG, 6) to A(LG, gc_nAWIN)
      the_working_interest = A(LG, gc_nAWIN)
      
      '>>>>>> End (C0372)
      
      '<<<<<< 21 Sep 2001 JWD (C0458)
      ' Extend the working interest forecast by the
      ' additional intervening periods until the
      ' abandonment expenditure is made.
      For i = LG + 1 To LG + zzz_AbandonmentExpenditurePeriodOffset
         ' GDP 11 Feb 2003
         ' Changed A(i, 6) = A(i - 1, 6) to A(i, gc_nAWIN) = A(i - 1, gc_nAWIN)
         A(i, gc_nAWIN) = A(i - 1, gc_nAWIN)
      Next i
      '>>>>>> End (C0458)
      
      ' Increase project life by period offset.
      LG = LG + zzz_AbandonmentExpenditurePeriodOffset
   
      ' Add an entry for the abandonment cost
      
      
      '<<<<<< 8 Aug 2001 JWD (C0375)
      ' Determine the inflation factor
      
      '<<<<<< 21 Aug 2001 JWD (C0392)
      inflation_factor = zzzGetInflatedFutureValueFactor(LG)
      '~~~~~~ was:
      'i = UBound(zzz_InflationForecast)
      'If LG < i Then
      '   i = LG
      'End If
      'inflation_factor = zzz_InflationForecast(i)
      '>>>>>> End (C0392)
      
      ' Compute the inflated expenditure amount
      inflated_expenditure = zzz_GrossAbandonmentExpenditure * inflation_factor
      
      AddCapitalExpenditure CPXCategoryCode_AbandonmentCashExpenditure, 7, YR + LG - 1, 0, inflated_expenditure, the_working_interest, 0
      '~~~~~~ was:
      ''<<<<<< 6 Aug 2001 JWD (C0372)
      'AddCapitalExpenditure CPXCategoryCode_AbandonmentCashExpenditure, 7, YR + LG - 1, 0, zzz_GrossAbandonmentExpenditure, the_working_interest, 0
      ''~~~~~~ was:
      '''<<<<<< 2 Aug 2001 JWD (C0363)
      ''AddCapitalExpenditure CPXCategoryCode_AbandonmentCashExpenditure, 7, YR + LG - 1, 0, zzz_GrossAbandonmentExpenditure, zzz_WIN, 0
      '''~~~~~~ was:
      '''AddCapitalExpenditure the_code, 7, YR + LG - 1, 0, zzz_GrossAbandonmentExpenditure, zzz_WIN, 0
      '''>>>>>> End (C0363)
      ''>>>>>> End (C0372)
      '>>>>>> End (C0375)
      
   End If
   
End Sub
'
' Modifications:
' 6 Aug 2001 JWD
'  -> Change codes for the capital expenditure categories
'     that have the timing changed. (C0363)
'  -> Change to adjust the project life to last year + 1
'     only when no abandonment provisions. (C0371)
'
' 26 Jun 2001 JWD New (C0341)
'
' This routine adjusts the expenditure date
' for abandonment for change in life due to
' application of economic limit.
'
Sub ChangeAbandonmentTimingForEconomicLimit()

   Dim the_code As Integer
   Dim i As Integer
   Dim abandon_period As Single
   
   '<<<<<< 6 Aug 2001 JWD (C0363)
   '' Determined the numerical code for abandonment
   'SearchCodeString CPXCategoryCodesString, "ABN", 3, the_code
   '>>>>>> End (C0363)
   
   '<<<<<< 6 Aug 2001 JWD (C0371)
   If Not zzz_HaveAbandonmentFundingProvisions Then
      ' Increase project life by period offset.
      LG = LG + zzz_AbandonmentExpenditurePeriodOffset
   End If
   '~~~~~~ was:
   '' Increase project life by period offset.
   'LG = LG + zzz_AbandonmentExpenditurePeriodOffset
   '>>>>>> End (C0371)
   
   abandon_period = YR + LG - 1
   
   ' Go through all of the capital expenditures
   For i = 1 To my3tt
      ' For the abandonment expenditures...
      
      '<<<<<< 6 Aug 2001 JWD (C0363)
      If my3(i, 1) = CPXCategoryCode_AbandonmentCashExpenditure Or my3(i, 1) = CPXCategoryCode_AbandonmentAccrualEntry Then
         ' This expenditure is an abandonment expenditure,
         ' so see when the expenditure occurred.
         If my3(i, 3) > abandon_period Then
            ' The expenditure occurred after the project
            ' life, so correct to life.
            my3(i, 3) = abandon_period
         End If
         
      End If
      '~~~~~~ was:
      'If my3(i, 1) = the_code Then
      '   ' This expenditure is an abandonment expenditure,
      '   ' so see when the expenditure occurred.
      '   If my3(i, 3) > abandon_period Then
      '      ' The expenditure occurred after the project
      '      ' life, so correct to life.
      '      my3(i, 3) = abandon_period
      '   End If
      '
      'End If
      '>>>>>> End (C0363)
   Next i
   
End Sub

'
' Modifications:
' 24 Jul 2001 JWD
'  -> Add initialization of inflation forecast
'     array. (C0355)
'
' 5 Jul 2001 JWD
'
' Initialize the abandonment expenditure data.
' This should be done for each run.
'
'
Public Sub InitializeAbandonmentExpenditureData()

   zzz_HaveAbandonmentExpenditureData = False
   zzz_GrossAbandonmentExpenditure = 0
   zzz_AbandonmentExpenditurePeriodOffset = 0
   
   '<<<<<< 24 Jul 2001 JWD (C0355)
   ' Initialize the forecast array
   ReDim zzz_InflationForecast(1 To gc_nMAXLIFE)
   '>>>>>> End (C0355)

End Sub

'
' Modifications:
' 1 Aug 2001 JWD
'  -> Add initialization of zzz_CashAccrualOption. (C0363)
'
' 2 Jul 2001 JWD New
'
' Initialize the abandonment funding provisions. This
' should be done each time a country file is loaded
' to ensure that the provisions are properly set to
' default values for instances where country files do
' not have an abandonment provisions section (earlier
' version files).
'
Public Sub InitializeAbandonmentFundingProvisions()

   zzz_HaveAbandonmentFundingProvisions = False
   
   ReDim zzz_StartTriggerParameters(0 To 0)
   ReDim zzz_StartTriggerParameterValues(0 To 0)
   
   zzz_StartOption = 0
   zzz_StartTriggerFunction = 0
   zzz_Method = 0
   zzz_MethodParameter = 0
   
   '<<<<<< 1 Aug 2001 JWD (C0363)
   zzz_CashAccrualOption = zzz_CashAccrualOption_Default
   '>>>>>> End (C0363)
   
End Sub

'
' Modifications:
' 24 Jul 2001 JWD
'  -> Add input of inflation forecast array conditioned
'     on version of expenditure data section. (C0355)
'  -> Add version number symbol. (C0355)
'
' 7 Aug 2001 JWD
'  -> Remove input of inflation data. This is moved to
'     new procedure ReadAbandonmentInflationData().
'     (C0362)
'
' 5 Jul 2001 JWD New (C0341)
'
' Read the abandonment expenditure data into storage
' from the specified file. This procedure is called
' by the project data file input procedure (Read5Gnt).
'
Public Sub ReadAbandonmentExpenditureData(ByVal FileHandle As Long)

   '<<<<<< 24 Jul 2001 JWD (C0355)
   Dim i As Integer
   Dim version_number As Long
   '>>>>>> End (C0355)
   
   Dim number As Long
   
   ' Get the version number
   '<<<<<< 24 Jul 2001 JWD (C0355)
   Input #FileHandle, version_number
   '~~~~~~ was:
   'Input #FileHandle, number
   '
   '' For now the version number is a dummy
   '' and is included for future use in event
   '' design of this changes.
   '>>>>>> End (C0355)
   
   ' Get flag indicating that data is present...
   Input #FileHandle, number
   If number = 0 Then
      ' no data following, so exit...
      Exit Sub
   End If
   
   zzz_HaveAbandonmentExpenditureData = True
   
   Input #FileHandle, zzz_GrossAbandonmentExpenditure
   Input #FileHandle, zzz_AbandonmentExpenditurePeriodOffset
   
   '<<<<<< 7 Aug 2001 JWD (C0374)
   ''<<<<<< 24 Jul 2001 JWD (C0355)
   '' version 1 does not have inflation data
   'If version_number < 2 Then
   '   Exit Sub
   'End If
   '
   '' Get the inflation forecast
   '' Reinitialize the forecast array
   'ReDim zzz_InflationForecast(1 To gc_nMAXLIFE)
   '
   ''<<<<<< 7 Aug 2001 JWD (C0374)
   '' Get the inflation section version number
   'Input #FileHandle, number
   ''>>>>>> End (C0374)
   '
   '' Get the number of data items that follow
   'Input #FileHandle, number
   '
   '' Load the forecast
   'For i = 1 To number
   '   Input #FileHandle, zzz_InflationForecast(i)
   'Next i
   ''>>>>>> End (C0355)
   '>>>>>> End (C0374)
   
End Sub

'
' Modifications:
' 25 Jul 2001 JWD
'  -> Add version_number symbol for identifying change
'     in section structure and content. (C0357)
'  -> Add handling of 'no entry' values that are to be
'     interpreted as meaning to use the default values.
'     (C0357)
'
' 1 Aug 2001 JWD
'  -> Add input of cash/accrual option value added with
'     version 2 data structure. (C0363)
'
' 2 Jul 2001 JWD
'
' Read the abandonment funding provisions into storage
' from the specified file. This procedure is called by
' the country file input procedure (Read6Cty)
'
Public Sub ReadAbandonmentFundingProvisions(ByVal FileHandle As Long)

   Dim i As Integer
   Dim number As Long
   
   '<<<<<< 25 Jul 2001 JWD (C0357)
   Dim version_number As Long
   '>>>>>> End (C0357)
   
   '<<<<<< 25 Jul 2001 JWD (C0357)
   ' Get the version number
   Input #FileHandle, version_number
   '~~~~~~ was:
   '' Get the version number
   'Input #FileHandle, number
   '
   '' For now the version number is a dummy
   '' and is included for future use in event
   '' design of this changes.
   '>>>>>> End (C0357)
   
   Input #FileHandle, number
   zzz_HaveAbandonmentFundingProvisions = (number <> 0)
   
   If Not zzz_HaveAbandonmentFundingProvisions Then
      Exit Sub
   End If
   
   ' Get the trigger parameters and values
   ' First, get the number of triggers
   Input #FileHandle, number
   ReDim zzz_StartTriggerParameters(1 To number)
   ReDim zzz_StartTriggerParameterValues(1 To number)
   
   For i = 1 To number
      Input #FileHandle, zzz_StartTriggerParameters(i)
      Input #FileHandle, zzz_StartTriggerParameterValues(i)
   Next i
   
   Input #FileHandle, zzz_StartTriggerFunction
   '<<<<<< 25 Jul 2001 JWD (C0357)
   ' Replace 'no entry' with default (0)
   If zzz_StartTriggerFunction < 0 Then
      zzz_StartTriggerFunction = 0
   End If
   '>>>>>> End (C0357)
   
   Input #FileHandle, zzz_StartOption
   '<<<<<< 25 Jul 2001 JWD (C0357)
   ' Replace 'no entry' with default (0)
   If zzz_StartOption < 0 Then
      zzz_StartOption = 0
   End If
   '>>>>>> End (C0357)
      
   Input #FileHandle, zzz_Method
      '<<<<<< 25 Jul 2001 JWD (C0357)
   ' Replace 'no entry' with default (1)
   If zzz_Method < 1 Then
      zzz_Method = zzz_Method_Straightline
   End If
   '>>>>>> End (C0357)

   Input #FileHandle, zzz_MethodParameter
   '<<<<<< 25 Jul 2001 JWD (C0357)
   ' Replace 'no entry' with default (0)
   If zzz_MethodParameter < 0 Then
      Select Case zzz_Method
      Case zzz_Method_Straightline
         zzz_MethodParameter = zzz_MethodParameterDefaultValue_Straightline
      Case zzz_Method_UnitOfProduction
         zzz_MethodParameter = zzz_MethodParameterDefaultValue_UnitOfProduction
      Case zzz_Method_DecliningBalance
         zzz_MethodParameter = zzz_MethodParameterDefaultValue_DecliningBalance
      End Select
   End If
   '>>>>>> End (C0357)
   
   '<<<<<< 1 Aug 2001 JWD (C0363)
   If version_number < 2 Then
      ' Apply the defaults for items that won't be loaded
      zzz_CashAccrualOption = zzz_CashAccrualOption_Default
      Exit Sub
   End If
   
   Input #FileHandle, zzz_CashAccrualOption
   If zzz_CashAccrualOption < 0 Then
      zzz_CashAccrualOption = zzz_CashAccrualOption_Default
   End If
   '>>>>>> End (C0363)
   
End Sub

'
' New 7 Aug 2001 JWD (C0374)
'
'
'
' Read the inflation forecast from the file.
'
Public Sub ReadAbandonmentInflationData(ByVal FileHandle As Long)

   Dim i As Long
   Dim version_number As Long
   Dim number As Long
   Dim item_count As Long
   Dim dummy As Single
   
   ' Get the inflation forecast
   ' Reinitialize the forecast array
   ReDim zzz_InflationForecast(1 To gc_nMAXLIFE)
   
   ' Get the inflation section version number
   Input #FileHandle, version_number
   
   ' Get the number of data items that follow
   Input #FileHandle, item_count
   
   number = item_count
   If number > gc_nMAXLIFE Then
      number = gc_nMAXLIFE
   End If
   
   ' Load the forecast
   For i = 1 To number
      Input #FileHandle, zzz_InflationForecast(i)
   Next i

   ' Discard any excess entries...
   For i = number + 1 To item_count
      Input #FileHandle, dummy
   Next i

End Sub

'
' 10 Sep 2003 JWD New (C0744)
'
' Set the abandonment expenditure timing
'
Public Sub SetAbandonmentExpenditurePeriodOffset(ByVal NewValue As Integer)

    If NewValue = 0 Or NewValue = 1 Then
        zzz_AbandonmentExpenditurePeriodOffset = NewValue
    End If

End Sub

'
' 10 Sep 2003 JWD New (C0744)
'
' Set the abandonment inflation forecast values.
'
Public Sub SetAbandonmentInflationForecast(ByRef NewValues() As Double)

   Dim i As Long
   Dim number As Long
   
   ' Get the inflation forecast
   ' Reinitialize the forecast array
   ReDim zzz_InflationForecast(1 To gc_nMAXLIFE)
   
   
   number = UBound(NewValues)
   If number > gc_nMAXLIFE Then
      number = gc_nMAXLIFE
   End If
   
   ' Load the forecast
   For i = 1 To number
      zzz_InflationForecast(i) = NewValues(i)
   Next i
    
End Sub

'
' 10 Sep 2003 JWD New (C0744)
'
' Set the abandonment expenditure amount. Sets have data
' flag if value is non-zero.
'
Public Sub SetGrossAbandonmentExpenditure(ByVal NewValue As Double)

    zzz_GrossAbandonmentExpenditure = NewValue
    
    zzz_HaveAbandonmentExpenditureData = (NewValue <> 0)
    
End Sub

'
' Modifications:
' 20 Aug 2001 JWD
'  -> Change to condition execution of this procedure on
'     whether or not this is a pre-tax consolidation run.
'     (C0391) (Note: See comments this date for the
'     ApplyAbandonmentFundingProvisions() procedure for
'     additional information about the change.)
'
' 12 Sep 2002 JWD
'  -> Move procedure code from this procedure to the
'     TriggerAbandonmentProvisionsModelEvent() procedure.
'     This procedure is kept to satisfy references from
'     other procedures, but it redirects the call to
'     the TriggerAbandonmentProvisionsModelEvent()
'     procedure. (C0592)
'
' New 2 Aug 2001 JWD (C0363)
'
' Do the things that have to be done for the abandonment
' provisions when the economic limit is reached.
'
' As of:
' 2 Aug 2001, abandonment entries in capital expenditures
'             are rescheduled based on the newly established
'             life.
' 20 Aug 2001, above still applies, but the procedure is not
'              executed if the run is a pre-tax consolidation
'              run.
'
'
Public Sub TriggerAbandonmentProvisionsEconomicLimitEvent()

   TriggerAbandonmentProvisionsModelEvent OnChangeOfProjectLifeForEconomicLimit
   
   ' 12 Sep 2002 JWD (C0592) Following is implemented
   ' in above called procedure.
   '
   'Dim i As Integer
   '
   ''<<<<<< 20 Aug 2001 JWD (C0391)
   '' Is this run a pre-tax consolidation?
   'If g_bPTCons Then
   '   ' It is, so ...
   '   Exit Sub
   'End If
   ''>>>>>> End (C0391)
   '
   'zzzRemoveAbandonmentEntriesFromMY3
   '
   '' At this point (the occurrence of the the economic
   '' limit being applied), MY3TT is used to record the
   '' count of capital expenditures, but
   '' AddCapitalExpenditure() uses MY3T as the count, so
   '' save the value of MY3T and then make it equal to
   '' MY3TT before calling the
   '' ApplyAbandonmentFundingProvisions() procedure
   '' (which calls AddCapitalExpenditure).
   'Dim save_value As Single
   'save_value = MY3T
   'MY3T = my3tt
   'ApplyAbandonmentFundingProvisions
   'my3tt = MY3T
   'MY3T = save_value
   '
   '
   '' Make sure the capital expenditures are in date order.
   'OrderCapitalExpendituresByDate
   '
   '' After the MY3 array is updated, need to regenerate
   '' the WINC() array in case any of the entries have
   '' changed positions. This is done only if the WIN
   '' command in Fiscal Definition has already been
   '' processed. (Because the order of entries in WINC()
   '' if it (WIN) hasn't been processed doesn't matter
   '' since they are all 100% beforehand.)
   '
   'If WINT <> 0 Then
   '   For i = 1 To my3tt
   '      WINC(i) = my3(i, 6) / 100
   '   Next i
   'End If
   
End Sub

'
' 26 Jun 2001 JWD New (C0341)
'
' Determine funding payments based on declining balance
' method. Add the funding payments to the capital
' expenditures array.
'
' FundingStartPeriod is the actual year, not the ordinal
' period number.
'
Private Sub zzzDecliningBalance(ByVal ExpenditureCategoryCode As Integer, ByVal FundingStartPeriod As Integer, ByVal NumberOfPeriods As Integer, ByVal TotalAmount As Single, ByVal DecliningBalanceRate As Single)

   Dim i As Integer
   Dim last_period As Integer
   Dim periodic_amount As Single
   Dim remaining_balance As Single
   Dim periodic_rate_factor As Single
   
   last_period = FundingStartPeriod + NumberOfPeriods - 1
   
   ' Initial amount to spread
   remaining_balance = TotalAmount
   
   If NumberOfPeriods > 1 Then
      ' The constant rate factor
      periodic_rate_factor = (DecliningBalanceRate / 100) / NumberOfPeriods
   
      ' Compute payment amount for each period, except last
      For i = FundingStartPeriod To last_period - 1
         periodic_amount = remaining_balance * periodic_rate_factor
         AddCapitalExpenditure ExpenditureCategoryCode, 7, i, 0, periodic_amount, zzz_WIN, 0
         remaining_balance = remaining_balance - periodic_amount
      Next i
      
   End If
            
   If remaining_balance > 0 Then
      ' Put any remaining balance in the last period
      AddCapitalExpenditure ExpenditureCategoryCode, 7, last_period, 0, remaining_balance, zzz_WIN, 0
   End If
               
End Sub

'
' Modifications:
' 30 Jul 2001 JWD
'  -> Add extreme value handling code for trigger events.
'     (C0359)
'  -> Correct determination of event period for cumulative
'     equivalent production. (C0360)
'
' 5 Jul 2001 JWD New (C0341)
'
' Determine and return the period in which the specified
' event occurs. This returns the ordinal number of the
' period (beginning with project start) in which the event
' occurs (1-LG).
'
Private Function zzzGetEventPeriod _
               ( _
               Parameter As Integer, _
               Value As Single _
               ) As Integer

   Dim i As Integer
   Dim event_period As Integer
   Dim cumulative As Single
   Dim total As Single
   Dim vector As Integer
   Dim the_volume() As Single
   
   ' See the list in declarations section
   Select Case Parameter
   Case 1            ' years from production start
      event_period = Y1 - YR + Value + 1
      '<<<<<< 30 Jul 2001 JWD (C0359)
      If event_period < Y1 - YR + 1 Then
         event_period = Y1 - YR + 1
      End If
      If event_period > LG Then
         event_period = LG
      End If
      '>>>>>> End (C0359)
   Case 2            ' years to production end
      event_period = LG - Value
      '<<<<<< 30 Jul 2001 JWD (C0359)
      If event_period < Y1 - YR + 1 Then
         event_period = Y1 - YR + 1
      End If
      If event_period > LG Then
         event_period = LG
      End If
      '>>>>>> End (C0359)
   Case 3            ' calendar year
      event_period = Value - YR + 1
      '<<<<<< 30 Jul 2001 JWD (C0359)
      If event_period < 1 Then
         event_period = 1
      End If
      If event_period > LG Then
         event_period = LG
      End If
      '>>>>>> End (C0359)
   Case 4            ' cumulative oil production
      vector = 1
      GoSub zzzGetEventPeriod_Cumulative
   Case 5            ' cumulative gas production
      vector = 2
      GoSub zzzGetEventPeriod_Cumulative
   Case 6            ' cumulative other volume 1 production
      vector = 3
      GoSub zzzGetEventPeriod_Cumulative
   Case 7            ' cumulative other volume 2 production
      vector = 4
      GoSub zzzGetEventPeriod_Cumulative
   Case 8            ' cumulative equivalent production (price equivalence)
      GoSub zzzGetEventPeriod_Equivalent
      cumulative = 0
      event_period = LG
      '<<<<<< 30 Jul 2001 JWD (C0359)
      ' Change to start at production rather than project start
      For i = Y1 - YR + 1 To LG
      '~~~~~~ was:
      'For i = 1 To LG
      '>>>>>> End (C0359)
         cumulative = cumulative + the_volume(i)
         If cumulative >= Value Then
            event_period = i
            Exit For
         End If
      Next i
   Case 9            ' percent of oil reserves
      vector = 1
      GoSub zzzGetEventPeriod_Percent
   Case 10           ' percent of gas reserves
      vector = 2
      GoSub zzzGetEventPeriod_Percent
   Case 11           ' percent of other volume 1 reserves
      vector = 3
      GoSub zzzGetEventPeriod_Percent
   Case 12           ' percent of other volume 2 reserves
      vector = 4
      GoSub zzzGetEventPeriod_Percent
   Case 13           ' percent of equivalent reserves (price equivalence)
      GoSub zzzGetEventPeriod_Equivalent
      total = 0
      For i = 1 To LG
         total = total + the_volume(i)
      Next i
      cumulative = 0
      event_period = LG
      '<<<<<< 30 Jul 2001 JWD (C0359)
      ' Change to start at production rather than project start
      For i = Y1 - YR + 1 To LG
      '~~~~~~ was:
      'For i = 1 To LG
      '>>>>>> End (C0359)
         cumulative = cumulative + the_volume(i)
         
         If total = 0 Then
            If total >= Value Then
               event_period = i
               Exit For
            End If
         Else
            If cumulative * 100 / total >= Value Then
               event_period = i
               Exit For
            End If
         End If
      Next i
   Case Else
      event_period = -1
   End Select
   
   zzzGetEventPeriod = event_period
   
   Exit Function

zzzGetEventPeriod_Cumulative:
   ' Accumulate the specified vector and return
   ' the period in which the value is exceeded.
   cumulative = 0
   event_period = LG
   '<<<<<< 30 Jul 2001 JWD (C0359)
   ' Change to start at production rather than project start
   For i = Y1 - YR + 1 To LG
   '~~~~~~ was:
   'For i = 1 To LG
   '>>>>>> End (C0359)
      cumulative = cumulative + A(i, vector)
      If cumulative >= Value Then
         event_period = i
         Exit For
      End If
   Next i
   Return
   
zzzGetEventPeriod_Percent:
   total = 0
   For i = 1 To LG
      total = total + A(i, vector)
   Next i
   cumulative = 0
   event_period = LG
   '<<<<<< 30 Jul 2001 JWD (C0359)
   ' Change to start at production rather than project start
   For i = Y1 - YR + 1 To LG
   '~~~~~~ was:
   'For i = 1 To LG
   '>>>>>> End (C0359)
      cumulative = cumulative + A(i, vector)
      
      If total = 0 Then
      
        If total >= Value Then
           event_period = i
           Exit For
        End If
      
      Else
      
        If cumulative * 100 / total >= Value Then
           event_period = i
           Exit For
        End If
      End If
   Next i
   Return
   
zzzGetEventPeriod_Equivalent:
   '<<<<<< 30 Jul 2001 JWD (C0360)
   ' Determine the price vector for the primary stream
   If PPR = 2 Then
      vector = gc_nAGPC        ' gas price
   Else
      vector = gc_nAOPC        ' oil price
   End If
   '>>>>>> End (C0360)
   ReDim the_volume(1 To LG)
   For i = 1 To LG
      '<<<<<< 30 Jul 2001 JWD (C0360)
      ' Divide by primary stream price to get price equivalent volume
      If A(i, vector) = 0 Then
         the_volume(i) = 0
      Else
      
         'the_volume(i) = (A(i, 1) * A(i, 7) + A(i, 2) * A(i, 8) + A(i, 3) * A(i, 9) + A(i, 4) * A(i, 10)) / A(i, vector)
         
         the_volume(i) = ((A(i, gc_nAOIL) * A(i, gc_nAOPC)) + _
                (A(i, gc_nAGAS) * A(i, gc_nAGPC)) + _
                (A(i, gc_nAOV1) * A(i, gc_nAOP1)) + _
                (A(i, gc_nAOV2) * A(i, gc_nAOP2)) + _
                (A(i, gc_nAOV3) * A(i, gc_nAOP3)) + _
                (A(i, gc_nAOV4) * A(i, gc_nAOP4)) + _
                (A(i, gc_nAOV5) * A(i, gc_nAOP5)) + _
                (A(i, gc_nAOV6) * A(i, gc_nAOP6)) + _
                (A(i, gc_nAOV7) * A(i, gc_nAOP7)) + _
                (A(i, gc_nAOV8) * A(i, gc_nAOP8)) + _
                (A(i, gc_nAOV9) * A(i, gc_nAOP9)) + _
                (A(i, gc_nAOV0) * A(i, gc_nAOP0))) / A(i, vector)
      End If
      '~~~~~~ was:
      'the_volume(i) = A(i, 1) * A(i, 7) + A(i, 2) * A(i, 8) + A(i, 3) * A(i, 9) + A(i, 4) * A(i, 10)
      '>>>>>> End (C0360)
   Next i
   Return
   
End Function

'
' 21 Aug 2001 JWD New (C0392)
'
' Compute and return the compounded inflation factor for
' the specified period. This factor is suitable for
' calculating the inflated future value from the present
' value.
'
' Note: This assumes that if the inflation forecast is not
' long enough to cover the period for which the factor is
' requested, there is no inflation in the intervening
' periods.
'
Private Function zzzGetInflatedFutureValueFactor(ByVal ThePeriod As Long) As Double

   Dim i As Long
   Dim return_value As Double
   Dim the_limit As Long
   
   ' Set the loop limit
   the_limit = UBound(zzz_InflationForecast)
   If the_limit > ThePeriod Then
      the_limit = ThePeriod
   End If
   
   ' The initial value
   return_value = 1
   
   ' Compute the compounded factor from the
   ' forecast of the inflation.
   For i = 1 To the_limit
      return_value = return_value * (1 + zzz_InflationForecast(i) / 100)
   Next i
   
   zzzGetInflatedFutureValueFactor = return_value
   
End Function

'
' Modifications:
' 17 Jul 2001 JWD
'  -> Change so that only periods in the legal range
'     are returned regardless of whether or not the
'     trigger parameters are recognized. (C0352)
'
' 5 Jul 2001 JWD New (C0341)
'
' Determine the period in which the funding start trigger
' occurs.
'
Private Function zzzGetTriggerPeriod() As Integer

   Dim i As Integer
   Dim event_period As Integer
   Dim periodX As Integer
   
   '<<<<<< 17 Jul 2001 JWD (C0352)
   Select Case zzz_StartTriggerFunction
   Case 0         ' earlier
      event_period = LG
   Case 1         ' later
      event_period = 0
   End Select
   
   ' For each of the specified triggers
   For i = 1 To UBound(zzz_StartTriggerParameters)
      ' Get the period in which the event occurred
      periodX = zzzGetEventPeriod(zzz_StartTriggerParameters(i), zzz_StartTriggerParameterValues(i))
      ' Make sure a valid period was returned
      If periodX > 0 Then
         ' Update the current trigger period, based
         ' on trigger event selection function...
         Select Case zzz_StartTriggerFunction
         Case 0         ' earlier
            If periodX < event_period Then
               event_period = periodX
            End If
         Case 1         ' later
            If periodX > event_period Then
               event_period = periodX
            End If
         End Select
      End If
   Next i
   '~~~~~~ was:
   'event_period = zzzGetEventPeriod(zzz_StartTriggerParameters(1), zzz_StartTriggerParameterValues(1))
   '
   'For i = 2 To UBound(zzz_StartTriggerParameters)
   '   periodX = zzzGetEventPeriod(zzz_StartTriggerParameters(i), zzz_StartTriggerParameterValues(i))
   '   Select Case zzz_StartTriggerFunction
   '   Case 0         ' earlier
   '      If periodX < event_period Then
   '         event_period = periodX
   '      End If
   '   Case 1         ' later
   '      If periodX > event_period Then
   '         event_period = periodX
   '      End If
   '   End Select
   'Next i
   '>>>>>> End (C0352)
   
   zzzGetTriggerPeriod = event_period
   
End Function

'
' New 2 Aug 2001 JWD
'
' Remove capital expenditure entries for the automatically
' scheduled cash payments/accrual entries of abandonment.
'
Private Sub zzzRemoveAbandonmentEntriesFromMY3()

   Dim i As Integer
   
   ' First, scan through the capital expenditures array
   ' looking for entries that are the automatically
   ' scheduled abandonment entries. If any are found,
   ' set the dates to values greater than any other
   ' possible.
   
   For i = 1 To my3tt
      If my3(i, 1) = CPXCategoryCode_AbandonmentCashExpenditure Or my3(i, 1) = CPXCategoryCode_AbandonmentAccrualEntry Then
         ' Set year to an impossibly large value, so that these will
         ' be sorted to the end of the array in the next step.
         my3(i, 3) = 9999
      End If
   Next i
   
   OrderCapitalExpendituresByDate
   
   ' Re-establish the number of capital expenditures in the
   ' array by as those entries up to but not including the
   ' abandonment entries.
   For i = 1 To my3tt
      If my3(i, 1) = CPXCategoryCode_AbandonmentCashExpenditure Or my3(i, 1) = CPXCategoryCode_AbandonmentAccrualEntry Then
         my3tt = i - 1
         Exit For
      End If
   Next i
   
End Sub

'
' 26 Jun 2001 JWD New (C0341)
'
' Determine funding payments based on straightline method.
' Add the funding payments to the capital expenditures
' array.
'
' FundingStartPeriod is the actual year, not the ordinal
' period number.
'
Private Sub zzzStraightline(ByVal ExpenditureCategoryCode As Integer, ByVal FundingStartPeriod As Integer, ByVal NumberOfPeriods As Integer, ByVal TotalAmount As Single)
            
   Dim i As Integer
   Dim last_period As Integer
   Dim periodic_amount As Single
   
   last_period = FundingStartPeriod + NumberOfPeriods - 1
   
   If NumberOfPeriods > 1 Then
      
      periodic_amount = TotalAmount / NumberOfPeriods
      
      For i = FundingStartPeriod To last_period
         AddCapitalExpenditure ExpenditureCategoryCode, 7, i, 0, periodic_amount, zzz_WIN, 0
      Next i
   
   Else
      
      If TotalAmount > 0 Then
         AddCapitalExpenditure ExpenditureCategoryCode, 7, FundingStartPeriod, 0, TotalAmount, zzz_WIN, 0
      End If
   
   End If

End Sub

'
' 26 Jun 2001 JWD New (C0341)
'
' Determine funding payments based on unit of production
' method. Add the funding payments to the capital
' expenditures array.
'
' FundingStartPeriod is the actual year, not the ordinal
' period number.
'
Private Sub zzzUnitOfProduction(ByVal ExpenditureCategoryCode As Integer, ByVal FundingStartPeriod As Integer, ByVal NumberOfPeriods As Integer, ByVal TotalAmount As Single, ByVal UOPStreamCode As Integer)
               
   Dim i As Integer
   Dim last_period As Integer
   Dim periodic_amount As Single
   Dim remaining_balance As Single
   Dim periodic_rate_factor() As Single
   Dim total_recovered_reserves As Single
   Dim uop_vol() As Single
   Dim have_reserve_additions As Boolean
   Dim remaining_reserves() As Single
   Dim reserves_added() As Single
   Dim temp_sum As Single
   Dim production_start As Integer
   Dim energy_equivalence_factor As Single
   
   last_period = FundingStartPeriod + NumberOfPeriods - 1
   
   ' Initial amount to spread
   remaining_balance = TotalAmount

   ReDim uop_vol(LG)

   ' Compute the periodic production of the selected stream
   Select Case UOPStreamCode
   Case 1               ' Total production - price equivalence
      For i = 1 To LG
         'uop_vol(i) = (A(i, 1) * A(i, 7)) + (A(i, 2) * A(i, 8)) + (A(i, 3) * A(i, 9)) + (A(i, 4) * A(i, 10))
         
         uop_vol(i) = (A(i, gc_nAOIL) * A(i, gc_nAOPC)) + _
                    (A(i, gc_nAGAS) * A(i, gc_nAGPC)) + _
                    (A(i, gc_nAOV1) * A(i, gc_nAOP1)) + _
                    (A(i, gc_nAOV2) * A(i, gc_nAOP2)) + _
                    (A(i, gc_nAOV3) * A(i, gc_nAOP3)) + _
                    (A(i, gc_nAOV4) * A(i, gc_nAOP4)) + _
                    (A(i, gc_nAOV5) * A(i, gc_nAOP5)) + _
                    (A(i, gc_nAOV6) * A(i, gc_nAOP6)) + _
                    (A(i, gc_nAOV7) * A(i, gc_nAOP7)) + _
                    (A(i, gc_nAOV8) * A(i, gc_nAOP8)) + _
                    (A(i, gc_nAOV9) * A(i, gc_nAOP9)) + _
                    (A(i, gc_nAOV0) * A(i, gc_nAOP0))
         
      Next i
   Case 2               ' Total production - energy equivalence
   
      Dim dTotalOilProduction As Double
      Dim dTotalGasProduction As Double
   
      energy_equivalence_factor = gn(2)
      If gn(2) <= 0 Then
         energy_equivalence_factor = 6
      End If
      
      For i = 1 To LG
         'uop_vol(i) = A(i, 1) + (A(i, 2) / energy_equivalence_factor) + A(i, 3) + (A(i, 4) / energy_equivalence_factor)
         
        dTotalOilProduction = A(i, gc_nAOIL) + A(i, gc_nAOV1) + A(i, gc_nAOV3) + _
                A(i, gc_nAOV5) + A(i, gc_nAOV7) + A(i, gc_nAOV9)
                
        dTotalGasProduction = A(i, gc_nAGAS) + A(i, gc_nAOV2) + A(i, gc_nAOV4) + _
                A(i, gc_nAOV6) + A(i, gc_nAOV8) + A(i, gc_nAOV0)
        
        
        uop_vol(i) = dTotalOilProduction + (dTotalGasProduction / energy_equivalence_factor)
         
         
      Next i
   Case 3               ' Oil production only
      For i = 1 To LG
         uop_vol(i) = A(i, 1)
      Next i
   Case 4               ' Gas production only
      For i = 1 To LG
         uop_vol(i) = A(i, 2)
      Next i
   Case 5               ' Other volume 1 only
      For i = 1 To LG
         uop_vol(i) = A(i, 3)
      Next i
   Case 6               ' Other volume 2 only
      For i = 1 To LG
         uop_vol(i) = A(i, 4)
      Next i
   Case Else '3/24/97  added to allow UOP depreciation to be based on any previously defined variable
'      If UOPStrm > 100 Then
'         For i = 1 To LG
'            uop_vol(i) = RVN(i, UOPStrm - 100)
'            recovered_reserves = recovered_reserves + uop_vol(i)
'         Next i
'      End If
'
   End Select
   
   ' Compute the total recovered reserves
   total_recovered_reserves = 0
   For i = 1 To LG
      total_recovered_reserves = total_recovered_reserves + uop_vol(i)
   Next i
   
   
   ' Now determine Reserve Additions
   have_reserve_additions = False
   For i = 1 To LG
      If A(i, 5) <> 0 Then
         have_reserve_additions = True
         Exit For
      End If
   Next i
   
   production_start = Y1 - YR + 1

   ReDim reserves_added(LG)
   If have_reserve_additions Then   ' Reserve Additions(%) entered in Base Data
      temp_sum = 0
      For i = 1 To production_start   ' Accumulate Reserve Additions prior to production start
         temp_sum = temp_sum + (total_recovered_reserves * (A(i, 5) / 100))
      Next i
      reserves_added(production_start) = temp_sum
      For i = (production_start + 1) To LG           ' Now put in any reserve additions after production start year
         reserves_added(i) = total_recovered_reserves * (A(i, 5) / 100)
      Next i
   Else                             ' No Reserve Additions entered in Base Data
      reserves_added(production_start) = total_recovered_reserves
      For i = (production_start + 1) To LG
         reserves_added(i) = 0
      Next i
   End If

   ' Calculate Remaining reserves
   ReDim remaining_reserves(LG)
   remaining_reserves(production_start) = reserves_added(production_start)

   For i = (production_start + 1) To LG
      remaining_reserves(i) = remaining_reserves(i - 1) - uop_vol(i - 1) + reserves_added(i)
   Next i

   ' Calculate UOP 'Depreciation' Rate
   ReDim periodic_rate_factor(1 To LG) As Single
   For i = production_start To LG
      If remaining_reserves(i) > 0 Then
         periodic_rate_factor(i) = uop_vol(i) / remaining_reserves(i)
      Else
         periodic_rate_factor(i) = 0
      End If
   Next i

   ' Add the funding payments to the capital expenditures
   If NumberOfPeriods > 1 Then
      For i = FundingStartPeriod To last_period - 1
         periodic_amount = remaining_balance * periodic_rate_factor(i - YR + 1)
         AddCapitalExpenditure ExpenditureCategoryCode, 7, i, 0, periodic_amount, zzz_WIN, 0
         remaining_balance = remaining_balance - periodic_amount
      Next i
   End If
   
   If remaining_balance > 0 Then
      ' Put the remaining amount in the last period
      AddCapitalExpenditure ExpenditureCategoryCode, 7, last_period, 0, remaining_balance, zzz_WIN, 0
   End If

End Sub

'
' Modifications:
' 12 Sep 2002 JWD
'  -> Move the trigger on change of life for economic
'     limit code to this procedure. Puts all relevant
'     code in this routine. (C0592)
'  -> Add code to reimplement the economic limit event
'     behavior in accordance with new specification.
'     (C0592)
'
' 20 May 2004 JWD
'  -> Add test for no abandonment expenditure entry and
'     exit if none. Procedure assumes that there is an
'     expenditure amount. Assumption led to subscript
'     out of range error when no expenditure amount, the
'     country file specified abandonment provisions, and
'     economic limit was imposed. (C0799)
'
' 10 Sep 2002 JWD New (C0588)
'
'
' Do the things that have to be done for the abandonment
' provisions when certain things happen.
'
' This procedure provides a generic interface for causing
' things to be done with respect to the abandonment provisions.
'
'
Public Sub TriggerAbandonmentProvisionsModelEvent(ByVal TheEvent As Long)
   
   Dim z_method As Long

   Select Case TheEvent
   Case OnChangeOfProjectLifeForLicenseTerm
      ' Project Life changed because of license term limitation
      ' Assumption here is same as for economic limit...
      ' Abandonment is rescheduled based on new life, and is not
      ' done if a pre-tax consolidation.
      
      ' Is this run a pre-tax consolidation?
      If g_bPTCons Then
         ' It is, so ...
         Exit Sub
      End If
   
      ' 20 May 2004 JWD (C0799) Add test for expenditure
      If Not zzz_HaveAbandonmentExpenditureData Then
          Exit Sub
      End If
   
      zzzRemoveAbandonmentEntriesFromMY3
      zzzRescheduleAbandonment
      
   ' 12 Sep 2002 JWD (C0592) Add this new case
   '  Part of this code is moved from TriggerAbandonment-
   '  ProvisionsEconomicLimitEvent().
   Case OnChangeOfProjectLifeForEconomicLimit
      ' Project life changed because of economic limit
      ' of project reached.
      
      ' Is this run a pre-tax consolidation?
      If g_bPTCons Then
         ' It is, so ...
         Exit Sub
      End If
      
      ' 20 May 2004 JWD (C0799) Add test for expenditure
      If Not zzz_HaveAbandonmentExpenditureData Then
          Exit Sub
      End If
   
      If zzz_HaveAbandonmentFundingProvisions Then
         ' As of 12 Sep 2002, the method has changed for
         ' handling economic limit when the fiscal terms
         ' contain abandonment privisions.
         z_method = 3
      Else
         z_method = 2
      End If
      
      Select Case z_method
      Case 2
         ' Per specification given 2 Aug 2001, abandonment
         ' completely redone based on the new project end
         ' year. That is, any previously scheduled
         ' abandonment expenditures are removed from the
         ' capex array, and the abandonment provisions are
         ' applied again.
                  
         zzzRemoveAbandonmentEntriesFromMY3
         
         ' Reapply the abandonment provisions
         zzzRescheduleAbandonment
         
         ' Make sure the capital expenditures are in date order.
         OrderCapitalExpendituresByDate
         
         ' Recompute the capex working interest array to
         ' correspond with a reordered capex array
         zzzRecomputeWINCValues
         
      Case 3
         ' Per specification given 11 Sep 2002, will not
         ' re-apply provisions with new project end year
         ' but will expend the entire remaining amount of
         ' abandonment in last year of project.
                  
         Select Case zzz_CashAccrualOption
         Case zzz_CashAccrualOption_Accrual
            zzzUpdateAbandonmentFunding_AccrualMethod
            
         Case zzz_CashAccrualOption_Cash
            zzzUpdateAbandonmentFunding_CashMethod
            
         End Select
      
      End Select
      ' End (C0592)
      
      
   'Case ?????
      ' Add others here?
      
   Case Else
   
   End Select
   

End Sub

'
' 10 Sep 2002 JWD New (C0588)
'
' Reschedule the abandonment provisions. This
' is done if the life of the project changes
' for some reason.
'
Private Sub zzzRescheduleAbandonment()

   ' This assumes MY3TT is used to record the
   ' count of capital expenditures, but
   ' AddCapitalExpenditure() uses MY3T as the count, so
   ' save the value of MY3T and then make it equal to
   ' MY3TT before calling the
   ' ApplyAbandonmentFundingProvisions() procedure
   ' (which calls AddCapitalExpenditure).
   Dim save_value As Single
   
   save_value = MY3T
   MY3T = my3tt
   
   ApplyAbandonmentFundingProvisions
   AbandonmentPlacementOffset = zzz_AbandonmentExpenditurePeriodOffset
      
   my3tt = MY3T
   MY3T = save_value
   
End Sub

'
' 12 Sep 2002 JWD New (C0592)
'
' Recompute the capital working interest values
' from the capital expense array. (Basically,
' WINC() is just a copy of the working interest
' column of the capex array, converted to a
' fraction.)
'
Private Sub zzzRecomputeWINCValues()

   Dim i As Integer
   
   ' After the MY3 array is updated, need to regenerate
   ' the WINC() array in case any of the entries have
   ' changed positions. This is done only if the WIN
   ' command in Fiscal Definition has already been
   ' processed. (Because the order of entries in WINC()
   ' if it (WIN) hasn't been processed doesn't matter
   ' since they are all 100% beforehand.)
      
   If WINT <> 0 Then
      For i = 1 To my3tt
         WINC(i) = my3(i, 6) / 100
      Next i
   End If

End Sub

'
' 12 Sep 2002 JWD New (C0592)
'
' Re-establish the count of capital expenditures
' after removal of unwanted entries. These entries
' will have 9999 as the year and should have been
' sorted to the end of the MY3() array.
'
Private Sub zzzDetermineMY3Count()

   Dim i As Integer
   
   ' Re-establish the number of capital expenditures in the
   ' array by as those entries up to but not including the
   ' abandonment entries with the large year value.
   For i = 1 To my3tt
      If my3(i, 3) = 9999 Then
         my3tt = i - 1
         Exit For
      End If
   Next i
   
End Sub

'
' 13 Sep 2002 JWD New (C0592)
'
' Determine the cumulative amount of abandonment fund
' accruals through the last year and update the entry
' in the last year with the balance remaining to be
' accrued. This is done in the event of the economic
' limit being reached, on projects that have abandonment
' funding provisions.
'
Private Sub zzzUpdateAbandonmentFunding_AccrualMethod()

   Dim i As Integer
   Dim z_amount As Single
   Dim z_sum As Single
   Dim z_update_flag As Boolean
   
   
   ' Remove accrual entries scheduled after the new
   ' project end, and ALL cash expenditures.
   
      ' Set year to an impossibly large value, so that these will
      ' be sorted to the end of the array in the next step.
      For i = 1 To my3tt
         If my3(i, 3) > YR + LG - 1 Then
            If my3(i, 1) = CPXCategoryCode_AbandonmentAccrualEntry Then
               my3(i, 3) = 9999 ' Accruals after project end
            End If
         Else
            If my3(i, 1) = CPXCategoryCode_AbandonmentCashExpenditure Then
               my3(i, 3) = 9999 ' All of them
            End If
         End If
      Next i
      
      ' Sort them to the end of the array
      OrderCapitalExpendituresByDate
      
      ' Truncate off the deleted items
      zzzDetermineMY3Count
   
   
   ' Get the inflated abandonment amount for the new
   ' project end. Since it is a new ending period,
   ' presumably before the un-limited end of the project,
   ' the amount should be different because of "less"
   ' inflation. Get the abandonment amount, inflated
   ' to the new last year of the project.
   z_amount = zzz_GrossAbandonmentExpenditure * zzzGetInflatedFutureValueFactor(LG)
   
   ' Add up the scheduled abandonment accrual entries.
   ' These were scheduled based on the un-limited project
   ' end and the specified abandonment funding provisions.
   ' Only sum the accrual entries.
   z_sum = 0
   For i = 1 To my3tt
      If my3(i, 1) = CPXCategoryCode_AbandonmentAccrualEntry Then
         z_sum = z_sum + my3(i, 5)
      End If
   Next i
      
   ' Now go back and update the first abandonment entry
   ' made in the last year with the additional amount
   ' needed to accrue the entire inflated abandonment
   ' amount by the end of the project.
   z_update_flag = False
   For i = 1 To my3tt
      If my3(i, 1) = CPXCategoryCode_AbandonmentAccrualEntry Then
         If my3(i, 3) = YR + LG - 1 Then
            my3(i, 5) = my3(i, 5) + (z_amount - z_sum)
            z_update_flag = True
            ' Just in case there is more than one, not likely, but....
            Exit For
         End If
      End If
   Next i
      
   ' See if an entry was actually updated. It is possible
   ' that all of the entries were removed by the limit.
   ' If no entry was updated, add an entry for the entire
   ' amount.
   
   ' AddCapitalExpenditure uses MY3T as the count,
   ' so save value and set equal to MY3TT.
   Dim z_my3t As Integer
   z_my3t = MY3T
   MY3T = my3tt
   
   If Not z_update_flag Then
      AddCapitalExpenditure CPXCategoryCode_AbandonmentAccrualEntry, 7, YR + LG - 1, 0, z_amount, zzz_WIN, 0
   End If
         
   ' Finally, add in the cash expenditure to provide the
   ' cash flow effect.
   AddCapitalExpenditure CPXCategoryCode_AbandonmentCashExpenditure, 7, YR + LG - 1, 0, z_amount, zzz_WIN, 0
      
   ' Now restore the MY3T value
   my3tt = MY3T
   MY3T = z_my3t
   
   OrderCapitalExpendituresByDate
   
   ' Recreate working interest array
   zzzRecomputeWINCValues
   
End Sub

'
' 13 Sep 2002 JWD New (C0592)
'
' Determine the cumulative amount of abandonment fund
' cash expenditures through the last year and update
' the entry in the last year with the balance remaining
' to be expended. This is done in the event of the
' economic limit being reached, on projects that have
' abandonment funding provisions.
'
' This routine assumes that abandonment entries after the
' new project end period have been removed from the capex
' array (MY3()).
'
Private Sub zzzUpdateAbandonmentFunding_CashMethod()

   Dim i As Integer
   Dim z_amount As Single
   Dim z_sum As Single
   Dim z_update_flag As Boolean
   
   ' Remove cash expenditure entries scheduled
   ' after the new project end
   
      ' Set year to an impossibly large value, so that these will
      ' be sorted to the end of the array in the next step.
      For i = 1 To my3tt
         If my3(i, 3) > YR + LG - 1 Then
            If my3(i, 1) = CPXCategoryCode_AbandonmentCashExpenditure Then
               my3(i, 3) = 9999 ' cash expenditures after project end
            End If
         End If
      Next i
      
      ' Sort them to the end of the array
      OrderCapitalExpendituresByDate
      
      ' Truncate off the deleted items
      zzzDetermineMY3Count
   
   
   ' Get the inflated abandonment amount for the new
   ' project end. Since it is a new ending period,
   ' presumably before the un-limited end of the project,
   ' the amount should be different because of "less"
   ' inflation. Get the abandonment amount, inflated
   ' to the new last year of the project.
   z_amount = zzz_GrossAbandonmentExpenditure * zzzGetInflatedFutureValueFactor(LG)
   
   ' Add up the scheduled abandonment cash expenditure
   ' entries. These were scheduled based on the un-limited
   ' project end and the specified abandonment funding
   ' provisions.
   ' Only sum the cash expenditure entries.
   z_sum = 0
   For i = 1 To my3tt
      If my3(i, 1) = CPXCategoryCode_AbandonmentCashExpenditure Then
         z_sum = z_sum + my3(i, 5)
      End If
   Next i
      
   ' Now go back and update the first abandonment entry
   ' made in the last year with the additional amount
   ' needed to accrue the entire inflated abandonment
   ' amount by the end of the project.
   z_update_flag = False
   For i = 1 To my3tt
      If my3(i, 1) = CPXCategoryCode_AbandonmentCashExpenditure Then
         If my3(i, 3) = YR + LG - 1 Then
            my3(i, 5) = my3(i, 5) + (z_amount - z_sum)
            z_update_flag = True
            ' Just in case there is more than one, not likely, but....
            Exit For
         End If
      End If
   Next i
      
   ' See if an entry was actually updated. It is possible
   ' that all of the entries were removed by the limit.
   ' If no entry was updated, add an entry for the entire
   ' amount.
   If Not z_update_flag Then
      ' AddCapitalExpenditure uses MY3T as the count,
      ' so save value and set equal to MY3TT.
      Dim z_my3t As Integer
      z_my3t = MY3T
      MY3T = my3tt
   
      AddCapitalExpenditure CPXCategoryCode_AbandonmentCashExpenditure, 7, YR + LG - 1, 0, z_amount, zzz_WIN, 0
      
      my3tt = MY3T
      MY3T = z_my3t
   End If
         
   OrderCapitalExpendituresByDate
   
   ' Recreate working interest array
   zzzRecomputeWINCValues
   
End Sub

