VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "IVariableTitlesA"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' Name:         IVariableTitlesA.cls
' Function:     Fiscal Variable Titles table interface
' Date:         20 Jan 2004 JWD
'---------------------------------------------------------
' Defines an interface for objects that provide fiscal
' variable title lookup services to other objects.
'---------------------------------------------------------
Option Explicit


Public Function LongTitle _
    ( _
    ByVal VariableCode As String _
    ) As String
End Function

Public Function IsUserVariable _
    ( _
    ByVal VariableCode As String _
    ) As Boolean

End Function
    

