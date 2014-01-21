using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
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

        private const int InitialColumnWidth = 100;

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
                    m_HashWidth = m_MaxWidth-30 - m_PathWidth - m_NameWidth - m_LengthWidth;
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
                if (value != m_PathWidth && m_NameWidth >= InitialColumnWidth)
                {
                    m_NameWidth += m_PathWidth - value;
                    m_PathWidth = value;
                    OnPropertyChanged("PathWidth");
                    OnPropertyChanged("NameWidth");
                }
                else
                {
                    if (value < m_PathWidth)
                    {
                        m_NameWidth += m_PathWidth - value;
                        m_PathWidth = value;
                        OnPropertyChanged("PathWidth");
                        OnPropertyChanged("NameWidth");
                    }
                }
            }
        }

        private int m_NameWidth = InitialColumnWidth;
        public int NameWidth
        {
            get { return m_NameWidth; }
            set
            {
                if (value != m_NameWidth && m_LengthWidth >= InitialColumnWidth)
                {
                    m_LengthWidth += m_NameWidth - value;
                    m_NameWidth = value;
                    OnPropertyChanged("NameWidth");
                    OnPropertyChanged("LengthWidth");
                }
                else
                {
                    if (value < m_NameWidth)
                    {
                        m_LengthWidth += m_NameWidth - value;
                        m_NameWidth = value;
                        OnPropertyChanged("NameWidth");
                        OnPropertyChanged("LengthWidth");
                    }
                }
            }
        }

        private int m_LengthWidth = InitialColumnWidth;
        public int LengthWidth
        {
            get { return m_LengthWidth; }
            set
            {
                if (value != m_LengthWidth && m_HashWidth >= InitialColumnWidth)
                {
                    m_HashWidth += m_LengthWidth - value;
                    m_LengthWidth = value;                 
                    OnPropertyChanged("LengthWidth");
                    OnPropertyChanged("HashWidth");
                }
                else
                {
                    if (value < m_LengthWidth)
                    {
                        m_HashWidth += m_LengthWidth - value;
                        m_LengthWidth = value;
                        OnPropertyChanged("LengthWidth");
                        OnPropertyChanged("HashWidth");
                    }
                }
            }
        }

        private int m_HashWidth = m_MaxWidth - 3*InitialColumnWidth-30;
        public int HashWidth
        {
            get { return m_HashWidth ; }
            set
            {
                if (value != m_HashWidth)
                {

                    m_HashWidth = value;
                    OnPropertyChanged("HashWidth");

                }
                //if (value != m_HashWidth && m_HashWidth >= InitialColumnWidth)
                //{
                //    m_LengthWidth += m_HashWidth - value;
                //    m_HashWidth = value;
                //    OnPropertyChanged("HashWidth");
                //    OnPropertyChanged("LengthWidth");
                //}
                //else
                //{
                //    if (value < m_HashWidth)
                //    {
                //        m_LengthWidth += m_HashWidth - value;
                //        m_HashWidth = value;
                //        OnPropertyChanged("HashWidth");
                //        OnPropertyChanged("LengthWidth");
                //    }
                //}
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
                CurrentPath += m_Popup.SelectedItem + @"\";
                if (m_Tb != null)
                {
                    m_Tb.Focus();
                    m_Tb.SelectionStart = m_Tb.Text.Length;
                    if (m_Popup.Items.Count == 0)
                    {
                        ((Popup) m_Popup.Parent).IsOpen = false;
                    }
                }
            }
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

        }

        private void SavePopup(object o)
        {
            m_Popup = (ListView)((Popup)((StackPanel)o).Children[1]).Child;
        }

        private void SaveTb(object o)
        {
            m_Tb = (TextBox)((StackPanel)o).Children[0];
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
