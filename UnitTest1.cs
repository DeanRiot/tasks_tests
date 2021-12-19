using Xunit;
using TasksApi.Models;
using Microsoft.EntityFrameworkCore;
using TasksApi.App.DataBase;
using TasksApi.App.Models;

namespace ApiTests
{
    public class UnitTest1
    {
        LoginDBWork _loginTable;
        TasksDBWork _taskTable;
        void setup(string test)
        {
            DbContextOptions<tasks_dbContext> options = new DbContextOptionsBuilder<tasks_dbContext>()
                            .UseInMemoryDatabase(databaseName: $"{test}_db")
                            .Options;
            _loginTable = new LoginDBWork(options);
            _taskTable = new TasksDBWork(options);
        }
        [Fact]
        public void AddUserTest()
        {
            setup("AddUser");
            _loginTable.AddUser(new TasksApi.App.Models.LoginModel()
            {
                Login = "user",
                Password = "user"
            });
            var id = _loginTable.GetID(new TasksApi.App.Models.LoginModel()
            {
                Login = "user",
                Password = "user"
            });
            Assert.True(id.Equals(1));
        }
        private int AddUser(TasksApi.App.Models.LoginModel user)
        {
            _loginTable.AddUser(user);
            return _loginTable.GetID(user);
        }

        [Fact]
        public void GetAllTasksTest()
        {
            setup("GetAllTasks");
            var id = AddUser(new TasksApi.App.Models.LoginModel()
            {
                Login = "user",
                Password = "user"
            });

            for (int i = 0; i < 10; i++)
            {
                _taskTable.AddTask(id, new TaskModel()
                {
                    ID = 0,
                    Header = "Задача 1",
                    Text = "Новая задача",
                    EndDate = System.DateTime.Now,
                    Status = false
                });
            }

            var tasks = _taskTable.GetAllTasks(id);
            Assert.True(tasks.Count.Equals(10));
        }

        [Fact]
        public void GetTasksTest()
        {
            setup("GetActual");
            
            var id = AddUser(new TasksApi.App.Models.LoginModel()
            {
                Login = "user",
                Password = "user"
            });

            for (int i = 0; i < 5; i++)
            {
                _taskTable.AddTask(id, new TaskModel()
                {
                    ID = 0,
                    Header = $"Задача {i}",
                    Text = "Новая задача",
                    EndDate = System.DateTime.Now.AddDays(-10),
                    Status = false
                });
            }

            for (int i = 0; i < 5; i++)
            {
                _taskTable.AddTask(id, new TaskModel()
                {
                    ID = 0,
                    Header = $"Задача {i+5}",
                    Text = "Новая задача",
                    EndDate = System.DateTime.Today,
                    Status = false
                });
            }

            var tasks = _taskTable.GetTodayTasks(id);
            Assert.True(tasks.Count.Equals(5));

             tasks = _taskTable.GetInProcess(id);
            Assert.True(tasks.Count.Equals(5));

            tasks = _taskTable.GetOldTasks(id);
            Assert.True(tasks.Count.Equals(5));

            tasks = _taskTable.GetEnded(id);
            Assert.True(tasks.Count.Equals(0));

            tasks = _taskTable.GetOldTasks(id);
            _taskTable.SetStatus(id, (int) tasks[0].ID, true);
            tasks = _taskTable.GetEnded(id);
            Assert.True(tasks.Count.Equals(1));
        }
    }
}