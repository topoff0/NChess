namespace Chess.Main.Core.Helpers.BitOperation
{
    public static class BitHelper
    {
        public static int BitsCount(ulong value)
        {
            int count = 0;
            while(value != 0)
            {
                value &= value - 1;
                count++;
            }
            return count;
        }

        public static int GetFirstBitIndex(ulong value)
        {
            return System.Numerics.BitOperations.TrailingZeroCount(value);
        }

        public static List<int> SquareIndexesFromBitboard(ulong bitboard)
        {
            List<int> indexes = [];
            while(bitboard != 0)
            {
                int index = GetFirstBitIndex(bitboard);
                indexes.Add(index);
                bitboard &= ~(1UL << index);
            }
            return indexes;
        }

    }
}