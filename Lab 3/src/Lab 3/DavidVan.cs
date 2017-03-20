using System.Linq;
using System.Collections.Generic;
using System;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;

namespace Cecs475.BoardGames.Chess.Test
{
    /// <summary>
    /// This partial class implementation includes many utility methods to make writing tests easier.
    /// </summary>
    public partial class ChessTests
    {
        /// <summary>
        /// Test initial state by checking if rooks are in their correct places/have correct types.
        /// This tests involves both White and Black players.
        /// </summary>
        [Fact]
        public void InitialStartingStateWhiteAndBlack()
        {
            ChessBoard b = new ChessBoard();

            b.CurrentPlayer.Should().Be(1, "First player should be White.");
            b.Value.Should().Be(0, "New game, so score should be 0.");

            b.GetPlayerAtPosition(Pos("a1")).Should().Be(1, "This is Player 1's Rook.");
            b.GetPieceAtPosition(Pos("a1")).PieceType.Should().Be(ChessPieceType.RookQueen, "This should be the Rook to the left of the Queen.");

            b.GetPlayerAtPosition(Pos("a8")).Should().Be(2, "This is Player 2's Rook.");
            b.GetPieceAtPosition(Pos("a8")).PieceType.Should().Be(ChessPieceType.RookQueen, "This should be the Rook to the left of the Queen.");

            b.GetPlayerAtPosition(Pos("h1")).Should().Be(1, "This is Player 1's Rook.");
            b.GetPieceAtPosition(Pos("h1")).PieceType.Should().Be(ChessPieceType.RookKing, "This should be the Rook to the right of the King.");

            b.GetPlayerAtPosition(Pos("h8")).Should().Be(2, "This is Player 2's Rook.");
            b.GetPieceAtPosition(Pos("h8")).PieceType.Should().Be(ChessPieceType.RookKing, "This should be the Rook to the right of the King.");

        }

        /// <summary>
        /// Test castling on Black player.
        /// </summary>
        [Fact]
        public void TrickyCastlingBlack()
        {
            ChessBoard b = CreateBoardFromMoves(new ChessMove[]
            {
                Move("g2", "g3"),
                Move("g7", "g6"),
                Move("g1", "f3"),
                Move("g8", "f6"),
                Move("f1", "g2"),
                Move("f8", "g7"),
                Move("b2", "b3")
            });

            b.CurrentPlayer.Should().Be(2, "Current player should be Black.");

            var possMoves = b.GetPossibleMoves();
            possMoves.Should().Contain(m => ((ChessMove)m).MoveType == ChessMoveType.CastleKingSide, "Castling is possible."); // If available, King hasn't moved yet.

            ApplyMove(b, Move("e8", "g8")); // Execute Black Player's King-side Castle

            b.CurrentPlayer.Should().Be(1, "Castling is a move so player should have changed after castling.");

            var rook = b.GetPieceAtPosition(Pos("f8"));
            rook.PieceType.Should().Be(ChessPieceType.RookKing, "Piece should be King-side rook.");
            rook.Player.Should().Be(2, "Rook should be Black's piece.");

            var king = b.GetPieceAtPosition(Pos("g8"));
            king.PieceType.Should().Be(ChessPieceType.King, "Piece should be King.");
            king.Player.Should().Be(2, "King should be Black's piece.");

            b.UndoLastMove(); // Undo castling.

            b.CurrentPlayer.Should().Be(2, "Current player should switch back to Black after undo.");
            b.GetPieceAtPosition(Pos("f8")).PieceType.Should().Be(ChessPieceType.Empty, "Space should be empty after undo.");
            b.GetPieceAtPosition(Pos("g8")).PieceType.Should().Be(ChessPieceType.Empty, "Space should be empty after undo.");

            rook = b.GetPieceAtPosition(Pos("h8"));
            rook.PieceType.Should().Be(ChessPieceType.RookKing, "Piece should be King-side rook.");
            rook.Player.Should().Be(2, "Rook should be Black's piece.");

            king = b.GetPieceAtPosition(Pos("e8"));
            king.PieceType.Should().Be(ChessPieceType.King, "Piece should be King.");
            king.Player.Should().Be(2, "King should be Black's piece.");

        }

