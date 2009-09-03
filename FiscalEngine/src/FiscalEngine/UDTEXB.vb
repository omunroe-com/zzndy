Option Strict Off
Option Explicit On
Module UDTEXB
	'record for exchange rate data
	Structure EXBType
        Public Cat As String
        Public unit As String
		Dim dat As Short
		Dim mtd As Short
		Dim parm1 As Single
		Dim parm2 As Single
		Dim parm3 As Single
		Dim parm4 As Single
		Dim parm5 As Single
		Dim parm6 As Single
	End Structure
End Module