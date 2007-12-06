// html widget class

function Widget()	{

}

Widget.makeElt = function(tag, attrs, content)	{
	var elt = document.createElement(tag)
	if(attrs)
		for([name, val] in attrs)
			elt.setAttribute(name, val);
	if(content)
		if(typeof content == 'string') elt.innerHTML = content;
		else elt.appendChild(content);
	return elt;
}
