Attribute VB_Name = "UDTEXB"
'record for exchange rate data
   TYPE EXBType
     Cat AS STRING * 3
     unit AS STRING * 3
     dat AS INTEGER
     mtd AS INTEGER
     parm1 AS SINGLE
     parm2 AS SINGLE
     parm3 AS SINGLE
     parm4 AS SINGLE
     parm5 AS SINGLE
     parm6 AS SINGLE
   END TYPE
