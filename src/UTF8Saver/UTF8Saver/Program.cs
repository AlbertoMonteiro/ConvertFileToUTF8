using System;
using System.IO;
using System.Linq;
using UTF8Converter;

namespace UTF8Saver
{
    class Program
    {
        static void Main(string[] args)
        {
            string directoryPath;
            if (args != null && args.Any())
            {
                directoryPath = args[0];
            }
            else
            {
                do
                {
                    Console.WriteLine("What the path of folder to be processed: ");
                    directoryPath = Console.ReadLine();
                } while (string.IsNullOrWhiteSpace(directoryPath));
            }

            if (Directory.Exists(directoryPath))
            {
                Console.WriteLine("Starting UTF-8 signature inclusion process");
                var extencoesParaProcessar = ".cs|.js|.css|.scss|.cshtml".Split('|');
                var ignorePattern = @"\obj\|\packages\|\bin\".Split('|');
                var utf8FileConvert = new UTF8FileConverter(extencoesParaProcessar, ignorePattern);
                utf8FileConvert.OnStartingToConvert += filePath => Console.WriteLine("\tAdding UTF-8 signature in file: {0}", filePath);
                utf8FileConvert.OnConvertFinished += filePath => Console.WriteLine("\tIncluded signature in file: {0}", filePath);
                utf8FileConvert.ConvertFilesInDirectory(directoryPath);
                Console.WriteLine("Inclusion process end");
            }
            else
            {
                Console.WriteLine("The path: {0} doesn't exist.", directoryPath);
            }
        }
    }
}
