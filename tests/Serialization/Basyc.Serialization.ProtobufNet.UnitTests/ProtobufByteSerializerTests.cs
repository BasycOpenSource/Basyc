using Basyc.Serialization.ProtobufNet.UnitTests.TestMessages;
using Basyc.Serializaton.Abstraction;
using Bogus;
using FluentAssertions;
using System.Text.Json;
using Xunit;

namespace Basyc.Serialization.ProtobufNet.UnitTests;

public class ProtobufByteSerializerTests
{
	private readonly ProtobufByteSerializer serializer;

	private readonly Faker<TestCar> carFaker;

	private readonly Faker<TestCustomer> customerFaker;

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
		byte[] seriCustomer = serializer.Serialize(originalCustomer, typeof(TestCustomer));
		var deseriCustomer = (TestCustomer?)serializer.Deserialize(seriCustomer, typeof(TestCustomer));

		string origialJson = JsonSerializer.Serialize(originalCustomer);
		string deseriJson = JsonSerializer.Serialize(deseriCustomer);
		origialJson.Should().Be(deseriJson);
	}

	[Fact]
	public void Should_Serialize_Even_Nested()
	{
		var originalCustomer = customerFaker.Generate();
		byte[] seriCustomer = serializer.Serialize(originalCustomer, typeof(TestCustomer));
		string customerMessageType = TypedToSimpleConverter.ConvertTypeToSimple(originalCustomer.GetType());
		var originalWrapper = new ParentWrapperMessage(0, customerMessageType, seriCustomer);

		byte[] seriWrapper = serializer.Serialize(originalWrapper, typeof(ParentWrapperMessage));
		var deseriWrapper = (ParentWrapperMessage?)serializer.Deserialize(seriWrapper, typeof(ParentWrapperMessage));

		string origialJson = JsonSerializer.Serialize(originalWrapper);
		string deseriJson = JsonSerializer.Serialize(deseriWrapper);
		deseriJson.Should().Be(origialJson);
	}

	[Fact]
	public void Should_Serialize_Empty_Class()
	{
		var input = new Class_Empty();
		byte[] seriInput = serializer.Serialize<Class_Empty?>(input);
		object? deseriInput = serializer.Deserialize<Class_Empty?>(seriInput);
		string origialJson = JsonSerializer.Serialize(input);
		string deseriJson = JsonSerializer.Serialize(deseriInput);
		deseriJson.Should().Be(origialJson);
	}

	[Fact]
	public void Should_Serialize_Empty_Record()
	{
		var input = new Class_Empty();
		byte[] seriInput = serializer.Serialize<Record_Empty?>(input);
		object? deseriInput = serializer.Deserialize<Record_Empty?>(seriInput);
		string origialJson = JsonSerializer.Serialize(input);
		string deseriJson = JsonSerializer.Serialize(deseriInput);
		deseriJson.Should().Be(origialJson);
	}

	[Fact]
	public void Should_Serialize_Inits_And_Sets_Properties()
	{
		var input = new Class_Inits_And_Sets_Properties("John")
		{
			NickName = "Johny",
			NickNames = new List<string>() { "Johny1", "Johny2" }
		};
		input.Ages.Add(1);
		input.Ages.Add(2);
		byte[] seriInput = serializer.Serialize<Class_Inits_And_Sets_Properties?>(input);
		seriInput.Should().NotBeEmpty();
		object? deseriInput = serializer.Deserialize<Class_Inits_And_Sets_Properties?>(seriInput);
		string origialJson = JsonSerializer.Serialize(input);
		string deseriJson = JsonSerializer.Serialize(deseriInput);
		deseriJson.Should().Be(origialJson);
	}
}
