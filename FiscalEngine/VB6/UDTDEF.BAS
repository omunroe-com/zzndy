Attribute VB_Name = "UDTDEF"
'---------------------------------------------------------
' Modifications:
' 7 May 2001 JWD
'  -> Change definition of deduct columns to permit more
'     than the current 3 characters. This to support
'     numeric entries that might be longer than three
'     characters and would otherwise be truncated. (C0305)
'---------------------------------------------------------
        
        'record type for DEFScreen (Fiscal Definition)
  Type DEFType
    var As String * 3                  'variable
    fld As String * 3                  'field type
    str As String * 3                  'currency and stream
    csh As String * 1                  'cash flow
    inc1 As String * 3                 'income 1
    inc2 As String * 3                 'income 2
    PRC As String * 3                  'price
'    ded1 As String * 3                 'deduction 1
'    ded2 As String * 3                 'deduction 2
'    ded3 As String * 3                 'deduction 3
'    ded4 As String * 3                 'deduction 4
'    ded5 As String * 3                 'deduction 5
    ded1 As String                     'deduction 1
    ded2 As String                     'deduction 2
    ded3 As String                     'deduction 3
    ded4 As String                     'deduction 4
    ded5 As String                     'deduction 5
    crd1 As String * 3                 'credit variable 1
    crd2 As String * 3                 'credit variable 2
    cal1 As String * 3                 'calc variable 1
    cal2 As String * 3                 'calc variable 2
    fnc As String * 1                  'function for calc
    cde As String * 3                  'code
  End Type
