using System;
using System.Collections.Generic;
using System.Linq;
using ChessChallenge.API;

public class MyBot : IChessBot
{
    private readonly int[] _pieceValues = { 0, 100, 320, 330, 500, 900, 20000 };
    
    private Board _board;
    private int _white = -1;
    private int _positionCount;
    public Move Think(Board board, Timer timer)
    {
        _positionCount = 0;
        _board = board;
        
        
        if (board.IsWhiteToMove) 
            _white = 1;
        
        var x = NegaMax(6, -1000000, 1000000, new Move());
        
        Console.WriteLine($"{x.Item1} {_positionCount} {timer.MillisecondsElapsedThisTurn}");
        return x.Item2;
    }

    private Move[] OrderMove()
    {
        var moves = _board.GetLegalMoves();
        Span<int> scores = stackalloc int[moves.Length];
        
        for (var i = 0; i < moves.Length; i++)
        {
            var score = 0;
            if (moves[i].IsPromotion) score += 1000;
            if (moves[i].IsCapture)
            {
                score += 10 * _pieceValues[(int)moves[i].CapturePieceType] - _pieceValues[(int)moves[i].MovePieceType];
            }

            scores[i] = score;
        }
        
        for (var i = 0; i < moves.Length; i++)
        for (var j = 0; j < moves.Length - 1 - i; j++)
        {
            if (scores[j] < scores[j + 1])
            {
                (scores[j], scores[j + 1]) = (scores[j + 1], scores[j]);
                (moves[j], moves[j + 1]) = (moves[j + 1], moves[j]);
            }
        }
        
        return moves;
    }
    
    private (int, Move) NegaMax(int depth, int alpha, int beta, Move bestMove)
    {
        _positionCount++;
        
        
        if (depth == 0) return (Evaluate(), bestMove);

        var moves = OrderMove();
        
        if (moves.Length == 0) {
            if (_board.IsInCheck())
            {
                var mateScore = 1000000 - _board.PlyCount;
                return (-mateScore, bestMove);
            } 
            return (0, bestMove);
        }
        
        foreach (var move in moves)  
        {
            _board.MakeMove(move);
            
            var score = -NegaMax(depth - 1, -beta, -alpha, bestMove).Item1;
            
            //if (_board.IsInCheck()) score += 10;
            
            _board.UndoMove(move);

            if (score >= beta)
            {
                return (beta, bestMove);
            }
            if (score > alpha)
            {
                alpha = score;
                bestMove = move;
            }
        }
        return (alpha, bestMove);
    }
    
    private int Evaluate()
    {
        var sum = 0;
        
        for(var i = 1; i < 7; i++)
        {
            sum += (_board.GetPieceList((PieceType)i, true).Count - _board.GetPieceList((PieceType)i, false).Count) * _pieceValues[i];
        }
        
        return sum * _white;
    }
}