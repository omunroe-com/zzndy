Comet = {
    open : function( url, args )
    {
        var ifr = document.createElement('iframe');
        ifr.setAttribute('style', 'display:none');

        document.body.appendChild(ifr);
        ifr.src = url + '?' + toQueryString(args);

        return ifr;
    }
};

/**
 * Toggle multiplayer menu options.
 * @param {Boolean} displayRoot - if true display root menu option
 */
function toggleMultiplayerMenu( displayRoot )
{
    var menu1 = document.getElementById('prompt-multiplayer').style;
    var menu2 = document.getElementById('choose-multiplayer').style;

    // Auto optimization
    toggleMultiplayerMenu = function( displayRoot )
    {
        menu1.display = displayRoot ? 'block' : 'none';
        menu2.display = displayRoot ? 'none' : 'block';
    };

    toggleMultiplayerMenu(displayRoot);
}

initDomGen(['div','p','a', 'input']);

var comet = null;
var popup = null;

/**
 * Show create multiplayer game menu.
 */
function create()
{
    filler.makeNext();
    comet = Comet.open('mp.php', {a:'start', f:filler.next.serialize()});

    popup = $div({'class':'popup'}, [
        $div({'class':'content'}, [
            $div({'class':'title', 'id':'title'}, 'Starting new multiplayer game ...'),
            $p({'class':'message','id':'msg'}, 'Please wait ...'),
            $div({'class':'cancel'}, [$a({'href':'#cancel','onclick':'cancel();return false;'}, 'Cancel')])
        ])]);

    document.body.appendChild(popup);
}

/**
 * Show join multiplayer game menu.
 */
function join()
{
    popup = $div({'class':'popup'}, [
        $div({'class':'content'}, [
            $div({'class':'title', 'id':'title'}, 'Enter game key'),
            $p({'class':'message','id':'msg'}, ['The shared key is ',
                $input({'id':'key', 'type':'text', 'maxlength':5,'style':'width:8ex', 'onkeypress':'disableEvent(event)', 'onkeyup':'updateOkLink()', 'onsubmit':'alert("submit")'})
            ]),
            $div({'class':'cancel'}, [$a({'href':'#cancel','onclick':'cancel();return false;'}, 'Cancel')]),
            $div({'class':'ok disabled'}, [$a({'id':'ok', 'href':'#ok','onclick':'ok();return false;'}, 'Ok')])
        ])]);

    document.body.appendChild(popup);
    document.getElementById("key").focus();
}

/**
 * Handle multiplayer menu 'OK' action.
 */
function ok()
{
    var cls = okLinkClass();
    if ( cls == null || cls.match(/\bdisabled\b/) ) return;

    okLinkClass('ok disabled');
    var key = document.getElementById("key").value;

    var e = document.getElementById('msg');
    e.innerHTML = text_input.fmt({'key':key});

    comet = Comet.open('mp.php', {a:'join', k: key});
}

/**
 * Handle multiplayer menu 'Cancel' action.
 */
function cancel()
{
    function remove( el )
    {
        if ( el != null )
        {
            el.parentNode.removeChild(el);
            el = null;
        }
    }

    remove(popup);
    remove(comet);
    toggleMultiplayerMenu(true);
}

var text_share = 'Game key is <em title="{key:u} - case insensitive">{key:u}</em>. Share it with the other party to start playing.';
var text_wait = 'Waiting for the server ...';
var text_input = 'The shared key is <b>${key:u}</b>. ' + text_wait;

function reportCode( code )
{
    var e = document.getElementById('msg');
    e.innerHTML = text_share.fmt({key:code});

    e = document.getElementById('title');
    e.innerHTML = e.innerHTML.replace(/ \.{3}$/, '');
}

function reportGame( w, h, fld )
{

}

var timeout = null;
var secondsLeft = 0;
function reportFirst( party )
{
    var e = document.getElementById('msg');
    if ( party == 'us' )
    {
        e.innerHTML = "Ready to play, first move is yours.";
    }
    else
    {
        e.innerHTML = 'Ready to play, the other party makes first move.';
    }

    secondsLeft = 3;
    timeout = window.setTimeout(closeMenu, 1);
}

function closeMenu()
{
    var e = document.getElementById('ok');
    if ( e ) {
        if ( --secondsLeft > 0 )
        {
            e.innerHTML = 'Ok (' + secondsLeft + ')';
            timeout = window.setTimeout(closeMenu, 1000);
        }
        else {
            e.innerHTML = 'Ok';

            window.clearTimeout(timeout);
            timeout = null;
            secondsLeft = 0;
            ok();
        }
    }
}

function disableEvent( e )
{
    e.cancelBubble = true;
}

function updateOkLink()
{
    var key = document.getElementById("key");
    okLinkClass((key.value.length == 5) ? 'ok' : 'ok disabled');
}

function okLinkClass( value )
{
    var link = document.getElementById("ok");
    if ( link != null )
    {
        var parent = link.parentNode;
        if ( parent != null )
        {
            if ( value !== undefined )
                parent.className = value;
            return parent.className;
        }
    }

    return null;
}

function reportNoGame( key )
{
    var e = document.getElementById('msg');
    if ( key == undefined )
    {
        e.innerHTML = 'Could not start a game, the server must be full.';
        document.getElementById('title').innerHTML = 'Sorry for inconvenience';
    }
    else
    {
        e.innerHTML = 'A game with such key ({key:u}) is not available.'.fmt({key:key});
    }
}

function showMpMenu() {
}
function getCode() {
}
function enterCode() {
}
function shareCode() {
}
function showIntro() {
}
function startMpGame() {
}
function getGame() {
}
function showFail() {
}

var mp = new Fsm('mp-menu', showMpMenu);

mp
        .from('mp-menu')
        .to('get-code', getCode).on('start')
        .to('enter-code', enterCode).on('join')
        .from('get-code')
        .to('share-code', shareCode).on('reportCode')
        .to('show-fail', showFail).on('reportFail')
        .from('share-code')
        .to('show-intro', showIntro).on('reportAccept')
        .to('mp-menu').on('cancel')
        .from('show-intro')
        .to('play', startMpGame).on('ok')
        .to('play').on('ok-timeout')
        .from('show-fail').to('mp-menu').on('ok')
        .from('enter-code')
        .to('mp-menu').on('cancel')
        .to('get-game', getGame).on('ok')
        .from('get-game')
        .to('show-fail').on('reportFail')
        .to('show-intro').on('reportAccept');


