using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;

namespace GA_Drugs
{
    public class DomainKnowledge
    {
        //[Serializable]
        //public class Chromosome
        //{
        //    private int[] _internalArray;
        //    private double _fitness;

        //    public Chromosome()
        //        : this(_db.Drugs.Count)
        //    {
        //    }

        //    public Chromosome(int length)
        //    {
        //        _internalArray = new int[length];
        //    }

        //    //public void MakeRandom()
        //    //{
        //    //    RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        //    //    byte[] randomBytes = new byte[_internalArray.Length];
        //    //    rngCsp.GetBytes(randomBytes);
        //    //    for (int i = 0; i < _internalArray.Length; i++)
        //    //    {
        //    //        _internalArray[i] = ((randomBytes[i] % 2) == 0) ? 1 : 0;
        //    //    }
        //    //}

        //    public void MakeRandom()
        //    {
        //        RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        //        byte[] randomBytes = new byte[_internalArray.Length];
        //        rngCsp.GetBytes(randomBytes);
        //        byte[] EightBytes = new byte[8];
        //        rngCsp.GetBytes(EightBytes);
        //        Int64 randInt64 = BitConverter.ToInt64(EightBytes, 0);
        //        int numberOfDrugs = (int)(randInt64 % _internalArray.Length);
        //        if (numberOfDrugs == 0)
        //            numberOfDrugs++;
        //        for (int i = 0; i < _internalArray.Length; i++)
        //        {
        //            _internalArray[i] = ((randomBytes[i] % (_internalArray.Length / numberOfDrugs)) == 0) ? 1 : 0;
        //            //_internalArray[i] = ((randomBytes[i] % 2) == 0) ? 1 : 0;
        //        }
        //    }

        //    // Indexer declaration.
        //    // Input parameter is validated by client 
        //    // code before being passed to the indexer.
        //    public int[] Data
        //    {
        //        get
        //        {
        //            return _internalArray;
        //        }
        //    }

        //    public double Fitness
        //    {
        //        get
        //        {
        //            return _fitness;
        //        }
        //    }

        //    public void UpdateFitness()
        //    {
        //        _fitness = GetFitness(this);
        //    }
        //}

        [Serializable]
        public class Chromosome
        {
            private int[] _internalArray;
            private double _fitness;
            private int _count;
            private string _info;
            private RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();

            // if Length needs to be > 255, modify Shuffle
            public int MinLength { get; set; }
            public int Length { get; set; }
            public int PointMax { get; set; }

            public int[] Data
            {
                get
                {
                    return _internalArray;
                }
                set
                {
                    _internalArray = value;
                }
            }

            public double Fitness
            {
                get
                {
                    return _fitness;
                }
                set
                {
                    _fitness = value;
                }
            }

            public int Count
            {
                get
                {
                    return _count;
                }
                set
                {
                    _count = value;
                }
            }

            public string Info
            {
                get
                {
                    return _info;
                }
                set
                {
                    _info = value;
                }
            }

            public void UpdateInfo()
            {
                StringBuilder info = new StringBuilder();
                info.AppendFormat("Fitness: {0}\n", Fitness);
                info.AppendFormat("Drug Count: {0}\n", Count);
                info.Append("Drug List:\n");
                string[] spliter = { " (", ";" };
                for (int i = 0; i < _internalArray.Length; i++)
                    if (_internalArray[i] != -1)
                    {
                        info.AppendFormat("{0},{1},{2},{3}\n", _internalArray[i], _db.Drugs[_internalArray[i]].DrugId, _db.Drugs[_internalArray[i]].Name.Split(spliter, StringSplitOptions.None)[0], _db.Drugs[_internalArray[i]].Score);
                    }
                _info = info.ToString();
            }

            public Chromosome() : this(2, 4) { }

            public Chromosome(int minLength, int maxLength = 4)
            {
                _internalArray = new int[maxLength];
                for (int i = 0; i < _internalArray.Length; i++)
                {
                    _internalArray[i] = -1;
                }
                MinLength = minLength;
                Length = maxLength;
                if (_db != null)
                {
                    PointMax = _db.Drugs.Count;
                }
            }

            //public void MakeRandom()
            //{
            //    RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            //    byte[] randomBytes = new byte[_internalArray.Length];
            //    rngCsp.GetBytes(randomBytes);
            //    for (int i = 0; i < _internalArray.Length; i++)
            //    {
            //        _internalArray[i] = ((randomBytes[i] % 2) == 0) ? 1 : 0;
            //    }
            //}

