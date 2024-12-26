using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClasses
{
    public class LikesDislikes
    {
        public int like_dislike_id { get; set; }  // מזהה ייחודי לכל פעולה
        public int user_id { get; set; }          // המשתמש שמבצע את הפעולה
        public int target_user_id { get; set; }   // המשתמש שמקבל את הפעולה (לייק/דיסלייק)
        public string reaction { get; set; }      // 'like' או 'dislike'
        public DateTime reacted_at { get; set; } = DateTime.Now; // זמן ביצוע הפעולה
        public LikesDislikes(int like_dislike_id, int user_id, int target_user_id, string reaction, DateTime reacted_at)
        {
            like_dislike_id = like_dislike_id;
            user_id = user_id;
            target_user_id = target_user_id;
            this.reaction = reaction;
            reacted_at = DateTime.Now;
        }

    }

}