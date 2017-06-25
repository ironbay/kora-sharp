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
		public abstract T Get<T>(params string[] path);

		public void Put(string[] path, JToken value) {
			var mut = new Mutation().Mrg (path, value);
			this.Apply(mut);
		}

		public void Apply(Mutation mut) {
            this.Trigger(mut);
			mut.Delete.Flatten().ForEach(layer => this.Delete(layer.Path));
			mut.Merge.Flatten().ForEach(layer => this.Merge(layer));
		}

		private void Trigger(Mutation mut) {
			this._interceptors.ForEach(interceptor => {
                mut.GetPattern(interceptor.Path).ForEach(layer => {
                    interceptor.Value(layer.Value, layer.Path);
                });
			});
		}

        public void Intercept(string pattern, Interceptor cb) {
            var splits = pattern.Split('.');
            this._interceptors.Add(new Layer<Interceptor>(cb, splits));
        }

	}

    public class MemoryStore : Store
    {
        private JObject _data = new JObject();
  
        protected override void Delete(params string[] path)
        {
            this._data.Delete(path);
        }

        public override T Get<T>(params string[] path)
        {
            return this._data.Get<T>(path);
        }

        protected override void Merge(Layer<JToken> layer)
        {
            this._data.Put(layer.Path, layer.Value);
        }
    }
}

