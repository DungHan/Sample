using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Sinyi.Caching
{
    public class StringCompress
    {
        public static string Compress(string text)
        {
            #region Contracts

            if (string.IsNullOrEmpty(text) == true) throw new ArgumentException(nameof(text));

            #endregion

            // Fields
            var buffer = Encoding.UTF8.GetBytes(text);
            var lengthBytes = BitConverter.GetBytes((int)buffer.Length);

            // Write
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(lengthBytes, 0, lengthBytes.Length);

                // Compress
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    gZipStream.Write(buffer, 0, buffer.Length);
                    gZipStream.Flush();

                    // Return
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }

        public static string Decompress(string text)
        {
            #region Contracts

            if (string.IsNullOrEmpty(text) == true) throw new ArgumentException(nameof(text));

            #endregion

            // Fields
            var gZipBuffer = Convert.FromBase64String(text);
            int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
            var buffer = new byte[dataLength];

            // Write
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);
                memoryStream.Position = 0;

                // Decompress
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    int totalRead = 0;
                    while (totalRead < buffer.Length)
                    {
                        int bytesRead = gZipStream.Read(buffer, totalRead, buffer.Length - totalRead);
                        if (bytesRead == 0) break;
                        totalRead += bytesRead;
                    }

                    // Return
                    return Encoding.UTF8.GetString(buffer);
                }
            }
        }
    }
}
