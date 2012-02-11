//// TAG Class (html manipulation)
function Tag() {
}

Tag.get = function( simpleSel ) {
    if( simpleSel[0] == '#' )
        return document.getElementById(simpleSel.substr(1));
    return document.getElementsByTagName(simpleSel);
};

Tag.mk = function( tag, attrs, content ) {
    var elt = document.createElement(tag);
    if( attrs )for( var name in attrs )
        if( attrs[name] instanceof Function ) elt[name] = attrs[name];
        else elt.setAttribute(name, attrs[name]);
    if( content != undefined )
        if( typeof content == 'string' || typeof content == 'number' ) elt.innerHTML = content;
        else elt.appendChild(content);
    return elt;
};

Tag.addRow = function( table, cells, rowAttrs, cellsattrs ) {
    return Tag.enclose(table,
            Tag.enclose(
                    Tag.mk('tr', rowAttrs),
                    cells.map(function( c, n ) {
                        return Tag.mk('td', cellsattrs && cellsattrs[n], c);
                    })));
};

Tag.enclose = function( parent, children ) {
    if( children instanceof Array )
        children.map(function( c ) {
            parent.appendChild(c);
        });
    else parent.appendChild(children);
    return parent;
};

Tag.nest = function( elements ) {
    return elements.reduce(function( child, el ) {
        if( child )el.appendChild(child);
        return el;
    }, null);
};

Tag.rm = function( id ) {
    if( typeof id == 'string' ) {
        var floater = document.getElementById(id);
        if( floater ) floater.parentNode.removeChild(floater);
    }
    else
        if( id && id.parentNode )
            id.parentNode.removeChild(id);
};

/**
 *    Conver JSON object to a css string
 */
Tag.mkCss = function( obj ) {
    var res = '';
    for( var i in obj )
        if( typeof obj[i] != 'function' )
            res += i + ':' + obj[i] + ';';
    return res;
};

Tag.resize = function( tag, width, height ) {
    function setProperty( element, attr, value ) {
        var adjust, number, unit;
        var m = value.match(/([+-]?)(\d+?\.\d+|\d+)((?:\w+)?)/);
        adjust = m[1];
        number = m[2];
        unit = m[3];
        element.style[attr] = (adjust ? parseFloat(element.style[attr]) + parseFloat(adjust + number) : number) + unit;
    }

    setProperty(tag, 'width', width);
    setProperty(tag, 'height', height);
};

Tag.floater2over = function( floater, points ) {
    return Tag.center(Tag.enclose(floater, [
        Tag.mk('div', {id: 'gameover-msg'}, '◆ Game Over ◆'),
        Tag.mk('div', {id: 'gameover-top'}, 'You scored <em>' + points + '</em> points')
    ]));
};

Tag.floater2pause = function( floater ) {
    return Tag.center(Tag.enclose(floater, [
        Tag.mk('div', {id: 'pause-msg'}, '● Pause ●'),
        Tag.mk('div', {id: 'pause-top'}, 'Press <em>pause</em> or <em>p</em> to continue')
    ]));

};

Tag.center = function( tag ) {
    var w, hunit;

    with( tag.style ) {
        var m = width.match(/(\d+?\.\d+|\d+)((?:\w+)?)/);
        w = m[1];
        hunit = m[2];
        left = '50%';
        marginTop = '-' + tag.clientHeight / 2 + 'px';
        marginLeft = '-' + parseFloat(w) / 2 + hunit;
    }

    return tag;
};

Tag.floater2start = function( floater, click, opts ) {
    var button = Tag.mk('button', {id: 'runit', onclick: click}, 'Start');
    var options = [
        Tag.mk('input', {type: 'checkbox', id:'opt-classic'}),
        Tag.mk('label', {'for': 'opt-classic', title: 'Speed increases every 10K points, points given only for burnt lines'}, 'Play by "classic" rules'), Tag.mk('br'),
        Tag.mk('input', {type: 'checkbox', id:'opt-beams'}),
        Tag.mk('label', {'for': 'opt-beams', title: 'Highlight where figure is going to land'}, 'Landing "beams"'), Tag.mk('br'),
        Tag.mk('input', {type: 'checkbox', id:'opt-score'}),
        Tag.mk('label', {'for': 'opt-score', title: 'Animate score (turning off increses performance)'}, 'Animate score'), Tag.mk('br'),
        Tag.mk('input', {type: 'checkbox', id:'opt-cw'}),
        Tag.mk('label', {'for': 'opt-cw', title: 'Set main rotation direction to clocwise'}, 'Rotate mainly clockwise')
    ];

    if( opts && opts.indexOf('classic') != -1 )
        options[0].setAttribute('checked', 'checked');
    if( !(opts && opts.indexOf('nobeam') != -1) )
        options[3].setAttribute('checked', 'checked');
    if( !(opts && opts.indexOf('noAnimScore') != -1) )
        options[6].setAttribute('checked', 'checked');
    if( opts && opts.indexOf('cw') != -1 )
        options[9].setAttribute('checked', 'checked');

    Tag.enclose(floater, [button, Tag.enclose(Tag.mk('div', {'class': 'opts'}), options)]);
    Tag.resize(floater, '+14.5ex', '+4ex');
    Tag.center(floater);
    button.focus();
    return floater;
};
