using EPiServer.Core;
using EPiServer.Core.Transfer;

namespace Epiconsole
{
    public class TestPropertyTransform : IPropertyTransform
    {
        public bool TransformForExport(PropertyData source, RawProperty output, PropertyExportContext context)
        {
            return true;
        }

        public bool TransformForImport(RawProperty source, PropertyData output, PropertyImportContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}