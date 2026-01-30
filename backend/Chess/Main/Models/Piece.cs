namespace Chess.Main.Models
{

    public static class Piece
    {
        // Pieces Types
        public const int None = 0;
        public const int Pawn = 1;
        public const int Knight = 2;
        public const int Bishop = 3;
        public const int Rook = 4;
        public const int Queen = 5;
        public const int King = 6;

        // Colors
        public const int White = 0;
        public const int Black = 8;

        // Pieces
        public const int WhitePawn = Pawn | White;
        public const int WhiteKnight = Knight | White;
        public const int WhiteBishop = Bishop | White;
        public const int WhiteRook = Rook | White;
        public const int WhiteQueen = Queen | White;
        public const int WhiteKing = King | White;

        public const int BlackPawn = Pawn | Black;
        public const int BlackKnight = Knight | Black;
        public const int BlackBishop = Bishop | Black;
        public const int BlackRook = Rook | Black;
        public const int BlackQueen = Queen | Black;
        public const int BlackKing = King | Black;

        // Bit Masks
        const int typeMask = 0b111;
        const int colorMask = 0b1000;

        public static int MakePiece(int pieceType, int pieceColor) => pieceType | pieceColor;

        public static int PieceType(int piece) => piece & typeMask;

        public static int PieceColor(int piece) => piece & colorMask;

        // if piece is none always return false
        public static bool isColor(int piece, int color) => (piece & colorMask) == color && piece != 0;

        public static bool isWhite(int piece) => isColor(piece, White);

        public static char GetSymbol(int piece)
        {
            int pieceType = PieceType(piece);
            char symbol = pieceType switch
            {
                Pawn => 'P',
                Knight => 'N',
                Bishop => 'B',
                Rook => 'R',
                Queen => 'Q',
                King => 'K',
                _ => ' '
            };

            symbol = isWhite(piece) ? symbol : char.ToLower(symbol);
            return symbol;
        }

        public static int GetPieceTypeFromSymbol(char symbol)
        {
            symbol = char.ToUpper(symbol);
            return symbol switch
            {
                'P' => Pawn,
                'N' => Knight,
                'B' => Bishop,
                'R' => Rook,
                'Q' => Queen,
                'K' => King,
                _ => None
            };
        }


    }
}