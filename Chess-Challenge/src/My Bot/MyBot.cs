using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ChessChallenge.API;

public class MyBot : IChessBot
{
    private readonly int[] _pieceValues = { 0, 100, 320, 330, 500, 900, 20000 };
    private readonly int[] _scores = new int[256];
    private readonly HashSet<(int, Move)> _killerMoves = new();
    private readonly HashSet<Move> _bestMoves = new();
    
    private Board _board;
    private int _white = -1;
    private int _positionCount;
    
    public Move Think(Board board, Timer timer)
    {
        _positionCount = 0;
        _board = board;
        
        if (board.IsWhiteToMove) 
            _white = 1;
        
        var x = AlphaBeta(6, -1000000, 1000000, new Move());
        
        Console.WriteLine($"{x.Item1} {_positionCount} {timer.MillisecondsElapsedThisTurn}");
        return x.Item2;
    }

    private Move[] OrderMove(int depth, bool captureOnly = false)
    {
        var moves = _board.GetLegalMoves(captureOnly);
        //var lastMove = new Move();
        
        Array.Clear(_scores);
        
        //if (_board.GameMoveHistory.Length != 0) lastMove =_board.GameMoveHistory[^1];
        
        for (var i = 0; i < moves.Length; i++)
        {
            /*
            var pawnAttacks = BitboardHelper.GetPawnAttacks(moves[i].TargetSquare, _board.IsWhiteToMove);
            var attackCount = BitboardHelper.GetNumberOfSetBits(pawnAttacks);
            
            for (var j = 0; j < attackCount; j++)
            {
                var index = BitboardHelper.ClearAndGetIndexOfLSB(ref pawnAttacks);
                if (_board.GetPiece(new Square(index)).IsPawn) _scores[i] -= 1000;
            }
            */
            if (moves[i].IsPromotion) _scores[i] += 900;
            if (moves[i].IsCapture)
            {
                //if (lastMove.TargetSquare == moves[i].TargetSquare) _scores[i] += 100 * _pieceValues[(int)moves[i].CapturePieceType] - _pieceValues[(int)moves[i].MovePieceType];
                //if (_killerMoves.Contains((depth, moves[i]))) _scores[i] += 1000;
                //if (_bestMoves.Contains(moves[i])) _scores[i] += 1000;
                _scores[i] += 10 * _pieceValues[(int)moves[i].CapturePieceType] - _pieceValues[(int)moves[i].MovePieceType];
            }
        }

        bool swapRequired;
        for (var i = 0; i < moves.Length - 1; i++) 
        {
            swapRequired = false;
            for (var j = 0; j < moves.Length - i - 1; j++)
                if (_scores[j] < _scores[j + 1])
                {
                    (_scores[j], _scores[j + 1]) = (_scores[j + 1], _scores[j]);
                    (moves[j], moves[j + 1]) = (moves[j + 1], moves[j]);
                    swapRequired = true;
                }
            if (swapRequired == false)
                break;
        }

        return moves;
    }
    
    private (int, Move) AlphaBeta(int depth, int alpha, int beta, Move bestMove)
    {
        if (depth == 0) return (Evaluate(), bestMove);
        var moves = OrderMove(depth);
        
        if (moves.Length == 0) {
            if (_board.IsInCheck())
            {
                return (-1000000 + _board.PlyCount, bestMove);
            } 
            return (0, bestMove);
        }

        for (var i = 0; i < moves.Length; i++)
        {
            _board.MakeMove(moves[i]);
            
            var score = -AlphaBeta(depth - 1, -beta, -alpha, bestMove).Item1;
            
            _board.UndoMove(moves[i]);

            if (score >= beta)
            {
                //_killerMoves.Add((depth,moves[i]));
                return (beta, bestMove);
            }
            
            if (score > alpha)
            {
                alpha = score;
                bestMove = moves[i];
            }
        }

        //_bestMoves.Add(bestMove);
        return (alpha, bestMove);
    }
    
    private int Evaluate()
    {
        _positionCount++;
        var sum = 0;
        
        for(var i = 1; i < 7; i++) 
            sum += (_board.GetPieceList((PieceType)i, true).Count - _board.GetPieceList((PieceType)i, false).Count) * _pieceValues[i];
        
        return sum * _white;
    }
}