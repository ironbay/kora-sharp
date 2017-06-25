using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kora
{
	public static class Dynamic
	{
		public static T Get<T>(this JObject input, params string[] path) {
			var ht = path.Destructure();
			var child = input[ht.Head];
			if (child == null)
				return default(T);
			if (ht.Tail.Length == 0)
				return child.Value<T>();
			var casted = child as JObject;
			if (casted != null)
				return casted.Get<T>(ht.Tail);
			return default(T);
		}

		public static JObject Put(this JObject input, string[] path, JToken value) {
			var ht = path.Destructure();
			if (ht.Tail.Length == 0) {
				input[ht.Head] = value;
				return input;
			}
			var child = input[ht.Head];
			if (child == null || !(child is JObject))
				child = new JObject();
			var casted = (JObject)child;
			input[ht.Head] = casted.Put(ht.Tail, value);
			return input;
		}

		public static JObject Delete(this JObject input, params string[] path) {
			var ht = path.Destructure();
			if (ht.Tail.Length == 0) {
				input.Remove(ht.Head);
				return input;
			}
			var child = input[ht.Head];
			if (child == null || !(child is JObject))
				return input;
			input[ht.Head] = ((JObject)child).Delete(ht.Tail);
			return input;
		}

		public static List<Layer<JToken>> GetPattern(this JObject input, params string[] path) {
			return ((JToken)input).GetPattern(path);
		}

		private static List<Layer<JToken>> GetPattern(this JToken input, string[] path, params string[] left) {
			var ht = path.Destructure();
			if (ht.Head == null)
				return new List<Layer<JToken>> {
					new Layer<JToken>(input, left),
				};
			var children = ht.Head == "+" ? (JObject)input : new JObject().Put(new string[] { ht.Head }, input[ht.Head]);
			return children
				.Properties()
				.Where(p => p != null)
				.Aggregate(new List<Layer<JToken>>(), (collect, p) => {
					var child = children[p.Name];
                    var next_left = new List<string>(left) { p.Name };
                    collect
						.AddRange(
							child.GetPattern(
								ht.Tail,
								next_left.ToArray()
							)
						);
					return collect;
				});
		}
		
		public static List<Layer<JToken>> Flatten(this JObject input, params string[] path) {
			return input
				.Properties()
				.Aggregate(new List<Layer<JToken>>(), (collect, p) => {
					var value = p.Value;
					var next = new List<string>(path);
					next.Add(p.Name);
					if (p.Value is JObject) {
						var children = ((JObject)p.Value).Flatten(next.ToArray());
						collect.AddRange(children);
						return collect;
					}
					collect.Add(new Layer<JToken>(value, next.ToArray()));
					return collect;
				});
		}

		public class HeadTail {
			public string Head { get; set; }
			public string[] Tail { get; set; }
		}

		public static HeadTail Destructure(this string[] input) {
			return new HeadTail {
				Head = input.FirstOrDefault(),
				Tail = input.Skip(1).ToArray()
			};
		}

	}

}

