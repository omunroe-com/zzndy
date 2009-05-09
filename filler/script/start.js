var canvasId = 'main-filler-canvas';
var canvas = document.getElementById(canvasId);
var coldrun = true;
var playerwins = 0;
var computerwins = 0;

var filler;

function report_shares( a, b )
{
    document.getElementById('player').innerHTML = a.toFixed(2) + '%';
    document.getElementById('computer').innerHTML = b.toFixed(2) + '%';
}

function start()
{
    filler = new Filler(canvas);
    filler.render();

    if( coldrun ) init_controls();

    range(7).forEach(enableButton);

    report_shares(filler.logic.p1share, filler.logic.p2share);

    if( playerwins == 0 && computerwins == 0 )
        document.getElementById('message').innerHTML = '';
    else
        document.getElementById('message').innerHTML = playerwins + ':' + computerwins;

    disableButton(filler.p1color);
    disableButton(filler.p2color);
}

function init_controls()
{
    var control_holder = document.getElementById('colors');
    var colors = filler.getColors();

    var controlTmp = '<a href="#color{n}" id="color-{n}" class="color-control color-{n}" onclick="setColor({n}); return false;">{sn}</a>';
    var styleTmp
            = '.color-{n} {background-color: {color}; border-color: {light1} {dark1} {dark2} {light2}; color: {text-color}}\n\n'
            + '.color-{n}:hover, .color-{n}:visited, .color-{n}:visited:hover {color: {text-color}}\n\n'
            + '.color-{n}:hover, .color-{n}:visited:hover {background-color: {light2}}\n'
            + '.color-{n}.disabled, .color-{n}.disabled:hover, .color-{n}.disabled:visited, .color-{n}.disabled:hover:visited {background-color:transparent;color:#888;border-color:#888 } ';

    function generateControl( colorStr, n )
    {
        return controlTmp.fmt({n:n, sn:n + 1 });
    }

    function generateStyle( colorStr, n )
    {
        var color = new Color(colorStr);
        var textColor = ((color.red + color.green + color.blue) / 3 + 128) % 256 < 128 ? '#262626' : '#e0e0e0';
        return styleTmp.fmt({n:n , color:color.tint(-20), light1:color.tint(80), light2:color.tint(30), dark1:color.tint(-55), dark2:color.tint(-90),'text-color':textColor});
    }

    var style = document.createElement('style');
    style.setAttribute('type', 'text/css');
    style.appendChild(document.createTextNode(colors.map(generateStyle).join('\n\n')));

    document.getElementsByTagName('head')[0].appendChild(style);

    control_holder.innerHTML = colors.map(generateControl).join('');

    window.onkeypress = function( e )
    {
        if( e.charCode >= 49 && e.charCode <= 55 )
            setColor(e.charCode - 49);
    };

    coldrun = false;
}

function disableButton( i )
{
    var button = document.getElementById('color-' + i);
    button.className += ' disabled';
}

function enableButton( i )
{
    var button = document.getElementById('color-' + i);
    button.className = button.className.replace(/\bdisabled\b/g, '');
}


function setColor( n )
{
    var control = document.getElementById('color-' + n);
    if( !control.className.match(/disabled/) ) {
        enableButton(filler.p1color);
        enableButton(filler.p2color);

        filler.setColor(n);
        filler.moveP2();

        disableButton(filler.p1color);
        disableButton(filler.p2color);

        var a = filler.logic.p1share;
        var b = filler.logic.p2share;

        report_shares(a, b);

        if( a > 50 ) {
            ++playerwins;
            gamover('You win!');
        }
        if( b > 50 ) {
            ++computerwins;
            gamover('You lose.');
        }
    }
}

function gamover( message )
{
    range(7).forEach(disableButton);
    document.getElementById('message').innerHTML = message + ' <a href="#restart" onclick="start(); return false;">Play again</a>';
}

start();
