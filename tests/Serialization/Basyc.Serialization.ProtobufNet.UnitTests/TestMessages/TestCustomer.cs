using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Serialization.ProtobufNet.Tests.TestMessages;

public class TestCustomer
{
	public TestCustomer()
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
