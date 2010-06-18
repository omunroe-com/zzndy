
function ProgressBar()
{
	this.progress = 0;
	this.threshold = 0.01;
	this.reported = -Infinity;
}

ProgressBar.prototype.set = function(progress)
{
	if(progress > 1) progress = 1;
	if(progress < 0) progress = 0;
	var d = progress - this.reported;
	this.progress = progress;

	if(d >= this.threshold || (progress == 1 && progress != this.reported)){
		this.reported = this.progress;
		this.report.call(null, this.progress);
	}
}
