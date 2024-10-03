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
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;


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
                    Fleche.Visibility = Visibility.Collapsed;
                }else if (selectedIndex == 1)
                {
                    SelectionNone.Visibility = Visibility.Collapsed;
                    Demultiplication.Visibility = Visibility.Collapsed;
                    Bras.Visibility = Visibility.Visible;
                    Fleche.Visibility = Visibility.Collapsed;
                }else if (selectedIndex == 2) {
                    SelectionNone.Visibility = Visibility.Collapsed;
                    Demultiplication.Visibility = Visibility.Collapsed;
                    Bras.Visibility = Visibility.Collapsed;
                    Fleche.Visibility = Visibility.Visible;
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

        private void OnCalculateFlecheClick(object sender, RoutedEventArgs e) {
            // Remplacer les virgules par des points dans les entrées
            string poidsText = PoidsInput.Text.Replace(',', '.');
            string longueurText = LongueurInput.Text.Replace(',', '.');
            string youngText = YoungInput.Text.Replace(',', '.');
            string momentQuadratiqueText = MomentQuadratiqueInput.Text.Replace(',', '.');

            // Vérification des entrées après avoir remplacé les virgules
            if (double.TryParse(poidsText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double poidsKg) &&
                double.TryParse(longueurText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double longueur) &&
                double.TryParse(youngText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double young) &&
                double.TryParse(momentQuadratiqueText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double momentQuadratique)) {

                // Calcul de la flèche
                double fleche = ((poidsKg * 9.81) * Math.Pow(longueur, 3)) / (3 * young * momentQuadratique);

                // Affichage du résultat avec 5 chiffres après la virgule
                ResultFlecheCalculs.Text = fleche.ToString("F5");
            } else {
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

        private void OpenLinkButton_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://www.inrs.fr/risques/activite-physique/ce-qu-il-faut-retenir.html";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        public void ShowButton()
        {
            OpenLinkButton.Visibility = Visibility.Visible; // Rendre le bouton visible
        }

        int compteurAlerte = 0;
        private Alerte alertWindow;

        private async void ShowAlert()
        {
            compteurAlerte ++;

            if (alertWindow == null || !alertWindow.IsVisible)
            {
                alertWindow = new Alerte();
                alertWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                alertWindow.Show(); 
                ShowButton();

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
        
        private void csvDownload_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV file (*.csv)|*.csv";
            saveFileDialog.Title = "Enregistrer les calculs en CSV";
            saveFileDialog.FileName = "force_calculations.csv";

            if (saveFileDialog.ShowDialog() == true)
            {
                // Récupérer la charge, le nombre de poulies et le rapport d'engrenage
                double charge = ChargeSlider.Value * 9.81;
                int nombrePoulies = (int)PoulieSlider.Value;
                double rapportEngrenage = GearRatioSlider.Value;

                // Créer le fichier CSV
                using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName))
                {
                    // Écrire les en-têtes des colonnes
                    sw.WriteLine("Nombre de Poulies;Force (N);Rapport d'Engrenage;Force par Engrenage (N)");

                    // Remplir les données pour chaque valeur de poulie
                    for (int pulleys = 1; pulleys <= nombrePoulies; pulleys++)
                    {
                        // Calculer la force pour chaque poulie
                        double forcePoulie = (charge / pulleys);
                        double forceEngrenage = charge / (pulleys * rapportEngrenage);

                        // Écrire les données dans le fichier CSV sous forme de ligne
                        sw.WriteLine($"{pulleys};{forcePoulie:F2};{rapportEngrenage:F2};{forceEngrenage:F2}");
                    }
                }

                MessageBox.Show("Le fichier CSV a été enregistré avec succès !");
            } 
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

        // Affiche le bon nombre de poulies en fonction du slider
        private void AfficherPoulies(int nbPoulies) {
            // Nettoie le canvas en conservant la corde et la charge
            LevageCanvas.Children.Clear();
            LevageCanvas.Children.Add(Corde);
            LevageCanvas.Children.Add(Charge);

            double poulieY = 50; // Position verticale des poulies
            double espacement = 100; // Espacement entre les poulies

            // Ajoute dynamiquement le nombre de poulies sélectionné
            for (int i = 0; i < nbPoulies; i++) {
                Image poulie = new Image {
                    Width = 60,
                    Height = 60,
                    Source = new BitmapImage(new Uri("poulie.png", UriKind.Relative))
                };

                // Positionne les poulies horizontalement
                double poulieX = 100 + (i * espacement);
                Canvas.SetLeft(poulie, poulieX);
                Canvas.SetTop(poulie, poulieY);
                LevageCanvas.Children.Add(poulie);

                // Dessine des lignes entre les poulies
                if (i > 0) // S'il y a déjà une poulie précédente
                {
                    Line ligne = new Line {
                        Stroke = Brushes.Black,
                        StrokeThickness = 2,
                        X1 = poulieX - espacement + 30, // X de la poulie précédente (ajout d'un décalage)
                        Y1 = poulieY + 30, // Y fixe au bas de la poulie
                        X2 = poulieX + 30, // X de la poulie actuelle (ajout d'un décalage)
                        Y2 = poulieY + 30 // Y fixe au bas de la poulie
                    };

                    LevageCanvas.Children.Add(ligne);
                }
            }

            // Ajouter une ligne pour relier la dernière poulie à la corde
            if (nbPoulies > 0) {
                double lastPulleyX = 100 + ((nbPoulies - 1) * espacement);
                Line lastLine = new Line {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    X1 = lastPulleyX + 30, // Position horizontale de la dernière poulie
                    Y1 = poulieY + 30, // Position verticale de la dernière poulie
                    X2 = lastPulleyX + 30, // Position horizontale de la corde
                    Y2 = 100 // Hauteur à laquelle la corde est attachée (ajuste si nécessaire)
                };
                LevageCanvas.Children.Add(lastLine);

                // Aligner la charge avec la dernière poulie
                Canvas.SetLeft(Charge, lastPulleyX); // Aligne horizontalement avec la dernière poulie
                Canvas.SetTop(Charge, poulieY + 30 + 10); // Position verticale juste en dessous de la dernière poulie (ajoute un petit décalage)

                // Ajouter une ligne pour relier la charge à la corde
                Line chargeToPulleyLine = new Line {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    X1 = lastPulleyX + 30, // Position horizontale de la dernière poulie
                    Y1 = poulieY + 30, // Position verticale de la dernière poulie
                    X2 = lastPulleyX + 30, // Aligne horizontalement avec la charge
                    Y2 = Canvas.GetTop(Charge) // Hauteur de la charge
                };
                LevageCanvas.Children.Add(chargeToPulleyLine);

            }
        }

        // Animation de la charge et de la corde
        private void Simuler_Click(object sender, RoutedEventArgs e) {
            int nbPoulies = (int)SliderPoulies.Value;
            double tempsLevage = nbPoulies * 1.0;  // Ajuste le temps de levage en fonction du nombre de poulies

            // Créer une animation pour déplacer la charge vers le haut
            DoubleAnimation animationCharge = new DoubleAnimation {
                From = 250,
                To = 50,
                Duration = TimeSpan.FromSeconds(tempsLevage)
            };

            // Créer une animation pour déplacer la corde en synchronisation avec la charge
            DoubleAnimation animationCorde = new DoubleAnimation {
                From = 250,
                To = 50,
                Duration = TimeSpan.FromSeconds(tempsLevage)
            };

            // Appliquer les animations
            Charge.BeginAnimation(Canvas.TopProperty, animationCharge);
            Corde.BeginAnimation(Line.Y1Property, animationCorde);
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