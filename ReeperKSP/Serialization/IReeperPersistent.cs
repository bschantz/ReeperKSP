namespace ReeperKSP.Serialization
{
    public interface IReeperPersistent
    {
        // Serialize bits that aren't handled via field surrogates here. DO NOT serializer.Serialize(this)!
        // That's nonsensical and will lead to stack overflows
        void DuringSerialize(IConfigNodeSerializer serializer, ConfigNode node);
        void DuringDeserialize(IConfigNodeSerializer serializer, ConfigNode node);
    }
}
