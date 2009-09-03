Option Strict Off
Option Explicit On
Friend Class CDPersistFormatDummy
    Implements IDPersistFormat

    ' This is a do-nothing format



    Private Function IDPersistFormat_Restore(ByVal TheStore As IDStore, ByVal TheObject As IDPersistObject) As Boolean Implements IDPersistFormat.Restore

    End Function

    Private Function IDPersistFormat_Store(ByVal TheStore As IDStore, ByVal TheObject As IDPersistObject) As Boolean Implements IDPersistFormat.Store

    End Function
End Class