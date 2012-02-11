(function(){
/**
 *	Hexagon drawing class
 */
var sqr3 = 1 / Math.sqrt(3), sqrt32 = Math.sqrt(3) / 2

Hexagon = function(width, side)	{
	side = side || width * sqr3
	var w = width/2, s = side/2

	this.vertices = [
		new Point(0, side), new Point(w, s), 
		new Point(w, - s), new Point(0, - side),
		new Point(-w, - s), new Point(-w, s)
	]
}

Hexagon.prototype.makePath = function(origin, ctx, noNeedToClose)	{
	ctx.moveTo(origin.x + this.vertices[0].x, origin.y - this.vertices[0].y)
	ctx.lineTo(origin.x + this.vertices[1].x, origin.y - this.vertices[1].y)
	ctx.lineTo(origin.x + this.vertices[2].x, origin.y - this.vertices[2].y)
	ctx.lineTo(origin.x + this.vertices[3].x, origin.y - this.vertices[3].y)
	ctx.lineTo(origin.x + this.vertices[4].x, origin.y - this.vertices[4].y)
	ctx.lineTo(origin.x + this.vertices[5].x, origin.y - this.vertices[5].y)
	noNeedToClose || ctx.lineTo(origin.x + this.vertices[0].x, origin.y - this.vertices[0].y)
}

//[private]
function makeClosedPath(origin, ctx)	{
	ctx.beginPath()
	this.makePath(origin, ctx, true)
	ctx.closePath()
}

Hexagon.prototype.stroke = function(origin, ctx)	{
	makeClosedPath.call(this, origin, ctx)
	ctx.stroke()
}

Hexagon.prototype.fill = function(origin, ctx)	{
	makeClosedPath.call(this, origin, ctx)
	ctx.fill()
}

Hexagon.prototype.lineTo = function(origin, n, ctx)	{
	ctx.lineTo(origin.x + this.vertices[n%6].x, origin.y - this.vertices[n%6].y)
}

Hexagon.prototype.moveTo = function(origin, n, ctx)	{
	ctx.moveTo(origin.x + this.vertices[n%6].x, origin.y - this.vertices[n%6].y)
}
})()