        /// <summary>
        /// Tests if checkmate detection is working correctly.
        /// </summary>
        [Fact]
        public void CheckMateWhite()
        {
            ChessBoard b = CreateBoardFromMoves(new ChessMove[]
            {
                Move ("e2", "e4"),
                Move ("e7", "e5"),
                Move ("f1", "c4"),
                Move ("b8", "c6"),
                Move ("d1", "h5"),
                Move ("d7", "d6")
            });

            b.CurrentPlayer.Should().Be(1, "Current player should be White.");

            b.ApplyMove(Move("h5", "f7")); // Checkmate (White's doing)!

            b.CurrentPlayer.Should().Be(2, "Current player should be Black.");

            var possMoves = b.GetPossibleMoves();

            possMoves.Count().Should().Be(0); // There should be no available moves!

            b.IsCheck.Should().Be(false, "Should not be checked since it's already checkmate.");
            b.IsCheckmate.Should().Be(true, "Should be checkmate.");
            b.IsStalemate.Should().Be(false, "Not a stalemate");
        }

        /// <summary>
        /// Checks if check detection is working correctly.
        /// </summary>
        [Fact]
        public void KingCheckBlack()
        {
            ChessBoard b = CreateBoardWithPositions(
                Pos("c2"), ChessPieceType.RookQueen, 1,
                Pos("c6"), ChessPieceType.King, 2,
                Pos("e1"), ChessPieceType.King, 1,
                Pos("g2"), ChessPieceType.Pawn, 1
            );

            b.ApplyMove(Move("g2", "g3")); // This is to make sure player is now Black (to switch players from White to Black).

            b.CurrentPlayer.Should().Be(2, "Current player should be Black.");

            var possMoves = b.GetPossibleMoves();

            possMoves.Count().Should().Be(6, "There should only be six moves available.");

            possMoves.Should().Contain(Move("c6", "b7"), "Should contain C6 to B7.")
                .And.Contain(Move("c6", "d7"), "Should contain C6 to D7.")
                .And.Contain(Move("c6", "b6"), "Should contain C6 to B6.")
                .And.Contain(Move("c6", "d6"), "Should contain C6 to D6.")
                .And.Contain(Move("c6", "d5"), "Should contain C6 to B5.")
                .And.Contain(Move("c6", "d5"), "Should contain C6 to D5.");

            b.IsCheck.Should().Be(true, "Black's king should be checked.");
            b.IsCheckmate.Should().Be(false, "Should NOT be checkmate.");
            b.IsStalemate.Should().Be(false, "Not a stalemate.");

        }

        /// <summary>
        /// Checks if pawn movement is working as intended.
        /// </summary>
        [Fact]
        public void PawnsPossibleMovesBlack() // (ID + 1) % 6 = 1 => Test Pawns
        {
            ChessBoard b = new ChessBoard();
            b.ApplyMove(Move("a2", "a3")); // Make sure we switch to player 2 (Black).
            var pawnsPosition = b.GetPositionsOfPiece(ChessPieceType.Pawn, 2);
            char c = 'a'; // Set up for later.
			bool movePawnsNow = false;
            foreach (BoardPosition pawns in pawnsPosition) {
                var pawn = b.GetPieceAtPosition(pawns);
                pawn.PieceType.Should().Be(ChessPieceType.Pawn, "This should be a pawn.");
                pawn.Player.Should().Be(2, "This should be Black's pawn.");
                var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
                var expectedMoves = GetMovesAtPosition(possMoves, pawns);
                c.Should().BeLessThan('i', "var c should not have gone past letter h. Check pawnPositions (including its size/contents).");
                expectedMoves.Should().HaveCount(2, "There should only be two moves in the initial state")
                    .And.Contain(Move(c + "7", c + "6"), "First move should be one space ahead")
                    .And.Contain(Move(c + "7", c + "5"), "Second move should be two spaces ahead.");
                b.ApplyMove(Move(c + "7", c + "6")); // Move Black's pawn forward one spot.
                if (movePawnsNow) {
					b.ApplyMove(Move(c + "2", c + "3")); // Dummy move to change player.
				}
				else {
					b.ApplyMove(Move(c + "1", c + "2")); // Dummy move to change player.
					movePawnsNow = true;
				}
                var newPossMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
                var newExpectedMoves = GetMovesAtPosition(newPossMoves, Pos(c + "6"));
                newExpectedMoves.Should().HaveCount(1, "There should only be one move...")
                    .And.Contain(Move(c + "6", c + "5"), "Move an only go forward one spot.");
                b.UndoLastMove(); // Undo dummy move.
                b.UndoLastMove(); // Undo pawn move.
                b.ApplyMove(Move(c + "7", c + "5")); // Move Black's pawn forward two spots now.
                b.ApplyMove(Move(c + "2", c + "3")); // Dummy move to change player.
                newPossMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
                newExpectedMoves = GetMovesAtPosition(newPossMoves, Pos(c + "5"));
                newExpectedMoves.Should().HaveCount(1, "There should only be one move...")
                    .And.Contain(Move(c + "5", c + "4"), "Move an only go forward one spot.");
                b.UndoLastMove(); // Undo dummy move.
                b.UndoLastMove(); // Undo pawn move.
                c++; // Go on to the next letter...
            }
        }

