Option Strict Off
Option Explicit On
Interface IVariableTitlesA
    Function LongTitle(ByVal VariableCode As String) As String
    Function IsUserVariable(ByVal VariableCode As String) As Boolean
End Interface
