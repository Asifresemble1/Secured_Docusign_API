using System;
using System.IO;

namespace Secured_Docusign_API
{
    internal class DSHelper
    {
        internal static string PrepareFullPrivateKeyFilePath(string fileName)
        {
            const string DefaultRSAPrivateKeyFileName = "private.key.txt"; //docusign_private_key.txt

            var fileNameOnly = Path.GetFileName(fileName);
            if (string.IsNullOrEmpty(fileNameOnly))
            {
                fileNameOnly = DefaultRSAPrivateKeyFileName;
            }

            var filePath = Path.GetDirectoryName(fileName);
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Directory.GetCurrentDirectory();
            }

            return Path.Combine(filePath, fileNameOnly);
        }

        internal static byte[] ReadFileContent(string path)
        {
            return File.ReadAllBytes(path);
        }
    }
}