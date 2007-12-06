//// Action class ////////////////////////////////////////////////////////////

function Action(name, score, perline)	{
	this.name = name;
	this.points = parseInt(score);
	this.perline = !!perline;
}

Action.prototype.score = function(lines)	{
	if(this.perline)	{
		if(arguments.length == 0)
			lines = 1;	
			
		return this.points * lines;
	}
	else
		return this.points;
}

function Record(nick, score, time)
{
	if(arguments.length == 1)
		[score,nick,time] = nick.split('/');
	
	this.nick = nick.replace('/', '-').replace(';', ',');
	this.score = parseInt(score);
	this.time = new Date(time);
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

//// Score class /////////////////////////////////////////////////////////////

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
	drop:		new Action('drop', 5, true),
	lineout1:	new Action('1lineout',  5),
	lineout2:	new Action('2lineout', 12),
	lineout3:	new Action('3lineout', 29),
	lineout4:	new Action('4lineout', 70),
	lower:		new Action('lower', 0),
}
