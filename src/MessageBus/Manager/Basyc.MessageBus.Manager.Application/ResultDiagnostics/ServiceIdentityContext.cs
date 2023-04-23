using Basyc.Diagnostics.Shared;
using System.Collections.Generic;

namespace Basyc.MessageBus.Manager.Application.ResultDiagnostics;

public class ServiceIdentityContext
{
    private readonly List<ActivityContext> activities = new List<ActivityContext>();
    public IReadOnlyList<ActivityContext> Activities => activities;
    public ServiceIdentity ServiceIdentity { get; }

    public ServiceIdentityContext(ServiceIdentity serviceIdentity)
    {
        ServiceIdentity = serviceIdentity;
    }

    public void AddActivity(ActivityContext activity)
    {
        activities.Add(activity);
        activities.Sort((x, y) => x.StartTime.CompareTo(y.StartTime));
    }
}
