String.prototype.wrapLetter = function(element, class)
{
	return this
		.split('')
		.map(function(c){return '<' + element + ' class="' + class + '">' + c + '</' + element + '>'})
		.join('');
}

Node = function(name, type, data)
{
	this.getName = function(){return name;}
	this.getType = function(){return type;}
	this.getData = function(){return data;}
}

Node.prototype.toString = function()
{
	switch(this.getType())
	{
		case "DIR":
			this.toString = function()
			{
				return '<table>' + this.getData().map(function(d){return '<tr><td class="type">' + d.getType().wrapLetter('span', 'letter') + '</td><td class="name">' + d.getName().wrapLetter('span', 'letter') + '</td></tr>'}).join('\n') + '</table>';
			}
			break;
		default:
			this.toString = function(){return '&lt;UNSUPPORTED TYPE&gt;'}
	}

	return this.toString();
}
