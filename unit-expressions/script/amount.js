(function() {
    Amount = function( value, unitName )
    {
        if ( arguments.length == 1 ) {
            var args = value.split(' ');
            value = parseFloat(args[0]);
            unitName = args[1];
        }

        if ( unitName instanceof Unit )
            this.unit = unitName;
        else
            this.unit = Unit.get(unitName);

        this.value = value;
    };

    var A = Amount.prototype;

    A.toString = function()
    {
        return this.value.toFixed(2) + '\u00a0' + this.unit.name;
    };

    A.plus = function( amount )
    {
        return new Amount(this.value + Unit.convert(amount.value, amount.unit, this.unit), this.unit);
    };

    A.minus = function(amount)
    {
        return new Amount(this.value - Unit.convert(amount.value, amount.unit, this.unit), this.unit);
    };

    A.mul = function(amount)
    {
        return new Amount(this.value * amount.value, Unit.get((new Expression('*', this.unit.name, amount.unit.name))));
    };

    A.div = function(amount)
    {
	return new Amount(this.value * amount.value, Unit.get((new Expression('/', this.unit.name, amount.unit.name))));
    }

    A.as = function( unit )
    {
        return new Amount(this.unit.show(this.value, unit), unit);
    };
})();
