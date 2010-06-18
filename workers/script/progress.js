
function ProgressBar()
{
	this.progress = 0;
	this.threshold = 0.0001;
	this.reported = -Infinity;
	this.scales = [];
}

ProgressBar.prototype.set = function(progress)
{
	if(progress > 1) progress = 1;
	if(progress < 0) progress = 0;

	if(this.scales.length >0){
		var scale = this.scales.pop();
		progress = scale[0] + progress * (scale[1] - scale[0]);
		if(progress < 0 || progress > 1)
			throw new Error('Progress Bar subdivisions failed.');

		this.scales.push(scale);
	}

	var d = progress - this.reported;
	this.progress = progress;

	if(d >= this.threshold || (progress == 1 && progress != this.reported)){
		this.reported = this.progress;
		this.report.call(null, this.progress);
	}
}

ProgressBar.prototype.start = function(start, end)
{
	if(this.scales.length>0)this.scales.pop();
	this.scales.push([start, end]);
}

ProgressBar.prototype.end = function()
{
	if(this.scales.length>0)this.scales.pop();
}
