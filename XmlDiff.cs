using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Utils;

namespace XmlDiff
{
    public class XmlDiff
    {
        private static void Partition(IEnumerable<XNode> nodes)
        {
            var numbered = nodes.Select((x, i) => Tuple.Create(i, x, x.ToString())).ToList();
            List<List<Tuple<int, XNode, string>>> partitions = new List<List<Tuple<int, XNode, string>>>();
            foreach (var num in numbered)
            {
                var partition = partitions.FirstOrDefault(p => p.Any(x => XNode.DeepEquals(x.Item2, num.Item2)));
                if (partition == null)
                {
                    partition = new List<Tuple<int, XNode, string>>();
                    partitions.Add(partition);
                }
                partition.Add(num);
            }
            foreach (var partition in partitions)
            {
                var p = partition[0];
                var text = p.Item2.ToString();
                File.WriteAllText(string.Format("node-{0}-{1}.txt", p.Item1, partition.Count), text);
            }
        }

        private static XmlNamespaceManager CreateXmlNamespaceManager(XmlReader reader, XElement root)
        {
            XmlNamespaceManager result = new XmlNamespaceManager(reader.NameTable);
            var namespaces = root.Attributes().Select(attr => Tuple.Create(attr.Name.LocalName != "xmlns" ? attr.Name.LocalName : "ns", attr.Value)).ToList();
            namespaces.ForEach(x => result.AddNamespace(x.Item1, x.Item2));
            return result;
        }
        private static XmlReader CreateReader(string file)
        {
            return XmlReader.Create(file);
        }
        public static void CreatePartitions(string file, string xpath)
        {
            CreateReader(file).TryExe(reader =>
            {
                XElement.Load(reader).TryExe(root =>
                {
                    RemoveComments(root);
                    IEnumerable<XNode> nodes = root.XPathSelectElements(xpath, CreateXmlNamespaceManager(reader, root)).ToList();
                    Partition(nodes);
                    var pairs = nodes.Reverse().Skip(1).Reverse().Zip(nodes.Skip(1), (x, y) => Tuple.Create(x, y)).ToList();
                    var equals = pairs.Select(x => XNode.DeepEquals(x.Item1, x.Item2)).ToList();
                    Console.WriteLine("{0}", string.Join(", ", equals.Select(x => x.ToString())));
                });
            });

        }
        private static void RemoveComments(XElement root)
        {
            var comments = root.DescendantNodes().OfType<XComment>().ToList();
            foreach (var c in comments)
            {
                c.Remove();
            }
        }
    }
}
