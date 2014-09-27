using Nancy;
using Raven.Client;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace HashServer
{
    public class HashModule : NancyModule
    {
        public static Func<SHA1> SHA1Provider = () => SHA1.Create();

        public HashModule(IAsyncDocumentSession session)
        {
            Get["/dir/{path*}", true] =
            Get["/list/{path*}", true] = async (args, ct) =>
            {
                string path = args.Path;

                if (Directory.Exists(path))
                    return await Task.Run(() => Directory.EnumerateFiles(path).ToList());

                else throw new DirectoryNotFoundException(path);
            };

            Get["/dir/rec/{path*}", true] =
            Get["/list/rec/{path*}", true] = async (args, ct) =>
            {
                string path = args.Path;

                if (Directory.Exists(path))
                    return await Task.Run(() => Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).ToList());

                else throw new DirectoryNotFoundException(path);
            };

            Get["/file/{path*}", true] = async (args, ct) =>
            {
                string path = args.Path;
                var fh = await session.GetFileHashByPath(path);

                if (File.Exists(path))
                {
                    var modified = File.GetLastWriteTimeUtc(path);

                    if (fh != null && fh.Modified == modified)
                    {
                        return fh.Hash;
                    }
                    else
                    {
                        await session.StoreAsync(fh = new FileHash()
                        {
                            Path = path,
                            Modified = modified,
                            Hash = await Hash.SHA1Async(path),
                        });

                        await session.SaveChangesAsync();
                        return fh.Hash;
                    }
                }
                else
                {
                    if (fh != null)
                    {
                        session.Delete(fh);
                        await session.SaveChangesAsync();
                    }
                    throw new FileNotFoundException(path);
                }
            };

            Get["/duplicates", true] = async (args, ct) =>
            {
                return await session.Query<FileHashDuplicate, DuplicateHashIndex>().ToListAsync();
            };

            Delete["/file/{path*}", true] = async (args, ct) =>
            {
                string path = args.Path;
                var fh = await session.GetFileHashByPath(path);

                if (fh != null)
                {
                    session.Delete(fh);
                    await session.SaveChangesAsync();
                }

                if (File.Exists(path))
                {
                    File.Delete(path);
                    return HttpStatusCode.OK;
                }
                else throw new FileNotFoundException(path);
            };
        }
    }
}
