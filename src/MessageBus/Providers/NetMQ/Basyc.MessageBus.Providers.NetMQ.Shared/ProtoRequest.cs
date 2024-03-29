﻿using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.MessageBus.NetMQ.Shared;

[ProtoContract]
public class ProtoRequest
{
	[ProtoMember(1)]
	public int Id { get; set; }
	[ProtoMember(2)]
	public string? Name { get; set; }
	[ProtoMember(3)]
	public string? Address { get; set; }
}
