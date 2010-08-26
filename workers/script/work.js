
importScripts('progress.js');

var progress = new ProgressBar();
var n = 0;
var msg = ["Hello world", "Loading confidential data", "Decoding", "Decoupling", "Dechunking chunks", "Spewing Spwe"];

progress.report = function(progress)
{
	postMessage({
        progress:progress, 
        messages:[
            msg[Math.round(++n/10)%msg.length], 
            msg[Math.round((++n*2)/15)%msg.length]
        ]
    });
}

var prog = -1;

var max1 = 500 + Math.floor(Math.random() * 500);
var max2 = 200 + Math.floor(Math.random() * 300);

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
