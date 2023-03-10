using Basyc.DomainDrivenDesign.Domain;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.TestApp;

public record TestCommandWithResponse(string Name) : ICommand<string>;
