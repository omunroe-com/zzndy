Attribute VB_Name = "UDTBDA"
'record for BDAScreen (Base Data)
   TYPE BDAType
     cat AS STRING * 3                  'category
     unit AS STRING * 3                 'unit
     dat AS STRING * 3                  'start date code
     mtd AS INTEGER                     'method (1-0)
     parm1 AS STRING * 8                'parameter 1
     parm2 AS SINGLE                    'parameter 2
     parm3 AS SINGLE                    'parameter 3
     parm4 AS SINGLE                    'parameter 4
     parm5 AS SINGLE                    'parameter 5
     parm6 AS SINGLE                    'parameter 6
   END TYPE
