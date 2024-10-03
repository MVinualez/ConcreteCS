using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ConcreteCS
{
    /// <summary>
    /// Lógica de interacción para alerte.xaml
    /// </summary>
    public partial class Alerte : Window
    {
        public Alerte()
        {
            InitializeComponent();
        }

        private void CloseAlert(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }

   

}
