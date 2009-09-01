Option Strict Off
Option Explicit On
Interface IDPersistClassMap
    Function ClassFormat(ByVal TheObject As IDPersistObject) As IDPersistFormat
    Function MakeObjectOfClass(ByVal TheClassName As String) As IDPersistObject
End Interface
