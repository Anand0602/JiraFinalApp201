using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using JiraFinalApp201.Controllers;
using JiraFinalApp201.Models.Database;
using JiraFinalApp201.Services.Tasks;
using JiraFinalApp201.Services.Projects;
using JiraFinalApp201.Services.User;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TestProject1
{
    public class SimpleTaskControllerTest
    {
        [Fact]
        public async Task Details_ShouldRedirectToLogin_WhenUserNotLoggedIn()
        {
            // Arrange
            var taskService = new Mock<ITaskService>();
            var projectService = new Mock<IProjectService>();
            var userService = new Mock<IUserService>();
            var db = new Mock<JiraFinalApp201Db>();
            var controller = new TaskController(db.Object, taskService.Object, projectService.Object, userService.Object);

            // Simulate no session (Session is not null, but contains no UserId)
            var context = new DefaultHttpContext();
            context.Session = new FakeSession(); // <-- This is the fix!
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };

            // Act
            var result = await controller.Details(1);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
            Assert.Equal("Account", redirect.ControllerName);
        }

        // Minimal fake session implementation
        private class FakeSession : ISession
        {
            public IEnumerable<string> Keys => new List<string>();
            public string Id => "fake";
            public bool IsAvailable => true;
            public void Clear() { }
            public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
            public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
            public void Remove(string key) { }
            public void Set(string key, byte[] value) { }
            public bool TryGetValue(string key, out byte[] value) { value = null; return false; }
        }
    }
}
