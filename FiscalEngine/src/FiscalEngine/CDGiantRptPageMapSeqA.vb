Option Strict Off
Option Explicit On
Friend Class CDGiantRptPageMapSeqA
    Implements IDPersistClassMap
    ' Name:         CDGiantRptPageMapSeqA.cls
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
            Case 1, 4, 5, 6, 7, 8
                IDPersistClassMap_ClassFormat = New CGiantRptPageStdFormatSeq
            Case 2
                IDPersistClassMap_ClassFormat = New CGiantRptPageIndFormatSeq
            Case 3
                IDPersistClassMap_ClassFormat = New CGiantRptPagePRTFormatSeq
            Case 9
                IDPersistClassMap_ClassFormat = New CGiantRptPageHdrFormatSeq
            Case Else
                ' Return the do-nothing format
                IDPersistClassMap_ClassFormat = New CDPersistFormatDummy
        End Select

    End Function

    Private Function IDPersistClassMap_MakeObjectOfClass(ByVal TheClassName As String) As IDPersistObject Implements IDPersistClassMap.MakeObjectOfClass
        Throw New NotImplementedException()
    End Function
End Class