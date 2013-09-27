using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    {
                        ConvertParaUTF8(path);
                    }
                });
                Console.WriteLine("Finalizado do processo de inclusão de assinatura UTF-8.");
            }
            else
            {
                Console.WriteLine("O diretório: {0} não existe.", directoryPath);
            }
        }

        public static void ConvertParaUTF8(string filePath)
        {
            using (var memoryStream = new MemoryStream())
            {
                var utfSignature = new byte[] { 0xef, 0xbb, 0xbf };
                foreach (var i in utfSignature)
                    memoryStream.WriteByte(i);
                var hasUtf8Signature = true;
                using (var file = new FileStream(filePath, FileMode.Open))
                {
                    var signatureBuffer = new byte[3];
                    file.Read(signatureBuffer, 0, 3);

                    for (int i = 0; i < 3; i++)
                        hasUtf8Signature = hasUtf8Signature && utfSignature[i] == signatureBuffer[i];

                    if (!hasUtf8Signature)
                    {
                        Console.WriteLine("\tIncluindo assinatura UTF-8 no arquivo: {0}", filePath);
                        var fileBuffer = new byte[file.Length];
                        file.Seek(0, SeekOrigin.Begin);
                        file.Read(fileBuffer, 0, (int)file.Length);
                        foreach (var readByte in fileBuffer)
                        {
                            var utf8Char = Encoding.UTF8.GetChars(new[] { readByte }).FirstOrDefault();
                            var @char = Convert.ToChar(readByte);
                            if (utf8Char != @char)
                            {
                                var utf8Bytes = Encoding.UTF8.GetBytes(new[] { @char });
                                foreach (var utf8Byte in utf8Bytes)
                                    memoryStream.WriteByte(utf8Byte);
                            }
                            else
                                memoryStream.WriteByte(readByte);
                        }
                    }
                }
                if (!hasUtf8Signature)
                {
                    using (var newFile = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        memoryStream.CopyTo(newFile);
                    }
                    Console.WriteLine("\tAssinatura incluida no arquivo: {0}", filePath);
                }
            }
        }
    }
}
