import { PieceImages } from "@/features/game/assets/pieces/pieceImages";
import { parseFenBoard } from "@/features/game/lib/fen";
import { useMemo } from "react";

type ChessBoardProps = {
  fen: string;
  legalMoves: Record<string, number[]>;
  selectedSquare: number | null;
  onSquareClick: (square: number) => void;
};

export const ChessBoard = ({ fen, legalMoves, selectedSquare, onSquareClick }: ChessBoardProps) => {
  const board = useMemo(() => {
    try {
      return parseFenBoard(fen.split(" ")[0]);
    } catch {
      return null;
    }
  }, [fen]);

  const legalTargets = selectedSquare !== null ? (legalMoves[selectedSquare] ?? []) : [];

  if (!board) {
    return <p className="text-sm font-bold text-red-700">Received an invalid board position</p>;
  }
  return (
    <div
      className="mx-auto grid aspect-square
                    w-full max-w-xl grid-cols-8
                    overflow-hidden rounded-2xl
                    border-4 border-wood-dark">
      {board.map((square) => {
        const isSelected = square.square === selectedSquare;
        const isLegalTarget = legalTargets.includes(square.square);
        const isPawn = square.piece?.toLowerCase() === "p";

        return (
          <button
            className={`relative flex aspect-square items-center justify-center text-3xl font-bold sm:text-5xl
            ${square.isLight ? "bg-cream" : "bg-wood"}
            ${isSelected ? "outline outline-4 -outline-offset-4 outline-moss" : ""}`}
            key={square.key}
            type="button"
            onClick={() => onSquareClick(square.square)}>
            {square.piece && (
              <img
                className={`${isPawn ? "h-[65%] w-[65%]" : "h-[86%] w-[86%]"} object-contain`}
                src={PieceImages[square.piece]}
                alt=""
                draggable={false}
              />
            )}
            {isLegalTarget && <span aria-hidden="true" className="absolute h-3 w-3 rounded-full bg-moss/70" />}
          </button>
        );
      })}
    </div>
  );
};
