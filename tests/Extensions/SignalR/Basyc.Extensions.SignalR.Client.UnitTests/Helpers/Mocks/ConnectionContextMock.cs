using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Features;
using System.Collections;
using System.IO.Pipelines;

namespace Basyc.Extensions.SignalR.Client.Tests.Mocks;

internal class ConnectionContextMock : ConnectionContext
{

	public ConnectionContextMock(Pipe pipe)
	{
		Transport = new DuplexPipeMock(pipe);
	}
	public override IDuplexPipe Transport { get; set; }
	public override string ConnectionId { get; set; } = "-1";

	public override IFeatureCollection Features { get; } = new FeatureCollectionMock();

	public override IDictionary<object, object?> Items { get; set; } = new Dictionary<object, object?>();

	private class FeatureCollectionMock : IFeatureCollection
	{
		private readonly Dictionary<Type, object?> map = new Dictionary<Type, object?>();

		public object? this[Type key]
		{
			get => map[key];

			set => map[key] = value;
		}

		public bool IsReadOnly => true;

		public int Revision => 1;

		public TFeature? Get<TFeature>()
		{
			map.TryGetValue(typeof(TFeature), out var value);
			return (TFeature?)value;
		}

		public IEnumerator<KeyValuePair<Type, object>> GetEnumerator()
		{
			return map.GetEnumerator();
		}

		public void Set<TFeature>(TFeature? instance)
		{
			map[typeof(TFeature)] = instance;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return map.GetEnumerator();
		}
	}
}