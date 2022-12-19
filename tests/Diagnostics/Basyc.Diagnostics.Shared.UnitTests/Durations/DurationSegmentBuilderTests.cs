using Basyc.Diagnostics.Shared.Durations;

namespace Basyc.MessageBus.Manager.Application.Tests.Durations
{
	public class DurationSegmentBuilderTests
	{
		private static ServiceIdentity serviceIdentity = new ServiceIdentity("test");

		[Fact]
		public void When_BuildingEmpty_Should_BeEmpty()
		{
			const string segmentName = "segmentName";
			var segmentBuilder = new InMemoryDurationSegmentBuilder(serviceIdentity, "1", "1", segmentName);
			segmentBuilder.Start();
			segmentBuilder.End();
			segmentBuilder.HasEnded.Should().BeTrue();
			var segment = segmentBuilder.Build();

			segment.NestedSegments.Length.Should().Be(0);
			segment.StartTime.Should().NotBe(default(DateTimeOffset));
			segment.EndTime.Should().NotBe(default(DateTimeOffset));
		}

		[Theory]
		[InlineData(0, 0)]
		[InlineData(1, 0)]
		[InlineData(1, 1)]
		[InlineData(3, 0)]
		[InlineData(3, 3)]
		[InlineData(2, 4)]
		[InlineData(4, 2)]
		public void When_Building_Should_FinishEvenNested(uint nestedSegmentsNumber, uint levelOfNesting)
		{
			InMemoryDurationSegmentBuilder? rootSegmentBuilder = CreateNesting(nestedSegmentsNumber, levelOfNesting, out var allBuilders);
			var rootSegment = rootSegmentBuilder.Build();
			allBuilders.All(x => x.HasEnded).Should().BeTrue();
		}

		private InMemoryDurationSegmentBuilder CreateNesting(uint nestedSegmentsNumber, uint levelOfNestingInNestedSegments, out List<IDurationSegmentBuilder> allBuilders)
		{
			allBuilders = new List<IDurationSegmentBuilder>();
			const string segmentName = "segmentName";
			var rootSegmentBuilder = new InMemoryDurationSegmentBuilder(serviceIdentity, "1", "1", "root");
			rootSegmentBuilder.Start();
			allBuilders.Add(rootSegmentBuilder);

			for (int rootNestedSegmentIndex = 0; rootNestedSegmentIndex < nestedSegmentsNumber; rootNestedSegmentIndex++)
			{
				var nestedSegment = rootSegmentBuilder.StartNested(segmentName);
				allBuilders.Add(nestedSegment);

				for (int levelOfNestingIndex = 0; levelOfNestingIndex < levelOfNestingInNestedSegments; levelOfNestingIndex++)
				{
					nestedSegment = nestedSegment.StartNested(segmentName);
					allBuilders.Add(nestedSegment);
				}
			}
			uint levelOfNesteding = levelOfNestingInNestedSegments + 1;
			((uint)allBuilders.Count).Should().Be(1 + nestedSegmentsNumber * levelOfNesteding);

			return rootSegmentBuilder;
		}

		[Fact]
		public void EndAndStart_Should_HaveSameEndAndStarTimes()
		{
			var rootSegmentBuilder = new InMemoryDurationSegmentBuilder(serviceIdentity, "1", "1", "root");
			rootSegmentBuilder.Start();
			var nestedSegmentBuilder1 = rootSegmentBuilder.StartNested("nested1");
			var nestedSegmentBuilder2 = nestedSegmentBuilder1.EndAndStartFollowing("nested2");

			nestedSegmentBuilder1.HasEnded.Should().BeTrue();
			nestedSegmentBuilder2.HasEnded.Should().BeFalse();

			nestedSegmentBuilder1.EndTime.Should().Be(nestedSegmentBuilder2.StartTime);
			nestedSegmentBuilder2.EndTime.Should().Be(default(DateTimeOffset));

			var rootSegment = rootSegmentBuilder.Build();
			rootSegment.NestedSegments.Length.Should().Be(2);
			rootSegment.NestedSegments.All(x => x.NestedSegments.Length == 0).Should().BeTrue();
		}

		[Theory]
		[InlineData(1)]
		[InlineData(3)]
		[InlineData(2)]
		[InlineData(4)]
		public void StartNested_Should_AddNested(int nestedSegmentsNumber)
		{
			var rootSegmentBuilder = new InMemoryDurationSegmentBuilder(serviceIdentity, "1", "1", "root");
			rootSegmentBuilder.Start();
			for (int segmentIndex = 0; segmentIndex < nestedSegmentsNumber; segmentIndex++)
			{
				rootSegmentBuilder.StartNested("nestedSegment");
			}

			var rootSegment = rootSegmentBuilder.Build();
			rootSegment.NestedSegments.Length.Should().Be(nestedSegmentsNumber);
		}


		[Theory]
		[InlineData(1)]
		[InlineData(3)]
		[InlineData(2)]
		[InlineData(4)]
		public void EndAndStart_Should_AddNestedToTheParent(int nestedSegmentsNumber)
		{
			var rootSegmentBuilder = new InMemoryDurationSegmentBuilder(serviceIdentity, "1", "1", "root");
			rootSegmentBuilder.Start();
			var nestedSegment = rootSegmentBuilder.StartNested("nestedFollowing0");
			for (int segmentIndex = 1; segmentIndex < nestedSegmentsNumber; segmentIndex++)
			{
				var previousSegmentBuilder = nestedSegment;
				nestedSegment = nestedSegment.EndAndStartFollowing($"nestedFollowing{segmentIndex}");
				previousSegmentBuilder.HasEnded.Should().BeTrue();
				nestedSegment.HasEnded.Should().BeFalse();
			}

			var rootSegment = rootSegmentBuilder.Build();
			rootSegment.NestedSegments.Length.Should().Be(nestedSegmentsNumber);
		}

