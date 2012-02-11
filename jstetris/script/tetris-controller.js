var $ = window;

function Beam( i, j, l )
{
    this.i = i;
    this.j = j;
    this.l = l;
}

/**
 * @class TetrisController
 * Keeps track of and manipulates the field and figures.
 *
 * @param {TetrisModel} model      tetris model
 * @param {TetrisView} view        view
 * @param {TetrisRuleSet} ruleset  ruleset
 * @constructor
 */
function TetrisController( model, view, ruleset )
{
    this.model = model;
    this.current = null;
    this.currentPos = null;
    this.next = model.next();
    this.view = view;


    this.defaultCcw = true;
    this.km = new KeyboardAdapter(this);

    this.cols = 9;
    this.rows = 19;
    /**
     * {Array} of Tupples
     */
    this.beam = [];
    this.initialPos = new Point(Math.floor((this.cols - Blk.width) / 2), Math.floor(this.rows + Blk.height / 2));
    this.field = this.rows.times(this.cols.times(0));

    this.speed = 1;
    this.score = new Score(ruleset);

    this.score.addEventListener('score', this, function( e ) {
        this.fireScore(e.newScore, e.bonus);
    });
    this.score.addEventListener('speedup', this, function( e ) {
        console.log('speed += ' + e.speedup);
        this.speed += e.speedup;
        this.fireSpeedup(e.speedup);
    });

    this.fallTimer = new Timer(fall.bind(this), 300);

    this.state = S_NOT_STARTED;

    var events = {};
    events['gameover'] = new Event(this);
    events['score'] = new Event(this);
    events['speedup'] = new Event(this);
    events['burn'] = new Event(this);

    // Evens handling
    this.addEventListener = function( name, handler, method )
    {
        if( name in events )
            events[name].addListener(handler, method);
    };

    this.removeEventListener = function( name, handler, method )
    {
        if( name in events )
            events[name].removeListener(handler, method);
    };

    // events firing
    this.fireGamover = function() {
        events['gameover'].fire(this);
    };

    this.fireScore = function( newScore, lastBonus )
    {
        events['score'].fire(this, {newScore:newScore, bonus:lastBonus});
    };

    this.fireSpeedup = function( speedup )
    {
        events['speedup'].fire(this, {speed:this.speed, speedup:speedup});
    };

    this.fireBurn = function( burnedRows ) {
        events['burn'].fire(this, {burnedRows:burnedRows});
    };
}

var T = TetrisController.prototype;

/**
 * @return {Boolean} - true if controller is not paused or otherwise deactivated.
 */
T.isActive = function()
{
    return this.state == S_RUNNING;
};

/**
 * Rotate current block in default direction or in opposite direction
 * @param {Boolean} backwards  if true rotate in direction opposite to default
 * @return {BlockDiff}  current block rotated difference from previous position
 */
T.rotate = function( backwards )
{
    var dif;
    if( this.defaultCcw ? !backwards : backwards ) {
        dif = this.current.left();
        if( currentFailing.call(this) ) {
            dif = null;
            this.current.right();
        }
    }
    else {
        dif = this.current.right();
        if( currentFailing.call(this) ) {
            dif = null;
            this.current.left();
        }
    }

    if( dif )this.invalidateView();
    return dif;
};

/**
 * Move current figure left.
 */
T.left = function()
{
    --this.currentPos.x;
    if( currentFailing.call(this) )
        ++this.currentPos.x;
    else this.invalidateView();
};

/**
 * Move current figure right.
 */
T.right = function()
{
    ++this.currentPos.x;
    if( currentFailing.call(this) )
        --this.currentPos.x;
    else this.invalidateView();
};

T.lower = function()
{
    var success = true;
    --this.currentPos.y;

    if( currentFailing.call(this) ) {
        ++this.currentPos.y;
        cristalize.call(this);
        success = serveNext.call(this);
    }

    this.invalidateView();
    return success;
};

T.drop = function()
{
    var h = getDropHeight.call(this);
    this.currentPos.y -= h;
    cristalize.call(this);
    this.score.score('drop', h, this.speed);
    this.fallTimer.restart();
    serveNext.call(this);
    this.view.render();
};

T.getBeam = function()
{
    return this.beam;
};

T.start = function()
{
    if( this.state != S_NOT_STARTED )
        throw new Error('Tetris was already started.');

    this.state = S_RUNNING;
    this.view.setController(this);

    serveNext.call(this);
    this.view.render();
    this.fallTimer.run();
    this.score.start();
};

// States
var S_NOT_STARTED = 1, S_RUNNING = 2, S_PAUSED = 4, S_OVER = 8;

T.pause = function()
{
    switch( this.state ) {
        case S_RUNNING:
            this.state = S_PAUSED;
            break;
        case S_PAUSED:
            this.state = S_RUNNING;
            break;
    }
};

function makeBeam()
{
    var beam = [];
    if( this.state != S_OVER )
    {
        var b = this.current.body;

        var j = -1, m = Blk.width;
        while( ++j < m )
        {
            var i = Blk.height;
            while( --i >= 0 )
            {
                if( b[i][j] )
                {
                    var fj = this.currentPos.x + j;
                    var fi = this.currentPos.y - i;

                    var gap = getGap.call(this, fi, fj);
                    beam.push(new Beam(fi - gap, fj, gap));
                    break;
                }
            }
        }
    }

    return beam;
}

function serveNext()
{
    this.current = this.next;

    this.currentPos = new Point(this.initialPos);
    this.beam = [];
    if( currentFailing.call(this) )
    {
        gameOver.call(this);
        return false;
    }

    this.beam = makeBeam.call(this);
    this.next = this.model.next();
    return true;
}

