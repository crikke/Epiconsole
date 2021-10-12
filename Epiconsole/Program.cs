using EPiServer;
using EPiServer.Core;
using EPiServer.Core.Transfer;
using EPiServer.Data;
using EPiServer.Enterprise;
using EPiServer.Enterprise.Internal;
using EPiServer.Framework;
using EPiServer.Framework.Blobs;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Epiconsole
{
    internal class Program
    {
        public static AppConfiguration Configuration { get; set; }
        static void Main(string[] args)
        {
            LoadConfig();


            var engine = new InitializationEngine((IEnumerable<IInitializableModule>)null, HostType.WebApplication, EPiServerAssemblies());
            engine.Initialize();

            var contentRepository = engine.Locate.Advanced.GetInstance<IContentRepository>();
            var startPage = contentRepository.Get<IContent>(SiteDefinition.Current.StartPage);


            ExportContentNode(Directory.GetCurrentDirectory(), startPage.ContentLink, contentRepository);

            engine.Uninitialize();

        }

        private static void LoadConfig()
        {
            var ds = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var path = Path.Combine(Directory.GetCurrentDirectory(), "config.yml");

            Configuration = ds.Deserialize<AppConfiguration>(File.ReadAllText(path));
        }

        public static void ExportContentNode(
          string currentPath,
          ContentReference contentLink,
          IContentRepository contentRepository)
        {
            ExportSource node = new ExportSource(contentLink)
            {
                RecursiveLevel = 0,
            };

            var c = contentRepository.Get<IContent>(contentLink);


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
                var o = new EPiServer.Enterprise.ExportOptions()
                {
                    
                    ExcludeFiles = !Program.Configuration.Export.IncludeLinkedFiles.GetValueOrDefault(true),
                    IncludeReferencedContentTypes = Program.Configuration.Export.IncludeReferencedContentTypes,
                    ExportPropertySettings = Program.Configuration.Export.ExportPropertySettings
                };
                
                exporter.Export(fs, o);
                exporter.Dispose();
            }


            //Run for each child node
            var children = contentRepository
                .GetChildren<IContent>(contentLink)
                .ToList();

            foreach (var child in children)
            {
                ExportContentNode(path, child.ContentLink, contentRepository);
            }
        }

        private static IEnumerable<Assembly> EPiServerAssemblies()
        {
            var assemblies = new List<Assembly>();
            assemblies.Add(typeof(Program).Assembly);
            assemblies.Add(typeof(EPiServer.Data.ConnectionStringOptions).Assembly);//EPiServer.Data
            assemblies.Add(typeof(EPiServer.Framework.InitializableModuleAttribute).Assembly);//EPiServer.Framework
            assemblies.Add(typeof(EPiServer.Core.IContent).Assembly);//EPiServer
            assemblies.Add(typeof(EPiServer.Enterprise.IDataExporter).Assembly);//EPiServer.Enterprise
            assemblies.Add(typeof(EPiServer.Personalization.VisitorGroups.VisitorGroupCriterion).Assembly);//EPiServer.ApplicationModules
            assemblies.Add(typeof(EPiServer.Events.EventMessage).Assembly);//EPiServer.Events
            assemblies.Add(typeof(EPiServer.ServiceLocation.StructureMapServiceLocator).Assembly);//EPiServer.ServiceLocation.StructureMap
            return assemblies;
        }
    }


    [ModuleDependency(typeof(EPiServer.Data.DataInitialization))]
    public class DataAccessInitialization : IConfigurableModule
    {

        public DataAccessInitialization()
        {
        }

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

            context.Services.Configure<DataAccessOptions>(o => o.SetConnectionString(Program.Configuration.ConnectionString));
        }

        public void Initialize(InitializationEngine context)
        { }

        public void Uninitialize(InitializationEngine context)
        { }
    }

}
