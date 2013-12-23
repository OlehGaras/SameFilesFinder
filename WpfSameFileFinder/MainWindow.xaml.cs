using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using SameFileFinder;

namespace WpfSameFileFinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string CurrentPath { get; set; }
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        List<MyFileInfo> Groups = new List<MyFileInfo>();

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            this.backgroundWorker.DoWork += BackgroundWorker_DoWork;
            this.backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.WorkerReportsProgress = true;
            lastStackPanel.RegisterName("wpfProgressBar", wpfProgressBar);
        }
       
        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                // Back on primary thread, can access ui controls 
                wpfProgressBarAndText.Visibility = Visibility.Collapsed;

                if (e.Cancelled)
                {
                    files.ItemsSource = "There is no Data to display";
                }
                else
                {
                    files.ItemsSource = Groups;
                }
            }
            finally
            {
                this.Calculate.Content = "Start";
                fileExtension.Visibility = Visibility.Visible;
            }
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            
            var manager = new FileManager();
            var logger = new Logger("log.txt");
            var finder = new Finder();
            e.Result = finder.FindGroupOfSameFiles(this.CurrentPath, logger, manager);
            Groups = new List<MyFileInfo>();
            foreach (var gr in (List<FileGroup>)e.Result)
            {
                Groups.AddRange(gr.Group);
                Groups.Add(new MyFileInfo());
            }
            if (this.backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
        }

        private void EnterTheFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var source = files.ItemsSource.Cast<MyFileInfo>().ToList();
            var deleteFilePath = source.Where((s, i) => i == files.SelectedIndex).Select(s => s.Information.DirectoryName + @"\" + s.Information.FullName).First();
            var fileFolder = new ProcessStartInfo("explorer.exe", "/select,\"" + deleteFilePath + "\"");
            fileFolder.UseShellExecute = false;
            Process.Start(fileFolder);
        }

        private void DeleteTheElement_Click(object sender, RoutedEventArgs e)
        {

        }

        private void selectPath_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog();
            folderDialog.Description = @"Select folder to find similar files.";

            folderDialog.ShowNewFolderButton = false;
            DialogResult result = folderDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.CurrentPath = folderDialog.SelectedPath;
                this.folderPath.Text = this.CurrentPath;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(@"You have not selected or canceled Path selection popup");
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(this.CurrentPath))
            {

                if (this.Calculate.Content.Equals("Start"))
                {
                    this.Calculate.Content = "Stop";
                    this.files.AutoGenerateColumns = true;
                    this.backgroundWorker.RunWorkerAsync();
                    this.wpfProgressBarAndText.Visibility = Visibility.Visible;
                    this.fileExtension.Visibility = Visibility.Collapsed;

                }
                else
                {
                    // Cancel the asynchronous operation.                 
                    backgroundWorker.CancelAsync();

                    backgroundWorker = new BackgroundWorker();
                    backgroundWorker.DoWork += BackgroundWorker_DoWork;
                    backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
                    backgroundWorker.WorkerSupportsCancellation = true;
                    backgroundWorker.WorkerReportsProgress = true;

                    backgroundWorker = new BackgroundWorker();
                    Calculate.Content = "Start";
                    wpfProgressBarAndText.Visibility = Visibility.Collapsed;
                    fileExtension.Visibility = Visibility.Visible;
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(@"Please select correct folder");
            }
        }

        
    }
}
