function Point(x, y) {
    this.x = x;
    this.y = y;
}

var sin = Math.sin;
var cos = Math.cos;
function rnd(n)
{
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
    this.pos.x = this.pos.x + this.speed * sin(this.dir);
    this.pos.y = this.pos.y + this.speed * cos(this.dir);
};

var k = 2;
Vehicle.prototype.steer = function(delay) {
    this.dir += rnd(k/mass) - k/(mass*2);
};

Vehicle.prototype.clone = function() {
    return new Vehicle(new Point(this.pos.x, this.pos.y), this.dir, this.mass, this.speed);
};