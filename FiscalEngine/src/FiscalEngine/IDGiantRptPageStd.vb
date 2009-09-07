Option Strict Off
Option Explicit On
Interface IDGiantRptPageStd
    Property PageHeader() As CGiantRptPageHdr1
    Property Headers() As String()
    Property Values() As Single(,)
End Interface
