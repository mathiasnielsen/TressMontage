using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TressMontage.Utilities.Extensions;

namespace TressMontage.Utilities.Tests.Extensions
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void StringExtensions_GetCombined_WindowsPath()
        {
            var knownPath = "hello/im/a/path";
            var expectedPath = "hello\\im\\a\\path";

            var combinedKnownPath = knownPath.GetPlatformSpecificPath();

            Assert.AreEqual(expectedPath, combinedKnownPath);
        }
    }
}
