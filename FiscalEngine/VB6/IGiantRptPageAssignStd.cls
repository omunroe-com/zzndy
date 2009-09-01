VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "IGiantRptPageAssignStd"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' Name:         IGiantRptPageAssignStd.cls
' Function:     Interface for standard page
' Date:         19 Jan 2004 JWD
'---------------------------------------------------------
' This defines a standard interface for pages to have
' data elements assigned to the page object. This specific
' interface implements methods for assignment of the page
' header, assignment of the column headers by explicit
' enumeration of the headers in the method call as a
' ParamArray, and row by row assignment of values by
' explicit enumeration of the values of each row in the
' method call as a ParamArray. These methods replace the
' Write statements previously used.
'---------------------------------------------------------
Option Explicit


'
' Set all elements of the page header.
'
' This is to replace the Write statement that writes
' this data on the report file.
'
Public Sub SetPageHeader _
    ( _
    ByVal PageType As Integer, _
    ByVal startyear As Integer, _
    ByVal PageCount As Integer, _
    ByVal ProjectLife As Integer, _
    ByVal ProfileCount As Integer, _
    ByVal PageTitle As String, _
    ByVal ColumnWidth As Integer, _
    ByVal FinalWorkingInt As Single, _
    ByVal FinalParticipation As Single, _
    ByVal PageCurrency As String _
    )
End Sub

'
' Set the profile (column) names
'
' This is to replace the Write statement that writes
' this data on the report file.
'
Public Sub SetProfileHeaders _
    ( _
    ParamArray ProfileNames() As Variant _
    )
End Sub

'
' Set the values for all profiles for the specified row
'
' This is to replace the Write statement that writes
' this data on the report file.
'
' Assumes that the RowIndex is in the range 1-ProjectLife
' (1 To Ubound(ma_rValues, 1) + 1).
'
Public Sub SetProfileValues _
    ( _
    ByVal RowIndex As Integer, _
    ParamArray ProfileValues() As Variant _
    )
End Sub


