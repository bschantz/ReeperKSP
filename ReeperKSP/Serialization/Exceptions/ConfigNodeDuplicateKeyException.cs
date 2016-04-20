namespace ReeperKSP.Serialization.Exceptions
{
    public class ConfigNodeDuplicateKeyException : ReeperSerializationException
    {
        public ConfigNodeDuplicateKeyException(string key) : base("ConfigNode already contains key \"" + key + "\"")
        {
            
        }

        public ConfigNodeDuplicateKeyException(string key, ConfigNode node)
            : base("ConfigNode already contains key \"" + key + "\": " + node.ToString().Replace("{", "{{").Replace("}", "}}"))
        {
        }
    }
}
