/**
 * Clone given object.
 * @param {Object} obj  an object to clone
 * @return {Object}     cloned object
 */
function clone( obj ) {
    var newOne = {};
    for( var i in obj ) {
        var prop = obj[i];
        if( prop instanceof Array )
            prop = prop.clone();
        else if( typeof prop == 'object' )
            prop = clone(prop);
        newOne[i] = prop;
    }

    return newOne;
}

/**
 * Extend first object given with properties from all other objects.
 */
function extend() {
    var obj = arguments[0];
    for( var i = 1; i < arguments.length; ++i )
        for( var field in arguments[i] )
            obj[field] = arguments[i][field]

    return obj;
}

/**
 * Convert object to URL query string (part after ? in url).
 * @param {Object} obj  object to convert
 * @return {String}     query string representaion
 */
function toQueryString( obj ) {
    var res = [];
    for( var i in obj ) if( typeof obj[i] != 'function' )
        res.push(encodeURIComponent(i) + '=' + encodeURIComponent(obj[i]));
    return res.join('&');
}

/**
 * Convert URL query string to object.
 * @param {String} string  query string
 * @return {Object}        javascript object
 */
function fromQueryString( string )
{
    var obj = {};
    string.split('&').map(function( part ) {
        var x = part.split('=');
        obj[decodeURIComponent(x[0])] = decodeURIComponent(x[1]);
    });

    return obj;
}

/**
 * Convert object to css string, using px as default unit
 * @param {Object} obj  object to convert
 * @return {String}     css string representaion
 */
function toCssString( obj ) {
    var res = [];
    for( var i in obj ) if( typeof obj[i] != 'function' )
        res.push(i + ':' + obj[i] + ((typeof obj[i] == 'number' && Math.abs(obj[i]) > 1) ? 'px' : ''));
    return res.join(';');
}

function range( n ) {
    var ar = [], i = -1;
    while( ++i < n ) ar.push(i);
    return ar;
}

// operators to use in reduce
function add( a, b ) {
    return a + b;
}
function mul( a, b ) {
    return a * b;
}
function sub( a, b ) {
    return a - b;
}
function div( a, b ) {
    return a / b;
}
function sqr( a ) {
    return a * a;
}

function random(n)
{
	return Math.floor(Math.random()*n);
}

