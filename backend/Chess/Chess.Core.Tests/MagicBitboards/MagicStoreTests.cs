using Chess.Core.Helpers.MagicBitboards;

namespace Chess.Core.Tests.MagicBitboards;

public sealed class MagicStoreTests
{
    [Fact]
    public void LoadMagicNumbers_WhenBishopFileExists_ReturnsAllSquareValues()
    {
        Dictionary<int, ulong> magicNumbers = MagicStore.LoadMagicNumbers(
            "Resources/MagicBitboards/magic_numbers_bishop.json");

        Assert.Equal(64, magicNumbers.Count);
        Assert.All(Enumerable.Range(0, 64), square => Assert.True(magicNumbers.ContainsKey(square)));
    }

    [Fact]
    public void LoadMagicNumbers_WhenRookFileExists_ReturnsAllSquareValues()
    {
        Dictionary<int, ulong> magicNumbers = MagicStore.LoadMagicNumbers(
            "Resources/MagicBitboards/magic_numbers_rook.json");

        Assert.Equal(64, magicNumbers.Count);
        Assert.All(Enumerable.Range(0, 64), square => Assert.True(magicNumbers.ContainsKey(square)));
    }
}
