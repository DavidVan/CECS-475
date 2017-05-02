using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Cecs475.BoardGames;
using Cecs475.BoardGames.View;
using System;

namespace Cecs475.BoardGames.Chess.View {

   public class ChessSquare : INotifyPropertyChanged {
      private int mPlayer;

      public int Player {
         get { return mPlayer; }
         set {
            if (value != mPlayer) {
               mPlayer = value;
               OnPropertyChanged(nameof(Player));
            }
         }
      }

      public BoardPosition Position {
         get; set;
      }

      private ChessPiecePosition mPiece;
      public ChessPiecePosition Piece {
         get {
            return mPiece;
         }
         set {
            mPiece = value;
            OnPropertyChanged(nameof(Piece));
         }
      }

      private bool mIsSelected;
      public bool IsSelected {
         get { return mIsSelected; }
         set {
            if (value != mIsSelected) {
               mIsSelected = value;
               OnPropertyChanged(nameof(IsSelected));
            }
         }
      }

      private bool mIsHovered;
      public bool IsHovered {
         get { return mIsHovered; }
         set {
            if (value != mIsHovered) {
               mIsHovered = value;
               OnPropertyChanged(nameof(IsHovered));
            }
         }
      }

      public ChessViewModel ViewModel {
         get; set;
      }

      public event PropertyChangedEventHandler PropertyChanged;

      private void OnPropertyChanged(string name) {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
      }

   }

   public class ChessViewModel : INotifyPropertyChanged, IGameViewModel {
      private ChessBoard mBoard;
      private ObservableCollection<ChessSquare> mSquares;

      public event PropertyChangedEventHandler PropertyChanged; // Needed for INotifyPropertyChanged
      public event EventHandler GameFinished; // For IGameViewModel

      private void OnPropertyChanged(string name) { // Needed for INotifyPropertyChanged
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
      }

      public ChessViewModel() {
         mBoard = new ChessBoard();
         mSquares = new ObservableCollection<ChessSquare>(
            from pos in (
               from r in Enumerable.Range(0, 8)
               from c in Enumerable.Range(0, 8)
               select new BoardPosition(r, c)
            )
            select new ChessSquare() {
               Player = mBoard.GetPieceAtPosition(pos).Player,
               Position = pos,
               Piece = mBoard.GetPieceAtPosition(pos),
               ViewModel = this
            }
         );

         PossibleMoves = new HashSet<ChessMove>(
            from ChessMove m in mBoard.GetPossibleMoves()
            select m
         );

         ShowPromotionWindow = false;
      }

      public void UndoMove() {
         var lastMove = (ChessMove) mBoard.MoveHistory[mBoard.MoveHistory.Count() - 1];
         if (lastMove.MoveType == ChessMoveType.PawnPromote) {
            mBoard.UndoLastMove();
            mBoard.UndoLastMove();
         }
         else {
            mBoard.UndoLastMove();
            if (ShowPromotionWindow) {
               ShowPromotionWindow = false;
            }
         }

         PossibleMoves = new HashSet<ChessMove>(
            from ChessMove m in mBoard.GetPossibleMoves()
            select m
         );
         var newSquares =
            from r in Enumerable.Range(0, 8)
            from c in Enumerable.Range(0, 8)
            select new BoardPosition(r, c);
         int i = 0;
         foreach (var pos in newSquares) {
            mSquares[i].Piece = mBoard.GetPieceAtPosition(pos);
            mSquares[i].Player = mBoard.GetPieceAtPosition(pos).Player;
            i++;
         }

         

         OnPropertyChanged(nameof(BoardValue));
         OnPropertyChanged(nameof(CurrentPlayer));
         OnPropertyChanged(nameof(CanUndo));
         OnPropertyChanged(nameof(IsCheck));
      }

      public void ApplyMove(ChessMove userMove) {
         var possMoves = mBoard.GetPossibleMoves() as IEnumerable<ChessMove>;
         foreach (var move in possMoves) {
            if (move.Equals(userMove)) {
               mBoard.ApplyMove(move);
               break;
            }
         }

         PossibleMoves = new HashSet<ChessMove>(
            from ChessMove m in mBoard.GetPossibleMoves()
            select m
         );
         var newSquares =
            from r in Enumerable.Range(0, 8)
            from c in Enumerable.Range(0, 8)
            select new BoardPosition(r, c);
         int i = 0;
         foreach (var pos in newSquares) {
            mSquares[i].Piece = mBoard.GetPieceAtPosition(pos);
            mSquares[i].Player = mBoard.GetPieceAtPosition(pos).Player;
            i++;
         }
         OnPropertyChanged(nameof(BoardValue));
         OnPropertyChanged(nameof(CurrentPlayer));
         OnPropertyChanged(nameof(CanUndo));
         OnPropertyChanged(nameof(IsCheck));

         if (mBoard.IsStalemate || mBoard.IsCheckmate) {
            GameFinished?.Invoke(this, new EventArgs());
         }
      }

      public ObservableCollection<ChessSquare> Squares {
         get { return mSquares; }

      }

      public HashSet<ChessMove> PossibleMoves {
         get; private set;
      }

      public int BoardValue { get { return mBoard.Value; } }

      public int CurrentPlayer { get { return mBoard.CurrentPlayer; } }

      public ChessSquare SelectedSquare {
         get; set;
      }

      public bool IsCheck {
         get {
            return mBoard.IsCheck;
         }
      }

      public IEnumerable<Tuple<ChessMove, int>> PossibleMovesForPromotion {
         get {
            var temp = PossibleMoves.ToList();
            temp.Sort((m1, m2) => mBoard.GetPieceValue((ChessPieceType) m1.EndPosition.Col).CompareTo(mBoard.GetPieceValue((ChessPieceType) m2.EndPosition.Col)));
            var moves = temp.Select(m => Tuple.Create(m, CurrentPlayer)).ToList();
            return moves;
         }
      }

      private bool mShowPromotionalWindow;
      public bool ShowPromotionWindow {
         get {
            return mShowPromotionalWindow;
         }
         set {
            if (value != mShowPromotionalWindow) {
               mShowPromotionalWindow = value;
            }
         }
      }

      public bool CanUndo {
         get {
            return mBoard.MoveHistory.Count > 0;
         }
      }
   }
}
