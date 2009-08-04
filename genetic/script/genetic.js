/**
 * @description
 * A gene is an ASCII letter
 *
 * Algorithm:
 *     1. Prepare initial generation;
 *     2. Select using fitness function;
 *     3. Breed via crossover and mutation;
 *     4. Until termination goto step 2;
 */

(function() {

    /**
     *
     * @param {Array} population            initial population
     * @param {Function} fitness            fitnes function
     * @param {Number} elitism              percent of instances selected for breeding
     * @param {Number} mutationProbability  possibility of gene mutation
     */
    Genetic = function( population, fitness, elitism, mutationProbability )
    {
        this.population = population;
        this.fitness = fitness;
        this.nBreedSelect = Math.round(population.length * elitism / 100);
        this.mutationProbability = mutationProbability;
        this.bestScore = null;
        this.n = 0;
    };

    Genetic.prototype.run = function()
    {
        var scores = this.population.map(function( elt, i ) {
            return [i, this.fitness.call(null, elt)];
        });
        var best = [];

        scores.sort(function( a, b ) {
            return a[1] - b[1];
        });

        var consumed = [];
        do
        {
            var i = Math.floor(Math.random() * Math.random() * Math.random() * scores.length);
            if( consumed.indexOf(i) == -1 )
            {
                consumed.push(i);
                best.push(this.population[scores[i][0]]);
            }
        }
        while( best.length < this.nBreedSelect );

        var newpop = [];
        var couples = [];
        do
        {
            if( couples.length == 0 )
                couples = range(best.length).shuffle();
            var i1 = couples.pop();

            if( couples.length == 0 )
                couples = range(best.length).shuffle();
            var i2 = couples.pop();

            if( i1 != i2 ) {
                newpop.push(crossover.call(this, best[i1], best[i2]));
            }
        }
        while( newpop.length < this.population.length );

        this.best = this.population[scores[0][0]];
        this.bestScore = scores[0][1];

        this.population = newpop;
        ++this.n;
    };

    var chars = ' ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*(),./';
    var maxMut = 5; // how far mutated character can change

    /**
     * Corssover two genes
     * @param {String} a  first gene
     * @param {String} b  second gene
     * @return {String}   child gene
     */
    function crossover( a, b )
    {
        var doubleXover = Math.random() < .5;
        var len = Math.min(a.length, b.length);
        if( Math.random() < .3 )
        {
            var t = a;
            a = b;
            b = t;
        }

        var c;

        // Crossover
        if( doubleXover )
        {
            var crop1 = 1 + Math.floor(Math.random() * (len / 2 - 2));
            var crop2 = Math.floor(len / 2 + 1 + Math.random() * (len / 2 - 2));
            c = a.slice(0, crop1) + b.slice(crop1, crop2) + a.slice(crop2, a.length);
        }
        else
        {
            var crop = 1 + Math.floor(Math.random() * (len - 2));
            c = a.slice(0, crop) + b.slice(crop, b.length);
        }

        // Mutate
        var p = this.mutationProbability / 100;
        if( Math.random() < p )
            c = mutate(c);
        if( Math.random() < p * p )
            c = mutate(c);
        if( Math.random() < p * p * p )
            c = mutate(c);

        return c;
    }

    function mutate( c )
    {
        var i = Math.floor(Math.random() * c.length);
        var n = chars.indexOf(c[i]);
        if( n == -1 )
            n = Math.floor(Math.random() * chars.length);

        var dn = Math.floor(Math.random() * 2 * maxMut);
        if( dn >= 0 )
            dn += 1;
        else if( n + dn < 0 )
            n += chars.length;

        n = (n + dn) % chars.length;

        var mutated = chars[n];
        return c.slice(0, i) + mutated + c.slice(i + 1, c.length);
    }
})();