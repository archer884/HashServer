using Raven.Client;
using Raven.Client.Document;
using System.Linq;
using System.Threading.Tasks;

namespace HashServer
{
    static class Data
    {
        public static IDocumentStore DocumentStore = new DocumentStore()
        {
            Url = "http://blackmetal-iv:8080/",
            DefaultDatabase = "HashServer",
        }.Initialize();

        public static IAsyncDocumentSession GetSession()
        {
            return DocumentStore.OpenAsyncSession();
        }

        public static FileHash GetFileHashByPath(this IDocumentSession session, string path)
        {
            return session.Query<FileHash, FileHashIndex>().SingleOrDefault(f => f.Path == path);
        }

        public static async Task<FileHash> GetFileHashByPath(this IAsyncDocumentSession session, string path)
        {
            return await session.Query<FileHash, FileHashIndex>().SingleOrDefaultAsync(f => f.Path == path);
        }
    }
}
