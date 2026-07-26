using Chess.Core.Exceptions;
using System.Text.Json;

namespace Chess.Core.Helpers.MagicBitboards;

public static class MagicStore
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

    static MagicStore()
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
        string resolvedFilePath = ResolveMagicNumbersFilePath(filePath);

        if (!File.Exists(resolvedFilePath))
        {
            throw new MagicNumbersException($"Magic numbers file was not found: {resolvedFilePath}");
        }

        string json = File.ReadAllText(resolvedFilePath);
        Dictionary<int, ulong>? dictionary = JsonSerializer.Deserialize<Dictionary<int, ulong>>(json)
            ?? throw new MagicNumbersException($"Magic numbers file is invalid: {resolvedFilePath}");

        if (dictionary.Count != ExpectedMagicNumbersCount)
        {
            throw new MagicNumbersException(
                $"Magic numbers file must contain {ExpectedMagicNumbersCount} values, but contains: {dictionary.Count}: {resolvedFilePath}");
        }

        return dictionary;
    }

    public static void SaveMagicNumbers(string filePath, Dictionary<int, ulong> magicNumbers)
    {
        string json = JsonSerializer.Serialize(magicNumbers, _jsonOptions);

        File.WriteAllText(filePath, json);
    }

    private static string ResolveMagicNumbersFilePath(string filePath)
    {
        if (Path.IsPathRooted(filePath) || File.Exists(filePath))
        {
            return filePath;
        }

        return Path.Combine(AppContext.BaseDirectory, filePath);
    }
}
