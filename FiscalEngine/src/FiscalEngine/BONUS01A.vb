Option Strict Off
Option Explicit On
Module BONUS01A
	' Name:        BONUS01A.BAS
	' Function:    Calculate Bonuses
	'---------------------------------------------------------
	' ********************************************************
	' *            COPYRIGHT - PETROCONSULTANTS, INC.        *
	' *                   1991, 1995, 1996                   *
	' *                  ALL RIGHTS RESERVED                 *
	' ********************************************************
	' *  This program file is proprietary information of     *
	' *  Petroconsultants, Incorporated.  Unauthorized use   *
	' *  for any purpose is prohibited.                      *
	' ********************************************************
	'---------------------------------------------------------
	' Modifications:
	' 8 Feb 1996 JWD
	'        Changed CalculateBonus().
	'        Replaced explicit sub declares with BONUS.BI.
	'        Replaced include file CTYIN.BAS with CTYIN1.BG.
	'        Add explicit declaration of default storage class
	'     as Single.
	'
	' 19 Feb 1996 JWD
	'        Changed references to common array RATE() to
	'     PARTRATE().  RATE is reserved function name in VB.
	'        Replace variables True/False with constants
	'     declared in include file TRUFALSE.BC.  Removed
	'     initialization of variables.
	'
	' 20 Nov 1996 JWD
	'        Modified from BONUS.BAS.
	'        Changed CalculateBonus().  (SCO0012)
	'        Removed include & dynamic metacommands.
	'        Add Option Explicit statement.
	'
	' 10 Sep 2002 JWD
	'  -> Change CalculateBonus(). (C0588)
	' 20 Jan 2003 GDP
	'  -> Changed definition of cBNSCds to include new volume streams.
	'  -> Changed CalculateBonus().
	'  -> Changed FilterBNSRecs().
	'
	' 3 Feb 2004 JWD
	'  -> Changed CalculatedBonus(). (C0776)
	'---------------------------------------------------------
	
	'UPGRADE_NOTE: DefSng A-Z statement was removed. Variables were explicitly declared as type Single. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="92AFD3E3-440D-4D49-A8BF-580D74A8C9F2"'
	
	' Added GDP 20 Jan 2003
	' Extra production streams
	Const cBNSCds As String = "OILGASOV1OV2OV3OV4OV5OV6OV7OV8OV9OV0PRDOLCGSCV1CV2CV3CV4CV5CV6CV7CV8CV9CV0CCUM"
	
	'=========================================================
	
	'-----------------------------------------------------------------------
	' Modifications:
	' 8 Feb 1996 JWD
	'        Removed include of SCRA1OUT.BAS.  Data is now in
	'     common.
	'        Removed dimensioning of unreferenced global
	'     (common) arrays.  These are now done on entry to
	'     FiscalDef().
	'        Moved dimension of CUR() and LCur() to immediately
	'     precede initialization of arrays, unreferenced until
	'     then.
	'
	' 20 Feb 1996 JWD
	'        Renamed RPT.DRIVE$ to sRptDir, previous name not
	'     acceptable to VB.
	'        Change CUR$ to sCur, duplicate definition (CUR()).
	'        Replace SWAP statements in sorting subroutine
	'     with loop to produce same result, SWAP statement not
	'     supported in VB.
	'
	' 21 Feb 1996 JWD
	'        Moved set of Program$ to beginning of procedure.
	'        Removed error trap, will let MAINEXEC handle.
	'
	' 20 Nov 1996 JWD
	'        Add loop to ensure that no capital items occur
	'     after end of project.  (SCO0012)
	'        Add variable declarations as required by Option
	'     Explicit statement.
	'
	' 10 Sep 2002 JWD
	'  -> Add variable to save project life value before
	'     application of license term. (C0588)
	'  -> Add trigger of re-do of abandonment on change of
	'     project life when life changes because of license
	'     term. (C0588)
	'
	' 20 Jan 2003 GDP
	'  -> Changed to accommodate new volume streams. Also changed
	'     A array references to use constants defined in modArrayConst.bas
	'
	' 3 Feb 2004 JWD
	'  -> Remove open of report file. This has been replaced
	'     by the report object. (C0776)
	'---------------------------------------------------------
	Sub CalculateBonus()
		'-----------------------------------------------------------------------
		' This program calculates bonuses.
		'-----------------------------------------------------------------------
		Dim bSORTED As Short
		Dim bSWAPPING As Short
		Dim i As Short
		Dim iRow As Short
		Dim iSST As Short
		Dim iZ As Short
		Dim rTmp As Single
		
		'<<<<<<<<<<<<<<<<<<<<<<<
		' 20 Nov 1996 JWD Add next 2 declarations for SCO0012
		Dim intLastYear As Short
		Dim intQ As Short
		
		' 20 Nov 1996 JWD Add following required declarations.
		Dim DUR1 As Single
		Dim DUR2 As Single
		Dim DelayDisc As Single
		Dim DelayProd As Single
		Dim DUR3 As Single
		Dim ZD As Single
		Dim PrdTot As Single
		Dim PrdBon As Single
		Dim totrev As Single
		Dim totprod As Single
		Dim cumamt As Single
		Dim foundit As String
		Dim DTES As Single
		Dim OUTFIL As String
		Dim j As Short
		'>>>>>>>>>>>>>>>>>>>>>>>
		
		' 10 Sep 2002 JWD (C0588)
		Dim z_natural_project_life As Short
		
		'---------------------------------------------------------
		Program = "BONUS"
		
		Dim STM(LG) As Object
		ReDim BONS(LG)
		ReDim OthBon(LG)
		
		' Added GDP 27/08/99
		
		If g_bPTCons Then
			BNT = 0 ' Set the number of bonus items in the country file to 0
			GM(2, PPR) = 0 ' Set project license terms to 0
			GM(3, PPR) = 0 ' Set discovery license terms to 0
			GM(4, PPR) = 0 ' Set production license terms to 0
		End If
		
		
		'convert null values to zeros
		For i = 1 To BNT
			If BN(i, 4) = "-3.4E+35" Then 'bonus amount
				BN(i, 4) = "0"
			End If
			If BN(i, 5) = "-3.4E+35" Then 'water depth
				BN(i, 5) = "0"
			End If
			If BN(i, 6) = "-3.4E+35" Then 'break point
				BN(i, 6) = "0"
			End If
		Next i
		
