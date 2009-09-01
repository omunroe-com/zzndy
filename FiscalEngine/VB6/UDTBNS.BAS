Attribute VB_Name = "UDTBNS"        
        'record type for BNSScreen (Bonuses)
  TYPE BNSType
    Cat AS STRING * 3                   'bonus category
    fld AS STRING * 3                   'field type
    bnsamt AS SINGLE                    'bonus amount
    wat AS SINGLE                       'water depth
    PRM AS STRING * 3                   'production stream
    amt AS SINGLE                       'amount
  END TYPE
