//// Action class ////////////////////////////////////////////////////////////

function Action(score, perline)	{
	this.points = parseInt(score);
	this.perline = !!perline;
}

Action.prototype.score = function(lines)	{
	if(!this.perline)
		return this.points;

	lines = lines || 1
	return this.points * lines;
}

////    CLASS RECORD    ///////////////////////////////////////////////////////
// represent one top scores record
function Record(nick, score, time)
{
	if(arguments.length == 1)
		[score,nick,time] = nick.split('/');
	
	this.nick = nick.replace('/', '-').replace(';', ',');
	this.score = parseInt(score) || 0;
	this.time = new Date(time) && new Date();
}

Record.prototype.toString = function()	{
	return this.score + '/' + this.nick + '/' + this.time.getTime();
}

Record.compare = function(a, b)	{
	if(!(a instanceof Record && b instanceof Record)) return 0;
	if(a.score < b.score) return -1;
	if(a.score > b.score) return 1;
	return 0;
}

////    Score class    ////////////////////////////////////////////////////////

function getEscalation(speed)   {
    return Math.pow(2, (parseFloat(speed)) / 3.0)
}

function Score()	{
	this.points = 0;
	this.onScore = function(score){} // callback
}

Score.prototype.score = function(speed, action, lines)	{
	var bonus;
	this.points += bonus = parseInt(action.score(lines) * getEscalation(speed))
	this.onScore(this.points, bonus)
}

Score.action = {
	drop:		new Action(5, true),
	lineout1:	new Action(5),
	lineout2:	new Action(12),
	lineout3:	new Action(29),
	lineout4:	new Action(70),
	lower:		new Action(0) // just placing a figure is not rewarded
}

/// class TopScores

function TopScores(current, cookie)	{
	this.current = new Record('Anonymvs', current);
	this.cookie = cookie;
	this.localPos = -1;
	this.globalPos = -1;
	this.__local = null;
	this.__global = null;
}

TopScores.prototype.__defineGetter__('local', function()	{
	if(this.__local)
		return this.__local;
	
	var scores = Cookie.get(this.cookie);
	if(!scores)	{
		this.__local = [this.current];
		return this.__local;
	}
	
	return this.__local;
});

TopScores.prototype.__defineGetter__('global', function()	{
	if(this.__global)
		return this.__global;
	
	return this.__global;
});

