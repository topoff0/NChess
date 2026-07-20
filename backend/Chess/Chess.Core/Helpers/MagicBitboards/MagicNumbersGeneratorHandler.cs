using Chess.Core.Helpers.BitOperation;

namespace Chess.Core.Helpers.MagicBitboards;

public static class MagicNumbersGeneratorHandler
{
    public static Dictionary<int, ulong> Handle(bool isRook)
    {
        Dictionary<int, ulong> magicNumbers = [];

        for (int square = 0; square < 64; square++)
        {
            ulong mask = isRook
                ? MagicBitboards.GenerateRookMask(square)
                : MagicBitboards.GenerateBishopMask(square);

            int relevantBits = BitHelper.BitsCount(mask);
            ulong magic = MagicGenerator.FindMagicNumber(square, mask, relevantBits, isRook);

            magicNumbers[square] = magic;
        }

        return magicNumbers;
    }
}

