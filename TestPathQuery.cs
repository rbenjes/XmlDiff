using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XmlDiff
{
    class CT_MMAntwort
    {
        public int FragenID { get; set; }
        public string Erfuellt { get; set; }
        public string Kosten { get; set; }
        public string Langantwort { get; set; }
        public string Kurzantwort { get; set; }
    }
    public class TestPathQuery
    {
        public static void Test()
        {
            var files = System.IO.Directory.GetFiles(@"D:\Projects\neu\MMOffice\dev\src\main\Common\MM.Office.Common\Data\CV\Bedingungen\", "BedingungenAntwortenTap*.zip");
            foreach (var file in files)
                Test(file);
        }

        private static void Test(string file)
        {
            var fileName = System.IO.Path.GetFileNameWithoutExtension(file);
            // /ArrayOfCT_Ergebniseintrag/CT_Ergebniseintrag/Antworten[count(CT_MMAntwort[FragenID='1']/FragenID)>2]/../TarifauspraegungID/node()
            var atts = new List<XAttribute>() { new XAttribute(XName.Get("a"), "http://schemas.microsoft.com/2003/10/Serialization/Arrays") };
            string path = "//ns:CT_Ergebniseintrag/ns:Antworten/ns:CT_MMAntwort";
            PathQuery pathQuery = new PathQuery(XmlReader.Create(ZipUtils.Unzip(file)), atts);
            var result = pathQuery.SelectElements(path, null);
            var tapRows = result.GroupBy(x => x.Parent).SelectMany(x => CreateTapRows(x, pathQuery)).ToList();
            foreach (var tapRow in tapRows)
            {
                Console.WriteLine("{0};{1}", fileName, tapRow);
            }
        }


        private static CT_MMAntwort CreateAntwort(XElement x, PathQuery pathQuery)
        {
            return new CT_MMAntwort()
            {
                FragenID = (int)x.XPathSelectElement("ns:FragenID", pathQuery.NamespaceManager),
                Erfuellt = (string)x.XPathSelectElement("ns:Erfuellt", pathQuery.NamespaceManager),
                Kosten = (string)x.XPathSelectElement("ns:Kosten", pathQuery.NamespaceManager),
                Langantwort = (string)x.XPathSelectElement("ns:Langantwort", pathQuery.NamespaceManager),
                Kurzantwort = (string)x.XPathSelectElement("ns:Kurzantwort", pathQuery.NamespaceManager),
            };
        }

        private static List<string> CreateTapRows(IGrouping<XElement, XElement> group, PathQuery pathQuery)
        {
            var tapElem = group.Key.Parent;
            var tap = (int)tapElem.XPathSelectElement("ns:TarifauspraegungID/a:string", pathQuery.NamespaceManager);
            var g = group.ToList();
            var antworten = g.Select(x => CreateAntwort(x, pathQuery)).GroupBy(x => x.FragenID).Select(x => x.ToList()).Where(x => x.Count > 2).ToList();
            var result = antworten.Select(antwGroup => string.Join(";", GetCsvRow(antwGroup, tap))).ToList();
            return result;
        }

        private static IEnumerable<string> GetCsvRow(List<CT_MMAntwort> antwGroup, int tap)
        {
            yield return tap.ToString();
            yield return antwGroup[0].FragenID.ToString();
            yield return antwGroup.Count.ToString();
            yield return string.Join(",", antwGroup.Select(x => x.Erfuellt));
            yield return string.Join(",", antwGroup.Select(x => x.Kosten));
            yield return string.Join(",", antwGroup.Select(x => x.Langantwort));
            yield return string.Join(",", antwGroup.Select(x => x.Kurzantwort));
        }
    }
}
