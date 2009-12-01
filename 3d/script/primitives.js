/**
 * Create 3d objects.
 */

(function($){
 
// box ////////////////////////////////////////////////////////////////////////

$.newRect = function( x, y, z, w, l, h )
{
    var obj = new Obj(x, y, z);
    obj.pts = [
        [w / 2, -h / 2, l / 2]
        ,[w / 2, -h / 2, -l / 2]
        ,[-w / 2, -h / 2, -l / 2]
        ,[-w / 2, -h / 2, l / 2]
        ,[w / 2, h / 2, l / 2]
        ,[-w / 2, h / 2, l / 2]
        ,[-w / 2, h / 2, -l / 2]
        ,[w / 2, h / 2, -l / 2]
    ];

    obj.edgs = [
        [0,1],[1,2],[2,3],[3,0],
        [4,5],[5,6],[6,7],[7,4],
        [0,4],[1,7],[2,6],[3,5]
    ];

    return obj;
}

// grid plane /////////////////////////////////////////////////////////////////

$.newPlane = function( n, m, dx, dz )
{
    var obj = new Obj(0, 0, 0, 0, 0, 0);
    var i = -1;
    obj.pts = [];
    obj.edgs = [];

    var x = n * dx / 2;
    var z = m * dz / 2;

    while( ++i <= n )
    {
        obj.pts.push([-x,0,dz * i - z]);
        obj.pts.push([ x,0,dz * i - z]);

        obj.edgs.push([i * 2, i * 2 + 1]);
    }

    obj.edgs.push([0,n*2]);

    i = 0;
    while( ++i < m )
    {
        obj.pts.push([dx * i - x, 0, -z]);
        obj.pts.push([dx * i - x, 0, z]);

        obj.edgs.push([n*2+i * 2, n*2+i * 2 + 1]);
    }

    obj.edgs.push([1,n*2+1]);

    return obj;
}
 
// rounded box ////////////////////////////////////////////////////////////////

$.newRounded = function(x, y, z, size, gap)
{
    var obj = new Obj(x,y,z);
    var s2 = size / 2;
    if (gap === undefined) gap = .14;
    size *= (1 - gap);

    obj.pths = [
        makeRounded(x, y, z+s2, size, size, .2),
        roundedxz(makeRounded(x, z, y+s2, size, size, .2)),
        roundedyz(makeRounded(z, y, x+s2, size, size, .2)),
        makeRounded(x, y, z-s2, size, size, .2),
        roundedxz(makeRounded(x, z, y-s2, size, size, .2)),
        roundedyz(makeRounded(z, y, x-s2, size, size, .2))
    ];

    obj.color = planeColors[(colorCount++)%planeColors.length];
    obj.lineWidth = 4;

    return obj;
}

/**
 * Generate path for rounded rectangle in x-y plane.
 * @param {Number} x  abscissa of the center
 * @param {Number} y  ordinate of the center
 * @param {Number} z  applicate of the center
 * @param {Number} w  width
 * @param {Number} h  height
 * @param {Number} q  round factor - 0 - sharp corners, 1 - round shape
 */
function makeRounded( x, y, z, w, h, q )
{
    w = w / 2;
    h = h / 2;
    q = (1 - q);
    return [
        [x - w * q, y - h, z, x - w, y - h, z],
        [x + w * q, y - h, z],
        [x + w, y - h * q, z, x + w, y - h, z],
        [x + w, y + h * q, z],
        [x + w * q, y + h, z, x + w, y + h, z],
        [x - w * q, y + h, z],
        [x - w, y + h * q, z, x - w, y + h, z],
        [x - w, y - h * q, z]
    ];
}

// convert path in xy plane to one in xz plane.
function roundedxz( rounded )
{
    var i = -1, n = rounded.length;
    while( ++i < n )
    {
        var p = rounded[i];
        var t = p[1];
        p[1] = p[2];
        p[2] = t;
        if( p.length == 6 )
        {
            t = p[4];
            p[4] = p[5];
            p[5] = t;
        }
    }
    return rounded;
}

// convert path in xy plane to one yz plane.
function roundedyz( rounded )
{
    var i = -1, n = rounded.length;
    while( ++i < n )
    {
        var p = rounded[i];
        var t = p[0];
        p[0] = p[2];
        p[2] = t;
        if( p.length == 6 )
        {
            t = p[3];
            p[3] = p[5];
            p[5] = t;
        }
    }
    return rounded;
}


 
 })(this);
