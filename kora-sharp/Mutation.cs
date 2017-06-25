using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Kora
{
	public class Mutation
	{
		public Mutation()
		{
			this.Merge = new JObject();
			this.Delete = new JObject();
		}

		public JObject Merge { get; set; }
		public JObject Delete { get; set; }

		public Mutation Mrg(string[] path, JToken value)  {
			this.Merge.Put(path, value);
			return this;
		}

		public Mutation Del(params string[] path)  {
			this.Delete.Put(path, 1);
			return this;
		}

		public List<Layer<Mutation>> GetPattern(params string[] path) {
            var obj = new JObject();

            Dynamic
                .GetPattern(this.Merge, path)
                .Aggregate(obj, (collect, item) => {
                    var joined = String.Join(".", item.Path);
                    collect.Put(new []{joined, "merge"}, item.Value);
                    return collect;
                });

            Dynamic
                .GetPattern(this.Delete, path)
                .Aggregate(obj, (collect, item) => {
                    var joined = String.Join(".", item.Path);
                    collect.Put(new []{joined, "delete"}, item.Value);
                    return collect;
                });

            return obj
                .Properties()
                .Aggregate(new List<Layer<Mutation>>(), (collect, item) => {
                    var p = item.Name.Split('.');
                    var value = (JObject)item.Value;
                    var layer = new Layer<Mutation>(new Mutation() {
                        Merge = value.Get<JObject>("merge") ?? new JObject(),
                        Delete = value.Get<JObject>("delete") ?? new JObject(),
                    }, p);
                    collect.Add(layer);
                    return collect;
                });
		}
	}
}

