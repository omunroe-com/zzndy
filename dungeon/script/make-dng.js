/**
 * Setup and render dungeon.
 */

try
{
    var out = document.getElementById('dng');
    out = {};

    var cl = '&#xb7';
    var cm = '-';
    var ct = ' ';

    function renderDng( dng )
    {
        var txt = [];
        var i = -1, n = dng.length;
        while( ++i < n )
        {
            var j = -1, m = dng[i].length;
            while( ++j < m )
            {
                txt.push(dng[i][j] ? ct : cl);
            }
            txt.push('\n');
        }
        out.innerHTML = txt.join('');
    }

    var width = 120;
    var height = 40;
    var minDim = 3; // minimal room width or height
    var maxAsp = 2; // maximum room aspect

    var dng = makeDng(width, height, minDim, maxAsp);
    renderDng(dng);
}
catch( ex )
{
    console.log(ex);
}