10000: 
		
		' 10 Sep 2002 JWD (C0588)
		' Save the project life before checking license terms
		z_natural_project_life = LG
		
		
		'MO             'project start month (from general parameters)
		'YR             'project start year (from general parameters)
		'M1             'production start month (from general parameters)
		'Y1             'production start year (from general parameters)
		'M2             'discovery start month (from general parameters)
		'Y2             'discovery start year (from general parameters)
		
		' CHECK LICENSE TERMS
		
		'apply project license term
		If Int(GM(2, PPR)) > 0 Then
			DUR1 = Int(GM(2, PPR))
			If DUR1 < LGI Then
				LGI = DUR1
				LG = Int(LGI)
				If LG < LGI Then
					LG = LG + 1
				End If
				LFI = LGI - ((((M1 - mo) / 12) + Y1) - YR)
			End If
		End If
		'IF INT(GM(2, PPR)) > 0 THEN
		'  DUR1 = INT(GM(2, PPR))
		'  IF DUR1 < LG THEN
		'    LG = DUR1
		'    LGI = LG
		'    LFI = LGI - ((((M1 - 1) / 12) + Y1) - YR)
		'  END IF
		'END IF
		
10010: 'apply discovery license term
		If Int(GM(3, PPR)) > 0 Then
			DelayDisc = ((M2 - mo) / 12) + Y2 - YR
			DUR2 = Int(GM(3, PPR)) + DelayDisc
			If DUR2 < LGI Then
				LGI = DUR2
				LG = Int(LGI)
				If LG < LGI Then
					LG = LG + 1
				End If
				LFI = LGI - ((((M1 - mo) / 12) + Y1) - YR)
			End If
		End If
		'IF INT(GM(3, PPR)) > 0 THEN
		'  DUR2 = INT(GM(3, PPR)) + Y2 - YR
		'  IF DUR2 < LG THEN
		'    LG = DUR2
		'    LGI = LG
		'    LFI = LGI - ((((M1 - MO) / 12) + Y1) - YR)
		'  END IF
		'END IF
		
