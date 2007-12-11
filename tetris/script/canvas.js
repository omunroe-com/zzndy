// Extensions for Canvas (CanvasRenderingContext2D) used by tetris

CanvasRenderingContext2D.prototype.sharpRect
  = function(x, y, w, h){with(this)	{
	fillRect(x, y, w, 1);
	fillRect(x, y+1, 1, h-1);
	fillRect(x, y+h, w+1, 1);
	fillRect(x+w, y, 1, h);
}}

CanvasRenderingContext2D.prototype.vLine = function(x, y, h)	{
	this.fillRect(x, y, 1, h);
}

CanvasRenderingContext2D.prototype.hLine = function(x, y, w)	{
	this.fillRect(x, y, w, 1);
}

/**
  render a number using dots
*/
CanvasRenderingContext2D.prototype.renderNumber = function(/*string*/num, obj)	{
	this.translate(obj.orgn.x, obj.orgn.y);
	this.fillStyle = obj.fill;
	var s = obj.size;
	
	for(var c=0; c<num.length; ++c)	{
		var [cc, pc] = [num[c], obj.last[c]];
		if(cc != pc)	{
			var a = -(num.length - c) * 7 * (s + 1);
			for(var i=0; i<8; ++i)	{
				var h = i * (s + 1);
				var [cp, pp] = [Digit[cc][i], Digit[pc][i]];
				for(var j=0; j<7; ++j)	{
					var [cur, pre] = [cp[j], pp[j]];
					if(cur != pre)	{
						if(cur) this.fillRect(a + j * (s + 1), h, s, s);
						else this.clearRect(a + j * (s + 1), h, s, s);
					}
				}
			}
		}
	}
	this.translate(-obj.orgn.x, -obj.orgn.y);
	obj.last = num;
}

CanvasRenderingContext2D.prototype.styleFill
  = function(s){with(this)	{
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

	if(color == 0)
		this.clearRect(0, 0, this.tetris.box.width, this.tetris.box.height);
	else with(this)	{
		fillStyle = color.toString();
		fillRect(0, 0, tetris.box.width, tetris.box.height);
		makePath(tetris.val.hi);
		styleFill(tetris.val.hlColor);
		makePath(tetris.val.lo);
		styleFill(tetris.val.shadowColor);
	}
	this.translate(-origin.x, -origin.y);
	return color;// needed for use with reduce
}

var Digit = {
	'0':	[[0,0,1,1,1,1,0],
		 [0,1,1,0,0,1,1],
		 [0,1,1,0,0,1,1],
		 [0,1,1,0,0,1,1],
		 [0,1,1,0,0,1,1],
		 [0,1,1,0,0,1,1],
		 [0,1,1,0,0,1,1],
		 [0,0,1,1,1,1,0]],

	'1':	[[0,0,0,1,1,0,0],
		 [0,0,1,1,1,0,0],
		 [0,1,0,1,1,0,0],
		 [0,0,0,1,1,0,0],
		 [0,0,0,1,1,0,0],
		 [0,0,0,1,1,0,0],
		 [0,0,0,1,1,0,0],
		 [0,1,1,1,1,1,1]],

	'2':	[[0,0,1,1,1,1,0],
		 [0,1,1,0,0,1,1],
		 [0,0,0,0,0,1,1],
		 [0,0,0,1,1,1,0],
		 [0,0,1,1,0,0,0],
		 [0,1,1,0,0,0,0],
		 [0,1,1,0,0,1,1],
		 [0,1,1,1,1,1,1]],

	'3':	[[0,0,1,1,1,1,0],
		 [0,1,1,0,0,1,1],
		 [0,0,0,0,0,1,1],
		 [0,0,0,1,1,1,0],
		 [0,0,0,0,0,1,1],
		 [0,0,0,0,0,1,1],
		 [0,1,1,0,0,1,1],
		 [0,0,1,1,1,1,0]],

	'4':	[[0,0,0,0,1,1,1],
		 [0,0,0,1,0,1,1],
		 [0,0,1,1,0,1,1],
		 [0,0,1,0,0,1,1],
		 [0,1,1,0,0,1,1],
		 [0,1,1,1,1,1,1],
		 [0,0,0,0,0,1,1],
		 [0,0,0,0,0,1,1]],

	'5':	[[0,1,1,1,1,1,1],
		 [0,1,1,0,0,0,0],
		 [0,1,1,0,0,0,0],
		 [0,1,1,1,1,1,0],
		 [0,0,0,0,0,1,1],
		 [0,0,0,0,0,1,1],
		 [0,1,1,0,0,1,1],
		 [0,0,1,1,1,1,0]],

	'6':	[[0,0,1,1,1,1,0],
		 [0,1,1,0,0,1,1],
		 [0,1,1,0,0,0,0],
		 [0,1,1,1,1,1,0],
		 [0,1,1,0,0,1,1],
		 [0,1,1,0,0,1,1],
		 [0,1,1,0,0,1,1],
		 [0,0,1,1,1,1,0]],

	'7':	[[0,1,1,1,1,1,1],
		 [0,1,1,0,0,1,1],
		 [0,0,0,0,1,1,0],
		 [0,0,0,1,1,0,0],
		 [0,0,0,1,1,0,0],
		 [0,0,0,1,1,0,0],
		 [0,0,0,1,1,0,0],
		 [0,0,0,1,1,0,0]],

	'8':	[[0,0,1,1,1,1,0],
		 [0,1,1,0,0,1,1],
		 [0,1,1,0,0,1,1],
		 [0,0,1,1,1,1,0],
		 [0,1,1,0,0,1,1],
		 [0,1,1,0,0,1,1],
		 [0,1,1,0,0,1,1],
		 [0,0,1,1,1,1,0]],

	'9':	[[0,0,1,1,1,1,0],
		 [0,1,1,0,0,1,1],
		 [0,1,1,0,0,1,1],
		 [0,1,1,0,0,1,1],
		 [0,0,1,1,1,1,1],
		 [0,0,0,0,0,1,1],
		 [0,1,1,0,0,1,1],
		 [0,0,1,1,1,1,0]],

	'.':	[[0,0,0,0,0,0,0],
		 [0,0,0,0,0,0,0],
		 [0,0,0,0,0,0,0],
		 [0,0,0,0,0,0,0],
		 [0,0,0,0,0,0,0],
		 [0,0,0,0,0,0,0],
		 [0,0,0,1,1,0,0],
		 [0,0,0,1,1,0,0]],

	'#':	[[0,0,0,0,0,0,0],
		 [0,0,0,0,0,0,0],
		 [0,0,0,0,0,0,0],
		 [0,0,0,0,0,0,0],
		 [0,0,0,0,0,0,0],
		 [0,0,0,0,0,0,0],
		 [0,0,0,0,0,0,0],
		 [0,0,0,0,0,0,0]]
}