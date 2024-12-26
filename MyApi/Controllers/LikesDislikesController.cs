using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace MyApi.Controllers
{
    public class LikesDislikesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("LikesDislikesCount")]
        public async Task<IActionResult> GetLikesDislikesCount([FromServices] IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection"); // שליפת מחרוזת החיבור מבסיס הנתונים

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    await conn.OpenAsync(); // פתיחת חיבור אסינכרוני לבסיס הנתונים

                    // שאילתה לספירת לייקים ודיסלייקים לכל משתמש
                    string query = @"
                SELECT 
                    target_user_id AS UserId, 
                    SUM(CASE WHEN reaction = 'like' THEN 1 ELSE 0 END) AS LikesCount,
                    SUM(CASE WHEN reaction = 'dislike' THEN 1 ELSE 0 END) AS DislikesCount
                FROM likesdislikes
                GROUP BY target_user_id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var results = new List<object>(); // רשימת התוצאות

                            while (await reader.ReadAsync())
                            {
                                results.Add(new
                                {
                                    UserId = reader.GetInt32("UserId"), // מזהה המשתמש
                                    LikesCount = reader.IsDBNull(reader.GetOrdinal("LikesCount")) ? 0 : reader.GetInt32("LikesCount"), // מספר הלייקים
                                    DislikesCount = reader.IsDBNull(reader.GetOrdinal("DislikesCount")) ? 0 : reader.GetInt32("DislikesCount") // מספר הדיסלייקים
                                });
                            }

                            return Ok(results); // החזרת רשימת התוצאות
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // טיפול בשגיאה
                return StatusCode(500, new { Message = "An error occurred", Error = ex.Message });
            }
        }


    }
}
