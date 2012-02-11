(function($) {
    $.Point = function( x, y ) {
        if ( x instanceof Object && 'x' in x && 'y' in x ) {
            this.x = x.x;
            this.y = x.y;
        }
        else {
            this.x = x;
            this.y = y;
        }
    };

    var P = Point.prototype;

    P.toString = function()
    {
        return ['(', this.x, ', ', this.y, ')'].join('');
    };

    P.inVicinityOf = function( p, r )
    {
        if ( r === undefined ) r = 5;
        return Math.abs(p.x - this.x) < r && Math.abs(p.y - this.y) < r;
    };

    P.inside = function( x1, y1, x2, y2 )
    {
        if ( arguments.length == 2 ) return this.x >= 0 && this.y >= 0 && this.x <= x1 && this.y <= y1;
        return this.x >= x1 && this.y >= y1 && this.x <= x2 && this.y <= y2;
    };
}(window));