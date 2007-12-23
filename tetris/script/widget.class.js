// html widget class

function Widget()	{

}

Widget.make = function(tag, attrs, content)	{
	var elt = document.createElement(tag);
	if(attrs)
		for([name, val] in attrs)
			if(val instanceof Function) elt[name] = val;
			else elt.setAttribute(name, val);
	if(content)
		if(typeof content == 'string') elt.innerHTML = content;
		else elt.appendChild(content);
	return elt;
}

Widget.enclose = function(parent, children)	{
	children.some(function(c){parent.appendChild(c)});
	return parent;
}

Widget.nest = function(elements)	{
	return elements.reduce(function(child, el){if(child)el.appendChild(child);return el;}, null);
}

Widget.rm = function(id)	{
	var floater = document.getElementById(id);
	if(floater) floater.parentNode.removeChild(floater);
}

Widget.floater2over = function(floater, points)	{
	Widget.enclose(floater, [
		Widget.make('div', {id: 'gameover-msg'}, '◆ Game Over ◆'), 
		Widget.make('div', {id: 'gameover-top'}, 'You scored <em>'+points+'</em> points')
	]);
}

Widget.floater2pause = function(floater)	{
	Widget.enclose(floater, [
		Widget.make('div', {id: 'pause-msg'}, '● Pause ●'), 
		Widget.make('div', {id: 'pause-top'}, 'Press <em>pause</em> or <em>p</em> to continue')
	]);
}

Widget.floater2start = function(floater, click)	{
	var button = Widget.make('button', {id: 'runit', onclick: click}, 'Start');
	Widget.enclose(floater, [button]);
	button.focus();
}


