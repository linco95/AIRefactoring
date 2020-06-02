using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



// TODO: Implement equals

namespace NeuralNetwork.Genetic {
    class Debug {
        public static void Log(string s) {
            // Disable prints
        }
    }


    class GeneticNeuralNet : NeuralNet {
        private Population population;

        
        public GeneticNeuralNet(int inputSize, int hiddenSize, int outputSize, int populationSize, double? learnRate = null, double? momentum = null) : base(inputSize, hiddenSize, outputSize, learnRate, momentum) {
            population = new Population(inputSize * hiddenSize + hiddenSize * outputSize, populationSize, 0.05f);
        }


        public void Train(List<DataSet> dataSets, int numEpochs, int numGenerations) {
            List<Chromosone> bestInEpoch = new List<Chromosone>();
            for (int epoch = 0; epoch < numEpochs; epoch++) {
                string debugStr = "";
                debugStr += ("**********NEW EPOCH (" + epoch + ")*************\n");
                //population.individuals.ForEach(chrom => debugStr += chrom.Genes.Count + ", ");
                Debug.Log(debugStr);
                // Generate genes
                population.regenerate();
                for (int generation = 0; generation < numGenerations; generation++) {
                    debugStr = "";
                    debugStr += ("**********NEW GENERATION (" + generation + ")*************\n");
                    //population.individuals.ForEach(chrom => debugStr += chrom.Genes.Count + ", ");
                    Debug.Log(debugStr);
                    population.individuals.ForEach(chrom => chrom.resetFitness());
                    dataSets.ForEach(ds => {
                        for (int individual = 0; individual < population.individuals.Count; individual++) {
                            // Set weights to genes  
                            updateWeights(population.individuals[individual].getWeights());

                            ForwardPropagate(ds.Values);
                            // Calculate fitness
                            // TODO: Only calculates last dataset technically, should be nested, dataset => evolve => new dataset
                            population.individuals[individual].calculateFitness(OutputLayer.Select(neuron => neuron.Value).ToArray(), ds.Targets, dataSets.Count);

                        }
                        // Evolve
                        population.evolve();
                    });
                }
                // Save best individual
                bestInEpoch.Add(Population.getBest(population.individuals, false));
                string outputStr = "";
                var descendingPopulation = new List<Chromosone>(new List<Chromosone>(population.individuals).OrderByDescending(chrom => chrom.Fitness));
                descendingPopulation.ForEach(chrom => outputStr += chrom.Fitness + ", ");
                Debug.Log(outputStr + "\nBest in epoch: " + bestInEpoch.Last().Fitness); 
            }
            // Get best individual from the epochs
            Chromosone bestOverall = Population.getBest(bestInEpoch, false);
            updateWeights(bestOverall.getWeights());
            Debug.Log("Best overall: " + bestOverall.Fitness);

        }

        public void updateWeights(double[] weights) {
            // Set each weight in the neural net to its gene representation
            int i = 0;
            InputLayer.ForEach(neuron => neuron.OutputSynapses.ForEach(synapse => synapse.Weight = weights[i++]));
            HiddenLayer.ForEach(neuron => neuron.OutputSynapses.ForEach(synapse => synapse.Weight = weights[i++]));
        }

    }



    public class Population {
        float mutationRate = 0.01f;
        private bool shouldMutate { get { return NeuralNet.Random.NextDouble() < mutationRate; } }
        float combinationProbability = 0.7f;
        private bool shouldCombine { get { return NeuralNet.Random.NextDouble() < combinationProbability; } }
        public enum SELECTIONTYPE {
            ROULETTE,
            TOURNAMENT
        }
        public enum CROSSOVERTYPE {
            DUAL
        }



        public List<Chromosone> individuals = new List<Chromosone>();

        public void regenerate() {
            individuals.ForEach(chrom => chrom.generateGenes(chrom.Genes.Count));
        }

        public static Chromosone getBest(List<Chromosone> individuals, bool max) {
            //string outputStr = "";
            var descendingPopulation = new List<Chromosone>(new List<Chromosone>(individuals).OrderByDescending(chrom => chrom.Fitness));
            //descendingPopulation.ForEach(chrom => outputStr += chrom.Fitness + ", ");
            //Debug.Log("GetBest: " + outputStr);
            return new Chromosone(max ? descendingPopulation.First() : descendingPopulation.Last());
        }

        public void evolve() {
            Population childPopulation = new Population(0, 0, mutationRate);
            while (childPopulation.individuals.Count < individuals.Count) {
                // Selection
                Chromosone chromA = Selection(SELECTIONTYPE.TOURNAMENT);
                Chromosone chromB = Selection(SELECTIONTYPE.TOURNAMENT);
                // Cross over
                crossover(chromA, chromB, CROSSOVERTYPE.DUAL);
                // Mutations
                mutate(chromA);
                mutate(chromB);
                // Add new individuals
                childPopulation.individuals.Add(chromA);
                childPopulation.individuals.Add(chromB);
            }

            individuals = new List<Chromosone>(childPopulation.individuals); // NOPE SHALLOW COPY!
        }

