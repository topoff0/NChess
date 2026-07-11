export type FenPiece = "p" | "r" | "b" | "n" | "q" | "k" | "P" | "R" | "B" | "N" | "Q" | "K";

export type BoardSquare = {
  key: string;
  rank: number;
  file: number;
  square: number;
  piece: FenPiece | null;
  isLight: boolean;
};

export const InitialBoardFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

export const PieceSymbols: Record<FenPiece, string> = {
  p: "♟",
  r: "♜",
  n: "♞",
  b: "♝",
  q: "♛",
  k: "♚",
  P: "♙",
  R: "♖",
  N: "♘",
  B: "♗",
  Q: "♕",
  K: "♔"
};

const isFenPiece = (value: string): value is FenPiece => {
  return value in PieceSymbols;
};

export const toSquareIndex = (square: Pick<BoardSquare, "rank" | "file">): number => {
  return (7 - square.rank) * 8 + (7 - square.file);
};

export const parseFenBoard = (fen: string): BoardSquare[] => {
  const ranks = fen.split("/");

  return ranks.flatMap((rankValue, rankIndex) => {
    const squares: BoardSquare[] = [];
    let fileIndex = 0;

    for (const value of rankValue) {
      const emptySquares = Number(value);

      if (Number.isInteger(emptySquares) && emptySquares > 0) {
        for (let index = 0; index < emptySquares; index += 1) {
          squares.push({
            key: `${rankIndex}-${fileIndex}`,
            rank: rankIndex,
            file: fileIndex,
            square: toSquareIndex({ rank: rankIndex, file: fileIndex }),
            piece: null,
            isLight: (rankIndex + fileIndex) % 2 == 0
          });

          fileIndex += 1;
        }
        continue;
      }

      if (!isFenPiece(value)) {
        throw new Error("Invalid FEN board");
      }

      squares.push({
        key: `${rankIndex}-${fileIndex}`,
        rank: rankIndex,
        file: fileIndex,
        square: toSquareIndex({ rank: rankIndex, file: fileIndex }),
        piece: value,
        isLight: (rankIndex + fileIndex) % 2 == 0
      });

      fileIndex += 1;
    }

    return squares;
  });
};

export const InitialBoardSquares = parseFenBoard(InitialBoardFen);
