using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Utils;

namespace XmlDiff
{
    public class PathQuery
    {
        XElement root;
        XmlNamespaceManager namespaceManager;

        public XmlNamespaceManager NamespaceManager
        {
            get { return namespaceManager; }
        }
        public PathQuery(string file, List<XAttribute> additionalNamespaces)
            : this(XmlReader.Create(file), additionalNamespaces)
        {

        }

        public PathQuery(XmlReader reader, List<XAttribute> additionalNamespaces)
        {
            root = XElement.Load(reader);
            namespaceManager = NamespaceUtils.CreateXmlNamespaceManager(reader, root, additionalNamespaces);
        }

        public IEnumerable<XElement> SelectElements(string xpath, Func<XElement, XElement> rootTransformer)
        {
            return this.root.TryEval(root =>
            {
                root = rootTransformer.TryEval(x => x(root)) ?? root;
                var nodes = root.XPathSelectElements(xpath, namespaceManager).ToList();
                return nodes;
            });
        }
    }
}
