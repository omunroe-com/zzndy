/**
 * Create new projectile
 * @param {Number} damage   - damage dealt in hp points
 * @param {Number} speed    - flying speed
 */
function Projectile(damage, speed) {
    this.damage = damage;
    this.speed = speed;
}

Projectile.prototype.clone = function() {
    return new Projectile(this.damage, this.speed);
};

/**
 * Create new bullet - instance of projectile
 * @param {Projectile} projectile - type of projectile
 * @param {Point} pos   - current bullet position
 * @param {Number} dir  - bullet direction
 * @param {Number} spread - bullet velocity spread
 */
function Bullet(projectile, pos, dir, distance, spread) {
    this.projectile = projectile;
    this.pos = new Point(pos.x, pos.y);
    this.dir = dir;
    this.distance = distance.dev(spread/100, true);
    this.flew = 0;
	this.speed = projectile.speed.dev(spread / 100, true);
}

Bullet.prototype.move = function(delay) {
    var p = this.pos;
    this.pos = new Point(p.x + this.speed * sin(this.dir) * delay / 1000, p.y + this.speed * cos(this.dir) * delay / 1000);

    this.flew += p.to(this.pos);
};

/**
 * Create new gun
 * @param {Projectile} projectile - type of projectile
 * @param {Number} distance   - fired as this maximum distance (in units)
 * @param {Number} rate       - rate of fire (shots per second)
 * @param {Number} clip       - clip size
 * @param {Number} reload     - reload time (milliseconds)
 * @param {Number} spreadPct  - projectile spread in % (100% = 45°) - affect both speed and direction
 */
function Gun(projectile, distance, rate, clip, reload, spreadPct) {
    this.projectile = projectile;
    this.distance = distance;
    this.rate = rate;
    this.clip = clip;
    this.reload = reload;
	this.spread = Math.atan(spreadPct / 100);
	this.spreadPct = spreadPct
	
    this.reloadingFor = 0;
    this.loaded = clip;
    this.waitingToShoot = 0;
	
	this.charge = 0;
	this.isReloading = false;
	
	this.clone = function(){return new Gun(projectile, distance, rate, clip, reload, spreadPct)}
}

Gun.prototype.ready = function(delay) {

};

Gun.prototype.shoot = function(delay, pos, dir) {
	this.charge += delay;
	var bullets = [];
	
	while(this.charge >= 0)
	{
		var bullet = new Bullet(this.projectile, pos, dir.dev(this.spread), this.distance, this.spreadPct);
		bullet.move(this.charge);
		bullets.push(bullet);
		
		this.charge -= 1000 / this.rate;
		
		if(this.loaded--<0){
			this.loaded = this.clip;		
			this.charge -= this.reload;		
		}
	}
	
	return bullets;
};
