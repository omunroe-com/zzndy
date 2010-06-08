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
}

Vehicle.prototype.move = function(delay) {
    if (delay == 0)return;
    this.pos.x = this.pos.x + this.speed * sin(this.dir) * 1000 / delay;
    this.pos.y = this.pos.y + this.speed * cos(this.dir) * 1000 / delay;
};

var k = 2;
Vehicle.prototype.steer = function(delay) {
    this.dir += rnd(k / mass) - k / (mass * 2);
};

Vehicle.prototype.clone = function() {
    return new Vehicle(new Point(this.pos.x, this.pos.y), this.dir, this.mass, this.speed);
};

Vehicle.prototype.isHitBy = function(bullet) {
    return this.pos.to(bullet.pos) < this.mass;
};