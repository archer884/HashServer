using Nancy;
using Nancy.TinyIoc;
using Raven.Client;
using Raven.Client.Indexes;
using System.Reflection;

namespace HashServer
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register<IDocumentStore>(Data.DocumentStore);

            IndexCreation.CreateIndexes(Assembly.GetExecutingAssembly(), Data.DocumentStore);
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);
            container.Register<IAsyncDocumentSession>(container.Resolve<IDocumentStore>().OpenAsyncSession());
        }
    }
}
