try {
    function echo(text) {
        WScript.StdOut.Write(text);
    }

    String.prototype.startsWith = function (text) {
        return this.indexOf(text) == 0;
    }

    String.prototype.endsWith = function (text) {
        return this.indexOf(text) == this.length - text.length;
    }

    if (!('map' in Array.prototype))
        Array.prototype.map = function (fn) {
            var result = [];
            for (var i = 0; i < this.length; ++i) {
                result.push(fn(this[i], i, this));
            }
            return result;
        }

    if (!('filter' in Array.prototype))
        Array.prototype.filter = function (fn) {
            var result = [];
            for (var i = 0; i < this.length; ++i) {
                if (!!fn(this[i], i, this)) result.push(this[i]);
            }
            return result;
        }

    if (!('indexOf' in Array.prototype))
        Array.prototype.indexOf = function (fn) {
            for (var i = 0; i < this.length; ++i) {
                if (fn(this[i])) return i;
            }

            return -1;
        }

    function ajaxGet(url, callback, assync) {
        var request;

        try {
            request = WScript.CreateObject("Msxml2.XMLHTTP");
        }
        catch (e) {
            try {
                request = WScript.CreateObject("Microsoft.XMLHTTP");
            }
            catch (e) {
            }
        }

        request.onreadystatechange = function () {
            if (request.readyState == 4) {
                callback(request.responseText)
            }

            return false;
        };

        request.open('GET', url, assync);
        request.send(null);
    }

    var quotes = [];
    function store(vals) {
        for (var i = 0; i < vals.length; ++i) quotes.push(vals[i]);

    }

    var fn = function (data) {
        var text = data.replace(/\r?\n/g, '').split(/(<p\/>)?\s*<p class="quote">/).map(function (t) {
            var mt = t.match(/\((\d+)\)/)
            var mi = t.match(/\?(\d+)/);

            return {
                id: mi && mi[1] || -1,
                score: mt && mt[1] || -1,
                text: t
            }
        }).filter(function (t) { return t.text != '' && t.score > 3000 && t.id != -1 && quotes.indexOf(function (n) { return n.id == t.id }) == -1 });

        store(text);
    }

    function fmtTime(time) {
        var ns = 1000;
        var nm = ns * 60;
        var nh = nm * 60;

        var h = (time - time % nh) / nh;
        time -= h * nh;

        var m = (time - time % nm) / nm;
        time -= m * nm;

        var s = (time - time % ns) / ns;

        function f(n, t) {
            return n > 0 ? n + ' ' + t + (n == 1 ? '' : 's') + ' ' : '';
        }

        return f(h, 'hour') + f(m, 'minute') + f(s, 'second');
    }

    var url = 'http://bash.org/?random1';
    var max = 5000;
    var mlen = 0;

    var start = (new Date()).getTime();
    var last = 0;
    var zeroed = 0;

    while (quotes.length < max) {
        // Load quotes bypassing explorer's caching.
        ajaxGet(url + '&rn=' + (new Date()).getTime(), fn, false);
        var time = (new Date()).getTime();
        var got = quotes.length;

        // Safeguard agains to restrictive conditions
        if (last == got) {
            ++zeroed;
            // Stop extraction if we faile to get any resources for some time.
            if (zeroed > 20) {
                echo('\nUnable to get more quotes.\n');
                break;
            }
        }
        else
            last = got;

        var left = (max * (time - start)) / got;
        var msg = '\rLoading quotes - ' + fmtTime(left) + 'left.';

        // Needed to clear previous messages' leftovers.
        var mlen = Math.max(mlen, msg.length);

        echo(msg + (new Array(Math.max(mlen - msg.length + 1, 0))).join(' '));
    }

    var msg = '\rSaving ' + quotes.length + ' quotes.';
    echo(msg + (new Array(Math.max(mlen - msg.length + 1, 0))).join(' ') + '\n');

    var fso = WScript.CreateObject("Scripting.FileSystemObject");
    var f = fso.CreateTextFile('quotes.html', true, -1);

    f.Write('<html>');
    f.Write('<head><base src="http://www.bash.org/" /></head>');
    f.Write('<body>');
    f.Write(quotes.map(function (t) { return t.text }).join('<hr/>\n\n'));
    f.Write('</body></html>');

    f.Close();
}
catch (ex) {
    echo('Error: ' + ex.message + '\n');
}




