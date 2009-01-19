function Amount(value, unitName)
{
    if(arguments.length == 1)    {
        var args = value.split(' ');
        value = parseFloat(args[0]);
        unitName = args[1];
    }

    if(unitName instanceof Unit)
        this.unit = unitName;
    else
        this.unit = Unit.get(unitName);

    this.value = value;
}

Amount.prototype.toString = function()
{
    return this.value + this.unit.name;
}

Amount.prototype.plus = function(amount)
{
    return new Amount(this.value + Unit.convert(amount.value, amount.unit, this.unit), this.unit);
}


