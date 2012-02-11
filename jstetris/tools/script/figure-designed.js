// Figure desined class - enable interactive figure creation;
//(function($){
var size = 5;

FigureDesigned = function(id)	{
	this.mask = [];
	this.primary = [];
	this.revolving = [];
	this.rotation = 0;
	this.id = id;

	for(var i=0; i<size; ++i)	{
		this.mask.push([]);
		this.primary.push([]);
		this.revolving.push([]);

		for(var j=0; j<size; ++j)	{
			this.mask[i].push(false);
			this.primary[i].push($td({
				'class': 'disabled', 
				onmousedown: 'cellClick('+[id, i, j]+')', 
			}));
			this.revolving[i].push($td({'class': 'disabled'}));
		}
	}
}

var F = FigureDesigned.prototype;

F.render = function(target)	{
	var rows1 = [], rows2 = [];

	for(var i=0; i<size; ++i)	{
		var row1 = [], row2 = [];

		for(var j=0; j<size; ++j)	{
			row1.push(this.primary[i][j]);
			row2.push(this.revolving[i][j]);
		}

		rows1.push($tr(row1));
		rows2.push($tr(row2));
	}

	target.appendChild($div({'class' : 'figure', id: 'figure-' + this.id}, [
		$table({'class': 'figure-designed', id: 'primary-' + this.id}, rows1), 
		$br(),
		$table({'class': 'figure-designed', id: 'revolving-' + this.id}, rows2)
	]));
}

F.toText = function()	{
	var text = ['['];
	for(var i=0; i<size; ++i)	{
		text.push('    [' + this.mask[i].join(', ') + ']' + (i==size-1?'':',') );
	}
	text.push(']');

	return text.join('\n').replace(/true/g, 1).replace(/false/g, 0);
}

//})(this)
