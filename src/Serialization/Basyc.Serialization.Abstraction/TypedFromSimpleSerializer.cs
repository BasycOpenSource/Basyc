﻿using Basyc.Serializaton.Abstraction;

namespace Basyc.Serialization.Abstraction;

public sealed class TypedFromSimpleSerializer : ITypedByteSerializer
{
	private readonly IObjectToByteSerailizer byteSerailizer;

	public TypedFromSimpleSerializer(IObjectToByteSerailizer byteSerailizer)
	{
		this.byteSerailizer = byteSerailizer;
	}

	public object? Deserialize(byte[] input, Type dataType)
	{
		return byteSerailizer.Deserialize(input, TypedToSimpleConverter.ConvertTypeToSimple(dataType));
	}

	public byte[] Serialize(object? input, Type dataType)
	{
		return byteSerailizer.Serialize(input, TypedToSimpleConverter.ConvertTypeToSimple(dataType));
	}
}
