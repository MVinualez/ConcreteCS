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
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Wpf;

namespace ConcreteCS
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SelectionNone.Visibility = Visibility.Visible;
            
            InitializeChart(); //Cumul
            InitializeChart2(); //Poulie
            InitializeChart3(); //Engrenage

            // Définir les valeurs initiales pour les sliders
            ChargeSlider.Value = 300;  // Valeur par défaut de la charge
            PoulieSlider.Value = 3;     // Valeur par défaut du nombre de poulies
            GearRatioSlider.Value = 1;  // Valeur par défaut du rapport d'engrenage

            // Appeler la méthode pour calculer la force nécessaire
            OnSliderValueChanged(null, null);
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

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}