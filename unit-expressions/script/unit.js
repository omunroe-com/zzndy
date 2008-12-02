
    units = {};
    Unit = function(name)
    {
        this.name = name;
    }

    Unit.add = function(unit, ratio, otherUnit)
    {
        switch (arguments.length) {
            case 1:
                addNewUnit(unit);
                break;
            case 3:
                addNewUnitConversion(unit, ratio, otherUnit);
        }
    }

Unit.get = function(unitName)
{
    return units[unitName];
}

    function addNewUnit(name) {
        var unit = new Unit(name);
        if (name in units)
            throw new Error('Unit ' + name + ' is already defined');

        units[name] = unit;
    }

    function addNewUnitConversion(name, ratio, otherUnit)
    {
        var unit = new Unit(name);
        if (name in units)
            throw new Error('Unit ' + name + ' is already defined');

        units[name] = unit;
    }


    