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
using System.Reflection.Metadata;


namespace ConcreteCS
{
    public partial class MainWindow : Window
    {
        private LevageSysteme levage;
        private LogWriter logWriter;
        public MainWindow()
        {
            logWriter = new LogWriter("Démarrage de l'application ConcreteCS");
            InitializeComponent();
            SelectionNone.Visibility = Visibility.Visible;
            levage = new LevageSysteme();

            SliderPoulies.ValueChanged += SliderPoulies_ValueChanged;
            InitializeChart(); //Cumul
            InitializeChart2(); //Poulie
            InitializeChart3(); //Engrenage
            AfficherPoulies(1); // Graphiques poulies

            // Définir les valeurs initiales pour les sliders
            ChargeSlider.Value = 300;
            PoulieSlider.Value = 3;
            GearRatioSlider.Value = 1;

            // Appeler la méthode pour calculer la force nécessaire
            OnSliderValueChanged(null, null);
            this.SizeChanged += MainWindow_SizeChanged;
            logWriter.LogWrite("Valeurs initialisées");
            AfficherPoulies(1);
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e) {
            logWriter.LogWrite("Dimension de la fenête modifiée");
        }

        private void OnCalculsClick(object sender, RoutedEventArgs e)
        {
            logWriter.LogWrite("Ouverture de l'onglet calculs");
            MainTabControl.SelectedIndex = 0;
            MainTabControl.Visibility = Visibility.Visible; //Changer d'onglet sur Calcul
        }

        private void OnGraphClick(object sender, RoutedEventArgs e)
        {
            logWriter.LogWrite("Ouverture de l'onglet graphiques");
            MainTabControl.SelectedIndex = 1;
            MainTabControl.Visibility = Visibility.Visible; //Changer d'onglet sur graphique
        }

        private void OnSimulationClick(object sender, RoutedEventArgs e)
        {
            logWriter.LogWrite("Ouverture de l'onglet simulation");
            MainTabControl.SelectedIndex = 2;
            MainTabControl.Visibility = Visibility.Visible; //Changer d'onglet sur simulation
        } 

