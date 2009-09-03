Option Strict Off
Option Explicit On
Interface IDPersistObject
    ReadOnly Property ClassIDNumber() As Integer
    ReadOnly Property ClassName() As String
    Property ObjectIDNumber() As Integer
    Function RegisterInTable(ByVal ObjectTable As IDPersistObjectTable) As Boolean
    Function RegisterObjectConnections() As Boolean
    Function RestoreUsingFormat(ByVal TheStore As IDStore, ByVal TheMap As IDPersistClassMap) As Boolean
    Function StoreUsingFormat(ByVal TheStore As IDStore, ByVal TheMap As IDPersistClassMap) As Boolean
End Interface
