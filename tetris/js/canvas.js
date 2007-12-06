// Extensions for Canvas (CanvasRenderingContext2D) used by tetris

CanvasRenderingContext2D.prototype.sharpRect = function(x, y, w, h)	{
	this.fillRect(x, y, w, 1);
	this.fillRect(x, y+1, 1, h-1);
	this.fillRect(x, y+h, w+1, 1);
	this.fillRect(x+w, y, 1, h);
}

CanvasRenderingContext2D.prototype.vLine = function(x, y, h)	{
	this.fillRect(x, y, 1, h);
}

CanvasRenderingContext2D.prototype.hLine = function(x, y, w)	{
	this.fillRect(x, y, w, 1);
}

CanvasRenderingContext2D.prototype.renderNumber = function(/*string*/num, obj)	{
	this.translate(obj.orgn.x, obj.orgn.y);
	this.fillStyle = obj.fill;
	
	for(var c=0; c<num.length; ++c)	{
		if(num[c] != obj.last[c])	{
			var a = -(num.length - c) * 7 * (obj.size + 1);
			for(var i=0; i<8; ++i)	{
				var h = i * (obj.size + 1);
				for(var j=0; j<7; ++j)	{
					if(Digit[num[c]][i][j] != Digit[obj.last[c]][i][j])	{
						if(Digit[num[c]][i][j])
							this.fillRect(a + j * (obj.size + 1), h, obj.size, obj.size);
						else
							this.clearRect(a + j * (obj.size + 1), h, obj.size, obj.size);
					}
				}
			}
		}
	}
	this.translate(-obj.orgn.x, -obj.orgn.y);
	obj.last = num;
}

CanvasRenderingContext2D.prototype.styleFill = function(s)
{with(this){
	var t = fillStyle;
	fillStyle = s;
	fill();
	fillStyle = t;
}}

CanvasRenderingContext2D.prototype.makePath = function(list)	{
	with(this){
		beginPath();
		moveTo(list[0], list[1]);
		for(var i=2; i<list.length; i += 2)lineTo(list[i], list[i+1]);
		closePath();
	}
}

CanvasRenderingContext2D.prototype.renderBox = function(color, point)	{
	var origin = new Point(point.x * this.tetris.gridsize.width, point.y * this.tetris.gridsize.height);

	this.translate(origin.x, origin.y);

	if(color == -1)
		this.clearRect(0, 0, this.tetris.box.width, this.tetris.box.height);
	else with(this)	{
		fillStyle = color;
		fillRect(0, 0, tetris.box.width, tetris.box.height);

		makePath(tetris.val.hi);
		styleFill(tetris.val.hlColor);
		makePath(tetris.val.lo);
		styleFill(tetris.val.shadowColor);
	}
	this.translate(-origin.x, -origin.y);
	return color;// neede for use with reduce
}