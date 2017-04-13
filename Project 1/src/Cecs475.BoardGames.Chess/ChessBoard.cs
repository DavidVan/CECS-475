using System;
using System.Collections.Generic;
using System.Linq;

namespace Cecs475.BoardGames.Chess {

   public class ChessBoard : IGameBoard {
      /// <summary>
      /// The number of rows and columns on the chess board.
      /// </summary>
      public const int BOARD_SIZE = 8;

      private sbyte[,] mBoard = new sbyte[8, 8] {
         {-2, -4, -5, -6, -7, -5, -4, -3 },
         {-1, -1, -1, -1, -1, -1, -1, -1 },
         {0, 0, 0, 0, 0, 0, 0, 0 },
         {0, 0, 0, 0, 0, 0, 0, 0 },
         {0, 0, 0, 0, 0, 0, 0, 0 },
         {0, 0, 0, 0, 0, 0, 0, 0 },
         {1, 1, 1, 1, 1, 1, 1, 1 },
         {2, 4, 5, 6, 7, 5, 4, 3 }
      };

      private int mCurrentPlayer;

      private int alreadySetCountWhite;
      private int alreadySetCountBlack;
      private bool alreadySetWhite {
         get {
            if (alreadySetCountWhite >= 1) {
               return true;
            }
            else {
               return false;
            }
         }
      }
      private bool alreadySetBlack {
         get {
            if (alreadySetCountBlack >= 1) {
               return true;
            }
            else {
               return false;
            }
         }
      }

      private bool whiteKingMoved;
      private bool whiteKingSideRookMoved;
      private bool whiteQueenSideRookMoved;

      private bool blackKingMoved;
      private bool blackKingSideRookMoved;
      private bool blackQueenSideRookMoved;

      private bool whitePawnPromote;

      private bool blackPawnPromote;

      /// <summary>
      /// Constructs a new chess board with the default starting arrangement.
      /// </summary>
      public ChessBoard() {
         MoveHistory = new List<IGameMove>();

         // Set up flags
         mCurrentPlayer = 1;

         Value = 0;

         alreadySetCountWhite = 0;
         alreadySetCountBlack = 0;

         whiteKingMoved = false;
         whiteKingSideRookMoved = false;
         whiteQueenSideRookMoved = false;

         blackKingMoved = false;
         blackKingSideRookMoved = false;
         blackQueenSideRookMoved = false;

         whitePawnPromote = false;

         blackPawnPromote = false;
      }

      /// <summary>
      /// Constructs a new chess board by only placing pieces as specified.
      /// </summary>
      /// <param name="startingPositions">a sequence of tuple pairs, where each pair specifies the starting
      /// position of a particular piece to place on the board</param>
      public ChessBoard(IEnumerable<Tuple<BoardPosition, ChessPiecePosition>> startingPositions) : this() {


         foreach (int i in Enumerable.Range(0, 8)) { // another way of doing for i = 0 to < 8
            foreach (int j in Enumerable.Range(0, 8)) {
               mBoard[i, j] = 0;
            }
         }
         foreach (var pos in startingPositions) {
            SetPosition(pos.Item1, pos.Item2);
         }
      }

      /// <summary>
      /// A difference in piece values for the pieces still controlled by white vs. black, where
      /// a pawn is value 1, a knight and bishop are value 3, a rook is value 5, and a queen is value 9.
      /// </summary>
      public int Value { get; private set; }

      public int CurrentPlayer {
         get {
            return mCurrentPlayer == 1 ? 1 : 2;
         }
      }

      // An auto-property suffices here.
      public IList<IGameMove> MoveHistory {
         get; private set;
      }

      /// <summary>
      /// Returns the piece and player at the given position on the board.
      /// </summary>
      public ChessPiecePosition GetPieceAtPosition(BoardPosition position) {
         var boardVal = mBoard[position.Row, position.Col];
         return new ChessPiecePosition((ChessPieceType) Math.Abs(mBoard[position.Row, position.Col]),
            boardVal > 0 ? 1 : boardVal < 0 ? 2 : 0);
      }


