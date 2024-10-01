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

            // Définir les valeurs initiales pour les sliders
            ChargeSlider.Value = 300;  // Valeur par défaut de la charge
            PoulieSlider.Value = 3;     // Valeur par défaut du nombre de poulies
            GearRatioSlider.Value = 1;  // Valeur par défaut du rapport d'engrenage

            // Appeler la méthode pour calculer la force nécessaire
            OnSliderValueChanged(null, null);
        }

        private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double>? e)
        {
            // Vérifie que les sliders sont correctement initialisés
            if (ChargeSlider != null && PoulieSlider != null && GearRatioSlider != null && ResultForce != null)
            {
                // Récupérer les valeurs des sliders
                double charge = ChargeSlider.Value * 9.81; // Convertir la charge en Newton (kg -> N)
                int nombrePoulies = (int)PoulieSlider.Value;
                double rapportEngrenage = GearRatioSlider.Value;

                // Calcul de la force nécessaire (sans frottements)
                double forceNecessaire = charge / (nombrePoulies * rapportEngrenage);

                // Afficher le résultat
                ResultForce.Text = forceNecessaire.ToString("F2");
            }
        }
    }
}