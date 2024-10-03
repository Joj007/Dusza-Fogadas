using System;
using System.Collections.Generic;
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

namespace Dusza_Fogadas.pages
{
    public partial class FogadasWindow : Window
    {
        public FogadasWindow()
        {
            InitializeComponent();
            DataContext = new GameViewModel();
        }

        private void GamesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // The UI will automatically update due to data binding
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the input from the TextBox
            string input = InputField.Text;

            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Please enter a value.");
                return;
            }

            // You can handle the input here (e.g., save it to the database or process it)
            MessageBox.Show($"Submitted: {input}");
        }
    }
}