      public void ApplyMove(IGameMove move) {
         ChessMove chessMove = move as ChessMove;
         ChessMove lastMove = null;

         if (MoveHistory.Count() != 0) {
            lastMove = MoveHistory.Last() as ChessMove;
         }

         MoveHistory.Add(chessMove);

         int player = CurrentPlayer;

         BoardPosition startPos = chessMove.StartPosition;
         BoardPosition endPos = chessMove.EndPosition;

         ChessPieceType startPieceType = GetPieceAtPosition(startPos).PieceType;
         ChessPieceType capturedType;

         if (chessMove.Captured.PieceType != ChessPieceType.Empty) {
            capturedType = chessMove.Captured.PieceType;
         }
         else if (PositionInBounds(endPos)) {
            capturedType = GetPieceAtPosition(endPos).PieceType;
         }
         else {
            capturedType = ChessPieceType.Empty;
         }


         Value += GetPieceValue(capturedType) * mCurrentPlayer;

         if (chessMove.MoveType == ChessMoveType.EnPassant) { // Apply En Passant
            mBoard[startPos.Row, startPos.Col] = (sbyte) ChessPieceType.Empty;
            mBoard[endPos.Row, endPos.Col] = (sbyte) ((sbyte) startPieceType * mCurrentPlayer);
            mBoard[lastMove.EndPosition.Row, lastMove.EndPosition.Col] = (sbyte) ChessPieceType.Empty;
         }
         else if (chessMove.MoveType == ChessMoveType.PawnPromote) { // Apply Pawn Promotion
            mBoard[startPos.Row, startPos.Col] = (sbyte) ((sbyte) endPos.Col * mCurrentPlayer);

            Value += GetPieceValue(GetPieceAtPosition(startPos).PieceType) * mCurrentPlayer;

            if (whitePawnPromote) {
               Value -= 1;
               whitePawnPromote = false;
            }
            else if (blackPawnPromote) {
               Value += 1;
               blackPawnPromote = false;
            }
         }
         else if (chessMove.MoveType == ChessMoveType.CastleKingSide) { // Apply King-side Castle Move
            mBoard[startPos.Row, startPos.Col] = (sbyte) ChessPieceType.Empty;
            mBoard[endPos.Row, endPos.Col] = (sbyte) ((sbyte) ChessPieceType.King * mCurrentPlayer);

            mBoard[endPos.Row, endPos.Col + 1] = (sbyte) ChessPieceType.Empty;
            mBoard[startPos.Row, startPos.Col + 1] = (sbyte) ((sbyte) ChessPieceType.RookKing * mCurrentPlayer);

            if (player == 1) {
               whiteKingMoved = true;
               whiteKingSideRookMoved = true;
               alreadySetCountWhite++;
            }
            else {
               blackKingMoved = true;
               blackKingSideRookMoved = true;
               alreadySetCountBlack++;
            }
         }
         else if (chessMove.MoveType == ChessMoveType.CastleQueenSide) { // Apply Queen-side Castle Move
            mBoard[startPos.Row, startPos.Col] = (sbyte) ChessPieceType.Empty;
            mBoard[endPos.Row, endPos.Col] = (sbyte) ((sbyte) ChessPieceType.King * mCurrentPlayer);

            mBoard[endPos.Row, endPos.Col - 2] = (sbyte) ChessPieceType.Empty;
            mBoard[startPos.Row, startPos.Col - 1] = (sbyte) ((sbyte) ChessPieceType.RookQueen * mCurrentPlayer);

            if (player == 1) {
               whiteKingMoved = true;
               whiteQueenSideRookMoved = true;
               alreadySetCountWhite++;
            }
            else {
               blackKingMoved = true;
               blackQueenSideRookMoved = true;
               alreadySetCountBlack++;
            }
         }
         else { // Regular Move            
            // Setting up for Pawn Promotion
            if (player == 1) {
               if (GetPieceAtPosition(startPos).PieceType == ChessPieceType.Pawn && endPos.Row == 0) {
                  whitePawnPromote = true;
               }
            }
            else {
               if (GetPieceAtPosition(startPos).PieceType == ChessPieceType.Pawn && endPos.Row == 7) {
                  blackPawnPromote = true;
               }
            }

            // Setting up for Castling
            if (GetPieceAtPosition(startPos).PieceType == ChessPieceType.King) {
               if (player == 1) {
                  whiteKingMoved = true;
                  alreadySetCountWhite++;
               }
               else {
                  blackKingMoved = true;
                  alreadySetCountBlack++;
               }
            }

            if (GetPieceAtPosition(startPos).PieceType == ChessPieceType.RookKing) {
               if (player == 1) {
                  whiteKingSideRookMoved = true;
                  alreadySetCountWhite++;
               }
               else {
                  blackKingSideRookMoved = true;
                  alreadySetCountBlack++;
               }
            }

            if (GetPieceAtPosition(startPos).PieceType == ChessPieceType.RookQueen) {
               if (player == 1) {
                  whiteQueenSideRookMoved = true;
                  alreadySetCountWhite++;
               }
               else {
                  whiteQueenSideRookMoved = true;
                  alreadySetCountBlack++;
               }
            }

            mBoard[startPos.Row, startPos.Col] = (sbyte) ChessPieceType.Empty;
            mBoard[endPos.Row, endPos.Col] = (sbyte) ((sbyte) startPieceType * mCurrentPlayer);
         }

         if (whitePawnPromote || blackPawnPromote) {
            return;
         }

         mCurrentPlayer = -mCurrentPlayer;
      }

      public IEnumerable<IGameMove> GetPossibleMoves() {
         List<ChessMove> possibleMoves = new List<ChessMove>();

         if (whitePawnPromote || blackPawnPromote) {
            return GetPossibleMovesPawnPromote();
         }

         var pawns = GetPositionsOfPiece(ChessPieceType.Pawn, CurrentPlayer);
         var movesPawns = GetPossibleMovesPawn(pawns);
         possibleMoves.AddRange(movesPawns);

         var rookKing = GetPositionsOfPiece(ChessPieceType.RookKing, CurrentPlayer);
         var movesRookKing = GetPossibleMovesRook(rookKing);
         possibleMoves.AddRange(movesRookKing);

         var rookQueen = GetPositionsOfPiece(ChessPieceType.RookQueen, CurrentPlayer);
         var movesRookQueen = GetPossibleMovesRook(rookQueen);
         possibleMoves.AddRange(movesRookQueen);

         var rookPawns = GetPositionsOfPiece(ChessPieceType.RookPawn, CurrentPlayer);
         var movesRookPawns = GetPossibleMovesRook(rookPawns);
         possibleMoves.AddRange(movesRookPawns);

         var bishops = GetPositionsOfPiece(ChessPieceType.Bishop, CurrentPlayer);
         var movesBishops = GetPossibleMovesBishop(bishops);
         possibleMoves.AddRange(movesBishops);

         var knights = GetPositionsOfPiece(ChessPieceType.Knight, CurrentPlayer);
         var movesKnights = GetPossibleMovesKnight(knights);
         possibleMoves.AddRange(movesKnights);

         var queens = GetPositionsOfPiece(ChessPieceType.Queen, CurrentPlayer);
         var movesQueens = GetPossibleMovesQueen(queens);
         possibleMoves.AddRange(movesQueens);

         var king = GetPositionsOfPiece(ChessPieceType.King, CurrentPlayer);
         var movesKings = GetPossibleMovesKing(king);
         possibleMoves.AddRange(movesKings);

         List<ChessMove> invalidMoves = new List<ChessMove>();

         foreach (ChessMove move in possibleMoves) {
            ApplyMove(move); // This does switch the player... maybe
            IEnumerable<BoardPosition> threatened;
            IEnumerable<BoardPosition> kingPositions;
            if (whitePawnPromote || blackPawnPromote) {
               threatened = GetThreatenedPositions(mCurrentPlayer == 1 ? 2 : 1); // Pawn-promote mode doesn't change players.
               kingPositions = GetPositionsOfPiece(ChessPieceType.King, CurrentPlayer); // Get position of King for opposite player.
            }
            else {
               threatened = GetThreatenedPositions(CurrentPlayer); // Should be the correct player.
               kingPositions = GetPositionsOfPiece(ChessPieceType.King, mCurrentPlayer == 1 ? 2 : 1); // Get position of King for opposite player.
            }
            BoardPosition kingPos;
            if (kingPositions.Count() == 0) {
               UndoLastMove();
               continue;
            }
            else {
               kingPos = kingPositions.First();
            }

            if (threatened.Contains(kingPos)) {
               invalidMoves.Add(move);
            }
            UndoLastMove();
         }

         possibleMoves.RemoveAll(m => invalidMoves.Contains(m));

         return possibleMoves;
      }

      private IEnumerable<ChessMove> GetAllPossibleMovesUsing(Func<BoardPosition, IEnumerable<ChessMove>> possibleMovesFunction, IEnumerable<BoardPosition> pieces) {
         HashSet<ChessMove> moves = new HashSet<ChessMove>();
         foreach (BoardPosition pos in pieces) {
            var possibleMovesFromFunction = possibleMovesFunction(pos);
            moves.UnionWith(possibleMovesFromFunction);
         }
         return moves;
      }

