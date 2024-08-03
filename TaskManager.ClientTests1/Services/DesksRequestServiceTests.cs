using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using TaskManager.Client.Models;
using System.Net;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services.Tests
{
    [TestClass()]
    public class DesksRequestServiceTests
    {
        private AuthToken _authToken;
        private DesksRequestService _service;

        public DesksRequestServiceTests()
        {
            _authToken = new UsersRequestService().GetTokenAsync("admin@admin.com", "qwerty123").Result;
            _service = new DesksRequestService();
        }

        [TestMethod()]
        public void GetDesksForCurrentUserTest()
        {
            var result = _service.GetDesksForCurrentUserAsync(_authToken).Result;

            Console.WriteLine(string.Join("\n", result.Select(d => d.Name)));

            Assert.AreNotEqual(0, result.Count);
        }

        [TestMethod()]
        public void GetDeskByIdTest()
        {
            var result = _service.GetDeskByIdAsync(_authToken, 3).Result;

            Console.WriteLine(result.Name);

            Assert.AreNotEqual(null, result);
        }

        [TestMethod()]
        public void GetDeskByProjectTest()
        {
            var result = _service.GetDesksByProjectAsync(_authToken, 5).Result;

            Console.WriteLine(string.Join("\n", result.Select(d => d.Name)));

            Assert.AreNotEqual(null, result);
        }

        [TestMethod()]
        public void CreateDeskTest()
        {
            var desk = new DeskModel("UnitTestDesk", "Desk for UnitTest", true, ["New", "Finished"]);
            desk.ProjectId = 2;
            desk.AuthorId = 1;
            var result = _service.CreateDeskAsync(_authToken, desk).Result;
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod()]
        public void UpdateDeskTest()
        {
            var desk = new DeskModel("UnitTestDesk v1.1", "Desk for UnitTest v1.1", true, ["New", "Finished"]);
            desk.Id = 5;
            var result = _service.UpdateDeskAsync(_authToken, desk).Result;
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod()]
        public void DeleteDeskByIdTest()
        {
            var result = _service.DeleteDeskAsync(_authToken, 5).Result;

            Assert.AreEqual(HttpStatusCode.OK, result);
        }
    }
}