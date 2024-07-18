using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using TaskManager.Common.Models;


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

        [TestMethod()]
        public void CreateUserTest()
        {
            var service = new UsersRequestService();
            var token = service.GetToken("admin@admin.com", "qwerty123");

            var userTest = new UserModel("TestName", "TestSurname", "test@test.com", "qwerty123", UserRole.User, "88005553535");

            var result = service.CreateUser(token, userTest).Result;

            Assert.AreEqual(HttpStatusCode.OK, result);
        }

        [TestMethod()]
        public void GetAllUsersTest2()
        {
            var service = new UsersRequestService();
            var token = service.GetToken("admin@admin.com", "qwerty123");

            var result = service.GetAllUsers(token);

            Console.WriteLine(result.Count);

            Assert.AreNotEqual(0, result.Count);
        }

        [TestMethod()]
        public void DeleteUserTest()
        {
            var service = new UsersRequestService();
            var token = service.GetToken("admin@admin.com", "qwerty123");

            var result = service.DeleteUser(token, 15).Result;

            Assert.AreEqual(HttpStatusCode.OK, result);
        }

        [TestMethod()]
        public void CreateMultipleUsersTest()
        {
            var service = new UsersRequestService();
            var token = service.GetToken("admin@admin.com", "qwerty123");

            var userTest1 = new UserModel("TestName", "TestSurname", "test@test.com", "qwerty123", UserRole.User, "88005553535");
            var userTest2 = new UserModel("TestName2", "TestSurname2", "test2@test.com", "qwerty123", UserRole.Editor, "88005550000");
            var userTest3 = new UserModel("TestName3", "TestSurname3", "test3@notest.com", "qwerty123", UserRole.User, "88000000000");

            var result = service.CreateMultipleUsers(token, [userTest1, userTest2, userTest3]).Result;

            Assert.AreEqual(HttpStatusCode.OK, result);
        }

        [TestMethod()]
        public void UpdateUserTest()
        {
            var service = new UsersRequestService();
            var token = service.GetToken("admin@admin.com", "qwerty123");

            var userTest3 = new UserModel("TestName3", "TestSurname3", "test3@truetest.com", "qwerty123", UserRole.User, "88123000000");
            userTest3.Id = 18;

            var result = service.UpdateUser(token, userTest3).Result;

            Assert.AreEqual(HttpStatusCode.OK, result);
        }
    }
}