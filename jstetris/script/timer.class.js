(function( $ ) {
    /**
     * @class Timer
     * Incapsulate window set-timeout behavior.
     *
     * @param {Function} action  function to run after timer elapses
     * @param {Number} timeout   number of milliseconds before fireing an action
     */
    $.Timer = function( action, timeout )
    {
        this.t = timeout || 100;
        this.a = action;
        this.h = null;
    };

    var T = Timer.prototype;

    /**
     * Run timeout iteration
     * @param {Number} t  optional timeout value (if not given, timers default is used)
     */
    T.run = function( t ) {
        this.t = t || this.t;
        if ( !this.h ) this.h = window.setTimeout(this.a, this.t);
    };

    /**
     * Clear timeout.
     */
    T.stop = function() {
        if ( this.h ) {
            window.clearTimeout(this.h);
            this.h = null;
        }
    };

    /**
     * Shortcut for clearing timeout and then starting timer again.
     * @param {Number} t
     */
    T.restart = function( t ) {
        this.stop();
        this.run(t);
    };
})(window);

