Attribute VB_Name = "MForecastUtilsA"
'
' Contains Forecasting support procedures common to
' data forecasting and country annual forecasting.
'---------------------------------------------------------
Option Explicit

'
' Originally in DTA2000A.BAS
'
Sub ForecastLoadA(item As Integer, Datacol() As Single, z() As Single)
'--------------------------------------------------------------------
'  parameters: Category$, datacol!(), z()
'  function: takes datacol() and fills out z(1-LG, item)
'  returns: ---
'---------------------------------------------------------
   Dim p As Integer
'---------------------------------------------------------
4350
   For p = 1 To LG
      z(p, item) = Datacol(p)
   Next p

End Sub


