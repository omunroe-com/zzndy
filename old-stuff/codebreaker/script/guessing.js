
var guess, secret;

function try_guess()
{
	// *** check if the guess is full ***
	var placed = 0, guessd = 0;
	var style = 'used';
	
	for(i in guess)	{
		if(guess[i] == '.') return;
	}
	// *** check the guess ***		
	if(guess.join() == secret.join())	{
		style = 'win';
		placed = 4;
	}
	else	{
		var dummy1 = new Array(opt['lenght'].value);
		var dummy2 = new Array(opt['lenght'].value);
		var i=0;
		for(i=0; i<opt['lenght'].value; ++i)	{
			if(guess[i] == secret[i])	{
				++placed;
			}
			else	{
				dummy1[i-placed] = guess[i];
				dummy2[i-placed] = secret[i];
			}
		}
		for(var j=0; j<i-placed; ++j)	{
			for(var k=0; k<i-placed; ++k)	{
				if(dummy1[j] == dummy2[k])	{
					++guessd;
					dummy2[k] = '~';
					continue;
				}
			}
		}
	}
	if(current_try == opt['tries'].value-1)	{
		style = 'loose'; 
	}
	fill_line(style, placed, guessd);
	++current_try;
	document.getElementById('tries_left').innerHTML = opt['tries'].value - current_try;
	switch(style)	{
		case 'win':
			win_game();
			break;
		case 'loose':
			loose_game();
			break;
	}
}

// *** end game **************************************************************
function win_game()
{
	for(var i=0; i<opt['lenght'].value; ++i)	{
		document.getElementById('guess'+i).className = 'win';
	}
	end_game('win');
}

function loose_game()
{
	for(var i=0; i<opt['lenght'].value; ++i)	{
		object = document.getElementById('guess'+i);
		object.className = 'loose';
		object.innerHTML = secret[i];
	}
	end_game('loose');
}

function end_game(how)
{
	if(!gamon) return;
	gamon = false;
	document.getElementById('game').className = 'grayed';
	window.status = lang_col[language][how]['status'];
	window.alert( lang_col[language][how]['alert']);
}

function fill_line(style, placed, guessd)
{
	for(var i=0; i<opt['lenght'].value; ++i)	{
		object = document.getElementById('hist'+current_try+'_'+i);
		object.innerHTML = guess[i];
		object.className = style;
	}
	object = document.getElementById('hint'+current_try);
	object.innerHTML = placed+':'+guessd;
	object.className = style;
}

// *** navigation over guess *************************************************
function move_forward()
{
	document.getElementById('guess'+current_char).className = '';
		if(++current_char == opt['lenght'].value)	{
			current_char = 0;
		}
	document.getElementById('guess'+current_char).className = 'active';		
}
function move_backward()
{
	document.getElementById('guess'+current_char).className = '';
		if(--current_char < 0)	{
			current_char = opt['lenght'].value - 1;
		}
	document.getElementById('guess'+current_char).className = 'active';		
}
// ***************************************************************************
function send_key(code)
{
	if(code>=65&&code<65+opt['symbols'].value ||
		code>=97&&code<97+opt['symbols'].value)	{	// player is typing the code
		var letter = String.fromCharCode(code).toUpperCase();
		document.getElementById('guess'+current_char).innerHTML = letter;
		guess[current_char] = letter;
		move_forward();
	}
}

