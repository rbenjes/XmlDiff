using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Utils;

namespace XmlDiff
{
    public class NamespaceUtils
    {
        private static List<Tuple<string, string>> CreateNamespaceTuples(List<XAttribute> attributes)
        {
            var result = attributes.Select(attr => Tuple.Create(attr.Name.LocalName != "xmlns" ? attr.Name.LocalName : "ns", attr.Value)).ToList();
            return result;
        }
        public static XmlNamespaceManager CreateXmlNamespaceManager(XmlReader reader, XElement root, List<XAttribute> additionalNamespaces = null)
        {
            XmlNamespaceManager result = new XmlNamespaceManager(reader.NameTable);
            var namespaces = CreateNamespaceTuples(root.Attributes().ToList());
            additionalNamespaces.TryExe(add => 
            {
                namespaces.AddRange(CreateNamespaceTuples(add));
            });
            namespaces.ForEach(x => result.AddNamespace(x.Item1, x.Item2));
            return result;
        }
    }
}
