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
            Map = fileHashes => fileHashes.Select(fh => new { fh.Path, fh.Hash, fh.Modified });
        }
    }

    public class DuplicateHashIndex : AbstractIndexCreationTask<FileHash, FileHashDuplicate>
    {
        public DuplicateHashIndex()
        {
            Map = results => results.Select(r => new FileHashDuplicate()
            {
                Hash = r.Hash,
                Count = 1,
                Paths = new[] { r.Path },
            });

            Reduce = results => results.GroupBy(r => r.Hash).Select(g => new FileHashDuplicate()
            {
                Hash = g.Key,
                Count = g.Sum(r => r.Count),
                Paths = g.SelectMany(r => r.Paths),
            });
        }
    }
}
