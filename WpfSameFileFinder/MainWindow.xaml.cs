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
        }
    }
}
