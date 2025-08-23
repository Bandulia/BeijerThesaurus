using System.Windows;
using Thesaurus.wpf.ViewModels;

namespace Thesaurus.wpf.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
