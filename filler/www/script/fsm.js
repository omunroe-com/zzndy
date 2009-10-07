/**
 * @class Fsm - Finite State machine
 * Initialize new instance of FSM.
 * @param {String} initial - initial state
 * @param {Function} initialEntry - initial state entry function
 */
function Fsm( initial, initialEntry )
{
    this.current = new State(initial, initialEntry);
    this.states = {};
    this.states[initial] = this.current;
}

var F = Fsm.prototype;

/**
 * Define new transition specifying start state. Transition start state must be defined.
 * @param {String} state - transition start state
 * @return {TransitionStart} - intermediate transition object
 */
F.from = function( state )
{
    if ( this.getState(state) == null )
        throw new Error('State ' + state + ' not found.');

    return new TransitionStart(this, this.states[state]);
};

/**
 * Get a FSM state by name.
 * @param {String} name - State name
 * @return {State} - state with name given or null
 */
F.getState = function ( name )
{
    if ( name in this.states )
        return this.states[name];
    else
        return null;
};

/**
 * Create new transition
 * @param {State} start - start state
 * @param {State} end - end state
 * @param {String} event - trigger event
 */
F.addTransition = function(start, end, event)
{
    if(!(end.name in this.states))
    this.states[end.name] = end;

    start.addTransition(end, event);
};

F.enable = function()
{
    this.enabled = true;
};

F.disable = function()
{
    this.enabled = false;
};

/**
 * Handle event with optional arguments.
 * @param {String} event - event name
 * @param arg - event arguments optional (multiple)
 */
F.on = function( event, arg )
{
    if ( !this.enabled )
        throw new Error("Cannot handle " + event + " fsm is not enabled.");

    var target = this.current.getTarget(event);

    var args = [];
    for(var i=1;i<arguments.length;++i)
        args.push(arguments[i]);

    if ( target != null )
    {
        console.log('handling ' + event + ' with arguments ' + args,arguments);
        target.entry.apply(null, args);
        this.current = target;
    }
    else console.log(this.current.name + ' cannot handle ' + event);
};

/**
 * @constructor State - FSM state
 * @param {String} name - state name
 * @param {Function} entry - entry function
 */
function State( name, entry )
{
    this.name = name;
    this.entry = entry;
    this.transitions = {};
}

var A = State.prototype;


/**
 * Get target state for given event.
 * @param {String} event - event name
 * @return {State} - target state or null
 */
A.getTarget = function( event )
{
    if ( event in this.transitions )
        return this.transitions[event];
    else return null;
};

var strBindError = 'Cannot bind transition from {start} to {end} on `{event}` as it already leads to {target}';

/**
 * Create transition to givent state.
 * @param {State} target - target event.
 * @param {String} event - event name.
 */
A.addTransition = function(target, event)
{
    if(event in this.transitions)
    {
        var obj = {'start':this.name, 'end':target.name, 'event':event, 'target':this.transitions[event].name};
        throw new Error(strBindError.fmt(obj));
    }

    this.transitions[event] = target;
};

/**
 * @class TransitionStart - Intermediate object denoting an intention to create a transition.
 * @param {Fsm} fsm - finite state machine
 * @param {State} state - starting state
 */
function TransitionStart( fsm, state )
{
    this.fsm = fsm;
    this.state = state;
}

var S = TransitionStart.prototype;

/**
 * Create transition.
 * @param {String} stateName - target state name
 * @param {Function} entry - target state entry function, optional. If left out state must be defined.
 * @return {Transition} - new transition
 */
S.to = function( stateName, entry )
{
    var state = null;
    if ( entry === undefined ) {
        state = this.fsm.getState(stateName);
        if ( state == null )
            throw new Error('State ' + stateName + ' not found. Cannot complete transition from ' + this.state.name);
    }
    else {
        state = new State(stateName, entry);
    }

    return new Transition(this.fsm, this.state, state);
};

/**
 * Start transition from different state.
 * @param {String} state - transition start state
 * @return {TransitionStart} - intermediate transition object
 */
S.from = function( state )
{
    return this.fsm.from(state);
};

/**
 * @class Transition - transition between two states.
 * @param {Fsm} fsm - finite state machine
 * @param {State} start - start state
 * @param {State} end - target state
 */
function Transition( fsm, start, end )
{
    this.fsm = fsm;
    this.start = start;
    this.end = end;
}

var T = Transition.prototype;

/**
 * Specify event triggering the transition.
 * @param {String} event - event name, must not be used by start state for different transition
 * @return {TransitionStart} - intermediate transition object
 */
T.on = function( event )
{
    var target;
    if ( target = this.start.getTarget(event) )
        throw new Error(['Cannot create transition from state', this.start.name, 'to state', this.end.name, 'on event', event, 'because it already leads to',target.name].join(' '));

    this.fsm.addTransition(this.start, this.end, event);

    return new TransitionStart(this.fsm, this.start);
};

