using Basyc.Diagnostics.Shared;

namespace Basyc.MessageBus.Manager.Application.ResultDiagnostics;

public class ServiceIdentityContext
{
    private readonly List<ActivityContext> activities = new();

    public ServiceIdentityContext(ServiceIdentity serviceIdentity)
    {
        ServiceIdentity = serviceIdentity;
    }

    public ServiceIdentity ServiceIdentity { get; }

    public IReadOnlyList<ActivityContext> Activities => activities;

    public void AddActivity(ActivityContext activity)
    {
        activities.Add(activity);
        activities.Sort((x, y) => x.StartTime.Value.CompareTo(y.StartTime.Value));
    }
}
