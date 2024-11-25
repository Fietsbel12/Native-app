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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WedstrijdGokApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private int saldo = 50;

        public MainWindow()
        {
            this.InitializeComponent();
            LaadGegevens();
        }

        private void LaadGegevens() 
        { 
            SaldoText.Text = $"Saldo: {saldo} 4S-Dollars";

            // Test wedstrijden
            var opgeslagenWedstrijden = new List<Wedstrijd>
            {
                new Wedstrijd { TeamA = "Team 1", TeamB = "Team 2", Datum = "2024-12-01" },
                new Wedstrijd { TeamA = "Team 3", TeamB = "Team 4", Datum = "2024-12-02" }
            };

            WedstrijdListView.ItemsSource = opgeslagenWedstrijden;
        }



        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await SynchroniseerWedstrijden();

        }

        private async Task SynchroniseerWedstrijden() 
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = "https://pokeapi.co/api/v2/pokemon/gengar";

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
                catch 
                {
                    SaldoText.Text = "Fout bij verbinden met API";
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (saldo < 10) 
            {
                SaldoText.Text = "Onvoldoende saldo!";
                return;
            }
            if (string.IsNullOrWhiteSpace(TeamInput.Text)) 
            {
                SaldoText.Text = "Voer een geldig team in.";
                return;
            }
            
            saldo -= 10;
            SaldoText.Text = $"Saldo: {saldo} 4S-dollars";
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
