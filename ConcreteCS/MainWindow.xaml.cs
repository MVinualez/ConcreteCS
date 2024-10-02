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
using LiveCharts;
using LiveCharts.Wpf;

namespace ConcreteCS
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
            }
        }

        // Méthode pour mettre à jour le graphique et afficher le résultat final du cumul
        private void UpdateChart(double charge, int nombrePoulies, double rapportEngrenage)
        {
            // Calcul de la force nécessaire
            var lineSeries = (LineSeries)MyChart.Series[0];
            lineSeries.Values.Clear(); // Efface les anciennes données

            // Ajoute les forces calculées au graphique
            for (int pulleys = 1; pulleys <= nombrePoulies; pulleys++)
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
            for (int pulleys = 1; pulleys <= nombrePoulies; pulleys++)
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
    }
}