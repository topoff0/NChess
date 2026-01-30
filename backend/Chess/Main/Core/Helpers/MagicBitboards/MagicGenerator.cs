using Chess.Main.Core.Helpers.BitOperation;

namespace Chess.Main.Core.Helpers.MagicBitboards
{
    public class MagicGenerator
    {
        private static readonly Random rand = new Random();

        public static ulong FindMagicNumber(int square, ulong mask, int relevantBits, bool isRook)
        {
            while(true)
            {
                ulong magic = GetRandomU64();
                if (BitHelper.BitsCount((mask * magic) & 0xFF_00_00_00_00_00_00_00UL) < 6)
                    continue;
                if (isMagicValid(square, magic, mask, relevantBits, isRook))
                {
                    return magic;
                }
            }
        }


        private static ulong GetRandomU64()
        {
            return (ulong)rand.NextInt64() & (ulong)rand.NextInt64() & (ulong)rand.NextInt64();
        }

        private static bool isMagicValid(int square, ulong magic, ulong mask, int relevantBits, bool isRook)
        {
            ulong[] occupancies = MagicBitboards.GenerateAllBlockerCombinations(mask); // all blockers combinations
            ulong[] attacks = new ulong[occupancies.Length]; // all possible attacks
            ulong[] usedAttacks = new ulong[1 << relevantBits];

            for (int i = 0; i < occupancies.Length; i++)
            {
                // set i-th attack for i-th blocker combinations and piece on chosen square
                attacks[i] = isRook
                 ? MagicBitboards.GenerateRookAttacks(square, occupancies[i])
                 : MagicBitboards.GenerateBishopAttacks(square, occupancies[i]);
            }

            for (int i = 0; i < occupancies.Length; i++)
            {
                ulong index = (occupancies[i] * magic) >> (64 - relevantBits);

                if (usedAttacks[index] == 0)
                    usedAttacks[index] = attacks[i]; // set hash for this attack
                else if (usedAttacks[index] != attacks[i]) // one hash - 2 different attacks (wrong)
                    return false;
            }

            return true;
        }

    }
}