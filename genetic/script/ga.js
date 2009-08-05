var defaultalphabeth = range(10);

GeneticAlgorithm = function( initialPopulation, fitnessFunction, elitism, mutationRate, alphabeth )
{
    this.ff = fitnessFunction;
    this.elitism = elitism;
    this.probf = makeprobability(elitism);
    this.mr = mutationRate;
    this.alphabeth = alphabeth || clone(defaultalphabeth);

    this.generation = 0;


    this.pop = initialPopulation;

    assessgeneration.call(this);

};

function assessgeneration()
{
    this.scores = range(this.pop, this.ff);
    this.best = this.pop[this.scores[0][1]];
}

var GA = GeneticAlgorithm.prototype;

/**
 * Produce next generation.
 * @return {GeneticAlgorithm} current instance.
 */
GA.nextGeneration = function()
{
    var npop = 0, newpop = [];
    while( ++npop < this.pop.lenght )
    {
        var parenta = this.pop[selectone(this.scores, this.probf)];
        var parentb = this.pop[selectone(this.scores, this.probf)];
        newpop.push(crossover(parenta, parentb));
    }

    ++this.generation;
    this.pop = newpop;

    assessgeneration.call(this);
    return this;
};

/**
 * Perform generic crossover of two parents.
 * @param p1 first parent
 * @param p2 second parent
 */
function crossover( p1, p2 )
{


    var d1, d2;
    var d = Math.abs(Math.floor((p2.length - p1.length) / 2));
    var x = 1 + Math.floor(Math.random() * (Math.min(p1.length, p2.length) - 1));
    var r = [];

    if( p1.length < p2.length )
    {
        d1 = 0;
        d2 = d;
    }
    else
    {
        d2 = 0;
        d1 = d;
    }

    var c1 = p1.clone();
    var c2 = p2.clone();
    var e1 = c1.splice(0, d1 + x);
    var e2 = c2.splice(0, d2 + x);
    r = [c1.concat(e2),c2.concat(e1)];
    
    return r[Math.round(Math.random())];
}

function makeprobability( elitism )
{
    var m = elitism;
    var w = .1;
    var a = .05;
    var s = 1 - a;
    var tpw = 2 * Math.PI / w;

    // the formula is
    // (1-tanh(2*pi*x/w-m*2*pi/w))/2;
    // where w is front length
    // and m is the x coordinate where function equals 0.5
    // like this
    // .95*(1-tanh(2*pi*x/.1-.2*2*pi/.1))/2-x*.05+.05
    return function( x )
    {
        return s * (1 - tanh(tpw * ( x - m ))) / 2 - x * a + a;
    };
}

function rnd( probf )
{
    var xi, fi, y;
    do
    {
        xi = Math.random();
        fi = probf(xi);
        y = Math.random();
    }
    while( fi < y );

    return xi;
}
;

function selectone( weighed, probf )
{
    return weighed[Math.floor(weighed.length * rnd(probf))];
}

/**
 * Range population by fitness function.
 * @param {Array} population  population to range
 * @oaram {Function} ff       fitness function
 * @return {Array} of pairs (score, index) sorted by score.
 */
function range( population, ff )
{
    function makepair( value, i )
    {
        return [ff(value), i];
    }

    function pairsort( a, b )
    {
        return a[0] - b[0];
    }

    return population.map(makepair).sort(pairsort);
}

var exp = Math.exp;
function sinh( x ) {
    return (exp(x) - exp(-x)) / 2;
}
function cosh( x ) {
    return (exp(x) + exp(-x)) / 2;
}
function tanh( x ) {
    return (exp(2 * x) - 1) / (exp(2 * x) + 1);
}