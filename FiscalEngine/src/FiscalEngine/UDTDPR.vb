Option Strict Off
Option Explicit On
Module UDTDPR
	'record type for DPRScreen (Depreciation/Cost Recovery)
	Structure DPRType
        Public var As String 'variable
        Public Cat As String 'category
        Public pre As String 'pre/post
        Public tan As String 'tangible/intangible
        Dim DPR As Single '% depreciated
        Public mtd As String 'method
		Dim dbr As Single 'db rate
		Dim PRD As Single 'period (years)
		Dim all As Single 'year 1 allowance
		Dim crd As Single 'credit %
		'UPGRADE_NOTE: int was upgraded to int_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim int_Renamed As Single 'interest %
		'UPGRADE_WARNING: Fixed-length string size must fit in the buffer. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"'
		<VBFixedString(3),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=3)> Public acc() As Char 'accrual method
	End Structure
End Module