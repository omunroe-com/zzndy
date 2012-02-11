(function(){
/**
 * Hexademical field class
 * handles all padding and placement issues
 */
var sqr3 = 1 / Math.sqrt(3), sqrt32 = Math.sqrt(3) / 2

HexField = function(rows, cols, width, spacing)	{
	this.rows = rows
	this.cols = cols
	this.cap = Math.max(rows, cols) - Math.floor(Math.min(rows, cols) / 2)
	this.width = width
	this.spacing = spacing
	this.dh = width * sqrt32
	this.walls = []

	var side = width * sqr3 - spacing / 2
	this.hex = new Hexagon(width - spacing, side)
	this.borderHex = new Hexagon(width + spacing, width * sqr3 + spacing / 2)
	this.ci = Math.floor(rows / 2)
	this.cj = Math.floor(cols / 2)
}

HexField.prototype.addWalls = function(walls)	{
	this.walls = this.walls.concat(walls)
}

HexField.prototype.hexOk = function(i, j)	{
	return this.cols - this.rows + i - j < this.cap && j-i < this.cap
}

HexField.prototype.makeAllPaths = function(ctx)	{
	ctx.beginPath()
	for(var i=0; i<this.rows; ++i)
		for(var j=0; j<this.cols; ++j)
			this.makePath(i, j, ctx)

	return ctx.closePath()
}

HexField.prototype.traceBorder = function(ctx)	{
	ctx.beginPath()
	this.borderHex.moveTo(getCoords.call(this, 0, 0), 0, ctx)
	
	for(var j=0; j < this.cap; ++j)	{
		this.borderHex.lineTo(getCoords.call(this, 0, j), 0, ctx)
		this.hex.lineTo(getCoords.call(this, -1, j), 3, ctx)
	}

	for(var i=0; i<=this.cols - this.cap; ++i)	{
		this.borderHex.lineTo(getCoords.call(this, i, this.cap + i - 1), 1, ctx)
		this.hex.lineTo(getCoords.call(this, i, this.cap + i), 4, ctx)
	}

	for(; i<this.rows; ++i)	{
		this.hex.lineTo(getCoords.call(this, i, this.cols), 5, ctx)
		this.borderHex.lineTo(getCoords.call(this, i, this.cols - 1), 2, ctx)
	}

	for(j=this.cols-1; j>=this.cols - this.cap; --j)	{
		this.borderHex.lineTo(getCoords.call(this, this.rows-1, j), 3, ctx)
		this.hex.lineTo(getCoords.call(this, this.rows, j), 0, ctx)
	}

	for(i=this.rows-1; i>=this.cols - this.cap; --i)	{
		this.borderHex.lineTo(getCoords.call(this, i, this.cols - this.cap + i - this.rows + 1), 4, ctx)
		this.hex.lineTo(getCoords.call(this, i, this.cols - this.cap + i - this.rows), 1, ctx)
	}

	for(++i;i>=0;--i)	{
		this.borderHex.lineTo(getCoords.call(this, i, 0), 5, ctx)
		this.hex.lineTo(getCoords.call(this, i-1, -1), 2 , ctx)
	}

	return ctx.closePath()
}

HexField.prototype.strokeAll = function(ctx)	{
	this.traceBorder(ctx).stroke()

	return this.makeAllPaths(ctx).stroke()
}

HexField.prototype.fillAll = function(ctx)	{
	this.traceBorder(ctx).stroke()
	return this.makeAllPaths(ctx).fill()
}

HexField.prototype.fillWalls = function(ctx)	{
	ctx.beginPath()
	this.walls.forEach(function(p){this.makePath(this.ci + p.y, this.cj + p.x, ctx, true)}, this)
	return ctx.closePath().fill()
}

//[private]
function getCoords(i, j){
	var even = i%2 * this.width / 2
	var pos = (j-this.cj)*this.width
	var shift = (this.ci - i) * this.width / 2

	return new Point(pos + shift, (i-this.ci) * this.dh)
}

HexField.prototype.makePath = function(i, j, ctx, dontCheck)	{
	if(!(dontCheck || this.hexOk(i, j))) return
	this.hex.makePath(getCoords.call(this, i, j), ctx)
}

HexField.prototype.stroke = function(i, j, ctx)	{
	if(!this.hexOk(i, j)) return
	this.hex.stroke(getCoords.call(this, i, j), ctx)
}

HexField.prototype.fill = function(i, j, ctx)	{
	if(!this.hexOk(i, j)) return
	this.hex.fill(getCoords.call(this, i, j), ctx)
}
})()