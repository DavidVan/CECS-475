using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Cecs475.BoardGames.Chess.View {
   /// <summary>
   /// Interaction logic for ChessView.xaml
   /// </summary>
   public partial class ChessView : UserControl {
      public ChessView() {
         InitializeComponent();
      }

      private void Border_MouseEnter(object sender, MouseEventArgs e) {
         Border b = sender as Border;
         var square = b.DataContext as ChessSquare;
         var vm = FindResource("vm") as ChessViewModel;
         if (vm.SelectedSquare != null) {
            var possibleMovesFromSquare = vm.PossibleMoves.Where(m => m.StartPosition.Equals(vm.SelectedSquare.Position));
            if (possibleMovesFromSquare.Select(m => m.EndPosition).Contains(square.Position)) {
               square.IsHovered = true;
            }
         }
         else {
            if (vm.PossibleMoves.Select(m => m.StartPosition).Contains(square.Position)) {
               square.IsHovered = true;
            }
         }
      }

      private void Border_MouseLeave(object sender, MouseEventArgs e) {
         Border b = sender as Border;
         var square = b.DataContext as ChessSquare;
         square.IsHovered = false;
      }

      private void Border_MouseUp(object sender, MouseButtonEventArgs e) {
         Border b = sender as Border;
         var square = b.DataContext as ChessSquare;
         var vm = FindResource("vm") as ChessViewModel;

         if (vm.SelectedSquare != null) {
            if (vm.PossibleMoves.Select(m => m.StartPosition).Contains(vm.SelectedSquare.Position) && vm.PossibleMoves.Select(m => m.EndPosition).Contains(square.Position)) {
               ChessMove move = new ChessMove(vm.SelectedSquare.Position, square.Position);
               square.IsHovered = false;
               vm.SelectedSquare.IsSelected = false;
               vm.ApplyMove(move);
               vm.SelectedSquare = null;
               if (vm.PossibleMoves.Select(m => m.MoveType).Contains(ChessMoveType.PawnPromote)) {
                  PromotionWindow window = new PromotionWindow(this);
                  bool? result = window.ShowDialog();
                  if (!result.GetValueOrDefault()) { // If result is false, execute this if statement.
                     vm.ShowPromotionWindow = true; // User exited the pawn promotion window... We need a way to show it again!
                  }
               }
            }
            else if (vm.ShowPromotionWindow) {
               vm.SelectedSquare.IsSelected = false;
               vm.SelectedSquare = null;
               PromotionWindow window = new PromotionWindow(this);
               bool? result = window.ShowDialog();
               if (!result.GetValueOrDefault()) { // If result is false, execute this if statement.
                  vm.ShowPromotionWindow = true; // User exited the pawn promotion window... We need a way to show it again!
               }
               else {
                  vm.ShowPromotionWindow = false;
               }
            }
            else {
               vm.SelectedSquare.IsSelected = false;
               vm.SelectedSquare = null;
            }
         }
         else {
            if (square.Player == vm.CurrentPlayer) {
               vm.SelectedSquare = square;
               square.IsSelected = true;
            }
         }

      }

      public ChessViewModel Model {
         get { return FindResource("vm") as ChessViewModel; }
      }
   }
}
