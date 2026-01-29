using Masuit.Tools.DateTimeExt;
using System.Data;

namespace ZR.NUnit
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var timeStamp = DateTime.Now.GetTotalSeconds();
            Console.WriteLine(timeStamp);
            Assert.Pass();
        }
    }
}
