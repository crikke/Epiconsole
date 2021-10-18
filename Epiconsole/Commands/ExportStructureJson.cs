using CommandLine;
using EPiServer;
using EPiServer.Core;
using EPiServer.Core.Transfer;
using EPiServer.Enterprise;
using EPiServer.Enterprise.Internal;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epiconsole.Commands
{
    public class ExportStructureJson
    {
        private readonly ILogger _log;
        private readonly IContentLoader _contentLoader;

        public ExportStructureJson(
            IContentLoader contentLoader)
        {
            _log = LogManager.GetLogger(typeof(ExportStructureJsonOptions));
            this._contentLoader = contentLoader;
        }


        public void Execute(ExportStructureJsonOptions options)
        {
            var root = new Content();
            var startPage = _contentLoader.Get<IContent>(SiteDefinition.Current.StartPage);

            var startPageChildren = new Content();
            root.Children[startPage.ContentGuid] = startPageChildren;

            ExportContentNode(startPageChildren, startPage.ContentLink);


            using (StreamWriter sw = File.CreateText($"{Directory.GetCurrentDirectory()}/out.json"))
            {
                var res = JsonConvert.SerializeObject(root);
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(sw, root);
            }
        }

        public void ExportContentNode(
         Content currentPath,
         ContentReference contentReference)
        {
            var children = _contentLoader.GetChildren<IContent>(contentReference);

            foreach (var item in children)
            {
                var s = new Content();

                currentPath.Children[item.ContentGuid] = s;
                ExportContentNode(s, item.ContentLink);
            }
        }
    }

    [Verb(
    "exportstructure",
    isDefault: true,
    HelpText = "Exports structure from EPiServer CMS")]
    public class ExportStructureJsonOptions
    {
        
    }

    public class Content
    {
        //[Newtonsoft.Json.JsonDictionary(]
        public Dictionary<Guid, Content> Children { get; set; } = new Dictionary<Guid, Content>();
    }
}
