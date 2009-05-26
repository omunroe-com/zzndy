deg = Math.PI / 180
sin = Math.sin;
cos = Math.cos;
acos = Math.acos;
sqrt = Math.sqrt;

function dot( v1, v2 )
{
    return v1[0] * v2[0] + v1[1] * v2[1] + v1[2] * v2[2];
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
    return acos(dot(v1, v2) / (len(v1) * len(v2)));
}

function convert3dTo2d( point, camera, angles, viewer )
{
    var stx = sin(angles[0]);
    var ctx = cos(angles[0]);
    var sty = sin(angles[1]);
    var cty = cos(angles[1]);
    var stz = sin(angles[2]);
    var ctz = cos(angles[2]);

    var dx = point[0] - camera[0];
    var dy = point[1] - camera[1];
    var dz = point[2] - camera[2];

    var a = cty * dz + sty * (stz * dy + ctz * dx);
    var b = ctz * dy - stz * dx;

    var ddx = cty * (stz * dy + ctz * dx) - sty * dz;
    var ddy = stx * (a) + ctx * (b);
    var ddz = ctx * (a) - stx * (b);

    var vz = (viewer[2] / ddz);

    return [
        (ddx - viewer[0]) * vz
        ,(ddy - viewer[1]) * vz
    ];
}

CanvasRenderingContext2D.prototype.line3d = function( p1, p2 )
{
    var angles =
            [
                angle(this.cameraTarget, [1,0,0])
                ,angle(this.cameraTarget, [0,1,0])
                ,angle(this.cameraTarget, [0,0,1])
            ];
    //angles = [0,0,0];

    var cp1 = convert3dTo2d(p1, this.camera, angles, this.cameraTarget);
    var cp2 = convert3dTo2d(p2, this.camera, angles, this.cameraTarget);

    this.beginPath()
            .moveTo(cp1[0] * this.scale3d, cp1[1] * this.scale3d)
            .lineTo(cp2[0] * this.scale3d, cp2[1] * this.scale3d)
            .stroke();
};