/**
 * JS Tetris library.
 * Copyright (c) 2008-2009 Andriy Vynogradov
 */
// TODO: refactor Lib to jstetris specific and general functions.
(function() {
    Lib = {
        /**
         * Output information about game statistics. The information is
         * only valid after game is over.
         *
         * @argument {Object} stats    statistics objects to report
         * @argument {Boolean} global  true if reporting globals statistics
         * @return {Array}             array of game traits
         */
        mkStatsReport: function( stats, global, classic ) {
            var res = [];
            global = !!global;
            classic = !!classic;
            if( global && stats.score ) // briefly show globally earned score
            {
                res.push([stats.score.abbr(), stats.score + 'pts earned globally']);
            }

            if( stats.lines ) {
                var x = [stats.lines.abbr(), stats.lines + ' lines burned'];
                if( global )
                    x[1] += ' globally';
                if( stats.forLines )
                    x[1] += ' (' + stats.forLines + 'pts)';
                res.push(x);
            }

            if( stats.figures && (classic || stats.drops != stats.figures) ) {
                var x = [stats.figures.abbr(), stats.figures + ' figures used'];
                if( global )
                    x[1] += ' globally';
                if( stats.forFigures )
                    x[1] += ' (' + stats.forFigures + 'pts)';
                res.push(x);
            }

            if( !classic && stats.drops ) {
                var x = [stats.drops.abbr(), stats.drops + ' figures dropped'];
                if( stats.dropLevels ) {
                    if( stats.forDrops )
                        x[1] += ' (' + stats.forDrops + 'pts for ' + stats.dropLevels + ' levels down)';
                    else
                        x[1] += ' ' + stats.dropLevels + ' levels down';
                }
                res.push(x);
            }

            var gt = stats.time || stats.gameTime
            if( gt ) {
                var d = Math.floor(gt / 864e5 % 24);
                var h = Math.floor(gt / 36e5 % 60);
                var m = Math.floor(gt / 6e4 % 60);
                var s = Math.floor(gt / 1e3 % 60);
                var txt = 'game time ' + (d ? d + ' days ' : '') + [h.zf(2), m.zf(2), s.zf(2)].join(':');
                if( global )
                    txt = 'total ' + txt;
                if( d || h )
                    res.push([[(d ? d + ' days ' : ''), h.zf(2), 'h ', m.zf(2), 'm'].join(''), txt]);
                else
                    if( m < 2 )
                        res.push([(m * 60 + s) + 's', txt]);
                    else
                        res.push([m.zf(2) + 'm ' + s.zf(2) + 's', txt]);
            }

            return res.map(function( pair ) {
                return Tag.mk('span', {
                    title: pair[1]
                }, pair[0]);
            });
        },

        animateFavicon: function( delay, array ) {
            function getFavicon() {
                var lns = document.getElementsByTagName('link');
                for( var l in lns )
                    if( lns[l].rel == 'shortcut icon' && lns[l].type == 'image/x-icon' )
                        return lns[l];
            }

            function mkLink( url ) {
                var link = document.createElement('link');
                link.setAttribute('type', 'image/x-icon');
                link.setAttribute('rel', 'shortcut icon');
                link.setAttribute('href', url);
                return link;
            }

            array.delayedReduce(delay, function( fav, element ) {
                var parent = fav.parentNode;
                parent.removeChild(fav);
                return parent.appendChild(mkLink(element));
            }, getFavicon());
        }
    }


    // Seven Segment Display block
    var ssdMasks = [1008, 384, 872, 968, 408, 728, 760, 1920, 1016, 984, 4, 11];
    var ssdPowers = [512, 256, 128, 64, 32, 16, 8, 4, 2, 1];

    // seven-segment display
    function calculateSegments( object ) {
        with( object ) {
            var s = squeeze / (1 + squeeze);
            var w = segWidth;
            var a = w * aspect; // thickness of vertical bars
            var b = w / 2, m = a / 2; // half widths
            var y = b / 3, z = m / 3; // paddings
            var paths = [ /*A*/[z, 0, 1 - z, 0, 1 - a - z, w, a + z, w], /*B*/ [1, y, 1, .5 - y, 1 - a, .5 - b - y, 1 - a, w + y], /*C*/ [1, .5 + y, 1, 1 - y, 1 - a, 1 - w - y, 1 - a, .5 + b + y], /*D*/ [1 - z, 1, z, 1, a + z, 1 - w, 1 - a - z, 1 - w], /*E*/ [0, 1 - y, 0, .5 + y, a, .5 + b + y, a, 1 - w - y], /*F*/ [0, .5 - y, 0, y, a, w + y, a, .5 - b - y], /*G*/ [z, .5, a + z, .5 - b, 1 - a - z, .5 - b, 1 - z, .5, 1 - a - z, .5 + b, a + z, .5 + b], /*,*/ [1 - a - 2 * z, 1 - w - 2 * y, 1 - a - b - z, 1 - w - y, 1 - 3 * a, 1 - 2 * w, 1 - 3 * a, 1 - 3 * w, 1 - 2 * a, 1 - 3 * w, 1 - a - 2 * z, 1 - 3 * b - 2 * y], /*+*/ [.5, .25, .5 + m, .25 + b, .5 + m, .5 - b - 2 * y, .5 - m, .5 - b - 2 * y, .5 - m, .25 + b], /*+*/ [.5, .75, .5 + m, .75 - b, .5 + m, .5 + b + 2 * y, .5 - m, .5 + b + 2 * y, .5 - m, .75 - b]]

            // Apply sqeeze
            paths.forEach(function( i ) {
                i.forEach(function( x, j, a ) {
                    if( !(j % 2) )
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
        options = options ||
                  {};

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
        for( var i = 0; i < nl; ++i ) {
            var d = newVal[i];
            if( !(oldVal && (oldVal[i] == d)) ) {
                c.clearRect(-.02, -.02, 1.04, 1.04);
                if( d == '.' )
                    d = 10;
                else
                    if( d == '+' )
                        d = 11;

                var mask = ssdMasks[d || 0], l = this.segments.length;
                for( var j = 0; j < l; ++j ) {
                    var seg = this.segments[j];
                    if( ssdPowers[j] & mask )
                        c.fillStyle = this.on;
                    else
                        c.fillStyle = this.off;

                    c.fillPoly(seg);
                }
            }
            c.translate(1.3 - this.squeeze, 0);
        }

        return c.restore();
    };

    /**
     * @class Viewport
     * @constructor
     * Tetris viewport - encapsulate tetris window operations
     *
     * @param {Point} pos      position of upper left corner of the viewport
     * @param {Rectangle} dim  number of boxes along x and y axises
     * @param {Rectangle} box  single box dimensions
     * @param {Number} border  border width
     * @param {Point} margin   margin widths along x and y axises
     */
    Viewport = function( pos, dim, box, border, margin ) {
        this.pos = pos;
        this.border = border;
        this.margin = margin;
        this.box = box;
        this.dim = dim;

        /**
         * @property Viewport size
         */
        this.size = new Rectangle((dim.width + 1) * margin.x + dim.width * box.width + 2 * border, (dim.height + 1) * margin.y + dim.height * box.height + 2 * border);

        this.canvas = null;
    };

    var V = Viewport.prototype;

    V.__defineGetter__('colors', function() {
        if( !this['private:colors'] ) {
            this['private:colors'] = ['rgba(250, 104, 255, 0.3)', 'rgba(255, 250,  53, 0.3)', 'rgba(134, 255, 132, 0.3)', 'rgba(255, 112, 100, 0.3)', 'rgba(122, 250, 244, 0.3)']
            this['private:colors'].shuffle();
        }
        return this['private:colors'];
    });

    V.__defineGetter__('origin', function() {
        return new Point(this.pos.x + this.margin.x / 2 + this.border, this.pos.y + this.margin.y / 2 + this.border)
    });

    V.shift = function() {
        var origin = this.origin;
        this.canvas.translate(origin.x, origin.y);
        return this;
    };

    V.unshift = function() {
        var origin = this.origin;
        this.canvas.translate(-origin.x, -origin.y);
        return this;
    };

    V.mkFill = function( decay ) {
        var grad = this.canvas.createLinearGradient(this.size.width / 4, 0, this.size.width * 3 / 4, this.size.height), l = this.colors.length - 1;

        this.colors.forEach(function( c, i ) {
            if( decay ) {
                c = new Color(c);
                c.alpha /= decay;
                c = c.toString();
            }
            grad.addColorStop(i / l, c);
        });
        return grad;
    };

    V.enframe = function() {
        this.canvas.save().strokeStyle = this.mkFill(.8);
        this.canvas.lineWidth = this.border;
        this.canvas.translate(this.pos.x + this.border / 2, this.pos.y + this.border / 2)
                .strokeRect(0, 0, this.size.width - this.border, this.size.height - this.border).restore();
        return this;
    };

    V.renderBox = function( color, point ) {
        this.shift();

        var origin = new Point(
                this.margin.x / 2 + (this.box.width + this.margin.x) * point.x
                , this.margin.y / 2 + (this.box.height + this.margin.y) * point.y);

        this.canvas.translate(origin.x, origin.y);
        with( this.canvas ) {
            if( color == 0 )
                clearRect(0, 0, this.box.width, this.box.height);
            else {
                // TODO: Highlight path is to be moved from etris to viewport
                fillStyle = color.toString();
                fillRect(0, 0, this.box.width, this.box.height)
                        .fillStyle = tetris.highlight;
                fillPoly(tetris.highlightPath)
                        .fillStyle = tetris.shadow;
                fillPoly(tetris.shadowPath);
            }
        }
        this.canvas.translate(-origin.x, -origin.y);
        this.unshift();
        return color; // needed for use with reduce
    };

    V.mkGrid = function( gridType ) {
        this.canvas.strokeStyle = this.mkFill(2.5);
        this.canvas.lineWidth = this.border;

        this.shift();
        switch( gridType ) {
            case '+':
                break;
            case '|':
            default:
                var i = this.dim.width;
                while( --i ) {
                    var x = (this.box.width + this.margin.x) * i;
                    this.canvas.beginPath()
                            .moveTo(x, -this.margin.y / 2)
                            .lineTo(x, this.size.height - this.margin.y - this.border)
                            .stroke();

                    var j = this.dim.height;
                    while( --j ) {
                        var y = (this.box.height + this.margin.y) * j
                        this.canvas.beginPath()
                                .moveTo(x - this.box.width * .03, y)
                                .lineTo(x + this.box.width * .03, y)
                                .stroke()
                                .stroke();
                    }
                }
                break;
        }
        /*return */
        this.unshift();

        return this;
    };

})();
