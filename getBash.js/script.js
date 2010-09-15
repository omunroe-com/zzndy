try {
    function echo(text) {
        WScript.Echo(text);
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
        }).filter(function (t) { return t.text != '' && t.score > 1000 && t.id != -1 && quotes.indexOf(function (n) { return n.id == t.id }) == -1 });

        store(text);

        //        var waitForQuote = true;
        //        var quote = "";
        //        var quotes = [];
        //        for (var i = 0; i < text.length; ++i) {
        //            var line = text[i];
        //            if (waitForQuote) {
        //                if (line.startsWith('<p class="quote">')) {
        //                    waitForQuote = false;
        //                    if (line.endsWith('</p>')) {
        //                        waitForQuote = true;
        //                        quotes.push(line);
        //                    }
        //                    else {
        //                        quote += line;
        //                    }

        //                }
        //            }
        //            else {
        //                if (line.endsWith('</p>')) {
        //                    waitForQuote = true;
        //                    quotes.push(quote + line);
        //                    quote = '';
        //                }
        //                else {
        //                    quote += line;
        //                }
        //            }
        //        }
        //        WScript.Echo(quotes[1]);
        //        WScript.Echo(quotes[5]);
        //        WScript.Echo(quotes[10]);
    }

    var url = 'http://bash.org/?random1';
    var max = 10;

    while (quotes.length < max) {
        ajaxGet(url + '&rn=' + (new Date()).getTime(), fn, false);
        var got = quotes.length;
        echo('Have ' + got + ' quotes, ' + (max - got) + ' to go.');
    }

    echo('Saving ' + quotes.length + ' quotes.');
    var fso = WScript.CreateObject("Scripting.FileSystemObject");
    var f = fso.CreateTextFile('quotes.html', true);
    f.Write('<html><body>');
    f.Write(quotes.map(function (t) { return t.text}).join('<hr/>\n\n'));

    f.Write('</body></html>');
    f.Close();
}
catch (ex) {
    echo('Error: ' + ex.message);
}




