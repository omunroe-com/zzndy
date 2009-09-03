Option Strict Off
Option Explicit On
Friend Class CSFGiantRunSummarySeqA
    Implements IDPersistFormat
    ' Name:         CSFGiantRunSummarySeqA.cls
    ' Function:     Sequential text Giant Run Summary format
    ' Date:         22 Sep 2004 JWD
    '---------------------------------------------------------
    ' Format for one run's run summary (other economic
    ' indicators) file data.
    '
    ' This format outputs the data that is present in the
    ' run summary file 'run-file.sum'.
    '---------------------------------------------------------
    ' Modifications:
    '  -> Changed IDPersistFormat_Store(). (C0846)
    '---------------------------------------------------------




    '=========================================================
    '
    ' IDPersistFormat
    '

    Private Function IDPersistFormat_Restore(ByVal TheStore As IDStore, ByVal TheObject As IDPersistObject) As Boolean Implements IDPersistFormat.Restore
    End Function

    '
    ' Modifications:
    ' 3 Dec 2004 JWD
    '  -> Add 3rd party and NOC ROR indicators, reorder Gov.
    '     and Comp. ROR indicators relative to one another,
    '     Comp. first, and Govt. last. (C0846)
    '
    Private Function IDPersistFormat_Store(ByVal TheStore As IDStore, ByVal TheObject As IDPersistObject) As Boolean Implements IDPersistFormat.Store

        ' Interface references
        Dim l_oObject As _IDGiantRunSummaryA ' Stores Other Economic Indicator
        Dim l_oStore As IDStoreSeq ' in sequential text format

        ' Get the desired interfaces from the object and store
        l_oObject = TheObject
        l_oStore = TheStore

        ' Write out the Run Summary data

        With l_oStore

            .NextItem = l_oObject.RunTitle
            .NextItemLineEnd = l_oObject.RunCurrency

            .NextItem = l_oObject.TotalEquivalentReserves
            '.NextItem = l_oObject.TotalEquivalentVolumetricReserves
            .NextItem = l_oObject.TotalOperatingExpenses
            .NextItem = l_oObject.TotalExplorationCapital
            .NextItem = l_oObject.TotalDevelopmentCapital
            .NextItem = l_oObject.TotalOtherCapital
            .NextItem = l_oObject.AverageEquivalentPrice
            .NextItem = l_oObject.UnitAverageOperatingExpenses
            .NextItem = l_oObject.UnitAverageExplorationCapital
            .NextItem = l_oObject.UnitAverageDevelopmentCapital
            .NextItem = l_oObject.UnitAverageOtherCapital
            .NextItem = l_oObject.ProductionLife
            .NextItem = l_oObject.ProjectLife

            ' 3 Dec 2004 JWD (C0846) Change order of company and govt ror with respect to one another, insert 3rd party and NOC ROR indicators
            .NextItem = l_oObject.CompanyRateOfReturn
            .NextItem = l_oObject.ThirdPartyRateOfReturn
            .NextItem = l_oObject.NOCRateOfReturn
            .NextItemLineEnd = l_oObject.GovernmentRateOfReturn
            ' was:
            '.NextItem = l_oObject.GovernmentRateOfReturn
            '.NextItemLineEnd = l_oObject.CompanyRateOfReturn
            ' End (C0846)

            .NextItem = l_oObject.CompanyIncomeDCF1
            .NextItem = l_oObject.CompanyOperatingExpenseDCF1
            .NextItem = l_oObject.CompanyCapitalExpenditureDCF1
            .NextItemLineEnd = l_oObject.CompanyRoyaltyTaxDCF1

            .NextItem = l_oObject.CompanyNetCashFlowDCF1
            .NextItem = l_oObject.CompanyPayoutDCF1
            .NextItem = l_oObject.CompanyRiskReturnRatioDCF1
            .NextItem = l_oObject.CompanyProfitabilityIndexDCF1
            .NextItemLineEnd = l_oObject.CompanyGovernmentTakeDCF1

            .NextItem = l_oObject.CompanyIncomeDCF5
            .NextItem = l_oObject.CompanyOperatingExpenseDCF5
            .NextItem = l_oObject.CompanyCapitalExpenditureDCF5
            .NextItemLineEnd = l_oObject.CompanyRoyaltyTaxDCF5

            .NextItem = l_oObject.CompanyNetCashFlowDCF5
            .NextItem = l_oObject.CompanyPayoutDCF5
            .NextItem = l_oObject.CompanyRiskReturnRatioDCF5
            .NextItem = l_oObject.CompanyProfitabilityIndexDCF5
            .NextItemLineEnd = l_oObject.CompanyGovernmentTakeDCF5

        End With

    End Function
End Class