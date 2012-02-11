	
Direction = {
	  Up        : 0x01
	, Right     : 0x02
	, DownRight : 0x04
	, Down      : 0x08
	, Left      : 0x10
	, UpLeft    : 0x20
}

function getFollowinfPoint(point, dir, i)	{
	i = i || 1
	return new Point(
		point.x + (dir == Direction.Left  || dir == Direction.UpLeft
			? -i : dir == Direction.Right || dir == Direction.DownRight ? i : 0),
		point.y + (dir == Direction.Up    || dir == Direction.UpLeft
			? -i : dir == Direction.Down  || dir == Direction.DownRight ? i : 0)
	)
}

Snake = function(head, dir, length)	{
	this.dir = this.nextDir = dir
	var body = [new Point(head)]
	
	for(var i=1; i<length; ++i)
		body.push(getFollowinfPoint(body[i-1], dir, -i))

	this.__defineGetter__('body', function(){return body.clone()})
	this.__defineGetter__('head', function(){return new Point(body[0])})
	
	this.move = function()
	{
		this.dir = this.nextDir
		var head = getFollowinfPoint(body[0], this.dir)
		
		body.splice(0, 0, head)
		var newBody = body.splice(0, body.length-1)
		var tail = body
		body = newBody
		return tail
	}
}

Snake.prototype.turnLeft = function()
{
	this.nextDir = this.dir >> 1
	if(this.nextDir < Direction.Up) this.nextDir = Direction.UpLeft
	
	return this.nextDir
}

Snake.prototype.turnRight = function()
{
	this.nextDir = this.dir << 1
	if(this.nextDir > Direction.UpLeft) this.nextDir = Direction.Up;
	
	return this.nextDir
}