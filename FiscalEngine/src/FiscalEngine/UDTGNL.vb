Option Strict Off
Option Explicit On
Module UDTGNL
	'record for GNLScreen (Data file General Parms)
    Class GNLType 'DIMMED in common as GNL
        'UPGRADE_ISSUE: Declaration type not supported: Array of fixed-length strings. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="934BD4FF-1FF9-47BD-888F-D411E47E78FA"'
        Public ttl(4) As String 'titleline 1-4
        'UPGRADE_ISSUE: Declaration type not supported: Array of fixed-length strings. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="934BD4FF-1FF9-47BD-888F-D411E47E78FA"'
        Public dt(4) As String 'dates (proj, discov, prod, discnt)
        'UPGRADE_ISSUE: Declaration type not supported: Array of fixed-length strings. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="934BD4FF-1FF9-47BD-888F-D411E47E78FA"'
        Public pcd(2) As String 'price codes (oil and gas)
        Public wdepth As Single 'water depth in meters
        'UPGRADE_WARNING: Fixed-length string size must fit in the buffer. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"'
        Public og() As Char '1-OIL 2-GAS
        Public pval(6) As Single 'discount factors 1-6
        'UPGRADE_WARNING: Fixed-length string size must fit in the buffer. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"'
        Public dmtd() As Char 'discount method
        Public eqvl As Single

    End Class
End Module