        //        Population evolve(Population parentPopulation) {
        //            Population childPopulation;
        //            while (childPopulation.size() < parentPopulation.size()) {
        //                Genome genome0 = select(population);
        //                Genome genome1 = select(population);
        //                if (p(combinationProbability)) {
        //                    combine(genome0, genome1);
        //                }
        //                for (int i = 0; i < GENE_COUNT; i++) {
        //                    if (p(mutationProbability)) mutate(genome0[i]);
        //                    if (p(mutationProbability)) mutate(genome1[i]);
        //                }
        //                childPopulation.add(genome0, evaluate(genome0));
        //                childPopulation.add(genome1, evaluate(genome1));
        //            }
        //            return childPopulation
        //         }

        public Population(int chromosoneSize, int populationSize, float mutationRate) {
            this.mutationRate = mutationRate;
            for (int j = 0; j < populationSize; ++j) {
                individuals.Add(new Chromosone(chromosoneSize));
            }
        }

        private void mutate(Chromosone chrom) {
            chrom.Genes.ForEach(gene => {
                if (!shouldMutate) return;
                gene.generateRandom();
            });
        }

        private void crossover(Chromosone chromosoneA, Chromosone chromosoneB, CROSSOVERTYPE type) {
            if (!shouldCombine) return;
            switch (type) {
                case CROSSOVERTYPE.DUAL:
                    int pointA = NeuralNet.Random.Next(0, chromosoneA.Genes.Count);
                    int pointB = NeuralNet.Random.Next(0, chromosoneA.Genes.Count);
                    int temp = pointA;
                    pointA = pointA < pointB ? pointA : pointB;
                    pointB = temp > pointB ? temp : pointB; // assert A is smaller than B

                    var copyOfA = new List<Gene>(chromosoneA.Genes);
                    var copyOfB = new List<Gene>(chromosoneB.Genes);
                    chromosoneA.Genes.Clear();
                    chromosoneB.Genes.Clear();

                    chromosoneA.Genes.AddRange(new List<Gene>(copyOfA.GetRange(0, pointA)));
                    chromosoneA.Genes.AddRange(new List<Gene>(copyOfB.GetRange(pointA, pointB - pointA)));
                    chromosoneA.Genes.AddRange(new List<Gene>(copyOfA.GetRange(pointB, copyOfA.Count - pointB)));

                    chromosoneB.Genes.AddRange(new List<Gene>(copyOfB.GetRange(0, pointA)));
                    chromosoneB.Genes.AddRange(new List<Gene>(copyOfA.GetRange(pointA, pointB - pointA)));
                    chromosoneB.Genes.AddRange(new List<Gene>(copyOfB.GetRange(pointB, copyOfB.Count - pointB)));
                    break;
                default:
                    crossover(chromosoneA, chromosoneB, CROSSOVERTYPE.DUAL); // Should never happen
                    break;
            }
        }

        private Chromosone Selection(SELECTIONTYPE type) {
            switch (type) {
                case SELECTIONTYPE.TOURNAMENT:
                    int n = 10; // TODO: add n to parameter list
                    List<Chromosone> selectedTournament = new List<Chromosone>();
                    for (int i = 0; i < n; i++) {
                        selectedTournament.Add(individuals[NeuralNet.Random.Next(0, individuals.Count)]); // TODO: inclusive range end?
                    }
                    return getBest(selectedTournament, false);
                default:
                    return Selection(SELECTIONTYPE.TOURNAMENT);
            }
        }

        public override int GetHashCode() {
            return individuals.GetHashCode() * 4723 + mutationRate.GetHashCode() * 3203 + combinationProbability.GetHashCode() * 7213;
        }

        public Population(Population other) {
            individuals = new List<Chromosone>(other.individuals);
            mutationRate = other.mutationRate;
            combinationProbability = other.combinationProbability;
        }
    }

    public class Chromosone {
        public List<Gene> Genes { get; private set; }
        public float Fitness { get; private set; }

        public Chromosone(List<Gene> newGenes) {
            Genes = newGenes;
            Fitness = 0;
        }

        public Chromosone(int size) {
            generateGenes(size);
            Fitness = 0;
        }

        // Generate genes
        public void generateGenes(int size) {
            Genes = new List<Gene>();
            for (int i = 0; i < size; ++i) {
                Genes.Add(new Gene(NeuralNet.GetRandom()));
            }
        }

        // Set weights to genes
        public double[] getWeights() {
            // Set each weight in the neural net to its gene representation
            return Genes.Select(gene => gene.Allele).ToArray();
        }


        // Calculate fitness
        // Values is calculated from network, targets is expected value/correct
        public void calculateFitness(double[] values, double[] targets, int nrDataSets) {
            int i = 0;
            // Sum the difference from the correct value
            Fitness += new List<double>(values).Sum(val => UnityEngine.Mathf.Abs((float)(targets[i++] - val))) / nrDataSets; // TODO: Average?
            //Debug.Log("Fitness: " + Fitness);
        }

        public Chromosone(Chromosone other) {
            Genes = new List<Gene>(other.Genes);
            Fitness = other.Fitness;
        }

        public override int GetHashCode() {
            return Genes.GetHashCode() * 13 + Fitness.GetHashCode() * 1723;
        }

        internal void resetFitness() {
            Fitness = 0;
        }
    }

    public class Gene {
        public double Allele { get; private set; }

        public override int GetHashCode() {
            return Allele.GetHashCode() * 11;
        }

        public void generateRandom() {
            Allele = NeuralNet.GetRandom();
        }

        public Gene(double val) {
            Allele = val;
        }

        public Gene(Gene other) {
            Allele = other.Allele;
        }

    }
}
