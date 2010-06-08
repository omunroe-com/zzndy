/**
 * Create new shooters controller
 * @param {ShooterView} view - shooters view handling display
 * @param {Array} shooters   - collection of shooters
 * @param {Number} width     - width in units
 * @param {Number} height    - height in units
 */
function ShooterController(view, shooters, width, height) {
    this.view = view;
    this.shooters = shooters;
    this.width = width;
    this.height = height;
    this.lastRun = 0;
    this.bullets = [];
}

var max = Math.max;
var minhp = -10;

ShooterController.prototype.frame = function() {
    var time = (new Date).getTime();
    var delay = this.lastRun == 0 ? 0 : time - this.lastRun;
    this.lastRun = time;

    var mw = this.width / 2;
    var mh = this.height / 2;

    var v = this.view;
    var bs = this.bullets;
    var newbs = [];

    var first = true;
    v.clear();
    this.shooters = this.shooters.filter(function(s) {
        s.vehicle.steer(delay);
        s.vehicle.move(delay);

        var p = s.vehicle.pos;
        if (p.x > mw)p.x -= mw * 2;
        else if (p.x < -mw)p.x += mw * 2;

        if (p.y > mh)p.y -= mh * 2;
        else if (p.y < -mh)p.y += mh * 2;

        bs = bs.filter(function(b) {
            if (first)b.move(delay);

            if (s.hp > 0 && s.vehicle.isHitBy(b)) {
                s.hp -= max(0, s.hp - b.projectile.damage);
                return false;
            }
            return true;
        });

        if (s.hp > 0) {
            newbs = newbs.concat(s.shoot(delay));
        }
        else if (s.hp < minhp) {
            first = false;
            return false;
        }
        else {
            s.hp -= 1;
        }

        v.render(s);

        first = false;
        return true;
    });

    this.bullets = bs.concat(newbs);
    this.bullets.forEach(v.render);
};