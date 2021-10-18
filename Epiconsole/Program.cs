using CommandLine;
using Epiconsole.Commands;
using Epiconsole.InitializableModules;
using EPiServer;
using EPiServer.Core;
using EPiServer.Core.Transfer;
using EPiServer.Enterprise;
using EPiServer.Enterprise.Internal;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging;
using EPiServer.Logging.Log4Net;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
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
    public class Program
    {
        public static GlobalConfiguration Configuration { get; set; }
        static void Main(string[] args)
        {

            var fileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "EPiServerLog.config");
            var config = log4net.Config.XmlConfigurator.Configure(fileInfo);
            log4net.ILog log = log4net.LogManager.GetLogger(typeof(Program));
            EPiServer.Logging.LogManager.LoggerFactory = () => new EPiServer.Logging.Log4Net.Log4NetLoggerFactory();
            log.Debug("FOO");

            var parser = CommandLine.Parser.Default;
            LoadConfig();


            var engine = new InitializationEngine((IEnumerable<IInitializableModule>)null, HostType.WebApplication, EPiServerAssemblies());
            engine.Initialize();

            var lm = LogManager.Instance;


           
            parser.ParseArguments<Commands.ExportOptions, Commands.ExportStructureJsonOptions>(args)
                .MapResult(
                (Commands.ExportOptions o) => { engine.Locate.Advanced.GetInstance<Export>().Execute(o); return 0; },
                (ExportStructureJsonOptions o) => { engine.Locate.Advanced.GetInstance<ExportStructureJson>().Execute(o); return 0; },
                err => 1);
            engine.Uninitialize();

#if DEBUG
            Console.ReadKey();
#endif

        }

        private static void LoadConfig()
        {
            var ds = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var path = Path.Combine(Directory.GetCurrentDirectory(), "config.yml");

            Configuration = ds.Deserialize<GlobalConfiguration>(File.ReadAllText(path));
        }

       

        private static IEnumerable<Assembly> EPiServerAssemblies()
        {
            var assemblies = new List<Assembly>();
            assemblies.Add(typeof(Program).Assembly);
            assemblies.Add(typeof(EPiServer.Logging.Log4Net.Log4NetLoggerFactory).Assembly);
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

}
