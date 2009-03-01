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
(function() {

    var C, G;

    if ( !('CanvasRenderingContext2D' in this) ) // WebKit
    {
        var ctx = document.createElement('canvas').getContext('2d');
        C = ctx.__proto__;
        G = ctx.createLinearGradient(0, 0, 0, 0).__proto__;
        CanvasRenderingContext2D = {prototype:C};
        CanvasGradient = {prototype:G};
    }
    else
    {
        C = CanvasRenderingContext2D.prototype;
        G = CanvasGradient.prototype;
    }

    // Make canvas' methods chainable
    ('restore,rotate,save,scale,translate,arc,arcTo,bezierCurveTo,beginPath,clip,closePath,lineTo,moveTo,quadraticCurveTo,rect,stroke,strokeRect,clearRect,fill,fillRect,clip,drawImage,drawImageFromRect')
            .split(',').forEach(function( method ) {
        if ( method in C ) {
            var meth = C[method];
            C[method] = function() {
                return meth.apply(this, arguments) || this
            };
        }
    });

    // Make gradient's addColorStop chainable
    var addColorStop = G.addColorStop;
    G.addColorStop = function() {
        addColorStop.apply(this, arguments);
        return this;
    };

    C.strokeCircle = function( x, y, r )
    {
        return this.beginPath()
                .arc(x, y, r, 0, Math.PI * 2, true)
                .closePath()
                .stroke();
    };

    C.fillCircle = function( x, y, r )
    {
        return this.beginPath()
                .arc(x, y, r, 0, Math.PI * 2, true)
                .closePath()
                .fill();
    };

    C.vLine = function( x, y, h ) {
        return this.fillRect(x, y, 1, h);
    };

    C.hLine = function( x, y, w ) {
        return this.fillRect(x, y, w, 1);
    };

    C.fillPoly = function( vx ) {
        this.beginPath().moveTo(vx[0], vx[1]);
        for ( var i = 2; i < vx.length; i += 2 )this.lineTo(vx[i], vx[i + 1]);
        return this.closePath().fill();
    };

    C.strokePoly = function( vx ) {
        this.beginPath().moveTo(vx[0], vx[1]);
        for ( var i = 2; i < vx.length; i += 2 )this.lineTo(vx[i], vx[i + 1]);
        return this.stroke();
    };

    C.clear = function()
    {
        if ( 'size' in this && 'origin' in this )
            this.clearRect(this.origin.x, this.origin.y, this.size.w, this.size.h)
        return this;
    };

})();
