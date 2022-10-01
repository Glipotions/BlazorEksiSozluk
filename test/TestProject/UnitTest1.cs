using BlazorSozluk.Common.Infrastructure;

namespace TestProject
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
            Assert.Pass();
        }

        [Test]
        public void Test2()
        {

            var password=PasswordEncryptor.Encrpt("glipotions");
            string pass = "CC97461715C81419D721D390F6991716";
            Assert.Pass();
        }
    }
}