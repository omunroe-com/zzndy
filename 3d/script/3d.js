deg = Math.PI / 180;
sin = Math.sin;
cos = Math.cos;
acos = Math.acos;
sqrt = Math.sqrt;
tan = Math.tan;
atan2 = Math.atan2;

function Obj( x, y, z, ax, ay, az )
{
    this.p = [x,y,z];

    this.ax = ax || 0;
    this.ay = ay || 0;
    this.az = az || 0;

    this.worldMatrix = makeWorldMatrix.call(this);
}

Obj.prototype.__defineGetter__('pos', function()
{
    return this.p;
});

Obj.prototype.__defineSetter__('pos', function( x )
{
    this.p = x;
    this.worldMatrix = makeWorldMatrix.call(this);
});

Obj.prototype.__defineGetter__('alpha', function()
{
    return this.ax;
});

Obj.prototype.__defineGetter__('beta', function()
{
    return this.ay;
});

Obj.prototype.__defineGetter__('gamma', function()
{
    return this.az;
});

Obj.prototype.__defineSetter__('alpha', function( x )
{
    this.ax = x % (2 * Math.PI);
    this.worldMatrix = makeWorldMatrix.call(this);
});

Obj.prototype.__defineSetter__('beta', function( x )
{
    this.ay = x % (2 * Math.PI);
    this.worldMatrix = makeWorldMatrix.call(this);
});

Obj.prototype.__defineSetter__('gamma', function( x )
{
    this.az = x % (2 * Math.PI);
    this.worldMatrix = makeWorldMatrix.call(this);
});

function makeWorldMatrix()
{
    var tr = matrix(4, 4, 0, 1);
    var rx = matrix(4, 4, 0, 1);
    var ry = matrix(4, 4, 0, 1);
    var rz = matrix(4, 4, 0, 1);

    tr[0][3] = this.p[0];
    tr[1][3] = this.p[1];
    tr[2][3] = this.p[2];

    var cx = cos(this.ax);
    var sx = sin(this.ax);

    var cy = cos(this.ay);
    var sy = sin(this.ay);

    var cz = cos(this.az);
    var sz = sin(this.az);

    rx[1][1] = rx[2][2] = cx;
    rx[1][2] = -sx;
    rx[2][1] = sx;

    ry[0][0] = ry[2][2] = cy;
    ry[0][2] = sy;
    ry[2][0] = -sy;

    rz[0][0] = rz[1][1] = cz;
    rz[0][1] = -sz;
    rz[1][0] = sz;

    return mm(tr, rx, ry, rz);
}

/**
 * Multiply matrices
 */
function mm()
{
    function multiply( m1, m2 )
    {
        var n = m1.length;
        var m = m2[0].length;
        var mk = m1[0].length;

        if(m === undefined || mk == undefined)
            throw new Error(['Multiplying martix by vector not supported. Use [[],[],[]] and such instead.']);

        if( mk != m2.length )
            throw new Error(['Cannot multiply matrices ',n,'x',mk,' and ',m2.length,'x',m,'.'].join(''));

        var mx = matrix(n, m, 0, 0);

        var i = -1;
        while( ++i < n )
        {
            var j = -1;
            while( ++j < m )
            {
                // rij = sum(ain*bmj)
                var k = -1;
                while( ++k < mk )
                    mx[i][j] += m1[i][k] * m2[k][j];
            }
        }

        return mx;
    }

    return [].reduce.call(arguments, multiply);
}

/**
 * Create matrix n x m with elements defaulting to deflt and diagonal elements set to diag
 * @param {Number} n      number of rows
 * @param {Number} m      number of columns
 * @param {Number} deflt  default value
 * @param {Number} diag   diagonal value
 * @return {Array}        the matrix
 */
function matrix( n, m, deflt, diag )
{
    var a = [];
    diag = diag || deflt;
    var i = -1;
    while( ++i < n )
    {
        a.push([]);
        var j = -1;
        while( ++j < m )
        {
            a[i].push(i == j ? diag : deflt);
        }
    }

    return a;
}

