
var NetWalkLevel = { Easy : 1, Medium : 2, Hard : 3 }
var NetWalkState = { Up : 0x1, Left : 0x2, Down : 0x4, Right : 0x8, Server : 0x10}

function NetWalk(level)
{
	this.level = level;
	this.cells = [];
	for(var i=0; i<9; ++i)	{
		this.cells.push([]);
		for(var j=0; j<9; ++j)	{
			var cell = Math.floor(Math.random() * 32);
			this.cells[i].push((i*9+j)%32);
		}
	}
}
