using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Consumer.ServiceTests
{
    class FileLoader
    {
        public static readonly string FeedJson;

        static FileLoader()
        {
            FeedJson = LoadFile("Consumer.ServiceTests.json.feed.json");
        }

        public static void BOOO()
        {
        }


        private static string LoadFile(string resourceName)
        {
            var txt = string.Empty;

            var a = typeof(FileLoader).GetTypeInfo().Assembly;

            using (var str = a.GetManifestResourceStream(resourceName))
            {
                using (var r = new StreamReader(str))
                {
                    txt = r.ReadToEnd();
                }
            }
            return txt;
        }

        private static IEnumerable<string> LoadFiles(string resourceFolder)
        {
            var files = new List<string>();

            var txt = string.Empty;

            var a = typeof(FileLoader).GetTypeInfo().Assembly;

            var resourceNames = a.GetManifestResourceNames();

            foreach (var resourceName in resourceNames)
            {
                if (resourceName.StartsWith(resourceFolder))
                {
                    using (var str = a.GetManifestResourceStream(resourceName))
                    {
                        using (var r = new StreamReader(str))
                        {
                            txt = r.ReadToEnd();
                            files.Add(txt);
                        }
                    }
                }
            }

            return files;
        }
    }
}