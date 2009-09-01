Option Strict Off
Option Explicit On
Interface IGiantRptPageAssignStd
    Sub SetPageHeader(ByVal PageType As Short, ByVal startyear As Short, ByVal PageCount As Short, ByVal ProjectLife As Short, ByVal ProfileCount As Short, ByVal PageTitle As String, ByVal ColumnWidth As Short, ByVal FinalWorkingInt As Single, ByVal FinalParticipation As Single, ByVal PageCurrency As String)
    Sub SetProfileHeaders(ByVal ParamArray ProfileNames() As Object)
    Sub SetProfileValues(ByVal RowIndex As Short, ByVal ParamArray ProfileValues() As Object)
End Interface
