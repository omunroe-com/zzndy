function Event( owner, name )
{
    this.owner = owner;
    this.name = name;
    var handlers = [];

    this.addListener = function( object, method )
    {
        handlers.push([object, method]);
    };

    this.removeListener = function( object, method )
    {
        var index = -1;
        var found = handlers.some(function( pair, i ) {
            if( pair[0] == object && pair[1] == method )
            {
                index = i;
                return true;
            }
            return false;
        });

        if( found )
            handlers.splice(index, 1);
    };

    this.fire = function( sender, e )
    {
        if( sender == this.owner )
            handlers.map(function( pair ) {
                pair[1].call(pair[0], e);
            });
    };
}

var E = Event.prototype;