        private void OnQuitClick(object sender, RoutedEventArgs e)
        {
            logWriter.LogWrite("Fermeture de l'application ConcreteCS");
            Application.Current.Shutdown();  //Quitter
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
                    Fleche.Visibility = Visibility.Collapsed;
                    Bras.Visibility = Visibility.Collapsed;
                }else if (selectedIndex == 1)
                {
                    SelectionNone.Visibility = Visibility.Collapsed;
                    Demultiplication.Visibility = Visibility.Collapsed;
                    Fleche.Visibility = Visibility.Visible;
                    Bras.Visibility = Visibility.Collapsed;
                }else if (selectedIndex == 2) {
                    SelectionNone.Visibility = Visibility.Collapsed;
                    Demultiplication.Visibility = Visibility.Collapsed;
                    Fleche.Visibility = Visibility.Collapsed;
                    Bras.Visibility = Visibility.Visible;
                }
            }
        }



        private void OnCalculateDemuClick(object sender, RoutedEventArgs e)
        {
            logWriter.LogWrite("Lancement du traitement des calculs de démultiplication");

            // Vérification des entrées
            if (double.TryParse(ChargeInput.Text, out double chargeKg) &&
                int.TryParse(PoulieInput.Text, out int nombrePoulies) &&
                double.TryParse(GearRatioInput.Text, out double rapportEngrenage))
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
                    logWriter.LogWrite("ERREUR : Le nombre de poulies et le rapport d'engrenage doivent être supérieurs à 0");
                    MessageBox.Show("Le nombre de poulies et le rapport d'engrenage doivent être supérieurs à 0", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                logWriter.LogWrite("ERREUR : Veuillez entrer des valeurs numériques valides");
                MessageBox.Show("Veuillez entrer des valeurs numériques valides", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnCalculateFlecheClick(object sender, RoutedEventArgs e) {
            logWriter.LogWrite("Lancement du traitement des calculs de la flèche");

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
                logWriter.LogWrite("ERREUR : Veuillez entrer des valeurs numériques valides");
                MessageBox.Show("Veuillez entrer des valeurs numériques valides", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void InitializeChart()
        {
            logWriter.LogWrite("Initialisation du graphique Force en fonction de la démultiplication");
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
            logWriter.LogWrite("Initialisation du graphique Force en fonction des poulies");
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
            logWriter.LogWrite("Initialisation du graphique Force en fonction des engrenages");
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
            logWriter.LogWrite("Valeur(s) des sélecteurs de la simulation modifiée(s)");
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
            logWriter.LogWrite("Ouverture du lien externe vers inrs vers les règlementations des charges au travail");
            string url = "https://www.inrs.fr/risques/activite-physique/ce-qu-il-faut-retenir.html";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        public void ShowButton()
        {
            logWriter.LogWrite("Affichage du bouton En savoir plus");
            OpenLinkButton.Visibility = Visibility.Visible; // Rendre le bouton visible
        }

        int compteurAlerte = 0;
        private Alerte alertWindow;

        private async void ShowAlert()
        {
            logWriter.LogWrite("Afficher l'alerte de dépassement du poids reglementaire");
            compteurAlerte ++;

            if (alertWindow == null || !alertWindow.IsVisible)
            {
                alertWindow = new Alerte();
                alertWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
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
            logWriter.LogWrite("Démarrage de l'export du fichier csv");
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
            ResetAnimation();
            AfficherPoulies(nbPoulies);
        }

        // Fonction de réinitialisation des animations
        private void ResetAnimation()
        {
            // Position des poids
            Charge1.BeginAnimation(FrameworkElement.MarginProperty, null);
            Charge2.BeginAnimation(FrameworkElement.MarginProperty, null);
            Charge3.BeginAnimation(FrameworkElement.MarginProperty, null);
            Charge4.BeginAnimation(FrameworkElement.MarginProperty, null);
            Charge5.BeginAnimation(FrameworkElement.MarginProperty, null);

            Charge1.Margin = new Thickness(Charge1.Margin.Left, Charge1.Margin.Top, Charge1.Margin.Right, 0);
            Charge2.Margin = new Thickness(Charge2.Margin.Left, Charge2.Margin.Top, Charge2.Margin.Right, 0);
            Charge3.Margin = new Thickness(Charge3.Margin.Left, Charge3.Margin.Top, Charge3.Margin.Right, 0);
            Charge4.Margin = new Thickness(Charge4.Margin.Left, Charge4.Margin.Top, Charge4.Margin.Right, 0);
            Charge5.Margin = new Thickness(Charge5.Margin.Left, Charge5.Margin.Top, Charge5.Margin.Right, 0);

            // Position des poulies
            Poulie22.BeginAnimation(FrameworkElement.MarginProperty, null);
            Poulie32.BeginAnimation(FrameworkElement.MarginProperty, null);
            Poulie42.BeginAnimation(FrameworkElement.MarginProperty, null);
            Poulie44.BeginAnimation(FrameworkElement.MarginProperty, null);
            Poulie52.BeginAnimation(FrameworkElement.MarginProperty, null);
            Poulie54.BeginAnimation(FrameworkElement.MarginProperty, null);

            Poulie22.Margin = new Thickness(Poulie22.Margin.Left, Poulie22.Margin.Top, Poulie22.Margin.Right, 47);
            Poulie32.Margin = new Thickness(Poulie32.Margin.Left, Poulie32.Margin.Top, Poulie32.Margin.Right, 47);
            Poulie42.Margin = new Thickness(Poulie42.Margin.Left, Poulie42.Margin.Top, Poulie42.Margin.Right, 47);
            Poulie44.Margin = new Thickness(Poulie44.Margin.Left, Poulie44.Margin.Top, Poulie44.Margin.Right, 47);
            Poulie52.Margin = new Thickness(Poulie52.Margin.Left, Poulie52.Margin.Top, Poulie52.Margin.Right, 47);
            Poulie54.Margin = new Thickness(Poulie54.Margin.Left, Poulie54.Margin.Top, Poulie54.Margin.Right, 47);

            // Position des cordes
            CordeLast1.BeginAnimation(Line.Y1Property, null);
            CordeLast21.BeginAnimation(Line.Y1Property, null);
            CordeLast22.BeginAnimation(Line.Y1Property, null);
            CordeLast31.BeginAnimation(Line.Y1Property, null);
            CordeLast32.BeginAnimation(Line.Y1Property, null);
            CordeLast33.BeginAnimation(Line.Y1Property, null);
            CordeLast41.BeginAnimation(Line.Y1Property, null);
            CordeLast42.BeginAnimation(Line.Y1Property, null);
            CordeLast43.BeginAnimation(Line.Y1Property, null);
            CordeLast44.BeginAnimation(Line.Y1Property, null);
            CordeLast51.BeginAnimation(Line.Y1Property, null);
            CordeLast52.BeginAnimation(Line.Y1Property, null);
            CordeLast53.BeginAnimation(Line.Y1Property, null);
            CordeLast54.BeginAnimation(Line.Y1Property, null);
            CordeLast55.BeginAnimation(Line.Y1Property, null);

            CordeLast1.Y1 = 215;
            CordeLast21.Y1 = 180;
            CordeLast22.Y1 = 180;
            CordeLast31.Y1 = 180;
            CordeLast32.Y1 = 180;
            CordeLast33.Y1 = 216;
            CordeLast41.Y1 = 180;
            CordeLast42.Y1 = 180;
            CordeLast43.Y1 = 180;
            CordeLast44.Y1 = 180;
            CordeLast51.Y1 = 180;
            CordeLast52.Y1 = 180;
            CordeLast53.Y1 = 180;
            CordeLast54.Y1 = 180;
            CordeLast55.Y1 = 216;

            // Position des barres horizontales
            HorizontalBar3.BeginAnimation(FrameworkElement.MarginProperty, null);
            HorizontalBar4.BeginAnimation(FrameworkElement.MarginProperty, null);
            HorizontalBar5.BeginAnimation(FrameworkElement.MarginProperty, null);

            HorizontalBar3.Margin = new Thickness(HorizontalBar3.Margin.Left, HorizontalBar3.Margin.Top, HorizontalBar3.Margin.Right, 0);
            HorizontalBar4.Margin = new Thickness(HorizontalBar4.Margin.Left, HorizontalBar4.Margin.Top, HorizontalBar4.Margin.Right, 0);
            HorizontalBar5.Margin = new Thickness(HorizontalBar5.Margin.Left, HorizontalBar5.Margin.Top, HorizontalBar5.Margin.Right, 0);
        }

        // Animation de la charge et de la corde
        private void Simuler_Click(object sender, RoutedEventArgs e) {
            int nbPoulies = (int)SliderPoulies.Value;
            double tempsLevage = nbPoulies * 1.0;  // Ajuste le temps de levage en fonction du nombre de poulies
            Thickness currentMargin;

            ResetAnimation();

            // Créer une animation pour déplacer la charge vers le haut
            ThicknessAnimation animationLevage = new ThicknessAnimation
            {
                Duration = TimeSpan.FromSeconds(tempsLevage)
            };

            // Créer une animation pour déplacer la corde en synchronisation avec la charge
            DoubleAnimation animationCordeImpair = new DoubleAnimation {
                From = 215,
                To = 55,
                Duration = TimeSpan.FromSeconds(tempsLevage)
            };

            DoubleAnimation animationCordePair = new DoubleAnimation
            {
                From = 180,
                To = 65,
                Duration = TimeSpan.FromSeconds(tempsLevage)
            };

            DoubleAnimation animationCordeSpecialEnd = new DoubleAnimation
            {
                From = 216,
                To = 100,
                Duration = TimeSpan.FromSeconds(tempsLevage)
            };

            // Appliquer les animations
            switch (nbPoulies)
            {
                case 1:
                    // Récupération de la marge du poids
                    currentMargin = Charge1.Margin;
                    animationLevage.From = currentMargin;
                    // Création d'une nouvelle marge
                    animationLevage.To = new Thickness(currentMargin.Left, 0, currentMargin.Right, 105);
                    Charge1.BeginAnimation(FrameworkElement.MarginProperty, animationLevage);
                    CordeLast1.BeginAnimation(Line.Y1Property, animationCordeImpair);
                    break;
                case 2:
                    currentMargin = Charge2.Margin;
                    animationLevage.From = currentMargin;
                    animationLevage.To = new Thickness(currentMargin.Left, 0, currentMargin.Right, 20);
                    Charge2.BeginAnimation(FrameworkElement.MarginProperty, animationLevage);
                    currentMargin = Poulie22.Margin;
                    animationLevage.From = currentMargin;
                    animationLevage.To = new Thickness(currentMargin.Left, 0, currentMargin.Right, 120);
                    Poulie22.BeginAnimation(FrameworkElement.MarginProperty, animationLevage);
                    CordeLast21.BeginAnimation(Line.Y1Property, animationCordePair);
                    CordeLast22.BeginAnimation(Line.Y1Property, animationCordePair);
                    break;
                case 3:
                    currentMargin = Charge3.Margin;
                    animationLevage.From = currentMargin;
                    animationLevage.To = new Thickness(currentMargin.Left, 0, currentMargin.Right, 10);
                    Charge3.BeginAnimation(FrameworkElement.MarginProperty, animationLevage);
                    currentMargin = Poulie32.Margin;
                    animationLevage.From = currentMargin;
                    animationLevage.To = new Thickness(currentMargin.Left, 0, currentMargin.Right, 120);
                    Poulie32.BeginAnimation(FrameworkElement.MarginProperty, animationLevage);
                    currentMargin = HorizontalBar3.Margin;
                    animationLevage.From = currentMargin;
                    animationLevage.To = new Thickness(currentMargin.Left, 100, currentMargin.Right, 0);
                    HorizontalBar3.BeginAnimation(FrameworkElement.MarginProperty, animationLevage);
                    CordeLast31.BeginAnimation(Line.Y1Property, animationCordePair);
                    CordeLast32.BeginAnimation(Line.Y1Property, animationCordePair);
                    CordeLast33.BeginAnimation(Line.Y1Property, animationCordeSpecialEnd);
                    break;
                case 4:
                    currentMargin = Charge4.Margin;
                    animationLevage.From = currentMargin;
                    animationLevage.To = new Thickness(currentMargin.Left, 0, currentMargin.Right, 10);
                    Charge4.BeginAnimation(FrameworkElement.MarginProperty, animationLevage);
                    currentMargin = Poulie42.Margin;
                    animationLevage.From = currentMargin;
                    animationLevage.To = new Thickness(currentMargin.Left, 0, currentMargin.Right, 120);
                    Poulie42.BeginAnimation(FrameworkElement.MarginProperty, animationLevage);
                    currentMargin = Poulie44.Margin;
                    animationLevage.From = currentMargin;
                    animationLevage.To = new Thickness(currentMargin.Left, 0, currentMargin.Right, 120);
                    Poulie44.BeginAnimation(FrameworkElement.MarginProperty, animationLevage);
                    currentMargin = HorizontalBar4.Margin;
                    animationLevage.From = currentMargin;
                    animationLevage.To = new Thickness(currentMargin.Left, 100, currentMargin.Right, 0);
                    HorizontalBar4.BeginAnimation(FrameworkElement.MarginProperty, animationLevage);
                    CordeLast41.BeginAnimation(Line.Y1Property, animationCordePair);
                    CordeLast42.BeginAnimation(Line.Y1Property, animationCordePair);
                    CordeLast43.BeginAnimation(Line.Y1Property, animationCordePair);
                    CordeLast44.BeginAnimation(Line.Y1Property, animationCordePair);
                    break;
                case 5:
                    currentMargin = Charge5.Margin;
                    animationLevage.From = currentMargin;
                    animationLevage.To = new Thickness(currentMargin.Left, 0, currentMargin.Right, 10);
                    Charge5.BeginAnimation(FrameworkElement.MarginProperty, animationLevage);
                    currentMargin = Poulie52.Margin;
                    animationLevage.From = currentMargin;
                    animationLevage.To = new Thickness(currentMargin.Left, 0, currentMargin.Right, 120);
                    Poulie52.BeginAnimation(FrameworkElement.MarginProperty, animationLevage);
                    currentMargin = Poulie54.Margin;
                    animationLevage.From = currentMargin;
                    animationLevage.To = new Thickness(currentMargin.Left, 0, currentMargin.Right, 120);
                    Poulie54.BeginAnimation(FrameworkElement.MarginProperty, animationLevage);
                    currentMargin = HorizontalBar5.Margin;
                    animationLevage.From = currentMargin;
                    animationLevage.To = new Thickness(currentMargin.Left, 100, currentMargin.Right, 0);
                    HorizontalBar5.BeginAnimation(FrameworkElement.MarginProperty, animationLevage);
                    CordeLast51.BeginAnimation(Line.Y1Property, animationCordePair);
                    CordeLast52.BeginAnimation(Line.Y1Property, animationCordePair);
                    CordeLast53.BeginAnimation(Line.Y1Property, animationCordePair);
                    CordeLast54.BeginAnimation(Line.Y1Property, animationCordePair);
                    CordeLast55.BeginAnimation(Line.Y1Property, animationCordeSpecialEnd);
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
        
        private void OnOpenLogClick(object sender, RoutedEventArgs e) {
            // Obtenir le chemin de l'exécutable
            string exePath = System.AppDomain.CurrentDomain.BaseDirectory;

            // Construire le chemin complet du fichier log.txt
            string logFilePath = System.IO.Path.Combine(exePath, "log.txt");

            // Vérifier si le fichier existe
            if (File.Exists(logFilePath)) {
                // Ouvrir le fichier avec Notepad
                Process.Start(new ProcessStartInfo("notepad.exe", logFilePath) {
                    UseShellExecute = true
                });
            } else {
                // Afficher un message d'erreur si le fichier n'existe pas
                MessageBox.Show("Le fichier log.txt n'existe pas.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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