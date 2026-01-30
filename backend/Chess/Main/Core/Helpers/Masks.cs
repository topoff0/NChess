namespace Chess.Main.Core.Helpers
{
    public static class Masks
    {
        // Pawns
        public const ulong WhitePawnsStartingPosition = 0x00_00_00_00_00_00_FF_00;
        public const ulong BlackPawnsStartingPosition = 0x00_FF_00_00_00_00_00_00;

        // For all
        public const ulong NotAFile = 0x7F_7F_7F_7F_7F_7F_7F_7F;
        public const ulong NotHFile = 0xFE_FE_FE_FE_FE_FE_FE_FE;
        public const ulong NotABFile = 0x3F_3F_3F_3F_3F_3F_3F_3F;
        public const ulong NotAFFile = 0x7B_7B_7B_7B_7B_7B_7B_7B;
        public const ulong NotHFFile = 0xFA_FA_FA_FA_FA_FA_FA_FA;
        public const ulong NotGHFile = 0xFC_FC_FC_FC_FC_FC_FC_FC;

        public const ulong WhiteKingCastleMask = 0x00_00_00_00_00_00_00_02;
        public const ulong WhiteQueenCastleMask = 0x00_00_00_00_00_00_00_20;

        public const ulong BlackKingCastleMask = 0x02_00_00_00_00_00_00_00;
        public const ulong BlackQueenCastleMask = 0x20_00_00_00_00_00_00_00;
    }
}