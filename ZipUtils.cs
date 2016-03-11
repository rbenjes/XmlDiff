using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Utils;

namespace XmlDiff
{
    public class ZipUtils
    {
        private static readonly string Guid = "E55764A1-CB32-469C-8B5F-9D9A0C49C856";

        public static Stream Unzip(string file)
        {
            return File.OpenRead(file).TryEval(stream =>
            {
                using (ZipFile zipFile = new ZipFile(stream, Guid))
                {
                    using (var zipStream = zipFile.GetInputStream(zipFile[0]))
                    {
                        MemoryStream result = new MemoryStream();
                        zipStream.CopyTo(result);
                        result.Seek(0, SeekOrigin.Begin);
                        return result;
                    }
                }
            });
        }
    }
}
