using System.IO;

namespace WebApiCoreSeed.WebApi.IntegrationTests.Fake
{
    public static class SwaggerXmlFaker
    {
        /// <summary>
        /// Creates a swagger xml file on the excecution path with an empty doc tag
        /// </summary>
        public static void Fake()
        {
            Fake("<?xml version=\"1.0\"?><doc></doc>");
        }

        /// <summary>
        /// Creates a swagger xml file on the excecution path with the xml provided
        /// </summary>
        /// <param name="xml">string formmated as xml</param>
        public static void Fake(string xml)
        {
            Fake(Path.Combine(Directory.GetCurrentDirectory(), "testhost.xml"), xml);
        }

        /// <summary>
        /// Creates a file on the specified path, with the specified xml
        /// </summary>
        /// <param name="path">a path with the file at the end</param>
        /// <param name="xml">string formmated as xml</param>
        public static void Fake(string path, string xml)
        {
            var xmlSwagger = new FileInfo(path);
            if (!xmlSwagger.Exists)
            {
                using (var fs = xmlSwagger.Create())
                {
                    using (var asd = new StreamWriter(fs))
                    {
                        asd.WriteLine(xml);
                        asd.Close();
                    }

                    fs.Close();
                }
            }
        }
    }
}
