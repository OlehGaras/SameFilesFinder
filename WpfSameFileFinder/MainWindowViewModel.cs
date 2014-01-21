using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using SameFileFinder;
using FileInfo = SameFileFinder.FileInfo;
using ListView = System.Windows.Controls.ListView;
using TextBox = System.Windows.Controls.TextBox;

namespace WpfSameFileFinder
{
    public class MainWindowViewModel : ViewModelBase
    {
        private DelegateCommand m_GetGroupsCommand;
        private DelegateCommand m_SetThePath;
        private DelegateCommand m_EnterTheFolder;
        private DelegateCommand m_FindTheDirectories;
        private DelegateCommand m_OnTextBoxKeyPressed;
        private DelegateCommand m_OnPopupKeyPressed;
        private DelegateCommand m_GetPopup;
        private DelegateCommand m_GetTb;

        private ListView m_Popup;
        private TextBox m_Tb;

        private BackgroundWorker m_BackgroundWorker = new BackgroundWorker();
        private const int InitialColumnWidth = 100;
        private CancellationTokenSource m_Cts;

        public MainWindowViewModel()
        {
            m_Cts = new CancellationTokenSource();
            m_BackgroundWorker.DoWork += GetGroupsOfFiles;
            m_BackgroundWorker.RunWorkerCompleted += BackgroundWorkerRunWorkerCompleted;
            m_BackgroundWorker.WorkerSupportsCancellation = true;
            m_BackgroundWorker.WorkerReportsProgress = true;

            //lastStackPanel.RegisterName("wpfProgressBar", wpfProgressBar);
        }

        private bool m_Visible = true;
        public bool Visible
        {
            get { return m_Visible; }
            set
            {
                if (value != m_Visible)
                {
                    m_Visible = value;
                    OnPropertyChanged("Visible");
                }
            }
        }

        private string m_StartContent = "Start";
        public string StartContent
        {
            get { return m_StartContent; }
            set
            {
                if (value != m_StartContent)
                {
                    m_StartContent = value;
                    OnPropertyChanged("StartContent");
                }
            }
        }

        private List<FileGroup> m_Groups;
        public List<FileGroup> Groups
        {
            get { return m_Groups; }
            set
            {
                if (value != m_Groups)
                {
                    m_Groups = value;
                    OnPropertyChanged("Groups");
                }
            }
        }

        private FileInfo m_SelectedItem;
        public FileInfo SelectedItem
        {
            get { return m_SelectedItem; }
            set
            {
                if (m_SelectedItem != value)
                {
                    m_SelectedItem = value;
                    OnPropertyChanged("SelectedItem");
                }
            }
        }

        private bool m_ShowPopUp;
        public bool ShowPopUp
        {
            get { return m_ShowPopUp; }
            set
            {
                if (value != m_ShowPopUp)
                {
                    m_ShowPopUp = value;
                    OnPropertyChanged("ShowPopUp");
                }
            }
        }

        private List<DirectoryInfo> m_Directories;
        public List<DirectoryInfo> Directories
        {
            get { return m_Directories; }
            set
            {
                if (value != m_Directories)
                {
                    m_Directories = value;
                    OnPropertyChanged("Directories");
                }
            }
        }

        private static int m_MaxWidth = 525;
        public int MaxWidth
        {
            get { return m_MaxWidth; }
            set
            {
                if (value != m_MaxWidth)
                {
                    m_HashWidth = m_MaxWidth - m_PathWidth - m_NameWidth - m_LengthWidth - 30;
                    m_MaxWidth = value;
                    OnPropertyChanged("MaxWidth");
                    OnPropertyChanged("HashWidth");
                }
            }
        }

        private bool m_Entered;
        public bool Entered
        {
            get { return m_Entered; }
            set
            {
                if (value != m_Entered)
                {
                    m_Entered = value;
                    OnPropertyChanged("Entered");
                }
            }
        }

        private int m_PathWidth = InitialColumnWidth;
        public int PathWidth
        {
            get { return m_PathWidth; }
            set
            {
                if (value != m_PathWidth)
                {
                    if (m_NameWidth <= InitialColumnWidth && value > m_PathWidth)
                        return;
                    m_NameWidth += m_PathWidth - value;
                    m_PathWidth = value;
                    OnPropertyChanged("PathWidth");
                    OnPropertyChanged("NameWidth");
                }
            }
        }

