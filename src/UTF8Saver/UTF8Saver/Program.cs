using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
                    Console.WriteLine("Caminho da pasta para ser processada: ");
                    directoryPath = Console.ReadLine();
                } while (string.IsNullOrWhiteSpace(directoryPath));
            }

            if (Directory.Exists(directoryPath))
            {
                Console.WriteLine("Iniciando processo de inclusão de assinatura UTF-8.");
                var extencoesParaProcessar = ".cs|.js|.css|.scss|.cshtml".Split('|');
                var ignorePattern = @"\obj\|\packages\|\bin\".Split('|');
                Parallel.ForEach(Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories), path =>
                {
                    var fileInfo = new FileInfo(path);

                    var extensaoPermitida = extencoesParaProcessar.Contains(fileInfo.Extension);
                    var diretorioPermitido = !ignorePattern.Any(path.Contains);
                    if (extensaoPermitida && diretorioPermitido)
                        UTF8FileConverter.ConvertToUTF8(path);
                });
                Console.WriteLine("Finalizado do processo de inclusão de assinatura UTF-8.");
            }
            else
            {
                Console.WriteLine("O diretório: {0} não existe.", directoryPath);
            }
        }
    }
}
