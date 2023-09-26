using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace Sinyi.Caching
{
    public static class JsonExtension
    {
        public static string Serialize(this object item, bool compression = false)
        {
            // Requirement
            if (item == null) return string.Empty;

            // Convert
            var result = JsonSerializer.Serialize(item);
            if (compression == true) result = StringCompress.Compress(result);

            // Return
            return result;
        }

        public static T Deserialize<T>(this string serializedObject, bool decompression = false)
        {
            #region Contracts

            if (serializedObject == null) throw new ArgumentException(nameof(serializedObject));

            #endregion

            try
            {
                // Decompression
                if (decompression == true) serializedObject = StringCompress.Decompress(serializedObject);

                // Deserialize
                var result = JsonSerializer.Deserialize<T>(serializedObject);

                // Return
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
