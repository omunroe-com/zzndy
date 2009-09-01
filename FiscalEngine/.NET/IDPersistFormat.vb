Option Strict Off
Option Explicit On
Interface IDPersistFormat
    Function Restore(ByVal TheStore As IDStore, ByVal TheObject As IDPersistObject) As Boolean
    Function Store(ByVal TheStore As IDStore, ByVal TheObject As IDPersistObject) As Boolean
End Interface