            public void MakeRandom()
            {
                // roll for MinLength to MaxLength number of drugs
                int n = Length - MinLength + 1;
                byte[] box = new byte[1];
                do _rng.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int drugCount = (box[0] % n) + MinLength;
                // roll for drug at each slot
                n = PointMax;
                box = new byte[2]; //65535
                int drugindex;
                HashSet<int> usedDrugs = new HashSet<int>();
                do
                {
                    do
                    {
                        _rng.GetBytes(box);
                        drugindex = BitConverter.ToUInt16(box, 0);
                    } while (!(drugindex < n * (UInt16.MaxValue / n)));
                    drugindex %= n;
                    if (!usedDrugs.Contains(drugindex))
                    {
                        usedDrugs.Add(drugindex);
                        _internalArray[--drugCount] = drugindex;
                    }
                } while (drugCount > 0);
                // shuffle
                n = Length;
                box = new byte[1];
                while (n > 1)
                {
                    do _rng.GetBytes(box);
                    while (!(box[0] < n * (Byte.MaxValue / n)));
                    int k = box[0] % n;
                    n--;
                    int value = _internalArray[k];
                    _internalArray[k] = _internalArray[n];
                    _internalArray[n] = value;
                }
            }

            public void UpdateFitness()
            {
                Dictionary<int, List<double>> geneFoldChanges = new Dictionary<int, List<double>>();
                for (int i = 0; i < Data.Length; i++)
                {
                    if (Data[i] != -1)
                    {
                        Drug drug = _db.Drugs[Data[i]];
                        foreach (int geneId in drug.Genes.Keys)
                        {
                            if (!geneFoldChanges.ContainsKey(geneId))
                            {
                                geneFoldChanges.Add(geneId, new List<double>());
                            }
                            geneFoldChanges[geneId].Add(drug.Genes[geneId]);
                        }
                    }
                }
                double score = 0;
                foreach (int geneId in geneFoldChanges.Keys)
                {
                    int posCount = 0;
                    for (int i = 0; i < geneFoldChanges[geneId].Count; i++)
                    {
                        if (geneFoldChanges[geneId][i] > 0) // fc won't equal 0, filtered out during ParseCmapStyle
                        {
                            posCount++;
                        }
                    }
                    double combinedFoldChange;
                    if (posCount == 0)
                    {
                        combinedFoldChange = geneFoldChanges[geneId].Min();
                    }
                    else if (posCount == geneFoldChanges[geneId].Count)
                    {
                        combinedFoldChange = geneFoldChanges[geneId].Max();
                    }
                    else
                    {
                        combinedFoldChange = geneFoldChanges[geneId].Average();
                    }
                    score += Math.Pow(2, Math.Abs(_db.GetGeneScore[geneId])) - Math.Pow(2, Math.Abs(_db.GetGeneScore[geneId] + combinedFoldChange));
                }
                //var chromoScores = _db.DiseaseGeneScores.AsParallel();
                //// find int==1 in individual
                //var selectDrugs = individual.Data.Select((value, index) => new { value, index }).Where(entry => entry.value == 1);
                //int callCount = 0;
                //foreach (var drugBit in selectDrugs)
                //{
                //    callCount++;
                //    chromoScores = chromoScores.Zip(_db.Drugs[drugBit.index].GeneArray.AsParallel(), (x, y) => x + y).AsParallel();
                //}
                //double score = _db.DiseaseScore - chromoScores.Select(x => Math.Pow(2, Math.Abs(x))).Sum();
                ////return score / (Math.Pow(drugCount, 8.0));
                //return score / (1.0 + individual.Count * 0.1);
                _fitness = score;
            }

            public void UpdateCount()
            {
                _count = 0;
                for (int i = 0; i < _internalArray.Length; i++)
                {
                    if (_internalArray[i] != -1)
                    {
                        _count++;
                    }
                }
            }

