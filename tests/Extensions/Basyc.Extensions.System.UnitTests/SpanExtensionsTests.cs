namespace Basyc.Extensions.System.UnitTests;

public class SpanExtensionsTests
{
    [Fact]
    public void Split_Should_Split()
    {
        string text = "ABC.ABC:ABC_ABC";
        char[][] splittedText = SpanExtensions.Split(text, '.').ToArray();
        splittedText.Should().HaveCount(2);
        splittedText.First().Should().HaveSameCount("ABC".ToArray());
        splittedText.Last().Should().HaveSameCount("ABC:ABC_ABC".ToArray());
    }
}
