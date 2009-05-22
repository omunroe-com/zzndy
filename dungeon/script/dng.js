function Region( i, j, mi, mj )
{
    this.i = i;
    this.j = j;
    this.mi = mi;
    this.mj = mj;
}

/**
 * Create dungeon map h rows high and w rows wide.
 * @param {Number} h
 * @param {Number} w
 */
function makeDng( h, w )
{
    var dng = [];
    var i = -1, j = -1;
    while( ++i < h )
    {
        dng[i] = [];
        j = -1;
        while( ++j < w ) {
            var val = i == 0 || i == h - 1 || j == 0 || j == w - 1;
            dng[i].push(val);
        }
    }

    var mh = 3, mw = 3, wall;

    var regions = [new Region(1, 1, h - 2, w - 2)];
    var r;
    while( r = regions.pop() )
    {
        wall = mw + 1 + Math.floor(Math.random() * (r.mj - r.j - (mw+1) * 2));
        i = -1;
        while( ++i <= r.mi - r.i )
            dng[r.i+i][wall] = !dng[r.i+i][wall];
    }

    return dng;
}