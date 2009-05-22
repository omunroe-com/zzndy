var out = document.getElementById('dng');

function renderDng( dng )
{
    var txt = [];
    var i = -1, n = dng.length;
    while( ++i < n )
    {
        var j = -1, m = dng[i].length;
        while( ++j < m )
        {
            txt.push(dng[i][j] ? '#' : '.');
        }
        txt.push('\n');
    }
    out.innerHTML = txt.join('');
}

var dng = makeDng(30, 10);
renderDng(dng);