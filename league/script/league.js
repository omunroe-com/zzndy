function League()	{
	this.root = '/league/';
	this.body = null; 
}

var docroot = 'league/';

var league = new League();
var ready = false;
var menuHtml;

var initData = {
	menu:	[
		{name: 'Home', href: 'home'}, {name: 'Top players', href: 'top'}
	]
}
// load modules
$.getScript(league.root + 'script/module/menu+skeleton.js');

$(function()	{
	league = new League();
	league.body = document.getElementsByTagName('body')[0];
})
