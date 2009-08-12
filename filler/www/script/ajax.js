var Ajax = {};

Ajax.reduceQuery = function( args ) {
    var parts = [];
    for ( var i in args ) if ( typeof args[i] != 'function' ) parts.push(i + '=' + encodeURIComponent(args[i]));
    return parts.join('&');
}

Ajax.getQuery = function( url ) {
    var query = {};
    var parts = url.replace(/.*?(\?|$)/, '').split('&');
    for ( i in parts )
        if ( typeof parts[i] == 'string' ) {
            var pair = parts[i].split('=');
            query[pair[i]] = decodeURIComponent(pair[1] || null);
        }
    return query;
}

Ajax.stateChangeHandler = function( ok, err, extract ) {
    if ( this.readyState == 4 )
    {
        if ( this.status == 200 || this.status == 0 ) {
            if ( ok ) ok(extract(this))
        }
        else {
            if ( err )err(this.status + ': ' + this.statusText)
        }
    }
}

Ajax.request = function( method, url, data, ok, err, format )
{
    if ( typeof data == 'function' ) {
        format = err;
        err = ok;
        ok = data;
        data = null;
    }

    if ( err && typeof err == 'string' ) {
        format = err;
        err = null;
    }

    if ( data ) data = Ajax.reduceQuery(data);

    if ( method == 'GET' && data ) url += '?' + data;

    var req = new XMLHttpRequest();

    var extract = Ajax.extractText;
    if ( format == 'json' || format == 'js' ) extract = Ajax.extractJs;
    else if ( format == 'xml' ) extract = Ajax.extractXml;

    req.onreadystatechange = function() {
        Ajax.stateChangeHandler.apply(req, [ok, err, extract])
    }

    try {
        req.open(method, url, true);
    } catch( e ) {
        if ( err )err(e);
    }

    if ( method == 'POST' )req.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
    if ( format == 'xml' ) req.overrideMimeType('text/xml');

    req.send(data);
}

Ajax.get = function( url, args, ok, err, format ) {
    Ajax.request('GET', url, args,
            ok, err, format)
}

Ajax.post = function( url, args, ok, err, format ) {
    Ajax.request('POST', url, args,
            ok, err, format)
}

Ajax.extractText = function( http ) {
    return http.responseText;
}

Ajax.extractJs = function( http ) {
    return eval(http.responseText);
}

Ajax.extractXml = function( http ) {
    return http.responseXML;
}
