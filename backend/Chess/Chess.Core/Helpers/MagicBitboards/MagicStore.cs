using Chess.Core.Exceptions;
using System.Text.Json;

namespace Chess.Core.Helpers.MagicBitboards;

public static class MagicsStore
{
    private const string BishopMagicNumbersFilePath = "Resources/MagicBitboards/magic_numbers_bishop.json";
    private const string RookMagicNumbersFilePath = "Resources/MagicBitboards/magic_numbers_rook.json";
    private const int ExpectedMagicNumbersCount = 64;

    private static readonly Dictionary<int, ulong> _magicNumbersBishop;
    private static readonly Dictionary<int, ulong> _magicNumbersRook;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };


    static MagicsStore()
    {
        _magicNumbersBishop = LoadMagicNumbers(BishopMagicNumbersFilePath);
        _magicNumbersRook = LoadMagicNumbers(RookMagicNumbersFilePath);
    }

    public static ulong GetMagicNumberValue(int squareIndex, bool isRook)
    {
        Dictionary<int, ulong> magicNumbers = isRook ? _magicNumbersRook : _magicNumbersBishop;

        return magicNumbers.TryGetValue(squareIndex, out ulong magicNumberValue)
            ? magicNumberValue
            : throw new MagicNumbersException($"Magic number for square {squareIndex} was not found");
    }

    public static Dictionary<int, ulong> LoadMagicNumbers(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new MagicNumbersException($"Magic numbers file was not found: {filePath}");
        }

        string json = File.ReadAllText(filePath);
        Dictionary<int, ulong>? dictionary = JsonSerializer.Deserialize<Dictionary<int, ulong>>(json)
            ?? throw new MagicNumbersException($"Magic numbers file is invalid: {filePath}");

        if (dictionary.Count != ExpectedMagicNumbersCount)
        {
            throw new MagicNumbersException(
                $"Magic numbers file must contain {ExpectedMagicNumbersCount} values, but contains: {dictionary.Count}: {filePath}");
        }

        return dictionary;
    }

    public static void SaveMagicNumbers(string filePath, Dictionary<int, ulong> magicNumbers)
    {
        string json = JsonSerializer.Serialize(magicNumbers, _jsonOptions);

        File.WriteAllText(filePath, json);
    }
}