        private int m_NameWidth = InitialColumnWidth;
        public int NameWidth
        {
            get { return m_NameWidth; }
            set
            {
                if (value != m_NameWidth)
                {
                    if (m_LengthWidth <= InitialColumnWidth && value > m_NameWidth)
                        return;
                    m_LengthWidth += m_NameWidth - value;
                    m_NameWidth = value;
                    OnPropertyChanged("NameWidth");
                    OnPropertyChanged("LengthWidth");
                }

            }
        }

        private int m_LengthWidth = InitialColumnWidth;
        public int LengthWidth
        {
            get { return m_LengthWidth; }
            set
            {
                if (value != m_LengthWidth)
                {
                    if (m_HashWidth <= InitialColumnWidth && value > m_LengthWidth)
                        return;
                    m_HashWidth += m_LengthWidth - value;
                    m_LengthWidth = value;
                    OnPropertyChanged("LengthWidth");
                    OnPropertyChanged("HashWidth");
                }
            }
        }

        private int m_HashWidth = m_MaxWidth - 3 * InitialColumnWidth - 30;
        public int HashWidth
        {
            get { return m_HashWidth; }
            set
            {
                if (value != m_HashWidth)
                {
                    m_HashWidth = value;
                    OnPropertyChanged("HashWidth");
                }
            }
        }

        private string m_CurrentPath;
        public string CurrentPath
        {
            get
            {
                return m_CurrentPath;
            }
            set
            {
                if (value != m_CurrentPath)
                {
                    m_CurrentPath = value;
                    FindDirectories();
                    OnPropertyChanged("CurrentPath");
                }
            }
        }

        public ICommand GetTb
        {
            get
            {
                if (m_GetTb == null)
                {
                    m_GetTb = new DelegateCommand(param => SaveTb(param));
                }
                return m_GetTb;
            }
        }

        public ICommand GetPopup
        {
            get
            {
                if (m_GetPopup == null)
                {
                    m_GetPopup = new DelegateCommand(param => SavePopup(param));
                }
                return m_GetPopup;
            }
        }

        public ICommand FindTheDirectories
        {
            get
            {
                if (m_FindTheDirectories == null)
                {
                    m_FindTheDirectories = new DelegateCommand(param => FindDirectories());
                }
                return m_FindTheDirectories;
            }
        }

        public ICommand EnterTheFolder
        {
            get
            {
                if (m_EnterTheFolder == null)
                {
                    m_EnterTheFolder = new DelegateCommand(param => GoToTheFolder(param));
                }
                return m_EnterTheFolder;
            }
        }

        public ICommand SetThePath
        {
            get
            {
                if (m_SetThePath == null)
                {
                    m_SetThePath = new DelegateCommand(param => SetPathDialog());
                }
                return m_SetThePath;
            }
        }

        public ICommand GetGroups
        {
            get
            {
                if (m_GetGroupsCommand == null)
                {
                    m_GetGroupsCommand = new DelegateCommand(param => GetGroupsOfFiles());
                }
                return m_GetGroupsCommand;
            }
        }

        public ICommand OnTextBoxKeyPressed
        {
            get
            {
                if (m_OnTextBoxKeyPressed == null)
                {
                    m_OnTextBoxKeyPressed = new DelegateCommand(param => OnTextBoxPressed(param));
                }
                return m_OnTextBoxKeyPressed;
            }
        }

        public ICommand OnPopupKeyPressed
        {
            get
            {
                if (m_OnPopupKeyPressed == null)
                {
                    m_OnPopupKeyPressed = new DelegateCommand(param => OnPopupPressed(param));
                }
                return m_OnPopupKeyPressed;
            }

        }

