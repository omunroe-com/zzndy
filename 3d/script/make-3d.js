var ctx = getContext('canvas');
var camera = new Obj(0, 0, 0, 0, 45 * deg, 0);

var grid = newPlane(10, 10, .5, .5);
var xcube = newRect(5, 1, 1);
var ycube = newRect(1,1,5);
var zcube = newRect(1,5,1);

xcube.pos = [2.5, 0, 0];
ycube.pos = [0, 2.5, 0];
zcube.pos = [0, 0, 2.5];

xcube.color = 'red';
ycube.color = 'green';
zcube.color = 'blue';


var deflt = new Color('#c7c5c5');
var b =deflt.tint(-100);
b.alpha=.3;
grid.color = b.toString();

objects = [grid, xcube, ycube, zcube];

rotate();
camera.alpha = -20*deg;

function rotate()
{
    ctx.clearRect(-400, -250, 800, 500);

    var i=-1, n=objects.length;
    while(++i<n)
    renderObj(objects[i]);

    camera.beta += 2 * deg;

    window.setTimeout(rotate, 20);
}

function renderObj( cube )
{
    var i = -1, n = cube.edgs.length;
    while( ++i < n )
    {
        var edge = cube.edgs[i];
        if( cube.color )
            ctx.strokeStyle = cube.color;
        else
            ctx.strokeStyle = '#c7c5c5';
        ctx.line3d(cube.pts[edge[0]], cube.pts[edge[1]], mm(camera.worldMatrix, cube.worldMatrix));
    }
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
