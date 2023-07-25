#define experimental
#define values

using ChessChallenge.API;
using System;

namespace ChessChallenge.Example {
    public class MyBot : IChessBot {
        int startDepth = 5;
        int bignumber = 999999999;
        int[] pieceValues = { 0, 100, 300, 310, 500, 900, 10000 };
        Move bestMove;
        Random randomizer = new();
        public Move Think(Board board, Timer timer){
            Move[] lmoves = board.GetLegalMoves();
            bestMove = lmoves[0];

            if(timer.MillisecondsRemaining < 220){
                bestMove = lmoves[randomizer.Next(lmoves.Length)];
            } else {
                int cl = -1;
                if(board.IsWhiteToMove) cl = 1;

                // aviod tokens like I care about them so much:
                #if values 
                    int evalForMove = Search(board, startDepth, cl, -bignumber, bignumber, Move.NullMove);
                    Console.WriteLine("{0}, eval: {1}", bestMove, evalForMove);
                #else
                    Search(board, startDepth, cl, -bignumber, bignumber, Move.NullMove);
                #endif
            }

            return bestMove;
        }

        int Search(Board board, int depth, int color, int a, int b, Move lastMove){

            if (board.IsDraw()){
                return 0;
            }
            
            if(depth == 0 || board.GetLegalMoves().Length == 0){
                return Evaluate(board, color, lastMove);
            }

            if(color == 1){
                int maxEval = -bignumber;
                foreach(Move move in board.GetLegalMoves()){
                    board.MakeMove(move);
                    int evaluation = Search(board, depth - 1, -1, a, b, move);
                    board.UndoMove(move);
                    int oldM = maxEval;
                    maxEval = Math.Max(maxEval, evaluation);
                    if(maxEval != oldM && depth == startDepth) bestMove = move;
                    
                    a = Math.Max(a, evaluation);
                    if (b <= a) break;

                }
                    return maxEval;
            } else {
                int minEval = bignumber;
                foreach(Move move in board.GetLegalMoves()){
                    board.MakeMove(move);
                    int evaluation = Search(board, depth - 1, 1, a, b, move);
                    board.UndoMove(move);
                    int oldM = minEval;
                    minEval = Math.Min(minEval, evaluation);
                    if(minEval != oldM && depth == startDepth) bestMove = move;
                    
                    b = Math.Min(b, evaluation);
                    if (b <= a) break;
                    
                }
                    return minEval;
            }
        }
        int Evaluate(Board board, int color, Move lastMove){
            int sum = 0;

            if (board.IsInCheckmate())
                return board.IsWhiteToMove ? -bignumber : bignumber;
            
            // got this from .augs
            for (int i = 0; ++i < 7;)
			    sum += (board.GetPieceList((PieceType)i, true).Count - board.GetPieceList((PieceType)i, false).Count) * pieceValues[i];

            if (board.IsInCheck())
			    sum += board.IsWhiteToMove ? -800 : 800;

            // Is in progress of being tested for effectiveness

            #if experimental
                if(board.SquareIsAttackedByOpponent(lastMove.StartSquare)){
                    sum += 200;
                }
            #endif

            return color * sum;
        }
    }
}
