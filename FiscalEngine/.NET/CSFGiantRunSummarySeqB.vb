Option Strict Off
Option Explicit On
Friend Class CSFGiantRunSummarySeqB
    Implements IDPersistFormat
    ' Name:         CSFGiantRunSummarySeqB.cls
    ' Function:     Sequential text Giant Run Summary format
    ' Date:         22 Sep 2004 JWD
    '---------------------------------------------------------
    ' Format for one run's run summary (other economic
    ' indicators) file data.
    '
    ' This specific format only outputs the individual
    ' indicators that are not also in the present value table
    ' (i. e. does not include the discounted cash flow items).
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
    '  -> Add 3rd party and NOC ROR indicators between Comp.
    '     and Gov. ROR indicators. (C0846)
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

            .NextItem = l_oObject.TotalEquivalentReserves
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
            .NextItem = l_oObject.CompanyRateOfReturn
            ' 3 Dec 2004 JWD (C0846) Add 3rd Party and NOC ROR indicators
            .NextItem = l_oObject.ThirdPartyRateOfReturn
            .NextItem = l_oObject.NOCRateOfReturn
            ' End (C0846)
            .NextItemLineEnd = l_oObject.GovernmentRateOfReturn

        End With

    End Function
End Class