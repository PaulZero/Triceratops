using Microsoft.VisualStudio.TestTools.UnitTesting;
using Triceratops.Libraries.Helpers;

namespace Triceratops.Libraries.Tests.Helpers
{
    [TestClass]
    public class DictionaryHelperTest
    {
        [TestMethod]
        public void TestValuesAreCorrectlyExtractedFromObject()
        {
            var obj = new
            {
                name = "Dave",
                age = 17,
                lovesPretzels = false
            };

            var dictionary = DictionaryHelper.ObjectToDictionary(obj);

            Assert.IsTrue(dictionary.ContainsKey("name"));
            Assert.IsTrue(dictionary.ContainsKey("age"));
            Assert.IsTrue(dictionary.ContainsKey("lovesPretzels"));

            Assert.AreEqual("Dave", dictionary["name"]);
            Assert.AreEqual(17, dictionary["age"]);
            Assert.AreEqual(false, dictionary["lovesPretzels"]);
        }
    }
}
