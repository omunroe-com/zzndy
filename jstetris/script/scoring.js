/**
 * @class Action
 * Scorable action within the game.
 *
 * @constructor
 * @param {Number} score     amount of score given for achieving this action
 * @param {Boolean} perline  if true, the score is given for each line scored
 */
function Action( score, perline ) {
    this.points = parseInt(score);
    this.perline = !!perline;
}

/**
 * Return scored points
 * @param {Number} lines  optional number of lines scored
 * @return {Number}       score earned for this action
 */
Action.prototype.score = function( lines ) {
    if ( !this.perline )
        return this.points;

    lines = lines || 1
    return this.points * lines;
}

/**
 * @class Record
 * A line in top scores table.
 *
 * @constructor
 * @param {String} nick   player name or string representation of another record object
 * @param {Number} score  score
 * @param {Number} time   timestamp specifiing where the score was acheived
 */
function Record( nick, score, time ) {
    if ( arguments.length == 1 ) {
        var parts = nick.split('/');
        score = parts[0];
        nich = parts[1];
        time = parts[2];
    }

    this.nick = nick.replace('/', '-').replace(';', ',');
    this.score = parseInt(score) || 0;

    this.time = parseInt(time) || Date.now();
}

Record.prototype.toString = function() {
    return this.score + '/' + this.nick + '/' + this.time;
}

/**
 * Construct Record from a string. Used in map.
 * @param {String} str  string representation of a Record
 * @return {Record}
 */
Record.factory = function( str ) {
    return str && new Record(str);
}

Record.compare = function( a, b ) {
    if ( !(a instanceof Record && b instanceof Record) )
        return 0;
    if ( a.score < b.score )
        return -1;
    if ( a.score > b.score )
        return 1;
    return 0;
}

/**
 * @class Score
 * Number of points of score earned and rules of scoring.
 * @constructor
 */
function Score() {
    this.points = 0;
    this.scoring = Score.speedyScoring;
    this.onScore = function( score ) {
    } // callback
}

Score.prototype.score = function( speed, action, lines ) {
    var bonus;
    this.points += bonus = parseInt(this.scoring[action].score(lines) * this.scoring.escalation(speed));
    this.onScore(this.points, bonus);
    return bonus;
}

/**
 * Classic score rules. Most impartant, speed does not affect scoring.
 */
Score.classicScoring = {
    drop: new Action(0),
    burn1: new Action(100),
    burn2: new Action(200),
    burn3: new Action(500),
    burn4: new Action(1500),
    lower: new Action(0),
    escalation: function( speed ) {
        return 1
    }
}

/**
 * Speed rules. Dropping figures is very lucrative, scores assigned for action grow with speed.
 */
Score.speedyScoring = {
    drop: new Action(7, true),
    burn1: new Action(6),
    burn2: new Action(15),
    burn3: new Action(50),
    burn4: new Action(200),
    lower: new Action(1), // just placing a figure is now rewarded ;)
    escalation: function( speed ) {
        return Math.pow(2, (parseFloat(speed - 1)) / 2)
    }
}

/**
 * @class TopScores
 * Top scores table, saved in cookies.
 *
 * @constructor
 * @param {Object} current
 * @param {Object} cookie
 */
function TopScores( current, cookie ) {
    this.current = new Record('Anonymvs', current);
    this.cookie = cookie;
    this.localPos = -1;
    this.__local = null;
}

TopScores.prototype.loadScores = function() {
    var scores = Cookie.get(this.cookie);
    scores = scores && scores.split(';').map(Record.factory) || [];
    if ( this.current.score && scores.push(this.current) )
        scores = scores.sort(Record.compare).reverse().slice(0, 10);
    this.localPos = scores.indexOf(this.current);
    this.__local = scores;
}

TopScores.prototype.save = function() {
    if ( this.__local )
        Cookie.set(this.cookie, this.__local.join(';'));
}

TopScores.prototype.clear = function() {
    Cookie.set(this.cookie, '');
    this.__local = null;
}

TopScores.prototype.__defineGetter__('local', function() {
    if ( !this.__local )
        this.loadScores();

    return this.__local;
});
