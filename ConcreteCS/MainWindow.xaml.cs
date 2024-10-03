using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Wpf;

namespace ConcreteCS
{
    public partial class MainWindow : Window
    {
        private LevageSysteme levage;
        public MainWindow()
        {
            InitializeComponent();
            SelectionNone.Visibility = Visibility.Visible;
            levage = new LevageSysteme();
            SliderPoulies.ValueChanged += SliderPoulies_ValueChanged;
            InitializeChart(); //Cumul
            InitializeChart2(); //Poulie
            InitializeChart3(); //Engrenage

            // Définir les valeurs initiales pour les sliders
            ChargeSlider.Value = 300;  // Valeur par défaut de la charge
            PoulieSlider.Value = 3;     // Valeur par défaut du nombre de poulies
            GearRatioSlider.Value = 1;  // Valeur par défaut du rapport d'engrenage

            // Appeler la méthode pour calculer la force nécessaire
            OnSliderValueChanged(null, null);
            this.SizeChanged += MainWindow_SizeChanged;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e) {
            // Recalculer et afficher les poulies à chaque changement de taille de fenêtre
            int nbPoulies = (int)PoulieSlider.Value;
            AfficherPoulies(nbPoulies);
        }
        private void OnCalculsClick(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 0; // Select the Calculs tab
            MainTabControl.Visibility = Visibility.Visible; // Show the TabControl
        }

        private void OnSimulationClick(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 1; // Select the Simulation tab
            MainTabControl.Visibility = Visibility.Visible; // Show the TabControl
        }

        private void OnGraphClick(object sender, RoutedEventArgs e)
        {
            // Assuming you have a Graphs tab as well
            MainTabControl.SelectedIndex = 2; // Adjust the index as necessary
            MainTabControl.Visibility = Visibility.Visible; // Show the TabControl
        }

        private void OnQuitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // Exit the application
        }
        private void listeCalculs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                int selectedIndex = listeCalculs.SelectedIndex;

                if (selectedIndex == 0)
                {
                    SelectionNone.Visibility = Visibility.Collapsed;
                    Demultiplication.Visibility = Visibility.Visible;
                    Bras.Visibility = Visibility.Collapsed;
                }

