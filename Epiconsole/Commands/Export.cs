using CommandLine;
using EPiServer;
using EPiServer.Core;
using EPiServer.Core.Transfer;
using EPiServer.Enterprise;
using EPiServer.Enterprise.Internal;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epiconsole.Commands
{
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

    public class Export
    {
        private readonly ILogger _log;
        private readonly IContentLoader _contentLoader;

        public Export(
            IContentLoader contentLoader)
        {
            _log = LogManager.GetLogger(typeof(Export));
            this._contentLoader = contentLoader;
        }


        public void Execute(ExportOptions options)
        {
            var startPage = _contentLoader.Get<IContent>(SiteDefinition.Current.StartPage);
            ExportContentNode(Directory.GetCurrentDirectory(), startPage.ContentLink, options);

        }

        public void ExportContentNode(
         string currentPath,
         ContentReference contentLink,
         ExportOptions options)
        {
            ExportSource node = new ExportSource(contentLink)
            {
                RecursiveLevel = 0,
            };

            var c = _contentLoader.Get<IContent>(contentLink);
            _log.Trace($"Exporting {c.ContentLink} - {c.Name}");

            // Export Node 
            if (c == null)
            {
                return;
            }

            var path = $"{currentPath}\\{c.Name}";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (var exporter = ServiceLocator.Current.GetInstance<IDataExporter>() as DefaultDataExporter)
            {
                var fs = new FileStream($@"{path}\data.episerverdata", FileMode.Create, FileAccess.ReadWrite, FileShare.None);

                exporter.SourceRoots.Add(node);
                exporter.IncludeImplicitContentDependencies = options.IncludeImplicitContentDependencies;

                var o = new EPiServer.Enterprise.ExportOptions()
                {

                    ExcludeFiles = !options.IncludeLinkedFiles,
                    IncludeReferencedContentTypes = options.IncludeReferencedContentTypes,
                    ExportPropertySettings = options.ExportPropertySettings
                };

                exporter.Export(fs, o);
            }


            //Run for each child node
            var children = _contentLoader
                .GetChildren<IContent>(contentLink)
                .ToList();

            foreach (var child in children)
            {
                ExportContentNode(path, child.ContentLink, options);
            }
        }
    }
}