      private IEnumerable<ChessMove> RemoveFriendlyPiecesUsing(Func<BoardPosition, IEnumerable<BoardPosition>> threatenedFunction, BoardPosition pos) {
         HashSet<ChessMove> moves = new HashSet<ChessMove>();
         var threatened = threatenedFunction(pos).Where(t => GetPieceAtPosition(t).Player != CurrentPlayer); // Get rid of friendly pieces.
         foreach (BoardPosition threatenedPos in threatened) {
            ChessMove move = new ChessMove(pos, threatenedPos);
            move.Piece = GetPieceAtPosition(pos);
            move.Captured = GetPieceAtPosition(threatenedPos); // If enemy, save what piece we will capture, else just save as empty spot.
            moves.Add(move);
         }
         return moves;
      }

      private IEnumerable<ChessMove> GetPossibleMovesPawn(BoardPosition pos) {
         HashSet<ChessMove> moves = new HashSet<ChessMove>();

         ChessMove lastMove = null;

         if (MoveHistory.Count() != 0) {
            lastMove = MoveHistory.Last() as ChessMove;
         }

         int player = GetPlayerAtPosition(pos);

         if (player == 1) { // Player 1
            // Capture Logic
            if (PositionInBounds(pos.Translate(-1, -1)) && GetPieceAtPosition(pos.Translate(-1, -1)).PieceType != ChessPieceType.Empty && PositionIsEnemy(pos.Translate(-1, -1), CurrentPlayer)) { // Looks like it's the enemy!
               BoardPosition newPos = pos.Translate(-1, -1);
               ChessMove left = new ChessMove(pos, newPos);
               left.Piece = GetPieceAtPosition(pos);
               left.Captured = GetPieceAtPosition(newPos);
               moves.Add(left);
            }
            if (PositionInBounds(pos.Translate(-1, 1)) && GetPieceAtPosition(pos.Translate(-1, 1)).PieceType != ChessPieceType.Empty && PositionIsEnemy(pos.Translate(-1, 1), CurrentPlayer)) { // Looks like it's the enemy!
               BoardPosition newPos = pos.Translate(-1, 1);
               ChessMove right = new ChessMove(pos, newPos);
               right.Piece = GetPieceAtPosition(pos);
               right.Captured = GetPieceAtPosition(newPos);
               moves.Add(right);
            }

            // En Passant Logic
            if (lastMove != null && lastMove.StartPosition.Row == 1 && lastMove.EndPosition.Row == lastMove.StartPosition.Row + 2 && pos.Row == lastMove.EndPosition.Row) {
               if (Math.Abs(lastMove.EndPosition.Col - pos.Col) == 1) {
                  ChessMove enPassant = new ChessMove(pos, pos.Translate(-1, lastMove.EndPosition.Col - pos.Col), ChessMoveType.EnPassant);
                  enPassant.Piece = GetPieceAtPosition(pos);
                  enPassant.Captured = GetPieceAtPosition(lastMove.EndPosition);
                  moves.Add(enPassant);
               }
            }

            if (pos.Row == 6) { // Pawn still in starting location. It has two possible moves.
               if (PositionInBounds(pos.Translate(-1, 0)) && GetPieceAtPosition(pos.Translate(-1, 0)).PieceType == ChessPieceType.Empty
                     && PositionInBounds(pos.Translate(-2, 0)) && GetPieceAtPosition(pos.Translate(-2, 0)).PieceType == ChessPieceType.Empty) {
                  ChessMove upOne = new ChessMove(pos, pos.Translate(-1, 0));
                  upOne.Piece = GetPieceAtPosition(pos);
                  upOne.Captured = GetPieceAtPosition(pos.Translate(-1, 0)); // Should be empty.
                  moves.Add(upOne); // Up One
                  ChessMove upTwo = new ChessMove(pos, pos.Translate(-2, 0));
                  upTwo.Piece = GetPieceAtPosition(pos);
                  upTwo.Captured = GetPieceAtPosition(pos.Translate(-2, 0)); // Should be empty.
                  moves.Add(upTwo); // Up Two
               }
            }
            else { // Just a regular move.
               if (PositionInBounds(pos.Translate(-1, 0)) && GetPieceAtPosition(pos.Translate(-1, 0)).PieceType == ChessPieceType.Empty) {
                  ChessMove upOne = new ChessMove(pos, pos.Translate(-1, 0));
                  upOne.Piece = GetPieceAtPosition(pos);
                  upOne.Captured = GetPieceAtPosition(pos.Translate(-1, 0)); // Should be empty.
                  moves.Add(upOne); // Up One
               }
            }
         }
         else { // Player 2
            if (PositionInBounds(pos.Translate(1, -1)) && GetPieceAtPosition(pos.Translate(1, -1)).PieceType != ChessPieceType.Empty && PositionIsEnemy(pos.Translate(1, -1), CurrentPlayer)) { // Looks like it's the enemy!
               BoardPosition newPos = pos.Translate(1, -1);
               ChessMove left = new ChessMove(pos, newPos);
               left.Piece = GetPieceAtPosition(pos);
               left.Captured = GetPieceAtPosition(newPos);
               moves.Add(left);
            }
            if (PositionInBounds(pos.Translate(1, 1)) && GetPieceAtPosition(pos.Translate(1, 1)).PieceType != ChessPieceType.Empty && PositionIsEnemy(pos.Translate(1, 1), CurrentPlayer)) { // Looks like it's the enemy!
               BoardPosition newPos = pos.Translate(1, 1);
               ChessMove right = new ChessMove(pos, newPos);
               right.Piece = GetPieceAtPosition(pos);
               right.Captured = GetPieceAtPosition(newPos);
               moves.Add(right);
            }

            // En Passant Logic
            if (lastMove != null && lastMove.StartPosition.Row == 6 && lastMove.EndPosition.Row == lastMove.StartPosition.Row - 2 && pos.Row == lastMove.EndPosition.Row) {
               if (Math.Abs(lastMove.EndPosition.Col - pos.Col) == 1) {
                  ChessMove enPassant = new ChessMove(pos, pos.Translate(1, lastMove.EndPosition.Col - pos.Col), ChessMoveType.EnPassant);
                  enPassant.Piece = GetPieceAtPosition(pos);
                  enPassant.Captured = GetPieceAtPosition(lastMove.EndPosition);
                  moves.Add(enPassant);
               }
            }

            if (pos.Row == 1) { // Pawn still in starting location. It has two possible moves.
               if (PositionInBounds(pos.Translate(1, 0)) && GetPieceAtPosition(pos.Translate(1, 0)).PieceType == ChessPieceType.Empty
                     && PositionInBounds(pos.Translate(2, 0)) && GetPieceAtPosition(pos.Translate(2, 0)).PieceType == ChessPieceType.Empty) {
                  ChessMove downOne = new ChessMove(pos, pos.Translate(1, 0));
                  downOne.Piece = GetPieceAtPosition(pos);
                  downOne.Captured = GetPieceAtPosition(pos.Translate(1, 0)); // Should be empty.
                  moves.Add(downOne); // Down On
                  ChessMove downTwo = new ChessMove(pos, pos.Translate(2, 0));
                  downTwo.Piece = GetPieceAtPosition(pos);
                  downTwo.Captured = GetPieceAtPosition(pos.Translate(2, 0)); // Should be empty.
                  moves.Add(downTwo); // Down Two
               }
            }
            else { // Just a regular move.
               if (PositionInBounds(pos.Translate(1, 0)) && GetPieceAtPosition(pos.Translate(1, 0)).PieceType == ChessPieceType.Empty) {
                  ChessMove downOne = new ChessMove(pos, pos.Translate(1, 0));
                  downOne.Piece = GetPieceAtPosition(pos);
                  downOne.Captured = GetPieceAtPosition(pos.Translate(1, 0)); // Should be empty.
                  moves.Add(downOne); // Down One
               }
            }
         }
         return moves;
      }

