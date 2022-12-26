namespace Basyc.Extensions.IO.UnitTests;
public class TemporaryFileTest
{
	[Fact]
	public void GetNewTempFileName_Should_GiveUniqueName()
	{
		var names = Enumerable.Range(0, 10).Select(x => Basyc.Extensions.IO.TemporaryFile.GetNewTempFilename());
		names.Should().OnlyHaveUniqueItems();
	}
}
