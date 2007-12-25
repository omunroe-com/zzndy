
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
				[t, x, y, r, a1, a2] = [0, p1.x, p1.y, p2, p3, p4];	
			else
				[x, y, w, h] = [p1, p2, p3, p4];
			break; 
		case 5: // arc
			[t, x, y, r, a1, a2] = [0, p1, p2, p3, p4, p5];
	}
	
	if(t == 0)	{ // circle
		var aa = 0;
		if(a1 < 0 && a2 > 0)
			return this.vertices.some(function(vertex){
				var [dx, dy] = [vertex.x - x, vertex.y - y];
				var d = Math.sqrt(dx*dx + dy*dy);
				if(d > r) return false;
				var a = (dy<0?pii - Math.acos(dx/d):Math.acos(dx/d));
				return (a1 + pii <= a && a <= pii) || (0 <= a && a <= a2);
			})
		else	{
			if(a1 < 0 && a2 < 0) [a1, a2] = [a1 + pii, a2 + pii];
			return this.vertices.some(function(vertex){
				var [dx, dy] = [vertex.x - x, vertex.y - y];
				var d = Math.sqrt(dx*dx + dy*dy);
				if(d > r) return false;
				var a = (dy<0?pii - Math.acos(dx/d):Math.acos(dx/d));
				return a1 <= a && a <= a2;
			})	
		}	
	}
	else if(t == 1)	{ // box
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