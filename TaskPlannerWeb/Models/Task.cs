using System;

namespace TaskPlannerWeb.Models
{
    public class Task
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; }
        public string UserEmail { get; set; } // ✅ Привязка к пользователю

        public Task() { }

        public Task(string title, string description, DateTime deadline, string userEmail, string status = "In Progress")
        {
            Title = title;
            Description = description;
            Deadline = deadline;
            UserEmail = userEmail;
            Status = status;
        }

        public void ToggleStatus()
        {
            Status = (Status == "In Progress") ? "Done" : "In Progress";
        }
    }
}