using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HashServer
{
    class Program
    {
        public static readonly HostConfiguration HostConfig = new HostConfiguration() { UrlReservations = new UrlReservations() { CreateAutomatically = true } };
        public static readonly Uri HostUri = new Uri("http://localhost:5556");

        public static HashServerConfig Config;

        static void Main(string[] args)
        {
            Config = HashServerConfig.Parse(args);

            Task.Factory.StartNew(() => ScanDirectories(Config.RecursiveDirectories), TaskCreationOptions.LongRunning);

            using (var host = new NancyHost(HostConfig, HostUri))
            {
                Nancy.StaticConfiguration.DisableErrorTraces = false;
                host.Start();
                Console.ReadLine();
            }
        }

        static void ScanDirectories(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                Console.WriteLine("Precaching path: {0}", path);

                var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
                    .Where(f => !f.StartsWith(@"E:\downloads\running") && !f.EndsWith("thumbs.db"));

                foreach (var file in files)
                {
                    try
                    {
                        using (var session = Data.DocumentStore.OpenSession())
                        {
                            var fh = session.GetFileHashByPath(file);

                            if (fh == null)
                            {
                                Console.WriteLine("Caching: {0}", file);

                                fh = new FileHash()
                                {
                                    Path = file,
                                    Modified = File.GetLastWriteTimeUtc(file),
                                    Hash = Hash.SHA1(file),
                                };
                                session.Store(fh);
                            }
                            else
                            {
                                if (fh.Modified != File.GetLastWriteTimeUtc(file))
                                {
                                    Console.WriteLine("Updating cache: {0}", file);

                                    fh.Hash = Hash.SHA1(file);
                                    fh.Modified = File.GetLastWriteTimeUtc(file);
                                }
                                else Console.WriteLine("Skipping: {0}", file);
                            }

                            session.SaveChanges();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                Console.WriteLine("Precaching complete");
            }
        }
    }
}