10020: 'apply production license term
		If Int(GM(4, PPR)) > 0 Then
			DelayProd = ((M3 - mo) / 12) + Y3 - YR
			DUR3 = Int(GM(4, PPR)) + Y1 - YR
			If DUR3 < LGI Then
				LGI = DUR3
				LG = Int(LGI)
				If LG < LGI Then
					LG = LG + 1
				End If
				LFI = LGI - ((((M1 - mo) / 12) + Y1) - YR)
			End If
		End If
		
		'IF INT(GM(4, PPR)) > 0 THEN
		'  DUR3 = INT(GM(4, PPR)) + Y1 - YR
		'  IF DUR3 < LG THEN
		'    LG = DUR3
		'    LGI = LG
		'    LFI = LGI - ((((M1 - MO) / 12) + Y1) - YR)
		'  END IF
		'END IF
10030: 
		
		RNU = RNU + 1
		ZD = Int(Y1 - YR) + 1
10050: 
		
		'filter out the lines with water depths that are not applicable
		If BNT > 0 Then
			FilterBNSRecs()
		End If
		
		If BNT = 0 Then GoTo 10540 'no bonuses - exit!
		
		' put in signature bonuses, if any
		SIG = 0
		For i = 1 To BNT
			If Left(BN(i, 1), 3) <> "SIG" Then GoTo nextx
			If Left(BN(i, 2), 3) = "ALL" Then GoTo va1
			If PPR = 1 And Left(BN(i, 2), 3) <> "OIL" Then GoTo nextx
			If PPR = 2 And Left(BN(i, 2), 3) <> "GAS" Then GoTo nextx
			
va1: If Left(BN(i, 4), 3) = "" Then GoTo va2
			If Left(BN(i, 4), 2) = "ON" And gn(1) = 0 Then GoTo va2
			If gn(1) >= Val(BN(i, 4)) Then GoTo va2
			GoTo nextx
			
va2: SIG = SIG + Val(BN(i, 3))
			
nextx: 
		Next i
10060: 
		
		
		' put in discovery bonuses, if any
		DIS = 0
		For i = 1 To BNT
			If Left(BN(i, 1), 3) <> "DIS" Then GoTo nextx1
			If Left(BN(i, 2), 3) = "ALL" Then GoTo va11
			If PPR = 1 And Left(BN(i, 2), 3) <> "OIL" Then GoTo nextx1
			If PPR = 2 And Left(BN(i, 2), 3) <> "GAS" Then GoTo nextx1
va11: If Left(BN(i, 4), 3) = "" Then GoTo va21
			If Left(BN(i, 4), 2) = "ON" And gn(1) = 0 Then GoTo va21
			If gn(1) >= Val(BN(i, 4)) Then GoTo va21
			GoTo nextx1
va21: DIS = DIS + Val(BN(i, 3))
nextx1: 
		Next i
10070: 
		'--------------------------------------------------------------------
		'This is the start of the big Production bonuses loop
		PrdTot = 0
		For i = 1 To BNT
			ReDim STM(LG)
			
			If Left(BN(i, 1), 3) <> "PRD" Then GoTo nextx2
			If Left(BN(i, 2), 3) = "ALL" Then GoTo va12
			If PPR = 1 And Left(BN(i, 2), 3) <> "OIL" Then GoTo nextx2
			If PPR = 2 And Left(BN(i, 2), 3) <> "GAS" Then GoTo nextx2
va12: If Left(BN(i, 4), 3) = "" Then GoTo va22
			If Left(BN(i, 4), 2) = "ON" And gn(1) = 0 Then GoTo va22
			If gn(1) >= Val(BN(i, 4)) Then GoTo va22
			GoTo nextx2
