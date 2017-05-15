using Cecs475.BoardGames.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace Cecs475.BoardGames.WpfApplication {
   /// <summary>
   /// Interaction logic for GameChoiceWindow.xaml
   /// </summary>
   public partial class GameChoiceWindow : Window {
      public GameChoiceWindow() {
         InitializeComponent();
         this.Resources.Add("GameTypes", FindGames());
      }

      private IEnumerable<IGameType> FindGames() {
         string currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); // Get the location of our program
         string libFolder = Path.Combine(currentDirectory, "lib"); // We are looking for the "lib" folder
         string[] files = Directory.GetFiles(libFolder, "*.dll"); // Get all .dll files

         foreach (var dll in files) {
            Assembly.LoadFrom(dll);
         }

         Type GameType = typeof(IGameType);

         var gameTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => GameType.IsAssignableFrom(t) && t.IsClass);

         return gameTypes.Select(g => Activator.CreateInstance(g) as IGameType);
      }

      private void Button_Click(object sender, RoutedEventArgs e) {
         Button b = sender as Button;
         IGameType gameType = b.DataContext as IGameType;
         var gameWindow = new MainWindow(gameType,
            mHumanBtn.IsChecked.Value ? NumberOfPlayers.Two : NumberOfPlayers.One) {
            Title = gameType.GameName
         };
         gameWindow.Closed += GameWindow_Closed;

         gameWindow.Show();
         this.Hide();
      }

      private void GameWindow_Closed(object sender, EventArgs e) {
         this.Show();
      }
   }
}
