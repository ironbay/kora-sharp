using System;

namespace Kora
{
	public class Layer<T>
	{
		public Layer(T value, params string[] path) {
			this.Value = value;
			this.Path = path;
		}
		public T Value { get; set; }
		public string[] Path { get; set; }
	}
}

