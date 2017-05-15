using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace Cecs475.BoardGames.WpfApplication {

   public class File {
      public string FileName { get; set; }
      public string Url { get; set; }
      public string PublicKey { get; set; }
      public string Version { get; set; }
   }

   public class Game {
      public string Name { get; set; }
      public IList<File> Files { get; set; }
   }

   /// <summary>
   /// Interaction logic for LoadGames.xaml
   /// </summary>
   public partial class LoadGames : Window {
      public LoadGames() {
         InitializeComponent();
      }

      private async void Window_Loaded(object sender, RoutedEventArgs e) {

         var restClient = new RestClient("http://cecs475-boardgames.azurewebsites.net");
         var request = new RestRequest("api/games", Method.GET);
         var task = restClient.ExecuteTaskAsync(request);
         var response = await task;
         if (response.StatusCode != System.Net.HttpStatusCode.OK) {
            MessageBox.Show("There was an error connecting to the games list server... Playing using existing games...");
            GameChoiceWindow window = new GameChoiceWindow();
            this.Hide();
            window.Show();
         }
         else {
            Game[] games = JsonConvert.DeserializeObject<Game[]>(response.Content);
            foreach (var game in games) {
               WebClient client = new WebClient();
               await client.DownloadFileTaskAsync(game.Files[0].Url, "lib\\" + game.Files[0].FileName);
               await client.DownloadFileTaskAsync(game.Files[1].Url, "lib\\" + game.Files[1].FileName);
               
            }
            GameChoiceWindow window = new GameChoiceWindow();
            this.Hide();
            window.Show();
         }
      }
   }
}
