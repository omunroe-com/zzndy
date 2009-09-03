Option Strict Off
Option Explicit On
Module UDTCPX
	'record for CPXScreen (Capital Expenditure
	Structure CPXType
		'UPGRADE_WARNING: Fixed-length string size must fit in the buffer. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"'
		<VBFixedString(3),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=3)> Public cat() As Char 'category
		Dim dat As String 'month/year
		Dim tan As Single 'tangible percent
		Dim amt As Single 'gross amount
		Dim WIN As Single 'working interest percent
		Dim remb As Single 'reimburse percent
		'UPGRADE_WARNING: Fixed-length string size must fit in the buffer. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"'
		<VBFixedString(24),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=24)> Public desc() As Char 'description
	End Structure
End Module