using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace TFG.Infraestructure.Helpers
{
    public class JsonDataReader
    {
        public T JsonToObject<T>(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Seek(0, SeekOrigin.Begin);
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                return (T)ser.ReadObject(stream);
            }
        }

        public virtual T ReadDataFromJson<T>(string jsonFileName, Encoding encoding)
        {
            if (File.Exists(jsonFileName) == false)
            {
                throw new FileNotFoundException($"{jsonFileName} file is missing");
            }
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            string jsonString = File.ReadAllText(jsonFileName);
            var result = JsonToObject<T>(encoding.GetBytes(jsonString));

            return result;
        }
    }
}
