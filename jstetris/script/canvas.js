(function() {
    var C, G;

    if ( !('CanvasRenderingContext2D' in this) ) // WebKit
    {
        var ctx = document.createElement('canvas').getContext('2d');
        C = ctx.__proto__;
        G = ctx.createLinearGradient(0, 0, 0, 0).__proto__;
    }
    else
    {
        C = CanvasRenderingContext2D.prototype;
        G = CanvasGradient.prototype;
    }

    // Make canvas' methods chainable
    ('restore,rotate,save,scale,translate,arc,arcTo,bezierCurveTo,beginPath,clip,closePath,lineTo,moveTo,quadraticCurveTo,rect,stroke,strokeRect,clearRect,fill,fillRect,clip,drawImage,drawImageFromRect')
            .split(',').forEach(function( method ) {
        if ( method in C ) {
            var meth = C[method];
            C[method] = function() {
                return meth.apply(this, arguments) || this;
            };
        }
    });

    // Make gradient's addColorStop chainable
    var meth = G.addColorStop;
    G.addColorStop = function() {
        meth.apply(this, arguments);
        return this;
    };

    C.strokeCircle = function( x, y, r )
    {
        return this.beginPath()
                .arc(x, y, r, 0, Math.PI * 2, true)
                .closePath()
                .stroke();
    };

    C.fillCircle = function( x, y, r )
    {
        return this.beginPath()
                .arc(x, y, r, 0, Math.PI * 2, true)
                .closePath()
                .fill();
    };

    C.clear = function()
    {
        if ( 'size' in this && 'origin' in this )
            this.clearRect(this.origin.x, this.origin.y, this.size.w, this.size.h);

        return this;
    };

    C.sharpRect = function( x, y, w, h ) {
        return this.fillRect(x, y, w, 1).fillRect(x, y + 1, 1, h - 1).fillRect(x, y + h, w + 1, 1).fillRect(x + w, y, 1, h);
    };

    C.vLine = function( x, y, h ) {
        return this.fillRect(x, y, 1, h);
    };

    C.hLine = function( x, y, w ) {
        return this.fillRect(x, y, w, 1);
    };

    C.fillPoly = function( vertices ) {
        with ( this ) {
            beginPath().moveTo(vertices[0], vertices[1]);
            for ( var i = 2; i < vertices.length; i += 2 )lineTo(vertices[i], vertices[i + 1]);
            return closePath().fill();
        }
    };

    C.strokePoly = function( vertices ) {
        with ( this ) {
            beginPath().moveTo(vertices[0], vertices[1]);
            for ( var i = 2; i < vertices.length; i += 2 )lineTo(vertices[i], vertices[i + 1]);
            return closePath().stroke();
        }
    };

    C.renderBox = function( color, point ) {
        var origin = new Point(point.x * this.tetris.gridsize.width, point.y * this.tetris.gridsize.height);

        this.translate(origin.x, origin.y);

        if ( color == 0 )
            this.clearRect(0, 0, this.tetris.box.width, this.tetris.box.height);
        else with ( this ) {
            fillStyle = color.toString();
            fillRect(0, 0, tetris.box.width, tetris.box.height);
            fillStyle = tetris.highlight;
            fillPoly(tetris.highlightPath);
            fillStyle = tetris.shadow;
            fillPoly(tetris.shadowPath);
        }

        this.translate(-origin.x, -origin.y);
        return color; // needed for use with reduce
    };

    // Seven Segment Display block
    var ssdMasks = [1008, 384, 872, 968, 408, 728, 760, 1920, 1016, 984, 4, 11];
    var ssdPowers = [512, 256, 128, 64, 32, 16, 8, 4, 2, 1];

    // seven-segment display
    function calculateSegments( object ) {
        with ( object ) {
            var s = squeeze / (1 + squeeze);
            var w = segWidth;
            var a = w * aspect; // thickness of vertical bars
            var b = w / 2, m = a / 2; // half widths
            var y = b / 3, z = m / 3; // paddings

            var paths = [
                /*A*/        [z, 0,    1 - z, 0,        1 - a - z, w,        a + z, w],
                /*B*/        [1, y,    1, .5 - y,    1 - a, .5 - b - y,    1 - a, w + y],
                /*C*/        [1, .5 + y,    1, 1 - y,        1 - a, 1 - w - y,        1 - a, .5 + b + y],
                /*D*/        [1 - z, 1,    z, 1,         a + z, 1 - w,        1 - a - z, 1 - w],
                /*E*/        [0, 1 - y,    0, .5 + y,    a, .5 + b + y,        a, 1 - w - y],
                /*F*/        [0, .5 - y,    0, y,         a, w + y,         a, .5 - b - y],
                /*G*/        [z, .5,    a + z, .5 - b,    1 - a - z, .5 - b,    1 - z, .5,        1 - a - z, .5 + b,    a + z, .5 + b],
                /*,*/        [1 - a - 2 * z, 1 - w - 2 * y,    1 - a - b - z, 1 - w - y,    1 - 3 * a, 1 - 2 * w,    1 - 3 * a, 1 - 3 * w,    1 - 2 * a, 1 - 3 * w,     1 - a - 2 * z, 1 - 3 * b - 2 * y],
                /*+*/        [.5, .25,    .5 + m, .25 + b, .5 + m, .5 - b - 2 * y,   .5 - m, .5 - b - 2 * y, .5 - m, .25 + b],
                /*+*/        [.5, .75,    .5 + m, .75 - b, .5 + m, .5 + b + 2 * y,   .5 - m, .5 + b + 2 * y, .5 - m, .75 - b]
            ];

            // Apply sqeeze
            paths.forEach(function( i ) {
                i.forEach(function( x, j, a ) {
                    if ( (j % 2) )return;
                    a[j] = a[j] * (1 - s) + s * (1 - a[j + 1]);
                });
            });

            return paths;
        }
    }

    /**
     * @constructor @class SSD (Seven Segment Display)
     *
     * @argument p {Point}    position
     * @argument dw {Number}  digit width
     * @argument on {Color}   on color
     * @argument off {Color}  off color
     */
    SSD = function( p, dw, on, off, options ) {
        options = options || {};

        this.pos = p;
        this.width = dw;
        this.aspect = options.aspect || 2;
        this.squeeze = options.squeeze || .2;
        this.segWidth = options.segWidth || .08;
        this.on = on || 'rgba(94, 220, 50, .9)';
        this.off = off || 'rgba(94, 220, 50, .1)';

        this.segments = calculateSegments(this);
    };

    var S = SSD.prototype;

    /**
     * @argument c {CanvasRenderingContext2D}
     * @argument newVal {String}
     * @argument oldVal {String}
     */
    S.render = function( c, newVal, oldVal ) {
        var h = this.width * this.aspect, aw = this.width * (this.squeeze + 1);
        c.save().translate(this.pos.x, this.pos.y).scale(aw, h);
        var nl = newVal.length;
        for ( var i = 0; i < nl; ++i ) {
            var d = newVal[i];
            if ( !(oldVal && (oldVal[i] == d)) ) {
                c.clearRect(-.02, -.02, 1.04, 1.04);
                if ( d == '.' ) d = 10;
                else if ( d == '+' ) d = 11;

                var mask = ssdMasks[d || 0], l = this.segments.length;
                for ( var j = 0; j < l; ++j ) {
                    var seg = this.segments[j];
                    if ( ssdPowers[j] & mask ) c.fillStyle = this.on;
                    else c.fillStyle = this.off;

                    c.fillPoly(seg);
                }
            }
            c.translate(1.3 - this.squeeze, 0);
        }

        return c.restore();
    }

    /**
     * @class Viewport - encapsulate tetris windows.
     *
     * @argument {Point} point
     * @argument {Rectangle} size
     * @argument {Rectangle} size
     */
    Viewport = function( pos, size, pad ) {
        this.pos = pos;
        this.size = size;
        this.pad = pad;
        this.canvas = null;
    }

    var V = Viewport.prototype;

    V.__defineGetter__('ambientSize', function() {
        return Rectangle.sum(this.size, this.pad);
    });

    V.__defineGetter__('contentPos', function() {
        return new Point(this.pos.x + this.pad.width, this.pos.y + this.pad.height);
    });

    V.__defineGetter__('colors', function() {
        if ( !this['private:colors'] ) {
            this['private:colors'] = [
                'rgba(250, 104, 255, 0.3)',
                'rgba(255, 250,  53, 0.3)',
                'rgba(134, 255, 132, 0.3)',
                'rgba(255, 141, 127, 0.3)',
                'rgba(122, 250, 244, 0.3)']
            this['private:colors'].shuffle();
        }
        return this['private:colors'];
    });

    V.shift = function( sh ) {
        this.canvas.translate(this.pos.x + this.pad.width, this.pos.y + this.pad.height + (sh || 0));
    }

    V.unshift = function( sh ) {
        this.canvas.translate(-this.pos.x - this.pad.width, -this.pos.y - this.pad.height - (sh || 0));
    }

    V.mkFill = function( size, decay ) {
        var grad = this.canvas.createLinearGradient(size.width / 4, 0, this.size.width * 3 / 4, size.height), l = this.colors.length - 1;

        this.colors.forEach(function( c, i ) {
            if ( decay ) {
                c = new Color(c);
                c.alpha /= decay;
                c = c.toString();
            }

            grad.addColorStop(i / l, c);
        });
        return grad;
    }

    V.enframe = function() {
        var as = this.ambientSize;

        with ( this.canvas ) {
            save();
            fillStyle = this.mkFill(as);
            translate(this.pos.x, this.pos.y);
            sharpRect(0, 0, as.width, as.height);
            restore();
        }
    }

    V.mkGrid = function( fieldSize, gridType ) {
        var v, h, vv, hh, width = this.size.width / fieldSize.width, height = this.size.height / fieldSize.height;
        if ( gridType == '|' ) {
            h = 1;
            v = height;
        }
        else if ( gridType == '+' ) {
            h = Math.floor(width * .55);
            v = Math.floor(height * .3);
        }

        vv = Math.floor(v / 2);
        hh = Math.floor(h / 2);

        this.shift();
        with ( this.canvas ) {
            fillStyle = this.mkFill(this.size, 3);
            for ( var j = 1; j < fieldSize.width; ++j ) {
                if ( gridType == '|' ) {
                    vLine(j * width - 1, -this.pad.height, this.pad.height + vv + 1);
                    vLine(j * width - 1, fieldSize.height * height - vv, this.pad.height + vv - 1);
                }
                for ( var i = 1; i < fieldSize.height; ++i ) {
                    vLine(j * width - 1, i * height - vv, v);
                    hLine(j * width - hh - 1, i * height - 1, h);
                }
            }
        }
        this.unshift();
    }
})()
