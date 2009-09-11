function Fsm( initial, initialEntry )
{
    this.current = new State(initial, initialEntry);
    this.states = {};
    this.states[initial] = this.current;
}

var F = Fsm.prototype;

F.from = function( state )
{
    if ( !(state in this.states) )
        throw new Error('State ' + state + ' not found.');

    return new TransitionStart(this, this.states[state]);
};

F.getState = function ( name )
{
    if ( name in this.states )
        return this.states[name];
    else
        return null;
};

function State( name, entry )
{
    this.name = name;
    this.entry = entry;
    this.transitions = {};
}

State.prototype.getTarget = function( event )
{
    if ( event in this.transitions )
        return this.transitions[event];
    else return null;
};

function TransitionStart( fsm, state )
{
    this.fsm = fsm;
    this.state = state;
}

var S = TransitionStart.prototype;

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

S.from = function( state )
{
    return this.fsm.from(state);
};

function Transition( fsm, start, end )
{
    this.fsm = fsm;
    this.start = start;
    this.end = end;
}

var T = Transition.prototype;

T.on = function( event )
{
    var target;
    if ( target = this.start.getTarget(event) )
        throw new Error(['Cannot create transition from state', this.start.name, 'to state', this.end.name, 'on event', event, 'because it already leads to',target.name].join(' '));

    this.start.transitions[event] = this.end;
    if ( !(this.end.name in this.fsm.states) )
        this.fsm.states[this.end.name] = this.end;

    return new TransitionStart(this.fsm, this.start);
};