        /// <summary>
        /// Checks if pawn promotion is working.
        /// </summary>
        [Fact]
        public void PawnPromotionWhite()
        {
            ChessBoard b = CreateBoardWithPositions(
                Pos("a7"), ChessPieceType.Pawn, 1,
                Pos("b7"), ChessPieceType.Queen, 2,
                Pos("e8"), ChessPieceType.Queen, 1,
                Pos("f5"), ChessPieceType.Pawn, 1,
                Pos("f6"), ChessPieceType.Pawn, 2,
                Pos("g4"), ChessPieceType.Pawn, 1,
                Pos("g7"), ChessPieceType.Pawn, 2,
                Pos("h4"), ChessPieceType.Pawn, 1,
                Pos("h5"), ChessPieceType.King, 1,
                Pos("h6"), ChessPieceType.Pawn, 2,
                Pos("h7"), ChessPieceType.King, 2
            );

            var possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
            var expectedMoves = GetMovesAtPosition(possMoves, Pos("a7"));
            expectedMoves.Should().HaveCount(1, "There should only be one move.")
                .And.Contain(Move("a7", "a8"), "The only possible mve is a7 to a8.");

            b.CurrentPlayer.Should().Be(1, "Current player should be White.");

            b.ApplyMove(Move("a7", "a8"));

            var piece = b.GetPieceAtPosition(Pos("a8"));

            piece.PieceType.Should().Be(ChessPieceType.Pawn, "Should be a pawn still.");

            b.CurrentPlayer.Should().Be(1, "Current player should still be White.");

            possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
            expectedMoves = GetMovesAtPosition(possMoves, Pos("a8"));
            expectedMoves.Should().HaveCount(4, "There should be four moves.")
                .And.OnlyContain(m => ((ChessMove) m).MoveType == ChessMoveType.PawnPromote);

            b.ApplyMove(Move("(a8, Bishop)")); // Apply pawn promotion.

            piece = b.GetPieceAtPosition(Pos("a8"));

            piece.PieceType.Should().Be(ChessPieceType.Bishop, "Should be a bishop now.");

            b.CurrentPlayer.Should().Be(2, "Current player should be Black.");

            b.UndoLastMove();

            b.CurrentPlayer.Should().Be(1, "Current player should be White.");

            piece = b.GetPieceAtPosition(Pos("a8"));

            piece.PieceType.Should().Be(ChessPieceType.Pawn, "Should be a pawn now.");

            possMoves = b.GetPossibleMoves() as IEnumerable<ChessMove>;
            expectedMoves = GetMovesAtPosition(possMoves, Pos("a8"));
            expectedMoves.Should().HaveCount(4, "There should be four moves.")
                .And.OnlyContain(m => ((ChessMove)m).MoveType == ChessMoveType.PawnPromote);

        }

        /// <summary>
        /// Checks if attacking is working.
        /// </summary>
        [Fact]
        public void EasyPeasyAttackWhite()
        {
            ChessBoard b = CreateBoardFromMoves(new ChessMove[]
            {
                Move("d2", "d4"),
                Move("e7", "e5"),
            });

            var possMoves = b.GetPossibleMoves();
            possMoves.Should().Contain(Move("d4", "e5"), "Move from d4 to e5 should exist.");

            var blackPawn = b.GetPieceAtPosition(Pos("e5"));
            blackPawn.PieceType.Should().Be(ChessPieceType.Pawn, "This spot should contain a pawn.");
            blackPawn.Player.Should().Be(2, "This piece should be Black's.");

            b.ApplyMove(Move("d4", "e5")); // Attack the pawn (White's doing).

            var oldLocation = b.GetPieceAtPosition(Pos("d4"));
            oldLocation.PieceType.Should().Be(ChessPieceType.Empty, "This spot should now be empty.");

            var newLocation = b.GetPieceAtPosition(Pos("e5"));
            newLocation.PieceType.Should().Be(ChessPieceType.Pawn, "This spot should now contain a pawn.");
            newLocation.Player.Should().Be(1, "This piece should be White's.");

        }