                else if (selectedIndex == 1)
                {
                    SelectionNone.Visibility = Visibility.Collapsed;
                    Demultiplication.Visibility = Visibility.Collapsed;
                    Bras.Visibility = Visibility.Visible;
                }
            }
        }

        private void OnCalculateDemuClick(object sender, RoutedEventArgs e)
        {
            // Vérification des entrées
            if (double.TryParse(ChargeInput.Text, out double chargeKg) &&
                int.TryParse(PoulieInput.Text, out int nombrePoulies) &&
                double.TryParse(GearRatioInput.Text, out double rapportEngrenage) &&
                int.TryParse(AgeInput.Text, out int age))
            {
                string genre = HommeRadioButton.IsChecked == true ? "Homme" : "Femme";

                if (nombrePoulies > 0 && rapportEngrenage > 0)
                {
                    // Conversion de la charge en Newton (kg -> N)
                    double charge = chargeKg * 9.81;

                    // Calcul de la force nécessaire (sans frottements)
                    double forceNecessaire = (charge / nombrePoulies) / rapportEngrenage;

                    // Affichage du résultat
                    ResultForceCalculs.Text = forceNecessaire.ToString("F2");
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

        private void InitializeChart()
        {
            MyChart.Series = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Force en fonction de la démultiplication",
                        Values = new ChartValues<double> { 0 }  // Valeur initiale
                    }
                };
        }

        private void InitializeChart2()
        {
            MyChart2.Series = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Force en fonction des poulies",
                        Values = new ChartValues<double> { 0 }  // Valeur initiale
                    }
                };
        }

        private void InitializeChart3()
        {
            MyChart3.Series = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Force en fonction des engrenages",
                        Values = new ChartValues<double> { 0 }  // Valeur initiale
                    }
                };
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
                double forceNecessaireKg = forceNecessaire / 9.81;

                // Afficher le résultat
                ResultForce.Text = forceNecessaire.ToString("F2");
                ResultForceKg.Text = forceNecessaireKg.ToString("F2");
                UpdateChart(charge, nombrePoulies, rapportEngrenage);
                UpdateChart2(charge, nombrePoulies);
                UpdateChart3(charge, rapportEngrenage);

                string genre = HommeRadioButton.IsChecked == true ? "Homme" : "Femme";
                int ageOuvrier = 0;

                if (int.TryParse(AgeInput.Text, out int age))
                {
                    if (genre == "Homme")
                    {
                        if (age >= 18 && age <= 45 && forceNecessaireKg > 25)
                        {
                            ShowAlert();
                        }
                        else if (age >= 45 && forceNecessaireKg > 20)
                        {
                            ShowAlert();
                        }
                    }

                    else if (genre == "Femme")
                    {
                        if (age >= 18 && age <= 45 && forceNecessaireKg > 13)
                        {
                            ShowAlert();
                        }
                        else if (age >= 45 && forceNecessaireKg > 10)
                        {
                            ShowAlert();
                        }
                    }
                }

                else 
                {
                    if(ageOuvrier >= 26 && ageOuvrier < 50 && forceNecessaireKg > 80)
                    {
                        MessageBox.Show($"Genre: {genre}, Âge: {age} ans - Tranche d'âge: 26-50 ans.");
                    }
                    
                }

            }
        }

        private Alerte alertWindow;

        private async void ShowAlert()
        {
            if (alertWindow == null || !alertWindow.IsVisible)
            {
                // Créer une instance de la fenêtre d'alerte
                alertWindow = new Alerte();
                alertWindow.Show(); // Affiche l'alerte

                await Task.Delay(3000);

                // Fermer l'alerte
                alertWindow.Close();
                alertWindow = null; // Réinitialiser la variable
            }
        }

        // Méthode pour mettre à jour le graphique et afficher le résultat final du cumul
        private void UpdateChart(double charge, int nombrePoulies, double rapportEngrenage)
        {
            // Calcul de la force nécessaire
            var lineSeries = (LineSeries)MyChart.Series[0];
            lineSeries.Values.Clear(); // Efface les anciennes données

            // Ajoute les forces calculées au graphique
            for (int pulleys = 1; pulleys <= nombrePoulies+1; pulleys++)
            {
                double force = (charge / pulleys) / rapportEngrenage;
                lineSeries.Values.Add(force);
            }

            // Met à jour la dernière force dans le TextBox
            double forceNecessaire = (charge / nombrePoulies) / rapportEngrenage;
            ResultForce.Text = forceNecessaire.ToString("F2") + " N";
        }

        // Méthode pour mettre à jour le graphique et afficher le résultat final des poulies
        private void UpdateChart2(double charge, int nombrePoulies)
        {
            // Calcul de la force nécessaire
            var lineSeries = (LineSeries)MyChart2.Series[0];
            lineSeries.Values.Clear(); // Efface les anciennes données

            // Ajoute les forces calculées au graphique
            for (int pulleys = 1; pulleys <= nombrePoulies+1; pulleys++)
            {
                double force = (charge / pulleys) ;
                lineSeries.Values.Add(force);
            }

            // Met à jour la dernière force dans le TextBox
            double forceNecessaire = (charge / nombrePoulies);
            ResultForce.Text = forceNecessaire.ToString("F2") + " N";
        }

        // Méthode pour mettre à jour le graphique et afficher le résultat final des engrenages
        private void UpdateChart3(double charge, double rapportEngrenage)
        {
            // Calcul de la force nécessaire
            var lineSeries = (LineSeries)MyChart3.Series[0];
            lineSeries.Values.Clear(); // Efface les anciennes données

            // Ajoute les forces calculées au graphique
            for (int engrenage = 1; engrenage <= rapportEngrenage; engrenage++)
            {
                double force = charge / (engrenage * rapportEngrenage);
                lineSeries.Values.Add(force);
            }
            // Met à jour la dernière force dans le TextBox
            double forceNecessaire = (charge / rapportEngrenage);
            ResultForce.Text = forceNecessaire.ToString("F2") + " N";
        }

        // Mise à jour du nombre de poulies et de la force nécessaire
        private void SliderPoulies_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            int nbPoulies = (int)SliderPoulies.Value;  // Récupère la valeur du slider
            LabelNbPoulies.Content = nbPoulies.ToString();  // Met à jour l'affichage du nombre de poulies
            double force = levage.CalculerForce(nbPoulies);  // Calcule la force nécessaire
            LabelForce.Content = $"{force:F2} N";  // Affiche la force calculée

            // Met à jour le visuel des poulies sur le canvas
            AfficherPoulies(nbPoulies);
        }

        // Animation de la charge et de la corde
        private void Simuler_Click(object sender, RoutedEventArgs e) {
            int nbPoulies = (int)SliderPoulies.Value;
            double tempsLevage = nbPoulies * 1.0;  // Ajuste le temps de levage en fonction du nombre de poulies

            // Créer une animation pour déplacer la charge vers le haut
            DoubleAnimation animationChargeImpair = new DoubleAnimation {
                From = 250,
                To = 50,
                Duration = TimeSpan.FromSeconds(tempsLevage)
            };

            // Créer une animation pour déplacer la corde en synchronisation avec la charge
            DoubleAnimation animationCordeImpair = new DoubleAnimation {
                From = 250,
                To = 50,
                Duration = TimeSpan.FromSeconds(tempsLevage)
            };

            // Appliquer les animations
            switch (nbPoulies)
            {
                case 1:
                    Charge1.BeginAnimation(Canvas.TopProperty, animationCharge);
                    CordeLast1.BeginAnimation(Line.Y1Property, animationCorde);
                    break;
                case 2:

                    break;
                case 3:

                    break;
                case 4:

                    break;
                case 5:

                    break;
            }
        }

        private void AfficherPoulies(int nombreDePoulies)
        {
            PulleyScene1.Visibility = Visibility.Hidden;
            PulleyScene2.Visibility = Visibility.Hidden;
            PulleyScene3.Visibility = Visibility.Hidden;
            PulleyScene4.Visibility = Visibility.Hidden;
            PulleyScene5.Visibility = Visibility.Hidden;

            switch (nombreDePoulies)
            {
                case 1:
                    PulleyScene1.Visibility = Visibility.Visible;
                    break;
                case 2:
                    PulleyScene2.Visibility = Visibility.Visible;
                    break;
                case 3:
                    PulleyScene3.Visibility = Visibility.Visible;
                    break;
                case 4:
                    PulleyScene4.Visibility = Visibility.Visible;
                    break;
                case 5:
                    PulleyScene5.Visibility = Visibility.Visible;
                    break;
            }
        }
    }

    public class LevageSysteme {
        private double masse = 300; // Masse de la charge en kg
        private double g = 9.81; // Accélération gravitationnelle en m/s^2

        // Calcul de la force nécessaire en fonction du nombre de poulies
        public double CalculerForce(int nbPoulies) {
            if (nbPoulies <= 0) return 0;
            return (masse * g) / nbPoulies;
        }
    }
}