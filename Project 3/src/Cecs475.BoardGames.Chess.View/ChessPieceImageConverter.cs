﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Cecs475.BoardGames.Chess.View {
   class ChessPieceImageConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         try {
            ChessPiecePosition piece = (ChessPiecePosition) value;

            if (piece.PieceType == ChessPieceType.Empty) {
               return null;
            }

            string player;
            string pieceType;

            if (piece.Player == 1) {
               player = "white";
            }
            else {
               player = "black";
            }
            
            switch (piece.PieceType) {
               case ChessPieceType.Pawn:
                  pieceType = "pawn";
                  break;
               case ChessPieceType.RookKing:
                  pieceType = "rook";
                  break;
               case ChessPieceType.RookQueen:
                  pieceType = "rook";
                  break;
               case ChessPieceType.RookPawn:
                  pieceType = "rook";
                  break;
               case ChessPieceType.Knight:
                  pieceType = "knight";
                  break;
               case ChessPieceType.Bishop:
                  pieceType = "bishop";
                  break;
               case ChessPieceType.Queen:
                  pieceType = "queen";
                  break;
               case ChessPieceType.King:
                  pieceType = "king";
                  break;
               default:
                  pieceType = "";
                  break;
            }
            
            string src = player + "_" + pieceType;

            return new BitmapImage(new Uri("/Cecs475.BoardGames.Chess.View;component/Resources/" + src + ".png", UriKind.Relative));
         }
         catch (Exception e) {
            return null;
         }
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}
