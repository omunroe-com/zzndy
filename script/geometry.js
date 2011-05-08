(function($) {
    $.Point = function( x, y ) {
		// duck typing
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

    P.plus = function(point)    {
        return new Point(this.x + point.x, this.y + point.y);
    }
	
	$.Size= function(w, h)
	{
		// duck typing
		if(w instanceof Object && 'w' in w && 'h' in w)
		{
			// copy constructor
			this.w = w.w;
			this.h = h.w;
		}
		else
		{
			this.w = w;
			this.h = h;
		}
	}
}(window));
