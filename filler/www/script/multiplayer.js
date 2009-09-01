/**
 * Toggle multiplayer menu options.
 * @param {Boolean} displayRoot - if true display root menu option
 */
function toggleMultiplayerMenu(displayRoot)
{
    var menu1 = document.getElementById('prompt-multiplayer').style;
    var menu2 = document.getElementById('choose-multiplayer').style;

    // Auto optimization
    toggleMultiplayerMenu = function(displayRoot)
    {
        menu1.display = displayRoot ? 'block' : 'none';
        menu2.display = displayRoot ? 'none' : 'block';
    };

    toggleMultiplayerMenu(displayRoot);
}

initDomGen(['div','p','a', 'input']);

var comet = null;
var popup = null;

function create()
{
    comet = Comet.open('push.php', {a:'start'});

    popup = $div({'class':'popup'}, [
        $div({'class':'content'}, [
            $div({'class':'title', 'id':'title'}, 'Starting new multiplayer game ...'),
            $p({'class':'message','id':'msg'}, 'Please wait ...'),
            $div({'class':'cancel'}, [$a({'href':'#cancel','onclick':'cancel();return false;'}, 'Cancel')])
        ])]);

    document.body.appendChild(popup);
}

function cancel()
{
    function remove(el)
    {
        if (el != null)
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

function reportCode(code)
{
    var e = document.getElementById('msg');
    e.innerHTML = text_share.fmt({key:code});

    e = document.getElementById('title');
    e.innerHTML = e.innerHTML.replace(/ \.{3}$/, '');
}

function istart(flag)
{
    var e = document.getElementById('msg');
    document.getElementById('title').innerHTML = 'Ready to play';
    e.innerHTML = e.innerHTML.replace(text_wait, (flag ? 'Your move.' : 'The other party has first move.'));
}

function join()
{
    popup = $div({'class':'popup'}, [
        $div({'class':'content'}, [
            $div({'class':'title', 'id':'title'}, 'Enter game key'),
            $p({'class':'message','id':'msg'}, ['The shared key is ',
                $input({'id':'key', 'type':'text', 'maxlength':5,'style':'width:8ex', 'onkeypress':'disableEvent(event)', 'onkeyup':'updateOkLink()', 'onsubmit':'alert("submit")'})
            ]),
            $div({'class':'cancel'}, [$a({'href':'#cancel','onclick':'cancel();return false;'}, 'Cancel')]),
            $div({'class':'ok disabled'}, [$a({'id':'join-ok', 'href':'#ok','onclick':'ok();return false;'}, 'Ok')])
        ])]);

    document.body.appendChild(popup);
    document.getElementById("key").focus();
}

function disableEvent(e)
{
    e.cancelBubble = true;
}

function updateOkLink()
{
    var key = document.getElementById("key");
    okLinkClass((key.value.length == 5) ? 'ok' : 'ok disabled');
}

function okLinkClass(value)
{
    var link = document.getElementById("join-ok");
    if (link != null)
    {
        var parent = link.parentNode;
        if (parent != null)
        {
            if (value !== undefined)
                parent.className = value;
            return parent.className;
        }
    }
    return null;
}

function ok()
{
    var cls = okLinkClass();
    if (cls == null || cls.match(/\bdisabled\b/)) return;

    okLinkClass('ok disabled');
    var key = document.getElementById("key").value;

    var e = document.getElementById('msg');
    e.innerHTML = text_input.fmt({'key':key});

    comet = Comet.open('push.php', {a:'join', k: key});
}

function nogame(key)
{
    var e = document.getElementById('msg');
    if (key == undefined)
    {
        e.innerHTML = 'Could not start a game, the server must be full.';
        document.getElementById('title').innerHTML = 'Sorry for inconvenience';
    }
    else
    {
        e.innerHTML = 'A game with such key ({key:u}) is not available.'.fmt({key:key});
    }
}