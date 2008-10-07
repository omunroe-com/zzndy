/*
Copyright (c) 2008 Andriy Vynogradov
      based on Dan Webb's dombuilder

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial 
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
IN THE SOFTWARE.
*/
(function($){
	var IE_TRANSLATIONS = {'class': 'className', 'for': 'htmlFor'}

	function setAttributeIe(elt, name, value)
	{
		if(name in IE_TRANSLATIONS) elt.setAttribute(IE_TRANSLATIONS[name], value);
		else if(name == 'style') elt.style.cssText = value;
		else if(name.match(/^on/)) elt[name] = new Function(value);
		else elt.setAttribute(name, value);
	}

	function mkElt(name, attrs, children)
	{
		if(navigator.userAgent.match(/MSIE/))
			function mkEmptyElt(name, attrs)	{
				var elt = document.createElement(attrs.name ? '<' + name + ' name="' + attrs.name + '>' : name);
				for(var i in attrs)
					if(typeof attrs[i] != 'function')
						setAttributeIe(elt, i, attrs[i]);

				return elt;
			}
		else
			function mkEmptyElt(name, attrs)	{
				var elt = document.createElement(name);

				for(var i in attrs)
					if(typeof attrs[i] != 'function')
						elt.setAttribute(i, attrs[i]);

				return elt;
			}

		mkElt = function(name, attrs, children)	{
			attrs = attrs || {};
			children = children || [];

			if(children.length == 0 && (typeof attrs == 'string' || length in attrs)) {children = attrs; attrs = {};}
			if(children.nodeName || typeof children == 'string') children = [children];

			var elt = mkEmptyElt(name, attrs);

			for(var i=0; i<children.length; ++i)	{
				if(typeof children[i] == 'string') children[i] = document.createTextNode(children[i]);
				elt.appendChild(children[i]);
			}

			return elt;
		}
	}

	function tagFn(name)	{
		return function(attrs, children)	{
			return mkElt(name, attrs, children);
		}
	}
	
	$.initDomGen = function(elts)
	{
		var elts = elts || ('p|div|span|canvas|strong|em|img|table|tr|td|th|thead|tbody|tfoot|pre|code|' + 
			'h1|h2|h3|h4|h5|h6|ul|ol|li|form|input|textarea|legend|fieldset|select|option|blockquote|' +
			'cite|br|hr|dd|dl|dt|address|a|button|abbr|acronym|script|link|style|bdo|ins|del|object|' +
			'param|col|colgroup|optgroup|caption|label|dfn|kbd|samp|var').split("|");

		var name, i=0;
		while(name = elts[i++])
			$['$' + name] = tagFn(name);

		return $;
	}
 })(this)