      private IEnumerable<ChessMove> GetPossibleMovesPawn(IEnumerable<BoardPosition> pieces) {
         return GetAllPossibleMovesUsing(GetPossibleMovesPawn, pieces);
      }

      private IEnumerable<ChessMove> GetPossibleMovesPawnPromote() {
         List<ChessMove> pawnPromotionMoves = new List<ChessMove>();

         ChessMove lastMove = null;

         if (MoveHistory.Count() != 0) {
            lastMove = MoveHistory.Last() as ChessMove;
         }
         else {
            return pawnPromotionMoves;
         }

         BoardPosition bishopPosition = new BoardPosition(-1, (int) ChessPieceType.Bishop);
         BoardPosition knightPosition = new BoardPosition(-1, (int) ChessPieceType.Knight);
         BoardPosition queenPosition = new BoardPosition(-1, (int) ChessPieceType.Queen);
         BoardPosition rookPosition = new BoardPosition(-1, (int) ChessPieceType.RookPawn);

         ChessMove pawnToBishop = new ChessMove(lastMove.EndPosition, bishopPosition, ChessMoveType.PawnPromote);
         pawnToBishop.Piece = lastMove.Piece;

         ChessMove pawnToKnight = new ChessMove(lastMove.EndPosition, knightPosition, ChessMoveType.PawnPromote);
         pawnToKnight.Piece = lastMove.Piece;

         ChessMove pawnToQueen = new ChessMove(lastMove.EndPosition, queenPosition, ChessMoveType.PawnPromote);
         pawnToQueen.Piece = lastMove.Piece;

         ChessMove pawnToRook = new ChessMove(lastMove.EndPosition, rookPosition, ChessMoveType.PawnPromote);
         pawnToRook.Piece = lastMove.Piece;

         pawnPromotionMoves.Add(pawnToBishop);
         pawnPromotionMoves.Add(pawnToKnight);
         pawnPromotionMoves.Add(pawnToQueen);
         pawnPromotionMoves.Add(pawnToRook);

         return pawnPromotionMoves;
      }

      private IEnumerable<ChessMove> GetPossibleMovesRook(BoardPosition pos) {
         return RemoveFriendlyPiecesUsing(GetThreatenedByRook, pos);
      }

      private IEnumerable<ChessMove> GetPossibleMovesRook(IEnumerable<BoardPosition> pieces) {
         return GetAllPossibleMovesUsing(GetPossibleMovesRook, pieces);
      }

      private IEnumerable<ChessMove> GetPossibleMovesBishop(BoardPosition pos) {
         return RemoveFriendlyPiecesUsing(GetThreatenedByBishop, pos);
      }

      private IEnumerable<ChessMove> GetPossibleMovesBishop(IEnumerable<BoardPosition> pieces) {
         return GetAllPossibleMovesUsing(GetPossibleMovesBishop, pieces);
      }

      private IEnumerable<ChessMove> GetPossibleMovesKnight(BoardPosition pos) {
         return RemoveFriendlyPiecesUsing(GetThreatenedByKnight, pos);
      }

      private IEnumerable<ChessMove> GetPossibleMovesKnight(IEnumerable<BoardPosition> pieces) {
         return GetAllPossibleMovesUsing(GetPossibleMovesKnight, pieces);
      }

      private IEnumerable<ChessMove> GetPossibleMovesQueen(BoardPosition pos) {
         HashSet<ChessMove> moves = new HashSet<ChessMove>();
         // Use Rook Logic
         var rookLogic = GetPossibleMovesRook(pos);
         moves.UnionWith(rookLogic);

         // Use Bishop Logic
         var bishopLogic = GetPossibleMovesBishop(pos);
         moves.UnionWith(bishopLogic);

         return moves;
      }

      private IEnumerable<ChessMove> GetPossibleMovesQueen(IEnumerable<BoardPosition> pieces) {
         return GetAllPossibleMovesUsing(GetPossibleMovesQueen, pieces);
      }

