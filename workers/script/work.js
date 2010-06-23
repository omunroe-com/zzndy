
importScripts('progress.js');

var progress = new ProgressBar();
progress.report = function(progress)
{
	postMessage(progress);
}

var prog = -1;

var max1 = 800;
var max2 = 400;


function frame()
{
	progress.set((++prog) / max1);
	if(prog <= max1)
		setTimeout(frame, 30);
	else	{
		prog = -1;
		progress.start(.6, 1);
		frame2();
	}
}

function frame2()
{
	progress.set((++prog) / max2);
	if(prog <= max2)
		setTimeout(frame2, 30);
	else
		close();
}

progress.start(0, .6);
frame();
