using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using SameFileFinder;
using FileInfo = SameFileFinder.FileInfo;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using ListView = System.Windows.Controls.ListView;

namespace WpfSameFileFinder
{
    public class MainWindowViewModel : ViewModelBase
    {
        private DelegateCommand m_GetGroupsCommand;
        private DelegateCommand m_SetThePath;
        private DelegateCommand m_EnterTheFolder;
        private DelegateCommand m_FindTheDirectories;
        private DelegateCommand m_KeyDown;

        private int m_MaxWidth = 525;
        public int MaxWidth
        {
            get { return m_MaxWidth; }
            set
            {
                if (value != m_MaxWidth)
                {
                    m_MaxWidth = value;
                    HashWidth = m_MaxWidth - LengthWidth - NameWidth - PathWidth;
                    OnPropertyChanged("MaxWidth");
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

        private const int InitialColumnWidth = 60;

        private int m_PathWidth = InitialColumnWidth;
        public int PathWidth
        {
            get { return m_PathWidth; }
            set
            {
                if (value != m_PathWidth)
                {
                    m_PathWidth = value;
                    OnPropertyChanged("PathWidth");
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
                    m_NameWidth = value;
                    OnPropertyChanged("NameWidth");

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
                    m_LengthWidth = value;
                    OnPropertyChanged("LengthWidth");
                }
            }
        }

        private int m_HashWidth = 525 - 3 * InitialColumnWidth;
        public int HashWidth
        {
            get { return m_HashWidth = MaxWidth - LengthWidth - NameWidth - PathWidth; }
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
                    m_GetGroupsCommand = new DelegateCommand(param => GetGroupsOfFiles(), param => ValidPath());
                }
                return m_GetGroupsCommand;
            }
        }

        public ICommand KeyDown
        {
            get
            {
                if (m_KeyDown == null)
                {
                    m_KeyDown = new DelegateCommand(param => KeyPressed(param));
                }
                return m_KeyDown;
            }
        }

        public ICommand FocusAutoComplete
        {
            get { return new DelegateCommand(ExecuteFocus); }
        }

        private void ExecuteFocus(object o)
        {
            var listView = o as ListView;
            if (listView != null)
            {
                listView.Focus();
            }


        }

        private void KeyPressed(object o)
        {
            var k = (System.Windows.Input.KeyEventArgs) o;
            if (k.Key == Key.Down)
            {

            }
        }

        private bool ValidPath()
        {
            return true;
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
            ShowPopUp = false;

            var manager = new FileManager();
            var logger = new Logger("", "log.txt");
            var finder = new Finder();
            var groups = finder.FindGroupOfSameFiles(m_CurrentPath, logger, manager);
            Groups = groups;
        }

        private void SetPathDialog()
        {
            var folderDialog = new FolderBrowserDialog();
            folderDialog.Description = @"Select folder to find similar files.";

            folderDialog.ShowNewFolderButton = false;
            DialogResult result = folderDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                CurrentPath = folderDialog.SelectedPath;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(@"You have not selected or canceled Path selection popup");
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

        private void FindDirectories()
        {
            ShowPopUp = true;
            try
            {
                var di = new DirectoryInfo(CurrentPath);
                Directories = di.GetDirectories("*.*", SearchOption.TopDirectoryOnly).ToList();
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
}
