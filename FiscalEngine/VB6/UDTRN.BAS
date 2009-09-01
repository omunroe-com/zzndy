Attribute VB_Name = "UDTRN"
        'record type for RNScreen (RUN FILE)
  TYPE RNType
    command AS STRING * 7               'operation
    path AS STRING * 25                 'path
    filename AS STRING * 8              'file name
    sen1 AS STRING * 3                  'sensitivity 1
    sen2 AS STRING * 8                  'sensitivity 2
    sen3 AS STRING * 8                  'sensitivity 3
    sen4 AS STRING * 8                  'sensitivity 4
  END TYPE

