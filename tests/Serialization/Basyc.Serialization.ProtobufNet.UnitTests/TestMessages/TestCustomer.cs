namespace Basyc.Serialization.ProtobufNet.UnitTests.TestMessages;

public class TestCustomer
{
#pragma warning disable CS8618
    public TestCustomer()
#pragma warning restore CS8618
    {
    }

    public TestCustomer(string firstName, string lastName, int age, TestCar car)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
        Car = car;
    }

    public string FirstName { get; init; }

    public string LastName { get; init; }

    public int Age { get; init; }

    public TestCar Car { get; init; }
}
