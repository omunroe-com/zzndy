// vertices
var vx =
        [
            [1, -1, 1]
            ,[1, 1, 1]
            ,[-1, 1, 1]
            ,[-1,-1, 1]
            ,[1, -1, -1]
            ,[1, 1, -1]
            ,[-1, 1, -1]
            ,[-1, -1, -1]
        ];

// edges
var eg = [
    [vx[0], vx[1]]
    ,[vx[1], vx[2]]
    ,[vx[2], vx[3]]
    ,[vx[3], vx[0]]
    ,[vx[4], vx[5]]
    ,[vx[5], vx[6]]
    ,[vx[6], vx[7]]
    ,[vx[7], vx[4]]
    ,[vx[0], vx[4]]
    ,[vx[1], vx[5]]
    ,[vx[2], vx[6]]
    ,[vx[3], vx[7]]
    ,[[0,0,0], [4, 0 ,0], 'red']
    ,[[0,0,0], [0,4,0], 'green']
    ,[[0,0,0], [0,0,4], 'blue']
];

var canvas1 = document.getElementById('canvas1');

var ctx1 = canvas1.getContext('2d');

setupCtx(ctx1, canvas1);
var theta = 0;
var elev =7;

loop();

function loop()
{
    var tario =5/5;
    ctx1.camera = [tario * elev * cos(theta),tario * elev * sin(theta),tario * elev];
    ctx1.cameraTarget = [elev * cos(theta),elev * sin(theta),elev];

    theta += 2 * deg;
    ctx1.clearRect(-400, -250, 800, 500);
    render(ctx1, eg);
    window.setTimeout(loop, 50);
}


function setupCtx( ctx, canvas )
{
    ctx.translate(parseInt(canvas.width) / 2, parseInt(canvas.height) / 2);
    ctx.scale3d = 10;
    ctx.camera = [4, 4, 4];
    ctx.cameraTarget = [5,5,5];
    ctx.strokeStyle = '#c7c5c5';
}

function render( ctx, edges )
{
    for( var i = 0; i < edges.length; ++i )
    {
        var edge = edges[i];
        if( edge.length == 3 )
            ctx.strokeStyle = edge[2];
        ctx.line3d(edge[0], edge[1]);
        if( edge.length == 2 )
            ctx.strokeStyle = '#c7c5c5';
    }
}
