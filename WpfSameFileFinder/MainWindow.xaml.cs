using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using FileInfo = SameFileFinder.FileInfo;

namespace WpfSameFileFinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            //backgroundWorker.DoWork += BackgroundWorker_DoWork;
            //backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            //backgroundWorker.WorkerSupportsCancellation = true;
            //backgroundWorker.WorkerReportsProgress = true;
            //lastStackPanel.RegisterName("wpfProgressBar", wpfProgressBar);
        }

        //private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    try
        //    {
        //        // Back on primary thread, can access ui controls 
        //        wpfProgressBarAndText.Visibility = Visibility.Collapsed;

        //        if (e.Cancelled)
        //        {
        //            files.ItemsSource = "There is no Data to display";
        //        }
        //        else
        //        {
        //            files.ItemsSource = Groups;
        //        }
        //    }
        //    finally
        //    {
        //        Calculate.Content = "Start";
        //    }
        //}

        //private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    var manager = new FileManager();
        //    var logger = new Logger("", "log.txt");
        //    var finder = new Finder();
        //    e.Result = finder.FindGroupOfSameFiles(
        //        CurrentPath, logger, manager);
        //    Groups = new List<FileInfo>();

        //    foreach (var gr in (List<FileGroup>)e.Result)
        //    {
        //        Groups.AddRange(gr.Files);
        //    }
        //    if (backgroundWorker.CancellationPending)
        //    {
        //        e.Cancel = true;
        //    }
        //}

        //private void EnterTheFolderMenuItem_Click(object sender, RoutedEventArgs e)
        //{
        //    var source = files.ItemsSource.Cast<FileInfo>().ToList();
        //    var deleteFilePath = source.Where((s, i) => i == files.SelectedIndex).Select(s => s.Path).First();
        //    var fileFolder = new ProcessStartInfo("explorer.exe", "/select,\"" + deleteFilePath + "\"");
        //    fileFolder.UseShellExecute = false;
        //    Process.Start(fileFolder);
        //}

        //private void DeleteTheElement_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void selectPath_Click(object sender, RoutedEventArgs e)
        //{
        //    var folderDialog = new FolderBrowserDialog();
        //    folderDialog.Description = @"Select folder to find similar files.";

        //    folderDialog.ShowNewFolderButton = false;
        //    DialogResult result = folderDialog.ShowDialog();
        //    if (result == System.Windows.Forms.DialogResult.OK)
        //    {
        //        CurrentPath = folderDialog.SelectedPath;
        //        folderPath.Text = CurrentPath;
        //    }
        //    else
        //    {
        //        System.Windows.Forms.MessageBox.Show(@"You have not selected or canceled Path selection popup");
        //    }
        //}

        //private void StartButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (Directory.Exists(CurrentPath))
        //    {

        //        if (Calculate.Content.Equals("Start"))
        //        {
        //            Calculate.Content = "Stop";
        //            files.AutoGenerateColumns = true;
        //            backgroundWorker.RunWorkerAsync();
        //            wpfProgressBarAndText.Visibility = Visibility.Visible;

        //        }
        //        else
        //        {
        //            // Cancel the asynchronous operation.                 
        //            backgroundWorker.CancelAsync();
        //            backgroundWorker = new BackgroundWorker();
        //            backgroundWorker.DoWork += BackgroundWorker_DoWork;
        //            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        //            backgroundWorker.WorkerSupportsCancellation = true;
        //            backgroundWorker.WorkerReportsProgress = true;

        //            backgroundWorker = new BackgroundWorker();
        //            Calculate.Content = "Start";
        //            wpfProgressBarAndText.Visibility = Visibility.Collapsed;

        //        }
        //    }
        //    else
        //    {
        //        System.Windows.Forms.MessageBox.Show(@"Please select correct folder");
        //    }
        //}


    }
}