function gameOver()
{
    this.score.stop();
    this.fallTimer.stop();
    this.state = S_OVER;
    this.beam = [];

    this.fireGamover();
}

/**
 * Check if cell on given position is occupied.
 * @param {Number} x  column coordinate to check
 * @param {Number} y  row coordinate to check
 * @return {Boolean}
 */
function cellOccupied( x, y )
{
    return x < 0 || x >= this.cols || y < 0 || !(y >= this.rows || !this.field[y][x]);
}

function currentFailing()
{
    var rowNo = 0, self = this;

    function cellNotOk( cell, i )
    {
        return cell && cellOccupied.call(self, self.currentPos.x + i, self.currentPos.y - rowNo);
    }

    function rowNotOk( row, i )
    {
        rowNo = i;
        return row.some(cellNotOk);
    }

    return this.current.body.some(rowNotOk);
}

function getGap( i, j )
{
    var y = i;
    while( y > 0 && (y >= this.rows || !this.field[y - 1][j]) ) --y;
    return i - y;
}

/**
 * Calculate height for dropping current figure.
 */
function getDropHeight()
{
    var minheight = 10e6;
    var j = Blk.width;
    while( --j >= 0 )
    {
        var i = Blk.height;
        while( --i >= 0 ) {
            if( this.current.body[i][j] )
            {
                var fj = this.currentPos.x + j;
                var fi = this.currentPos.y - i;

                var d = getGap.call(this, fi, fj);

                minheight = Math.min(minheight, d);
            }
        }
    }

    return minheight;
}

// TetrisController private
function fall()
{
    if( this.lower() )
        this.fallTimer.restart();
}

/**
 * Add current figure to field's solid blocks
 */
function cristalize()
{
    var rowNo = 0, it = this;

    function saveCell( prev, cell, i )
    {
        var x = it.currentPos.x + i, y = it.currentPos.y - rowNo;
        if( y < it.rows && cell ) it.field[y][x] = cell;
        return prev || cell;
    }

    function saveRow( ids, row, i )
    {
        rowNo = i;
        if( row.reduce(saveCell, false) ) {
            ids.push(it.currentPos.y - rowNo);
        }
        return ids;
    }

    function cellIsOn( cell )
    {
        return !!cell;
    }

    function rowIsFull( row )
    {
        return !!it.field[row] && it.field[row].every(cellIsOn);
    }

    var burnedRows = this.current.body.reduce(saveRow, []).filter(rowIsFull).sort();
    if( burnedRows.length > 0 ) {
        var i = burnedRows[0] - 1, d = 1, j = -1, empty = false;
        while( ++i < this.rows ) {
            if( burnedRows.indexOf(i + d) != -1 )
                ++d;

            j = -1;
            empty = true;
            while( ++j < this.cols )
            {
                empty &= ! this.field[i][j];
                this.field[i][j] = (i + d < this.rows) ? this.field[i + d][j] : null;
            }
            if( empty )break;
        }

        this.fireBurn(burnedRows);
        this.score.score('burn' + burnedRows.length, burnedRows.length, this.speed);
    }
}

T.invalidateView = function()
{
    this.beam = makeBeam.call(this);
    if( this.view )this.view.render();
};

(function( $ ) {
    /**
     * @constructor
     * @param {TetrisController} ctrl  controller to pass events to
     */
    $.KeyboardAdapter = function( ctrl )
    {
        this.ctrl = ctrl;
        this.source = window;
        this.bind();
    };

    var K = KeyboardAdapter.prototype;

    K.bind = function()
    {
        if( this.source != null && this.source.addEventListener ) {
            this.source.addEventListener('keydown', this.onkeydown.detach(this), false);
            this.source.addEventListener('keypress', this.onkeypress.detach(this), false);
        }
    };

    K.unbind = function()
    {
        if( this.source != null && this.source.removeEventListener ) {
            this.source.removeEventListener('keydown', this.onkeydown.detach(this), false);
            this.source.removeEventListener('keypress', this.onkeypress.detach(this), false);
        }
        this.source = null;
    };

    var KEY_left = 37, KEY_up = 38, KEY_right = 39, KEY_down = 40, KEY_a = 97, KEY_w = 119, KEY_d = 100
            , KEY_s = 115, KEY_h = 104, KEY_j = 106, KEY_k = 107, KEY_l = 108, KEY_W = 87, KEY_J = 74
            , KEY_p = 112, KEY_P = 80, KEY_cr = 13, KEY_space = 32, KEY_pause = 19;

    /**
     * Handle arrow key events
     * @param {Event} event
     */
    K.onkeydown = function( event )
    {
        if( this.ctrl.isActive() )
            with( this.ctrl )
                switch( event.keyCode ) {
                    case KEY_cr:drop();break;
                    case KEY_up:rotate(event.ctrlKey);break;
                    case KEY_left:left();break;
                    case KEY_right:right();break;
                    case KEY_down:lower();break;
                    case KEY_pause:pause();break;
                }
        else if( event.keyCode == KEY_pause )
            this.ctrl.pause();
    };

    /**
     * Handle letter key events
     * @param {Event} event
     */
    K.onkeypress = function( event ) {
        if( this.ctrl.isActive() )
            with( this.ctrl )
                switch( event.charCode ) {
                    case KEY_space:drop();break;
                    case KEY_w: case KEY_j:rotate(false);break;
                    case KEY_W: case KEY_J:rotate(true);break;
                    case KEY_a: case KEY_h:left();break;
                    case KEY_d: case KEY_l:right();break;
                    case KEY_s: case KEY_k:lower();break;
                    case KEY_p: case KEY_P:pause();break;
                }
        else if( event.charCode == KEY_p || event.charCode == KEY_P )
            this.ctrl.pause();
    };
})(window);