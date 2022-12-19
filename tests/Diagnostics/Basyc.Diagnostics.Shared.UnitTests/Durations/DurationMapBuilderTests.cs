using Basyc.Diagnostics.Shared.Durations;

namespace Basyc.MessageBus.Manager.Application.Tests.Durations
{
    public class DurationMapBuilderTests
    {
        private static readonly ServiceIdentity TestServiceIndentity = new ServiceIdentity("TestService");

        [Fact]
        public void When_BuildingEmpty_Should_BeEmpty()
        {
            var mapBuilder = new DurationMapBuilder(TestServiceIndentity, "1");
            var durationMap = mapBuilder.Build();

            durationMap.Segments.Length.Should().Be(0);
            durationMap.TotalDuration.Should().BeCloseTo(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(1));
        }

        [Fact]
        public void When_BuildWithoutStartCounting_Should_StartAutomatically()
        {
            var mapBuilder = new DurationMapBuilder(TestServiceIndentity, "1");
            var durationMap = mapBuilder.Build();
            mapBuilder.HasStarted.Should().BeTrue();
            mapBuilder.StartTime.Should().NotBe(default(DateTimeOffset));
        }

        [Fact]
        public void When_AddAndStartSegmentWithoutStartCounting_Should_StartAutomatically()
        {
            var mapBuilder = new DurationMapBuilder(TestServiceIndentity, "1");
            _ = mapBuilder.StartNewSegment("segmentName");
            mapBuilder.HasStarted.Should().BeTrue();
            mapBuilder.StartTime.Should().NotBe(default(DateTimeOffset));
        }

        [Fact]
        public void When_AddAndStartSegment_Should_HaveOneSegment()
        {
            var mapBuilder = new DurationMapBuilder(TestServiceIndentity, "1");
            const string segment1Name = "segment1";
            _ = mapBuilder.StartNewSegment(segment1Name);
            var durationMap = mapBuilder.Build();
            durationMap.Segments.Length.Should().Be(1);
            DurationSegment firstSegment = durationMap.Segments.First();
            firstSegment.EndTime.Should().NotBe(default(DateTimeOffset));
            durationMap.TotalDuration.Should().NotBe(default(TimeSpan));
            durationMap.TotalDuration.Should().Be(firstSegment.EndTime - firstSegment.StartTime);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3)]
        public void When_StartingMultipleSegment_Should_ContainAll(int numberOfSegments)
        {
            var durationMapBuilder = new DurationMapBuilder(TestServiceIndentity, "1");

            for (int segmentIndex = 0; segmentIndex < numberOfSegments; segmentIndex++)
            {
                _ = durationMapBuilder.StartNewSegment("segmentName");
            }

            var durationMap = durationMapBuilder.Build();
            durationMap.Segments.Length.Should().Be(numberOfSegments);
            TimeSpan totalSegmentsDuration = TimeSpan.FromSeconds(0);
            foreach (var segment in durationMap.Segments)
            {
                segment.EndTime.Should().NotBe(default(DateTimeOffset));
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
            List<IDurationSegmentBuilder> allSegments = new List<IDurationSegmentBuilder>();
            var mapBuilder = new DurationMapBuilder(TestServiceIndentity, "1");

            for (int segmentIndex = 0; segmentIndex < 4; segmentIndex++)
            {
                var segment = mapBuilder.StartNewSegment("segmentName");
                allSegments.All(segmentFromHistory => object.ReferenceEquals(segmentFromHistory, segment) is false).Should().BeTrue();
                allSegments.Add(segment);
            }
        }

        [Theory]
        [InlineData(0, new int[] { })]
        [InlineData(2, new int[] { 100, 150 })]
        [InlineData(4, new int[] { 100, 150, 50, 150 })]
        public void When_AddingSegments_Should_TotalDurationBeSumOfAll(int numberOfSegments, int[] durationsMs)
        {
            durationsMs.Length.Should().Be(numberOfSegments);

            var mapBuilder = new DurationMapBuilder(TestServiceIndentity, "1");

            for (int segmentIndex = 0; segmentIndex < numberOfSegments; segmentIndex++)
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
            TimeSpan totalSegmentsDuration = TimeSpan.FromMilliseconds(durationsMs.Sum());

            //https://stackoverflow.com/questions/31742521/accuracy-of-task-delay
            durationMap.TotalDuration.Should().BeCloseTo(totalSegmentsDuration, DurationTestsHelper.TaskDelayPrecision);
        }

        [Fact]
        public void StartingNested_With_StartTime_Before_Map_Start_Should_Throw()
        {
            var mapBuilder = new DurationMapBuilder(TestServiceIndentity, "1");
            mapBuilder.Start();
            mapBuilder.Invoking(x => x.StartNewSegment("nested1", DateTimeOffset.UtcNow.AddMinutes(-1)))
                .Should()
                .Throw<Exception>();
        }
    }
}