va22: PrdBon = Val(BN(i, 3))
10080: 
			' GDP 20 Jan 2003
			' Changed refences to A array in this case statement to
			' use the defined constants
			
			Select Case BN(i, 5)
				Case "OIL"
					For iZ = 1 To LG
						'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						STM(iZ) = A(iZ, gc_nAOIL) / 0.365
					Next iZ
				Case "GAS"
					For iZ = 1 To LG
						'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						STM(iZ) = A(iZ, gc_nAGAS) / 0.365
					Next iZ
				Case "OV1"
					For iZ = 1 To LG
						'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						STM(iZ) = A(iZ, gc_nAOV1) / 0.365
					Next iZ
				Case "OV2"
					For iZ = 1 To LG
						'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						STM(iZ) = A(iZ, gc_nAOV2) / 0.365
					Next iZ
					' Added GDP 20 Jan 2003
					' Additional Production Streams
				Case "OV3"
					For iZ = 1 To LG
						'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						STM(iZ) = A(iZ, gc_nAOV3) / 0.365
					Next iZ
				Case "OV4"
					For iZ = 1 To LG
						'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						STM(iZ) = A(iZ, gc_nAOV4) / 0.365
					Next iZ
				Case "OV5"
					For iZ = 1 To LG
						'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						STM(iZ) = A(iZ, gc_nAOV5) / 0.365
					Next iZ
				Case "OV6"
					For iZ = 1 To LG
						'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						STM(iZ) = A(iZ, gc_nAOV6) / 0.365
					Next iZ
				Case "OV7"
					For iZ = 1 To LG
						'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						STM(iZ) = A(iZ, gc_nAOV7) / 0.365
					Next iZ
				Case "OV8"
					For iZ = 1 To LG
						'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						STM(iZ) = A(iZ, gc_nAOV8) / 0.365
					Next iZ
				Case "OV9"
					For iZ = 1 To LG
						'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						STM(iZ) = A(iZ, gc_nAOV9) / 0.365
					Next iZ
				Case "OV0"
					For iZ = 1 To LG
						'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						STM(iZ) = A(iZ, gc_nAOV0) / 0.365
					Next iZ
				Case "PRD"
					For iZ = 1 To LG
						' GDP 20 Jan 2003
						' Replace previous line with call to function to total revenues
						' for time period
						'totrev = (A(iZ, 1) * A(iZ, 7)) + (A(iZ, 2) * A(iZ, 8)) + (A(iZ, 3) * A(iZ, 9)) + (A(iZ, 4) * A(iZ, 10))
						totrev = ATotalRevenues(iZ)
						' GDP 20 Jan 2003
						' Changed A array references to use defined constants
						If PPR = 1 And A(iZ, gc_nAOPC) <> 0 Then
							'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
							STM(iZ) = (totrev / A(iZ, gc_nAOPC)) / 0.365
						ElseIf PPR = 2 And A(iZ, gc_nAGPC) Then 
							'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
							STM(iZ) = (totrev / A(iZ, gc_nAGPC)) / 0.365
						End If
					Next iZ
					
				Case "PRE"
					' 11 Jul 2008 JWD Add volume equivalent production average daily rate
					For iZ = 1 To LG
						'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						STM(iZ) = EquivalencyVolumeProduction(iZ, True, True) / 0.365
					Next iZ
					
					' GDP 20 Jan 2003
					' Added codes for new volumes
				Case "OLC", "GSC", "V1C", "V2C", "V3C", "V4C", "V5C", "V6C", "V7C", "V8C", "V9C", "V0C", "CUM", "CUV"
                    ''REDIM stm(LG)
                    Select Case BN(i, 5)
                        Case "OLC"
                            For iZ = 1 To LG
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                STM(iZ) = STM(iZ) + A(iZ, gc_nAOIL)
                            Next iZ
                        Case "GSC"
                            For iZ = 1 To LG
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                STM(iZ) = STM(iZ) + A(iZ, gc_nAGAS)
                            Next iZ
                        Case "V1C"
                            For iZ = 1 To LG
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                STM(iZ) = STM(iZ) + A(iZ, gc_nAOV1)
                            Next iZ
                        Case "V2C"
                            For iZ = 1 To LG
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                STM(iZ) = STM(iZ) + A(iZ, gc_nAOV2)
                            Next iZ
                            ' Added GDP 20 Jan 2003
                            ' extra volume streams
                        Case "V3C"
                            For iZ = 1 To LG
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                STM(iZ) = STM(iZ) + A(iZ, gc_nAOV3)
                            Next iZ
                        Case "V4C"
                            For iZ = 1 To LG
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                STM(iZ) = STM(iZ) + A(iZ, gc_nAOV4)
                            Next iZ
                        Case "V5C"
                            For iZ = 1 To LG
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                STM(iZ) = STM(iZ) + A(iZ, gc_nAOV5)
                            Next iZ
                        Case "V6C"
                            For iZ = 1 To LG
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                STM(iZ) = STM(iZ) + A(iZ, gc_nAOV6)
                            Next iZ
                        Case "V7C"
                            For iZ = 1 To LG
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                STM(iZ) = STM(iZ) + A(iZ, gc_nAOV7)
                            Next iZ
                        Case "V8C"
                            For iZ = 1 To LG
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                STM(iZ) = STM(iZ) + A(iZ, gc_nAOV8)
                            Next iZ
                        Case "V9C"
                            For iZ = 1 To LG
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                STM(iZ) = STM(iZ) + A(iZ, gc_nAOV9)
                            Next iZ
                        Case "V0C"
                            For iZ = 1 To LG
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                STM(iZ) = STM(iZ) + A(iZ, gc_nAOV0)
                            Next iZ
                        Case "CUM"
                            totrev = 0
                            totprod = 0
                            For iZ = 1 To LG
                                ' GDP 20 Jan 2003
                                ' Replace previous line with call to function to total revenues
                                ' for time period
                                '  totrev = (A(iZ, 1) * A(iZ, 7)) + (A(iZ, 2) * A(iZ, 8)) + (A(iZ, 3) * A(iZ, 9)) + (A(iZ, 4) * A(iZ, 10))
                                totrev = ATotalRevenues(iZ)

                                ' GDP 20 Jan 2003
                                ' Changed A array references to use defined constants

                                If PPR = 1 And A(iZ, gc_nAOPC) <> 0 Then
                                    totprod = totrev / A(iZ, gc_nAOPC)
                                ElseIf PPR = 2 And A(iZ, gc_nAGPC) <> 0 Then
                                    totprod = totrev / A(iZ, gc_nAGPC)
                                End If
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                STM(iZ) = STM(iZ) + totprod
                            Next iZ

                        Case "CUV"
                            ' 11 Jul 2008 JWD Add cumulative volume equivalent production
                            For iZ = 1 To LG
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                STM(iZ) = STM(iZ) + EquivalencyVolumeProduction(iZ, True, True)
                            Next iZ

                    End Select

                    'stm() now contains the annual values
                    '  now, cum the amounts

                    cumamt = 0
                    For iZ = 1 To LG
                        'UPGRADE_WARNING: Couldn't resolve default property of object STM(). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        STM(iZ) = STM(iZ) + cumamt
                        'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        cumamt = STM(iZ)
                    Next iZ
            End Select

            iZ = Y1 - YR
            foundit = "N"
