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

    ctx.save().translate(width/2, height/2).scale(1, -1);
}

ShooterView.prototype.render = function(s) {
    var size = s.vehicle.mass;
    this.ctx.save()
            .translate(s.vehicle.pos.x, s.vehicle.pos.y)
            .rotate(-s.vehicle.dir)
            .beginPath()
            .moveTo(-size, -size)
            .lineTo(size, -size)
            .lineTo(0, 3 * size)
            .closePath()
            .stroke()
            .restore();
};

ShooterView.prototype.clear = function()
{
    this.ctx.clearRect(-this.width/2, -this.height/2, this.width, this.height);
};