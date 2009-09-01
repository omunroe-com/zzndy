Option Strict Off
Option Explicit On
Interface IDGiantRptPageH
    Property PageHeader() As CGiantRptPageHdr1
    Property Headers() As String()
    Property Values() As Single()
    Property VariableCode() As String
End Interface