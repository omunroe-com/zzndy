function AsciiTetrisView( target ) {
    TetrisView.call(this);

    this.target = target;
    this.latest = 0;
    this.targetScore = 0;
    this.currentScore = 0;
    this.latestBonus = 0;
    this.scoreLife = 0;
    this.scoreTimer = new Timer(updScore.bind(this), 100);
    this.scoreTimer.run();
}

function updScore()
{
    if( this.currentScore != this.targetScore )
        this.currentScore += (this.targetScore - this.currentScore) / 6;

    if( this.latestBonus != 0 )
        if( ++this.scoreLife > 4 )
            this.latestBonus = 0;

    this.scoreTimer.restart();
}

var V = AsciiTetrisView.prototype = new TetrisView;
AsciiTetrisView.prototype.base = TetrisView.prototype;

var block = '[+]';
var space = ' . ';
var shadow = ' ~ ';
var bar = ' * '
var cellWidth = block.length;
var emptyCell = ' '.x(cellWidth);


function realCells( cell )
{
    if( cell ) return block;
    else return space;
}

function tidy( n, w, p )
{
    return ((p || '') + n.toFixed(w || 0)).pad(-Blk.width * cellWidth);
}

V.render = function()
{
    var c = this.controller, out = [];

    var y = -1, n = c.rows, m = c.cols, cells;
    var sh = c.getBeam();
    // for each row (from bottom to the top)
    while( ++y < n ) {
        // get y coordinate (with zero on the top side)
        var i = c.rows - 1 - y;

        cells = m.times(space);
        // for each column
        var x = -1;
        while( ++x < m )
        {
            if( c.field[i][x] ) cells[x] = block;
            else if( sh.some(function( el ) {
                return el.j == x && i >= el.i && i <= el.i + el.l;
            }) )
                cells[x] = shadow;

            // get coordinates within block
            var by = c.currentPos.y - i, bx = x - c.currentPos.x;

            if( bx >= 0 && bx < Blk.width && by >= 0 && by < Blk.height )
                if( c.current.body[by][bx] )cells[x] = block;
        }

        out.push(bar + cells.join(''));
        out.push(bar + emptyCell);

        if( y < Blk.height ) {
            out.push(c.next.body[y].map(realCells).join(''));
        }
        else
        {
            switch( y ) {
                case Blk.height + 1:
                    out.push(' Score:'.pad(Blk.width * cellWidth));
                    break;
                case Blk.height + 2:
                    out.push(tidy(this.currentScore));
                    break;
                case Blk.height + 3:
                    out.push(this.latestBonus == 0 ? emptyCell.x(Blk.width) : tidy(this.latestBonus, 0, '+ '));
                    break;
                case Blk.height + 5:
                    out.push(' Speed:'.pad(Blk.width * cellWidth));
                    break;
                case Blk.height + 6:
                    out.push(tidy(c.speed, 1));
                    break;
                default:
                    out.push(emptyCell.x(Blk.width));
		    break;
            }
        }

        out.push('\n');
    }

	out.push(bar.x(m+2));
	out.push(emptyCell.x(Blk.width+1)+'\n');
	
    var body = out.join('');
    if( c.state == S_OVER )
    {
        var lines = body.split('\n');
		var l1 = "=  G  A  M  E  ="; 
		var l2 = "=  O  V  E  R  =";
					
        var l = lines[0].length;
		
		var a = lines[Math.floor(c.rows / 2) - 1].split('');
		a.splice(Math.floor(((m+2)*cellWidth-l1.length)/2), l1.length, l1);
		
		var b = lines[Math.floor(c.rows / 2) - 1].split('');
		b.splice(Math.floor(((m+2)*cellWidth-l1.length)/2), l2.length, l2);
		
        lines.splice(Math.floor(c.rows / 2) - 1, 3,			
				a.join(''),
		        lines[Math.floor(c.rows / 2)],
                b.join('')
                );
        body = lines.join('\n');
    }
    this.target.innerHTML = body;
};

V.onScore = function( e )
{
    this.targetScore = e.newScore;

    if( this.scoreLife == 0 )
        this.latestBonus += e.bonus;
    else
        this.latestBonus = e.bonus;

    this.scoreLife = 0;
    this.render();
};
