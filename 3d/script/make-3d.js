var ctx = getContext('canvas');
var camera = new Obj(0, 0, 0, 0, 45 * deg, 0);

var grid = newPlane(10, 10, .5, .5);
var xcube = newRect(5, 1, 1);
var ycube = newRect(1, 1, 5);
var zcube = newRect(1, 5, 1);

xcube.pos = [2.5, 0, 0];
ycube.pos = [0, 2.5, 0];
zcube.pos = [0, 0, 2.5];

xcube.color = 'red';
ycube.color = 'green';
zcube.color = 'blue';


var deflt = new Color('#c7c5c5');
var b = deflt.tint(-100);
b.alpha = .3;
grid.color = b.toString();

objects = [grid, xcube, ycube, zcube];
objects = [];

var colorCount = 0;
var planeColors = ['rgba(255, 255, 100, 1)', 'rgba(100, 255, 255, 1)',  'rgba(255, 100, 255, 1)'];
planeColors = ['rgba(255, 180, 0, 1)', 'rgba(230, 0, 180, 1)', 'rgba(0, 180, 230, 1)'];

function makeBox(x, y, z, size)
{
    var a = {
        planes:[
            makeRounded(x, y, z+2, size, size, .2),
            roundedxz(makeRounded(x, z, y+2, size, size, .2)),
            roundedyz(makeRounded(z, y, x+2, size, size, .2)),
            makeRounded(x, y, z-2, size, size, .2),
            roundedxz(makeRounded(x, z, y-2, size, size, .2)),
            roundedyz(makeRounded(z, y, x-2, size, size, .2))
        ],

        color : planeColors[(colorCount++)%planeColors.length]
    }
    return a;
}

var size = 3.5;
var boxes = [
    makeBox(0,0,0,size),
    makeBox(size*1.5, 0, 0, size),
    makeBox(0, size*1.5, 0, size)
];
    
function renderBox(planes, matrix)
{
    var i = -1;
    var n = planes.planes.length;

    while( ++i < n )
        renderRounded(planes.planes[i], planes.color, matrix);
}

rotate();
camera.alpha = -10 * deg;

function rotate()
{
    ctx.clearRect(-400, -250, 800, 500);

    var i = -1, n = objects.length;
    while( ++i < n )
        renderObj(objects[i]);

    camera.beta += .2 * deg;
    camera.alpha -= .05 * deg;

    var matrix = mm(camera.worldMatrix, grid.worldMatrix);

    ctx.lineWidth = 4;

    i=-1;
    n = boxes.length;
    while(++i<n)
        renderBox(boxes[i], matrix);

    ctx.lineWidth = 1;

    window.setTimeout(rotate, 20);
}

function renderRounded( rect, color, matrix )
{
    var c = new Color(color);
    c.alpha = .07;
    ctx.strokeStyle = color;
    ctx.fillStyle = c.toString();
    ctx
            .beginPath()
            .path3d(rect, matrix)
            .closePath()
            .stroke()
            .fill();
}

function renderObj( cube )
{
    var matrix = mm(camera.worldMatrix, cube.worldMatrix);
    var i = -1, n = cube.edgs.length;
    while( ++i < n )
    {
        var edge = cube.edgs[i];
        if( cube.color )
            ctx.strokeStyle = cube.color;
        else
            ctx.strokeStyle = '#c7c5c5';
        ctx.line3d(cube.pts[edge[0]], cube.pts[edge[1]], matrix);
    }


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

function getContext( canvasId )
{
    var canvas = document.getElementById(canvasId);
    var ctx = canvas.getContext('2d');
    ctx.translate(parseInt(canvas.width, 10) / 2, parseInt(canvas.height, 10) / 2);

    ctx.scale = 30;
    ctx.strokeStyle = '#c7c5c5';

    return ctx;
}
