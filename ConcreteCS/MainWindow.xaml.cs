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

        private void OnSimulateClick(object sender, RoutedEventArgs e)
        {
            // Vérification des entrées
            if (double.TryParse(ChargeInput.Text, out double chargeKg) &&
                int.TryParse(PoulieInput.Text, out int nombrePoulies) &&
                double.TryParse(GearRatioInput.Text, out double rapportEngrenage))
            {
                if (nombrePoulies > 0 && rapportEngrenage > 0)
                {
                    // Conversion de la charge en Newton (kg -> N)
                    double charge = chargeKg * 9.81;

                    // Calcul de la force nécessaire (sans frottements)
                    double forceNecessaire = (charge / nombrePoulies) / rapportEngrenage;

                    // Affichage du résultat
                    ResultForce.Text = forceNecessaire.ToString("F2");
                }
                else
                {
                    MessageBox.Show("Le nombre de poulies et le rapport d'engrenage doivent être supérieurs à 0", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Veuillez entrer des valeurs numériques valides", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}