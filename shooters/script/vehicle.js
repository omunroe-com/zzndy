/**
 * Create new point
 * @param {Number} x
 * @param {Number} y
 */
function Point(x, y) {
    this.x = x;
    this.y = y;
}

Point.prototype.to = function(p) {
    var dx = this.x - p.x;
    var dy = this.y - p.y;
    return Math.sqrt(dx * dx + dy * dy);
};

Point.prototype.toString = function() {
    return '( ' + this.x.toFixed(2) + ', ' + this.y.toFixed(2) + ')';
};

var sin = Math.sin;
var cos = Math.cos;
function rnd(n) {
    return Math.random() * n;
}

/**
 * Create new vehicle.
 * @param pos   - position in units from center (positive direction in north and east)
 * @param dir   - direction in radians (from north clockwise)
 * @param mass  - mass
 * @param speed - speed in units per second
 */
function Vehicle(pos, dir, mass, speed) {
    this.pos = new Point(pos.x, pos.y);
    this.dir = dir;
    this.mass = mass;
    this.speed = speed;

    this.sd = rnd(100) < 50 ? 1 : -1; 
    this.sa = rnd(.2);
}

Vehicle.prototype.move = function(delay) {
    this.pos.x = this.pos.x + this.speed * sin(this.dir) * delay / 1000;
    this.pos.y = this.pos.y + this.speed * cos(this.dir) * delay / 1000;
};

Vehicle.prototype.steer = function(delay) {
    this.dir += this.sd * this.sa;
    if(rnd(100) < 20) 
	    this.sd = rnd(100) < 50 ? 1 : -1; 
};

Vehicle.prototype.clone = function() {
    return new Vehicle(new Point(this.pos.x, this.pos.y), this.dir, this.mass, this.speed);
};

Vehicle.prototype.isHitBy = function(bullet) {
    return this.pos.to(bullet.pos) < this.mass;
};
