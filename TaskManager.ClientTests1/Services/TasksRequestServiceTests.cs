using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using TaskManager.Client.Models;
using TaskManager.Common.Models;
using System.Net;

namespace TaskManager.Client.Services.Tests
{
    [TestClass()]
    public class TasksRequestServiceTests
    {
        private AuthToken _authToken;
        private TasksRequestService _service;
        public TasksRequestServiceTests()
        {
            _authToken = new UsersRequestService().GetToken("admin@admin.com", "qwerty123").Result;
            _service = new TasksRequestService();
        }

        [TestMethod()]
        public void GetTasksForCurrentUserTest()
        {
            var result = _service.GetTasksForCurrentUser(_authToken).Result;

            Console.WriteLine(string.Join("\n", result.Select(d => d.Name)));

            Assert.AreNotEqual(0, result.Count);
        }

        [TestMethod()]
        public void GetTaskByIdTest()
        {
            var result = _service.GetTaskById(_authToken, 1).Result;

            Console.WriteLine(result.Name);

            Assert.AreNotEqual(null, result);
        }

        [TestMethod()]
        public void GetTasksByDeskTest()
        {
            var result = _service.GetTasksByDesk(_authToken, 1).Result;

            Console.WriteLine(string.Join("\n", result.Select(d => d.Name)));

            Assert.AreNotEqual(null, result);
        }

        [TestMethod()]
        public void CreateTaskTest()
        {
            var task = new TaskModel("UnitTestTask", "Task for UnitTest", DateTime.Now, DateTime.Now, "UnitTestColumn");
            task.DeskId = 3;
            task.ExecutorId = 1;
            task.CreatorId = 1;

            var result = _service.CreateTask(_authToken, task).Result;
            Assert.AreEqual(HttpStatusCode.OK, result);
        }

        [TestMethod()]
        public void UpdateDeskTest()
        {
            var task = new TaskModel("UnitTestTask v1.1", "Task for UnitTest v1.1", DateTime.Now, DateTime.Now, "In Progress");
            task.DeskId = 3;
            task.Id = 3;
            task.ExecutorId = 2;
            task.CreatorId = 1;

            var result = _service.UpdateTask(_authToken, task).Result;
            Assert.AreEqual(HttpStatusCode.OK, result);
        }

        [TestMethod()]
        public void DeleteTaskByIdTest()
        {
            var result = _service.DeleteTaskById(_authToken, 3).Result;

            Assert.AreEqual(HttpStatusCode.OK, result);
        }
    }
}