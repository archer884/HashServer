using System.Collections.Generic;

namespace HashServer
{
    public class FileHashDuplicate
    {
        public string Hash;
        public int Count;
        public IEnumerable<string> Paths;
    }
}
