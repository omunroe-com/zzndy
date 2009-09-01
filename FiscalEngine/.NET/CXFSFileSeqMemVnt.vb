Option Strict Off
Option Explicit On
Friend Class CXFSFileSeqMemVnt
    Implements IEFSFile
    Implements IEFSFileSeq
    Implements IEFSFileSeqIn
    Implements IEFSFileSeqOut
	'
	' Implements a memory based sequential file
	'
	' This object holds the actual data. This is good for
	' temporary (scratch) files.
	'
	' The actual storage is a variant array. This permits
	' object references to be stored in the file.
	'
	
	
	Private ma_vValues() As Object
	
	Private m_lCursor As Integer ' current read/write element
	Private m_lCount As Integer ' count of elements in file
	Private m_bOpen As Boolean
    Private m_oFile As IEFSFile


    Public Property File() As IEFSFile
        Get

            File = m_oFile

        End Get
        Set(ByVal Value As IEFSFile)

            m_oFile = Value

        End Set
    End Property

    '=========================================================
    '
    ' IEFSFile Interface
    '
    '
    Private ReadOnly Property IEFSFile_FullName() As String Implements IEFSFile.FullName
        Get

            IEFSFile_FullName = m_oFile.FullName

        End Get
    End Property


    Private ReadOnly Property IEFSFile_Name() As String Implements IEFSFile.Name
        Get

            IEFSFile_Name = m_oFile.Name

        End Get
    End Property


    Private ReadOnly Property IEFSFile_Path() As String Implements IEFSFile.Path
        Get

            IEFSFile_Path = m_oFile.path

        End Get
    End Property


    '=========================================================
    '
    ' IEFSFileSeq Interface
    '
    '

    Private ReadOnly Property IEFSFileSeq_Length() As Integer Implements IEFSFileSeq.Length
        Get
            IEFSFileSeq_Length = m_lCount
        End Get
    End Property

    Private WriteOnly Property IEFSFileSeqOut_NextItem() As Object Implements IEFSFileSeqOut.NextItem
        Set(ByVal Value As Object)

            If Not m_bOpen Then
                Err.Raise(52)
            End If

            'UPGRADE_WARNING: Couldn't resolve default property of object DataItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            'UPGRADE_WARNING: Couldn't resolve default property of object ma_vValues(m_lCursor). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            ma_vValues(m_lCursor) = Value

            m_lCount = m_lCursor
            m_lCursor = m_lCursor + 1

            If m_lCursor > UBound(ma_vValues) Then
                'UPGRADE_WARNING: Lower bound of array ma_vValues was changed from LBound(ma_vValues) to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
                ReDim Preserve ma_vValues(UBound(ma_vValues) + (UBound(ma_vValues) - LBound(ma_vValues) + 1))
            End If

        End Set
    End Property

    '
    ' This is the same as the NextItem property
    ' because there is no concept of 'lines' in this
    ' storage scheme.
    '
    Private WriteOnly Property IEFSFileSeqOut_NextItemLineEnd() As Object Implements IEFSFileSeqOut.NextItemLineEnd
        Set(ByVal Value As Object)

            If Not m_bOpen Then
                Err.Raise(52)
            End If

            'UPGRADE_WARNING: Couldn't resolve default property of object DataItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            'UPGRADE_WARNING: Couldn't resolve default property of object ma_vValues(m_lCursor). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            ma_vValues(m_lCursor) = Value

            m_lCount = m_lCursor
            m_lCursor = m_lCursor + 1

            If m_lCursor > UBound(ma_vValues) Then
                'UPGRADE_WARNING: Lower bound of array ma_vValues was changed from LBound(ma_vValues) to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
                ReDim Preserve ma_vValues(UBound(ma_vValues) + (UBound(ma_vValues) - LBound(ma_vValues) + 1) \ 2)
            End If

        End Set
    End Property

    'UPGRADE_NOTE: Class_Initialize was upgraded to Class_Initialize_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    Private Sub Class_Initialize_Renamed()
        m_lCount = 0
        m_lCursor = 1
        m_bOpen = False
    End Sub
    Public Sub New()
        MyBase.New()
        Class_Initialize_Renamed()
    End Sub

    Private Function IEFSFileSeq_OpenForAppend() As IEFSFileSeqOut Implements IEFSFileSeq.OpenForAppend

        If m_lCount = 0 Then
            'UPGRADE_WARNING: Lower bound of array ma_vValues was changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ReDim ma_vValues(4096)
        End If

        m_lCursor = m_lCount + 1

        IEFSFileSeq_OpenForAppend = Me
        m_bOpen = True

    End Function

    Private Function IEFSFileSeq_OpenForInput() As IEFSFileSeqIn Implements IEFSFileSeq.OpenForInput

        m_lCursor = 1

        IEFSFileSeq_OpenForInput = Me
        m_bOpen = True

    End Function

    Private Function IEFSFileSeq_OpenForOutput() As IEFSFileSeqOut Implements IEFSFileSeq.OpenForOutput

        m_lCursor = 1
        m_lCount = 0

        'UPGRADE_WARNING: Lower bound of array ma_vValues was changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim ma_vValues(4096)

        IEFSFileSeq_OpenForOutput = Me
        m_bOpen = True

    End Function


    '=========================================================
    '
    ' IEFSFileSeqIn Interface
    '
    '

    Private Function IEFSFileSeqIn_AtBeginning() As Boolean Implements IEFSFileSeqIn.AtBeginning
        IEFSFileSeqIn_AtBeginning = (m_lCursor = 1)
    End Function

    Private Function IEFSFileSeqIn_AtEnd() As Boolean Implements IEFSFileSeqIn.AtEnd
        IEFSFileSeqIn_AtEnd = (m_lCursor > m_lCount)
    End Function

    Private Function IEFSFileSeqIn_CloseFile() As IEFSFileSeq Implements IEFSFileSeqIn.CloseFile
        m_bOpen = False
        IEFSFileSeqIn_CloseFile = Me
    End Function

    Private Function IEFSFileSeqIn_NextItem() As Object Implements IEFSFileSeqIn.NextItem

        If Not m_bOpen Then
            Err.Raise(52) ' bad file name or number
        End If

        'UPGRADE_WARNING: Couldn't resolve default property of object ma_vValues(m_lCursor). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        IEFSFileSeqIn_NextItem = ma_vValues(m_lCursor)
        m_lCursor = m_lCursor + 1

    End Function


    '=========================================================
    '
    ' IEFSFileSeqOut Interface
    '
    '
    Private Function IEFSFileSeqOut_CloseFile() As IEFSFileSeq Implements IEFSFileSeqOut.CloseFile
        m_bOpen = False
        IEFSFileSeqOut_CloseFile = Me
    End Function
End Class