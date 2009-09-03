Option Strict Off
Option Explicit On
''' <summary>
''' This defines a standard interface for pages to have
''' data elements assigned to the page object. This specific
''' interface implements methods for assignment of the page
''' header, assignment of the column headers by explicit
''' enumeration of the headers in the method call as a
''' ParamArray, and row by row assignment of values by
''' explicit enumeration of the values of each row in the
''' method call as a ParamArray. These methods replace the
''' Write statements previously used.
''' 
''' This interface extends the IGiantRptPageAssignStd inter-
''' face with the addition of the SetRptPageVariableCode
''' method.
''' </summary>
''' <remarks></remarks>
Interface IGiantRptPageAssignSub
    Inherits IGiantRptPageAssignStd

    ''' <summary>
    ''' This sets the variable code for the page implementing
    ''' this interface. This is primarily for worksheet pages
    ''' that are associated with another page. The code
    ''' identifies the user defined variable page with which
    ''' this page is associated.
    ''' </summary>
    ''' <param name="VariableCode"></param>
    ''' <remarks></remarks>
    Sub SetRptPageVariableCode(ByVal VariableCode As String)

'    ''' Set all elements of the page header.
'    ''' 
'    ''' This is to replace the Write statement that writes
'    ''' this data on the report file.
'    Sub SetPageHeader(ByVal PageType As Short, ByVal startyear As Short, ByVal PageCount As Short, ByVal ProjectLife _
'                          As Short, ByVal ProfileCount As Short, ByVal PageTitle As String, ByVal ColumnWidth As Short, _
'                       ByVal _
'                          FinalWorkingInt As Single, ByVal FinalParticipation As Single, ByVal PageCurrency As String)
'
'    ' Set the profile (column) names
'    '
'    ' This is to replace the Write statement that writes
'    ' this data on the report file.
'    Sub SetProfileHeaders (ByVal ParamArray ProfileNames() As Object)
'
'    ' Set the values for all profiles for the specified row
'    '
'    ' This is to replace the Write statement that writes
'    ' this data on the report file.
'    '
'    ' Assumes that the RowIndex is in the range 1-ProjectLife
'    ' (1 To Ubound(ma_rValues, 1) + 1).
'    Sub SetProfileValues(ByVal RowIndex As Short, ByVal ParamArray ProfileValues() As Object)
End Interface
