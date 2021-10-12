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

    [Verb(
        "export",
        isDefault: true,
        HelpText = "Exports content from EPiServer CMS")]
    public class ExportOptions
    {
        [Option("include-files", HelpText = "If true, referenced files on a page will also be exported", Default = true)]
        public bool IncludeLinkedFiles { get; set; }
        
        [Option("include-implicit-content-dependencies", HelpText = "If true, referenced content will exported", Default = true)]
        public bool IncludeImplicitContentDependencies { get; set; }

        [Option("include-referenced-contenttypes", HelpText = "If true, referenced files on a page will also be exported")]
        public bool IncludeReferencedContentTypes { get; internal set; }
        
        [Option("export-property-settings")]
        public bool ExportPropertySettings { get; internal set; }
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
