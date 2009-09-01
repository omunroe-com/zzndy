Option Strict Off
Option Explicit On
Friend Class CDGiantRptPageMapSeqB
    Implements IDPersistClassMap
    ' Name:         CDGiantRptPageMapSeqB.cls
    ' Function:     Giant Report Page Format Map (sequential)
    ' Date:         28 Jan 2004 JWD
    '---------------------------------------------------------
    ' This provides formats for sequentially formatted Giant
    ' report pages
    '---------------------------------------------------------



    '=========================================================
    '
    ' IDPersistClassMap Interface
    '


    Private Function IDPersistClassMap_ClassFormat(ByVal TheObject As IDPersistObject) As IDPersistFormat Implements IDPersistClassMap.ClassFormat

        Select Case TheObject.ClassIDNumber
            Case 1, 3, 4, 7, 8
                IDPersistClassMap_ClassFormat = New CDFGiantRptPgStdASPEAmts
            Case Else
                ' Return the do-nothing format
                IDPersistClassMap_ClassFormat = New CDPersistFormatDummy
        End Select

    End Function

    Private Function IDPersistClassMap_MakeObjectOfClass(ByVal TheClassName As String) As IDPersistObject Implements IDPersistClassMap.MakeObjectOfClass

    End Function
End Class