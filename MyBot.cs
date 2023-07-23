using ChessChallenge.API;
using System;

public class MyBot : IChessBot{

    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
    // string status = "none";

    public Move Think(Board board, Timer timer){

        Move[] legalMoves = board.GetLegalMoves();
        System.Random rng = new();
        Move selectedMove = legalMoves[rng.Next(legalMoves.Length)];

        int highestValueCapture = 0;

        foreach (Move move in legalMoves){
            /*
                the problem with this is that if the move that draws the game
                comes first in the array while it also having a checkmate move,
                it will draw the game without checking for other stuf more valuable.

                It is the next task to do.
            */
            if (MoveIsDraw(board, move)) {
                selectedMove = move;
                break;
            } else if (MoveIsDraw(board, move)) {
                selectedMove = move;
                break;
            } else if (move.IsPromotion) {
                if ((int)move.PromotionPieceType == 5) {
                    Console.WriteLine("prom5");
                    selectedMove = move;
                    break;
                };
            } else if (move.IsCastles && board.PlyCount / 2 < 8) {
                Console.WriteLine("castles at {0}", board.PlyCount);
                selectedMove = move;
                break;
            }

            Piece capturedPiece = board.GetPiece(move.TargetSquare);
            int capturedPieceValue = pieceValues[(int)capturedPiece.PieceType];
            if (capturedPieceValue != 0) {
                Console.WriteLine("test {0}", (int)capturedPiece.PieceType);
            }

            if (capturedPieceValue > highestValueCapture){
                selectedMove = move;
                highestValueCapture = capturedPieceValue;
            }
        }

        if (selectedMove.MovePieceType == PieceType.King && board.PlyCount/2 <= 4){
            selectedMove = legalMoves[rng.Next(legalMoves.Length)];
        }

        return selectedMove;
    }

    bool MoveIsCheckmateOrDraw(Board board, Move move){
        board.MakeMove(move);
        bool isMate = board.IsInCheckmate();
        board.UndoMove(move);
        return isMate;
    }

    bool MoveIsDraw(Board board, Move move)
    {
        board.MakeMove(move);
        bool isDraw = board.IsDraw();
        board.UndoMove(move);
        return isDraw;
    }
}