using ChessChallenge.API;
using System;

namespace ChessChallenge.Example
{
    // A simple bot that can spot mate in one, and always captures the most valuable piece it can.
    // Plays randomly otherwise.
    public class EvilBot : IChessBot
    {
        ///*
        int[] pieceValues = { 0, 100, 320, 330, 500, 900, 20000 };

        private Board board;
        private int white = -1;
        private Move finalMove;

        public Move Think(Board board, Timer timer)
        {
            this.board = board;

            if (board.IsWhiteToMove)
                white = 1;

            var x = NegaMax(4);

            return finalMove;
        }

        private int NegaMax(int depth)
        {
            if (depth == 0) return Evaluate();

            var max = int.MinValue;

            foreach (var move in board.GetLegalMoves())
            {
                board.MakeMove(move);

                var score = -NegaMax(depth - 1);

                if (board.IsInCheckmate())
                {
                    score = 1000000 + depth * 1000;
                }

                board.UndoMove(move);

                if (score > max)
                {
                    max = score;
                    if (depth == 4) finalMove = move;
                }
            }

            return max;
        }

        private int Evaluate()
        {
            var sum = 0;

            for (var i = 1; i < 7; i++)
            {
                sum += (board.GetPieceList((PieceType)i, true).Count - board.GetPieceList((PieceType)i, false).Count) *
                       pieceValues[i];
            }

            return sum * white;
        }
        //*/
        
        /*
        int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };

        public Move Think(Board board, Timer timer)
        {
            Move[] allMoves = board.GetLegalMoves();

            // Pick a random move to play if nothing better is found
            Random rng = new();
            Move moveToPlay = allMoves[rng.Next(allMoves.Length)];
            int highestValueCapture = 0;

            foreach (Move move in allMoves)
            {
                // Always play checkmate in one
                if (MoveIsCheckmate(board, move))
                {
                    moveToPlay = move;
                    break;
                }

                // Find highest value capture
                Piece capturedPiece = board.GetPiece(move.TargetSquare);
                int capturedPieceValue = pieceValues[(int)capturedPiece.PieceType];

                if (capturedPieceValue > highestValueCapture)
                {
                    moveToPlay = move;
                    highestValueCapture = capturedPieceValue;
                }
            }

            return moveToPlay;
        }

        // Test if this move gives checkmate
        bool MoveIsCheckmate(Board board, Move move)
        {
            board.MakeMove(move);
            bool isMate = board.IsInCheckmate();
            board.UndoMove(move);
            return isMate;
        }
        //*/
    }
}