﻿namespace Basyc.MessageBus.Manager.Application.Tests.Durations;

public static class DurationTestsHelper
{
	public const int TaskDelayPrecisionMs = 150;
	public static TimeSpan TaskDelayPrecision = TimeSpan.FromMilliseconds(TaskDelayPrecisionMs);
}
