using ReeperKSP.Serialization;

namespace ReeperKSPUnitTests.TestData
{
    class SimplePersistentObjectNative : IReeperPersistent
    {
        public void DuringSerialize(IConfigNodeSerializer formatter, ConfigNode node)
        {
            
        }

        public void DuringDeserialize(IConfigNodeSerializer formatter, ConfigNode node)
        {

        }
    }
}
