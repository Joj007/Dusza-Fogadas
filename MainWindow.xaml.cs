using Dusza_Fogadas.pages;
using System.Windows;

namespace Dusza_Fogadas
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLetrehozas_Click(object sender, RoutedEventArgs e)
        {
            UjJatek ujJatek = new UjJatek();
            ujJatek.Show();
        }

        private void btnLeadas_Click(object sender, RoutedEventArgs e)
        {
            FogadasWindow asd = new FogadasWindow();
            asd.Show();
        }

        private void btnLezaras_Click(object sender, RoutedEventArgs e)
        {
            Lezaras uj = new Lezaras();
            uj.Show();
        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            AdminPanel adminPanel = new AdminPanel();
            adminPanel.Show();
        }

        private void btnKilepes_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Bejelentkezes_Click(object sender, RoutedEventArgs e)
        {
            Bejelentkezes wasd = new Bejelentkezes();
            bool? result = wasd.ShowDialog();

            if (result == true)
            {
                // Assuming the Bejelentkezes dialog sets a property or returns the user role
                string userRole = UserSession.Instance.Role; // Example property

                // Enable buttons based on user role
                ConfigureButtonAccess(userRole);
            }
        }

        private void ConfigureButtonAccess(string userRole)
        {
            // Disable all action buttons initially
            btnLetrehozas.IsEnabled = false;
            btnLeadas.IsEnabled = false;
            btnLezaras.IsEnabled = false;
            btnAdmin.IsEnabled = false;

            // Enable buttons based on the user role
            if (userRole == "szervez�")
            {
                btnLetrehozas.IsEnabled = true;
                btnLezaras.IsEnabled = true;
            }
            else if (userRole == "fogad�")
            {
                btnLeadas.IsEnabled = true;
            }
            else if (userRole == "admin")
            {
                btnAdmin.IsEnabled = true;
            }
        }
    }
}