using Chess.Core.Common;

namespace Chess.Core.Tests.MoveNotation;

public sealed class MoveNotationTests
{
    [Fact]
    public void ApplyEndGameNotation_WhenCheckmateNotationAlreadyHasCheck_ReplacesCheckWithCheckmate()
    {
        string result = Core.MoveNotation.MoveNotation.ApplyEndGameNotation("Qh5+", GameCondition.Lose);

        Assert.Equal("Qh5#", result);
    }

    [Fact]
    public void ApplyEndGameNotation_WhenCheckmateNotationHasNoCheck_AppendsCheckmate()
    {
        string result = Core.MoveNotation.MoveNotation.ApplyEndGameNotation("Qh5", GameCondition.Lose);

        Assert.Equal("Qh5#", result);
    }

    [Fact]
    public void ApplyEndGameNotation_WhenDraw_AppendsDrawNotation()
    {
        string result = Core.MoveNotation.MoveNotation.ApplyEndGameNotation("Qh5", GameCondition.Draw);

        Assert.Equal("Qh5 1/2-1/2", result);
    }

    [Fact]
    public void ApplyEndGameNotation_WhenDrawNotationAlreadyExists_DoesNotDuplicateDrawNotation()
    {
        string result = Core.MoveNotation.MoveNotation.ApplyEndGameNotation("Qh5 1/2-1/2", GameCondition.Draw);

        Assert.Equal("Qh5 1/2-1/2", result);
    }
}
