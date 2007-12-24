var canvas, field, dot, done = false;
var pi = Math.PI, pi2 = pi / 2, deg = pi / 180.0;
var [astep, gstep] = [6, 5];
var bxsize = 10;
var treshold = 0.001;

window.onload = function()	{
	var element = document.getElementById('walker-canvas');
	canvas = element.getContext('2d');
	canvas.fillStyle = 'silver';
	canvas.size = new Box(element.width, element.height);
	//canvas.fill_rect(new Point(0), canvas.size);
	
	field = new Field(50, 50);
	
	canvas.draw(field);		
	dot = new Player(new Point(200, 100), 0*deg);	
	loop();
}

window.onkeypress = function(e)	{
	var scale = e.shiftKey?10:1;
	switch(e.keyCode)	{
		case 37://left
			dot.dir -= scale*astep*deg;
			dot.dir %= 2*pi;
			break;
		case 38://up
			dot.pos.x += scale*gstep*Math.cos(dot.dir);
			dot.pos.y += scale*gstep*Math.sin(dot.dir);
			break;
		case 39://right
			dot.dir += scale*astep*deg;
			dot.dir %= 2*pi;
			break;
		case 40://down
			dot.pos.x -= scale*gstep*Math.cos(dot.dir);
			dot.pos.y -= scale*gstep*Math.sin(dot.dir);
			break;
		case 27:
			done = true;
			break;
	}
	loop();
}

function loop()	{
	if(!loop.lastdot || !loop.lastdot.eq(dot))	{
		canvas.clear_rect(new Point(0, 0), canvas.size);
		var visible;
		with(dot) visible = field.asObjects().filter(function(rect){
					return rect.in(pos, 200, dir - angle / 2, dir + angle / 2)
				})
		canvas.draw(field);
		canvas.draw(dot, visible);
	}
}

function Field(w,h)	{
	this.size = new Box(w, h);
	fl = new Array(h);
	for(var i=0; i<h; ++i)	{
		fl[i] = new Array(w);
		for(var j=0; j<w; ++j)
			if(i==0 || j==0 || i == h-1 || j == w-1) fl[i][j] = 1;
			else
				fl[i][j] = (i+j)%2//((i*j)%(i+j))?0:1;
	}
	this.field = fl;
}

Field.prototype.asObjects = function()	{
	var rval = [];
	for(var i=0;i<this.size.height;++i)	{
		for(var j=0;j<this.size.width;++j)	{
			if(this.field[i][j] == 1) rval.push((new Rect(j*bxsize, i*bxsize, bxsize, bxsize))); 
		}
	}
	return rval;
}

function Player(pos, dir, angle){
	this.pos = new Point(pos);
	this.dir = dir || 0;
	this.angle = angle || 60*deg;
}

Player.prototype.eq = function(player){
	return Math.abs(player.dir - this.dir) < treshold && this.pos.eq(player.pos);
}

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

function Rect(x, y, w, h)	{
	var c1, c2, d1, d2;
	
	if(arguments.length == 2 && x instanceof Point && y instanceof Box)	{
		[c1, c2] = [x.x, x.y];
		[d1, d2] = [y.width, y.height];
	}
	else if(arguments.length == 4 && typeof x == 'number' && typeof y == 'number' && typeof w == 'number' && typeof h == 'number')
		[c1, c2, d1, d2] = [x, y, w, h];
		
	this.pos = new Point(c1, c2);
	this.size = new Box(d1, d2);
}

Rect.prototype.in = function(p1, p2, p3, p4, p5)	{
	var t=1, x, y, w, h, r, a1 = 0, a2 = 2*pi; //t=[0, 1] -> [circle, rectangle]
	switch(arguments.length)	{
		case 1: // rect version
			if(!(p1 instanceof Rect)) return undefined;
			[x, y, w, h] = [p1.pos.x, p1.pos.y, p1.size.width, p1.size.height];
			break;
		case 2: // two pairs or pair-n-radius
			if(!(p1 instanceof Point)) return undefined;
			if(p2 instanceof Box)
				[x, y, w, h] = [p1.x, p1.y, p2.width, p2.height];
			else
				[t, x, y, r] = [0, p1.x, p1.y, p2];
			break;
		case 3: // (point, w, h), (x, y, box) or (x, y, r) 
			if(p1 instanceof Point)
				[x, y, w, h] = [p1.x, p1.y, p2, p3];
			else if(p3 instanceof Box)
				[x, y, w, h] = [p1, p2, p3.width, p3.height];
			else
				[t, x, y, r] = [0, p1, p2, p3];
			break;
		case 4: // rect or arc
			if(p1 instanceof Point)
				[t, x, y, r, a1, a2] = [0, p1.x, p1.x, p2, p3, p4];	
			else
				[x, y, w, h] = [p1, p2, p3, p4];
			break; 
		case 5: // arc
			[t, x, y, r, a1, a2] = [0, p1, p2, p3, p4, p5];
	}
	
	if(t == 0)	{
		return this.vertices.some(function(vertex){
			var [dx, dy] = [vertex.x - x, vertex.y - y];
			var g = Math.sqrt(dx*dx + dy*dy);
			var a = Math.acos(dx / g) + (dx<0?pi:0);
			return g <= r && a >= a1 && a <= a2;
		})		
	}
	else if(t == 1)	{
		return this.vertices.some(function(vertex){
			
			return vertex.x >= x && vertex.x <= x + w && vertex.y >= y && vertex.y <= y + h;
		})
	}
	return undefined;
}

