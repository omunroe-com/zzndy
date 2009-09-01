Attribute VB_Name = "RVSCON"
Option Explicit
' $linesize: 132
' $title:    'GIANT v6.1 - 1999                RVSCON.VBG'
' $subtitle: 'RVS file read and write routines for consolidation ne of participation'
' ********************************************************
' *    COPYRIGHT © 1991-1998 PETROCONSULTANTS, INC.      *
' *                ALL RIGHTS RESERVED                   *
' ********************************************************
' * This program file is proprietary information of      *
' * Petroconsultants, Incorporated.  Unauthorized use    *
' * for any purpose is prohibited.                       *
' ********************************************************
'-----------------------------------------------------------------------
' This file is for the VB version of the system.
'-----------------------------------------------------------------------

' Modifications:
' 20 Jan 2003 GDP
'   -> Changed ReadRVS()
'   -> Changed WriteRVS()
'
' 23 May 2003 JWD
'   -> Changed ReadRVS(). (C0700)
'   -> Changed WriteRVS(). (C0700)
'
' 22 Apr 2005 JWD
'   -> Changed ReadRVS(). (C0875)
'   -> Changed WriteRVS(). (C0875)
'
' 12 May 2005 JWD
'   -> Changed ReadRVS(). (C0876)
'   -> Changed WriteRVS(). (C0876)
'---------------------------------------------------------

Public Const c_sRVSEXT = "RVS"



' Modifications:
'
' 20 Jan 2003 GDP
'   -> Write out new bigger A array to RVS file
'
' 23 May 2003 JWD
'  -> Add new adjustment categories AJ6-AJ0. (C0700)
'
' 22 Apr 2005 JWD
'  -> Add write of variable g_nFinanceEvents. (C0875)
'
' 12 May 2005 JWD
'  -> Change to write A() array using nested loop to
'     replace explicit array element subscript references.
'     Order of output is changed with respect to row-major
'     versus column-major; now category (subscript 2) is
'     the major and year (subscript 1) is the minor, see
'     ReadRVS also. (C0876)
'
Public Sub WriteRVS()
    '~^********************************************************************************
    '? Author=       Glyn Phillips
    '? Date_Created= 18-Aug-99
    '~^
    '~ Writes out a file <temp dir>\<data file name>.RVS. This contains information necessary
    '~ to consolidate (pre-tax) values after govt participation has been applied.
    '~^
    '~^********************************************************************************
    
    Dim sFilename As String
    Dim i As Integer
    Dim hFile As Integer
    
    ' Add path and extension to the filename
    sFilename = TempDir$ & g_sDataFileNoExt & "." & c_sRVSEXT
    
    hFile = FreeFile
    
    Open sFilename For Output As hFile
    
    ' Write the currency
    Write #hFile, sCur
    
    ' Write dates and lifes
    Write #hFile, mo, YR, M1, Y1, M3, Y3, LG, LFI
        
    ' 12 May 2005 JWD (C0876) Add next to replace following
    Dim j As Integer
    For j = 1 To UBound(A, 2)
        For i = 1 To LG - 1
            Write #hFile, A(i, j);
        Next i
        Write #hFile, A(LG, j)
    Next j
    ' Was:
    'For i = 1 To LG
    '    ' GDP 20 Jan 2003
    '    ' Write A aray for additional volumes
    '    Write #hFile, A(i, gc_nAOIL), A(i, gc_nAGAS), A(i, gc_nAOV1), A(i, gc_nAOV2), A(i, gc_nAOV3), _
    '                  A(i, gc_nAOV4), A(i, gc_nAOV5), A(i, gc_nAOV6), A(i, gc_nAOV7), A(i, gc_nAOV8), _
    '                  A(i, gc_nAOV9), A(i, gc_nAOV0), A(i, gc_nARES), A(i, gc_nAWIN), A(i, gc_nAOPC), _
    '                  A(i, gc_nAGPC), A(i, gc_nAOP1), A(i, gc_nAOP2)
    '    Write #hFile, A(i, gc_nAOP3), A(i, gc_nAOP4), A(i, gc_nAOP5), A(i, gc_nAOP6), A(i, gc_nAOP7), _
    '                  A(i, gc_nAOP8), A(i, gc_nAOP9), A(i, gc_nAOP0), A(i, gc_nAOX1), A(i, gc_nAOX2), _
    '                  A(i, gc_nAOX3), A(i, gc_nAOX4), A(i, gc_nAOX5), A(i, gc_nAAJ1), A(i, gc_nAAJ2), _
    '                  A(i, gc_nAAJ3), A(i, gc_nAAJ4), A(i, gc_nAAJ5), A(i, gc_nAAJ6), A(i, gc_nAAJ7), _
    '                  A(i, gc_nAAJ8), A(i, gc_nAAJ9), A(i, gc_nAAJ0)
    '
    'Next i
    ' End (C0876)
    
    For i = 1 To LG
        Write #hFile, Inflate(i, 1), Inflate(i, 2)
    Next i
    
    For i = 1 To LG
        Write #hFile, DFL(i)
    Next i
    
    Write #hFile, my3tt
    
    For i = 1 To my3tt
        Write #hFile, my3(i, 1), my3(i, 2), my3(i, 3), my3(i, 4), my3(i, 5), my3(i, 6), my3(i, 7)
    Next i
    
    Write #hFile, gn(1), gn(2), gn(3), gn(4), gn(5), gn(6), gn(7), gn(8), gn(9)
    Write #hFile, PPR
    Write #hFile, DiscMthd
    Write #hFile, PN$(1), PN$(2), PN$(3), PN$(4)
    
    For i = 1 To LG
        Write #hFile, PARTRATE(i), OPEXRATE(i), TOTPMT(i), FINANCE(i)
    Next i
    
    For i = 1 To my3tt
        Write #hFile, GPRATE(i), WINC(i)
    Next i
    
    Write #hFile, g_nFinanceEvents                  ' 22 Apr 2005 JWD (C0875)
    
    Close #hFile