            public void MakeMinLength()
            {
                int needCount = MinLength - _count;
                if (needCount <= 0)
                {
                    return;
                }
                int i = 0;
                int n = PointMax;
                byte[] box = new byte[2]; //65535
                int drugIndex;
                while (needCount > 0)
                {
                    if (_internalArray[i] == -1)
                    {
                        // roll new point value
                        do
                        {
                            _rng.GetBytes(box);
                            drugIndex = BitConverter.ToUInt16(box, 0);
                        } while (!(drugIndex < n * (UInt16.MaxValue / n)));
                        drugIndex %= n;
                        _internalArray[i] = drugIndex;
                        needCount--;
                    }
                    i++;
                }
                // shuffle
                n = Length;
                box = new byte[1];
                while (n > 1)
                {
                    do _rng.GetBytes(box);
                    while (!(box[0] < n * (Byte.MaxValue / n)));
                    int k = box[0] % n;
                    n--;
                    int value = _internalArray[k];
                    _internalArray[k] = _internalArray[n];
                    _internalArray[n] = value;
                }
                _count = 2;
            }
        }
        
        //Drugs = new List<Drug>();
        //GetGenesInPathway = new Dictionary<string, Dictionary<string, int>>();
        //GetPathwaysInGene = new Dictionary<string, Dictionary<string, int>>();
        //GetGeneSymbolFromId = new Dictionary<string, string>();
        //GetGeneIdFromSymbol = new Dictionary<string, string>();
        //GetHccGeneScore = new Dictionary<string, int>();
        public static Database _db;

        public DomainKnowledge(Database db)
        {
            _db = db;
        }

        public DomainKnowledge()
        {
            _db = new Database();
        }

        // higher fitness means better
        public static double GetFitness(Chromosome individual)
        {
            Dictionary<int, List<double>> geneFoldChanges = new Dictionary<int, List<double>>();
            for (int i = 0; i < individual.Data.Length; i++)
            {
                if (individual.Data[i] != -1)
                {
                    Drug drug = _db.Drugs[individual.Data[i]];
                    foreach (int geneId in drug.Genes.Keys)
                    {
                        if (!geneFoldChanges.ContainsKey(geneId))
                        {
                            geneFoldChanges.Add(geneId, new List<double>());
                        }
                        geneFoldChanges[geneId].Add(drug.Genes[geneId]);
                    }
                }
            }
            double score = 0;
            foreach (int geneId in geneFoldChanges.Keys)
            {
                int posCount = 0;
                for (int i = 0; i < geneFoldChanges[geneId].Count; i++)
			    {
                    if (geneFoldChanges[geneId][i] > 0) // fc won't equal 0, filtered out during ParseCmapStyle
	                {
                        posCount++;
	                }
			    }
                double combinedFoldChange;
                if (posCount == 0)
	            {
                    combinedFoldChange = geneFoldChanges[geneId].Min();
	            }
                else if (posCount == geneFoldChanges[geneId].Count)
                {
                    combinedFoldChange = geneFoldChanges[geneId].Max();
                }
                else
                {
                    combinedFoldChange = geneFoldChanges[geneId].Average();
                }
                score += Math.Pow(2, Math.Abs(_db.GetGeneScore[geneId])) - Math.Pow(2, Math.Abs(_db.GetGeneScore[geneId] + combinedFoldChange));
            }
            //var chromoScores = _db.DiseaseGeneScores.AsParallel();
            //// find int==1 in individual
            //var selectDrugs = individual.Data.Select((value, index) => new { value, index }).Where(entry => entry.value == 1);
            //int callCount = 0;
            //foreach (var drugBit in selectDrugs)
            //{
            //    callCount++;
            //    chromoScores = chromoScores.Zip(_db.Drugs[drugBit.index].GeneArray.AsParallel(), (x, y) => x + y).AsParallel();
            //}
            //double score = _db.DiseaseScore - chromoScores.Select(x => Math.Pow(2, Math.Abs(x))).Sum();
            ////return score / (Math.Pow(drugCount, 8.0));
            //return score / (1.0 + individual.Count * 0.1);
            return score;
        }

        public class ChromosomeComparer : IComparer<Chromosome>
        {
            public int Compare(Chromosome x, Chromosome y)
            {
                return -(x.Fitness.CompareTo(y.Fitness)); // minus to sort decending
            }
        }

        public class ChromosomeEqualityComparer : IEqualityComparer<Chromosome>
        {
            public bool Equals(Chromosome x, Chromosome y)
            {
                return x.Data.SequenceEqual(y.Data);
            }
            public int GetHashCode(Chromosome x)
            {
                return x.Data.GetHashCode();
            }
        }
    }
}
