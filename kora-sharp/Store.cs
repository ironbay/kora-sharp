using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Kora
{
	public delegate void Interceptor(Mutation mut, string[] path);

	public abstract class Store
	{
		private List<Layer<Interceptor>> _interceptors;

		public Store()
		{
			this._interceptors = new List<Layer<Interceptor>>();
		}

		protected abstract void Merge(Layer<JToken> layer);
		protected abstract void Delete(params string[] path);
		protected abstract void Get(params string[] path);

		public void Put(string[] path, JToken value) {
			var mut = new Mutation().Mrg (path, value);
			this.Apply(mut);
		}

		public void Apply(Mutation mut) {
			mut.Delete.Flatten().ForEach(layer => this.Delete(layer.Path));
			mut.Merge.Flatten().ForEach(layer => this.Merge(layer));
		}

		private void Trigger(Mutation mut) {
			this._interceptors.ForEach (layer => {

			});
		}

	}
}

