using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TaskPlannerWeb.Models;

namespace TaskPlannerWeb.Controllers
{
    public class AccountController : Controller
    {
        private static List<User> users = new List<User>();
        private static User loggedInUser = null;
        private static string filePath = "users.json"; // Файл для хранения пользователей

        public AccountController()
        {
            LoadUsers(); // Загружаем пользователей при старте приложения
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string email, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "All fields are required.";
                return View();
            }

            if (users.Exists(u => u.Email == email))
            {
                ViewBag.Error = "This email is already registered.";
                return View();
            }

            users.Add(new User(username, email, password));
            SaveUsers(); // Сохраняем в файл
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            LoadUsers(); // Загружаем пользователей перед проверкой

            var user = users.Find(u => u.Email == email);
            if (user != null && user.CheckPassword(password))
            {
                loggedInUser = user;
                return RedirectToAction("Index", "Task");
            }

            ViewBag.Error = "Invalid email or password.";
            return View();
        }

        public IActionResult Logout()
        {
            loggedInUser = null;
            return RedirectToAction("Login");
        }

        public static User GetLoggedInUser()
        {
            return loggedInUser;
        }

        private void SaveUsers()
        {
            string json = JsonSerializer.Serialize(users);
            System.IO.File.WriteAllText(filePath, json);
        }

        private void LoadUsers()
        {
            if (System.IO.File.Exists(filePath))
            {
                string json = System.IO.File.ReadAllText(filePath);
                users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
            }
        }
    }
}