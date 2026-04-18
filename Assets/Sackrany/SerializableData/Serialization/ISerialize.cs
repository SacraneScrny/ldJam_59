namespace Sackrany.SerializableData.Serialization
{
    internal interface ISerialize
    {
        object Get();
        void Set(object value);
    }
}
