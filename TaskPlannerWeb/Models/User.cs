using System;

namespace TaskPlannerWeb.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        // ✅ Пустой конструктор для JSON-десериализации
        public User() { }

        public User(string username, string email, string password)
        {
            Username = username;
            Email = email;
            PasswordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }

        public bool CheckPassword(string password)
        {
            return PasswordHash == Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}