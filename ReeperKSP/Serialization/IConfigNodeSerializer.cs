namespace ReeperKSP.Serialization
{
    public interface IConfigNodeSerializer
    {
        ConfigNode CreateConfigNodeFromObject(object target);

        void WriteObjectToConfigNode(ref object source, ConfigNode config);
        void WriteObjectToConfigNode<T>(ref T source, ConfigNode config);
        void LoadObjectFromConfigNode<T>(ref T target, ConfigNode config);

        ISerializerSelector SerializerSelector { get; set; }
    }
}
