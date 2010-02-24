function Point(x, y)
{
	this.x = x;
	this.y = y;
}

Point.prototype.add = function(vec)
{
	return new Point(this.x + vec.x, this.y + vec.y);
}

Point.prototype.to = function(p)
{
	var dx = this.x - p.x;
	var dy = this.y - p.y;
	return Math.sqrt(dx*dx + dy*dy);
}

Point.prototype.toString = function()
{
	return ['(', this.x.toFixed(2), ', ', this.y.toFixed(2), ')'].join('');
}

function Vector(x, y)
{
	Point.call(this, x, y);
}

Vector.prototype = new Point;
Vector.prototype.base = Point.prototype;

Vector.prototype.getDir = function()
{
	return Math.atan2(this.y, this.x);
}

Vector.prototype.mul = function(num)
{
	return new Vector(this.x * num, this.y * num);
}

Vector.prototype.abs = function()
{
	return Math.sqrt(this.x*this.x + this.y*this.y);
}

Vector.prototype.norm = function()
{
	var d = this.abs();
	return new Vector(this.x / d, this.y / d);
}

function Projectile(pos, mov, dam, dist)
{
	Movable.call(this, pos, mov);
	this.maxDist = dist;
	this.dist = 0;
	this.dam = dam;
	this.dead = 0;
}

function Movable(pos, mov)
{
	this.pos = new Point(pos.x, pos.y);
	this.mov = new Vector(mov.x, mov.y);
}

Movable.prototype.advance = function()
{
	this.pos = this.pos.add(this.mov);
}

Projectile.prototype = new Movable(new Point(0, 0), new Vector(0, 0));
Projectile.prototype.base = Movable.prototype;

Projectile.prototype.advance = function()
{
	if(this.dead) return;

	var p1 = this.pos;
	this.base.advance.call(this);

	this.dist += p1.to(this.pos);

	if(this.dist >= this.maxDist) this.dead = 100;
}


/**
 * @class Shooter
 *
 * @param {Point} pos - shooter initial position
 * @param {Vector} mov - movement vector
 * @param {Number} hp - health points
 * @param {Number} rate - rate of fire - time in milliseconds between shots
 * @param {Number} dam - projectile damage
 * @param {Number} speed - projectile speed
 * @param {Number} dist - projectile max distance
 * @param {Number} clip - clip size (in shots)
 * @param {Number} reload - reload time (milliseconds)
 * @param {Number} spread - projectiles released per shot
 */
function Shooter(pos, mov, hp, rate, dam, speed, dist, clip, reload, spread)
{
	if(pos instanceof Shooter)
	{
		Movable.call(this, pos.pos, pos.mov);

		this.hp = pos.hp;
		this.rate = pos.rate;
		this.dam = pos.dam;
		this.speed = pos.speed;
		this.dist = pos.dist;
		this.clip = pos.clip;
		this.reload = pos.reload;
		this.spread = pos.spread;
		this.lastShot = pos.lastShot;
	}
	else
	{
		Movable.call(this, pos, mov);

		this.hp = hp;
		this.rate = rate;
		this.dam = dam;
		this.speed = speed;
		this.dist = dist;
		this.clip = clip;
		this.reload = reload;
		this.spread = spread;

		this.lastShot = 0;
	}

	this.inClip = this.clip;
	this.reloading = false;

}

Shooter.prototype = new Movable(new Point(0, 0), new Vector(0, 0));
Shooter.prototype.base = Movable.prototype;

Shooter.prototype.steer = function(rad)
{
	var c = Math.cos(rad);
	var s = Math.sin(rad);
	var p = this.mov;

	this.mov = new Vector(p.x * c - p.y * s, p.y * c + p.x * s);
}

Shooter.prototype.readyToShoot = function(ts)
{
	var ok = false;
	if(this.reloading)
	{
		ok =  (ts - this.lastShot) > this.reload;
		if(ok){
			this.reloading = false;
			this.inClip = this.clip;
		}
	}
	else
	{
		ok = (ts - this.lastShot) >= this.rate;
	}

	return ok;
}

Shooter.prototype.shoot = function(ts)
{
	if(this.reloading)return [];
	if(--this.inClip == 0)
	{
		this.reloading = true;
	}

	this.lastShot = ts;
	return [new Projectile(this.pos, this.mov.norm().mul(this.speed), this.dam, this.dist)];
}