        private void OnPopupPressed(object o)
        {
            if (!m_Popup.IsFocused)
            {
                m_Popup.Focus();
                return;
            }
            var k = (System.Windows.Input.KeyEventArgs)o;
            if (k.Key == Key.Up)
            {
                if (m_Popup.SelectedIndex > 0)
                {
                    m_Popup.SelectedIndex -= 1;
                    return;
                }
                if (m_Tb != null)
                {
                    m_Tb.Focus();
                }
            }
            if (k.Key == Key.Down)
            {
                if (m_Popup.SelectedIndex != m_Popup.Items.Count - 1)
                    m_Popup.SelectedIndex += 1;
            }
            if (k.Key == Key.Return)
            {
                int startIndex = CurrentPath.LastIndexOf(@"\", StringComparison.Ordinal);
                CurrentPath = CurrentPath.Remove(startIndex, CurrentPath.Length - startIndex);
                CurrentPath += @"\" + m_Popup.SelectedItem + @"\";
                if (m_Tb != null)
                {
                    m_Tb.Focus();
                    m_Tb.SelectionStart = m_Tb.Text.Length;
                    m_Tb.ScrollToEnd();
                    if (m_Popup.Items.Count == 0)
                    {
                        ((Popup)m_Popup.Parent).IsOpen = false;
                    }
                }
            }
            //if (k.Key == Key.Escape)
            //{
            //    ((Popup) m_Popup.Parent).IsOpen = false;
            //    m_Tb.Focus();
            //}
        }

        private void OnTextBoxPressed(object o)
        {
            var k = (System.Windows.Input.KeyEventArgs)o;
            if (k.Key == Key.Down)
            {
                if (m_Popup != null)
                {
                    m_Popup.Focus();
                }
            }
            if (k.Key == Key.Escape)
            {
                ((Popup)m_Popup.Parent).IsOpen = false;
            }

        }

        private void SavePopup(object o)
        {
            m_Popup = (ListView)o;
        }

        private void SaveTb(object o)
        {
            m_Tb = (TextBox)o;
        }

        private void GoToTheFolder(object o)
        {
            var files = new List<FileInfo>();
            foreach (var group in Groups)
            {
                files.AddRange(group.Files);
            }

            var fileFolder = new ProcessStartInfo("explorer.exe", "/select,\"" + SelectedItem.Path + "\"");
            fileFolder.UseShellExecute = false;
            Process.Start(fileFolder);
        }

        private void GetGroupsOfFiles()
        {
            if (String.Compare(StartContent, "Start", StringComparison.OrdinalIgnoreCase) == 0)
            {
                StartContent = "Stop";
                ShowPopUp = false;
                Visible = false;
                m_BackgroundWorker = new BackgroundWorker();
                m_BackgroundWorker.DoWork += GetGroupsOfFiles;
                m_BackgroundWorker.RunWorkerCompleted += BackgroundWorkerRunWorkerCompleted;
                m_BackgroundWorker.WorkerSupportsCancellation = true;
                m_BackgroundWorker.RunWorkerAsync();
                m_Cts = new CancellationTokenSource();
            }
            else
            {
                m_BackgroundWorker.CancelAsync();
                m_Cts.Cancel();
            }
        }

        private void BackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                Visible = true;
                if (!e.Cancelled)
                {
                    Groups = (List<FileGroup>)e.Result;
                }
            }
            finally
            {
                StartContent = "Start";
            }
        }

        private void GetGroupsOfFiles(object sender, DoWorkEventArgs e)
        {
            var token = m_Cts.Token;
            var manager = new FileManager();
            var logger = new Logger("", "log.txt");
            var finder = new Finder();

            var groups = finder.FindGroupOfSameFiles(m_CurrentPath, logger, manager, token);

            e.Result = groups;
        }

        private void SetPathDialog()
        {
            var folderDialog = new FolderBrowserDialog
                               {
                                   Description = @"Select folder to find similar files.",
                                   ShowNewFolderButton = false
                               };

            DialogResult result = folderDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                CurrentPath = folderDialog.SelectedPath;
            }
        }
        private void FindDirectories()
        {
            ShowPopUp = true;
            try
            {
                if (CurrentPath.EndsWith(@"\") && CurrentPath.Length > 1)
                {
                    var di = new DirectoryInfo(CurrentPath);
                    Directories = di.GetDirectories("*.*", SearchOption.TopDirectoryOnly).ToList();
                }
            }
            catch (Exception)
            {
            }
        }
    }


    public class DataGridLenthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType);
        }

        private static object Convert(object value, Type targetType)
        {
            if (targetType == typeof(int))
            {
                var length = (DataGridLength)value;
                return length.Value;
            }
            else
            {
                var length = (int)value;
                return new DataGridLength(length);
            }
        }
    }
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InversBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
