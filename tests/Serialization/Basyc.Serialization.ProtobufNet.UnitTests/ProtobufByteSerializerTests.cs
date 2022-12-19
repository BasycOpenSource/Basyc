using Basyc.Serialization.ProtobufNet.Tests.TestMessages;
using Basyc.Serializaton.Abstraction;
using Bogus;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace Basyc.Serialization.ProtobufNet.Tests
{
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
            var input = new Class_Empty();
            var seriInput = serializer.Serialize<Class_Empty?>(input);
            var deseriInput = serializer.Deserialize<Class_Empty?>(seriInput);
            var origialJson = JsonSerializer.Serialize(input);
            var deseriJson = JsonSerializer.Serialize(deseriInput);
            deseriJson.Should().Be(origialJson);
        }

        [Fact]
        public void Should_Serialize_Empty_Record()
        {
            var input = new Class_Empty();
            var seriInput = serializer.Serialize<Record_Empty?>(input);
            var deseriInput = serializer.Deserialize<Record_Empty?>(seriInput);
            var origialJson = JsonSerializer.Serialize(input);
            var deseriJson = JsonSerializer.Serialize(deseriInput);
            deseriJson.Should().Be(origialJson);
        }

        [Fact]
        public void Should_Serialize_Inits_And_Sets_Properties()
        {
            var input = new Class_Inits_And_Sets_Properties("John");
            input.NickName = "Johny";
            input.NickNames = new List<string>() { "Johny1", "Johny2" };
            input.Ages.Add(1);
            input.Ages.Add(2);
            var seriInput = serializer.Serialize<Class_Inits_And_Sets_Properties?>(input);
            seriInput.Should().NotBeEmpty();
            var deseriInput = serializer.Deserialize<Class_Inits_And_Sets_Properties?>(seriInput);
            var origialJson = JsonSerializer.Serialize(input);
            var deseriJson = JsonSerializer.Serialize(deseriInput);
            deseriJson.Should().Be(origialJson);
        }


    }
}