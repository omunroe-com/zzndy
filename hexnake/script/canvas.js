/*
Copyright (c) 2008 Andriy Vynogradov

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
(function(){

if(!('CanvasRenderingContext2D' in this)) // WebKit
{
	var ctx = document.createElement('canvas').getContext('2d')
	var C = ctx.__proto__
	var G = ctx.createLinearGradient(0, 0, 0, 0).__proto__
}
else
{
	var C = CanvasRenderingContext2D.prototype
	var G = CanvasGradient.prototype
}

// Make canvas' methods chainable
('restore,rotate,save,scale,translate,arc,arcTo,bezierCurveTo,beginPath,clip,closePath,lineTo,moveTo,quadraticCurveTo,rect,stroke,strokeRect,clearRect,fill,fillRect,clip,drawImage,drawImageFromRect')
.split(',').forEach(function(method){
	if(method in C)	{
		var meth = C[method]
		C[method] = function()	{return meth.apply(this, arguments) || this}
	}
})

// Make gradient's addColorStop chainable
var meth = G.addColorStop
G.addColorStop = function(){meth.apply(this, arguments); return this}

// Alter canvas mehods to add perspective
var mt = C.moveTo
var lt = C.lineTo

function applyPerspective(x, y)	{
	x = x * (1 - y*this.perspective)
	y = y * (1 - y*this.perspective / 1.4)
	return [x, y]
}

C.perspective = 0;

C.moveTo = function(x, y)
{
	return mt.apply(this, applyPerspective.call(this, x, y))
}

C.lineTo = function(x, y)
{
	return lt.apply(this, applyPerspective.call(this, x, y))
}

C.strokeCircle = function(x, y, r)
{
	return this.beginPath()
	 .arc(x, y, r, 0, Math.PI*2, true)
	 .closePath()
	 .stroke()
}

C.fillCircle = function(x, y, r)
{
	return this.beginPath()
	 .arc(x, y, r, 0, Math.PI*2, true)
	 .closePath()
	 .fill()
}

C.clear = function()
{
	if('size' in this && 'origin' in this)
		this.clearRect(this.origin.x, this.origin.y, this.size.w, this.size.h)
	
	return this
}

})()
