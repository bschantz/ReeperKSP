using ReeperKSP.Serialization;

namespace ReeperKSPUnitTests.TestData
{
    public class SimplePersistentObject
    {
        [ReeperPersistent]
        public string PersistentField = "Value";

        public string NonpersistentField = "Anonymous";
    }
}
