function newRect( w, l, h )
{
    var obj = new Obj(0, 0, 0);
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

function newPlane( n, m, dx, dz )
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