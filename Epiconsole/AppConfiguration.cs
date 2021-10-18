using CommandLine;
using Epiconsole.InitializableModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epiconsole
{

    public class GlobalConfiguration
    {
        public BlobProviderConfig Blob { get; set; }

        [Option(
            "connection-string",
            HelpText = "Override EPiServerDB connection string. If not set it is loaded from config.yml")]
        public string ConnectionString { get; set; }
    }



    public class BlobProviderConfig
    {
        public List<BlobProvider> BlobProviders { get; set; }
        public string DefaultProvider { get; set; }
    }

    public class BlobProvider
    {
        public string Name { get; set; }
        public BlobProviderType ProviderType { get; set; }
        public string Path { get; set; }
    }

    public enum BlobProviderType
    {
        FileBlobProvider,
        NOT_IMPLEMENTED,
    }
}
