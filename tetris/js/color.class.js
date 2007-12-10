//// Class (structure) Color ////////////////////////////////////////
// Represents an RGB(A) colour

function Color(r, g, b, a)	{
	if(arguments.length == 0)	{ // default constructor
		this.red = this.green = this.blue = 0;
		this.alpha = NaN; // not specified
	}
	else if(arguments.length == 3 || arguments.length == 4)	{
		this.red = parseInt(r);
		this.green = parseInt(g);
		this.blue = parseInt(b);
		if(arguments.length == 4)	{
			this.alpha = parseFloat(a);
			if(this.alpha > 1) this.alpha = 1;
			else if(this.alpha < 0) this.alpha = 0;
		}
		else
			this.alpha = NaN;
	}
}

Color.prototype.toString = function()	{
	if(isNaN(this.alpha))
		return 'rgb(' + this.red + ', ' + this.green + ', ' + this.blue + ')';
	else
		return 'rgba(' + this.red + ', ' + this.green + ', ' + this.blue + ', ' + this.alpha + ')';
}

/*	Parse a colour specified in HTML format and
 *	return as a Color object, alpha is optional
 */
Color.parse = function(scol, alpha)	{
	var [base, r, g, b] = [10, null, null, null];
	if(scol.substr(0, 3) == 'rgb')	{ // parsing RGB(A) text
		var a;
		[,a,r,g,b,alpha] = scol.match(/(a)?\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*(?:,\s*(\d+)\s*)?\)/)
		alpha = (a && alpha)? parseFloat(alpha): null;
	}
	else	{
		base = 16;
		if(scol.substr(0,1) == '#')	scol = scol.substr(1);
		var w = scol.length/3;
		if(w != 2 && w != 1) return;
		
		[r, g, b] = [scol.substr(0, w), scol.substr(w, w), scol.substr(2*w, w)];
		
		if(w == 1)[r, g, b] = [r.rep(2), g.rep(2), b.rep(2)];
	}
	
	return new Color(
		parseInt(r, base), parseInt(g, base),
		parseInt(b, base), alpha
	);
}

Color.prototype.deviate = function(deviation)	{
	var dev = deviate(deviation.base);
	console.log(dev);
	return new Color(
		limit(0, 255, this.red + dev + deviate(deviation.r)),
		limit(0, 255, this.green + dev + deviate(deviation.g)),
		limit(0, 255, this.blue + dev + deviate(deviation.b)),
		limit(0, 1, this.alpha + deviate(deviation.a))
	);
}
