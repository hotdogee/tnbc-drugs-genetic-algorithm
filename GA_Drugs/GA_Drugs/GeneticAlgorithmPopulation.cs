using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GA_Drugs
{
    // Decide on selection stratagies for crossover and mutation and replacement stratagies
    class GeneticAlgorithmPopulation : List<DomainKnowledge.Chromosome>
    {
        private GeneticAlgorithmTask _task;
        private double[] _selectionProbability;
        private Random rand = new Random();

        public GeneticAlgorithmPopulation(GeneticAlgorithmTask task)
            : base(task.PopulationSize)
        {
            _task = task;
            _selectionProbability = new double[task.PopulationSize];
            switch (_task.SelectionType)
            {
                case GeneticAlgorithmTask.Selection.RankSelection:
                    for (int i = 0; i < _task.PopulationSize; i++)
                    {
                        _selectionProbability[i] = 1.3 - (1.3 - 0.7) * i / (_task.PopulationSize - 1);
                    }
                    break;
            }
        }
        
        public void InitializeGeneration()
        {
            for (int i = 0; i < _task.PopulationSize; i++)
            {
                DomainKnowledge.Chromosome individual = new DomainKnowledge.Chromosome();
                individual.MakeRandom();
                individual.UpdateFitness();
                individual.UpdateCount();
                this.Add(individual);
            }
            this.Sort(new DomainKnowledge.ChromosomeComparer());
        }

        public void ProduceNextGeneration()
        {
            List<DomainKnowledge.Chromosome> nextGen = new List<DomainKnowledge.Chromosome>(_task.PopulationSize);
            if (_task.ElitismPreservationAmount > 0)
            {
                for (int i = 0; i < _task.ElitismPreservationAmount; i++)
                {
                    nextGen.Add(this[i]);
                }
            }
            while (nextGen.Count < _task.PopulationSize)
            {
                //rank selection
                double roll = rand.NextDouble() * _task.PopulationSize;
                double sum = 0;
                int i = 0;
                while (sum < roll)
                {
                    sum += _selectionProbability[i++];
                }
                int fatherIndex = i - 1;
                int motherIndex;
                do
                {
                    roll = rand.NextDouble() * _task.PopulationSize;
                    sum = 0;
                    i = 0;
                    while (sum < roll)
                    {
                        sum += _selectionProbability[i++];
                    }
                    motherIndex = i - 1;
                } while (motherIndex == fatherIndex);
                DomainKnowledge.Chromosome child1 = new DomainKnowledge.Chromosome();
                DomainKnowledge.Chromosome child2 = new DomainKnowledge.Chromosome();
                roll = rand.NextDouble();
                if (roll < _task.CrossoverProbability)
                {
                    OnePointCrossover(this[fatherIndex], this[motherIndex], ref child1, ref child2);
                }
                else
                {
                    for (int j = 0; j < this[fatherIndex].Data.Length; j++)
                    {
                        child1.Data[j] = this[fatherIndex].Data[j];
                        child2.Data[j] = this[motherIndex].Data[j];
                    }
                }
                roll = rand.NextDouble();
                if (roll < _task.MutationProbability)
                    OneIntRoll(ref child1);
                if (!nextGen.Contains(child1, new DomainKnowledge.ChromosomeEqualityComparer()))
                {
                    //child1.UpdateFitness();
                    nextGen.Add(child1);
                }
                if (nextGen.Count < _task.PopulationSize)
                {
                    roll = rand.NextDouble();
                    if (roll < _task.MutationProbability)
                        OneIntRoll(ref child2);
                    if (!nextGen.Contains(child2, new DomainKnowledge.ChromosomeEqualityComparer()))
                    {
                        //child2.UpdateFitness();
                        nextGen.Add(child2);
                    }
                }
            }
            Parallel.For(_task.ElitismPreservationAmount, nextGen.Count, i => // new ParallelOptions { MaxDegreeOfParallelism = 50 },
                {
                    nextGen[i].UpdateFitness();
                });
            this.Clear();
            this.AddRange(nextGen);
            this.Sort(new DomainKnowledge.ChromosomeComparer());
        }

        public void OnePointCrossover(DomainKnowledge.Chromosome father, DomainKnowledge.Chromosome mother, ref DomainKnowledge.Chromosome child1, ref DomainKnowledge.Chromosome child2)
        {
            int roll = rand.Next(father.Data.Length - 1); // exclude all father and all mother possibilities
            for (int i = 0; i < father.Data.Length; i++)
            {
                child1.Data[i] = (i <= roll) ? father.Data[i] : mother.Data[i];
                child2.Data[i] = (i <= roll) ? mother.Data[i] : father.Data[i];
            }
            child1.UpdateCount();
            child2.UpdateCount();
            child1.MakeMinLength();
            child2.MakeMinLength();
        }

        public void OneBitMutation(ref DomainKnowledge.Chromosome individual)
        {
            int roll = rand.Next(individual.Data.Length);
            individual.Data[roll] = (individual.Data[roll] == 0) ? 1 : 0;
        }

        public void OneIntRoll(ref DomainKnowledge.Chromosome individual)
        {
            // roll index
            int index = rand.Next(individual.Data.Length);
            if (individual.Count == individual.MinLength && individual.Data[index] != -1)
            {
                // roll new point value
                int drugIndex = rand.Next(individual.PointMax);
                individual.Data[index] = drugIndex;
            }
            else
            {
                // roll for empty of not
                int isEmpty = rand.Next(2);
                if (isEmpty == 0)
                {
                    individual.Data[index] = -1;
                }
                else
                {
                    // roll new point value
                    int drugIndex = rand.Next(individual.PointMax);
                    individual.Data[index] = drugIndex;
                }
            }
            individual.UpdateCount();
        }
    }
}
