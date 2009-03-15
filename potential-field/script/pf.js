/**
 * @class PotentialField
 */

function PotentialField( rows, cols )
{
    this.rows = rows;
    this.cols = cols;
    this.body = rows.times(cols.times(function() {
        return new Vector(0, 0);
    }));
}

var PF = PotentialField.prototype;

PF.add = function( other )
{
    if( !(other instanceof PotentialField) )
        throw new Error("Can only add potential field to another potential field.");

    if( other.rows != this.rows || other.cols != this.cols )
        throw new Error("Cannot add potetntial fields with different dimentions.");

    var res = new PotentialField(this.rows, this.cols);
    var i = -1;
    while( ++i < this.rows )
    {
        var j = -1;
        while( ++j < this.cols )
        {
            res.body[i][j] = this.body[i][j].add(other.body[i][j]);
        }
    }

    return res;
};

PF.apply = function( charge )
{
    var i = -1;
    while( ++i < this.rows )
    {
        var j = -1;
        while( ++j < this.cols )
        {
            this.body[i][j] = this.body[i][j].add(charge.getVector(j, i));
        }
    }

    return this;
};

function Charge( x, y, magnitude, reach )
{
    this.x = x;
    this.y = y;
    this.m = magnitude;
    this.reach = reach || this.m * 10;
    this.r2 = this.reach * this.reach;
}

var C = Charge.prototype;
C.getVector = function( x, y )
{
    var dx = this.x - x, dy = this.y - y;
    var d2 = dx * dx + dy * dy;

    var ang = Math.atan2(dy, dx);


    var v;
    if( d2 > this.r2 )
    {
        v = new Vector(0, 0);
    }
    else
    {
        var k = this.m * (this.r2 - d2) / this.r2;
        v = new Vector(k * Math.cos(ang), k * Math.sin(ang));
    }

    return v;
};

function Wall( x1, y1, x2, y2, magnitude, reach )
{
    this.x1 = x1;
    this.x2 = x2;
    this.y1 = y1;
    this.y2 = y2;

    this.m = magnitude;
    this.reach = reach || this.m * 10;
    this.r2 = this.reach * this.reach;
    this.len = Math.sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
}

var W = Wall.prototype;
W.getVector = function( x, y )
{
    var d = Math.abs((this.x2 - this.x1) * (this.y1 + y) - (this.x1 - x) * (this.y2 + this.y1) / this.len);

    var v;
    if( d > this.reach ) {
        v = new Vector(0, 0);
    }
    else {
        var k = this.m * (this.reach - d) / this.reach;
        v = new Vector(0, k);
    }
    return v;
};

W.dist = function(x, y)
{
    return Math.abs((this.x2 - this.x1) * (this.y1 - y) - (this.x1 - x) * (this.y2 - this.y1) / this.len);
};
