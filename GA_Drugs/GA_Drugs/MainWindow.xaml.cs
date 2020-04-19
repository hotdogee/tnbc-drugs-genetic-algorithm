using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Controls.DataVisualization.Charting;
using System.Data;
using System.Xml.Serialization;
using System.Web.Script.Serialization;
using System.Collections;

namespace GA_Drugs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private class TaskManager
        {
            public class ObservableCollectionGeneticAlgorithmTaskConverter : JavaScriptConverter
            {

                public override IEnumerable<Type> SupportedTypes
                {
                    //Define the ListItemCollection as a supported type. 
                    get { return new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(ObservableCollection<GeneticAlgorithmTask>) })); }
                }

                public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
                {
                    ObservableCollection<GeneticAlgorithmTask> taskList = obj as ObservableCollection<GeneticAlgorithmTask>;

                    if (taskList != null)
                    {
                        // Create the representation.
                        Dictionary<string, object> result = new Dictionary<string, object>();
                        ArrayList itemsList = new ArrayList();
                        foreach (GeneticAlgorithmTask task in taskList)
                        {
                            //Add each entry to the dictionary.
                            Dictionary<string, object> taskDict = new Dictionary<string, object>();
                            taskDict.Add("Results", task.Results);
                            taskDict.Add("Name", task.Name);
                            taskDict.Add("CreationTime", task.CreationTime);
                            taskDict.Add("PopulationSize", task.PopulationSize);
                            taskDict.Add("MaxGenerations", task.MaxGenerations);
                            taskDict.Add("StagnationLimit", task.StagnationLimit);
                            taskDict.Add("ElitismPreservationAmount", task.ElitismPreservationAmount);
                            taskDict.Add("SelectionType", task.SelectionType);
                            taskDict.Add("MutationProbability", task.MutationProbability);
                            taskDict.Add("CrossoverProbability", task.CrossoverProbability);
                            itemsList.Add(taskDict);
                        }
                        result["List"] = itemsList;

                        return result;
                    }
                    return new Dictionary<string, object>();
                }

                public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
                {
                    if (dictionary == null)
                        throw new ArgumentNullException("dictionary");

                    if (type == typeof(ObservableCollection<GeneticAlgorithmTask>))
                    {
                        // Create the instance to deserialize into.
                        ObservableCollection<GeneticAlgorithmTask> taskList = new ObservableCollection<GeneticAlgorithmTask>();

                        // Deserialize the ListItemCollection's items.
                        ArrayList itemsList = (ArrayList)dictionary["List"];
                        for (int i = 0; i < itemsList.Count; i++)
                        {
                            //Dictionary<string, object> taskDict = itemsList[i] as Dictionary<string, object>;
                            //Dictionary<string, object> resultsDict = taskDict["Results"] as Dictionary<string, object>;
                            //GeneticAlgorithmTask task = new GeneticAlgorithmTask();
                            //GenerationInfo results = new GenerationInfo();
                            //results.FitnessDistributionCurrentGeneration = serializer.ConvertToType<List<KeyValuePair<int, double>>>(resultsDict["FitnessDistributionCurrentGeneration"]);
                            //results.BestFitnessEachGeneration = serializer.ConvertToType<List<KeyValuePair<int, double>>>(resultsDict["BestFitnessEachGeneration"]);
                            //results.Report = serializer.ConvertToType<string>(resultsDict["Report"]);
                            //results.BestIndividual = serializer.ConvertToType<DomainKnowledge.Chromosome>(resultsDict["BestIndividual"]);
                            //task.Results = results;
                            //task.Results = serializer.ConvertToType<GenerationInfo>(taskDict["Results"]);
                            //task.Name = serializer.ConvertToType<string>(taskDict["Name"]);
                            //task.CreationTime = serializer.ConvertToType<DateTime>(taskDict["CreationTime"]);
                            //task.PopulationSize = serializer.ConvertToType<int>(taskDict["PopulationSize"]);
                            //task.MaxGenerations = serializer.ConvertToType<int>(taskDict["MaxGenerations"]);
                            //task.StagnationLimit = serializer.ConvertToType<int>(taskDict["StagnationLimit"]);
                            //task.ElitismPreservationAmount = serializer.ConvertToType<int>(taskDict["ElitismPreservationAmount"]);
                            //task.SelectionType = serializer.ConvertToType<GeneticAlgorithmTask.Selection>(taskDict["SelectionType"]);
                            //task.CrossoverProbability = serializer.ConvertToType<double>(taskDict["MutationProbability"]);
                            //task.MutationProbability = serializer.ConvertToType<double>(taskDict["CrossoverProbability"]);
                            //taskList.Add(task);
                            taskList.Add(serializer.ConvertToType<GeneticAlgorithmTask>(itemsList[i]));
                        }
                        return taskList;
                    }
                    return null;
                }

            }

            public ObservableCollection<GeneticAlgorithmTask> TaskList;
            private string defaultTaskListFileName;
            private string workingTaskListFileName;
            private JavaScriptSerializer serializer;

            public TaskManager()
            {
                TaskList = new ObservableCollection<GeneticAlgorithmTask>();
                string cwd = System.IO.Directory.GetCurrentDirectory();
                defaultTaskListFileName = Path.Combine(cwd, "TaskList.json");
                serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(new JavaScriptConverter[] { new ObservableCollectionGeneticAlgorithmTaskConverter() });
            }

            public void SaveWorkingTaskList()
            {
                StreamWriter streamWriter = new StreamWriter(workingTaskListFileName, false);
                streamWriter.Write(serializer.Serialize(TaskList));
                streamWriter.Close();
            }

            public void LoadWorkingTaskList()
            {
                // check if application setting exist
                if (Properties.Settings.Default.WorkingTaskListFileName.Length == 0)
                {
                    Properties.Settings.Default.WorkingTaskListFileName = defaultTaskListFileName;
                    Properties.Settings.Default.Save();
                    workingTaskListFileName = defaultTaskListFileName;
                }
                else
                    workingTaskListFileName = Properties.Settings.Default.WorkingTaskListFileName;
                // check if taskListFileName exists
                if (!File.Exists(workingTaskListFileName)) // load default it file doesn't exist
                {
                    Properties.Settings.Default.WorkingTaskListFileName = defaultTaskListFileName;
                    Properties.Settings.Default.Save();
                    workingTaskListFileName = defaultTaskListFileName;
                }
                // now load file if exists, if not do nothing
                FileInfo fileInfo = new FileInfo(workingTaskListFileName);
                if (fileInfo.Exists)
                {
                    StreamReader sr = new StreamReader(fileInfo.OpenRead());
                    TaskList = serializer.Deserialize<ObservableCollection<GeneticAlgorithmTask>>(sr.ReadToEnd());
                    if (TaskList == null) // if file exists but is empty, in case of manual deletion
                    {
                        TaskList = new ObservableCollection<GeneticAlgorithmTask>();
                    }
                    sr.Close();
                }
            }
        }
        public class Task2 : System.ComponentModel.INotifyPropertyChanged
        {
            // The Task class implements INotifyPropertyChanged so that 
            // the datagrid row will be notified of changes to the data
            // that are made in the row details section.

            // Private task data.
            private string m_Name;
            private DateTime m_DueDate;
            private bool m_Complete;
            private string m_Notes;

            // Define the public properties.
            public string Name
            {
                get { return this.m_Name; }
                set
                {
                    if (value != this.m_Name)
                    {
                        this.m_Name = value;
                        NotifyPropertyChanged("Name");
                    }
                }
            }

            public DateTime DueDate
            {
                get { return this.m_DueDate; }
                set
                {
                    if (value != this.m_DueDate)
                    {
                        this.m_DueDate = value;
                        NotifyPropertyChanged("DueDate");
                    }
                }
            }

            public bool Complete
            {
                get { return this.m_Complete; }
                set
                {
                    if (value != this.m_Complete)
                    {
                        this.m_Complete = value;
                        NotifyPropertyChanged("Complete");
                    }
                }
            }

            public string Notes
            {
                get { return this.m_Notes; }
                set
                {
                    if (value != this.m_Notes)
                    {
                        this.m_Notes = value;
                        NotifyPropertyChanged("Notes");
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
        }

        private Database db;
        private GeneticAlgorithmEngine engine;
        private TaskManager taskManager;
        private Stopwatch stopWatch; // for Chart update interval

        public MainWindow()
        {
            InitializeComponent();
            // Add version to Title
            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Title = string.Format("GA Drugs {0} - NTU CSIE R401 Han Lin", version);

            // DEBUG
            Button buttonDebug;

            // Initialize Database
            db = new Database();
            // Generate CheckBoxes
            List<CheckBox> checkBoxLoadDatabase = new List<CheckBox>(db.InitializeProgressText.Length);
            foreach (string progressText in db.InitializeProgressText)
            {
                // <CheckBox Content="KEGG Drugs" IsChecked="False" IsHitTestVisible="False" Name="checkBoxDrugs" />
                CheckBox checkBox = new CheckBox();
                checkBox.Content = progressText;
                checkBox.IsChecked = false;
                checkBox.IsHitTestVisible = false;
                checkBoxLoadDatabase.Add(checkBox);
                stackPanelInitialize.Children.Add(checkBox);
            }
            // <Button Content="Load Database" Margin="3" Name="buttonLoadDatabase" Click="buttonLoadDatabase_Click" />
            Button buttonLoadDatabase = new Button();
            buttonLoadDatabase.Content = "Load Database";
            buttonLoadDatabase.Margin = new Thickness(3);
            buttonLoadDatabase.Tag = new Stopwatch();
            buttonLoadDatabase.Click += new RoutedEventHandler(delegate(object sender, RoutedEventArgs args)
            {
                (buttonLoadDatabase.Tag as Stopwatch).Start();
                db.Initialize();
                buttonLoadDatabase.IsEnabled = false;
                buttonLoadDatabase.Content = "Loading...";
            });
            stackPanelInitialize.Children.Add(buttonLoadDatabase);
            int dbLoadProgress = 0;
            db.Worker.ProgressChanged += new ProgressChangedEventHandler(delegate(object sender, ProgressChangedEventArgs e)
            {
                checkBoxLoadDatabase[dbLoadProgress].IsChecked = true;
                if (e.UserState != null && (e.UserState as string).Length > 0)
                {
                    checkBoxLoadDatabase[dbLoadProgress].Content += string.Format(" - {0}", e.UserState as string);
                }
                dbLoadProgress++;
            });
            db.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate(object senderdb, RunWorkerCompletedEventArgs edb)
            {
                (buttonLoadDatabase.Tag as Stopwatch).Stop();
                buttonLoadDatabase.Content = string.Format("Loaded in {0:0}s", (buttonLoadDatabase.Tag as Stopwatch).Elapsed.TotalSeconds);
                expanderDatabase.Header = "Database - Loaded";
                expanderCreateTask.IsEnabled = true;
                expanderCreateTask.IsExpanded = true;
                expanderConfigFile.IsEnabled = true;
                dataGridTaskList.IsEnabled = true;
                // display data in datagrids
                DataSet ds = new DataSet();
                foreach (DataTab dataTab in db.DataTabs)
                {
                    //<TabItem Header="Drug Targets" IsEnabled="True" Visibility="Hidden" Name="tabItemDrugTargets">
                    //  <DataGrid x:Name="dataGridDrugTargets" AlternatingRowBackground="PaleGoldenrod" AlternationCount="2"></DataGrid>
                    //</TabItem>
                    ds.Tables.Add(dataTab.DataTable);
                    DataGrid dataGrid = new DataGrid();
                    dataGrid.AlternatingRowBackground = Brushes.PaleGoldenrod;
                    dataGrid.AlternationCount = 2;
                    dataGrid.ItemsSource = dataTab.DataTable.DefaultView;
                    TabItem tabItem = new TabItem();
                    tabItem.Header = dataTab.Name;
                    tabItem.IsEnabled = true;
                    tabItem.Visibility = Visibility.Visible;
                    tabItem.Content = dataGrid;
                    tabControlMain.Items.Add(tabItem);
                    // Output total genes
                    textBoxOutput.Text += dataTab.Info + "\n";
                }
                // DEBUG
                // Get Gene Symbol
                TextBox textBoxGetGeneSymbol = new TextBox();
                textBoxGetGeneSymbol.Margin = new Thickness(3);
                textBoxGetGeneSymbol.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                textBoxGetGeneSymbol.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                stackPanelDebug.Children.Add(textBoxGetGeneSymbol);

                buttonDebug = new Button();
                buttonDebug.Content = "Get Gene Symbol";
                buttonDebug.Margin = new Thickness(3);
                buttonDebug.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                buttonDebug.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                buttonDebug.Tag = new Stopwatch();
                buttonDebug.Click += new RoutedEventHandler(delegate(object sender, RoutedEventArgs args)
                {
                    (buttonDebug.Tag as Stopwatch).Start();
                    try
                    {
                        int geneId = int.Parse(textBoxGetGeneSymbol.Text);
                        textBoxOutput.Text += string.Format("{0} = {1}\n", geneId.ToString(), db.GetGeneSymbolFromEntrez[geneId]);
                    }
                    catch (Exception ex)
                    {
                        textBoxOutput.Text += ex.Message + '\n';
                    }
                    (buttonDebug.Tag as Stopwatch).Stop();
                });
                stackPanelDebug.Children.Add(buttonDebug);
                // Get Drug
                TextBox textBoxGetDrug = new TextBox();
                textBoxGetDrug.Margin = new Thickness(3);
                textBoxGetDrug.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                textBoxGetDrug.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                stackPanelDebug.Children.Add(textBoxGetDrug);

                buttonDebug = new Button();
                buttonDebug.Content = "Get Drug";
                buttonDebug.Margin = new Thickness(3);
                buttonDebug.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                buttonDebug.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                buttonDebug.Tag = new Stopwatch();
                buttonDebug.Click += new RoutedEventHandler(delegate(object sender, RoutedEventArgs args)
                {
                    (buttonDebug.Tag as Stopwatch).Start();
                    try
                    {
                        foreach (Drug drug in db.Drugs)
                        {
                            if (drug.DrugId == textBoxGetDrug.Text || drug.Name == textBoxGetDrug.Text)
                            {
                                textBoxOutput.Text += string.Format("ID={0}\nName={1}\nScore={2}\nInfo={3}\n", drug.DrugId, drug.Name, drug.Score, drug.Info);
                                // build stringBuilderGenes
                                StringBuilder stringBuilderGenes = new StringBuilder();
                                var top10Genes = drug.Genes.OrderByDescending(x => Math.Abs(x.Value)).Take(10);
                                foreach (var kvp in top10Genes)
                                {
                                    stringBuilderGenes.AppendFormat("{0}({1})={2}, ", db.GetGeneSymbolFromEntrez[kvp.Key], kvp.Key, kvp.Value);
                                }
                                if (stringBuilderGenes.Length > 2)
                                    stringBuilderGenes.Remove(stringBuilderGenes.Length - 2, 2);
                                textBoxOutput.Text += string.Format("Top 10 Targets={0}\n", stringBuilderGenes.ToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxOutput.Text += ex.Message + '\n';
                    }
                    (buttonDebug.Tag as Stopwatch).Stop();
                });
                stackPanelDebug.Children.Add(buttonDebug);

                // Find best ppi parameters
                BackgroundWorker greedyPpiWorker = new BackgroundWorker();
                greedyPpiWorker.WorkerReportsProgress = true;
                greedyPpiWorker.DoWork += new DoWorkEventHandler((se, e) =>
                {
                    string cwd = System.IO.Directory.GetCurrentDirectory();
                    string path = Path.Combine(cwd, @"data\ppi_parameters");
                    using (StreamWriter sw = new StreamWriter(path, true)) // append
                    {
                        sw.AutoFlush = true;
                        int i = 0;
                        string[] spliter = { " (" };
                        for (double up_value = 1; up_value < 6.1; up_value += 0.5)
                        {
                            for (double down_value = 1; down_value < 6.1; down_value += 0.5)
                            {
                                for (double ppi_factor = 0.3; ppi_factor < 0.8; ppi_factor += 0.1)
                                {
                                    for (int ppi_level = 0; ppi_level < 3; ppi_level++)
                                    {
                                        Database db_new = new Database(db);
                                        DrugTargetPpiParameters p = new DrugTargetPpiParameters(up_value, down_value * -1, ppi_factor, ppi_level);
                                        db_new.ApplyPpiToDrugTargets(p);
                                        db_new.CalculateScores();
                                        // stats
                                        Drug[] positiveScoreDrugs = db_new.Drugs.Where(d => d.Score > 0 && d.DrugId[0] == 'D').OrderByDescending(d => d.Score).ToArray();
                                        double[] positiveScores = positiveScoreDrugs.Select(d => d.Score).ToArray();
                                        int positiveCount = positiveScoreDrugs.Length;
                                        double maxScore = 0;
                                        double medianScore = 0;
                                        if (positiveCount > 0)
                                        {
                                            maxScore = positiveScores[0];
                                            medianScore = positiveScores[positiveCount / 2];
                                        }
                                        StringBuilder sb = new StringBuilder();
                                        foreach (Drug drug in positiveScoreDrugs.Take(3))
                                        {
                                            sb.AppendFormat("{0}={1}={2}\t", drug.DrugId, drug.Name.Split(spliter, StringSplitOptions.None)[0], drug.Score);
                                        }
                                        string result = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", up_value, down_value * -1, ppi_factor, ppi_level, positiveCount, maxScore, medianScore, sb.ToString());
                                        sw.WriteLine(result);
                                        greedyPpiWorker.ReportProgress(i++, result);
                                    }
                                }
                            }
                        }
                    }
                });
                greedyPpiWorker.ProgressChanged += new ProgressChangedEventHandler((s, e) =>
                {
                    if (e.UserState != null && (e.UserState as string).Length > 0)
                    {
                        textBoxOutput.Text += e.UserState as string + "\n";
                    }
                });
                greedyPpiWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((s, e) =>
                {
                    textBoxOutput.Text += "Greedy PPI Args Complete!\n";
                });
                buttonDebug = new Button();
                buttonDebug.Content = "Greedy PPI Args";
                buttonDebug.Margin = new Thickness(3);
                buttonDebug.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                buttonDebug.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                buttonDebug.Tag = new Stopwatch();
                buttonDebug.Click += new RoutedEventHandler(delegate(object sender, RoutedEventArgs args)
                {
                    (buttonDebug.Tag as Stopwatch).Start();
                    greedyPpiWorker.RunWorkerAsync();
                    (buttonDebug.Tag as Stopwatch).Stop();
                });
                stackPanelDebug.Children.Add(buttonDebug);
            });

            // for Chart update interval
            stopWatch = new Stopwatch();

            taskManager = new TaskManager();
            taskManager.LoadWorkingTaskList();

            // Create a list to store task data.
            List<Task2> taskList = new List<Task2>();
            int itemsCount = 50;

            // Generate some task data and add it to the task list.
            for (int i = 1; i <= itemsCount; i++)
            {
                taskList.Add(new Task2()
                {
                    Name = "Task " + i.ToString(),
                    DueDate = DateTime.Now.AddDays(i),
                    Complete = (i % 3 == 0),
                    Notes = "Task " + i.ToString() + " is due on "
                          + DateTime.Now.AddDays(i) + ". Lorum ipsum..."
                });
            }
            //dataGridTaskList.ItemsSource = taskList;
            dataGridTaskList.ItemsSource = taskManager.TaskList;

            //List<KeyValuePair<int, double>> GenerationFitnessList = new List<KeyValuePair<int, double>>();
            //GenerationFitnessList.Add(new KeyValuePair<int, double>(1, 3.4));
            //GenerationFitnessList.Add(new KeyValuePair<int, double>(2, 5.7));
            //GenerationFitnessList.Add(new KeyValuePair<int, double>(3, 8.0));
            //GenerationFitnessList.Add(new KeyValuePair<int, double>(4, 10.4));
            //GenerationFitnessList.Add(new KeyValuePair<int, double>(2, 13.7));
            //GenerationFitnessList.Add(new KeyValuePair<int, double>(7, 14.4));
            //lineSeriesBestFitnessEachGeneration.ItemsSource = GenerationFitnessList;

            // DEBUG
            // <Button Content="Load Database" Margin="3" Name="buttonLoadDatabase" Click="buttonLoadDatabase_Click" />
            // <Button Content="Parse Drugs" HorizontalAlignment="Stretch" Name="buttonTest" VerticalAlignment="Top" Click="buttonParseDrugs_Click" />
            buttonDebug  = new Button();
            buttonDebug.Content = "Parse Drugs";
            buttonDebug.Margin = new Thickness(3);
            buttonDebug.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            buttonDebug.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            buttonDebug.Tag = new Stopwatch();
            buttonDebug.Click += new RoutedEventHandler(delegate(object sender, RoutedEventArgs args)
            {
                (buttonDebug.Tag as Stopwatch).Start();
                // get running directory
                string cwd = System.IO.Directory.GetCurrentDirectory();
                string path;
                int drugCount, geneCount;
                // cmap
                path = Path.Combine(cwd, @"data\ratioMatrix_cmap_gene_max_filtered_log");
                db.ParseCmapStyle(path, out drugCount, out geneCount);
                // drugbank
                path = Path.Combine(cwd, @"data\drugbank_approved_with_usable_target_d4.4_u5.5.cmap_style");
                db.ParseCmapStyle(path, out drugCount, out geneCount);
                // kegg
                path = Path.Combine(cwd, @"data\kegg_drug_with_usable_target_d4.4_u5.5.cmap_style");
                db.ParseCmapStyle(path, out drugCount, out geneCount);
                (buttonDebug.Tag as Stopwatch).Stop();
            });
            stackPanelDebug.Children.Add(buttonDebug);

            // <Button Content="Parse Pathways" HorizontalAlignment="Stretch" Name="buttonParsePathways" VerticalAlignment="Top" Click="buttonParsePathways_Click" />
            buttonDebug = new Button();
            buttonDebug.Content = "Parse Pathways";
            buttonDebug.Margin = new Thickness(3);
            buttonDebug.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            buttonDebug.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            buttonDebug.Tag = new Stopwatch();
            buttonDebug.Click += new RoutedEventHandler(delegate(object sender, RoutedEventArgs args)
            {
                (buttonDebug.Tag as Stopwatch).Start();
                textBoxOutput.Text += "ParsePathways: ";
                // get running directory
                string cwd = System.IO.Directory.GetCurrentDirectory();
                string path = Path.Combine(cwd, @"data\kegg_pathway_filtered");
                (buttonDebug.Tag as Stopwatch).Stop();
            });
            stackPanelDebug.Children.Add(buttonDebug);

            // <Button Content="Parse Disease Gene Expression" HorizontalAlignment="Stretch" Name="buttonParseHccScore" VerticalAlignment="Top" Click="buttonParseDiseaseGeneExpression_Click" />
            buttonDebug = new Button();
            buttonDebug.Content = "Parse Disease Gene Expression";
            buttonDebug.Margin = new Thickness(3);
            buttonDebug.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            buttonDebug.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            buttonDebug.Tag = new Stopwatch();
            buttonDebug.Click += new RoutedEventHandler(delegate(object sender, RoutedEventArgs args)
            {
                (buttonDebug.Tag as Stopwatch).Start();
                textBoxOutput.Text += "ParseHccScore: ";
                // get running directory
                string cwd = System.IO.Directory.GetCurrentDirectory();
                string path;

                // Disease Gene Expression
                path = Path.Combine(cwd, @"data\tnbc");
                db.ParseDiseaseGeneExpression(path);
                (buttonDebug.Tag as Stopwatch).Stop();
            });
            stackPanelDebug.Children.Add(buttonDebug);

            // <Button Content="Run GA" HorizontalAlignment="Stretch" Name="buttonRunGa" VerticalAlignment="Top" Click="buttonRunGa_Click" />
            buttonDebug = new Button();
            buttonDebug.Content = "Run GA";
            buttonDebug.Margin = new Thickness(3);
            buttonDebug.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            buttonDebug.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            buttonDebug.Tag = new Stopwatch();
            buttonDebug.Click += new RoutedEventHandler(delegate(object sender, RoutedEventArgs args)
            {
                (buttonDebug.Tag as Stopwatch).Start();
                //DomainKnowledge.Chromosome indiv = new DomainKnowledge.Chromosome(5);
                //indiv.MakeRandom();
                //foreach (int num in indiv.Data)
                //{
                //    Trace.Write(num);
                //}
                //Trace.Write("\n");
                //DomainKnowledge.GetFitness(indiv);
                (buttonDebug.Tag as Stopwatch).Stop();
            });
            stackPanelDebug.Children.Add(buttonDebug);

        }

        private void buttonJobStart_Click(object sender, RoutedEventArgs e)
        {
            GeneticAlgorithmTask task;
            int index = dataGridTaskList.SelectedIndex;
            if (index != -1 && index < taskManager.TaskList.Count)
                task = taskManager.TaskList[index];
            else
                return;

            engine = new GeneticAlgorithmEngine(task);
            engine.Start();

            // Start chart update timer
            stopWatch.Start();
        }

        public void GaWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            GenerationInfo results = (GenerationInfo)(e.UserState);
            bool updatePlotGeneration = false;
            bool updatePlotIndividual = false;
            if (stopWatch.ElapsedMilliseconds > long.Parse(textBoxChartUpdateInterval.Text))
            {
                if (checkBoxPlotGeneration.IsChecked.Value)
                    updatePlotGeneration = true;
                if (checkBoxPlotIndividual.IsChecked.Value)
                    updatePlotIndividual = true;
                stopWatch.Reset();
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();
            DisplayResults(results, updatePlotGeneration, updatePlotIndividual);
            sw.Stop();
            // to prevent ui hanging, make sure the textBoxChartUpdateInterval.Text is larger than 10x DisplayResults runtimes
            if (long.Parse(textBoxChartUpdateInterval.Text) < sw.ElapsedMilliseconds * 10)
            {
                textBoxChartUpdateInterval.Text = (sw.ElapsedMilliseconds * 11).ToString();
            }
            if (!stopWatch.IsRunning)
            {
                stopWatch.Restart();
            }
        }

        public void GaWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
            }
            else if (e.Cancelled)
            {
                // Next, handle the case where the user canceled 
                // the operation.
                // Note that due to a race condition in 
                // the DoWork event handler, the Cancelled
                // flag may not have been set, even though
                // CancelAsync was called.
                DisplayResults((GenerationInfo)(e.Result));
            }
            else
            {
                // Finally, handle the case where the operation 
                // succeeded.
                DisplayResults((GenerationInfo)(e.Result));
            }
        }

        private void DisplayResults(GenerationInfo results, bool updatePlotGeneration = true, bool updatePlotIndividual = true)
        {
            textBoxCurrentAnswer.Text = results.Report;
            // Set new context for Fitness Distribution In Current Generation chart
            if (updatePlotIndividual)
            {
                lineSeriesFitnessDistributionCurrentGeneration.Title = string.Format("Gen{0}", results.BestFitnessEachGeneration.Count);
                lineSeriesFitnessDistributionCurrentGeneration.ItemsSource = results.FitnessDistributionCurrentGeneration;
            }
            // Set new context for Best Fitness In Each Generation chart
            if (updatePlotGeneration)
            {
                double axisMax = 20.0;
                while (axisMax < results.BestFitnessEachGeneration.Count)
                    axisMax *= 2;
                independentAxisBestFitnessEachGeneration.Maximum = axisMax;
                lineSeriesBestFitnessEachGeneration.Title = results.Report.Split(':')[0];
                lineSeriesBestFitnessEachGeneration.ItemsSource = results.BestFitnessEachGeneration;
            }
        }

        private void dataGridPathwayScores_AutoGeneratedColumns(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            // pathway names are too long, seeing the first few letters should be enough
            if (e.Column.Header.ToString() == "Name")
                e.Column.Width = 100;
        }

        private void buttonCreateTask_Click(object sender, RoutedEventArgs e)
        {
            GeneticAlgorithmTask task = new GeneticAlgorithmTask();
            task.Name = "Task" + (taskManager.TaskList.Count + 1).ToString();
            task.CreationTime = DateTime.Now;
            task.DomainKnowledge = new DomainKnowledge(db);

            int temp;
            double tempDouble;
            // Population Size
            if (!int.TryParse(textBoxPopulationSize.Text, out temp))
            {// if failed
                temp = 100;
                textBoxPopulationSize.Text = temp.ToString();
            }
            task.PopulationSize = temp;

            // Max Generations
            if (!int.TryParse(textBoxMaxGenerations.Text, out temp))
            {// if failed
                temp = 500;
                textBoxMaxGenerations.Text = temp.ToString();
            }
            task.MaxGenerations = temp;

            // Stagnation Limit
            if (!int.TryParse(textBoxStagnationLimit.Text, out temp))
            {// if failed
                temp = 20;
                textBoxStagnationLimit.Text = temp.ToString();
            }
            task.StagnationLimit = temp;

            // Elite Count
            if (!int.TryParse(textBoxElitismPreservationAmount.Text, out temp))
            {// if failed
                temp = 5;
                textBoxElitismPreservationAmount.Text = temp.ToString();
            }
            task.ElitismPreservationAmount = temp;

            task.SelectionType = GeneticAlgorithmTask.Selection.RankSelection;

            // Crossover Probability
            if (!double.TryParse(textBoxCrossoverRate.Text, out tempDouble))
            {// if failed
                tempDouble = 0.8;
                textBoxCrossoverRate.Text = tempDouble.ToString();
            }
            task.CrossoverProbability = tempDouble;

            // Mutation Probability
            if (!double.TryParse(textBoxMutationRate.Text, out tempDouble))
            {// if failed
                tempDouble = 0.5;
                textBoxMutationRate.Text = tempDouble.ToString();
            }
            task.MutationProbability = tempDouble;

            task.ProgressChangedEventHandler = new ProgressChangedEventHandler(GaWorker_ProgressChanged);
            task.RunWorkerCompletedEventHandler = new RunWorkerCompletedEventHandler(GaWorker_RunWorkerCompleted);

            // add to taskManager
            taskManager.TaskList.Add(task);
        }

        private void dataGridTaskList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            taskManager.SaveWorkingTaskList();
        }

        private void buttonTaskDelete_Click(object sender, RoutedEventArgs e)
        {
            int index = dataGridTaskList.SelectedIndex;
            if (index != -1 && index < taskManager.TaskList.Count)
                taskManager.TaskList.RemoveAt(index);
        }

        private void dataGridTaskList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = dataGridTaskList.SelectedIndex;
            if (index != -1 && index <taskManager.TaskList.Count)
            {
                groupBoxSelectedJob.Header = "Selected: " + taskManager.TaskList[index].Name;
                // check for valid _db in task
                if (taskManager.TaskList[index].DomainKnowledge != null)
                    SetTaskControlButtonEnabled(true);
                else
                    SetTaskControlButtonEnabled(false);

                // update charts if task list has results in it
                if (taskManager.TaskList[index].Results != null)
                {
                    DisplayResults(taskManager.TaskList[index].Results);
                }
            }
            else
            {
                groupBoxSelectedJob.Header = "Selected: None";
                SetTaskControlButtonEnabled(false);
            }
        }

        private void SetTaskControlButtonEnabled(bool isEnabled)
        {
            buttonJobStart.IsEnabled = isEnabled;
            buttonJobPause.IsEnabled = isEnabled;
            buttonJobStop.IsEnabled = isEnabled;
            buttonJobStep.IsEnabled = isEnabled;
            imageJobStart.Opacity = (isEnabled) ? 1.0 : 0.5;
            imageJobPause.Opacity = (isEnabled) ? 1.0 : 0.5;
            imageJobStop.Opacity = (isEnabled) ? 1.0 : 0.5;
            imageJobStep.Opacity = (isEnabled) ? 1.0 : 0.5;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
