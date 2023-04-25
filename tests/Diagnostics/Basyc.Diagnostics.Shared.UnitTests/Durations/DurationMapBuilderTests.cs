using Basyc.Diagnostics.Shared;
using Basyc.Diagnostics.Shared.Durations;

namespace Basyc.MessageBus.Manager.Application.Tests.Durations;

public class DurationMapBuilderTests
{
    private static readonly ServiceIdentity testServiceIndentity = new("TestService");

    [Fact]
    public void When_BuildingEmpty_Should_BeEmpty()
    {
        var mapBuilder = new DurationMapBuilder(testServiceIndentity, "1");
        var durationMap = mapBuilder.Build();

        durationMap.Segments.Length.Should().Be(0);
        durationMap.TotalDuration.Should().BeCloseTo(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public void When_BuildWithoutStartCounting_Should_StartAutomatically()
    {
        var mapBuilder = new DurationMapBuilder(testServiceIndentity, "1");
        var durationMap = mapBuilder.Build();
        mapBuilder.HasStarted.Should().BeTrue();
        mapBuilder.StartTime.Should().NotBe(default);
    }

    [Fact]
    public void When_AddAndStartSegmentWithoutStartCounting_Should_StartAutomatically()
    {
        var mapBuilder = new DurationMapBuilder(testServiceIndentity, "1");
        _ = mapBuilder.StartNewSegment("segmentName");
        mapBuilder.HasStarted.Should().BeTrue();
        mapBuilder.StartTime.Should().NotBe(default);
    }

    [Fact]
    public void When_AddAndStartSegment_Should_HaveOneSegment()
    {
        var mapBuilder = new DurationMapBuilder(testServiceIndentity, "1");
        const string segment1Name = "segment1";
        _ = mapBuilder.StartNewSegment(segment1Name);
        var durationMap = mapBuilder.Build();
        durationMap.Segments.Length.Should().Be(1);
        var firstSegment = durationMap.Segments.First();
        firstSegment.EndTime.Should().NotBe(default);
        durationMap.TotalDuration.Should().NotBe(default);
        durationMap.TotalDuration.Should().Be(firstSegment.EndTime - firstSegment.StartTime);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    public void When_StartingMultipleSegment_Should_ContainAll(int numberOfSegments)
    {
        var durationMapBuilder = new DurationMapBuilder(testServiceIndentity, "1");

        for (var segmentIndex = 0; segmentIndex < numberOfSegments; segmentIndex++)
        {
            _ = durationMapBuilder.StartNewSegment("segmentName");
        }

        var durationMap = durationMapBuilder.Build();
        durationMap.Segments.Length.Should().Be(numberOfSegments);
        var totalSegmentsDuration = TimeSpan.FromSeconds(0);
        foreach (var segment in durationMap.Segments)
        {
            segment.EndTime.Should().NotBe(default);
            totalSegmentsDuration += segment.EndTime - segment.StartTime;
        }

        if (durationMap.Segments.Length > 0)
        {
            durationMapBuilder.EndTime.Should().BeOnOrAfter(durationMap.Segments.Max(x => x.EndTime));
        }
    }

    [Fact]
    public void AddAndStartSegment_Should_ReturnNewSegment()
    {
        var allSegments = new List<IDurationSegmentBuilder>();
        var mapBuilder = new DurationMapBuilder(testServiceIndentity, "1");

        for (var segmentIndex = 0; segmentIndex < 4; segmentIndex++)
        {
            var segment = mapBuilder.StartNewSegment("segmentName");
            allSegments.All(segmentFromHistory => ReferenceEquals(segmentFromHistory, segment) is false).Should().BeTrue();
            allSegments.Add(segment);
        }
    }

    [Theory]
    [InlineData(0, new int[] { })]
    [InlineData(4, new[] { 100, 150, 50, 150 })]
    [InlineData(2, new[] { 100, 150 })]
    public void When_AddingSegments_Should_TotalDurationBeSumOfAll(int numberOfSegments, int[] durationsMs)
    {
        durationsMs.Length.Should().Be(numberOfSegments);

        var mapBuilder = new DurationMapBuilder(testServiceIndentity, "1");

        for (var segmentIndex = 0; segmentIndex < numberOfSegments; segmentIndex++)
        {
            var segmentDuration = durationsMs[segmentIndex];
            var segment = mapBuilder.StartNewSegment("segmentName");
            //await Task.Delay(segmentDuration); //Not accurate
            Thread.Sleep(segmentDuration);
            segment.End();
        }

        var durationMap = mapBuilder.Build();
        durationMap.Segments.Length.Should().Be(numberOfSegments);
        durationMap.Segments.All(x => x.NestedSegments.Length == 0).Should().BeTrue();
        var totalSegmentsDuration = TimeSpan.FromMilliseconds(durationsMs.Sum());

        //https://stackoverflow.com/questions/31742521/accuracy-of-task-delay
        durationMap.TotalDuration.Should().BeCloseTo(totalSegmentsDuration, DurationTestsHelper.TaskDelayPrecision);
    }

    [Fact]
    public void StartingNested_With_StartTime_Before_Map_Start_Should_Throw()
    {
        var mapBuilder = new DurationMapBuilder(testServiceIndentity, "1");
        mapBuilder.Start();
        mapBuilder.Invoking(x => x.StartNewSegment("nested1", DateTimeOffset.UtcNow.AddMinutes(-1)))
            .Should()
            .Throw<Exception>();
    }
}
