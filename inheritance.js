function Base(x, y)
{
    this.x = x;
    this.y = y;
    console.log('Base here', this.x, this.y);
}

Base.prototype.echo = function()
{
    console.log('Base echo here', this.x, this.y);
}

function Derived(x, y, z)
{
    console.log('calling base');
    Base.call(this, x, y);
    console.log('base called');
    this.z = y;
    console.log('Derived here', this.x, this.y, this.z);
}

Derived.prototype = new Base; // Base constructor null null is called
Derived.prototype.base = Base.prototype;

Derived.prototype.echo = function()
{
    this.base.echo.call(this); 
    console.log('Derived echo here', this.x, this.y, this.z);
}

var t = new Derived(10, 20, 30);
console.log('init complete');
t.echo()
Derived.constructor.toSource()

console.log(t instanceof Base); // true
console.log(t instanceof Derived); // true

