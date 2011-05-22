/**
 * Create new shooter
 * @param {Vehicle} vehicle - vehicle type
 * @param {Number} hp       - health points
 * @param {Number} numgun   - number of guns
 * @param {Array} guns      - guns type
 */
function Shooter(vehicle, hp, guns)
{
    this.vehicle = vehicle;
    this.hp = hp;
    this.guns = guns;
}

Shooter.prototype.clone = function()
{
    return new Shooter(this.vehicle.clone(), this.hp, this.guns.map(function(g){return g.clone()}));
};

Shooter.prototype.shoot = function(delay)
{
    var p = this.vehicle.pos;
    var pos = new Point(p.x + this.vehicle.mass * 1.1 * sin(this.vehicle.dir), p.y + this.vehicle.mass * 1.1 * cos(this.vehicle.dir));
	var self = this;
	
	return this.guns.reduce(function (a, g){	
		return a.concat(g.shoot(delay, pos, self.vehicle.dir));			
	}, []);
};