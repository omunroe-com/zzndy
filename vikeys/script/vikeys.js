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

this.ViKeysTrainer = function(ctx, stdout)
{
	this.start = function()
	{
		var d = 1;
        var handle;
        var score = 0;
        var lvl = 0;
        var delay = 1200;
        var special = ['yubn.'];

        function out(msg)  {
            stdout.innerHTML = ['Score: ', score, '; ', msg].join('');
        }

        window.onkeypress = function(e)
        {
                if(dir[d].charCodeAt(0) == e.charCode)
                {
                    window.clearTimeout(handle);
                    score += lvl;
                    out('Bingo');
                    handle = window.setTimeout(f, 300);
                }
                else
                {
                    window.clearTimeout(handle);
                    out('Wrong, <em>' + dir[d] + '</em> expected.');
                    handle = window.setTimeout(f, 300);
                }
        }

		var f = function()
		{
            var nd;
            do{
                nd = 1 + Math.floor(Math.random()*(dir.length - 1));
                }while(nd == d || (lvl < special.length && special[lvl].indexOf(dir[nd]) != -1));
                
            d = nd;
            out(dir[d]);
			ctx.clearRect(-1, -1, 2, 2);
			draw(ctx, d);
			handle = window.setTimeout(f, delay);
		}

		f();
	}
}
//})()
