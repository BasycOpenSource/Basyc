using System.Collections.Generic;

namespace Basyc.Serialization.ProtobufNet.Tests.TestMessages
{
	public class Class_Inits_And_Sets_Properties
	{
		public Class_Inits_And_Sets_Properties(string name)
		{
			Name = name;
		}

		public string Name { get; init; }

		public string? NickName { get; set; }
		public List<string>? NickNames { get; set; }

		public int Age { get; init; } = 5;
		public List<int> Ages { get; init; } = new List<int> { 1,2,3};

	}
}
