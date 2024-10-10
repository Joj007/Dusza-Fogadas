using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MySql.Data.MySqlClient;

namespace Dusza_Fogadas.pages
{
    public partial class FogadasWindow : Window
    {
        public FogadasWindow()
        {
            InitializeComponent();
            DataContext = new GameViewModel();
            lbBalance.Content = UserSession.Instance.Balance;
        }

        private void GamesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // The UI will automatically update due to data binding
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string betValue = InputField.Text;
            string betAmountText = BetAmountField.Text;

            if (string.IsNullOrWhiteSpace(betValue) || string.IsNullOrWhiteSpace(betAmountText) ||
                !int.TryParse(betAmountText, out int betAmount))
            {
                MessageBox.Show("Töltsd ki az összeg és a tipp mezőt.");
                return;
            }

            if (betAmount <= 0)
            {
                MessageBox.Show("Töltsd ki helyesen a tipp mezőt.");
                return;
            }

            // Get the current user's balance
            string userName = UserSession.Instance.UserName; // Assume user name is set when user logs in
            decimal? userBalance = UserSession.Instance.GetUserBalance(userName);

            if (userBalance == null || userBalance < betAmount)
            {
                MessageBox.Show("Nincs elég kredited.");
                return;
            }

            // Cast DataContext to GameViewModel
            GameViewModel viewModel = DataContext as GameViewModel;

            if (viewModel == null || viewModel.SelectedGame == null || viewModel.SelectedSubject == null || viewModel.SelectedEvent == null)
            {
                MessageBox.Show("Kérlek válassz egy játékot, egy alanyt és egy eseményt.");
                return;
            }

            Game selectedGame = viewModel.SelectedGame;
            Subject selectedSubject = viewModel.SelectedSubject;
            Event selectedEvent = viewModel.SelectedEvent;

            using (MySqlConnection conn = new MySqlConnection(UserSession.Instance.ConnectionString))
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Insert the bet
                        string query = "INSERT INTO bets (user_id, game_id, subject_id, event_id, bet_amount, bet_value) VALUES (@userId, @gameId, @subjectId, @eventId, @betAmount, @betValue)";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@userId", Convert.ToInt32(UserSession.Instance.GetId(userName)));
                            cmd.Parameters.AddWithValue("@gameId", selectedGame.Id);
                            cmd.Parameters.AddWithValue("@subjectId", selectedSubject.Id);
                            cmd.Parameters.AddWithValue("@eventId", selectedEvent.Id);
                            cmd.Parameters.AddWithValue("@betAmount", betAmount);
                            cmd.Parameters.AddWithValue("@betValue", betValue);
                            cmd.ExecuteNonQuery();
                        }

                        // Update user's balance
                        decimal newBalance = userBalance.Value - betAmount;
                        string updateBalanceQuery = "UPDATE users SET balance = @newBalance WHERE name = @userName";
                        using (MySqlCommand cmd = new MySqlCommand(updateBalanceQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@newBalance", newBalance);
                            cmd.Parameters.AddWithValue("@userName", userName);
                            cmd.ExecuteNonQuery();
                        }

                        // Commit the transaction
                        transaction.Commit();
                        lbBalance.Content = UserSession.Instance.Balance;
                        MessageBox.Show("Sikeres fogadás. Új egyenleged: " + newBalance);
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of error
                        transaction.Rollback();
                        MessageBox.Show($"Hiba történt a fogadás tétele közben: {ex.Message}");
                    }
                }
            }
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            return int.TryParse(text, out _);
        }

        private void btnVissza_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