function dot( v1, v2 )
{
    return v1[0] * v2[0] + v1[1] * v2[1] + v1[2] * v2[2];
}

function cross( v1, v2 )
{
    return [v1[1] * v2[2] - v1[2] * v2[1], v1[2] * v2[0] - v1[0] * v2[2], v1[0] * v2[1] - v1[1] * v2[0]];
}

function len( v )
{
    return sqrt(v[0] * v[0] + v[1] * v[1] + v[2] * v[2]);
}

function norm( v )
{
    var d = len(v);
    return [v[0] / d, v[1] / d,v[2] / d];
}

function angle( v1, v2 )
{
    return atan2(len(cross(v1, v2)), dot(v1, v2));
}
var acrane =0;
function convert3dTo2d( p, worldMatrix, camera )
{
    var pos = mm(worldMatrix, [[p[0]], [p[1]], [p[2]], [1]]);

    if(false && camera)
    {
        var pp = [
                [1,0,0,-camera.pos[0]],
                [0,1,0,-camera.pos[1]],
                [0,0,1,0],
                [0,0,1/camera.pos[2],0]
            ];

        var ppos = mm(pp, [[pos[0][0]], [pos[1][0]], [pos[2][0]], [1]]);
        return [ppos[0]/ppos[3], ppos[1]/ppos[3], ppos[2]/ppos[3]];
    }

    return [pos[0][0], pos[1][0], pos[2][0]];
}

var C = CanvasRenderingContext2D.prototype;

C.line3d = function( p1, p2, worldMatrix )
{
    var cp1 = convert3dTo2d(p1, worldMatrix );
    var cp2 = convert3dTo2d(p2, worldMatrix );

    this.beginPath()
            .moveTo(cp1[0] * this.scale, -cp1[1] * this.scale)
            .lineTo(cp2[0] * this.scale, -cp2[1] * this.scale)
            .stroke();
};

C.path3d = function( shape, worldMatrix )
{
    var i = -1, n = shape.length;

    var point = convert3dTo2d(shape[n - 1], worldMatrix);

    this.moveTo(point[0] * this.scale, -point[1] * this.scale);
    while( ++i < n )
    {
        var p = shape[i];
        point = convert3dTo2d(p, worldMatrix);
        if( p.length == 3 ) {
            this.lineTo(point[0] * this.scale, -point[1] * this.scale);
        }
        else if( p.length == 6 )
        {
            var control = convert3dTo2d([p[3],p[4],p[5]], worldMatrix);
            this.quadraticCurveTo(control[0] * this.scale, -control[1] * this.scale, point[0] * this.scale, -point[1] * this.scale);
        }
    }

    return this;
};

C.render = function(obj)
{
    if(!(obj instanceof Obj) || (!('pts' in obj && 'edgs' in obj) && !('pths' in obj)))
        throw new Error('Can only render 3D objects of edges or paths.');

    var matrix = mm(this.camera.worldMatrix, obj.worldMatrix);

    if( obj.color )
        this.strokeStyle = obj.color;
    else
        this.strokeStyle = this.strokeStyleDefault;

    if( 'lineWidth' in obj )
        this.lineWidth = obj.lineWidth;
    else
        this.lineWidth = 1;

    if('pths' in obj)
    {
        var c = new Color(this.strokeStyle);
        c.alpha = .07;
        this.fillStyle = c.toString();

        var i = -1, n = obj.pths.length;
        while(++i<n)
        {
            var rect = obj.pths[i];

            this
                .beginPath()
                .path3d(rect, matrix)
                .closePath()
                .stroke()
                .fill();
        }
    }
    else
    {
        var i = -1, n = obj.edgs.length;

        while( ++i < n )
        {
            var edge = obj.edgs[i];
            this.line3d(obj.pts[edge[0]], obj.pts[edge[1]], matrix);
        }
    }
}

function perspectiveTransform( fov, aspect, near, far )
{
    return [
        [tan(fov / 2) / aspect, 0,0,0],
        [0, 1 / tan(fov / 2), 0,0],
        [0,0,(near + far) / (near - far), near * far / (near - far)],
        [0,0,-1,0]
    ];
}
