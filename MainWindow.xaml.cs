﻿using Dusza_Fogadas.pages;
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

namespace Dusza_Fogadas
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
<<<<<<< Updated upstream
=======

        private void btnLetrehozas_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnLeadas_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnLezaras_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnLekerdezes_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnKilepes_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Bejelentkezes_Click(object sender, RoutedEventArgs e)
        {
            Bejelentkezes wasd = new Bejelentkezes();
            bool? result = wasd.ShowDialog();
            if (result == true)
            {
               
            }

        }
>>>>>>> Stashed changes
    }
}