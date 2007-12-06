//// Point class /////////////////////////////////////////////////////

function Point(x, y)	{
	if(arguments.length == 1 && typeof x == 'object')	{
		this.x = x.x;
		this.y = x.y;
	}
	else	{
		this.x = x;
		this.y = y;
	}
}

Point.sum = function(x,y)	{
	return new Point(x.x + y.x, x.y + y.y);
}

Point.dif = function(x,y)	{
		return new Point(x.x - y.x, x.y - y.y);
}

Point.prototype.add = function(x, y)	{
	if(arguments.length == 1 && typeof x == 'object')	{
		this.x += x.x;
		this.y += x.y;
	}
	else	{
		this.x += x;
		this.y += y;
	}
}

Point.prototype.sub = function(x, y)	{
	if(arguments.length == 1 && typeof x == 'object')	{
		this.x -= x.x;
		this.y -= x.y;
	}
	else	{
		this.x -= x;
		this.y -= y;
	}
}

Point.prototype.mul = function(x, y)	{
	if(arguments.length == 1 && typeof x == 'object')	{
		this.x *= x.x;
		this.y *= x.y;
	}
	else	{
		this.x *= x;
		this.y *= y;
	}
}

Point.prototype.div = function(x, y)	{
	if(arguments.length == 1 && typeof x == 'object')	{
		this.x /= x.x;
		this.y /= x.y;
	}
	else	{
		this.x /= x;
		this.y /= y;
	}
}

Point.prototype.toString = function()	{
	return "Point: (" + this.x + ', ' + this.y + ')';
}