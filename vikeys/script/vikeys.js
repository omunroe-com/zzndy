//(function(){
var dir = ['', 'b', 'j', 'n', 'h', '.', 'l', 'y', 'k', 'u'];

function draw(c, d)
{
	var deg = Math.PI / 180;
	var r = [0, 225*deg, 180*deg, 135*deg, 270*deg, 0, 90*deg, 315*deg, 0, 45*deg];

	draw = function(c, d)	{
		return d == 5 
			?c.fillCircle(0, 0, .4)
			:c.save()
				.rotate(r[d])
				.beginPath()
				.moveTo(0, -1)
				.lineTo(.7, -.25)
				.lineTo(.13, -.5)
				.lineTo(.15, 1)
				.lineTo(-.15, 1)
				.lineTo(-.1, -.5)
				.lineTo(-.7, -.25)
				.closePath()
				.fill()
				.restore();
	}

	draw(c,d);
}

this.ViKeysTrainer = function(canvas, stdout)
{
	var ctx = canvas.getContext('2d');

	this.start = function()
	{
		var d = 1;
		var f = function()
		{
			ctx.clearRect(-1, -1, 2, 2);
			draw(ctx, d);
			stdout.innerHTML = d + '. ' + dir[d];
			if(++d>=dir.length)d = 1;
			window.setTimeout(f, 800);
		}

		f();
	}
}
//})()
