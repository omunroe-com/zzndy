// Geometric primitives: Point and Rectangle

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

//// Rectangle class /////////////////////////////////////////////////////

function Rectangle(width, height)	{
	if(arguments.length == 1 && typeof width == 'object')	{
		this.width = width.width;
		this.height = width.height;
	}
	else	{
		this.width = width;
		this.height = height;
	}
}

Rectangle.sum = function(width,height)	{
	return new Rectangle(width.width + height.width, width.height + height.height);
}

Rectangle.dif = function(width,height)	{
	return new Rectangle(width.width - height.width, width.height - height.height);
}

Rectangle.mul = function(width,height)	{
	return new Rectangle(width.width * height.width, width.height * height.height);
}

Rectangle.div = function(width,height)	{
	return new Rectangle(width.width / height.width, width.height / height.height);
}

Rectangle.prototype.add = function(width, height)	{
	if(arguments.length == 1 && typeof width == 'object')	{
		this.width += width.width;
		this.height += width.height;
	}
	else	{
		this.width += width;
		this.height += height;
	}
}

Rectangle.prototype.sub = function(width, height)	{
	if(arguments.length == 1 && typeof width == 'object')	{
		this.width -= width.width;
		this.height -= width.height;
	}
	else	{
		this.width -= width;
		this.height -= height;
	}
}

Rectangle.prototype.mul = function(width, height)	{
	if(arguments.length == 1 && typeof width == 'object')	{
		this.width *= width.width;
		this.height *= width.height;
	}
	else	{
		this.width *= width;
		this.height *= height;
	}
}

Rectangle.prototype.div = function(width, height)	{
	if(arguments.length == 1 && typeof width == 'object')	{
		this.width /= width.width;
		this.height /= width.height;
	}
	else	{
		this.width /= width;
		this.height /= height;
	}
}

Rectangle.prototype.toString = function()	{
	return "Rectangle: " + this.width + 'X' + this.height;
}