using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Cecs475.BoardGames.Chess.View {
   public class ChessSquareColorConverter : IMultiValueConverter {

      public static SolidColorBrush PATTERN_ONE = new SolidColorBrush(Colors.SandyBrown);
      public static SolidColorBrush PATTERN_TWO = new SolidColorBrush(Colors.Tan);
      public static SolidColorBrush RED_BRUSH = new SolidColorBrush(Colors.Red);
      public static SolidColorBrush GREEN_BRUSH = new SolidColorBrush(Colors.LightGreen);
      public static SolidColorBrush YELLOW_BRUSH = new SolidColorBrush(Colors.Yellow);

      public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
         BoardPosition pos = (BoardPosition) values[0];
         ChessPiecePosition piece = (ChessPiecePosition) values[1];
         bool isSelected = (bool) values[2];
         bool isHovered = (bool) values[3];
         ChessViewModel viewModel = (ChessViewModel) values[4];

         if (viewModel.SelectedSquare != null && isHovered) {
            return RED_BRUSH;
         }

         if (isSelected) {
            return RED_BRUSH;
         }

         if (isHovered) {
            return GREEN_BRUSH;
         }

         if (viewModel.IsCheck && piece.Player == viewModel.CurrentPlayer && piece.PieceType == ChessPieceType.King) {
            return YELLOW_BRUSH;
         }

         if ((pos.Row + pos.Col) % 2 == 0) {
            return PATTERN_ONE;
         }
         else {
            return PATTERN_TWO;
         }
      }

      public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}
