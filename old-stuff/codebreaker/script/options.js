
// *** param class *********************

function param(min, max, deflt)
{
	this.min = min;
	this.max = max;
	this.deflt = deflt;
	this.value = deflt;
}

// *** end of param class **************

var opt = new Object();

opt['tries'] = new param(4, 100, 10);
opt['symbols'] = new param(2, 27, 6);
opt['lenght'] = new param(2, 10, 4);

var nickname = lang_col[language]['nickname'];

function update_info()
{
	document.getElementById('tries2').innerHTML = opt['tries'].value;	
	document.getElementById('symbols2').innerHTML = opt['symbols'].value;
	document.getElementById('lenght2').innerHTML = opt['lenght'].value;
	document.getElementById('nickname').value = nickname;
	document.getElementById('combinations').innerHTML = Math.pow(opt['lenght'].value, opt['symbols'].value);
	document.getElementById('progress').innerHTML = Math.pow(opt['lenght'].value, opt['symbols'].value)/opt['tries'].value;
}

function fill_edit()
{
	document.getElementById('tries').value = opt['tries'].value;	
	document.getElementById('symbols').value = opt['symbols'].value;
	document.getElementById('lenght').value = opt['lenght'].value;
	document.getElementById('nickname').value = nickname;
}

function change(object)
{
	var val = parseInt(object.value);
	var option = object.id;
	
	if( val >= opt[option].min && val <= opt[option].max)	{
		opt[option].value = val;
		update_info();
		save_cookies();
	}
	else object.value = opt[option].value;
}

function change_nick(object)
{
	nickname = object.value;
	save_cookies();
}
