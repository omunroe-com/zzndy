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
        return this.value.toFixed(2) + this.unit.name;
    };

    A.plus = function( amount )
    {
        return new Amount(this.value + Unit.convert(amount.value, amount.unit, this.unit), this.unit);
    };

    A.as = function( unit )
    {
        return new Amount(this.unit.show(this.value, unit), unit);
    };
})();