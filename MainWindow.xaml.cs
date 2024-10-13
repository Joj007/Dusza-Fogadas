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
            ujJatek.ShowDialog();
        }

        private void btnLeadas_Click(object sender, RoutedEventArgs e)
        {
            openLeadas();
        }

        private void openLeadas()
        {
            lblEgyenleg.Content = "Egyenleg: " + UserSession.Instance.Balance + "$";
            FogadasWindow fogadas = new FogadasWindow();
            bool? result = fogadas.ShowDialog();
            if (result == true)
            {
                openLeadas();
            }
        }

        private void btnLezaras_Click(object sender, RoutedEventArgs e)
        {
            Lezaras lezaras = new Lezaras();
            lezaras.ShowDialog();
        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            AdminPanel adminPanel = new AdminPanel();
            adminPanel.ShowDialog();
        }

        private void btnKilepes_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Bejelentkezes_Click(object sender, RoutedEventArgs e)
        {

            Bejelentkezes bejelentkezes = new Bejelentkezes();
            bool? result = bejelentkezes.ShowDialog();

            if (result == true)
            {
                // Assuming the Bejelentkezes dialog sets a property or returns the user role
                string userRole = UserSession.Instance.Role; // Example property

                // Enable buttons based on user role
                ConfigureButtonAccess(userRole);
                Bejelentkezes.Click -= Bejelentkezes_Click;
                Bejelentkezes.Click += Kijelentkezes_Click;
                Bejelentkezes.Content = "Kijelentkezés";
                lblNev.Content = UserSession.Instance.UserName;
                grdKartya.Visibility = Visibility.Visible;
                lblEgyenleg.Content = "Egyenleg: " + UserSession.Instance.Balance + "$";
                if (UserSession.Instance.Role=="fogadó")
                {
                    lblNev.FontSize = 24;
                    lblNev.VerticalAlignment = VerticalAlignment.Top;
                    lblEgyenleg.Visibility = Visibility.Visible;
                }
                else
                {
                    lblNev.FontSize = 36;
                    lblNev.VerticalAlignment = VerticalAlignment.Center;
                    lblEgyenleg.Visibility = Visibility.Hidden;
                }
            }
        }

        private void Kijelentkezes_Click(object sender, RoutedEventArgs e)
        {
            Bejelentkezes.Click += Bejelentkezes_Click;
            Bejelentkezes.Click -= Kijelentkezes_Click;
            Bejelentkezes.Content = "Bejelentkezés";
            ConfigureButtonAccess("");
            lblNev.Content = "Vendég";
            lblEgyenleg.Content = "Egyenleg: 0";
            grdKartya.Visibility = Visibility.Hidden;
            MessageBox.Show("A kijelntkezés sikeres!");

        }


        private void ConfigureButtonAccess(string userRole)
        {
            // Disable all action buttons initially
            btnLetrehozas.IsEnabled = false;
            btnLeadas.IsEnabled = false;
            btnLezaras.IsEnabled = false;
            btnAdmin.IsEnabled = false;

            // Enable buttons based on the user role
            if (userRole == "szervezõ")
            {
                btnLetrehozas.IsEnabled = true;
                btnLezaras.IsEnabled = true;
            }
            else if (userRole == "fogadó")
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