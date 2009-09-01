Attribute VB_Name = "UDTPRR"
'record type for PRRScreen (Government Participation Rates)
  TYPE PRRType
    var AS STRING * 3                   'variable
    rat AS SINGLE                       'rate %
    typ AS STRING * 3                   'field type
    wat AS STRING * 8                   'water depth
    prm AS STRING * 3                   'prm 1
    amt AS SINGLE                       'amount
    recu AS STRING * 3                  'sliding scale
  END TYPE