		[Fact]
		public void When_DoesNotHaveParent_Should_ThrowWhenEndingAndStartingNew()
		{
			var rootSegmentBuilder = new InMemoryDurationSegmentBuilder(serviceIdentity, "1", "1", "root");
			rootSegmentBuilder.Start();
			Assert.Throws<InvalidOperationException>(() => rootSegmentBuilder.EndAndStartFollowing("followingSegment"));
		}

		[Theory]
		[InlineData(1)]
		[InlineData(3)]
		[InlineData(2)]
		[InlineData(4)]
		public void StartNested_When_NotEndingPrevious_Should_BeInParrarel(int segmentsInParrarel)
		{
			var rootSegmentBuilder = new InMemoryDurationSegmentBuilder(serviceIdentity, "1", "1", "root");
			rootSegmentBuilder.Start();
			var nestedSegmentBuilder = rootSegmentBuilder.StartNested("nestedSegment");

			for (int segmentIndex = 0; segmentIndex < segmentsInParrarel - 1; segmentIndex++)
			{
				var previous = nestedSegmentBuilder;
				nestedSegmentBuilder = rootSegmentBuilder.StartNested("nestedSegment");
				nestedSegmentBuilder.HasEnded.Should().BeFalse();
				previous.HasEnded.Should().BeFalse();
				Thread.Sleep(150);
			}

			var rootSegment = rootSegmentBuilder.Build();
			rootSegment.NestedSegments.Length.Should().Be(segmentsInParrarel);
			rootSegment.NestedSegments.DistinctBy(x => x.StartTime).Count().Should().Be(rootSegment.NestedSegments.Length);
			rootSegment.NestedSegments.DistinctBy(x => x.EndTime).Count().Should().Be(1);
		}

		[Fact]
		public void StartNewNested_Should_StartParent_With_Same_StartTime()
		{
			var rootSegmentBuilder = new InMemoryDurationSegmentBuilder(serviceIdentity, "1", "1", "root");
			var nestedSegmentBuilder1 = rootSegmentBuilder.StartNested("nested1");

			rootSegmentBuilder.HasStarted.Should().BeTrue();

			var rootSegment = rootSegmentBuilder.Build();
			rootSegment.NestedSegments.Length.Should().Be(1);
			rootSegment.NestedSegments.First().StartTime.Should().Be(rootSegment.StartTime);
		}

		[Fact]
		public void Build_Should_Set_StartTime_When_Not_Started()
		{
			var rootSegmentBuilder = new InMemoryDurationSegmentBuilder(serviceIdentity, "1", "1", "root");
			var rootSegment = rootSegmentBuilder.Build();
			rootSegmentBuilder.HasStarted.Should().BeTrue();
			rootSegmentBuilder.StartTime.Should().NotBe(default);
			rootSegment.StartTime.Should().NotBe(default);
		}

		[Fact]
		public void Build_ShouldNot_Set_StartTime_When_AleradyStarted()
		{
			var rootSegmentBuilder = new InMemoryDurationSegmentBuilder(serviceIdentity, "1", "1", "root");
			var rootbuilderStartTime = rootSegmentBuilder.Start();
			var nestedSegmentBuilder1 = rootSegmentBuilder.StartNested("nested1");
			var beforeBuildTime = DateTimeOffset.UtcNow;
			var rootSegment = rootSegmentBuilder.Build();
			rootSegmentBuilder.StartTime.Should().Be(rootbuilderStartTime);
			rootSegment.StartTime.Should().Be(rootbuilderStartTime);

			nestedSegmentBuilder1.StartTime.Should().BeBefore(beforeBuildTime);
			var nestedSegment = rootSegment.NestedSegments.First();
			nestedSegment.StartTime.Should().BeBefore(beforeBuildTime);
		}

		[Fact]
		public void BuildWithStartTime_Should_Set_StartTime()
		{
			var rootSegmentBuilder = new InMemoryDurationSegmentBuilder(serviceIdentity, "1", "1", "root");
			var startTime = DateTimeOffset.UtcNow;
			var rootSegment = rootSegmentBuilder.Build(startTime);
			rootSegmentBuilder.HasStarted.Should().BeTrue();
			rootSegmentBuilder.StartTime.Should().Be(startTime);
			rootSegment.StartTime.Should().Be(startTime);
		}

		[Fact]
		public void BuildWithStartTime_ShouldNot_Set_StartTime_When_AleradyStarted()
		{
			var rootSegmentBuilder = new InMemoryDurationSegmentBuilder(serviceIdentity, "1", "1", "root");
			var startTime = DateTimeOffset.UtcNow;
			var rootbuilderStartTime = rootSegmentBuilder.Start();
			var nestedSegmentBuilder1 = rootSegmentBuilder.StartNested("nested1");

			var beforeBuildTime = DateTimeOffset.UtcNow;
			var rootSegment = rootSegmentBuilder.Build(startTime);
			rootSegmentBuilder.StartTime.Should().Be(rootbuilderStartTime);
			rootSegment.StartTime.Should().Be(rootbuilderStartTime);

			nestedSegmentBuilder1.StartTime.Should().BeBefore(beforeBuildTime);
			var nestedSegment = rootSegment.NestedSegments.First();
			nestedSegment.StartTime.Should().BeBefore(beforeBuildTime);
		}


	}
}
