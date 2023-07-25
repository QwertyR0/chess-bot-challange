using ChessChallenge.API;
using System;

namespace ChessChallenge.Example {
    public class MyBot : IChessBot {
        int startDepth = 5;
        int bignumber = 999999999;
        int[] pieceValues = { 0, 100, 300, 310, 500, 900, 10000 };
        Move bestMove;
        public Move Think(Board board, Timer timer){
            bestMove = board.GetLegalMoves()[0];
            int cl = -1;

            if(board.IsWhiteToMove) cl = 1;

            Search(board, startDepth, cl, -bignumber, bignumber);
            Console.WriteLine("{0}", bestMove);

            return bestMove;
        }

        int Search(Board board, int depth, int color, int a, int b){

            if (board.IsDraw()){
                return 0;
            }
            
            if(depth == 0 || board.GetLegalMoves().Length == 0){
                return Evaluate(board, color);
            }

            if(color == 1){
                int maxEval = -bignumber;
                foreach(Move move in board.GetLegalMoves()){
                    board.MakeMove(move);
                    int evaluation = Search(board, depth - 1, -1, a, b);
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
                    int evaluation = Search(board, depth - 1, 1, a, b);
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
        int Evaluate(Board board, int color){
			int sum = 0;

			if (board.IsInCheckmate())
				return board.IsWhiteToMove ? -bignumber : bignumber;

            // got this from .augs
			for (int i = 0; ++i < 7;)
				sum += (board.GetPieceList((PieceType)i, true).Count - board.GetPieceList((PieceType)i, false).Count) * pieceValues[i];

            if (board.IsInCheck())
				sum += board.IsWhiteToMove ? -800 : 800;

			return color * sum;
        }
    }
}
