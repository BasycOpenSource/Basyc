using Basyc.Serialization.ProtobufNet.UnitTests.TestMessages;
using Basyc.Serializaton.Abstraction;
using Bogus;
using FluentAssertions;
using System.Text.Json;
using Xunit;

namespace Basyc.Serialization.ProtobufNet.UnitTests;

public class ProtobufByteSerializerTests
{
    private readonly Faker<TestCar> carFaker;

    private readonly Faker<TestCustomer> customerFaker;
    private readonly ProtobufByteSerializer serializer;

    public ProtobufByteSerializerTests()
    {
        serializer = new ProtobufByteSerializer();
        carFaker = new Faker<TestCar>()
            .RuleFor(x => x.Name, x => x.Name.LastName());

        customerFaker = new Faker<TestCustomer>()
            .RuleFor(x => x.FirstName, x => x.Name.FirstName())
            .RuleFor(x => x.LastName, x => x.Name.LastName())
            .RuleFor(x => x.Age, x => x.Random.Int(0, 100))
            .RuleFor(x => x.Car, carFaker.Generate(1).First());
    }

    [Fact]
    public void Should_Serialize_All_Properties()
    {
        var originalCustomer = customerFaker.Generate();
        var seriCustomer = serializer.Serialize(originalCustomer, typeof(TestCustomer));
        var deseriCustomer = (TestCustomer?)serializer.Deserialize(seriCustomer, typeof(TestCustomer));

        var origialJson = JsonSerializer.Serialize(originalCustomer);
        var deseriJson = JsonSerializer.Serialize(deseriCustomer);
        origialJson.Should().Be(deseriJson);
    }

    [Fact]
    public void Should_Serialize_Even_Nested()
    {
        var originalCustomer = customerFaker.Generate();
        var seriCustomer = serializer.Serialize(originalCustomer, typeof(TestCustomer));
        var customerMessageType = TypedToSimpleConverter.ConvertTypeToSimple(originalCustomer.GetType());
        var originalWrapper = new ParentWrapperMessage(0, customerMessageType, seriCustomer);

        var seriWrapper = serializer.Serialize(originalWrapper, typeof(ParentWrapperMessage));
        var deseriWrapper = (ParentWrapperMessage?)serializer.Deserialize(seriWrapper, typeof(ParentWrapperMessage));

        var origialJson = JsonSerializer.Serialize(originalWrapper);
        var deseriJson = JsonSerializer.Serialize(deseriWrapper);
        deseriJson.Should().Be(origialJson);
    }

    [Fact]
    public void Should_Serialize_Empty_Class()
    {
        var input = new ClassEmpty();
        var seriInput = serializer.Serialize<ClassEmpty?>(input);
        var deseriInput = serializer.Deserialize<ClassEmpty?>(seriInput);
        var origialJson = JsonSerializer.Serialize(input);
        var deseriJson = JsonSerializer.Serialize(deseriInput);
        deseriJson.Should().Be(origialJson);
    }

    [Fact]
    public void Should_Serialize_Empty_Record()
    {
        var input = new ClassEmpty();
        var seriInput = serializer.Serialize<RecordEmpty?>(input);
        var deseriInput = serializer.Deserialize<RecordEmpty?>(seriInput);
        var origialJson = JsonSerializer.Serialize(input);
        var deseriJson = JsonSerializer.Serialize(deseriInput);
        deseriJson.Should().Be(origialJson);
    }

    [Fact]
    public void Should_Serialize_Inits_And_Sets_Properties()
    {
        var input = new ClassInitsAndSetsProperties("John")
        {
            NickName = "Johny",
            NickNames = new List<string>
            {
                "Johny1",
                "Johny2"
            }
        };
        input.Ages.Add(1);
        input.Ages.Add(2);
        var seriInput = serializer.Serialize<ClassInitsAndSetsProperties?>(input);
        seriInput.Should().NotBeEmpty();
        var deseriInput = serializer.Deserialize<ClassInitsAndSetsProperties?>(seriInput);
        var origialJson = JsonSerializer.Serialize(input);
        var deseriJson = JsonSerializer.Serialize(deseriInput);
        deseriJson.Should().Be(origialJson);
    }
}
