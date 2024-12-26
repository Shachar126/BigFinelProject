using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc; // הוספת תמיכה ב-ASP.NET Core MVC
using MySql.Data.MySqlClient; // הוספת תמיכה בספריית MySQL

using System.Data;
using MyClasses;


[Route("api/[controller]")]
[ApiController]
public class PersonController : ControllerBase
{
    private readonly IConfiguration _configuration; // תלות בקובץ הקונפיגורציה
    private readonly ILogger<PersonController> _logger; // תלות בלוגים לצורך מעקב ותיעוד

    // בנאי לקבלת התלויות
    public PersonController(IConfiguration configuration, ILogger<PersonController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    // פעולה לבדיקת שם משתמש וסיסמה באמצעות GET
    [HttpGet("ValidateLogin")]
    public IActionResult ValidateLogin([FromQuery] string username, [FromQuery] string password)    
    {
        // שליפת מחרוזת החיבור מתוך קובץ הקונפיגורציה
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        try
        {
            // פתיחת חיבור לבסיס הנתונים
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // שאילתת SQL לבדיקת שם משתמש וסיסמה
                string query = "SELECT COUNT(*) FROM person WHERE username = @Username AND password = @Password";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // הוספת פרמטרים לשאילתה
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    // הרצת השאילתה וקבלת התוצאה
                    int userCount = Convert.ToInt32(cmd.ExecuteScalar());

                    // אם יש התאמה, מחזירים true
                    if (userCount > 0)
                    {
                        return Ok(new { IsValid = true, Message = "Login successful" });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // רישום השגיאה ל-logger והחזרת שגיאה כללית ללקוח
            _logger.LogError(ex, "Error occurred while validating login.");
            return StatusCode(500, new { IsValid = false, Message = "Internal server error" });
        }

        // אם לא נמצאה התאמה, מחזירים false
        return Ok(new { IsValid = false, Message = "Invalid username or password" });
    }


    // פעולה להתחברות משתמש והחזרת נתוני המשתמש
    [HttpGet("Login")]
    public async Task<IActionResult> Login(
        [FromQuery] string username,
        [FromQuery] string password,
        [FromServices] IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection"); // שליפת מחרוזת החיבור מבסיס הנתונים

        try
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync(); // פתיחת חיבור אסינכרוני לבסיס הנתונים

                // שאילתה לשליפת נתוני המשתמש
                string query = @"
                SELECT user_id, username, first_name, last_name, email 
                FROM person 
                WHERE username = @Username AND password = @Password";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username); // הכנסת פרמטר שם משתמש
                    cmd.Parameters.AddWithValue("@Password", password); // הכנסת פרמטר סיסמה

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            // יצירת אובייקט להחזרת נתוני המשתמש
                            var userResponse = new
                            {
                                UserId = reader.GetInt32("user_id"), // שליפת מזהה המשתמש
                                Username = reader.GetString("username"), // שליפת שם המשתמש
                                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString("email"), // שליפת מייל
                                FirstName = reader.IsDBNull(reader.GetOrdinal("first_name")) ? null : reader.GetString("first_name"), // שליפת שם פרטי
                                LastName = reader.IsDBNull(reader.GetOrdinal("last_name")) ? null : reader.GetString("last_name") // שליפת שם משפחה
                            };

                            return Ok(userResponse); // החזרת נתוני המשתמש
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // טיפול בשגיאה
            return StatusCode(500, new { Message = "An error occurred", Error = ex.Message });
        }

        return Unauthorized(new { Message = "Invalid username or password" }); // הרשאות לא תקינות
    }

    [HttpGet("InactiveUsers")]
    public async Task<IActionResult> GetInactiveUsers(
     [FromQuery] int daysInactive,
     [FromServices] IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection"); // שליפת מחרוזת החיבור מבסיס הנתונים

        try
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync(); // פתיחת חיבור לבסיס הנתונים

                // שאילתה למציאת משתמשים שלא שלחו הודעה בזמן התקופה המוגדרת
                string query = @"
                SELECT p.user_id, p.username, p.first_name, p.last_name, p.email, p.created_at
                FROM person p
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM chatdates c
                    WHERE c.sender_id = p.user_id
                    AND c.sent_at > @InactiveSince
                )";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    // הכנסת פרמטר לתאריך התקופה הלא פעילה
                    var inactiveSince = DateTime.Now.AddDays(-daysInactive);
                    cmd.Parameters.AddWithValue("@InactiveSince", inactiveSince);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var inactiveUsers = new List<object>(); // רשימת משתמשים לא פעילים

                        while (await reader.ReadAsync())
                        {
                            // יצירת אובייקט עבור כל משתמש
                            inactiveUsers.Add(new
                            {
                                UserId = reader.GetInt32("user_id"),
                                Username = reader.GetString("username"),
                                FirstName = reader.IsDBNull(reader.GetOrdinal("first_name")) ? null : reader.GetString("first_name"),
                                LastName = reader.IsDBNull(reader.GetOrdinal("last_name")) ? null : reader.GetString("last_name"),
                                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString("email"),
                                CreatedAt = reader.GetDateTime("created_at")
                            });
                        }

                        return Ok(inactiveUsers); // החזרת המשתמשים הלא פעילים
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred", Error = ex.Message }); // טיפול בשגיאה
        }
    }


}