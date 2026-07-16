import blackBishop from "@/features/game/assets/pieces/black/bishop.svg";
import blackKing from "@/features/game/assets/pieces/black/king.svg";
import blackKnight from "@/features/game/assets/pieces/black/knight.svg";
import blackPawn from "@/features/game/assets/pieces/black/pawn.svg";
import blackQueen from "@/features/game/assets/pieces/black/queen.svg";
import blackRook from "@/features/game/assets/pieces/black/rook.svg";
import whiteBishop from "@/features/game/assets/pieces/white/bishop.svg";
import whiteKing from "@/features/game/assets/pieces/white/king.svg";
import whiteKnight from "@/features/game/assets/pieces/white/knight.svg";
import whitePawn from "@/features/game/assets/pieces/white/pawn.svg";
import whiteQueen from "@/features/game/assets/pieces/white/queen.svg";
import whiteRook from "@/features/game/assets/pieces/white/rook.svg";
import type { FenPiece } from "@/features/game/lib/fen";

export const PieceImages: Record<FenPiece, string> = {
  p: blackPawn,
  r: blackRook,
  n: blackKnight,
  b: blackBishop,
  q: blackQueen,
  k: blackKing,
  P: whitePawn,
  R: whiteRook,
  N: whiteKnight,
  B: whiteBishop,
  Q: whiteQueen,
  K: whiteKing
};
