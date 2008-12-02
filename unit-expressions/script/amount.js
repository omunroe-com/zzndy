function Amount(value, unitName)
{
    this.unit = Unit.get(unitName);
    this.value = value;
}

Amount.prototype.toString = function()
{
    return this.value + this.unit.name;
}