using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epiconsole
{
    public class AppConfiguration
    {
        public BlobProviderConfig Blob { get; set; }
        public string ConnectionString { get; set; }
        public ExportOptions Export { get; set; }
    }

    public class ExportOptions
    {
        public bool? IncludeLinkedFiles { get; set; }
        public List<string> IgnoreContentTypes {  get; set; }
        public bool IncludeReferencedContentTypes { get; internal set; }
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
