using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using TaskManager.Common.Models;
using TaskManager.Client.Models;


namespace TaskManager.Client.Services.Tests
{
    [TestClass()]
    public class UsersRequestServiceTests
    {
        private AuthToken _token { get; set; }
        private UsersRequestService _service {  get; set; }
        public UsersRequestServiceTests()
        {
            _service = new UsersRequestService();
            _token = _service.GetTokenAsync("admin@admin.com", "qwerty123").Result;
        }

        [TestMethod()]
        public void GetTokenTest()
        {
            var token = _service.GetTokenAsync("admin@admin.com", "qwerty123").Result;
            Console.WriteLine(token.access_token);
            Assert.IsNotNull(token);
        }

        [TestMethod()]
        public void CreateUserTest()
        {
            var userTest = new UserModel("TestName", "TestSurname", "test@test.com", "qwerty123", UserRole.User, "88005553535");

            var result = _service.CreateUserAsync(_token, userTest).Result;

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod()]
        public void GetAllUsersTest2()
        {
            var result = _service.GetAllUsersAsync(_token).Result;

            Console.WriteLine(result.Count);

            Assert.AreNotEqual(0, result.Count);
        }

        [TestMethod()]
        public void DeleteUserTest()
        {
            var result = _service.DeleteUserAsync(_token, 15).Result;

            Assert.AreEqual(HttpStatusCode.OK, result);
        }

        [TestMethod()]
        public void CreateMultipleUsersTest()
        {
            var userTest1 = new UserModel("TestName", "TestSurname", "test@test.com", "qwerty123", UserRole.User, "88005553535");
            var userTest2 = new UserModel("TestName2", "TestSurname2", "test2@test.com", "qwerty123", UserRole.Editor, "88005550000");
            var userTest3 = new UserModel("TestName3", "TestSurname3", "test3@notest.com", "qwerty123", UserRole.User, "88000000000");

            var result = _service.CreateMultipleUsersAsync(_token, [userTest1, userTest2, userTest3]).Result;

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod()]
        public void UpdateUserTest()
        {
            var userTest3 = new UserModel("TestName3", "TestSurname3", "test3@truetest.com", "qwerty123", UserRole.User, "88123000000");
            userTest3.Id = 18;

            var result = _service.UpdateUserAsync(_token, userTest3).Result;

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod()]
        public void GetProjectUserAdminTest()
        {
            var id = _service.GetProjectUserAdmin(_token, 1).Result;

            Assert.AreEqual(2, id);
        }
    }
}