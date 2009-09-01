Option Strict Off
Option Explicit On
Module MCPXConvert2
	' Name:        MCPXConvert2.bas
	' Function:    Convert Capex from annual to Giant internal
	' Date:        21 June 2005 JWD
	'---------------------------------------------------------
	' Modified from MCPXConvert.bas.
	' Changed ConvertCapexForecasts parameter list.
	'---------------------------------------------------------
	' Modifications:
	' 20 Jun 2005 JWD
	'  -> Changed ConvertCapexForecasts(). (C0880)
	'---------------------------------------------------------
	
	'
	' Modifications:
	' 20 Jun 2005 JWD
	'  -> Change type of CapexAmts(), CapexWI(), CapexRP(),
	'     and CapexTan() to Single precision to support the
	'     external preparation of data (text representation
	'     state). (C0880)
	'
	' Transform the input time series of capital expenditure
	' forecasts into the standard Giant internal form (MY3()).
	' LMY3() is the actual Giant form capital expenditures
	' array.
	' LMY3T is the count of capital expenditures in the LMY3()
	' array.
	'
    Public Sub ConvertCapexForecasts(ByRef CapexAmts(,) As Single, ByRef CapexWI(,) As Single, ByRef CapexRP(,) As Single, ByRef CapexTan() As Single, ByVal AllocationMethod As Short, ByVal ProjectStartYear As Short, ByRef LMY3(,) As Single, ByRef LMY3T As Short)

        ' Following are the values that AllocationMethod may assume:
        Const am_even As Short = 1 ' Expenditure spread over 12 months
        Const am_beg As Short = 2 ' Expenditure in Jan
        Const am_mid As Short = 3 ' Expenditure in Jul
        Const am_end As Short = 4 ' Expenditure in Dec


        Const ncc As Short = 27 ' number of capital categories defined in engine


        Dim xl As Short ' Expenditures life (years)

        Dim byr As Short ' year before project start year
        Dim mcx As Short ' month of capital expenditure
        Dim tcx(,) As Single ' temporary capex array in my3 form

        Dim np As Short ' number of expenditures (rows) in tcx()
        Dim CP As Short ' category (row) pointer
        Dim pp As Short ' period (column) pointer

        Dim i As Short
        Dim j As Short

        ' Determine the life of expenditures for the project
        ' Lower bound of the time dimension of the array is
        ' assumed to be 1.
        xl = UBound(CapexAmts, 2)

        ' Determine year before project start year.
        byr = ProjectStartYear - 1

        If AllocationMethod = am_even Then
            ' Allocate the maximum number of expenditures entries there could be
            'UPGRADE_WARNING: Lower bound of array tcx was changed from 1,1 to 0,0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ReDim tcx((xl * ncc * 12), 7)

            np = 0
            For pp = 1 To xl
                For CP = 1 To ncc
                    If CapexAmts(CP, pp) <> 0 Then
                        For i = 1 To 12
                            np = np + 1
                            tcx(np, 1) = CP
                            tcx(np, 2) = i
                            tcx(np, 3) = byr + pp
                            tcx(np, 4) = CapexTan(CP)
                            tcx(np, 5) = CapexAmts(CP, pp) / 12
                            tcx(np, 6) = CapexWI(CP, pp)
                            tcx(np, 7) = CapexRP(CP, pp)
                        Next i
                    End If
                Next CP
            Next pp

        Else
            ' Allocate the maximum number of expenditures entries there could be
            'UPGRADE_WARNING: Lower bound of array tcx was changed from 1,1 to 0,0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ReDim tcx((xl * ncc), 7)

            Select Case AllocationMethod
                Case am_beg
                    mcx = 1
                Case am_mid
                    mcx = 7
                Case am_end
                    mcx = 12
                Case Else
                    mcx = 7 ' default to mid-year if not a valid code value
            End Select

            np = 0
            For pp = 1 To xl
                For CP = 1 To ncc
                    If CapexAmts(CP, pp) <> 0 Then
                        np = np + 1
                        tcx(np, 1) = CP ' capital category number
                        tcx(np, 2) = mcx ' month of expenditure
                        tcx(np, 3) = byr + pp ' year of expenditure
                        tcx(np, 4) = CapexTan(CP) ' tangible percentage
                        ' 27 May 2005 JWD (C0880) Remove the text conversion state from assignment
                        tcx(np, 5) = CapexAmts(CP, pp) ' expenditure gross amount
                        ' was:
                        'tcx(np, 5) = CapexAmts(CP, pp)   ' expenditure gross amount
                        '' 7 Oct 2003 JWD (C0757) Change next 1
                        '' Add text representation state to assignment
                        'tcx(np, 5) = Val(str$(CSng(CapexAmts(CP, pp))))   ' expenditure gross amount
                        ' End (C0880)
                        tcx(np, 6) = CapexWI(CP, pp) ' working interest override
                        tcx(np, 7) = CapexRP(CP, pp) ' repayment percentage
                    End If
                Next CP
            Next pp

        End If

        ' Now copy to the real capital expenditure array
        If np > 0 Then
            'ReDim LMY3(1 To np + 20, 1 To 7)
            ' 8 Oct 2003 JWD (C0762) Change next 1, increase dim 1 upper bound
            'UPGRADE_WARNING: Lower bound of array LMY3 was changed from 1,1 to 0,0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ReDim LMY3(np + 120, 7)

            For j = 1 To 7
                For i = 1 To np
                    LMY3(i, j) = tcx(i, j)
                Next i
            Next j
        Else
            'ReDim LMY3(0 To 20, 1 To 7)
            ' 8 Oct 2003 JWD (C0762) Change next 1, increase dim 1 upper bound
            'UPGRADE_WARNING: Lower bound of array LMY3 was changed from 0,1 to 0,0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ReDim LMY3(120, 7)
        End If

        LMY3T = np

    End Sub
End Module