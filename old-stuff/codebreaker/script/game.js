
var gamon = false;		// true if there is a game running
var animation = false;	// toggles on/off anumation
var anim_timer = null;
var current_char = 0;
var current_try = 0;
                         
function reset()
{
	opt['tries'].value = opt['tries'].deflt;
	opt['symbols'].value = opt['symbols'].deflt;
	opt['lenght'].value = opt['lenght'].deflt;
	fill_edit();
	save_cookies();
}

function hint(what, where)
{
	document.getElementById(where).innerHTML = lang_col[language]['help'][what.id];
	window.status = lang_col[language]['help'][what.id];	// TODO: HTML in the status is not good.
}
function clear_hint(what)
{
	document.getElementById(what).innerHTML = '';
	window.status = '';
}

function random(max)
{
	return Math.floor(Math.random()*max);
}

function new_game()
{
	deselect();
	clear_hint('help_status');
	gamon = true;
	current_char = 0;
	current_try = 0;
	document.getElementById('nickname').blur();
	document.getElementById('tries_left').innerHTML = opt['tries'].value;
	guess = new Array(opt['lenght'].value);
	secret = new Array(opt['lenght'].value);
	
	for(var i=0; i<opt['lenght'].value; ++i)	{
		guess[i] = '.';
		secret[i] = String.fromCharCode(65+random(opt['symbols'].value));
	}
	
	activate('game');
	select(document.getElementById('game'));
	var wid = 500/(opt['lenght'].value+1);
	var text = '<table><tr>';
	for(var i=0;i<opt['lenght'].value;++i)
	{
		text += '<td id="guess'+i+'" width="'+wid+'px" class="'+(i?'normal':'active')+'">&#x25cf;</td>';
	}
	text += '<td id="animator" width="'+wid+'px">&#x25cb;</td></tr></table>';
	document.getElementById('guess_table').innerHTML = text;
	
	text = '<table><tr>';
	for(var i=0;i<opt['tries'].value;++i)	{
		text += '<tr>';
		for(var j=0;j<opt['lenght'].value;++j)	{
			text += '<td id="hist'+i+'_'+j+'" width="'+wid+'px">&#x25cf;</td>';	
		}
		text += '<td class="hint" id="hint'+i+'" width="'+wid+'px">&#x25cf;</td></tr>';
	}
	text += '</table>';
	
	document.getElementById('history_table').innerHTML = text;		
	
	if(animation)	{
		anim_timer = window.setInterval("animate()", 200);
	}
	
}

// *** animation *************************************************************

//var frames = new Array('.','o','O','0','O','o');
var frames = new Array('&#x2022;', '&#x25cf;');
var frame=0;

function animate()
{
	++frame;
	if(frame>=frames.length)	{
		frame = 0;
	}
	document.getElementById('animator').innerHTML = frames[frame];
}


// *** key pressing **********************************************************

var vk_enter = 13;
var vk_space = 32;
var vk_tilde = 96;	// actually this is for '`' and '~' is 126;
var vk_left = 37;	// mozilla only
var vk_left2 = 44;	// ','
var vk_left3 = 60;	// '<'
var vk_right = 39;	// mozilla only
var vk_right2 = 46;	// '.'
var vk_right3 = 62;	// '>'

document.onkeypress = function key_press(event)
{
	var key, code;
	if(navigator.appName.indexOf('Microsoft') != -1)	{
		event = window.event;
		code = event.keyCode;
	}
	else 
	{
		code = event.charCode;
		if(code == 0)	code = event.keyCode;
	}
	event.cancelBubble = true;

// *** textual prosessing ***
	var key = String.fromCharCode(code).toLowerCase();
	if(event.shiftKey == true)	{	// menu
		switch(key)	{
			case 'e':
				change_selection('new_game');
				break;
			case 'g':
				change_selection('game');
				break;
			case 'o':
				change_selection('options');
				break;
			case 's':
				change_selection('statistics');
				break;
			case 'h':
				change_selection('help');
				break;
			case 'n':
				new_game();
				break;
		}
	}
	else if(gamon == true)	{	// game is running
		send_key(code);
		switch(code)	{
			case vk_enter:
				try_guess();
				break;
			case vk_left:
			case vk_left2:
			case vk_left3:
				move_backward();
    			break;
    		case vk_right:
			case vk_right2:
			case vk_right3:
				move_forward();
    			break;
			case vk_tilde:
				window.status = 'Code:';
				for(var i=0; i<opt['lenght'].value; ++i) window.status += secret[i];
				break;
		}
	}
	
	return true;
}
