using ReeperKSP.Extensions;
using ReeperKSPUnitTests.Fixtures;
using Xunit;
using Xunit.Extensions;

namespace ReeperKSPUnitTests.Extensions
{
    public class ConfigNodeExtensionsTests
    {
        [Theory, Fixtures.AutoDomainData]
        public void MatchesContentsOf_WithSameConfigNode_Passes_Test(string nodeName, string subName, string valueName, string value)
        {
            var empty = new ConfigNode(nodeName);
            var hasValues = new ConfigNode(nodeName);

            hasValues.AddValue(valueName, value);

            var hasNodes = new ConfigNode(nodeName);
            hasNodes.AddNode(subName);

            var hasValuesAndNodes = new ConfigNode(nodeName);
            hasValuesAndNodes.AddValue(valueName, value);
            hasValuesAndNodes.AddNode(subName);

            Assert.True(empty.MatchesContentsOf(empty));
            Assert.True(hasValues.MatchesContentsOf(hasValues));
            Assert.True(hasNodes.MatchesContentsOf(hasNodes));
            Assert.True(hasValuesAndNodes.MatchesContentsOf(hasValuesAndNodes));
        }


        [Theory, AutoDomainData]
        public void MatchesContentsOf_WithTwoEmptyConfigNodes_SameName_Passes_Test(string nodeName)
        {
            var config1 = new ConfigNode(nodeName);
            var config2 = new ConfigNode(nodeName);

            Assert.True(config1.MatchesContentsOf(config2));
            Assert.True(config2.MatchesContentsOf(config1));
        }


        [Theory, AutoDomainData]
        public void MatchesContentsOf_WithTwoEmptyConfigNodes_DifferentNames_Fails_Test(string firstName,
            string secondName)
        {
            var config1 = new ConfigNode(firstName);
            var config2 = new ConfigNode(secondName);

            Assert.False(config1.MatchesContentsOf(config2));
            Assert.False(config2.MatchesContentsOf(config1));
        }


        [Theory, AutoDomainData]
        public void MatchesContentsOf_WhenOneHasValueData_OtherDoesNot_BothSameName_Fails_Test(string nodeName, string valueName, string value)
        {
            var source = new ConfigNode(nodeName);
            var target = new ConfigNode(nodeName);

            source.AddValue(valueName, value);

            Assert.False(source.MatchesContentsOf(target));
            Assert.False(target.MatchesContentsOf(source));
        }


        [Theory, AutoDomainData]
        public void MatchesContentsOf_WhenOneHasNodeData_OtherDoesNot_BothSameName_Fails_Test(string nodeName, string subNode)
        {
            var source = new ConfigNode(nodeName);
            source.AddNode(subNode);

            var empty = new ConfigNode(nodeName);

            Assert.False(source.MatchesContentsOf(empty));
            Assert.False(empty.MatchesContentsOf(source));
        }
    }
}