10100:
incr:
            iZ = iZ + 1
            If iZ > LG Then GoTo nextx2
            'UPGRADE_WARNING: Couldn't resolve default property of object STM(iZ). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            If STM(iZ) >= Val(BN(i, 6)) Then
                BONS(iZ) = BONS(iZ) + PrdBon
                foundit = "Y"
            End If
            If foundit = "N" Then GoTo incr
nextx2:
        Next i

10540:  'Determining the Other Bonus
        For i = 1 To MY3T
            If my3(i, 1) = 1 Then
                iSST = my3(i, 3) - YR + 1
                If iSST < 31 Then
                    OthBon(iSST) = OthBon(iSST) + my3(i, 5)
                End If
            End If
        Next i

        'CONVERT SIG,DIS AND BONS() TO MY3()
        my3tt = MY3T
        If SIG = 0 Then GoTo 20010
        my3tt = my3tt + 1
        my3(my3tt, 1) = 1 : my3(my3tt, 2) = mo : my3(my3tt, 3) = YR
        ' GDP 20 Jan 2003
        ' Changed A array ref. to use constant
        my3(my3tt, 4) = 0 : my3(my3tt, 5) = SIG : my3(my3tt, 6) = A(1, gc_nAWIN)
20010:  If DIS = 0 Then GoTo 20020
        my3tt = my3tt + 1
        my3(my3tt, 1) = 1 : my3(my3tt, 2) = M2 : my3(my3tt, 3) = Y2
        DTES = my3(my3tt, 3) - YR + 1
        ' GDP 20 Jan 2003
        ' Changed A array ref. to use constant
        my3(my3tt, 4) = 0 : my3(my3tt, 5) = DIS : my3(my3tt, 6) = A(DTES, gc_nAWIN)
20020:  For i = 1 To LG
            If BONS(i) = 0 Then GoTo 20030
            my3tt = my3tt + 1
            my3(my3tt, 1) = 1
            my3(my3tt, 2) = 7
            my3(my3tt, 3) = YR + i - 1
            DTES = my3(my3tt, 3) - YR + 1
            my3(my3tt, 4) = 0
            my3(my3tt, 5) = BONS(i)
            ' GDP 20 Jan 2003
            ' Changed A array ref. to use constant
            my3(my3tt, 6) = A(DTES, gc_nAWIN)
            If my3(my3tt, 3) = Y1 Then my3(my3tt, 2) = M1
