using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace TaskManager.Client.Services.Tests
{
    [TestClass()]
    public class UsersRequestServiceTests
    {
        [TestMethod()]
        public void GetTokenTest()
        {
            var token = new UsersRequestService().GetToken("admin@admin.com", "qwerty123");
            Console.WriteLine(token.access_token);
            Assert.IsNotNull(token);
        }
    }
}