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
    return this.gun.shoot(delay, this.vehicle.pos, this.vehicle.dir);
};