      private IEnumerable<ChessMove> GetPossibleMovesKing(BoardPosition pos) {
         HashSet<ChessMove> moves = new HashSet<ChessMove>();

         int player = mCurrentPlayer == 1 ? 1 : 2;

         var movesUsingThreatened = RemoveFriendlyPiecesUsing(GetThreatenedByKing, pos);

         moves.UnionWith(movesUsingThreatened);

         // Below doesn't check move count since we're in the middle of generating it!
         bool currentlyInCheck = false;
         var threatenedPositions = GetThreatenedPositions(mCurrentPlayer == 1 ? 2 : 1);
         if (threatenedPositions.Contains(GetPositionsOfPiece(ChessPieceType.King, CurrentPlayer).First())) {
            currentlyInCheck = true;
         }

         // Castling Logic
         if (player == 1) { // Player 1
            if (!alreadySetWhite && !currentlyInCheck && !whiteKingMoved && GetPieceAtPosition(new BoardPosition(7, 4)).PieceType == ChessPieceType.King) {
               if (!whiteKingSideRookMoved && GetPieceAtPosition(new BoardPosition(7, 7)).PieceType == ChessPieceType.RookKing) {
                  if (!threatenedPositions.Contains(pos.Translate(0, 1)) && GetPieceAtPosition(pos.Translate(0, 1)).PieceType == ChessPieceType.Empty
                        && !threatenedPositions.Contains(pos.Translate(0, 2)) && GetPieceAtPosition(pos.Translate(0, 2)).PieceType == ChessPieceType.Empty) {
                     ChessMove castleKingSide = new ChessMove(pos, pos.Translate(0, 2), ChessMoveType.CastleKingSide);
                     castleKingSide.Piece = GetPieceAtPosition(pos);
                     moves.Add(castleKingSide);
                  }
               }
               if (!whiteQueenSideRookMoved && GetPieceAtPosition(new BoardPosition(7, 0)).PieceType == ChessPieceType.RookQueen) {
                  if (!threatenedPositions.Contains(pos.Translate(0, -1)) && GetPieceAtPosition(pos.Translate(0, -1)).PieceType == ChessPieceType.Empty
                        && !threatenedPositions.Contains(pos.Translate(0, -2)) && GetPieceAtPosition(pos.Translate(0, -2)).PieceType == ChessPieceType.Empty
                           && GetPieceAtPosition(pos.Translate(0, -3)).PieceType == ChessPieceType.Empty) {
                     ChessMove castleQueenSide = new ChessMove(pos, pos.Translate(0, -2), ChessMoveType.CastleQueenSide);
                     castleQueenSide.Piece = GetPieceAtPosition(pos);
                     moves.Add(castleQueenSide);
                  }
               }
            }
         }
         else { // Player 2
            if (!alreadySetBlack && !currentlyInCheck && !blackKingMoved && GetPieceAtPosition(new BoardPosition(0, 4)).PieceType == ChessPieceType.King) {
               if (!blackKingSideRookMoved && GetPieceAtPosition(new BoardPosition(0, 7)).PieceType == ChessPieceType.RookKing) {
                  if (!threatenedPositions.Contains(pos.Translate(0, 1)) && GetPieceAtPosition(pos.Translate(0, 1)).PieceType == ChessPieceType.Empty
                        && !threatenedPositions.Contains(pos.Translate(0, 2)) && GetPieceAtPosition(pos.Translate(0, 2)).PieceType == ChessPieceType.Empty) {
                     ChessMove castleKingSide = new ChessMove(pos, pos.Translate(0, 2), ChessMoveType.CastleKingSide);
                     castleKingSide.Piece = GetPieceAtPosition(pos);
                     moves.Add(castleKingSide);
                  }
               }
               if (!blackQueenSideRookMoved && GetPieceAtPosition(new BoardPosition(0, 0)).PieceType == ChessPieceType.RookQueen) {
                  if (!threatenedPositions.Contains(pos.Translate(0, -1)) && GetPieceAtPosition(pos.Translate(0, -1)).PieceType == ChessPieceType.Empty
                        && !threatenedPositions.Contains(pos.Translate(0, -2)) && GetPieceAtPosition(pos.Translate(0, -2)).PieceType == ChessPieceType.Empty
                           && GetPieceAtPosition(pos.Translate(0, -3)).PieceType == ChessPieceType.Empty) {
                     ChessMove castleQueenSide = new ChessMove(pos, pos.Translate(0, -2), ChessMoveType.CastleQueenSide);
                     castleQueenSide.Piece = GetPieceAtPosition(pos);
                     moves.Add(castleQueenSide);
                  }
               }
            }
         }
         return moves;
      }

      private IEnumerable<ChessMove> GetPossibleMovesKing(IEnumerable<BoardPosition> pieces) {
         return GetAllPossibleMovesUsing(GetPossibleMovesKing, pieces);
      }

      /// <summary>
      /// Gets a sequence of all positions on the board that are threatened by the given player. A king
      /// may not move to a square threatened by the opponent.
      /// </summary>
      public IEnumerable<BoardPosition> GetThreatenedPositions(int byPlayer) {
         HashSet<BoardPosition> threatenedPositions = new HashSet<BoardPosition>();

         var pawns = GetPositionsOfPiece(ChessPieceType.Pawn, byPlayer);
         var threatenedByPawns = GetThreatenedByPawn(pawns);
         threatenedPositions.UnionWith(threatenedByPawns);

         var rookKing = GetPositionsOfPiece(ChessPieceType.RookKing, byPlayer);
         var threatenedByRookKing = GetThreatenedByRook(rookKing);
         threatenedPositions.UnionWith(threatenedByRookKing);

         var rookQueen = GetPositionsOfPiece(ChessPieceType.RookQueen, byPlayer);
         var threatenedByRookQueen = GetThreatenedByRook(rookQueen);
         threatenedPositions.UnionWith(threatenedByRookQueen);

         var rookPawns = GetPositionsOfPiece(ChessPieceType.RookPawn, byPlayer);
         var threatenedByRookPawns = GetThreatenedByRook(rookPawns);
         threatenedPositions.UnionWith(threatenedByRookPawns);

         var bishops = GetPositionsOfPiece(ChessPieceType.Bishop, byPlayer);
         var threatenedByBishops = GetThreatenedByBishop(bishops);
         threatenedPositions.UnionWith(threatenedByBishops);

         var knights = GetPositionsOfPiece(ChessPieceType.Knight, byPlayer);
         var threatenedByKnights = GetThreatenedByKnight(knights);
         threatenedPositions.UnionWith(threatenedByKnights);

         var queens = GetPositionsOfPiece(ChessPieceType.Queen, byPlayer);
         var threatenedByQueens = GetThreatenedByQueen(queens);
         threatenedPositions.UnionWith(threatenedByQueens);

         var king = GetPositionsOfPiece(ChessPieceType.King, byPlayer);
         var threatenedByKings = GetThreatenedByKing(king);
         threatenedPositions.UnionWith(threatenedByKings);

         return threatenedPositions;
      }

      private IEnumerable<BoardPosition> GetAllThreatenedUsing(Func<BoardPosition, IEnumerable<BoardPosition>> threatenedFunction, IEnumerable<BoardPosition> pieces) {
         HashSet<BoardPosition> threatened = new HashSet<BoardPosition>();
         foreach (BoardPosition pos in pieces) {
            var threatenedFromFunction = threatenedFunction(pos);
            foreach (BoardPosition threatenedPos in threatenedFromFunction) {
               threatened.Add(threatenedPos);
            }
         }
         return threatened;
      }

      private IEnumerable<BoardPosition> GetThreatenedByPawn(BoardPosition pos) {
         HashSet<BoardPosition> threatened = new HashSet<BoardPosition>();
         int player = GetPieceAtPosition(pos).Player;
         BoardPosition left;
         BoardPosition right;
         if (player == 1) // Player 1
         {
            left = new BoardPosition(pos.Row - 1, pos.Col - 1);
            right = new BoardPosition(pos.Row - 1, pos.Col + 1);
         }
         else // Player 2
         {
            left = new BoardPosition(pos.Row + 1, pos.Col - 1);
            right = new BoardPosition(pos.Row + 1, pos.Col + 1);
         }

         if (PositionInBounds(left)) {
            threatened.Add(left);
         }
         if (PositionInBounds(right)) {
            threatened.Add(right);
         }
         return threatened;
      }

