using Epiconsole.Commands;
using EPiServer.Data;
using EPiServer.Framework;
using EPiServer.Framework.Blobs;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epiconsole.InitializableModules
{
    [InitializableModule]
    public class VerbInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.AddTransient<Export>();
            context.Services.AddTransient<ExportStructureJson>();

        }

        public void Initialize(InitializationEngine context)
        { }

        public void Uninitialize(InitializationEngine context)
        { }
    }
}
