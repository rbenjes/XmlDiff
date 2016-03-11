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
        private static bool XNodeDeepEquals(XNode x, XNode y)
        {
            return XNode.DeepEquals(x, y);
        }
        private static void Partition(IEnumerable<XNode> nodes)
        {
            var numbered = nodes.Select((x, i) => new DiffItem() { Number = i, Node = x, NodeAsString = x.ToString() }).ToList();
            List<List<DiffItem>> partitions = numbered.GroupBy(x => x.Node, LambdaEqualityComparer.Create<XNode>(XNodeDeepEquals)).Select(CreateDiffItems).ToList();
            foreach (var partition in partitions)
            {
                var p = partition[0];
                File.WriteAllText(string.Format("node-{0}-{1}.txt", p.Number, partition.Count), p.NodeAsString);
            }
        }

        private static List<DiffItem> CreateDiffItems(IGrouping<XNode, DiffItem> group)
        {
            return new List<DiffItem>(group.Select(x => x));
        }

        private static XmlReader CreateReader(string file)
        {
            return XmlReader.Create(file);
        }
        public static void CreatePartitions(string file, string xpath)
        {
            PathQuery pathQuery = new PathQuery(file, null);
            var nodes = pathQuery.SelectElements(xpath, RemoveComments);
            Partition(nodes);
            var pairs = nodes.Reverse().Skip(1).Reverse().Zip(nodes.Skip(1), (x, y) => Tuple.Create(x, y)).ToList();
            var equals = pairs.Select(x => XNode.DeepEquals(x.Item1, x.Item2)).ToList();
            Console.WriteLine("{0}", string.Join(", ", equals.Select(x => x.ToString())));
        }

        //public static void CreatePartitions(string file, string xpath)
        //{
        //    CreateReader(file).TryExe(reader =>
        //    {
        //        XElement.Load(reader).TryExe(root =>
        //        {
        //            RemoveComments(root);
        //            IEnumerable<XNode> nodes = root.XPathSelectElements(xpath, NamespaceUtils.CreateXmlNamespaceManager(reader, root)).ToList();
        //            Partition(nodes);
        //            var pairs = nodes.Reverse().Skip(1).Reverse().Zip(nodes.Skip(1), (x, y) => Tuple.Create(x, y)).ToList();
        //            var equals = pairs.Select(x => XNode.DeepEquals(x.Item1, x.Item2)).ToList();
        //            Console.WriteLine("{0}", string.Join(", ", equals.Select(x => x.ToString())));
        //        });
        //    });

        //}

        private static XElement RemoveComments(XElement root)
        {
            var comments = root.DescendantNodes().OfType<XComment>().ToList();
            foreach (var c in comments)
            {
                c.Remove();
            }
            return root;
        }
    }
}
