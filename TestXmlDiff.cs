using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlDiff
{
    class TestXmlDiff
    {
        enum XPaths
        {
            Schnelleinstieg,
            SchnelleinstiegLvCommand,
            SchnelleinstiegLvHonorarCommand,
        }

        private static Dictionary<XPaths, string> xpaths = new Dictionary<XPaths, string>()
        {
            { XPaths.Schnelleinstieg, "//ns:Grid/telerik:RadRibbonView/telerik:RadRibbonTab/telerik:RadRibbonGroup[@Header='Schnelleinstieg']" },
            { XPaths.SchnelleinstiegLvCommand, "//ns:Grid/telerik:RadRibbonView/telerik:RadRibbonTab/telerik:RadRibbonGroup/telerik:RadRibbonButton[@Command='{Binding SchnelleinstiegLvCommand}']" },
            { XPaths.SchnelleinstiegLvHonorarCommand, "//ns:Grid/telerik:RadRibbonView/telerik:RadRibbonTab/telerik:RadRibbonGroup/telerik:RadRibbonButton[@Command='{Binding SchnelleinstiegLvHonorarCommand}']" },
        };
        public static void Test()
        {
            XmlDiff.CreatePartitions(@"D:\projects\neu\MMOffice\dev\src\main\Client\StartApp\Desktop\DesktopView.xaml", xpaths[XPaths.SchnelleinstiegLvHonorarCommand]);
        }
    }
}
