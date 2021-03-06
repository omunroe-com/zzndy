//// Helper Functions ////////////////////////////////////////////////

Object.prototype.map = function(fn, thisObj)	{
	var res = {}
	for(var i in this) if(typeof this[i] != 'function')
		res[i] = fn.call(thisObj, this[i], i, this)
	return res
}

Object.prototype.clone = function()	{
	var newOne = {}
	for(var i in this)
		newOne[i] = this[i]
		
	return newOne
}

Object.prototype.extend = function()	{
	for(var i=0; i<arguments.length; ++i)
		for(var field in arguments[i])
			this[field] = arguments[i][field]
			
	return this
}

Object.prototype.toQuery = function()	{
	var res = []
	for(var i in this) if(typeof this[i] != 'function')
		res.push(i + '=' + encodeURIComponent(this[i]))
	return res.join('&')
}

Object.prototype.toCss = function()	{
	var res = []
	for(var i in this) if(typeof this[i] != 'function')
		res.push(i + ':' + this[i] + ((typeof this[i] == 'number' && Math.abs(this[i]) > 1)?'px':''))
	return res.join(';')
}

String.prototype.pad = function(n, s)	{
	s = s || ' ';
	var l = this.length;
	return (Math.abs(n)>l)?(n>0?this+s.rep(n-l):s.rep(Math.abs(n)-l)+this):this.toString();
}

String.prototype.rep =
String.prototype.x = function(n)	{
	return (new Array(n+1)).join(this)
}

String.prototype.trim = function(){ return this.replace(/^\s+|\s+$/g, "")}
function trim(str){return str.trim()}

String.prototype.trimsplit =
String.prototype.ts = function(splitter)	{
	return this.split(splitter).map(trim);
}
String.prototype.isIn = function(str)	{
	return str.indexOf(this) != -1;
}
String.prototype.has = function(str)	{
	return this.indexOf(str) != -1;
}
String.prototype.toArray = function(){return this.split('')}
String.prototype.map = function(fn, self)	{return this.toArray().map(fn, self)}
String.prototype.reduce = function(fn, init)	{return this.toArray().reduce(fn, init)}

Number.prototype.zf =
Number.prototype.zerofill = function(w, p)	{
	if(p) return this.toFixed(p).pad(-w, '0')
	return this.toString().pad(-w, '0')
}

Number.prototype.pad = function(w, p, s)	{
	if(p) return this.toFixed(p).pad(w, s)
	return this.toString().pad(w, s)
}

Number.prototype.times = function(fn, self)	{
	var res = []
	if(typeof fn == 'function')
		for(var i=0; i<this; ++i) res.push(fn.call(this, i, self))
	else if(typeof fn == 'object' || fn instanceof Array)
		for(var i=0; i<this; ++i) res.push(fn.clone())
	else
		for(var i=0; i<this; ++i) res.push(fn)

	return res;
}

Number.prototype.constrain = function(min, max, bounce)	{
	if(bounce)	{
		if(this < min) return (2*min - this).constrain(min, max)
		else if(this > max) return (2*max - this).constrain(min, max)
	}
	return Math.min(Math.max(min, this), max)
}

Number.prototype.positive = function()	{return this.constrain(1, Infinity)}
Number.prototype.negative = function()	{return this.constrain(-Infinity, -1)}
Number.prototype.nonPositive = function()	{return this.constrain(0, Infinity)}
Number.prototype.nonNegative = function()	{return this.constrain(Infinity, 0)}

/**
 * Format is <Name[:{-|0}[width[.decimal]][base]]>
 * where
 *  name		name of a field, can also be in form "property[10].another.prop[10][11].length"
 * 	-           forces right align *
 * 	0           zerofill *,**
 * 	width		align width
 * 	decimal		decimal places**
 *  base		enumeration base**, can be one of
 * 		x			hexademical
 * 		o			octal
 * 		b			binary
 * 		n			verbose `base`, converts 1 to 1-st, 2 to 2-nd and so on
 * 		r			roman numerals
 *
 * *  - useless w/o width
 * ** - for numbers only
 * Watch the case
 */
