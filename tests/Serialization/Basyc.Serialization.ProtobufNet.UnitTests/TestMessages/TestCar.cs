using System;

namespace Basyc.Serialization.ProtobufNet.Tests.TestMessages;

public class TestCar
{
    public TestCar()
    {

    }
    public TestCar(string name, DateTime assemblyDate)
    {
        Name = name;
        AssemblyDate = assemblyDate;
    }
    public string Name { get; init; }
    public DateTime AssemblyDate { get; init; }
}
