var defaultalphabeth = range(10);

GeneticAlgorithm = function( initialPopulation, fitnessFunction, elitism, mutationRate, alphabeth )
{
    this.ff = fitnessFunction;
    this.elitism = elitism;
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
        var parenta = this.pop[selectone(this.scores, this.elitism)];
        var parentb = this.pop[selectone(this.scores, this.elitism)];
        newpop.push(crossover(parenta, parentb));
    }

    ++this.generation;
    this.pop = newpop;

    assessgeneration.call(this);
    return this;
};

function selectone( weighed, elitism )
{
    // the formula is
    // (1-tanh(2*pi*x/w-m*2*pi/w))/2;
    // where w is front length
    // and m is the x coordinate where function equals 0.5
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