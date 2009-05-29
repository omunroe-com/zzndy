deg = Math.PI / 180
sin = Math.sin;
cos = Math.cos;
acos = Math.acos;
sqrt = Math.sqrt;
tan = Math.tan;

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
                angle(this.cameraTarget, [0,1,0])
                ,angle(this.cameraTarget, [1,0,0])
                ,angle(this.cameraTarget, [0,0,1])
            ];
    angles = [0,0,0];

    var cp1 = convert3dTo2d(p1, this.camera, angles, this.cameraTarget);
    var cp2 = convert3dTo2d(p2, this.camera, angles, this.cameraTarget);

    this.beginPath()
            .moveTo(cp1[0] * this.scale3d, cp1[1] * this.scale3d)
            .lineTo(cp2[0] * this.scale3d, cp2[1] * this.scale3d)
            .stroke();
};

function perspectiveTransform( fov, aspect, near, far )
{
    return [
        [tan(fov / 2) / aspect, 0,0,0],
        [0, 1 / tan(fov / 2), 0,0],
        [0,0,(near + far) / (near - far), near * far / (near - far)],
        [0,0,-1,0]
    ];
}
           /*

///Start with the update, right now is just an enterframe
this.onEnterFrame = function() {
 //Cosine and sin calculation from the angles
 var cosY:Number = Math.cos(angleY);
 var sinY:Number = Math.sin(angleY);
 var cosX:Number = Math.cos(angleX);
 var sinX:Number = Math.sin(angleX);
 var cosZ:Number = Math.cos(angleZ);
 var sinZ:Number = Math.sin(angleZ);
 //We need to clear the stage to be able to draw the new cube on every frame
 clear();
 //This array will store the distance form the camera to each Shape
 distanceArray = [];
 //Ok, let's start with each shape, remember each one is made with 4 points.
 for (var j:Number = 0; j<shapes.length; j++) {
  //Vars to store the distance from each point in the shape, in each axis.
  dx = 0;
  dy = 0;
  dz = 0;
  //Now, we need to look inside each shape, obviously, we will find 4 points, so let's take a look to all the points, and make some calculations.
  for (var i:Number = 0; i<shapes[j].length; i++) {
   //Here, is where we apply the projections for each point coordinate
   x1 = cosY*(sinZ*(points[shapes[j][i]].y)+cosZ*(points[shapes[j][i]].x))-sinY*(points[shapes[j][i]].z);
   y1 = sinX*(cosY*(points[shapes[j][i]].z)+sinY*(sinZ*(points[shapes[j][i]].y)+cosZ*(points[shapes[j][i]].x)))+cosX*(cosZ*(points[shapes[j][i]].y)-sinZ*(points[shapes[j][i]].x));
   z1 = cosX*(cosY*(points[shapes[j][i]].z)+sinY*(sinZ*(points[shapes[j][i]].y)+cosZ*(points[shapes[j][i]].x)))-sinX*(cosZ*(points[shapes[j][i]].y)-sinZ*(points[shapes[j][i]].x));
   //Now we store the new point coodinates, based on the angles.
   points[shapes[j][i]].x = x1;
   points[shapes[j][i]].y = y1;
   points[shapes[j][i]].z = z1;
   //we add each point coordinate to the relative distance var
   dx += x1;
   dy += y1;
   dz += z1;
   //Next code, is the way to get the X and Y points to draw in the Stage, we take care about offsets, and distance from the camera in the Z Axis
   //Each point will be scaled acording the distance to the camera. We are using the _x and _y as variables to render, not for calculations
   points[shapes[j][i]]._x = vpX+(points[shapes[j][i]].x+xOffset)*(fl/(points[shapes[j][i]].z+fl+zOffset));//scale;
   points[shapes[j][i]]._y = vpY+(points[shapes[j][i]].y+yOffset)*(fl/(points[shapes[j][i]].z+fl+zOffset));//scale;
  }
  //Ok, you have checked all the points in the shape, now is time too look at the distances
  //This is important, cos the distance to the observer, will mean which shape is render on top.
  //Since we have a sum of distances in X Y and Z, we need an average
  //Next code is the same than dividing by four (cos the four points per shape)
  dx /= shapes[j].length-1;
  dy /= shapes[j].length-1;
  dz /= shapes[j].length-1;
  //We add an element to the distances, and is basically the distance from each point, to the camera. Note than fl is on the Z axis, and is where the camera is.
  //We store and object with the distance, and also a shape ID to know which shape corresponds with the distance.
  distanceArray.push({d:Math.round(Math.sqrt(Math.pow(dx-xOffset, 2)+Math.pow(dy-yOffset, 2)+Math.pow(fl-dz, 2))), index:j});
 }
 //Now, we need to sort the array, to know wich shape is near and need to be render on top. So a simple Sort function.
 distanceArray.sortOn("d");
 //Here is where the fun starts. Now is time to render each polygon
 lineStyle(0,0xFFFFFF,40);
 //for each element in the distance array (the same length than the shape array, we will render the shape.
 for (w=0; w<distanceArray.length; w++) {
  //Drawing sfuff to do the line and the fill color (this can be better :D)
  beginFill(0xff-(distanceArray[w].index*10),100);
  moveTo(points[shapes[distanceArray[w].index][0]]._x,points[shapes[distanceArray[w].index][0]]._y);
  //Now in each shape, we draw lines to each point to close the shape.
  for (var j:Number = 0; j<shapes[distanceArray[w].index].length; j++) {
   lineTo(points[shapes[distanceArray[w].index][j]]._x,points[shapes[distanceArray[w].index][j]]._y);
  }
  //Close the shape with the endFill
  endFill();
 }
 //Capture Keys pressed to move and rotate the cube
 checkKeys();
 //DONE!
};
*/