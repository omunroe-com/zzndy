/**
 * Create new shooters view
 * @param {CanvasContext} ctx - drawing context
 * @param {Number} width                 - width in pixels
 * @param {Number} height                - height in pixels
 */
function ShooterView(ctx, width, height) {
    this.ctx = ctx;
    this.width = width;
    this.height = height;

    ctx.save().translate(width / 2, height / 2).scale(1, -1);
}

var abs = Math.abs;

ShooterView.prototype.render = function(s) {
    if (s instanceof Shooter) {
        if(s.hp > 0){
        var size = s.vehicle.mass;
        this.ctx.save()
                .translate(s.vehicle.pos.x, s.vehicle.pos.y)
                .rotate(-s.vehicle.dir)
                .beginPath()
                .moveTo(-size, -size)
                .lineTo(size, -size)
                .lineTo(0, 3 * size)
                .closePath()
                .fill()
                .restore();
        }
        else{
            this.ctx.save()
                    .translate(s.vehicle.pos.x, s.vehicle.pos.y)
                    .fillCircle(0,0, abs(s.hp * 5))
                    .restore();
        }
    }
    else if (s instanceof Bullet) {
        this.ctx.save()
                .translate(s.pos.x, s.pos.y)
                .rotate(-s.dir)
                .beginPath()
                .moveTo(0, 0)
                .lineTo(0, s.projectile.speed / 100)
                .closePath()
                .stroke()
                .restore();
    }
};

ShooterView.prototype.clear = function() {
    this.ctx.clear();
};