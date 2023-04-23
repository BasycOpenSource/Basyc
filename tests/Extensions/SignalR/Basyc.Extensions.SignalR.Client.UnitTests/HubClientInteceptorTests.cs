using System.Reflection;
using Basyc.Extensions.SignalR.Client.Tests.Helpers;
using Basyc.Extensions.SignalR.Client.Tests.Mocks;

namespace Basyc.Extensions.SignalR.Client.Tests;

public class HubClientInteceptorTests
{
    private readonly HubConnectionMock connection;

    public HubClientInteceptorTests()
    {
        connection = new HubConnectionMockBuilder().BuildAsMock();
    }

    [Fact]
    public void Should_Not_Throw_When_CorrectHub()
    {
        foreach (var correctHubType in CorrectMethodsClientCanCallHubs.CorrectMethodsClientCanCallTypes)
        {
            var publicMethods = correctHubType.GetMethodsRecursive(BindingFlags.Public | BindingFlags.Instance);
            var inteceptor = new HubClientInterceptor(connection, correctHubType);
            inteceptor.InterceptedMethods.Count.Should().Be(publicMethods.Length);
            for (int methodIndex = 0; methodIndex < publicMethods.Length; methodIndex++)
            {
                var publicMethod = publicMethods[methodIndex];
                var methodMetadata = inteceptor.InterceptedMethods[methodIndex];
                (methodMetadata.MethodInfo == publicMethod).Should().BeTrue();

                methodMetadata.ReturnsVoid.Should().Be(publicMethod.ReturnType == typeof(void));
                methodMetadata.ReturnsTask.Should().Be(publicMethod.ReturnType == typeof(Task));
                var parameterInfos = publicMethod.GetParameters();
                methodMetadata.Parameters.Should().Equal(parameterInfos.Select(x => x.ParameterType));
                methodMetadata.HasCancelToken.Should().Be(parameterInfos.Any(x => x.ParameterType == typeof(CancellationToken)));
                if (methodMetadata.HasCancelToken)
                {
                    int cancelTokenIndex = -1;
                    for (int paramIndex = 0; paramIndex < parameterInfos.Length; paramIndex++)
                    {
                        var paraInfo = parameterInfos[paramIndex];
                        if (paraInfo.ParameterType == typeof(CancellationToken))
                        {
                            cancelTokenIndex = paramIndex;
                            break;
                        }
                    }

                    methodMetadata.CancelTokenIndex.Should().Be(cancelTokenIndex);
                }
            }
        }
    }

    [Fact]
    public void Should_Include_Inherited_Public_Methods()
    {
        var hubClientType = typeof(ICorrectMethodsClientCanCallInheritedVoids);
        var publicMethods = hubClientType.GetMethodsRecursive(BindingFlags.Public | BindingFlags.Instance);

        var inteceptor = new HubClientInterceptor(connection, hubClientType);
        inteceptor.InterceptedMethods.Count.Should().Be(publicMethods.Length);
    }

    [Fact]
    public void Should_Throw_When_CreatingWrongHub()
    {
        foreach (var hubType in WrongHubs.TypesThatShouldFailCreating)
        {
            var action = () =>
            {
                var inteceptor = new HubClientInterceptor(connection, hubType);
            };
            action.Should().Throw<ArgumentException>();
        }
    }
}
