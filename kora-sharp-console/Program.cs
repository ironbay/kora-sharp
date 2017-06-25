using Kora;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kora_sharp_console
{
    class Program
    {
        static void Main(string[] args)
        {
            var store = new MemoryStore();
            store.Intercept("+", (m, path) => {
                var result = JsonConvert.SerializeObject(m);
                Console.WriteLine(result);

            });
            store.Apply(
                new Mutation()
                    .Mrg(new[] { "a", "hello" }, 1)
                    .Mrg(new[] { "b", "bye" }, 2)
                    .Mrg(new[] { "c", "cool" }, 3)
                    .Del(new[] { "a", "hello" })
            );
            Console.ReadLine();

        }
    }
}
