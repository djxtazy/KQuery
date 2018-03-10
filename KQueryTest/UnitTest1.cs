using System;
using KQuery;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KQueryTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CheckIfMemberOfSecurityGroup()
        {
            string[] args = { "+SG", "AF-C-eBuilder,AF-C-Domain Computers" };

            // assert
            int expected = 0;

            // actual
            int actual = Program.Main(args);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CheckIfMemberOfSecurityGroup2()
        {
            string[] args = { "+SG", "AF-C-eBuilder,AF-C-Random Group" };

            // assert
            int expected = 1;

            // actual
            int actual = Program.Main(args);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CheckIfNotMemberOfSecurityGroup()
        {
            string[] args = { "-SG", "AF-C-Random Group" };

            // assert
            int expected = 0;

            // actual
            int actual = Program.Main(args);

            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void CheckIfNotMemberOfSecurityGroup2()
        {
            string[] args = { "-SG", "AF-Random Groups,AF-C-Domain Computers" };

            // assert
            int expected = 1;

            // actual
            int actual = Program.Main(args);

            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void CheckComputerName()
        {
            string[] args = { "+CN", "AF-154649" };

            // assert
            int expected = 0;

            // actual
            int actual = Program.Main(args);

            Assert.AreEqual(expected, actual);

        }

        public void CheckComputerNameInList()
        {
            string[] args = { "+CN", "AF-SDfsdf,AF-154648" };

            // assert
            int expected = 1;

            // actual
            int actual = Program.Main(args);

            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void CheckNotComputerName()
        {
            string[] args = { "-CN", "AF-154649" };

            // assert
            int expected = 1;

            // actual
            int actual = Program.Main(args);

            Assert.AreEqual(expected, actual);

        }

        public void CheckNotComputerNameList()
        {
            string[] args = { "-CN", "AF-234234,AF-154648" };

            // assert
            int expected = 0;

            // actual
            int actual = Program.Main(args);

            Assert.AreEqual(expected, actual);

        }

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
