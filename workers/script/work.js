
importScripts('progress.js');

var progress = new ProgressBar();
progress.report = function(progress)
{
	postMessage(progress);
}

var prog = -1;

var max1 = 200;
var max2 = 100;


function frame()
{
	progress.set((++prog) / max1);
	if(prog <= max1)
		setTimeout(frame, 50);
	else	{
		prog = -1;
		progress.start(.4, 1);
		frame2();
	}
}

function frame2()
{
	progress.set((++prog) / max2);
	if(prog <= max2)
		setTimeout(frame2, 50);
	else
		close();
}

progress.start(0, .4);
frame();
