using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashServer
{
    public class FileHash
    {
        public string Path { get; set; }
        public string Hash { get; set; }
        public DateTime Modified { get; set; }
    }
}
