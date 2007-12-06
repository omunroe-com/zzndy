//// Field class /////////////////////////////////////////////////////////////

function Field(width, height)	{
	this.size = new Rectangle(width, height);

	this.field = new Array(height);
	for(var i=0; i<height; ++i)	{
		this.field[i] = new Array(width);
		for(var j=0; j<width; ++j)
			this.field[i][j] = -1;
	}
}

Field.prototype.burnLines = function(lowestLine, lines)	{
	var rslt = [];

	var i=lowestLine, k=i-1;
	
	for(i;k>0;--i,--k)	{
		var lineClear = true;
		while(lines[k]) --k;
		for(var j=0;j<this.field[i].length;++j)	{
			if(this.field[i][j] != this.field[k][j])	{
				this.field[i][j] = this.field[k][j];
				rslt.push({color: Figure.colors[this.field[i][j]], point: new Point(j,i)});
			}
			if(lineClear && this.field[i][j] != -1) lineClear = false;
		}
		if(lineClear)break;
	}
	for(;i>k;--i)	{
		for(var j=0;j<this.field[i].length;++j)	{
			if(this.field[i][j] != -1)	{
				this.field[i][j] = -1;
				rslt.push({color: -1, point: new Point(j,i)});
			}
		}
	}
	return rslt;
}

Field.prototype.fill = function(figure)	{
	var lines = {};
	var blocks = figure.getBlocksToRender();
	lines[10] = true;
	for(var i=0; i<blocks.length; ++i)	{
		var block = blocks[i];
		this.field[block.y][block.x] = figure.figureId;
		lines[block.y] = true;
	}
	return lines;
}

// called through reduce
Field.prototype.fillBlocks = function(lines, point)	{
	this.field[point.y][point.x] = true;
	lines[point.y] = true;
	return lines;
}

Field.prototype.getFilledLines = function(allLines)	{
	var res = {};
	for(var i in allLines)	{
		var filled = true;
		for(var j=0; j<this.field[i].length; ++j)
			if(this.field[i][j] == -1)	{
				filled = false;
				break;
			}
		if(filled)res[i] = true;
	}
	return res;
}

/**
 *  Yield  coordinates for all blocks in line spceified
 */
Field.prototype.getLineBlocks = function(i)	{
	var res = [];
	for(var j=0; j<this.field[i].length; ++j)
		if(this.field[i] != -1) res.push(new Point(j, i));
	return res;
}

Field.prototype.getState = function()	{
	var res = [[], []];
	for(var i=0; i<this.field.length; ++i)	{
		for(var j=0; j<this.field[i].length; ++j)
			if(this.field[i][j] == -1) res[0].push(new Point(j, i));
			else res[1].push(new Point(j, i));
	}
	return res;
}

// called through reduce
Field.prototype.pointOk = function(point)	{
	return ((point.x >= 0) 
		&& (point.x < this.size.width) 
		&& (point.y < this.size.height) 
		&& (point.y >= 0)
		&& this.field[point.y][point.x] == -1);
}

// called through reduce
Field.prototype.getMinHeight = function(result, point)	{
	var heightTreshold = Math.min(result, this.size.height - point.y - 1);
	for(var y = point.y + 1; y<point.y + heightTreshold + 1; ++y)
		if(this.field[y][point.x] != -1)
			return Math.min(heightTreshold, y - point.y - 1);
	return heightTreshold;
}