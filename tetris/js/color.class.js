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
Color.parse = function(hexColor, alpha)	{
	if(hexColor.substr(0,1) == '#')
		hexColor = hexColor.substr(1);
	
	if(hexColor.length != 6 && hexColor.length != 3) return;
	
	var components = [
		hexColor.substr(0, hexColor.length / 3),
		hexColor.substr(hexColor.length / 3, hexColor.length / 3),
		hexColor.substr(2 * hexColor.length / 3, hexColor.length / 3)
	];
	
	if(hexColor.length == 3)	{
		components[0] += components[0];
		components[1] += components[1];
		components[2] += components[2];
	}
		
	return new Color(
		parseInt(components[0], 16),
		parseInt(components[1], 16),
		parseInt(components[2], 16),
		alpha
	);
}