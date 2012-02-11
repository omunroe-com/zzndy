(function() {
    Color = function( r, g, b, a ) {
        this.alpha = 1;
        this.red = this.green = this.blue = 0;

        switch ( arguments.length ) {
            case 4:
                this.alpha = parseFloat(a).within(0, 1);
            case 3:
                this.red = parseInt(r).within(0, 255);
                this.green = parseInt(g).within(0, 255);
                this.blue = parseInt(b).within(0, 255);
                break;
            case 2:
                parse.call(this, r, g);
                break;
            case 1:
                if ( typeof r == 'string' )
                    parse.call(this, r);
                else {
                    this.red = r.red;
                    this.green = r.green;
                    this.blue = r.blue;
                    this.alpha = r.alpha;
                }
                break;
        }
    }

    var C = Color.prototype;

    C.toString = function( h ) {
        with ( this ) {
            if ( alpha != 1 )
                return 'rgba(' + [red, green, blue, alpha] + ')';
            if ( h == 'h' )
                return '#' + red.toString(16).pad(2, '0') + green.toString(16).pad(2, '0') + blue.toString(16).pad(2, '0');
            else
                return 'rgb(' + [red, green, blue] + ')';
        }
    }

    var mask = /rgb(a)?\(\s*(25[0-5]|2[0-4]\d|[01]?\d?\d)\s*,\s*(25[0-5]|2[0-4]\d|[01]?\d?\d)\s*,\s*(25[0-5]|2[0-4]\d|[01]?\d?\d)\s*(?:,\s*((?:1|0)?\.\d+|1|0)\s*)?\)/i;

    /*	Parse a colour specified in HTML format and
     *	return as a Color object, alpha is optional
     */
    function parse( scol, alpha ) {
        var base = 10, r, g, b;

        if ( scol.substr(0, 3) == 'rgb' ) { // parsing RGB(A) text
            var a, m = scol.match(mask);
            if ( !m ) return;
            a = m[1];
            r = m[2];
            g = m[3];
            b = m[4];
            alpha = m[5];

            alpha = (a && alpha) ? parseFloat(alpha) : 1;
        }
        else {
            base = 16;
            if ( scol.substr(0, 1) == '#' )    scol = scol.substr(1);
            var w = scol.length / 3;
            if ( w != 2 && w != 1 ) return;

            var m = [scol.substr(0, w), scol.substr(w, w), scol.substr(2 * w, w)];
            r = m[0];
            g = m[1];
            b = m[2];

            if ( w == 1 ) {
                r = r.x(2);
                g = g.x(2);
                b = b.x(2);
            }

            alpha || (alpha = 1);
        }

        this.red = parseInt(r, base).within(0, 255);
        this.green = parseInt(g, base).within(0, 255);
        this.blue = parseInt(b, base).within(0, 255);
        this.alpha = alpha.within(0, 1);
    }

    C.tint = function( tones ) {
        return new Color(
                (this.red + tones).within(0, 255),
                (this.green + tones).within(0, 255),
                (this.blue + tones).within(0, 255),
                this.alpha);
    }

    // Alter color in random fassion
    // deviation is {base: int, r: int, g: int, b:int, a: float}
    C.deviate = function( base, r, g, b, a ) {
        var step = 4;
        var dev = deviate(base / step) * step;
        return new Color(
                this.red + dev + deviate(r / step) * step,
                this.green + dev + deviate(g / step) * step,
                this.blue + dev + deviate(b / step) * step,
                this.alpha + deviate(a)
                );
    }

})()