      private IEnumerable<BoardPosition> GetThreatenedByPawn(IEnumerable<BoardPosition> pieces) {
         return GetAllThreatenedUsing(GetThreatenedByPawn, pieces);
      }

      private IEnumerable<BoardPosition> GetThreatenedByRook(BoardPosition pos) {
         HashSet<BoardPosition> threatened = new HashSet<BoardPosition>();
         int upRange = pos.Row;
         int leftRange = pos.Col;

         int downRange = BOARD_SIZE - pos.Row - 1;
         int rightRange = BOARD_SIZE - pos.Col - 1;

         foreach (int i in Enumerable.Range(0, upRange)) {
            BoardPosition nextSpot = new BoardPosition(pos.Row - 1 - i, pos.Col);
            if (PositionInBounds(nextSpot)) { // Sanity Check. It should be in bounds.
               threatened.Add(nextSpot);
               if (GetPieceAtPosition(nextSpot).PieceType != ChessPieceType.Empty) { // Hit something! Break!
                  break;
               }
            }
         }
         foreach (int i in Enumerable.Range(0, leftRange)) {
            BoardPosition nextSpot = new BoardPosition(pos.Row, pos.Col - 1 - i);
            if (PositionInBounds(nextSpot)) { // Sanity Check. It should be in bounds.
               threatened.Add(nextSpot);
               if (GetPieceAtPosition(nextSpot).PieceType != ChessPieceType.Empty) { // Hit something! Break!
                  break;
               }
            }
         }
         foreach (int i in Enumerable.Range(0, downRange)) {
            BoardPosition nextSpot = new BoardPosition(pos.Row + 1 + i, pos.Col);
            if (PositionInBounds(nextSpot)) { // Sanity Check. It should be in bounds.
               threatened.Add(nextSpot);
               if (GetPieceAtPosition(nextSpot).PieceType != ChessPieceType.Empty) { // Hit something! Break!
                  break;
               }
            }
         }
         foreach (int i in Enumerable.Range(0, rightRange)) {
            BoardPosition nextSpot = new BoardPosition(pos.Row, pos.Col + 1 + i);
            if (PositionInBounds(nextSpot)) { // Sanity Check. It should be in bounds.
               threatened.Add(nextSpot);
               if (GetPieceAtPosition(nextSpot).PieceType != ChessPieceType.Empty) { // Hit something! Break!
                  break;
               }
            }
         }
         return threatened;
      }

      private IEnumerable<BoardPosition> GetThreatenedByRook(IEnumerable<BoardPosition> pieces) {
         return GetAllThreatenedUsing(GetThreatenedByRook, pieces);
      }

      private IEnumerable<BoardPosition> GetThreatenedByBishop(BoardPosition pos) {
         HashSet<BoardPosition> threatened = new HashSet<BoardPosition>();
         foreach (int i in Enumerable.Range(1, 8)) { // Didn't calcualte like for Rook since there are too many factors.
            BoardPosition nextSpot = new BoardPosition(pos.Row + i, pos.Col - i); // +i row, -i col - Go Up Left
            if (PositionInBounds(nextSpot)) { // It should be in bounds.
               threatened.Add(nextSpot);
               if (GetPieceAtPosition(nextSpot).PieceType != ChessPieceType.Empty) { // Hit something! Break!
                  break;
               }
            }
            else {
               break; // Went past bounds. Break.
            }
         }
         foreach (int i in Enumerable.Range(1, 8)) { // Didn't calcualte like for Rook since there are too many factors.
            BoardPosition nextSpot = new BoardPosition(pos.Row + i, pos.Col + i); // +i row, +i col - Go Up Right
            if (PositionInBounds(nextSpot)) { // It should be in bounds.
               threatened.Add(nextSpot);
               if (GetPieceAtPosition(nextSpot).PieceType != ChessPieceType.Empty) { // Hit something! Break!
                  break;
               }
            }
            else {
               break; // Went past bounds. Break.
            }
         }
         foreach (int i in Enumerable.Range(1, 8)) { // Didn't calcualte like for Rook since there are too many factors.
            BoardPosition nextSpot = new BoardPosition(pos.Row - i, pos.Col - i); // -i row, -i col - Go Down Left
            if (PositionInBounds(nextSpot)) { // It should be in bounds.
               threatened.Add(nextSpot);
               if (GetPieceAtPosition(nextSpot).PieceType != ChessPieceType.Empty) { // Hit something! Break!
                  break;
               }
            }
            else {
               break; // Went past bounds. Break.
            }
         }
         foreach (int i in Enumerable.Range(1, 8)) { // Didn't calcualte like for Rook since there are too many factors.
            BoardPosition nextSpot = new BoardPosition(pos.Row - i, pos.Col + i); // -i row, +i col - Go Down Right
            if (PositionInBounds(nextSpot)) { // It should be in bounds.
               threatened.Add(nextSpot);
               if (GetPieceAtPosition(nextSpot).PieceType != ChessPieceType.Empty) { // Hit something! Break!
                  break;
               }
            }
            else {
               break; // Went past bounds. Break.
            }
         }
         return threatened;
      }

      private IEnumerable<BoardPosition> GetThreatenedByBishop(IEnumerable<BoardPosition> pieces) {
         return GetAllThreatenedUsing(GetThreatenedByBishop, pieces);
      }

      private IEnumerable<BoardPosition> GetThreatenedByKnight(BoardPosition pos) {
         HashSet<BoardPosition> threatened = new HashSet<BoardPosition>();
         BoardPosition[] allDirections = {
            new BoardPosition(pos.Row - 1, pos.Col - 2), // Left Up
            new BoardPosition(pos.Row - 2, pos.Col - 1), // Up Left
            new BoardPosition(pos.Row - 1, pos.Col + 2), // Right Up
            new BoardPosition(pos.Row - 2, pos.Col + 1), // Up Right
            new BoardPosition(pos.Row + 1, pos.Col - 2), // Left Down
            new BoardPosition(pos.Row + 2, pos.Col - 1), // Down Left
            new BoardPosition(pos.Row + 1, pos.Col + 2), // Right Down
            new BoardPosition(pos.Row + 2, pos.Col + 1)  // Down Right
         }; // Down Right
         foreach (BoardPosition dir in allDirections) {
            if (PositionInBounds(dir)) {
               threatened.Add(dir);
            }
         }

         return threatened;
      }

