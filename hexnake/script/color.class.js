(function(){
/**
 * Color class
 * RGB(A) representation and operations
 * 
 * requires utils.js
 */

function parse(scol, alpha)	{
	var base=10, r, g, b;
	if(scol.substr(0, 3) == 'rgb')	{ // parsing RGB(A) text
		var m = scol.match(/(a)?\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*(?:,\s*((?:1|0)?\.\d+|1|0)\s*)?\)/)
		var a = m[1], r = m[2], g = m[3], b = m[4], o = m[5]
		alpha = (a && o)? parseFloat(o): parseFloat(alpha);
	}
	else	{
		base = 16;
		if(scol.substr(0,1) == '#')	scol = scol.substr(1);
		var w = scol.length/3;
		if(w != 2 && w != 1) return;

		r = scol.substr(0, w);
		g = scol.substr(w, w);
		b = scol.substr(2*w, w);

		if(w == 1)	{
				r = r.rep(2);
				g = g.rep(2);
				b = b.rep(2);
		}
	}

	return [parseInt(r, base), parseInt(g, base),
		parseInt(b, base), alpha];
}

Color = function(r, g, b, a)	{
	switch(arguments.length)	{
		case 0: // default construtor
			this.red = this.green = this.blue = 0;
			this.alpha = NaN; // not specified
			break;
		case 1:
			if(typeof t == 'object' && 'red' in r && 'green' in r && 'blue' in r && 'alpha' in r)	{
				// copy constructor
				this.red = r.red.constrain(0, 255);
				this.green = r.green.constrain(0, 255);
				this.blue = r.blue.constrain(0, 255);
				this.alpha = r.alpha.constrain(0, 1);
			}
		case 2: // fallthrough for string constructor with optional alpha argument
			if(typeof r != 'string')	{
				this.red = this.green = this.blue = 0;
				this.alpha = NaN; // not specified
				break;
			}
			
			var components = parse(r, g === undefined ? NaN : g)
			this.red   = components[0].constrain(0, 255);
			this.green = components[1].constrain(0, 255);
			this.blue  = components[2].constrain(0, 255);
			this.alpha = components[3].constrain(0, 1);
			break;
		case 3:
		case 4:
			this.red = parseInt(r).constrain(0, 255);
			this.green = parseInt(g).constrain(0, 255);
			this.blue = parseInt(b).constrain(0, 255);
			if(arguments.length == 4)
				this.alpha = parseFloat(a).constrain(0, 1);
			else
				this.alpha = NaN;
			break;
		default:
			throw new Error('Invalid arguments to Color constructor');
	}
}

Color.prototype.toString = function(asHex)	{
	if(asHex)
		return '#' + this.red.toString(16) + this.green.toString(16) + this.blue.toString(16);
	if(isNaN(this.alpha))
		return 'rgb(' + this.red + ', ' + this.green + ', ' + this.blue + ')';
	return 'rgba(' + this.red + ', ' + this.green + ', ' + this.blue + ', ' + this.alpha + ')';
}

Color.prototype.tint = function(tones)	{
	return new Color(
		 this.red   + tones
		,this.green + tones
		,this.blue  + tones
		,this.alpha);
}

/**
 * Randomly change current color, yield new object
 */
Color.prototype.deviate = function(r, g, b, a, minStep)	{
	function deviate(value, amount)	{
		return value + Math.random() * (amount + 1) * 2 - amount
	}
	
	var step = minStep === undefined ? 4 : minStep;
	var dev = deviate(deviation.base/step)*step;
	return new Color(
		 this.red   + deviate(this.red, r/step)*step
		,this.green + deviate(this.green, r/step)*step
		,this.blue  + deviate(this.blue, r/step)*step
		,this.alpha + deviate(this.alpha, r/step)*step
	);
}
})()
