//// Helper Functions ////////////////////////////////////////////////

/*	miscellaneous helper functions
 *	and simple structures
 */

String.prototype.pad = function(n, s)	{
	s = s || ' ';
	var l = this.length;
	return (Math.abs(n)>l)?(n>0?this+s.rep(n-l):s.rep(Math.abs(n)-l)+this):this.toString();
}

String.prototype.rep = function(n)	{
	return Array(n+1).join(this);
}

Number.prototype.zf = 
Number.prototype.zerofill = function(w, p)	{
	if(p) return this.toFixed(p).pad(-w, '0')
	return this.toString().pad(-w, '0')
}

Number.prototype.pad = function(w, p, s)	{
	if(p) return this.toFixed(p).pad(w, s)
	return this.toString().pad(w, s)
}

String.prototype.fmt = function(a)	{
	var r = this
	for(var [k, v] in a)	{
		var rx = new RegExp('\\$\\{'+k+'\\}', 'g')
		r = r.replace(rx, v)
	}
	return r
}

String.prototype.format = function(a)	{
	var r = this.fmt(a);
	for(var [i, l] in a)	{
		var rx = new RegExp('\\$\\{('+i+')(?::(-|0)?(\\d+)?(?:(?:\\.)(\\d+))?)?\\}');
		var n = new Number(l), m;
		while(m = r.match(rx))	{
			var [o,,f,w,p] = m;
			if(f&&f=='-') w=-w;
			var s = (f && f == '0')?n.zerofill(w, p):(p?n.pad(w, p):l.pad(w))
			r = r.replace(o, s);
		}
	}
	return r;
}

// operators to use in reduce or 
function add(a, b){return a + b}
function mul(a, b){return a * b}
function sub(a, b){return a - b}
function div(a, b){return a / b}
function sqr(a)   {return a * a}

function limit(lower, upper, value)	{
	return Math.min(Math.max(value, lower), upper);
}

function deviate(deviation){
	return deviation && Math.floor(2*deviation * Math.random() - deviation) || 0;
}

Array.prototype.reduce = function(fn2, init)    {
        if(!(fn2 instanceof Function))fn2 = function(a){return a};
		var res = init, l = this.length;
        for(var i=0; i<l; ++i) res = fn2(res, this[i]);
        return res;
}

Array.prototype.sum = function(){return this.reduce(add, 0)}
Array.prototype.prod = function(){return this.reduce(mul, 0)}

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