      private IEnumerable<BoardPosition> GetThreatenedByKnight(IEnumerable<BoardPosition> pieces) {
         return GetAllThreatenedUsing(GetThreatenedByKnight, pieces);
      }

      private IEnumerable<BoardPosition> GetThreatenedByQueen(BoardPosition pos) {
         HashSet<BoardPosition> threatened = new HashSet<BoardPosition>();
         // Use Rook Logic
         var rookLogic = GetThreatenedByRook(pos);
         threatened.UnionWith(rookLogic);

         // Use Bishop Logic
         var bishopLogic = GetThreatenedByBishop(pos);
         threatened.UnionWith(bishopLogic);

         return threatened;
      }

      private IEnumerable<BoardPosition> GetThreatenedByQueen(IEnumerable<BoardPosition> pieces) {
         return GetAllThreatenedUsing(GetThreatenedByQueen, pieces);
      }

      private IEnumerable<BoardPosition> GetThreatenedByKing(BoardPosition pos) {
         HashSet<BoardPosition> threatened = new HashSet<BoardPosition>();
         BoardPosition[] allDirections = {
            new BoardPosition(pos.Row - 1, pos.Col - 1), // Upper Left
            new BoardPosition(pos.Row - 1, pos.Col),     // Upper Middle
            new BoardPosition(pos.Row - 1, pos.Col + 1), // Upper Right
            new BoardPosition(pos.Row, pos.Col - 1),     // Middle Left
            new BoardPosition(pos.Row, pos.Col + 1),     // Middle Right
            new BoardPosition(pos.Row + 1, pos.Col - 1), // Bottom Left
            new BoardPosition(pos.Row + 1, pos.Col),     // Bottom Middle
            new BoardPosition(pos.Row + 1, pos.Col + 1)  // Bottom Right
         };
         foreach (BoardPosition dir in allDirections) {
            if (PositionInBounds(dir)) {
               threatened.Add(dir);
            }
         }
         return threatened;
      }

      private IEnumerable<BoardPosition> GetThreatenedByKing(IEnumerable<BoardPosition> pieces) {
         return GetAllThreatenedUsing(GetThreatenedByKing, pieces);
      }

      public void UndoLastMove() {
         ChessMove move = MoveHistory[MoveHistory.Count() - 1] as ChessMove;

         bool skipChangePlayer = false;

         int player = mCurrentPlayer == 1 ? 2 : 1;

         BoardPosition startPos = move.StartPosition;
         BoardPosition endPos = move.EndPosition;

         ChessPieceType capturedType;

         if (move.Captured.PieceType != ChessPieceType.Empty) {
            capturedType = move.Captured.PieceType;
         }
         else {
            capturedType = ChessPieceType.Empty;
         }

         if (whitePawnPromote || blackPawnPromote) {
            Value += GetPieceValue(capturedType) * -mCurrentPlayer;
         }
         else {
            Value += GetPieceValue(capturedType) * mCurrentPlayer;
         }

         if (move.MoveType == ChessMoveType.EnPassant) {
            mBoard[startPos.Row, startPos.Col] = (sbyte) ((sbyte) ChessPieceType.Pawn * -mCurrentPlayer);
            mBoard[endPos.Row, endPos.Col] = (sbyte) ChessPieceType.Empty;
            mBoard[endPos.Row - mCurrentPlayer, endPos.Col] = (sbyte) ((sbyte) ChessPieceType.Pawn * mCurrentPlayer);
         }
         else if (move.MoveType == ChessMoveType.PawnPromote) {
            Value += GetPieceValue(GetPieceAtPosition(startPos).PieceType) * mCurrentPlayer;

            mBoard[startPos.Row, startPos.Col] = (sbyte) ((sbyte) ChessPieceType.Pawn * -mCurrentPlayer);

            if (GetPlayerAtPosition(startPos) == 1) { // Starting Position because it was a Pawn Promote move.
               Value += 1;
               whitePawnPromote = true;
            }
            else {
               Value -= 1;
               blackPawnPromote = true;
            }
         }
         else if (move.MoveType == ChessMoveType.CastleKingSide) {
            mBoard[startPos.Row, startPos.Col] = (sbyte) ((sbyte) ChessPieceType.King * -mCurrentPlayer);
            mBoard[endPos.Row, endPos.Col] = (sbyte) ChessPieceType.Empty;

            mBoard[endPos.Row, endPos.Col + 1] = (sbyte) ((sbyte) ChessPieceType.RookKing * -mCurrentPlayer);
            mBoard[startPos.Row, startPos.Col + 1] = (sbyte) ChessPieceType.Empty;

            if (mCurrentPlayer == -1 && whiteKingMoved && whiteKingSideRookMoved) {
               whiteKingMoved = false;
               whiteKingSideRookMoved = false;
               alreadySetCountWhite--;
            }
            else if (mCurrentPlayer == 1 && blackKingMoved && blackKingSideRookMoved) {
               blackKingMoved = false;
               blackKingSideRookMoved = false;
               alreadySetCountBlack--;
            }
         }
         else if (move.MoveType == ChessMoveType.CastleQueenSide) {
            mBoard[startPos.Row, startPos.Col] = (sbyte) ((sbyte) ChessPieceType.King * -mCurrentPlayer);
            mBoard[endPos.Row, endPos.Col] = (sbyte) ChessPieceType.Empty;

            mBoard[endPos.Row, endPos.Col - 2] = (sbyte) ((sbyte) ChessPieceType.RookQueen * -mCurrentPlayer);
            mBoard[startPos.Row, startPos.Col - 1] = (sbyte) ChessPieceType.Empty;

            if (mCurrentPlayer == -1 && whiteKingMoved && whiteQueenSideRookMoved) {
               whiteKingMoved = false;
               whiteQueenSideRookMoved = false;
               alreadySetCountWhite--;
            }
            else if (mCurrentPlayer == 1 && blackKingMoved && blackQueenSideRookMoved) {
               blackKingMoved = false;
               blackQueenSideRookMoved = false;
               alreadySetCountBlack--;
            }
         }
         else {
            ChessPieceType startPieceType = GetPieceAtPosition(endPos).PieceType;
            if (whitePawnPromote || blackPawnPromote) { // Make sure we can undo a pre-Pawn Promote move correctly.
               mBoard[startPos.Row, startPos.Col] = (sbyte) ((sbyte) startPieceType * mCurrentPlayer);
               mBoard[endPos.Row, endPos.Col] = (sbyte) ((sbyte) capturedType * -mCurrentPlayer);
            }
            else {
               mBoard[startPos.Row, startPos.Col] = (sbyte) ((sbyte) startPieceType * -mCurrentPlayer);
               mBoard[endPos.Row, endPos.Col] = (sbyte) ((sbyte) capturedType * mCurrentPlayer);
            }

            if (whitePawnPromote) {
               whitePawnPromote = false;
               skipChangePlayer = true;
            }
            else if (blackPawnPromote) {
               blackPawnPromote = false;
               skipChangePlayer = true;
            }

            if (startPieceType == ChessPieceType.King) {
               if (player == 1) {
                  whiteKingMoved = false;
                  alreadySetCountWhite--;
               }
               else {
                  blackKingMoved = false;
                  alreadySetCountBlack--;
               }
            }
            else if (startPieceType == ChessPieceType.RookKing) {
               if (player == 1) {
                  whiteKingSideRookMoved = false;
                  alreadySetCountWhite--;
               }
               else {
                  blackKingSideRookMoved = false;
                  alreadySetCountBlack--;
               }
            }
            else if (startPieceType == ChessPieceType.RookQueen) {
               if (player == 1) {
                  whiteQueenSideRookMoved = false;
                  alreadySetCountWhite--;
               }
               else {
                  blackQueenSideRookMoved = false;
                  alreadySetCountBlack--;
               }
            }
         }

         if (!skipChangePlayer) {
            mCurrentPlayer = -mCurrentPlayer;
         }

         MoveHistory.RemoveAt(MoveHistory.Count() - 1);
      }


