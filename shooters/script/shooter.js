/**
 * Create new shooter
 * @param {Vehicle} vehicle - vehicle type
 * @param {Number} hp       - health points
 * @param {Number} numgun   - number of guns
 * @param {Gun} gun         - gun type
 */
function Shooter(vehicle, hp, numgun, gun)
{
    this.vehicle = vehicle;
    this.hp = hp;
    this.numgun = numgun;
    this.gun = gun;
}

Shooter.prototype.clone = function()
{
    return new Shooter(this.vehicle.clone(), this.hp, this.numgun, this.gun.clone());
};

Shooter.prototype.shoot = function(delay)
{
    var p = this.vehicle.pos;
    var pos = new Point(p.x + this.vehicle.mass * sin(this.vehicle.dir), p.y + this.vehicle.mass * cos(this.vehicle.dir))
    return this.gun.shoot(delay, pos, this.vehicle.dir);
};