String.prototype.format =
String.prototype.fmt = function(fmtObj)	{
	function getValue(path)	{
		var prop = path.match(/^(\.?\w+|\[\d+\])(?=\.|$|\[\d+)/)[0]
		var rest = path.replace(prop, '')
		if(rest) return getValue.call(this[prop.replace(/^\.|^\[|\]$/g, '')], rest)
		else return this[prop.replace(/^\.|^\[|\]$/g, '')]
	}

	function simpleFormat(a)	{
		var r = this
		for(var k in a)	{
			if(typeof a[k] == 'function') continue
			if(typeof k == 'string')
				var rx = new RegExp('<' + k.charAt(0).toUpperCase() + k.substr(1) + '>', 'g')
			else var rx = new RegExp('<' + k + '>', 'g')
			r = r.replace(rx, a[k])
		}
		return r
	}

	var res = simpleFormat.call(this, fmtObj)
	var opts = '(?:\\.|\\[\\d+\\])?[\\w\\[\\]\\.]*)(?::(?:(-|0)?(\\d+)(?:\\.(\\d+))?)?(r|n|x|o|b)?'

	for(name in fmtObj)	{
		var value, backUp = []
		value = backUp[0] = fmtObj[name]
		if(typeof value == 'function') continue
		if(typeof name == 'string') name = name.charAt(0).toUpperCase() + name.substr(1)
		var rx = new RegExp('<(' + name + opts + ')?>')
		var num = backUp[1] = new Number(value), m;
		while(m = res.match(rx))	{
			var match = m[0], path = m[1], align=m[2], width=m[3], deci=m[4], base=m[5]
			var useBackup = !!(path || base)
			if(path) {
				if(typeof name == 'string') path = path.charAt(0).toLowerCase() + path.substr(1)
				value = getValue.call(fmtObj, path)
				num = new Number(value)
			}
			switch(base)	{
				case 'b': value = num.toString(2); break
				case 'o': value = num.toString(8); break
				case 'x': value = num.toString(16); break
				case 'n': value = num.toPos(); break
				case 'r': value = num.toRoman(); break
			}

			res = res.replace(match, (isNaN(num) || base) ? value.pad(align == '-' ? -width : width) : (align == '0' ? num.zf(width, deci) : num.pad(width, deci)))

			if(useBackup)	{
				value = backUp[0]
				num = backUp[1]
			}
		}
	}
	return res
}

// operators to use in reduce
function add(a, b){return a + b}
function mul(a, b){return a * b}
function sub(a, b){return a - b}
function div(a, b){return a / b}
function sqr(a)   {return a * a}

if(!('reduce' in Array))
Array.prototype.reduce = function(fn2, init)	{
	var i = 0;
	if(arguments.length < 2) init = this[++i];

	var res = init, l = this.length;
	for(; i<l; ++i) res = fn2(res, this[i], i, this);
	return res;
}

Array.prototype.delayedReduce = function(delay, fn2, init, callback)	{
	var i = 0;
	if(arguments.length < 2) init = this[++i];
	var res = init, l = this.length;
	var fn = function(res, elt, idx, array){
		res = fn2(res, elt, idx, array);
		if(++idx < l)
			window.setTimeout(arguments.callee, delay, res, array[idx], idx, array);
		else if(callback) callback(res);
	}

	fn(res, this[i], i, this);
}

Array.prototype.diff = function(other)	{
	return this.map(function(x, i){
		return (x == other[i]) ? undefined : [x, other[i]]
	})
}

Array.prototype.clone = function()	{
	return [].concat(this);
}

// pick n randomly chosen items from the array
Array.prototype.pick = function(n)	{
	var picked = []
	n = Math.floor(n)
	return n.map(function(){
		if(picked.length == this.length) return undefined
		var i

		do i = Math.floor(Math.random() * this.length)
		while (i in picked)

		picked.push(i)
		return this[i]
	}, this)
}

Array.prototype.shuffle = function () {
	var i,L;
	i = L = this.length;
	while (i--)	{
		var r = Math.floor(Math.random()*L);
		var x = this[i];
		this[i] = this[r];
		this[r] = x;
	}
}

Array.prototype.sum = function(){return this.reduce(add, 0)}
Array.prototype.prod = function(){return this.reduce(mul, 0)}

Function.prototype.timeit = function(self, args)	{
	var start = new Date()
	this.apply(self, args)
	var time = (new Date()) - start
	return (time < 1000) ? time + 'ms' : (time/1000).toFixed(3) + 's'
}

Function.prototype.append = function(g){
	var f = this;
	return function(){f();g()}
}

Function.prototype.wrap = function(g){
	var f = this;
	return function(){f(g())}
}

Function.prototype.detach = function(obj)	{
	var fn = this;
	return function(){return fn.apply(obj, arguments)}
}

Number.prototype.map = function(fn, self){
	if(typeof fn != 'function') throw new TypeError('Type error: map expecting first parameter to be a function not ' + typeof fn)

	if(self === undefined) self = this

	var ar = []
	for(var i=0; i<this; ++i)ar.push(fn.call(self, i, i, undefined))
	return ar
}

Number.prototype.abbr = function()	{
	var s = this.toString();
	var order = Math.floor((s.length - 1) / 3), disp;
	if(order > 0) return s.substr(0, s.length - 3*order) + [, 'K', 'M', 'B'][Math.min(3, order)];
	else return s;
}

Number.prototype.toPos = function()	{
	var s = this % 10, sfx='th';
	if(d = this % 100 - s == 10) return this + '-th';
	switch(s)	{
		case 1: sfx = 'st'; break;
		case 2: sfx = 'nd'; break;
		case 3: sfx = 'rd'; break;
	}
	return this + '-' + sfx;
}

Number.prototype.toRoman = (function(){
	var rn = ['IIII','V','XXXX','L','CCCC','D','MMMM']

	return function(original) {
		if(this < 1 || this > 4000) return this
		var n = this
		var original = !!original, res = '', d
		for (var i=0, item; item=rn[i]; ++i) {
			var x = item.length+1;
			var d = n%x;
			res = rn[i].substr(0,d) + res;
			n = (n-d)/x;
		}

		if (!original)
			res=res.replace(/DCCCC/g,'CM').replace(/CCCC/g,'CD').replace(/LXXXX/g,'XC')
			.replace(/XXXX/g,'XL').replace(/VIIII/g,'IX').replace(/IIII/g,'IV')
		return res
	}
})()

Number.prototype.toAgoInterval = (function(){
	/**
	*	Convert milliseconds since epoch to nice message
	*	saying how long ago something happened
	*/
	var periods = ["second", "minute", "hour", "day", "week", "month", "year", "decade"]
	var lengths = [1, 60, 3600, 86400, 604800, 2630880, 31570560, 315705600]

	Number.prototype.toAgoInterval = function()	{
		var interval = (Date.now() - this) / 1000, i, n
		for(i=lengths.length - 1; i>=0 && (n = Math.floor(interval / lengths[i])) < 1; --i);
		if(i==-1 || i==0 && n < 20) return 'just now'

		return (n > 1 ? n : 'a') + ' ' + periods[i] + (n > 1 ? 's' : '') + ' ago'
	}
})()

/**
 *	Deviate this number by specified quantities
 *	(10).dev(margins[, relative])
 *	(10).dev(lower, upper[, relative])
 */
Number.prototype.dev =
Number.prototype.deviate = function()	{
	var upper, lower = arguments[0], relative = false
	switch(arguments.length)	{
		case 0: throw new Error('To few arguments for deviate'); break
		case 1: upper = lower; break
		case 2:
			if(typeof arguments[1] == 'boolean')	{
				upper = lower
				relative = arguments[1]
			}
			else
				upper = arguments[1]
			break
		default:
			upper = arguments[1]
			relative = !!arguments[2]
			break
	}
	if(relative)	{
		upper = this * upper
		lower = this * lower
	}
	return this - lower + Math.random() * (upper + lower)
}

function range(n)	{
	var ar = []
	for(i=0;i<n;++i) ar.push(i)
	return ar
}
