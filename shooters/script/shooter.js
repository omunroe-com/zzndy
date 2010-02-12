function Point(x, y)
{
	this.x = x;
	this.y = y;
}

Point.prototype.move = function(r, d)
{
	return new Point(
		this.x - d*Math.sin(r),
		this.y + d*Math.cos(r)
	);
}

function Projectile(shooter, x, y, dir, vel, dam, dist)
{
	Point.call(this, x, y);
	this.dir = dir;
	this.vel = vel;
	this.dam = dam;
	this.dist = dist;
	this.r = 3;
	this.shooter = shooter;
}

Projectile.prototype = new Point;
Projectile.prototype.base = Point.prototype;

Projectile.prototype.move = function()
{
	var pos = this.base.move.call(this, this.dir, this.vel);
	this.x = pos.x;
	this.y = pos.y;

	this.dist -= this.vel;
}

function Shooter(pos, dir, vel, rate, dam, speed, pershot, dist, clip, reload)
{
	this.pos = pos;
	this.dir = dir;
	this.vel = vel;

	this.rate = rate;
	this.dam = dam;
	this.speed = speed;
	this.pershot = pershot;

	this.dist = dist;
	this.clip = clip;
	this.reload = reload;

	this.frame = null;
	this.score = 0;
}

function toDNA(num, max)
{

}

Shooter.prototype.shoot = function()
{
	var pos = this.pos.move(this.dir, 5);
	return [new Projectile(this, pos.x, pos.y, this.dir, this.speed, this.dam, this.dist)];
}

Shooter.prototype.getVector = function()
{
	return new Point(this.x + this.vel * Math.sin(this.dir), this.y + this.vel * Math.cos(this.dir));
}

Shooter.prototype.toShoot = function(frame)
{
	if(this.frame === null || this.frame + this.rate <= frame)
	{
			this.frame = frame;
			return true;
	}
	return false;
}

Shooter.prototype.toDNA = function()
{

}

Shooter.fromDNA = function(dna)
{

}
