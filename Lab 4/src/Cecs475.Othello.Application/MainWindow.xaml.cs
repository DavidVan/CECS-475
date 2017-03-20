using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cecs475.Othello.Application {
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window {
      public MainWindow() {
         InitializeComponent();
      }

      private void UndoButton_Click(object sender, RoutedEventArgs e) {
         var viewModel = OthelloView.FindResource("vm") as OthelloViewModel;
         viewModel.UndoLastMove();
      }
   }

   public class BoardScoreConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         int boardValue = (int) value;
         if (boardValue < 0) {
            return "white is winning by " + Math.Abs(boardValue).ToString();
         }
         else if (boardValue > 0) {
            return "black is winning by " + boardValue.ToString();
         }
         else {
            return "tie game";
         }
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}