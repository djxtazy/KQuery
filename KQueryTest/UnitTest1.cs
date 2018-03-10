using System;
using KQuery;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KQueryTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CheckIfX64()
        {
            string[] args = { "/X64" };

            // assert
            int expected = 0;

            // actual
            int actual = Program.Main(args);

            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void CheckIfX86()
        {
            string[] args = { "/X86" };

            // assert
            int expected = 1;

            // actual
            int actual = Program.Main(args);

            Assert.AreEqual(expected, actual);

        }

    }
}
