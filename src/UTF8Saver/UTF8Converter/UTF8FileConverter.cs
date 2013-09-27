using System;
using System.IO;
using System.Linq;
using System.Text;

namespace UTF8Converter
{
    public class UTF8FileConverter
    {
        private static byte[] _utfSignature;

        public static void ConvertToUTF8(string filePath)
        {
            using (var memoryStream = new MemoryStream())
            {
                InsertUTF8Signature(memoryStream);
                bool hasUtf8Signature;
                using (var file = new FileStream(filePath, FileMode.Open))
                {
                    hasUtf8Signature = HasUtf8Signature(file, _utfSignature);

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

        private static void InsertUTF8Signature(MemoryStream memoryStream)
        {
            _utfSignature = new byte[] {0xef, 0xbb, 0xbf};
            foreach (var i in _utfSignature)
                memoryStream.WriteByte(i);
        }

        private static bool HasUtf8Signature(Stream file, byte[] utfSignature)
        {
            var hasUtf8Signature = true;
            var signatureBuffer = new byte[3];
            file.Read(signatureBuffer, 0, 3);

            for (int i = 0; i < 3; i++)
                hasUtf8Signature = hasUtf8Signature && utfSignature[i] == signatureBuffer[i];
            return hasUtf8Signature;
        }
    }
}