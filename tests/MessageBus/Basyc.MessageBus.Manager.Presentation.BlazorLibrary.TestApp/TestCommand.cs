using Basyc.DomainDrivenDesign.Domain;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.TestApp;

public record TestCommand(string Name) : ICommand;
