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
 */
function Bullet(projectile, pos, dir, distance) {
    this.projectile = projectile;
    this.pos = new Point(pos.x, pos.y);
    this.dir = dir;
    this.distance = distance;
    this.flew = 0;
}

Bullet.prototype.move = function(delay) {
    var p = this.pos;
    this.pos = new Point(p.x + this.projectile.speed * sin(this.dir) * delay / 1000, p.y + this.projectile.speed * cos(this.dir) * delay / 1000);

    this.flew += p.to(this.pos);
};

/**
 * Create new gun
 * @param {Projectile} projectile - type of projectile
 * @param {Number} distance   - fired as this maximum distance (in units)
 * @param {Number} rate       - rate of fire (shots per second)
 * @param {Number} clip       - clip size
 * @param {Number} reload     - reload time (milliseconds)
 */
function Gun(projectile, distance, rate, clip, reload) {
    this.projectile = projectile;
    this.distance = distance;
    this.rate = rate;
    this.clip = clip;
    this.reload = reload;

    this.reloadingFor = 0;
    this.loaded = clip;
    this.waitingToShoot = 0;
}

Gun.prototype.clone = function() {
    return new Gun(this.projectile.clone(), this.distance, this.rate, this.clip, this.reload);
};

Gun.prototype.ready = function(delay) {

};

Gun.prototype.shoot = function(delay, pos, dir) {
    if (this.reloadingFor > 0) {
        this.reloadingFor += delay;
        if (this.reloadingFor >= this.reload) {
            this.waitingToShoot = this.reloadingFor - this.reload;
            this.reloadingFor = 0;
            this.loaded = this.clip;
        }
        else {
            return [];
        }
    }
    else {
        this.waitingToShoot += delay
    }

    if (this.waitingToShoot >= 1000 / this.rate) {
        this.waitingToShoot -= 1000 / this.rate;
        if (this.loaded-- <= 0) {
            this.reloadingFor = this.waitingToShoot;
        }
        else {
            return [new Bullet(this.projectile, pos, dir, this.distance)];
        }
    }

    return [];
};
