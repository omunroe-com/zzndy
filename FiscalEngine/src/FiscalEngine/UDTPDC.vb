Option Strict Off
Option Explicit On
Module UDTPDC
	'record for PDCScreen (Production Decline Curves)
	Structure PDCType 'DIMMED in common as PDC()
		'UPGRADE_WARNING: Fixed-length string size must fit in the buffer. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"'
		<VBFixedString(3),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=3)> Public cat() As Char 'category
		'UPGRADE_WARNING: Fixed-length string size must fit in the buffer. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"'
		<VBFixedString(3),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=3)> Public unit() As Char 'time unit (day, mon, yrs)
		'UPGRADE_WARNING: Fixed-length string size must fit in the buffer. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"'
		<VBFixedString(3),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=3)> Public mtd() As Char 'decline method (exp, har, hyp)
		Dim begprod As Single 'beginning production rate
		'UPGRADE_NOTE: RATE was upgraded to RATE_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim RATE_Renamed As Single 'escalation/decline rate
		Dim hypexp As Single 'hyperbolic exponent
		Dim endprod As Single 'ending prod rate
		Dim cumprod As Single 'cumulative production rate
		Dim time As Single 'time in years
	End Structure
End Module