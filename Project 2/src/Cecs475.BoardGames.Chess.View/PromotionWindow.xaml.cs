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

namespace Cecs475.BoardGames.Chess.View {
   /// <summary>
   /// Interaction logic for PromotionWindow.xaml
   /// </summary>
   public partial class PromotionWindow : Window {

      private SolidColorBrush HIGHLIGHT = new SolidColorBrush(Colors.DarkSeaGreen);

      public PromotionWindow(ChessView view) {
         InitializeComponent();
         this.Resources.Add("vm", view.Model);
      }

      private void Border_MouseUp(object sender, MouseButtonEventArgs e) {
         Border b = sender as Border;
         var tuple = b.DataContext as Tuple<ChessMove, int>;
         var vm = FindResource("vm") as ChessViewModel;
         vm.ApplyMove(tuple.Item1);
         this.DialogResult = true;
         this.Close();
      }

      private void Border_MouseEnter(object sender, MouseEventArgs e) {
         Border b = sender as Border;
         b.Background = HIGHLIGHT;
      }

      private void Border_MouseLeave(object sender, MouseEventArgs e) {
         Border b = sender as Border;
         b.Background = null;
      }
   }
}
