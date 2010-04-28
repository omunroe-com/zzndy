// JScript source code

function debug(object)
{
	report = '<table><tr><td colspan=2>'+object+'</td></tr>';
	for(var i in object)	{
		report += '<tr><td><b>'+i+'</b></td><td>'+object[i]+'</td></tr>';
	}
	document.getElementById('debug').innerHTML = report+'</table>';
	document.getElementById('debug').style.zIndex = 1000;
	document.getElementById('debug').style.visibility = 'visible';
}

function setCookie(name, value, expires)
{
	document.cookie = name + '=' + escape(value)+'; expires='+expires + ';';
	//"; expires="+expires.toUTCString()+"; path="+path+"; domain="+domain+"; secure";
}

function getCookie(name)
{
	var fr = document.cookie.indexOf(name+'=');
	if(fr==-1)return '';
	fr += name.length+1;
	var to = document.cookie.indexOf(';', fr);
	if(to == -1) to = document.cookie.length;
	return unescape(document.cookie.substring(fr, to));
}

function delCookie(name)
{
	alert('deleting '+name);
	var exp=new Date();
	var cval = getCookie(name);
	document.cookie=name+"="+cval+"; expires=0; ";
}
