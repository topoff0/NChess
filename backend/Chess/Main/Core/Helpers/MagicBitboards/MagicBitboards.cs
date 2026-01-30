using Chess.Main.Core.Helpers.BitOperation;

namespace Chess.Main.Core.Helpers.MagicBitboards
{
    public static class MagicBitboards
    {

        public struct MagicLookUpTable
        {
            public ulong MagicNumber;
            public ulong Mask;
            public int RelevantBits;
            public ulong[] AttackTable;
        }
        public static readonly MagicLookUpTable[] MagicBishopTable = new MagicLookUpTable[64];
        public static readonly MagicLookUpTable[] MagicRookTable = new MagicLookUpTable[64];

        static MagicBitboards()
        {
            for (int sq = 0; sq < 64; sq++)
            {
                // Initialize bishop lookup table
                ulong mask = GenerateBishopMask(sq);
                int relevantBits = BitHelper.BitsCount(mask);
                ulong magic = MagicsStore.GetMagicNumberValue(sq, false);

                int attackTableSize = 1 << relevantBits;
                ulong[] attackTable = new ulong[attackTableSize];

                ulong[] blockers = GenerateAllBlockerCombinations(mask);

                for (int i = 0; i < blockers.Length; i++)
                {
                    ulong attackIndex = (blockers[i] * magic) >> (64 - relevantBits);
                    ulong attacks = GenerateBishopAttacks(sq, blockers[i]);
                    attackTable[attackIndex] = attacks;
                }

                MagicBishopTable[sq] = new MagicLookUpTable
                {
                    MagicNumber = magic,
                    Mask = mask,
                    RelevantBits = relevantBits,
                    AttackTable = attackTable
                };

                // Initialize rook lookup table
                mask = GenerateRookMask(sq);
                relevantBits = BitHelper.BitsCount(mask);
                magic = MagicsStore.GetMagicNumberValue(sq, true);

                attackTableSize = 1 << relevantBits;
                attackTable = new ulong[attackTableSize];

                blockers = GenerateAllBlockerCombinations(mask);

                for (int i = 0; i < blockers.Length; i++)
                {
                    ulong attackIndex = (blockers[i] * magic) >> (64 - relevantBits);
                    attackTable[attackIndex] = GenerateRookAttacks(sq, blockers[i]);
                }

                MagicRookTable[sq] = new MagicLookUpTable
                {
                    MagicNumber = magic,
                    Mask = mask,
                    RelevantBits = relevantBits,
                    AttackTable = attackTable
                };
            }
        }

        public static ulong[] GenerateAllBlockerCombinations(ulong mask)
        {
            int bitsInMask = BitHelper.BitsCount(mask);
            int variations = 1 << bitsInMask; // 2^bitInMask possible variations

            ulong[] result = new ulong[variations];

            for (int i = 0; i < result.Length; i++)
            {
                ulong blocker = 0;
                int bitIndex = 0;
                
                for (ulong bit = 1; bit != 0; bit <<= 1)
                {
                    if ((mask & bit) != 0) // if blocker can be on this bit
                    {
                        if ((i & (1 << bitIndex)) != 0) // if blocker bit must be included in this combination
                        {
                            // Console.WriteLine($"BitIndex: {bitIndex}");
                            // Console.WriteLine($"bit: {bit}");
                            blocker |= bit;
                        }
                            
                        bitIndex++;
                    }
                }

                result[i] = blocker;
            }

            return result;
        }

        public static ulong GenerateBishopMask(int squareIndex)
        {
            ulong mask = 0UL;

            int rank = squareIndex / 8;
            int file = squareIndex % 8;

            // (↖)
            for (int r = rank + 1, f = file + 1; r < 7 && f < 7; r++, f++)
                mask |= 1UL << (r * 8 + f);

            // (↗)
            for (int r = rank + 1, f = file - 1; r < 7 && f > 0; r++, f--)
                mask |= 1UL << (r * 8 + f);

            // (↙)
            for (int r = rank - 1, f = file + 1; r > 0 && f < 7; r--, f++)
                mask |= 1UL << (r * 8 + f);

            // (↘)
            for (int r = rank - 1, f = file - 1; r > 0 && f > 0; r--, f--)
                mask |= 1UL << (r * 8 + f);

            return mask;
        }


        public static ulong GenerateRookMask(int squareIndex)
        {
            ulong mask = 0UL;
            int rank = squareIndex / 8;
            int file = squareIndex % 8;

            // Up
            for (int r = rank + 1; r < 7; r++)
                mask |= 1UL << (r * 8 + file);

            // Down
            for (int r = rank - 1; r > 0; r--)
                mask |= 1UL << (r * 8 + file);

            // Right
            for (int f = file + 1; f < 7; f++)
                mask |= 1UL << (rank * 8 + f);

            // Left
            for (int f = file - 1; f > 0; f--)
                mask |= 1UL << (rank * 8 + f);

            return mask;
        }

        public static ulong GenerateBishopAttacks(int squareIndex, ulong blocker)
        {
            ulong attack = 0UL;
            int rank = squareIndex / 8;
            int file = squareIndex % 8;

            // (↖)
            for (int r = rank + 1, f = file + 1; r < 8 && f < 8; r++, f++)
            {
                attack |= 1UL << (r * 8 + f);
                if ((blocker & (1UL << (r * 8 + f))) != 0) break; // blocker on target square
            }

            // (↗)
            for (int r = rank + 1, f = file - 1; r < 8 && f >= 0; r++, f--)
            {
                attack |= 1UL << (r * 8 + f);
                if ((blocker & (1UL << (r * 8 + f))) != 0) break; // blocker on target square
            }

            // (↙)
            for (int r = rank - 1, f = file + 1; r >= 0 && f < 8; r--, f++)
            {
                attack |= 1UL << (r * 8 + f);
                if ((blocker & (1UL << (r * 8 + f))) != 0) break; // blocker on target square
            }

            // (↘)
            for (int r = rank - 1, f = file - 1; r >= 0 && f >= 0; r--, f--)
            {
                attack |= 1UL << (r * 8 + f);
                if ((blocker & (1UL << (r * 8 + f))) != 0) break; // blocker on target square
            }

            return attack;
        }

        public static ulong GenerateRookAttacks(int squareIndex, ulong blocker)
        {
            ulong attack = 0UL;
            int rank = squareIndex / 8;
            int file = squareIndex % 8;

            // Up
            for (int r = rank + 1; r < 8; r++)
            {
                attack |= 1UL << (r * 8 + file);
                if ((blocker & (1UL << (r * 8 + file))) != 0) break; // blocker on target square
            }
            // Down
            for (int r = rank - 1; r >= 0; r--)
            {
                attack |= 1UL << (r * 8 + file);
                if ((blocker & (1UL << (r * 8 + file))) != 0) break; // blocker on target square
            }

            // Left
            for (int f = file + 1; f < 8; f++)
            {
                attack |= 1UL << (rank * 8 + f);
                if ((blocker & (1UL << (rank * 8 + f))) != 0) break; // blocker on target square
            }

            // Right
            for (int f = file - 1; f >= 0; f--)
            {
                attack |= 1UL << (rank * 8 + f);
                if ((blocker & (1UL << (rank * 8 + f))) != 0) break; // blocker on target square
            }

            return attack;
        }
    }
}