var defaultalphabeth = range ( 10 );

GeneticAlgorithm = function( initialPopulation, fitnessFunction, elitism, mutationRate, alphabeth )
{
    this.pop = initialPopulation;
    this.ff = fitnessFunction;
    this.elitism = elitism;
    this.mr = mutationRate;
    this.alphabeth = alphabeth || clone ( defaultalphabeth );

    this.generation = 0;
    this.best = null;
};

var GA = GeneticAlgorithm.prototype;

/**
 * Produce next generation.
 * @return {GeneticAlgorithm} current instance.
 */
GA.nextGeneration = function()
{
    // Select
    // Crossover
    // Mutate

    ++this.generation; 
    return this;
};