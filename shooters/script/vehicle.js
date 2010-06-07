function Point(x, y)
{
    this.x = x;
    this.y = y;
}

/**
 * Create new vehicle.
 * @param pos   - position in units from center (positive direction in north and east)
 * @param dir   - direction in radians (from north clockwise)
 * @param mass  - mass
 * @param speed - speed in units per second
 */
function Vehicle(pos, dir, mass, speed)
{
    this.pos = new Point(pos.x, pos.y);
    this.dir = dir;
    this.mass = mass;
    this.speed = speed;
}