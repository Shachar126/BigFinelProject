using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClasses
{
    public class Games
    {
        public int game_id { get; set; }
        public string game_name { get; set; }
        public string genre { get; set; }
        public string description { get; set; }
        public double rating { get; set; }
        public string image_url { get; set; }
        public DateTime Created_at { get; set; } = DateTime.Now;

        public Games(int game_id, string game_name, string genre, string description, double rating, string image_url, DateTime created_at)
        {
            this.game_id = game_id;
            this.game_name = game_name;
            this.genre = genre;
            this.description = description;
            this.rating = rating;
            this.image_url = image_url;
            Created_at = created_at;
        }
    }
}