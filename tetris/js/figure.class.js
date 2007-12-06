//// Figure class /////////////////////////////////////////////////////////

function Figure(block)	{
	if(arguments.length == 0)	{ // default constructor
		this.figureId = Figure.defaultFigureId;
		this.figure = Figure.defaultFigure;
		this.color = Figure.colors[Figure.defaultFigureId];
		this.position = new Point(0, 0);
	}
	else if(typeof block == 'number')	{ // initialize by id
		this.figureId = block;
		this.figure = Figure.figures[block];
		this.color = Figure.colors[block];
		this.position = new Point(0, 0);
	}
	else	{ // initializa by another block
		this.figureId = block.figureId;
		this.figure = block.figure;
		this.color = block.color;
		this.position = block.position;
	}
}

Figure.figures = [
	/*0 |*/  [ [0,0,1,0,0], [0,0,1,0,0], [0,0,1,0,0], [0,0,1,0,0], [0,0,0,0,0] ],
	/*1 _|*/ [ [0,0,0,0,0], [0,0,1,1,0], [0,0,1,0,0], [0,0,1,0,0], [0,0,0,0,0] ],
	/*2 |_*/ [ [0,0,0,0,0], [0,1,1,0,0], [0,0,1,0,0], [0,0,1,0,0], [0,0,0,0,0] ],
	/*3 T*/  [ [0,0,0,0,0], [0,0,0,0,0], [0,1,1,1,0], [0,0,1,0,0], [0,0,0,0,0] ],
	/*4 _-*/ [ [0,0,0,0,0], [0,0,0,0,0], [0,0,1,1,0], [0,1,1,0,0], [0,0,0,0,0] ],
	/*5 -_*/ [ [0,0,0,0,0], [0,0,0,0,0], [0,1,1,0,0], [0,0,1,1,0], [0,0,0,0,0] ],
	/*6 []*/ [ [0,0,0,0,0], [0,1,1,0,0], [0,1,1,0,0], [0,0,0,0,0], [0,0,0,0,0] ]
];

Figure.margins = [0, ,-1];

Figure.colors = [
	'rgb(150,224,89)','rgb( 75,193,213)','rgb(231,96,73)','rgb(255,220,29)',
	'rgb(221,156,216)','rgb( 82,133,238)','rgb(240,134,62)',
]
Figure.colors[-1]=-1;

Figure.area = 5;
Figure.defaultFigureId = 4;
Figure.defaultFigure = Figure.figures[Figure.defaultFigureId];

Figure.prototype.rotate = function(direction)	{
	if(this.figureId == 6) return null; // Box is not rotating
	if(arguments.length == 0)
		direction = 'cw';
	
	var newFigure = new Figure(this);
	newFigure.figure = new Array(Figure.array);
	
	for(var i=0; i<Figure.area; ++i)	{
		newFigure.figure[i] = new Array(Figure.array);
		for(var j=0; j<Figure.area; ++j)	{
			if(direction == 'cw')
				newFigure.figure[i][j] = this.figure[Figure.area - 1 - j][i];
			else if(direction == 'ccw')
				newFigure.figure[i][j] = this.figure[j][Figure.area - 1 - i];
		}
	}

	return newFigure;
}

Figure.prototype.getBlocksToRender = function()	{
	var res = new Array();
	for(var i=0; i<this.figure.length; ++i)
		for(var j=0; j<this.figure[i].length; ++j)
			if(this.figure[i][j] == 1) 
				res.push(Point.sum(this.position, new Point(j,i)));
	return res;
}

Figure.prototype.andNot = function(figure)	{
	var res = new Array();
	for(var i=0; i<this.figure.length; ++i)
		for(var j=0; j<this.figure[i].length; ++j)
			if(this.figure[i][j] == 1 && this.figure[i][j] != figure.figure[i][j]) 
				res.push(Point.sum(this.position, new Point(j,i)));
	return res;
}

Figure.prototype.shifted = function(by)	{
	var res = {draw:new Array(),clear:new Array()}
	for(var i=0; i<this.figure.length; ++i)
		for(var j=0; j<this.figure[i].length; ++j)	{
			var k=j+by;	
		
			if(k>=0 && k < this.figure[i].length)	{
				if(this.figure[i][j] == 1 && this.figure[i][k] == 0)
					res.draw.push(Point.sum(this.position, new Point(k,i)));
			
				else if (this.figure[i][j] == 0 && this.figure[i][k] == 1)
					res.clear.push(Point.sum(this.position, new Point(k,i)));
				
				else if (this.figure[i][j] == 1 && (j==0 || j== this.figure[i].length - 1))
					res.clear.push(Point.sum(this.position, new Point(j,i)));
			}
			else if(this.figure[i][j])
				res.draw.push(Point.sum(this.position, new Point(k,i)));
		}
	return res;
}