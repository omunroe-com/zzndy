/**
 * @class ForceObj - abstract base force object
 * @constructor
 */
ForceObj = function() {
};

/**
 * Get force at these coordinates.
 * @param {Number} x
 * @param {Number} y
 */
ForceObj.prototype.getForce = function(x, y)
{
    throw new Error('ForceObj is abstract');
};


/**
 * @class PotentialField
 * @constructor PotentialField - initializes an instance of potential field
 * @param {Number} rows - number of rows
 * @param {Number} cols - number of cols
 */
function PotentialField(rows, cols)
{
    this.rows = rows;
    this.cols = cols;
    this.body = rows.times(cols.times(function() {
        return new Vector(0, 0);
    }));
}

var PF = PotentialField.prototype = new ForceObj;
PF.base = ForceObj.prototype;

/**
 * Create a sum of two potential fields.
 * @param {PotentialField} other other potential field
 * @return {PotentialField} sum of this and other potential fields.
 */
PF.add = function(other)
{
    if (!(other instanceof PotentialField))
        throw new Error("Can only add potential field to another potential field.");

    if (other.rows != this.rows || other.cols != this.cols)
        throw new Error("Cannot add potetntial fields with different dimentions.");

    var res = new PotentialField(this.rows, this.cols);
    var i = -1;
    while (++i < this.rows)
    {
        var j = -1;
        while (++j < this.cols)
        {
            res.body[i][j] = this.body[i][j].add(other.body[i][j]);
        }
    }

    return res;
};

/**
 * Save given charge influence in the field.
 * @param {Number} x
 * @param {Number} y
 * @param {ForceObj} charge
 */
PF.apply = function(x, y, charge)
{
    if (!(charge instanceof ForceObj))
        throw new Error('Cannot apply non force object to potential field.');

    var i = -1;
    while (++i < this.rows)
    {
        var j = -1;
        while (++j < this.cols)
        {
            this.body[i][j] = this.body[i][j].add(charge.getForce(x, y, j, i));
        }
    }

    return this;
};

/**
 * Get force at these coordinates.
 * @param {Number} x
 * @param {Number} y
 */
PF.getForce = function(x, y)
{
    var x0 = Math.floor(x);
    var x1 = Math.ceil(x);
    var y0 = Math.floor(y);
    var y1 = Math.ceil(y);

    var a = 0;
    var force = new Vector(0, 0);

    if (x0 >= 0 && x0 < this.cols) {
        if (y0 >= 0 && y0 < this.rows) {
            a = (1-(x - x0)) * (1-(y - y0));
            force = force.add(this.body[y0][x0].mul(a));
        }

        if (y0!=y1&&y1 >= 0 && y1 < this.rows) {
            a = (1-(x - x0)) *(1- (y1 - y));
            force = force.add(this.body[y1][x0].mul(a));
        }
    }

    if (x0!=x1 && x1 >= 0 && x1 < this.cols) {
        if (y0 >= 0 && y0 < this.rows) {
            a = (1-(x1 - x)) * (1-(y - y0));
            force = force.add(this.body[y0][x1].mul(a));
        }

        if (y0!=y1&&y1 >= 0 && y1 < this.rows) {
            a = (1-(x1 - x)) * (1-(y1 - y));
            force = force.add(this.body[y1][x1].mul(a));
        }
    }

    return force;
};

/**
 * Create a charge object
 * @param {Number} centerforce force magnitude at the centre
 * @param {Number} zeroradius radius (in cells) at which force magnitude is 0.
 * @constructor
 */
function Charge(centerforce, zeroradius)
{
    this.centerforce = centerforce;
    this.zeroradius = zeroradius;
    this.r2 = zeroradius * zeroradius;
}

var C = Charge.prototype = new ForceObj;
C.base = ForceObj.prototype;

C.getForce = function(x0, y0, x, y)
{
    var dx = x0 - x, dy = y0 - y;
    var d2 = dx * dx + dy * dy;

    var force;
    if (d2 >= this.r2)
    {
        force = new Vector(0, 0);
    }
    else {
        var ang = Math.atan2(dy, dx);
        var k = this.centerforce * (this.r2 - d2) / this.r2;
        force = new Vector(k * Math.cos(ang), k * Math.sin(ang));
    }

    return force;
};

function Wall(x1, y1, x2, y2, magnitude, reach)
{
    this.x1 = x1;
    this.x2 = x2;
    this.y1 = y1;
    this.y2 = y2;

    this.m = magnitude;
    this.reach = reach || this.m * 10;
    this.r2 = this.reach * this.reach;
    this.len = Math.sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));

    this.dx = x2 - x1;
    this.dy = y2 - y1;
}

var W = Wall.prototype = new ForceObj;
W.base = ForceObj.prototype;

W.getForce = function(x, y)
{
    var A = x - this.x1;
    var B = y - this.y1;
    var s = A * this.dy - this.dx * B;
    var d = Math.abs(s) / this.len;
    s = s >= 0 ? 1 : -1;

    var v;
    if (d > this.reach) {
        v = new Vector(0, 0);
    }
    else {
        var ang = Math.atan2(this.dy, this.dx) - s * Math.PI / 2;
        var k = this.m * (this.reach - d) / this.reach;
        v = new Vector(k * Math.cos(ang), k * Math.sin(ang));
    }
    return v;
};
