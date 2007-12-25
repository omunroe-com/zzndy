var canvas, field, dot, done = false;
var pi = Math.PI, pi2 = pi / 2, deg = pi / 180.0, pii = 2 * pi;
var [astep, gstep] = [6, 5];
var bxsize = 20;
var treshold = 0.001;

window.onload = function()	{
	var element = document.getElementById('walker-canvas');
	canvas = element.getContext('2d');
	canvas.fillStyle = 'silver';
	canvas.size = new Box(element.width, element.height);
	//canvas.fill_rect(new Point(0), canvas.size);
	
	field = new Field(25, 25);
	dot = new Player(new Point(200, 100), 180*deg);	
	loop();
}

window.onkeypress = function(e)	{
	var scale = e.shiftKey?10:1;
	switch(e.keyCode)	{
		case 37://left
			dot.dir -= scale*astep*deg;
			dot.dir %= pii;
			break;
		case 38://up
			dot.pos.x += scale*gstep*Math.cos(dot.dir);
			dot.pos.y += scale*gstep*Math.sin(dot.dir);
			break;
		case 39://right
			dot.dir += scale*astep*deg;
			dot.dir %= pii;
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
				fl[i][j] = ((i*j)%(i+j))?0:1;
	}
	this.field = fl;
}

Field.prototype.asObjects = function()	{
	var rval = [];
	for(var i=0;i<this.size.height;++i)
		for(var j=0;j<this.size.width;++j)
			if(this.field[i][j] == 1) rval.push(new Rect(j*bxsize, i*bxsize, bxsize, bxsize)); 
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
		this.fillStyle.addColorStop(0, 'rgba(240, 235, 160, .9)');
		this.fillStyle.addColorStop(1, 'rgba(230, 225, 150, .1)');
		this.beginPath();
		
		this.moveTo(what.pos.x, what.pos.y);
		this.arc(what.pos.x, what.pos.y, 200, what.dir - what.angle / 2, what.dir + what.angle / 2, false);
		this.moveTo(what.pos.x, what.pos.y);
		this.arc(what.pos.x, what.pos.y, 3, 0, 2*pi, false);
		
		this.closePath();
		this.fill();
		this.fillStyle = 'grey';
		obstacles.some(function(bx){canvas.fill_rect(bx)});
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