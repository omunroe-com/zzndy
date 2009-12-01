var ctx = getContext('canvas');
var camera = new Obj(0, 0, 0, 0, 45 * deg, 0);

function inc_x(d) { camera.pos[0] += d; updateTitle(); }
function inc_y(d) { camera.pos[1] += d; updateTitle(); }
function inc_z(d) { camera.pos[2] += d; updateTitle(); }

function updateTitle()
{
    document.getElementsByTagName('h1')[0].innerHTML = 
    document.getElementsByTagName('title')[0].innerHTML = '3d (' + camera.pos.join(', ') + ')';
}

var grid = newPlane(10, 10, .5, .5);

var a = 5, b = 1;

var xcube = newRect(a/2, 0, 0, a, b, b);
var ycube = newRect(0, a/2, 0, b, a, b);
var zcube = newRect(0, 0, a/2, b, b, a);

xcube.color = 'red';
ycube.color = 'green';
zcube.color = 'blue';

var deflt = new Color('#c7c5c5');
var b = deflt.tint(-100);
b.alpha = .3;
grid.color = b.toString();

var colorCount = 0;
var planeColors = ['rgba(255, 255, 100, 1)', 'rgba(100, 255, 255, 1)',  'rgba(255, 100, 255, 1)'];
planeColors = ['rgba(255, 180, 0, 1)', 'rgba(230, 0, 180, 1)', 'rgba(0, 180, 230, 1)'];

var size = 3.5;

var objects = [
    xcube
    ,ycube
    ,zcube
    ,newRounded(0,0,0,size)
    ,newRounded(size, 0, 0, size, 0)
    ,newRounded(0, size, 0, size)
];
   
camera.alpha = -10 * deg;
ctx.camera = camera;
rotate();

function rotate()
{
    ctx.clearRect(-400, -250, 800, 500);

    var i = -1, n = objects.length;
    while( ++i < n )
        ctx.render(objects[i]);

    camera.beta += .2 * deg;

    window.setTimeout(rotate, 20);
}

function getContext( canvasId )
{
    var canvas = document.getElementById(canvasId);
    var ctx = canvas.getContext('2d');
    ctx.translate(parseInt(canvas.width, 10) / 2, parseInt(canvas.height, 10) / 2);

    ctx.scale = 30;
    ctx.strokeStyle = '#c7c5c5';
    ctx.strokeStyleDefault = '#c7c5c5';

    return ctx;
}
