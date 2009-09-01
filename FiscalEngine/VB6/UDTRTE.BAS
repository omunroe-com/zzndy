Attribute VB_Name = "UDTRTE"
'record type for RTEScreen (Variable Rates)
  TYPE RTEType
    var AS STRING * 3                   'variable
    rat AS SINGLE                       'rate %
    typ AS STRING * 3                   'field type
    wat AS STRING * 8                   'water depth
    prm AS STRING * 3                   'prm 1
    amt AS SINGLE                       'amount
    recu AS STRING * 3
  END TYPE
