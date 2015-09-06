using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace MapResourceRating
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Model _model = new Model();
        BackgroundWorker _worker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = _model;

            _model.MapFolder =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Steam\steamapps\common\OMSI 2\maps\Grundorf");

            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;
            _worker.ProgressChanged += worker_ProgressChanged;
            _worker.DoWork += worker_DoWork;
            _worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, OpenExecuted, OpenCanExcute));
            CommandBindings.Add(new CommandBinding(MediaCommands.Play, PlayExecuted, PlayCanExcute));
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
           AddMessage("Analysis complete");
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var mapFolder = e.Argument as string;

            AnalyseMap(mapFolder);

        }


        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            AddMessage(e.UserState as string);
        }

        private void PlayCanExcute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute =
                !_worker.IsBusy &&
                !string.IsNullOrWhiteSpace(_model.MapFolder) &&
                File.Exists(Path.Combine(_model.MapFolder, "Global.cfg"));
        }

        private void PlayExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            _worker.RunWorkerAsync(_model.MapFolder);
            _model.Messages.Clear();
            AddMessage("Analysis started");
        }

        private void OpenCanExcute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = true;
        }

        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = _model.MapFolder;
                dialog.ShowNewFolderButton = false;
                dialog.Description = "Select your map folder";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _model.MapFolder = dialog.SelectedPath;
                }
            }
        }

        private void AddMessage(string message)
        {
            _model.Messages.Add(string.Format("{0}: {1}", DateTime.Now.ToLongTimeString(), message));
        }

        private void BackgroundAddMessage(string message)
        {
            _worker.ReportProgress(0, message);
        }

        private void AnalyseMap(string mapFolder)
        {
            var map = new Map();

            using (var config = File.OpenText(Path.Combine(mapFolder, "global.cfg")))
            {
                int lineNumber = 0;
                while (true)
                {
                    var line = config.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    lineNumber++;
                    if (line == "[map]")
                    {
                        var tile = new Tile();
                        if (!int.TryParse(config.ReadLine(), out tile.X))
                        {
                            BackgroundAddMessage(string.Format("Failed to read X tile co-ordinate at line {0}", lineNumber));
                            return;
                        }
                        tile.Y = int.Parse(config.ReadLine());
                        tile.FileName = config.ReadLine();
                        map.Tiles.Add(tile);
                    }
                }
                BackgroundAddMessage(string.Format("Found {0} map tiles", map.Tiles.Count));

            }
        }

    }
}
