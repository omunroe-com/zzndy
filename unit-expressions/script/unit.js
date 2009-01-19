var system = 'metric';
var units = {};
var systems = {};
var groups = {};

/**
 * Private method to add conversion to a unit definition
 * @param {Unit} from        unit from which conversion starts
 * @param {Conversion} conv  conversion from from to another unit
 */
function addConversion(from, conv)
{
    var to = conv.to;

    //if(to.name in from.conversions) 
    //    throw new Error(['Conversion from', from.name, 'to', to.name, 'is already defined.'].join(' '));

    conv.from = from;
    from.conversions[to.name] = conv;
    console.log('add %s to %s', conv, from);

    // Add reverse conversion
    if(!(from.name in to.conversions))    {
        var inv = conv.invert();
        addConversion(to, inv);
    }

    // Add conversions to other units from the group
    for(var i in to.conversions)    {
        var other_conv = to.conversions[i];
        var unit = other_conv.to;
        if(unit.name != from.name && !(from.name in unit.conversions))
            addConversion(unit, new Conversion(unit, from, conv.ratio * other_conv.ratio));
    }
}

Unit = function(name, group, conversions)
{
    this.name = name;
    this.system = system;
    this.group = group;
    this.conversions = {};

    for(var i in conversions)    {
        var conv = conversions[i];
        addConversion(this, conv);
    }

    addUnit(this);
}

Unit.prototype.toString = function()
{
    return [this.name, ' (', this.system, ' ', this.group, ')'].join('');
}

/**
 * Represent given amount in this unit
 * @param {Number} n         amount to be represented in this unit
 * @param {String} unitName  unit in which amount is currently expressed
 */
Unit.prototype.show = function(n, unitName)
{
    return Unit.convert(n, Unit.get(unitName), this);
}

Unit.convert = function(n, fromUnit, toUnit)
{
    if(!(toUnit.name in fromUnit.conversions))
        throw new Error('Cannot convert from ' + fromUnit.name + ' to ' + toUnit.name + '.');
    return n / fromUnit.conversions[toUnit.name].ratio;
}

Unit.add = function()
{
    var name = arguments[0];
    var group = '', conversions = {};
    console.log(arguments);

    if(arguments.length == 2)    {
        group = arguments[1];
        if(groups[group] === undefined) groups[group] = {};
    }
    if(arguments.length == 3)    {
        var base = getUnit(arguments[2]);
        conversions[base.name] = new Conversion(null, base, arguments[1]);
        group = base.group;
    }

    return new Unit(name, group, conversions);
}

Unit.get = function(name)
{
    return getUnit(name);
}

Unit.system = function(new_system)
{
    system = new_system;
    if(systems[system] === undefined)
        systems[system] = {};
}


function addUnit(unit)
{
    if(unit.name in units)    {
        var base = getUnit(unit.name);
        for(var i in unit.conversions)    {
            var conv = unit.conversions[i];
            if(conv instanceof Conversion)
                addConversion(base, conv);
        }
    }
    else    {
        units[unit.name] = unit;
        systems[unit.system][unit.name] = unit;
        groups[unit.group][unit.name] = unit;
    }
}

function getUnit(name)
{
    if(!(name in units)) throw new Error('Unit ' + name + ' not defined.');
    return units[name];
}

Conversion = function(from, to, ratio)
{
    this.from = from;
    this.to = to;
    this.ratio = ratio;
}

Conversion.prototype.toString = function()
{
    return [this.from.name, '=', this.ratio, this.to.name].join(' ');
}

Conversion.prototype.invert = function()
{
    return new Conversion(this.to, this.from, 1/this.ratio);
}

Unit.system('metric');
