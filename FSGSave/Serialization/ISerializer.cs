using System.IO;

namespace FSGSave
{
    public interface ISerializer<T>
    {
        public void Serialize(Stream stream, T value);

        public T Deserialize(Stream stream);
    }
}