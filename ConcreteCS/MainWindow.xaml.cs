using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConcreteCS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void listeCalculs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {

                int selectedIndex = listeCalculs.SelectedIndex;

                if (selectedIndex == 0)
                {
                    bras.Visibility = Visibility.Visible;
                    engrenage.Visibility = Visibility.Collapsed;
                }

                else if (selectedIndex == 1)
                {
                    bras.Visibility = Visibility.Collapsed;
                    engrenage.Visibility = Visibility.Visible;
                }
            }
        }
    }
}