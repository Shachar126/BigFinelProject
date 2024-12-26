using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace MyApi.Controllers
{
    public class MatchesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("MatchesByHour")]
        public async Task<IActionResult> GetMatchesByHour([FromServices] IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection"); // שליפת מחרוזת החיבור מבסיס הנתונים

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    await conn.OpenAsync(); // פתיחת חיבור לבסיס הנתונים

                    // שאילתה לספירת התאמות לפי שעה ביום
                    string query = @"
                SELECT 
                    HOUR(matched_at) AS MatchHour, 
                    COUNT(*) AS MatchCount
                FROM matches
                GROUP BY HOUR(matched_at)
                ORDER BY MatchHour";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var results = new List<object>(); // רשימת התוצאות

                            while (await reader.ReadAsync())
                            {
                                results.Add(new
                                {
                                    MatchHour = reader.IsDBNull(reader.GetOrdinal("MatchHour")) ? 0 : reader.GetInt32("MatchHour"), // השעה שבה נוצרה ההתאמה
                                    MatchCount = reader.IsDBNull(reader.GetOrdinal("MatchCount")) ? 0 : reader.GetInt32("MatchCount") // מספר ההתאמות
                                });
                            }

                            return Ok(results); // החזרת התוצאה
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
