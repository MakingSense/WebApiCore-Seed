using System.IO;

namespace Seed.Api.IntegrationTests.Fake
{
    public static class FileFaker
    {
        /// <summary>
        /// Creates the files that a server needs in order to run
        /// </summary>
        public static void Fake()
        {
            Fake(Path.Combine(Directory.GetCurrentDirectory(), "testhost.xml"), "<?xml version=\"1.0\"?><doc></doc>");
            Fake(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), "{}");
        }

        /// <summary>
        /// Creates a file on the specified path, with the specified content
        /// </summary>
        /// <param name="path">a path with the file at the end</param>
        /// <param name="content">internal content</param>
        public static void Fake(string path, string content)
        {
            var file = new FileInfo(path);
            if (!file.Exists)
            {
                using (var fileStream = file.Create())
                {
                    using (var streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.WriteLine(content);
                        streamWriter.Close();
                    }

                    fileStream.Close();
                }
            }
        }
    }
}
