using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.Runtime.InteropServices;

namespace GA_Drugs
{
    /// <summary>
    /// It's just like a System.Collections.Generic.KeyValuePair,
    /// but the XmlSerializer will serialize the
    /// Key and Value properties!
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct KeyValuePair<TKey, TValue>
    {
        private TKey key;
        private TValue value;
        public KeyValuePair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
        public override string ToString()
        {
            StringBuilder builder1 = new StringBuilder();
            builder1.Append('[');
            if (this.Key != null)
            {
                builder1.Append(this.Key.ToString());
            }
            builder1.Append(", ");
            if (this.Value != null)
            {
                builder1.Append(this.Value.ToString());
            }
            builder1.Append(']');
            return builder1.ToString();
        }
        /// <summary>
        /// Gets the Value in the Key/Value Pair
        /// </summary>
        public TValue Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }
        /// <summary>
        /// Gets the Key in the Key/Value pair
        /// </summary>
        public TKey Key
        {
            get
            {
                return this.key;
            }
            set
            {
                this.key = value;
            }
        }
    }

    [Serializable]
    public class GenerationInfo
    {
        public DomainKnowledge.Chromosome BestIndividual;
        public List<KeyValuePair<int, double>> FitnessDistributionCurrentGeneration;
        public List<KeyValuePair<int, double>> BestFitnessEachGeneration;
        public string Report;

        public GenerationInfo(int populationSize, int currentGeneration)
        {
            FitnessDistributionCurrentGeneration = new List<KeyValuePair<int, double>>(populationSize);
            BestFitnessEachGeneration = new List<KeyValuePair<int, double>>(currentGeneration);
            BestIndividual = new DomainKnowledge.Chromosome();
            Report = "";
        }
        public GenerationInfo()
        {
            FitnessDistributionCurrentGeneration = new List<KeyValuePair<int, double>>();
            BestFitnessEachGeneration = new List<KeyValuePair<int, double>>();
            BestIndividual = new DomainKnowledge.Chromosome();
            Report = "";
        }
    }

    // Processes tasks, handle background threading house keeping
    class GeneticAlgorithmEngine
    {
        // General
        private BackgroundWorker _worker;
        private bool _step; // true if do one step

        // GA setting
        private int _generationLimit;
        private int _stagnationLimit;
        private GeneticAlgorithmTask _task;

        // State values
        private GeneticAlgorithmPopulation _population;
        public GeneticAlgorithmPopulation Population
        {
            get
            {
                return _population;
            }
        }
        private int _generationCount; // initial generation = 0
        private int _stagnationCount;
        private double _bestFitness;
        private List<KeyValuePair<int, double>> _bestFitnessEachGeneration;

        public GeneticAlgorithmEngine(GeneticAlgorithmTask task)
        {
            _task = task;
            _step = false;
            _population = new GeneticAlgorithmPopulation(task);
            _generationLimit = task.MaxGenerations;
            _stagnationLimit = task.StagnationLimit;
            _generationCount = 0;

            _worker = new BackgroundWorker();
            _worker.WorkerSupportsCancellation = true;
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += new DoWorkEventHandler(DoWork);
            _worker.ProgressChanged += task.ProgressChangedEventHandler;
            _worker.RunWorkerCompleted += task.RunWorkerCompletedEventHandler;
        }

        // make a GeneticAlgorithmBatcher
        //public void RunBatch() // run a sequence of tasks
        //{
        //}

        public void DoWork(object sender, DoWorkEventArgs e)
        {
            GenerationInfo doWorkReport;
            StringBuilder report;
            while ((_generationCount < _generationLimit) // generation check
                && (_stagnationCount < _stagnationLimit) // convergence check
                && !_worker.CancellationPending) // user paused or stopped
            {
                // Report
                doWorkReport = new GenerationInfo(_task.PopulationSize, _generationCount);
                doWorkReport.BestIndividual = _population[0];
                for (int i = 0; i < _generationCount; i++)
                {
                    doWorkReport.BestFitnessEachGeneration.Add(new KeyValuePair<int, double>(i + 1, _bestFitnessEachGeneration[i].Value));
                }
                for (int i = 0; i < _task.PopulationSize; i++)
                {
                    doWorkReport.FitnessDistributionCurrentGeneration.Add(new KeyValuePair<int, double>(i + 1, _population[i].Fitness));
                }
                report = new StringBuilder();
                report.AppendFormat("{0}: Current Result\n", _task.Name);
                report.AppendFormat("Generation: {0}\n", _generationCount);
                _population[0].UpdateInfo();
                report.AppendFormat("{0}", _population[0].Info);
                doWorkReport.Report = report.ToString();
                _worker.ReportProgress(_generationCount, doWorkReport);
                // ProduceNextGeneration
                _population.ProduceNextGeneration();
                _generationCount++;
                _stagnationCount++;
                if (_bestFitness < _population[0].Fitness) // Maximize
                {
                    _bestFitness = _population[0].Fitness;
                    _stagnationCount = 0;
                }
                _bestFitnessEachGeneration.Add(new KeyValuePair<int, double>(_generationCount, _bestFitness));
                // check if we want to stop after a single run
                if (_step == true)
                {
                    _step = false;
                    _worker.CancelAsync();
                }
            }
            // Report
            doWorkReport = new GenerationInfo(_task.PopulationSize, _generationCount);
            doWorkReport.BestIndividual = _population[0];
            for (int i = 0; i < _generationCount; i++)
            {
                doWorkReport.BestFitnessEachGeneration.Add(new KeyValuePair<int, double>(i + 1, _bestFitnessEachGeneration[i].Value));
            }
            for (int i = 0; i < _task.PopulationSize; i++)
            {
                doWorkReport.FitnessDistributionCurrentGeneration.Add(new KeyValuePair<int, double>(i + 1, _population[i].Fitness));
            }
            report = new StringBuilder();
            if (_worker.CancellationPending)
            {
                e.Cancel = true;
                report.AppendFormat("{0}: Current Result\n", _task.Name);
            }
            else
            {
                report.AppendFormat("{0}: Final Result\n", _task.Name);
            }
            report.AppendFormat("Generation: {0}\n", _generationCount);
            _population[0].UpdateInfo();
            report.AppendFormat("{0}", _population[0].Info);
            doWorkReport.Report = report.ToString();
            _task.Results = doWorkReport;
            e.Result = doWorkReport;
        }

        public void Start() // run a single task, from initialization to termination
        {
            if (_generationCount == 0)
            {
                // do initialization
                _population.InitializeGeneration();
                _generationCount++;
                _bestFitness = _population[0].Fitness;
                _stagnationCount = 0;
                _bestFitnessEachGeneration = new List<KeyValuePair<int, double>>();
                _bestFitnessEachGeneration.Add(new KeyValuePair<int, double>(_generationCount, _bestFitness));
                // check if we want to stop after a single run
                if (_step == true)
                {
                    _step = false;
                    _worker.CancelAsync();
                }
            }
            // start worker
            _worker.RunWorkerAsync();
        }

        public void Pause()
        {
            _worker.CancelAsync();
        }

        // stop after producing one more generation
        public void Step()
        {
            _step = true;
            // start worker
            Start();
        }
    }
}
