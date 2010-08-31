//(function(){
var dir = ['', 'b', 'j', 'n', 'h', '.', 'l', 'y', 'k', 'u'];

function draw(c, d)
{
	var deg = Math.PI / 180;
	var r = [0, 225*deg, 180*deg, 135*deg, 270*deg, 0, 90*deg, 315*deg, 0, 45*deg];

	draw = function(c, d, stroke)	{
        
        
			c.save()
				.rotate(r[d])
                .clearRect(-1, -1, 2, 2);
                
		if(d == 5)  {
            if(stroke)
            c.strokeCircle(0, 0, .4);
            else
			c.fillCircle(0, 0, .4)
        }
        else{
				c.beginPath()
				.moveTo(0, -1)
				.lineTo(.7, -.25)
				.lineTo(.13, -.5)
				.lineTo(.15, 1)
				.lineTo(-.15, 1)
				.lineTo(-.1, -.5)
				.lineTo(-.7, -.25)
				.closePath();
            if(stroke)
                c.stroke();
            else
				c.fill()
        }
		    c.restore();
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
        var success = 0;
        var maxsc = 10;
        var delay = [1200, 1100, 1000, 1200, 1100, 1000, 900, 900, 1000, 1000];
        var special = ['yubn.', 'yubn.', 'yubn.', '.', '.', '.', '.'];
        var number = [1, 2, 3, 1, 2, 3, 3, 3, 4, 4];

        function out(msg)  {
            stdout.innerHTML = ['Score: ', score, ', Level ',lvl+1,' [',success,'/',maxsc,']; ', msg].join('');
        }

        function okp(e)
        {
            window.removeEventListener('keypress', okp, false);
            window.clearTimeout(handle);

            if(dir[d].charCodeAt(0) == e.charCode)
            {
                ctx.strokeStyle = 'white';
                score += Math.round((5+Math.pow(lvl+1, 1.5)*5)/5)*5;

                if(++success == maxsc) {
                    success = 0;
                    if(++lvl >= delay.length){
                        out('You won!');
                    }
                }

                if(lvl < delay.length)
                {
                    out('Bingo');
                    handle = window.setTimeout(f, 300);
                }
            }
            else
            {
                out('Wrong, <em>' + dir[d] + '</em> expected.');
                ctx.strokeStyle = 'maroon';
                handle = window.setTimeout(f, 300);
            }

            draw(ctx, d, true);
        }

		var f = function()
		{
            var nd;
            do{
                nd = 1 + Math.floor(Math.random()*(dir.length - 1));
            }while(nd == d || (lvl < special.length && special[lvl].indexOf(dir[nd]) != -1));
                
            d = nd;

            window.removeEventListener('keypress', okp, false);
            window.addEventListener('keypress', okp, false);
            
            // Only show hint on levels where new keys are introduced.
            if(lvl == 0 || lvl == 3)
            out(dir[d]);
            else 
            out();
            ctx.fillStyle = "#eeeeee";
			draw(ctx, d);
			handle = window.setTimeout(f, delay[lvl]);
		}

		f();
	}
}
//})()