      /// <summary>
      /// Returns true if the given position on the board is empty.
      /// </summary>
      /// <remarks>returns false if the position is not in bounds</remarks>
      public bool PositionIsEmpty(BoardPosition pos) {
         return GetPieceAtPosition(pos).PieceType == ChessPieceType.Empty;
      }

      /// <summary>
      /// Returns true if the given position contains a piece that is the enemy of the given player.
      /// </summary>
      /// <remarks>returns false if the position is not in bounds</remarks>
      public bool PositionIsEnemy(BoardPosition pos, int player) {
         if (!PositionInBounds(pos) || GetPlayerAtPosition(pos) == 0) {
            return false;
         }
         return GetPlayerAtPosition(pos) != player;
      }

      /// <summary>
      /// Returns true if the given position is in the bounds of the board.
      /// </summary>
      public static bool PositionInBounds(BoardPosition pos) {
         return pos.Row >= 0 && pos.Row < BOARD_SIZE && pos.Col >= 0 && pos.Col < BOARD_SIZE;
      }

      /// <summary>
      /// Returns which player has a piece at the given board position, or 0 if it is empty.
      /// </summary>
      public int GetPlayerAtPosition(BoardPosition pos) {
         // TODO: implement this method, returning 1, 2, or 0.
         var piece = mBoard[pos.Row, pos.Col];
         return piece > 0 ? 1 : piece < 0 ? 2 : 0;
      }

      /// <summary>
      /// Gets the value weight for a piece of the given type.
      /// </summary>
      /*
       * VALUES:
       * Pawn: 1
       * Knight: 3
       * Bishop: 3
       * Rook: 5
       * Queen: 9
       * King: infinity (maximum integer value)
       */
      public int GetPieceValue(ChessPieceType pieceType) {
         switch (pieceType) {
            case ChessPieceType.Pawn:
               return 1;
            case ChessPieceType.Knight:
               return 3;
            case ChessPieceType.Bishop:
               return 3;
            case ChessPieceType.RookKing:
               return 5;
            case ChessPieceType.RookQueen:
               return 5;
            case ChessPieceType.RookPawn:
               return 5;
            case ChessPieceType.Queen:
               return 9;
            case ChessPieceType.King:
               return int.MaxValue;
            default:
               return 0;
         }
      }

      /// <summary>
      /// Returns a sequence of all positions that contain the given piece controlled by the given player.
      /// </summary>
      /// <returns>an empty sequence if the given player does not control any of the given piece type</returns>
      public IEnumerable<BoardPosition> GetPositionsOfPiece(ChessPieceType piece, int player) {
         IList<BoardPosition> pieces = new List<BoardPosition>();
         foreach (int i in Enumerable.Range(0, 8)) {
            foreach (int j in Enumerable.Range(0, 8)) {
               BoardPosition pos = new BoardPosition(i, j);
               ChessPiecePosition currentPiece = GetPieceAtPosition(pos);
               if (currentPiece.PieceType == piece && currentPiece.Player == player) {
                  pieces.Add(pos);
               }
            }
         }
         return pieces;
      }

      /// <summary>
      /// True if the current player is in check and has no possible moves.
      /// </summary>
      public bool IsCheckmate {
         get {
            bool isInCheck = false;
            var listOfKingPos = GetPositionsOfPiece(ChessPieceType.King, CurrentPlayer);
            if (listOfKingPos.Count() == 0) { // No King on board...
               return false;
            }
            BoardPosition kingPos = listOfKingPos.First();
            if (GetThreatenedPositions(mCurrentPlayer == 1 ? 2 : 1).Contains(kingPos)) {
               isInCheck = true;
            }
            return isInCheck && GetPossibleMoves().Count() == 0;
         }
      }

      /// <summary>
      /// True if the game is a statemate because the current player has no moves, but is not in check.
      /// </summary>
      public bool IsStalemate {
         get {
            bool isInCheck = false;
            var listOfKingPos = GetPositionsOfPiece(ChessPieceType.King, CurrentPlayer);
            if (listOfKingPos.Count() == 0) { // No King on board...
               return false;
            }
            BoardPosition kingPos = listOfKingPos.First();
            if (GetThreatenedPositions(mCurrentPlayer == 1 ? 2 : 1).Contains(kingPos)) {
               isInCheck = true;
            }
            return !isInCheck && GetPossibleMoves().Count() == 0;
         }
      }

      /// <summary>
      /// True if the current player is in check but has at least one possible move.
      /// </summary>
      public bool IsCheck {
         get {
            bool isInCheck = false;
            var listOfKingPos = GetPositionsOfPiece(ChessPieceType.King, CurrentPlayer);
            if (listOfKingPos.Count() == 0) { // No King on board...
               return false;
            }
            BoardPosition kingPos = listOfKingPos.First();
            if (GetThreatenedPositions(mCurrentPlayer == 1 ? 2 : 1).Contains(kingPos)) {
               isInCheck = true;
            }
            return isInCheck && GetPossibleMoves().Count() >= 1;
         }
      }

      /// <summary>
      /// Manually places the given piece at the given position.
      /// </summary>
      // This is used in the constructor
      private void SetPosition(BoardPosition position, ChessPiecePosition piece) {
         mBoard[position.Row, position.Col] = (sbyte) ((int) piece.PieceType * (piece.Player == 2 ? -1 : piece.Player));
      }
   }
}
