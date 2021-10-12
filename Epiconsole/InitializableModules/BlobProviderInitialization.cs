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
    public class BlobProviderInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            foreach (var blobProvider in Program.Configuration.Blob.BlobProviders)
            {
                if (blobProvider.ProviderType == BlobProviderType.FileBlobProvider)
                {
                    context.Services.AddFileBlobProvider(blobProvider.Name, blobProvider.Path);
                }
            }

            context.Services.Configure<BlobOptions>(o =>
            {
                o.DefaultProvider = Program.Configuration.Blob.DefaultProvider;
            });

        }

        public void Initialize(InitializationEngine context)
        { }

        public void Uninitialize(InitializationEngine context)
        { }
    }
}
