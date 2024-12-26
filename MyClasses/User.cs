using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClasses
{
    public class User : Person
    {

        public string bio { get; set; }

        public string profile_picture_url { get; set; }

        public User(string bio, string profile_picture_url,int user_id, string username, string password, DateTime created_at, string first_name, string last_name, string email) : base(user_id, username, password, created_at, first_name, last_name, email)
        {

            this.bio = bio;
            this.profile_picture_url = profile_picture_url;
        }
    }
}