(function() {
    var S = String.prototype;
    var A = Array.prototype;
    var N = Number.prototype;
    var F = Function.prototype;

    S.pad = function( n, s ) {
        s = s || ' ';
        var l = this.length;
        return (Math.abs(n) > l) ? (n > 0 ? this + s.rep(n - l) : s.rep(Math.abs(n) - l) + this) : this.toString();
    };

    S.rep =
    S.x = function( n ) {
        return (new Array(n + 1)).join(this);
    };

    /**
     * Insert whitespace. Convert CamelCasedString to 'Camel Cased String'.
     */
    S.insws = function() {
        return this.replace(/(\w)(?=[A-Z](?=[a-z]))/g, '$1 ');
    };

    S.trim = function() {
        return this.replace(/^\s+|\s+$/g, "");
    };

    S.trimsplit =
    S.ts = function( splitter ) {
        return this.split(splitter).map(trim);
    };
    S.map = function( fn, self ) {
        return this.split('').map(fn, self);
    };
    S.reduce = function( fn, init ) {
        return this.split('').reduce(fn, init);
    };

    N.zf =
    N.zerofill = function( w, p ) {
        if( p ) return this.toFixed(p).pad(-w, '0');
        return this.toString().pad(-w, '0');
    };

    N.pad = function( w, p, s ) {
        if( p ) return this.toFixed(p).pad(w, s);
        return this.toString().pad(w, s);
    };

    N.times = function( fn, self ) {
        var res = [], k = -1;

        if( typeof fn == 'function' )
            while( ++k < this )
                res.push(fn.call(this, k, self));
        else if( fn instanceof Array )
            while( ++k < this )
                res.push(fn.clone());
        else if( typeof fn == 'object' )
                while( ++k < this )
                    res.push(clone(fn));
            else
                while( ++k < this )
                    res.push(fn);

        return res;
    };

    N.within =
    N.constrain = function( min, max, bounce ) {
        if( bounce ) {
            if( this < min ) return (2 * min - this).constrain(min, max);
            else if( this > max ) return (2 * max - this).constrain(min, max);
        }
        return Math.min(Math.max(min, this), max);
    };

    N.positive = function() {
        return this.constrain(1, Infinity);
    };
    N.negative = function() {
        return this.constrain(-Infinity, -1);
    };
    N.nonPositive = function() {
        return this.constrain(0, Infinity);
    };
    N.nonNegative = function() {
        return this.constrain(Infinity, 0);
    };

    /**
     * Format is {name[:{-|0}[width[.decimal]][base]][casing]}
     * where
     *  name        name of a field, can also be in form "property[10].another.prop[10][11].length"
     *     -           forces right align *
     *     0           zerofill *,**
     *     width        align width
     *     decimal        decimal places**
     *  base        enumeration base**, can be one of
     *         x            hexademical
     *         o            octal
     *         b            binary
     *         n            verbose `base`, converts 1 to 1-st, 2 to 2-nd and so on
     *         r            roman numerals
     * casing       enforce character casing
     *         u            uppercase
     *         l            lowercase
     *
     * *  - useless w/o width
     * ** - for numbers only
     */
    S.format =
    S.fmt = function( fmtObj ) {
        function getValue( path ) {
            var prop = path.match(/^(\.?\w+|\[\d+\])(?=\.|$|\[\d+)/)[0];
            var rest = path.replace(prop, '');
            if( rest ) return getValue.call(this[prop.replace(/^\.|^\[|\]$/g, '')], rest);
            else return this[prop.replace(/^\.|^\[|\]$/g, '')];
        }

        function simpleFormat( a ) {
            var r = this;
            for( var k in a ) {
                if( typeof a[k] == 'function' ) continue;
                var rx = new RegExp('\\$?\\{' + k + '\\}', 'g');
                r = r.replace(rx, a[k]);
            }
            return r;
        }

        var res = simpleFormat.call(this, fmtObj);
        var opts = '(?:\\.|\\[\\d+\\])?[\\w\\[\\]\\.]*)(?::(?:(-|0)?(\\d+)(?:\\.(\\d+))?)?(r|n|x|o|b)?(u|l)?';

        for( var name in fmtObj ) {
            var value, backUp = [];
            value = backUp[0] = fmtObj[name];
            if( typeof value == 'function' ) continue;
            var rx = new RegExp('\\$?\\{(' + name + opts + ')?\\}');
            var num = backUp[1] = new Number(value), m;
            while( m = res.match(rx) ) {
                var match = m[0], path = m[1], align = m[2], width = m[3], deci = m[4], base = m[5], casing = m[6];
                var useBackup = !!(path || base);
                if( path ) {
                    value = getValue.call(fmtObj, path);
                    num = new Number(value);
                }
                switch( base ) {
                    case 'b': value = num.toString(2); break;
                    case 'o': value = num.toString(8); break;
                    case 'x': value = num.toString(16); break;
                    case 'n': value = num.toPos(); break;
                    case 'r': value = num.toRoman(); break;
                }
                switch (casing) {
                    case 'u':value = value.toString().toUpperCase();break;
                    case 'l':value = value.toString().toLowerCase();break;
                }

                try {
                    res = res.replace(match, (isNaN(num) || base) ? value.pad(align == '-' ? -width : width) : (align == '0' ? num.zf(width, deci) : num.pad(width, deci)));
                }
                catch( ex )
                {
                    throw new Error('Undefined parameter ' + path + ' in template ' + this);
                }

                if( useBackup ) {
                    value = backUp[0];
                    num = backUp[1];
                }
            }
        }
        return res;
    };

	if(!('each' in Array) && ('forEach' in Array))
		A.each = A.forEach;

    if( !('reduce' in Array) )
        A.reduce = function( fn2, init ) {
            var i = 0;
            if( arguments.length < 2 ) init = this[++i];

            var res = init, l = this.length;
            for( ; i < l; ++i ) res = fn2(res, this[i], i, this);
            return res;
        };

    if( !('flatten' in Array) )
        A.flatten = function()
        {
            function flatten( prev, current ) {
                if( current instanceof Array ) return current.reduce(flatten, prev);
                prev.push(current);
                return prev;
            }
            return this.reduce(flatten, []);
        };

    if( !('uniq' in Array) )
        A.uniq = function()
        {
            function uniq( prev, current )
            {
                if( prev.indexOf(current) == -1 )
                    prev.push(current);

                return prev;
            }

            return this.reduce(uniq, []);
        };

    A.delayedReduce = function( delay, fn2, init, callback ) {
        var i = 0;
        if( arguments.length < 2 ) init = this[++i];
        var res = init, l = this.length;
        var fn = function( res, elt, idx, array ) {
            res = fn2(res, elt, idx, array);
            if( ++idx < l )
                window.setTimeout(arguments.callee, delay, res, array[idx], idx, array);
            else if( callback ) callback(res);
        };

        fn(res, this[i], i, this);
    };

    A.diff = function( other ) {
        return this.map(function( x, i ) {
            return (x == other[i]) ? undefined : [x, other[i]];
        });
    };

    A.clone = function() {
        return this.map(function( i ) {
            if( i instanceof Array )return i.clone();
            else if( typeof i == 'object' )return clone(i);
            else return i;
        });
    };

    /**
     * Randomly pick n items from the array
     * @param {Number} n  number of items to pick
     */
    A.pick = function( n ) {
        if( n === undefined )return this[Math.floor(Math.random() * this.length)];
        var picked = [];
        n = Math.floor(n) || 1;
        return n.map(function() {
            if( picked.length == this.length ) return undefined;
            do var i = Math.floor(Math.random() * this.length);
            while( i in picked );
            picked.push(i);
            return this[i];
        }, this);
    };
	
	A.random = function()
	{
		return this[Math.floor(Math.random()*this.length)];
	}
	
    A.shuffle = function () {
        var i = -1, n = this.length;
        while( ++i < n ) {
            var k = Math.floor(Math.random() * n);
            var tmp = this[i];
            this[i] = this[k];
            this[k] = tmp;
        }

        return this;
    };

    A.sum = function() {
        return this.reduce(add, 0);
    };
    A.prod = function() {
        return this.reduce(mul, 1);
    };

    F.timeit = function( self, args ) {
        var start = new Date();
        this.apply(self, args);
        var time = (new Date()) - start;
        return (time < 1000) ? time + 'ms' : (time / 1000).toFixed(3) + 's';
    };

    F.bind = F.detach = function( obj ) {
        var fn = this;
        return function() {
            return fn.apply(obj, arguments);
        };
    };

    F.fork = function() {
        var args = arguments, fn = this;
        var delegate = function() {
            return fn.apply(null, args);
        };
        window.setTimeout(delegate, 10);
    };

    N.map = function( fn, self ) {
        if( typeof fn != 'function' )
            throw new Error('Type error: map expecting first parameter to be a function not ' + typeof fn);

        if( self === undefined ) self = this;

        var ar = [];
        for( var i = 0; i < this; ++i )ar.push(fn.call(self, i, i, undefined))
        return ar;
    };

    N.abbr = function() {
        var s = this.toString();
        var order = Math.floor((s.length - 1) / 3);
        if( order > 0 ) return s.substr(0, s.length - 3 * order) + [, 'K', 'M', 'B'][Math.min(3, order)];
        else return s;
    };

    N.toPos = function() {
        var s = this % 10, sfx = 'th', d;
        if( d = this % 100 - s == 10 ) return this + '-th';
        switch( s ) {
            case 1: sfx = 'st'; break;
            case 2: sfx = 'nd'; break;
            case 3: sfx = 'rd'; break;
        }
        return this + '-' + sfx;
    };

    N.toRoman = (function() {
        var rn = ['IIII','V','XXXX','L','CCCC','D','MMMM'];

        return function( original ) {
            if( this < 1 || this > 4000 ) return this;
            var n = this, res = '';
            original = !!original;
            for( var i = 0, item; item = rn[i]; ++i ) {
                var x = item.length + 1;
                var d = n % x;
                res = rn[i].substr(0, d) + res;
                n = (n - d) / x;
            }

            if( !original )
                res = res.replace(/DCCCC/g, 'CM').replace(/CCCC/g, 'CD').replace(/LXXXX/g, 'XC')
                        .replace(/XXXX/g, 'XL').replace(/VIIII/g, 'IX').replace(/IIII/g, 'IV');
            return res;
        };
    })();

    N.toAgoInterval = (function() {
        /**
         *    Convert milliseconds since epoch to nice message
         *    saying how long ago something happened
         */
        var periods = ["second", "minute", "hour", "day", "week", "month", "year", "decade"];
        var lengths = [1, 60, 3600, 86400, 604800, 2630880, 31570560, 315705600];

        return function() {
            var interval = (Date.now() - this) / 1000, i, n;
            for( i = lengths.length - 1; i >= 0 && (n = Math.floor(interval / lengths[i])) < 1; --i );
            if( i == -1 || i == 0 && n < 20 ) return 'just now';

            return (n > 1 ? n : 'a') + ' ' + periods[i] + (n > 1 ? 's' : '') + ' ago';
        };
    })();

    /**
     *    Deviate this number by specified quantities
     *    (10).dev(margins[, relative])
     *    (10).dev(lower, upper[, relative])
     */
    N.dev =
    N.deviate = function() {
        var upper, lower = arguments[0], relative = false;
        switch( arguments.length ) {
            case 0: throw new Error('To few arguments for deviate'); break;
            case 1: upper = lower; break;
            case 2:
                if( typeof arguments[1] == 'boolean' ) {
                    upper = lower;
                    relative = arguments[1];
                }
                else
                    upper = arguments[1];
                break;
            default:
                upper = arguments[1];
                relative = !!arguments[2];
                break;
        }
        if( relative ) {
            upper = this * upper;
            lower = this * lower;
        }
        return this - lower + Math.random() * (upper + lower);
    };

})();


