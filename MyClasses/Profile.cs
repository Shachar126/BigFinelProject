using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClasses
{
    public class Profile
    {
        public int profile_id { get; set; } // Primary Key (convention-based)
        public int user_id { get; set; }    // Foreign Key linking to Users table
        public int game_id { get; set; }    // Foreign Key linking to Games table
        public string profile_description { get; set; } // Description of the profile
        public DateTime created_at { get; set; } = DateTime.Now; // Timestamp

        // Constructor to initialize a new Profile
        public Profile(int userId, int gameId, string profileDescription)
        {
            user_id = userId;
            game_id = gameId;
            profile_description = profileDescription;
            created_at = DateTime.Now;
        }

    }
}