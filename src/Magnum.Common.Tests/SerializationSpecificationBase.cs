namespace Magnum.Common.Tests
{
    using System.IO;
    using System.Runtime.Serialization;
    using Serialization;

    public class SerializationSpecificationBase :
        SpecificationBase
    {
        public byte[] Serialize<T>(T obj) where T : new()
        {
            byte[] bytes;
            using (var storage = new MemoryStream())
            using (var binaryWriter = new BinaryWriter(storage))
            {
                var writer = new BinarySerializationWriter(binaryWriter);

                SerializationUtil<T>.Serialize(writer, obj);

                binaryWriter.Flush();
                binaryWriter.Close();

                storage.Flush();
                bytes = storage.ToArray();

                storage.Close();
            }
            return bytes;
        }

        public T Deserialize<T>(byte[] bytes) where T : class, new()
        {
            T obj = null;

            using (var output = new MemoryStream(bytes))
            using (var binaryReader = new BinaryReader(output))
            {
                var writer = new BinarySerializationReader(binaryReader);


                for (int i = 0; i < 300000; i++)
                {
                    output.Seek(0, SeekOrigin.Begin);

                    obj = (T) FormatterServices.GetUninitializedObject(typeof (T));
                    SerializationUtil<T>.Deserialize(writer, obj);

                }

            }

            return obj;
        }
    }
}