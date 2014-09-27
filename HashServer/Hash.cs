using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashServer
{
    public static class Hash
    {
        public static string SHA1(string path)
        {
            using (var stream = File.OpenRead(path))
                return System.Security.Cryptography.SHA1.Create().ComputeHash(stream)
                    .Aggregate(new StringBuilder(), (a, b) => a.Append(b.ToString("x2"))).ToString();
        }

        public static Task<string> SHA1Async(string path)
        {
            return Task.Run(() => SHA1(path));
        }
    }
}
