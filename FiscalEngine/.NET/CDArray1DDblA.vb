Option Strict Off
Option Explicit On
Friend Class CDArray1DDblA
    Implements IDStore
    Implements IDStoreSeq




    Private ma_dValues() As Double

    Private m_lIndex As Integer

    Public Sub Initialize(ByVal LowerBound As Integer, ByVal UpperBound As Integer)

        'UPGRADE_WARNING: Lower bound of array ma_dValues was changed from LowerBound to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim ma_dValues(UpperBound)
        ResetCursor()

    End Sub

    Public Sub ResetCursor()

        m_lIndex = -1
        On Error Resume Next
        m_lIndex = LBound(ma_dValues, 1)

    End Sub

    Public ReadOnly Property AllValues() As Double()
        Get
            AllValues = VB6.CopyArray(ma_dValues)
        End Get
    End Property

    '=========================================================
    '
    ' IDStoreSeq Interface
    '


    Private WriteOnly Property IDStoreSeq_NextItem() As Object Implements IDStoreSeq.NextItem
        Set(ByVal Value As Object)

            'UPGRADE_WARNING: Couldn't resolve default property of object DataItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            ma_dValues(m_lIndex) = CDbl(Value)
            m_lIndex = m_lIndex + 1

        End Set
    End Property

    Private WriteOnly Property IDStoreSeq_NextItemLineEnd() As Object Implements IDStoreSeq.NextItemLineEnd
        Set(ByVal Value As Object)

            ' No "lines" in this storage, so same as NextItem
            'UPGRADE_WARNING: Couldn't resolve default property of object DataItem. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            ma_dValues(m_lIndex) = CDbl(Value)
            m_lIndex = m_lIndex + 1

        End Set
    End Property
End Class