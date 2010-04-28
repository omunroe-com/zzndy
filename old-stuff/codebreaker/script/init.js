// JScript source code

var version = '2.rc3';

window.onload = function init()
{
	// COOKIE CODEBREAKER_LANG
	load_cookies();
	load_language(language);	
	deactivate('game');
	select(document.getElementById('new_game'));
	update_info();
	fill_stats();
	fill_edit();
	fill_language();
}

function fill_language()	{
	var j=0, text='<table class="menu_table">';
	for(var i in lang_col)	{
			text += '<tr><td><a id="'+i+'" onmouseover="javascript:mouse_over(this,0)" onmouseout="javascript:mouse_out(this,0)" onmousedown="javascript:deselect_lang(this)" onmouseup="javascript:select_lang(this)"'+(i==language?' class="selected"':'')+'><table class="lang"><tr><td>'+lang_col[i]['lang_name']+'</td><td width="24px"><img src="./images/'+i+'.png" /></td></tr></table></a></td></tr>';
			++j;
	}
	document.getElementById('lang_selector').innerHTML = text+'</table>';
}

function fill_stats()
{
	document.getElementById('appCodeName').innerHTML = window.navigator.appCodeName;
	document.getElementById('appName').innerHTML = window.navigator.appName;
	document.getElementById('userAgent').innerHTML = window.navigator.userAgent;
	document.getElementById('cookieEnabled').innerHTML = window.navigator.cookieEnabled?'yes':'no';
	document.getElementById('install_date').innerHTML = getCookie('codebreaker_date');
}

function load_cookies()
{
	if(getCookie('codebrk_installed')!='yes')	{
		var exp = new Date();
		exp.setTime(exp.getTime() + 1000*60*60*24*365);
		setCookie('codebrk_installed', 'yes', exp.toUTCString());
		alert((new Date(0)).toUTCString());
		setCookie('codebrk_date', (new Date(0)).toUTCString(), exp.toUTCString());
		save_cookies();
		return;
	}
	opt['tries'].value = parseInt(getCookie('codebrk_tries'));
	opt['lenght'].value = parseInt(getCookie('codebrk_lenght'));
	opt['symbols'].value = parseInt(getCookie('codebrk_symbols'));

	language = getCookie('codebrk_language');	
	nickname = getCookie('codebrk_nickname');
}

function save_cookies()
{
	var exp = new Date();
	exp.setTime(exp.getTime() + 1000*60*60*24*365);
	setCookie('codebrk_tries',	opt['tries'].value, exp.toUTCString());
	setCookie('codebrk_lenght',	opt['lenght'].value, exp.toUTCString());
	setCookie('codebrk_symbols',opt['symbols'].value, exp.toUTCString());
	
	setCookie('codebrk_language', language, exp.toUTCString());
	setCookie('codebrk_nickname', nickname, exp.toUTCString());
}