End Sub

' Modifications:
' 20 Jan 2003 GDP
'    -> Read in larger A array
'
' 23 May 2003 JWD
'  -> Add new adjustment categories AJ6-AJ0. (C0700)
'
' 22 Apr 2005 JWD
'  -> Add read of variable g_nFinanceEvents. (C0875)
'
' 12 May 2005 JWD
'  -> Change A() array input loop to use nested loop on
'     dimension 2 rather than explicit element references
'     by using the symbols for each category. Order of
'     input is changed with respect to row-major versus
'     column-major; now category is the major and year
'     is the minor, see WriteRVS also. (C0876)
'
Public Sub ReadRVS()
    '~^********************************************************************************
    '? Author=       Glyn Phillips
    '? Date_Created= 18-Aug-99
    '~^
    '~ Writes out a file <temp dir>\<data file name>.RVS. This contains information necessary
    '~ to consolidate (pre-tax) values after govt participation has been applied.
    '~^
    '~^********************************************************************************
    
    Dim sFilename As String
    Dim i As Integer, j As Integer
    Dim hFile As Integer
    Dim sCurrency As String
    Dim fConversionFactor() As Single
    
    ' Add path and extension to the filename
    sFilename = TempDir$ & g_sDataFileNoExt & "." & c_sRVSEXT
    
    hFile = FreeFile
    
    Open sFilename For Input As hFile
    
    Input #hFile, sCurrency
    
    ' Write dates and lifes
    Input #hFile, mo, YR, M1, Y1, M3, Y3, LG, LFI
        
    ' 12 May 2005 JWD (C0876) Read in using nested loop
    For j = 1 To UBound(A, 2)
        For i = 1 To LG
            Input #hFile, A(i, j)
        Next i
    Next j
    ' was:
    'For i = 1 To LG
    '    ' GDP 20 Jan 2003
    '    ' Read additional volumes into A Array
    '    Input #hFile, A(i, gc_nAOIL), A(i, gc_nAGAS), A(i, gc_nAOV1), A(i, gc_nAOV2), A(i, gc_nAOV3), _
    '                  A(i, gc_nAOV4), A(i, gc_nAOV5), A(i, gc_nAOV6), A(i, gc_nAOV7), A(i, gc_nAOV8), _
    '                  A(i, gc_nAOV9), A(i, gc_nAOV0), A(i, gc_nARES), A(i, gc_nAWIN), A(i, gc_nAOPC), _
    '                  A(i, gc_nAGPC), A(i, gc_nAOP1), A(i, gc_nAOP2)
    '    Input #hFile, A(i, gc_nAOP3), A(i, gc_nAOP4), A(i, gc_nAOP5), A(i, gc_nAOP6), A(i, gc_nAOP7), _
    '                  A(i, gc_nAOP8), A(i, gc_nAOP9), A(i, gc_nAOP0), A(i, gc_nAOX1), A(i, gc_nAOX2), _
    '                  A(i, gc_nAOX3), A(i, gc_nAOX4), A(i, gc_nAOX5), A(i, gc_nAAJ1), A(i, gc_nAAJ2), _
    '                  A(i, gc_nAAJ3), A(i, gc_nAAJ4), A(i, gc_nAAJ5), A(i, gc_nAAJ6), A(i, gc_nAAJ7), _
    '                  A(i, gc_nAAJ8), A(i, gc_nAAJ9), A(i, gc_nAAJ0)
    'Next i
    ' End (C0876)
    
    For i = 1 To LG
        Input #hFile, Inflate(i, 1), Inflate(i, 2)
    Next i
    
    For i = 1 To LG
        Input #hFile, DFL(i)
    Next i
    
    Input #hFile, my3tt
    
    For i = 1 To my3tt
        Input #hFile, my3(i, 1), my3(i, 2), my3(i, 3), my3(i, 4), my3(i, 5), my3(i, 6), my3(i, 7)
    Next i
    
    Input #hFile, gn(1), gn(2), gn(3), gn(4), gn(5), gn(6), gn(7), gn(8), gn(9)
    Input #hFile, PPR
    Input #hFile, DiscMthd
    Input #hFile, PN$(1), PN$(2), PN$(3), PN$(4)
    ' Note that PRTRTE(), OPXRTE(), GRPRTE() and CAPWIN() are new arrays
    ' which possibly have not been referenced yet so need to be ReDimmed
    ReDim PRTRTE(LG), OPXRTE(LG), TOTREPAY(LG), TOTFINANCE(LG)
    For i = 1 To LG
        Input #hFile, PRTRTE(i), OPXRTE(i), TOTREPAY(i), TOTFINANCE(i)
    Next i
    ReDim GPRTE(my3tt), CAPWIN(my3tt)
    For i = 1 To my3tt
        Input #hFile, GPRTE(i), CAPWIN(i)
    Next i
    
    Input #hFile, g_nFinanceEvents              ' 22 Apr 2005 JWD (C0875)
    
    Close #hFile
    
    ' Need to convert into US$
    GetCurrencyConversion sCurrency, "USA", fConversionFactor()
    
    ' Perform currency conversions
    If my3tt <> 0 Then
        For i = 1 To my3tt
            
            my3(i, 5) = my3(i, 5) * fConversionFactor(my3(i, 3) - YR + 1)
            
        Next i
    End If
    
    For i = 1 To LG
       ' GDP 20 Jan 2003
       ' Change loop bounds to constants
        For j = gc_nAMINPRC To gc_nASIZE
            
            A(i, j) = A(i, j) * fConversionFactor(i)
            
        Next j
        ' GDP 13 Nov 2001
        ' Bug fix OMV - consolidated Government Repayment currency was incorrect
        TOTREPAY(i) = TOTREPAY(i) * fConversionFactor(i)
        TOTFINANCE(i) = TOTFINANCE(i) * fConversionFactor(i)
    Next i
    
    
    'Kill TempDir$ & g_sDataFileNoExt & "." & c_sRVSEXT
End Sub
