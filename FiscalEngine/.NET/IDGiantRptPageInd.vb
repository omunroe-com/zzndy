Option Strict Off
Option Explicit On
Interface IDGiantRptPageInd
    Property PageHeader() As CGiantRptPageHdr1
    Property Headers() As String()
    Property Rates() As Single()
    Property Values() As Single()
End Interface
