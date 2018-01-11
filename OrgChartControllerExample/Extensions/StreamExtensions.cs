using System.IO;

namespace OrgChartControllerExample.Extensions {
    internal static class StreamExtensions {
        public static string GetString(this MemoryStream memoryStream) {
            memoryStream.Seek(0L, SeekOrigin.Begin);
            using (var streamReader = new StreamReader(memoryStream)) {
                return streamReader.ReadToEnd();
            }
        }
    }
}
