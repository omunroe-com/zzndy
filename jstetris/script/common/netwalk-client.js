function NetWalkManager(game)
{
	this.game = game;
}

NetWalkManager.prototype.getCell = function(i, j)
{
	return this.game.cells[i][j];
}

function NetWalkTableView(manager)
{
	this.manager = manager;
}

NetWalkTableView.prototype.render = function(element)	{
	var rows = [$tr($th({'colspan':9}, '&#x2756; NetWalk &#x2756;'))];

	for(var i=0; i<9; ++i)	{
		var cells = [];

		for(var j=0; j<9; ++j)	{
			cells.push($td({'class':'cell-'+this.manager.getCell(i, j)}));
		}

		rows.push($tr(cells));
	}
	console.log(rows);
	console.log('here it is');
	var table = $table({'class':'netwalk'}, rows);
	element.innerHTML = "";
	element.appendChild(table);
}
