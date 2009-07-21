//(function() {
var system = 'metric';
var units = {};
var systems = {};
var groups = {};
var group_relationships = {};
var lastUnit = undefined;

group_relationships[new Expression ( 'length/time' )] = 'velocity';

/**
 * Private method to add conversion to a unit definition
 * @param {Unit} from        unit from which conversion starts
 * @param {Conversion} conv  conversion from from to another unit
 */
function addConversion( from, conv )
{
    var to = conv.to;

    conv.from = from;
    from.conversions[to.name] = conv;

    // Add reverse conversion
    if ( !(from.name in to.conversions) ) {
        var inv = conv.invert ( );
        addConversion ( to, inv );
    }

    // Add conversions to other units from the group
    for ( var i in to.conversions ) {
        var other_conv = to.conversions[i];
        var unit = other_conv.to;
        if ( unit.name != from.name && !(from.name in unit.conversions) )
            addConversion ( unit, new Conversion ( unit, from, conv.ratio * other_conv.ratio ) );
    }
}

Unit = function( name, group, conversions )
{
    this.name = name;
    this.system = system;
    this.group = group;
    this.conversions = {};

    for ( var i in conversions ) {
        var conv = conversions[i];
        addConversion ( this, conv );
    }

    addUnit ( this );
};

Unit.prototype.toString = function()
{
    return [this.name, ' (', this.system, ' ', this.group, ')'].join ( '' );
};

/**
 * Represent given amount in this unit
 * @param {Number} n         amount to be represented in this unit
 * @param {String} unitName  unit in which amount is currently expressed
 */
Unit.prototype.show = function( n, unitName )
{
    return Unit.convert ( n, Unit.get ( unitName ), this );
};

/**
 *
 * @param {String} group - new current unit group (e.g. metric).
 * @return an object that contains unit function which will define given unit as basic for this group.
 */
Unit.base = function( group )
{
    return {
        /**
         *
         * @param {String} unit - name of current group basic unit.
         */
        unit:function( unit )
        {
            lastUnit = unit;
            Unit.add ( unit, group );
            return Unit;
        }
    };
};

/**
 * Add a new unit.
 * @param {String} name - name of the unit.
 * @return handle to define conversions.
 */
Unit.unit = function( name )
{
    lastUnit = name;
    return {
        /**
         * Define unit conversion.
         * @param {Number} rate - conversion rate
         * @param {String} other - other unit name (unit must be defined)
         */
        equals:function( rate, other )
        {
            Unit.add ( name, rate, other );
            return Unit;
        }
    };
};

/**
 * Add an alias for current unit.
 * @param {String} alias
 */
Unit.alias = function( alias )
{
    if ( lastUnit === undefined )
        throw new Error ( 'Define a unit before assigning an alias.' );
    return Unit;
};

/**
 * Define unit conversion for the current unit.
 * @param {Number} rate - conversion rate
 * @param {String} other - other unit name (unit must be defined)
 */
Unit.equals = function( rate, other )
{
    if ( lastUnit === undefined )
        throw new Error ( 'Define a unit before creating conversions.' );
    Unit.add ( lastUnit, rate, other );
    return Unit;
};

/**
 * Convert given amount from one unit to another.
 * @param {Number} n         amount expressed in 'fromUnit'
 * @param {Unit}   fromUnit  original unit
 * @param {Unit}   toUnit    unit to convert to
 *
 * @return {Number} amount expressed in 'toUnit'
 */
Unit.convert = function( n, fromUnit, toUnit )
{
    if ( !(toUnit.name in fromUnit.conversions) )
        throw new Error ( 'Cannot convert from ' + fromUnit.name + ' to ' + toUnit.name + '.' );
    return n / fromUnit.conversions[toUnit.name].ratio;
};

/**
 * Add new unit definition
 * @param {String} name       default unit for given group
 * @param {String} group      unit group (e.g. lenght, time, velocit etc.)
 * - or -
 * @param {String} name       new unit name
 * @param {Number} ratio      conversion ratio between new and base unit
 * @param {String} base unit  base unit which is already defined
 *
 * @return {Unit}  newly added unit
 */
Unit.add = function()
{
    var name = arguments[0];
    var group = '', conversions = {};

    if ( arguments.length == 2 ) {
        group = arguments[1];
        if ( groups[group] === undefined ) groups[group] = {base:name};
    }
    if ( arguments.length == 3 ) {
        var base = getUnit ( arguments[2] );
        conversions[base.name] = new Conversion ( null, base, arguments[1] );
        group = base.group;
    }

    return new Unit ( name, group, conversions );
};

/**
 * Get unit by it's name
 * @param {String} name  unit name
 * @return {Unit}
 */
Unit.get = function( name )
{
    return getUnit ( name );
};

/**
 * Get or set current unit system (e.g. metric or imperial)
 * @param new_system
 */
Unit.system = function( new_system )
{
    if ( new_system !== undefined ) {
        system = new_system;
        if ( systems[system] === undefined )
            systems[system] = {};
    }

    return Unit;
};

function deduce_group( unitname )
{
    function get_group( expr )
    {
        if ( expr instanceof Expression )
            return   get_groups ( expr );
        else
            return Unit.get ( expr ).group;
    }

    function get_groups( expr )
    {
        var left = get_group ( expr.left );
        var right = get_group ( expr.right );

        return new Expression ( expr.op, left, right );
    }

    var unitExpr = new Expression ( unitname );
    var expr = get_groups ( unitExpr );

    return group_relationships[expr];
}

function addUnit( unit )
{
    if ( unit.name in units ) {
        var base = getUnit ( unit.name );
        for ( var i in unit.conversions ) {
            var conv = unit.conversions[i];
            if ( conv instanceof Conversion )
                addConversion ( base, conv );
        }
    }
    else {
        units[unit.name] = unit;
        systems[unit.system][unit.name] = unit;

        var group = groups[unit.group];
        group[unit.name] = unit;
        if ( group.base == unit.name )
            group.base = unit;
    }
}

function getUnit( name )
{
    if ( !(name in units) ) throw new Error ( 'Unit ' + name + ' not defined.' );
    return units[name];
}

Conversion = function( from, to, ratio )
{
    this.from = from;
    this.to = to;
    this.ratio = ratio;
};

Conversion.prototype.toString = function()
{
    return [this.from.name, '=', this.ratio, this.to.name].join ( ' ' );
};

Conversion.prototype.invert = function()
{
    return new Conversion ( this.to, this.from, 1 / this.ratio );
};

Unit.system ( 'metric' );
//}) ( );