var A_FLOOR= 0x200
var A_CEIL = 0x400 

function Point(x, y, action)	{
	if(x instanceof Point)	{
		y = x.y
		x = x.x
	}
	
	this.x = (action === undefined) ? x : (action == A_FLOOR) ? Math.floor(x) : (action == A_CEIL) ? Math.ceil(x) : x
	this.y = (action === undefined) ? y : (action == A_FLOOR) ? Math.floor(y) : (action == A_CEIL) ? Math.ceil(y) : y	
}

Point.prototype.inside = function(x1, y1, x2, y2)
{
	if(arguments.length == 2) return this.x >= 0 && this.y >= 0 && this.x <= x1 && this.y <= y1
	return this.x >= x1 && this.y >= y1 && this.x <= x2 && this.y <= y2
}

Point.prototype.toString = function()
{
	return '('+this.x+', '+this.y+')'.fmt(this)
}

function Rect(w, h, action)
{
	if(w instanceof Rect)	{
		h = w.h; w = w.w
	}
	
	this.w = (action === undefined) ? w : (action == A_FLOOR) ? Math.floor(w) : (action == A_CEIL) ? Math.ceil(w) : w
	this.h = (action === undefined) ? h : (action == A_FLOOR) ? Math.floor(h) : (action == A_CEIL) ? Math.ceil(h) : h
}

Rect.prototype.toString = function()	{
	return this.w + 'x' + this.h
}

/**
 * Accepts arguments {Point} position plus {Rect} size
 * or {Number} x, {Number} y, {Number} w, {Number} h 
 */
function Region()	{
	if(arguments.length == 1)	{
		this.pos = new Point(arguments[0])
		this.size = new Rect(arguments[1])
	}
	else	{
		this.pos = new Point(arguments[0], arguments[1])
		this.size = new Rect(arguments[2], arguments[3])	
	}
}
