function AssiiTetrisView( target ) {
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

var V = AssiiTetrisView.prototype = new TetrisView;
AssiiTetrisView.prototype.base = TetrisView.prototype;

var block = String.fromCharCode(0x2590) + String.fromCharCode(0x2588) + String.fromCharCode(0x258C);
var space = ' ' + String.fromCharCode(0xb7) + ' ';
var shadow = ' = ';

block = '[&equiv;]';
space = ' &middot; ';
shadow = ' &times; ';

function realCells( cell )
{
    if( cell ) return block;
    else return space;
}

function tidy( n, w, p )
{
    return ((p || '') + n.toFixed(w || 0)).pad(-Blk.width * 3);
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

        out.push(cells.join(''));
        out.push('    ');

        if( y < Blk.height ) {
            out.push(c.next.body[y].map(realCells).join(''));
        }
        else
        {
            switch( y ) {
                case Blk.height + 1:
                    out.push(' Score:'.pad(Blk.width * 3));
                    break;
                case Blk.height + 2:
                    out.push(tidy(this.currentScore));
                    break;
                case Blk.height + 3:
                    out.push(this.latestBonus == 0 ? '   '.x(Blk.width) : tidy(this.latestBonus, 0, '+ '));
                    break;
                case Blk.height + 5:
                    out.push(' Speed:'.pad(Blk.width * 3));
                    break;
                case Blk.height + 6:
                    out.push(tidy(c.speed, 1));
                    break;
                default:
                    out.push('   '.x(Blk.width));
            }
        }

        out.push('\n');
    }

    var body = out.join('');
    if( c.state == S_OVER )
    {
        var lines = body.split('\n');
        var l = lines[0].length;
        lines.splice(Math.floor(c.rows / 2) - 1, 3,
                "=  G  A  M  E  =".pad(-(Math.floor(l / 2) - 5)).pad(l),
                lines[Math.floor(c.rows / 2)],
                "=  O  V  E  R  =".pad(-(Math.floor(l / 2) - 5)).pad(l)
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