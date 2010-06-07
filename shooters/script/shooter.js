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