        /// <summary>
        /// Buster test case.
        /// </summary>
        [Fact]
        public void DavidVanBuster()
        {
            ChessBoard b = new ChessBoard();

            Action act;

            act = () => b.GetPossibleMoves();
            act.ShouldNotThrow("No exceptions should be thrown for GetPossibleMoves()");

            act = () => b.GetThreatenedPositions(1);
            act.ShouldNotThrow("No exceptions should be thrown for GetThreatenedPositions()");

            act = () => b.GetPositionsOfPiece(ChessPieceType.Pawn, 1);
            act.ShouldNotThrow("No exceptions should be thrown for GetPositionsOfPiece()");

            act = () => b.GetPlayerAtPosition(Pos("a1"));
            act.ShouldNotThrow("No exceptions should be thrown for GetPlayerAtPosition()");

            act = () => b.GetPieceAtPosition(Pos("a1"));
            act.ShouldNotThrow("No exceptions should be thrown for GetPieceAtPosition()");

            act = () => b.GetPieceValue(ChessPieceType.Pawn);
            act.ShouldNotThrow("No exceptions should be thrown for GetPieceValue()");

            act = () => b.ApplyMove(Move("a2", "a3"));
            act.ShouldNotThrow("No exceptions should be thrown for ApplyMove()");

            act = () => b.UndoLastMove();
            act.ShouldNotThrow("No exceptions should be thrown for UndoLastMove()");

            act = () => b.PositionIsEmpty(Pos("a1"));
            act.ShouldNotThrow("No exceptions should be thrown for PositionIsEmpty()");

            act = () => b.PositionIsEnemy(Pos("a1"), 2);
            act.ShouldNotThrow("No exceptions should be thrown for PositionIsEnemy()");

            act = () => b.UndoLastMove();
            act.ShouldThrow<Exception>("No moves to undo!");

            b = new ChessBoard();

            var possMoves = b.GetPossibleMoves();
            possMoves.Should().NotBeNullOrEmpty("No empty moves or nulls allowed in initial state.")
                .And.OnlyHaveUniqueItems("Should not contain duplicate moves.");

            var threatened1 = b.GetThreatenedPositions(1);
            var threatened2 = b.GetThreatenedPositions(2);
            threatened1.Should().NotContain(threatened2, "Player 1's threatened positions should not contain Player 2's threatened positions.");
            threatened2.Should().NotContain(threatened1, "Player 2's threatened positions should not contain Player 1's threatened positions.");

        }

      [Fact]
      public void zz() {
         ChessBoard c = CreateBoardFromMoves(new ChessMove[]
         {
            Move("c2" , "c4"),
            Move("c7" , "c5"),
            Move("b2" , "b4"),
            Move("b7" , "b5"),
            Move("d2" , "d4"),
            Move("d7" , "d5"),
            Move("d1" , "a4"),
            Move("d8" , "a5"),
            Move("c1" , "f4"),
            Move("c8" , "f5"),
            Move("b1" , "a3"),
            Move("b8" , "a6")
         });
         var poss1 = c.GetPossibleMoves() as IEnumerable<ChessMove>;
         var castling = GetMovesAtPosition(poss1, Pos("e1"));
         castling.Should().Contain(Move("e1", "c1"), "King and rook both haven't moved and the spaces between them are clear to allow castling");
         ChessBoard b = CreateBoardFromMoves(new ChessMove[] {
                Move("g2", "g4"),
                Move("a7", "a6"),
                Move("g4", "g5"),
                Move("h7", "h5")
            });
         var poss = b.GetPossibleMoves() as IEnumerable<ChessMove>;
         var enpassant = GetMovesAtPosition(poss, Pos("g5"));
         enpassant.Should().Contain(Move("g5", "h6"), "Enemy pawn moved 2 spaces and fits criteria for en passant");
      }

   }
}
