using ReeperKSP.Serialization;

namespace ReeperKSPUnitTests.TestData
{
    public class DefaultConstructableType : IReeperPersistent
    {
        public DefaultConstructableType()
        {
                
        }

        public void DuringSerialize(IConfigNodeSerializer formatter, ConfigNode node)
        {
                
        }

        public void DuringDeserialize(IConfigNodeSerializer formatter, ConfigNode node)
        {

        }
    }
}