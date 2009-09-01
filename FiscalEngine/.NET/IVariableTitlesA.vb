Option Strict Off
Option Explicit On
Interface _IVariableTitlesA
	Function LongTitle(ByVal VariableCode As String) As String
	Function IsUserVariable(ByVal VariableCode As String) As Boolean
End Interface
Friend Class IVariableTitlesA
	Implements _IVariableTitlesA
	' Name:         IVariableTitlesA.cls
	' Function:     Fiscal Variable Titles table interface
	' Date:         20 Jan 2004 JWD
	'---------------------------------------------------------
	' Defines an interface for objects that provide fiscal
	' variable title lookup services to other objects.
	'---------------------------------------------------------
	
	
	Public Function LongTitle(ByVal VariableCode As String) As String Implements _IVariableTitlesA.LongTitle
	End Function
	
	Public Function IsUserVariable(ByVal VariableCode As String) As Boolean Implements _IVariableTitlesA.IsUserVariable
		
	End Function
End Class