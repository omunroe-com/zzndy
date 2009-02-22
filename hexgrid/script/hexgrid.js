//(function() {
/**
 * Hexademical grid
 * handles all padding and placement issues
 */
var sqr3 = 1 / Math.sqrt(3), sqrt32 = Math.sqrt(3) / 2;

HexGrid = function(rows, cols, width, spacing) {
    if(!(rows%2) || !(cols%2))
        throw new Error('Rows and columnd must be odd numbers, got ' + rows + ' and ' + cols + '.');

    this.rows = rows;
    this.cols = cols;
    this.cap = cols - Math.floor(rows / 2);
    this.width = width;
    this.spacing = spacing;
    this.dh = width * sqrt32;

    var side = width * sqr3 - spacing / 2;
    this.hex = new HexCell(width - spacing, side);
    this.borderHex = new HexCell(width + spacing, width * sqr3 + spacing / 2);
    this.ci = Math.floor(rows / 2);
    this.cj = Math.floor(cols / 2);
};

var H = HexGrid.prototype;

H.hexOk = function(i, j) {
    return this.cols - this.rows + i - j < this.cap && j - i < this.cap;
};

H.makeAllPaths = function(ctx) {
    ctx.beginPath();
    for (var i = 0; i < this.rows; ++i)
        for (var j = 0; j < this.cols; ++j)
            this.makePath(i, j, ctx)

    return ctx.closePath();
};

H.traceBorder = function(ctx) {
    ctx.beginPath();
    this.borderHex.moveTo(getCoords.call(this, 0, 0), 0, ctx);

    for (var j = 0; j < this.cap; ++j) {
        this.borderHex.lineTo(getCoords.call(this, 0, j), 0, ctx);
        this.hex.lineTo(getCoords.call(this, -1, j), 3, ctx);
    }

    for (var i = 0; i <= this.cols - this.cap; ++i) {
        this.borderHex.lineTo(getCoords.call(this, i, this.cap + i - 1), 1, ctx);
        this.hex.lineTo(getCoords.call(this, i, this.cap + i), 4, ctx);
    }

    for (; i < this.rows; ++i) {
        this.hex.lineTo(getCoords.call(this, i, this.cols), 5, ctx);
        this.borderHex.lineTo(getCoords.call(this, i, this.cols - 1), 2, ctx);
    }

    for (j = this.cols - 1; j >= this.cols - this.cap; --j) {
        this.borderHex.lineTo(getCoords.call(this, this.rows - 1, j), 3, ctx);
        this.hex.lineTo(getCoords.call(this, this.rows, j), 0, ctx);
    }

    for (i = this.rows - 1; i >= this.cols - this.cap; --i) {
        this.borderHex.lineTo(getCoords.call(this, i, this.cols - this.cap + i - this.rows + 1), 4, ctx);
        this.hex.lineTo(getCoords.call(this, i, this.cols - this.cap + i - this.rows), 1, ctx);
    }

    for (++i; i >= 0; --i) {
        this.borderHex.lineTo(getCoords.call(this, i, 0), 5, ctx);
        this.hex.lineTo(getCoords.call(this, i - 1, -1), 2, ctx);
    }

    return ctx.closePath();
};

H.strokeAll = function(ctx) {
    this.traceBorder(ctx).stroke();

    return this.makeAllPaths(ctx).stroke();
};

H.fillAll = function(ctx) {
    this.traceBorder(ctx).stroke();
    return this.makeAllPaths(ctx).fill();
};

//[private]
function getCoords(i, j) {
    var pos = (j - this.cj) * this.width;
    var shift = (this.ci - i) * this.width / 2;

    return new Point(pos + shift, (i - this.ci) * this.dh);
}

H.makePath = function(i, j, ctx, dontCheck) {
    if (dontCheck || this.hexOk(i, j)) this.hex.makePath(getCoords.call(this, i, j), ctx);
};

H.stroke = function(i, j, ctx) {
    if (this.hexOk(i, j))  this.hex.stroke(getCoords.call(this, i, j), ctx);
};

H.fill = function(i, j, ctx) {
    if (this.hexOk(i, j))
        this.hex.fill(getCoords.call(this, i, j), ctx);
};

// TODO: optimize to only check no more than 7 hexagons.
H.getAt = function(x, y) {
    var si, sj, md = 1e+16;
    for (var i = 0; i < this.rows; ++i)
        for (var j = 0; j < this.cols; ++j)
        {
            var pos = getCoords.call(this, i, j);
            var dist = (pos.x - x)*(pos.x - x) + (pos.y - y)*(pos.y - y);
            if(dist<md){
                md=dist;
                si=i;
                sj=j;
            }
        }

    return [si, sj];
}
    //})();