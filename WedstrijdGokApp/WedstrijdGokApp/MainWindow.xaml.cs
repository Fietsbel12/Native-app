using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WedstrijdGokApp.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WedstrijdGokApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private List<Bet> Bets = new List<Bet>();
        private decimal CurrentBalance = 50;

        public MainWindow()
        {
            this.InitializeComponent();
            LaadGegevens();
        }

        private void LaadGegevens() 
        { 
            UpdateBalance();
            // Test wedstrijden
            var opgeslagenWedstrijden = new List<Wedstrijd>
            {
                new Wedstrijd { TeamA = "Team 1", TeamB = "Team 2", Datum = "2024-12-01" },
                new Wedstrijd { TeamA = "Team 3", TeamB = "Team 4", Datum = "2024-12-02" }
            };

            WedstrijdListView.ItemsSource = opgeslagenWedstrijden;
        }

        private void UpdateBalance()
        {
            SaldoText.Text = $"Saldo: {CurrentBalance} 4S-Dollars";
        }



        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await SynchroniseerWedstrijden();

        }

        private static readonly HttpClient client = new HttpClient();

        private async Task SynchroniseerWedstrijden()
        {
            string apiUrl = "http://c3-school-voetbal-laravel.test/api/matches";
            try
            {
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode) 
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var wedstrijden = JsonSerializer.Deserialize<List<Wedstrijd>>(json);

                    WedstrijdListView.ItemsSource = wedstrijden;
                }
                else
                {
                    SaldoText.Text = "Fout: Kon data niet ophalen.";
                }
            }
            catch (HttpRequestException ex)
            {
                SaldoText.Text = $"Fout bij verbinden met API: {ex.Message}";
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // Dit haalt de ingevoerde gegevens op
            string team = TeamInput.Text;
            string betAmountText = BetAmoutInput.Text;

            if (decimal.TryParse(betAmountText, out decimal betAmount) && betAmount > 0) 
            { 
                if(betAmount > CurrentBalance)
                {
                    FeedbackMessage.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red);
                    FeedbackMessage.Text = "Je hebt niet voldoende saldo voor deze weddenschap!";
                    return;
                }

                // Voegt een weddenschap toe
                Bet newBet = new Bet
                {
                    Team = team,
                    Amount = betAmount,
                };
                Bets.Add(newBet);

                // voegt een weddenschap toe aan de lijst 
                PlacedBetList.Items.Add($"Team: {team}, Bedrag: {betAmount}");

                // Pas het saldo aan
                CurrentBalance -= betAmount;
                UpdateBalance();

                FeedbackMessage.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Green);
                FeedbackMessage.Text = $"Je hebt {betAmount} ingezet op {team}";
            }
            else
            {
                FeedbackMessage.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red);
                FeedbackMessage.Text = "Voer een geldig bedrag in!";
            }

        }

        public class Wedstrijd 
        { 
            public string TeamA {  get; set; }
            public string TeamB { get; set; }
            public string Datum { get; set; }

            public override string ToString()
            {
                return $"{TeamA} vs {TeamB} op {Datum}";
            }
        }
    }
}