20030:  Next i

        'UPGRADE_ISSUE: GoSub statement is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C5A1A479-AB8B-4D40-AAF4-DB19A2E5E77F"'
        'GoSub 5000 'This gosub is to Sort MY3() by Month and Year

        ' SORTING ROUTINE
        '******************
        'sort the MY3() (CAPEX) by date
        For i = 2 To 3
            bSORTED = False
            While Not bSORTED
                bSWAPPING = False
                iRow = 1
                While iRow < my3tt ' Number of records
                    If my3(iRow, i) > my3(iRow + 1, i) Then
                        For j = 1 To 7
                            rTmp = my3(iRow + 1, j)
                            my3(iRow + 1, j) = my3(iRow, j)
                            my3(iRow, j) = rTmp
                        Next j
                        '~~~~                SWAP MY3(iRow, 1), MY3(iRow + 1, 1)
                        '~~~~                SWAP MY3(iRow, 2), MY3(iRow + 1, 2)
                        '~~~~                SWAP MY3(iRow, 3), MY3(iRow + 1, 3)
                        '~~~~                SWAP MY3(iRow, 4), MY3(iRow + 1, 4)
                        '~~~~                SWAP MY3(iRow, 5), MY3(iRow + 1, 5)
                        '~~~~                SWAP MY3(iRow, 6), MY3(iRow + 1, 6)
                        '~~~~                SWAP MY3(iRow, 7), MY3(iRow + 1, 7)
                        bSWAPPING = True
                    End If
                    iRow = iRow + 1
                End While
                If Not bSWAPPING Then
                    bSORTED = True
                End If
            End While
        Next i

        '<<<<<<<<<<<<<<<<<<<<<<<<<
        ' 20 Nov 1996 JWD Add following
        ' Now examine MY3 to be sure that no capital is
        ' being spent after the end of the project. MY3()
        ' is already sorted by date. Find the last record
        ' that is valid and reset MY3TT to point to that
        ' record.

        intLastYear = YR + LG - 1
        For intQ = 1 To my3tt
            If my3(intQ, 3) > intLastYear Then
                my3tt = intQ - 1
                Exit For
            End If
        Next intQ
        '>>>>>>>>>>>>>>>>>>>>>>>>

        ' 10 Sep 2002 JWD (C0588)
        ' Trigger reschedule of abandonment provisions on
        ' change of project life due to application of license
        ' terms
        If LG <> z_natural_project_life Then
            TriggerAbandonmentProvisionsModelEvent(OnChangeOfProjectLifeForLicenseTerm)
        End If

        '  APPLY DEFAULT PARTICIPATION RATES
        ReDim GPBASE(my3tt)
        ReDim GPRATE(my3tt)
        ReDim PARTRATE(LG)
        ReDim OPEXRATE(LG)
        ReDim WIN(LG)
        ReDim WINC(my3tt)
        ReDim GPDPCR(my3tt)

        For i = 1 To my3tt
            GPBASE(i) = 1
            GPRATE(i) = 1
            GPDPCR(i) = 1
        Next i

        For i = 1 To LG
            PARTRATE(i) = 0
            OPEXRATE(i) = 0
        Next i

        ' SET DEFAULT CURRENCY TO USA DOLLARS
        ReDim CUR(LG)
        ReDim LCur(LG)
        sCur = "USA"
        For i = 1 To LG
            CUR(i) = 1
            LCur(i) = 1
        Next i

        If Len(RN) > 6 Then
            RNP = Mid(RN, 1, 6) & Right(Str(RNU), Len(Str(RNU)) - 1)
        Else
            RNP = RN & Right(Str(RNU), Len(Str(RNU)) - 1)
        End If

        ' OPEN FILE THAT WILL HOLD ALL OUTPUT REPORTS FOR THIS RUN
        OUTFIL = sRptDir & RNP & ".PRN"
        If Left(RF(4), 3) = "FIL" Or Left(RF(4), 3) = "SCR" Or Left(RF(4), 3) = "PTR" Or Left(RF(4), 3) = "PTD" Then
            If Left(RF(5), 3) = "ALL" Or Left(RF(5), 3) = "VAR" Or Left(RF(5), 3) = "SUM" Then
                ''            Open OUTFIL$ For Output As #5
            End If
        End If

        Erase BN

        ''PushStats "BONUS", dStart
        Exit Sub

        '--------------------------------------------------------------------
