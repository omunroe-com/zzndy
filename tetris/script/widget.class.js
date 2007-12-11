// html widget class

function Widget()	{

}

Widget.make = function(tag, attrs, content)	{
	var elt = document.createElement(tag);
	if(attrs)
		for([name, val] in attrs)	{
			if(val instanceof Function) elt[name] = val;
			else elt.setAttribute(name, val);
		}
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