Rect.prototype.__defineGetter__('vertices', function(){
	if(!this._vertices)
		this._vertices = [
			new Point(this.pos), 
			(new Point(this.pos)).add(this.size.width, 0), 
			(new Point(this.pos)).add(this.size.width, this.size.height), 
			(new Point(this.pos)).add(0, this.size.height)
		]
	return this._vertices
})

Rect.prototype.__defineGetter__('x', function(){
	return this.pos.x;	
})

Rect.prototype.__defineSetter__('x', function(val){
	this._vertices = null;
	this.pos.x = val;	
})

Rect.prototype.__defineGetter__('y', function(){
	return this.pos.y;	
})

Rect.prototype.__defineSetter__('y', function(val){
	this._vertices = null;
	this.pos.y = val;	
})

Rect.prototype.__defineGetter__('width', function(){
	return this.size.width;	
})

Rect.prototype.__defineSetter__('width', function(val){
	this._vertices = null;
	this.size.width = val;	
})

Rect.prototype.__defineGetter__('height', function(){
	return this.size.height;	
})

Rect.prototype.__defineSetter__('height', function(val){
	this._vertices = null;
	this.size.height = val;	
})

Rect.prototype.toString = function()	{
	return this.pos + ':' + this.size; 
}

CanvasRenderingContext2D.prototype.fill_rect = function(rect, size)	{
	if(arguments.length == 1 && rect instanceof Rect)
		this.fillRect(rect.pos.x, rect.pos.y, rect.size.width, rect.size.height)
	else if(arguments.length == 2 && rect instanceof Point && size instanceof Box)
		this.fillRect(rect.x, rect.y, size.width, size.height)
	else this.fillRect.apply(this, arguments);		
}

CanvasRenderingContext2D.prototype.clear_rect = function(rect, size)	{
	if(arguments.length == 1 && rect instanceof Rect)
		this.clearRect(rect.pos.x, rect.pos.y, rect.size.width, rect.size.height)
	else if(arguments.length == 2 && rect instanceof Point && size instanceof Box)
		this.clearRect(rect.x, rect.y, size.width, size.height)
	else this.clearRect.apply(this, arguments);		
}

CanvasRenderingContext2D.prototype.draw = function(what, obstacles)	{
	if(what instanceof Player)	{
		var fs = this.fillStyle;
		this.fillStyle = canvas.createRadialGradient(what.pos.x, what.pos.y, 0, what.pos.x, what.pos.y, 200);
		this.fillStyle.addColorStop(0, 'red');
		this.fillStyle.addColorStop(1, 'rgba(255, 0, 0, .05)');
		this.beginPath();
		
		this.moveTo(what.pos.x, what.pos.y);
		this.arc(what.pos.x, what.pos.y, 200, what.dir - what.angle / 2, what.dir + what.angle / 2, false);
		this.moveTo(what.pos.x, what.pos.y);
		this.arc(what.pos.x, what.pos.y, 3, 0, 2*pi, false);
		
		this.closePath();
		this.fill();
		this.fillStyle = 'green';
		for(var i=0, bx; bx = obstacles[i]; ++i)
			canvas.fill_rect(bx);
		
		this.fillStyle = fs;
	}
	else if( what instanceof Field ){
		bx = new Box(bxsize);
		for(var i=0; i<what.size.height; ++i)
			for(var j=0; j<what.size.width; ++j)
				if(what.field[i][j] == 1)
					canvas.fill_rect(new Point(i*bx.height, j*bx.width), bx);
	}
}