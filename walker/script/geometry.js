
function Box(width, height)	{
	if(width instanceof Box){
		height = width.height;
		width = width.width;
	}
	else if(typeof width == 'number' && arguments.length == 1) height = width;
	
	this.width = width || 0;
	this.height = height || 0;
}

Box.prototype.add = function(dw, dh)	{
	if(dw instanceof Box){
		dh = dw.height;
		dw = dw.width;
	}
	else if(typeof dw == 'number' && arguments.length == 1) dh = dw;
	
	this.width += dw || 0;
	this.height += dh || 0;
	
	return this;
}

Box.prototype.toString = function()	{
	return this.width + 'x' + this.height;
}

function Point(x, y)	{
	if(x instanceof Point){
		y = x.y;
		x = x.x;
	}
	else if(typeof x == 'number' && arguments.length == 1) y = x;
	
	this.x = x || 0;
	this.y = y || 0;
}

Point.prototype.add = function(dx, dy)	{
	if(dx instanceof Point){
		dy = dx.y;
		dx = dx.x;
	}
	else if(typeof dx == 'number' && arguments.length == 1) dy = dx;
	
	this.x += dx || 0;
	this.y += dy || 0;
	
	return this;
}

Point.prototype.eq = function(point)	{
	return Math.abs(this.x - point.x) < treshold && Math.abs(this.y - point.y) < treshold;
}
Point.prototype.toString = function()	{
	return '(' + this.x + ',' + this.y + ')';
}