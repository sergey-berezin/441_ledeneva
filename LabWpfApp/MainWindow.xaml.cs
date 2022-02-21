using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LabWpfApp
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AppViewModel _vm = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _vm;
        }

        public void btn_Browse_onClick(object sender, RoutedEventArgs e)
        {
            _vm.BrowseHandler();
        }

        public void btn_Run_onClick(object sender, RoutedEventArgs e)
        {
            Task.Run(_vm.RunHandler);
        }

        public void btn_Stop_onClick(object sender, RoutedEventArgs e)
        {
            _vm.StopHandler();
        }

        public void btn_Clear_onClick(object sender, RoutedEventArgs e)
        {
            _vm.ClearHandler();
        }

        public void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            _vm.SelectionChangedHandler(((ListBox)sender).SelectedItem as string);
        }
    }
}