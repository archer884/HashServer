using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashServer
{
    public class FileHashIndex : AbstractIndexCreationTask<FileHash>
    {
        public FileHashIndex()
        {
            this.Map = fileHashes => fileHashes.Select(fh => new { fh.Path, fh.Hash, fh.Modified });
        }
    }
}
