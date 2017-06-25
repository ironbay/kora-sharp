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

		public List<Layer<Mutation>> GetPattern(string[] path) {
			Dynamic
				.GetPattern(this.Merge, path)
				.Aggregate();
		}
	}
}

