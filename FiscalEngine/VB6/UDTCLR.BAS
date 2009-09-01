Attribute VB_Name = "UDTCLR"
'record type for CLRScreen (Ceilings)
  TYPE CLRType
    var AS STRING * 3                   'variable
    rat AS SINGLE                       'rate %
    typ AS STRING * 3
    wat AS STRING * 8                   'water depth
    prm AS STRING * 3                   'prm 1
    amt AS SINGLE                       'amount
    recu AS STRING * 3
  END TYPE
