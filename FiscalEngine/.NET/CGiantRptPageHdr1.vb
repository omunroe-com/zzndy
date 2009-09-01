Option Strict Off
Option Explicit On
Friend Class CGiantRptPageHdr1
    Implements IDPersistObject
	' Name:         CGiantRptPageHdr1.cls
	' Function:     Defines Page Header common to all pages
	' Date:         28 Jan 2004 JWD
	'---------------------------------------------------------
	
	
	'
	' IDPersistObject attributes
	'
	Private Const m_lClassID As Integer = 9
	Private Const m_sClassName As String = "CGiantRptPageHdr1"
	Private m_lObjectID As Integer
	
	
	Public Columns As Short
	Public ColumnWidth As Short
	Public CompanyWorkingInterest As Single
	Public CurrencyCode As String
	
	Public GovernmentParticipation As Single
	
	Public PageCounter As Short
	Public PageTitle As String
	Public PageType As Short
	
	Public Rows As Short
	
	'UPGRADE_NOTE: year was upgraded to year_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Public year_Renamed As Short
	
	
	'=========================================================
	'
	' IDPersistObject Interface
	'
	
	
    Private ReadOnly Property IDPersistObject_ClassIDNumber() As Integer Implements IDPersistObject.ClassIDNumber
        Get
            IDPersistObject_ClassIDNumber = m_lClassID
        End Get
    End Property

    Private ReadOnly Property IDPersistObject_ClassName() As String Implements IDPersistObject.ClassName
        Get
            IDPersistObject_ClassName = m_sClassName
        End Get
    End Property


    Private Property IDPersistObject_ObjectIDNumber() As Integer Implements IDPersistObject.ObjectIDNumber
        Get
            IDPersistObject_ObjectIDNumber = m_lObjectID
        End Get
        Set(ByVal Value As Integer)
            m_lObjectID = Value
        End Set
    End Property

    Private Function IDPersistObject_RegisterInTable(ByVal ObjectTable As IDPersistObjectTable) As Boolean Implements IDPersistObject.RegisterInTable
        'UPGRADE_WARNING: Couldn't resolve default property of object Me. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        ObjectTable.RegisterObject(Me)
    End Function

    Private Function IDPersistObject_RegisterObjectConnections() As Boolean Implements IDPersistObject.RegisterObjectConnections
    End Function

    Private Function IDPersistObject_RestoreUsingFormat(ByVal TheStore As IDStore, ByVal TheMap As IDPersistClassMap) As Boolean Implements IDPersistObject.RestoreUsingFormat
    End Function

    Private Function IDPersistObject_StoreUsingFormat(ByVal TheStore As IDStore, ByVal TheMap As IDPersistClassMap) As Boolean Implements IDPersistObject.StoreUsingFormat

        'UPGRADE_WARNING: Couldn't resolve default property of object Me. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        TheMap.ClassFormat(Me).Store(TheStore, Me)

    End Function
End Class