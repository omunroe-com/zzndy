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