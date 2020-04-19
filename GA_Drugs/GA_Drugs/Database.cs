using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Data;
using System.Collections.Concurrent;

namespace GA_Drugs
{
    class DataTools
    {
        public static void GrepHsaIdKeggPathway(string originalPath, string outputPath)
        {
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(originalPath))
                {
                    //String wholeFile = sr.ReadToEnd();
                    //Trace.WriteLine("1. File read done");
                    //string[] stringSeparators = new string[] {"///"};
                    //string[] splitFile = wholeFile.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                    //Trace.WriteLine("2. Split file done");
                    using (StreamWriter outfile = new StreamWriter(outputPath))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            string pattern = @"ENTRY\s+(hsa\d+)";
                            Match match = Regex.Match(line, pattern);
                            if (match.Success)
                            {
                                outfile.WriteLine(match.Groups[1].Value);
                                while ((line = sr.ReadLine()) != null)
                                {
                                    if (line.Contains("///"))
                                    {
                                        //outfile.WriteLine("///");
                                        break;
                                    }
                                    else
                                    {
                                        //outfile.WriteLine(line);
                                    }
                                }
                            }
                        }
                        //List<string> filteredEntries = new List<string>();
                        //foreach (string entryBlock in splitFile)
                        //{
                        //    string pattern = @"ENTRY\s+hsa\d+";
                        //    Match match = Regex.Match(entryBlock, pattern);
                        //    if (match.Success)
                        //    {
                        //        outfile.WriteLine(entryBlock);
                        //        outfile.WriteLine("///");
                        //    }
                        //}
                    }
                    Trace.WriteLine("**Filter successful");
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Trace.WriteLine("The file could not be read:");
                Trace.WriteLine(e.Message);
            }
        }
        public static void FilterHsaEntryKeggPathway(string originalPath, string outputPath)
        {
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(originalPath))
                {
                    //String wholeFile = sr.ReadToEnd();
                    //Trace.WriteLine("1. File read done");
                    //string[] stringSeparators = new string[] {"///"};
                    //string[] splitFile = wholeFile.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                    //Trace.WriteLine("2. Split file done");
                    using (StreamWriter outfile = new StreamWriter(outputPath))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            string pattern = @"ENTRY\s+(hsa\d+)";
                            Match match = Regex.Match(line, pattern);
                            if (match.Success)
                            {
                                outfile.WriteLine(line);
                                while ((line = sr.ReadLine()) != null)
                                {
                                    if (line.Contains("///"))
                                    {
                                        outfile.WriteLine("///");
                                        break;
                                    }
                                    else
                                    {
                                        outfile.WriteLine(line);
                                    }
                                }
                            }
                        }
                        //List<string> filteredEntries = new List<string>();
                        //foreach (string entryBlock in splitFile)
                        //{
                        //    string pattern = @"ENTRY\s+hsa\d+";
                        //    Match match = Regex.Match(entryBlock, pattern);
                        //    if (match.Success)
                        //    {
                        //        outfile.WriteLine(entryBlock);
                        //        outfile.WriteLine("///");
                        //    }
                        //}
                    }
                    Trace.WriteLine("**Filter successful");
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Trace.WriteLine("The file could not be read:");
                Trace.WriteLine(e.Message);
            }
        }
    }
    
    public class DrugTargetPpiParameters
    {
        public double UpValue;
        public double DownValue;
        public double PpiFactor;
        public int PpiLevel;
        // constructor
        public DrugTargetPpiParameters(double upValue, double downValue, double ppiFactor, int ppiLevel)
        {
            UpValue = upValue;
            DownValue = downValue;
            PpiFactor = ppiFactor;
            PpiLevel = ppiLevel;
        }
    }

    public class DataTab
    {
        public string Name;
        public string Info;
        public DataTable DataTable;
    }

    [Serializable]
    public class Drug
    {
        public string DrugId;
        public string Name;
        public string Info;
        public Dictionary<string, double> Pathways; // +1 as activator, -1 as inhibitor
        public Dictionary<int, double> Genes;
        //public double[] GeneArray;
        public double Score;

        // constructor
        public Drug(string drugId)
        {
            DrugId = drugId;
            Pathways = new Dictionary<string, double>();
            Genes = new Dictionary<int, double>();
            //GeneArray = new double[1];
        }

        public Drug(Drug drug)
        {
            DrugId = drug.DrugId;
            Name = drug.Name;
            Info = drug.Info;
            Pathways = new Dictionary<string, double>(drug.Pathways);
            Genes = new Dictionary<int, double>(drug.Genes);
            Score = drug.Score;
        }
    }

    [Serializable]
    public class Database
    {
        private BackgroundWorker _worker;
        public BackgroundWorker Worker
        {
            get
            {
                return _worker;
            }
        }
        public string[] InitializeProgressText = {
            "TNBC Expressions",
            "Connectivity Map",
            "DrugBank",
            "KEGG DRUG",
            "PPI",
            "Apply PPI",
            //"Build Gene Array",
            "Calculate Scores",
            "Gene Symbols",
            "Build Data Tabs",
        };
        public List<DataTab> DataTabs;
        
        public List<Drug> Drugs;
        public Dictionary<int, double> GetGeneScore;
        public double DiseaseScore;
        //public int[] GeneIdArray;
        //public Dictionary<int, int> GetGeneIdArrayIndexFromGeneId;
        //public double[] DiseaseGeneScores;
        public Dictionary<int, string> GetGeneSymbolFromEntrez;
        public Dictionary<int, List<int>> Ppi;

        public Database()
        {
            DataTabs = new List<DataTab>();
            Drugs = new List<Drug>();
            GetGeneScore = new Dictionary<int, double>();
            DiseaseScore = 0;
            //GeneIdArray = new int[1];
            //GetGeneIdArrayIndexFromGeneId = new Dictionary<int, int>();
            //DiseaseGeneScores = new double[1];
            GetGeneSymbolFromEntrez = new Dictionary<int, string>();
            Ppi = new Dictionary<int, List<int>>();
            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += new DoWorkEventHandler(DoWork);
        }

        public Database(Database db)
        {
            DataTabs = new List<DataTab>();
            Drugs = new List<Drug>();
            foreach (Drug drug in db.Drugs)
            {
                Drugs.Add(new Drug(drug));
            }
            GetGeneScore = new Dictionary<int, double>(db.GetGeneScore);
            DiseaseScore = db.DiseaseScore;
            //GeneIdArray = new int[1];
            //GetGeneIdArrayIndexFromGeneId = new Dictionary<int, int>();
            //DiseaseGeneScores = new double[1];
            GetGeneSymbolFromEntrez = db.GetGeneSymbolFromEntrez;
            Ppi = db.Ppi;
            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += new DoWorkEventHandler(DoWork);
        }

        public void DoWork(object sender, DoWorkEventArgs e)
        {
            string cwd = System.IO.Directory.GetCurrentDirectory();
            string path;
            int i = 1;
            int drugCount, geneCount;
            // Disease Gene Expression
            path = Path.Combine(cwd, @"data\tnbc_gene_max");
            ParseDiseaseGeneExpression(path);
            _worker.ReportProgress(i++, string.Format("{0}", GetGeneScore.Count));
            // cmap
            path = Path.Combine(cwd, @"data\ratioMatrix_cmap_gene_max_filtered_log");
            ParseCmapStyle(path, out drugCount, out geneCount);
            _worker.ReportProgress(i++, string.Format("{0} * {1}", drugCount, geneCount));
            // drugbank
            path = Path.Combine(cwd, @"data\drugbank_approved_with_usable_target.cmap_style");
            ParseCmapStyle(path, out drugCount, out geneCount);
            _worker.ReportProgress(i++, string.Format("{0} * {1}", drugCount, geneCount));
            // kegg
            path = Path.Combine(cwd, @"data\kegg_drug_with_usable_target.cmap_style");
            ParseCmapStyle(path, out drugCount, out geneCount);
            _worker.ReportProgress(i++, string.Format("{0} * {1}", drugCount, geneCount));
            // PPI
            path = Path.Combine(cwd, @"data\interactions_9606_entrez");
            ParsePPI(path);
            _worker.ReportProgress(i++, string.Format("{0}", Ppi.Count));
            // Apply PPI
            //up	down	factor	level	pos_count	max	median	d1	d2	d3
            //1	-1	0.3	2	2639	551.6483451	256.708128	D09640=Marizomib=551.648345149336	D03150=Bortezomib=521.494250446387	D10110=Delanzomib=521.494250446387
            DrugTargetPpiParameters drugTargetPpiParameters = new DrugTargetPpiParameters(1, -1, 0.3, 2);
            drugCount = Drugs.Count;
            ApplyPpiToDrugTargets(drugTargetPpiParameters);
            _worker.ReportProgress(i++, string.Format("{0} -> {1}", drugCount, Drugs.Count));
            // Build Gene Array
            //BuildGeneArray();
            //_worker.ReportProgress(i++, string.Format("{0}", GeneIdArray.Length));
            // Calculate Drug Scores
            CalculateScores();
            _worker.ReportProgress(i++, string.Format("{0}", Drugs.Count));
            // Gene Symbols
            path = Path.Combine(cwd, @"data\entrez_symbol");
            ParseEntrezSymbol(path);
            _worker.ReportProgress(i++, string.Format("{0}", GetGeneSymbolFromEntrez.Count));
            // Build DataTabs
            BuildDataTabs();
            _worker.ReportProgress(i++, string.Format("{0:0.}", DiseaseScore));
        }

        private void ParsePPI(string filePath)
        {
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] tokens = line.Split('\t');
                        int p1 = int.Parse(tokens[0]);
                        int p2 = int.Parse(tokens[1]);
                        if (!Ppi.ContainsKey(p1))
                        {
                            Ppi[p1] = new List<int>();
                        }
                        Ppi[p1].Add(p2);
                        if (!Ppi.ContainsKey(p2))
                        {
                            Ppi[p2] = new List<int>();
                        }
                        Ppi[p2].Add(p1);
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Trace.WriteLine("The file could not be read:");
                Trace.WriteLine(e.Message);
            }
        }

        public void ApplyPpiToDrugTargets(DrugTargetPpiParameters p)
        {
            foreach (Drug drug in Drugs)
            {
                if (drug.DrugId[0] == 'D') // select only DrugBank and KEGG
                {
                    // initialize up down values
                    List<int> keys = new List<int>(drug.Genes.Keys);
                    foreach (int entrez in keys)
                    {
                        if (drug.Genes[entrez] > 0)
                        {
                            drug.Genes[entrez] = p.UpValue;
                        }
                        else if (drug.Genes[entrez] < 0)
                        {
                            drug.Genes[entrez] = p.DownValue;
                        }
                        else
                        {
                            drug.Genes[entrez] = 0;
                        }
                    }
                    // grow ppi network
                    for (int i = 0; i < p.PpiLevel; i++)
                    {
                        Dictionary<int, double> genesNew = new Dictionary<int, double>(drug.Genes);
                        foreach (int entrez in drug.Genes.Keys)
                        {
                            if (Ppi.ContainsKey(entrez))
                            {
                                foreach (int neighbor in Ppi[entrez])
                                {
                                    if (!drug.Genes.ContainsKey(neighbor))
                                    {
                                        if (genesNew.ContainsKey(neighbor))
                                        {
                                            genesNew[neighbor] += (drug.Genes[entrez] - genesNew[neighbor]) * p.PpiFactor;
                                        }
                                        else
                                        {
                                            genesNew.Add(neighbor, drug.Genes[entrez] * p.PpiFactor);
                                        }
                                    }
                                }
                            }
                        }
                        drug.Genes = genesNew;
                    }
                }
                // remove drug targets which doesn't have a disease score
                List<int> itemsToRemove = new List<int>();
                foreach (int entrez in drug.Genes.Keys)
                {
                    if (!GetGeneScore.ContainsKey(entrez))
                    {
                        itemsToRemove.Add(entrez);
                    }
                }
                foreach (int entrez in itemsToRemove)
                {
                    drug.Genes.Remove(entrez);
                }
            }
            // remove drugs with no targets
            Drugs.RemoveAll(d => d.Genes.Count == 0);
        }

        public void Initialize() // run a single task, from initialization to termination
        {
            // start worker
            _worker.RunWorkerAsync();
        }

        public void BuildDataTabs()
        {
            DataTab dataTab = new DataTab();
            dataTab.Name = "Drug Info";
            // Populate dataGridDrugTargets
            DataTable dtDrugs = new DataTable();
            dtDrugs.Columns.AddRange(new DataColumn[] { 
                new DataColumn("ID"),
                new DataColumn("Name"),
                new DataColumn("Score", typeof(int)),
                new DataColumn("Info"),
                new DataColumn("Top 10 Targets"),
            });

            foreach (Drug drug in Drugs)
            {
                DataRow rd = dtDrugs.NewRow();
                rd["ID"] = drug.DrugId;
                rd["Name"] = drug.Name;
                rd["Score"] = drug.Score;
                rd["Info"] = drug.Info;
                // build stringBuilderGenes
                StringBuilder stringBuilderGenes = new StringBuilder();
                var top10Genes = drug.Genes.OrderByDescending(x => Math.Abs(x.Value)).Take(10);
                foreach (var kvp in top10Genes)
                {
                    stringBuilderGenes.AppendFormat("{0}({1})={2:0.00}{3:+0.00;-0.00;0.00}, ", GetGeneSymbolFromEntrez[kvp.Key], kvp.Key, GetGeneScore[kvp.Key], kvp.Value);
                }
                if (stringBuilderGenes.Length > 2)
                    stringBuilderGenes.Remove(stringBuilderGenes.Length - 2, 2);
                rd["Top 10 Targets"] = stringBuilderGenes.ToString();
                dtDrugs.Rows.Add(rd);
            }
            dataTab.DataTable = dtDrugs;
            dataTab.Info = string.Format("Drug Count = {0}", Drugs.Count);
            DataTabs.Add(dataTab);
        }

        public void ParseEntrezSymbol(string filePath)
        {
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] tokens = line.Split('\t');
                        GetGeneSymbolFromEntrez.Add(int.Parse(tokens[0]), tokens[1]);
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Trace.WriteLine("The file could not be read:");
                Trace.WriteLine(e.Message);
            }
        }

        public void CalculateScores()
        {
            // Calculate Disease Score
            DiseaseScore = 0;
            foreach (int geneId in GetGeneScore.Keys)
            {
                DiseaseScore += Math.Pow(2, Math.Abs(GetGeneScore[geneId]));
            }
            // Calculate drug scores
            foreach (Drug drug in Drugs)
            {
                //drug.GeneArray = new double[GeneIdArray.Length];
                //foreach (int geneId in drug.Genes.Keys)
                //{
                //    drug.GeneArray[GetGeneIdArrayIndexFromGeneId[geneId]] = drug.Genes[geneId];
                //}
                //drug.Score = DiseaseScore - DiseaseGeneScores.Zip(drug.GeneArray, (x, y) => x + y).Select(x => Math.Pow(2, Math.Abs(x))).Sum();
                double score = 0;
                foreach (int geneId in drug.Genes.Keys)
                {
                    score += Math.Pow(2, Math.Abs(GetGeneScore[geneId])) - Math.Pow(2, Math.Abs(GetGeneScore[geneId] + drug.Genes[geneId]));
                }
                drug.Score = score;
            }
        }

        // Build a list of used gene ids
        public void BuildGeneArray()
        {
            //HashSet<int> geneIdSet = new HashSet<int>();
            //// go through all disease genes
            //foreach (int geneId in GetGeneScore.Keys)
            //{
            //    geneIdSet.Add(geneId);
            //}
            //// go through all drugs
            //foreach (Drug drug in Drugs)
            //{
            //    foreach (int geneId in drug.Genes.Keys)
            //    {
            //        geneIdSet.Add(geneId);
            //    }
            //}
            //// sorted list
            //GeneIdArray = geneIdSet.OrderBy(x => x).ToArray();
            //// build GetGeneArrayIndexFromGeneId
            //GetGeneIdArrayIndexFromGeneId = GeneIdArray.Select((geneId, index) => new { geneId, index }).ToDictionary(x => x.geneId, x => x.index);
        }

        // File: tnbc
        // 210337_s_at	ACLY	47	2.191696988	1.98E-67
        // 201629_s_at	ACP1	52	2.194764244	8.39E-58
        public void ParseDiseaseGeneExpression(string filePath)
        {
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] tokens = line.Split('\t');
                        if (tokens.Length == 2)
                        {
                            GetGeneScore.Add(int.Parse(tokens[0]), double.Parse(tokens[1]));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Trace.WriteLine("The file could not be read:");
                Trace.WriteLine(e.Message);
            }
        }

        public void ParseCmapStyle(string filePath, out int drugCount, out int geneCount)
        {
            drugCount = 0;
            geneCount = 0;
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                drugCount = 0;
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    bool header_read = false;
                    int[] gene_list = new int[1];
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!header_read)
                        {
                            gene_list = line.Split('\t').Skip(2).Select(x => int.Parse(x)).ToArray();
                            header_read = true;
                        }
                        else
                        {
                            string[] tokens = line.Split('\t');
                            string drugId = tokens[0];
                            Drug drug = new Drug(drugId);
                            string[] info = tokens[1].Split(',');
                            drug.Name = info[0].Split('=')[1];
                            drug.Info = tokens[1].Substring(info[0].Length + 1);
                            for (int i = 2; i < tokens.Length; i++)
                            {
                                double fc = double.Parse(tokens[i]);
                                if (fc != 0)
                                {
                                    drug.Genes.Add(gene_list[i - 2], fc);
                                }
                            }
                            Drugs.Add(drug);
                            drugCount++;
                        }
                    }
                    geneCount = gene_list.Where(x => GetGeneScore.ContainsKey(x)).Count();
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Trace.WriteLine("The file could not be read:");
                Trace.WriteLine(e.Message);
            }
        }
    }
}
