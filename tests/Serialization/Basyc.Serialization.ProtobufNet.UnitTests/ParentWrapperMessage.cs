//using Bogus;
//using FluentAssertions;
//using System.Text.Json;
//using Xunit;

using ProtoBuf;

namespace Basyc.Serialization.ProtobufNet.Tests;

[ProtoContract]
public class ParentWrapperMessage
{
	//Supressing warning since this ctor is only used for serializers
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	protected ParentWrapperMessage()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	{

	}

	public ParentWrapperMessage(int sessionId, string messageType, byte[] messageData)
	{
		SessionId = sessionId;
		MessageType = messageType;
		MessageData = messageData;
	}

	[ProtoMember(1)]
	public int SessionId { get; set; }

	[ProtoMember(3)]
	public string MessageType { get; set; }

	[ProtoMember(4)]
	public byte[] MessageData { get; set; }
}
