using EPiServer.Data;
using EPiServer.Framework;
using EPiServer.Framework.Blobs;
using EPiServer.Framework.Initialization;
using EPiServer.Logging;
using EPiServer.ServiceLocation;

namespace Epiconsole.InitializableModules
{
    [ModuleDependency(typeof(DataInitialization))]
    public class DataAccessInitialization : IConfigurableModule
    {

        public DataAccessInitialization()
        {
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.Configure<DataAccessOptions>(o => o.SetConnectionString(Program.Configuration.ConnectionString));
        }

        public void Initialize(InitializationEngine context)
        { }

        public void Uninitialize(InitializationEngine context)
        { }
    }

}
