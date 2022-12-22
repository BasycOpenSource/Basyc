using System;
using System.Collections.Generic;

namespace Basyc.MessageBus.Manager.Infrastructure;

public class TypedDomainSettings
{
	/// <summary>
	/// Name of request group, usually name of a service e.g CustomerService
	/// </summary>
	public string DomainName { get; set; }

	/// <summary>
	/// If you dont know waht CQRS is, use only <see cref="GenericRequestTypes"/> and <see cref="GenericRequestWithResponseTypes"/> properties
	/// </summary>
	public List<Type> GenericRequestTypes { get; set; } = new List<Type>();

	/// <summary>
	/// If you dont know waht CQRS is, use only <see cref="GenericRequestTypes"/> and <see cref="GenericRequestWithResponseTypes"/> properties
	/// </summary>
	public List<TypedReqeustResponseTypePair> GenericRequestWithResponseTypes { get; set; } = new List<TypedReqeustResponseTypePair>();

	/// <summary>
	/// Use when your system implements CQRS
	/// </summary>
	public List<TypedReqeustResponseTypePair> QueryTypes { get; set; } = new List<TypedReqeustResponseTypePair>();

	/// <summary>
	/// Use when your system implements CQRS
	/// </summary>
	public List<Type> CommandTypes { get; set; } = new List<Type>();

	/// <summary>
	/// Use when your system implements CQRS
	/// </summary>
	public List<TypedReqeustResponseTypePair> CommandWithResponseTypes { get; set; } = new List<TypedReqeustResponseTypePair>();
}