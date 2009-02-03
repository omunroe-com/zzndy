function Vector(x, y)
{
    this.x = x;
    this.y = y;
}

Vector.prototype.add = function(v)
{
    return new Vector(this.x + v.x, this.y + v.y);
}

function Point(x, y)
{
    this.x = x;
    this.y = y;
}

function Force(point, vector)
{
    this.point = point;
    this.vector = vector;
}