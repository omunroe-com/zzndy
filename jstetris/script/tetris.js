var $ = this;
//(function( $ ) {
/**
 * @class TetrisModel - base class for tetris models
 * Provides new figures to controller.
 *
 * @constructor
 */
$.TetrisModel = function()
{
    this.figures = [
        [ [0,0,0,0,0], [0,1,1,0,0], [0,1,1,0,0], [0,0,0,0,0], [0,0,0,0,0] ],
        [ [0,0,1,0,0], [0,0,1,0,0], [0,0,1,0,0], [0,0,1,0,0], [0,0,0,0,0] ],
        [ [0,0,0,0,0], [0,0,1,1,0], [0,0,1,0,0], [0,0,1,0,0], [0,0,0,0,0] ],
        [ [0,0,0,0,0], [0,1,1,0,0], [0,0,1,0,0], [0,0,1,0,0], [0,0,0,0,0] ],
        [ [0,0,0,0,0], [0,0,0,0,0], [0,1,1,1,0], [0,0,1,0,0], [0,0,0,0,0] ],
        [ [0,0,0,0,0], [0,0,0,0,0], [0,0,1,1,0], [0,1,1,0,0], [0,0,0,0,0] ],
        [ [0,0,0,0,0], [0,0,0,0,0], [0,1,1,0,0], [0,0,1,1,0], [0,0,0,0,0] ]
    ];

    this.tray = [];
};

var M = $.TetrisModel.prototype;
/**
 * next - Get next figure
 * @return {Blk}  next block.
 */
M.next = function() {
    if( !this.tray.length > 0 )
        this.tray = range(this.figures.length).shuffle();

    var index = this.tray.pop();
    return new Blk(this.figures[index], index == 0);
};

/**
 * @class ColoredTetrisModel
 * Serve colored blocks.
 *
 * @param colors
 */
function ColoredTetrisModel( colors )
{
    TetrisModel.call(this);

    if( colors.length < this.figures.length )
        throw new Error('At least ' + this.figures.length + ' colors must be provided.');
    this.colors = colors;
}
var C = ColoredTetrisModel.prototype = new TetrisModel;
C.base = TetrisModel.prototype;

C.next = function()
{
    var id = Math.floor(Math.random() * this.figures.length);
    var fig = this.figures[id];
    var col = this.colors[id].deviate(25, 5, 5, 5, 0);
    for( var i = 0; i < 5; ++i )
        for( var j = 0; j < 5; ++j )
            if( fig[i][j] ) fig[i][j] = col;

    return new Blk(fig);
};

/**
 * @class TetrisView
 * Visualize controller.
 *
 * @constructor
 */
$.TetrisView = function()
{
    this.controller = null;
};

var V = TetrisView.prototype;

V.render = function()
{
    throw new Error('TetrisView is abstract');
};

V.setController = function( controller )
{
    this.controller = controller;

    controller.addEventListener('gameover', this, V.onGameOver);
    controller.addEventListener('score', this, V.onScore);
    controller.addEventListener('speedup', this, V.onSpeedUp);
    controller.addEventListener('burn', this, V.onBurn);
};

V.onScore = function( e )
{
    var score = e.newScore;
    var bonus = e.bonus;
    this.render();
};

V.onSpeedUp = function( e )
{
    var speed = e.speed;
    var speedup = e.speedup;
    this.render();
};

V.onGameOver = function()
{
    this.render();
};

V.onBurn = function( e )
{
    this.render();
};

/**
 * @constructor
 * @param {Number} points    points awarded for this event
 * @param {Boolean} perline  if true, points are givent on per line basis
 */
$.TetrisEvent = function( points, perline )
{
    this.points = points;
    this.perline = !!perline;
};

$.TetrisEvent.prototype.score = function( lines )
{
    return this.points * (this.perline ? lines : 1);
};

$.TetrisRuleSet = function()
{
    this.drop = null;
    this.burn1 = null;
    this.burn2 = null;
    this.burn3 = null;
    this.burn4 = null;
    this.burn5 = null;
    this.sink = null;

    /**
     * During speedup increase speed by this amount.
     */
    this.speedup = 0;
    /**
     * Increase speed each {timeSpeedup} milliseconds.
     */
    this.timeSpeedup = 0;
    /**
     * Increase speed each {scoreSpeedup} points.
     */
    this.scoreSpeedup = 0;
};

