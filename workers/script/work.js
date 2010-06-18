
importScripts('progress.js');

var progress = new ProgressBar();
progress.report = function(progress)
{
	postMessage(progress);
}

var prog = -1;
function frame()
{
	progress.set((++prog) / 1000);
	if(prog <= 1000)
		setTimeout(frame, 10);
	else
		close();
}

frame();
