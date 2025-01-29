using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TaskPlannerWeb.Models;
using TaskModel = TaskPlannerWeb.Models.Task;

namespace TaskPlannerWeb.Controllers
{
    public class TaskController : Controller
    {
        private static List<TaskModel> tasks = new List<TaskModel>();
        private static string filePath = "tasks.json";

        public TaskController()
        {
            LoadTasks();
        }

        public IActionResult Index(string filter)
        {
            var user = AccountController.GetLoggedInUser();
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userTasks = tasks.Where(t => t.UserEmail == user.Email).ToList();

            if (filter == "progress")
            {
                userTasks = userTasks.Where(t => t.Status == "In Progress").ToList();
            }
            else if (filter == "done")
            {
                userTasks = userTasks.Where(t => t.Status == "Done").ToList();
            }

            return View(userTasks);
        }

        [HttpGet]
        public IActionResult AddTask()
        {
            if (AccountController.GetLoggedInUser() == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        public IActionResult AddTask(string title, string description, string deadline)
        {
            var user = AccountController.GetLoggedInUser();
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(deadline))
            {
                ViewBag.Error = "All fields are required.";
                return View();
            }

            if (tasks.Any(t => t.Title == title && t.UserEmail == user.Email))
            {
                ViewBag.Error = "You already have a task with this title.";
                return View();
            }

            if (DateTime.TryParse(deadline, out DateTime parsedDeadline))
            {
                tasks.Add(new TaskModel(title, description, parsedDeadline, user.Email));
                SaveTasks();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Error = "Invalid date format.";
                return View();
            }
        }

        public IActionResult DeleteTask(string title)
        {
            var user = AccountController.GetLoggedInUser();
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int removedCount = tasks.RemoveAll(t => t.Title == title && t.UserEmail == user.Email);

            if (removedCount > 0)
            {
                SaveTasks();
            }

            return RedirectToAction("Index");
        }

        public IActionResult SortByDeadline()
        {
            var user = AccountController.GetLoggedInUser();
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            tasks = tasks.OrderBy(t => t.Deadline).ToList();
            SaveTasks();
            return RedirectToAction("Index");
        }

        public IActionResult ToggleStatus(string title)
        {
            var user = AccountController.GetLoggedInUser();
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var task = tasks.FirstOrDefault(t => t.Title == title && t.UserEmail == user.Email);
            if (task != null)
            {
                task.ToggleStatus();
                SaveTasks();
            }
            return RedirectToAction("Index");
        }

        private void SaveTasks()
        {
            string json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(filePath, json);
        }

        private void LoadTasks()
        {
            if (System.IO.File.Exists(filePath))
            {
                string json = System.IO.File.ReadAllText(filePath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    tasks = new List<TaskModel>();
                }
                else
                {
                    tasks = JsonSerializer.Deserialize<List<TaskModel>>(json) ?? new List<TaskModel>();
                }
            }
            else
            {
                tasks = new List<TaskModel>();
            }
        }
    }
}