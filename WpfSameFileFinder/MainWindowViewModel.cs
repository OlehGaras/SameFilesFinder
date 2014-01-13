using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using SameFileFinder;

namespace WpfSameFileFinder
{
    public class MainWindowViewModel : ViewModelBase
    {
        private DelegateCommand m_GetGroupsCommand;
        private DelegateCommand m_SetThePath;
        private DelegateCommand m_EnterTheFolder;

        private bool m_Entered;
        public bool Entered {
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
                    OnPropertyChanged("CurrentPath");
                }
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
    }
}
