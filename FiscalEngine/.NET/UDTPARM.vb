Option Strict Off
Option Explicit On
Module UDTParm
	'record for ANNScreen (Country Annual Forecasts)
	Structure ParmType
		'UPGRADE_WARNING: Fixed-length string size must fit in the buffer. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"'
		<VBFixedString(3),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=3)> Public Cat() As Char 'category
		'UPGRADE_WARNING: Fixed-length string size must fit in the buffer. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"'
		<VBFixedString(3),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=3)> Public unit() As Char 'unit
		'UPGRADE_WARNING: Fixed-length string size must fit in the buffer. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"'
		<VBFixedString(3),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=3)> Public dat() As Char 'start date code
		Dim mtd As Short 'method (1-0)
		'parm1 As String * 8                'parameter 1
		Dim parm1 As String 'parameter 1
		Dim parm2 As Single 'parameter 2
		Dim parm3 As Single 'parameter 3
		Dim parm4 As Single 'parameter 4
		Dim parm5 As Single 'parameter 5
		Dim parm6 As Single 'parameter 6
	End Structure
End Module