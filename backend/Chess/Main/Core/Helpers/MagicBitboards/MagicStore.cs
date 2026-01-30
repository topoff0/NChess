using Chess.Exceptions;
using Chess.Main.Core.Helpers.BitOperation;
using Newtonsoft.Json;

namespace Chess.Main.Core.Helpers.MagicBitboards
{
    public static class MagicsStore
    {
        private static readonly string filePathBishop = "magic_numbers_bishop.json";
        private static readonly string filePathRook = "magic_numbers_rook.json";

        private static readonly Dictionary<int, ulong> _magicNumbersBishop;
        private static readonly Dictionary<int, ulong> _magicNumbersRook;

        static MagicsStore()
        {
            _magicNumbersBishop = LoadOrGenerateMagicNumbers(filePathBishop, false);
            _magicNumbersRook = LoadOrGenerateMagicNumbers(filePathRook, true);
        }

        private static Dictionary<int, ulong> LoadOrGenerateMagicNumbers(string filePath, bool isRook)
        {
            try
            {
                return LoadMagicNumbers(filePath);
            }
            catch (MagicNumbersException ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");

                var magicNumbers = new Dictionary<int, ulong>();

                for (int square = 0; square < 64; square++)
                {
                    Console.WriteLine($"square: {square}");
                    ulong mask = isRook ? MagicBitboards.GenerateRookMask(square) 
                                        : MagicBitboards.GenerateBishopMask(square);
                    int relevantBits = BitHelper.BitsCount(mask);
                    ulong magic = MagicGenerator.FindMagicNumber(square, mask, relevantBits, isRook);
                    magicNumbers[square] = magic;
                }

                SaveMagicNumbers(filePath, magicNumbers);
                return magicNumbers;
            }
        }

        public static ulong GetMagicNumberValue(int squareIndex, bool isRook)
        {
            if (isRook)
            {
                return _magicNumbersRook.TryGetValue(squareIndex, out var magicNumberValue) ? magicNumberValue : 0;
            }
            else
            {
                return _magicNumbersBishop.TryGetValue(squareIndex, out var magicNumberValue) ? magicNumberValue : 0;
            }
        }

        public static void SaveMagicNumbers(string filePath, Dictionary<int, ulong> magicNumbers)
        {
            try
            {
                string json = JsonConvert.SerializeObject(magicNumbers, Formatting.Indented);
                using (FileStream fs = File.Create(filePath)) { }
                File.WriteAllText(filePath, json);
                Console.WriteLine($"[INFO] Magic numbers successfully saved into file: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error was occurred while saving magic numbers. Error message: {ex.Message}");
            }
        }

        public static Dictionary<int, ulong> LoadMagicNumbers(string filePath)
        {
            if (!File.Exists(filePath))
                throw new MagicNumbersException("File of this path was not found. An error occurred in the LoadMagicNumbers().");
            
            string json = File.ReadAllText(filePath);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<int, ulong>>(json);

            if (dictionary == null)
            {
                throw new MagicNumbersException("Deserialized data is null. An error occurred in the LoadMagicNumbers().");
            }

            return dictionary;
        }
    }
}