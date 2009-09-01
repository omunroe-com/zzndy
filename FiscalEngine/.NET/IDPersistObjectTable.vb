Option Strict Off
Option Explicit On
Friend Interface IDPersistObjectTable
    Function Store(ByVal TheStore As IDStore) As Boolean
    Function Load(ByVal TheStore As IDStore) As Boolean
    Function RegisterObject(ByVal TheObject As IDPersistObject) As Boolean
    Property ClassMap() As IDPersistClassMap
End Interface