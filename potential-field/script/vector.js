(function() {

    Vector = function( x, y )
    {
        this.x = x;
        this.y = y;
    };

    var V = Vector.prototype;

    V.add = function( v )
    {
        return new Vector(this.x + v.x, this.y + v.y);
    };

    V.mul = function( n )
    {
        return new Vector(this.x * n, this.y * n);
    };

    V.div = function( n )
    {
        return new Vector(this.x / n, this.y / n);
    };

    Vector.prototype.__defineGetter__('angle', function() {
        return Math.atan2(this.y, this.x);
    });

    V.toString = function()
    {
        return ['[',this.x, '; ', this.y, ']'].join('')
    };

    Point = function( x, y )
    {
        this.x = x;
        this.y = y;
    };

    var P = Point.prototype;
    P.dist = function( other )
    {
        var dx = this.x - other.x;
        var dy = this.y - other.y;
        return Math.sqrt(dx * dx + dy * dy);
    };

    P.toString = function()
    {
        return ['(',this.x, ', ', this.y, ')'].join('')
    };

    Force = function( point, vector )
    {
        this.point = point;
        this.vector = vector;
    };

})();