function set_level_easy()
{
    default_mx = 12;
    default_my = 14;
}

var default_mx = 32;
var default_my = 24;
var pad = .945;
var margin = 1;

var shade_high = 'rgba(255, 255, 255, .12)';
var shade_left = 'rgba(0, 0, 0, .05)';
var shade_bott = 'rgba(0, 0, 0, .1)';

function Filler(canvas, mx, my)
{
    this.ctx = canvas.getContext('2d');
    this.logic = new FillerLogic(mx || default_mx, my || default_my);
    this.dx = margin * canvas.width / (this.logic.mx + .5);
    this.dy = margin * canvas.height / (this.logic.my + 1);
    this.height = canvas.height;
    this.width = canvas.width;
}

var F = Filler.prototype;

F.render = function()
{
    var it = this;
    this.ctx.clearRect(0, 0, this.width, this.height);

    range(it.logic.my).forEach(function(i) {
        range(it.logic.mx).forEach(function(j)
        {
            it.renderCell(i, j);
        });
    });
};

F.renderCell = function(i, j)
{
    this.ctx.save();

    this.ctx.fillStyle = this.logic.getColor(i, j);

    var x = j * this.dx;
    var y = i * this.dy;

    this.ctx
        .translate(x + (i % 2 + 1) * this.dx / 2 + this.width * (1 - margin) / 2, this.height - this.height * (1 - margin ) / 2 - y - this.dy)
        .beginPath()
        .moveTo(- pad * this.dx / 2, 0)
        .lineTo(0, pad * this.dy)
        .lineTo(pad * this.dx / 2, 0)
        .lineTo(0, - pad * this.dy)
        .closePath()
        .fill();

    drawSegment.call(this, shade_high, -1, 0, 0, -1, 0, 0);
    drawSegment.call(this, shade_left, 0, -1, 1, 0, 0, 1);
    drawSegment.call(this, shade_bott, -1, 0, 1, 0, 0, 1);

    function drawSegment(color, x0, y0, x1, y1, x2, y2)
    {
        this.ctx.fillStyle = color.toString();
        this.ctx.beginPath()
            .moveTo(x0 * pad * this.dx / 2, y0 * pad * this.dy)
            .lineTo(x1 * pad * this.dx / 2, y1 * pad * this.dy)
            .lineTo(x2 * pad * this.dx / 2, y2 * pad * this.dy)
            .closePath()
            .fill();
    }

    this.ctx.restore();

    return this;
};

F.clearCell = function(i, j)
{
    this.ctx
        .save()
        .globalCompositeOperation = 'destination-out';

    var x = j * this.dx;
    var y = i * this.dy;

    this.ctx
        .translate(x + (i % 2 + 1) * this.dx / 2 + this.width * (1 - margin) / 2, this.height - this.height * (1 - margin ) / 2 - y - this.dy)
        .beginPath()
        .moveTo(- (1 + pad * this.dx / 2), 0)
        .lineTo(0, 1 + pad * this.dy)
        .lineTo(1 + pad * this.dx / 2, 0)
        .lineTo(0, - (1 + pad * this.dy))
        .closePath()
        .fill()
        .restore();

    return this;
};

F.getColors = function()
{
    return this.logic.getColors();
};

F.setColor = function(color)
{
    var tr = this.logic.setColor(color);
    redrawCluster.call(this, tr);
};

F.moveP2 = function()
{
    var tr = this.logic.moveP2();
    redrawCluster.call(this, tr);
};

var alter = false;

function redrawCluster(points)
{
    function sort(p1, p2)
    {
        var a = p1[0] + p1[1];
        var b = p2[0] + p2[1];

        if (alter)
            return a - b;
        else return b - a;
    }

    var it = this;
    alter = !alter;

    points.sort(sort).forEach(function(i, n)
    {
        var fn = function()
        {
            it.clearCell(i[0], i[1]).renderCell(i[0], i[1]);
        };

        window.setTimeout(fn, n * 1.3);
    });
}

F.__defineGetter__('p1color', function() {
    return this.logic.p1color;
});

F.__defineGetter__('p2color', function() {
    return this.logic.p2color;
});

Comet = {};

Comet.open = function(url, args)
{
    var ifr = document.createElement('iframe');

    ifr.setAttribute('src', url + '?' + toQueryString(args));
    ifr.setAttribute('style', 'display:none');

    document.body.appendChild(ifr);

    return ifr;
};