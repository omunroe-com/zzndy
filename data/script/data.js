(function(){
BarView = function(elt)
{
	this.elt = elt;
}

BarView.prototype.update = function(pct)
{
	this.elt.style.width = pct + '%';
}

ProgressMeter = function(view)
{
	this._scale = 1;
	this._progress = 0;

	this._views = [view];
	this._scales = [];
}

ProgressMeter.prototype.reset = function()
{
	this._progress = 0;
	this.refresh();
}

ProgressMeter.prototype.scale = function(minpct, maxpct)
{
	this._scales.push(this._scale);
	this._scale = 1
}

ProgressMeter.prototype.progress = function(pct)
{
	if(!this.done() && pct > 0)
	{
		this._progress  = Math.min(100, this._progress + pct);
		var p = this._progress;
		this.refresh();
	}
}

ProgressMeter.prototype.done = function()
{
	return this._progress >= 100;
}

ProgressMeter.prototype.refresh = function()
{
	var vs = this._views;
	var p = this._progress;
	window.setTimeout(function(){vs.forEach(function(v){v.update(p)})}, 1);
}
})()
