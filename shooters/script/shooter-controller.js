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
}

ShooterController.prototype.frame = function() {
    var time = (new Date).getTime();
    var delay = this.lastRun == 0 ? 0 : time - this.lastRun;
    this.lastRun = time;

    var mw = this.width / 2;
    var mh = this.height / 2;
    var v = this.view;
    v.clear();
    this.shooters = this.shooters.map(function(s) {
        s.vehicle.steer(delay);
        s.vehicle.move(delay);

        var p = s.vehicle.pos;
        if (p.x > mw)p.x -= this.width;
        else if (p.x < -mw)p.x += this.width;

        if (p.y > mh)p.y -= mh;
        else if (p.y < -mh)p.y += mh;

        v.render(s);
        return s;
    });

};