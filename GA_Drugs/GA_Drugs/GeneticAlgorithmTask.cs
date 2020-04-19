using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;


namespace GA_Drugs
{
    [Serializable]
    public class GeneticAlgorithmTask : System.ComponentModel.INotifyPropertyChanged
    {
        private string _name;
        private DateTime _creationTime;

        // Engine
        private int _populationSize;
        private int _maxGenerations;
        private int _stagnationLimit;

        [XmlIgnore]
        public ProgressChangedEventHandler ProgressChangedEventHandler;
        [XmlIgnore]
        public RunWorkerCompletedEventHandler RunWorkerCompletedEventHandler;

        // Population
        public enum Selection
        {
            RankSelection,
        };
        private int _elitismPreservationAmount;
        private Selection _selectionType;
        private double _mutationProbability;
        private double _crossoverProbability;

        // Results
        public GenerationInfo Results;

        //public GeneticAlgorithmTask()
        //{
        //    Results = new GenerationInfo();
        //}


        // Define the public properties.
        public string Name
        {
            get { return this._name; }
            set
            {
                if (value != this._name)
                {
                    this._name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        [Display(Name = "Creation Time")]
        [DataType(DataType.DateTime)]
        public DateTime CreationTime
        {
            get { return this._creationTime; }
            set
            {
                if (value != this._creationTime)
                {
                    this._creationTime = value;
                    NotifyPropertyChanged("CreationTime");
                }
            }
        }

        [Display(Name = "Population Size")]
        public int PopulationSize
        {
            get { return this._populationSize; }
            set
            {
                if (value != this._populationSize)
                {
                    this._populationSize = value;
                    NotifyPropertyChanged("PopulationSize");
                }
            }
        }

        [Display(Name = "Max Generations")]
        public int MaxGenerations
        {
            get { return this._maxGenerations; }
            set
            {
                if (value != this._maxGenerations)
                {
                    this._maxGenerations = value;
                    NotifyPropertyChanged("MaxGenerations");
                }
            }
        }

        [Display(Name = "Stagnation Limit")]
        public int StagnationLimit
        {
            get { return this._stagnationLimit; }
            set
            {
                if (value != this._stagnationLimit)
                {
                    this._stagnationLimit = value;
                    NotifyPropertyChanged("StagnationLimit");
                }
            }
        }

        [Display(Name = "Elitism Preservation Amount")]
        public int ElitismPreservationAmount
        {
            get { return this._elitismPreservationAmount; }
            set
            {
                if (value != this._elitismPreservationAmount)
                {
                    this._elitismPreservationAmount = value;
                    NotifyPropertyChanged("ElitismPreservationAmount");
                }
            }
        }

        [Display(Name = "Selection Type")]
        [EnumDataType(typeof(Selection))]
        public Selection SelectionType
        {
            get { return this._selectionType; }
            set
            {
                if (value != this._selectionType)
                {
                    this._selectionType = value;
                    NotifyPropertyChanged("SelectionType");
                }
            }
        }

        [Display(Name = "Mutation Probability")]
        public double MutationProbability
        {
            get { return this._mutationProbability; }
            set
            {
                if (value != this._mutationProbability)
                {
                    this._mutationProbability = value;
                    NotifyPropertyChanged("MutationProbability");
                }
            }
        }

        [Display(Name = "Crossover Probability")]
        public double CrossoverProbability
        {
            get { return this._crossoverProbability; }
            set
            {
                if (value != this._crossoverProbability)
                {
                    this._crossoverProbability = value;
                    NotifyPropertyChanged("CrossoverProbability");
                }
            }
        }

        // Implement INotifyPropertyChanged interface.
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public GA_Drugs.DomainKnowledge DomainKnowledge;
    }
}
