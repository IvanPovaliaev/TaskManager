using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Client.Models;
using TaskManager.Common.Models;
using System.Net;
using System;

namespace TaskManager.Client.Services.Tests
{
    [TestClass()]
    public class ProjectsRequestServiceTests
    {
        private AuthToken _authToken;
        private ProjectsRequestService _service;

        public ProjectsRequestServiceTests()
        {
            _authToken = new UsersRequestService().GetToken("admin@admin.com", "qwerty123");
            _service = new ProjectsRequestService();
        }

        [TestMethod()]
        public void GetAllProjectsTest()
        {
            var result = _service.GetAllProjects(_authToken).Result;

            Console.WriteLine(result.Count);

            Assert.AreNotEqual(0, result.Count);
        }

        [TestMethod()]
        public void GetProjectByIdTest()
        {
            var result = _service.GetProjectById(_authToken, 2).Result;

            Assert.AreNotEqual(null, result);
        }

        [TestMethod()]
        public void CreateProjectTest()
        {
            var project = new ProjectModel("UnitTest", "Project for UnitTest", ProjectStatus.InProgress);
            project.AdminId = 1;
            var result = _service.CreateProject(_authToken, project).Result;
            Assert.AreEqual(HttpStatusCode.OK, result);
        }

        [TestMethod()]
        public void UpdateProjectTest()
        {
            var project = new ProjectModel("UnitTest v0.2", "Project for UnitTest v0.2", ProjectStatus.Suspended);
            project.Id = 6;
            var result = _service.UpdateProject(_authToken, project).Result;
            Assert.AreEqual(HttpStatusCode.OK, result);
        }

        [TestMethod()]
        public void DeleteProjectTest()
        {
            var result = _service.DeleteProject(_authToken, 6).Result;

            Assert.AreEqual(HttpStatusCode.OK, result);
        }

        [TestMethod()]
        public void AddUsersToProjectTest()
        {
            var result = _service.AddUsersToProject(_authToken, 5, [16, 17, 18]).Result;

            Assert.AreEqual(HttpStatusCode.OK, result);
        }

        [TestMethod()]
        public void RemoveUsersFromProjectTest()
        {
            var result = _service.RemoveUsersFromProject(_authToken, 5, [16, 17, 18]).Result;

            Assert.AreEqual(HttpStatusCode.OK, result);
        }
    }
}