var R = $.TetrisRuleSet.prototype;

/**
 * Return speed score multiplyer.
 * @param {Number} speed  current speed
 * @return {Number}       multiplier
 */
R.k = function( speed )
{
    throw new Error('TetrisRuleSet is abstract.');
};

/**
 * Classic score rules. Speed does not affect scoring, score is given only for burning
 * lines out.
 */
$.ClassicRuleSet = function()
{
    TetrisRuleSet.call(this);

    this.drop = new TetrisEvent(0);
    this.burn1 = new TetrisEvent(100);
    this.burn2 = new TetrisEvent(200);
    this.burn3 = new TetrisEvent(500);
    this.burn4 = new TetrisEvent(1500);
    this.burn5 = new TetrisEvent(4000);
    this.lower = new TetrisEvent(0);

    this.speedup = 1;
    this.scoreSpeedup = 10000;
};

$.ClassicRuleSet.prototype = new $.TetrisRuleSet;
$.ClassicRuleSet.prototype.base = $.TetrisRuleSet.prototype;

$.ClassicRuleSet.prototype.k = function()
{
    return 1;
};

/**
 * Speedplay rules. Dropping figures is very lucrative, scores assigned for action grow with speed.
 */
$.SpeedyRuleSet = function()
{
    TetrisRuleSet.call(this);

    this.drop = new TetrisEvent(7, true);
    this.burn1 = new TetrisEvent(6);
    this.burn2 = new TetrisEvent(15);
    this.burn3 = new TetrisEvent(50);
    this.burn4 = new TetrisEvent(200);
    this.burn5 = new TetrisEvent(1000);
    this.lower = new TetrisEvent(1);

    this.speedup = .1;
    this.timeSpeedup = 20000; // 2s
};

$.SpeedyRuleSet.prototype = new $.TetrisRuleSet;
$.SpeedyRuleSet.prototype.base = $.TetrisRuleSet.prototype;

$.SpeedyRuleSet.prototype.k = function( speed ) {
    return Math.pow(2, (parseFloat(speed - 1)) / 2);
};

/**
 * @constructor
 * @param {TetrisRuleSet} ruleset
 */
$.Score = function( ruleset )
{
    this.points = 0;
    this.ruleset = ruleset;

    var events = {};
    events['score'] = new Event(this);
    events['speedup'] = new Event(this);

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
    this.fireScore = function( newScore, lastBonus )
    {
        events['score'].fire(this, {newScore:newScore, bonus:lastBonus});
    };

    this.fireSpeedup = function( speedup )
    {
        events['speedup'].fire(this, {speed:this.speed, speedup:speedup});
    };

    this.interval = null;
};
var S = $.Score.prototype;
S.start = function()
{
    if( !this.interval && this.ruleset.timeSpeedup ) {
        var it = this;
        var method = function() {
            it.fireSpeedup(it.ruleset.speedup);
            it.interval = window.setTimeout(method, it.ruleset.timeSpeedup);
        };
        this.interval = window.setTimeout(method, this.ruleset.timeSpeedup);
    }
};

S.stop = function()
{
    window.clearTimeout(this.interval);
    this.interval = null;
};

/**
 * Add points for an event.
 * @param {TetrisEvent} or {String} event
 * @param {Number} n
 * @param {Number} speed
 * @return {Number} Latest current score
 */
S.score = function( event, n, speed )
{
    if( typeof event == 'string' )
        if( event in this.ruleset && this.ruleset[event] instanceof TetrisEvent )
            event = this.ruleset[event];
        else
            throw new Error("Unknown event " + event);

    var bonus = event.score(n) * this.ruleset.k(speed);
    if( bonus ) {
        var oldScore = this.points;
        this.points += bonus;

        this.fireScore(this.points, bonus);

        if( this.ruleset.scoreSpeedup && oldScore > 0 )
            with( this.ruleset )
                if( Math.floor(oldScore / scoreSpeedup) != Math.floor(this.points / scoreSpeedup) )
                    this.fireSpeedup(speedup);
    }
    return this.points;
};
//})(window);