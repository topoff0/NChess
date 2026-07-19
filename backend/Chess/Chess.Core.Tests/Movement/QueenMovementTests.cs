using Chess.Core.FEN;
using Chess.Core.Helpers.Squares;
using Chess.Core.Movement.Generator;
using Chess.Core.Models;

namespace Chess.Core.Tests.Movement;

public sealed class QueenMovementTests
{
    [Fact]
    public void Generate_WhenWhiteQueenIsInInitialPosition_ReturnsNoMoves()
    {
        Board board = new();

        ulong moves = QueenMovement.Generate(Square("d1"), board, isWhiteTurn: true);

        Assert.Equal(0UL, moves);
    }

    [Fact]
    public void Generate_WhenWhiteQueenIsOnOpenBoard_ReturnsRookAndBishopMoves()
    {
        Board board = FenUtility.LoadBoardFromFen("8/8/8/8/3Q4/8/8/8 w - - 0 1");

        ulong moves = QueenMovement.Generate(Square("d4"), board, isWhiteTurn: true);

        AssertMoves(
            moves,
            "a4", "b4", "c4", "e4", "f4", "g4", "h4",
            "d1", "d2", "d3", "d5", "d6", "d7", "d8",
            "a1", "b2", "c3", "e5", "f6", "g7", "h8",
            "a7", "b6", "c5", "e3", "f2", "g1");
    }

    [Fact]
    public void Generate_WhenQueenHasAlliedAndOpponentBlockers_StopsAtBlockersAndCanCaptureOpponent()
    {
        Board board = FenUtility.LoadBoardFromFen("8/8/1p6/2P5/3Q1p2/8/3P4/8 w - - 0 1");

        ulong moves = QueenMovement.Generate(Square("d4"), board, isWhiteTurn: true);

        AssertMoves(
            moves,
            "a4", "b4", "c4", "e4", "f4",
            "d3", "d5", "d6", "d7", "d8",
            "a1", "b2", "c3", "e5", "f6", "g7", "h8",
            "e3", "f2", "g1");
    }

    private static int Square(string square)
    {
        return SquaresHelper.StringSquareToSquareIndex[square];
    }

    private static void AssertMoves(ulong actualMoves, params string[] expectedSquares)
    {
        ulong expectedMoves = expectedSquares.Aggregate(0UL, (moves, square) => moves | (1UL << Square(square)));

        Assert.Equal(expectedMoves, actualMoves);
    }
}
