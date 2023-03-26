using Basyc.DomainDrivenDesign.Domain;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.TestApp;

public record TestQuery(string ToLower) : IQuery<string>;
