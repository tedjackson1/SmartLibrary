using LMSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace LMSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IConfiguration _config;

        public DashboardController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            var model = new DashboardModel();

            using var connection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection"));

            connection.Open();

            // Count Students
            using (var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Students",
                connection))
            {
                model.TotalStudents =
                    Convert.ToInt32(cmd.ExecuteScalar());
            }

            // Count Books
            using (var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Books12",
                connection))
            {
                model.TotalBooks =
                    Convert.ToInt32(cmd.ExecuteScalar());
            }

            // Count Publications
            using (var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Publications",
                connection))
            {
                model.TotalPublications =
                    Convert.ToInt32(cmd.ExecuteScalar());
            }

            // Count Librarians
            using (var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Librarians",
                connection))
            {
                model.TotalLibrarians =
                    Convert.ToInt32(cmd.ExecuteScalar());
            }

            return View(model);
        }

    }
}