5000:   '******************
        ' SORTING ROUTINE
        '******************
        'sort the MY3() (CAPEX) by date
        For i = 2 To 3
            bSORTED = False
            While Not bSORTED
                bSWAPPING = False
                iRow = 1
                While iRow < my3tt ' Number of records
                    If my3(iRow, i) > my3(iRow + 1, i) Then
                        For j = 1 To 7
                            rTmp = my3(iRow + 1, j)
                            my3(iRow + 1, j) = my3(iRow, j)
                            my3(iRow, j) = rTmp
                        Next j
                        '~~~~                SWAP MY3(iRow, 1), MY3(iRow + 1, 1)
                        '~~~~                SWAP MY3(iRow, 2), MY3(iRow + 1, 2)
                        '~~~~                SWAP MY3(iRow, 3), MY3(iRow + 1, 3)
                        '~~~~                SWAP MY3(iRow, 4), MY3(iRow + 1, 4)
                        '~~~~                SWAP MY3(iRow, 5), MY3(iRow + 1, 5)
                        '~~~~                SWAP MY3(iRow, 6), MY3(iRow + 1, 6)
                        '~~~~                SWAP MY3(iRow, 7), MY3(iRow + 1, 7)
                        bSWAPPING = True
                    End If
                    iRow = iRow + 1
                End While
                If Not bSWAPPING Then
                    bSORTED = True
                End If
            End While
        Next i
        ' *********************
        ' END SORTING ROUTINE
        ' *********************
        'UPGRADE_WARNING: Return has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        Return

    End Sub

    '---------------------------------------------------------
    ' Modifications:
    ' 20 Nov 1996 JWD
    '        Add variable declarations as required by Option
    '     Explicit statement.
    '---------------------------------------------------------
    Sub FilterBNSRecs()
        '---------------------------------------------------------
        ' This sub examines the BN$() [Bonus screen] and removes
        ' the records that do not apply to the water depth of the
        ' current case.
        ' For example: the data file says 75 meters water depth.
        ' The country file has two SIG lines on the bonus screen.
        ' One is for depth >= 1 meter and the second is for depth
        ' >= 100 meters.  The only record we want is the >= 1
        ' meter.
        ' SIG (signature) and DIS (discovery) are handled the same
        ' way (only one line is used for each).
        ' PRD is different. We need all of the lines that have the
        ' correct water depth range.
        '---------------------------------------------------------
        Dim goodsig As Short
        Dim gooddis As Short

        '<<<<<<<<<<<<<<<<<<<<<<<
        ' 20 Nov 1996 JWD Add following required declarations.
        Dim i As Short
        Dim SIGCount As Short
        Dim DISCount As Short
        Dim PRDCount As Short
        Dim SIGDepth As Single
        Dim DISDepth As Single
        Dim depth As Single
        Dim q As Short
        Dim wdep As Single
        Dim ptr As Short
        Dim w As Short
        '>>>>>>>>>>>>>>>>>>>>>>>
        '---------------------------------------------------------

        Dim SIGLines(BNT, 6) As Object
        Dim DISLines(BNT, 6) As Object
        Dim PRDLines(BNT, 6) As Object
        ' GDP 20 Jan 2003
        ' Changed PRDDepth size from 10 to 26
        Dim PRDDepth(26) As Object 'depths for each of 26 PRM categories for PRD bonus type
        'how many of this type
        SIGCount = 0
        DISCount = 0
        PRDCount = 0
        'correct depth range of each type
        SIGDepth = -999
        DISDepth = -999
        For i = 1 To UBound(PRDDepth)
            'UPGRADE_WARNING: Couldn't resolve default property of object PRDDepth(i). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            PRDDepth(i) = -999
        Next i

        depth = gn(1) 'data file depth

        'first find the correct depth range for each type
        For q = 1 To BNT
            wdep = Val(BN(q, 4)) 'water depth of this BONUS record
            If wdep <= depth Then
                Select Case BN(q, 1)
                    Case "SIG"
                        If wdep > SIGDepth Then
                            SIGDepth = wdep
                        End If
                    Case "DIS"
                        If wdep > DISDepth Then
                            DISDepth = wdep
                        End If
                    Case "PRD"
                        SearchCodeString(cBNSCds, BN(q, 5), 3, ptr)
                        ''ptr% = WhichParm%(BN$(q%, 5))
                        'UPGRADE_WARNING: Couldn't resolve default property of object PRDDepth(ptr). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        If wdep > PRDDepth(ptr) Then
                            'UPGRADE_WARNING: Couldn't resolve default property of object PRDDepth(ptr). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            PRDDepth(ptr) = wdep
                        End If
                End Select
            End If
        Next q
        'now we have the applicable depth for each type (if there
        '  weren't any records for a type, then the xxxdepth
        '  variable will be 0
        'NOW put the selected records in to SIGLines$(), DISLines$(),
        '  and PRDLines$()
        For q = 1 To BNT
            wdep = Val(BN(q, 4)) 'water depth of this BONUS record
            Select Case BN(q, 1)
                Case "SIG"
                    If wdep = SIGDepth Then
                        goodsig = False
                        If PPR = 1 Then
                            If BN(q, 2) = "ALL" Or BN(q, 2) = "OIL" Then
                                goodsig = True
                            End If
                        ElseIf PPR = 2 Then
                            If BN(q, 2) = "ALL" Or BN(q, 2) = "GAS" Then
                                goodsig = True
                            End If
                        End If
                        If goodsig = True Then
                            If SIGCount = 0 Then
                                SIGCount = 1 'only 1 signature bonus is valid
                            End If
                            For w = 1 To 6
                                'UPGRADE_WARNING: Couldn't resolve default property of object SIGLines$(SIGCount, w). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                SIGLines(SIGCount, w) = BN(q, w)
                            Next w
                        End If
                    End If
                Case "DIS"
                    If wdep = DISDepth Then
                        gooddis = False
                        If PPR = 1 Then
                            If BN(q, 2) = "ALL" Or BN(q, 2) = "OIL" Then
                                gooddis = True
                            End If
                        ElseIf PPR = 2 Then
                            If BN(q, 2) = "ALL" Or BN(q, 2) = "GAS" Then
                                gooddis = True
                            End If
                        End If
                        If gooddis = True Then
                            If DISCount = 0 Then
                                DISCount = 1 'only 1 signature bonus is valid
                            End If
                            For w = 1 To 6
                                'UPGRADE_WARNING: Couldn't resolve default property of object DISLines$(DISCount, w). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                DISLines(DISCount, w) = BN(q, w)
                            Next w
                        End If
                    End If
                Case "PRD"
                    SearchCodeString(cBNSCds, BN(q, 5), 3, ptr)
                    ''        ptr% = WhichParm%(BN$(q%, 5))
                    'UPGRADE_WARNING: Couldn't resolve default property of object PRDDepth(ptr). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    If wdep = PRDDepth(ptr) Then
                        PRDCount = PRDCount + 1
                        For w = 1 To 6
                            'UPGRADE_WARNING: Couldn't resolve default property of object PRDLines$(PRDCount, w). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            PRDLines(PRDCount, w) = BN(q, w)
                        Next w
                    End If
            End Select
        Next q
        'now there are three arrays with all of the needed records
        'Consolidate them into a new BN$() and update BNT
        BNT = SIGCount + DISCount + PRDCount
        ReDim BN(BNT, 6)

        ptr = 0 'record in BN$() we are writing into

        'signature line (if any)
        For q = 1 To SIGCount
            ptr = ptr + 1
            For w = 1 To 6
                'UPGRADE_WARNING: Couldn't resolve default property of object SIGLines$(q, w). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                BN(ptr, w) = SIGLines(q, w)
            Next w
        Next q
        'discovery line (if any)
        For q = 1 To DISCount
            ptr = ptr + 1
            For w = 1 To 6
                'UPGRADE_WARNING: Couldn't resolve default property of object DISLines$(q, w). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                BN(ptr, w) = DISLines(q, w)
            Next w
        Next q
        'production lines (if any)
        For q = 1 To PRDCount
            ptr = ptr + 1
            For w = 1 To 6
                'UPGRADE_WARNING: Couldn't resolve default property of object PRDLines$(q, w). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                BN(ptr, w) = PRDLines(q, w)
            Next w
        Next q

        'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        System.Array.Clear(SIGLines, 0, SIGLines.Length)
        'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        System.Array.Clear(DISLines, 0, DISLines.Length)
        'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        System.Array.Clear(PRDLines, 0, PRDLines.Length)
        'UPGRADE_NOTE: Erase was upgraded to System.Array.Clear. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        System.Array.Clear(PRDDepth, 0, PRDDepth.Length)

    End Sub
End Module